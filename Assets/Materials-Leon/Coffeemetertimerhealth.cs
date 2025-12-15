using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Coffeemetertimerhealth : MonoBehaviour
{
    public float coffe_meter_health;
    public float coffee_meter_drain_speed = 3.5f;
    public float score;
    public bool InitiatePause = false;

    public TextMeshProUGUI coffee_meter_text;
    public Slider coffee_meter_slider;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI isPaused;

    public Canvas hudCanvas;
    public Canvas pausedCanvas;
    public Canvas gameOverCanvas;
    
    [Header("Handle Shake")]
    public bool enableHandleShake = true;
    public float shakeThreshold = 85f; // start shaking when coffee <= this value
    public float shakeAmplitude = 9f; // pixels
    public float shakeFrequency = 5f; // speed
    public float rotationAmplitude = 8f; // degrees of tilt when shaking
    public float verticalShakeMultiplier = 0.3f;
    public float horizontalShakeMultiplier = 0.3f;
    public float severeShakeThreshold = 35f; // below this, increase shake
    public float severeShakeMultiplier = 3f; // multiply amplitude (doubling by default)

    private RectTransform handleRect;
    private Vector3 handleOriginalLocalPos;
    private bool handleOriginalSet = false;
    private Vector3 lastHandleOffset = Vector3.zero;
    private Quaternion lastRotationOffset = Quaternion.identity;


    void Start()
    {
        // Ensure pause is false by default at runtime
        InitiatePause = false;
     

        if (isPaused != null)
            isPaused.text = "";
        coffe_meter_health = 100f;
        score = 0f;

      
        if (gameOverCanvas != null)
            gameOverCanvas.enabled = false;

        if (coffee_meter_text != null)
            coffee_meter_text.text = "" + Mathf.RoundToInt(coffe_meter_health).ToString();

        if (scoreText != null)
            scoreText.text = "Score: " + Mathf.RoundToInt(score).ToString();



        if (coffee_meter_slider != null)
        {
            coffee_meter_slider.maxValue = 100f;
            coffee_meter_slider.value = coffe_meter_health;
            // Cache handle rect and original position for shaking effect
            if (coffee_meter_slider.handleRect != null)
            {
                handleRect = coffee_meter_slider.handleRect;
                handleOriginalLocalPos = handleRect.localPosition;
                handleOriginalSet = true;
            }
        }
    }

    void Update()
    {
        // Toggle pause when Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InitiatePause = !InitiatePause;
            
            // Toggle canvas visibility based on pause state
            if (hudCanvas != null)
                hudCanvas.enabled = !InitiatePause;
            if (pausedCanvas != null)
                pausedCanvas.enabled = InitiatePause;
        }

        // Update pause UI
        if (isPaused != null)
            isPaused.text = InitiatePause ? "Game Paused" : "";

        // If paused, skip the rest of the update cycle
        if (InitiatePause) return;

        // Decrease the coffee meter by configured drain speed (units per second)
        coffe_meter_health -= coffee_meter_drain_speed * Time.deltaTime;

        if (scoreText != null)
            scoreText.text = "Score: " + Mathf.RoundToInt(score).ToString();

        // Clamp to zero so it doesn't go negative
        if (coffe_meter_health < 0f) coffe_meter_health = 0f;

        // Update UI text
        if (coffee_meter_text != null)
            coffee_meter_text.text = "" + Mathf.RoundToInt(coffe_meter_health).ToString();

        // Update slider value if assigned
        if (coffee_meter_slider != null)
            coffee_meter_slider.value = coffe_meter_health;

        // (Pause UI already handled above)

        // Handle shake when coffee is low — apply a per-frame offset so the slider's own positioning still works
        if (enableHandleShake && handleOriginalSet && handleRect != null)
        {
            // Remove previous frame's offset to get the slider's true handle position
                    // Remove previous frame's positional offset to get the slider's true handle position
                    if (lastHandleOffset != Vector3.zero)
                    {
                        handleRect.localPosition -= lastHandleOffset;
                        lastHandleOffset = Vector3.zero;
                    }
                    // Remove previous frame's rotational offset so base rotation is preserved
                    if (lastRotationOffset != Quaternion.identity)
                    {
                        handleRect.localRotation *= Quaternion.Inverse(lastRotationOffset);
                        lastRotationOffset = Quaternion.identity;
                    }

            if (coffe_meter_health > 0f && coffe_meter_health <= shakeThreshold && !InitiatePause)
            {
                float t = Time.time;
                float amplitude = shakeAmplitude;
                if (coffe_meter_health <= severeShakeThreshold)
                    amplitude *= severeShakeMultiplier;

                // Make vertical (Y) shake stronger than horizontal (X)
                float y = Mathf.Sin(t * shakeFrequency) * amplitude * verticalShakeMultiplier;
                float x = (Mathf.PerlinNoise(t * shakeFrequency, 0f) - 0.5f) * amplitude * horizontalShakeMultiplier;

                // basePos reflects slider-determined position for this frame
                Vector3 basePos = handleRect.localPosition;
                float currentZ = basePos.z;
                basePos.z = currentZ;

                Vector3 newOffset = new Vector3(x, y, 0f);
                handleRect.localPosition = basePos + newOffset;
                lastHandleOffset = newOffset;

                    // Apply a small rotational tilt around Z and track it so we can remove next frame
                    float angle = Mathf.Sin(t * shakeFrequency) * rotationAmplitude;
                    Quaternion rot = Quaternion.Euler(0f, 0f, angle);
                    handleRect.localRotation *= rot;
                    lastRotationOffset = rot;
            }
            else
            {
                // Ensure any previous offset is cleared
                if (lastHandleOffset != Vector3.zero)
                {
                    handleRect.localPosition -= lastHandleOffset;
                    lastHandleOffset = Vector3.zero;
                }
                    if (lastRotationOffset != Quaternion.identity)
                    {
                        handleRect.localRotation *= Quaternion.Inverse(lastRotationOffset);
                        lastRotationOffset = Quaternion.identity;
                    }
            }
        }
    }
}
