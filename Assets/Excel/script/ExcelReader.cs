using System.Collections;
using System.Collections.Generic;
using System.Data;
using Excel;
using System.IO;
using UnityEngine;
using UnityEditor;
using ExcelData;


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
        public static void CreateItemAsset()
        {
            CreateBulletItemAsset();
            CreateBuffItemAsset();
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
    }


}