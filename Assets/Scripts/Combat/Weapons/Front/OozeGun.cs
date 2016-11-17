
public class OozeGun : SingleFireWeapon
{
	private const float COOLDOWN_TIME = 1f;

	private const int PROJECTILES_PER_LEVEL = 1;

	private const string NAME = "Ooze Gun";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)FrontWeaponType.OozeGun;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.OozeGun;
		AmmoPrefab = ResourcesHelper.OozeGlobby;

		base.Init();
	}
}