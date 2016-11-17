using UnityEngine;

public class Projectile : MonoBehaviour, IProjectile
{
	protected GameObject _myGameObject;

	protected Transform _myTransform;

	#region IProjectile Properties
	public float MaxLiveTime { get; set; }
	public float ProjectileSpeed { get; set; }
	public float Damage { get; set; }

	public bool AmmoUsesGravity { get; set; }

	public GameObject Owner { get; set; }

	public Transform AmmoSpawnPoint { get; set; }
	#endregion

	#region IProjectile Methods
	public virtual void Init()
	{
		_myGameObject = gameObject;
		_myTransform = gameObject.transform;

		if (_myGameObject.rigidbody != null)
		{
			_myGameObject.rigidbody.useGravity = AmmoUsesGravity;
		}
	}

	public virtual void DoCollision(GameObject target)
	{
		DoDamage(target);

		Destroy(_myGameObject);
	}

	public virtual void DoDamage(GameObject target)
	{
		DamageController damageController = target.GetComponent<DamageController>();

		float damage = Damage;

		ForceField forceField = target.GetComponent<ForceField>();
		if (forceField != null)
		{
			damage = forceField.AbsorbDamage(damage);
		}

		damageController.ApplyDamage(Owner, damage);
	}
	#endregion
}