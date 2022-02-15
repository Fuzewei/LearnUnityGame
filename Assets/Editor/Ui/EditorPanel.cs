using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class EditorPanel
{
    [MenuItem("Tools/从Start场景开始Play")]
    private static void rePlay()
    {
        EditorApplication.ExitPlaymode();
        EditorSceneManager.OpenScene("Assets/start.unity");
        EditorApplication.EnterPlaymode();
    }
}
