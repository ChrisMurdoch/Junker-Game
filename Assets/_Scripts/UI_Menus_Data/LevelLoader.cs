using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    private GameObject CrossFadeObject;

    private void Awake()
    {
        CrossFadeObject = transition.gameObject;
        CrossFadeObject.SetActive(true);
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {

        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        Time.timeScale = 0;
        SceneManager.LoadScene(levelIndex);
        Time.timeScale = 1;
    }
}
