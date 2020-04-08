using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExcelData;

public class SceneItemManager
{
    private static readonly SceneItemManager _instance = new SceneItemManager();
    public static SceneItemManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private SceneItemManager()
    {
        //m_BIM.Init();
    }



    protected Dictionary<int, GameObject> m_SceneItems = new Dictionary<int, GameObject>();

    

    //创建新单位CreateHaloEntity
    public void CreateSceneItem(GameScene gs, Protomsg.SceneItemDatas data)
    {

        //

        var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(data.TypeID);
        if(clientitem == null)
        {
            return;
        }

        GameObject hero = (GameObject)(GameObject.Instantiate(Resources.Load("Item/prefabs/itemprefabs")));
        hero.transform.parent = gs.transform;
        hero.transform.localPosition = new Vector3(data.X, 0.1f, data.Y);
        //Material mat = Resources.Load<Material>("Item/itemMa");
        //mat.mainTexture = Resources.Load<Texture>(clientitem.SceneItem);
        //hero.GetComponent<MeshRenderer>().materials[0] = mat;

        hero.transform.Find("words").GetComponent<ItemName>().LoadName(clientitem.Name);

        //hero.GetComponent<>
        m_SceneItems[data.ID] = hero;
    }
    //删除单位
    public void DestroySceneItem(int id)
    {
        GameObject unity = m_SceneItems[id];
        if( unity != null)
        {
            GameObject.Destroy(unity);
            m_SceneItems.Remove(id);
        }

    }
    
    


}
