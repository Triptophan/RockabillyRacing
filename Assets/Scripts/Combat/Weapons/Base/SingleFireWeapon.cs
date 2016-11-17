using UnityEngine;
using System.Collections;

public class SingleFireWeapon : IWeapon
{
	private bool _canFire;

	private FrontWeaponType _frontWeaponType;

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
		get { return (int)_frontWeaponType; }
		set { _frontWeaponType = (FrontWeaponType)value; }
	}

	public float CooldownTime { get; set; }
	public float Damage { get; set; }

	public GameObject WeaponPrefab { get; set; }
	public GameObject AmmoPrefab { get; set; }
	public GameObject Owner { get; set; }

	public Transform WeaponMount { get; set; }
	public Transform AmmoSpawnPoint { get; set; }

	public ParticleSystem BarrelFireEffect { get; set; }

	public AudioSource WeaponSound { get; set; }
	#endregion

	#region IWeapon Methods
	public virtual IEnumerator Fire(GameObject target)
	{
		if (CanFire)
		{
			GameObject projectile = (GameObject)GameObject.Instantiate(AmmoPrefab,
															WeaponMount.transform.TransformPoint(AmmoSpawnPoint.position),
															Owner.transform.rotation);
			projectile.transform.Rotate(AmmoPrefab.transform.rotation.eulerAngles);

			IProjectile projectileComponent = projectile.GetComponent<Projectile>();

			if (projectileComponent != null)
			{
				projectileComponent.Init();
				projectileComponent.Owner = Owner;
				projectileComponent.AmmoSpawnPoint = AmmoSpawnPoint;

				IHomingProjectile homingProjectile = projectileComponent as IHomingProjectile;

				if (homingProjectile != null)
				{
					homingProjectile.Target = target;
					Vector3 heading = (target.transform.position - Owner.transform.position).normalized;
					float dot = Vector3.Dot(heading, Owner.transform.forward);
					if (dot <= 0.7f)
					{
						GameObject.Destroy(projectile);
						yield break;
					}
				}

				projectile.rigidbody.velocity = Owner.transform.rigidbody.velocity +
												WeaponMount.TransformDirection(AmmoSpawnPoint.forward * projectileComponent.ProjectileSpeed);

				PlayFireSound();
				AmmoRemaining--;

				GameObject.Destroy(projectile, projectileComponent.MaxLiveTime);
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

		Transform barrelEffectTransform = weaponObject.transform.FindChild("BarrelEffect");
		if (barrelEffectTransform != null)
		{
			BarrelFireEffect = barrelEffectTransform.GetComponent<ParticleSystem>();
		}

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
}