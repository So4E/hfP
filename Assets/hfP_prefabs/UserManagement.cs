using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserManagement : MonoBehaviour
{
    [SerializeField] private GameObject UserWarningDisconnect;
    [SerializeField] private TMP_Text UserWarningMessage;

    private GameObject _selectedUser;
    private string _userWarning = "Are you sure you want to disconnect ";

    public void ReallyWantToDisconnect(GameObject _user)
    {
        _selectedUser = _user;
        UserWarningDisconnect.SetActive(true);
        if(_user.name == "Tester 1")
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
        if (_selectedUser.name == "Tester 1")
        {
            //todo - close all windows and start again with onboarding -- mock option: end application and start again manually?
            return;
        }
        GameObject _thisUserParent = _selectedUser.transform.parent.gameObject;
        _thisUserParent.SetActive(false);
        UserWarningDisconnect.SetActive(false);
    }
}
