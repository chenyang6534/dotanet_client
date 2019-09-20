using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ExcelData
{
   

    [System.Serializable]
    public class SceneManager : ScriptableObject
    {
        public Scene[] dataArray;

        protected Dictionary<int, Scene> m_Data = new Dictionary<int, Scene>();
        public void Init()
        {
            var tt = dataArray;
            Debug.Log("SceneManager");
            foreach (Scene i in dataArray)
            {
                m_Data[i.TypeID] = i;
            }
        }

        public Scene GetSceneByID(int id)
        {
            if (m_Data.ContainsKey(id) == false)
            {
                return null;
            }
            return m_Data[id];
        }


    }
}
