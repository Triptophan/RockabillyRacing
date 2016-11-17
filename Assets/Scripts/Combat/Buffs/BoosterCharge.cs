
public class BoosterCharge : Projectile, IBuffEffect
{
	private const bool AMMO_USES_GRAVITY = false;

	private const float MAX_LIVE_TIME = 1f;
	private const float PROJECTILE_SPEED = 0f;
	private const float BOOST_MULTIPLIER = 2f;
	private const float DAMAGE = 0f;

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
		VehicleMovement movement = Owner.GetComponent<VehicleMovement>();

		if (movement != null)
		{
			movement.StartBoost(BOOST_MULTIPLIER, MAX_LIVE_TIME);
		}
	}
	#endregion
}