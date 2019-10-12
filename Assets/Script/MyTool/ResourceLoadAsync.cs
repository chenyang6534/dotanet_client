using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyResource
{
    public delegate void Progress(ResourceRequest re);
    public ResourceRequest request;
    public Progress call;
}



public class ResourceLoadAsync : MonoBehaviour {

    

    public static  ResourceLoadAsync Instance;
    public int Count;

    Dictionary<int, MyResource> MyRes = new Dictionary<int, MyResource>();

    
    void Start()
    {
        Instance = this;
        //request = Resources.LoadAsync("tt/myTD00"); //调用异步加载方法
        //Debug.Log("异步方法！");
    }
    public void AddLoad(string path, MyResource.Progress call)
    {
        if(path.Length <= 0)
        {
            return;
        }
        MyResource mres = new MyResource();
        mres.request = Resources.LoadAsync(path);
        mres.call = call;
        MyRes[Count] = mres;
        Count++;
    }

    void Update()
    {
        List<int> removeres = new List<int>();
        foreach(var item in MyRes)
        {
            item.Value.call(item.Value.request);
            if (item.Value.request.isDone)
            {
                removeres.Add(item.Key);
                //MyRes.Remove(item.Key);
            }
        }
        
        foreach(var item in removeres)
        {
            MyRes.Remove(item);
        }

    }
}
