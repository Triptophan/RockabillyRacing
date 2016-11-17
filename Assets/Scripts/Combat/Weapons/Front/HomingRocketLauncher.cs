
public class HomingRocketLauncher : SingleFireWeapon
{
	private const float COOLDOWN_TIME = 0.5f;

	private const int PROJECTILES_PER_LEVEL = 1;

	private const string NAME = "Homing Rocket Launcher";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)FrontWeaponType.HomingRocketLauncher;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.HomingRocketLauncher;
		AmmoPrefab = ResourcesHelper.HomingRocket;

		base.Init();
	}
}