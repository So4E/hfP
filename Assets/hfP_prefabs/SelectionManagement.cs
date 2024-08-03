using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectionManagement : MonoBehaviour
{
    [SerializeField] private GameObject SelectMenu;
    [SerializeField] private TMP_Text SelectMenuHeader;
    [SerializeField] private GameObject ObjectsParent;
    [SerializeField] private GameObject AnnotationManager;

    private GameObject _selectedObject;

    public void Start()
    {
    }

    public void OnSelectObject(GameObject _object)
    {
        if(_selectedObject == _object) { return; }
        _selectedObject = _object;
        SelectMenu.SetActive(true);
        SelectMenuHeader.text = _object.name;
        //place selected menu next to object and let it follow along
    }

    public void OnDuplicateObject()
    {
        Instantiate(ObjectsParent, (_selectedObject.transform.position + _selectedObject.transform.right * .3f), Quaternion.identity);
    }

    public void OnOpenInspector()
    {
        //todo - open inspector window with all pieces of information on object
    }

    public void OnOpenColorPicker()
    {
        //todo - open colour picker tool to enable selection of different object color
    }

    public void OnAnnotateObject()
    {
        AnnotationManager.gameObject.GetComponent<SpeechAnnotations>().CreateAnnotation(_selectedObject);
    }

    public void OnDeleteObject()
    {
        Destroy(_selectedObject);
        SelectMenu.SetActive(false);
    }
}
