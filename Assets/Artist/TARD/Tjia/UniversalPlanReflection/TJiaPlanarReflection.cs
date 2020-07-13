using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TJiaPlanarReflection : MonoBehaviour {

    [HideInInspector] public Camera mReflectionCamera;
    [HideInInspector] public RenderTexture mReflectionRT;
    private static bool mIsReflectionCameraRendering = false;
    private Material mReflectionMaterial;

    public bool ReInit = false;
    public TexResOption RefResolution = TexResOption.R1024;
    private TexResOption mRefResolutionBuffer = TexResOption.R1024;
    [Space(10)]
    public float Displacement = 0;

    public enum TexResOption
    {
        R256 = 256,
        R512 = 512,
        R1024 = 1024,
        R2048 = 2048
    }

    void OnDisable()
    {
        if (mReflectionRT != null)
        {
            //RenderTexture.ReleaseTemporary(mReflectionRT);
            mReflectionRT.Release();
        }
    }

    void OnEnable()
    {
        ReInit = true;
        mIsReflectionCameraRendering = false;
        Camera[] camList = GetComponentsInChildren<Camera>();
        for (int i = 0; i < camList.Length; i++)
        {
            if (Application.isPlaying)
            {
                Destroy(camList[i].gameObject);
            }
            else
            {
                DestroyImmediate(camList[i].gameObject);
            }
        }
    }

    void OnWillRenderObject()
    {
        if (mIsReflectionCameraRendering)
        {
            return;
        }

        if (ReInit)
        {
            Camera[] camList = GetComponentsInChildren<Camera>();
            for (int i = 0; i < camList.Length; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(camList[i].gameObject);
                }
                else
                {
                    DestroyImmediate(camList[i].gameObject);
                }
            }

            ReInit = false;

            if (mReflectionRT != null)
            {
                mReflectionRT.Release();
            }
        }

        mIsReflectionCameraRendering = true;
        if (mReflectionCamera == null)
        {
            GameObject go = new GameObject("Reflection Camera");
            mReflectionCamera = go.AddComponent<Camera>();
            mReflectionCamera.CopyFrom(Camera.current);
            mReflectionCamera.enabled = false;
            go.transform.parent = transform;
            if (mReflectionRT != null)
            {
                mReflectionCamera.targetTexture = mReflectionRT;
            }
        }
        if (mReflectionRT == null || mRefResolutionBuffer != RefResolution)
        {
            if (mReflectionRT != null)
            {
                RenderTexture.ReleaseTemporary(mReflectionRT);
            }
            mRefResolutionBuffer = RefResolution;
            int resolution = (int)RefResolution;
            mReflectionRT = RenderTexture.GetTemporary(resolution * 2, resolution, 24);
            mReflectionCamera.targetTexture = mReflectionRT;
        }

        UpdateCameraParams(Camera.current, mReflectionCamera);


        Vector3 normal = transform.up;
        Vector3 pos = transform.position;
        pos += Displacement * normal;
        float d = -Vector3.Dot(normal, pos);

        Matrix4x4 reflectMatrix = GetReflectMatrix(normal, d);
        mReflectionCamera.worldToCameraMatrix = Camera.current.worldToCameraMatrix * reflectMatrix;

        var plane = new Vector4(normal.x, normal.y, normal.z, d);
        var viewSpacePlane = mReflectionCamera.worldToCameraMatrix.inverse.transpose * plane;
        var clipMatrix = mReflectionCamera.CalculateObliqueMatrix(viewSpacePlane);
        mReflectionCamera.projectionMatrix = clipMatrix;



        GL.invertCulling = true;
        mReflectionCamera.Render();
        GL.invertCulling = false;

        if (mReflectionMaterial == null)
        {
            Renderer r = GetComponent<Renderer>();
            mReflectionMaterial = r.sharedMaterial;
        }
        mReflectionMaterial.SetTexture("_ReflectionTex", mReflectionRT);

        mIsReflectionCameraRendering = false;
    }

    private Matrix4x4 GetReflectMatrix(Vector3 normal, float d) //position should be on the plan
    {
        Matrix4x4 res = new Matrix4x4();

        //d -= Displacement;

        float xx = normal.x * normal.x;
        float yy = normal.y * normal.y;
        float zz = normal.z * normal.z;
        float xy = normal.x * normal.y;
        float xz = normal.x * normal.z;
        float yz = normal.y * normal.z;

        res.m00 = 1 - 2 * xx;
        res.m01 =   - 2 * xy;
        res.m02 =   - 2 * xz;
        res.m03 =   - 2 * d * normal.x;

        res.m10 =   - 2 * xy;
        res.m11 = 1 - 2 * yy;
        res.m12 =   - 2 * yz;
        res.m13 =   - 2 * d * normal.y;

        res.m20 =   - 2 * xz;
        res.m21 =   - 2 * yz;
        res.m22 = 1 - 2 * zz;
        res.m23 =   - 2 * d * normal.z;

        res.m30 = 0;
        res.m31 = 0;
        res.m32 = 0;
        res.m33 = 1;

        return res;
    }
    private Matrix4x4 GetReflectMatrix(Vector3 pos) //up
    {
        Matrix4x4 res = new Matrix4x4();

        float d = -pos.y;// - Displacement;

        res.m00 = 1;
        res.m01 = 0;
        res.m02 = 0;
        res.m03 = 0;

        res.m10 = 0;
        res.m11 = -1;
        res.m12 = 0;
        res.m13 = -2 * d;

        res.m20 = 0;
        res.m21 = 0;
        res.m22 = 1;
        res.m23 = 0;

        res.m30 = 0;
        res.m31 = 0;
        res.m32 = 0;
        res.m33 = 1;

        return res;
    }

    private void UpdateCameraParams(Camera src, Camera dest)
    {
        if (src == null || dest == null)
        {
            return;
        }
        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
        dest.useOcclusionCulling = false;
    }
}
