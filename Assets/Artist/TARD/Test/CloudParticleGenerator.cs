using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CloudParticleGenerator : MonoBehaviour {

    public ParticleSystem CloudParticle;
    [Range(1,60)]
    public float UpdateInterval = 1;
    private float mTimer = 0;
    private Ray mRay;
    private RaycastHit mRayInfo; 


    // Use this for initialization
    void Start () {
        mTimer = Time.time + UpdateInterval;
        mRay.direction = -Vector3.up;
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time > mTimer)
        {
            CloudParticle.Emit(1);
            mTimer = Time.time + UpdateInterval;
            do
            {
                Vector3 origin = transform.position;
                do
                {
                    origin.x += Random.Range(-200, 200);
                    origin.z += Random.Range(-200, 200);

                }
                while (Vector3.Distance(transform.position, origin) < 30);
                origin.y += 200;  
                mRay.origin = origin;
            }
            while (!Physics.Raycast(mRay, out mRayInfo, 500f));
            CloudParticle.transform.position = mRayInfo.point;
        }
        else
        {
            //mTimer = Time.time + UpdateInterval;
        }
	}
}
