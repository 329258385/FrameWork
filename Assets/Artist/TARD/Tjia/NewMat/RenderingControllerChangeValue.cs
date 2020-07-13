using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingControllerChangeValue : MonoBehaviour
{

    private RenderingController renderingController;
    private ClockSelfRotateManager clockSelfRotateManager;
    private RainSky rainSky;
    private List<List<float>> timeChangeRange = new List<List<float>>();

    private float currentSceneTime;
    private GameObject objRainbow;
    private float tempTime = -1;

    //下雨
    int CloudySwitch = 0;//0关 1开
    float CloudyLevel = 0;//阴天程度（晴天0-1阴天）
    float RainLevel = 0;//雨量（0 - 1）
    float Wetness = 0;//湿度（0 - 1）
    GameObject rainSkyKey;

    private InGameTimer mTimer;
    private Tweener tweener;
    void Start()
    {
        renderingController = this.GetComponent<RenderingController>();
        if (renderingController != null)
        {
            List<float> list_1 = new List<float>();
            list_1.Add(renderingController.NightEnd);
            list_1.Add(renderingController.MorningStart);
            timeChangeRange.Add(list_1);

            List<float> list_2 = new List<float>();
            list_2.Add(renderingController.MorningEnd);
            list_2.Add(renderingController.DayStart);
            timeChangeRange.Add(list_2);

            List<float> list_3 = new List<float>();
            list_3.Add(renderingController.DayEnd);
            list_3.Add(renderingController.DuskStart);
            timeChangeRange.Add(list_3);

            List<float> list_4 = new List<float>();
            list_4.Add(renderingController.DuskEnd);
            list_4.Add(renderingController.NightStart);
            timeChangeRange.Add(list_4);
        }

        clockSelfRotateManager = this.GetComponent<ClockSelfRotateManager>();
        objRainbow = GameObject.Find("fx_Env_lv00_main_battle_rainbow02");
        GameObject objRs = GameObject.Find("RainSky");
        if (objRs != null)
        {
            rainSky = objRs.GetComponent<RainSky>();
        }
    }

    public void SetMainCameraTimeOf24(float time, bool isChange, float speed, bool istask)
    {
        if (tempTime == time)
            return;
        tempTime = time;
        if (renderingController != null)
        {
            if (time == renderingController.TimeOf24)
                return;
            if (tweener != null)
                tweener.Kill();
            if (isChange)
            {
                if (time > renderingController.TimeOf24)
                {
                    tweener = DOTween.To(() => renderingController.TimeOf24, x => setTimeOf24(x), time, speed);
                }
                else
                {
                    tweener = DOTween.To(() => renderingController.TimeOf24, x => setTimeOf24(x), time + 24, speed);
                }
            }
            else
            {
                setTimeOf24(time);
            }
        }
    }

    //设置24小时时间每隔一小时增加+1
    public void TimeCallMianCameraTimeOf24(float time, float changeSec)
    {
        if (tweener != null)
            tweener.Kill();
        tweener = DOTween.To(() => renderingController.TimeOf24, x => setTimeOf24(x), time + 24, 5);
    }

    public void DayOf24Add(float _sec, float time)
    {
        if (clockSelfRotateManager != null)
        {
            float _time = renderingController.TimeOf24;
            if (_time + 0.1f >= 24)
                _time = 0;
            clockSelfRotateManager.TweenVector3(_time, _time + 0.1f, _sec);
        }
        if (renderingController != null)
        {
            if (tweener != null)
                tweener.Kill();            
            currentSceneTime = time;               
            SetRainbowActive();            
            bool boolInRange = GetInRange(renderingController.TimeOf24);
            if (boolInRange)
                //TimeCallMianCameraTimeOf24(time, _sec);
                tweener = DOTween.To(() => renderingController.TimeOf24, x => setTimeOf24_1(x), time, _sec);
            else
                //TimeCallMianCameraTimeOf24(time, 3);
                tweener = DOTween.To(() => renderingController.TimeOf24, x => setTimeOf24_1(x), time, 3);
        }
    }

    private bool GetInRange(float time_)
    {
        for (int i = 0; i < timeChangeRange.Count; i++)
        {
            if (timeChangeRange[i][0] <= time_ && time_ < timeChangeRange[i][1])
                return true;
        }
        return false;
    }

    //切换场景恢复上一场景时间
    public void RecoverLastWorldTime(float _time)
    {
        if (renderingController != null)
        {
            currentSceneTime = _time;
            renderingController.TimeOf24 = _time;
            if (clockSelfRotateManager != null)
            {
                clockSelfRotateManager.rotateMinute(_time);
            }
            SetRainbowActive();
        }
    }

    private void setTimeOf24(float val)
    {
        if (val >= 24)
        {
            val = val % 24;
        }
        if (renderingController != null)
        {
            currentSceneTime = val;
            renderingController.TimeOf24 = val;
            if (clockSelfRotateManager != null)
            {
                clockSelfRotateManager.rotateMinute(val);
            }
            SetRainbowActive();
        }
    }

    private void setTimeOf24_1(float val)
    {
        if (val >= 24)
        {
            val = val % 24;
        }
        if (renderingController != null)
        {
            //currentSceneTime = val;
            renderingController.TimeOf24 = val;
            //SetRainbowActive();
        }
    }

    public void SetRainSky(float cloudyLevel, float cloudyLevelSpeed, float rainLevel, float rainLevelSpeed, float wetness, float wetnessSpeed)
    {
        if (cloudyLevelSpeed == 0 && rainLevelSpeed == 0 && wetnessSpeed == 0)
            return;

        if (CloudyLevel == cloudyLevel && RainLevel == rainLevel && Wetness == wetness)
            return;
        if (renderingController == null)
            return;
        if (rainSky == null)
            return;
        renderingController.CloudySwitch = true;
        if (CloudyLevel != cloudyLevel)
        {
            DOTween.To(() => renderingController.CloudyLevel, x => { renderingController.CloudyLevel = x; }, cloudyLevel, cloudyLevelSpeed);
        }

        if (RainLevel != rainLevel)
        {
            DOTween.To(() => rainSky.RainLevel, x => { rainSky.RainLevel = x; }, rainLevel, rainLevelSpeed);
        }

        if (Wetness != wetness)
        {
            DOTween.To(() => rainSky.Wetness, x => { rainSky.Wetness = x; }, wetness, wetnessSpeed);
        }

        CloudySwitch = 1;
        CloudyLevel = cloudyLevel;
        RainLevel = rainLevel;
        Wetness = wetness;
        SetRainbowActive();
    }

    //恢复大世界晴天
    public void RecoverBigWorldRainSky()
    {
        if (renderingController == null)
            return;
        if (rainSky == null)
            return;

        DOTween.To(() => renderingController.CloudyLevel, x => { renderingController.CloudyLevel = x; }, 0, 10);

        DOTween.To(() => rainSky.RainLevel, x => { rainSky.RainLevel = x; }, 0, 10);

        DOTween.To(() => rainSky.Wetness, x => { rainSky.Wetness = x; }, 0, 10);
        mTimer = TimeManager.Instance.CreateTimer(10, true).RegisterCompleteCallback(timeOut);
        mTimer.Start();
        CloudySwitch = 0;
        CloudyLevel = 0;
        RainLevel = 0;
        Wetness = 0;
    }

    private void timeOut(InGameTimer item = null)
    {
        if (renderingController == null)
        {
            renderingController.CloudySwitch = false;
            SetRainbowActive();
        }
        mTimer = null;
    }

    //切换场景恢复上一场景阴天雨天
    public void RecoverLastWorldRainSky(float cloudyLevel, float rainLevel, float wetness)
    {
        if (renderingController == null)
            return;
        if (rainSky == null)
            return;
        renderingController.CloudyLevel = cloudyLevel;
        rainSky.RainLevel = rainLevel;
        rainSky.Wetness = wetness;
        CloudySwitch = 1;
        CloudyLevel = cloudyLevel;
        RainLevel = rainLevel;
        Wetness = wetness;
        SetRainbowActive();
    }

    private void SetRainbowActive()
    {
        if (objRainbow != null)
        {
            if (CloudySwitch == 0 && currentSceneTime >= 5 && currentSceneTime <= 19)
            {
                if (!objRainbow.activeSelf)
                    objRainbow.SetActive(true);
            }
            else
            {
                if (objRainbow.activeSelf)
                    objRainbow.SetActive(false);
            }
        }
    }

}
