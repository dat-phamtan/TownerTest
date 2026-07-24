using Assets.Scripts.Controller;
using Assets.Scripts.Manager;
using Assets.Scripts.Scenes;
using Assets.Scripts.Utility;
using UnityEngine;

public class UISetup : MonoBehaviour
{
    public UIManager uiManager;
    private DisplayManager _displayManager;
    private void Start()
    {
        var iLogSource = Locator.Get<Assets.Scripts.Logger.ILogSource>();
        var controller = Locator.Get<IFlightController>();
        _displayManager = new DisplayManager(controller, uiManager, iLogSource);
    }
}
