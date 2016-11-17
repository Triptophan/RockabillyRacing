using UnityEngine;

public class MachineGunBullet : Projectile
{
	private const float MAX_LIVE_TIME = 5f;
	private const float PROJECTILE_SPEED = 150f;
	private const float DAMAGE = 20f;

	private const bool AMMO_USES_GRAVITY = false;

	public override void Init()
	{
		MaxLiveTime = MAX_LIVE_TIME;
		ProjectileSpeed = PROJECTILE_SPEED;
		Damage = DAMAGE;

		AmmoUsesGravity = AMMO_USES_GRAVITY;

		base.Init();
	}

	public override void DoCollision(GameObject target)
	{
		base.DoCollision(target);
	}
}