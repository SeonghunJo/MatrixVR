using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO; // For File Class


public static class Utility 
{
	public static string cacheFolderPath = Application.persistentDataPath;

	static public bool FindCachedImageFromID(string id)
	{
		if( File.Exists(cacheFolderPath + "/" + id + "_front.png")
		   && File.Exists(cacheFolderPath + "/" + id + "_back.png")
		   && File.Exists(cacheFolderPath + "/" + id + "_left.png")
		   && File.Exists(cacheFolderPath + "/" + id + "_right.png")
		   && File.Exists(cacheFolderPath + "/" + id + "_up.png")
		   && File.Exists(cacheFolderPath + "/" + id + "_down.png"))
		{
			return true;
		}
		return false;
	}
}