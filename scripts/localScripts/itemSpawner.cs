using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSpawner : MonoBehaviour
{

    [SerializeField] private List<GameObject> potentialSpawns;
    [SerializeField] private float scale;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void TriggerSpawn()
    {
        GameObject spawn = Instantiate(potentialSpawns[Random.Range(0,potentialSpawns.Count)], transform.position, Quaternion.Euler(Random.Range(0f,300f),Random.Range(0f,300f),Random.Range(0f,300f)));
        spawn.transform.localScale = new Vector3(scale,scale,scale);
        spawn.transform.parent = transform.parent;
        //gameObject.SetActive(false);
    }
}
