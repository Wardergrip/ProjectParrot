using System.Collections;
using UnityEngine;

public class ReplayControlPanel : MonoBehaviour
{
    [SerializeField][Range(float.Epsilon,1.0f)] private float _stateInterval = 0.2f;

	[SerializeField] private int _actionsPerSecond = 1;
	[SerializeField] private bool _performRandomActions = false;

	private Coroutine _randomActionsCoroutine;

	private void Update()
	{
		if (!StateBasedReplaySystem.Instance.IsRecording)
		{
			StateBasedReplaySystem.s_WaitTime = _stateInterval;
		}
		else
		{
			_stateInterval = StateBasedReplaySystem.s_WaitTime;
		}

		if (_performRandomActions) 
		{
			_randomActionsCoroutine ??= StartCoroutine(PerformRandomActionsCoroutine());
		}
		else
		{
			if (_randomActionsCoroutine != null)
			{
				StopCoroutine(_randomActionsCoroutine);
				_randomActionsCoroutine = null;
			}
		}
	}

	private IEnumerator PerformRandomActionsCoroutine()
	{
		while (true)
		{
			for (int i = 0; i < _actionsPerSecond; ++i)
			{
				Vector2 randomVector = new(Random.Range(-1, 1), Random.Range(-1, 1));
				randomVector.Normalize();
				InputBasedReplaySystem.Instance.RegisterAction(
					new PlayerAction()
					{
						ActionType = (PlayerAction.Action)Random.Range(0, (int)PlayerAction.Action.OnPlayerShoot),
						VectorValue = randomVector
					}
					);
			}
			yield return new WaitForSeconds(1.0f);
		}
	}
}
