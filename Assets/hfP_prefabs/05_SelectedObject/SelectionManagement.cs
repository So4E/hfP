using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MixedReality.Toolkit.SpatialManipulation;


public class SelectionManagement : MonoBehaviour
{
    [SerializeField] private GameObject SelectMenu;
    [SerializeField] private TMP_Text SelectMenuHeader;
    [SerializeField] private GameObject ObjectsParent;
    [SerializeField] private GameObject AnnotationManager;

    [SerializeField] private GameObject EmptyGameObject;
    [SerializeField] private GameObject Cube;
    [SerializeField] private GameObject Sphere;
    [SerializeField] private GameObject Cylinder;
    [SerializeField] private GameObject Capsule;

    private GameObject _selectedObject;

    public void OnSelectObject(GameObject _object)
    {
        if(_selectedObject != _object) {
            _selectedObject = _object;
            SelectMenu.SetActive(true);
            SelectMenuHeader.text = _object.name;
        }
        PositionNextTo(SelectMenu, _object);
        //place selected menu not only next to object but let it face user and follow along with object
    }

    public void OnDuplicateObject()
    {
        // --- APPROACH 1 --- (workaround for cloning)
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
            case "Empty":
                _rootObject = EmptyGameObject;
                break;
        }

        //instantiating root object because cloning _selectedObject will result in Select Exit Error for Input Manager due to confusion that both object and clone are/ were selected
        GameObject _clone = Instantiate(_rootObject, (_selectedObject.transform.position + _selectedObject.transform.right * - .3f), _selectedObject.transform.rotation, ObjectsParent.transform);
        _clone.transform.localScale = _selectedObject.transform.localScale;
        _clone.SetActive(true);
        //if this method will be used: todo: transfer also other object info into this new object -> if cloning _selectedObject to avoid that - find out how to work around selection error described above

        /*--- APPROACH 2 ---
        //second approach - back to cloning
        GameObject _clone = Instantiate(_selectedObject, (_selectedObject.transform.position + _selectedObject.transform.right * -.3f), _selectedObject.transform.rotation, ObjectsParent.transform);
        Destroy(_clone.transform.GetChild(2).gameObject);
        _clone.GetComponent<BoundsControl>().RecomputeBounds();
        //PROBLEM: how to address this issue when istanciating objects, method needs to be in there then.. */
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
