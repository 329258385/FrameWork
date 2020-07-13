using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUAWEISetting : MonoBehaviour {

    private bool isHUAWEI=false;
    private Camera[] cams;

    void Awake()
    {
        if (GlobalGameDefine.mIsDisableHuaWeiSetting) Destroy(this);
    }

    // Use this for initialization
    void Start () {
        if (SystemInfo.deviceModel.StartsWith("HUAWEI"))
        {
            QualitySettings.antiAliasing = 0;
            isHUAWEI = true;
            cams = FindObjectsOfType<Camera>();

            if (cams!=null)
            {
                for (int i = 0; i < cams.Length; i++)
                {
                    cams[i].allowMSAA = false;
                }
            }
        }
        
	}

    private void Update()
    {
        if (isHUAWEI)
        {
            if (QualitySettings.antiAliasing != 0)
            {
                QualitySettings.antiAliasing = 0;
            }

            if (cams != null)
            {
                for (int i = 0; i < cams.Length; i++)
                {
                    if (cams[i].allowMSAA != false)
                    {
                        cams[i].allowMSAA = false;
                    }
                }
            }
        }
    }

}
