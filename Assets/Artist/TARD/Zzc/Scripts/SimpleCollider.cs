using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCollider : MonoBehaviour {
    public enum ColliderType
    {
        Pos,
        Transform
    }
    public enum Axis
    {
        X,
        Y,
        Z
    }
    public ColliderType colliderType = ColliderType.Pos;
    public Axis axis = Axis.Y;
    public Transform followObj;
    public float offsetValue;
    private int count = 0;
    private float reCheckTime;

    // Update is called once per frame
    void Update () {

        reCheckTime += Time.deltaTime;

        if (reCheckTime>2f&&count<3)
        {
            reCheckTime = 0;
            count++;
            if (transform.parent.tag!="MainPlayer"&&!transform.parent.name.StartsWith("e")&&!transform.parent.name.StartsWith("1"))
            {
                enabled = false;
            }
        }

        if (followObj!=null)
        {
            if (colliderType == ColliderType.Pos)
            {
                transform.position = followObj.position;
            }
            else if (colliderType == ColliderType.Transform)
            {
                transform.position = followObj.position;
                transform.rotation = followObj.rotation;

                if (axis == Axis.X)
                {
                    transform.position = followObj.position + followObj.right * offsetValue;
                }
                else if(axis == Axis.Y)
                {
                    transform.position = followObj.position + followObj.up * offsetValue;
                }
                else if (axis == Axis.Z)
                {
                    transform.position = followObj.position + followObj.forward * offsetValue;
                }
            }

        }
       
	}
}
