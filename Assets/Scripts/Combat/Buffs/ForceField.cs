using UnityEngine;

public class ForceField : Projectile, IBuffEffect, IDamageAbsorber
{
	private const bool AMMO_USES_GRAVITY = false;

	private const float MAX_LIVE_TIME = 5f;
	private const float PROJECTILE_SPEED = 0f;
	private const float DAMAGE_ABSORPTION = 25f;
	private const float DAMAGE = 0f;
	private const float EFFECT_HORIZONTAL_OFFSET = 1.5f;

	private float _DamageLeft;

	#region IProjectile Methods
	public override void Init()
	{
		MaxLiveTime = MAX_LIVE_TIME;
		ProjectileSpeed = PROJECTILE_SPEED;
		Damage = DAMAGE;

		AmmoUsesGravity = AMMO_USES_GRAVITY;

		base.Init();
	}
	#endregion

	#region IBuffEffect Methods
	public void ApplyBuff()
	{
		_myTransform.parent = Owner.transform;
		_myTransform.localPosition += new Vector3(0f, 0f, EFFECT_HORIZONTAL_OFFSET);
		_myTransform.localRotation = Quaternion.Euler(Vector3.zero);
		_DamageLeft = DAMAGE_ABSORPTION;
	}
	#endregion

	#region IDamageAbsorber Methods
	public float AbsorbDamage(float damage)
	{
		float unabsorbedDamage = 0f;

		_DamageLeft -= damage;

		if (_DamageLeft < 0f)
		{
			unabsorbedDamage = _DamageLeft;
			_DamageLeft = 0f;
		}

		return unabsorbedDamage;
	}
	#endregion
}