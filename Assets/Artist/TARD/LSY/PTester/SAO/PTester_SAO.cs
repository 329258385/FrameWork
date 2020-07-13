using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;
using UnityEngine.Rendering;

public class PTester_SAO :PTester {
	#region On Off
	public static PTester_SAO Instance;
	public static void Show(bool show)
	{
		if (Instance == null) {
			GameObject go = new GameObject ("PTester_SAO");
			Instance = go.AddComponent<PTester_SAO> ();
		}

		Instance.enabled = show;
	}
		
	#endregion
	public List<GameObject> hides;

	public GameObject rootGrass;
	public GameObject rootFX;
	public GameObject rootWater;
	public Light mainLight;

	public Shader shaderUnlit;
	public Shader shaderDiffuse;
	public Shader shaderStandard;

	public Transform charRoot;
	public GameObject charPrefab;
	public GameObject chara;

	public GameObject uiRoot;
	public GameObject uiRoot_Backpack;

	public Transform target;
	public float followH;
	public float followV;

    public RenderTexture InteractiveGrassTexture;
    public Material InteractiveGrassMat;
    public Mesh instanceMesh;
    public Material IMLOD0;
    public Material IMLOD1;
    public Material IMLOD2;



    protected override void Init ()
	{
		#region On Off
		Instance = this;

		#if BUILD_SINGLESCNE_MODE
		foreach(var item in hides)
		{
		item.SetActive(false);
		}
		#else
		enabled = false;
		#endif
			
		#endregion


		base.Init ();
		gameObject.AddComponent<TimeConsume> ();

		testGroup1_title = "设备";
		testGroup2_title = "品质";
		testGroup3_title = "渲染";
		testGroup4_title = "美术";
	}

