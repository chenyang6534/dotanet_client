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

    protected bool m_IsDestroy;
	void Start () {
        m_IsDestroy = false;


        
	}

    public void ShowStartParticle()
    {
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab);
            muzzleVFX.transform.position = transform.position;
            muzzleVFX.transform.localScale = gameObject.transform.localScale;
            muzzleVFX.transform.localRotation = gameObject.transform.localRotation;
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
    public void ShowEndParticle()
    {
        if (m_IsDestroy == true)
        {
            return;
        }
        if (shotSFX != null && GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().PlayOneShot(hitSFX);
        }

        //if (trails.Count > 0)
        //{
        //    for (int i = 0; i < trails.Count; i++)
        //    {
        //        trails[i].transform.parent = null;
        //        var ps = trails[i].GetComponent<ParticleSystem>();
        //        if (ps != null)
        //        {
        //            ps.Stop();
        //            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        //        }
        //    }
        //}
        
        GetComponent<Rigidbody>().isKinematic = true;

        //ContactPoint contact = co.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, gameObject.transform.forward);
        Vector3 pos = gameObject.transform.position;

        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot);
            hitVFX.transform.localScale = gameObject.transform.localScale;
            hitVFX.transform.forward = gameObject.transform.forward;
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
        StartCoroutine(DestroyParticle(1.1f));
    }
    
	public IEnumerator DestroyParticle (float waitTime) {
        
        if (m_IsDestroy == false)
        {
            m_IsDestroy = true;
            if (transform.childCount > 0 && waitTime != 0)
            {
                List<Transform> tList = new List<Transform>();

                foreach (Transform t in transform.GetChild(0).transform)
                {
                    tList.Add(t);
                }
                while (transform.GetChild(0).localScale.x > 0)
                {
                    yield return new WaitForSeconds(0.01f);
                    transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                    for (int i = 0; i < tList.Count; i++)
                    {
                        tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                    }
                    
                }
            }
            
            yield return new WaitForSeconds(waitTime);
            //isdestroy = true;
            Destroy(gameObject);
            
        }

		
	}


}
