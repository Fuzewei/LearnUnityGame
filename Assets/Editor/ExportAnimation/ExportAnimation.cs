/******************************************/
/*                                        */
/*    
/*                                        */
/******************************************/


using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using SwordMaster;
using UnityEngine.SceneManagement;

public class ExportAnimation : EditorWindow
{
    private const string anim_PATH = "Assets/GreatSword_Animset/Animation/Root/";
    private const string anim_motion_PATH = "Assets/GreatSword_Animset/Animation/MotionInfo";
    static Dictionary<string, List<rootMotionInfo>> rootMotion = new Dictionary<string, List<rootMotionInfo>>();

    [MenuItem("ExportScene/导出rootmotion信息")]
    public static void Export()
    {
        DirectoryInfo dirinfo = new DirectoryInfo(anim_PATH);
        if (!dirinfo.Exists)
        {
            Debug.LogError("error");
            return;
        }

        dirinfo = new DirectoryInfo(anim_motion_PATH);
        if (!dirinfo.Exists)
        {
            Directory.CreateDirectory(anim_motion_PATH);
        }
        rootMotion.Clear();

        ExportThisDir(anim_PATH);

        var _t = new Dictionary<string, List<rootMotionInfo>>();


        foreach (var item in rootMotion)
        {
            var key = item.Key;
            var value = item.Value;
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(key);
            _t[fileNameWithoutExtension] = value;
        }


        using (StreamWriter streamWriter = new StreamWriter(anim_motion_PATH + "/data.json", false))
        {
            foreach (var item in _t)
            {
                streamWriter.Write(item.Key);
                streamWriter.WriteLine();
                foreach (var info in item.Value)
                {
                    streamWriter.Write("{0} ", info.x);
                    streamWriter.Write("{0} ", info.y);
                    streamWriter.Write("{0} ", info.z);
                    streamWriter.Write("{0} ", info.timeStamp);
                }
                streamWriter.WriteLine();
            }
        }
       
    }

    public static void ExportThisDir(string dirPath)
    {
        var files = Directory.GetFiles(dirPath, "*.fbx");
        var subDir = Directory.GetDirectories(dirPath);

        foreach (var file in files)
        {
            rootMotion.Add(file, new List<rootMotionInfo>());
            float[] x = null;
            float[] y = null;
            float[] z = null;
            float[] timeStamp = null;


            Debug.Log(file);
            Object[] clipobjs = AssetDatabase.LoadAllAssetsAtPath(file);

            foreach (var clipobj in clipobjs)
            {
                AnimationClip srcclip = clipobj as AnimationClip;
                if (srcclip == null || srcclip.name.StartsWith("__preview"))
                    continue;
                //Debug.Log(srcclip.name);

                foreach (var item in AnimationUtility.GetCurveBindings(srcclip))
                {
                    if (item.path == "root")
                    {
                        AnimationCurve curve = AnimationUtility.GetEditorCurve(srcclip, item);

                        if (item.propertyName == "m_LocalPosition.x")
                        {
                            x = new float[curve.length];
                            timeStamp = new float[curve.length];
                            for (int i = 0; i < curve.length; i++)
                            {
                                x[i] = curve[i].value;
                                timeStamp[i] = curve[i].time;
                            }

                        }
                        if (item.propertyName == "m_LocalPosition.y")
                        {
                            y = new float[curve.length];
                            for (int i = 0; i < curve.length; i++)
                            {
                                y[i] = curve[i].value;
                            }

                        }
                        if (item.propertyName == "m_LocalPosition.z")
                        {
                            z = new float[curve.length];
                            for (int i = 0; i < curve.length; i++)
                            {
                                z[i] = curve[i].value;
                            }

                        }

                    }

                }

            }
            if (x.Length != y.Length || x.Length != z.Length || z.Length != y.Length)
            {
                Debug.LogError("");
            }
            for (int i = 0; i < x.Length; i++)
            {
                rootMotion[file].Add(new rootMotionInfo(x[i], y[i], z[i], timeStamp[i]));
            }
        }
        foreach (var item in subDir)
        {
            ExportThisDir(item);
        }
    }

    
}
