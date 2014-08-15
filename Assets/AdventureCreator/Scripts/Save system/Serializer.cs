/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"Serializer.cs"
 * 
 *	This script serializes saved game data and performs the file handling.
 * 
 * 	It is partially based on Zumwalt's code here:
 * 	http://wiki.unity3d.com/index.php?title=Save_and_Load_from_XML
 *  and uses functions by Nitin Pande:
 *  http://www.eggheadcafe.com/articles/system.xml.xmlserialization.asp 
 * 
 */

using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml; 
using System.Xml.Serialization;
using System.IO;

public class Serializer : MonoBehaviour
{
	
	public static T returnComponent <T> (int constantID) where T : MonoBehaviour
	{
		T result = null;
		
		if (constantID != 0)
		{
			T[] components = FindObjectsOfType (typeof(T)) as T[];
	
			foreach (T component in components)
			{
				if (component.GetComponent <ConstantID>())
				{
					if (component.GetComponent <ConstantID>().constantID == constantID)
					{
						// Found it
						result = component;
						break;
					}
				}
			}
		}
		
		return result;
	}
	
	
	public static int GetConstantID (GameObject _gameObject)
	{
		if (_gameObject.GetComponent <ConstantID>())
		{
			if (_gameObject.GetComponent <ConstantID>().constantID != 0)
			{
				return (_gameObject.GetComponent <ConstantID>().constantID);
			}
			else
			{	
				Debug.LogWarning ("GameObject " + _gameObject.name + " was not saved because it does not have an ID.");
			}
		}
		else
		{
			Debug.LogWarning ("GameObject " + _gameObject.name + " was not saved because it does not have a constant ID script!");
		}
		
		return 0;
	}
	

	public static string SerializeObjectBinary (object pObject)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream ();
		binaryFormatter.Serialize (memoryStream, pObject);
		return (Convert.ToBase64String (memoryStream.GetBuffer ()));
	}
	
	
	public static T DeserializeObjectBinary <T> (string pString)
	{
		  BinaryFormatter binaryFormatter = new BinaryFormatter();
		
		  MemoryStream memoryStream = new MemoryStream (Convert.FromBase64String (pString));
		
		  return (T) binaryFormatter.Deserialize (memoryStream);
	}
	
	
	public static string SerializeObjectXML <T> (object pObject) 
	{ 
		string XmlizedString = null; 
		
		MemoryStream memoryStream = new MemoryStream(); 
		XmlSerializer xs = new XmlSerializer (typeof (T)); 
		XmlTextWriter xmlTextWriter = new XmlTextWriter (memoryStream, Encoding.UTF8); 
		
		xs.Serialize (xmlTextWriter, pObject); 
		memoryStream = (MemoryStream) xmlTextWriter.BaseStream; 
		XmlizedString = UTF8ByteArrayToString (memoryStream.ToArray());
		
		return XmlizedString; 
	}
	
 
	public static object DeserializeObjectXML <T> (string pXmlizedString) 
	{ 
		XmlSerializer xs = new XmlSerializer (typeof (T)); 
		MemoryStream memoryStream = new MemoryStream (StringToUTF8ByteArray (pXmlizedString)); 
		return xs.Deserialize(memoryStream); 
	}
	
	
	private static string UTF8ByteArrayToString (byte[] characters) 
	{		
		UTF8Encoding encoding = new UTF8Encoding(); 
		string constructedString = encoding.GetString (characters); 
		return (constructedString); 
	}
 
	private static byte[] StringToUTF8ByteArray (string pXmlString) 
	{ 
		UTF8Encoding encoding = new UTF8Encoding(); 
		byte[] byteArray = encoding.GetBytes (pXmlString); 
		return byteArray; 
	}
	
	
	public static List<SingleLevelData> DeserializeRoom (string pString)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream (Convert.FromBase64String (pString));
		return (List<SingleLevelData>) binaryFormatter.Deserialize (memoryStream);
	}
	
	
	public static void CreateSaveFile (string fullFileName, string _data)
	{
		#if !UNITY_WEBPLAYER
		StreamWriter writer;
		
		FileInfo t = new FileInfo (fullFileName);
		
		if (!t.Exists)
		{
			writer = t.CreateText ();
		}
		
		else
		{
			t.Delete ();
			writer = t.CreateText ();
		}
		
		writer.Write (_data);
		writer.Close ();
		
		Debug.Log ("File written: " + fullFileName);
		#endif
	}
	
	
	public static string LoadSaveFile (string fullFileName)
	{
		string _data;
		
		StreamReader r = File.OpenText (fullFileName);
		
		string _info = r.ReadToEnd ();
		r.Close ();
		_data = _info;
		
		Debug.Log ("File Read: " + fullFileName);
		return (_data);
	}
	
}
