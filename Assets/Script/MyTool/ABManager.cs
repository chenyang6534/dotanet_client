using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABManager{

    public static AssetBundle HeroAB;
    public static void LoadAB()
    {
        if(HeroAB == null)
        {
            HeroAB = AssetBundle.LoadFromFile("AssetBundles/hero1.ab");
        }
        
        //ab1.LoadAsset<Object>("t1");
    }
    public static Object LoadHero(string path)
    {
        if(HeroAB == null)
        {
            Debug.Log("---LoadHero--11111");
            return Resources.Load(path);
        }
        var p1 = path.Split('/');
        Object re1 = HeroAB.LoadAsset(p1[p1.Length - 1]);
        if(re1 == null)
        {
            Debug.Log("---LoadHero--22222");
            return Resources.Load(path);
        }
        Debug.Log("------------------------------LoadHero"+path+"----:"+ Resources.Load(path));
        return re1;
    }
    
}
