using UnityEngine;
using Tobii.Research.Unity;

public class AttentionCompass : MonoBehaviour
{
    private Transform stimuli;
    private SpriteRenderer stimuliSprite;
    public Color stimuliColor = new Color(1.0f, 1.0f, 1.0f);
    public Vector3 stimuliScale = new Vector3(0.5f, 0.5f, 0.0f);
    public float stimuliCenterOffset = 1.0f;

    public GameObject target;
    public Vector3 targetToCameraVector;
    public float targetToCameraDistance;
    public bool targetInFront;
    public bool targetInView;

    public float alphaMin = 0.25f;
    public float alphaMax = 0.75f;
    public float alphaNearFade = 0.2f;
    public float alphaFarFade = 0.8f;

    public bool pulseOscillator = true;
    public float pulseFrequency = 10;
    public float pulseDutyCycle = 0.5f;
    public float oscillatorFrequency = 45;

    // Start is called before the first frame update
    void Start()
    {
        // vSync needs to be disabled (Edit -> Projekt Settings... -> Quality -> V Sync Count: Don't Sync)
        Application.targetFrameRate = 90;
        stimuli = transform;
        stimuli.localScale = stimuliScale;
        stimuliSprite = GetComponent<SpriteRenderer>();
        stimuliSprite.color = stimuliColor;
    }

    // Update is called once per frame
    void Update()
    {
        // Translate target and gaze world positions to viewport vectors
        targetToCameraVector = Camera.main.WorldToViewportPoint(target.transform.position);
        //gazeToCameraVector = Camera.main.WorldToViewportPoint(vrEyeTracker.LatestGazeData.CombinedGazeRayWorld.direction);

        // Adjust target and gaze vectors zero to center of viewport
        targetToCameraVector.x -= 0.5f;
        targetToCameraVector.y -= 0.5f;

        // Calculate target parameters
        targetToCameraDistance = Mathf.Sqrt(Mathf.Pow(targetToCameraVector.x, 2) + Mathf.Pow(targetToCameraVector.y, 2)) * 2;
        targetInFront = targetToCameraVector.z >= 0.0f;
        targetInView = targetToCameraDistance <= 1.0f;

        // If target is behind camera, flip x- and y-axis
        if (!targetInFront)
        {
            targetToCameraVector.x *= -1;
            targetToCameraVector.y *= -1;
        }

        // Set stimuli alpha and/or flicker
        stimuliColor.a = (targetInFront && targetInView) ? Mathf.Clamp(((alphaMax - alphaMin) * Mathf.Clamp01((targetToCameraDistance - alphaNearFade) / (alphaFarFade - alphaNearFade))) + alphaMin, alphaMin, alphaMax) : alphaMax;
        stimuliSprite.color = pulseOscillator ? stimuliColor * pulseOscillate(pulseFrequency, pulseDutyCycle, oscillatorFrequency) : stimuliColor;

        // Calculate stumli position
        float angle = Mathf.Atan2(targetToCameraVector.x, targetToCameraVector.y);
        targetToCameraVector.x = stimuliCenterOffset / 2 * Mathf.Sin(angle) + 0.5f;
        targetToCameraVector.y = stimuliCenterOffset / 2 * Mathf.Cos(angle) + 0.5f;
        targetToCameraVector.z = 0.5f;

        // Update stumli position and scale
        stimuli.LookAt(Camera.main.transform.position, Camera.main.transform.forward);
        stimuli.position = Camera.main.ViewportToWorldPoint(targetToCameraVector);
        stimuli.localScale = stimuliScale;
    }

    // Pulse oscillator
    float pulseOscillate(float pulseFrequency, float pulseDutyCycle, float oscillatorFrequency)
    {
        float pulse = Time.time * pulseFrequency;
        float oscillator = Time.time * oscillatorFrequency;
        pulse -= Mathf.Floor(pulse);
        oscillator -= Mathf.Floor(oscillator);
        return (pulse < pulseDutyCycle && oscillator < 0.5f) ? 1.0f : 0.0f;
    }
}