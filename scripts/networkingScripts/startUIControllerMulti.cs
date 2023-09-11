using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class startUIControllerMulti : NetworkBehaviour
{
    [SerializeField] private Text text;
    [SerializeField]private GameObject GameUI, startButton, spawner;

    public override void OnNetworkSpawn()
    {
        if(NetworkManager.Singleton.IsServer) text.text = "Your friends can join now.\nPress Start once everyone is in.";
        else text.text = "You have successfully connected.\nWait for the host to start the game.";
    }

    public void DoneLoading()
    {
        if(NetworkManager.Singleton.IsServer) text.text = "Your friends can join now.\nPress Start once everyone is in.";
        startButton.SetActive(true);
    }

    [ServerRpc]
    public void StartGameServerRpc()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        Destroy(spawner);
        StartCoroutine(start());
    }

    public void SpawnBot()
    {
        levelManager.instance.SpawnBot();
        //text.text = (players).ToString() + " players have joined the game!";
    }

    private IEnumerator start()
    {
        yield return new WaitForSeconds(0.25f);
        levelManager.instance.EnablePlayers();
        //startButton.transform.GetChild(1).gameObject.SetActive(false);
        GameUI.SetActive(true);
        Destroy(gameObject);
    }
}
