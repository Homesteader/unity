using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FastKey {

    [MenuItem("FastKey/Go UICreate &#1", false, 0)]
    static void GoUICreateScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(Application.dataPath + "/Scene/UICreate.unity");
        }
    }

    [MenuItem("FastKey/Go Login &#2", false, 0)]
    static void GoLoginScene()
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(Application.dataPath + "/Scene/Login.unity");
        }
    }

    [MenuItem("FastKey/Go Home &#3", false, 0)]
    static void GoHomeScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(Application.dataPath + "/Scene/Home.unity");
        }
    }

    [MenuItem("FastKey/Go LiKui &#4", false, 0)]
    static void GoFightScene()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(Application.dataPath + "/Game/LiKui/Scene/LiKui.unity");
        }
    }

    [MenuItem("FastKey/Open AssetPath &#z", false, 0)]
    static void GoPerAsset()
    {
        EditorUtility.OpenWithDefaultApp(Application.dataPath);
    }

    [MenuItem("FastKey/Open Persistent &#p", false, 0)]
    static void GoPersistent()
    {
        EditorUtility.OpenWithDefaultApp(Application.persistentDataPath);
    }
}
