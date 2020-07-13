using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TARDSwitchesPC_Editor{
	[MenuItem("TARD/Bloom/PC On")]
	private static void BloomOn()
	{
		TARDSwitchesPC.SetBool (TARDSwitchesPC.KeyBloom, true);
	}
	[MenuItem("TARD/Bloom/PC Off")]
	private static void BloomOff()
	{
		TARDSwitchesPC.SetBool (TARDSwitchesPC.KeyBloom, false);
	}

	[MenuItem("TARD/Grass/PC On")]
	private static void GrassOn()
	{
		TARDSwitchesPC.SetBool (TARDSwitchesPC.KeyGrass, true);
	}
	[MenuItem("TARD/Grass/PC Off")]
	private static void GrassOff()
	{
		TARDSwitchesPC.SetBool (TARDSwitchesPC.KeyGrass, false);
	}
}
