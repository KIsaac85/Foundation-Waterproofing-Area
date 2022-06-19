using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Substructure_Area
{
    class Publisher
    {
        public event EventHandler<EventArgs> ElementSelected;

        protected virtual void OnUserSelection(UserControl1 userControl1)
        {
            if (ElementSelected!=null)
            {
                ElementSelected(this, EventArgs.Empty);
            }
           
        }
        public void Select(UserControl1 userControl1)
        {
            OnUserSelection(userControl1);
        }
    }
    public class SelectionEventArgs : EventArgs
    {
        public UserControl1 usercontrol { get; set; }
    }
}
