using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevertMaterial : MonoBehaviour {
    public Renderer renderer;
    public Material mat;
    private int count;
    private void Update()
    {
        if (renderer.sharedMaterial!=mat)
        {
            count++;
            renderer.sharedMaterial = mat;

        }

        if (count>=100)
        {
            Destroy(this);
        }
    }
}
