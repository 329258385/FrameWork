using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class ZzcGridDissolve : MonoBehaviour {
    [Range(-0.2f,0.5f)]
    public float width = -0.2f;

    private Image img;

    private void Start()
    {
        img = GetComponent<Image>();
        Material newMat = Instantiate(img.material);
        img.material = newMat;
    }
    // Update is called once per frame
    void Update () {
        if (img.material)
        {            
            img.material.SetFloat("_ZzcGridWidth", width);
        }
	}
}
