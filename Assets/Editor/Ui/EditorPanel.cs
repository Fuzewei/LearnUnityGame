using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class EditorPanel
{
    [MenuItem("Tools/��Start������ʼPlay")]
    private static void rePlay()
    {
        EditorApplication.ExitPlaymode();
        EditorSceneManager.OpenScene("Assets/start.unity");
        EditorApplication.EnterPlaymode();
    }
}
