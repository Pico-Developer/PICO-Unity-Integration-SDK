using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.Rendering;

public class PXR_ObjectTrackingBlock : MonoBehaviour
{
    private Transform objectTrackers;
    private bool updateOT = true;
    private int objectTrackersMaxNum = 3;
    int DeviceCount = 1;
    List<long> trackerIds = new List<long>();
   

    // Start is called before the first frame update
    void Start()
    {
        objectTrackers = transform;
        for (int i = 0; i < objectTrackersMaxNum; i++)
        {
            GameObject ga = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ga.transform.parent = objectTrackers;
            ga.transform.localScale = Vector3.one * 0f;
#if UNITY_6000_0_OR_NEWER
            if (GraphicsSettings.defaultRenderPipeline != null)
#else
            if (GraphicsSettings.renderPipelineAsset != null)
#endif
            {
                Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                Renderer renderer = ga.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = material;
                }
            }
        }
        PXR_MotionTracking.RequestMotionTrackerCompleteAction += RequestMotionTrackerComplete;
        int res = -1;
        res = PXR_MotionTracking.CheckMotionTrackerNumber(MotionTrackerNum.TWO);
        if (res == 0)
        {
            objectTrackers.gameObject.SetActive(true);
           
        }
    }
    private void RequestMotionTrackerComplete(RequestMotionTrackerCompleteEventData obj)
    {
        DeviceCount = (int)obj.trackerCount;
        for (int i = 0; i < DeviceCount; i++)
        {
            trackerIds.Add(obj.trackerIds[i]);
        }
        
        updateOT = true;
    }
    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
       
        for (int i = 0; i < objectTrackersMaxNum; i++)
        {
            var child = objectTrackers.GetChild(i);
            if (child)
            {
                child.localScale = Vector3.zero;
            }
        }

        // Update motiontrackers pose.
        if (updateOT )
        {
            MotionTrackerLocation location = new MotionTrackerLocation();
            for (int i = 0; i < trackerIds.Count; i++)
            {
                bool isValidPose = false;
                int result = PXR_MotionTracking.GetMotionTrackerLocation(trackerIds[i], ref location, ref isValidPose);

                // if the return is successful
                if (result == 0)
                {
                    var child = objectTrackers.GetChild(i);
                    if (child)
                    {
                        child.localPosition = location.pose.Position.ToVector3();
                        child.localRotation = location.pose.Orientation.ToQuat();
                        child.localScale = Vector3.one * 0.1f;
                    }
                }
            }
        }
#endif
    }
}