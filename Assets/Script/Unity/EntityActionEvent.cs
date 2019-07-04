using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityActionEvent : MonoBehaviour {

    public TrailRenderer[] m_AttackFireTrail;
    public int m_AttackAnim;
	// Use this for initialization
	void Start () {
		for( var i = 0; i < m_AttackFireTrail.Length; i++)
        {
            m_AttackFireTrail[i].enabled = false;
        }
            
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //刀光测试
    public void ShowDaoGuang(string name)
    {
        GameObject daoguang = (GameObject)(GameObject.Instantiate(Resources.Load("daoguang/"+name)));
        if(daoguang == null)
        {
            return;
        }
        daoguang.transform.parent = gameObject.transform.parent;
        daoguang.transform.position = gameObject.transform.position;
        daoguang.transform.rotation = gameObject.transform.rotation;
        //Debug.LogFormat("--- pos:{0}", daoguang.transform.position," ", daoguang.transform.rotation);
        Destroy(daoguang, 1);//1秒后删除粒子系统
    }
    public void AttackStart(string arg)
    {
        for (var i = 0; i < m_AttackFireTrail.Length; i++)
        {
            m_AttackFireTrail[i].enabled = true;
        }
        if ( arg.Length > 0 && m_AttackAnim > 0)
        {
            ShowDaoGuang(arg);
        }
    }
    public void AttackEnd(string _arg1)
    {
        for (var i = 0; i < m_AttackFireTrail.Length; i++)
        {
            m_AttackFireTrail[i].enabled = false;
        }
    }


    public void AttackActionEvent1(string _arg1)
    {
        Debug.LogFormat("--- AttackActionEvent1:{0}", _arg1);
        ShowDaoGuang(_arg1);
    }
}
