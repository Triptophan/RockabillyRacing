
public class RocketLauncher : SingleFireWeapon
{
	private const float COOLDOWN_TIME = 1f;

	private const int PROJECTILES_PER_LEVEL = 1;

	private const string NAME = "Rocket Launcher";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)FrontWeaponType.RocketLauncher;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.RocketLauncher;
		AmmoPrefab = ResourcesHelper.Rocket;

		base.Init();
	}
}