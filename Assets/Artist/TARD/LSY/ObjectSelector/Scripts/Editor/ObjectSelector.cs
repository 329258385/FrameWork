namespace ObjectSelectorSpace
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine.SceneManagement;

	public enum ResultType
	{
		OK=0,
		Self=1,
		Parent=2,
		NotActive=3,
		NoMesh=4,
		Cube=5,
		Minus=6
	}
	public class ObjInfo
	{
		public GameObject go;
		public ResultType result;
		public Rect rect;
	}
	public class ObjectSelector : EditorWindow {
		string[] solutions = { 
			"",
			"旋转0，缩放1",
			"父缩放不为负",
			"删物体/去Component",
			"删物体/去Component",
			"改BoxCollider",
			"正Scale"
		};

		float gridWidth = 100;
		List<ObjInfo> objs = new List<ObjInfo> ();
		Vector2 scrollPos;
		int selected;
		int selectedSub;

		List<GameObject> mySelection = new List<GameObject>();
		List<int> mySelectionIndex = new List<int>();
		string currentScene;

		[MenuItem("TARD/优化/LOD/4 碰撞体 Mesh读写等检查",false,4)]
		public static void Show()
		{
			var win = EditorWindow.GetWindow<ObjectSelector> ();
			win.title = "Mesh检查工具";
		}

		void Clear()
		{
			currentScene = "";
			objs.Clear ();
			ClearSelection ();
		}

		void Update()
		{
			this.Repaint ();
		}

		void AddToSelection(int i)
		{
			if (mySelectionIndex.Contains (i))
				return;
			mySelectionIndex.Add (i);
		
			var go = objs [i].go;
			mySelection.Add (go);
			Selection.objects = mySelection.ToArray ();
		}
		void RemoveFromSelection(int i)
		{
			if (!mySelectionIndex.Contains (i))
				return;
			int id = mySelectionIndex.IndexOf (i);
			mySelectionIndex.RemoveAt (id);
			mySelection.RemoveAt (id);
			Selection.objects = mySelection.ToArray ();
		}
		void ClearSelection()
		{
			selected = -1;
			selectedSub = -1;

			mySelectionIndex.Clear ();
			mySelection.Clear ();
		}
		void OnGUI()
		{
			this.titleContent.text = "碰撞体 Mesh读写等检查";

			if (!string.IsNullOrEmpty(currentScene) && currentScene != SceneManager.GetActiveScene ().name) {
				Clear ();
			}
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("搜场景",GUILayout.Width(gridWidth))) {
				SearchFromScene ();
			}
			if (GUILayout.Button ("搜已选",GUILayout.Width(gridWidth))) {
				SearchFromSelection ();
			}
			if (GUILayout.Button ("△",GUILayout.Width(20))) {
				Undo.RegisterCompleteObjectUndo (Selection.activeGameObject, "");
				Selection.activeGameObject.transform.parent = null;
			}
			if (GUILayout.Button ("修CubeScale为正",GUILayout.Width(gridWidth+10))) {
				FixCubeScale ();
			}


			GUILayout.EndHorizontal ();

			GUILayout.Space (30);
			GUILayout.Label ("结果",GUILayout.Width(gridWidth));
			scrollPos = GUILayout.BeginScrollView (scrollPos);

			if (mySelectionIndex.Count > 0) {
				GUI.color = new Color (0, 0.5f, 0.5f);
				for (int i = 0; i < mySelectionIndex.Count; i++) {
					int id = mySelectionIndex [i];
					GUI.DrawTexture (objs[id].rect, Texture2D.whiteTexture);
				}
				GUI.color = Color.white;
			}

			for (int i = 0; i < objs.Count; i++) {
				var go = objs [i].go;
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("△",GUILayout.Width(20))) {
					Undo.RegisterCompleteObjectUndo (objs [i].go, "");
					objs [i].go.transform.parent = null;
				}

				if(selected ==i && selectedSub==0)
					GUI.color = Color.green;
				if (GUILayout.Button (objs [i].go.name,GUILayout.Width(gridWidth))) {
					if (Event.current.shift) {
						//Multi Add
						if (selected != -1) {
							int min = Mathf.Min (selected, i);
							int max = Mathf.Max (selected, i);
							for (int j = min; j < max+1; j++) {
								AddToSelection (j);
							}
						}
					} else {
						//Single Select
						ClearSelection ();
						selected = i;
						selectedSub = 0;
						Selection.activeObject = objs [i].go;
						AddToSelection (i);
					}
				}
				if (GUILayout.Button ("+",GUILayout.Width(20))) {
					//Add
					AddToSelection (i);
				}
				if (GUILayout.Button ("-",GUILayout.Width(20))) {
					//Add
					RemoveFromSelection (i);
				}
				GUI.color = Color.white;


				string info="";
				if (objs [i].result == ResultType.Self)
					info = "自身";
				else if (objs [i].result == ResultType.Parent)
					info = "父级";
				else if (objs [i].result == ResultType.NotActive)
					info = "隐藏";
				else if (objs [i].result == ResultType.Cube)
					info = "Cube";
				else if (objs [i].result == ResultType.NoMesh)
					info = "空Mesh";
				else if (objs [i].result == ResultType.Minus)
					info = "缩放为负";

				GUILayout.Label (info, GUILayout.Width (gridWidth*0.5f));
				GUILayout.Label (string.Format("建议:{0}",solutions[(int)objs [i].result]), GUILayout.Width (gridWidth*2f));
				#region parent
				GUILayout.Label("父级:", GUILayout.Width (gridWidth*0.5f));
				var p = objs [i].go.transform.parent;
				int index = 1;
				while(p!=null)
				{
					if(selected ==i && selectedSub==index)
						GUI.color = Color.green;
					if(GUILayout.Button(string.Format("[{0}]{1}",index,p.name), GUILayout.Width (gridWidth*2)))
					{
						selected = i;
						selectedSub = index;
						Selection.activeObject = p.gameObject;
					}
					GUI.color = Color.white;
					p = p.parent;
					index++;
				}
				#endregion
				GUILayout.EndHorizontal ();

				if (Event.current.type == EventType.Repaint) {
					var rect = GUILayoutUtility.GetLastRect ();
					rect = new Rect (0, rect.y, rect.xMax, rect.height+5);
					objs [i].rect = rect;
				}
			}
			GUILayout.EndScrollView ();
		}

		void SearchFromScene()
		{
			List<Collider> mcs = new List<Collider> ();
			var objs = SceneManager.GetActiveScene ().GetRootGameObjects ();
			foreach (var item in objs) {
				var m = item.GetComponentsInChildren<Collider> (true);
				mcs.AddRange (m);
			}
			Search (mcs.ToArray(), NeedMod);
		}
		void SearchFromSelection()
		{
			var objs = Selection.gameObjects;
			Collider[] mcs = Selection.GetFiltered<Collider> (SelectionMode.Deep);
			Search (mcs, NeedMod);
		}
		void Search(Collider[] list,System.Func<MeshCollider,ResultType> condition )
		{
			Clear ();
			currentScene = SceneManager.GetActiveScene ().name;

			foreach (var item in list) {
				if (item is MeshCollider) {
					var col = (MeshCollider)item;
					Search_MeshCollider (col, condition);
				} else {
					Search_OtherCollider (item);
				}
			}
		}

		void Search_MeshCollider(MeshCollider item,System.Func<MeshCollider,ResultType> condition )
		{
			var result = condition (item); 
			if (result == ResultType.Self || result == ResultType.Parent) {
				if (IsReadable (item))
					return;
			}

			if(result!= ResultType.OK)
			{
				var info = new ObjInfo ();
				info.go = item.gameObject;
				info.result = result;
				objs.Add (info);
			}
		}

		void Search_OtherCollider(Collider item )
		{
			ResultType result = ResultType.OK;
			var t = item.transform;

			//if (!NoMinus (t.localScale) || !NoMinus (t.lossyScale)) {
			if (!NoMinus (t.localScale)) {
				result = ResultType.Minus;
			}

			if(result!= ResultType.OK)
			{
				var info = new ObjInfo ();
				info.go = item.gameObject;
				info.result = result;
				objs.Add (info);
			}
		}


		#region Mesh Collider check
		ResultType NeedMod(MeshCollider col)
		{
			if (col.sharedMesh == null)
				return ResultType.NoMesh;
			if (col.enabled == false || col.gameObject.activeSelf == false)
				return ResultType.NotActive;
			if (col.sharedMesh.name == "Cube")
				return ResultType.Cube;
			
			if(ParOk(col.transform))
				return ResultType.OK;

			var pos = col.transform.localPosition;
			var ro = col.transform.localEulerAngles;
			var scale = col.transform.localScale;
//			if (!AllMatch (pos, 0)) {
//				return ResultType.Self;
//			}
			if (!AllMatch (ro, 0)) {
				return ResultType.Self;
			}
			if (!AllMatch (scale, 1)) {
				return ResultType.Self;
			}

			var pa = col.transform.parent;
			while (pa != null) {
				if (!NoMinus (pa.localScale))
					return ResultType.Parent;
				pa = pa.parent;
			}
			return ResultType.OK;
		}
		#endregion
		bool ParOk(Transform tran)
		{
			var p = tran.parent;
			while (p != null) {
				var pos = p.localPosition;
				var ro = p.localEulerAngles;
				var scale = p.localScale;
				if (!AllMatch (ro, 0)) {
					return false;
				}
				if (!AllMatch (scale, 1)) {
					return false;
				}
				p = p.parent;
			}
			return true;
		}
		#region common
