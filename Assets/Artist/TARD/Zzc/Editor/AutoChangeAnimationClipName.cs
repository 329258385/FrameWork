using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutoChangeAnimationClipName : AssetPostprocessor {
    //void OnPostprocessModel(GameObject root)
    //{
    //    /*
    //    if (assetPath.Contains(".FBX"))
    //    {
    //        string str = assetPath.Replace(".FBX", "");
    //        string[] strs = str.Split('/');
    //        string name = strs[strs.Length - 1];
    //        ModelImporter mi = assetImporter as ModelImporter;
    //        if (mi != null)
    //        {
                
    //            ModelImporterClipAnimation[] c = mi.defaultClipAnimations;
    //            if (mi.clipAnimations.Length>0)
    //            {
    //                return;
    //            }
    //            if (c != null && c.Length > 0)
    //            {
    //                if (c.Length > 1)
    //                {
    //                    return;
    //                }
    //                //Debug.Log(name);
    //                c[0].name = name;
    //                //Debug.Log(c[0].name);
    //            }
    //            else
    //            {
    //                return;
    //            }
    //            mi.clipAnimations = c;
    //            if (mi.clipAnimations.Length>0)
    //            {
    //                mi.clipAnimations[0].name = name;
    //            }
    //            else
    //            {
    //                return;
    //            }
    //            mi.animationCompression = ModelImporterAnimationCompression.Optimal;
    //            mi.SaveAndReimport();
    //        }
            
    //    }*/
    //}
}
