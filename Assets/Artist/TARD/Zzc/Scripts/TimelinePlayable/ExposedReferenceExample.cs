using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ExposedReferenceExample : PlayableAsset
{
    public ExposedReference<GameObject> mySceneObject;
    public Vector3 sceneObjectVelocity;



    public override Playable CreatePlayable(PlayableGraph graph, GameObject myGameObject)
    {
        GetWeapon playableBehaviour = new GetWeapon();
        //playableBehaviour.mySceneObject = mySceneObject.Resolve(graph.GetResolver());
       // playableBehaviour.sceneObjectVelocity = sceneObjectVelocity;
        return ScriptPlayable<GetWeapon>.Create(graph, playableBehaviour);
    }

}
