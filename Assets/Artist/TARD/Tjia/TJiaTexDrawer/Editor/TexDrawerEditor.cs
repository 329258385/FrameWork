using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TexDrawer))]
public class TexDrawerEditor : Editor
{

    public TexDrawer mTarget;

    //Vector3 HitPos;

    void OnEnable()
    {
        mTarget = (TexDrawer)target;
    }

    void OnSceneGUI()
    {
        if (mTarget.Edit)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Event e = Event.current;

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 20000, mTarget.DrawLayer))
            {
                //mTarget.HitPos = hitInfo.point;
                //Shader.SetGlobalVector("_BrushPos", hitInfo.textureCoord);
                bool paint = false;
                bool finish = false;
                //Debug.Log(HitPos);
                if (e.button == 0 && (e.type == EventType.MouseDown) && !e.alt)
                {
                    if (mTarget.Target != null)
                    {
                        mTarget.Target.SaveLastStep();
                    }
                }
                if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && !e.alt)
                {
                    paint = true;
                }
                else if (e.button == 0 && e.type == EventType.MouseUp && !e.alt)
                {
                    finish = true;
                }
                mTarget.UpdateHitInfo(hitInfo.transform, hitInfo.textureCoord, paint, finish);
            }
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.LeftBracket)
                {
                    mTarget.BrushSizeModifer(0.707f);
                }
                if (e.keyCode == KeyCode.RightBracket)
                {
                    mTarget.BrushSizeModifer(1.414f);
                }
                if (e.keyCode == KeyCode.X)
                {
                    if (mTarget.BrushColor != Color.black)
                    {
                        mTarget.BrushColor = Color.black;
                    }
                    else
                    {
                        mTarget.BrushColor = Color.white;
                    }
                }
                if (e.keyCode == KeyCode.L)
                {
                    mTarget.ResetOneStep();
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {

        if (mTarget.Edit)
        {
            if (GUILayout.Button("♌♌♌♌ 保存 ♌♌♌♌"))
            {
                mTarget.SavePic();
            }
        }
        base.OnInspectorGUI();
        //mTarget.Edit = EditorGUILayout.Toggle("Reset", mTarget.Edit);

    }
}
