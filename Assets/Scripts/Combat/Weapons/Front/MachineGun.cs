
public class MachineGun : BurstWeapon
{
	private const float RATE_OF_FIRE = 0.1f;
	private const float BURST_RATE = 3f;

	private const float COOLDOWN_TIME = 0.4f;

	private const int PROJECTILES_PER_LEVEL = 3;

	private const string NAME = "Machine Gun";

	public override void Init()
	{
		RateOfFire = RATE_OF_FIRE;
		BurstRate = BURST_RATE;

		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)FrontWeaponType.MachineGun;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.MachineGun;
		AmmoPrefab = ResourcesHelper.MachineGunBullet;

		base.Init();
	}
}