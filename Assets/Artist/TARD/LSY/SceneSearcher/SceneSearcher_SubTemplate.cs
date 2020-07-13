#if UNITY_EDITOR
namespace SceneSearcher
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.SceneManagement;

	public class SceneSearcher_SubTemplate : SceneSearcher<MeshRenderer> {
		//[MenuItem("TARD/优化/LOD/Template")]
		public static void MenuBT()
		{
			SceneSearcher_SubTemplate s = (SceneSearcher_SubTemplate)EditorWindow.GetWindow(typeof(SceneSearcher_SubTemplate));
			s.Show ();
		}

		protected override void Init ()
		{
			base.Init ();
			titleBig = "标题";
			titleRef = "参考";
			titleSearch = "搜索";
			titleResult = "结果";
			titleApplyAll = "设置";
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
			return base.Search_ConditionCheck(mr);
		}

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