using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ExcelData
{
    [System.Serializable]
    public class ItemManager : ScriptableObject
    {
        public Item[] dataArray;

        protected Dictionary<int, Item> m_Data = new Dictionary<int, Item>();
        public void Init()
        {
            var tt = dataArray;
            Debug.Log("ItemManager");
            foreach (Item i in dataArray)
            {
                m_Data[i.TypeID] = i;
            }
        }

        public Item GetItemByID(int id)
        {
            if(m_Data.ContainsKey(id) == false)
            {
                return null;
            }
            return m_Data[id];
        }


    }
}
