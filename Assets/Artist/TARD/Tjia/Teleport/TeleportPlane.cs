using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TeleportPlane : MonoBehaviour {

    private float ZCoord = 0;
    private float XCoord = 0;
    private Vector3 AngleMemo;
    private Vector3 PosMemo;

	
	// Update is called once per frame
	void Update () {

        if (transform.eulerAngles != AngleMemo || transform.position != PosMemo)
        {
            AngleMemo = transform.eulerAngles;
            PosMemo = transform.position;

            float angle = (180 - transform.eulerAngles.y + 36000) % 360;
            float tanAngle = Mathf.Tan(Mathf.Deg2Rad * angle);
            float a = tanAngle;
            float b = -tanAngle * transform.position.x + transform.position.z;
            ZCoord = a * XCoord + b;

            float coef = 1;
            if (angle > 90 && angle < 270)
            {
                coef = -coef;
            }

            Shader.SetGlobalVector("_PlanEuqa", new Vector4(a, -1, b, coef));
        }
    }
}
