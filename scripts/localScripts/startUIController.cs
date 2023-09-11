using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class startUIController : MonoBehaviour
{
    [SerializeField] private Text text;
    private GameObject playerSpawn;
    [SerializeField]private GameObject GameUI, startButton, spawner;
    private int players;

    void Start()
    {
        playerSpawn = transform.parent.GetChild(0).gameObject;
        playerSpawn.GetComponent<PlayerInputManager>().DisableJoining();
    }

    public void DoneLoading()
    {
        playerSpawn.GetComponent<PlayerInputManager>().EnableJoining();
        text.text = "Press START to join!";
        startButton.SetActive(true);
    }

    public void playerJoined()
    {
        players++;
        text.text = (players).ToString() + " players have joined the game!";
        if(players == 4)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if(players == 0) return;
        Destroy(spawner);
        StartCoroutine(start());
    }

    public void SpawnBot()
    {
        levelManager.instance.SpawnBot();
        players++;
        text.text = (players).ToString() + " players have joined the game!";
        if(players == 4)
        {
            StartGame();
        }
    }

    private IEnumerator start()
    {
        yield return new WaitForSeconds(0.25f);
        levelManager.instance.EnablePlayers();
        startButton.transform.GetChild(1).gameObject.SetActive(false);
        GameUI.SetActive(true);
        Destroy(gameObject);
    }
}
