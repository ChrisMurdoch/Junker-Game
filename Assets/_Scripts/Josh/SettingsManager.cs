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

    private string resolution = "Resolution";
    private void Awake()
    {
        //SaveManager.Save(volume);
        volume = SaveManager.Load();

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

        masterSlider.value = volume.Master;
        sfxSlider.value = volume.SFX;
        musicSlider.value = volume.Music;

        AudioManager.instance.setMasterVolume(volume.Master);
        AudioManager.instance.setSFXVolume(volume.SFX);
        AudioManager.instance.setMusicVolume(volume.Music);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