//		bool AllMatch(Vector3 v,float f)
//		{
//			if (v.x != f)
//				return false;
//			if (v.y != f)
//				return false;
//			if (v.z != f)
//				return false;
//			return true;
//		}
		bool AllMatch(Vector3 v,float f)
		{
			float min = 0.001f;
			if ((v.x > f + min) || (v.x < f - min))
				return false;
			if ((v.y > f + min) || (v.y < f - min))
				return false;
			if ((v.z > f + min) || (v.z < f - min))
				return false;
			return true;
		}

		bool NoMinus(Vector3 v)
		{
			if (v.x <0)
				return false;
			if (v.y <0)
				return false;
			if (v.z <0)
				return false;
			return true;
		}

		bool IsReadable(MeshCollider col)
		{
			if (col.sharedMesh == null)
				return false;
			return col.sharedMesh.isReadable;
		}
		#endregion

		#region Set All
		void FixCubeScale()
		{
			for (int i = 0; i < objs.Count; i++) {
				var item = objs [i];
				if (item.result == ResultType.Minus) {
					var s = item.go.transform.localScale;
					item.go.transform.localScale = new Vector3 (
						Mathf.Abs(s.x),
						Mathf.Abs(s.y),
						Mathf.Abs(s.z));

					EditorUtility.SetDirty (item.go);
				}
			}

			EditorSceneManager.MarkAllScenesDirty ();
		}
		#endregion
	}
}