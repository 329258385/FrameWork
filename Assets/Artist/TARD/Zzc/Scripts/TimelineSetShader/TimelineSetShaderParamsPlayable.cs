using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineSetShaderParamsPlayable : PlayableAsset
{

    //public ExposedReference<GameObject> timelineSetShaderParams;

    public int clipIndex;
    public string trackName;
    public AnimationCurve curve;    
    public string timelineGameObjName;
    public string targetGameObjName;


    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        TimelineSetShaderParams playableBehaviour = new TimelineSetShaderParams();
        playableBehaviour.curve = curve;
        playableBehaviour.timelineGameObjName = timelineGameObjName;
        playableBehaviour.targetGameObjName = targetGameObjName;
        playableBehaviour.clipIndex = clipIndex;
        playableBehaviour.trackName = trackName;
        return ScriptPlayable<TimelineSetShaderParams>.Create(graph, playableBehaviour);
    }
}
