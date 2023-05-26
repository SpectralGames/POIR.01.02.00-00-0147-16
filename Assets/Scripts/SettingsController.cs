using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsController : MonoBehaviour {

    public Material selectedMaterial;
    public Material normalMaterial;
    public List<Button3dBody> qualityButtonsList = new List<Button3dBody>();


    // video Quality settings
    public Text videoQualityResolutionInfo;
    public Text videoQualityBloomInfo;
    public Text videoQualityAmbientInfo;

    // audio settings
    public AudioMixer audioMixer;
    public Text musicVolumeText;
    public Text sfxVolumeText;
    public Slider3d musicVolumeSlider;
    public Slider3d sfxVolumeSlider;

    private void Awake()
    {
        // set quality from save data
        //OnChangeVideoQuality(SaveGameController.instance.GetVideoSettings());

        // set default value to easy mode
        PlayerPrefs.SetInt(GameModeWriter.Prefs_ID, (int)(GameMode.Easy));

    }

    private void Start()
    {
        musicVolumeSlider.SetValue(SaveGameController.instance.GetMusicVolumeSettings());
        UpdateMusicVolume(musicVolumeSlider.GetValue());

        sfxVolumeSlider.SetValue(SaveGameController.instance.GetSFXVolumeSettings());
        UpdateSfxVolume(sfxVolumeSlider.GetValue());
    }

    
    /*
    public void OnChangeVideoQuality(int quality) // 0 - low, 1 - medium, 2 - high
    {
        switch(quality)
        {
            case 0:
               bloom.active = false;
               ambientOcclusion.active = false;
                XRSettings.eyeTextureResolutionScale = .75f;
                videoQualityResolutionInfo.text = "low";
                break;
            case 1:
               bloom.active = false;
               ambientOcclusion.active = true;
                XRSettings.eyeTextureResolutionScale = 0.82f;
                videoQualityResolutionInfo.text = "medium";
                break;
            case 2:
              bloom.active = true;
              ambientOcclusion.active = true;
                XRSettings.eyeTextureResolutionScale = 1f;
                videoQualityResolutionInfo.text = "high";
                break;
        }

        videoQualityBloomInfo.text = bloom.active ? "yes" : "no";
        videoQualityAmbientInfo.text = ambientOcclusion.active ? "yes" : "no";

        SaveGameController.instance.SetVideoSettings(quality);
        SaveGameController.instance.SaveGameData();
        SetSelectedButton(quality);
    }
    */
    
    private void SetSelectedButton(int selected)
    {
        for(int i = 0; i < qualityButtonsList.Count;i++)
        {
            if (i == selected)
            {
                qualityButtonsList[i].GetComponent<MeshRenderer>().material = selectedMaterial;
            }
            else
            {
                qualityButtonsList[i].GetComponent<MeshRenderer>().material = normalMaterial;
            }
        } 
    }

    public void OnSliderSoundVolume(float value)
    {
        UpdateMusicVolume(value);
    }

    private void UpdateMusicVolume(float value)
    {
        audioMixer.SetFloat("musicVol", (value * 100) - 80);
        musicVolumeText.text = ((int)(value * 100)).ToString() + "%";
    }

    public void OnSliderSfxVolume(float value)
    {
        UpdateSfxVolume(value);
    }

    private void UpdateSfxVolume(float value)
    {
        audioMixer.SetFloat("sfxVol", (value * 100) - 80);
        sfxVolumeText.text = ((int)(value * 100)).ToString() + "%";
    }

    public void OnSaveSoundSettings()
    {
        SaveGameController.instance.SetMusicVolumeSettings(musicVolumeSlider.GetValue());
        SaveGameController.instance.SetSFXVolumeSettings(sfxVolumeSlider.GetValue());
        SaveGameController.instance.SaveGameData();
    }
}

