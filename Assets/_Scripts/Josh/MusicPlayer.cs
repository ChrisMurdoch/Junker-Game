using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip Song;

    // Start is called before the first frame update
    void Start()
    {
        if(Song != null)
        {
            AudioManager.instance.PlayMusic(Song);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
