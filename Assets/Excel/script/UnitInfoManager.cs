using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ExcelData
{


    [System.Serializable]
    public class UnitInfoManager : ScriptableObject
    {
        public UnitInfo[] dataArray;

        protected Dictionary<int, UnitInfo> m_Data = new Dictionary<int, UnitInfo>();
        public void Init()
        {
            var tt = dataArray;
            Debug.Log("UnitInfoManager");
            foreach (UnitInfo i in dataArray)
            {
                m_Data[i.TypeID] = i;
            }
        }

        public UnitInfo GetUnitInfoByID(int id)
        {
            if (m_Data.ContainsKey(id) == false)
            {
                return null;
            }
            return m_Data[id];
        }


    }
}
