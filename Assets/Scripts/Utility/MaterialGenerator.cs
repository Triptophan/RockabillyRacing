/*using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class MaterialGenerator : MonoBehaviour 
{
	void Awake()
	{
		Type myType = typeof(XKCDColors);
		PropertyInfo[] properties = myType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
		
		foreach(PropertyInfo property in properties)
		{
			Material newMaterial = new Material(Shader.Find("Diffuse"));
			newMaterial.color = (Color)property.GetValue(myType, null);
			
			string assetPath = "Assets/Materials/" + property.Name + ".mat";
			
			AssetDatabase.CreateAsset(newMaterial, assetPath);
		}
	}
}
*/