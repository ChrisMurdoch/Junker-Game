using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject SettingsPanel;


    // Start is called before the first frame update
    void Start()
    {
        if(SettingsPanel != null)
        {
            SettingsPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleSettingsPanel()
    {
        SettingsPanel.SetActive(!SettingsPanel.activeSelf);
    }
}
