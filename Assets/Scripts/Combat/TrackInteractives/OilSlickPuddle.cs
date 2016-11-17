using UnityEngine;

public class OilSlickPuddle : MonoBehaviour
{
	private const float SLIP_DURATION = 1f;
	private const float LIVE_DURATION = 5f;

	private GameObject _myGameObject;

	void Awake()
	{
		_myGameObject = gameObject;
	}

	void Start()
	{
		Destroy(_myGameObject, LIVE_DURATION);
	}

	void OnTriggerEnter(Collider other)
	{
		Transform target = other.transform.root;

		DamageController damageController = target.GetComponent<DamageController>();

		if (damageController != null)
		{
			damageController.StartSlip();
		}
	}

	void OnTriggerExit(Collider other)
	{
		Transform target = other.transform.root;

		DamageController damageController = target.GetComponent<DamageController>();

		if (damageController != null)
		{
			damageController.StopSlip(SLIP_DURATION);
		}
	}
}