using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityEntitySpecial : MonoBehaviour {

	// Use this for initialization
    protected Dictionary<SkinnedMeshRenderer, Material> m_AllMaterial = new Dictionary<SkinnedMeshRenderer, Material>();
    
    protected int m_GreenCount;
    protected int m_WhiteCount;
    protected int m_AngerRed;//愤怒红

    public Material AngerRedMat;//愤怒红材质

    void Awake () {
        //Debug.Log("aaaaaaaaaaaaaa");
        m_GreenCount = 0;
        m_WhiteCount = 0;
        m_AngerRed = 0;
        var allChild = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChild)
        {
            //Debug.Log("name:"+child.name);
            if (child.GetComponents<SkinnedMeshRenderer>() != null)
            {
                //Debug.Log("len:" + child.GetComponents<SkinnedMeshRenderer>().Length);
                for (var i = 0; i < child.GetComponents<SkinnedMeshRenderer>().Length; i++)
                {
                    m_AllMaterial[child.GetComponents<SkinnedMeshRenderer>()[0]] = child.GetComponents<SkinnedMeshRenderer>()[0].material;
                }
                
            }
        }
	}
    public void Reset()
    {

        foreach ( var item in m_AllMaterial)
        {
            item.Key.material = item.Value;
        }
    }

    public void CheckNextShow()
    {
        if(m_GreenCount <= 0 && m_WhiteCount <= 0 && m_AngerRed <= 0)
        {
            Reset();
            return;
        }
        if(m_GreenCount > 0)
        {
            SetGreen();
        }else if(m_WhiteCount > 0)
        {
            SetWhite();
        }else if( m_AngerRed > 0)
        {

        }

    }

    public void RemoveGreen()
    {
        m_GreenCount--;
        CheckNextShow();
    }
    protected void SetGreen()
    {
        Material mat = Resources.Load<Material>("Specialeffects/unitygreen");// new Material(Shader.Find("UnityEntity/Specail1"));

        foreach (var item in m_AllMaterial)
        {
            item.Key.material = mat;
        }
    }
    public void AddGreen()
    {
        SetGreen();
        m_GreenCount++;
    }
    public void RemoveWhite()
    {
        m_WhiteCount--;
        CheckNextShow();
    }
    protected void SetWhite()
    {
        Material mat = Resources.Load<Material>("Specialeffects/unitywhite");// new Material(Shader.Find("UnityEntity/Specail1"));

        foreach (var item in m_AllMaterial)
        {
            item.Key.material = mat;
        }
    }
    public void AddWhite()
    {
        SetWhite();
        m_WhiteCount++;
    }

    public void RemoveAngerRed()
    {
        m_AngerRed--;
        CheckNextShow();
    }
    protected void SetAngerRed()
    {
        if(AngerRedMat == null)
        {
            return;
        }
        //Material mat = Resources.Load<Material>("Specialeffects/unitywhite");// new Material(Shader.Find("UnityEntity/Specail1"));

        foreach (var item in m_AllMaterial)
        {
            item.Key.material = AngerRedMat;
        }
    }
    public void AddAngerRed()
    {
        SetAngerRed();
        m_AngerRed++;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
