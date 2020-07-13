using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LsyWater_ScreenFX : MonoBehaviour {
	Camera mCam;
	public Material mat;

	void Awake()
	{
		mCam = GetComponent<Camera> ();
		//mCam.depthTextureMode = DepthTextureMode.Depth;
	}

	void OnRenderImage(RenderTexture source,RenderTexture dest)
	{
		if (mat != null)
		{

			float tanHalfFOV = Mathf.Tan(0.5f * mCam.fieldOfView * Mathf.Deg2Rad);
			float halfHeight = tanHalfFOV * mCam.nearClipPlane;
			float halfWidth = halfHeight * mCam.aspect;
			Vector3 toTop = mCam.transform.up * halfHeight;
			Vector3 toRight = mCam.transform.right * halfWidth;
			Vector3 forward = mCam.transform.forward * mCam.nearClipPlane;
			Vector3 toTopLeft = forward + toTop - toRight;
			Vector3 toBottomLeft = forward - toTop - toRight;
			Vector3 toTopRight = forward + toTop + toRight;
			Vector3 toBottomRight = forward - toTop + toRight;

			toTopLeft /= mCam.nearClipPlane;
			toBottomLeft /= mCam.nearClipPlane;
			toTopRight /= mCam.nearClipPlane;
			toBottomRight /= mCam.nearClipPlane;

			Matrix4x4 frustumDir = Matrix4x4.identity;
			frustumDir.SetRow(0, toBottomLeft);
			frustumDir.SetRow(1, toBottomRight);
			frustumDir.SetRow(2, toTopLeft);
			frustumDir.SetRow(3, toTopRight);


			mat.SetMatrix("_FrustumDir", frustumDir);
			mat.SetVector("_CameraForward", mCam.transform.forward);
			var p = mCam.transform.position;
			mat.SetVector("_CamPos", new Vector4(p.x,p.y,p.z,p.y<0?1:0));


			Graphics.Blit (source, dest, mat);
		}
		else
		{
			Graphics.Blit(source, dest);
		}
	}
}
