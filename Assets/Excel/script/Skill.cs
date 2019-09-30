using UnityEngine;

namespace ExcelData
{
    //这里根据自己的表结构来

   

    [System.Serializable]
    public class Skill
    {
        public int TypeID;
        public string IconPath;
        public string Name;
        public string Des;
        public int AutoAimType; //技能自动瞄准类型 1:普通 2:位移技能(朝目标方向最大距离) 3:进攻技能(瞄准敌人)
    }

    


}
