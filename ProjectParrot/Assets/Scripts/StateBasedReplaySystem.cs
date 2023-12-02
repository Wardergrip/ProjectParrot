public interface IStateBasedSavable
{
	void SaveState();
	void LoadState();
}

// How store this in container? Everything needs different data?

public class StateBasedReplaySystem : Singleton<StateBasedReplaySystem>, IReplaySystem
{
	public ReplayState ReplayState { get; private set; }

	public bool IsRecording => ReplayState == ReplayState.Recording;

	public void PlayRecording()
	{
		if (ReplayState != ReplayState.Idle && ReplayState != ReplayState.DoneRecording)
		{
			return;
		}
	}

	public void StartRecording()
	{
		if (ReplayState == ReplayState.Playing)
		{
			return;
		}
	}

	public void StopRecording()
	{
		if (ReplayState != ReplayState.Recording)
		{
			return;
		}
	}
}
