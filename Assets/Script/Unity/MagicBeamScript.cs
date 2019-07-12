using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MagicBeamScript : MonoBehaviour {

    [Header("Prefabs")]
    //public GameObject beamLineRendererPrefab;
    //public GameObject beamStartPrefab;
    //public GameObject beamEndPrefab;


    public GameObject beamStart;
    public GameObject beamEnd;
    public GameObject beam;
    private LineRenderer line;

    [Header("Adjustable Variables")]
    public float beamEndOffset = 1f; //How far from the raycast hit point the end effect is positioned
    public float textureScrollSpeed = 8f; //How fast the texture scrolls along the beam
	public float textureLengthScale = 3; //Length of the beam texture

  

    // Use this for initialization
    void Start()
    {
        //if(beamStartPrefab != null)
        //{
        //    beamStart = Instantiate(beamStartPrefab) as GameObject;
        //    beamStart.transform.parent = GameScene.Singleton.m_GameScene.transform;
        //}
        //if (beamEndPrefab != null)
        //{
        //    beamEnd = Instantiate(beamEndPrefab) as GameObject;
        //    beamEnd.transform.parent = GameScene.Singleton.m_GameScene.transform;
        //}
        //if (beamLineRendererPrefab != null)
        //{
        //    beam = Instantiate(beamLineRendererPrefab) as GameObject;
        //    beam.transform.parent = GameScene.Singleton.m_GameScene.transform;
        //    line = beam.GetComponent<LineRenderer>();
        //}
        if(beam != null)
        {
            line = beam.GetComponent<LineRenderer>();
        }
        //ShootBeamInDir(new Vector3(0,0,0),new Vector3(10,0,0));



    }

    void Destroy()
    {
        if(beamStart != null)
        {
            Destroy(beamStart);
        }
        if (beamEnd != null)
        {
            Destroy(beamEnd);
        }
        if (beam != null)
        {
            Destroy(beam);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
       

       
		
		
    }
    

    

    public void ShootBeamInDir(Vector3 start, Vector3 end1)
    {
        
        Vector3 dir = end1 - start;
        Vector3 end = end1 - (dir.normalized * beamEndOffset);
        //Debug.Log("start:" + start + " end:" + end);
        if (line != null)
        {
            line.positionCount = 2;
            line.SetPosition(0, start);
            line.SetPosition(1, end);
            float distance = Vector3.Distance(start, end);
            line.sharedMaterial.mainTextureScale = new Vector2(distance / textureLengthScale, 1);
            line.sharedMaterial.mainTextureOffset -= new Vector2(Time.deltaTime * textureScrollSpeed, 0);
        }
        if (beamStart != null)
        {
            beamStart.transform.position = start;
            beamStart.transform.LookAt(end);
        }
        if (beamEnd != null)
        {
            beamEnd.transform.position = end;
            beamEnd.transform.LookAt(start);
        }
        
        
    }
}
