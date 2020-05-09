using UnityEngine;

namespace ExcelData
{
    //这里根据自己的表结构来



    [System.Serializable]
    public class NoticeWords
    {
        public int TypeID;
        public string Words;
        public string Sound;
        public int Type;//1 弹出文字 2 跑马灯
        public string AnimName;
        public int Pos;//位置 1上 2中 3下
    }


}
