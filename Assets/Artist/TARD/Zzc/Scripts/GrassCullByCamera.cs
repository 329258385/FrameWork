using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassCullByCamera : MonoBehaviour {

	public Transform camTrans;
	public float cullDis1=3f;
	public float cullDis2=5f;
	private TJiaGrassGenerator gg;

	const float NULL_VALUE = -99999;
	private Vector3 LastPos = new Vector3(NULL_VALUE, NULL_VALUE, NULL_VALUE);
	private float last_cullDis1 = NULL_VALUE;
	private float last_cullDis2 = NULL_VALUE;

	private void Start()
	{
		gg = Camera.main.GetComponent<TJiaGrassGenerator>();
        Shader.SetGlobalVector("_CullDis", Vector4.zero);
        Shader.SetGlobalVector("_CamPos1", Vector4.zero);
    }

	bool FloatEqualsZero(float value)
	{
		return value >= -0.000001f && value <= 0.000001f;
	}

	private void Update()
	{

		if (camTrans==null)
		{
			return;
		}

		Vector3 newPos = camTrans.position;
		if (newPos == LastPos && FloatEqualsZero(cullDis1 - last_cullDis1) && FloatEqualsZero(cullDis2 - last_cullDis2))
			return;
		LastPos = newPos;
		last_cullDis1 = cullDis1;
		last_cullDis2 = cullDis2;

        //gg.IMs[0].InstanceMaterial.SetVector("_CamPos1", new Vector4(newPos.x, newPos.y, newPos.z, 0));
        //gg.IMs[0].InstanceMaterial.SetVector("_CullDis", new Vector4(cullDis1, cullDis2, 0, 0));

        Shader.SetGlobalVector("_CullDis", new Vector4(cullDis1, cullDis2, 0, 0));
        Shader.SetGlobalVector("_CamPos1", new Vector4(newPos.x, newPos.y, newPos.z, 0));
	}



    private void OnDisable()
    {
        Shader.SetGlobalVector("_CullDis", Vector4.zero);
        Shader.SetGlobalVector("_CamPos1", Vector4.zero);
    }
}
