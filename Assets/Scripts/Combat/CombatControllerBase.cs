using UnityEngine;
using System.Collections;

public class CombatControllerBase : MonoBehaviour, ICombatController
{
	private bool _canFire;

	public FrontWeaponType frontWeaponType;
	public RearWeaponType rearWeaponType;

	public int AmmoAbilityLevel { get; set; }

	protected IWeapon frontWeapon;
	protected IWeapon rearWeapon;

	protected GameObject _myGameObject;
	protected Transform _myTransform;

	public bool CanFire
	{
		get { return _canFire; }
		set
		{
			_canFire = value;
			if (frontWeapon != null) { frontWeapon.CanFire = _canFire; }
			if (rearWeapon != null) { rearWeapon.CanFire = _canFire; }
		}
	}

	public void SetWeapon(FrontWeaponType type)
	{
		if (type == FrontWeaponType.None) return;

		frontWeaponType = type;

		frontWeapon = WeaponHelper.SetWeapon(type);

		frontWeapon.Owner = _myGameObject;
		frontWeapon.WeaponMount = WeaponHelper.GetWeaponMount(_myTransform, true);
		frontWeapon.AmmoAbilityLevel = AmmoAbilityLevel;

		frontWeapon.Init();

		frontWeapon.CanFire = CanFire;
	}

	public void SetWeapon(RearWeaponType type)
	{
		if (type == RearWeaponType.None) return;

		rearWeaponType = type;

		rearWeapon = WeaponHelper.SetWeapon(type);

		rearWeapon.Owner = _myGameObject;
		rearWeapon.WeaponMount = WeaponHelper.GetWeaponMount(_myTransform, false);
		rearWeapon.AmmoAbilityLevel = AmmoAbilityLevel;
		rearWeapon.Init();

		rearWeapon.CanFire = CanFire;
	}

	public void ReloadWeapons()
	{
		frontWeapon.Reload();
		rearWeapon.Reload();
	}

	public void AddAmmo(bool isFront)
	{
		if (isFront)
		{
			frontWeapon.AddAmmo();
		}
		else
		{
			rearWeapon.AddAmmo();
		}
	}

	private void SaveAndResizeCollider(Collider collider, int index, bool resize)
	{
		BoxCollider boxTest = collider as BoxCollider;
		if (boxTest != null)
		{
			if (resize)
			{
				boxTest.size = Vector3.zero;
			}

			return;
		}

		CapsuleCollider capTest = collider as CapsuleCollider;
		if (capTest != null)
		{
			if (resize)
			{
				capTest.radius = 0f;
				capTest.height = 0f;
			}

			return;
		}
	}
}