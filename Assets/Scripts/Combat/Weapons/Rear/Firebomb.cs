
public class Firebomb : RearArmament
{
	private const float COOLDOWN_TIME = 5f;

	private const int PROJECTILES_PER_LEVEL = 2;

	private const string NAME = "Firebomb";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)RearWeaponType.Firebomb;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.Firebomb;
		AmmoPrefab = ResourcesHelper.FirebombCharge;

		base.Init();
	}
}