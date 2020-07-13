using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SaveSkills : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SavePrefabObj(GameObject go,string prefabName)
    {
        string path = "Assets/Artist/TARD/" + prefabName + ".prefab";
#if UNITY_EDITOR
        PrefabUtility.CreatePrefab(path, go);
#endif
    }

}
