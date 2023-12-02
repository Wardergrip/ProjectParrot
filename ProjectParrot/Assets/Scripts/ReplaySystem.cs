using UnityEngine;

public enum ReplaySystemType
{
	Input,
	State
}

public class ReplaySystem : MonoBehaviour
{
	private IReplaySystem _replaySystem = InputBasedReplaySystem.Instance;
	private ReplaySystemType _type = ReplaySystemType.Input;

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
					break;
				case ReplaySystemType.State:
					_replaySystem = InputBasedReplaySystem.Instance;
					break;
			}
		}

	}
}
