using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineSetShaderParams : PlayableBehaviour
{
    private SkinnedMeshRenderer smr;
    private Material mat;
    public AnimationCurve curve;
    private PlayableDirector timeline;
    private TimelineClip timelineClip;
    public int clipIndex;
    public string trackName;
    private GameObject timelineObj;
    public string timelineGameObjName;
    public string targetGameObjName;
    private GameObject smrObj;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        smrObj = GameObject.Find(targetGameObjName);
        if (!smrObj)
        {
            string[] ss = targetGameObjName.Split('/');
            ss[0]=ss[0]+ "(Clone)";
            string s="";
            for (int i = 0; i < ss.Length; i++)
            {
                s = s + ss[i];
                if (i!=ss.Length-1)
                {
                    s = s + "/";
                }
            }
            //Debug.Log(s);
            smrObj = GameObject.Find(s);
        }
        if (smrObj)
        {
            smr = smrObj.GetComponent<SkinnedMeshRenderer>();
        }     
        //mat = smr.sharedMaterial;

        timelineObj = GameObject.Find(timelineGameObjName);
        if (!timelineObj)
        {
            string[] ss = timelineGameObjName.Split('/');
            ss[0] = ss[0] + "(Clone)";
            string s = "";
            for (int i = 0; i < ss.Length; i++)
            {
                s = s + ss[i];
                if (i != ss.Length - 1)
                {
                    s = s + "/";
                }
            }
            timelineObj = GameObject.Find(s);
        }
        if (timelineObj)
        {
            timeline = timelineObj.GetComponent<PlayableDirector>();
        }

        if (timeline)
        {
            var timelineAsset = timeline.playableAsset as TimelineAsset;

            for (int i = 0; i < timelineAsset.GetOutputTracks().Count(); i++)
            {
                var playableTrack = timelineAsset.GetOutputTrack(i) as PlayableTrack;

                if (playableTrack != null)
                {
                    if (trackName == playableTrack.name)
                    {
                        if (playableTrack.GetClips().ToList().Count > clipIndex)
                            timelineClip = playableTrack.GetClips().ToList()[clipIndex];
                    }                   
                }
            }
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        
        float left = (float)(timeline.time - timelineClip.start);
        float all = (float)(timelineClip.end - timelineClip.start);
        float time = left / all;
        smr.material.SetFloat("_DisPercentage", curve.Evaluate(time));        
    }
}
