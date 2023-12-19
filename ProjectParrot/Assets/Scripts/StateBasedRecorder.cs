using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public struct TransformData
{
	public Vector3 Position;
	public Quaternion Rotation;
	public Vector3 Scale;
}

public class StateBasedRecorder : MonoBehaviour
{
	private static int s_idCounter = 0;

	public int Id { get; private set; }
	[SerializeField] private string _thisPrefab;
	public GameObject ThisPrefab { get => Resources.Load(_thisPrefab) as GameObject; }
	private Coroutine _lerpCoroutine;
	private TransformData _stateToLerpTo;
	private bool _isStateToLerpToUpdated = false;

	public bool ForceId(int id)
	{
		if (StateBasedReplaySystem.Instance.StateBasedRecorders.Any((StateBasedRecorder sbr) => sbr.Id == id))
		{
			return false;
		}
		Id = id;
		--s_idCounter;
		return true;
	}

	private void Awake()
	{
		Id = s_idCounter++;
		Debug.Assert(ThisPrefab.GetComponentOrSearchInParentAndChilderen<StateBasedRecorder>(), $"{ThisPrefab} is not a valid StateBasedRecorder prefab. Make sure it contains a StateBasedRecorder component");
	}

	private void OnEnable()
	{
		StateBasedReplaySystem.Instance.StateBasedRecorders.Add(this);
	}

	private void OnDisable()
	{
		var replayInst = StateBasedReplaySystem.Instance;
		if (replayInst) replayInst.StateBasedRecorders.Remove(this);
	}

	public TransformData RecordState()
	{
		return new TransformData()
		{
			Position = transform.position,
			Rotation = transform.rotation,
			Scale = transform.localScale
		};
	}

	public void LoadState(TransformData transformData)
	{
		if (StateBasedReplaySystem.s_ShouldLerp)
		{
			_stateToLerpTo = transformData;
			_isStateToLerpToUpdated = true;
			if (_lerpCoroutine == null)
			{
				_lerpCoroutine = StartCoroutine(LerpToState());
			}
			return;
		}
		transform.SetLocalPositionAndRotation(transformData.Position, transformData.Rotation);
		transform.localScale = transformData.Scale;
	}

	private IEnumerator LerpToState()
	{
		bool atState = false;
		TransformData transformData = _stateToLerpTo;
		float amountOfTimeToLerp = StateBasedReplaySystem.s_WaitTime;
		float ratioHelper = 1.0f / StateBasedReplaySystem.s_WaitTime;
		float t = 0.0f;
		TransformData originalTransformData = new()
		{
			Position = transform.position,
			Rotation = transform.rotation,
			Scale = transform.localScale
		};
		while (!atState || t < 1.0f)
		{
			if (_isStateToLerpToUpdated)
			{
				originalTransformData = transformData;
				transformData = _stateToLerpTo;
				amountOfTimeToLerp = StateBasedReplaySystem.s_WaitTime;
				_isStateToLerpToUpdated = false;
			}
			else 
			{ 
				atState = 
					(transform.position == _stateToLerpTo.Position &&
					transform.rotation == _stateToLerpTo.Rotation &&
					transform.localScale == _stateToLerpTo.Scale);
			}

			t = 1.0f - (amountOfTimeToLerp * ratioHelper);
			Debug.Log($"t:{t}");
			transform.SetLocalPositionAndRotation(
				Vector3.Lerp(originalTransformData.Position, transformData.Position, t),
				Quaternion.Slerp(originalTransformData.Rotation, transformData.Rotation,t)
				);
			transform.localScale = Vector3.Lerp(originalTransformData.Scale, transformData.Scale, t);

			amountOfTimeToLerp -= Time.deltaTime;
			yield return null;
		}
		transform.SetLocalPositionAndRotation(_stateToLerpTo.Position, _stateToLerpTo.Rotation);
		transform.localScale = _stateToLerpTo.Scale;
		_lerpCoroutine = null;
	}
}
