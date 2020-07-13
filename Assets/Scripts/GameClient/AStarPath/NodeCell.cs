using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCell : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public List<Transform> lst = new List<Transform>();
    private void Awake()
    {
#if !UNITY_EDITOR
        GameObject.DestroyImmediate(this);
#endif
    }
}
