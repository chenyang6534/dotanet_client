using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ExcelData
{
    [System.Serializable]
    public class BulletItemManager : ScriptableObject
    {
        public BulletItem[] dataArray;

        protected Dictionary<int, BulletItem> m_Data = new Dictionary<int, BulletItem>();
        public void Init()
        {
            var tt = dataArray;
            Debug.Log("BulletItemManager");
            foreach (BulletItem i in dataArray)
            {
                m_Data[i.TypeID] = i;
            }
        }

        public BulletItem GetBIByID(int id)
        {
            return m_Data[id];
        }


    }
}
