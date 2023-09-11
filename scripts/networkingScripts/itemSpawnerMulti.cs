using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class itemSpawnerMulti : NetworkBehaviour
{
    [SerializeField] private List<GameObject> potentialSpawns;
    [SerializeField] private float scale;
    public spawnManagerMulti spawnMan;

    void Start()
    {
        gameObject.SetActive(false);
        //spawnMan = transform.parent.GetComponent<spawnManagerMulti>();
    }

    public void TriggerSpawn()
    {
        GameObject spawn = Instantiate(potentialSpawns[Random.Range(0,potentialSpawns.Count)], transform.position, Quaternion.Euler(Random.Range(0f,300f),Random.Range(0f,300f),Random.Range(0f,300f)));
        spawn.transform.localScale = new Vector3(scale,scale,scale);
        //spawn.transform.SetParent(transform.parent);

        NetworkObject networkObject = spawn.GetComponent<NetworkObject>();
        networkObject.Spawn();
        networkObject.transform.SetParent(transform.parent);
        //gameObject.SetActive(false);
    }
}
