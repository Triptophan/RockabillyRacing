using UnityEngine;

public class OozePuddle : MonoBehaviour
{
	private GameObject _myGameObject;
	
	private const float SLOW_FACTOR = 0.98f;
	private const float NO_SLOW_FACTOR = 1f;
	
	void Awake()
	{
		_myGameObject = gameObject;
	}

	void Start ()
	{
		ParticleSystem effect = _myGameObject.GetComponent<ParticleSystem>();
			
		if(effect != null)
		{
			Destroy(_myGameObject, effect.duration);
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		Rigidbody otherRigidBody = other.transform.root.rigidbody;
		
		if(otherRigidBody != null)
		{
			otherRigidBody.velocity = otherRigidBody.velocity * SLOW_FACTOR;
		}
	}
}