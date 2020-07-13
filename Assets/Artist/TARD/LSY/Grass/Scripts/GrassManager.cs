using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassManager : MonoBehaviour {
	public Texture bigWave;
	public Texture smallWave;
	void Start () {
		Shader.SetGlobalTexture ("_WDirWaveTex", bigWave);
		Shader.SetGlobalTexture ("_WSubWaveTex", smallWave);

		Shader.SetGlobalFloat ("_WForce", 2f);
		Shader.SetGlobalVector ("_WDir", new Vector4 (1,0,0,0));
		Shader.SetGlobalVector ("_WDirSubSpdTiling", new Vector4 (3,30,3,8));
	}
}
