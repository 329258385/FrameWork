using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class ZzcSwitchUI : MonoBehaviour {
    //[Range(1f,10f)]
    //public float waitTime;
    //[Range(0f,3f)]
    //public float speed;
    [Range(-1f, 1f)]
    public float t=-1f;
    private Material mat;
    private Image img;
    private int id=Shader.PropertyToID("_ZzcSwitchUITime");

    private void Awake()
    {
        t = -1f;
        img = GetComponent<Image>();
        mat = img.material;

        Material newMat = Instantiate(mat);
        img.material = newMat;

        if (img.material)
        {
            img.material.SetFloat(id, t);
        }
    }
    // Use this for initialization
    void Start () {
        
    }

    private void OnEnable()
    {
        t = -1f;
    }

    private void OnDisable()
    {
        t = -1f;
        //img.material = mat;
    }


    private void OnDestroy()
    {
        //img.material = mat;
    }
    // Update is called once per frame
    void Update () {
        //t += Time.fixedDeltaTime * speed;

        //if (t>=1f)
        //{
        //    enabled = false;
        //}

        //if (t>=waitTime*speed)
        //{
        //    t = -1f;
        //}
        if (img.material)
        {
            img.material.SetFloat(id, t);
        }
        
	}
}
