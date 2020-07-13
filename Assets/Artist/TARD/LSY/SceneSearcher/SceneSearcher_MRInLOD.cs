#if UNITY_EDITOR
namespace SceneSearcher
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.SceneManagement;

	public class SceneSearcher_MRInLOD : SceneSearcher<MeshRenderer> {
		[MenuItem("TARD/优化/LOD/3 MR 不在LOD下等检查",false,3)]
		public static void MenuBT()
		{
			SceneSearcher_MRInLOD s = (SceneSearcher_MRInLOD)EditorWindow.GetWindow(typeof(SceneSearcher_MRInLOD));
			s.Show ();
		}

		protected override void Init ()
		{
			base.Init ();
			titleBig = "MR 不在LOD下等检查";
			titleRef = "";
			titleSearch = "搜索";
			titleResult = "结果";
			titleApplyAll = "";
		}

		protected override void Ref_GUI ()
		{
			base.Ref_GUI ();
		}

		protected override void Search_GUI_Condition ()
		{
			base.Search_GUI_Condition ();
		}

		protected override bool Search_ConditionCheck (MeshRenderer mr)
		{
			if (mr.GetComponent<MeshFilter> () != null && mr.GetComponent<MeshFilter> ().sharedMesh == null)
				return true;
			if (mr.enabled == false)
				return false;
			if (mr.sharedMaterial == null)
				return true;

			var g = mr.GetComponentInParent<LODGroup> ();
			if (g == null) {
				return true;
			} else {
				var lods = g.GetLODs ();
				foreach (var lod in lods) {
					foreach (var r in lod.renderers) {
						if (r == mr)
							return false;
					}
				}
			}
			return true;
		}

	//	protected override bool Search_ConditionCheck (MeshRenderer mr)
	//	{
	//		if (mr.enabled == false)
	//			return false;
	//		var g = mr.GetComponentInParent<LODGroup> ();
	//		if (g == null) {
	//			return true;
	//		} 
	//		return false;
	//	}

		protected override void Result_GUI (MeshRenderer t)
		{
			base.Result_GUI (t);
		}

		protected override void SetAll_GUI_Setting ()
		{
			base.SetAll_GUI_Setting ();
		}

		protected override void SetOne (MeshRenderer t)
		{
			base.SetOne (t);
		}
	}
}
#endif