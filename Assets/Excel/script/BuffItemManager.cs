using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ExcelData
{
    [System.Serializable]
    public class BuffItemManager : ScriptableObject
    {
        public BuffItem[] dataArray;

        protected Dictionary<int, BuffItem> m_Data = new Dictionary<int, BuffItem>();
        public void Init()
        {
            var tt = dataArray;
            Debug.Log("BuffItemManager");
            foreach (BuffItem i in dataArray)
            {
                m_Data[i.TypeID] = i;
            }
        }

        public BuffItem GetBIByID(int id)
        {
            if (m_Data.ContainsKey(id) == false)
            {
                return null;
            }
            return m_Data[id];
        }


    }
}
