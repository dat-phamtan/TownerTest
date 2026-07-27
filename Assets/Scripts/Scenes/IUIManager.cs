using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Scenes
{
    public interface IUIManager
    {
        public void ChangeStatus(string statusStr);
        public void ChangeLandingQueue(string num);
        public void ChangeTakeoffQueue(string num);
        public void ActiveRunways();
        public void AppendLog(string message);
        public void RenderSchedule(List<FlightSchedule> schedule);
        public void ShowDiary(List<FlightDiary> diary, float intervalSeconds);
    }
}
