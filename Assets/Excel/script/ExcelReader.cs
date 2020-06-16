using System.Collections;
using System.Collections.Generic;
using System.Data;
using Excel;
using System.IO;
using UnityEngine;
using UnityEditor;
using ExcelData;

#if UNITY_EDITOR
public class ExcelReader
{



    public class ExcelConfig
    {
        /// <summary>
        /// 存放excel表文件夹的的路径，本例xecel表放在了"Assets/Excels/"当中
        /// </summary>
        //public static readonly string excelsFolderPath = Application.dataPath + "/Excel/resource/";
        public static readonly string excelsFolderPath = "Excel/";

        /// <summary>
        /// 存放Excel转化CS文件的文件夹路径
        /// </summary>
        public static readonly string assetPath = "Assets/Resources/Conf/";
    }

    public class ExcelTool
    {

        /// <summary>
        /// 读取表数据，生成对应的数组
        /// </summary>
        /// <param name="filePath">excel文件全路径</param>
        /// <returns>Item数组</returns>
        public static BulletItem[] CreateBulletItemArrayWithExcel(string filePath)
        {
            //获得表数据
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);

            //根据excel的定义，第二行开始才是数据
            BulletItem[] array = new BulletItem[rowNum - 1];
            for (int i = 1; i < rowNum; i++)
            {
                BulletItem item = new BulletItem();
                //解析每列的数据
                item.TypeID = int.Parse(collect[i][0].ToString());
                item.ModePath = collect[i][1].ToString();
                item.Level = int.Parse(collect[i][2].ToString());
                array[i - 1] = item;
            }
            return array;
        }

        public static BuffItem[] CreateBuffItemArrayWithExcel(string filePath)
        {
            //获得表数据
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);

