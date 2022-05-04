using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingsManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropDown;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    public Volume volume;

    public static SettingsManager Instance;

    private string resolution = "Resolution";

    public float ma;
    public float sfx;
    public float mu;
    private void Awake()
    {
        Instance = this;

        SaveManager.InitialSave(volume);

        volume = SaveManager.Load();

        mu = volume.Music;
        ma = volume.Master;
        sfx = volume.SFX;


        if (PlayerPrefs.GetInt(resolution) == 0)
        {
            Screen.SetResolution(1920, 1080, true);
            resolutionDropDown.value = 0;
        }
        else if (PlayerPrefs.GetInt(resolution) == 1)
        {
            Screen.SetResolution(1600, 900, true);
            resolutionDropDown.value = 1;

        }
        else if (PlayerPrefs.GetInt(resolution) == 2)
        {
            Screen.SetResolution(1280, 720, true);
            resolutionDropDown.value = 2;
        }

    }

    private void Start()
    {
        Debug.Log(mu);

        musicSlider.value = mu;
        sfxSlider.value = sfx;
        masterSlider.value = ma;

        setMusicVolume(mu);
        setSFXVolume(sfx);
        setMasterVolume(ma);
    }

    public void SaveSettings()
    {
        SaveManager.Save(volume);
    }

    public void setMasterVolume(float value)
    {
        AudioManager.instance.setMasterVolume(value);
        volume.Master = value;
        //SaveManager.Save(volume);

    }

    public void setSFXVolume(float value)
    {
        AudioManager.instance.setSFXVolume(value);
        volume.SFX = value;
        //SaveManager.Save(volume);


    }

    public void setMusicVolume(float value)
    {
        AudioManager.instance.setMusicVolume(value);
        volume.Music = value;
        //SaveManager.Save(volume);

    }

    public void ChangeResolution()
    {
        if (resolutionDropDown.value == 0)
        {
            Screen.SetResolution(1920, 1080, true);
            PlayerPrefs.SetInt(resolution, 0);
        }
        else if (resolutionDropDown.value == 1)
        {
            Screen.SetResolution(1600, 900, true);
            PlayerPrefs.SetInt(resolution, 1);

        }
        else if (resolutionDropDown.value == 2)
        {
            Screen.SetResolution(1280, 720, true);
            PlayerPrefs.SetInt(resolution, 2);

        }
    }
}
