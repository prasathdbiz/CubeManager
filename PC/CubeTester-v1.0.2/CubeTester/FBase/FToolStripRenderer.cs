using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FBase
{
    public class FToolStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            var btn = e.Item as ToolStripButton;
            if (btn != null)
            {
                SolidBrush b = new SolidBrush(btn.BackColor);
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                e.Graphics.FillRectangle(b, bounds);
            }
            else base.OnRenderButtonBackground(e);
        }

        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
        {
            var lbl = e.Item as ToolStripLabel;
            if (lbl != null)
            {
                SolidBrush b = new SolidBrush(lbl.BackColor);
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                e.Graphics.FillRectangle(b, bounds);
            }
            else base.OnRenderLabelBackground(e);
        }
    }
}
