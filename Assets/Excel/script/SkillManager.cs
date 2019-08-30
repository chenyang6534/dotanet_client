using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ExcelData
{
   

    [System.Serializable]
    public class SkillManager : ScriptableObject
    {
        public Skill[] dataArray;

        protected Dictionary<int, Skill> m_Data = new Dictionary<int, Skill>();
        public void Init()
        {
            var tt = dataArray;
            Debug.Log("SkillManager");
            foreach (Skill i in dataArray)
            {
                m_Data[i.TypeID] = i;
            }
        }

        public Skill GetSkillByID(int id)
        {
            if (m_Data.ContainsKey(id) == false)
            {
                return null;
            }
            return m_Data[id];
        }


    }
}
