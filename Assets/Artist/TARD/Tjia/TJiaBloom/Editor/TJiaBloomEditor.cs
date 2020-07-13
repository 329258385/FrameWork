using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(TJiaBloom))]
public class TJiaBloomEditor : Editor {

    SerializedProperty mFastBloom;
    SerializedProperty mResolution;
    SerializedProperty mThreshold;
    SerializedProperty mSoftKnee;
    SerializedProperty mIntensity;
    SerializedProperty mRadius;
    SerializedProperty mAntiFlicker;
    SerializedProperty mSoftFocus;
    
    static GUIContent mTextFastBloom = new GUIContent("快速Bloom");
    static GUIContent mTextResolution = new GUIContent("分辨率(尽量小)");
    static GUIContent mTextThreshold = new GUIContent("阈值 (Gamma)");
    static GUIContent mTextSoftKnee = new GUIContent("拐点");
    static GUIContent mTextIntensity = new GUIContent("强度");
    static GUIContent mTextRadius = new GUIContent("半径（尽量小）");
    static GUIContent mTextAntiFlicker = new GUIContent("防闪(不闪不开)");
    static GUIContent mTextSoftFocus = new GUIContent("柔焦");
    

    private void OnEnable()
    {
        mFastBloom = serializedObject.FindProperty("FastBloom");
        mResolution = serializedObject.FindProperty("_Resolution");
        mThreshold = serializedObject.FindProperty("_Threshold");
        mSoftKnee = serializedObject.FindProperty("_SoftKnee");
        mIntensity = serializedObject.FindProperty("_Intensity");
        mRadius = serializedObject.FindProperty("_Radius");
        mAntiFlicker = serializedObject.FindProperty("_AntiFlicker");
        mSoftFocus = serializedObject.FindProperty("_SoftFocus");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.Space();
        PrepareGraph();
        DrawGraph();
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(mFastBloom, mTextFastBloom);
        EditorGUILayout.PropertyField(mResolution, mTextResolution);        
        EditorGUILayout.PropertyField(mThreshold, mTextThreshold);
        EditorGUILayout.PropertyField(mSoftKnee, mTextSoftKnee);
        EditorGUILayout.PropertyField(mIntensity, mTextIntensity);
        EditorGUILayout.PropertyField(mRadius, mTextRadius);
        EditorGUILayout.PropertyField(mAntiFlicker, mTextAntiFlicker);
        EditorGUILayout.PropertyField(mSoftFocus, mTextSoftFocus);
        serializedObject.ApplyModifiedProperties();
    }

    float mRangeX = 5;
    float mRangeY = 2;
    float mGraphThreshold;
    float mGraphSoftKnee;
    float mGraphIntensity;
    float mGraphSoftFocus;
    Vector3[] mRectVertices = new Vector3[4];
    Vector3[] mLineVertices = new Vector3[2];
    Rect mRectGraph;
    const int mCurveResolution = 48;
    Vector3[] mCurveVertices = new Vector3[mCurveResolution];

    private void PrepareGraph()
    {
        var bloom = (TJiaBloom)target;
        mGraphThreshold = bloom.ThresholdLinear;
        mGraphSoftKnee = bloom._SoftKnee * mGraphThreshold + 1e-5f;
        mGraphIntensity = Mathf.Min(10, bloom._Intensity);
        mGraphSoftFocus = bloom._SoftFocus;
    }

    private Vector3 PointInRect(float x, float y)
    {
        x = Mathf.Lerp(mRectGraph.x, mRectGraph.xMax, x / mRangeX);
        y = Mathf.Lerp(mRectGraph.yMax, mRectGraph.y, y / mRangeY);
        return new Vector3(x, y, 0);
    }

    void DrawRect(float x1, float y1, float x2, float y2, Color fill, Color line)
    {
        mRectVertices[0] = PointInRect(x1, y1);
        mRectVertices[1] = PointInRect(x2, y1);
        mRectVertices[2] = PointInRect(x2, y2);
        mRectVertices[3] = PointInRect(x1, y2);
        Handles.DrawSolidRectangleWithOutline(mRectVertices, fill, line);
    }

    private void DrawLine(float x1, float y1, float x2, float y2, Color color)
    {
        mLineVertices[0] = PointInRect(x1, y1);
        mLineVertices[1] = PointInRect(x2, y2);
        Handles.color = color;
        Handles.DrawAAPolyLine(2.0f, mLineVertices);
    }

    private float ResponseFunction(float x)
    {
        var rq = Mathf.Clamp(x - mGraphThreshold + mGraphSoftKnee, 0, mGraphSoftKnee * 2);
        rq = rq * rq * 0.25f / mGraphSoftKnee;
        return Mathf.Max(rq, x - mGraphThreshold) * mGraphIntensity;// + mGraphSoftFocus;
    }

    private void DrawGraph()
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Space(EditorGUI.indentLevel * 15f);
            mRectGraph = GUILayoutUtility.GetRect(128, 80);
        }
        //BG
        DrawRect(0, 0, mRangeX, mRangeY, Color.blue * 0.3f, Color.yellow * 0.8f);
        //SoftKnee
        DrawRect(mGraphThreshold - mGraphSoftKnee, 0, mGraphThreshold + mGraphSoftKnee, mRangeY, Color.cyan * 0.4f, Color.clear);
        //VLines
        DrawLine(1, 0, 1, mRangeY, Color.yellow);
        for (float i = 2; i < mRangeX; i++)
        {
            DrawLine(i, 0, i, mRangeY, Color.yellow * 0.8f);
        }
        //ThresholdLine
        DrawLine(mGraphThreshold, 0, mGraphThreshold, mRangeY, Color.cyan);
        //HLines
        for (float i = 1; i < mRangeY; i++)
        {
            DrawLine(0, i, mRangeX, i, Color.yellow * 0.8f);
        }
        Handles.Label(PointInRect(0, mRangeY) + Vector3.right, "亮度响应(线性)", EditorStyles.largeLabel);
        //ResponseLine
        int vcount = 0;
        while (vcount < mCurveResolution)
        {
            var x = mRangeX * vcount / (mCurveResolution - 1);
            var y = ResponseFunction(x);
            if (y < mRangeY)
            {
                mCurveVertices[vcount] = PointInRect(x, y);
                vcount++;
            }
            else
            {
                if (vcount > 1)
                {
                    var v1 = mCurveVertices[vcount - 2];
                    var v2 = mCurveVertices[vcount - 1];
                    var clip = (mRectGraph.y - v1.y) / (v2.y - v1.y);
                    mCurveVertices[vcount - 1] = v1 + (v2 - v1) * clip;
                }
                break;
            }
        }
        if (vcount > 1)
        {
            Handles.color = Color.cyan * 0.5f + Color.white * 0.5f;
            Handles.DrawAAPolyLine(2.0f, vcount, mCurveVertices);
        }
    }
}
