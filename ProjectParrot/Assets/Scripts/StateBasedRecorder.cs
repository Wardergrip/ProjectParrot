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
	[SerializeField] private GameObject _thisPrefab;
	public GameObject ThisPrefab { get => _thisPrefab; }

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
		transform.SetLocalPositionAndRotation(transformData.Position, transformData.Rotation);
		transform.localScale = transformData.Scale;
	}
}
