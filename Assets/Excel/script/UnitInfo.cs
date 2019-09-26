using UnityEngine;

namespace ExcelData
{
    //这里根据自己的表结构来



    [System.Serializable]
    public class UnitInfo
    {
        public int TypeID;
        public string HeroName;
        public string IconPath;
        public string Des;
        public string Attack_Range;//攻击距离类型 近战和远程
        public int AttributePrimary;//主属性(1:力量 2:敏捷 3:智力)
        public string Skills_ID;//技能

    }


}
