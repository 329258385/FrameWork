using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VolumeMixer;
public static class VMMenu {

	[MenuItem("TARD/VolumeMixer/Add VMVolume")]
	private static void AddVMVolume()
	{
		var obj = Selection.activeGameObject;
		LsyCommon.TryAddComponent<VMVolume> (obj);
	}

	[MenuItem("TARD/VolumeMixer/Add VMEvent")]
	private static void AddVMEvent()
	{
		var obj = Selection.activeGameObject;
		LsyCommon.TryAddComponent<VMEvent> (obj);
	}

	[MenuItem("TARD/VolumeMixer/Add VMManager")]
	private static void AddVMManager()
	{
		var obj = Selection.activeGameObject;
		LsyCommon.TryAddComponent<VMManager> (obj);
	}
}