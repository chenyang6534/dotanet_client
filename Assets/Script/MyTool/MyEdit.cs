using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using LitJson;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
public class MyEditor : Editor
{
    
    static string  []SceneName = { "Map/Scene1" };
    static string[] CreateUnits = { "Map/CreateUnits/CreateUnits0","Map/CreateUnits/CreateUnits1", "Map/CreateUnits/CreateUnits2","Map/CreateUnits/CreateUnits3"
            ,"Map/CreateUnits/CreateUnits100","Map/CreateUnits/CreateUnits101","Map/CreateUnits/CreateUnits102"
    ,"Map/CreateUnits/CreateUnits200","Map/CreateUnits/CreateUnits201","Map/CreateUnits/CreateUnits202"
    ,"Map/CreateUnits/CreateUnits300","Map/CreateUnits/CreateUnits301","Map/CreateUnits/CreateUnits302"
    ,"Map/CreateUnits/CreateUnits400","Map/CreateUnits/CreateUnits401","Map/CreateUnits/CreateUnits402"
    ,"Map/CreateUnits/CreateUnits500","Map/CreateUnits/CreateUnits501","Map/CreateUnits/CreateUnits502"
    ,"Map/CreateUnits/CreateUnits600","Map/CreateUnits/CreateUnits601","Map/CreateUnits/CreateUnits602"
    ,"Map/CreateUnits/CreateUnitsGuild1","Map/CreateUnits/CreateUnitsGuild2","Map/CreateUnits/CreateUnitsGuild3"
    ,"Map/CreateUnits/CreateUnitsGuild4","Map/CreateUnits/CreateUnitsGuild5","Map/CreateUnits/CreateUnitsGuild6"
    ,"Map/CreateUnits/CreateUnitsGuild7","Map/CreateUnits/CreateUnitsGuild8","Map/CreateUnits/CreateUnitsGuild9"
    ,"Map/CreateUnits/CreateUnitsActivity1","Map/CreateUnits/CreateUnitsActivity2","Map/CreateUnits/CreateUnitsActivity3"
    ,"Map/CreateUnits/CreateUnitsActivity4","Map/CreateUnits/CreateUnitsActivity5","Map/CreateUnits/CreateUnitsActivity6"
    ,"Map/CreateUnits/CreateUnitsActivity7","Map/CreateUnits/CreateUnitsActivity8","Map/CreateUnits/CreateUnitsActivity9"};
    static string DestPath = "D://sheshe/bin/conf/";

