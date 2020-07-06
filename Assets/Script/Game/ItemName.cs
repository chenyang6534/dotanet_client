using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class ItemName : MonoBehaviour {
    public string Name = "item1";
    public Object WordsObj;
    // Use this for initialization
    public GTextField NameTxt;
    protected GameObject Obj;

    void OnLoad()
    {
        LoadName(Name,0);
    }
    
	void Start () {

        //var words = (GameObject)(GameObject.Instantiate(WordsObj));
        //words.transform.parent = gameObject.transform;
        //words.transform.localPosition = new Vector3(0, 0, 0);
        //words.GetComponent<UIPanel>().ui.GetChild("text").asTextField.text = Name;
        //NameTxt = words.GetComponent<UIPanel>().ui.GetChild("text").asTextField;
    }

    public void LoadName(string name,int isbest)
    {
        if(isbest == 1)
        {
            name = "[color=#ff2222]" + name + "[/color]";
        }
        //AddDes("[color=#ff2222]");
        if (Obj != null)
        {
            Obj.GetComponent<UIPanel>().ui.GetChild("text").asTextField.text = name;
            return;
        }
        Obj = (GameObject)(GameObject.Instantiate(WordsObj));
        Obj.transform.parent = gameObject.transform;
        Obj.transform.localPosition = new Vector3(0, 0, 0);
        Obj.GetComponent<UIPanel>().ui.GetChild("text").asTextField.text = name;
        //NameTxt = Obj.GetComponent<UIPanel>().ui.GetChild("text").asTextField;
    }
    
	// Update is called once per frame
	void Update () {
		
	}
}
