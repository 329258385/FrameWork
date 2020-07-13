using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TARDSwitchesPC{
	public const string KeyBloom = "Bloom_Switch";
	public const string KeyGrass = "Grass_Switch";



	public static bool GrassSwitch()
	{
		return GetBool (KeyGrass);
	}
	public static bool BloomSwitch()
	{
		return GetBool (KeyBloom);
	}





	public static bool GetBool(string key)
	{
		if (!PlayerPrefs.HasKey (key))
			PlayerPrefs.SetInt (key, 1);
		return PlayerPrefs.GetInt (key) != 0;
	}
	public static void SetBool(string key,bool b)
	{
		PlayerPrefs.SetInt (key, b?1:0);
	}
}
