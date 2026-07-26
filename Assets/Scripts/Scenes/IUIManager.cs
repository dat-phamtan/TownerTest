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
    }
}
