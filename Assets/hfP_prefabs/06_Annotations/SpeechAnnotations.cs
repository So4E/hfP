using System;
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
    [SerializeField] private MRTKTMPInputField AnnotationNameWindowText;
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
    private AudioSource _testRecordAnnotation;

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
        if (keyboard != null && AnnotationNameWindow.activeSelf == true) //&& AnnotationNameWindow is active
        {
            string _keyboardInput = keyboard.text;
            AnnotationNameWindowText.text =_keyboardInput;
        }
    }


    //** All Annotations List - Called via Tools
    public void OpenAllAnnotationsList()
    {
        ReloadOverView();
        PositionNextTo(AnnotationOverview, ToolsWindow);

    }

    private void ReloadOverView()
    {
        AnnotationOverview.SetActive(true);
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
            //check if object and with that annotation might have been deleted. if so remove from list and break -> next item at this index also has to be checked
            try{
                string _name = _createdAnnotations[i].name;
            } catch(Exception _MissingReferenceException)
            {
                _createdAnnotations.RemoveAt(i);
                i--;
                break;
            }

            //create one annotation button for each name in list
            GameObject _annotationButtonElement = Instantiate(AnnotationButton, AnnotationButtonParent.transform);
            _annotationButtonElement.GetComponent<PressableButton>().OnClicked.AddListener(() => OnOpenAnnotationFromList(_annotationButtonElement));

            TMP_Text _annotationName = GetChildTMPText(_annotationButtonElement, "TextText");
            _annotationName.SetText(_createdAnnotations[i].name);
            _annotationButtonElement.name = _annotationName.text;

            TMP_Text _annotatedObject = GetChildTMPText(_annotationButtonElement, "ObjectText");
            _annotatedObject.SetText(_createdAnnotations[i].transform.parent.gameObject.name);
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
        _comingFromObject = true;
        //now waiting for user to input name and confirm
    }

    public void OnExitNameOfAnnotation()
    {
        SelectionMenu.SetActive(true);
        CloseAnnotationControlWindow();
        AnnotationNameWindow.SetActive(false);
    }

    public void OnConfirmNameOfAnnotation()
    {
        GameObject _newAnnotation = Instantiate(AnnotationObject, _currentObjectToBeAnnotated.transform);
        _newAnnotation.GetComponent<PressableButton>().OnClicked.AddListener(() => PlayAnnotationAudio(_newAnnotation));

        _currentAnnotation = _newAnnotation;
        string _textInput = AnnotationNameWindowText.text;
        //if input is empty or name is already taken, reset it automatically:
        if (string.IsNullOrEmpty(_textInput) || FindAnnotationOnList(_textInput) != null) { 
            _textInput = "new Annotation_" + _currentAnnotation.GetInstanceID();
        }

        _currentAnnotation.name = _textInput;
        _createdAnnotations.Add(_currentAnnotation);
        if (AnnotationOverview.activeSelf)
        {
            ReloadOverView();
        }

        PositionNextTo(AnnotationControlWindow, _currentObjectToBeAnnotated);
        AnnotationNameWindow.SetActive(false);
        AnnotationControlWindow.SetActive(true);
        AnnotationControlWindowObjectText.SetText(" * " + _currentAnnotation.name + " * ");
    }

    //********************** 

    //** Annotation Options - Called via AnnotationControl Window

    //saves annotation in runtime at annotation object
    public void StartRecordingAnnotation()
    {
        AudioSource aud = _currentAnnotation.GetComponent<AudioSource>();
        //public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency); emptyDevice name = default microphone
        aud.clip = Microphone.Start("", true, 10, 44100);
    }

    public void StopRecordingAnnotation()
    {
        Microphone.End(""); //if empty string is passed, default microphone is used  
    }

    // -- Listen Button
    public void PlaySpeechAnnotation()
    {
        _currentAnnotation.transform.GetComponent<AudioSource>().Play();
    }
    
    // -- Delete Button
    public void DeleteSpeechAnnotation()
    {
        DeleteAnnotationFromListAndDestroy(_currentAnnotation.name);
        CloseAnnotationControlWindow();
    }

    private bool DeleteAnnotationFromListAndDestroy(string _annotationName)
    {
        for (int i = 0; i < _createdAnnotations.Count; i++)
        {
            if (_createdAnnotations[i].name == _annotationName)
            {
                GameObject _annotationObject = _createdAnnotations[i].gameObject;
                _createdAnnotations.Remove(_createdAnnotations[i].gameObject);
                DeleteAnnotationButtonFromOverview(_annotationName); //also delete annotation object in overview
                Destroy(_annotationObject);
                return true;
            }
        }
        return false;
    }

    private void DeleteAnnotationButtonFromOverview(string _annotationName)
    {
        foreach(Transform t in AnnotationButtonParent.transform)
        {
            if(t.gameObject.name == _annotationName)
            {
                Destroy(t.gameObject);
                return;
            }
        }
    }

    // -- Save Button
    public void SaveSpeechAnnotation()
    {
        _currentAnnotation.SetActive(true);
        PositionAnnotation(_currentAnnotation, _currentAnnotation.transform.parent.gameObject);
        CloseAnnotationControlWindow();
        //place annotation next to object somehow
    }

    public void CloseAnnotationControlWindow()
    {
        _currentAnnotation = null;
        if (_comingFromAnnotationOverview)
        {
            ReloadOverView();
            _comingFromAnnotationOverview = false;
        }
        if (_comingFromObject)
        {
            SelectionMenu.SetActive(true);
            PositionNextTo(SelectionMenu, _currentObjectToBeAnnotated);
            _comingFromObject = false;
        }
        StopRecordingAnnotation();
        _currentObjectToBeAnnotated = null;
        AnnotationControlWindow.SetActive(false);
    }

    //********************** 


    //********** SpeechAnnotation Object above GameObject
    
    public void PlayAnnotationAudio(GameObject _annotationObject)
    {
        //todo - what if there is no audio yet? - does not throw an error?
        _annotationObject.GetComponent<AudioSource>().Play();
    }

    //********************** 

    //utility
    private void PositionNextTo(GameObject _objectToRelocate, GameObject _objectToPositionItNextTo)
    {
        _objectToRelocate.transform.rotation = _objectToPositionItNextTo.transform.rotation;
        _objectToRelocate.transform.position = _objectToPositionItNextTo.transform.position + _objectToPositionItNextTo.transform.right * .3f;
    }
    
    private void PositionAnnotation(GameObject _objectToRelocate, GameObject _objectToPositionItNextTo)
    {
        _objectToRelocate.transform.rotation = _objectToPositionItNextTo.transform.rotation;
        _objectToRelocate.transform.position = _objectToPositionItNextTo.transform.position + _objectToPositionItNextTo.transform.up * .2f;
    }

    //changed version of script by Author: Isaac Dart, June-13. (see UICreation Script for more Info)
    private TMP_Text GetChildTMPText(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject.GetComponent<TMP_Text>();
        return null;
    }
}
