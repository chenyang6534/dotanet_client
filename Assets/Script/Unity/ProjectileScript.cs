using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class ProjectileScript : MonoBehaviour {
    
	public GameObject muzzlePrefab;
	public GameObject hitPrefab;
	public AudioClip shotSFX;
	public AudioClip hitSFX;
	public List<GameObject> trails;
    public bool IsRotationHit = true;

    protected bool m_IsDestroy;
	void Start () {
        m_IsDestroy = false;


        
	}

    public void ShowStartParticle(Vector3 pos,Vector3 endpos)
    {
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab);
            muzzleVFX.transform.position = pos;
            muzzleVFX.transform.localScale = gameObject.transform.localScale;
            muzzleVFX.transform.LookAt(endpos);
            //muzzleVFX.transform.localRotation = gameObject.transform.localRotation;
            //muzzleVFX.transform.forward = gameObject.transform.forward;

            //Debug.Log("ShowStartParticle localRotation:" + gameObject.transform.localRotation);
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }

        if (shotSFX != null && GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().PlayOneShot(shotSFX);
        }
    }
    public void ShowEndParticle(Vector3 direction)
    {
        //Debug.Log("111time:" + Time.frameCount);
        if (m_IsDestroy == true)
        {
            return;
        }
        if (shotSFX != null && GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().PlayOneShot(hitSFX);
        }

       
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, gameObject.transform.forward);
        Vector3 pos = gameObject.transform.position;

        if (hitPrefab != null)
        {
            //var hitVFX = Instantiate(hitPrefab, pos, rot);
            var hitVFX = Instantiate(hitPrefab);
            hitVFX.transform.position = gameObject.transform.position;
            //hitVFX.transform.localScale = gameObject.transform.localScale;
            //hitVFX.transform.localRotation = gameObject.transform.localRotation;
            //var q1:Quaternion;SetLookRotation
            if(IsRotationHit == true)
            {
                Quaternion q = new Quaternion();
                q.SetLookRotation(direction);
                hitVFX.transform.rotation = q;
            }
            

            //hitVFX.transform.rotation.SetLookRotation(q);
            //hitVFX.transform.localRotation = q1;
            Debug.Log("local rotation:" + hitVFX.transform.localRotation+" rotation:"+ hitVFX.transform.rotation);
            //hitVFX.transform.forward = gameObject.transform.forward;
            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null)
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        }

        //StartCoroutine(DestroyParticle(0f));
    }
    public void Destroy()
    {
        //Debug.Log("222time:" +Time.frameCount);
        if (m_IsDestroy == false)
        {
            m_IsDestroy = true;
            Destroy(gameObject);

        }
        //StartCoroutine(DestroyParticle(0.01f));
    }
    
	public IEnumerator DestroyParticle (float waitTime) {
        
        if (m_IsDestroy == false)
        {
            m_IsDestroy = true;
            //if (transform.childCount > 0 && waitTime != 0)
            //{
            //    List<Transform> tList = new List<Transform>();

            //    foreach (Transform t in transform.GetChild(0).transform)
            //    {
            //        tList.Add(t);
            //    }
            //    while (transform.GetChild(0).localScale.x > 0)
            //    {
            //        yield return new WaitForSeconds(0.01f);
            //        transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            //        //Debug.Log("----localScale:" + transform.GetChild(0).localScale);
            //        for (int i = 0; i < tList.Count; i++)
            //        {
            //            tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            //        }
                    
            //    }
            //}
            
            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);
            
        }

		
	}


}
