
public class OilSlick : RearArmament
{
	private const float COOLDOWN_TIME = 2f;

	private const int PROJECTILES_PER_LEVEL = 2;

	private const string NAME = "Oil Slicks";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)RearWeaponType.OilSlick;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.OilSlicksWeapon;
		AmmoPrefab = ResourcesHelper.OilSlickCharge;

		base.Init();
	}
}