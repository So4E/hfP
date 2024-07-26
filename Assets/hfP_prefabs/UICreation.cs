using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICreation : MonoBehaviour
{

    [SerializeField] private GameObject NewObjectWindow;
    [SerializeField] private GameObject UIPrototypingParent;
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private GameObject TextEnterWindow;

    private GameObject _currentUIPanel;
    //List of UIPanels
    private List<GameObject> _createdUIPanels = new List<GameObject>(); 


    public void SpawnUIPanel()
    {
        string _panelName = getInputFromTextEnterWindow();
        //get coordinates where to spawn panel-> with offset to new object panel
        Vector3 _positionNextToNewObjectPanel = NewObjectWindow.transform.position + new Vector3(0, 3, 0);
        GameObject _panel = Instantiate(UIPanel, _positionNextToNewObjectPanel, Quaternion.identity);
        //set Header to _panelName
        _panel.transform.FindChild("Header").GetComponent<TMP_Text>().text = _panelName;
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

}
