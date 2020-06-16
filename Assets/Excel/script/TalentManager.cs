using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ExcelData
{
   

    [System.Serializable]
    public class TalentManager : ScriptableObject
    {
        public Talent[] dataArray;

        protected Dictionary<int, Talent> m_Data = new Dictionary<int, Talent>();
        public void Init()
        {
            var tt = dataArray;
            Debug.Log("TalentManager");
            foreach (Talent i in dataArray)
            {
                m_Data[i.TypeID] = i;
            }
        }

        public Talent GetTalentByID(int id)
        {
            if (m_Data.ContainsKey(id) == false)
            {
                return null;
            }
            return m_Data[id];
        }


    }

    
}
