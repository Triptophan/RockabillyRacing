using UnityEngine;
using System.Collections;

public class FirebombPack : MonoBehaviour
{
	private const float FIRST_IMPACT_MULTIPLIER = 1.05f;

	private GameObject _myGameObject;
	private GameObject _effect;

	private Transform _myTransform;

	private AudioSource _audioSource;

	public GameObject Owner { get; set; }

	public float Damage { get; set; }
	
	void Awake()
	{
		_myGameObject = gameObject;
		_effect = ResourcesHelper.FirebombEffect;
		_myTransform = _myGameObject.transform;
		_audioSource = GetComponent<AudioSource>();
	}

	void OnTriggerEnter(Collider other)
	{
		Transform target = other.transform.root;

		DamageController damageController = target.GetComponent<DamageController>();
		if (damageController != null)
		{
			float damage = Damage * FIRST_IMPACT_MULTIPLIER;

			ForceField targetForceField = target.GetComponent<ForceField>();
			if (targetForceField != null)
			{
				damage = targetForceField.AbsorbDamage(damage);
			}

			if (_audioSource != null)
			{
				_audioSource.Play();
			}

			damageController.ApplyDamage(Owner, damage);

			if (_effect != null)
			{
				GameObject firebombEffectObject = (GameObject)Instantiate(_effect, _myTransform.position, _myTransform.rotation);
				if (firebombEffectObject != null)
				{
					FirebombEffect firebombEffect = firebombEffectObject.GetComponent<FirebombEffect>();
					if (firebombEffect != null)
					{
						firebombEffect.Owner = Owner;
						firebombEffect.Init();
					}
				}
			}

			Collider collider = _myGameObject.GetComponent<Collider>();
			if (collider != null)
			{
				collider.enabled = false;
			}

			Destroy(_myGameObject, 1f);
		}
	}
}