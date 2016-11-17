using UnityEngine;

public class AbilityScore : MonoBehaviour
{
	#region Constants
	public const float MIN_CORNERING_ABILITY = 1f;
	public const float MAX_CORNERING_ABILITY = 10f;
	private const float MIN_CORNERING_MULTIPLIER = 5f;
	private const float MAX_CORNERING_MULTIPLIER = 0.1f;

	public const float MIN_ACCELERATION_ABILITY = 1f;
	public const float MAX_ACCELERATION_ABILITY = 10f;
	private const float MIN_ACCELERATION_MULTIPLIER = 1f;
	private const float MAX_ACCELERATION_MULTIPLIER = 4f;

	public const float MIN_TOP_SPEED_ABILITY = 1f;
	public const float MAX_TOP_SPEED_ABILITY = 10f;
	private const float MIN_TOP_SPEED_MULTIPLIER = 1f;
	private const float MAX_TOP_SPEED_MULTIPLIER = 2f;

	public const float MIN_ARMOR_ABILITY = 1f;
	public const float MAX_ARMOR_ABILITY = 10f;
	private const float MIN_ARMOR_MULTIPLIER = 1f;
	private const float MAX_ARMOR_MULTIPLIER = 5f;
	#endregion

	#region Private Members
	private float _corneringAbilityMultiplier = 3.5f;
	private float _accelerationAbilityMultiplier = 4f;
	private float _topSpeedAbilityMultiplier = 1f;
	private float _armorAbilityMultiplier = 10f;
	#endregion

	#region Properties
	public float AccelerationAbility = 1f;
	public float AccelerationMultiplier { get { return _accelerationAbilityMultiplier; } }

	public float CorneringAbility = 1f;
	public float CorneringMultiplier { get { return _corneringAbilityMultiplier; } }

	public float TopSpeedAbility = 1f;
	public float TopSpeedMultiplier { get { return _topSpeedAbilityMultiplier; } }

	public float ArmorAbility = 1f;
	public float ArmorMultiplier { get { return _armorAbilityMultiplier; } }
	#endregion

	#region Unity Methods
	public void Awake()
	{
		CreateScoreRanges();
	}
	#endregion

	#region Private Methods
	private void CreateScoreRanges()
	{
		float oldRange, newRange;

		oldRange = (MAX_CORNERING_ABILITY - MIN_CORNERING_ABILITY);
		newRange = (MAX_CORNERING_MULTIPLIER - MIN_CORNERING_MULTIPLIER);

		_corneringAbilityMultiplier = (((CorneringAbility - MIN_CORNERING_ABILITY) * newRange) / oldRange) + MIN_CORNERING_MULTIPLIER;

		oldRange = (MAX_ACCELERATION_ABILITY - MIN_ACCELERATION_ABILITY);
		newRange = (MAX_ACCELERATION_MULTIPLIER - MIN_ACCELERATION_MULTIPLIER);

		_accelerationAbilityMultiplier = (((AccelerationAbility - MIN_ACCELERATION_ABILITY) * newRange) / oldRange) + MIN_ACCELERATION_MULTIPLIER;

		oldRange = (MAX_TOP_SPEED_ABILITY - MIN_TOP_SPEED_ABILITY);
		newRange = (MAX_TOP_SPEED_MULTIPLIER - MIN_TOP_SPEED_MULTIPLIER);

		_topSpeedAbilityMultiplier = (((TopSpeedAbility - MIN_TOP_SPEED_ABILITY) * newRange) / oldRange) + MIN_ACCELERATION_MULTIPLIER;

		oldRange = (MAX_ARMOR_ABILITY - MIN_ARMOR_ABILITY);
		newRange = (MAX_ARMOR_MULTIPLIER - MIN_ARMOR_MULTIPLIER);

		_armorAbilityMultiplier = (((ArmorAbility - MIN_ARMOR_ABILITY) * newRange) / oldRange) + MIN_ARMOR_ABILITY;
	}

	public void AssignAbilityScores()
	{
		PlayerData data = new PlayerData();
		ArmorAbility = ArmorAbility == 0 ? data.ArmorAbilityPoints : ArmorAbility;
		AccelerationAbility = AccelerationAbility == 0 ? data.AccelerationAbilityPoints : AccelerationAbility;
		CorneringAbility = CorneringAbility == 0 ? data.CorneringAbilityPoints : CorneringAbility;
		TopSpeedAbility = TopSpeedAbility == 0 ? data.TopSpeedAbilityPoints : TopSpeedAbility;
	}
	#endregion
}