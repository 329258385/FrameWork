using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包界面时几个物件不失活 by Zzc
/// </summary>
public class FollowerActive : MonoBehaviour
{
    private Transform c;
    private GameObject g;

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 0)
        {
            c = transform.GetChild(0);
            if (c.childCount >= 6)
            {
                for (int i = 0; i < 6; i++)
                {
                    g = c.GetChild(i).gameObject;
                    if (g.activeInHierarchy == false)
                    {
                        g.SetActive(true);
                    }
                }
            }
        }
    }
}
