using System.Collections.Generic;

namespace Unity.XR.PICO.TOBSupport
{
    public class LargeSpaceBoundsInfo
    {
        public const int TYPE_IN_SITU_SAFETY_ZONE = 10;
        public const int TYPE_OBSTACLE = 2;
        public const int TYPE_CUSTOMIZE_SECURITY_ZONE = 1;

        public override string ToString()
        {
            return $"LargeSpaceBoundsInfo{{type= {type},  bounds= {string.Join(", ", bounds)} }}";
        }

        private int type;
        private List<Point3D> bounds = new List<Point3D>();

        public List<Point3D> getBounds()
        {
            return this.bounds;
        }

        public void setType(int type)
        {
            this.type = type;
        }

        public int getType()
        {
            return this.type;
        }

        public void addPoint3D(Point3D point3D)
        {
            this.bounds.Add(point3D);
        }
    }
}