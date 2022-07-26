using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class SkillNodeAsset : PlayableAsset
{
    
    public string nodeName = "SkillNodeAsset";
    public Dictionary<string, string> skillParams = new Dictionary<string, string>();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        SkillNodeBehaviour bh = new SkillNodeBehaviour();
        bh.message = "fzw nb";
        Playable playable = ScriptPlayable<SkillNodeBehaviour>.Create(graph, bh);
        return playable;
    }

    protected void setParam(string key, string value)
    {
        skillParams[key] = value;
    }
}


[CustomEditor(typeof(SkillNodeAsset), true)]
public class SkillNodeEditor : Editor
{
    bool showParam = true;
    public Dictionary<string, string> paramChange = new Dictionary<string, string>();

    public override void OnInspectorGUI()
    {
        SkillNodeAsset t = (SkillNodeAsset)target;
        GUILayout.BeginHorizontal();
        GUILayout.Label("节点名：");
        GUILayout.Label(t.nodeName);
        GUILayout.EndHorizontal();

        showParam = EditorGUILayout.Foldout(showParam, "节点参数");
        if (showParam)
        {
            GUILayout.BeginVertical();
            foreach (var item in t.skillParams)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(item.Key);
                string set = GUILayout.TextField(item.Value);
                if (set != item.Value)
                {
                    paramChange[item.Key] = set;
                }
                GUILayout.EndHorizontal();

            }
            foreach (var item in paramChange)
            {
                t.skillParams[item.Key] = item.Value;
            }
            paramChange.Clear();
            GUILayout.EndVertical();
        }
       
    }
}
