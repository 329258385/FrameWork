using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GetWeapon : PlayableBehaviour {

    public string nodeName;
    public GameObject weapon;
    public int count;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (count==0)
        {
            GameObject g1 = GameObject.Find(nodeName);
            GameObject g2 = weapon;

            g2.transform.SetParent(g1.transform);

            count++;
        }
    }

    public override void OnGraphStop(Playable playable)
    {
        count = 0;
        Debug.Log("stop");
    }
}
