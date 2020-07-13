using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogBlur : MonoBehaviour
{
    public Material mat;
    public RenderTexture rt1;
    public RenderTexture rt2;
    public Camera cam;
    private int bg = Shader.PropertyToID("_BG");
    private int bg2 = Shader.PropertyToID("_BG2");
    private int count;
    private int temp = 0;

    private void OnEnable()
    {
        //Invoke("Wait", 0.1f);
        Wait();
    }

    private void Wait()
    {

        Shader.SetGlobalInt("_CrossfadeOn", 0);

        if (!cam)
        {
            cam = GameObject.FindGameObjectWithTag("DialogCamera").GetComponent<Camera>();
        }

        if (!rt1)
        {
            rt1 = new RenderTexture(/*Screen.width >> 2*/672, /*Screen.height >> 2*/310, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            rt1.enableRandomWrite = false;
            rt1.autoGenerateMips = false;
        }

        if (!rt2)
        {
            rt2 = new RenderTexture(/*Screen.width >> 2*/672, /*Screen.height >> 2*/310, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            rt2.enableRandomWrite = false;
            rt2.autoGenerateMips = false;
        }

        cam.enabled = true;
        if (!cam.targetTexture)
            cam.targetTexture = rt1;
        //0和1中低品质（渲染一次） 2高品质（rt实时渲染）
        if (TARDSwitches.Quality == 0 || TARDSwitches.Quality == 1)
            count = 1;
        else
            count = 2;

        temp = 0;
    }

    private void Update()
    {
        //只渲染一次
        if (count == 1)
        {
            
            mat.SetTexture(bg, rt1);
            Graphics.Blit(rt1, rt2, mat, 0);
            mat.SetTexture(bg2, rt2);
            temp++;
            //if (temp>3)
            {
                count = 0;
                cam.targetTexture = null;
                cam.enabled = false;
            }
            
        }
        //实时渲染
        else if (count == 2)
        {
            mat.SetTexture(bg, rt1);
            Graphics.Blit(rt1, rt2, mat, 0);
            mat.SetTexture(bg2, rt2);
        }
    }

    private void OnDisable()
    {

        Shader.SetGlobalInt("_CrossfadeOn", 1);

        rt1.Release();
        rt2.Release();
        if (cam)
        {
            cam.targetTexture = null;
            cam.enabled = false;
        }
        temp = 0;
    }
}
