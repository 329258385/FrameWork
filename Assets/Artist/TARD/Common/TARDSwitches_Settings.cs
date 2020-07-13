using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class TARDSwitches{
	public static int[] qualityLevel = { 0, 3, 4 };
	public static float[] shadowDistance = { 0, 20, 40 };
	public static float[] resolutionFactor = { 0.5f, 0.6f, 0.7f };

	static int screenDefaultWidth;
	static int screenDefaultHeight;
	public static void SetQuality_Settings()
	{
        // 对于华为手机高品质下开启anisotropicFiltering
        if (SystemInfo.deviceModel.StartsWith("HUAWEI"))
        {
            if (Quality == 2)
            {
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            }
            else
            {
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            }
        }

        /*
		#region Tiers
		QualitySettings.SetQualityLevel (qualityLevel[Quality]);
		QualitySettings.shadowDistance = shadowDistance [Quality];
        #endregion
        */

        // 为了更好的控制，转移到lua那边操作
        // Setting - ScreenScale
        // Setting - targetFrameRate
        // Setting - antiAliasing

        /*
        #region Uni-settings
        SetScreenF(resolutionFactor[Quality]);

        Application.targetFrameRate = 30;

		//if(Application.platform == RuntimePlatform.Android)
		if(SystemInfo.deviceModel.StartsWith("HUAWEI"))
			QualitySettings.antiAliasing = 0;
		else
			QualitySettings.antiAliasing = 2;
		#endregion
        */
    }


    public static void SetScreenF(float f)
    {
        if (screenDefaultWidth == 0)
        {
			Debug.Log (string.Format ("lsy screen w: h:", Screen.width, Screen.height));
			screenDefaultWidth = Screen.width;
			screenDefaultHeight = Screen.height;
        }

        int w = (int)((float)screenDefaultWidth * f);
        int h = (int)((float)screenDefaultHeight * f);
        //pc版不做全屏处理
        if (Application.platform != RuntimePlatform.WindowsPlayer)
        {
			Debug.Log (string.Format ("lsy screen NOW w: h:", Screen.width, Screen.height));
            Screen.SetResolution(w, h, true);
        }
    }
}