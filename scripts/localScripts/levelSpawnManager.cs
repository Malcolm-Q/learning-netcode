using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelSpawnManager : MonoBehaviour
{
    [SerializeField] private int spawnSequences;
    [SerializeField] private startUIController loadScreen;
    public void doneSpawning()
    {
        spawnSequences--;
        if(spawnSequences == 0)
        {
            loadScreen.DoneLoading();
        }
    }
}
