using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCenter : MonoBehaviour
{
    public Vector4 centerPos;
    // Start is called before the first frame update
    void Start()
    {
        centerPos = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0f);
        Shader.SetGlobalVector("_CenterPos", centerPos);
    }

}
