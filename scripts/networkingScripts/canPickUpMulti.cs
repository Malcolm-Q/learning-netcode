using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class canPickUpMulti : NetworkBehaviour
{
    private Rigidbody rb;
    private BoxCollider bc;
    private float size;
    [SerializeField] private NetworkObject networkObject;
    private GameObject player;
    public GameObject skeleton;
    private Vector3 relativePosition;
    private Quaternion relativeRotation;

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("katamari"))
        {
            player = col.gameObject;
            if (size > (col.gameObject.GetComponent<SphereCollider>().radius * levelManager.instance.levelScale) / 1.5f) return;
            bc.enabled=false;
            var to = col.gameObject.GetComponent<NetworkObject>();
            if(IsServer)PerformDestructionOnServerRpc(to.NetworkObjectId);
        }
    }

    [ServerRpc]
    private void PerformDestructionOnServerRpc(ulong targetObject)
    {
        Debug.Log("Server Destruction");
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float adjustedDistance = distanceToPlayer * 0.3f;
        Vector3 adjustedPosition = transform.position + (directionToPlayer.normalized * adjustedDistance);
        relativePosition = player.transform.InverseTransformPoint(adjustedPosition);
        PerformDestructionOnClientRpc(targetObject, relativePosition, relativeRotation);
        //NetworkObject.Destroy(networkObject);
    }

    [ClientRpc]
    private void PerformDestructionOnClientRpc(ulong targetObject, Vector3 relativePosition, Quaternion relativeRotation)
    {
        Debug.Log("Client Destruction");
        PerformDestruction(targetObject, relativePosition, relativeRotation);
    }

    private void PerformDestruction(ulong targetObjectId, Vector3 relativePosition, Quaternion relativeRotation)
    {
        NetworkObject targetObject = null;
        if (targetObjectId != 0)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetObjectId, out NetworkObject targetNetworkObject))
            {
                targetObject = targetNetworkObject;
            }
        }

        if (targetObject != null)
        {
            try
            {
                playerControllerMulti pc = targetObject.gameObject.GetComponent<playerControllerMulti>();

                pc.registerItemServerRpc(gameObject.GetComponent<NetworkObject>(), size, relativePosition, relativeRotation);

            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                enemyController pc = player.GetComponent<enemyController>();
                transform.parent = player.transform;
            }
        }
    }
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        Vector3 s = (bc.size) * transform.localScale.x;
        size = (s.x + s.y + s.z) / 3;
        transform.parent.GetComponent<spawnManagerMulti>().AddToSizePool(size);
        networkObject = GetComponent<NetworkObject>();
        StartCoroutine(EnableCollision());
    }

    private IEnumerator EnableCollision()
    {
        yield return new WaitForSeconds(3f);
        gameObject.layer = 3;
    }
}
