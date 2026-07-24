using Assets.Scripts;
using Assets.Scripts.Config;
using Assets.Scripts.Controller;
using Assets.Scripts.FlightPool;
using Assets.Scripts.Generator;
using Assets.Scripts.IO;
using Assets.Scripts.Manager;
using Assets.Scripts.Utility;
using ControlTowner.Utility;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Loading : MonoBehaviour
{
    public TextMeshProUGUI progress;
    public InputActionReference continueAction;
    public FlightView flightPrefab;
    public Object runwayPrefab;
    private GameObject container;
    private FlightPool flightPool;

    public float runwayWidth = 1.8f;
    public float runwayHeight = 11.5f;
    private readonly int _screenWidth = Screen.width;
    private readonly int _screenHeight = Screen.height;
    public float visualLoadDelta = 0.5f;
    private int _runwayCount = 8;
    private int numFlightPool = 20;


    private void Awake()
    {
        var controller = Locator.Get<IFlightController>();
        controller.Init();
        var config = Locator.Get<IConfig>();

        _runwayCount = config.Get().RunwayCount;
        FlightPoolInit();

    }

    private void FlightPoolInit()
    {
        container = new GameObject("Container");
        DontDestroyOnLoad(container);
        flightPool = new FlightPool(flightPrefab, container.transform);
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

    void Update()
    {
        
    }

    IEnumerator LoadScene()
    {
        yield return null;
        float actualprogress = 0f;
        float displayedProgress = 0f;

        int maxNumRunway = (int)(_screenWidth / (200 * runwayWidth));
        float x, scale;
        x = -(_screenWidth / 400) + runwayWidth/2 - 1f;

        // instantiate runway
        for (int i = 0; i < _runwayCount; i++)
        {
            if (_runwayCount <= maxNumRunway)
                scale = 1;
            else
                scale = _screenWidth / (200 * _runwayCount * runwayWidth);

            var spawnPos = new Vector3((float)(x + i * runwayWidth * scale), 0, 0);
            var rotation = Quaternion.Euler(0, 0, -90);
            var runway = Object.Instantiate(runwayPrefab, spawnPos, rotation, container.transform);
            runway.GameObject().transform.localScale = new Vector3(scale, scale, scale);

            runway.GameObject().SetActive(false);

            actualprogress = (float)(0.15 * (i + 1) / _runwayCount);
            yield return null;
        }

        // instantiate plane pool
        int poolSize = 20;
        for (int i = 0; i < poolSize; i++)
        {
            flightPool.PoolInit(poolSize);
            actualprogress += (float)(0.15 * (i + 1)/poolSize);
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
            {
                progress.text = (int)(displayedProgress * 100) + "%";
            }
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

    //private List<float> GenerateRunwayXPos()
    //{
    //    int maxNumRunway = (int)(_screenWidth / (200 * runwayWidth));
    //    float x, scale;
    //    x = -(_screenWidth / 400) + runwayWidth / 2 - 1f;
    //}

    //private void InstantiateRunway(int index, int x, int runwayCount, int maxNumRunway, int screenWidth, int runwayWidth, GameObject container)
    //{
    //    float scale;
    //    if (runwayCount <= maxNumRunway)
    //        scale = 1;
    //    else
    //        scale = _screenWidth / (200 * runwayCount * runwayWidth);

    //    var spawnPos = new Vector3((float)(x + i * runwayWidth * scale), 0, 0);
    //    var rotation = Quaternion.Euler(0, 0, -90);
    //    var runway = Object.Instantiate(runwayPrefab, spawnPos, rotation, container.transform);
    //    runway.GameObject().transform.localScale = new Vector3(scale, scale, scale);

    //    runway.GameObject().SetActive(false);
    //}
}
