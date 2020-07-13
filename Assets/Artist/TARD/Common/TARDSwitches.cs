using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class TARDSwitches{
	/// <summary>
	/// 0:Low
	/// 1:Med
	/// 2:High
	/// </summary>
	public static int Quality = 1;
	/// <summary>
	/// 0:Low
	/// 1:Med
	/// 2:High
	/// </summary>
	public static void SetQuality(int _Quality)
	{
		Quality = _Quality;
		//SetQuality_Settings ();

        ReSettingRelativeQualityWhenSceneLoad(Quality);

        //Lsy
        /*GrassFP_Manager.Instance.SetQuality(Quality);
		Kino.Bloom.SetQualityAll (Quality);*/
        //LsyBlur.SetQualityAll (Quality);
		//GrassSkillFX.useRaycast = Quality > 0;
		//SetMSAA (Camera.main);// 为了保证正确性, 通过lua那边每次切换场景来操作
		//Zzc
	}

    //
    // 当场景重新加载的时候，我们希望能再次刷新那些与品质相关的设置
    //
    public static void ReSettingRelativeQualityWhenSceneLoad(int intQuality)
    {
        //Jia
        switch (intQuality)
        {
            case 0:
                foreach (CameraFX cfx in GameObject.FindObjectsOfType<CameraFX>())
                {
                    TJiaGrassGenerator TGG = cfx.GetComponent<TJiaGrassGenerator>();
                    //TJiaBloom TB = cfx.GetComponent<TJiaBloom>();
                    if (TGG != null)
                    {
                        if (TGG.IMs.Length == 3)
                        {
                            TGG.IMs[0].Density = GlobalGameDefine.intGrassDensityLevel0;
                            TGG.IMs[1].Density = GlobalGameDefine.intGrassDensityLevel1;
                            TGG.IMs[2].Density = GlobalGameDefine.intGrassDensityLevel2;
                            TGG.IMs[0].StartRadias = 0;
                            TGG.IMs[0].Thickness = 45;
                            TGG.IMs[1].StartRadias = 35;
                            TGG.IMs[1].Thickness = 25;
                            TGG.IMs[2].StartRadias = 55;
                            TGG.IMs[2].Thickness = 88;
                        }
                    }
                    //if (TB != null)
                    //{
                    //    TB.enabled = false;
                    //}

                    // hw special setting
                    //if (GlobalGameDefine.mIsHWMate30BossDevice)
                    //{
                    //    cfx.enabled = true;
                    //    cfx.OpenShadow = true;
                    //    cfx.AA = false;
                    //}
                    //else
                    //{
                    //    cfx.OpenShadow = false;
                    //    cfx.DisableCFX();
                    //}
                }
                break;
            case 1:
                foreach (CameraFX cfx in GameObject.FindObjectsOfType<CameraFX>())
                {
                    TJiaGrassGenerator TGG = cfx.GetComponent<TJiaGrassGenerator>();
                    TJiaBloom TB = cfx.GetComponent<TJiaBloom>();
                    if (TGG != null)
                    {
                        if (TGG.IMs.Length == 3)
                        {
                            //TGG.IMs[0].Density = 2;
                            //TGG.IMs[1].Density = 2;
                            //TGG.IMs[2].Density = 1;
                            //TGG.IMs[0].StartRadias = 0;
                            //TGG.IMs[0].Thickness = 50;
                            //TGG.IMs[1].StartRadias = 40;
                            //TGG.IMs[1].Thickness = 40;
                            //TGG.IMs[2].StartRadias = 70;
                            //TGG.IMs[2].Thickness = 88;
                            TGG.IMs[0].Density = GlobalGameDefine.intGrassDensityLevel0;
                            TGG.IMs[1].Density = GlobalGameDefine.intGrassDensityLevel1;
                            TGG.IMs[2].Density = GlobalGameDefine.intGrassDensityLevel2;
                            TGG.IMs[0].StartRadias = 0;
                            TGG.IMs[0].Thickness = 59;
                            TGG.IMs[1].StartRadias = 46;
                            TGG.IMs[1].Thickness = 40;
                            TGG.IMs[2].StartRadias = 78;
                            TGG.IMs[2].Thickness = 88;
                        }
                    }
                    //if (TB != null)
                    //{
                    //    if(GlobalGameDefine.mIsForceDisableAnyBloom)
                    //    {
                    //        TB.enabled = false;
                    //    }
                    //    else
                    //    {
                    //        if (GlobalGameDefine.mIsHWDevice) 
                    //        {
                    //            TB.enabled = !GlobalGameDefine.mIsForceCloseBloomForHW;
                    //        }
                    //        else
                    //        {
                    //            TB.enabled = true;
                    //        }
                    //    }
                    //}
                    //cfx.enabled = true;
                    //cfx.OpenShadow = true;
                    //cfx.AA = false;
                }
                break;
            case 2:
                foreach (CameraFX cfx in GameObject.FindObjectsOfType<CameraFX>())
                {
                    TJiaGrassGenerator TGG = cfx.GetComponent<TJiaGrassGenerator>();
                    TJiaBloom TB = cfx.GetComponent<TJiaBloom>();
                    if (TGG != null)
                    {
                        if (TGG.IMs.Length == 3)
                        {
                            TGG.IMs[0].Density = GlobalGameDefine.intGrassDensityLevel0;
                            TGG.IMs[1].Density = GlobalGameDefine.intGrassDensityLevel1;
                            TGG.IMs[2].Density = GlobalGameDefine.intGrassDensityLevel2;
                            TGG.IMs[0].StartRadias = 0;
                            TGG.IMs[0].Thickness = 59;
                            TGG.IMs[1].StartRadias = 46;
                            TGG.IMs[1].Thickness = 40;
                            TGG.IMs[2].StartRadias = 78;
                            TGG.IMs[2].Thickness = 88;
                        }
                    }
                    //if (TB != null)
                    //{
                    //    if(GlobalGameDefine.mIsForceDisableAnyBloom)
                    //    {
                    //        TB.enabled = false;
                    //    }
                    //    else
                    //    {
                    //        if (GlobalGameDefine.mIsHWDevice) 
                    //        {
                    //            TB.enabled = !GlobalGameDefine.mIsForceCloseBloomForHW;
                    //        }
                    //        else
                    //        {
                    //            TB.enabled = true;
                    //        }
                    //    }
                    //}
                    //cfx.enabled = true;
                    //cfx.OpenShadow = true;
                    //cfx.AA = true;
                }
                break;
        }

        LsyBlur.SetQualityAll(intQuality);
    }

	public static void SetMSAA(Camera cam)
	{
		if (cam == null)
			return;
		cam.allowMSAA = true;
	}

    public static void SetScreen(float scale)
    {
        // TODO
    }

    /// <summary>
    /// The instancing grass Switch.
    /// </summary>
    public static bool GrassSwitch = true;
    public static bool CloudSwitch = true;

	public static bool BloomSwitch = true;
	public static float BloomThreshold = -1;
	public static float BloomIntensity = -1;

	public static bool ObjSpaceFx{get{return CameraFX.ObjSpaceFx;} set{CameraFX.ObjSpaceFx = value;}}
}
