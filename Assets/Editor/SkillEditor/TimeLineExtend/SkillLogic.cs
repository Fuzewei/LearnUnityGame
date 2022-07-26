using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackClipType(typeof(SkillNodeAsset))]
[TrackColor(0,1, 0)]
public class SkillLogic : PlayableTrack
{
    // Start is called before the first frame update
    protected override Playable CreatePlayable(PlayableGraph graph, GameObject go , TimelineClip clip)
    {
        Playable res = base.CreatePlayable(graph, go, clip);
        return res;
    }

    protected override void OnCreateClip(TimelineClip clip)
    {
        base.OnCreateClip(clip);
        Debug.Log("OnCreateClip:" + clip.displayName);
        //((SkillNodeAsset)clip.asset).nodeName = clip.displayName;
    }

}
