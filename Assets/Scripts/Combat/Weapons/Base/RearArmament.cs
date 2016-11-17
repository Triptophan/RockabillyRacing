using UnityEngine;
using System.Collections;

public class RearArmament : IWeapon
{
	private bool _canFire = true;

	private RearWeaponType _rearWeaponType;

	#region IWeapon Properties
	public bool CanFire
	{
		get { return _canFire && (Capacity == -1 || AmmoRemaining > 0); }
		set { _canFire = value; }
	}

	public string WeaponName { get; set; }
	public string ProjectilePrefabName { get; set; }
	public string WeaponPrefabName { get; set; }

	public int AmmoRemaining { get; set; }
	public int ProjectilesPerLevel { get; set; }
	public int Capacity { get; set; }
	public int AmmoAbilityLevel { get; set; }
	public int WeaponType
	{
		get { return (int)_rearWeaponType; }
		set { _rearWeaponType = (RearWeaponType)value; }
	}

	public float CooldownTime { get; set; }
	public float Damage { get; set; }

	public GameObject WeaponPrefab { get; set; }
	public GameObject AmmoPrefab { get; set; }
	public GameObject Owner { get; set; }

	public Transform WeaponMount { get; set; }
	public Transform AmmoSpawnPoint { get; set; }

	public AudioSource WeaponSound { get; set; }

	public ParticleSystem WeaponEffect { get; set; }
	#endregion

	#region IWeapon Methods
	public virtual IEnumerator Fire(GameObject target)
	{
		if (CanFire)
		{

			GameObject armament = (GameObject)GameObject.Instantiate(AmmoPrefab,
															WeaponMount.TransformPoint(AmmoSpawnPoint.position),
															AmmoSpawnPoint.rotation);

			IProjectile armamentComponent = armament.GetComponent<Projectile>();

			if (armamentComponent != null)
			{
				armamentComponent.Init();
				armamentComponent.Owner = Owner;
				armamentComponent.AmmoSpawnPoint = AmmoSpawnPoint;

				PlayFireSound();
				AmmoRemaining--;

				FireSpecial(armament, armamentComponent);

				if (WeaponEffect != null && !WeaponEffect.isPlaying)
				{
					WeaponEffect.Play();
				}

				GameObject.Destroy(armament, armamentComponent.MaxLiveTime);
			}
		}

		yield break;
	}

	public virtual void Init()
	{
		AmmoSpawnPoint = WeaponHelper.GetAmmoSpawn(WeaponPrefab);

		GameObject weaponObject = (GameObject)GameObject.Instantiate(WeaponPrefab,
																		WeaponMount.position,
																		WeaponMount.transform.rotation);

		weaponObject.transform.parent = WeaponMount;

		WeaponEffect = weaponObject.GetComponentInChildren<ParticleSystem>();

		WeaponSound = weaponObject.GetComponent<AudioSource>();

		Capacity = ProjectilesPerLevel * (AmmoAbilityLevel + 1);

		Reload();
	}

	public virtual void Reload()
	{
		if (AmmoRemaining < Capacity)
		{
			AmmoRemaining = Capacity;
		}
	}

	public virtual void AddAmmo()
	{
		AmmoRemaining++;
	}

	public void PlayFireSound()
	{
		if (WeaponSound == null) return;

		WeaponSound.Play();
	}
	#endregion

	private void FireSpecial(GameObject armament, IProjectile armamentComponent)
	{
		FireMultiProjectiles(armament, armamentComponent);

		FireGroundEffect(armament, armamentComponent);

		TriggerBuff(armament, armamentComponent);
	}

	private void FireMultiProjectiles(GameObject armament, IProjectile armamentComponent)
	{
		IMultiShot multiShotArmament = armamentComponent as IMultiShot;

		if (multiShotArmament != null)
		{
			multiShotArmament.FireShots();
		}
	}

	private void FireGroundEffect(GameObject armament, IProjectile armamentComponent)
	{
		IGroundEffect groundArmament = armamentComponent as IGroundEffect;

		if (groundArmament != null)
		{
			int layerMask = 1 << LayerMask.NameToLayer("Track");

			RaycastHit hit = new RaycastHit();

			if (Physics.Raycast(armament.transform.position, -armament.transform.up, out hit, Mathf.Infinity, layerMask))
			{
				armament.transform.position = hit.point + (Vector3.up * 0.1f);

				armamentComponent.Owner = Owner;
				groundArmament.ShowGroundEffect(armament.collider);
			}
		}
	}

	private void TriggerBuff(GameObject armament, IProjectile armamentComponent)
	{
		IBuffEffect buffArmament = armamentComponent as IBuffEffect;

		if (buffArmament != null)
		{
			buffArmament.ApplyBuff();
		}
	}
}