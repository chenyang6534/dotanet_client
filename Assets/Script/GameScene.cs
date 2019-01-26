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
    protected double m_GameServerStartTime;//游戏场景服务器第0帧的时候的本地时间
    protected float m_LogicDelayTime;//逻辑延时
    protected string m_ServerName;

    protected Dictionary<int,Protomsg.SC_Update> m_LogicFrameData;//逻辑帧数据


    protected List<int> m_MyControlUnit;

    void Start () {
        
        Init();

    }

    void Init()
    {
        m_LogicDelayTime = 0.02f;//延时0.02s
        MsgManager.Instance.AddListener("SC_Update", new HandleMsg(this.SC_Update));
        MsgManager.Instance.AddListener("SC_NewScene", new HandleMsg(this.SC_NewScene));
        m_LogicFrameData = new Dictionary<int, Protomsg.SC_Update>();

    }
    void OnDestroy()
    {
        MsgManager.Instance.RemoveListener("SC_Update");
        MsgManager.Instance.RemoveListener("SC_NewScene");
        MyKcp.Instance.Stop();
        Debug.Log("OnDestroy");
    }
    public int MaxFrame = 0;
    public bool SC_Update(Protomsg.MsgBase d1)
    {
        //Debug.Log("SC_Update:");
        IMessage IMperson = new Protomsg.SC_Update();
        Protomsg.SC_Update p1 = (Protomsg.SC_Update)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        m_LogicFrameData[p1.CurFrame] = p1;
        MaxFrame = p1.CurFrame;

        //Debug.Log("SC_Update:"+p1.CurFrame);

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
        m_GameServerStartTime = Tool.GetTime() - 1.0f/p1.LogicFps* p1.CurFrame;
        Debug.Log("starttime:"+ m_GameServerStartTime+ " LogicFps: "+ p1.LogicFps+"  curframe:"+m_CurFrame+"  time:"+ Time.realtimeSinceStartup);
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

        var pos = new Vector3(m_GameScene.transform.position.x, 0, m_GameScene.transform.position.z);
        m_PlaneScene.SetNormalAndPosition(m_GameScene.transform.up, pos);

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
                if (true)
                {
                    DHCameraManager.SetFollowingTarget(item.Value);
                }
            }
            
        }

            
    }



    void LogicUpdate()
    {
        //m_GameServerStartTime = Tool.GetTime() - 1.0f / p1.LogicFps * p1.CurFrame;
        var frame = (Tool.GetTime() - m_GameServerStartTime-m_LogicDelayTime) * m_LogicFps;
        //Debug.Log("frame:" + frame+"     curframe:"+ m_CurFrame+ " MaxFrame: " + MaxFrame+" time:"+ Tool.GetTime());
        for (var i = m_CurFrame; i <= frame; i++)
        {
            //帧数据存在
            if (m_LogicFrameData.ContainsKey(i))
            {
                var p1 = m_LogicFrameData[i];
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
                m_CurFrame = i+1;//下一帧序号
                FreshControl();

                //删除上上帧数据
                m_LogicFrameData.Remove(i - 1);
            }
            else
            {
                Debug.Log("no data-------------------------------");
            }
        }

        //位置进行线性插值计算
        if (frame - (m_CurFrame - 1) > 0.001)
        {
            float scale = (float)(frame - (m_CurFrame - 1));
            if (m_LogicFrameData.ContainsKey(m_CurFrame))
            {
                var p1 = m_LogicFrameData[m_CurFrame];
                foreach (var item in p1.OldUnits)
                {
                    UnityEntityManager.Instance.ChangeShowPos(item, scale);
                }
            }
        }

    }



    // Update is called once per frame
    void Update () {
        MsgManager.Instance.UpdateMessage();

        LogicUpdate();

        //Debug.Log("frame:"+Time.deltaTime);

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
