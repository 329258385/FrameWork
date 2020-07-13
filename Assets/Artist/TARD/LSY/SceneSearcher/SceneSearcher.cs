#if UNITY_EDITOR
namespace SceneSearcher
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.SceneManagement;

	public class SceneSearcher<T> : EditorWindow 
		where T:Component
	{
		private static Color highLightColor = new Color (0, 1, 1);
		protected float columnWidth=100f;
		protected float buttonWidth=60f;

		private bool init = false;
		protected string titleBig;
		protected string titleRef;
		protected string titleSearch;
		protected string titleResult;
		protected string titleApplyAll;
		protected bool HasRef{get{return !string.IsNullOrEmpty (titleRef);}}
		protected bool HasSearch{get{return !string.IsNullOrEmpty (titleSearch);}}
		protected bool HasResult{get{return !string.IsNullOrEmpty (titleResult);}}
		protected bool HasApply{get{return !string.IsNullOrEmpty (titleApplyAll);}}

		private Vector2 scrollPos;
		private List<T> mrs = new List<T>();

		private void Awake()
		{
			Init ();
		}
		private void OnGUI()
		{
			this.titleContent.text = titleBig;

			if(HasRef){
				OnGUI_Ref ();
				GUILayout.Space (15);
			}

			if(HasSearch){
				OnGUI_Search ();
				GUILayout.Space (15);
			}

			if(HasApply){
				OnGUI_ApplyAll ();
				GUILayout.Space (15);
			}

			if(HasResult){
				OnGUI_Result ();
				GUILayout.Space (15);
			}
		}

		private void OnGUI_Ref()
		{
			ShowTitle (titleRef);
			Ref_GUI ();
		}
		private void OnGUI_Search()
		{
			ShowTitle (titleSearch);
			Search_GUI_Condition ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("搜索全部",GUILayout.Width(buttonWidth))) {
				var m = Object.FindObjectsOfType<T> ();
				mrs.Clear ();
				foreach (var item in m) {
					if(!mrs.Contains(item) && Search_ConditionCheck(item))
						mrs.Add (item);
				}
				Repaint ();
			}
			if (GUILayout.Button ("搜索所选",GUILayout.Width(buttonWidth))) {
				mrs.Clear ();
				foreach (var obj in Selection.gameObjects) {
					var m = obj.GetComponentsInChildren<T> (true);
					foreach (var item in m) {
						if(!mrs.Contains(item) && Search_ConditionCheck(item))
							mrs.Add (item);
					}
				}
				Repaint ();
			}
			GUILayout.EndHorizontal ();

		}
		private void OnGUI_Result()
		{
			ShowTitle (titleResult);
			ShowList ();
		}
		private void OnGUI_ApplyAll()
		{
			ShowTitle (titleApplyAll);
			SetAll_GUI_Setting ();
			if (GUILayout.Button ("全部设置",GUILayout.Width(buttonWidth))) {
				Process ();
			}
		}
		private void Process()
		{
			if (!EditorApplication.isPlaying) {
				Undo.RecordObjects (mrs.ToArray (), titleBig);
				EditorSceneManager.MarkAllScenesDirty ();
			}
			foreach (var mr in mrs) {
				SetOne (mr);
			}
			Repaint ();
		}
		#region override
		protected virtual void Init ()
		{
		}
		protected virtual void Ref_GUI ()
		{
		}
		protected virtual void Search_GUI_Condition ()
		{
			
		}
		protected virtual bool Search_ConditionCheck (T t)
		{
			return true;
		}

		protected virtual void Result_GUI (T t)
		{
			
		}
		protected virtual void SetAll_GUI_Setting ()
		{
			
		}
		protected virtual void SetOne (T t)
		{

		}


		#endregion

		#region List
		private int startIndex=0;
		private void ShowList()
		{
			int range = 1000;
			GUILayout.Label ("总数：" + mrs.Count);
			int id=0;
			GUILayout.BeginHorizontal ();
			while (id < mrs.Count) {
				if (GUILayout.Button (string.Format("{0}~{1}",id,id+range-1),GUILayout.Width(120))) {
					startIndex = id;
				}
				if (id % 5000 == 4000) {
					GUILayout.EndHorizontal ();
					GUILayout.BeginHorizontal ();
				}
				id += 1000;
			}
			GUILayout.EndHorizontal ();

			GUILayout.Space (15);
			GUILayout.Label (string.Format ("列表：{0}~{1}", startIndex, startIndex + range-1));
			int toDelete = -1;
			scrollPos = GUILayout.BeginScrollView (scrollPos);
			for (int i = startIndex;i<Mathf.Min(startIndex+range,mrs.Count);i++) {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (mrs [i].name,GUILayout.Width(300))) {
					Selection.activeObject = mrs [i].gameObject;
				}
				if (GUILayout.Button ("X",GUILayout.Width(30))) {
					toDelete = i;
				}

				if (HasApply) {
					if (GUILayout.Button ("设置", GUILayout.Width (40))) {
						SetOne (mrs [i]);
					}
				}
				Result_GUI (mrs [i]);
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndScrollView ();
			if (toDelete >= 0) {
				mrs.RemoveAt (toDelete);
				Repaint ();
			}
		}
		#endregion

		#region common
		private void ShowTitle(string title)
		{
			GUI.color = highLightColor;
			GUI.skin.label.fontStyle = FontStyle.Bold;
			var size = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = 14;
			GUILayout.Label (string.Format ("{0}", title),GUILayout.Width(columnWidth));
			GUI.color = Color.white;
			GUI.skin.label.fontStyle = FontStyle.Normal;
			GUI.skin.label.fontSize = size;
		}

		public static float MRGetSize(MeshRenderer mr)
		{
			return GetMax (mr.bounds.size);
		}

		public static float GetMax(Vector3 v)
		{
			return Mathf.Max (v.x, v.y, v.z);
		}

		protected void Param_Float(string nm,ref float f)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Label (nm,GUILayout.Width(columnWidth));
			f = EditorGUILayout.FloatField (f,GUILayout.Width(columnWidth));
			GUILayout.EndHorizontal ();
		}
		protected void Param_FloatMinMax(string nm,ref float min,ref float max)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Label (nm,GUILayout.Width(columnWidth));
			min = EditorGUILayout.FloatField (min,GUILayout.Width(columnWidth));
			GUILayout.Label ("~",GUILayout.Width(20));
			max = EditorGUILayout.FloatField (max,GUILayout.Width(columnWidth));
			GUILayout.EndHorizontal ();
		}
		protected void Param_Bool(string nm,ref bool b)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Label (nm,GUILayout.Width(columnWidth));
			b = GUILayout.Toggle (b,"");
			GUILayout.EndHorizontal ();
		}
		#endregion
	}
}
#endif