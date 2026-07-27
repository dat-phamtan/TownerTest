using Assets.Scripts.Data;
using Assets.Scripts.FlightPool;
using Assets.Scripts.Scenes;
using ControlTowner.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class UIManager : MonoBehaviour, IUIManager
{
    public TextMeshProUGUI time;
    public TextMeshProUGUI date;
    public TextMeshProUGUI status;
    public TextMeshProUGUI landingQueueNum;
    public TextMeshProUGUI takeoffQueueNum;
    public TextMeshProUGUI scheduleText;
    public TextMeshProUGUI diaryText;
    public TextMeshProUGUI logText;

    private const int MAX_LOG_LINES = 50;
    private readonly Queue<string> _logLines = new();
    private Coroutine _diaryRoutine;

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

    public void AppendLog(string message)
    {
        _logLines.Enqueue(message);
        if (_logLines.Count > MAX_LOG_LINES)
            _logLines.Dequeue();
        logText.text = string.Join("\n", _logLines);
    }

    public void RenderSchedule(List<FlightSchedule> schedule)
    {
        if (schedule == null || schedule.Count == 0)
        {
            scheduleText.text = "No schedule for today";
            return;
        }
        var sb = new StringBuilder();
        foreach (var flightSchedule in schedule)
            sb.AppendLine($"{flightSchedule.ScheduleTime:HH:mm} - {flightSchedule.Code}");
        scheduleText.text = sb.ToString();
    }

    public void ShowDiary(List<FlightDiary> diary, float intervalSeconds)
    {
        if (_diaryRoutine != null)
            StopCoroutine(_diaryRoutine);
        _diaryRoutine = StartCoroutine(ShowDiaryRoutine(diary, intervalSeconds));
    }

    private IEnumerator ShowDiaryRoutine(List<FlightDiary> diary, float intervalSeconds)
    {
        diaryText.text = "";
        if (diary == null || diary.Count == 0)
        {
            diaryText.text = "No diary for now!";
            yield break;
        }
        foreach (var flightDiary in diary)
        {
            string action = flightDiary.IsLanding == 'L' ? "Landing" : "Takeoff";
            diaryText.text += $"{flightDiary.DiaryTime:HH:mm:ss} - {flightDiary.Code} - Runway {flightDiary.RunwayIndex} - {action}\n";
            yield return new WaitForSeconds(intervalSeconds);
        }
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
