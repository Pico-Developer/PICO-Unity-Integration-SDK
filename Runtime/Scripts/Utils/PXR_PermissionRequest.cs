using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class PXR_PermissionRequest : MonoBehaviour
{
    public bool requestMR=false;


    private List<string> _permissions = new List<string>();
    private const string _permissionMr = "com.picovr.permission.SPATIAL_DATA";

    private void Awake()
    {
        if (requestMR)
        {
            _permissions.Add(_permissionMr);
        }
        RequestUserPermissionAll();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestUserPermissionAll()
    {
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
        callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
        Debug.Log("HHHH Permission.Camera Request");
        Permission.RequestUserPermissions(_permissions.ToArray(), callbacks);
    }

    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        Debug.Log($"HHHH {permissionName} PermissionDeniedAndDontAskAgain");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        Debug.Log($"HHHH {permissionName} PermissionCallbacks_PermissionGranted");
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        Debug.Log($"HHHH {permissionName} PermissionCallbacks_PermissionDenied");
    }


    public static void RequestUserPermissionMR(Action<string> _PermissionDenied=null,Action<string> _PermissionGranted=null,Action<string> _PermissionDeniedAndDontAskAgain=null)
    {
        if (Permission.HasUserAuthorizedPermission(_permissionMr))
        {
            if (_PermissionGranted != null)
            {
                _PermissionGranted(_permissionMr);
            }
        }
        else
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += _PermissionDenied;
            callbacks.PermissionGranted += _PermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += _PermissionDeniedAndDontAskAgain;
            Permission.RequestUserPermission(_permissionMr,callbacks);
        }
    }

}
