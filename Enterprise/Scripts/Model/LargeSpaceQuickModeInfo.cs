namespace Unity.XR.PICO.TOBSupport
{
    public class LargeSpaceQuickModeInfo
    {
        public bool status;
        public int length;
        public int width;
        public int originType;

        public LargeSpaceQuickModeInfo()
        {
        }

        public override string ToString()
        {
            return
                $"{nameof(status)}: {status}, {nameof(length)}: {length}, {nameof(width)}: {width}, {nameof(originType)}: {originType}";
        }
    }
}