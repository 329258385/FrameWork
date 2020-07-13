using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GetWeaponPlayable : PlayableAsset {

    public ExposedReference<GameObject> getWeapon;

    public string nodeName;
    public GameObject weapon;
    [HideInInspector]
    public int count;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        GetWeapon playableBehaviour = new GetWeapon();
        playableBehaviour.nodeName = nodeName;
        playableBehaviour.weapon = weapon;
        playableBehaviour.count = count;
        return ScriptPlayable<GetWeapon>.Create(graph, playableBehaviour);
    }


}
