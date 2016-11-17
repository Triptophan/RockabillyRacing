using System.Collections;
using UnityEngine;

public class HomingBeconEffect : MonoBehaviour
{
	private Light _myLight;
	
	void Awake()
	{
		_myLight = GetComponent<Light>();
	}
	
	void Start()
	{
		StartCoroutine(DoBlink());
	}
	
	private IEnumerator DoBlink()
	{
		yield return new WaitForSeconds(1f);
		
		_myLight.enabled = !_myLight.enabled;
		
		StartCoroutine(DoBlink());
	}
}