using Assets.Scripts.Unity;
using UnityEngine;

public class FlightView : MonoBehaviour
{
    private Flight _flightData;

    public void InitData(Flight flightData)
    {
        _flightData = flightData;
    }

    private void Update()
    {
        if (_flightData != null) return;
        transform.Translate(Vector3.forward * 10f * Time.deltaTime);
    }
}
