
public class ForceFieldGenerator : RearArmament
{
	private const float COOLDOWN_TIME = 5f;

	private const int PROJECTILES_PER_LEVEL = 3;

	private const string NAME = "Force Field";

	public override void Init()
	{
		CooldownTime = COOLDOWN_TIME;

		ProjectilesPerLevel = PROJECTILES_PER_LEVEL;
		WeaponType = (int)RearWeaponType.ForceField;
		WeaponName = NAME;

		WeaponPrefab = ResourcesHelper.ForceFieldGenerator;
		AmmoPrefab = ResourcesHelper.ForceField;

		base.Init();
	}
}