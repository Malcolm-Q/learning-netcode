using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class levelManager : MonoBehaviour
{
    public static levelManager instance;

    public float levelTime = 120f;
    public float levelScale = 1;
    private List<SphereCollider> playerSizes;
    public List<float> sizeCheckmarks;
    public List<Transform> spawnPoints;
    [SerializeField] private List<Color> playerColors;
    [SerializeField] private GameObject bot, network, networkMan, local;
    public GameObject itemSpawner;

    private void Start()
    {
        playerSizes = new List<SphereCollider>();
        instance = this;
        for(int i = 0; i < spawnPoints.Count; i++)
        {
            spawnPoints[i].gameObject.SetActive(false);
        }
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.25f);
        if(GameManager.instance.playOnline)
        {
            network.SetActive(true);
            networkMan.SetActive(true);
        }
        else
        {
            local.SetActive(true);
        }
    }

    public List<SphereCollider> GetPlayerSizes()
    {
        return playerSizes;
    }

    public void AddPlayer(SphereCollider sc)
    {
        playerSizes.Add(sc);
        sc.gameObject.GetComponent<MeshRenderer>().material.color = playerColors[playerSizes.Count-1];
    }

    public void EnablePlayers()
    {
        if(GameManager.instance.playOnline) {
            EnablePlayersMultiServerRpc();
            return;
        }
        for(int i = 0; i < playerSizes.Count; i++)
        {
            try
            {
                playerSizes[i].gameObject.GetComponent<playerController>().EnablePlayer();
            }
            catch
            {
                playerSizes[i].gameObject.GetComponent<enemyController>().EnablePlayer();
            }
        }
    }

    [ServerRpc]
    private void EnablePlayersMultiServerRpc()
    {
        EnablePlayersMultiClientRpc();
    }

    [ClientRpc]
    private void EnablePlayersMultiClientRpc()
    {
        for(int i = 0; i < playerSizes.Count; i++)
        {
            try
            {
                playerSizes[i].gameObject.GetComponent<playerControllerMulti>().EnablePlayer();
                
            }
            catch
            {
                playerSizes[i].gameObject.GetComponent<enemyController>().EnablePlayer();
            }
        }
    }

    public void SpawnBot()
    {
        int choice = Random.Range(0,spawnPoints.Count);
        Transform spawn = spawnPoints[choice];
        Destroy(spawnPoints[choice].gameObject);
        spawnPoints.RemoveAt(choice);
        Instantiate(bot,spawn.position,spawn.rotation);
    }
}
