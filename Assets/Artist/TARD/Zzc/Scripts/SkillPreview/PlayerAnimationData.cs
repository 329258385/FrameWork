using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class PlayerAnimationData : MonoBehaviour {
    public string skillPath= "Artist/TARD/Zzc/SkillPrefabs";

    public ZzcPlayAnim playerZzcPlay;
   
    public InputField skillName;

    public Transform skillEffectParent;

    public GameObject effectPrefab;

    private ZzcPlayAnim selfZzcplay;

    public SkillEffectDatas[] skillEffectDatas;
    [Serializable]
    public struct SkillEffectDatas
    {
        public GameObject prefab;
        public float waitTime;
        public float destroyTime;
        public float playTime;
        [SerializeField]
        public AnimationCurve curve;
        public List<Vector3> oriPathPositions;
        public Vector3 startPos;
        public Vector3 oriPos;
        public Vector3 oriForward;
    }


    //public string prefabName;
	// Use this for initialization
	void Start () {
        selfZzcplay = GetComponent<ZzcPlayAnim>();
        //prefabName = "test";
    }


    public void SavePrefabObj()
    {
        if (skillName.text==""|| skillName.text.StartsWith(" "))
        {
            Debug.Log("请输入预设名");
            return;
        }
        //保存人物数据
        selfZzcplay.totalTime = playerZzcPlay.totalTime;
        selfZzcplay.skillName = playerZzcPlay.skillName;
        selfZzcplay.setType = playerZzcPlay.setType;
        selfZzcplay.curve = playerZzcPlay.curve;
        selfZzcplay.oriPathPositions.Clear();
        for (int i = 0; i < playerZzcPlay.oriPathPositions.Count; i++)
        {
            selfZzcplay.oriPathPositions.Add(playerZzcPlay.oriPathPositions[i]);
        }

        //保存特效数据
        skillEffectDatas = new SkillEffectDatas[skillEffectParent.childCount];
        for (int i = 0; i < skillEffectParent.childCount; i++)
        {
            SkillEffect playerSkillEffect = skillEffectParent.GetChild(i).GetComponent<SkillEffect>();
            skillEffectDatas[i] = new SkillEffectDatas
            {
                oriPathPositions = new List<Vector3>(),
                prefab = playerSkillEffect.prefab,
                waitTime = playerSkillEffect.waitTime,
                destroyTime = playerSkillEffect.destroyTime,
                playTime = playerSkillEffect.playTime,
                curve = playerSkillEffect.curve,
                startPos = playerSkillEffect.transform.localPosition,
                oriPos = playerSkillEffect.originalPos,
                oriForward = playerSkillEffect.oriForward
            };
            skillEffectDatas[i].oriPathPositions.Clear();
            for (int j = 0; j < playerSkillEffect.oriPathPositions.Count; j++)
            {
                skillEffectDatas[i].oriPathPositions.Add(playerSkillEffect.oriPathPositions[j]);
            }
        }

        string path = "Assets/"+skillPath+"/"+skillName.text + ".prefab";
#if UNITY_EDITOR
        PrefabUtility.CreatePrefab(path,gameObject);
#endif
        Debug.Log(skillName.text + "保存完毕");
    }

    public void LoadPrefabObj()
    {
        string path = Application.dataPath + "/"+ skillPath+"/" + skillName.text + ".prefab";
        if (!File.Exists(path))
        {
            Debug.Log("未找到技能预设");
            return;
        }
        PlayerAnimationData pad = null;
#if UNITY_EDITOR
        pad = Instantiate(AssetDatabase.LoadAssetAtPath<PlayerAnimationData>("Assets/"+ skillPath+"/" + skillName.text + ".prefab"));
#endif
        ZzcPlayAnim zpa = pad.GetComponent<ZzcPlayAnim>();
        //读取人物数据
        playerZzcPlay.totalTime = zpa.totalTime;
        playerZzcPlay.skillName = zpa.skillName;
        playerZzcPlay.setType = zpa.setType;
        playerZzcPlay.curve = zpa.curve;
        playerZzcPlay.oriPathPositions.Clear();
        for (int i = 0; i < zpa.oriPathPositions.Count; i++)
        {
            playerZzcPlay.oriPathPositions.Add(zpa.oriPathPositions[i]);
        }

        //读取特效数据
        for (int i = 0; i < skillEffectParent.childCount; i++)
        {
            Destroy(skillEffectParent.GetChild(i).gameObject);
        }
        for (int i = 0; i < pad.skillEffectDatas.Length; i++)
        {
            SkillEffect se=Instantiate(effectPrefab, skillEffectParent).GetComponent<SkillEffect>();
            se.prefab = pad.skillEffectDatas[i].prefab;
            se.waitTime = pad.skillEffectDatas[i].waitTime;
            se.destroyTime = pad.skillEffectDatas[i].destroyTime;
            se.playTime = pad.skillEffectDatas[i].playTime;
            se.curve = pad.skillEffectDatas[i].curve;
            se.transform.localPosition = pad.skillEffectDatas[i].startPos;
            se.oriForward = pad.skillEffectDatas[i].oriForward;
            se.originalPos = pad.skillEffectDatas[i].oriPos;
            se.oriPathPositions.Clear();
            for (int j = 0; j < pad.skillEffectDatas[i].oriPathPositions.Count; j++)
            {
                se.oriPathPositions.Add(pad.skillEffectDatas[i].oriPathPositions[j]);
            }
        }


        Destroy(zpa.gameObject);
        Debug.Log(skillName.text+"读取完毕");
    }
}
