#region

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NLog;
using SharpFlame.AppSettings;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Maths;
using SharpFlame.Painters;
using SharpFlame.Util;

#endregion

namespace SharpFlame
{
    public sealed class App
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static sRGB_sng MinimapFeatureColour;

        public static char PlatformPathSeparator;

        public static bool Debug_GL = false;

        public static string SettingsPath;
        public static string AutoSavePath;

        public static Random Random;

        public static bool ProgramInitialized = false;
        public static bool ProgramInitializeFinished = false;

        public static Icon ProgramIcon;

        public static SimpleList<string> CommandLinePaths = new SimpleList<string>();

        public static int GLTexture_NoTile;
        public static int GLTexture_OverflowTile;

        public static clsKeysActive IsViewKeyDown = new clsKeysActive();

        public static clsBrush TextureBrush = new clsBrush(0.0D, clsBrush.enumShape.Circle);
        public static clsBrush TerrainBrush = new clsBrush(2.0D, clsBrush.enumShape.Circle);
        public static clsBrush HeightBrush = new clsBrush(2.0D, clsBrush.enumShape.Circle);
        public static clsBrush CliffBrush = new clsBrush(2.0D, clsBrush.enumShape.Circle);

        public static clsBrush SmoothRadius = new clsBrush(1.0D, clsBrush.enumShape.Square);

        public static bool DisplayTileOrientation;

        public static clsObjectData ObjectData;

        public static int SelectedTextureNum = -1;
        public static TileOrientation TextureOrientation = new TileOrientation(false, false, false);

        public static Terrain SelectedTerrain;
        public static Road SelectedRoad;

        public static SimpleList<clsTileType> TileTypes = new SimpleList<clsTileType>();

        public static DroidDesign.clsTemplateDroidType[] TemplateDroidTypes = new DroidDesign.clsTemplateDroidType[0];
        public static int TemplateDroidTypeCount;

        public static readonly UTF8Encoding UTF8Encoding = new UTF8Encoding(false, false);
        public static readonly ASCIIEncoding ASCIIEncoding = new ASCIIEncoding();

        public static int VisionRadius_2E;
        public static double VisionRadius;

        public static clsMap Copied_Map;

        public static SimpleList<clsTileset> Tilesets = new SimpleList<clsTileset>();

        public static clsTileset Tileset_Arizona;
        public static clsTileset Tileset_Urban;
        public static clsTileset Tileset_Rockies;

        public static Painter Painter_Arizona;
        public static Painter Painter_Urban;
        public static Painter Painter_Rockies;

        public static GLFont UnitLabelFont;
        //Public TextureViewFont As GLFont

        public static clsPlayer[] PlayerColour = new clsPlayer[16];

        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Droid;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Cyborg;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_CyborgConstruct;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_CyborgRepair;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_CyborgSuper;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Transporter;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Person;
        public static DroidDesign.clsTemplateDroidType TemplateDroidType_Null;

        public static bool ShowIDErrorMessage = true;

        public static bool Draw_TileTextures = true;

        public static enumDrawLighting Draw_Lighting = enumDrawLighting.Half;
        public static bool Draw_TileWireframe;
        public static bool Draw_Units = true;
        public static bool Draw_VertexTerrain;
        public static bool Draw_Gateways;
        public static bool Draw_ScriptMarkers = true;

        public static enumView_Move_Type ViewMoveType = enumView_Move_Type.RTS;
        public static bool RTSOrbit = true;

        public static Matrix3DMath.Matrix3D SunAngleMatrix = new Matrix3DMath.Matrix3D();
        public static clsBrush VisionSectors = new clsBrush(0.0D, clsBrush.enumShape.Circle);

        public static sLayerList LayerList;

