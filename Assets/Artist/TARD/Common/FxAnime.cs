using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FxAnime : MonoBehaviour {

    private Material FxMat;
    [Range(0,0.02f)]
    public float ChannelDelta = 0;
    private float mChannelDelta = 0;

    // Use this for initialization
    void Start () {

        FxMat = FindObjectOfType<CameraFX>().Mat;


    }
	
	// Update is called once per frame
	void Update () {
        if (FxMat!=null && mChannelDelta != ChannelDelta)
        {
            mChannelDelta = ChannelDelta;
            FxMat.SetFloat("_ChannelDelta", ChannelDelta);
        }
	}
}
