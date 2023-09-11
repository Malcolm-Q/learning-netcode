using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class playerController : MonoBehaviour
{
    [SerializeField] Vector3 forwardVector = Vector3.forward;
    [SerializeField] Vector3 rightVector = Vector3.right;
    [SerializeField] private float TurningRotSpeed, ForwardRotSpeed;
    [SerializeField] SphereCollider sc;
    [SerializeField] private List<GameObject> collectedItems;
    private cameraController camCont;
    private Rigidbody rb;
    [SerializeField] private float forwardRotTmp;
    [SerializeField] GameObject cam;
    private Vector2 joystickInput;
    private Text sizeText;
    public bool canBoost = true, Enabled;


    void Start()
    {
        levelManager.instance.AddPlayer(sc);
        int spawnChoice = UnityEngine.Random.Range(0,levelManager.instance.spawnPoints.Count);
        transform.position = levelManager.instance.spawnPoints[spawnChoice].position;
        Destroy(levelManager.instance.spawnPoints[spawnChoice].gameObject);
        levelManager.instance.spawnPoints.RemoveAt(spawnChoice);
        rb = GetComponent<Rigidbody>();
        forwardRotTmp = ForwardRotSpeed;
        transform.localScale *= levelManager.instance.levelScale;
 
        GameObject camera = Instantiate(cam);
        camera.GetComponent<EasyObjectsFade>().setPlayerTransform(transform);
        camera.GetComponent<cameraController>().setPlayer(gameObject);
        camCont = camera.GetComponent<cameraController>();
        GetComponent<PlayerInput>().camera=camera.GetComponent<Camera>();
        sizeText = camera.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
        sizeText.text = "Size: " + (sc.radius * transform.localScale.x).ToString("F3") + " meters";
    }

    public void EnablePlayer()
    {
        Enabled = true;
    }

    private void FixedUpdate()
    {
        UserInputHandler();
    }

    public void registerItem(GameObject go, float size)
    {
        sc.radius += (size/45);
        sizeText.text = "Size: " + (sc.radius * transform.localScale.x).ToString("F3") + " meters";
        ForwardRotSpeed += (size*5);
        collectedItems.Add(go);
        camCont.multiplyOffset(sc.radius);
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
    
    private void UserInputHandler()
    {
        float vert = joystickInput[1];
        float hor = joystickInput[0];
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

    public void OnActionTriggered(InputAction.CallbackContext context)
    {
        if(!Enabled) return;
        if (context.action.name == "Look")
        {
            if(canBoost)
            {
                Vector2 rightStickInput = context.ReadValue<Vector2>();

                float rightStickX = rightStickInput.x;
                float rightStickY = rightStickInput.y;

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
        if (context.action.name == "Move")
        {
            joystickInput = context.ReadValue<Vector2>();
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
