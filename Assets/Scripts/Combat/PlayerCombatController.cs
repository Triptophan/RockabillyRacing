using System.Collections;
using UnityEngine;

public class PlayerCombatController : CombatControllerBase
{
	private GameObject[] allDrivers;

	private float _frontCooldownTime = 0f;
	private float _rearCooldownTime = 0f;

	public int FrontWeaponAmmoRemaining { get { return frontWeapon == null ? 0 : frontWeapon.AmmoRemaining; } }
	public int RearWeaponAmmoRemaining { get { return rearWeapon == null ? 0 : rearWeapon.AmmoRemaining; } }

	public string FrontWeaponName { get { return frontWeapon == null ? "" : frontWeapon.WeaponName; } }
	public string RearWeaponName { get { return rearWeapon == null ? "" : rearWeapon.WeaponName; } }

	public void Awake()
	{
		_myGameObject = gameObject;
		_myTransform = transform;
	}

	public void Start()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag(TagHelper.GAME_CONTROLLER);
		if (gameControllerObject != null)
		{
			GameController game = gameControllerObject.GetComponent<GameController>();
			if (game != null)
			{
				allDrivers = game.GetAllDrivers();
			}
		}
	}

	public void Update()
	{
		GetInput();
	}

	private void GetInput()
	{
		if (Input.GetButton("FireFront") && Time.time > _frontCooldownTime)
		{
			StartCoroutine(FireFront());

			_frontCooldownTime = Time.time + (frontWeapon == null ? 0f : frontWeapon.CooldownTime);
		}

		if (Input.GetButton("FireRear") && Time.time > _rearCooldownTime)
		{
			StartCoroutine(FireRear());

			_rearCooldownTime = Time.time + (rearWeapon == null ? 0f : rearWeapon.CooldownTime);
		}
	}

	private IEnumerator FireFront()
	{
		GameObject closestEnemy;
		if (frontWeaponType == FrontWeaponType.HomingRocketLauncher)
		{
			closestEnemy = FindClosestFrontEnemy();
		}
		else
		{
			closestEnemy = FindClosestEnemy();
		}

		if (closestEnemy == null)
		{
			if (frontWeaponType != FrontWeaponType.HomingRocketLauncher)
			{
				Debug.Log("Player cannot fire, no enemies on the track.");
			}
			yield break;
		}

		yield return StartCoroutine(frontWeapon.Fire(closestEnemy));
	}

	private IEnumerator FireRear()
	{
		return rearWeapon.Fire(null);
	}

	private GameObject FindClosestFrontEnemy()
	{
		GameObject closestSoFar = null;

		if (allDrivers.Length > 0)
		{
			Vector3 lengthToClosest = Vector3.zero;

			for (int i = 0; i < allDrivers.Length; i++)
			{
				if (allDrivers[i].GetInstanceID() != _myGameObject.GetInstanceID())
				{
					Transform targetTransform = allDrivers[i].transform;
					Vector3 distance = _myTransform.position - targetTransform.position;
					Vector3 heading = (targetTransform.position - _myTransform.position).normalized;
					float dot = Vector3.Dot(heading, _myTransform.forward);
					if (dot > 0.7f)
					{
						if ((allDrivers[i] != _myGameObject) && i == 0 || distance.sqrMagnitude < lengthToClosest.sqrMagnitude)
						{
							lengthToClosest = distance;
							closestSoFar = allDrivers[i];
						}
					}
				}
			}
		}

		return closestSoFar;
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

					if ((allDrivers[i] != _myGameObject) && i == 0 || distance.sqrMagnitude < lengthToClosest.sqrMagnitude)
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