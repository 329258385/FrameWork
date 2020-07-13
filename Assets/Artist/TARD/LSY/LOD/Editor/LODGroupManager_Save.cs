namespace LsyLOD
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	public partial class LODGroupManager
    {

        const char splitChar = '|';
	    public static void SaveAll2Prefs()
        {
            LODGroup[] lodGroups = GameObject.FindObjectsOfType<LODGroup>();
            foreach (var group in lodGroups)
            {
                LOD[] lods = group.GetLODs();
                string lodInfo = "";
                int index = 0;
                foreach (var lod in lods)
                {
                    index++;
                    lodInfo += lod.screenRelativeTransitionHeight;
                    if (index < lods.Length)
                    {
                        lodInfo += splitChar;
                    }
                }
                PlayerPrefs.SetString(group.gameObject.GetInstanceID().ToString(), lodInfo);
            }
        }

        public static void SaveFromPrefs()
        {
            LODGroup[] lodGroups = GameObject.FindObjectsOfType<LODGroup>();
            foreach (var group in lodGroups)
            {
                string lodInfo = PlayerPrefs.GetString(group.gameObject.GetInstanceID().ToString(), "");
                List<float> relativeHeights = new List<float>();
                string[] lodValueGroup = lodInfo.Split(splitChar);
                if (lodValueGroup.Length > 0)
                {
                    foreach (string lodValue in lodValueGroup)
                    {
                        relativeHeights.Add(float.Parse(lodValue));
                    }
                    SetLODGroup_RefHeight(group, relativeHeights);
                }
            }

            SaveScene();
        }


        public static void Save2Selected(List<float> relativeHeights)
        {
            GameObject[] gameObjects = Selection.gameObjects;
            foreach (var o in gameObjects)
            {
                LODGroup lod = o.GetComponent<LODGroup>();
                if (lod)
                {
                    SetLODGroup_RefHeight(lod, relativeHeights);
                }
            }

            SaveScene();
        }

        public static void SaveScene()
        {
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
        }

        public static void SaveLODInRuntime()
        {
            SaveAll2Prefs();
            if (EditorApplication.isPlaying)
            {
                EditorApplication.playModeStateChanged -= PlayModeStateChanged;
                EditorApplication.playModeStateChanged += PlayModeStateChanged;
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }
            else
            {
                SaveScene();
            }
        }

        static void PlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.playModeStateChanged -= PlayModeStateChanged;
                SaveFromPrefs();
            }
        }
    }
}