using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class GameConfig
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {

        Debug.Log("GameConfig:Init");
        Application.targetFrameRate = 120;
        var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
    }
}
