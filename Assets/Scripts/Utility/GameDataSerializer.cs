using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections.Generic;

public class GameDataSerializer<T>
{
	private static string _path = Application.dataPath + "/";
	
	public static void SerializeToXML(T t, string fileName)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		TextWriter textWriter = new StreamWriter(_path + fileName);
		
		try
		{
			serializer.Serialize(textWriter, t);	
		}
		catch(FileNotFoundException)
		{
			Debug.LogError("The file " + fileName + " could not be found.");
		}
		catch(DirectoryNotFoundException)
		{
			Debug.LogError("Application data path could not be found.");
		}
		finally
		{
			textWriter.Close ();
		}		
	}
	
	public static void SerializeListToXML(List<T> t, string fileName)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
		TextWriter textWriter = new StreamWriter(_path + fileName);
		
		try
		{
			serializer.Serialize(textWriter, t);	
		}
		catch(FileNotFoundException)
		{
			Debug.LogError("The file " + fileName + " could not be found.");
		}
		catch(DirectoryNotFoundException)
		{
			Debug.LogError("Application data path could not be found.");
		}
		finally
		{
			textWriter.Close ();
		}		
	}
	
	public static T DeserializeFromXML(string fileName)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		TextReader reader = new StreamReader(_path + fileName);
		
		T data = default(T);
		
		try
		{
			data = (T)serializer.Deserialize(reader);
		}
		catch(FileNotFoundException)
		{
			Debug.LogError("The file " + fileName + " could not be found.");
		}
		finally
		{
			reader.Close();
		}
		
		return data;
	}
	
	public static List<T> DeserializeListFromXML(string fileName)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
		TextReader reader = new StreamReader(_path + fileName);
		
		List<T> data = default(List<T>);
		
		try
		{
			data = (List<T>)serializer.Deserialize(reader);
		}
		catch(FileNotFoundException)
		{
			Debug.LogError("The file " + fileName + " could not be found.");
		}
		finally
		{
			reader.Close();
		}
		
		return data;
	}
}