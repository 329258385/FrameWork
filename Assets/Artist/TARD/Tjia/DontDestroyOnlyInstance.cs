using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnlyInstance : MonoBehaviour {

	void Awake ()
    {
        DontDestroyOnlyInstance[] ddoi = FindObjectsOfType<DontDestroyOnlyInstance>();
        for (int i = 0; i < ddoi.Length; i++)
        {
            if (ddoi[i].gameObject.name == gameObject.name && ddoi[i] != this)
            {
                Destroy(this);
                return;
            }
        }
        DontDestroyOnLoad(gameObject);
    }	
}
