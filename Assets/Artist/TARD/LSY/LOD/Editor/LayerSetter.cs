//namespace LsyLOD
//{
//	using System.Collections;
//	using System.Collections.Generic;
//	using UnityEngine;
//	using UnityEditor;
//	using UnityEditor.SceneManagement;
//	public class LayerSetter : EditorWindow {
//		static Color highLightColor = new Color (0, 1, 1);
//
//		float columnWidth=100f;
//		float buttonWidth=60f;
//		int layer;
//		float sizeMin = 0;
//		float sizeMax = 5;
//		bool includeDeactive = true;
//		bool includeCollider = false;
//
//		Vector2 scrollPos;
//		List<MeshRenderer> mrs = new List<MeshRenderer>();
//
//		[MenuItem("TARD/优化/LOD/Layer设置")]
//		public static void Init()
//		{
//			LayerSetter s = (LayerSetter)EditorWindow.GetWindow(typeof(LayerSetter));
//			s.Show ();
//		}
//
//		void Update()
//		{
//			
//		}
//
//		void OnGUI()
//		{
//			this.titleContent.text = "Layer设置";
//			ShowTitle ("当前物体Size");
//			if (Selection.activeGameObject != null) {
//				MeshRenderer mr = Selection.activeGameObject.GetComponent<MeshRenderer> ();
//				if (mr != null) {
//					var s = mr.bounds.size;
//					GUILayout.Label (string.Format ("Size:{0}", GetMax(s)),GUILayout.Width(columnWidth));
//					GUILayout.Label (string.Format ("X:{0} Y:{1} Z:{2}", s.x,s.y,s.z));
//				}
//			}
//			GUILayout.Space (15);
//
//
//			ShowTitle ("Layer搜索");
//			GUILayout.BeginHorizontal ();
//			GUILayout.Label ("Size",GUILayout.Width(columnWidth));
//			sizeMin = EditorGUILayout.FloatField (sizeMin,GUILayout.Width(columnWidth));
//			GUILayout.Label ("~",GUILayout.Width(20));
//			sizeMax = EditorGUILayout.FloatField (sizeMax,GUILayout.Width(columnWidth));
//			GUILayout.EndHorizontal ();
//
//			GUILayout.BeginHorizontal ();
//			GUILayout.Label ("包含隐藏",GUILayout.Width(columnWidth));
//			includeDeactive = GUILayout.Toggle (includeDeactive,"");
//			GUILayout.EndHorizontal ();
//
//			GUILayout.BeginHorizontal ();
//			GUILayout.Label ("包含Collider",GUILayout.Width(columnWidth));
//			includeCollider = GUILayout.Toggle (includeCollider,"");
//			GUILayout.EndHorizontal ();
//
//			GUILayout.BeginHorizontal ();
//			if (GUILayout.Button ("搜索全部",GUILayout.Width(buttonWidth))) {
//				var m = Object.FindObjectsOfType<MeshRenderer> ();
//				mrs.Clear ();
//				mrs.AddRange (m);
//				Repaint ();
//			}
//			if (GUILayout.Button ("搜索所选",GUILayout.Width(buttonWidth))) {
//				mrs.Clear ();
//				foreach (var obj in Selection.gameObjects) {
//					var m = obj.GetComponentsInChildren<MeshRenderer> (true);
//					mrs.AddRange (m);
//
//				}
//				Repaint ();
//			}
//			GUILayout.EndHorizontal ();
//			GUILayout.Space (15);
//
//
//			ShowTitle ("Layer设置");
//			GUILayout.BeginHorizontal ();
//			GUILayout.Label ("目标层",GUILayout.Width(columnWidth));
//			layer = EditorGUILayout.LayerField (layer,GUILayout.Width(columnWidth));
//			GUILayout.EndHorizontal ();
//			if (GUILayout.Button ("设置",GUILayout.Width(buttonWidth))) {
//				Process ();
//			}
//			GUILayout.Space (15);
//
//			ShowList ();
//		}
//
//		int startIndex=0;
//		void ShowList()
//		{
//			int range = 1000;
//			GUILayout.Label ("总数：" + mrs.Count);
//			int id=0;
//			GUILayout.BeginHorizontal ();
//			while (id < mrs.Count) {
//				if (GUILayout.Button (string.Format("{0}~{1}",id,id+range-1),GUILayout.Width(120))) {
//					startIndex = id;
//				}
//				if (id % 5000 == 4000) {
//					GUILayout.EndHorizontal ();
//					GUILayout.BeginHorizontal ();
//				}
//				id += 1000;
//			}
//			GUILayout.EndHorizontal ();
//
//			GUILayout.Space (15);
//			GUILayout.Label (string.Format ("列表：{0}~{1}", startIndex, startIndex + range-1));
//			int toDelete = -1;
//			scrollPos = GUILayout.BeginScrollView (scrollPos);
//			for (int i = startIndex;i<Mathf.Min(startIndex+range,mrs.Count);i++) {
//				GUILayout.BeginHorizontal ();
//				if (GUILayout.Button (mrs [i].name,GUILayout.Width(300))) {
//					Selection.activeObject = mrs [i].gameObject;
//				}
//				if (GUILayout.Button ("X",GUILayout.Width(30))) {
//					toDelete = i;
//				}
//				GUILayout.EndHorizontal ();
//			}
//			GUILayout.EndScrollView ();
//			if (toDelete >= 0) {
//				mrs.RemoveAt (toDelete);
//				Repaint ();
//			}
//		}
//
//		void Process()
//		{
//			if (!EditorApplication.isPlaying) {
//				Undo.RecordObjects (mrs.ToArray (), "LayerSet");
//				EditorSceneManager.MarkAllScenesDirty ();
//			}
//			foreach (var mr in mrs) {
//				SetLayer (mr, layer);
//			}
//			Repaint ();
//		}
//		void SetLayer(MeshRenderer mr,int layer)
//		{
//			if(NeedSet(mr))
//				mr.gameObject.layer = layer;
//		}
//
//		bool NeedSet(MeshRenderer mr)
//		{
//			if (!includeCollider) {
//				if (mr.GetComponent<Collider> () != null)
//					return false;
//			}
//
//			if (!includeDeactive) {
//				if (mr.gameObject.activeInHierarchy == false || mr.enabled == false)
//					return false;
//			}
//
//
//			float size = MRGetSize (mr);
//			if (size >= sizeMin && size <= sizeMax)
//				return true;
//			return false;
//		}
//
//
//		#region common
//		void ShowTitle(string title)
//		{
//			GUI.color = highLightColor;
//			GUI.skin.label.fontStyle = FontStyle.Bold;
//			var size = GUI.skin.label.fontSize;
//			GUI.skin.label.fontSize = 14;
//			GUILayout.Label (string.Format ("{0}", title),GUILayout.Width(columnWidth));
//			GUI.color = Color.white;
//			GUI.skin.label.fontStyle = FontStyle.Normal;
//			GUI.skin.label.fontSize = size;
//		}
//
//		public static float MRGetSize(MeshRenderer mr)
//		{
//			return GetMax (mr.bounds.size);
//		}
//
//		public static float GetMax(Vector3 v)
//		{
//			return Mathf.Max (v.x, v.y, v.z);
//		}
//		#endregion
//	}
//}