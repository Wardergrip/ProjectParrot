using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction
{
	public enum Action
	{
		OnPlayerMove,
		OnPlayerAim,
		OnPlayerShoot
	}
	public InputAction.CallbackContext Input { get; set; }
	public float SecondsPassedSinceAction { get; set; }
	public Action ActionType { get; set; }
	public Vector2 VectorValue { get; set; }
}

public enum ReplayState
{
	Idle,
	Recording,
	Playing
}

public class InputBasedReplaySystem : Singleton<InputBasedReplaySystem>
{
	public ReplayState ReplayState { get; private set; }

	private readonly List<PlayerAction> _registeredPlayerInputs = new();
	public List<PlayerAction> RegisteredPlayerInputs { get => _registeredPlayerInputs; }
	private float _startTime = 0;
	private float _lastTime = 0;

	public void StartRecord()
	{
		if (ReplayState != ReplayState.Idle)
		{
			return;
		}
		Debug.Log($"Started recording");
		_registeredPlayerInputs.Clear();
		_startTime = Time.realtimeSinceStartup;
		_lastTime = _startTime;
		ReplayState = ReplayState.Recording;
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			Play();
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			StartRecord();
		}
	}

	public void RegisterAction(PlayerAction action)
	{
		if (ReplayState != ReplayState.Recording)
		{
			return;
		}
		action.SecondsPassedSinceAction = Time.realtimeSinceStartup - _lastTime;
		_registeredPlayerInputs.Add(action);
		_lastTime = Time.realtimeSinceStartup;
	}

	public void RegisterAction(ref InputAction.CallbackContext context, PlayerAction.Action action)
	{
		RegisterAction(new PlayerAction()
		{
			Input = context,
			ActionType = action,
			VectorValue = context.valueType == typeof(Vector2) ? context.ReadValue<Vector2>() : Vector2.zero
		});
	}

	public void Play()
	{
		if (ReplayState == ReplayState.Playing)
		{
			return;
		}
		Debug.Log($"Registered inputs: {_registeredPlayerInputs.Count}");
		ReplayState = ReplayState.Playing;
		StartCoroutine(Simulate());
	}

	private IEnumerator Simulate()
	{
		for (int i = 0; i < _registeredPlayerInputs.Count; ++i)
		{
			yield return new WaitForSeconds(_registeredPlayerInputs[i].SecondsPassedSinceAction);
			SimulateAction(_registeredPlayerInputs[i]);
		}
		Debug.Log($"End of simulation");
		ReplayState = ReplayState.Idle;
	}

	private void SimulateAction(PlayerAction playerAction)
	{
		Debug.Log($"Simulating action...");
		Player player = Player.Instance;
		switch (playerAction.ActionType)
		{
			case PlayerAction.Action.OnPlayerMove:
				player.Move(playerAction.VectorValue);
				break;
			case PlayerAction.Action.OnPlayerAim:
				player.Aim(playerAction.VectorValue);
				break;
			case PlayerAction.Action.OnPlayerShoot:
				player.Shoot();
				break;
			default:
				Debug.LogError($"Action not implemented: {playerAction.ActionType}");
				break;
		}
	}
}