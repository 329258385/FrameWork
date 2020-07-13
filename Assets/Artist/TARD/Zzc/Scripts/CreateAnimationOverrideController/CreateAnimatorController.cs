#if UNITY_EDITOR
using System.Collections;

using UnityEditor.Animations;

using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

using UnityEditor;


public class CreateAnimatorController : MonoBehaviour
{
    private static string path;
    public InputField inputField;

    public static AnimatorController baseAnimatorController;

    private static AnimatorOverrideController aoc;
    public static List<AnimationClip> skillClips;
    private static string str;


    [MenuItem("Assets/SaoUtils/生成base_monster override")]
    public static void Create()
    {
        //path = "Assets/Artist/Character/Monster/"+ inputField.text+"/Animation";
        //if (Directory.Exists(path)==false)
        //{
        //   path = "Assets/Artist/Character/Monster/" + inputField.text + "/Animations";
        //}
        skillClips = new List<AnimationClip>();
        path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        string[] s = path.Split('/');
        string fatherPath = "";
        for (int k = 0; k < s.Length - 1; k++)
        {
            if (k == s.Length - 2)
            {
                fatherPath += s[k];
            }
            else
            {
                fatherPath += s[k] + "/";
            }
        }
        str = s[s.Length - 1].Replace("monster.controller", "override.anim");

        baseAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Artist/Prefabs/aniController/exclude_forexport/base_monster.controller");

        aoc = new AnimatorOverrideController(baseAnimatorController);
        aoc["monster_attack01"] = GetAnimationClip(fatherPath, "attack01");
        aoc["monster_attack02"] = GetAnimationClip(fatherPath, "attack02");
        aoc["monster_attack03"] = GetAnimationClip(fatherPath, "attack03");
        aoc["monster_attack04"] = GetAnimationClip(fatherPath, "attack04");
        aoc["monster_attack05"] = GetAnimationClip(fatherPath, "attack05");
        aoc["monster_attack06"] = GetAnimationClip(fatherPath, "attack06");
        aoc["monster_attack07"] = GetAnimationClip(fatherPath, "attack07");
        aoc["monster_attack08"] = GetAnimationClip(fatherPath, "attack08");
        aoc["monster_attack09"] = GetAnimationClip(fatherPath, "attack09");
        aoc["monster_attack10"] = GetAnimationClip(fatherPath, "attack10");
        aoc["monster_skill01"] = GetAnimationClip(fatherPath, "skill01");
        aoc["monster_skill02"] = GetAnimationClip(fatherPath, "skill02");
        aoc["monster_skill03"] = GetAnimationClip(fatherPath, "skill03");
        aoc["monster_skill04"] = GetAnimationClip(fatherPath, "skill04");
        aoc["monster_skill05"] = GetAnimationClip(fatherPath, "skill05");
        aoc["monster_skill06"] = GetAnimationClip(fatherPath, "skill06");
        aoc["monster_skill07"] = GetAnimationClip(fatherPath, "skill07");
        aoc["monster_skill08"] = GetAnimationClip(fatherPath, "skill08");
        aoc["monster_skill09"] = GetAnimationClip(fatherPath, "skill09");
        aoc["monster_skill10"] = GetAnimationClip(fatherPath, "skill10");
        aoc["monster_death"] = GetAnimationClip(fatherPath, "die");
        aoc["monster_hit"] = GetAnimationClip(fatherPath, "hit");
        aoc["monster_idle_01"] = GetAnimationClip(fatherPath, "idle");
        aoc["monster_knockback"] = GetAnimationClip(fatherPath, "knockback");
        aoc["monster_knockdown"] = GetAnimationClip(fatherPath, "knockdown");
        aoc["monster_knockfly"] = GetAnimationClip(fatherPath, "knockfly");
        aoc["monster_run"] = GetAnimationClip(fatherPath, "run");
        aoc["monster_standup"] = GetAnimationClip(fatherPath, "standup");
        aoc["monster_stun"] = GetAnimationClip(fatherPath, "stun");
        aoc["monster_walk"] = GetAnimationClip(fatherPath, "walk");
        //GetSkillClips(fatherPath);
        int i;
        for (i = 1; i < 11; i++)
        {
            if (aoc["monster_attack0" + i.ToString()].name.StartsWith("monster"))
            {
                break;
            }
        }
        for (int j = 0; j < skillClips.Count; j++)
        {
            if (i + j <= 10)
            {

                aoc["monster_attack0" + (i + j).ToString()] = skillClips[j];
                //Debug.Log(aoc["monster_attack0" + (i + j).ToString()].name);
            }
        }
        //str = inputField.text;
        //if (inputField.text.StartsWith("layer"))
        //{
        //   str = inputField.text.Split('/')[1];
        //}

        //aoc["monster_walk"] = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Artist/Character/Monster/m_emfw_0001_0001/Animations/m_emfw_0001_walk.FBX") as AnimationClip;

        AssetDatabase.CreateAsset(aoc, fatherPath + "/" + str);
        AssetDatabase.SaveAssets();

    }

    static AnimationClip GetAnimationClip(string path, string name)
    {
        if (Directory.Exists(path))
        {
            var dirctory = new DirectoryInfo(path);
            var files = dirctory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta")) continue;
                if (files[i].Name.EndsWith(name + ".FBX"))
                {

                    return AssetDatabase.LoadAssetAtPath<AnimationClip>(path + "/" + files[i].Name) as AnimationClip;

                }
            }
        }
        return null;
    }

    static void GetSkillClips(string path)
    {
        skillClips.Clear();
        if (Directory.Exists(path))
        {
            var dirctory = new DirectoryInfo(path);
            var files = dirctory.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta")) continue;
                if (files[i].Name.EndsWith(".FBX") && files[i].Name.Contains("skill"))
                {

                    skillClips.Add(AssetDatabase.LoadAssetAtPath<AnimationClip>(path + "/" + files[i].Name) as AnimationClip);

                }
            }
        }
    }
}
#endif