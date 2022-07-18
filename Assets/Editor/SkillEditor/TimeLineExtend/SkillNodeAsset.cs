using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class SkillNodeAsset : PlayableAsset
{
    public string nodeName;
    //public Dictionary<> nodeName;
    public override Playable  CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        SkillNodeBehaviour bh = new SkillNodeBehaviour();
        bh.message = "fzw nb";
        Playable playable = ScriptPlayable<SkillNodeBehaviour>.Create(graph, bh);
        return playable;
    }
}
