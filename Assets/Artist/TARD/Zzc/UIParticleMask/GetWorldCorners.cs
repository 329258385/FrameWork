using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWorldCorners : MonoBehaviour {
    private Vector3[] corners = new Vector3[4];
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    public Material[] mats;
    public RectTransform rt;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (rt)
        {
            rt.GetWorldCorners(corners);
        }
        
        minX = corners[0].x;
        minY = corners[0].y;
        maxX = corners[2].x;
        maxY = corners[2].y;

        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetFloat("_MinX", minX);
            mats[i].SetFloat("_MinY", minY);
            mats[i].SetFloat("_MaxX", maxX);
            mats[i].SetFloat("_MaxY", maxY);
        }
    }
}
