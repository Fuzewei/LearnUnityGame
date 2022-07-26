using System.IO;
using System.Collections.Generic;
using System.Xml;
using KBEngine;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SkillEditor : EditorWindow
{
    Scene? skillEditorScene;
    public GameObject entityPrefab;
    private GameObject _entity;
    PlayableDirector m_playabledirector;

    TimelineAsset m_Timelineasset;


    TimelineEditorWindow timeLineEditor; //时间线编辑器

    string timeLineDir = "";


    List<XmlSkillTimeLine> skillTimeLines;

    [MenuItem("Tools/技能编辑器")]
    private static void Main()
    {
        EditorApplication.ExecuteMenuItem("Window/Sequencing/Timeline");
        SkillEditor windows = (SkillEditor)EditorWindow.GetWindow(typeof(SkillEditor), false, "skillEditor");
        windows.Show();
    }

    private void Awake()
    {
        if (!skillEditorScene.HasValue)
        {
            skillEditorScene = EditorSceneManager.OpenScene("Assets/GreatSword_Animset/Scene/Preview.unity");
        }
        timeLineEditor = (TimelineEditorWindow)EditorWindow.GetWindow(typeof(TimelineEditorWindow));
        timeLineEditor.ShowPopup();
        this.ShowTab();
    }

    public GameObject EditorEntity
    {
        get
        {
            return _entity;
        }
        set
        {
            _entity = value;
            m_playabledirector = _entity.GetComponent<PlayableDirector>();
        }
    }


    private void Update()
    {
        //Selection.activeObject
    }

    void OnGUI()
    {
        var tPrefab = (GameObject)EditorGUILayout.ObjectField("选择编辑对象", entityPrefab, typeof(GameObject));
        if (tPrefab != entityPrefab)
        {
            if (EditorEntity)
            {
                Destroy(EditorEntity);
            }
            entityPrefab = tPrefab;
            EditorEntity = Instantiate(entityPrefab);
            EditorEntity.transform.position = new Vector3(0, 0, 0);
        }

        if (GUILayout.Button("新建技能timeline"))
        {
            m_Timelineasset = Generate("test");
        }

        if (GUILayout.Button("导出数据"))
        {
            exportAllSkillTimeLines("Assets/Resources/TimeLine/Skill/");
        }


        if (GUILayout.Button("测试接口"))
        {
            exportAllSkillTimeLines("Assets/Resources/TimeLine/Skill/");
        }


        //show info
        if (m_playabledirector)
        {
            EditorGUILayout.TextField("当前选中timeline名字：" + m_playabledirector.playableAsset.name);
            if (GUILayout.Button("导出技能timeline"))
            {
                SkillLogic skillLogic = (SkillLogic)m_Timelineasset.GetRootTrack(0);
                var clips = skillLogic.GetClips();
                foreach (var item in clips)
                {
                    Debug.Log(item.displayName + ":" + item.start + ":" + item.duration);
                }
            }
        }

    }

    private void OnDestroy()
    {
        if (EditorEntity)
        {
            Destroy(EditorEntity);
        }
    }

    TimelineAsset Generate(string timeLineName)
    {
        TimelineAsset asset = CreateInstance<TimelineAsset>();
        AssetDatabase.CreateAsset(asset, "Assets/Resources/TimeLine/Skill/" + timeLineName + ".playable");
        var track = asset.CreateTrack<SkillLogic>(null, "技能逻辑");
        TimelineClip clip = track.CreateClip<SkillNodeAsset>();
        clip.displayName = "QAQ";

        AssetDatabase.SaveAssets();
        return asset;
    }

    void exportAllSkillTimeLines(string fileRootPath)
    {
        List<string> timelines = ExportThisDir(fileRootPath);

        XmlDocument xmlDoc = new XmlDocument();
        XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "");
        xmlDoc.AppendChild(declaration);

        XmlNode root = xmlDoc.CreateElement("Root");
        xmlDoc.AppendChild(root);
        foreach (var item in timelines)
        {
            TimelineAsset asset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(item);
            XmlSkillTimeLine t = AssetToSkillTimeline(asset);
            XmlNode timeLinesNode = xmlDoc.CreateElement(t.name);
            root.AppendChild(timeLinesNode);
            t.setXmlAttr(xmlDoc, timeLinesNode);
        }
        xmlDoc.Save(Application.streamingAssetsPath + "/TimeLine/SkillTimeLine.xml");

    }

    public static List<string> ExportThisDir(string dirPath)
    {
        List<string> ans = new List<string>();
        var files = Directory.GetFiles(dirPath, "*.playable");
        var subDir = Directory.GetDirectories(dirPath);

        foreach (var file in files)
        {
            ans.Add(file);
        }
        foreach (var item in subDir)
        {
            var sub = ExportThisDir(item);
            ans.AddRange(sub);
        }
        return ans;
    }

    XmlSkillTimeLine AssetToSkillTimeline(TimelineAsset asset)
    {
        XmlSkillTimeLine ans = new XmlSkillTimeLine();
        SkillLogic ta = (SkillLogic)asset.GetRootTrack(0);
        var clips = ta.GetClips();
        ans.name = asset.name;
        foreach (var item in clips)
        {
            XmlSkillLogicName node = new XmlSkillLogicName();
            node.name = ((SkillNodeAsset)item.asset).nodeName;
            node.beginTime = (float)item.start;
            node.endTime = (float)item.end;
            node.skillParams = ((SkillNodeAsset)item.asset).skillParams;
            ans.nodes.Add(node);
        }
        return ans;
    }
}
