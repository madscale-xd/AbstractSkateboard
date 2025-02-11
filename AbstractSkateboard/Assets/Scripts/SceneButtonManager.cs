using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneButtonManager : MonoBehaviour
{
    public TextMeshProUGUI playerCountText;
    // Start is called before the first frame update
    void Start()
    {
         Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCountText != null)
        {
            playerCountText.text = "PLAYERS SURVIVED:\n " + DeathAndRespawn.playerCount;
        }
    }

    public void QuitGame()
        {
        Application.Quit();
    }

    public void LoadGame()
    {
        DeathAndRespawn.playerCount = 457f;
        SceneManager.LoadScene("Skatepark");
    }

    public void LoadRetry()
    {
        SceneManager.LoadScene("EndMenu");
    }

     public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}