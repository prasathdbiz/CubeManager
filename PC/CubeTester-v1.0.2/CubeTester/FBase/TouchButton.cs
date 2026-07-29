using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FBase
{
    internal class TouchButton : Button
    {

        public event EventHandler TouchDown;
        public event EventHandler TouchUp;

        protected virtual void OnTouchDown(EventArgs e)
        {
            EventHandler handler = this.TouchDown;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnTouchUp(EventArgs e)
        {
            EventHandler handler = this.TouchUp;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message msg)
        {
            const int WM_POINTERDOWN = 0x0246;
            const int WM_POINTERUP = 0x247;
            EventArgs e = new EventArgs();
            switch (msg.Msg)
            {
                case WM_POINTERDOWN:
                    OnTouchDown(e);
                    //Global.logger.LogMessage("Info", "Mouse Down");
                    break;
                case WM_POINTERUP:
                    OnTouchUp(e);
                    //Global.logger.LogMessage("Info", "Mouse Up");
                    break;
            }
            base.WndProc(ref msg);
        }
    }
}
