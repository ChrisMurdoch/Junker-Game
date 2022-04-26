using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LarryTriggerGameEnd : MonoBehaviour
{
    public GameObject youDidIt;

    void Start()
    {
        youDidIt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Cursor.visible = true;
            youDidIt.SetActive(true);
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
