using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.PXR.Input;

namespace Unity.XR.PXR
{
    public class PXR_ControllerWithHandAnimator : MonoBehaviour
    {
        public PXR_Input.Controller controller;

        private Animator mAnimator;
        private InputDevice mInputDevice;
        private PXR_Controller mXRController;

        private readonly float animation_time = 0.05f;
        private float per_animation_step = 0.1f;

        //trigger;
        private readonly string trigger_Touch_LayerName = "trigger_touch";
        private int trigger_Touch_LayerIndex;
        private readonly string trigger_Value_LayerName = "trigger_press";
        private int trigger_Value_LayerIndex;
        private bool trigger_Touch;
        private float trigger_Value;
        private float trigger_Touch_Weight = 0f;

        // A/X;
        private readonly string X_A_Touch_LayerName = "X_A_touch";
        private int X_A_Touch_LayerIndex;
        private readonly string X_A_Press_LayerName = "X_A_press";
        private int X_A_Press_LayerIndex;
        private bool X_A_Press;
        private bool X_A_Touch;
        private float X_A_Touch_Weight = 0f;

        // B/Y;
        private readonly string Y_B_Touch_LayerName = "Y_B_touch";
        private int Y_B_Touch_LayerIndex;
        private readonly string Y_B_Press_LayerName = "Y_B_press";
        private int Y_B_Press_LayerIndex;
        private bool Y_B_Press;
        private bool Y_B_Touch;
        private float Y_B_Touch_Weight = 0f;

        //Y/B or X/A
        private readonly string X_A_Y_B_Press_LayerName = "X_A_Y_B_press";
        private int X_A_Y_B_Press_LayerIndex;

        //Y/B or X/A
        private readonly string X_A_Y_B_Touch_LayerName = "X_A_Y_B_touch";
        private int X_A_Y_B_Touch_LayerIndex;
        private float X_A_Y_B_Touch_Weight = 0f;

        //grip;
        private readonly string grip_Value_LayerName = "grip_press";
        private int grip_Value_LayerIndex;
        private float grip_Value;

        //rocker
        private readonly string primary2DAxis_Touch_LayerName = "axis_touch";
        private int primary2DAxis_Touch_LayerIndex;
        private readonly string primary2DAxis_Vertical = "axis_vertical";
        private int primary2DAxis_Vertical_Index;
        private readonly string primary2DAxis_Horizontal = "axis_horizontal";
        private int primary2DAxis_Horizontal_Index;
        private Vector2 primary2DAxisVec2;
        private bool primary2DAxis_Touch;
        private float primary2DAxis_Touch_Weight = 0f;

        //print screen
        private readonly string menu_Press_LayerName = "thumbMenu";
        private int menu_Press_LayerIndex;
        private bool menu_Press;
        private float menu_Press_Weight;

        //home
        private readonly string pico_Press_LayerName = "thumbPico";
        private int pico_Press_LayerIndex;
        private bool pico_Press;
        private float pico_Press_Weight;

        //thumb rest
        private readonly string thumbstick_Touch_LayerName = "thumbstick_touch";
        private int thumbstick_Touch_LayerIndex;
        private bool thumbstick_Touch;
        private float thumbstick_Touch_Weight;


        // Start is called before the first frame update
        void Start()
        {
            per_animation_step = 1.0f / animation_time;
            mAnimator = GetComponent<Animator>();
            mInputDevice = InputDevices.GetDeviceAtXRNode(controller == PXR_Input.Controller.LeftController ? XRNode.LeftHand : XRNode.RightHand);
            mXRController = (controller == PXR_Input.Controller.LeftController ? PXR_Controller.leftHand : PXR_Controller.rightHand) as PXR_Controller;

            if (mAnimator != null)
            {
                trigger_Touch_LayerIndex = mAnimator.GetLayerIndex(trigger_Touch_LayerName);
                trigger_Value_LayerIndex = mAnimator.GetLayerIndex(trigger_Value_LayerName);
                grip_Value_LayerIndex = mAnimator.GetLayerIndex(grip_Value_LayerName);

                X_A_Touch_LayerIndex = mAnimator.GetLayerIndex(X_A_Touch_LayerName);
                X_A_Press_LayerIndex = mAnimator.GetLayerIndex(X_A_Press_LayerName);
                Y_B_Touch_LayerIndex = mAnimator.GetLayerIndex(Y_B_Touch_LayerName);
                Y_B_Press_LayerIndex = mAnimator.GetLayerIndex(Y_B_Press_LayerName);
                X_A_Y_B_Press_LayerIndex = mAnimator.GetLayerIndex(X_A_Y_B_Press_LayerName);
                X_A_Y_B_Touch_LayerIndex = mAnimator.GetLayerIndex(X_A_Y_B_Touch_LayerName);
                primary2DAxis_Touch_LayerIndex = mAnimator.GetLayerIndex(primary2DAxis_Touch_LayerName);
                thumbstick_Touch_LayerIndex = mAnimator.GetLayerIndex(thumbstick_Touch_LayerName);
                
                primary2DAxis_Vertical_Index = Animator.StringToHash(primary2DAxis_Vertical);
                primary2DAxis_Horizontal_Index = Animator.StringToHash(primary2DAxis_Horizontal);
            }
            else
            {
                Debug.Log("Animator is null");
            }
        }

