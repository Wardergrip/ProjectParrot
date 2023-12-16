using UnityEngine;

public enum ReplaySystemType
{
	Input,
	State
}

public class ReplaySystem : MonoBehaviour
{
	private IReplaySystem _replaySystem;
	private ReplaySystemType _type = ReplaySystemType.Input;

	private void Awake()
	{
		_replaySystem = InputBasedReplaySystem.Instance;
	}

	private void Update()
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
