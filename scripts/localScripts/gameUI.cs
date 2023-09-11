using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameUI : MonoBehaviour
{
    [SerializeField] private Text timer, endText;
    [SerializeField] private GameObject endScreen;
    private float clock;
    
    void Start()
    {
        clock = levelManager.instance.levelTime;
    }

    void Update()
    {
        clock -= Time.deltaTime;
        timer.text = clock.ToString("F1");
        if(clock <= 0f)
        {
            endScreen.SetActive(true);
            List<SphereCollider> playerSizes = levelManager.instance.GetPlayerSizes();
            for(int i = 0; i < playerSizes.Count; i++)
            {
                endText.text += "Player " + (i+1).ToString() + " size: " + (playerSizes[i].radius * levelManager.instance.levelScale).ToString("F3") + " meters\n\n";
            }
            StartCoroutine(restart());
            clock = 99999f;
        }
    }

    private IEnumerator restart()
    {
        yield return new WaitForSeconds(5f);
        try
        {
            Destroy(NetworkManager.Singleton.gameObject);
            Destroy(NetworkManager.Singleton);
        }
        catch
        {

        }
        SceneManager.LoadScene(1);
    }
}
