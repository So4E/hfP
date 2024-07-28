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
    [SerializeField] private GameObject UIText;

    [SerializeField] private GameObject TextEnterWindow;
    [SerializeField] private TMP_Text TextEnterWindowText;
    [SerializeField] private GameObject NoPanelSelectedErrorMessage;
    [SerializeField] private TouchScreenKeyboard keyboard;

    private GameObject _currentUIPanel;
    private List<GameObject> _createdUIPanels = new List<GameObject>();
    private string _whatToInitialize = "nothing";

    private void Update()
    {
        if (keyboard != null && TextEnterWindow.activeSelf == true) //&& TextEnterWindow is active
        {
            string _keyboardInput = keyboard.text;
            TextEnterWindowText.GetComponent<TMP_Text>().SetText(_keyboardInput);
        }
    }

    public void CreateUIElement(string element)
    {
        _whatToInitialize = element;
        if (element == "panel")
        {
            openTextEnterWindow("Please enter a name for your Panel here");
            return;
        }
        if (_currentUIPanel == null)
        {
            NoPanelSelectedErrorMessage.SetActive(true);
            SetPositionNextToNewObjectWindow(NoPanelSelectedErrorMessage);
            return;
        }
        switch (element)
        {          
            case "text":
                openTextEnterWindow("Please enter your text for the text field here");
                break;            
            case "button":
                openTextEnterWindow("Please enter your label text for the button here");
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

    private void openTextEnterWindow(string _description)
    {
        TextEnterWindow.SetActive(true);
        GameObject _descriptionText = getChildGameObject(TextEnterWindow, "Description_Text");
        _descriptionText.GetComponent<TMP_Text>().SetText(_description); //might be better to exchange getComponent in case of performance issues
        SetPositionNextToNewObjectWindow(TextEnterWindow);
    }

    public void OpenSystemKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    public void OnClickConfirm()
    {
        if (keyboard != null)
        {
            string _keyboardText = keyboard.text;

            switch (_whatToInitialize)
            {
                case "panel":
                    SpawnUIPanel(_keyboardText);
                    break;
                case "text":
                    SpawnText(_keyboardText);
                    break;
                case "button":
                    SpawnButton(_keyboardText);
                    break;
                default:
                    //todo - what to do as default?
                    break;
            }
        }
    }

    private void SpawnUIPanel(string _title)
    {
         if (String.IsNullOrEmpty(_title))
         {
             _title = "new panel";
         }
        //get coordinates where to spawn panel with offset to new object panel and instantiate it
        Vector3 _positionNextToNewObjectPanel = NewObjectWindow.transform.position + new Vector3(0.3f, 0, 0);
        GameObject _panel = Instantiate(UIPanel, _positionNextToNewObjectPanel, Quaternion.identity); //prefab, position, rotation
        _panel.transform.SetParent(UIPrototypingParent.transform, true); //position stays

        //set Header to _panelName
        GameObject _header = getChildGameObject(_panel, "Header");
        if (_header == null)
        {
            Debug.Log("header not found");
            return;
        }
        _header.transform.GetComponent<TMP_Text>().SetText(_title);
        _currentUIPanel = _panel;
        _createdUIPanels.Add(_panel);
    }

    private void SpawnText(string _inputText)
    {
        GameObject _textField = Instantiate(UIText, _currentUIPanel.transform);
        if (!String.IsNullOrEmpty(_inputText))
        {
            GameObject _textComponent = getChildGameObject(_textField, "TextText");
            _textComponent.transform.GetComponent<TMP_Text>().SetText(_inputText);
        }
        //note - this text object is a differently formatted button to make easy user editing possible on HoloLens -> must be handled differently later
        // if UI prototype is supposed to be used and further edited in UDE
    }

    private void SpawnButton(string _inputText)
    {
        GameObject _button = Instantiate(UIButton, _currentUIPanel.transform);
        if (!String.IsNullOrEmpty(_inputText))
        {
            GameObject _textComponent = getChildGameObject(_button, "ButtonText");
            _textComponent.transform.GetComponent<TMP_Text>().SetText(_inputText);
        }
    }

    //*********** further to do for UI prototyping
    //where to open list of created ui panels, how to open them again and set current panel to that ui
    //make onClick method for created ui panel to reedit the object created (text, delete, duplicate)

    //utility
    private GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13. link: https://discussions.unity.com/t/how-to-find-a-child-gameobject-by-name/31255/2
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

}
