using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static bool _isApplicationExiting = false;
	private static T _instance;

	/// <summary>
	/// Returns null if application is exiting. Perform null checks if you access Instance in OnDestroy
	/// </summary>
	public static T Instance
	{
		get
		{
			if (_isApplicationExiting)
			{
				return null;
			}

			if (_instance == null)
			{
				_instance = FindObjectOfType<T>();
				if (_instance == null)
				{
					GameObject obj = new($"[S] {typeof(T)}");
					_instance = obj.AddComponent<T>();
				}
			}
			return _instance;
		}
	}

	protected virtual void Awake()
	{
		if (_instance != null)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this as T;
		if (transform.parent == null)
		{
			DontDestroyOnLoad(gameObject);
		}
		LateAwake();
	}

	protected virtual void LateAwake() { }

	private void OnApplicationQuit()
	{
		_isApplicationExiting = true;
	}
}