using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateBasedReplaySystem : Singleton<StateBasedReplaySystem>, IReplaySystem
{
	private class DataStamp<T>
	{
		public DataStamp(T val, float time)
		{
			Value = val;
			TimeStamp = time;
		}

		public T Value { get; private set; }
		public float TimeStamp { get; private set; }
	}
	private class EntityRecording
	{
		public int Id { get; set; }
		public GameObject Prefab { get; set; }
		public List<DataStamp<TransformData>> Data { get; private set; } = new();
		public int Size(bool includeIdAndPtr = true)
		{
			int intSize = sizeof(int);
			int gameObjectPtrSize = 8; // GameObject pointer in 64bit
			int transformDataSize = TransformData.Size();
			int floatSize = sizeof(float);

			return
				(includeIdAndPtr ? intSize : 0) +
				(includeIdAndPtr ? gameObjectPtrSize : 0) +
				(transformDataSize + floatSize /*timestamp*/ ) * Data.Count;
		}
	}

	public ReplayState ReplayState { get; private set; }

	public bool IsRecording => ReplayState == ReplayState.Recording;

	public static float s_WaitTime { get; set; } = 0.2f;
	public static bool s_ShouldLerp { get => true; }

	public List<StateBasedRecorder> StateBasedRecorders { get; private set; } = new();
	private readonly List<EntityRecording> _entityRecordings = new();
	public int TotalSavedSize => _entityRecordings.Count * _entityRecordings[0].Size(false);

	public void PlayRecording()
	{
		if (!(ReplayState == ReplayState.Idle || ReplayState == ReplayState.DoneRecording))
		{
			return;
		}
		ReplayState = ReplayState.Playing;
		StartCoroutine(PlaybackCoroutine());
	}

	public void StartRecording()
	{
		if (ReplayState == ReplayState.Playing)
		{
			return;
		}
		Debug.Log($"[STATE] Started recording");
		_entityRecordings.Clear();
		ReplayState = ReplayState.Recording;
		StartCoroutine(RecordingCoroutine());
	}

	public void StopRecording()
	{
		if (ReplayState != ReplayState.Recording)
		{
			return;
		}
		int sum = _entityRecordings.Sum(x => x.Data.Count);
		Debug.Log($"[STATE] Stopped recording. Size: {Utils.FormatByteCount(TotalSavedSize)} (Entities: {_entityRecordings.Count}, amount of state total: {sum})");
		ReplayState = ReplayState.DoneRecording;
	}

	private IEnumerator RecordingCoroutine()
	{
		while (ReplayState == ReplayState.Recording) 
		{
			yield return new WaitForSeconds(s_WaitTime);
			foreach (StateBasedRecorder rec in StateBasedRecorders)
			{
				EntityRecording thisEntityRecording = _entityRecordings.Find((EntityRecording entityRec) => entityRec.Id == rec.Id);
				if (thisEntityRecording == null)
				{
					thisEntityRecording = new()
					{
						Id = rec.Id,
						Prefab = rec.ThisPrefab
					};
					_entityRecordings.Add(thisEntityRecording);
				}
				thisEntityRecording.Data.Add(new(rec.RecordState(), Time.realtimeSinceStartup));
			}
		}
	}

	private IEnumerator PlaybackCoroutine()
	{
		Debug.Log($"[STATE] Start of playback");
		float latestTimeStamp = 0.0f;
		float earliestTimeStamp = float.MaxValue;
		foreach (var entityRecordings in _entityRecordings)
		{
			foreach (var data in entityRecordings.Data)
			{
				latestTimeStamp = Mathf.Max(latestTimeStamp, data.TimeStamp);
				earliestTimeStamp = Mathf.Min(earliestTimeStamp, data.TimeStamp);
			}
		}
		int ticks = (int)((latestTimeStamp - earliestTimeStamp) / s_WaitTime);
		float currentTime = earliestTimeStamp;
		List<Tuple<StateBasedRecorder,TransformData>> actionList = new();
		while (ticks-- > 0)
		{
			// Find all actions on this timestamp.
			foreach (EntityRecording entityRecording in _entityRecordings) 
			{
				foreach (var data in entityRecording.Data) 
				{
					if ((data.TimeStamp - currentTime) > float.Epsilon)
					{
						continue;
					}
					// Find an object that corresponds with the recorded ID
					StateBasedRecorder sbr = StateBasedRecorders.Find((StateBasedRecorder sbr) => sbr.Id == entityRecording.Id);
					if (sbr == null)
					{
						// Make the object, it probably got deleted.
						sbr = Instantiate(entityRecording.Prefab)
							.GetComponentOrSearchInParentAndChilderen<StateBasedRecorder>();
						Debug.Assert(sbr, $"{entityRecording.Prefab.name} is not a valid StateBaseRecorder prefab");
						sbr.ForceId(entityRecording.Id);
						TransformData transformData = data.Value;
						sbr.transform.SetLocalPositionAndRotation(transformData.Position, transformData.Rotation);
						sbr.transform.localScale = transformData.Scale;
					}
					actionList.Add(new (sbr,data.Value));
				}
			}
			Debug.Log($"[STATE] LoadingState of batch...");
			foreach ((StateBasedRecorder sbr, TransformData data) in actionList)
			{
				sbr.LoadState(data);
			}
			yield return new WaitForSeconds(s_WaitTime);
			currentTime += s_WaitTime;
		}
		ReplayState = ReplayState.Idle;
		Debug.Log($"End of playback");
	}
}
