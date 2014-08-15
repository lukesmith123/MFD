/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013
 *	
 *	"CustomAssetUtility.cs"
 * 
 *	This script allows assets to be created based on a supplied script.
 *	It is based on code written by Jacob Pennock (http://www.jacobpennock.com/Blog/?p=670)
 * 
 */

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;

public static class CustomAssetUtility
{

	public static void CreateAsset<T> () where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();

		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "")
		{
			path = "Assets";
		}
		else if (Path.GetExtension (path) != "")
		{
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + Path.DirectorySeparatorChar.ToString () + "New " + typeof(T).ToString() + ".asset");

		AssetDatabase.CreateAsset (asset, assetPathAndName);

		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
	
	
	public static void CreateAsset<T> (string filename, string path) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath ("Assets" + Path.DirectorySeparatorChar.ToString () + path + Path.DirectorySeparatorChar.ToString () + filename + ".asset");
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
	}
	
	
	public static T CreateAndReturnAsset<T> (string filename, string path) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath ("Assets" + Path.DirectorySeparatorChar.ToString () + path + Path.DirectorySeparatorChar.ToString () + filename + ".asset");
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		
		return asset;
	}
	
	
	public static T CreateAndReturnAsset<T> (string path) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath ("Assets" + Path.DirectorySeparatorChar.ToString () + path + Path.DirectorySeparatorChar.ToString () + typeof(T).ToString() + ".asset");
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		
		return asset;
	}

}

#endif