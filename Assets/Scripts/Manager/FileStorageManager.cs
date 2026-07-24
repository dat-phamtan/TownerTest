#nullable enable
using Assets.Scripts.Data;
using Assets.Scripts.IO;
using Assets.Scripts.Logger;
using Assets.Scripts.Unity;
using ControlTowner.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Assets.Scripts.Manager
{
    public class FileStorageManager : IStorageManager
    {
        private readonly string[] _header = { "MH", "VN", "SK", "FA", "OL" };
        private readonly Random _random = new();
        private DateTime _date;
        private readonly IStorage _storage;
        private const string SCHEDULE_FILE_NAME = "FlightSchedule.json";
        private const string DIARY_FILE_NAME = "FlightDiary.json";

        public FileStorageManager(IStorage storage)
        {
            _storage = storage;
        }

        public void GenerateDailySchedule(MaintenancePeriod maintenancePeriod, List<Flight>? delayedDailySchedule)
        {
            var mainStartHour = maintenancePeriod.MaintenanceStartHour;
            var mainEndHour = maintenancePeriod.MaintenanceEndHour;
            var mainStartMinute = maintenancePeriod.MaintenanceStartMinute;
            var currentHour = mainEndHour + 1;
            var listSchedules = new List<FlightSchedule>();
            _date = SimpleClock.Instance.SimulatedTime.Date;

            //handle the delayed schedules
            if (delayedDailySchedule != null)
                listSchedules.AddRange(HandleDelayedSchedule(delayedDailySchedule, mainStartHour, mainStartMinute, ref currentHour));

            //maintenance at the same day
            if (mainStartHour < mainEndHour)
            {
                //before midnight
                for (int i = mainEndHour + 1; i < 24; i++)
                    listSchedules.Add(HandleGenerateNewFlight(ref currentHour));

                //cross the midnight
                currentHour = 0;
                _date = _date.AddDays(1);

                //new day
                for (int i = 0; i < mainStartHour; i++)
                    listSchedules.Add(HandleGenerateNewFlight(ref currentHour));
            }
            else
            {
                while (currentHour < mainStartHour)
                    listSchedules.Add(HandleGenerateNewFlight(ref currentHour));
            }
            _storage.Save(SCHEDULE_FILE_NAME, listSchedules);
        }

        public List<FlightSchedule> LoadDailySchedule(DateTime today)
        {
            return _storage.Load<List<FlightSchedule>>(SCHEDULE_FILE_NAME);
        }

        public void SaveDailyLog(FlightDiary diary)
        {
            _storage.Append(DIARY_FILE_NAME, diary);
        }


        // helper func
        private List<FlightSchedule>? HandleDelayedSchedule(List<Flight> delayDailySchedule, int mainStartHour, int mainStartMinute, ref int startScheduleHour)
        {
            if (delayDailySchedule.Count == 0) return null;

            var listSchedules = new List<FlightSchedule>();
            foreach (var delayedLine in delayDailySchedule)
            {
                int randomMinute = _random.Next(0, 60);
                if (IsEndOfTheDay(startScheduleHour, randomMinute, mainStartHour, mainStartMinute)) break;

                DateTime newDateTime = _date.AddHours(startScheduleHour).AddMinutes(randomMinute);

                var newSchedule = new FlightSchedule(delayedLine.FlightSchedule.Code, newDateTime);
                listSchedules.Add(newSchedule);
                startScheduleHour++;

                if (startScheduleHour > 23)
                {
                    startScheduleHour = 0;
                    _date = _date.AddDays(1);
                }
            }
            return listSchedules;
        }

        private string GenerateFlightCode()
        {
            int headerIndex = _random.Next(_header.Length);
            return _header[headerIndex] + _random.Next(100, 999);
        }

        private bool IsEndOfTheDay(int hour, int minute, int mainStartHour, int mainStartMinute)
        {
            return hour > mainStartHour || (hour == mainStartHour && minute >= mainStartMinute);
        }

        private FlightSchedule HandleGenerateNewFlight(ref int startScheduleHour)
        {
            int headerIndex = _random.Next(_header.Length);
            string randomCode = GenerateFlightCode();
            int randomMinute = _random.Next(0, 60);
            DateTime generatedDateTime = _date.AddHours(startScheduleHour).AddMinutes(randomMinute);
            var newSchedule = new FlightSchedule(randomCode, generatedDateTime);
            startScheduleHour++;
            return newSchedule;
        }
    }
}
