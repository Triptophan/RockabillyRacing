using UnityEngine;

public class TrackManager : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name.Contains("WP") || other.gameObject.tag == "Track") return;

		Projectile projectile = other.transform.root.GetComponent<Projectile>();

		if (projectile != null)
		{
			IGroundEffect groundEffectProjectile = projectile as IGroundEffect;
			if (groundEffectProjectile != null)
			{
				groundEffectProjectile.ShowGroundEffect(other);
			}

			if (!other.name.Contains("Flames") && !other.name.Contains("Firebomb"))
			{
				Destroy(other.gameObject);
			}
		}
	}

	void OnCollisionEnter(Collision other)
	{
		GameObject otherObject = other.gameObject;

		if (GameObjectHelper.IsACar(otherObject))
		{
			DamageController damageController = otherObject.GetComponent<DamageController>();
			if (damageController != null)
			{
				damageController.PlayCollisionNoises(otherObject);
			}
		}
		else
		{
			Projectile projectile = otherObject.transform.root.GetComponent<Projectile>();

			if (projectile != null)
			{
				IGroundEffect groundEffectProjectile = projectile as IGroundEffect;

				if (groundEffectProjectile != null)
				{
					groundEffectProjectile.ShowGroundEffect(other.collider);
				}

				Destroy(otherObject);
			}
		}
	}
}