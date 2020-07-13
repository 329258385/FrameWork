using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGrassInfo : MonoBehaviour {
	public Texture2D grassTex;

	public int width{ get; set;}
	public int height{ get; set;}

	private void Awake()
	{
        if (grassTex != null)
        {
            width = grassTex.width;
            height = grassTex.height;
        }
	}

	#if UNITY_EDITOR
	private void Update()
	{
        if (grassTex != null)
        {
            width = grassTex.width;
            height = grassTex.height;
        }
	}
	#endif
}
