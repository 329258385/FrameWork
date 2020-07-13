using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockSelfRotateManager : MonoBehaviour
{
    public List<ClockSelfRotate> clockSelfRotates = new List<ClockSelfRotate>();
    // Use this for initialization
    //void Start()
    //{
    //    TweenVector3(Vector3.zero, Vector3.up, 10);
    //}

    public void TweenVector3(float start, float end, float duration)
    {
        //Debug.LogError("TweenVector3");
        DOTween.To(() => start, x => rotateMinute(x), end, duration);
    }

    public void rotateMinute(float _scTime)
    {
        for (int i = 0; i < clockSelfRotates.Count; i++)
        {
            if (clockSelfRotates[i] != null)
                clockSelfRotates[i].ResetArrow(_scTime);
        }
    }

    //private void rotateHour(Vector3 v3)
    //{
    //    //for (int i = 0; i < clockSelfRotates.Count; i++)
    //    //{
    //    //    if (clockSelfRotates[i].arrow == ClockSelfRotate.Arrow.hour)
    //    //    {
    //    //        clockSelfRotates[i].selfTransform.eulerAngles = v3;
    //    //    }
    //    //}
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
