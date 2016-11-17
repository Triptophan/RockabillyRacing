using UnityEngine;
using System.Collections;

public class VehicleLightController : MonoBehaviour
{
	public Transform[] frontLights;
	public Transform[] rearLights;
	
	public void ToggleHeadLights(bool enabled)
	{
		foreach(Transform light in frontLights)
		{
			light.gameObject.SetActive(enabled);
		}
	}
	
	public void SetRearLights(VehicleLightState lightState)
	{
		foreach(Transform light in rearLights)
		{
			Light lightComponent = light.GetComponent<Light>();
			if(lightComponent != null)
			{
				switch(lightState)
				{
					case VehicleLightState.On:
						lightComponent.enabled = true;
						lightComponent.intensity = 0.5f;
						break;
					case VehicleLightState.Bright:
						lightComponent.enabled = true;
						lightComponent.intensity = 2f;
						break;						
					default: //Off
						lightComponent.enabled = false;
						break;
				}
			}
			
		}		
	}
}

public enum VehicleLightState
{
	Off,
	On,
	Bright
}