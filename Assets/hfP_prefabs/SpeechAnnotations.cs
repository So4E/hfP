using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;


public class SpeechAnnotations : MonoBehaviour
{
    [SerializeField] private GameObject ToolsWindow;
    [SerializeField] private GameObject AnnotationNameWindow;
    [SerializeField] private TMP_Text AnnotationNameWindowText;
    [SerializeField] private GameObject AnnotationOverview;
    [SerializeField] private GameObject AnnotationButtonParent;
    [SerializeField] private GameObject AnnotationButton;
    [SerializeField] private GameObject AnnotationControlWindow;
    [SerializeField] private TMP_Text AnnotationControlWindowObjectText;
    [SerializeField] private GameObject AnnotationObject;

    [SerializeField] private TouchScreenKeyboard keyboard;


    private GameObject _currentAnnotation;
    private GameObject _currentObjectToBeAnnotated; //not needed? 

    private List<AudioClip> _createdAudioClips = new List<AudioClip>();
    private List<GameObject> _createdAnnotations = new List<GameObject>();
    //better have key value store of object and attached audioClips - OR
    // add recorded audio clip as audio source to annotation prefab, and save it as child of object
    //

    // Start is called before the first frame update
    void Start()
    {
        foreach (string device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
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
        //set header to name
        //get currentAnnotation from list and save to edit
    }

    //********************** 

    //** Create Annotation - Called via Annotation Button when Object Is Selected
    public void CreateAnnotation(GameObject _objectToBeAnnotated) //todo - how to add object here? -> selectionManager for selected objects must call this method
    {
        //todo - place text input window next to Calling Element
        AnnotationNameWindow.SetActive(true);
        _currentObjectToBeAnnotated = _objectToBeAnnotated;
        _currentAnnotation = Instantiate(AnnotationObject, _objectToBeAnnotated.transform);
        _createdAnnotations.Add(_currentAnnotation);
        //now waiting for user to input name and confirm
    }

    public void OnExitNameOfAnnotation()
    {
        //todo - find object on list first and delete from list - otherwise a list element which is null remains?
        Destroy(_currentAnnotation);
    }

    public void OnConfirmNameOfAnnotation()
    {
        string _textInput = AnnotationNameWindowText.text;
        _currentAnnotation.name = "Annotation_" + _textInput;
        AnnotationControlWindow.SetActive(true);
        AnnotationControlWindowObjectText.SetText(" * " + _currentAnnotation.name + " * ");
    }

    //********************** 

    //** Annotation Options - Called via AnnotationControl Window ----------------------------------hier weitermachen """"""""""""""""""""""

    public void StartRecordingAnnotation()
    {
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = Microphone.Start("Built-in Microphone", true, 10, 44100); //public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency); 
        _createdAudioClips.Add(aud.clip);
        //audio clip iwo abspeichern in ordnerstruktur?
        //TODO ------------------ create object in scene to which audio is attached, that can be clicked on to har audio/ edit/ rerecord etc. 
        //_currentAnnotation - audio clip komponente finden, clip adden

    }

    // -- Listen Button
    public void PlaySpeechAnnotation()
    {
        // _currentAnnotation daraus audioClipComponente, dann  audioClip.Play();
    }
    
    // -- Delete Button
    public void DeleteSpeechAnnotation()
    {
        //delete audio button from list and object in scene  und audio clip von audioclip liste und aus ordnerstruktur
        //close all windows to edit this audio file
        //set currentAnnotation to null
    }

    // -- Save Button


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
