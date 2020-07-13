using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAvoidShadow : MonoBehaviour {

    private float storedValue;

    private void OnPreRender()
    {
        storedValue = QualitySettings.shadowDistance;
        QualitySettings.shadowDistance =0;
    }

    private void OnPostRender()
    {
        QualitySettings.shadowDistance = storedValue;
    }

}
