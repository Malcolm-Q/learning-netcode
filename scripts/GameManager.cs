using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool playOnline = false;
    void Start()
    {
        if(GameObject.FindGameObjectsWithTag("GameController").Length > 1) {
            Destroy(this);
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
}
