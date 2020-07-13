//
// Kino/Bloom v2 - Bloom filter for Unity
//
// Copyright (C) 2015, 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using UnityEditor;

namespace Kino
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Bloom))]
    public class BloomEditor : Editor
    {
        BloomGraphDrawer _graph;

		SerializedProperty _Lock;
        SerializedProperty _threshold;
        SerializedProperty _softKnee;
        SerializedProperty _radius;
        SerializedProperty _intensity;
       // SerializedProperty _highQuality;
        SerializedProperty _antiFlicker;
        static GUIContent _textThreshold = new GUIContent("阈值");

		SerializedProperty ex;
		SerializedProperty ex2;
		SerializedProperty _SoftFocus;
		static GUIContent _textLock = new GUIContent("锁参数");
		static GUIContent _textEX = new GUIContent("色调强化");
		static GUIContent _textEX2 = new GUIContent("色调强化2");
		static GUIContent _textSoftFocus = new GUIContent("柔焦");

		static GUIContent _textSoftKnee = new GUIContent("平滑");
		static GUIContent _textIntensity = new GUIContent("强度");
		static GUIContent _textRadius = new GUIContent("半径");
		static GUIContent _textAntiFlicker = new GUIContent("防抖动");
        void OnEnable()
        {
            _graph = new BloomGraphDrawer();
            _threshold = serializedObject.FindProperty("_threshold");
            _softKnee = serializedObject.FindProperty("_softKnee");
            _radius = serializedObject.FindProperty("_radius");
            _intensity = serializedObject.FindProperty("_intensity");
            //_highQuality = serializedObject.FindProperty("_highQuality");
            _antiFlicker = serializedObject.FindProperty("_antiFlicker");

			ex = serializedObject.FindProperty("ex");
			ex2 = serializedObject.FindProperty("ex2");
			_SoftFocus = serializedObject.FindProperty("_SoftFocus");
			_Lock = serializedObject.FindProperty("lockParameters");
        }

        public override void OnInspectorGUI()
        {
			Bloom bloom = (Bloom)target;
            serializedObject.Update();

            if (!serializedObject.isEditingMultipleObjects) {
                EditorGUILayout.Space();
				_graph.Prepare(bloom);
                _graph.DrawGraph();
                EditorGUILayout.Space();
            }

			EditorGUILayout.PropertyField (_Lock,_textLock);
			if (!bloom.lockParameters) {
				EditorGUILayout.PropertyField (_threshold, _textThreshold);
				EditorGUILayout.PropertyField (_softKnee,_textSoftKnee);
				EditorGUILayout.PropertyField (_intensity,_textIntensity);
				EditorGUILayout.PropertyField (_radius,_textRadius);
				//EditorGUILayout.PropertyField(_highQuality);
				EditorGUILayout.PropertyField (_antiFlicker,_textAntiFlicker);

				EditorGUILayout.PropertyField (ex, _textEX, null);
				EditorGUILayout.PropertyField (ex2, _textEX2, null);
				EditorGUILayout.PropertyField (_SoftFocus, _textSoftFocus, null);
			}
			serializedObject.ApplyModifiedProperties ();
        }
    }
}
