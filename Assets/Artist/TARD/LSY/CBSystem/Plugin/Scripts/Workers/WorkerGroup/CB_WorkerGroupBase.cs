using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CB_WorkerGroupBase : CB_WorkerBase {
	//public RenderTexture testRT;
	public int Resolution = 512;
	public List<CB_WorkerGroupSubBase> subs;
	public bool blurOn = true;
	public static bool _reversedZ = false;

	public Material mat_1_jitterOrigin;
	public Material mat_2_blurOrigin;
	public Material mat_2_blur;

	public override void OnEnable ()
	{
		base.OnEnable ();
		subs.Clear ();
		subs.AddRange( GetComponentsInChildren<CB_WorkerGroupSubBase> ());

		#if UNITY_5_5_OR_NEWER
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 ||
			SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal || SystemInfo.graphicsDeviceType == GraphicsDeviceType.PlayStation4 ||
			SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan || SystemInfo.graphicsDeviceType == GraphicsDeviceType.XboxOne)
		{
			_reversedZ = true;
		}
		#endif

		foreach (var item in subs) {
			item.On_Enable (mat_1_jitterOrigin);
		}

		if(CB_Lib.MatNeedCreate(mat_2_blur,mat_2_blurOrigin))
			mat_2_blur = new Material (mat_2_blurOrigin);

		//testRT = new RenderTexture (Resolution, Resolution, 0);
	}

	public override void OnDisable ()
	{
		base.OnDisable ();
		foreach (var item in subs) {
			if (item == null)
				continue;
			item.On_Disable ();
		}
	}

	protected override void OnDrawGizmosSelected ()
	{
		foreach (var item in subs) {
			DrawMesh (item.root,item.mat);
		}
	}

	public override void MatSyncAll ()
	{
		base.MatSyncAll ();
		matParamsOther [0].Sync (mat_2_blur);
	}
}