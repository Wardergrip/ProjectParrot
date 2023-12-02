using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private Transform _projectileSocket;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _movementSpeed = 4f;

    private Vector3 _initialPos;
	private Vector2 _movementInput;
    private Vector2 _aimInput;

	private void Awake()
	{
		Debug.Assert(Instance == null, $"Simple global instance with multiple instances trying. Update to singleton or other system.");
        Instance = this;
		_initialPos = transform.position;
	}

	public void OnMove(InputAction.CallbackContext ctx)
    {
		Move(ctx.ReadValue<Vector2>());
        InputBasedReplaySystem.Instance.RegisterAction(ref ctx, PlayerAction.Action.OnPlayerMove);
    }
    public void Move(Vector2 input)
    {
        _movementInput = input;
    }
    public void OnAim(InputAction.CallbackContext ctx)
    {
        Aim(ctx.ReadValue<Vector2>());
        InputBasedReplaySystem.Instance.RegisterAction(ref ctx, PlayerAction.Action.OnPlayerAim);
    }
    public void Aim(Vector2 input)
    {
        _aimInput = input;
    }
    public void OnShoot(InputAction.CallbackContext ctx) 
    {
        if (!ctx.performed) return;
        Shoot();
        InputBasedReplaySystem.Instance.RegisterAction(ref ctx, PlayerAction.Action.OnPlayerShoot);
    }
    public void Shoot()
    {
        Instantiate(_projectilePrefab, _projectileSocket.position, _projectileSocket.rotation);
    }

	private void Update()
	{
        transform.position += _movementSpeed * Time.deltaTime * new Vector3(_movementInput.x, 0, _movementInput.y);

        if (_aimInput.sqrMagnitude > 0.0f)
        {
            float targetAngle = Mathf.Atan2(_aimInput.x, _aimInput.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = targetRotation;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = _initialPos;
        }
	}
}
