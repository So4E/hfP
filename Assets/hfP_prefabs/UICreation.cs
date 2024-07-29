using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;

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

    private void Update() //check if this works on hololens
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
            PositionNextTo(NoPanelSelectedErrorMessage, NewObjectWindow); //todo - why is panel not displayed next to new object window but behind it?
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
                // throw error - object not found
                break;
        }
    }

    private void PositionNextTo(GameObject _objectToRelocate, GameObject _objectToPositionItNextTo)
    {
        Vector3 _positionRightOfObject = _objectToPositionItNextTo.transform.position + new Vector3(0.3f, 0, 0);
        _objectToRelocate.transform.position = _positionRightOfObject;
    }

    private void OpenTextEnterWindow(string _description)
    {
        TextEnterWindow.SetActive(true);
        TMP_Text _descriptionText = GetChildTMPText(TextEnterWindow, "Description_Text");
        _descriptionText.SetText(_description); //might be better to exchange getComponent in case of performance issues
        PositionNextTo(TextEnterWindow, NewObjectWindow);
        //Set text to nothing
        TextEnterWindowText.SetText(""); //************************* todo - why is this not working
    }

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void OnClickConfirm() // _updateElement == true update element
    {
        string _textInput = TextEnterWindowText.text;
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
         if (String.IsNullOrEmpty(_title.Trim())) //todo - does not work -> check, otherwise panel has no header.. no name.. etc.
         {
             _title = "new panel";
         }
        //get coordinates where to spawn panel with offset to new object panel and text enter panel and instantiate it
        Vector3 _positionNextToTextEnterWindow = TextEnterWindow.transform.position + new Vector3(0.3f, 0, 0);
        GameObject _panel = Instantiate(UIPanel, _positionNextToTextEnterWindow, Quaternion.identity); //prefab, position, rotation
        _panel.transform.SetParent(UIPrototypingParent.transform, true); //position stays
        _panel.name = "Panel_" + _title;
        GameObject _xButton = GetChildGameObject(_panel, "x_Action Button");
        _xButton.GetComponent<PressableButton>().OnClicked.AddListener(() => OnClosePanel());

        //set Header to _panelName
        TMP_Text _header = GetChildTMPText(_panel, "Header");
        _header.SetText(_title);
        _currentUIPanel = _panel;
        _createdUIPanels.Add(_panel);
    }

    private void SpawnElement(string _inputText, GameObject _whatToSpawn)
    {
        GameObject _element = Instantiate(_whatToSpawn, GetChildGameObject(_currentUIPanel, "Canvas").transform);
        _element.GetComponent<PressableButton>().OnClicked.AddListener(() => OnEditUIElement(_element));
        
        if (String.IsNullOrEmpty(_inputText)) { _inputText = _whatToSpawn.name; }
        TMP_Text _textComponent = GetChildTMPText(_element, "TextText");
        _textComponent.SetText(_inputText);
        _element.name = _inputText;

        //note - the text object is a differently formatted button to make easy user editing possible on HoloLens -> must be handled differently later
        // if UI prototype is supposed to be used and further edited in UDE
    }

    private void SpawnTwoButtons()
    {
        GameObject _horizontalLayoutGroup = Instantiate(UITwoButtons, GetChildGameObject(_currentUIPanel, "Canvas").transform);
        // spawn two buttons with this parent
        GameObject _elementOne = Instantiate(UIButton, _horizontalLayoutGroup.transform);
        _elementOne.GetComponent<PressableButton>().OnClicked.AddListener(() => OnEditUIElement(_elementOne));        
        GameObject _elementTwo = Instantiate(UIButton, _horizontalLayoutGroup.transform);
        _elementTwo.GetComponent<PressableButton>().OnClicked.AddListener(() => OnEditUIElement(_elementTwo));

    }

    public void OnClosePanel() //todo - add to xButtonon Panel when panel is created
    {
        _currentUIPanel = null;
        EditUIElement.SetActive(false);
    }

    public void OnEditUIElement(GameObject _objectToBeEdited)
    {
        _gameObjectToEdit = _objectToBeEdited;
        EditUIElement.SetActive(true);
        PositionNextTo(EditUIElement, _currentUIPanel);
        GetChildTMPText(EditUIElement, "Header").SetText(_objectToBeEdited.name);
    }

    public void OnDelete()
    {
        Destroy(_gameObjectToEdit);
        EditUIElement.SetActive(false);
    }

    public void OnDuplicate()
    {
        GameObject _clone = Instantiate(_gameObjectToEdit, GetChildGameObject(_currentUIPanel, "Canvas").transform); //doesnt work for button in horizontal layout group.. 
        _clone.GetComponent<PressableButton>().OnClicked.AddListener(() => OnEditUIElement(_clone));
    }

    public void OnEdit()
    {
        _updateElement = true;
        OpenTextEnterWindow("please enter new text here");
        TextEnterWindowText.GetComponent<TMP_Text>().SetText(_gameObjectToEdit.name); //todo - why is this not working???? *********************

    }

    private void EditNameOfObject(string _newName)
    {
        TMP_Text _textComponent = GetChildTMPText(_gameObjectToEdit, "TextText");
        _textComponent.SetText(_newName);
        _gameObjectToEdit.name = _newName;
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
