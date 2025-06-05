using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{

    [SerializeField] private Text ClockText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void UpdateClock()
    {
        DateTime now = DateTime.Now;
        string formattedTime = now.ToString("hh:mm tt");
        ClockText.text = formattedTime;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClock();
    }
}
