using Assets.Scripts;
using Assets.Scripts.Config;
using Assets.Scripts.Controller;
using Assets.Scripts.Data;
using Assets.Scripts.Generator;
using Assets.Scripts.IO;
using Assets.Scripts.Logger;
using Assets.Scripts.Manager;
using Assets.Scripts.Utility;
using System;
using System.Data.SqlTypes;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button _startButton;
    private ScreenData _screenData;
    private EventLogger _eventLogger;

    private void Awake()
    {
        _eventLogger = new EventLogger();
        Locator.Register<Assets.Scripts.Logger.ILogger>(_eventLogger);
        Locator.Register<Assets.Scripts.Logger.ILogSource>(_eventLogger);

        _screenData = new ScreenData(Screen.width, Screen.height);

        IStorage storage = new LocalStorage();
        Locator.Register(storage);

        ILandingGenerator generator = new RandomLandingGenerator();
        Locator.Register(generator);

        IConfig config = new SimulationConfig(storage);
        Locator.Register(config);

        IStorageManager storageManager = new FileStorageManager(storage);
        Locator.Register(storageManager);

        IRunwayManager runwayManager = new RunwayManager();
        Locator.Register(runwayManager);

        IFlightController flightController = new FlightController(config, generator, storageManager, runwayManager, _eventLogger, _screenData, 5, 30);
        Locator.Register(flightController);

    }

    private void Start()
    {
        _startButton.onClick.AddListener(HandleStartButton);
    }

    private void HandleStartButton()
    {
        SceneManager.LoadScene("Loading");
    }
}
