using System;
using UnityEngine;

public class GhostItem : MonoBehaviour
{
    //持续时间
    public float duration;
    //销毁时间戳
    public float deleteTime;
    //不消散维持时间
    public float keepTime = 0;

    public MeshRenderer meshRenderer;

    private bool isInit = false;

    public void SetDataAndRun(float dur,float deTime,float keepTime, MeshRenderer render)
    {
        this.duration = dur;
        this.deleteTime = deTime;
        this.keepTime = keepTime;
        this.meshRenderer = render;
        isInit = true;
    }

    void Update()
    {
        if (!isInit)
            return;
        float tempTime = deleteTime - Time.time;
        if (tempTime <= 0)
        {//到时间就销毁
            isInit = false;
            GameObject.Destroy(this.gameObject);    //解决换装问题此对象可进池
        }
        else if (meshRenderer.material)
        {
            float kTime = deleteTime - duration;
            if (kTime > Time.time)
                return;
            //float tempRtime = deleteTime - Time.time;
            float rate = tempTime / duration;//计算生命周期的比例
            //Color cal = meshRenderer.material.GetColor("_RimColor");
            //cal.a *= rate;//设置透明通道
            meshRenderer.material.SetFloat("_DisPercentage", 1 - rate);
        }

    }
}