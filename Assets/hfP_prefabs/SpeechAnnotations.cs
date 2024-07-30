using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.UX;


public class SpeechAnnotations : MonoBehaviour
{
    [SerializeField] private GameObject ToolsWindow;
    [SerializeField] private GameObject AnnotationOverview;
    [SerializeField] private GameObject AnnotationButtonParent;
    [SerializeField] private GameObject AnnotationButton;
    [SerializeField] private GameObject AnnotationControlWindow;
    [SerializeField] private GameObject AnnotationObject;

    private GameObject _currentAnnotation;

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

    public void RecordSpeechAnnotation()
    {
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = Microphone.Start("Built-in Microphone", true, 10, 44100); //public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency); 
        _createdAudioClips.Add(aud.clip);
        //TODO ------------------ create object in scene to which audio is attached, that can be clicked on to har audio/ edit/ rerecord etc. 
    }

    public void PlaySpeechAnnotation()
    {
        // _currentAnnotation daraus audioClipComponente, dann  audioClip.Play();
    }

    public void DeleteSpeechAnnotation()
    {
        //delete audio button from list and object in scene 
        //close all windows to edit this audio file
        //set currentAnnotation to null
    }

    //utility

    private void PositionNextTo(GameObject _objectToRelocate, GameObject _objectToPositionItNextTo)
    {
        Vector3 _positionRightOfObject = _objectToPositionItNextTo.transform.position + new Vector3(0.3f, 0, 0);
        _objectToRelocate.transform.position = _positionRightOfObject;
    }


    //changed version of script by Author: Isaac Dart, June-13. (see UICreation Script for more Info)
    private TMP_Text GetChildTMPText(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject.GetComponent<TMP_Text>();
        return null;
    }

}
