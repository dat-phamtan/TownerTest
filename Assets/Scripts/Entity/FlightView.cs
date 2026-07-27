using Assets.Scripts.Unity;
using TMPro;
using UnityEngine;

public class FlightView : MonoBehaviour
{
    public enum UIState { Waiting, Landing, Takeoff}
    public TextMeshPro flightCodeText;

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

    public void StartWaiting(Vector3 waitingPos, string flightCode)
    {
        _currentState = UIState.Waiting;
        _waitingPos = waitingPos;
        flightCodeText.text = flightCode;
    }

    public void StartLanding(float forwardSpeed)
    {
        _currentState = UIState.Landing;
        _forwardSpeed = forwardSpeed;
        transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    public void StartTakeoff(float forwardSpeed)
    {
        _currentState = UIState.Takeoff;
        _forwardSpeed = forwardSpeed;
        //transform.rotat
    }

    private void Update()
    {
        switch (_currentState)
        {
            case UIState.Waiting:
                if (_flightData.Type == FlightType.Landing)
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                else
                    transform.rotation = Quaternion.Euler(0, 0, 0);

                transform.position = _waitingPos;
                //transform.SetPositionAndRotation(_waitingPos, Quaternion.LookRotation(_runwayCenter));
                break;
            case UIState.Landing:
                //transform.rotation = Quaternion.LookRotation(_runwayCenter);
                transform.Translate(Vector3.up * _forwardSpeed * Time.deltaTime);
                break;
            case UIState.Takeoff:
                transform.Translate(Vector3.up * _forwardSpeed * Time.deltaTime);
                break;
        }
    }
}
