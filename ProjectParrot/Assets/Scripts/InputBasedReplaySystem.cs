using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
	public float SecondsPassedSinceAction { get; set; }
	public Action ActionType { get; set; }
	public Vector2 VectorValue { get; set; }
	public int Size()
	{
		int floatSize = sizeof(float);
		int actionSize = sizeof(Action);
		int vector2Size = Marshal.SizeOf(typeof(Vector2));

		return 
			floatSize + 
			actionSize + 
			vector2Size;
	}
}

public enum ReplayState
{
	Idle,
	Recording,
	DoneRecording,
	Playing
}

public class InputBasedReplaySystem : Singleton<InputBasedReplaySystem>, IReplaySystem
{
	public ReplayState ReplayState { get; private set; }

	private readonly List<PlayerAction> _registeredPlayerInputs = new();
	public List<PlayerAction> RegisteredPlayerInputs { get => _registeredPlayerInputs; }
	public int TotalSavedSize => RegisteredPlayerInputs.Count * _registeredPlayerInputs[0].Size();

	public bool IsRecording => ReplayState == ReplayState.Recording;

	private float _startTime = 0;
	private float _lastTime = 0;

	public void StartRecording()
	{
		if (ReplayState != ReplayState.Idle && ReplayState != ReplayState.DoneRecording)
		{
			return;
		}
		Debug.Log($"[INPUT] Started recording");
		_registeredPlayerInputs.Clear();
		_startTime = Time.realtimeSinceStartup;
		_lastTime = _startTime;
		ReplayState = ReplayState.Recording;
	}
	public void StopRecording()
	{
		if (ReplayState != ReplayState.Recording)
		{
			return;
		}
		Debug.Log($"[INPUT] Stopped recording. Size: {Utils.FormatByteCount(TotalSavedSize)} bytes");
		ReplayState = ReplayState.DoneRecording;
	}
	public void PlayRecording()
	{
		if (ReplayState == ReplayState.Playing)
		{
			return;
		}
		ReplayState = ReplayState.Playing;
		StartCoroutine(Simulate());
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
			ActionType = action,
			VectorValue = context.valueType == typeof(Vector2) ? context.ReadValue<Vector2>() : Vector2.zero
		});
	}

	private IEnumerator Simulate()
	{
		for (int i = 0; i < _registeredPlayerInputs.Count; ++i)
		{
			yield return new WaitForSeconds(_registeredPlayerInputs[i].SecondsPassedSinceAction);
			SimulateAction(_registeredPlayerInputs[i]);
		}
		Debug.Log($"[INPUT] End of simulation");
		ReplayState = ReplayState.Idle;
	}

	private void SimulateAction(PlayerAction playerAction)
	{
		Debug.Log($"[INPUT] Simulating action...");
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
				Debug.LogError($"[INPUT] Action not implemented: {playerAction.ActionType}");
				break;
		}
	}
}