    //将所有游戏场景导出为JSON格式
    [MenuItem("GameObject/ExportMapJSON")]
    static void ExportMapJSON()
    {
        //string filepath = Application.dataPath + @"/Output/SceneCollides.sc";
        string filepath = DestPath + "SceneCollides.sc";
        FileInfo t = new FileInfo(filepath);
        if (!File.Exists(filepath))
        {
            File.Delete(filepath);
        }
        StreamWriter sw = t.CreateText();

        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("Scenes");
        writer.WriteArrayStart();

        //int i11 = 0;
        foreach (string S in SceneName)
        {
            //int index = i11++;
            //EditorSceneManager.OpenScene(ScenePath[index]);
            var scene = (GameObject)(GameObject.Instantiate(Resources.Load(S)));
            if(scene != null)
            {
                string name = S;
                writer.WriteObjectStart();
                writer.WritePropertyName("name");
                writer.Write(name);

                var collides = scene.GetComponentsInChildren<BoxCollider>();
                if(collides.Length <= 0)
                {
                    continue;
                }
                writer.WritePropertyName("Collides");
                writer.WriteArrayStart();
                foreach (BoxCollider obj1 in collides)
                {
                    UnityEngine.Debug.Log("y:" + obj1.transform.rotation.y + " name:" + obj1.name);

                    Vector3[] vectors = GetBoxColliderVertexPositions(obj1);

                    if (IsRect(vectors))
                    {
                        writer.WriteObjectStart();
                        writer.WritePropertyName("IsRect");
                        writer.Write(true);

                        Vector3 center = getRectCenter(vectors);

                        writer.WritePropertyName("CenterX");
                        writer.Write(center.x);
                        writer.WritePropertyName("CenterY");
                        writer.Write(center.z);

                        var rectpoint = convert2BigRect(vectors);

                        writer.WritePropertyName("Width");
                        writer.Write((rectpoint[1].x - rectpoint[0].x)/2);
                        writer.WritePropertyName("Height");
                        writer.Write((rectpoint[1].z - rectpoint[2].z) / 2);

                        writer.WriteObjectEnd();
                    }
                    else
                    {
                        writer.WriteObjectStart();
                        writer.WritePropertyName("IsRect");
                        writer.Write(false);

                        Vector3 center = getRectCenter(vectors);

                        writer.WritePropertyName("CenterX");
                        writer.Write(center.x);
                        writer.WritePropertyName("CenterY");
                        writer.Write(center.z);

                        writer.WritePropertyName("Points");
                        writer.WriteArrayStart();
                        //Vector3[] vectors = GetBoxColliderVertexPositions(obj1);
                        for (int i = 0; i < vectors.Length; i++)
                        {
                            writer.WriteObjectStart();
                            writer.WritePropertyName("X");
                            writer.Write(vectors[i].x- center.x);
                            writer.WritePropertyName("Y");
                            writer.Write(vectors[i].z- center.z);
                            writer.WriteObjectEnd();
                        }
                        writer.WriteArrayEnd();

                        writer.WriteObjectEnd();
                    }

                            
                    
                }
                writer.WriteArrayEnd();

                writer.WriteObjectEnd();
                DestroyImmediate(scene);
            }
        }
        writer.WriteArrayEnd();
        writer.WriteObjectEnd();

        sw.WriteLine(sb.ToString());
        sw.Close();
        sw.Dispose();
        AssetDatabase.Refresh();

        UnityEngine.Debug.Log("export succ");
    }
    //将所有游戏创建单位导出为JSON格式
    [MenuItem("GameObject/ExportCreateUnitsJSON")]
    static void ExportCreateUnitsJSON()
    {
        //Debug.Log("111111111");
        //var scene222 = (GameObject)(GameObject.Instantiate(Resources.Load("D://unity3d/dotaNet/Assets/Output/CreateUnits602")));
        //if (scene222 != null)
        //{
        //    Debug.Log("222222222");
        //}
        //Debug.Log("333333333");


        //string filepath = Application.dataPath + @"/Output/SceneCollides.sc";
        string filepath = DestPath + "CreateUnits.sc";
        FileInfo t = new FileInfo(filepath);
        if (!File.Exists(filepath))
        {
            File.Delete(filepath);
        }
        StreamWriter sw = t.CreateText();

        StringBuilder sb = new StringBuilder();
        JsonWriter writer = new JsonWriter(sb);
        writer.WriteObjectStart();
        writer.WritePropertyName("CreateUnits");
        writer.WriteArrayStart();

        //int i11 = 0;
        foreach (string S in CreateUnits)
        {
            //int index = i11++;
            //EditorSceneManager.OpenScene(ScenePath[index]);
            var scene = (GameObject)(GameObject.Instantiate(Resources.Load(S)));
            if (scene != null)
            {
                string name = S;
                writer.WriteObjectStart();
                writer.WritePropertyName("name");
                writer.Write(name);

                var scenetag = scene.GetComponentsInChildren<SceneTag>();
                if (scenetag != null)
                {
                    writer.WritePropertyName("Units");
                    writer.WriteArrayStart();
                    foreach (SceneTag obj1 in scenetag)
                    {
                        //this.transform.localEulerAngles.x
                        UnityEngine.Debug.Log("y:" + obj1.transform.localEulerAngles.y + " name:" + obj1.name);
                        int typeid = obj1.EnemyID;
                        var pos = obj1.transform.position;
                        float recreatetime = obj1.ReCreateTime;
                        writer.WriteObjectStart();
                        writer.WritePropertyName("TypeID");
                        writer.Write(typeid);
                        writer.WritePropertyName("X");
                        writer.Write(pos.x);
                        writer.WritePropertyName("Y");
                        writer.Write(pos.z);
                        writer.WritePropertyName("Z");
                        writer.Write(pos.y);
                        writer.WritePropertyName("ReCreateTime");
                        writer.Write(recreatetime);
                        writer.WritePropertyName("Rotation");
                        writer.Write(obj1.transform.localEulerAngles.y);
                        

                        writer.WriteObjectEnd();

                    }
                    writer.WriteArrayEnd();
                }
                var doorway = scene.GetComponentsInChildren<DoorwayTag>();
                if (doorway != null)
                {
                    writer.WritePropertyName("Doorways");
                    writer.WriteArrayStart();
                    foreach (DoorwayTag obj1 in doorway)
                    {
                        //this.transform.localEulerAngles.x
                        
                        var pos = obj1.transform.position;
                        writer.WriteObjectStart();
                        writer.WritePropertyName("NextSceneID");
                        writer.Write(obj1.NextSceneID);
                        writer.WritePropertyName("X");
                        writer.Write(pos.x);
                        writer.WritePropertyName("Y");
                        writer.Write(pos.z);
                        writer.WritePropertyName("Z");
                        writer.Write(pos.y);
                        writer.WritePropertyName("R");
                        writer.Write(obj1.R);
                        writer.WritePropertyName("NeedLevel");
                        writer.Write(obj1.NeedLevel);
                        writer.WritePropertyName("NeedPlayer");
                        writer.Write(obj1.NeedPlayer);
                        writer.WritePropertyName("NextX");
                        writer.Write(obj1.NextScenePosition.x);
                        writer.WritePropertyName("NextY");
                        writer.Write(obj1.NextScenePosition.y);
                        writer.WritePropertyName("HaloTypeID");
                        writer.Write(obj1.HaloTypeID);


                        writer.WriteObjectEnd();

                    }
                    writer.WriteArrayEnd();
                }


                writer.WriteObjectEnd();
                DestroyImmediate(scene);
            }
        }
        writer.WriteArrayEnd();
        writer.WriteObjectEnd();

        sw.WriteLine(sb.ToString());
        sw.Close();
        sw.Dispose();
        AssetDatabase.Refresh();

        UnityEngine.Debug.Log("export createunits succ");
    }
    //是否为矩形
    static bool IsRect(Vector3[] points)
    {
        float mindis = 0.3f; //任意两点之间的 横纵 最小距离小于1就为矩形
        for(var i = 0; i < points.Length; i++)
        {
            var next = i + 1;
            if(next >= points.Length)
            {
                next = 0;
            }
            //System.Math.Abs
            var offx = points[i].x - points[next].x;
            var offz = points[i].z - points[next].z;
            if(System.Math.Abs(offx) > mindis && System.Math.Abs(offz) > mindis)
            {
                return false;
            }
        }

        return true;
    }
    static Vector3 getRectCenter(Vector3[] points)
    {
        var rectpoint = convert2BigRect(points);
        return new Vector3(rectpoint[0].x+(rectpoint[1].x-rectpoint[0].x)/2, 0, rectpoint[2].z + (rectpoint[1].z - rectpoint[2].z) / 2);
    }
    //转换为大矩形
    static Vector3[] convert2BigRect(Vector3[] points)
    {
        //左上 右上 右下 左下
        float minx = points[0].x;
        float minz = points[0].z;
        float maxx = points[0].x;
        float maxz = points[0].z;
        for (var i = 0; i < points.Length; i++)
        {
            if (points[i].x < minx)
            {
                minx = points[i].x;
            }
            if (points[i].x > maxx)
            {
                maxx = points[i].x;
            }
            if (points[i].z < minz)
            {
                minz = points[i].z;
            }
            if (points[i].z > maxz)
            {
                maxz = points[i].z;
            }
        }
        var vertices = new Vector3[4];
        //左上 右上 右下 左下
        vertices[0] = new Vector3(minx, 0, maxz);
        vertices[1] = new Vector3(maxx, 0, maxz);
        vertices[2] = new Vector3(maxx, 0, minz);
        vertices[3] = new Vector3(minx, 0, minz);
        return vertices;
    }

    static Vector3[] GetBoxColliderVertexPositions(BoxCollider boxcollider)
    {

        var vertices = new Vector3[4];
        //下面4个点
        //vertices[0] =  boxcollider.transform.TransformPoint(boxcollider.center+ new Vector3(-boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f) - boxcollider.transform.position;
        //vertices[1] = boxcollider.transform.TransformPoint(boxcollider.center  + new Vector3(boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f) - boxcollider.transform.position;
        //vertices[2] = boxcollider.transform.TransformPoint(boxcollider.center  + new Vector3(boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f) - boxcollider.transform.position;
        //vertices[3] = boxcollider.transform.TransformPoint(boxcollider.center  + new Vector3(-boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f) - boxcollider.transform.position;

        vertices[0] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(-boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f);
        vertices[1] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f);
        vertices[2] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f);
        vertices[3] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(-boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f);

        for (var i = 0; i < vertices.Length; i++)
        {
            Debug.Log("point:" + i + "  " + vertices[i]);
        }

        return vertices;
    }


    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string dir = "AssetBundles";
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        //BuildTarget 选择build出来的AB包要使用的平台
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);

        
    }
    
}
#endif