        public static void SetProgramSubDirs()
        {
#if !Portable
            var myDocumentsProgramPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).CombinePathWith(".flaME", true);
            SettingsPath = myDocumentsProgramPath.CombinePathWith("settings.json");
            AutoSavePath = myDocumentsProgramPath.CombinePathWith("autosave", true);

#else
            SettingsPath = AppDomain.CurrentDomain.BaseDirectory.CombinePathWith("settings.json");
            AutoSavePath = AppDomain.CurrentDomain.BaseDirectory.CombinePathWith("autosave", true);
#endif
            // Create the directories.
            if ( !Directory.Exists(AutoSavePath) )
            {
                try
                {
                    Directory.CreateDirectory(AutoSavePath);
                }
                catch ( Exception ex )
                {
                    logger.Error("Unable to create folder \"{0}\": {1}", AutoSavePath, ex.Message);
                    Application.Exit();
                }
            }
        }

        public static void VisionRadius_2E_Changed()
        {
            VisionRadius = 256.0D * Math.Pow(2.0D, (VisionRadius_2E / 2.0D));
            if ( Program.frmMainInstance.MapViewControl != null )
            {
                View_Radius_Set(VisionRadius);
                Program.frmMainInstance.View_DrawViewLater();
            }
        }

        public static string MinDigits(int Number, int Digits)
        {
            var ReturnResult = Number.ToStringInvariant();
            ReturnResult = ReturnResult.PadLeft(Digits, '0');

            return ReturnResult;
        }

        public static void ViewKeyDown_Clear()
        {
            IsViewKeyDown.Deactivate();

            foreach ( Option<KeyboardControl> control in KeyboardManager.OptionsKeyboardControls.Options )
            {
                ((KeyboardControl)(KeyboardManager.KeyboardProfile.get_Value(control))).KeysChanged(IsViewKeyDown);
            }
        }

        public static void CreateTemplateDroidTypes()
        {
            TemplateDroidType_Droid = new DroidDesign.clsTemplateDroidType("Droid", "DROID");
            TemplateDroidType_Droid.Num = TemplateDroidType_Add(TemplateDroidType_Droid);

            TemplateDroidType_Cyborg = new DroidDesign.clsTemplateDroidType("Cyborg", "CYBORG");
            TemplateDroidType_Cyborg.Num = TemplateDroidType_Add(TemplateDroidType_Cyborg);

            TemplateDroidType_CyborgConstruct = new DroidDesign.clsTemplateDroidType("Cyborg Construct", "CYBORG_CONSTRUCT");
            TemplateDroidType_CyborgConstruct.Num = TemplateDroidType_Add(TemplateDroidType_CyborgConstruct);

            TemplateDroidType_CyborgRepair = new DroidDesign.clsTemplateDroidType("Cyborg Repair", "CYBORG_REPAIR");
            TemplateDroidType_CyborgRepair.Num = TemplateDroidType_Add(TemplateDroidType_CyborgRepair);

            TemplateDroidType_CyborgSuper = new DroidDesign.clsTemplateDroidType("Cyborg Super", "CYBORG_SUPER");
            TemplateDroidType_CyborgSuper.Num = TemplateDroidType_Add(TemplateDroidType_CyborgSuper);

            TemplateDroidType_Transporter = new DroidDesign.clsTemplateDroidType("Transporter", "TRANSPORTER");
            TemplateDroidType_Transporter.Num = TemplateDroidType_Add(TemplateDroidType_Transporter);

            TemplateDroidType_Person = new DroidDesign.clsTemplateDroidType("Person", "PERSON");
            TemplateDroidType_Person.Num = TemplateDroidType_Add(TemplateDroidType_Person);

            TemplateDroidType_Null = new DroidDesign.clsTemplateDroidType("Null Droid", "ZNULLDROID");
            TemplateDroidType_Null.Num = TemplateDroidType_Add(TemplateDroidType_Null);
        }

        public static DroidDesign.clsTemplateDroidType GetTemplateDroidTypeFromTemplateCode(string Code)
        {
            var LCaseCode = Code.ToLower();
            var A = 0;

            for ( A = 0; A <= TemplateDroidTypeCount - 1; A++ )
            {
                if ( TemplateDroidTypes[A].TemplateCode.ToLower() == LCaseCode )
                {
                    return TemplateDroidTypes[A];
                }
            }
            return null;
        }

        public static int TemplateDroidType_Add(DroidDesign.clsTemplateDroidType NewDroidType)
        {
            var ReturnResult = 0;

            Array.Resize(ref TemplateDroidTypes, TemplateDroidTypeCount + 1);
            TemplateDroidTypes[TemplateDroidTypeCount] = NewDroidType;
            ReturnResult = TemplateDroidTypeCount;
            TemplateDroidTypeCount++;

            return ReturnResult;
        }

        public static void ShowWarnings(clsResult Result)
        {
            if ( !Result.HasWarnings )
            {
                return;
            }

            var WarningsForm = new frmWarnings(Result, Result.Text);
            WarningsForm.Show();
            WarningsForm.Activate();
        }

        public static enumTurretType GetTurretTypeFromName(string TurretTypeName)
        {
            switch ( TurretTypeName.ToLower() )
            {
                case "weapon":
                    return enumTurretType.Weapon;
                case "construct":
                    return enumTurretType.Construct;
                case "repair":
                    return enumTurretType.Repair;
                case "sensor":
                    return enumTurretType.Sensor;
                case "brain":
                    return enumTurretType.Brain;
                case "ecm":
                    return enumTurretType.ECM;
                default:
                    return enumTurretType.Unknown;
            }
        }

        public static void ErrorIDChange(UInt32 IntendedID, clsUnit IDUnit, string NameOfErrorSource)
        {
            if ( !ShowIDErrorMessage )
            {
                return;
            }

            if ( IDUnit.ID == IntendedID )
            {
                return;
            }

            var messageText = "An object\'s ID has been changed unexpectedly. The error was in \"{0}\"\n\n" +
                              "The object is of type {1} and is at map position {2}. " +
                              "It\'s ID was {3}, but is now {4}.\n\n" +
                              "Click Cancel to stop seeing this message. Otherwise, click OK.".Format2(NameOfErrorSource, IDUnit.TypeBase.GetDisplayTextCode(),
                                  IDUnit.GetPosText(), IntendedID.ToStringInvariant(), IDUnit.ID.ToStringInvariant());
            const string caption = "An object\'s ID has been changed unexpectedly.";

            var result = MessageBox.Show(messageText, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.None);
            if ( result == DialogResult.Cancel )
            {
                ShowIDErrorMessage = false;
            }
        }

        public static void ZeroIDWarning(clsUnit IDUnit, UInt32 NewID, clsResult Output)
        {
            var MessageText = "";

            MessageText = "An object\'s ID has been changed from 0 to " + NewID.ToStringInvariant() + ". Zero is not a valid ID. The object is of type " +
                          IDUnit.TypeBase.GetDisplayTextCode() + " and is at map position " + IDUnit.GetPosText() + ".";

            //MsgBox(MessageText, MsgBoxStyle.OkOnly)
            Output.WarningAdd(MessageText);
        }

        public static bool PosIsWithinTileArea(XYInt worldHorizontal, XYInt startTile, XYInt finishTile)
        {
            return worldHorizontal.X >= startTile.X * Constants.TerrainGridSpacing &
                   worldHorizontal.Y >= startTile.Y * Constants.TerrainGridSpacing &
                   worldHorizontal.X < finishTile.X * Constants.TerrainGridSpacing &
                   worldHorizontal.Y < finishTile.Y * Constants.TerrainGridSpacing;
        }

        public static bool SizeIsPowerOf2(int Size)
        {
            var Power = Math.Log(Size) / Math.Log(2.0D);
            return Power == (int)Power;
        }

        public static clsResult LoadTilesets(string TilesetsPath)
        {
            var ReturnResult = new clsResult("Loading tilesets", false);
            logger.Info("Loading tilesets");

            string[] TilesetDirs = null;
            try
            {
                TilesetDirs = Directory.GetDirectories(TilesetsPath);
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            if ( TilesetDirs == null )
            {
                return ReturnResult;
            }

            var Result = default(clsResult);
            var Path = "";
            var Tileset = default(clsTileset);

            foreach ( var tempLoopVar_Path in TilesetDirs )
            {
                Path = tempLoopVar_Path;
                Tileset = new clsTileset();
                Result = Tileset.LoadDirectory(Path);
                ReturnResult.Add(Result);
                if ( !Result.HasProblems )
                {
                    Tilesets.Add(Tileset);
                }
            }

            foreach ( var tempLoopVar_Tileset in Tilesets )
            {
                Tileset = tempLoopVar_Tileset;
                if ( Tileset.Name == "tertilesc1hw" )
                {
                    Tileset.Name = "Arizona";
                    Tileset_Arizona = Tileset;
                    Tileset.IsOriginal = true;
                    Tileset.BGColour = new sRGB_sng(204.0f / 255.0f, 149.0f / 255.0f, 70.0f / 255.0f);
                }
                else if ( Tileset.Name == "tertilesc2hw" )
                {
                    Tileset.Name = "Urban";
                    Tileset_Urban = Tileset;
                    Tileset.IsOriginal = true;
                    Tileset.BGColour = new sRGB_sng(118.0f / 255.0f, 165.0f / 255.0f, 203.0f / 255.0f);
                }
                else if ( Tileset.Name == "tertilesc3hw" )
                {
                    Tileset.Name = "Rocky Mountains";
                    Tileset_Rockies = Tileset;
                    Tileset.IsOriginal = true;
                    Tileset.BGColour = new sRGB_sng(182.0f / 255.0f, 225.0f / 255.0f, 236.0f / 255.0f);
                }
            }

            if ( Tileset_Arizona == null )
            {
                ReturnResult.WarningAdd("Arizona tileset is missing.");
            }
            if ( Tileset_Urban == null )
            {
                ReturnResult.WarningAdd("Urban tileset is missing.");
            }
            if ( Tileset_Rockies == null )
            {
                ReturnResult.WarningAdd("Rocky Mountains tileset is missing.");
            }

            return ReturnResult;
        }

        public static void View_Radius_Set(double Radius)
        {
            VisionSectors.Radius = Radius / (Constants.TerrainGridSpacing * Constants.SectorTileSize);
        }

        public static XYDouble CalcUnitsCentrePos(SimpleList<clsUnit> Units)
        {
            var Result = default(XYDouble);

            Result.X = 0.0D;
            Result.Y = 0.0D;
            var Unit = default(clsUnit);
            foreach ( var tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                Result += Unit.Pos.Horizontal.ToDoubles();
            }
            Result /= Units.Count;

            return Result;
        }
    }
}