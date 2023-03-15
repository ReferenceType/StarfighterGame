using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class QuickSettingsUI : MonoBehaviour
{
    public Slider QualitySlider;
    public Slider VolumeSlider;
    public Slider ResolutionSlider;
    // public Slider Brightness;
    public Toggle EngineFlame;

    public ParticleSystem PlayerJetFlame;

    private Canvas canvas;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        
        QualitySlider.value = 3;
        QualitySlider.onValueChanged.AddListener(QualitySliderChanged);

        VolumeSlider.onValueChanged.AddListener(VolumeSliderChanged);
        VolumeSlider.value = 50;

        ResolutionSlider.onValueChanged.AddListener(ResolutionSliderChanged);
        ResolutionSlider.value = 1;

        EngineFlame.isOn= true;
        EngineFlame.onValueChanged.AddListener(FlameToggleChanged);
    }

    private void FlameToggleChanged(bool arg0)
    {
        if(PlayerJetFlame!=null)
            PlayerJetFlame.gameObject.SetActive(arg0);
    }


    private void ResolutionSliderChanged(float arg0)
    {
        var pl = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        pl.renderScale = arg0;
    }

    private void VolumeSliderChanged(float arg0)
    {
        AudioListener.volume = arg0;
    }

    private void QualitySliderChanged(float arg0)
    {
        QualitySettings.SetQualityLevel((int)arg0, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}
