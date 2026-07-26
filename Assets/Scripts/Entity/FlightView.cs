using Assets.Scripts.Unity;
using UnityEngine;

public class FlightView : MonoBehaviour
{
    public enum UIState { Waiting, Landing, Takeoff}
    private UIState _currentState;

    private float _forwardSpeed;
    private Vector3 _runwayCenter;
    private Vector3 _waitingPos;
    private Flight _flightData;

    public void InitData(Flight flightData, Vector3 runwayCenter, float forwardSpeed)
    {
        _flightData = flightData;
        _forwardSpeed = forwardSpeed;
        _runwayCenter = runwayCenter;
    }

    public void StartWaiting(Vector3 waitingPos)
    {
        _currentState = UIState.Waiting;
        _waitingPos = waitingPos;

    }

    public void StartLanding(float forwardSpeed)
    {
        _currentState = UIState.Landing;
        _forwardSpeed = forwardSpeed;
    }

    public void StartTakeoff(float forwardSpeed)
    {
        _currentState = UIState.Takeoff;
        _forwardSpeed = forwardSpeed;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case UIState.Waiting:
                transform.SetPositionAndRotation(_waitingPos, Quaternion.LookRotation(_runwayCenter));
                break;
            case UIState.Landing:
                transform.Translate(Vector3.forward * _forwardSpeed * Time.deltaTime);
                break;
            case UIState.Takeoff:
                transform.Translate(Vector3.forward * _forwardSpeed * Time.deltaTime);
                break;
        }
    }
}
