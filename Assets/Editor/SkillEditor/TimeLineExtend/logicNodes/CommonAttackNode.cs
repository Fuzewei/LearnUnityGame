using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class CommonAttackNode : SkillNodeAsset
{

    CommonAttackNode()
    {
        nodeName = "CommonAttackNode";
        setParam("hitFlyDurationTime", "1.0"); //击飞时间
    }
}
