using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.XR.PXR
{
    /// <summary>
    /// PXR_ Utility Class contains helper methods that any script can use.
    /// </summary>
    public static class PXR_Utility
    {
        /// <summary>
        /// Computes the inverse of the given pose.
        /// </summary>
        private static Pose Inverse(Pose p)
        {
            Pose ret;
            ret.rotation = Quaternion.Inverse(p.rotation);
            ret.position = ret.rotation * -p.position;
            return ret;
        }

        /// <summary>
        /// Recalculate object position and rotation from tracking-space to world-space, for use cases like teleporting.
        /// </summary>
        /// <param name="t">original transform of the object in the scene, typically obtained by gameObject.transform</param>
        /// <param name="camera">camera the calculation is based on, normally it is the main camera</param>
        /// <returns>the recalculated pose <see cref="UnityEngine.Pose"/> in world-space.</returns>
        public static Pose ComputePoseToWorldSpace(Transform t, Camera camera)
        {
            if (camera == null)
                return default;

            Transform cameraTransform = camera.transform;
            Pose headPose = new Pose(cameraTransform.localPosition, cameraTransform.localRotation);
            Pose camPose = new Pose(cameraTransform.position, cameraTransform.rotation);
            Pose transformPose = new Pose(t.position, t.rotation);

            Pose headSpacePose = transformPose.GetTransformedBy(Inverse(camPose));
            return headSpacePose.GetTransformedBy(headPose);
        }

        /// <summary>
        /// Returns if the current session is in the focused state.
        /// See <a href="https://registry.khronos.org/PXR_/specs/1.0/html/xrspec.html#session-states">XR_SESSION_STATE_FOCUSED.</a> for reference.
        /// </summary>
        public static bool IsSessionFocused => Internal_IsSessionFocused();
        /// <summary>
        ///  Returns the change of user presence, such as when the user has taken off or put on an XR headset.
        ///  If the system does not support user presence sensing, runtime assumes that the user is always present and IsUserPresent always returns True.
        ///  If the system supports the sensing of user presence, returns true when detected the presence of a user and returns false when detected the absence of a user.
        ///  See <a href="https://registry.khronos.org/PXR_/specs/1.0/html/xrspec.html#XR_EXT_user_presence">XR_EXT_user_presence.</a> for reference.
        /// </summary>
        public static bool IsUserPresent => Internal_GetUserPresence();

        private const string LibraryName = "PxrPlatform";

        [DllImport(LibraryName, EntryPoint = "NativeConfig_IsSessionFocused")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool Internal_IsSessionFocused();

        [DllImport(LibraryName, EntryPoint = "NativeConfig_GetUserPresence")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool Internal_GetUserPresence();
    }
}
