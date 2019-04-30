using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
public class UnityEntityTopBar : MonoBehaviour
{
    protected GComponent mRoot;
    // Use this for initialization

    protected GTextField m_Name;
    protected GTextField m_Level;

    protected GProgressBar m_DebuffPro;
    protected GProgressBar m_HP;
    protected GProgressBar m_MP;
    
    void Awake()
    {
        

        mRoot = GetComponent<UIPanel>().ui;

         

        if (mRoot.GetChild("debuf") != null)
        {
            m_DebuffPro = mRoot.GetChild("debuf").asProgress;
            m_DebuffPro.visible = false;
        }
        if (mRoot.GetChild("hp") != null)
        {
            m_HP = mRoot.GetChild("hp").asProgress;
        }
        if (mRoot.GetChild("mp") != null)
        {
            m_MP = mRoot.GetChild("mp").asProgress;
        }
        if (mRoot.GetChild("name") != null)
        {
            m_Name = mRoot.GetChild("name").asTextField;
        }
        if (mRoot.GetChild("level") != null)
        {
            m_Level = mRoot.GetChild("level").asTextField;
        }
    }
    void Start()
    {
        
        
    }
    //设置为 玩家的敌人
    public void SetIsEnemy(bool enemy)
    {
        if (enemy == true)
        {
            if (m_HP != null)
            {
                m_HP.GetChild("bar").asGraph.color = new Color(192 / 255.0f, 50 / 255.0f, 0 / 255.0f);
            }
        }
        else
        {
            if (m_HP != null)
            {
                //m_HP.GetChild("bar").asGraph.color = new Color(80 / 255.0f, 146 / 255.0f, 57 / 255.0f);
                m_HP.GetChild("bar").asGraph.color = new Color(173 / 255.0f, 245 / 255.0f, 98 / 255.0f);
            }
        }
    }

    public void SetHP(int hp)
    {
        if (m_HP != null)
        {
            m_HP.value = hp;
        }
    }
    public void SetMP(int hp)
    {
        if (m_MP != null)
        {
            m_MP.value = hp;
        }
    }
    public void SetDebuf(int hp)
    {
        if (m_DebuffPro != null)
        {
            m_DebuffPro.value = hp;
        }
    }


    public void SetLevel(int level)
    {
        if (m_Level != null)
        {
            m_Level.text = level+"";
        }
    }

    public void SetName(string name)
    {
        if (m_Name != null)
        {
            m_Name.text = name;
        }
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Camera.main.transform.rotation;
    }

    void LateUpdate()
    {
        //this.transform.rotation = Camera.main.transform.rotation;
    }


}