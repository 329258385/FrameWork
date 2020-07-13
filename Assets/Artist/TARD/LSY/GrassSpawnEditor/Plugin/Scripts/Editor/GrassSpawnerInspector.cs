using UnityEngine;
using UnityEditor;

[CustomEditor( typeof( GrassSpawner ) )]
public class GrassSpawnerInspector : Editor
{
	#if UNITY_EDITOR
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		GrassSpawner gs = (GrassSpawner)target;
		if (GUILayout.Button ("保存")) {
			gs.Save ();
		}
	}
	#endif
}