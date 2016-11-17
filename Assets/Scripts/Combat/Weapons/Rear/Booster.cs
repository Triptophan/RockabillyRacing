
public class Booster : RearArmament
{
	private const float COOLDOWN_TIME = 2f;

	private const int PROJECTILES_PER_LEVEL = 2;

	private const string NAME = "Nitro Booster";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)RearWeaponType.NitroBooster;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.Booster;
		AmmoPrefab = ResourcesHelper.BoosterCharge;

		base.Init();
	}
}