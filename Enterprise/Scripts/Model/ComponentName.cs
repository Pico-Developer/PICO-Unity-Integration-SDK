namespace Unity.XR.PICO.TOBSupport
{
    public class ComponentName
    {
        private string _pkg;
        private string _cls;
        private string _flattenToShortString;
        private string _getShortClassName;
        private string _toShortString;
        private string _toString;

        public ComponentName(string pkg, string cls)
        {
            _pkg = pkg;
            _cls = cls;
        }

        public ComponentName(string pkg, string cls, string flattenToShortString, string getShortClassName, string toShortString, string toString)
        {
            _pkg = pkg;
            _cls = cls;
            _flattenToShortString = flattenToShortString;
            _getShortClassName = getShortClassName;
            _toShortString = toShortString;
            _toString = toString;
        }

        public string getPackageName()
        {
            return _pkg;
        }

        public string getClassName()
        {
            return _cls;
        }

        public string flattenToShortString()
        {
            return _flattenToShortString;
        }

        public string getShortClassName()
        {
            return _getShortClassName;
        }

        public string toShortString()
        {
            return _toShortString;
        }

        public override string ToString()
        {
            return _toString;
        }
    }
}