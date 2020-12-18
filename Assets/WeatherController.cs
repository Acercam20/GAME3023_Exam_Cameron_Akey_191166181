using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class WeatherController : MonoBehaviour
{
    //Variable decleration
    [Header("Essential Variables")]
    [Tooltip("A list of all weather states in the form of weather prefabs")]
    public List<GameObject> weatherStates;
    [Tooltip("The current weather state prefab as a GameObject. Used to get the settings for lights, audio, etc.")]
    public GameObject currentWeatherState;
    [Tooltip("The current weather state prefab as a particle system.")]
    public ParticleSystem currentWeather;
    [Tooltip("The random delay between weather states is calculated by minWeatherDuration - maxWeatherDuration")]
    public float minWeatherDuration = 15; 
    [Tooltip("The random delay between weather states is calculated by minWeatherDuration - maxWeatherDuration")]
    public float maxWeatherDuration = 20; 
    [Tooltip("The index of the next weather to change into")]
    public int nextWeather = 0;
    private ParticleSystem newWeather;
    private float randomVariance;
    private float transitionTime = 1;
    [Header("Optional Variables")]
    [Tooltip("AudioSource component reference")]
    public AudioSource audioSource;
    [Tooltip("A reference to a GameObject containing a Light2D component.")]
    public GameObject globalLight;

    void Start()
    {
        //Time.timeScale = 2.0f; //For testing purposes
        if (GetComponent<AudioSource>() != null)
            audioSource = GetComponent<AudioSource>();
        if (globalLight != null)
            globalLight.GetComponent<Light2D>().color = Color.Lerp(globalLight.GetComponent<Light2D>().color, currentWeatherState.GetComponent<WeatherPrefab>().weatherColor, 1);
        
        //This is just to make sure the emission of the particle system is enabled at the start.
        currentWeather = currentWeatherState.GetComponent<ParticleSystem>();
        var particleEmission = currentWeather.GetComponent<ParticleSystem>().emission;
        particleEmission.enabled = true;

        //Begins the actual cycle of weather
        InvokeRepeating("WeatherTransition", 1, 30); 
    }

    void WeatherTransition()
    {
        StartCoroutine("RandomDelay");
    }

    IEnumerator RandomDelay() //Used to add a random delay between weather states using the min and max variables
    {
        randomVariance = Random.Range(0, maxWeatherDuration - minWeatherDuration);
        yield return new WaitForSeconds(randomVariance);
        StartCoroutine("WeatherFade", false);
        Debug.Log("RandomDelay");
    }

    IEnumerator WeatherFade(bool fadeIn)
    {
        Debug.Log("WeatherFade");
        var particleEmission = currentWeather.GetComponent<ParticleSystem>().emission;
        float newEmissionRate = 0;
        if (fadeIn) //Fades particles, light, and sound in
        {
            NextWeatherIndex(); 
            for (float ft = 0; ft <= transitionTime; ft += 0.1f)
            {
                Debug.Log(ft);
                if (globalLight != null)
                    globalLight.GetComponent<Light2D>().color = Color.Lerp(globalLight.GetComponent<Light2D>().color, currentWeatherState.GetComponent<WeatherPrefab>().weatherColor, ft);
                if (GetComponent<AudioSource>() != null)
                    audioSource.volume = ft;
                newEmissionRate += ft * transitionTime * 2;
                particleEmission.rateOverTime = newEmissionRate;
                particleEmission.enabled = true;
                yield return new WaitForSeconds(1);
            }
        }
        else //Fades out of a weather state. Calls the code to fade into a new one at the end
        {
            for (float ft = transitionTime; ft >= 0; ft -= 0.1f)
            {
                if (globalLight != null)
                    globalLight.GetComponent<Light2D>().color = Color.Lerp(globalLight.GetComponent<Light2D>().color, currentWeatherState.GetComponent<WeatherPrefab>().weatherColor, 1-ft);
                if (GetComponent<AudioSource>() != null)
                    audioSource.volume = ft;
                newEmissionRate = (ft / transitionTime) * particleEmission.rateOverTime.constant;
                particleEmission.rateOverTime = newEmissionRate;
                yield return new WaitForSeconds(1);
            }
            Debug.Log("Weather Index: " + nextWeather);
            currentWeather = weatherStates[nextWeather].GetComponent<ParticleSystem>(); //Random.Range(0, weatherStates.Count)
            currentWeatherState = weatherStates[nextWeather];
            if (GetComponent<AudioSource>() != null)
            {
                audioSource.clip = currentWeatherState.GetComponent<WeatherPrefab>().weatherSFX;
                audioSource.Play();
            }
            StartCoroutine("WeatherFade", true);
        }
    }

    void NextWeatherIndex() //Decides the next weather cycle. 
    {
        bool flip = true;
        int previousWeather = nextWeather;
        while (flip)
        {
            nextWeather = Random.Range(0, weatherStates.Count);
            if (!(nextWeather == previousWeather) || !(previousWeather == 1 && nextWeather == 3))
            {
                flip = false;
            }
        }
    }
}
