using UnityEngine;

public class FlameToken : MonoBehaviour, IDamageToken
{
	private DamageController _damageController;

	public GameObject Owner { get; set; }

	public float DamagePerTick { get; set; }
	public float Duration { get; set; }

	protected DamageController DamageController
	{
		get
		{
			if (_damageController == null)
			{
				_damageController = transform.root.GetComponent<DamageController>();
			}

			return _damageController;
		}
	}

	void Start()
	{
		InvokeRepeating("TickDamage", Duration, 1f);
		Destroy(this, Duration);
	}

	void TickDamage()
	{
		float damage = DamagePerTick;

		ForceField forceField = transform.root.GetComponent<ForceField>();

		if (forceField != null)
		{
			damage = forceField.AbsorbDamage(damage);
		}

		if (DamageController != null && DamageController.enabled)
		{
			DamageController.ApplyDamage(Owner, DamagePerTick);
		}
	}
}