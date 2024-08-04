using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitivesControl : MonoBehaviour
{
    [SerializeField] private GameObject ObjectsParentInScene;
    [SerializeField] private GameObject Cube;
    [SerializeField] private GameObject Sphere;
    [SerializeField] private GameObject Cylinder;
    [SerializeField] private GameObject Capsule;

    private Vector3 _theObjectsNewGlobalPosition;

    //MRTK Events --> IsGrabSelected()
    public void OnGrabSelectExited(GameObject _BoundsControlObject)
    {
        _theObjectsNewGlobalPosition = _BoundsControlObject.transform.position; //check if moved if parent moves or only object
        GameObject _rootObject = null;
        switch (_BoundsControlObject.tag)
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
        GameObject _spawnedObject = Instantiate(_rootObject, _theObjectsNewGlobalPosition, Quaternion.identity, ObjectsParentInScene.transform);
        _spawnedObject.SetActive(true);
        //snap back UIObject to its position
        _BoundsControlObject.transform.localPosition = new Vector3(0, 0, 0);
    }
}
