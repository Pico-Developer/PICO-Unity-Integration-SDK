using System.Linq;

namespace Unity.XR.PICO.TOBSupport
{
    public class MarkerInfo
    {
        // position
        public double posX;
        public double posY;
        public double posZ;

        // rotation
        public double rotationX;
        public double rotationY;
        public double rotationZ;
        public double rotationW;

        // 标志位：识别无效=0，识别有效=1
        public int validFlag;

        // 类型：静态=1/动态=0
        public int markerType;

        // marker id
        public int iMarkerId;

        // 检测图像的时间戳
        public double dTimestamp;

        // 预留标志位
        public float[] reserve;

        public override string ToString()
        {
            return $"{nameof(posX)}: {posX}, {nameof(posY)}: {posY}, {nameof(posZ)}: {posZ}, {nameof(rotationX)}: {rotationX}, {nameof(rotationY)}: {rotationY}, {nameof(rotationZ)}: {rotationZ}, {nameof(rotationW)}: {rotationW}, {nameof(validFlag)}: {validFlag}, {nameof(markerType)}: {markerType}, {nameof(iMarkerId)}: {iMarkerId}, {nameof(dTimestamp)}: {dTimestamp}, {nameof(reserve)}: {string.Join(" ", reserve)}";
        }
    }
}