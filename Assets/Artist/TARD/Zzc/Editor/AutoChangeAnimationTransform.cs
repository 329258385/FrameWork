using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutoChangeAnimationTransform {

    [MenuItem("Assets/SaoUtils/自动勾选Transform")]
    static void ShowName()
    {
        
        string[] name = Selection.assetGUIDs;

        for (int k = 0; k < name.Length; k++)
        {
            ModelImporter mi = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(name[k])) as ModelImporter;
            var clips = mi.clipAnimations;
            for (int i = 0; i < clips.Length; i++)
            {
                mi.CreateDefaultMaskForClip(clips[i]);
                clips[i].maskType = ClipAnimationMaskType.CreateFromThisModel;
                var mask = new AvatarMask();
                clips[i].ConfigureMaskFromClip(ref mask);
                for (int j = 0; j < mask.transformCount; j++)
                {
                    mask.SetTransformActive(j, true);
                }
                //Debug.Log(mask.transformCount);
                clips[i].ConfigureClipFromMask(mask);
                Object.DestroyImmediate(mask);


            }
            mi.clipAnimations = clips;
            mi.SaveAndReimport();
        }    
    }
}
