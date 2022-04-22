using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LarryTriggerLevelLoad : MonoBehaviour
{
    public LevelLoader levelLoader;


    void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            levelLoader.LoadNextLevel();
        }
    }
}
