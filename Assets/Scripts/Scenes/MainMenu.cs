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
using System.Collections;
using System.Data.SqlTypes;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button _startButton;
    public GameObject loadingSprite;

    private void Awake()
    {
        var sceenData = new ScreenData(Screen.width, Screen.height);
        Locator.Register<ScreenData>(sceenData);

        var eventLogger = new EventLogger();
        Locator.Register<Assets.Scripts.Logger.ILogger>(eventLogger);
        Locator.Register<Assets.Scripts.Logger.ILogSource>(eventLogger);

        IStorage storage = new LocalStorage();
        //Locator.Register(storage);

        ILandingGenerator generator = new RandomLandingGenerator();
        //Locator.Register(generator);

        IConfig config = new SimulationConfig(storage);
        Locator.Register(config);

        IStorageManager storageManager = new FileStorageManager(storage);
        //Locator.Register(storageManager);

        IRunwayManager runwayManager = new RunwayManager();
        Locator.Register(runwayManager);

        IFlightController flightController = new FlightController(config, generator, storageManager, runwayManager, eventLogger, 5, 30);
        Locator.Register(flightController);

    }

    private void Start()
    {
        _startButton.onClick.AddListener(HandleStartButton);
    }

    private void Update()
    {
        
    }

    private void HandleStartButton()
    {
        StartCoroutine(LoadScene());
    }


    IEnumerator LoadScene()
    {
        loadingSprite.SetActive(true);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Loading");
        while (!asyncOperation.isDone)
        {
            loadingSprite.transform.Rotate(500f * Time.deltaTime * Vector3.back);
            yield return null;
        }
    }
}
