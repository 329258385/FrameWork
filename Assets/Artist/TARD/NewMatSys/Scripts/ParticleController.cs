using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class ParticleController : MonoBehaviour {

    public bool Active = false;
    [Range(0,10)]
    public float TimeToDisappear = 1;
    private ParticleSystem[] mPSs;
    private bool mInProcess = false;
    private float mTimer = 0;
    private GradientAlphaKey[] mPColors;
    private bool Finished = false;

	// Use this for initialization
	void Start () {
        

    }
	
	// Update is called once per frame
	void Update () {
        if (Active)
        {

            if (mInProcess == false)
            {
                mPSs = GetComponentsInChildren<ParticleSystem>();
                mTimer = Time.time;
                Finished = false;
                foreach (ParticleSystem ps in mPSs)
                {
                    var col = ps.colorOverLifetime;
                    mPColors = col.color.gradient.alphaKeys;

                }
            }
            mInProcess = true;
            float time = (mTimer - Time.time + TimeToDisappear) / TimeToDisappear;
            if (time > 0)
            {
                for (int i = 0; i < mPSs.Length; i++)
                {
                    var col = mPSs[i].colorOverLifetime;
                    col.enabled = true;
                    Gradient color = col.color.gradient;
                    GradientAlphaKey[] aKeys = color.alphaKeys;
                    for (int j = 0; j< aKeys.Length; j++)
                    {
                        aKeys[j].alpha = aKeys[j].alpha * (time * 0.25f + 0.75f);
                    }
                    color.SetKeys(color.colorKeys, aKeys);
                    col.color = color;
                }
            }
            else if(!Finished)
            {
                Finished = true;
                for (int i = 0; i < mPSs.Length; i++)
                {
                    Destroy(mPSs[i]);
                    /*var col = mPSs[i].colorOverLifetime;
                    col.enabled = true;
                    Gradient color = col.color.gradient;
                    GradientAlphaKey[] aKeys = color.alphaKeys;
                    for (int j = 0; j < aKeys.Length; j++)
                    {
                        aKeys[j].alpha *= time;
                    }
                    color.SetKeys(color.colorKeys, aKeys);
                    col.color = color;*/
                }
                Destroy(this);
            }
        }
        /*else
        {
            if (mInProcess == true && mPSs.Length > 0)
            {
                mInProcess = false;
                for (int i = 0; i < mPSs.Length; i++)
                {
                    var col = mPSs[i].colorOverLifetime;
                    col.enabled = true;
                    col.color.gradient.SetKeys(col.color.gradient.colorKeys,mPColors);
                }
            }
        }*/
    }
}
