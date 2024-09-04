namespace Unity.XR.PICO.TOBSupport
{
    public enum OnLineSystemUpdateStatusCode
    {
        IDLE = 0,
        CHECKING_FOR_UPDATE = 1,
        UPDATE_AVAILABLE = 2,

        DOWNLOADING = 3,
        DOWNLOAD_FINISH = 4,

        UPGRADE_EXTRACTING = 5,
        UPGRADE_VERIFYING = 6,
        UPGRADE_WAITING_REBOOT = 7,
        UPGRADE_FINISH = 8,
    }
}