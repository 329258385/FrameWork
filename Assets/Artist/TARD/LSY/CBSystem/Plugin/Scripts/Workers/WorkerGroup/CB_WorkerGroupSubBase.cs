using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CB_WorkerGroupSubBase : MonoBehaviour {
	public CB_MaterialParams matParam;
	public Transform root;
	public Material mat;
	public CB_LightCB lightCB;

	public virtual void On_Enable(Material m)
	{
		lightCB = GetComponentInChildren<CB_LightCB> ();
		lightCB.On_Enable ();

		if(CB_Lib.MatNeedCreate(mat,m))
			mat = new Material (m);

		SetShadowMap ();
		MatSyncAll ();
	}

	protected virtual void Update()
	{
		MatSyncAllEditor ();
	}

	protected void MatSyncAllEditor()
	{
		#if UNITY_EDITOR
		MatSyncAll();
		#endif
	}

	public void MatSyncAll()
	{
		matParam.Sync(mat);
	}
	public virtual void On_Disable()
	{
		lightCB.On_Disable ();
	}

	protected virtual void SetShadowMap()
	{

	}

	public virtual void Render(CB_WorkerGroupBase group)
	{
		
	}

	public void SetupLightVPMatrix()
	{
		if(mat!=null && lightCB!=null)
			mat.SetMatrix("worldToLight", lightCB.GetMatrixVP());
	}
}
