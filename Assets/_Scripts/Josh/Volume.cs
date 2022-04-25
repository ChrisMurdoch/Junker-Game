using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Volume
{
    [Range (0,1)]
    public float Master;
    [Range(0,1)]
    public float SFX;
    [Range(0,1)]
    public float Music;
}

