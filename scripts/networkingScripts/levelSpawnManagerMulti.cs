using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class levelSpawnManagerMulti : MonoBehaviour
{
    [SerializeField] private List<spawnManagerMulti> spawnMans;
    [SerializeField] private int spawnSequences;
    [SerializeField] private startUIControllerMulti loadScreen;

    void Start()
    {
        GetComponent<NetworkObject>().Spawn();
    }
    [ServerRpc]
    public void doneSpawningServerRpc()
    {
        doneSpawningClientRpc();
    }

    [ClientRpc]
    public void doneSpawningClientRpc()
    {
        Debug.Log("ASDF");
        spawnSequences--;
        if(spawnSequences == 0)
        {
            loadScreen.DoneLoading();
        }
    }
}
