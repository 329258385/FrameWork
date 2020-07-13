using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarReflection : MonoBehaviour {
	public Camera cam;
	public Camera refCam;


	private bool init = false;
	private Material mat;
	public bool hideObjects = true;
	public int textureSize = 512;
	public float clipPlaneOffset = 0.001f;//0.07f;
	public LayerMask reflectLayers = -1;

	RenderTexture reflTex = null;

	public void OnWillRenderObject() {
		Init ();

		SetRefCamPos ();
		UpdatePR ();
	}
	void Init()
	{
		if (init)
			return;
		init = true;
		mat = GetComponent<MeshRenderer> ().sharedMaterial;
		cam.depthTextureMode = DepthTextureMode.Depth;

		CreateCam ();
		CopyCam (cam, refCam);
		CreateRT ();
		refCam.targetTexture = reflTex;
		mat.SetTexture ("_ReflectionTex", reflTex);
	}

	void CreateCam()
	{
		GameObject go = new GameObject("Water Refl Camera id" + GetInstanceID() + " for " + cam.GetInstanceID(), typeof(Camera), typeof(Skybox));
		refCam = go.GetComponent<Camera>();
		refCam.enabled = false;
		refCam.transform.position = transform.position;
		refCam.transform.rotation = transform.rotation;
		refCam.gameObject.AddComponent<FlareLayer>();
		go.hideFlags = hideObjects ? HideFlags.HideAndDontSave : HideFlags.DontSave;
	}

	void CreateRT()
	{
		reflTex = new RenderTexture(textureSize, textureSize, 16);
		reflTex.name = "Lsy RT-Planar Reflection " + GetInstanceID();
		reflTex.isPowerOfTwo = true;
		reflTex.hideFlags = HideFlags.DontSave;
	}

	void SetRefCamPos()
	{
		Vector3 pos = transform.position;
		Vector3 normal = transform.up;
		// Reflect camera around reflection plane
		float d = -Vector3.Dot(normal, pos) - clipPlaneOffset;
		Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

		Matrix4x4 reflection = Matrix4x4.zero;
		CalculateReflectionMatrix(ref reflection, reflectionPlane);
		Vector3 oldpos = cam.transform.position;
		Vector3 newpos = reflection.MultiplyPoint(oldpos);
		refCam.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

		// Setup oblique projection matrix so that near plane is our reflection
		// plane. This way we clip everything below/above it for free.
		Vector4 clipPlane = CameraSpacePlane(refCam, pos, normal, 1.0f);
		refCam.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

		refCam.cullingMask = ~(1 << 4) & reflectLayers.value; // never render water layer
		refCam.targetTexture = reflTex;
		GL.invertCulling = true;
		refCam.transform.position = newpos;
		Vector3 euler = cam.transform.eulerAngles;
		refCam.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
		refCam.Render();
		refCam.transform.position = oldpos;
		GL.invertCulling = false;
		//GetComponent<Renderer>().sharedMaterial.SetTexture("_ReflectionTex", p.reflTex);
	}

	void UpdatePR()
	{
		var ma = CamPassVPMatrix (refCam);
		mat.SetMatrix ("refMA", ma);
	}


	#region common
	Matrix4x4 CamPassVPMatrix(Camera cam)
	{
		Matrix4x4 ma = cam.previousViewProjectionMatrix;
		ma = cam.projectionMatrix * cam.worldToCameraMatrix;
		return ma;
	}

	void CopyCam(Camera src, Camera dest) {
		if (dest == null) {
			return;
		}
		// set water camera to clear the same way as current camera
		dest.clearFlags = src.clearFlags;
		var bgCol = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox) {
			Skybox sky = src.GetComponent<Skybox>();
			Skybox mysky = dest.GetComponent<Skybox>();
			if (!sky || !sky.material) {
				mysky.enabled = false;
			} else {
				mysky.enabled = true;
				mysky.material = sky.material;
			}

			if (RenderSettings.skybox && RenderSettings.skybox.HasProperty("_GroundColor")) {
				bgCol = RenderSettings.skybox.GetColor("_GroundColor");
				src.backgroundColor = bgCol;
			}
		}
		dest.backgroundColor = bgCol;
		// update other values to match current camera.
		// even if we are supplying custom camera&projection matrices,
		// some of values are used elsewhere (e.g. skybox uses far plane)
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		//dest.fieldOfView = src.fieldOfView;
        GameHelper.SetCameraFOV(dest, src.fieldOfView);
        dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	// Given position/normal of the plane, calculates plane in camera space.
	Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign) {
		Vector3 offsetPos = pos + normal * clipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}

	// Calculates reflection matrix around the given plane
	static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane) {
		reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
		reflectionMat.m01 = (-2F * plane[0] * plane[1]);
		reflectionMat.m02 = (-2F * plane[0] * plane[2]);
		reflectionMat.m03 = (-2F * plane[3] * plane[0]);

		reflectionMat.m10 = (-2F * plane[1] * plane[0]);
		reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
		reflectionMat.m12 = (-2F * plane[1] * plane[2]);
		reflectionMat.m13 = (-2F * plane[3] * plane[1]);

		reflectionMat.m20 = (-2F * plane[2] * plane[0]);
		reflectionMat.m21 = (-2F * plane[2] * plane[1]);
		reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
		reflectionMat.m23 = (-2F * plane[3] * plane[2]);

		reflectionMat.m30 = 0F;
		reflectionMat.m31 = 0F;
		reflectionMat.m32 = 0F;
		reflectionMat.m33 = 1F;
	}

	#endregion
}
