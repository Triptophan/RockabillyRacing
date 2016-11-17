using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{	
	void OnCollisionEnter(Collision collision)
	{
		GameObject collidedObject = collision.gameObject.transform.root.gameObject;
		if(collidedObject != null)
		{
			if(GameObjectHelper.IsACar(collidedObject))
			{
				DamageController damageController = collidedObject.GetComponent<DamageController>();
			
				if(damageController != null)
				{
					damageController.DoDeathCondition(null, 1f);
				}
			}
			
			if(collidedObject.tag == "PowerUp")
			{
				Destroy(collidedObject);
			}
		}
	}
}