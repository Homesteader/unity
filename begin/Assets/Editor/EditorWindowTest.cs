using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorWindowTest : EditorWindow
{
    [MenuItem("MyMenu/openWindow")]
    static void OpenWindow()
    {
        Rect wr = new Rect(0, 0, 500, 500);
        EditorWindowTest windowTest = (EditorWindowTest) EditorWindow.GetWindowWithRect(typeof(EditorWindowTest),
            wr, true, "测试创建窗口");
        windowTest.Show();
    }

    private string text;
    private Texture tex;

    private void Awake()
    {
        tex = Resources.Load("1") as Texture;
    }

    private void OnGUI()
    {
        text = EditorGUILayout.TextField("输入文字", text);

        if (GUILayout.Button("打开通知",GUILayout.Width(200)))
        {
            this.ShowNotification(new GUIContent("This is a notification"));
        }

        if (GUILayout.Button("关闭通知",GUILayout.Width(200)))
        {
            this.RemoveNotification();
        }
        
        EditorGUILayout.LabelField("鼠标显示位置",Event.current.mousePosition.ToString());
        
        tex = EditorGUILayout.ObjectField("添加贴图",tex,typeof(Texture),true) as Texture;

        if (GUILayout.Button("关闭窗口",GUILayout.Width(200)))
        {
            this.Close();
        }
    }

    private void Update()
    {
        
    }

    private void OnFocus()
    {
        Debug.Log("当窗口获得焦点时调用一次");
    }

    private void OnLostFocus()
    {
        Debug.Log("当窗口失去焦点时调用一次");
    }

    private void OnHierarchyChange()
    {
        Debug.Log("当视图中任何对象改变时调用");
    }

    private void OnProjectChange()
    {
        Debug.Log("当工程视图中资源发生改变调用");
    }

    private void OnInspectorUpdate()
    {
        this.Repaint();
    }

    private void OnSelectionChange()
    {
        foreach (Transform t in Selection.transforms)
        {            
            Debug.Log("OnselectChange: " + t.name);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("窗口关闭");
    }
}
