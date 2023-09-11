using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switchSceneTimer : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private int sceneNumber;
    private float clock;

    void Update()
    {
        clock += Time.deltaTime;
        if(clock > time)
        {
            clock = -100f;
            SceneManager.LoadScene(sceneNumber);
            Destroy(this);
        }
    }
}