	protected override void MainTitle ()
	{
		base.MainTitle ();
		if (uiRoot == null)
			return;
		if (uiRoot.activeSelf) {
			if (GUILayout.Button ("隐藏UI", GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
				uiRoot.SetActive (false);
			}
		} else {
			if (GUILayout.Button ("显示UI", GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
				uiRoot.SetActive (true);
			}
		}
		if (uiRoot_Backpack.activeSelf) {
			if (GUILayout.Button ("隐藏背包", GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
				uiRoot_Backpack.SetActive (false);
			}
		} else {
			if (GUILayout.Button ("显示背包", GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
				uiRoot_Backpack.SetActive (true);
			}
		}
	}
	protected override void OnGUI_Mission1 ()
	{
		base.OnGUI_Mission1 ();
		string format = "{0}:{1}";
		GUILayout.Label (string.Format(format,"batteryLevel",SystemInfo.batteryLevel));
		GUILayout.Label (string.Format(format,"batteryStatus",SystemInfo.batteryStatus));
		GUILayout.Label (string.Format(format,"deviceModel",SystemInfo.deviceModel));
		GUILayout.Label (string.Format(format,"graphicsDeviceName",SystemInfo.graphicsDeviceName));
		GUILayout.Label (string.Format(format,"graphicsDeviceType",SystemInfo.graphicsDeviceType));
		GUILayout.Label (string.Format(format,"graphicsDeviceVendor",SystemInfo.graphicsDeviceVendor));
		GUILayout.Label (string.Format(format,"graphicsDeviceVersion",SystemInfo.graphicsDeviceVersion));
		GUILayout.Label (string.Format(format,"graphicsMemorySize",SystemInfo.graphicsMemorySize));
		GUILayout.Label (string.Format(format,"graphicsShaderLevel",SystemInfo.graphicsShaderLevel));
		GUILayout.Label (string.Format(format,"maxTextureSize",SystemInfo.maxTextureSize));
		GUILayout.Label (string.Format(format,"processorCount",SystemInfo.processorCount));
		GUILayout.Label (string.Format(format,"processorFrequency",SystemInfo.processorFrequency));
		GUILayout.Label (string.Format(format,"processorType",SystemInfo.processorType));
		GUILayout.Label (string.Format(format,"supportedRenderTargetCount",SystemInfo.supportedRenderTargetCount));
		GUILayout.Label (string.Format(format,"systemMemorySize",SystemInfo.systemMemorySize));

		OnGUI_Bloom ();
	}

	protected void OnGUI_Bloom ()
	{
		Bloom b = Camera.main.GetComponent<Bloom> ();
		if (b == null)
			return;

		TesterGroup ("BloomEpsilon",
			"0", delegate() {BloomRunTimeModifier.BloomEpsilon = 0f;},
			"0.5", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.5f;},
			"0.4", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.4f;},
			"0.3", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.3f;}
		);
		TesterGroup ("",
			"0.2", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.2f;},
			"0.1", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.1f;},
			"0.08", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.08f;},
			"0.05", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.05f;}
		);
		TesterGroup ("",
			"0.03", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.03f;},
			"0.02", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.02f;},
			"0.01", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.01f;},
			"0.009", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.009f;}
		);
		TesterGroup ("",
			"0.008", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.008f;},
			"0.007", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.007f;},
			"0.006", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.006f;},
			"0.005", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.005f;}
		);
		TesterGroup ("",
			"0.004", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.004f;},
			"0.003", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.003f;},
			"0.002", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.002f;},
			"0.001", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.001f;}
		);
		TesterGroup ("",
			"0.0008", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.0008f;},
			"0.0005", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.0005f;},
			"0.0003", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.0003f;},
			"0.0001", delegate() {BloomRunTimeModifier.BloomEpsilon = 0.0001f;}
		);
			

		TesterGroup ("Lock",
			"false", delegate() {b.lockParameters = false;},
			"true", delegate() {b.lockParameters = true;}
		);
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("threshold:"+b.thresholdGamma,GUILayout.Width(UIWidth*2));
		b.thresholdGamma = GUILayout.HorizontalSlider (b.thresholdGamma, 0, 2);
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("softKnee:"+b.softKnee,GUILayout.Width(UIWidth*2));
		b.softKnee = GUILayout.HorizontalSlider (b.softKnee, 0, 1);
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("intensity:"+b.intensity,GUILayout.Width(UIWidth*2));
		b.intensity = GUILayout.HorizontalSlider (b.intensity, 0, 10);
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("radius:"+b.radius,GUILayout.Width(UIWidth*2));
		b.radius = GUILayout.HorizontalSlider (b.radius, 0, 7);
		GUILayout.EndHorizontal ();

		TesterGroup ("antiFlicker:",
			"false", delegate() {b.antiFlicker = false;},
			"true", delegate() {b.antiFlicker = true;}
		);

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("ex:"+b.ex,GUILayout.Width(UIWidth*2));
		b.ex = GUILayout.HorizontalSlider (b.ex, 0, 1);
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("ex2:"+b.ex2,GUILayout.Width(UIWidth*2));
		b.ex2 = GUILayout.HorizontalSlider (b.ex2, 0, 3);
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("_SoftFocus:"+b._SoftFocus,GUILayout.Width(UIWidth*2));
		b._SoftFocus = GUILayout.HorizontalSlider (b._SoftFocus, 0, 0.3f);
		GUILayout.EndHorizontal ();
	}

	protected override void OnGUI_Mission2 ()
	{
		base.OnGUI_Mission2 ();
		TesterGroup ("品质","低", delegate() {
			TARDSwitches.SetQuality(0);
		}, "中", delegate() {
			TARDSwitches.SetQuality(1);
		}, "高", delegate() {
			TARDSwitches.SetQuality(2);
		});


		TesterGroup ("TextureQuality",
			"Quarter", delegate() {QualitySettings.masterTextureLimit = 2;},
			"Half", delegate() {QualitySettings.masterTextureLimit = 1;},
			"Full", delegate() {QualitySettings.masterTextureLimit = 0;}
		);

		TesterGroup ("Aniso",
			"Disable", delegate() {QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;},
			"PerTex", delegate() {QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;},
			"ForcedOn", delegate() {QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;}
		);

		TesterGroup ("AA",
			"Disable", delegate() {QualitySettings.antiAliasing = 0;},
			"2x", delegate() {QualitySettings.antiAliasing = 2;},
			"4x", delegate() {QualitySettings.antiAliasing = 4;},
			"8x", delegate() {QualitySettings.antiAliasing = 8;}
		);

		TesterGroup ("SoftParticles",
			"false", delegate() {QualitySettings.softParticles = false;},
			"true", delegate() {QualitySettings.softParticles = true;}
		);


		TesterGroup ("VSync",
			"Dont", delegate() {QualitySettings.vSyncCount = 0;},
			"EveryVBlank", delegate() {QualitySettings.vSyncCount = 1;},
			"EverySecondVBlank", delegate() {QualitySettings.vSyncCount = 2;}
		);


		if (mainLight != null) {
			TesterGroup ("阴影",
				"关", delegate() {
					mainLight.shadows = LightShadows.None;
				},
				"硬", delegate() {
					mainLight.shadows = LightShadows.Hard;
				},
				"软", delegate() {
					mainLight.shadows = LightShadows.Soft;
				}
			);
		}
		TesterGroup ("ShadowResolution",
			"Low", delegate() {QualitySettings.shadowResolution = ShadowResolution.Low;},
			"Med", delegate() {QualitySettings.shadowResolution = ShadowResolution.Medium;},
			"High", delegate() {QualitySettings.shadowResolution = ShadowResolution.High;}
		);
		TesterGroup ("ShadowDistance",
			"10", delegate() {QualitySettings.shadowDistance = 10;},
			"30", delegate() {QualitySettings.shadowDistance = 30;},
			"50", delegate() {QualitySettings.shadowDistance = 50;},
			"100", delegate() {QualitySettings.shadowDistance = 100;}
		);
		TesterGroup ("ShadowCaster",
			"Off", delegate() {SetShadowCasters(ShadowCastingMode.Off);},
			"On", delegate() {SetShadowCasters(ShadowCastingMode.On);}
		);
	}

	protected override void OnGUI_Mission3 ()
	{
		base.OnGUI_Mission3 ();

		TesterGroup ("分辨率",
			"0.3", delegate() {TARDSwitches.SetScreenF(0.3f);},
			"0.4", delegate() {TARDSwitches.SetScreenF(0.4f);},
			"0.5", delegate() {TARDSwitches.SetScreenF(0.5f);},
			"0.6", delegate() {TARDSwitches.SetScreenF(0.6f);}
		);
		TesterGroup ("",
			"0.7", delegate() {TARDSwitches.SetScreenF(0.7f);},
			"0.8", delegate() {TARDSwitches.SetScreenF(0.8f);},
			"0.9", delegate() {TARDSwitches.SetScreenF(0.9f);},
			"1", delegate() {TARDSwitches.SetScreenF(1f);}
		);

		TesterGroup ("目标FPS",
			"30", delegate() {Application.targetFrameRate = 30;},
			"60", delegate() {Application.targetFrameRate = 60;}
		);
			

		if (shaderUnlit != null) {
			TesterGroup ("材质",
				"Unlit", delegate() {
				PTester_SetShader.SetShader (shaderUnlit);
			},
				"还原", delegate() {
				PTester_SetShader.RevertShader ();
			},
				"Diffuse", delegate() {
				PTester_SetShader.SetShader (shaderDiffuse);
			},
				"Standard", delegate() {
				PTester_SetShader.SetShader (shaderStandard);
			}
			);
		}

		CameraMonos ();
	}

	protected override void OnGUI_Mission4 ()
	{
		base.OnGUI_Mission4 ();
		TesterGroup ("LOD距离",
			"0", delegate() { QualitySettings.lodBias = 0f;},
			"0.3", delegate() {QualitySettings.lodBias = 0.3f;},
			"1", delegate() {QualitySettings.lodBias = 1f;}
		);

		var g = GrassRenderer.Instance;

		if (rootGrass != null) {
			TesterGroup ("草",
				"关闭", delegate() {
				rootGrass.SetActive (false);
			},
				"40", delegate() {
				rootGrass.SetActive (true);
				g.settings.SetDis (40);
				g.settings.lodDis = new float[]{ 10, 20, 30 };
				DitherCrossFadeManager._FadeNear = 22;
				DitherCrossFadeManager._FadeFar = 27;
			},
				"50", delegate() {
				rootGrass.SetActive (true);
				g.settings.SetDis (50);
				g.settings.lodDis = new float[]{ 15, 25, 35 };
				DitherCrossFadeManager._FadeNear = 35;
				DitherCrossFadeManager._FadeFar = 40;
			},
				"60", delegate() {
				rootGrass.SetActive (true);
				g.settings.SetDis (60);
				g.settings.lodDis = new float[]{ 20, 30, 40 };
				DitherCrossFadeManager._FadeNear = 40;
				DitherCrossFadeManager._FadeFar = 45;
			}
			);
		}

		if (rootWater != null) {
			TesterGroup ("水",
				"关闭", delegate() {
				rootWater.SetActive (false);
			},
				"打开", delegate() {
				rootWater.SetActive (true);
			}
			);
		}


		if (rootFX != null) {
			TesterGroup ("特效",
				"关闭", delegate() {
				rootFX.SetActive (false);
			},
				"打开", delegate() {
				rootFX.SetActive (true);
			}
			);
		}
		TesterGroup ("粒子系统",
			"关闭", delegate() {SetParticleSystem(false);},
			"打开", delegate() {SetParticleSystem(true);}
		);


		if (charRoot != null) {
			TesterGroup ("角色",
				"创建", delegate() {
				Char_Create ();
			}
			);
			TesterGroup ("角色 Outline",
				"0.015", delegate() {
				Char_SetOutline (0.015f);
			},
				"0.03", delegate() {
				Char_SetOutline (0.03f);
			},
				"0.04", delegate() {
				Char_SetOutline (0.04f);
			},
				"0.05", delegate() {
				Char_SetOutline (0.05f);
			}
			);
		}

		GUILayout.BeginHorizontal ();
		GUILayout.Space (UIWidth+7);
		if (GUILayout.Button ("^", GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
			Camera.main.transform.localEulerAngles += new Vector3 (-5, 0, 0);
            Camera.main.transform.localPosition += new Vector3(0, 0, 30f);
        }
		GUILayout.EndHorizontal ();
		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("<", GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
			Camera.main.transform.localEulerAngles += new Vector3 (0, -5, 0);
            Camera.main.transform.localPosition += new Vector3(-30f, 0, 0);
        }
		if (GUILayout.Button ("v", GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
			Camera.main.transform.localEulerAngles += new Vector3 (5, 0, 0);
            Camera.main.transform.localPosition += new Vector3(0, 0f, 30f);
        }
		if (GUILayout.Button (">", GUILayout.Width (UIWidth), GUILayout.Height (UIHeight))) {
			Camera.main.transform.localEulerAngles += new Vector3 (0, 5, 0);
            Camera.main.transform.localPosition += new Vector3(30f, 0, 0f);
        }
		GUILayout.EndHorizontal ();

        if (!Camera.main.GetComponent<TJiaGrassGenerator>())
        {
            TJiaGrassGenerator tgg= Camera.main.gameObject.AddComponent<TJiaGrassGenerator>();
            tgg.InteractiveGrassTexture = InteractiveGrassTexture;
            tgg.InteractiveGrassMat = InteractiveGrassMat;
            tgg.IMs = new TJiaGrassGenerator.InstanceModule[3];
            tgg.IMs[0].Density = 3;
            tgg.IMs[0].StartRadias = 0;
            tgg.IMs[0].Thickness = 60;            
            tgg.IMs[0].InstanceMesh = instanceMesh;
            tgg.IMs[0].InstanceMaterial = IMLOD0;
            //tgg.IMs[0].Dither = TJiaGrassGenerator.InstanceModule.DitherMode.Off;

            tgg.IMs[0].Density = 3;
            tgg.IMs[0].StartRadias = 60;
            tgg.IMs[0].Thickness = 70;
            tgg.IMs[0].InstanceMesh = instanceMesh;
            tgg.IMs[0].InstanceMaterial = IMLOD1;
            //tgg.IMs[0].Dither = TJiaGrassGenerator.InstanceModule.DitherMode.Off;

            tgg.IMs[0].Density = 2;
            tgg.IMs[0].StartRadias = 130;
            tgg.IMs[0].Thickness = 160;
            tgg.IMs[0].InstanceMesh = instanceMesh;
            tgg.IMs[0].InstanceMaterial = IMLOD2;
            //tgg.IMs[0].Dither = TJiaGrassGenerator.InstanceModule.DitherMode.OneToZero;
        }


        if (Camera.main.GetComponent<TJiaGrassGenerator>())
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(UIWidth + 7);
            if (GUILayout.Button("^", GUILayout.Width(UIWidth), GUILayout.Height(UIHeight)))
            {
                Camera.main.transform.localPosition += new Vector3(0, 0, 20f);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<", GUILayout.Width(UIWidth), GUILayout.Height(UIHeight)))
            {
                Camera.main.transform.localPosition += new Vector3(-20f, 0, 0);
            }
            if (GUILayout.Button("v", GUILayout.Width(UIWidth), GUILayout.Height(UIHeight)))
            {
                Camera.main.transform.localPosition += new Vector3(0, 0f, 20f);
            }
            if (GUILayout.Button(">", GUILayout.Width(UIWidth), GUILayout.Height(UIHeight)))
            {
                Camera.main.transform.localPosition += new Vector3(20f, 0, 0f);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("TGG关", GUILayout.Width(UIWidth), GUILayout.Height(UIHeight)))
            {
                Camera.main.GetComponent<TJiaGrassGenerator>().enabled = false;
            }
            if (GUILayout.Button("TGG开", GUILayout.Width(UIWidth), GUILayout.Height(UIHeight)))
            {
                Camera.main.GetComponent<TJiaGrassGenerator>().enabled = true;
            }
            GUILayout.EndHorizontal();
        }

        if (Camera.main.GetComponent<TJiaGrassGenerator>())
        {
            if (GUILayout.Button("TGG:"+Camera.main.GetComponent<TJiaGrassGenerator>().enabled.ToString(), GUILayout.Width(UIWidth), GUILayout.Height(UIHeight)))
            {

            }
        }


        //if (target!=null)
		//	Camera.main.transform.position = target.position - target.forward * followH + Vector3.up*followV;

		var c = Camera.main.GetComponentInChildren<GameCamera> ();
		if (c != null) {
			Destroy (c);
		}
		Camera.main.allowMSAA = true;
	}





	#region Camera monos
	void CameraMonos()
	{
		TesterGroup ("相机距离",
			"0", delegate() {Camera.main.farClipPlane = 0;},
			"200", delegate() {Camera.main.farClipPlane = 200;},
			"250", delegate() {Camera.main.farClipPlane = 250;},
			"300", delegate() {Camera.main.farClipPlane = 300;}
		);
		TesterGroup ("",
			"350", delegate() {Camera.main.farClipPlane = 350;},
			"400", delegate() {Camera.main.farClipPlane = 400;},
			"500", delegate() {Camera.main.farClipPlane = 500;},
			"850", delegate() {Camera.main.farClipPlane = 850;}
		);

		TesterGroup ("相机OC",
			"Off", delegate() {Camera.main.useOcclusionCulling = false;},
			"On", delegate() {Camera.main.useOcclusionCulling = true;}
		);


		TesterGroup ("相机脚本",
			"全部关闭", delegate() {SetCamMono(false);},
			"全部打开", delegate() {SetCamMono(true);}
		);
		TesterGroup ("相机HDR",
			"关闭", delegate() {SetCameraHDR(false);},
			"打开", delegate() {SetCameraHDR(true);}
		);
		TesterGroup ("相机Dynamic Reso",
			"关闭", delegate() {SetCameraDynamicResolution(false);},
			"打开", delegate() {SetCameraDynamicResolution(true);}
		);

		TesterGroup ("Bloom",
			"关闭", delegate() {Bloom.SetQualityAll(0);},
			"打开", delegate() {Bloom.SetQualityAll(1);}
		);

		TesterGroup ("RT_Type",
			"Clear", delegate() {Bloom.RT_AllClear();},
			"ClearColor", delegate() {Bloom.RT_AllClearColor();},
			"New", delegate() {Bloom.RT_Type = BloomRTType.New;},
			"Temp", delegate() {Bloom.RT_Type = BloomRTType.Temp;}
		);
		TesterGroup ("",
			"0.2", delegate() {Bloom.SetThreshold(0.2f);},
			"0.5", delegate() {Bloom.SetThreshold(0.5f);},
			"0.9", delegate() {Bloom.SetThreshold(0.9f);}
		);

		TesterGroup ("Bloom RT Memoryless",
			"Color", delegate() {Bloom.RT_MemoryMode = RenderTextureMemoryless.Color;},
			"Depth", delegate() {Bloom.RT_MemoryMode = RenderTextureMemoryless.Depth;},
			"MSAA", delegate() {Bloom.RT_MemoryMode = RenderTextureMemoryless.MSAA;},
			"None", delegate() {Bloom.RT_MemoryMode = RenderTextureMemoryless.None;}
		);
		TesterGroup ("Bloom Depth",
			"0", delegate() {Bloom.RT_Depth = 0;},
			"1", delegate() {Bloom.RT_Depth = 1;},
			"2", delegate() {Bloom.RT_Depth = 2;}
		);
		TesterGroup ("Bloom AA",
			"1", delegate() {Bloom.RT_AA = 1;},
			"2", delegate() {Bloom.RT_AA = 2;},
			"4", delegate() {Bloom.RT_AA = 4;},
			"8", delegate() {Bloom.RT_AA = 8;}
		);
		TesterGroup ("Bloom RW",
			"Default", delegate() {Bloom.RT_RW = RenderTextureReadWrite.Default;},
			"Linear", delegate() {Bloom.RT_RW = RenderTextureReadWrite.Linear;},
			"sRGB", delegate() {Bloom.RT_RW = RenderTextureReadWrite.sRGB;}
		);

		TesterGroup ("CameraFX",
			"关闭", delegate() {SetCamCameraFX(false);},
			"打开", delegate() {SetCamCameraFX(true);}

		);

		TesterGroup ("DBPControler",
			"关闭", delegate() {SetCamDBPControler(false);},
			"打开", delegate() {SetCamDBPControler(true);}
		);

		TesterGroup ("RenderingController",
			"关闭", delegate() {SetCamRenderingController(false);},
			"打开", delegate() {SetCamRenderingController(true);}
		);
	}
		
	void SetCamMono(bool _enable)
	{
		Camera cam = Camera.main;

		foreach (var mono in cam.GetComponents<MonoBehaviour>()) {
			mono.enabled = _enable;
		}
	}

	void SetCamCameraFX(bool _enable)
	{
		Camera cam = Camera.main;
		cam.GetComponent<CameraFX> ().enabled = _enable;
	}
	void SetCamDBPControler(bool _enable)
	{
		Camera cam = Camera.main;
		cam.GetComponent<DBP_Controler> ().enabled = _enable;
	}
	void SetCamRenderingController(bool _enable)
	{
		Camera cam = Camera.main;
		cam.GetComponent<RenderingController> ().enabled = _enable;
	}
	#endregion

	ParticleSystem[] pss;
	void SetParticleSystem(bool _enable)
	{
		if (pss == null) {
			pss = Object.FindObjectsOfType<ParticleSystem> ();
		}

		foreach (var item in pss) {
			item.gameObject.SetActive (_enable);
		}
	}

	void Char_Create()
	{
		if (chara == null) {
			chara = GameObject.Instantiate (charPrefab) as GameObject;
			chara.transform.parent = charRoot;
			chara.transform.localPosition = Vector3.zero;
			chara.transform.localRotation = Quaternion.identity;
			chara.transform.localScale = Vector3.one;
			chara.GetComponentInChildren<FaceBlender> ().UpdateFace = true;
		}
	}
	void Char_SetOutline(float w)
	{
		foreach (var smr in chara.GetComponentsInChildren<SkinnedMeshRenderer>()) {
			if(smr.name.Contains("hair"))
				smr.material.SetFloat ("_OutlineWidth", w*0.5f);
			else
				smr.material.SetFloat ("_OutlineWidth", w);
		}
	}

	void SetCameraHDR(bool on)
	{
		if (Camera.main == null)
			return;
		Camera.main.allowHDR = on;
	}
	void SetCameraDynamicResolution(bool on)
	{
		if (Camera.main == null)
			return;
		Camera.main.allowDynamicResolution = on;
	}

	void SetShadowCasters(ShadowCastingMode mode)
	{
		var obj = Object.FindObjectsOfType<MeshRenderer> ();
		foreach (var item in obj) {
			item.shadowCastingMode = mode;
		}
	}
}
