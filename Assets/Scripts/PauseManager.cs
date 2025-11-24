using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Image dimBackground;
    private bool isPaused = false;
    public GameObject gameStateMachine;    
    public float distance = 2.5f;
    public Vector3 offset = new Vector3(0f, -0.3f, 0f);
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider voiceSlider;
    public Slider soundFXSlider;
    public float fadeDuration = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenuUI.SetActive(false);
        SetPosition();
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        voiceSlider.onValueChanged.AddListener(SetVoiceVolume);
        soundFXSlider.onValueChanged.AddListener(SetSoundFXVolume);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (isPaused) 
            {
                Resume();
            }
            else
            {
                Pause();
            }                    
        }       
    }

    public void SetPosition()
    {
        Camera centerEye = Camera.main;
        Vector3 forwardPos = centerEye.transform.position + centerEye.transform.forward * distance;
        Vector3 finalPos = forwardPos + centerEye.transform.TransformVector(offset);
        pauseMenuUI.transform.position = finalPos;
        pauseMenuUI.transform.rotation = Quaternion.LookRotation(pauseMenuUI.transform.position - centerEye.transform.position);
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        StartCoroutine(FadeBackground(0.5f));
        Time.timeScale = 0f;
        isPaused = true;
        SetPosition();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        StartCoroutine(FadeBackground(0f));
        Time.timeScale = 1;
        isPaused = false;
    }

    public void LeaveScenario()
    {
        //take in GSM and change state to frontend
        if (gameStateMachine != null)
        {
            GameStateMachine gsm = gameStateMachine.GetComponent<GameStateMachine>();
            if (gsm != null)
            {
                gsm.GoBackToFrontEndState();
            }            
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator FadeBackground(float targetAlpha)
    {
        float duration = 0.3f;
        float startAlpha = dimBackground.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            dimBackground.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
    public void SetVoiceVolume(float volume)
    {
        audioMixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 20);
    }
    public void SetSoundFXVolume(float volume)
    {
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(volume) * 20);
    }
}
