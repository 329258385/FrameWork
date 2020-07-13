#if UNITY_EDITOR
namespace SceneSearcher
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.SceneManagement;

	public class SceneSearcher_MRLayer : SceneSearcher<MeshRenderer> {
		float sizeMin = 0;
		float sizeMax = 5;
		bool includeDeactive = true;
		bool includeCollider = false;
		int layer;

		[MenuItem("TARD/优化/LOD/2 MR 设置Layer",false,2)]
		public static void MenuBT()
		{
			SceneSearcher_MRLayer s = (SceneSearcher_MRLayer)EditorWindow.GetWindow(typeof(SceneSearcher_MRLayer));
			s.Show ();
		}

		protected override void Init ()
		{
			base.Init ();
			titleBig = "MR 设置Layer";
			titleRef = "当前MR";
			titleSearch = "搜索";
			titleResult = "结果";
			titleApplyAll = "设置";
		}

		protected override void Ref_GUI ()
		{
			base.Ref_GUI ();
			if (Selection.activeGameObject != null) {
				MeshRenderer mr = Selection.activeGameObject.GetComponent<MeshRenderer> ();
				if (mr != null) {
					var s = mr.bounds.size;
					GUILayout.Label (string.Format ("Size:{0}", GetMax(s)),GUILayout.Width(columnWidth));
					GUILayout.Label (string.Format ("X:{0} Y:{1} Z:{2}", s.x,s.y,s.z));
				}
			}
		}


		protected override void Search_GUI_Condition ()
		{
			base.Search_GUI_Condition ();
			Param_FloatMinMax ("Size", ref sizeMin, ref sizeMax);
			Param_Bool ("包含隐藏", ref includeDeactive);
			Param_Bool ("包含Collider", ref includeCollider);
		}
		protected override bool Search_ConditionCheck (MeshRenderer mr)
		{
			if (!includeCollider) {
				if (mr.GetComponent<Collider> () != null)
					return false;
			}

			if (!includeDeactive) {
				if (mr.gameObject.activeInHierarchy == false || mr.enabled == false)
					return false;
			}


			float size = MRGetSize (mr);
			if (size >= sizeMin && size <= sizeMax)
				return true;
			return false;
		}

		protected override void Result_GUI (MeshRenderer t)
		{
			base.Result_GUI (t);
		}
			
		protected override void SetAll_GUI_Setting ()
		{
			base.SetAll_GUI_Setting ();
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("目标层",GUILayout.Width(columnWidth));
			layer = EditorGUILayout.LayerField (layer,GUILayout.Width(columnWidth));
			GUILayout.EndHorizontal ();
		}

		protected override void SetOne (MeshRenderer t)
		{
			base.SetOne (t);
			t.gameObject.layer = layer;
		}
	}
}
#endif