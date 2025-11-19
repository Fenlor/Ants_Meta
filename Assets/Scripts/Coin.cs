using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;
using Oculus.Haptics;

public class Coin : MonoBehaviour
{
    public float maxRotSpeed = 180f;
    public float acceleration = 90f;
    public float holdTime = 1;

    public bool isSpinning = false;

    public ParticleSystem sparkleEffect;
    public AudioSource spinSound;

    private float currentSpeed = 0f;
    private float timer = 0f;

    public TextMeshProUGUI coinCountText;

    public UnityEvent coinAddEvent;

    public GameObject recordManagerObject;

    //public HapticSource leftHandHapticSource;

    //coinCount shouldnt be here, put on HUD element OR write straight to record manager
    //how about CounUICanavs object has a coinManager script that has access to recordmanager and
    //a public function that gets called at the end of move? public unity even

    private int coinCount = 0;

    public int coinValue = 1;

    public Transform coinCanvasUI;
    public float moveDuration = 1f;

    public HapticClip hapticClip;
    private HapticClipPlayer hapticClipPlayer;
    private enum SpinState
    {
        Accelerating,
        Holding,
        Decelerating,
        Effects,
        Moving
    }
    private SpinState state = SpinState.Accelerating;

    void OnAwake()
    {
        if (coinAddEvent == null)
        {
            coinAddEvent = new UnityEvent();
        }

        
    }

    void Start()
    {
        hapticClipPlayer = new HapticClipPlayer(hapticClip);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpinning)
        {
            switch (state)
            {
                case SpinState.Accelerating:
                    currentSpeed += acceleration * Time.deltaTime;
                    if(currentSpeed >= maxRotSpeed)
                    {
                        currentSpeed = maxRotSpeed;
                        state = SpinState.Holding;
                    }                   
                    
                    break;
                case SpinState.Holding:
                    timer += Time.deltaTime;
                    if(timer >= holdTime)
                    {
                        state = SpinState.Decelerating;
                    }

                    StartCoroutine(MoveToHand());

                    break;
                case SpinState.Decelerating:
                    currentSpeed -= acceleration * Time.deltaTime;
                    if (currentSpeed <= 0f)
                    {
                        currentSpeed = 0f;
                        state = SpinState.Effects;
                        timer = 0f;
                    }
                    break;
                case SpinState.Effects:
                    if (sparkleEffect != null)
                    {
                        sparkleEffect.Play();
                    }
                    if (spinSound != null)
                    {
                        spinSound.Play();
                    }

                    state = SpinState.Moving;
                    break;
                case SpinState.Moving:
                    StartCoroutine(MoveToHand());
                    break;
                default:
                    break;
            }
            transform.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
        }
    }

    public void StartSpining()
    {
        isSpinning = true;
    }
    public void StopSpinning()
    {
        isSpinning = false;

    }

    //this needs to move
    public void IncreaseCoinCount()
    {
        coinCount += coinValue;
        coinCountText.text = "= " + coinCount.ToString();
    }

    IEnumerator MoveToHand()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = coinCanvasUI.position;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed);    
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        //IncreaseCoinCount();
        if (recordManagerObject != null)
        {
            RecordManager recordManager = recordManagerObject.GetComponent<RecordManager>();
            if (recordManager != null)
            {
                recordManager.AddCoinValue(coinValue);

                //get coin value from RecordManager and update HUD?
                coinCountText.text = "= " + recordManager.GetActivityCoinValue();
            }            
        }

        sparkleEffect.transform.position = transform.position;

        if (sparkleEffect != null)
        {
            sparkleEffect.Play();
        }
        if (spinSound != null)
        {
            spinSound.Play();
        }        

        Destroy(gameObject);

        //if (leftHandHapticSource != null)
        //{
        //    leftHandHapticSource.Play();
        //}
        //

        hapticClipPlayer.Play(Controller.Left);
    }
}
