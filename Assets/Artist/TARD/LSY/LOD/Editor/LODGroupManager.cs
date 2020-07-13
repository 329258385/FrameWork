namespace LsyLOD
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	public partial class LODGroupManager : EditorWindow {
		static float Bias{get{ return QualitySettings.lodBias;}}
		static Color highLightColor = new Color (0, 1, 1);
		float columnWidth=100f;
		float buttonWidth=60f;
		public Camera cam;
		static Camera sceneCam{get{ return SceneView.lastActiveSceneView.camera;}}


		public List<float> relativeHeights = new List<float> (){0.5f,0.3f,0.2f};
		[MenuItem("TARD/优化/LOD/1 LOD总管理",false,1)]
		public static void Init()
		{
			LODGroupManager s = (LODGroupManager)EditorWindow.GetWindow(typeof(LODGroupManager));
			s.Show ();
		}

		void Update()
		{
			this.Repaint ();
		}

        void OnGUI()
		{
			if (cam == null)
				cam = Camera.main;
			this.titleContent.text = "LOD总管理";
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("当前Bias",GUILayout.Width(columnWidth));
			GUILayout.Label ("" + Bias,GUILayout.Width(columnWidth));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("主相机",GUILayout.Width(columnWidth));
			cam = (Camera)EditorGUILayout.ObjectField ("",cam,typeof(Camera), true, GUILayout.Width(columnWidth));
			GUILayout.EndHorizontal ();

			GUILayout.Space (15);
			OnGUI_Global ();

			GUILayout.Space (15);
			OnGUI_CurrentObj ();

			GUILayout.Space (30);
			OnGUI_Recal ();

			GUILayout.Space (30);
			OnGUI_Fade ();
		}


		#region Global
		void OnGUI_Global()
		{
			ShowTitle ("占屏比设置");
			int toRemoveIndex = -1;
			for (int i = 0; i < relativeHeights.Count; i++) {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format("LOD {0}",i),GUILayout.Width(columnWidth));
				relativeHeights [i] = EditorGUILayout.FloatField ("", relativeHeights [i],GUILayout.Width(columnWidth));
				if (GUILayout.Button ("X",GUILayout.Width(30))) {
					toRemoveIndex = i;
				}
				GUILayout.EndHorizontal ();
			}
			if (toRemoveIndex >= 0)
				relativeHeights.RemoveAt (toRemoveIndex);
			if (GUILayout.Button ("+",GUILayout.Width(30))) {
				relativeHeights.Add (relativeHeights [relativeHeights.Count - 1]);
			}

			GUILayout.BeginHorizontal ();
//			if (GUILayout.Button ("设置全部",GUILayout.Width(buttonWidth))) {
//				var groups = Object.FindObjectsOfType<LODGroup> ();
//				Undo.RecordObjects (groups, "LODSet");
//				EditorSceneManager.MarkAllScenesDirty ();
//				foreach (var group in groups) {
//					SetLODGroup_RefHeight (group, relativeHeights);
//				}
//			}
			if (GUILayout.Button ("设置所选",GUILayout.Width(buttonWidth))) {
				var groups = Selection.GetFiltered<LODGroup> (SelectionMode.DeepAssets);
				Undo.RecordObjects (groups, "LODSet");
				EditorSceneManager.MarkAllScenesDirty ();
				foreach (var group in groups) {
					SetLODGroup_RefHeight (group, relativeHeights);
				}
			}

            GUILayout.EndHorizontal ();

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("运行时保存LOD数据", GUILayout.Width(buttonWidth * 2.5f)))
                {
                    SaveLODInRuntime();
                }
            }
        }

		static void SetLODGroup_RefHeight(LODGroup group, List<float> rHeights)
		{
			var lods = group.GetLODs ();
			for (int i = 0; i < lods.Length; i++) {
				lods [i].screenRelativeTransitionHeight = rHeights[i];
			}
			group.SetLODs (lods);
		}

		void OnGUI_Recal()
		{
			ShowTitle ("包围盒校正");
			GUILayout.BeginHorizontal ();
//			if (GUILayout.Button ("校正全部",GUILayout.Width(buttonWidth))) {
//				var groups = Object.FindObjectsOfType<LODGroup> ();
//
//				foreach (var group in groups) {
//					group.RecalculateBounds ();
//				}
//			}
			if (GUILayout.Button ("校正所选",GUILayout.Width(buttonWidth))) {
				var groups = Selection.GetFiltered<LODGroup> (SelectionMode.DeepAssets);
				foreach (var group in groups) {
					group.RecalculateBounds ();
				}
			}
			GUILayout.EndHorizontal ();
		}

		LODFadeMode lodFadeMode;
		bool animateCrossFading;
		void OnGUI_Fade()
		{
			ShowTitle ("Fade设置");
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("AnimateCrossFading",GUILayout.Width(columnWidth+30));
			animateCrossFading = GUILayout.Toggle (animateCrossFading, "",GUILayout.Width(columnWidth));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("LODFadeMode",GUILayout.Width(columnWidth+30));
			lodFadeMode = (LODFadeMode)EditorGUILayout.EnumPopup ((LODFadeMode)lodFadeMode,GUILayout.Width(columnWidth));
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
//			if (GUILayout.Button ("设置全部",GUILayout.Width(buttonWidth))) {
//				var groups = Object.FindObjectsOfType<LODGroup> ();
//				foreach (var group in groups) {
//					group.fadeMode = lodFadeMode;
//					group.animateCrossFading = animateCrossFading;
//				}
//			}

			if (GUILayout.Button ("设置所选",GUILayout.Width(buttonWidth))) {
				var groups = Selection.GetFiltered<LODGroup> (SelectionMode.DeepAssets);
				foreach (var group in groups) {
					group.fadeMode = lodFadeMode;
					group.animateCrossFading = animateCrossFading;
				}
			}
			GUILayout.EndHorizontal ();
		}
		#endregion




		#region CurrentObj
		void OnGUI_CurrentObj()
		{
			var obj = Selection.activeGameObject;
			if (obj == null)
				return;
			LODGroup group = obj.GetComponent<LODGroup> ();
			if (group == null)
				return;
			if (cam == null)
				return;

			ShowTitle ("当前物体");
			var sceneDis = Vector3.Distance (group.transform.TransformPoint (group.localReferencePoint), sceneCam.transform.position);
			var sceneReHeight = group.GetRelativeHeight (sceneCam);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Scene",GUILayout.Width(columnWidth));
			GUILayout.Label (string.Format ("占屏比:{0}", sceneReHeight.ToString("f2")),GUILayout.Width(columnWidth));
			GUILayout.Label (string.Format ("距离:{0}", sceneDis.ToString("f2")),GUILayout.Width(columnWidth));
			GUILayout.EndHorizontal ();

			var lods = group.GetLODs ();
			for (int i = 0; i < lods.Length; i++) {
				var lod = lods [i];
				var dis = LODGroupExtensions.RelativeHeightToDistance (cam, lod.screenRelativeTransitionHeight, group);
				dis *= Bias;

				GUILayout.BeginHorizontal ();
				GUILayout.Label (string.Format ("LOD {0}", i),GUILayout.Width(columnWidth));
				GUILayout.Label (string.Format ("占屏比:{0}",lod.screenRelativeTransitionHeight.ToString("f2")),GUILayout.Width(columnWidth));
				GUILayout.Label (string.Format ("距离:{0}",dis.ToString("f2")),GUILayout.Width(columnWidth));
				GUILayout.EndHorizontal ();
			}
		}
		#endregion




		#region common
		void ShowTitle(string title)
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
		#endregion
	}
}