#region

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using SharpFlame.AppSettings;
using Timer = System.Windows.Forms.Timer;

#endregion

namespace SharpFlame
{
    public sealed class Program
    {
        public static Timer InitializeDelay;
        public static clsResult InitializeResult = new clsResult("Startup result", false);

        public static frmMain frmMainInstance;
        public static frmGenerator frmGeneratorInstance;
        public static frmOptions frmOptionsInstance;

        public static GLControl OpenGL1;
        public static GLControl OpenGL2;

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();

            App.PlatformPathSeparator = Path.DirectorySeparatorChar;
            App.SetProgramSubDirs();

            SettingsManager.CreateSettingOptions();
            KeyboardManager.CreateControls(); //needed to load key control settings

            try
            {
                Toolkit.Init();
            }
            catch
            {
            }

            var SettingsLoadResult = SettingsManager.Settings_Load(ref SettingsManager.InitializeSettings);        

            InitializeResult.Add(SettingsLoadResult);


            OpenGL1 =
                new GLControl(new GraphicsMode(new ColorFormat(SettingsManager.InitializeSettings.MapViewBPP), SettingsManager.InitializeSettings.MapViewDepth, 0));
            OpenGL2 =
                new GLControl(new GraphicsMode(new ColorFormat(SettingsManager.InitializeSettings.TextureViewBPP), SettingsManager.InitializeSettings.TextureViewDepth,
                    0));

            App.Random = new Random();

            while ( OpenGL1.Context == null || OpenGL2.Context == null )
            {
                //todo, why is this needed
            }

            frmMainInstance = new frmMain();
            frmMainInstance.FormClosing += frmMainInstance.frmMain_FormClosing;
            frmMainInstance.DragEnter += frmMainInstance.OpenGL_DragEnter;
            frmMainInstance.DragDrop += frmMainInstance.OpenGL_DragDrop;

            try
            {
                App.ProgramIcon = Resources.flaME;
            }
            catch ( Exception ex )
            {
                InitializeResult.WarningAdd(Constants.ProgramName + " icon is missing: " + ex.Message);
            }
            frmMainInstance.Icon = App.ProgramIcon;
            frmGeneratorInstance.Icon = App.ProgramIcon;

            InitializeDelay = new Timer();
            InitializeDelay.Tick += frmMainInstance.Initialize;
            InitializeDelay.Interval = 50;
            InitializeDelay.Enabled = true;

            while ( !App.ProgramInitializeFinished )
            {
                Thread.Sleep(50);
                Application.DoEvents();
            }

            Application.Run(frmMainInstance);
        }
    }
}