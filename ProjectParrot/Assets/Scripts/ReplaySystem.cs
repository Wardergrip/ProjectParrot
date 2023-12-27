using System.Collections.Generic;
using UnityEngine;

public enum ReplaySystemType
{
	Input = 0,
	State = 1
}

public class ReplaySystem : MonoBehaviour
{
	private readonly List<IReplaySystem> _replaySystems = new();
	private IReplaySystem _replaySystem;
	private ReplaySystemType _type = ReplaySystemType.Input;

	private void Awake()
	{
		_replaySystem = InputBasedReplaySystem.Instance;
		_replaySystems.Add(InputBasedReplaySystem.Instance);
		_replaySystems.Add(StateBasedReplaySystem.Instance);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			_replaySystems[(int)_type].PlayRecording();
			string type = _type == ReplaySystemType.Input ? "Input" : "State";
			Debug.Log($"Now playing {type} based replay");
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			if (_replaySystems[0].IsRecording)
			{
				_replaySystems.ForEach(x => x.StopRecording());
			}
			else
			{
				_replaySystems.ForEach(x => x.StartRecording());
			}
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			switch (_type)
			{
				case ReplaySystemType.Input:
					_type = ReplaySystemType.State;
					Debug.Log($"Current replay system selected is now StateBased");
					break;
				case ReplaySystemType.State:
					_type = ReplaySystemType.Input;
					Debug.Log($"Current replay system selected is now InputBased");
					break;
			}
		}
	}

	private void SwitchReplaySystemCode()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			_replaySystem.PlayRecording();
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			if (_replaySystem.IsRecording)
			{
				_replaySystem.StopRecording();
			}
			else
			{
				_replaySystem.StartRecording();
			}
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			switch (_type)
			{
				case ReplaySystemType.Input:
					_replaySystem = StateBasedReplaySystem.Instance;
					_type = ReplaySystemType.State;
					Debug.Log($"Current replay system is now StateBased");
					break;
				case ReplaySystemType.State:
					_replaySystem = InputBasedReplaySystem.Instance;
					_type = ReplaySystemType.Input;
					Debug.Log($"Current replay system is now InputBased");
					break;
			}
		}
	}
}
