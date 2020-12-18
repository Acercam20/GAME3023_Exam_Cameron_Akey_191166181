using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowPileBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject weatherController;
    public GameObject winterPrefab;
    private bool isSnowing = false;
    float currentScale = 0;
    void Start()
    {
        gameObject.transform.localScale = new Vector3(1, currentScale, 1);
        InvokeRepeating("ExpandSnow", 1, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
            if (weatherController.GetComponent<WeatherController>().currentWeatherState == winterPrefab)
            {
                isSnowing = true;
            }
            else
            {
                isSnowing = false;
            }
    }

    void ExpandSnow()
    {
        if(isSnowing)
        {
            gameObject.transform.localScale = new Vector3(1, currentScale, 1);
            if (currentScale < 1)
                currentScale += 0.25f;

        }
        else
        {
            gameObject.transform.localScale = new Vector3(1, currentScale, 1);
            if (currentScale > 0)
                currentScale -= 0.25f;
        }
    }
}
