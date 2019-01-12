using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using LitJson;
using UnityEditor.SceneManagement;

public class MyEditor : Editor
{
    static string []ScenePath = { "Assets/ScenesRes/LOM/Scenes/set_5v5.unity" };
    static string  []SceneName = { "Map/set_5v5" };
    //将所有游戏场景导出为JSON格式
    [MenuItem("GameObject/ExportJSON")]
    static void ExportJSON()
    {
        string filepath = Application.dataPath + @"/Output/SceneCollides.sc";
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

        int i11 = 0;
        foreach (string S in ScenePath)
        {
            int index = i11++;
            EditorSceneManager.OpenScene("Assets/ScenesRes/LOM/Scenes/set_5v5.unity");//(GameObject)Instantiate(Resources.Load(S));
            //SObj = Object.fin
            //if (SObj != null)
            {
                string name = SceneName[index];
                //EditorApplication.OpenScene(name);
                //writer.WriteObjectStart();
                //writer.WritePropertyName("scenes");
                //writer.WriteArrayStart();
                writer.WriteObjectStart();
                writer.WritePropertyName("name");
                writer.Write(name);

                foreach (GameObject obj1 in Object.FindObjectsOfType(typeof(GameObject)))
                {
                    if (obj1.name.CompareTo("collides") != 0)
                    {
                        continue;
                    }
                    writer.WritePropertyName("Collides");
                    writer.WriteArrayStart();
                    foreach (Transform obj in obj1.transform.GetComponentInChildren<Transform>())
                    {
                        {
                            UnityEngine.Debug.Log("y:"+ obj.transform.rotation.y);
                            if (System.Math.Abs(obj.transform.rotation.y)  == 0)
                            {
                                writer.WriteObjectStart();
                                writer.WritePropertyName("IsRect");
                                writer.Write(true);

                                writer.WritePropertyName("CenterX");
                                writer.Write(obj.transform.position.x);
                                writer.WritePropertyName("CenterY");
                                writer.Write(obj.transform.position.z);

                                writer.WritePropertyName("Width");
                                writer.Write((obj.GetComponent<BoxCollider>().size.x* obj.transform.localScale.x/2));
                                writer.WritePropertyName("Height");
                                writer.Write((obj.GetComponent<BoxCollider>().size.z * obj.transform.localScale.z/2));

                                writer.WriteObjectEnd();
                            }
                            else
                            {
                                writer.WriteObjectStart();
                                writer.WritePropertyName("IsRect");
                                writer.Write(false);

                                writer.WritePropertyName("CenterX");
                                writer.Write(obj.transform.position.x);
                                writer.WritePropertyName("CenterY");
                                writer.Write(obj.transform.position.z);

                                writer.WritePropertyName("Points");
                                writer.WriteArrayStart();
                                Vector3[] vectors = GetBoxColliderVertexPositions(obj.GetComponent<BoxCollider>());
                                for (int i = 0; i < vectors.Length; i++)
                                {
                                    writer.WriteObjectStart();
                                    writer.WritePropertyName("X");
                                    writer.Write(vectors[i].x);
                                    writer.WritePropertyName("Y");
                                    writer.Write(vectors[i].z);
                                    writer.WriteObjectEnd();
                                }
                                writer.WriteArrayEnd();

                                writer.WriteObjectEnd();
                            }

                            
                        }
                        
                    }
                    writer.WriteArrayEnd();
                }
                
                writer.WriteObjectEnd();
            }
        }
        writer.WriteArrayEnd();
        writer.WriteObjectEnd();

        sw.WriteLine(sb.ToString());
        sw.Close();
        sw.Dispose();
        AssetDatabase.Refresh();
    }

    static Vector3[] GetBoxColliderVertexPositions(BoxCollider boxcollider)
    {

        var vertices = new Vector3[4];
        //下面4个点
        vertices[0] =  boxcollider.transform.TransformPoint(boxcollider.center+ new Vector3(-boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f) - boxcollider.transform.position;
        vertices[1] = boxcollider.transform.TransformPoint(boxcollider.center  + new Vector3(boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f) - boxcollider.transform.position;
        vertices[2] = boxcollider.transform.TransformPoint(boxcollider.center  + new Vector3(boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f) - boxcollider.transform.position;
        vertices[3] = boxcollider.transform.TransformPoint(boxcollider.center  + new Vector3(-boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f) - boxcollider.transform.position;
        return vertices;
    }
}
