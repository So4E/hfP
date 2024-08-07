using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorManager : MonoBehaviour
{
    [SerializeField] private GameObject SelectionMenu;
    [SerializeField] private GameObject ColorPickerWindow;
    [SerializeField] private TMP_Text CurrentlySelectedColorIcon;
    [SerializeField] private TMP_Text ColorHexText;


    private GameObject _selectedObject;
    private Color _colorSelected;
    private string _colorhex = "FFFFFF";

    public void OnOpenColorPicker(GameObject _objectSelected)
    {
        ColorPickerWindow.SetActive(true);
        PositionAbove(ColorPickerWindow, SelectionMenu);
        _selectedObject = _objectSelected;
        //get color of object selected and display?
    }

    public void OnColorSelect(TMP_Text _buttonColor)
    {
        _colorSelected = _buttonColor.color;
        _colorhex = ColorUtility.ToHtmlStringRGB(_colorSelected);
        ColorHexText.text = _colorhex.ToString();
        CurrentlySelectedColorIcon.color = _colorSelected;
    }

    public void OnCopySelectedColor()
    {
        //copy to clipboard
        GUIUtility.systemCopyBuffer = _colorhex;
    }

    public void OnApplyColor()
    {
        // --------------- TODO
        //get color component in object and set color
        //_selectedObject.transform.GetChild(0).GetComponent<Default-Material>().SetColor("Color", _colorSelected);
        //**************** component not existent.. how to change default color? 
    }

    public void OnCloseColorPicker()
    {
        _selectedObject = null;
        _colorSelected = Color.white;
        _colorhex = "FFFFFF";
        ColorHexText.text = _colorhex;
        CurrentlySelectedColorIcon.color = Color.white; // white = new Color(255, 255, 255, 255);
        ColorPickerWindow.SetActive(false);
    }

    //utility
    private void PositionAbove(GameObject _objectToRelocate, GameObject _objectToPositionItNextTo)
    {
        _objectToRelocate.transform.rotation = _objectToPositionItNextTo.transform.rotation;
        _objectToRelocate.transform.position = _objectToPositionItNextTo.transform.position + _objectToPositionItNextTo.transform.up * .25f;
    }
}
