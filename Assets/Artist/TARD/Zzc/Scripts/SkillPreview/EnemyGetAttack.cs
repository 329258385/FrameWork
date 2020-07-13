using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyGetAttack : MonoBehaviour {
    public ZzcPlayAnim charactor;
    public GameObject enemy;
    private Animator anim;
    public float totalTime;
    public KeyCode keyCode;

    [System.Serializable]
    public struct HI
    {
        public float time;
        public string name;
    }

    [System.Serializable]
    public struct EI
    {
        public float time;
        public GameObject go;
    }

    public HI[] hi;
    public EI[] ei;

    [SerializeField]
    public AnimationCurve curve;
    private DOTweenPath dtp;
    private Vector3 originalPos;
    private Vector3 offsetVector;
    private Vector3 oriForward;
    private Quaternion q;
    private List<Vector3> newPathPosition;
    [HideInInspector]
    public List<Vector3> oriPathPosition;

    private float myTime;
    private bool isDead=false;

    private Dictionary<float, string> hitInfo;
    private Dictionary<float, GameObject> effectInfo;

	void Start () {
        anim = enemy.GetComponent<Animator>();
        dtp = GetComponent<DOTweenPath>();
        dtp.relative = true;
        originalPos = transform.position;
        oriForward = transform.forward;

        hitInfo = new Dictionary<float, string>();
        effectInfo = new Dictionary<float, GameObject>();

        newPathPosition = new List<Vector3>();
        oriPathPosition = new List<Vector3>();

        if (dtp.wps!=null)
        {
            for (int i = 0; i < dtp.wps.Count; i++)
            {
                oriPathPosition.Add(dtp.wps[i] - originalPos);
            }
        }
        

        for (int i = 0; i < ei.Length; i++)
        {
            effectInfo.Add(ei[i].time, ei[i].go);
        }

        for (int i = 0; i < hi.Length; i++)
        {
            hitInfo.Add(hi[i].time, hi[i].name);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            isDead = false;
            anim.SetTrigger("hitBack");
        }

        if (Input.GetKeyDown(keyCode)&& charactor.canDo==true)
        {
            
            hitInfo.Clear();
            effectInfo.Clear();

            for (int i = 0; i < ei.Length; i++)
            {
                effectInfo.Add(ei[i].time, ei[i].go);
            }

            for (int i = 0; i < hi.Length; i++)
            {
                hitInfo.Add(hi[i].time, hi[i].name);
            }

            foreach (KeyValuePair<float,string> kv in hitInfo)
            {
                StartCoroutine(PlayerAnimationData(kv.Key,kv.Value));
            }

            foreach (KeyValuePair<float,GameObject> kv in effectInfo)
            {
                StartCoroutine(EffectData(kv.Key, kv.Value));
            }

            for (int i = 0; i < oriPathPosition.Count; i++)
            {
                Vector3 newVec = q * oriPathPosition[i];
                newVec += enemy.transform.position;
                newPathPosition.Add(newVec);
            }

            if (newPathPosition.Count>0)
            {
                if (newPathPosition.Count > 0)
                {
                    enemy.transform.DOPath(newPathPosition.ToArray(), totalTime).SetEase(curve);
                }
            }
           

        }

        offsetVector = enemy.transform.position - originalPos;
        q = Quaternion.FromToRotation(oriForward, transform.forward);

    }

    IEnumerator EffectData(float waitTime,GameObject go)
    {
        yield return new WaitForSeconds(waitTime);
        GameObject g=Instantiate(go);
        g.transform.position = enemy.transform.position;
        Destroy(g, 10f);
    }

    IEnumerator PlayerAnimationData(float waitTime,string animationName)
    {
        yield return new WaitForSeconds(waitTime);
        anim.SetTrigger(animationName);
        if (animationName=="die")
        {
            GetComponent<TweenMaterialMgr>().Play("die");
        }
        else
        {
            GetComponent<TweenMaterialMgr>().Play("hurt");
        }
        
    }
}
