using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class CBSystemMenu {
	public const string prefix= "TARD/CB光照系统/";
	//=================================
	//          光照系统
	//=================================
	[MenuItem(prefix+"1 光照系统",false,0)]
	private static void Add_CBSystem()
	{
		Create ("CBSystem");
	}

	//=================================
	//          实时阴影
	//=================================
	[MenuItem(prefix+"2 实时阴影",false,0)]
	private static void Add_Shadow()
	{
		Create ("Workers/3 RealtimeShadow/RealtimeShadow");
	}


	//=================================
	//          1 虚拟阴影
	//=================================
	[MenuItem(prefix+"3 虚拟阴影/贴花阴影")]
	private static void Add_DecalShadow()
	{
		Create ("Workers/4 FakeShadow/DecalShadow");
	}
	[MenuItem(prefix+"3 虚拟阴影/立体贴花阴影")]
	private static void Add_DecalVShadow()
	{
		Create ("Workers/4 FakeShadow/DecalVShadow");
	}
	[MenuItem(prefix+"3 虚拟阴影/球体阴影")]
	private static void Add_SphereShadow()
	{
		Create ("Workers/4 FakeShadow/DecalSphereShadow");
	}

	//=================================
	//          2 实时光照
	//=================================
	[MenuItem(prefix+"4 实时光照/体积光")]
	private static void Add_VLight()
	{
		Create ("Workers/1 Light/VLight");
	}
	[MenuItem(prefix+"4 实时光照/角色点光")]
	private static void Add_LightChar()
	{
		Create ("Workers/Char/DecalLightChar");
	}

	//=================================
	//          3 贴花系统
	//=================================
	[MenuItem(prefix+"5 贴花系统/贴花")]
	private static void Add_Decal()
	{
		Create ("Workers/2 Decal/Decal");
	}
	[MenuItem(prefix+"5 贴花系统/立体贴花")]
	private static void Add_VDecal()
	{
		Create ("Workers/2 Decal/DecalVFade");
	}

	//=================================
	//          4 贴花系统
	//=================================
	[MenuItem(prefix+"6 雾效系统/区域雾")]
	private static void Add_VolumeFog()
	{
		Create ("Workers/5 Fog/VolumeFog");
	}


	private static void Create(string title)
	{
		string path = string.Format ("Assets/Artist/TARD/LSY/CBSystem/Plugin/Prefabs/{0}.prefab", title);
		GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath (path);
		GameObject obj = PrefabUtility.InstantiatePrefab (prefab) as GameObject;
		Selection.activeGameObject = obj;

		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
	}
}
