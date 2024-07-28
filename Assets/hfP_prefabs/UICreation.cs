using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICreation : MonoBehaviour
{

    [SerializeField] private GameObject NewObjectWindow;
    [SerializeField] private GameObject UIPrototypingParent;
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private GameObject UIButton;
    [SerializeField] private GameObject UITwoButtons;
    [SerializeField] private GameObject UIText;

    [SerializeField] private GameObject EditUIElement;
    [SerializeField] private GameObject TextEnterWindow;
    [SerializeField] private TMP_Text TextEnterWindowText;
    [SerializeField] private GameObject NoPanelSelectedErrorMessage;
    [SerializeField] private TouchScreenKeyboard keyboard;

    private GameObject _currentUIPanel;
    private GameObject _gameObjectToEdit;
    private List<GameObject> _createdUIPanels = new List<GameObject>();
    private string _whatToInitialize = "nothing";
    private bool _updateElement = false;

    private void Update()
    {
        if (keyboard != null && TextEnterWindow.activeSelf == true) //&& TextEnterWindow is active
        {
            string _keyboardInput = keyboard.text;
            TextEnterWindowText.SetText(_keyboardInput);
        }
    }

    public void CreateUIElement(string element)
    {
        _whatToInitialize = element;
        if (element == "panel")
        {
            OpenTextEnterWindow("Please enter a name for your Panel here");
            return;
        }
        if (element != "panel" && _currentUIPanel == null)
        {
            NoPanelSelectedErrorMessage.SetActive(true);
            SetPositionNextToNewObjectWindow(NoPanelSelectedErrorMessage); //todo - why is panel not displayed next to new object window but behind it?
            return;
        }
        switch (element)
        {          
            case "text":
                OpenTextEnterWindow("Please enter your text for the text field here");
                break;            
            case "button":
                OpenTextEnterWindow("Please enter your label text for the button here");
                break;            
            case "twoButtons":
                SpawnTwoButtons();
                break;            
            default:
                //what to do as default?
                break;
        }
    }

    private void SetPositionNextToNewObjectWindow(GameObject _objectToRelocate)
    {
        Vector3 _positionNextToNewObjectPanel = NewObjectWindow.transform.position + new Vector3(0.3f, 0, 0);
        _objectToRelocate.transform.position = _positionNextToNewObjectPanel;
    }

    private void OpenTextEnterWindow(string _description)
    {
        TextEnterWindow.SetActive(true);
        TMP_Text _descriptionText = GetChildTMPText(TextEnterWindow, "Description_Text");
        _descriptionText.SetText(_description); //might be better to exchange getComponent in case of performance issues
        SetPositionNextToNewObjectWindow(TextEnterWindow);
    }

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void OnClickConfirm() // _updateElement == true update element
    {
        string _textInput = TextEnterWindowText.text;
        TextEnterWindowText.SetText(""); //todo - why is this not working????
        if (_updateElement)
        {
            EditNameOfObject(_textInput);
            _updateElement = false;
            return;
        }
        //spawn
        switch (_whatToInitialize)
        {
            case "panel":
                SpawnUIPanel(_textInput);
                break;
            case "text":
                SpawnElement(_textInput, UIText);
                break;
            case "button":
                SpawnElement(_textInput, UIButton);
                break;
            default:
                //todo - what to do as default?
                break;
        }
    }

    private void SpawnUIPanel(string _title)
    {
         if (String.IsNullOrEmpty(_title))
         {
             _title = "new panel";
         }
        //get coordinates where to spawn panel with offset to new object panel and text enter panel and instantiate it
        Vector3 _positionNextToNewObjectPanel = NewObjectWindow.transform.position + new Vector3(0.6f, 0, 0);
        GameObject _panel = Instantiate(UIPanel, _positionNextToNewObjectPanel, Quaternion.identity); //prefab, position, rotation
        _panel.transform.SetParent(UIPrototypingParent.transform, true); //position stays

        //set Header to _panelName
        TMP_Text _header = GetChildTMPText(_panel, "Header");
        _header.SetText(_title);
        _currentUIPanel = _panel;
        _createdUIPanels.Add(_panel);
    }

    private void SpawnElement(string _inputText, GameObject _whatToSpawn)
    {
        GameObject _textField = Instantiate(_whatToSpawn, GetChildGameObject(_currentUIPanel, "Canvas").transform); 
        if (!String.IsNullOrEmpty(_inputText))
        {
            TMP_Text _textComponent = GetChildTMPText(_textField, "TextText");
            _textComponent.SetText(_inputText);
        }
        //note - the text object is a differently formatted button to make easy user editing possible on HoloLens -> must be handled differently later
        // if UI prototype is supposed to be used and further edited in UDE
    }

    private void SpawnTwoButtons()
    {
        Instantiate(UITwoButtons, GetChildGameObject(_currentUIPanel, "Canvas").transform);
    }

    public void OnEditUIElement(GameObject _objectToBeEdited)
    {
        _gameObjectToEdit = _objectToBeEdited;
        EditUIElement.SetActive(true);
        //change position of EditUIElement to next to element
    }

    public void OnDelete()
    {
        Destroy(_gameObjectToEdit);
        EditUIElement.SetActive(false);
    }

    public void OnDuplicate()
    {
        Instantiate(_gameObjectToEdit); //check if this really does the trick or if any parent e.g. has to be set
    }

    public void OnEdit()
    {
        _updateElement = true;
        OpenTextEnterWindow("please enter new text here");
    }

    private void EditNameOfObject(string _newName)
    {
        // how to find TMP_Text object to set to new name?
        // _gameObjectToEdit. text = auf _newName setzen
    }

    

    //*********** further to do for UI prototyping
    //where to open list of created ui panels, how to open them again and set current panel to that ui
    //make onClick method for created ui panel to re-edit the object created (text, delete, duplicate)

    //utility
    private GameObject GetChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13. link: https://discussions.unity.com/t/how-to-find-a-child-gameobject-by-name/31255/2
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }
    
    
    private TMP_Text GetChildTMPText(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject.GetComponent<TMP_Text>();
        return null;
    }



}
