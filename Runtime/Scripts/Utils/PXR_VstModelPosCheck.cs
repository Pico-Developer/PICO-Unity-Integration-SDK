using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PXR_VstModelPosCheck : MonoBehaviour
{
    public bool IsController = false;
    private Transform mMainCamTrans;
    private XRBaseController mXRBaseController;
    private PXR_Hand mPXR_Hand;

    private float mVirtualWorldOffset = 0.03f;
    private readonly Vector3 mStartDirection = new Vector3(0f, 0f, 1.0f);
    private Quaternion mHeadRotation;
    private Vector3 mOffsetDirection;
    private Vector3 mOffsetPos;

    // Start is called before the first frame update
    void Start()
    {
        if (IsController)
        {
            if (mXRBaseController == null)
                mXRBaseController = GetComponent<XRBaseController>();
        }
        else
        {
            if (mPXR_Hand == null)
                mPXR_Hand = GetComponent<PXR_Hand>();
        }
        
        mMainCamTrans = Camera.main.transform;
        mVirtualWorldOffset = PXR_Plugin.System.UPxr_VstModelOffset();
    }


    private void OnEnable()
    {
        Application.onBeforeRender += CheckPos;
    }

    private void OnDisable()
    {
        Application.onBeforeRender -= CheckPos;
    }
    

    private void UpdatePos()
    {
        if(mMainCamTrans==null){
             mMainCamTrans = Camera.main.transform;
        }
        if(mMainCamTrans!=null){
             mHeadRotation = mMainCamTrans.localRotation;
             mOffsetDirection = mHeadRotation * (-1f * mStartDirection);
             mOffsetPos = mOffsetDirection * mVirtualWorldOffset;
        }
       
    }

    private void CheckPos()
    {
     
        if (IsController)
        {
            UpdatePos();
            if(mXRBaseController == null){
                mXRBaseController = GetComponent<XRBaseController>();
            }
            if(mXRBaseController != null){
                transform.localPosition = mXRBaseController.currentControllerState.position + mOffsetPos;
            }  
        }
    }

    public Vector3 GetHandPosOffset()
    {
        UpdatePos();
        return mOffsetPos;
    }
}
