using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClockSelfRotate : MonoBehaviour {

    public enum Type
    {
        systemTime,
        selfRotate
    }

    public enum Arrow
    {
        minute,
        hour
    }

    public Type type;
    public Arrow arrow;
    private float speed=5f;

    

    private int tFrameCount;   
    public Transform selfTransform;
    private void Start()
    {
        selfTransform = transform;
    }
    // Update is called once per frame
    void Update () {
        //15帧同步一次秒钟
        //tFrameCount = Time.frameCount % 15;
        //if (tFrameCount == 0)
        //{
        //    if (type == Type.selfRotate)
        //    {
        //        transform.Rotate(Vector3.forward, -speed * Time.deltaTime);
        //    }
        //}
        if (type == Type.selfRotate)
        {
            transform.Rotate(Vector3.forward, -speed * Time.deltaTime);
        }
    }

    public void ResetArrow(float _scTime)
    {
            if (arrow == Arrow.minute)
            {
                float z = _scTime - Mathf.Floor(_scTime);                
                selfTransform.eulerAngles = new Vector3(selfTransform.eulerAngles.x, selfTransform.eulerAngles.y, -360f * z);               
            }
            else if (arrow == Arrow.hour)
            {
                float z = _scTime / 12f;
                selfTransform.eulerAngles = new Vector3(selfTransform.eulerAngles.x, selfTransform.eulerAngles.y, -360f * z);
            }        
    }
}
