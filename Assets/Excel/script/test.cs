using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExcelData;

public class test : MonoBehaviour
{
    private void Start()
    {
        BulletItemManager manager = Resources.Load<BulletItemManager>("Conf/BulletItem");

        Debug.Log("manager:" + manager);

        //var manager1 = (BulletItemManager)(GameObject.Instantiate(Resources.Load("BulletItem")));
        //Debug.Log("manager1:" + manager1);

        foreach (BulletItem i in manager.dataArray)
        {
            Debug.Log(i.TypeID + "---" + i.ModePath + "---" + i.Level);
        }


    }
}
