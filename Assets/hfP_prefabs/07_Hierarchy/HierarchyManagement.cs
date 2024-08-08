using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchyManagement : MonoBehaviour
{
    [SerializeField] private GameObject HierarchyWindow;
    [SerializeField] private GameObject HierarchyObjectParent;
    [SerializeField] private GameObject ObjectButtonForHierarchy;

    public void CreateObjectList()
    {
        //skip the first 5 objects -> primitives and empty to be cloned from
        //create one button for each object in scene
        //add listener - if button clicked open hierarchy selection menu (delete, duplicate, rename)
    }

}
