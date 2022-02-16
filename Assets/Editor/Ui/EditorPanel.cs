using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class EditorPanel
{
    [MenuItem("Tools/重新开始")]
    private static void rePlay()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.ExitPlaymode();
        }
        EditorApplication.update += rePlayCheck;
    }

    #region rePlayCheck
    private static void rePlayCheck()
    {
        if (
            EditorApplication.isPlaying || EditorApplication.isPaused ||
            EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }
        EditorApplication.update -= rePlayCheck;

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene("Assets/start.unity");
            EditorApplication.EnterPlaymode();
        }

        

    }
    #endregion
}
