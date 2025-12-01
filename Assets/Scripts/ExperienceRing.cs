using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ExperienceRing : MonoBehaviour
{
    public Image circleImage;       // Reference to UI Image
    public float fillSpeed = 0.2f;  // Base speed
    public AudioSource levelUpSound;
    public AudioSource fillSound;
    public ParticleSystem levelUpEffect;    

    private float targetFill = 0f;
    private float currentFill = 0f;
    //private int currentXP = 0;
    public int pendingXP = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get logged in player coins and add to exp
        //SetStartingXP(75);
        //AddXP(22);
    }

    void Update()
    {
        if (currentFill < targetFill)
        {
            float dynamicSpeed = fillSpeed + (currentFill * 0.5f);
            currentFill = Mathf.MoveTowards(currentFill, targetFill, dynamicSpeed * Time.deltaTime);
            circleImage.fillAmount = currentFill / 100f;

            // Play sound while filling
            if (fillSound != null)
            {
                if (!fillSound.isPlaying)
                {
                    fillSound.Play();
                    
                }
                fillSound.pitch = 1f + (currentFill / 100f); // optional pitch increase
            }

            if (currentFill >= 100f)
            {
                LevelUp();
            }
            Debug.Log("ExpRing, currentFill: " + currentFill);
            Debug.Log("ExpRing, pendingXP: " + pendingXP);
            Debug.Log("ExpRing, targetFill: " + targetFill);            
        }
        else
        {
            // Stop sound when not filling
            if (fillSound != null)
            {
                if (fillSound.isPlaying)
                {
                    fillSound.Stop();
                }
            }

            // If we have leftover XP, animate it next
            if (pendingXP > 0)
            {
                int nextChunk = Mathf.Min(pendingXP, 100);
                targetFill = nextChunk;
                pendingXP -= nextChunk;
            }
            Debug.Log("ExpRing, pendingXP: " + pendingXP);
        }
    }
    public void SetStartingXP(int value)

    {
        // Clamp to valid range
        value = Mathf.Clamp(value, 0, 100);

        currentFill = value;
        targetFill = value;
        circleImage.fillAmount = currentFill / 100f;

        Debug.Log("ExpRing, SetStartingXP -  currentFill: " + currentFill);
        Debug.Log("ExpRing, SetStartingXP -  targetFill: " + targetFill);
    }

    public void AddXP(int amount)
    {
        pendingXP += amount;

        Debug.Log("ExpRing,Start AddXP -  amount: " + amount);
        Debug.Log("ExpRing,Start AddXP -  targetFill: " + targetFill);
        Debug.Log("ExpRing,Start AddXP -  currentFill: " + currentFill);
        Debug.Log("ExpRing,Start AddXP -  pendingXP: " + pendingXP);

        // If idle, start animating immediately
        if (targetFill == currentFill)
        {
            int nextChunk = Mathf.Min(pendingXP, 100);
            targetFill += nextChunk;
            pendingXP -= nextChunk;
        }

        Debug.Log("ExpRing,End AddXP -  targetFill: " + targetFill);
        Debug.Log("ExpRing,End AddXP -  currentFill: " + currentFill);
        Debug.Log("ExpRing,End AddXP -  pendingXP: " + pendingXP);
    }
    private void LevelUp()
    {
        currentFill = 0f;
        targetFill = 0f;
        circleImage.fillAmount = 0f;

        if (levelUpSound)
        {
            levelUpSound.Play();
        }

        if (levelUpEffect) 
        {
            levelUpEffect.Play();
        }

        // Haptics
        var leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        var rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        HapticCapabilities capabilities;
        if (leftHand.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
            leftHand.SendHapticImpulse(0u, 1f, 0.5f);

        if (rightHand.TryGetHapticCapabilities(out capabilities) && capabilities.supportsImpulse)
            rightHand.SendHapticImpulse(0u, 1f, 0.5f);

        Debug.Log("Level Up!");
    }
}
