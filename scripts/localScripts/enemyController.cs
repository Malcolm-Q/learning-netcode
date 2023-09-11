using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class enemyController : MonoBehaviour
{
    Vector3 forwardVector = Vector3.forward;
    Vector3 rightVector = Vector3.right;
    [SerializeField] private float TurningRotSpeed, ForwardRotSpeed;
    [SerializeField] SphereCollider sc;
    private List<GameObject> collectedItems;
    private Rigidbody rb;
    private float forwardRotTmp;
    [SerializeField] private float clock, vert, hor;
    [SerializeField] private float newMoveCooldown = 1.5f;
    /*[SerializeField] private List<GameObject> landmarks;
    private float landmarkDistance;
    private int currentLandmark = 0, lastLandmark = -1;*/
    public bool outOfBounds, Enabled;
    public GameObject currentTarget;


    void Start()
    {
        levelManager.instance.AddPlayer(sc);
        rb = GetComponent<Rigidbody>();
        forwardRotTmp = ForwardRotSpeed;
        collectedItems = new List<GameObject>();

        transform.localScale *= levelManager.instance.levelScale;

        int spawnChoice = UnityEngine.Random.Range(0,levelManager.instance.spawnPoints.Count);
        transform.position = levelManager.instance.spawnPoints[spawnChoice].position;
        Destroy(levelManager.instance.spawnPoints[spawnChoice].gameObject);
        levelManager.instance.spawnPoints.RemoveAt(spawnChoice);
    }

    public void EnablePlayer()
    {
        Enabled = true;
    }

    public void registerItem(GameObject go, float size)
    {
        sc.radius += (size/45);
        ForwardRotSpeed += (size*5);
        collectedItems.Add(go);

        /*if(sc.radius > levelManager.instance.sizeCheckmarks[currentLandmark])
        {
            currentLandmark++;
        }*/

        int c = collectedItems.Count;
        if(c > 30)
        {
            Destroy(collectedItems[c-31].GetComponent<BoxCollider>());
        }
        if(c > 100)
        {
            Destroy(collectedItems[0]);
            collectedItems.RemoveAt(0);
        }
        
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Wall" || col.gameObject.tag == "obstacle")
        {
            vert *= -1;
            hor *= -1;
        }
        else if(col.gameObject.tag == "katamari")
        {
            if((sc.radius * levelManager.instance.levelScale) / 2 <= (col.gameObject.GetComponent<SphereCollider>().radius * levelManager.instance.levelScale)) return;
            
            col.gameObject.transform.position = Vector3.Lerp(col.gameObject.transform.position, transform.position, 0.4f);
            col.gameObject.transform.parent = transform;
            Destroy(col.gameObject.GetComponent<Rigidbody>());
            try
            {
                playerController pc = col.gameObject.GetComponent<playerController>();
                Destroy(pc);
            }
            catch (Exception)
            {
                enemyController pc = col.gameObject.GetComponent<enemyController>();
                Destroy(pc);
            }
        }
    }

    private void Update()
    {
        if(!Enabled) return;
        EnemyMovement();
        clock += Time.deltaTime;
        if(clock > newMoveCooldown)
        {
            clock = 0f;
            DecideNewMove();
        }
    }

    private void DecideNewMove()
    {
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("item");
        int layerMask2 = LayerMask.GetMask("obstacle");
        if (Physics.SphereCast(transform.position, sc.radius * 2, transform.forward, out hit, 2,layerMask))
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(transform.position, hit.point - transform.position, out raycastHit, Vector3.Distance(transform.position, hit.point), layerMask2))
            {
                Debug.Log("Object in between");
                if(UnityEngine.Random.value<0.5f)
                vert=-1f;
                else
                    vert=1f;
                hor = UnityEngine.Random.Range(-0.5f,0.5f);
            }
            else{
                currentTarget = hit.collider.gameObject;
                Vector3 target = hit.collider.gameObject.transform.position;

                Vector3 direction = target - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                forwardVector = rotation * Vector3.forward;
                rightVector = rotation * Vector3.right;

                vert = 1f;
                hor = 0f;
            }
            
        }
        else
        {
            if(UnityEngine.Random.value<0.5f)
            vert=-1f;
            else
                vert=1f;
            hor = UnityEngine.Random.Range(-0.5f,0.5f);
        }
        
    }

    private void EnemyMovement()
    {
        if(vert != 0f)
        {
            float velo = (rb.velocity.x + rb.velocity.y + rb.velocity.z) / 3;
            velo = Mathf.Clamp(Mathf.Abs(velo),0f,1f);
            float bonus = ForwardRotSpeed * 2 - (velo * (ForwardRotSpeed*2));
            forwardRotTmp = ForwardRotSpeed + bonus;
            rb.AddTorque(rightVector * forwardRotTmp * vert * Time.deltaTime);
        }

        forwardVector = Quaternion.Euler(0.0f, TurningRotSpeed * hor, 0.0f) * forwardVector;
        rightVector = Quaternion.Euler(0.0f, TurningRotSpeed * hor, 0.0f) * rightVector;
    }
}
