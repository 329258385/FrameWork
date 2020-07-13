using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowPosController : MonoBehaviour {

    public Transform CamPos;
    public ParticleSystem Snow;
    private Vector3 mlastPos;
    private float mSnowSpeed;
    public float SpeedCoef;
    private float Velocity = 0;


	// Use this for initialization
	void Start () {
        if (CamPos == null && Camera.main != null)
        {
            CamPos = Camera.main.transform;
        }
        mSnowSpeed = Snow.main.startSpeedMultiplier;
    }
	
	// Update is called once per frame
	void Update () {
        if (CamPos != null)
        {
            transform.position = CamPos.position;
            Velocity = Velocity * 0.95f + Vector3.Distance(transform.position, mlastPos) / Time.deltaTime * 0.05f;
            var snowMain = Snow.main;
            snowMain.startSpeedMultiplier = mSnowSpeed + Velocity * SpeedCoef;
            //Debug.Log(mSnowSpeed + velocity * SpeedCoef);
            //Snow.main = snowMain;
            mlastPos = transform.position;
        }
    }
}
