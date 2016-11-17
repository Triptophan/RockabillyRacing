using UnityEngine;
using System.Collections;

public class OozeGlobby : Projectile, IGroundEffect
{
	private const bool AMMO_USES_GRAVITY = false;

	private const float MAX_LIVE_TIME = 1f;
	private const float PROJECTILE_SPEED = 35f;
	private const float DAMAGE = 5f;
	private const float EFFECT_OFFSET = 0.5f;

	#region IGroundEffect Properties
	public GameObject GroundEffectPrefab { get; set; }
	#endregion

	#region IProjectile Methods
	public override void Init()
	{
		MaxLiveTime = MAX_LIVE_TIME;
		ProjectileSpeed = PROJECTILE_SPEED;
		Damage = DAMAGE;

		AmmoUsesGravity = AMMO_USES_GRAVITY;

		GroundEffectPrefab = ResourcesHelper.OozeSplash;

		base.Init();
	}

	public override void DoCollision(GameObject target)
	{
		if (target == _myGameObject) return;

		Collider collider = target.GetComponentInChildren<BoxCollider>();

		if (collider == null)
		{
			Debug.LogError(target.name + " does not have a collider attached or one cannot be found.");
			return;
		}

		ShowGroundEffect(collider);

		base.DoCollision(target);
	}
	#endregion

	#region IGroundEffect Methods
	public void ShowGroundEffect(Collider other)
	{
		GameObject splash = (GameObject)Instantiate(GroundEffectPrefab,
					other.transform.position + Vector3.up * EFFECT_OFFSET,
					GroundEffectPrefab.transform.rotation);

		if (splash != null)
		{
			OozeSplash splashComponent = splash.GetComponent<OozeSplash>();

			if (splashComponent != null)
			{
				splashComponent.owner = Owner;
			}
		}

		GameObject.Destroy(gameObject);
	}
	#endregion
}