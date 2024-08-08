using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserManagement : MonoBehaviour
{
    [SerializeField] private GameObject UserSettings;
    [SerializeField] private GameObject UserWarningDisconnect;
    [SerializeField] private TMP_Text UserWarningMessage;

    private GameObject _selectedUser;
    private string _userWarning = "Are you sure you want to disconnect ";
    private string _testUserName = "Tester*in";

    public void ReallyWantToDisconnect(GameObject _user)
    {
        _selectedUser = _user;
        UserWarningDisconnect.SetActive(true);
        PositionInFrontOf(UserWarningDisconnect, UserSettings);
        if(_user.name == _testUserName)
        {
            UserWarningMessage.text = _userWarning + "yourself from this project AND EXIT THIS PROJECT?";
            return;
        }
        UserWarningMessage.text =_userWarning + _selectedUser.name + "?";
    }

    public void DoNotDisconnectUser()
    {
        UserWarningDisconnect.SetActive(false);
        _selectedUser = null;
    }

    //Mock Disconnect
    public void DisconnectUser()
    {
        if (_selectedUser.name == _testUserName)
        {
            //todo - close all windows and start again with onboarding -- mock option: end application and start again manually?
            return;
        }
        GameObject _thisUserParent = _selectedUser.transform.parent.gameObject;
        _thisUserParent.SetActive(false);
        UserWarningDisconnect.SetActive(false);
    }

    private void PositionInFrontOf(GameObject _objectToRelocate, GameObject _objectToPositionItNextTo)
    {
        _objectToRelocate.transform.rotation = _objectToPositionItNextTo.transform.rotation;
        _objectToRelocate.transform.position = _objectToPositionItNextTo.transform.position + _objectToPositionItNextTo.transform.forward * -0.2f;
    }
}
