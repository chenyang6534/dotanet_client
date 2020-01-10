using UnityEngine;

namespace ExcelData
{
    //这里根据自己的表结构来

    [System.Serializable]
    public class Item
    {
        public int TypeID;
        public string IconPath;
        public string SceneItem;
        public string Name;
        public string Des;
        public int ShowLevel;//显示等级 1表示显示 0表示不显示
    }

   


}
