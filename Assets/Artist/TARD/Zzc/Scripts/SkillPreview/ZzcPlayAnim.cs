using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class ZzcPlayAnim : MonoBehaviour {
    private Animator anim;
    private float myTime;
    private Vector3 currentPos;
    public Transform moveTransform;
    public KeyCode keyCode;
    public float totalTime;
    public string skillName= "Trigger";
    public int setType;
    public float runspeed = 5.0f;
    [SerializeField]
    public AnimationCurve curve;

    private DOTweenPath dtp;
    private Vector3 originalPos;
    private Vector3 offsetVecotr;

    private Vector3 oriForward;
    private Quaternion q;

    private List<Vector3> newPathPositions;
    [HideInInspector]
    public List<Vector3> oriPathPositions;
    [HideInInspector]
    public bool canDo = true;

    public GameObject tachi;
    public GameObject sword;
    public GameObject rapier;
    public GameObject mace;
    public GameObject axe;
    public GameObject bow;

	void Start () {
        newPathPositions = new List<Vector3>();
        oriPathPositions = new List<Vector3>();

        if (gameObject.name == "PlayerAnimationData" || gameObject.name.EndsWith("(Clone)"))
        {
            return;
        }

        if (sword&&rapier&&mace&&axe&&bow)
        {
            sword.SetActive(false);
            rapier.SetActive(false);
            mace.SetActive(false);
            axe.SetActive(false);
            bow.SetActive(false);
        }


        anim =moveTransform.GetComponent<Animator>();
        
        myTime = totalTime;
        dtp = GetComponent<DOTweenPath>();
        dtp.relative = true;
        originalPos = transform.position;
        oriForward = transform.forward;
        if (dtp.wps != null && dtp.wps.Count > 0)
        {
            for (int i = 0; i < dtp.wps.Count; i++)
            {
                oriPathPositions.Add(dtp.wps[i] - originalPos);
            }
        }
       

    }
	
    private void SetTrue()
    {
        moveTransform.GetComponent<ZzcPlayAnim>().canDo = true;
    }


	void LateUpdate () {
        if (gameObject.name=="PlayerAnimationData"||gameObject.name.EndsWith("(Clone)"))
        {
            return;
        }
        //anim.SetInteger("Type", setType);
        myTime += Time.deltaTime;       

        offsetVecotr = transform.position - originalPos;
        q = Quaternion.FromToRotation(oriForward, transform.forward);

        if (Input.GetKeyDown(keyCode) && moveTransform.GetComponent<ZzcPlayAnim>().canDo==true)
        {
            moveTransform.GetComponent<ZzcPlayAnim>().canDo = false;
            anim.SetInteger("Type", setType);

            newPathPositions.Clear();

            for (int i = 0; i < oriPathPositions.Count; i++)
            {
                Vector3 newVec = q * oriPathPositions[i];
                newVec += transform.position;
                newPathPositions.Add(newVec);
            }

            if (newPathPositions.Count > 0)
            {
                moveTransform.DOPath(newPathPositions.ToArray(), totalTime).SetEase(curve);
            }
            //for (int i = 0; i < GetComponents<DOTweenAnimation>().Length; i++)
            //{
            //    GetComponents<DOTweenAnimation>()[i].tween.Play();
            //    //GetComponents<DOTweenAnimation>()[i].DOPlay();

            //}

            anim.SetTrigger("Trigger");
            Invoke("SetTrue", totalTime);
            //myTime = 0f;
        }

        //if (myTime<=totalTime)
        //{
        //    moveTransform.GetComponent<ZzcPlayAnim>().canDo = false;
        //    return;
        //}
        //else
        //{
        //    moveTransform.GetComponent<ZzcPlayAnim>().canDo = true;          
        //}


        if (moveTransform!=transform|| moveTransform.GetComponent<ZzcPlayAnim>().canDo == false)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * runspeed, Space.Self);
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z) + new Vector3(-Camera.main.transform.right.x, 0, -Camera.main.transform.right.z));
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z) + new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z));
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(-Camera.main.transform.forward.x, 0, -Camera.main.transform.forward.z) + new Vector3(-Camera.main.transform.right.x, 0, -Camera.main.transform.right.z));
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(-Camera.main.transform.forward.x, 0, -Camera.main.transform.forward.z) + new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z));
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z));
        }
        else
        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(-Camera.main.transform.forward.x, 0, -Camera.main.transform.forward.z));
        }
        else
        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(-Camera.main.transform.right.x, 0, -Camera.main.transform.right.z));
        }
        else
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z));
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WeaponActive(tachi);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WeaponActive(sword);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            WeaponActive(rapier);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            WeaponActive(mace);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            WeaponActive(axe);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            WeaponActive(bow);
        }

    }

    void WeaponActive(GameObject weapon)
    {
        tachi.SetActive(false);
        sword.SetActive(false);
        rapier.SetActive(false);
        mace.SetActive(false);
        axe.SetActive(false);
        bow.SetActive(false);
        weapon.SetActive(true);
    }
}
