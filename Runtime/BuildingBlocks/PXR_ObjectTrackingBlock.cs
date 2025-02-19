using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.Rendering;

public class PXR_ObjectTrackingBlock : MonoBehaviour
{
    private Transform objectTrackers;
    private bool updateOT = true;
    private int objectTrackersMaxNum = 3;

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

        int res = -1;
        res = PXR_MotionTracking.CheckMotionTrackerModeAndNumber(MotionTrackerMode.MotionTracking);
        if (res == 0)
        {
            objectTrackers.gameObject.SetActive(true);
            updateOT = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        // Get the current tracking mode, either bodytracking or motiontracking.
        MotionTrackerMode trackingMode = PXR_MotionTracking.GetMotionTrackerMode();
        if (trackingMode != MotionTrackerMode.MotionTracking)
        {
            PXR_MotionTracking.CheckMotionTrackerModeAndNumber(MotionTrackerMode.MotionTracking);
        }

        for (int i = 0; i < objectTrackersMaxNum; i++)
        {
            var child = objectTrackers.GetChild(i);
            if (child)
            {
                child.localScale = Vector3.zero;
            }
        }

        // Update motiontrackers pose.
        if (updateOT && trackingMode == MotionTrackerMode.MotionTracking)
        {
            // Get the serial numbers of the motion trackers to identify them.
            MotionTrackerConnectState mtcs = new MotionTrackerConnectState();
            int ret = PXR_MotionTracking.GetMotionTrackerConnectStateWithSN(ref mtcs);

            if (ret == 0)
            {
                if (mtcs.trackersSN.Length > 0)
                {
                    for (int i = 0; i < mtcs.trackerSum; i++)
                    {
                        string sn = mtcs.trackersSN[i].value.ToString().Trim();
                        if (!string.IsNullOrEmpty(sn))
                        {
                            // Get the position and rotation estimates of each tracker.
                            MotionTrackerLocations locations = new MotionTrackerLocations();
                            MotionTrackerConfidence confidence = new MotionTrackerConfidence();
                            int result = PXR_MotionTracking.GetMotionTrackerLocations(mtcs.trackersSN[i], ref locations, ref confidence);

                            // if the return is successful
                            if (result == 0)
                            {
                                MotionTrackerLocation localLocation = locations.localLocation;
                                var child = objectTrackers.GetChild(i);
                                if (child)
                                {
                                    child.localPosition = localLocation.pose.Position.ToVector3();
                                    child.localRotation = localLocation.pose.Orientation.ToQuat();
                                    child.localScale = Vector3.one * 0.1f;
                                }
                            }
                        }
                    }
                }
            }
        }
#endif
    }
}