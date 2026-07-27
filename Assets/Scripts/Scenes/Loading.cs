using Assets.Scripts;
using Assets.Scripts.Config;
using Assets.Scripts.Controller;
using Assets.Scripts.FlightPool;
using Assets.Scripts.Generator;
using Assets.Scripts.IO;
using Assets.Scripts.Manager;
using Assets.Scripts.Utility;
using ControlTowner.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Loading : MonoBehaviour
{
    public TextMeshProUGUI progress;
    public InputActionReference continueAction;
    public FlightView flightPrefab;
    public UnityEngine.Object runwayPrefab;
    private GameObject _container;
    private IFlightPool _flightPool;

    public float runwayWidth = 1.72f;
    public float runwayHeight = 11.42f;
    public float planeWidth = 4.88f;
    public float planeHeight = 5.11f;

    private readonly int _screenWidth = Screen.width;
    private readonly int _screenHeight = Screen.height;
    public float visualLoadDelta = 0.9f;
    private int _runwayCount = 8;
    private int _numFlightPool = 20;
    private float _scale;
    private List<float> _runwayXPos;


    private void Awake()
    {
        var controller = Locator.Get<IFlightController>();
        controller.LoadData();
        var config = Locator.Get<IConfig>();

        _runwayCount = config.Get().RunwayCount;
        _scale = GetScale();
        _runwayXPos = GenerateRunwayXPos();
        Locator.Register(_runwayXPos);
        RunwayInit(controller);
        FlightPoolInit();
    }

    private void OnEnable()
    {
        if (continueAction != null)
            continueAction.action.Enable();
    }

    private void OnDisable()
    {
        if (continueAction != null)
            continueAction.action.Disable();
    }

    void Start()
    {
        StartCoroutine(LoadScene());
    }

    private void RunwayInit(IFlightController controller)
    {
        for (int i = 0; i < _runwayCount; i++)
        {
            Vector3 centerPos = new(_runwayXPos[i], 0f, 0f);
            controller.GetRunways()[i].SetPosition(centerPos);
            controller.GetRunways()[i].SetRunwayLong(_scale * runwayHeight);
        }
    }

    private void FlightPoolInit()
    {
        _container = new GameObject("Container");
        DontDestroyOnLoad(_container);
        _flightPool = new FlightPool(flightPrefab, _container.transform);
        Locator.Register<IFlightPool>(_flightPool);
    }

    IEnumerator LoadScene()
    {
        yield return null;
        float actualprogress = 0f;
        float displayedProgress = 0f;
      
        // instantiate runway
        for (int i = 0; i < _runwayCount; i++)
        {
            InstantiateRunways(i, InitXPos(), _scale);
            actualprogress = (float)(0.15 * (i + 1) / _runwayCount);
            yield return null;
        }

        // instantiate plane pool
        _flightPool.PoolInit(_numFlightPool, runwayWidth * _scale/planeWidth);
        for (int i = 0; i < _numFlightPool; i++)
        {
            actualprogress += (float)(0.15 * (i + 1)/ _numFlightPool);
            if (i % 5 == 0) yield return null;
        }

        // instantiate scene
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("GamePlay");
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            float currentTarget = (float)(0.3 + 0.7 * (asyncOperation.progress / 0.9));
            displayedProgress = Mathf.MoveTowards(displayedProgress, currentTarget, visualLoadDelta * Time.deltaTime);

            if (displayedProgress < 1)
                progress.text = (int)(displayedProgress * 100) + "%";
            else
            {
                progress.text = "Presss the space bar to continue";
                if (continueAction != null && continueAction.action.WasPressedThisFrame())
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }

    private List<float> GenerateRunwayXPos()
    {
        float x = InitXPos();
        List<float> runwaysXPos = new();
        if (_runwayCount <= 0) return runwaysXPos;

        runwaysXPos.Add(x);

        for (int i = 1; i < _runwayCount; i++)
        {
            runwaysXPos.Add(x + i * runwayWidth * _scale);
        }
        return runwaysXPos;
    }

    private void InstantiateRunways(int index, float x, float scale)
    {
        var spawnPos = new Vector3((float)(x + index * runwayWidth * scale), 0, 0);
        var rotation = Quaternion.Euler(0, 0, -90);
        var runway = UnityEngine.Object.Instantiate(runwayPrefab, spawnPos, rotation, _container.transform);
        runway.GameObject().transform.localScale = new Vector3(scale, scale, scale);
        runway.GameObject().SetActive(false);
    }

    private float GetScale()
    {
        int maxNumRunway = (int)Math.Floor(_screenWidth / (200f * runwayWidth));
        //Debug.Log(maxNumRunway);
        float result = 1.0f;

        if (_runwayCount > maxNumRunway)
            result = (float)_screenWidth / (200f * _runwayCount * runwayWidth);
        return result;
    }

    private float InitXPos()
    {
        return -(float)(_screenWidth / 400f) + runwayWidth / 2f;
    }
}
