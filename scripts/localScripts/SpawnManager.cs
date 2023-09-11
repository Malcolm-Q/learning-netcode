using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawners;
    [SerializeField] private float minimumRadius;
    [Header("sizePool should be == katamari collider radius!")]
    [SerializeField] private float sizePool = 0.5f;

    private void Start()
    {
        StartCoroutine(Spawning());
    }

    private IEnumerator Spawning()
    {
        for(int i = 0; i < spawners.Count; i++)
        {
            if(Random.Range(0f,1f) > 0.5f)
            {
                spawners[i].GetComponent<itemSpawner>().TriggerSpawn();
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
            transform.parent.gameObject.GetComponent<levelSpawnManager>().doneSpawning();
        }
    }

    public void AddToSizePool(float size)
    {
        sizePool += (size / 45);
    }
}
