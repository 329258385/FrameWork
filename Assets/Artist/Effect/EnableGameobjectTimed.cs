using UnityEngine;
using System.Collections;

public class EnableGameobjectTimed : MonoBehaviour {

    public float time;
    public bool DisableAtBegin = true;
    public GameObject[] GameObjects;
	
    void OnEnable()
    {
        if (DisableAtBegin)
        {
            foreach (GameObject go in GameObjects)
            {
                if (go != null)
                    go.SetActive(false);
            }
        }
        Invoke("SetActive", time);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void SetActive()
    {
        foreach (GameObject go in GameObjects)
        {
            if(go != null)
                go.SetActive(true);
        }
    }

}
