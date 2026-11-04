using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public AudioSource musicSource; 

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pauseMenuPanel.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void SaveAndQuit()
    {

        GameManager.Instance.SaveProgress(SceneManager.GetActiveScene().name);

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
    
    public void SetMainVolume(float volume)
    {
        AudioListener.volume = volume;
    }

 
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

   
    public void SetSFXVolume(float volume)
    {
        
        PlayerPrefs.SetFloat("SFXVolume", volume);

        
        AudioSource[] allSounds = FindObjectsOfType<AudioSource>();
        foreach (AudioSource sound in allSounds)
        {
            if (sound != musicSource)
            {
                sound.volume = volume;
            }
        }
    }
}