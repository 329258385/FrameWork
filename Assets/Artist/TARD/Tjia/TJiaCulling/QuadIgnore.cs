using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadIgnore : MonoBehaviour
{
    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.AddComponent<QuadIgnore>();
        }
        Invoke("SelfDestroy", Random.Range(5f, 10f));
    }

    void SelfDestroy()
    {
        if (this != null)
        {
            Destroy(this);
        }
    }
}
