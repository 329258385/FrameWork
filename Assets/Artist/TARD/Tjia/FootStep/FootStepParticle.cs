using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FootStepParticle : MonoBehaviour
{
    private ParticleSystem StepParticle;

    private Vector3 mCurrentStepPos;
    private Vector3 mPreviousStepPos;
    private Vector3 mForwardDir;

    private Ray mRay;
    private RaycastHit mRayInfo;

    private Transform mTransform;


    void Start()
    {
        StepParticle = GetComponent<ParticleSystem>();
        mRay = new Ray(transform.position + Vector3.up, -Vector3.up);
        mTransform = transform;
    }

    void Update()
    {

        mCurrentStepPos = mTransform.position;
        mCurrentStepPos.y = 0;
        if (Vector3.Distance(mPreviousStepPos, mCurrentStepPos) > 0.05f)
        {

            mForwardDir = mCurrentStepPos - mPreviousStepPos;
            float angle = Vector3.Angle(mForwardDir, Vector3.forward);

            var P_main = StepParticle.main;
            float correctedAngle = mForwardDir.x > 0 ? angle : -angle;
            P_main.startRotationY = Mathf.Deg2Rad * (correctedAngle + 180);

            mPreviousStepPos = mCurrentStepPos;

        }
        Vector3 pos = mTransform.position;
        mRay.origin = pos + Vector3.up;
        if (Physics.Raycast(mRay, out mRayInfo, 10, (1 << 17) | (1 << 28)))
        {
            mTransform.position = mRayInfo.point + Vector3.up * (0.05f + Random.Range(0f, 0.01f));
        }
        else
        {
            mTransform.position = mTransform.parent.position;
        }
    }
}
