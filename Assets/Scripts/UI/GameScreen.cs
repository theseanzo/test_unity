using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameScreen : MonoBehaviour
{
    public ExitPopup exitPopup;
    public TextMeshProUGUI textPrefab;
    public static bool isPaused;
    private TextMeshProUGUI[] textFields;
    
    private int nTeams;
    // Start is called before the first frame update
    void Start()
    {
        nTeams = GameManager.Instance.teams.Length;
        textFields = new TextMeshProUGUI[nTeams];
        for(int i = 0; i < nTeams; i++)
        {
            TextMeshProUGUI textField = Instantiate(textPrefab);
            textField.transform.SetParent(textPrefab.transform.parent, false);
            textField.color = GameManager.Instance.teams[i];
            textFields[i] = textField;
        }
        Destroy(textPrefab.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < nTeams; i++)
        {
            textFields[i].text = ScoreManager.Instance.scores[i].ToString();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isPaused = !isPaused;
            exitPopup.gameObject.SetActive(isPaused);
        }
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }
}
