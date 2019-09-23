using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int SelectHeroTypeID;
}


public static class SaveDataManager
{
    public static string sFilePath = Application.persistentDataPath + "/savedata.sd";
    public static SaveData sData;
    public static void Read()
    {
        //
        
        if (File.Exists(sFilePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(sFilePath, FileMode.Open);
            sData = (SaveData)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            sData = new SaveData();
        }
    }
    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(sFilePath);
        bf.Serialize(file, sData);
        file.Close();
    }
}

