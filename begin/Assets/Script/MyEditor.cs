using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(EditorTest))]
[ExecuteInEditMode]
public class MyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorTest editorTest = (EditorTest) target;
        editorTest.mRectValue = EditorGUILayout.RectField("窗口坐标", editorTest.mRectValue);
        editorTest.texture = EditorGUILayout.ObjectField("增加一个贴图",editorTest.texture,typeof(Texture),true) as Texture;
    }
}
