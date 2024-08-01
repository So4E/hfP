using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject ExitOptions;
    [SerializeField] private GameObject HelpMenu;
    [SerializeField] private GameObject ProjectSettings;
    [SerializeField] private GameObject UserAndNetworkSettings;
    [SerializeField] private GameObject NewObject;
    [SerializeField] private GameObject FileBrowser;
    [SerializeField] private GameObject Hierarchy;
    [SerializeField] private GameObject Tools;
    [SerializeField] private GameObject ErrorFeatureNotAvailable;
    [SerializeField] private GameObject ErrorAccessDenied;


    public void PlaceObjectRightNextToMainMenu(GameObject _objectToRelocate)
    {
        _objectToRelocate.SetActive(true);
        _objectToRelocate.transform.position = MainMenu.transform.position + MainMenu.transform.right * .3f;
        _objectToRelocate.transform.rotation = MainMenu.transform.rotation;
    }
}
