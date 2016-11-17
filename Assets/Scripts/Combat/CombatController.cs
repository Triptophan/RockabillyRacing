using UnityEngine;
using System.Collections;

public class CombatController : CombatControllerBase
{
	private GameObject[] allDrivers;

	private float _frontCooldownTime = 0f;
	private float _rearCooldownTime = 0f;

	public float AttackCooldown { get; set; }
	public float DefendCooldown { get; set; }

	public void Awake()
	{
		_myGameObject = gameObject;
		_myTransform = transform;
	}

	public void Start()
	{
		AmmoAbilityLevel = Random.Range(0, 3);
		SetWeapon(frontWeaponType);
		SetWeapon(rearWeaponType);

		CanFire = false;

		GameController game = GameObject.FindGameObjectWithTag(TagHelper.GAME_CONTROLLER).GetComponent<GameController>();

		if (game != null)
		{
			allDrivers = game.GetAllDrivers();
		}
	}

	public bool AttackFront()
	{
		if (Time.time > _frontCooldownTime)
		{
			GameObject closestEnemy = FindClosestEnemy();

			if (closestEnemy == null) return false;

			StartCoroutine("FireFront", closestEnemy);

			_frontCooldownTime = Time.time + frontWeapon.CooldownTime;

			return true;
		}

		return false;
	}

	public bool AttackRear()
	{
		if (Time.time > _rearCooldownTime)
		{
			StartCoroutine("FireRear");

			_rearCooldownTime = Time.time + rearWeapon.CooldownTime;

			return true;
		}

		return false;
	}

	private IEnumerator FireFront(GameObject target)
	{
		yield return StartCoroutine(frontWeapon.Fire(target));
	}

	private IEnumerator FireRear()
	{
		yield return StartCoroutine(rearWeapon.Fire(null));
	}

	private GameObject FindClosestEnemy()
	{
		GameObject closestSoFar = null;

		if (allDrivers.Length > 0)
		{
			Vector3 lengthToClosest = Vector3.zero;

			for (int i = 0; i < allDrivers.Length; i++)
			{
				if (allDrivers[i].GetInstanceID() != _myGameObject.GetInstanceID())
				{
					Vector3 distance = _myTransform.position - allDrivers[i].transform.position;

					if (lengthToClosest == Vector3.zero || distance.sqrMagnitude < lengthToClosest.sqrMagnitude)
					{
						lengthToClosest = distance;
						closestSoFar = allDrivers[i];
					}
				}
			}
		}

		return closestSoFar;
	}
}