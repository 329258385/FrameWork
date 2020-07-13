using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZzcFind : MonoBehaviour {
    public static ZzcFind Instance;

    public GetFrame getFrame;
    public Transform modelRoot;

    public RenderTexture playerRippleRT;
    public Material playerRippleMat;

    public bool playerRippleOn=false;

    public GameObject player;
    private float t;

    private void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start () {
        getFrame = GetComponent<GetFrame>();
        modelRoot = GameObject.Find("UGUI/Render/ModelRoot").transform;
	}

    private void Update()
    {
        if (!player)
        {
            t += Time.deltaTime;
            if (t>=1f)
            {
                player = GameObject.FindGameObjectWithTag("MainPlayer");
                t = 0f;
            }            
        }
    }

    public void DisablePlayerDBCollider()
    {
        if (player)
        {
            DynamicBone[] dbs = player.transform.GetComponentsInChildren<DynamicBone>();
            for (int i = 0; i < dbs.Length; i++)
            {
                dbs[i].enableCollider = false;
            }
        }
    }

    public void EnablePlayerDBCollider()
    {
        if (player)
        {
            DynamicBone[] dbs = player.transform.GetComponentsInChildren<DynamicBone>();
            for (int i = 0; i < dbs.Length; i++)
            {
                dbs[i].enableCollider = true;
            }
        }
    }

    public void DisableAllDBCollider()
    {
        DynamicBone[] dbs = GameObject.FindObjectsOfType<DynamicBone>();
        for (int i = 0; i < dbs.Length; i++)
        {
            dbs[i].enableCollider = false;
        }
    }
}