        // Update is called once per frame
        void Update()
        {
            mInputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out X_A_Press);
            mInputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out X_A_Touch);

            mInputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out Y_B_Press);
            mInputDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out Y_B_Touch);

            mInputDevice.TryGetFeatureValue(CommonUsages.trigger, out trigger_Value);
            mInputDevice.TryGetFeatureValue(PXR_Usages.triggerTouch, out trigger_Touch);

            mInputDevice.TryGetFeatureValue(CommonUsages.grip, out grip_Value);

            mInputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out primary2DAxisVec2);
            mInputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out primary2DAxis_Touch);
            if (!primary2DAxis_Touch)
            {
                if (primary2DAxisVec2 != Vector2.zero)
                    primary2DAxis_Touch = true;
            }
            
            mInputDevice.TryGetFeatureValue(CommonUsages.menuButton, out menu_Press);
            
            if (Y_B_Touch && primary2DAxisVec2 == Vector2.zero)
            {
                if (Y_B_Press)
                {
                    Y_B_Touch_Weight = 1.0f;
                    mAnimator.SetLayerWeight(Y_B_Touch_LayerIndex, Y_B_Touch_Weight);
                    mAnimator.SetLayerWeight(Y_B_Press_LayerIndex, 1.0f);
                    mAnimator.SetLayerWeight(X_A_Y_B_Press_LayerIndex, X_A_Press ? 1.0f : 0.0f);
                }
                else
                {
                    if (X_A_Touch)
                    {
                        if (X_A_Press)
                        {
                            X_A_Touch_Weight = 1.0f;
                            mAnimator.SetLayerWeight(X_A_Touch_LayerIndex, X_A_Touch_Weight);
                        }
                        else
                        {
                            if (X_A_Y_B_Touch_Weight < 0.9999f)
                            {
                                X_A_Y_B_Touch_Weight = Mathf.Min(mAnimator.GetLayerWeight(X_A_Y_B_Touch_LayerIndex) + Time.deltaTime * per_animation_step, 1.0f);
                                mAnimator.SetLayerWeight(X_A_Y_B_Touch_LayerIndex, X_A_Y_B_Touch_Weight);
                            }
                        }
                        mAnimator.SetLayerWeight(X_A_Press_LayerIndex, X_A_Press ? 1.0f : 0f);
                    }
                    else
                    {
                        if (Y_B_Touch_Weight < 0.9999f)
                        {
                            Y_B_Touch_Weight = Mathf.Min(mAnimator.GetLayerWeight(Y_B_Touch_LayerIndex) + Time.deltaTime * per_animation_step, 1.0f);
                            mAnimator.SetLayerWeight(Y_B_Touch_LayerIndex, Y_B_Touch_Weight);
                        }
                        if (X_A_Y_B_Touch_Weight > 0.0001f)
                        {
                            X_A_Y_B_Touch_Weight = Mathf.Max(mAnimator.GetLayerWeight(X_A_Y_B_Touch_LayerIndex) - Time.deltaTime * per_animation_step, 0.0f);
                            mAnimator.SetLayerWeight(X_A_Y_B_Touch_LayerIndex, X_A_Y_B_Touch_Weight);
                        }

                        if (X_A_Touch_Weight > 0.0001f)
                        {
                            X_A_Touch_Weight = Mathf.Max(mAnimator.GetLayerWeight(X_A_Touch_LayerIndex) - Time.deltaTime * per_animation_step, 0.0f);
                            mAnimator.SetLayerWeight(X_A_Touch_LayerIndex, X_A_Touch_Weight);
                        }
                    }
                    mAnimator.SetLayerWeight(Y_B_Press_LayerIndex, 0.0f);
                    mAnimator.SetLayerWeight(X_A_Y_B_Press_LayerIndex, 0.0f);
                }

            }
            else
            {
                if (Y_B_Touch_Weight > 0.0001f)
                {
                    Y_B_Touch_Weight = Mathf.Max(mAnimator.GetLayerWeight(Y_B_Touch_LayerIndex) - Time.deltaTime * per_animation_step, 0.0f);
                    mAnimator.SetLayerWeight(Y_B_Touch_LayerIndex, Y_B_Touch_Weight);
                    mAnimator.SetLayerWeight(Y_B_Press_LayerIndex, 0.0f);
                    mAnimator.SetLayerWeight(X_A_Y_B_Press_LayerIndex, 0.0f);
                }
                if (X_A_Y_B_Touch_Weight > 0.0001f)
                {
                    X_A_Y_B_Touch_Weight = Mathf.Max(mAnimator.GetLayerWeight(X_A_Y_B_Touch_LayerIndex) - Time.deltaTime * per_animation_step, 0.0f);
                    
                    mAnimator.SetLayerWeight(X_A_Y_B_Touch_LayerIndex, X_A_Y_B_Touch_Weight);
                    mAnimator.SetLayerWeight(Y_B_Press_LayerIndex, 0.0f);
                    mAnimator.SetLayerWeight(X_A_Y_B_Press_LayerIndex, 0.0f);
                }
                if (X_A_Touch && primary2DAxisVec2 == Vector2.zero)
                {
                    if (X_A_Press)
                    {
                        X_A_Touch_Weight = 1.0f;
                        mAnimator.SetLayerWeight(X_A_Touch_LayerIndex, X_A_Touch_Weight);
                    }
                    else
                    {
                        if (X_A_Touch_Weight < 0.9999f)
                        {
                            X_A_Touch_Weight = Mathf.Min(mAnimator.GetLayerWeight(X_A_Touch_LayerIndex) + Time.deltaTime * per_animation_step, 1.0f);
                            mAnimator.SetLayerWeight(X_A_Touch_LayerIndex, X_A_Touch_Weight);
                        }
                    }
                    mAnimator.SetLayerWeight(X_A_Press_LayerIndex, X_A_Press ? 1.0f : 0f);
                    mAnimator.SetFloat(primary2DAxis_Vertical_Index, 0f);
                    mAnimator.SetFloat(primary2DAxis_Horizontal_Index, 0f);
                }
                else
                {
                    if (X_A_Touch_Weight > 0.0001f)
                    {
                        X_A_Touch_Weight = Mathf.Max(mAnimator.GetLayerWeight(X_A_Touch_LayerIndex) - Time.deltaTime * per_animation_step, 0.0f);
                        mAnimator.SetLayerWeight(X_A_Touch_LayerIndex, X_A_Touch_Weight);
                        mAnimator.SetLayerWeight(X_A_Press_LayerIndex, 0f);
                    }
                    if (primary2DAxis_Touch)
                    {
                        if (primary2DAxis_Touch_Weight < 0.9999f)
                        {
                            primary2DAxis_Touch_Weight = Mathf.Min(mAnimator.GetLayerWeight(primary2DAxis_Touch_LayerIndex) + Time.deltaTime * per_animation_step, 1.0f);
                            mAnimator.SetLayerWeight(primary2DAxis_Touch_LayerIndex, primary2DAxis_Touch_Weight);
                        }
                        mAnimator.SetFloat(primary2DAxis_Vertical_Index, primary2DAxisVec2.y);
                        mAnimator.SetFloat(primary2DAxis_Horizontal_Index, primary2DAxisVec2.x);
                    }
                    else
                    {
                        if (primary2DAxis_Touch_Weight > 0.0001f)
                        {
                            primary2DAxis_Touch_Weight = Mathf.Max(mAnimator.GetLayerWeight(primary2DAxis_Touch_LayerIndex) - Time.deltaTime * per_animation_step, 0.0f);
                            mAnimator.SetLayerWeight(primary2DAxis_Touch_LayerIndex, primary2DAxis_Touch_Weight);

                            mAnimator.SetFloat(primary2DAxis_Vertical_Index, 0f);
                            mAnimator.SetFloat(primary2DAxis_Horizontal_Index, 0f);
                        }
                        if (thumbstick_Touch)
                        {
                            if (thumbstick_Touch_Weight < 0.9999f)
                            {
                                thumbstick_Touch_Weight = Mathf.Min(mAnimator.GetLayerWeight(thumbstick_Touch_LayerIndex) + Time.deltaTime * per_animation_step, 1.0f);
                                mAnimator.SetLayerWeight(thumbstick_Touch_LayerIndex, thumbstick_Touch_Weight);
                            }
                        }
                        else
                        {
                            if (thumbstick_Touch_Weight > 0.0001f)
                            {
                                thumbstick_Touch_Weight = Mathf.Max(mAnimator.GetLayerWeight(thumbstick_Touch_LayerIndex) - Time.deltaTime * per_animation_step, 0.0f);
                                mAnimator.SetLayerWeight(thumbstick_Touch_LayerIndex, thumbstick_Touch_Weight);
                            }
                        }

                    }
                }
            }

            if (trigger_Touch)
            {
                if (trigger_Touch_Weight < 0.9999f)
                {
                    trigger_Touch_Weight = Mathf.Min(mAnimator.GetLayerWeight(trigger_Touch_LayerIndex) + Time.deltaTime * per_animation_step, 1.0f);
                    mAnimator.SetLayerWeight(trigger_Touch_LayerIndex, trigger_Touch_Weight);
                }
                mAnimator.SetLayerWeight(trigger_Value_LayerIndex, trigger_Value);
            }
            else
            {
                if (trigger_Touch_Weight > 0.0001f)
                {
                    trigger_Touch_Weight = Mathf.Max(mAnimator.GetLayerWeight(trigger_Touch_LayerIndex) - Time.deltaTime * per_animation_step, 0.0f);
                    mAnimator.SetLayerWeight(trigger_Touch_LayerIndex, trigger_Touch_Weight);
                }
            }
            mAnimator.SetLayerWeight(grip_Value_LayerIndex, grip_Value);

        }
    }
}

