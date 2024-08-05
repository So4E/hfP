using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;


public class SpeechAnnotations : MonoBehaviour
{
    [SerializeField] private GameObject ToolsWindow;
    [SerializeField] private GameObject SelectionMenu;
    [SerializeField] private GameObject AnnotationNameWindow;
    [SerializeField] private TMP_Text AnnotationNameWindowText;
    [SerializeField] private GameObject AnnotationOverview;
    [SerializeField] private GameObject AnnotationButtonParent;
    [SerializeField] private GameObject AnnotationButton;
    [SerializeField] private GameObject AnnotationControlWindow;
    [SerializeField] private TMP_Text AnnotationControlWindowObjectText;
    [SerializeField] private GameObject AnnotationObject;

    [SerializeField] private TouchScreenKeyboard keyboard;

    private Microphone _microphone;
    private GameObject _currentAnnotation;
    private GameObject _currentObjectToBeAnnotated;
    private List<GameObject> _createdAnnotations = new List<GameObject>();
    private bool _comingFromAnnotationOverview = false;
    private bool _comingFromObject = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (string device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
            //todo save microphone as microphone to use for recording?
        }
    }

    private void Update() //check if this works on hololens
    {
        if (keyboard != null && AnnotationNameWindow.activeSelf == true) //&& TextEnterWindow is active
        {
            string _keyboardInput = keyboard.text;
            AnnotationNameWindowText.SetText(_keyboardInput);
        }
    }


    //** All Annotations List - Called via Tools
    public void OpenAllAnnotationsList()
    {
        AnnotationOverview.SetActive(true);
        PositionNextTo(AnnotationOverview, ToolsWindow);
        ClearAnnotationOverviewList();
        CreateAnnotationList();
    }

    private void ClearAnnotationOverviewList()
    {
        foreach (Transform t in AnnotationButtonParent.transform)
        {
            Destroy(t.gameObject);
        }
    }

    private void CreateAnnotationList()
    {
        for (int i = 0; i < _createdAnnotations.Count; i++)
        {
            //create one annotation button for each name in list
            GameObject _annotationButtonElement = Instantiate(AnnotationButton, AnnotationButtonParent.transform);
            _annotationButtonElement.GetComponent<PressableButton>().OnClicked.AddListener(() => OnOpenAnnotationFromList(_annotationButtonElement));

            TMP_Text _annotationName = GetChildTMPText(_annotationButtonElement, "TextText");
            _annotationName.SetText(_createdAnnotations[i].name);
            _annotationButtonElement.name = _annotationName.text;

            TMP_Text _annotatedObject = GetChildTMPText(_annotationButtonElement, "ObjectText");
            _annotatedObject.SetText(_createdAnnotations[i].transform.parent.gameObject.name);
            _annotationButtonElement.name = _annotatedObject.text;
        }
    }

    private void OnOpenAnnotationFromList(GameObject _annotationButton)
    {
        AnnotationControlWindow.SetActive(true);
        _comingFromAnnotationOverview = true;
        TMP_Text _annotationName = GetChildTMPText(_annotationButton, "TextText");
        _currentAnnotation = FindAnnotationOnList(_annotationName.text);
        if (_currentAnnotation == null) { 
            Debug.Log("object " + _annotationName + " not found.");
            return;
        }
        AnnotationControlWindowObjectText.SetText(" * " + _currentAnnotation.name + " * ");
        _currentObjectToBeAnnotated = _currentAnnotation.transform.parent.gameObject;
    }

    private GameObject FindAnnotationOnList(string _annotationName)
    {
        for (int i = 0; i < _createdAnnotations.Count; i++)
        {
            if (_createdAnnotations[i].name == _annotationName)
            {
                return _createdAnnotations[i].gameObject;
            }
        }
        return null;
    }

    //********************** 

    //** Create Annotation - Called via Annotation Button when Object Is Selected
    public void CreateAnnotation(GameObject _objectToBeAnnotated)
    {
        PositionNextTo(AnnotationNameWindow, _objectToBeAnnotated);
        AnnotationNameWindow.SetActive(true);
        SelectionMenu.SetActive(false);
        _currentObjectToBeAnnotated = _objectToBeAnnotated;
        _currentAnnotation = Instantiate(AnnotationObject, _currentObjectToBeAnnotated.transform);
        _createdAnnotations.Add(_currentAnnotation);
        //now waiting for user to input name and confirm
    }

    public void OnExitNameOfAnnotation()
    {
        bool _deletedFromList = DeleteAnnotationFromList(_currentAnnotation);
        if (!_deletedFromList) { Debug.Log("Error: Did not find current annotation on list to delete it. Please debug."); }
        Destroy(_currentAnnotation);
        SelectionMenu.SetActive(true);
    }

    private bool DeleteAnnotationFromList(GameObject _nameOfObjectToFind)
    {
        for (int i = 0; i < _createdAnnotations.Count; i++)
        {
            if (_createdAnnotations[i] == _nameOfObjectToFind)
            {
                return true;
            }
        }
        return false;
    }


    public void OnConfirmNameOfAnnotation()
    {
        string _textInput = AnnotationNameWindowText.text;
        //if input is empty or name is already taken, reset it automatically:
        if (string.IsNullOrEmpty(_textInput) && FindAnnotationOnList(_textInput) != null) { 
            _textInput = "new Annotation_" + _currentAnnotation.GetInstanceID(); 
        }
        _currentAnnotation.name = _textInput;
        PositionNextTo(AnnotationControlWindow, _currentObjectToBeAnnotated);
        AnnotationControlWindow.SetActive(true);
        AnnotationControlWindowObjectText.SetText(" * " + _currentAnnotation.name + " * ");
        _comingFromObject = true;
    }

    //********************** 

    //** Annotation Options - Called via AnnotationControl Window ----------------------------------hier weitermachen """"""""""""""""""""""

    public void StartRecordingAnnotation()
    {
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = Microphone.Start("Built-in Microphone", true, 10, 44100); //public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency); 

        //audio clip iwo abspeichern in ordnerstruktur
        //TODO ------------------ create object in scene to which audio is attached, that can be clicked on to har audio/ edit/ rerecord etc. 
        //_currentAnnotation - audio clip komponente finden, clip adden

    }

    // -- Listen Button
    public void PlaySpeechAnnotation()
    {
        _currentAnnotation.transform.GetComponent<AudioSource>().Play();
    }
    
    // -- Delete Button
    public void DeleteSpeechAnnotation()
    {
        //delete audio file in folder where it was saved
        DeleteAnnotationFromList(_currentAnnotation);
        Destroy(_currentAnnotation);
        AnnotationControlWindow.SetActive(false);
    }

    // -- Save Button
    public void SaveSpeechAnnotation()
    {
        _currentAnnotation.SetActive(true);
        CloseAnnotationControlWindow();
    }

    public void CloseAnnotationControlWindow()
    {
        _currentAnnotation = null;
        if (_comingFromAnnotationOverview)
        {
            OpenAllAnnotationsList();
            _comingFromAnnotationOverview = false;
        }
        if (_comingFromObject)
        {
            SelectionMenu.SetActive(true);
            PositionNextTo(SelectionMenu, _currentObjectToBeAnnotated);
            _comingFromObject = false;
        }
        _currentObjectToBeAnnotated = null;
        AnnotationControlWindow.SetActive(false);
    }

    //********************** 

    //utility
    private void PositionNextTo(GameObject _objectToRelocate, GameObject _objectToPositionItNextTo)
    {
        _objectToRelocate.transform.rotation = _objectToPositionItNextTo.transform.rotation;
        _objectToRelocate.transform.position = _objectToPositionItNextTo.transform.position + _objectToPositionItNextTo.transform.right * .3f;
    }

    //changed version of script by Author: Isaac Dart, June-13. (see UICreation Script for more Info)
    private TMP_Text GetChildTMPText(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject.GetComponent<TMP_Text>();
        return null;
    }
}
