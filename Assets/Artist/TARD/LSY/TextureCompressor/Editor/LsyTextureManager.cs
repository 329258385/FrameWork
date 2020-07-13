namespace LsyTextureCompressor
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public enum LsyPlatform
	{
		iPhone,
		Android,
	}

	public enum MaxSize
	{
		_32 = 32,
		_64 = 64,
		_128 = 128,
		_256 = 256,
		_512 = 512,
		_1024 = 1024,
		_2048 = 2048,
		_4096 = 4096,
		_8192 = 8192,
	}

	public enum TexSortMethod
	{
		size,
		size_rev,
		name,
		name_rev
	}
		
	public class LsyTextureManager:EditorWindow  {
		public static LsyTextureManager Instance;
		static Color highLightColor = new Color (0, 1, 1);
		#region Filter
		bool advancedMode = false;
		MaxSize sizeFilter = MaxSize._8192;
		bool above;
		LsyPlatform platform = LsyPlatform.Android;
		#endregion

		#region Global
		MaxSize sizeGlobal = MaxSize._8192;
		#endregion

		#region Local
		int overallSize = 0;
		List<LsyTextureInfo> textures = new List<LsyTextureInfo>();
		float columnWidth=100f;
		float buttonWidth=50f;
		Vector2 scrollViewPos;

		string searchText = "";
		List<LsyTextureInfo> texturesResult = new List<LsyTextureInfo>();
		private TexSortMethod texSortMethod = TexSortMethod.size;

        private bool bothSetting;

		#endregion


		[MenuItem("TARD/优化/TextureCompressor")]
		public static void Init()
		{
			LsyTextureManager s = (LsyTextureManager)EditorWindow.GetWindow(typeof(LsyTextureManager));
			s.Show ();
		}

		void Update()
		{
			Instance = this;
			//this.Repaint ();
		}

	
		void OnGUI()
		{
			titleContent.text = "贴图压缩系统";

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("模式",GUILayout.Width(columnWidth));
			LsyTextureImporter.Mode = (LsyTextureImporterMode)EditorGUILayout.EnumPopup ("",LsyTextureImporter.Mode,GUILayout.Width(columnWidth));
			GUILayout.EndHorizontal ();
			if (LsyTextureImporter.Mode != LsyTextureImporterMode.manual)
				return;
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("平台",GUILayout.Width(columnWidth));
			platform = (LsyPlatform)EditorGUILayout.EnumPopup ("",platform,GUILayout.Width(columnWidth));


            bothSetting=GUILayout.Toggle(bothSetting,"同时设置安卓IOS");


			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			ShowTitle ("查找贴图");
			if (GUILayout.Button ("",GUIStyle.none,GUILayout.Width(columnWidth))) {
				advancedMode = !advancedMode;
			}
			GUILayout.EndHorizontal ();
			OnGUI_Filter ();

			if (textures == null || textures.Count == 0)
				return;
			GUILayout.Space (30);
			ShowTitle ("全部设置");
			OnGUI_ProcessGlobal ();


			LocalHeader ();
			scrollViewPos = GUILayout.BeginScrollView (scrollViewPos);
			OnGUI_ProcessLocal ();
			GUILayout.EndScrollView ();
			GUILayout.Space (20);
		}

		void LocalHeader()
		{
			GUILayout.Space (30);
			ShowTitle ("逐个设置");

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("总大小",GUILayout.Width(buttonWidth));
			ShowSize (overallSize);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Space (20);
			string simbol = "";
			if (texSortMethod == TexSortMethod.name) {
				simbol = "▽";
				GUI.backgroundColor = highLightColor;
			} else if (texSortMethod == TexSortMethod.name_rev) {
				simbol = "△";
				GUI.backgroundColor = highLightColor;
			} else {
				simbol = "▽";
				GUI.backgroundColor = Color.white;
			}
			if (GUILayout.Button (simbol, GUILayout.Width (70))) {
				if (texSortMethod == TexSortMethod.name) {
					texSortMethod = TexSortMethod.name_rev;
				} else {
					texSortMethod = TexSortMethod.name;
				}
				LocalSort ();
			}
			GUILayout.Space (240);
			if (texSortMethod == TexSortMethod.size) {
				simbol = "▽";
				GUI.backgroundColor = highLightColor;
			} else if (texSortMethod == TexSortMethod.size_rev) {
				simbol = "△";
				GUI.backgroundColor = highLightColor;
			} else {
				simbol = "▽";
				GUI.backgroundColor = Color.white;
			}
			if (GUILayout.Button (simbol, GUILayout.Width (70))) {
				if (texSortMethod == TexSortMethod.size) {
					texSortMethod = TexSortMethod.size_rev;
				} else {
					texSortMethod = TexSortMethod.size;
				}
				LocalSort ();
			}
			GUI.backgroundColor = Color.white;
			GUILayout.Space (115);
			searchText = GUILayout.TextField(searchText,GUILayout.Width(200));
			if (GUILayout.Button ("Search", GUILayout.Width (70))) {
				LocalSearch ();
			}
			GUILayout.EndHorizontal ();
		}


		#region filter
		void OnGUI_Filter()
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("最大尺寸",GUILayout.Width(columnWidth));
			sizeFilter = (MaxSize)EditorGUILayout.EnumPopup ("",sizeFilter,GUILayout.Width(columnWidth));
			above = GUILayout.Toggle (above,"以上");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("搜索文件夹",GUILayout.Width(buttonWidth+20))) {
				Clear ();
				EditorUtility.DisplayProgressBar ("Searching", "Searching Textures", 0);
				Texture[] ts = Selection.GetFiltered<Texture> (SelectionMode.DeepAssets);
				ProcessAllTex (ts);
			}
			if (GUILayout.Button ("搜索场景",GUILayout.Width(buttonWidth+20))) {
				Clear ();
				EditorUtility.DisplayProgressBar ("Searching", "Searching Textures", 0);
				Texture[] ts = LsyTextureCommon.GetSceneTextures ();
				ProcessAllTex (ts);
			}
			if (GUILayout.Button ("清除",GUILayout.Width(buttonWidth))) {
				Clear ();
			}
			GUILayout.EndHorizontal ();
		}

		void ProcessAllTex(Texture[] ts)
		{
			EditorUtility.DisplayProgressBar ("Searching", "Searching Textures", 0.5f);
			for (int i = 0; i < ts.Length; i++) {
				EditorUtility.DisplayProgressBar ("Searching", "Searching Textures", 0.5f+ 0.5f*(float)i/ts.Length);
				Texture t = ts [i];
				if (t != null) {
					if (!LsyTextureCommon.HasTextureImporter (t))
						continue;

					var info = new LsyTextureInfo (t);
					var setting = info.GetPlatformSetting (platform.ToString ());
					if (above) {
						if (setting.maxTextureSize >= (int)sizeFilter)
							textures.Add (info);
					} else {
						if (setting.maxTextureSize <= (int)sizeFilter)
							textures.Add (info);
					}
				}
			}
			LocalSearch ();
			EditorUtility.ClearProgressBar ();
		}
		#endregion

		#region process
		void OnGUI_ProcessGlobal()
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("读写",GUILayout.Width(columnWidth));
			if (GUILayout.Button ("开",GUILayout.Width(buttonWidth))) {
				foreach (var t in texturesResult) {
					ReimportTexRW (t,true);
				}
			}
			if (GUILayout.Button ("关",GUILayout.Width(buttonWidth))) {
				foreach (var t in texturesResult) {
					ReimportTexRW (t,false);
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Mipmap",GUILayout.Width(columnWidth));
			if (GUILayout.Button ("开",GUILayout.Width(buttonWidth))) {
				foreach (var t in texturesResult) {
                    if (t.type == LsyTextureType.shadowmask)
                    {
                        ReimportTexMipmap(t, false);
                    }
                    else
                    {
                        ReimportTexMipmap(t, true);
                    }                        
				}
			}
			if (GUILayout.Button ("关",GUILayout.Width(buttonWidth))) {
				foreach (var t in texturesResult) {
					ReimportTexMipmap (t,false);
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("压缩品质",GUILayout.Width(columnWidth));
			List<string> str = new List<string> (){"低", "中", "高"};
			for (int i = 0; i < str.Count; i++) {
				//if (i == str.Count - 1 && !advancedMode)
					//continue;
				if (GUILayout.Button (str[i],GUILayout.Width(buttonWidth))) {
					foreach (var t in texturesResult) {
						ReimportTexQuality (t, i);
					}
				}
			}
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("最大尺寸",GUILayout.Width(columnWidth));
			sizeGlobal = (MaxSize)EditorGUILayout.EnumPopup ("",sizeGlobal,GUILayout.Width(columnWidth));
			if (GUILayout.Button ("执行",GUILayout.Width(buttonWidth))) {
				int size = (int)sizeGlobal;
				string p = platform.ToString ();
				foreach (var t in textures) {
					ReimportTexSize (t,p,size);
				}
			}
			GUILayout.EndHorizontal ();
		}

		void ShowSize(int size)
		{
			float memSize = size;
			memSize = memSize / 1024;

			if (memSize < 1024) {
				GUILayout.Label (string.Format ("{0}KB", memSize.ToString("f2")), GUILayout.Width (columnWidth));
			} else {
				memSize = memSize / 1024;
				GUILayout.Label (string.Format ("{0}MB", memSize.ToString("f2")), GUILayout.Width (columnWidth));
			}

		}
		void OnGUI_ProcessLocal()
		{
			float size = 20;
			overallSize = 0;
			foreach (var t in texturesResult) {
				var importer = t.importer;
				var pInfo = t.GetLsyTextureInfoPlatform(platform.ToString ());
				var settings = pInfo.setting;
				int quality = pInfo.quality;
				overallSize += pInfo.size;

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button (t.tex,GUIStyle.none, GUILayout.Width (size), GUILayout.Height (size))) {
					Selection.activeObject = t.tex;
				}
				GUILayout.Label (t.tex.name,GUILayout.Width(columnWidth*2));
				GUILayout.Label (string.Format("W:{0} H:{1}",pInfo.width,pInfo.height),GUILayout.Width(columnWidth));
			
				ShowSize (pInfo.size);

				GUILayout.Label ("读写",GUILayout.Width(28));
				importer.isReadable = GUILayout.Toggle (importer.isReadable, "",GUILayout.Width(28));
				GUILayout.Label ("Mipmap",GUILayout.Width(50));
				importer.mipmapEnabled = GUILayout.Toggle (importer.mipmapEnabled, "",GUILayout.Width(28));

				var mSize = (MaxSize)settings.maxTextureSize;
				var mSizeNew = (MaxSize)EditorGUILayout.EnumPopup ("",mSize,GUILayout.Width(buttonWidth)); 
				if (mSizeNew != mSize) {
					settings.overridden = true;
					settings.maxTextureSize = (int)mSizeNew;
					importer.SetPlatformTextureSettings (settings);
					ReimportTex (t);
				}

				GUILayout.Label ("压缩品质",GUILayout.Width(buttonWidth));
				List<string> str = new List<string> (){"低", "中", "高"};
				for (int i = 0; i < str.Count; i++) {
					//if (i == str.Count - 1 && !advancedMode)
					//	continue;

					if (i == quality)
						GUI.backgroundColor = highLightColor;
					if (GUILayout.Button (str[i],GUILayout.Width(buttonWidth))) {
						ReimportTexQuality (t, i);
					}
					if (i == quality)
						GUI.backgroundColor = Color.white;
				}
				GUILayout.EndHorizontal ();
			}
		}

		void ReimportTexQuality(LsyTextureInfo t,int q)
		{

            //---------------------------------------------
            //同时设置双平台
            //---------------------------------------------
            if (bothSetting)
            {
                var setting1 = LsyTextureCommon.Create_Setting(t, "iPhone", q);
                LsyTextureImporter.ManualSetting = setting1;

                t.importer.SetPlatformTextureSettings(setting1);

                if (t.type == LsyTextureType.shadowmask)
                {
                    t.importer.mipmapEnabled = false;
                }

                ReimportTex(t);
                LsyTextureImporter.ManualSetting = null;


                var setting2 = LsyTextureCommon.Create_Setting(t, "Android", q);
                LsyTextureImporter.ManualSetting = setting2;

                t.importer.SetPlatformTextureSettings(setting2);

                if (t.type == LsyTextureType.shadowmask)
                {
                    t.importer.mipmapEnabled = false;
                }

                ReimportTex(t);
                LsyTextureImporter.ManualSetting = null;
            }


			var setting = LsyTextureCommon.Create_Setting (t,platform.ToString(), q);
			LsyTextureImporter.ManualSetting = setting;
            
			t.importer.SetPlatformTextureSettings (setting);

            if (t.type == LsyTextureType.shadowmask)
            {
                t.importer.mipmapEnabled = false;
            }

            ReimportTex (t);
			LsyTextureImporter.ManualSetting = null;
		}
		void ReimportTexRW(LsyTextureInfo t,bool rw)
		{
			t.importer.isReadable = rw;
			ReimportTex (t);
		}
		void ReimportTexMipmap(LsyTextureInfo t,bool mipmapEnabled)
		{
			t.importer.mipmapEnabled = mipmapEnabled;
            //if (t.type==LsyTextureType.shadowmask)
            //{
            //    mipmapEnabled = false;
            //    t.importer.mipmapEnabled = mipmapEnabled;
            //}
			ReimportTex (t);
		}
		void ReimportTexSize(LsyTextureInfo t,string plat,int size)
		{
			var setting = t.GetPlatformSetting (plat);
			setting.maxTextureSize = size;
			t.importer.SetPlatformTextureSettings (setting);
			ReimportTex (t);
		}

		void ReimportTex(LsyTextureInfo t)
		{
			AssetDatabase.ImportAsset (t.assetPath);
			t.Refresh ();
		}
		#endregion


		#region Advance Browse
		void LocalSearch()
		{
			texturesResult.Clear ();
			if (string.IsNullOrEmpty (searchText)) {
				texturesResult.AddRange (textures);
			} else {
				
				foreach (var item in textures) {
					if (item.tex.name.IndexOf (searchText,System.StringComparison.OrdinalIgnoreCase)>=0) {
						texturesResult.Add (item);
					}
				}
			}
			LocalSort ();
		}
		void LocalSort()
		{
			SortByMethod (texSortMethod);
		}

		void SortByMethod(TexSortMethod method)
		{
			if (method == TexSortMethod.size) {
				texturesResult.Sort (delegate(LsyTextureInfo x, LsyTextureInfo y) {
					int sizeA = x.GetLsyTextureInfoPlatform (platform.ToString ()).size;
					int sizeB = y.GetLsyTextureInfoPlatform (platform.ToString ()).size;
					return sizeB - sizeA;	
				});
			}
			if (method == TexSortMethod.size_rev) {
				texturesResult.Sort (delegate(LsyTextureInfo x, LsyTextureInfo y) {
					int sizeA = x.GetLsyTextureInfoPlatform (platform.ToString ()).size;
					int sizeB = y.GetLsyTextureInfoPlatform (platform.ToString ()).size;
					return sizeA - sizeB;	
				});
			}

			if (method == TexSortMethod.name) {
				texturesResult.Sort (delegate(LsyTextureInfo x, LsyTextureInfo y) {
					string nameA = x.tex.name;
					string nameB = y.tex.name;
					return nameA.CompareTo(nameB);	
				});
			}
			if (method == TexSortMethod.name_rev) {
				texturesResult.Sort (delegate(LsyTextureInfo x, LsyTextureInfo y) {
					string nameA = x.tex.name;
					string nameB = y.tex.name;
					return nameB.CompareTo(nameA);	
				});
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
		void Clear()
		{
			textures.Clear ();
		}

		#endregion
	}
}