using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechAnnotations : MonoBehaviour
{
    private List<AudioClip> _createdAnnotations = new List<AudioClip>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (string device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }

    public void RecordSpeechAnnotation()
    {
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = Microphone.Start("Built-in Microphone", true, 10, 44100); //public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency); 
        _createdAnnotations.Add(aud.clip);
        //create object in scene to which audio is attached, that can be clicked on to har audio/ edit/ rerecord etc. 
    }

    public void PlaySpeechAnnotation()
    {
        //find Annotation in list
        // then:         aud.Play();
    }

    public void DeleteSpeechAnnotation()
    {
        //delete audio from list and object in scene 
        //close all windows to edit this audio file
    }



}
