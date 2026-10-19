using System.Collections.Generic;
using DantesInferno.SinSystem;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
public sealed class WrathWorldFeedbackDirector : MonoBehaviour
{
    [Header("Visual Response")]
    [SerializeField] private Color wrathTint = new Color(0.72f, 0.12f, 0.08f, 1f);
    [SerializeField] [Range(0f, 1f)] private float maximumLightTint = 0.58f;
    [SerializeField] [Range(0f, 1f)] private float maximumCameraTint = 0.35f;
    [SerializeField] [Range(0f, 1f)] private float minimumLightIntensityScale = 0.72f;
    [SerializeField] private float responseSpeed = 2.5f;

    private readonly List<LightBinding> lights = new List<LightBinding>();
    private SinManager boundManager;
    private Camera sceneCamera;
    private Color baseCameraColor;
    private WhisperManager whisper;
    private float targetIntensity;
    private float currentIntensity;
    private WrathFeedbackLevel currentLevel;
    private bool initializedLevel;

    void Awake()
    {
        CacheSceneEnvironment();
    }

    void OnEnable()
    {
        BindManager();
    }

    void Start()
    {
        BindManager();
        ApplySnapshot(boundManager != null ? boundManager.CurrentSnapshot : null, false);
    }

    void OnDisable()
    {
        UnbindManager();
        RestoreEnvironment();
    }

    void Update()
    {
        if (boundManager != SinManager.Instance)
            BindManager();

        currentIntensity = Mathf.MoveTowards(
            currentIntensity,
            targetIntensity,
            Mathf.Max(0.01f, responseSpeed) * Time.deltaTime);
        ApplyVisuals(currentIntensity);
    }

    private void BindManager()
    {
        UnbindManager();
        boundManager = SinManager.Instance;
        if (boundManager == null)
            return;

        boundManager.OnSnapshotChanged += HandleSnapshotChanged;
        ApplySnapshot(boundManager.CurrentSnapshot, false);
    }

    private void UnbindManager()
    {
        if (boundManager != null)
            boundManager.OnSnapshotChanged -= HandleSnapshotChanged;

        boundManager = null;
    }

    private void HandleSnapshotChanged(SinStateSnapshot snapshot)
    {
        ApplySnapshot(snapshot, true);
    }

    private void ApplySnapshot(SinStateSnapshot snapshot, bool allowWhisper)
    {
        if (snapshot == null)
        {
            targetIntensity = 0f;
            return;
        }

        float wrathPressure = snapshot.GetAxis(SinType.Wrath).Pressure;
        targetIntensity = WrathFeedbackPolicy.GetVisualIntensity(
            wrathPressure,
            snapshot.CurrentDominantSin == SinType.Wrath);

        WrathFeedbackLevel nextLevel = WrathFeedbackPolicy.GetLevel(wrathPressure);
        if (!initializedLevel)
        {
            currentLevel = nextLevel;
            initializedLevel = true;
            return;
        }

        if (allowWhisper && nextLevel > currentLevel)
            TryPlayDeathHint(nextLevel);

        currentLevel = nextLevel;
    }

    private void CacheSceneEnvironment()
    {
        lights.Clear();
        Light2D[] sceneLights = FindObjectsByType<Light2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int index = 0; index < sceneLights.Length; index++)
        {
            Light2D light = sceneLights[index];
            if (light.lightType == Light2D.LightType.Global)
                lights.Add(new LightBinding(light));
        }

        sceneCamera = Camera.main;
        if (sceneCamera != null)
            baseCameraColor = sceneCamera.backgroundColor;

        whisper = FindAnyObjectByType<WhisperManager>();
    }

    private void ApplyVisuals(float intensity)
    {
        float lightBlend = intensity * maximumLightTint;
        float lightIntensityScale = Mathf.Lerp(1f, minimumLightIntensityScale, intensity);
        for (int index = 0; index < lights.Count; index++)
        {
            LightBinding binding = lights[index];
            if (binding.Light == null)
                continue;

            binding.Light.color = Color.Lerp(binding.BaseColor, wrathTint, lightBlend);
            binding.Light.intensity = binding.BaseIntensity * lightIntensityScale;
        }

        if (sceneCamera != null)
            sceneCamera.backgroundColor = Color.Lerp(baseCameraColor, wrathTint, intensity * maximumCameraTint);
    }

    private void RestoreEnvironment()
    {
        for (int index = 0; index < lights.Count; index++)
        {
            LightBinding binding = lights[index];
            if (binding.Light == null)
                continue;

            binding.Light.color = binding.BaseColor;
            binding.Light.intensity = binding.BaseIntensity;
        }

        if (sceneCamera != null)
            sceneCamera.backgroundColor = baseCameraColor;
    }

    private void TryPlayDeathHint(WrathFeedbackLevel level)
    {
        if (whisper == null || whisper.IsWhispering)
            return;

        switch (level)
        {
            case WrathFeedbackLevel.Stirring:
                whisper.TryWhisper("Your hand moves sooner than your judgment.");
                break;
            case WrathFeedbackLevel.Surging:
                whisper.TryWhisper("You call it strength. The river hears anger.");
                break;
            case WrathFeedbackLevel.Overwhelming:
                whisper.TryWhisper("Wrath no longer follows you. It leads.");
                break;
        }
    }

    private sealed class LightBinding
    {
        public Light2D Light { get; }
        public Color BaseColor { get; }
        public float BaseIntensity { get; }

        public LightBinding(Light2D light)
        {
            Light = light;
            BaseColor = light.color;
            BaseIntensity = light.intensity;
        }
    }
}
