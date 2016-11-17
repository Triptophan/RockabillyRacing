
public class SingleMines : RearArmament
{
	private const float COOLDOWN_TIME = 5f;

	private const int PROJECTILES_PER_LEVEL = 4;

	private const string NAME = "Mines";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)RearWeaponType.Landmines;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.SingleMinesWeapon;
		AmmoPrefab = ResourcesHelper.SingleMinesCharge;

		base.Init();
	}
}