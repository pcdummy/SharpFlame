#region

using System.Windows.Forms;

#endregion

namespace SharpFlame
{
    public sealed class modWarnings
    {
        public static ImageList WarningImages = new ImageList();
    }

    public partial class frmWarnings
    {
        public frmWarnings(clsResult result, string windowTitle)
        {
            InitializeComponent();

            Icon = App.ProgramIcon;

            Text = windowTitle;

            tvwWarnings.StateImageList = modWarnings.WarningImages;
            result.MakeNodes(tvwWarnings.Nodes);
            tvwWarnings.ExpandAll();

            tvwWarnings.NodeMouseDoubleClick += NodeDoubleClicked;
        }

        private void NodeDoubleClicked(object sender, TreeNodeMouseClickEventArgs e)
        {
            if ( e.Button != MouseButtons.Left )
            {
                return;
            }
            var item = (clsResultItemInterface)e.Node.Tag;
            if ( item == null )
            {
                return;
            }
            item.DoubleClicked();
        }

        public void frmWarnings_FormClosed(object sender, FormClosedEventArgs e)
        {
            tvwWarnings.NodeMouseDoubleClick -= NodeDoubleClicked;
        }
    }
}