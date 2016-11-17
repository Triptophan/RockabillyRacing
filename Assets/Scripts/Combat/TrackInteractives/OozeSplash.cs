using UnityEngine;

public class OozeSplash : MonoBehaviour
{
	private GameObject _myGameObject;

	private const float SLOW_FACTOR = 0.98f;
	private const float NO_SLOW_FACTOR = 1f;

	public GameObject owner;

	void Awake()
	{
		_myGameObject = gameObject;
	}

	void Start()
	{
		ParticleSystem effect = _myGameObject.GetComponent<ParticleSystem>();

		if (effect != null)
		{
			Destroy(_myGameObject, effect.duration + 0.9f);
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.transform.root.name != owner.transform.name)
		{
			Rigidbody otherRigidBody = other.transform.root.rigidbody;

			if (otherRigidBody != null && !otherRigidBody.isKinematic)
			{
				otherRigidBody.velocity = otherRigidBody.velocity * SLOW_FACTOR;
			}
		}
	}
}