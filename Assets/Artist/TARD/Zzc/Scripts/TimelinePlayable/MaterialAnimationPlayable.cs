using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MaterialAnimationPlayable : PlayableAsset
{
    public ExposedReference<GameObject> materialAnimation;
    //public PlayableDirector timeline;
    //public string timelineGameObjName;
    public string targetGameObjName;
    public MaterialAnimationTimeline.MaterialIndex materialIndex;
    public MaterialAnimationTimeline.Part part;
    //public string NPCIndex;
    public GameObject testObj;
    public bool isCopy = false;
    public Vector4 tilingOffset;
    //public AnimationCurve animationCurveX = new AnimationCurve();
    //public AnimationCurve animationCurveY = new AnimationCurve();
    //public AnimationCurve animationCurveZ = new AnimationCurve();
    //public AnimationCurve animationCurveW = new AnimationCurve();
    //public int clipIndex;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        MaterialAnimationTimeline playableBehaviour = new MaterialAnimationTimeline();

        //playableBehaviour.timeline = timeline;
        //playableBehaviour.timelineGameObjName = timelineGameObjName;
        
        playableBehaviour.materialIndex = materialIndex;
        playableBehaviour.part = part;
        playableBehaviour.targetGameObjName = targetGameObjName;
        //playableBehaviour.NPCIndex = NPCIndex;
        playableBehaviour.testObj = testObj;
        playableBehaviour.isCopy = isCopy;
        playableBehaviour.tilingOffset = tilingOffset;
        //playableBehaviour.animationCurveX = animationCurveX;
        //playableBehaviour.animationCurveY = animationCurveY;
        //playableBehaviour.animationCurveZ = animationCurveZ;
        //playableBehaviour.animationCurveW = animationCurveW;
        //playableBehaviour.clipIndex = clipIndex;
        return ScriptPlayable<MaterialAnimationTimeline>.Create(graph, playableBehaviour);
    }
}
