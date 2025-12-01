using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;
using System.Data;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuAnchor;
    public GameObject pauseMenuUI;
    public Transform centerEyeTransform;
    public Image dimBackground;
    private bool isPaused = false;
    public GameObject gameStateMachine;    
    public float distance = 2.5f;
    public Vector3 offset = new Vector3(0f, 0f, 0f);
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider voiceSlider;
    public Slider soundFXSlider;
    public float fadeDuration = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenuAnchor.SetActive(false);
        //pauseMenuAnchor.transform.SetParent(centerEyeTransform, false);
        //SetPosition();
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
        Vector3 forwardPos = centerEyeTransform.position + centerEyeTransform.forward * distance;
        ////Vector3 finalPos = forwardPos + centerEyeTransform.TransformVector(offset);
        pauseMenuAnchor.transform.position = forwardPos; //finalPos;
        ////pauseMenuAnchor.transform.rotation = Quaternion.LookRotation(pauseMenuAnchor.transform.position - centerEye.transform.position);
        pauseMenuAnchor.transform.rotation = Quaternion.LookRotation(centerEyeTransform.forward, Vector3.up);

        //pauseMenuAnchor.transform.SetParent(centerEyeTransform);
        //pauseMenuAnchor.transform.localPosition = offset;
        //pauseMenuAnchor.transform.localRotation = Quaternion.identity;

        pauseMenuUI.transform.localPosition = offset;
        pauseMenuUI.transform.rotation = Quaternion.identity;
    }

    public void Pause()
    {
        pauseMenuAnchor.SetActive(true);
        StartCoroutine(FadeBackground(0.5f));
        Time.timeScale = 0f;
        isPaused = true;
        //SetPosition();
    }

    public void Resume()
    {
        pauseMenuAnchor.SetActive(false);
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
        //FadeToVolume("SoundFXVolume", volume);
        Debug.Log("SetMusicVolume, volume: " + volume);
    }
    public void SetVoiceVolume(float volume)
    {
        audioMixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 20);
        //FadeToVolume("SoundFXVolume", volume);
        Debug.Log("SetVoiceVolume, volume: " + volume);
    }
    public void SetSoundFXVolume(float volume)
    {
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(volume) * 20);
        //FadeToVolume("SoundFXVolume", volume);
        Debug.Log("SetSoundFXVolume, volume: " + volume);
    }

    public void FadeToVolume(string param, float targetVolume)
    {
        StartCoroutine(FadeVolumeRoutine(param, targetVolume));
    }

    IEnumerator FadeVolumeRoutine(string param, float targetVolume)
    {
        audioMixer.GetFloat(param, out float currentVolume);
        float startTime = Time.time;

        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            float newVolume = Mathf.Lerp(currentVolume, Mathf.Log10(targetVolume) * 20f, t);
            audioMixer.SetFloat(param, newVolume);
            yield return null;
        }
        audioMixer.SetFloat(param, Mathf.Log10(targetVolume) * 20f);
    }
}