            //根据excel的定义，第二行开始才是数据
            BuffItem[] array = new BuffItem[rowNum - 1];
            for (int i = 1; i < rowNum; i++)
            {
                BuffItem item = new BuffItem();
                //解析每列的数据
                item.TypeID = int.Parse(collect[i][0].ToString());
                item.BodyEffect = collect[i][2].ToString();
                item.FootEffect = collect[i][3].ToString();
                item.Enable = int.Parse(collect[i][4].ToString());
                item.HeadEffect = collect[i][5].ToString();
                item.TopBarEffect = collect[i][6].ToString();
                item.RenderColor = int.Parse(collect[i][7].ToString());
                item.OutEffect = collect[i][8].ToString();
                item.NativeTopBarEffect = collect[i][9].ToString();
                item.HandsEffect = collect[i][10].ToString();
                item.FootsEffect = collect[i][11].ToString();
                item.MaterialEffect = collect[i][12].ToString();
                item.ModeEnable = int.Parse(collect[i][13].ToString());
                item.IconPath = collect[i][14].ToString();
                item.IconTimeEnable = int.Parse(collect[i][15].ToString());
                item.IsShowControl = int.Parse(collect[i][16].ToString());

                array[i - 1] = item;
            }
            return array;
        }
        public static Item[] CreateItemArrayWithExcel(string filePath)
        {
            //获得表数据
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);

            //根据excel的定义，第二行开始才是数据
            Item[] array = new Item[rowNum - 1];
            for (int i = 1; i < rowNum; i++)
            {
                Item item = new Item();
                //解析每列的数据
                item.TypeID = int.Parse(collect[i][0].ToString());
                item.IconPath = collect[i][1].ToString();
                item.SceneItem = collect[i][2].ToString();
                item.Name = collect[i][3].ToString();
                item.Des = collect[i][4].ToString();
                item.ShowLevel = int.Parse(collect[i][5].ToString());
                array[i - 1] = item;
            }
            return array;
        }

        public static Skill[] CreateSkillArrayWithExcel(string filePath)
        {
            //获得表数据
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);

            //根据excel的定义，第二行开始才是数据
            Skill[] array = new Skill[rowNum - 1];
            for (int i = 1; i < rowNum; i++)
            {
                Skill item = new Skill();
                //解析每列的数据
                item.TypeID = int.Parse(collect[i][0].ToString());
                item.Name = collect[i][1].ToString();
                item.IconPath = collect[i][2].ToString();
                item.Des = collect[i][3].ToString();
                item.AutoAimType = int.Parse(collect[i][4].ToString());
                array[i - 1] = item;
            }
            return array;
        }
        public static Talent[] CreateTalentArrayWithExcel(string filePath)
        {
            //获得表数据
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);

            //根据excel的定义，第二行开始才是数据
            Talent[] array = new Talent[rowNum - 1];
            for (int i = 1; i < rowNum; i++)
            {
                Talent item = new Talent();
                //解析每列的数据
                item.TypeID = int.Parse(collect[i][0].ToString());
                item.Name = collect[i][1].ToString();
                item.IconPath = collect[i][2].ToString();
                item.Des = collect[i][3].ToString();
                array[i - 1] = item;
            }
            return array;
        }

        public static Scene[] CreateSceneArrayWithExcel(string filePath)
        {
            //获得表数据
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);

            //根据excel的定义，第二行开始才是数据
            Scene[] array = new Scene[rowNum - 1];
            for (int i = 1; i < rowNum; i++)
            {
                Scene item = new Scene();
                //解析每列的数据
                item.TypeID = int.Parse(collect[i][0].ToString());
                item.ScenePath = collect[i][1].ToString();
                item.Little_BG = collect[i][2].ToString();
                item.StartX = int.Parse(collect[i][3].ToString());
                item.StartY = int.Parse(collect[i][4].ToString());
                item.EndX = int.Parse(collect[i][5].ToString());
                item.EndY = int.Parse(collect[i][6].ToString());
                item.Name = collect[i][7].ToString();
                array[i - 1] = item;
                
            }
            return array;
        }

        public static UnitInfo[] CreateUnitInfoArrayWithExcel(string filePath)
        {
            //获得表数据
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);

            //根据excel的定义，第二行开始才是数据
            UnitInfo[] array = new UnitInfo[rowNum - 1];
            for (int i = 1; i < rowNum; i++)
            {
                UnitInfo item = new UnitInfo();
                //public int TypeID;
                //public string HeroName;
                //public string IconPath;
                //public string Des;
                //解析每列的数据
                item.TypeID = int.Parse(collect[i][0].ToString());
                item.HeroName = collect[i][1].ToString();
                item.IconPath = collect[i][2].ToString();
                item.Des = collect[i][3].ToString();
                item.Attack_Range = collect[i][4].ToString();
                item.AttributePrimary = int.Parse(collect[i][5].ToString());
                item.Skills_ID = collect[i][6].ToString();
                array[i - 1] = item;

            }
            return array;
        }

        public static NoticeWords[] CreateNoticeWordsArrayWithExcel(string filePath)
        {
            //获得表数据
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);

            //根据excel的定义，第二行开始才是数据
            NoticeWords[] array = new NoticeWords[rowNum - 1];
            for (int i = 1; i < rowNum; i++)
            {
                NoticeWords item = new NoticeWords();
                //public int TypeID;
                //public string HeroName;
                //public string IconPath;
                //public string Des;
                //解析每列的数据
                item.TypeID = int.Parse(collect[i][0].ToString());
                item.Words = collect[i][1].ToString();
                item.Sound = collect[i][2].ToString();
                item.Type = int.Parse(collect[i][3].ToString());
                item.AnimName = collect[i][4].ToString();
                item.Pos = int.Parse(collect[i][5].ToString());
                array[i - 1] = item;

            }
            return array;
        }

        /// <summary>
        /// 读取excel文件内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="columnNum">行数</param>
        /// <param name="rowNum">列数</param>
        /// <returns></returns>
        static DataRowCollection ReadExcel(string filePath, ref int columnNum, ref int rowNum)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelReader.AsDataSet();
            //Tables[0] 下标0表示excel文件中第一张表的数据
            columnNum = result.Tables[0].Columns.Count;
            rowNum = result.Tables[0].Rows.Count;
            return result.Tables[0].Rows;
        }
    }

    public class ExcelBuild : Editor
    {

        [MenuItem("CustomEditor/CreateItemAsset")]
        public static void CreateFileAsset()
        {
            CreateBulletItemAsset();
            CreateBuffItemAsset();
            CreateItemAsset();
            CreateSkillAsset();
            CreateTalentAsset();
            
            CreateSceneAsset();
            CreateUnitInfoAsset();
            CreateNoticeWordsAsset();
        }
        public static void CreateBulletItemAsset()
        {
            BulletItemManager manager = ScriptableObject.CreateInstance<BulletItemManager>();
            //赋值
            manager.dataArray = ExcelTool.CreateBulletItemArrayWithExcel(ExcelConfig.excelsFolderPath + "bullet.xlsx");

            //确保文件夹存在
            if (!Directory.Exists(ExcelConfig.assetPath))
            {
                Directory.CreateDirectory(ExcelConfig.assetPath);
            }

            //asset文件的路径 要以"Assets/..."开始，否则CreateAsset会报错
            string assetPath = string.Format("{0}{1}.asset", ExcelConfig.assetPath, "BulletItem");
            //生成一个Asset文件
            AssetDatabase.CreateAsset(manager, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public static void CreateBuffItemAsset()
        {
            BuffItemManager manager = ScriptableObject.CreateInstance<BuffItemManager>();
            //赋值
            manager.dataArray = ExcelTool.CreateBuffItemArrayWithExcel(ExcelConfig.excelsFolderPath + "buff_effect.xlsx");

            //确保文件夹存在
            if (!Directory.Exists(ExcelConfig.assetPath))
            {
                Directory.CreateDirectory(ExcelConfig.assetPath);
            }

            //asset文件的路径 要以"Assets/..."开始，否则CreateAsset会报错
            string assetPath = string.Format("{0}{1}.asset", ExcelConfig.assetPath, "BuffItem");
            //生成一个Asset文件
            AssetDatabase.CreateAsset(manager, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public static void CreateItemAsset()
        {
            ItemManager manager = ScriptableObject.CreateInstance<ItemManager>();
            //赋值
            manager.dataArray = ExcelTool.CreateItemArrayWithExcel(ExcelConfig.excelsFolderPath + "client_item.xlsx");

            //确保文件夹存在
            if (!Directory.Exists(ExcelConfig.assetPath))
            {
                Directory.CreateDirectory(ExcelConfig.assetPath);
            }

            //asset文件的路径 要以"Assets/..."开始，否则CreateAsset会报错
            string assetPath = string.Format("{0}{1}.asset", ExcelConfig.assetPath, "Item");
            //生成一个Asset文件
            AssetDatabase.CreateAsset(manager, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateSkillAsset()
        {
            SkillManager manager = ScriptableObject.CreateInstance<SkillManager>();
            //赋值
            manager.dataArray = ExcelTool.CreateSkillArrayWithExcel(ExcelConfig.excelsFolderPath + "client_skill.xlsx");

            //确保文件夹存在
            if (!Directory.Exists(ExcelConfig.assetPath))
            {
                Directory.CreateDirectory(ExcelConfig.assetPath);
            }

            //asset文件的路径 要以"Assets/..."开始，否则CreateAsset会报错
            string assetPath = string.Format("{0}{1}.asset", ExcelConfig.assetPath, "Skill");
            //生成一个Asset文件
            AssetDatabase.CreateAsset(manager, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public static void CreateTalentAsset()
        {
            TalentManager manager = ScriptableObject.CreateInstance<TalentManager>();
            //赋值
            manager.dataArray = ExcelTool.CreateTalentArrayWithExcel(ExcelConfig.excelsFolderPath + "client_talent.xlsx");

            //确保文件夹存在
            if (!Directory.Exists(ExcelConfig.assetPath))
            {
                Directory.CreateDirectory(ExcelConfig.assetPath);
            }

            //asset文件的路径 要以"Assets/..."开始，否则CreateAsset会报错
            string assetPath = string.Format("{0}{1}.asset", ExcelConfig.assetPath, "Talent");
            //生成一个Asset文件
            AssetDatabase.CreateAsset(manager, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateSceneAsset()
        {
            SceneManager manager = ScriptableObject.CreateInstance<SceneManager>();
            //赋值
            manager.dataArray = ExcelTool.CreateSceneArrayWithExcel(ExcelConfig.excelsFolderPath + "client_scene.xlsx");

            //确保文件夹存在
            if (!Directory.Exists(ExcelConfig.assetPath))
            {
                Directory.CreateDirectory(ExcelConfig.assetPath);
            }

            //asset文件的路径 要以"Assets/..."开始，否则CreateAsset会报错
            string assetPath = string.Format("{0}{1}.asset", ExcelConfig.assetPath, "Scene");
            //生成一个Asset文件
            AssetDatabase.CreateAsset(manager, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateUnitInfoAsset()
        {
            UnitInfoManager manager = ScriptableObject.CreateInstance<UnitInfoManager>();
            //赋值
            manager.dataArray = ExcelTool.CreateUnitInfoArrayWithExcel(ExcelConfig.excelsFolderPath + "unit_info.xlsx");

            //确保文件夹存在
            if (!Directory.Exists(ExcelConfig.assetPath))
            {
                Directory.CreateDirectory(ExcelConfig.assetPath);
            }

            //asset文件的路径 要以"Assets/..."开始，否则CreateAsset会报错
            string assetPath = string.Format("{0}{1}.asset", ExcelConfig.assetPath, "UnitInfo");
            //生成一个Asset文件
            AssetDatabase.CreateAsset(manager, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateNoticeWordsAsset()
        {
            NoticeWordsManager manager = ScriptableObject.CreateInstance<NoticeWordsManager>();
            //赋值
            manager.dataArray = ExcelTool.CreateNoticeWordsArrayWithExcel(ExcelConfig.excelsFolderPath + "noticewords.xlsx");

            //确保文件夹存在
            if (!Directory.Exists(ExcelConfig.assetPath))
            {
                Directory.CreateDirectory(ExcelConfig.assetPath);
            }

            //asset文件的路径 要以"Assets/..."开始，否则CreateAsset会报错
            string assetPath = string.Format("{0}{1}.asset", ExcelConfig.assetPath, "NoticeWords");
            //生成一个Asset文件
            AssetDatabase.CreateAsset(manager, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        //CreateNoticeWordsArrayWithExcel
    }


}

#endif