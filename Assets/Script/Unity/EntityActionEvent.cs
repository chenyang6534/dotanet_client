using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityActionEvent : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
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

    public void AttackActionEvent1(string _arg1)
    {
        Debug.LogFormat("--- AttackActionEvent1:{0}", _arg1);
        ShowDaoGuang(_arg1);
    }
}
