using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;
public class SkillEditor: EditorWindow
{
    string myString = "haha";
    Scene? skillEditorScene;
    public GameObject entityPrefab;
    public GameObject entity;
    PlayableDirector m_playabledirector;
    TimelineAsset m_Timelineasset;


    TimelineEditorWindow timeLineEditor; //ʱ���߱༭��

    string timeLineDir = "";

    [MenuItem("Tools/���ܱ༭��")]
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
    }
    void Start()
    {

    }

    private void Update()
    {
        if (entity!=null)
        {
            Selection.activeObject = entity;
        }
    }

    void OnGUI()
    {
        var tPrefab = (GameObject)EditorGUILayout.ObjectField("ѡ��༭����", entityPrefab, typeof(GameObject));
        if (tPrefab != entityPrefab)
        {
            if (entity)
            {
                Destroy(entity);
            }
            entityPrefab = tPrefab;
            entity = Instantiate(entityPrefab);
            entity.transform.position = new Vector3(0, 0, 0);
        }
        if (GUILayout.Button("����¼���"))
        {
           
        }

    }

    private void OnDestroy()
    {
        if (entity)
        {
            Destroy(entity);
        }
    }

}
