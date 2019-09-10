using UnityEngine;

namespace ExcelData
{
    //这里根据自己的表结构来

    [System.Serializable]
    public class BuffItem
    {
        public int TypeID;
        public string BodyEffect;
        public string FootEffect;
        public int Enable;
        public string HeadEffect;
        public string TopBarEffect;
        public int RenderColor; //0:没 1:白色 2:绿色 3:愤怒红
        public string OutEffect; //模型之外的效果 不以模型为父节点
        public string NativeTopBarEffect;
        public string HandsEffect;
        public string FootsEffect;
        public string MaterialEffect;//材质特效
        public int ModeEnable; //模型的隐藏和显示
    }

}
