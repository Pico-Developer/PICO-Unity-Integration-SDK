using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VirtualDisplayEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler,
    IInitializePotentialDragHandler
{
    public string tag = "VirtualDisplayEvent----";
    public XRRayInteractor xrLeftRayInteractor;
    public XRRayInteractor xrRightRayInteractor;
    public VirtualDisplayDemo virtualDisplayController;
    public Text mylog;
    private XRRayInteractor currentRayInteractor;
    private GameObject mDisplay;
    private RectTransform mDisplayTran;
    private int mKeyEvent;
    private const int KEYEVENT_DEFAULT = -1;
    private const int KEYEVENT_DOWN = 0;
    private const int KEYEVENT_UP = 1;
    bool LeftState = false;
    private bool mLeftTriggerPressTemp = false;
    bool RightState = false;
    private bool mRightTriggerPressTemp = false;

    // Start is called before the first frame update
    void Start()
    {
        mDisplay = this.gameObject;
        mDisplayTran = mDisplay.GetComponent<RectTransform>();
        mKeyEvent = KEYEVENT_DEFAULT;
        currentRayInteractor = xrRightRayInteractor;
    }
    public void showLog(string log)
    {
        Debug.Log(tag + log);
        mylog.text = log;
    }
    // Update is called once per frame
    void Update()
    {
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(PXR_Usages.controllerStatus, out LeftState);
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand)
            .TryGetFeatureValue(PXR_Usages.controllerStatus, out RightState);
        if (RightState)
        {
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand)
                .TryGetFeatureValue(CommonUsages.triggerButton, out mRightTriggerPressTemp);
            if (mRightTriggerPressTemp)
            {
                currentRayInteractor = xrRightRayInteractor;
            }
        }
        else if (LeftState)
        {
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand)
                .TryGetFeatureValue(CommonUsages.triggerButton, out mLeftTriggerPressTemp);
            if (mLeftTriggerPressTemp)
            {
                currentRayInteractor = xrLeftRayInteractor;
            }
        }


        if (mKeyEvent != KEYEVENT_DEFAULT)
        {
            DispatchMessageToAndroid(mKeyEvent, null);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mKeyEvent = KEYEVENT_DOWN;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mKeyEvent = KEYEVENT_UP;
    }

    public void OnDrag(PointerEventData eventData)
    {
        mKeyEvent = KEYEVENT_DOWN;
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }

    private void DispatchMessageToAndroid(int actionType, PointerEventData eventData)
    {
        #if XRI_240
        Vector3 eventPoint = mDisplay.transform.InverseTransformPoint(currentRayInteractor.rayEndPoint);
        if (Application.platform == RuntimePlatform.Android)
        {
            InstrumentationInput(eventPoint, actionType);
        }
        else
        {
            float x = (eventPoint.x + mDisplayTran.sizeDelta.x / 2) / mDisplayTran.sizeDelta.x;
            float y = (mDisplayTran.sizeDelta.y / 2 - eventPoint.y) / mDisplayTran.sizeDelta.y;
            mKeyEvent = KEYEVENT_DEFAULT;
            Debug.Log(actionType + "--->" + x + ", " + y + ", " + eventPoint.x + ", " + eventPoint.y);
        }
        #else
        showLog("com.unity.xr.interaction.toolkit Version needs to be greater than 2.3.x");
        // Debug.LogError("com.unity.xr.interaction.toolkit Version needs to be greater than 2.3.x");
        #endif
    }

    private bool mIsUp = true;
    private float mLastX, mLastY;

    private void InstrumentationInput(Vector3 eventPoint, int actionType)
    {
     
        float eventX = eventPoint.x;
        float eventY = eventPoint.y;
        float x = (eventX + mDisplayTran.sizeDelta.x / 2) / mDisplayTran.sizeDelta.x;
        float y = (mDisplayTran.sizeDelta.y / 2 - eventY) / mDisplayTran.sizeDelta.y;
     
        if (mIsUp && (eventX == 0.0f || eventY == 0.0f || x > 0.99f || x < 0.01f || y > 0.99f || y < 0.01f))
        {
            //处理在屏幕外操作的问题
            showLog("input--->out of the screen---1");
            mKeyEvent = KEYEVENT_DEFAULT;
            return;
        }

        if (actionType == KEYEVENT_DOWN)
        {
            if (mIsUp)
            {
                mIsUp = false;
                virtualDisplayController.InjectEvent(VirtualDisplayDemo.ACTION_DOWN,x,y);
                showLog("down--->" + x + ", " + y + ", " + eventX + ", " + eventY);
            }

            if (!mIsUp)
            {
                if (eventX == 0.0f || eventY == 0.0f || x > 0.99f || x < 0.01f || y > 0.99f || y < 0.01f)
                {
                    //处理划出屏幕，还未抬起的问题
                    showLog("input--->out of the screen---2");
                    mIsUp = true;
                    mKeyEvent = KEYEVENT_DEFAULT;
                    virtualDisplayController.InjectEvent(VirtualDisplayDemo.ACTION_UP,x,y);
                    showLog("up--->" + mLastX + ", " + mLastY + ", " + eventX + ", " + eventY);
                }
                else
                {
                    virtualDisplayController.InjectEvent(VirtualDisplayDemo.ACTION_MOVE,x,y);
                    showLog("move--->" + x + ", " + y + ", " + eventX + ", " + eventY);
                }
            }
        }
        else if (actionType == KEYEVENT_UP)
        {
            mIsUp = true;
            mKeyEvent = KEYEVENT_DEFAULT;
            virtualDisplayController.InjectEvent(VirtualDisplayDemo.ACTION_UP,x,y);
            showLog("up--->" + x + ", " + y + ", " + eventPoint.x + ", " + eventPoint.y);
        }

        mLastX = x;
        mLastY = y;
    }
}