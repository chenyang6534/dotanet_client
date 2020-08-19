using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ExcelData
{
   

    [System.Serializable]
    public class AoShuManager : ScriptableObject
    {
        public AoShu[] dataArray;

        protected Dictionary<int, AoShu> m_Data = new Dictionary<int, AoShu>();
        public void Init()
        {
            var tt = dataArray;
            Debug.Log("AoShuManager");
            foreach (AoShu i in dataArray)
            {
                m_Data[i.TypeID] = i;
            }
        }

        public AoShu GetAoShuByID(int id)
        {
            if (m_Data.ContainsKey(id) == false)
            {
                return null;
            }
            return m_Data[id];
        }


    }

    
}
