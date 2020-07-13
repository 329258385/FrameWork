using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MaterialAnimationTimeline : PlayableBehaviour
{

    public enum MaterialIndex
    {
        first,
        second,
        third,
        forth
    }

    public enum Part
    {
        playerFace,
        npcFace,
        
    }

    //private GameObject timelineObj;
    //public string timelineGameObjName;
    public string targetGameObjName;
    public PlayableDirector timeline;
    public MaterialIndex materialIndex;
    public Part part;
    //public string NPCIndex;
    public bool isCopy;
    public GameObject testObj;
    public Vector4 tilingOffset;
    //public AnimationCurve animationCurveX;
    //public AnimationCurve animationCurveY;
    //public AnimationCurve animationCurveZ;
    //public AnimationCurve animationCurveW;
    //public int clipIndex;
    //private float time;
    private Transform player;
    private SkinnedMeshRenderer smr;
    private Material mat;
    //private TimelineClip timelineClip;
    private int count;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //timelineObj = GameObject.Find(timelineGameObjName);
        //if (timelineObj)
        //{
        //    timeline = timelineObj.GetComponent<PlayableDirector>();
        //}

        //if (timeline)
        //{
        //    var timelineAsset = timeline.playableAsset as TimelineAsset;
        //    foreach (var track in timelineAsset.GetOutputTracks())
        //    {
        //        var playableTrack = track as PlayableTrack;
        //        if (playableTrack != null)
        //        {
        //            if (playableTrack.GetClips().ToList().Count > clipIndex)
        //                timelineClip = playableTrack.GetClips().ToList()[clipIndex];
        //        }
        //    }
        //}

        player = LsyCommon.FindPlayer();
        Transform display;
        if (part == Part.playerFace)
        {
            if (player)
            {
                display = player.GetChild(0);
                for (int i = 0; i < display.childCount; i++)
                {
                    if (display.GetChild(i).name.StartsWith("face"))
                    {
                        testObj = display.GetChild(i).gameObject;
                        break;
                    }
                }
            }
        }
        else if (part == Part.npcFace)
        {
            GameObject NPC;
            NPC = GameObject.Find(targetGameObjName);
            if (NPC)
                testObj = NPC;
        }


        if (testObj)
        {
            smr = testObj.GetComponent<SkinnedMeshRenderer>();
            if (smr)
            {
                if (count < 1)
                {
                    if ((int)materialIndex<=smr.materials.Length-1)
                    {
                        mat = smr.materials[(int)materialIndex];
                        count++;
                    }
                    
                }
                else
                {
                    if ((int)materialIndex<= smr.sharedMaterials.Length-1)
                    {
                        mat = smr.sharedMaterials[(int)materialIndex];
                    }                   
                }
            }
        }
    }

    public override void OnGraphStart(Playable playable)
    {

    }


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //if (timeline)
        //{
        //    float left = (float)(timeline.time - timelineClip.start);
        //    float all = (float)(timelineClip.end - timelineClip.start);
        //    time = left / all;
        //    X = animationCurveX.Evaluate(time);
        //    Y = animationCurveY.Evaluate(time);
        //    Z = animationCurveZ.Evaluate(time);
        //    W = animationCurveW.Evaluate(time);
        //}
        if (mat)
        {
            mat.SetVector("_MainTex_ST", tilingOffset);
            mat.SetVector("_LightMap_ST", tilingOffset);
        }
    }

    public override void OnGraphStop(Playable playable)
    {
        //Resources.UnloadUnusedAssets();
    }
}
