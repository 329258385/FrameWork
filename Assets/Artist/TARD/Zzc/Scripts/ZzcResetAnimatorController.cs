using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//--------------------------------------------
//加载新场景刷新下状态机防止骨链产生未知蒙皮错误
//--------------------------------------------


public class ZzcResetAnimatorController : MonoBehaviour {
    private RuntimeAnimatorController ac;
    private Animator ani;
    private int count;
    private Transform player;
    public Image progressBar;

    private void Awake()
    {
        Destroy(this);
        ac = null;
        ani = null;
        player = null;
        count = 1;
        SceneManager.activeSceneChanged += OnChangeScene;
    }

    void Update () {

        if (count>0)
        {
            return;
        }

        //加载进度条完毕后执行
        if (ani&&ac&&progressBar)
        {
            if (progressBar)
            {
                if (progressBar.fillAmount>=1f)
                {
                    StartCoroutine(Wait());
                    count++;
                }
            }            
        }
	}

    //等待几帧
    IEnumerator Wait()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        
        ani.runtimeAnimatorController = ac;
    }

    void OnChangeScene(Scene current, Scene next)
    {
        if (next.name!="passto")
        {
            player = LsyCommon.FindPlayer();
            //寻找进度条
            if (!progressBar)
            {
                if (GameObject.Find("UGUI/UILayerOverPop/LoadingView/loading_panel_new(Clone)/RectStrecth/Progress"))
                {
                    progressBar = GameObject.Find("UGUI/UILayerOverPop/LoadingView/loading_panel_new(Clone)/RectStrecth/Progress").GetComponent<Image>();
                }
                if (GameObject.Find("UGUI/UILayerOverPop/LoadingView/loading_panel_new_special(Clone)/RectStrecth/Progress"))
                {
                    progressBar = GameObject.Find("UGUI/UILayerOverPop/LoadingView/loading_panel_new_special(Clone)/RectStrecth/Progress").GetComponent<Image>();
                }
            }
            if (player)
            {
                if (this)
                {
                    ani = player.GetComponent<Animator>();
                    ac = ani.runtimeAnimatorController;
                    ani.runtimeAnimatorController = null;
                    count = 0;
                }
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnChangeScene;
    }
}
