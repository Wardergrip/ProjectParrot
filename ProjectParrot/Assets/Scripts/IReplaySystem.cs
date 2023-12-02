public interface IReplaySystem
{
	public bool IsRecording { get; }

	public void StartRecording();
	public void StopRecording();
	public void PlayRecording();
}
