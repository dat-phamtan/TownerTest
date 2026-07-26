using Assets.Scripts.FlightPool;
using Assets.Scripts.Scenes;
using ControlTowner.Utility;
using System;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

public class UIManager : MonoBehaviour, IUIManager
{
    public TextMeshProUGUI time;
    public TextMeshProUGUI date;
    public TextMeshProUGUI status;
    public TextMeshProUGUI landingQueueNum;
    public TextMeshProUGUI takeoffQueueNum;
    private IFlightPool _flightPool;
    //public GameObject runway;

    public void ChangeLandingQueue(string num)
    {
        landingQueueNum.text = num;
    }

    public void ChangeStatus(string statusStr)
    {
        status.text = statusStr;
    }

    public void ChangeTakeoffQueue(string num)
    {
        takeoffQueueNum.text = num;
    }

    public void ActiveRunways()
    {
        GameObject container = GameObject.Find("Container");
        if (container == null) return;

        foreach (Transform child in container.transform)
        {
            if (child.name.Contains("Runway"))
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SimpleClock.Instance.UpdateClock(Time.deltaTime);
        var clockTime = SimpleClock.Instance.SimulatedTime;
        time.text = clockTime.TimeOfDay.ToString(@"hh\:mm\:ss");
        date.text = clockTime.Date.ToString("dd/MM/yyyy");
    }
}
