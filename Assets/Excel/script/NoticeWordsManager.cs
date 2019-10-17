using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace ExcelData
{


    [System.Serializable]
    public class NoticeWordsManager : ScriptableObject
    {
        public NoticeWords[] dataArray;

        protected Dictionary<int, NoticeWords> m_Data = new Dictionary<int, NoticeWords>();
        public void Init()
        {
            var tt = dataArray;
            Debug.Log("NoticeWordsManager");
            foreach (NoticeWords i in dataArray)
            {
                m_Data[i.TypeID] = i;
            }
        }

        public NoticeWords GetNoticeWordsByID(int id)
        {
            if (m_Data.ContainsKey(id) == false)
            {
                return null;
            }
            return m_Data[id];
        }


    }
}
