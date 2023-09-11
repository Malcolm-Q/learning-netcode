using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControllerMulti : MonoBehaviour
{
    [SerializeField] private GameObject katamari;
    [SerializeField] private Vector3 offset;
    private Vector3 baseOffset;

    playerControllerMulti katamariController;
    Vector3 forwardVector;
    private Transform targetObject;
    public float maxDistance = 5f;
    public LayerMask obstacleLayer;

    public void multiplyOffset(float size)
    {
        offset = baseOffset * (1 + size);
    }

    public void setPlayer(GameObject go)
    {
        katamari = go;
    }

	void Start ()
    {
        targetObject = katamari.transform; 
        baseOffset = offset;
        multiplyOffset(0.5f);

        katamariController = katamari.GetComponent<playerControllerMulti>();

        transform.position = katamari.transform.position + offset;
	}
    
	void Update()
    {
        forwardVector = katamariController.GetForwardVector();
        
        Vector3 newPos = katamari.transform.position + Vector3.Scale(offset, forwardVector);
        newPos.y += offset.y;
        
        RaycastHit hit;
        if (Physics.Raycast(katamari.transform.position, -forwardVector, out hit, maxDistance, obstacleLayer))
        {
            newPos = hit.point;
            newPos.y += offset.y;
        }

        transform.position = newPos;
        transform.LookAt(new Vector3(katamari.transform.position.x, transform.position.y - offset.y / 4, katamari.transform.position.z));
    }
}
