using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    [SerializeField] private GameObject startingScreen, levelSelection, optionsScreen;
    [SerializeField] private Image thumbnail;
    [SerializeField] private Text levelName;
    [SerializeField] private List<levelDescription> levelDescriptions;
    private int selectedLevel = 0;

    private bool options, select;

    public void ChooseLevel()
    {
        SceneManager.LoadScene((levelDescriptions[selectedLevel].sceneNumber));
    }

    public void nextLevel(bool left)
    {
        if(left)
        {
            selectedLevel--;
            if(selectedLevel < 0) selectedLevel = levelDescriptions.Count-1;
            thumbnail.sprite = levelDescriptions[selectedLevel].thumbnail;
            levelName.text = levelDescriptions[selectedLevel].levelName;
        }
        else
        {
            selectedLevel++;
            if(selectedLevel == levelDescriptions.Count) selectedLevel = 0;
            thumbnail.sprite = levelDescriptions[selectedLevel].thumbnail;
            levelName.text = levelDescriptions[selectedLevel].levelName;
        }
    }

    public void StartButton()
    {
        if(select)
        {
            startingScreen.SetActive(true);
            levelSelection.SetActive(false);
            select = false;
        }
        else{
            GameManager.instance.playOnline = false;
            selectedLevel = 0;
            thumbnail.sprite = levelDescriptions[selectedLevel].thumbnail;
            levelName.text = levelDescriptions[selectedLevel].levelName;
            startingScreen.SetActive(false);
            levelSelection.SetActive(true);
            select = true;
        }
    }

    public void StartOnlineButton()
    {
        if(select)
        {
            startingScreen.SetActive(true);
            levelSelection.SetActive(false);
            select = false;
        }
        else{
            GameManager.instance.playOnline = true;
            selectedLevel = 0;
            thumbnail.sprite = levelDescriptions[selectedLevel].thumbnail;
            levelName.text = levelDescriptions[selectedLevel].levelName;
            startingScreen.SetActive(false);
            levelSelection.SetActive(true);
            select = true;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Options()
    {
        if(options)
        {
            startingScreen.SetActive(true);
            optionsScreen.SetActive(false);
            options = false;
        }
        else
        {
            startingScreen.SetActive(false);
            optionsScreen.SetActive(true);
            options = true;
        }
        
    }
}
