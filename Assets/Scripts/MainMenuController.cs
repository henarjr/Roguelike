using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainButtonsPanel;
    public GameObject levelSelectionPanel;
    public GameObject settingsPanel;

    public Button continueButton;

    void Start()
    {
        ShowMainMenu();
        if (PlayerPrefs.HasKey("SavedEnvironment"))
            continueButton.interactable = true;
        else
            continueButton.interactable = false;
    }

    public void ShowMainMenu()
    {
        mainButtonsPanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void OnNewGameClicked()
    {
        PlayerPrefs.DeleteAll(); 
        mainButtonsPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
    }

    public void OnContinueClicked()
    {
        string savedEnv = PlayerPrefs.GetString("SavedEnvironment");
        if (!string.IsNullOrEmpty(savedEnv))
        {
            SceneManager.LoadScene(savedEnv);
        }
    }

    public void OpenSettings()
    {
        mainButtonsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void LoadLevel(string levelName)
    {
        PlayerPrefs.SetString("SavedEnvironment", levelName);
        PlayerPrefs.Save();
        SceneManager.LoadScene(levelName);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Closed!");
    }
}