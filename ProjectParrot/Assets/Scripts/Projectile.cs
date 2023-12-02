using UnityEngine;

public interface IProjectileHittable
{
	bool HitBy(Projectile projectile);
}

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 10f;
	[SerializeField] private float _destroyTimer = 4f;

	private void Update()
	{
		transform.position += _movementSpeed * Time.deltaTime * transform.forward;
		_destroyTimer -= Time.deltaTime;
		if ( _destroyTimer <= 0 )
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.TryGetComponent(out IProjectileHittable hittable)) return;
		if (hittable.HitBy(this))
		{
			Destroy(gameObject);
		}
	}
}
