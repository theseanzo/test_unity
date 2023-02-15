using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPopup : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnYesButton()
    {
        GameScreen.isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }
    public void OnNoButton()
    {
        GameScreen.isPaused = false;
        gameObject.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
