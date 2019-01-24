using cocosocket4unity;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour {

    // Use this for initialization
    protected GameObject hero;
    protected GameObject m_GameScene;
    protected Plane m_PlaneScene;
    protected int m_CurFrame;
    protected int m_LogicFps;
    protected float m_GameServerStartTime;//游戏场景服务器第0帧的时候的本地时间
    protected float m_LogicDelayTime;//逻辑延时
    protected string m_ServerName;


    protected List<int> m_MyControlUnit;

    void Start () {
        
        Init();

    }

    void Init()
    {
        m_LogicDelayTime = 0.05f;//延时0.05s
        MsgManager.Instance.AddListener("SC_Update", new HandleMsg(this.SC_Update));
        MsgManager.Instance.AddListener("SC_NewScene", new HandleMsg(this.SC_NewScene));

    }

    public bool SC_Update(Protomsg.MsgBase d1)
    {
        //Debug.Log("SC_Update:");
        IMessage IMperson = new Protomsg.SC_Update();
        Protomsg.SC_Update p1 = (Protomsg.SC_Update)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        //new
        foreach (var item in p1.NewUnits)
        {
            UnityEntityManager.Instance.CreateUnityEntity(this, item);
        }
        foreach (var item in p1.OldUnits)
        {
            UnityEntityManager.Instance.ChangeUnityEntity(item);
        }
        foreach (var item in p1.RemoveUnits)
        {
            UnityEntityManager.Instance.DestroyUnityEntity(item);
        }

        FreshControl();

        return true;
    }
    //进入新场景
    public bool SC_NewScene(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_NewScene:");
        IMessage IMperson = new Protomsg.SC_NewScene();
        Protomsg.SC_NewScene p1 = (Protomsg.SC_NewScene)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        //p1.Name
        m_CurFrame = p1.CurFrame;
        m_LogicFps = p1.LogicFps;
        m_ServerName = p1.ServerName;
        m_GameServerStartTime = Time.realtimeSinceStartup-1/p1.LogicFps* p1.CurFrame;

        CleanScene();

        LoadScene(p1.Name);

        return true;
    }

    void LoadScene(string name)
    {
        if(m_GameScene != null)
        {
            CleanScene();
        }

        m_GameScene = (GameObject)Instantiate(Resources.Load(name));
        m_PlaneScene.SetNormalAndPosition(m_GameScene.transform.up, m_GameScene.transform.position);

        Debug.Log("SC_NewScene:"+name);
    }

    //清除场景资源 包括场景 单位 特效 
    void CleanScene()
    {
        Destroy(m_GameScene);
        m_GameScene = null;
        UnityEntityManager.Instance.Clear();


    }


    public void FreshControl()
    {
        //System.Array.Clear(m_MyControlUnit,0, m_MyControlUnit.Length);
        m_MyControlUnit = new List<int>();
        Dictionary<int, UnityEntity> all = UnityEntityManager.Instance.GetAllUnity();
        foreach (var item in all)
        {
            if (item.Value.ControlID == LoginUI.UID)
            {
                m_MyControlUnit.Add(item.Key);
            }
        }
            
    }







    // Update is called once per frame
    void Update () {
        MsgManager.Instance.UpdateMessage();

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("GetMouseButtonDown:"+ Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //摄像机需要设置MainCamera的Tag这里才能找到
            //RaycastHit hitInfo;
            float dis;

            m_PlaneScene.Raycast(ray, out dis);
            ////根据距离获得鼠标点击的目标点三维坐标
            Vector3 _vec3Target = ray.GetPoint(dis);
            Debug.Log(" hit point " + _vec3Target);

            Debug.Log(" idaa:" + m_MyControlUnit[0]);

            Protomsg.CS_PlayerMove msg1 = new Protomsg.CS_PlayerMove();
            msg1.IDs.AddRange(m_MyControlUnit);
            msg1.IsStart = true;
            msg1.X = _vec3Target.x;
            msg1.Y = _vec3Target.z;

            Debug.Log(" id:"+ msg1.IDs[0]);

            MyKcp.Instance.SendMsg(m_ServerName, "CS_PlayerMove", msg1);

        }
    }
    
}
