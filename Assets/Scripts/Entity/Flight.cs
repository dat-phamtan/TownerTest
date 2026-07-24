using Assets.Scripts.Data;
using Assets.Scripts.Entity;
using Assets.Scripts.Logger;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Unity
{
    public enum FlightType { Landing, Takeoff}
    public enum FlightState { Waiting, Operating, Completed}

    public class Flight
    {
        public FlightSchedule FlightSchedule { get; set; }
        public FlightType Type { get; set; }
        public FlightState State { get; set; }
        private readonly ILogger _logger;

        public Action<Runway, Flight> OnActionCompleted;
        public Func<Flight, Task> OnRequestConfirmation;

        public Flight(FlightSchedule flightSchedule, FlightType type, FlightState state, ILogger logger)
        {
            FlightSchedule = flightSchedule;
            Type = type;
            State = state;
            _logger = logger;
        }

        public async Task ExecuteActionAsync(Runway runway, float duration)
        {
            //State = FlightState.Operating;
            string action = (Type == FlightType.Landing) ? "Landing" : "Takeoff";
            _logger.Log($"[{FlightSchedule.Code}] {action} in runway: {runway.Id}");

            await OnRequestConfirmation.Invoke(this);
            string logAction = (Type == FlightType.Landing) ? "landing" : "taking off";
            _logger.Log($"[{FlightSchedule.Code}] Confirmed!! Start {logAction}.");

            await Task.Delay((int)duration * 1000);

            State = FlightState.Completed;
            _logger.Log($"[{FlightSchedule.Code}] Completed {logAction} - Runway: {runway.Id}");
            OnActionCompleted?.Invoke(runway, this);
        }
    }
}
