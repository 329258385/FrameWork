using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZzcPlanarReflection : MonoBehaviour
{

    private Camera reflectionCamera = null;
    public Camera mainCam;
    public RenderTexture reflectionRT = null;
    private static bool isReflectionCameraRendering = false;
    private Material reflectionMaterial = null;

    private void OnWillRenderObject()
    {
        if (isReflectionCameraRendering || !mainCam)
        {
            return;
        }

        isReflectionCameraRendering = true;

        if (reflectionCamera == null)
        {
            var go = new GameObject("Reflection Camera");
            reflectionCamera = go.AddComponent<Camera>();
            reflectionCamera.CopyFrom(mainCam);
        }

        UpdateCameraParams(mainCam, reflectionCamera);
        reflectionCamera.targetTexture = reflectionRT;
        reflectionCamera.enabled = false;

        var reflectM = CaculateReflectMatrix(transform.up, transform.position);

        reflectionCamera.worldToCameraMatrix = mainCam.worldToCameraMatrix * reflectM;

        GL.invertCulling = true;
        reflectionCamera.Render();
        GL.invertCulling = false;

        if (reflectionMaterial == null)
        {
            var renderer = GetComponent<Renderer>();
            reflectionMaterial = renderer.sharedMaterial;
        }
        reflectionMaterial.SetTexture("_ReflectionTex", reflectionRT);
        isReflectionCameraRendering = false;
    }


    Matrix4x4 CaculateReflectMatrix(Vector3 normal, Vector3 positionOnPlane)
    {
        float xx = 2 * normal.x * normal.x;
        float xy = 2 * normal.x * normal.y;
        float xz = 2 * normal.x * normal.z;
        float yy = 2 * normal.y * normal.y;
        float yz = 2 * normal.y * normal.z;
        float zz = 2 * normal.z * normal.z;

        var d = -Vector3.Dot(normal, positionOnPlane);
        var reflectM = new Matrix4x4();
        reflectM.m00 = 1 - xx;
        reflectM.m01 = -xy;
        reflectM.m02 = -xz;
        reflectM.m03 = -2 * d * normal.x;

        reflectM.m10 = -xy;
        reflectM.m11 = 1 - yy;
        reflectM.m12 = -yz;
        reflectM.m13 = -2 * d * normal.y;

        reflectM.m20 = -xz;
        reflectM.m21 = -yz;
        reflectM.m22 = 1 - zz;
        reflectM.m23 = -2 * d * normal.z;

        reflectM.m30 = 0;
        reflectM.m31 = 0;
        reflectM.m32 = 0;
        reflectM.m33 = 1;

        return reflectM;
    }

    private void UpdateCameraParams(Camera srcCamera, Camera destCamera)
    {
        if (destCamera == null || srcCamera == null)
        {
            return;
        }

        destCamera.clearFlags = CameraClearFlags.SolidColor;
        destCamera.backgroundColor = new Color(0, 0, 0, 0);
        destCamera.farClipPlane = srcCamera.farClipPlane;
        destCamera.nearClipPlane = srcCamera.nearClipPlane;
        destCamera.orthographic = srcCamera.orthographic;
        destCamera.fieldOfView = srcCamera.fieldOfView;
        destCamera.aspect = srcCamera.aspect;
        destCamera.orthographicSize = srcCamera.orthographicSize;
        destCamera.cullingMask = 2 << 9;
        destCamera.lensShift = srcCamera.lensShift;
    }

    private void OnDestroy()
    {
        Destroy(reflectionCamera);
        reflectionRT.Release();
    }
}
