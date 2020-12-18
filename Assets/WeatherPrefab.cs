using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherPrefab : MonoBehaviour
{
    [Header("Essential Variables")]
    [Tooltip("Reference to the audio to be played during the weather condition")]
    public AudioClip weatherSFX;
    [Tooltip("Reference to the color of the lights for the weather condition")]
    public Color weatherColor;
    void Start()
    {
        
    }
}
