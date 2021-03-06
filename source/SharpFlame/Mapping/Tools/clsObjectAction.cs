#region

using System.Diagnostics;
using SharpFlame.Collections;
using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Mapping.Tools
{
    public abstract class clsObjectAction : SimpleListTool<clsUnit>
    {
        private readonly SimpleClassList<clsUnit> _ResultUnits = new SimpleClassList<clsUnit>();
        public bool ActionPerformed;
        public clsMap Map;

        protected clsUnit ResultUnit;
        public clsUnit Unit;

        public SimpleClassList<clsUnit> ResultUnits
        {
            get { return _ResultUnits; }
        }

        public void ActionPerform()
        {
            ResultUnit = null;
            ActionPerformed = false;
            if ( Unit == null )
            {
                Debugger.Break();
                return;
            }
            ActionPerformed = true;
            ActionCondition();
            if ( !ActionPerformed )
            {
                return;
            }
            ResultUnit = new clsUnit(Unit, Map);
            _ActionPerform();
            if ( ResultUnit == null )
            {
                ResultUnit = Unit;
            }
            else
            {
                _ResultUnits.Add(ResultUnit);
                Map.UnitSwap(Unit, ResultUnit);
            }
        }

        public void SetItem(clsUnit Item)
        {
            Unit = Item;
        }

        protected virtual void ActionCondition()
        {
        }

        protected abstract void _ActionPerform();
    }
}