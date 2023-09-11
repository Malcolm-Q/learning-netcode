using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class spawnManagerMulti : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawners;
    [SerializeField] private float minimumRadius;
    [Header("sizePool should be == katamari collider radius!")]
    [SerializeField] private float sizePool = 0.5f;
    private Transform parent;

    private void Start()
    {
        parent = transform.parent;
        GetComponent<NetworkObject>().Spawn();
        StartCoroutine(instanceDelay());
    }

    private IEnumerator instanceDelay()
    {
        yield return new WaitForSeconds(0.5f);
        transform.parent = parent;
        StartCoroutine(Spawning());
    }

    private IEnumerator Spawning()
    {
        for(int i = 0; i < spawners.Count; i++)
        {
            if(Random.Range(0f,1f) > 0.5f)
            {
                spawners[i].GetComponent<itemSpawnerMulti>().TriggerSpawn();
            }
        }
        
        yield return new WaitForSeconds(0.01f);
        if(sizePool < minimumRadius) StartCoroutine(Spawning());
        else
        {
            for(int y = 0; y < spawners.Count; y++)
            {
                Destroy(spawners[y]);
            }
            transform.parent.gameObject.GetComponent<levelSpawnManagerMulti>().doneSpawningServerRpc();
        }
    }

    public void AddToSizePool(float size)
    {
        sizePool += (size / 45);
    }
}
