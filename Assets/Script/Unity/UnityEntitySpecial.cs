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

    //更改第一个材质
    public void ChangeFirstMaterial(SkinnedMeshRenderer skmr,Material mat)
    {
        //foreach (var item in m_AllMaterial)
        //{
            Material []materials = skmr.materials;
            materials[0] = mat;

            skmr.materials = materials;
        //}
    }
    //增加材质
    public void AddMat(string mat_string)
    {
        Material mat = Resources.Load<Material>(mat_string);
        mat.name = mat_string;
        Debug.Log("mat name:" + mat.name);
        
        if(mat == null)
        {
            return;
        }
        foreach (var item in m_AllMaterial)
        {
            int old_len = item.Key.materials.Length;
            Material[] materials = new Material[old_len + 1];
            for( var i = 0; i < item.Key.materials.Length; i++)
            {
                materials[i] = item.Key.materials[i];
            }
            materials[old_len] = mat;

            Debug.Log("11mat name:" + mat.name+ " name2:"+ materials[old_len].name);
            item.Key.materials = materials;
            item.Key.materials[old_len].name = mat_string;
            for (var i = 0; i < item.Key.materials.Length; i++)
            {
                Debug.Log("111 remove11 mat name:" + item.Key.materials[i].name );
                
            }
        }

        Debug.Log("22mat name:" + mat.name);
    }
    //删除材质
    public void RemoveMat(string mat_string)
    {
        //Material removemat = Resources.Load<Material>(mat_string);
        //if (removemat == null)
        //{
        //    return;
        //}
        //Debug.Log(" remove mat name:" + removemat.name+ "   removemat.GetInstanceID:"+ removemat.GetInstanceID());
        foreach (var item in m_AllMaterial)
        {
            int removeid = -1;
            
            for (var i = 0; i < item.Key.materials.Length; i++)
            {
                Debug.Log(" remove11 mat name:" + item.Key.materials[i].name + "   removemat.GetInstanceID:" + item.Key.materials[i].GetInstanceID());
                if (item.Key.materials[i].name == mat_string)
                {
                    removeid = i;
                    break;
                }
            }
            Debug.Log(" removeid:" + removeid);
            if (removeid >= 0)
            {
                Material[] materials = new Material[item.Key.materials.Length - 1];
                for (var i = 0; i < item.Key.materials.Length; i++)
                {
                    if( i != removeid)
                    {
                        materials[i] = item.Key.materials[i];
                    }
                    
                }
                item.Key.materials = materials;
            }

            //materials[item.Key.materials.Length] = mat;


            //item.Key.materials = materials;
        }
    }

    public void Reset()
    {
        
        foreach (var item in m_AllMaterial)
        {
            ChangeFirstMaterial(item.Key,item.Value);
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
            //item.Key.materials[0] = mat;
            ChangeFirstMaterial(item.Key, mat);
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
            //item.Key.materials[0] = mat;
            ChangeFirstMaterial(item.Key, mat);
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
            //item.Key.materials[0] = AngerRedMat;
            ChangeFirstMaterial(item.Key, AngerRedMat);
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
