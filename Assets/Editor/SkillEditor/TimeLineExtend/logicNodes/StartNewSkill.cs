using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;



[System.Serializable]
public class TimeLineEndNode : SkillNodeAsset
{
    TimeLineEndNode()
    {
        nodeName = "TimeLineEndNode";
    }
    public float hitFlyDurationTime;
}

[System.Serializable]
public class StartNewSkill : SkillNodeAsset
{

    StartNewSkill()
    {
        nodeName = "StartNewSkill";
        setParam("newSkillId", ""); //新技能id
    }
    public float hitFlyDurationTime;
}
