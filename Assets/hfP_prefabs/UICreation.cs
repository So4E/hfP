using System.Collections;
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

    private GameObject _currentUIPanel;
    //List of UIPanels
    private List<GameObject> _createdUIPanels = new List<GameObject>(); 


    public void SpawnUIPanel()
    {
        //get user to name his panel
        string _panelName = getInputFromTextEnterWindow();

        //get coordinates where to spawn panel with offset to new object panel and instantiate it
        Vector3 _positionNextToNewObjectPanel = NewObjectWindow.transform.position + new Vector3(0.3f, 0, 0);
        GameObject _panel = Instantiate(UIPanel, _positionNextToNewObjectPanel, Quaternion.identity); //prefab, position, rotation
        _panel.transform.SetParent(UIPrototypingParent.transform, true); //position stays

        //set Header to _panelName
        GameObject _header = getChildGameObject(_panel, "Header");
        if(_header == null) { 
            Debug.Log("header not found");
            return;
        }
        _header.transform.GetComponent<TMP_Text>().SetText(_panelName);
        _currentUIPanel = _panel;
        _createdUIPanels.Add(_panel);
    }

    private string getInputFromTextEnterWindow()
    {
        //open text enter window
        //clear old text // put in placeholder (Button, Text, etc.)
        //get input and return it
        string _textInput = "NewPanel";
        return _textInput;
    }

    private GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }

}
