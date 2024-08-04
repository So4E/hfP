using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitivesControl : MonoBehaviour
{
    [SerializeField] private GameObject ObjectsParentInScene;
    [SerializeField] private GameObject NewObjectUI;
    [SerializeField] private GameObject UIParentOfObjectToControl;

    private GameObject _thisInteractableObject;
    private Vector3 _ParentPositionInUI = new Vector3(0,0,0);
    private Vector3 _theObjectsLocalPosition;
    private Vector3 _theObjectsNewGlobalPosition;



    //----------------------- hier weitermachen: todo: check, ob wenn ich den cube wegbewege, sich die local Position des parentUIObjects überhaupt ändert,
    //oder ob ich nur das child verschiebe, je nachdem methoden unten anpassen!! 

    void OnEnable()
    {
        if(_ParentPositionInUI == new Vector3(0,0,0))
        {
            _ParentPositionInUI = UIParentOfObjectToControl.transform.localPosition;
        }
        Vector3 _thisObjectsLocalPosition = UIParentOfObjectToControl.transform.localPosition;
        Debug.Log(UIParentOfObjectToControl.name + "'s initial local position: " + _thisObjectsLocalPosition);
        _thisInteractableObject = UIParentOfObjectToControl.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(_theObjectsLocalPosition != UIParentOfObjectToControl.transform.localPosition)
        {
            Debug.Log(UIParentOfObjectToControl.name + "'s position changed to: " + UIParentOfObjectToControl.transform.localPosition);
            //if object is x far away from original local Position --- how to check??? 
            if (CheckIfObjectWasMovedOutOfUI())
            {
                //save current parent's global position 
                _theObjectsNewGlobalPosition = UIParentOfObjectToControl.transform.position;

                //change ObjectsParent to its new parent and change location to saved position (maybe somehow have to adjust localPosition as well?)
                _thisInteractableObject.gameObject.transform.parent = ObjectsParentInScene.gameObject.transform;
                _thisInteractableObject.transform.position = _theObjectsNewGlobalPosition;

                //Todo - also adjust scale ?

                //set UIParent to initial position
                UIParentOfObjectToControl.transform.localPosition = _ParentPositionInUI;
                //then spawn a new object of the same sort as child in ui
                //dont forget to reset all relevant variables in this script to null
            }
        }
        Vector3 _UIPosition = NewObjectUI.transform.position;
    }

    private bool CheckIfObjectWasMovedOutOfUI()
    {
        return false;
    }

}
