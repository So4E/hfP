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

    [SerializeField] private GameObject Cube;
    [SerializeField] private GameObject Sphere;
    [SerializeField] private GameObject Cylinder;
    [SerializeField] private GameObject Capsule;


    private GameObject _selectedObject;

    public void Start()
    {
    }

    public void OnSelectObject(GameObject _object)
    {
        if(_selectedObject != _object) {
            _selectedObject = _object;
            SelectMenu.SetActive(true);
            SelectMenuHeader.text = _object.name;
        }
        PositionNextTo(SelectMenu, _object);
        //place selected menu next to object facing user and let it follow along
    }

    public void OnDuplicateObject()
    {
        GameObject _rootObject = null;
        switch (_selectedObject.tag)
        {
            case "Cube":
                _rootObject = Cube;
                break;
            case "Sphere":
                _rootObject = Sphere;
                break;
            case "Cylinder":
                _rootObject = Cylinder;
                break;
            case "Capsule":
                _rootObject = Capsule;
                break;
        }
        //instantiating root object because cloning _selectedObject will result in Select Exit Error for Input Manager due to confusion that both object and clone are/ were selected
        GameObject _clone = Instantiate(_rootObject, (_selectedObject.transform.position + _selectedObject.transform.right * - .3f), _selectedObject.transform.rotation, ObjectsParent.transform);
        _clone.transform.localScale = _selectedObject.transform.localScale;

        //HIER WEITERMACHEN
        //todo -- add OnSelectObject() method to _clone so that menu is shown next to it when selected

        //todo transfer also other object info into this new object
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

    private void PositionNextTo(GameObject _objectToRelocate, GameObject _objectToPositionItNextTo)
    {
        _objectToRelocate.transform.rotation = _objectToPositionItNextTo.transform.rotation;
        _objectToRelocate.transform.position = _objectToPositionItNextTo.transform.position + _objectToPositionItNextTo.transform.right * .3f;
    }
}
