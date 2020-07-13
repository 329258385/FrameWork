using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkillEffect : MonoBehaviour {
    public KeyCode keyCode;
    public ZzcPlayAnim charactor;
    public GameObject prefab;
    public float waitTime;
    public float destroyTime;
    public float playTime;
    [SerializeField]
    public AnimationCurve curve;
    private DOTweenPath dtp;
    [HideInInspector]
    public Vector3 originalPos;
    private Vector3 offsetVecotr;
    [HideInInspector]
    public Vector3 oriForward;
    private Quaternion q;

    private List<Vector3> newPathPositions;
    [HideInInspector]
    public List<Vector3> oriPathPositions;


    void Start () {
        
        dtp = GetComponent<DOTweenPath>();
        newPathPositions = new List<Vector3>();
        
        originalPos = transform.position;
        
        if (!(gameObject.name == "PlayerAnimationData") &&! gameObject.name.EndsWith("(Clone)"))
        {
            oriForward = transform.forward;
            oriPathPositions = new List<Vector3>();
            if (dtp.wps!=null&&dtp.wps.Count>0)
            {
                for (int i = 0; i < dtp.wps.Count; i++)
                {
                    oriPathPositions.Add(dtp.wps[i] - originalPos);
                }
            }
            
        }
       
    }

	void Update () {
        offsetVecotr = transform.position - originalPos;
        q = Quaternion.FromToRotation(oriForward, transform.forward);
        if (Input.GetKeyDown(keyCode)&&charactor.canDo==true)
        {         
            Invoke("EffectSpawn", waitTime);
        }
	}

    void EffectSpawn()
    {
        newPathPositions.Clear();

        for (int i = 0; i < oriPathPositions.Count; i++)
        {
            Vector3 newVec = q * oriPathPositions[i];
            newVec += transform.position;
            newPathPositions.Add(newVec);

        }
        GameObject go = Instantiate(prefab, transform);
        go.transform.localPosition = Vector3.zero;
        if (newPathPositions.Count>0)
        {
            go.transform.DOPath(newPathPositions.ToArray(), playTime).SetEase(curve);
        }
        
        Destroy(go, destroyTime);
    }

}
