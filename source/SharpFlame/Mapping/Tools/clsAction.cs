#region

using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public abstract class clsAction
    {
        public double Effect;
        public clsMap Map;
        public XYInt PosNum;
        public bool UseEffect;

        public clsAction()
        {
            PosNum = new XYInt(0, 0);
        }

        public abstract void ActionPerform();
    }
}