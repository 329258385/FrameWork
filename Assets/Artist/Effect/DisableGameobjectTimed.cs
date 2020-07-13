using UnityEngine;
using System.Collections;

public class DisableGameobjectTimed : MonoBehaviour {

    public float time;
    public GameObject[] GameObjects;

    // Use this for initialization
    void Start()
    {
        Invoke("SetActive", time);
    }

    void SetActive()
    {
        foreach (GameObject go in GameObjects)
        {
            if (go != null)
                go.SetActive(false);
        }
    }

}
