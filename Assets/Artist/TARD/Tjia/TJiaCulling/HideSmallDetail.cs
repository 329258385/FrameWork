using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSmallDetail : MonoBehaviour {

    public bool TenMetreMode = false;
    public float HideSize = 0;
    private float bHideSize = 0;
    private List<GameObject> mMRObjs = new List<GameObject>();
    private List<float> mMRSizes = new List<float>();
    private List<Vector3> mMRPoses = new List<Vector3>();
    private List<bool> mMRStates = new List<bool>();
    private List<int> mMRLayers = new List<int>();

    [Space(20)]
    public bool UpdateList = false;
    private int bUBuffer = 0;
    public List<GameObject> HidenObjs = new List<GameObject>();

	void Start () {

#if !UNITY_EDITOR
        Destroy(this);
#endif

        List<int> mCullLayerList = new List<int>();

        mCullLayerList.Clear();

        mCullLayerList.Add(LayerMask.NameToLayer("UI"));
        mCullLayerList.Add(LayerMask.NameToLayer("Player"));
        mCullLayerList.Add(LayerMask.NameToLayer("Monster"));
        mCullLayerList.Add(LayerMask.NameToLayer("Boss"));
        mCullLayerList.Add(LayerMask.NameToLayer("NPC"));
        mCullLayerList.Add(LayerMask.NameToLayer("Collection"));

        mCullLayerList.Add(LayerMask.NameToLayer("UISpecial"));
        mCullLayerList.Add(LayerMask.NameToLayer("UI_Model"));
        mCullLayerList.Add(LayerMask.NameToLayer("3DUI"));

        mCullLayerList.Add(LayerMask.NameToLayer("Decal"));
        mCullLayerList.Add(LayerMask.NameToLayer("ClientPlayer"));
        mCullLayerList.Add(LayerMask.NameToLayer("Region"));

        MeshRenderer[] mMRs = FindObjectsOfType<MeshRenderer>();
        int length = mMRs.Length;

        for (int i = 0; i < length; i++)
        {
            if (mMRs[i].enabled && !mCullLayerList.Contains(mMRs[i].gameObject.layer) && !mMRs[i].GetComponent<QuadIgnore>())
            {
                mMRObjs.Add(mMRs[i].gameObject);
                mMRPoses.Add(mMRs[i].transform.position);
                mMRSizes.Add(Vector3.Magnitude(mMRs[i].bounds.extents));
                mMRStates.Add(true);
                mMRLayers.Add(mMRs[i].gameObject.layer);
            }
        }
	}
	
	void Update () {
        if (TenMetreMode)
        {
            if (UpdateList && bUBuffer == 0)
            {
                HidenObjs.Clear();
                bUBuffer = 1;
                for (int i = 0; i < mMRObjs.Count; i++)
                {
                    if (mMRObjs[i] != null)
                    {
                        mMRObjs[i].layer = mMRLayers[i];
                        mMRStates[i] = true;
                    }
                }
            }

            if (bUBuffer == 1)
            {
                bUBuffer = 2;
                return;
            }

            for (int i = 0; i < mMRObjs.Count; i++)
            {
                float dis = Vector3.Distance(transform.position, mMRPoses[i]);
                float hideSize = HideSize * 0.1f * dis;
                if (mMRObjs[i] != null)
                {
                    if (mMRSizes[i] < hideSize)
                    {
                        if (mMRStates[i])
                        {
                            mMRObjs[i].layer = 29; //DoNotRender
                            mMRStates[i] = false;
                        }
                    }
                    else
                    {
                        if (!mMRStates[i])
                        {
                            mMRObjs[i].layer = mMRLayers[i];
                            mMRStates[i] = true;
                        }
                    }
                    if (UpdateList && mMRStates[i] == false)
                    {
                        MeshRenderer mr = mMRObjs[i].GetComponent<MeshRenderer>();
                        mMRObjs[i].layer = mMRLayers[i];
                        if (mr.isVisible)
                        {
                            HidenObjs.Add(mMRObjs[i]);
                        }
                        mMRObjs[i].layer = 29;
                        bUBuffer = 0;
                    }
                }
            }
            UpdateList = false;
        }
        else if (bHideSize != HideSize)
        {
            bHideSize = HideSize;

            for (int i = 0; i < mMRObjs.Count; i++)
            {
                if (mMRObjs[i] != null)
                {
                    if (mMRSizes[i] < HideSize)
                    {
                        if (mMRStates[i])
                        {
                            mMRObjs[i].layer = 29; //DoNotRender
                            mMRStates[i] = false;
                        }
                    }
                    else
                    {
                        if (!mMRStates[i])
                        {
                            mMRObjs[i].layer = mMRLayers[i];
                            mMRStates[i] = true;
                        }
                    }
                }
            }            
        }
	}
}
