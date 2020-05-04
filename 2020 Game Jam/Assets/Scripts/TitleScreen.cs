using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 1;
    }

    public GameObject pauseMenu;
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Continue()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void QuitToTitle()
    {
        SceneManager.LoadScene(0);
    }
}
