
public class ScatterJacks : RearArmament
{
	private const float COOLDOWN_TIME = 5f;

	private const int PROJECTILES_PER_LEVEL = 3;

	private const string NAME = "Scatter Jacks";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)RearWeaponType.ScatterJack;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.ScatterJacks;
		AmmoPrefab = ResourcesHelper.ScatterJackCharge;

		base.Init();
	}
}