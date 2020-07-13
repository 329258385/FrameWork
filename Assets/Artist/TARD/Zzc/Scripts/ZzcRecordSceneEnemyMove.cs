using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZzcRecordSceneEnemyMove : MonoBehaviour {
    private enum States
    {
        idle,
        walk
    }

    private Animator animator;
    private PlaceMono pm;
    private States state;
    public float minRandomTime = 1f;
    public float maxRandomTime = 5f;
    public float moveSpeed = 2f;
    //public bool stayOnGround=false;
    private float time=5f;
    private float randomTime;
    private Transform selfTransform;
    private Transform camTransform;
    private SkinnedMeshRenderer smr;
    //private Camera cam;

    // Use this for initialization
    void Start () {
        if (transform.childCount>0)
        {
            animator = transform.GetChild(0).GetComponent<Animator>();
        }
        pm = GetComponent<PlaceMono>();
        state = (States)Random.Range(0, 1);
        GetRandomTime();
        selfTransform = transform;
        camTransform = Camera.main.transform;
        smr = GetComponentsInChildren<SkinnedMeshRenderer>()[0];
        //cam = Camera.main;
    }
	
	// Update is called once per frame
	void Update () {
        if (!animator)
        {
            return;
        }

        time += Time.fixedDeltaTime;
        if (time>randomTime)
        {
            time = 0f;
            GetRandomTime();
            if (state == States.idle)
            {                
                Idle2Walk();
            }
            else if(state == States.walk)
            {
                Walk2Idle();
            }
        }

        if (state==States.walk)
        {            
            transform.Translate(Vector3.forward * Time.fixedDeltaTime* moveSpeed);
            //Debug.Log("walk");
            if (Vector3.Dot(selfTransform.position-camTransform.position, camTransform.forward) >=0f)
            {                
                //if (Vector3.Distance(selfTransform.position, camTransform.position)<=100f)
                {
                    pm.DownFloor();
                }                
            }
            
        }
        
    }

    private void GetRandomTime()
    {
        randomTime = Random.Range(minRandomTime, maxRandomTime);
    }

    private void Walk2Idle()
    {
        state = States.idle;
        animator.SetInteger("speed", 0);
    }

    private void Idle2Walk()
    {
        state = States.walk;
        transform.Rotate(Vector3.up * Random.Range(0, 360));
        animator.SetInteger("speed", 1);
    }
}
