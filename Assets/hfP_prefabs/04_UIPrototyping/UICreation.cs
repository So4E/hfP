using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;

public class UICreation : MonoBehaviour
{

    [SerializeField] private GameObject NewObjectWindow;
    [SerializeField] private GameObject PanelOverview;
    [SerializeField] private GameObject PanelButtonParent;
    [SerializeField] private GameObject PanelButton;
    [SerializeField] private GameObject UIPrototypingParent;
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private GameObject UIButton;
    [SerializeField] private GameObject UITwoButtons;
    [SerializeField] private GameObject UIText;

    [SerializeField] private GameObject EditUIElement;
    [SerializeField] private GameObject EditUIPanel;
    [SerializeField] private GameObject TextEnterWindow;
    [SerializeField] private MRTKTMPInputField TextEnterWindowText;
    [SerializeField] private GameObject NoPanelSelectedErrorMessage;
    [SerializeField] private TouchScreenKeyboard keyboard;

    private GameObject _currentUIPanel;
    private GameObject _gameObjectToEdit;
    private List<GameObject> _createdUIPanels = new List<GameObject>();
    private string _whatToInitialize = "nothing";
    private bool _updateUIElement = false;
    private bool _updateUIPanel = false;

    private void Update() //todo -- no keyboard displayed on hololense... 
    {
        if (keyboard != null && TextEnterWindow.activeSelf == true) //&& TextEnterWindow is active
        {
            string _keyboardInput = keyboard.text;
            TextEnterWindowText.text = _keyboardInput;
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
            PositionNextTo(NoPanelSelectedErrorMessage, NewObjectWindow);
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

    private void OpenTextEnterWindow(string _description)
    {
        TextEnterWindow.SetActive(true);
        TMP_Text _descriptionText = GetChildTMPText(TextEnterWindow, "Description_Text");
        _descriptionText.SetText(_description);
        PositionNextTo(TextEnterWindow, NewObjectWindow);

        //Set text in inputField to nothing
        TextEnterWindowText.text = ""; 
    }

    public void OpenSystemKeyboard()
    {
        //keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void OnClickConfirm()
    {
        string _textInput = TextEnterWindowText.text;
        if (String.IsNullOrEmpty(_textInput)) { _textInput = ""; }
        if (_updateUIElement)
        {
            EditNameOfObject(_textInput);
            _updateUIElement = false;
            return;
        }
        if (_updateUIPanel)
        {
            if(String.IsNullOrEmpty(_textInput)) { _textInput = "updated panelName" + _currentUIPanel.GetInstanceID(); }
            _currentUIPanel.name = _textInput;
            TMP_Text _header = GetChildTMPText(_currentUIPanel, "Header");
            _header.SetText(_currentUIPanel.name);
            _updateUIPanel = false;
            EditNameOfObject(_textInput);
            return;
        }

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

        GameObject _panel = Instantiate(UIPanel, UIPrototypingParent.transform);
        PositionNextTo(_panel, TextEnterWindow);

        _panel.name = _title;
        GameObject _xButton = GetChildGameObject(_panel, "x_Action Button");
        _xButton.GetComponent<PressableButton>().OnClicked.AddListener(() => OnClosePanel());

        TMP_Text _header = GetChildTMPText(_panel, "Header");
        _header.SetText(_title);
        _currentUIPanel = _panel;
        _createdUIPanels.Add(_panel);

        //case new panel is created while panel overview is open
        if (PanelOverview.activeSelf == true)
        {
            ClearPanelOverviewList();
            CreatePanelList();
        }
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
        GameObject _elementOne = Instantiate(UIButton, _horizontalLayoutGroup.transform);
        _elementOne.GetComponent<PressableButton>().OnClicked.AddListener(() => OnEditUIElement(_elementOne));        
        GameObject _elementTwo = Instantiate(UIButton, _horizontalLayoutGroup.transform);
        _elementTwo.GetComponent<PressableButton>().OnClicked.AddListener(() => OnEditUIElement(_elementTwo));

    }

    public void OnClosePanel()
    {
        _currentUIPanel = null;
        EditUIElement.SetActive(false);
    }

    //*********** Edit UI Element in Panel
    public void OnEditUIElement(GameObject _objectToBeEdited)
    {
        _gameObjectToEdit = _objectToBeEdited;
        EditUIElement.SetActive(true);

        PositionNextTo(EditUIElement, _currentUIPanel);
        GetChildTMPText(EditUIElement, "Header").SetText(_objectToBeEdited.name);
    }

    public void OnDelete()
    {
        //check if parent of object to delete is called "TwoButtons_Horizontal" if this is the case and object is the only child, also delete parent
        GameObject _thisElementsParent = _gameObjectToEdit.transform.parent.gameObject;
        if (_thisElementsParent.name == "TwoButtons_Horizontal(Clone)")
        {
            int _howManyChildren = 0;
            foreach (Transform t in _thisElementsParent.transform)
            {
                _howManyChildren++;
            }
            if(_howManyChildren == 1)
            {
                Destroy(_thisElementsParent);
                EditUIElement.SetActive(false);
                return;
            }
        }
        Destroy(_gameObjectToEdit);
        EditUIElement.SetActive(false);
    }

    public void OnDuplicate()
    {
        switch (_gameObjectToEdit.tag)
        {
            case "Button":
                SpawnElement(_gameObjectToEdit.name, UIButton);
                break;
            case "Text":
                SpawnElement(_gameObjectToEdit.name, UIText);
                break;
        }
    }

    public void OnEdit()
    {
        _updateUIElement = true;
        OpenTextEnterWindow("please enter new text here");
        TextEnterWindowText.text =_gameObjectToEdit.name;
    }

    private void EditNameOfObject(string _newName)
    {
        TMP_Text _textComponent = GetChildTMPText(_gameObjectToEdit, "TextText");
        _textComponent.SetText(_newName);
        _gameObjectToEdit.name = _newName;
    }
    //*********** 

    //*********** List of Prototyped Panels and editing
    public void OpenListOfProtoPanels()
    {
        PanelOverview.SetActive(true);
        PositionNextTo(PanelOverview, NewObjectWindow);
        ClearPanelOverviewList();
        CreatePanelList();
    }

    private void ClearPanelOverviewList()
    {
        foreach(Transform t in PanelButtonParent.transform)
        {
            Destroy(t.gameObject);
        }
    }

    private void CreatePanelList()
    {
        for (int i = 0; i < _createdUIPanels.Count; i++)
        {
            //create one panel button for each name in list
            GameObject _panelButtonElement = Instantiate(PanelButton, PanelButtonParent.transform);
            _panelButtonElement.GetComponent<PressableButton>().OnClicked.AddListener(() => OnEditPanelInList(_panelButtonElement));

            TMP_Text _textComponent = GetChildTMPText(_panelButtonElement, "TextText");
            _textComponent.SetText(_createdUIPanels[i].name);
            _panelButtonElement.name = _textComponent.text;
        }
    }

    public void OnEditPanelInList(GameObject _panelButtonToEdit)
    {
        _gameObjectToEdit = _panelButtonToEdit;
        _currentUIPanel = FindPanelToEditInList(_panelButtonToEdit.name);
        if(_currentUIPanel == null) { 
            Debug.Log("ACHTUNG -- panel not found in list");
            return;
        }
        EditUIPanel.SetActive(true);
        PositionNextTo(EditUIPanel, PanelOverview);
        GetChildTMPText(EditUIPanel, "Header").SetText(_currentUIPanel.name);
    }

    private GameObject FindPanelToEditInList(string _nameOfPanel)
    {
        for (int i = 0; i < _createdUIPanels.Count; i++)
        {
            if (_createdUIPanels[i].name == _nameOfPanel)
            {
                return _createdUIPanels[i];
            }
        }
        return null;
    }

    public void OnDeletePanel()
    {
        _createdUIPanels.Remove(_currentUIPanel);
        Destroy(_currentUIPanel); //actual panel
        Destroy(_gameObjectToEdit); //panelButton in list
        EditUIPanel.SetActive(false);
    }

    public void OnRenamePanel()
    {
        _updateUIPanel = true;
        OpenTextEnterWindow("please enter new name for panel here");
        TextEnterWindowText.text = _gameObjectToEdit.name;
    }

    public void OnOpenPanel()
    {
        _currentUIPanel.SetActive(true);
        //this covers not set current panel in case: that you open panel from overview list,
        //then click on another panel in list, close list and want to edit the already opened panel - then _current panel is not set
        EditUIPanel.SetActive(false);
        PanelOverview.SetActive(false);
    }

    //utility

    private void PositionNextTo(GameObject _objectToRelocate, GameObject _objectToPositionItNextTo) 
    {
        _objectToRelocate.transform.rotation = _objectToPositionItNextTo.transform.rotation;
        _objectToRelocate.transform.position = _objectToPositionItNextTo.transform.position + _objectToPositionItNextTo.transform.right * .3f;
    }

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
