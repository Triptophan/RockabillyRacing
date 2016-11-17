using UnityEngine;

public class SingleMine : MonoBehaviour
{
	private GameObject _myGameObject;
	private GameObject _explosion;

	private Transform _myTransform;
	private Transform _childTransform;

	public GameObject Owner { get; set; }

	public float Damage { get; set; }

	void Awake()
	{
		_myGameObject = gameObject;

		_explosion = ResourcesHelper.ExplosionEffect;

		_myTransform = _myGameObject.transform;

		if (_myTransform.childCount > 0)
		{
			_childTransform = _myTransform.GetChild(0);

			if (_childTransform != null)
			{
				_childTransform.rotation = Random.rotation;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		Transform target = other.transform.root;

		DamageController damageController = target.GetComponent<DamageController>();

		if (damageController != null)
		{
			float damage = Damage;

			ForceField forceField = target.GetComponent<ForceField>();

			if (forceField != null)
			{
				damage = forceField.AbsorbDamage(damage);
			}

			damageController.ApplyDamage(Owner, damage);

			if (_explosion != null)
			{
				GameObject explosionObject = (GameObject)GameObject.Instantiate(_explosion,
																	_myTransform.position,
																	_myTransform.rotation);

				if (explosionObject != null)
				{
					ParticleSystem explosionEffect = explosionObject.GetComponent<ParticleSystem>();

					if (explosionEffect != null)
					{
						Destroy(explosionEffect, explosionEffect.duration);
						Destroy(explosionObject, explosionEffect.duration);
					}
					else
					{
						Destroy(explosionObject);
					}
				}
			}

			Destroy(_myGameObject);
		}
	}
}