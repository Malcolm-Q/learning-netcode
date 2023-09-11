using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanPickUp : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider bc;
    private float size;

    void Start()
    {
        bc = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        Vector3 s = (bc.size) * transform.localScale.x;
        size = (s.x + s.y + s.z) / 3;
        transform.parent.gameObject.GetComponent<SpawnManager>().AddToSizePool(size);
        StartCoroutine(EnableCollision());
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "katamari")
        {
            if(size > (col.gameObject.GetComponent<SphereCollider>().radius * levelManager.instance.levelScale) / 1.5f) return;
            try
            {
                playerController pc = col.gameObject.GetComponent<playerController>();
                transform.position = Vector3.Lerp(transform.position, col.gameObject.transform.position,0.4f);
                transform.parent = col.gameObject.transform;
                
                pc.registerItem(gameObject, size);
            }
            catch
            {
                enemyController pc = col.gameObject.GetComponent<enemyController>();
                //transform.position = Vector3.Lerp(transform.position, col.gameObject.transform.position,0.4f);
                transform.parent = col.gameObject.transform;
                
                pc.registerItem(gameObject, size);
            }
            
            Destroy(rb);
            Destroy(this);
        }
    }

    private IEnumerator EnableCollision()
    {
        yield return new WaitForSeconds(3f);
        gameObject.layer = 3;
    }
}
