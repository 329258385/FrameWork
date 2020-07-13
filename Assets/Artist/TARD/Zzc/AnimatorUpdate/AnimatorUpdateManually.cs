using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class AnimatorUpdateManually : MonoBehaviour {
    private int deltaSpeed = 1;
    private Animator animator;
    private int count;
    private Transform camTrans;
    private float distance;

    private Vector3 camera2Pos;
    private bool inFrontOfCam;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        count = Random.Range(0, 4);
        //如果物件上面没有animator，删除此脚本
        if (animator == null)
        {
            DestroyImmediate(this);
        }
    }
	
	// Update is called once per frame
	void Update () {

        return;

        //改为5帧计算一次
        //if (Time.frameCount % 5 != 0) { return; }
        if (!camTrans)
        {
            camTrans = Camera.main.transform;
        }

        if (!camTrans)
        {
            return;
        }
        //使用平方，不开根号运算
        camera2Pos = transform.position - camTrans.position;
        distance = Vector3.SqrMagnitude(camera2Pos);        

        deltaSpeed = 1;

        if (distance>300f)
        {
            deltaSpeed = 2;
        }

        if (distance>525f)
        {
            deltaSpeed = 3;
        }

        if (distance > 1225f)
        {
            deltaSpeed = 4;
        }

        inFrontOfCam = Vector3.Dot(camera2Pos, camTrans.forward) > 0;
        if (!inFrontOfCam)
        {
            //Debug.Log("1111111111111111111111111111");
            deltaSpeed = 8;
        }


        //if (distance>2500)
        //{
        //    if (animator.enabled)
        //    {
        //        animator.enabled = false;
        //    }
        //    return;
        //}

        if (!animator.enabled)
        {
            animator.enabled = true;
        }
        if (animator.speed != deltaSpeed)
        {
            animator.speed = deltaSpeed;
        }

        if (deltaSpeed > 1)
        {
            count++;
            if (count < deltaSpeed)
            {
                if (animator.enabled)
                {
                    animator.enabled = false;
                }
            }
            else
            {
                count = 0;
            }
        }
    }

    private void OnDisable()
    {
        if (animator == null) { return; }
        animator.enabled = true;
        animator.speed = 1;
    }
}
