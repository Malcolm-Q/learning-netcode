using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Netcode;

public class playerControllerMulti : NetworkBehaviour
{
    [SerializeField] Vector3 forwardVector = Vector3.forward;
    [SerializeField] Vector3 rightVector = Vector3.right;
    [SerializeField] private float TurningRotSpeed, ForwardRotSpeed;
    [SerializeField] SphereCollider sc;
    [SerializeField] private List<GameObject> collectedItems;
    private cameraControllerMulti camCont;
    private Rigidbody rb;
    [SerializeField] private float forwardRotTmp;
    [SerializeField] GameObject cam;
    private Vector2 joystickInput;
    private Text sizeText;
    private bool canBoost = true, Enabled = false;

    public override void OnNetworkSpawn()
    {
        
    }

    void Start()
    {
        if(NetworkManager.Singleton.IsServer)
        {
            try{
                levelManager.instance.itemSpawner.SetActive(true);
            }
            catch
            {
                Debug.Log("Client thinks it's server");
            }
            
        }
        levelManager.instance.AddPlayer(sc);
        int spawnChoice = UnityEngine.Random.Range(0,levelManager.instance.spawnPoints.Count);
        transform.position = levelManager.instance.spawnPoints[spawnChoice].position;
        Destroy(levelManager.instance.spawnPoints[spawnChoice].gameObject);
        levelManager.instance.spawnPoints.RemoveAt(spawnChoice);
        rb = GetComponent<Rigidbody>();
        forwardRotTmp = ForwardRotSpeed;
        transform.localScale *= levelManager.instance.levelScale;
        if(IsOwner){
            GameObject camera = Instantiate(cam);
            camera.GetComponent<EasyObjectsFade>().setPlayerTransform(transform);
            camCont = camera.GetComponent<cameraControllerMulti>();
            camCont.setPlayer(gameObject);
            sizeText = camera.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
            sizeText.text = "Size: " + (sc.radius * transform.localScale.x).ToString("F3") + " meters";
        }
    }

    public void EnablePlayer()
    {
        Enabled = true;
        Debug.Log("Enabled");
    }

    private void FixedUpdate()
    {
        if(!Enabled) return;
        UserInputHandler();
    }

    [ServerRpc]
    public void registerItemServerRpc(NetworkObjectReference go, float size, Vector3 relativePosition, Quaternion relativeRotation)
    {
        
        RpcUpdateItemRegistrationClientRpc(go, size, relativePosition, relativeRotation);
    }

    [ClientRpc]
    private void RpcUpdateItemRegistrationClientRpc(NetworkObjectReference item, float size, Vector3 relativePosition, Quaternion relativeRotation)
    {
        
        sc.radius += (size/45);
        item.TryGet(out NetworkObject no);
        ForwardRotSpeed += (size*5);
        GameObject prefab = no.gameObject.GetComponent<canPickUpMulti>().skeleton;
        GameObject go = Instantiate(prefab, transform.TransformPoint(relativePosition),(transform.rotation * relativeRotation));
        
        go.transform.parent = transform;
        Destroy(no.gameObject);
        collectedItems.Add(go);

        int c = collectedItems.Count;
        if(c > 30)
        {
            Destroy(collectedItems[c-31].GetComponent<BoxCollider>());
        }
        if(c > 200)
        {
            Destroy(collectedItems[0]);
            collectedItems.RemoveAt(0);
        }
        if(IsOwner)
        {
            sizeText.text = "Size: " + (sc.radius * transform.localScale.x).ToString("F3") + " meters";
            camCont.multiplyOffset(sc.radius);
        }
        
    }
    
    private void UserInputHandler()
    {
        // move
        float vert = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");
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

        //boost
        if(!canBoost) return;
        float rightStickX = Input.GetAxis("rightStickX");
        float rightStickY = Input.GetAxis("rightStickY");
        rightStickY *= -1;
        if(rightStickX != 0 || rightStickY != 0)
        {
            float angle = Mathf.Atan2(rightStickX, rightStickY) * Mathf.Rad2Deg;
            forwardVector = Quaternion.Euler(0.0f, angle, 0.0f) * forwardVector;
            rightVector = Quaternion.Euler(0.0f, angle, 0.0f) * rightVector;
            rb.velocity = Vector3.zero;
            rb.AddForce(forwardVector * 300);
            rb.AddTorque(rightVector * ForwardRotSpeed * 10);
            canBoost = false;
            StartCoroutine(boostCooldown());
        }
        
    }

    private IEnumerator boostCooldown()
    {
        yield return new WaitForSeconds(2f);
        canBoost = true;
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "katamari")
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

    public Vector3 GetForwardVector()
    {
        return forwardVector;
    }
}
