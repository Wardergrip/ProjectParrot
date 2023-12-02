using UnityEngine;

public class ReplaySystem : MonoBehaviour
{
	private IReplaySystem _replaySystem = InputBasedReplaySystem.Instance;

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
			// Switch
		}

	}
}
