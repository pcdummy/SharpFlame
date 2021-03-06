﻿#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SharpFlame.AppSettings;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using SharpFlame.Domain;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Tools;
using SharpFlame.Maths;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Controls
{
    public partial class MapViewControl
    {
        private readonly ContextMenuStrip ListSelect;
        private readonly frmMain _Owner;
        private readonly Timer tmrDraw;
        private readonly Timer tmrDrawDelay;

        public bool DrawPending;

        //public float GLSize_XPerY; //seems redundant, since OpenGLControl has a field called AspectRatio

        public bool DrawView_Enabled = false;

        private Timer GLInitializeDelayTimer;
        public XYInt GLSize;
        public bool IsGLInitialized = false;
        private bool ListSelectIsPicker;
        private ToolStripItem[] ListSelectItems = new ToolStripItem[0];

        public GLControl OpenGLControl;
        public Timer UndoMessageTimer;

        public MapViewControl(frmMain Owner)
        {
            _Owner = Owner;

            GLSize = new XYInt(0, 0);

            InitializeComponent();

            ListSelect = new ContextMenuStrip();
            ListSelect.ItemClicked += ListSelect_Click;
            ListSelect.Closed += ListSelect_Close;
            UndoMessageTimer = new Timer();
            UndoMessageTimer.Tick += RemoveUndoMessage;

            OpenGLControl = Program.OpenGL1;
            pnlDraw.Controls.Add(OpenGLControl);

            GLInitializeDelayTimer = new Timer();
            GLInitializeDelayTimer.Interval = 50;
            GLInitializeDelayTimer.Tick += GLInitialize;
            GLInitializeDelayTimer.Enabled = true;

            tmrDraw = new Timer();
            tmrDraw.Tick += tmrDraw_Tick;
            tmrDraw.Interval = 1;

            tmrDrawDelay = new Timer();
            tmrDrawDelay.Tick += tmrDrawDelay_Tick;
            tmrDrawDelay.Interval = 30;

            UndoMessageTimer.Interval = 4000;
        }

        private clsMap MainMap
        {
            get { return _Owner.MainMap; }
        }

        public void ResizeOpenGL()
        {
            if ( OpenGLControl.Context == null )
            {
                return;
            }

            OpenGLControl.Width = pnlDraw.Width;
            OpenGLControl.Height = pnlDraw.Height;
        }

        public void DrawView_SetEnabled(bool Value)
        {
            if ( Value )
            {
                if ( !DrawView_Enabled )
                {
                    DrawView_Enabled = true;
                    DrawViewLater();
                }
            }
            else
            {
                tmrDraw.Enabled = false;
                DrawView_Enabled = false;
            }
        }

        public void DrawViewLater()
        {
            DrawPending = true;
            if ( !tmrDrawDelay.Enabled )
            {
                tmrDraw.Enabled = true;
            }
        }

        private void tmrDraw_Tick(Object sender, EventArgs e)
        {
            tmrDraw.Enabled = false;
            if ( DrawPending )
            {
                DrawView();
                DrawPending = false;
                tmrDrawDelay.Enabled = true;
            }
        }

        private void GLInitialize(object sender, EventArgs e)
        {
            if ( OpenGLControl.Context == null )
            {
                return;
            }

            GLInitializeDelayTimer.Enabled = false;
            GLInitializeDelayTimer.Tick -= GLInitialize;
            GLInitializeDelayTimer.Dispose();
            GLInitializeDelayTimer = null;

            ResizeOpenGL();

            OpenGLControl.MouseDown += OpenGL_MouseDown;
            OpenGLControl.MouseUp += OpenGL_MouseUp;
            OpenGLControl.MouseWheel += OpenGL_MouseWheel;
            OpenGLControl.MouseMove += OpenGL_MouseMove;
            OpenGLControl.MouseEnter += OpenGL_MouseEnter;
            OpenGLControl.MouseLeave += OpenGL_MouseLeave;
            OpenGLControl.Resize += OpenGL_Resize;
            OpenGLControl.Leave += OpenGL_LostFocus;
            OpenGLControl.PreviewKeyDown += OpenGL_KeyDown;
            OpenGLControl.KeyUp += OpenGL_KeyUp;

            if ( GraphicsContext.CurrentContext != OpenGLControl.Context )
            {
                OpenGLControl.MakeCurrent();
            }

            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.ClearColor(0.0F, 0.0F, 0.0F, 1.0F);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);

            var ambient = new float[4];
            var specular = new float[4];
            var diffuse = new float[4];

            ambient[0] = 0.333333343F;
            ambient[1] = 0.333333343F;
            ambient[2] = 0.333333343F;
            ambient[3] = 1.0F;
            specular[0] = 0.6666667F;
            specular[1] = 0.6666667F;
            specular[2] = 0.6666667F;
            specular[3] = 1.0F;
            diffuse[0] = 0.75F;
            diffuse[1] = 0.75F;
            diffuse[2] = 0.75F;
            diffuse[3] = 1.0F;
            GL.Light(LightName.Light0, LightParameter.Diffuse, diffuse);
            GL.Light(LightName.Light0, LightParameter.Specular, specular);
            GL.Light(LightName.Light0, LightParameter.Ambient, ambient);

            ambient[0] = 0.25F;
            ambient[1] = 0.25F;
            ambient[2] = 0.25F;
            ambient[3] = 1.0F;
            specular[0] = 0.5F;
            specular[1] = 0.5F;
            specular[2] = 0.5F;
            specular[3] = 1.0F;
            diffuse[0] = 0.5625F;
            diffuse[1] = 0.5625F;
            diffuse[2] = 0.5625F;
            diffuse[3] = 1.0F;
            GL.Light(LightName.Light1, LightParameter.Diffuse, diffuse);
            GL.Light(LightName.Light1, LightParameter.Specular, specular);
            GL.Light(LightName.Light1, LightParameter.Ambient, ambient);

            var mat_diffuse = new float[4];
            var mat_specular = new float[4];
            var mat_ambient = new float[4];
            var mat_shininess = new float[1];

            mat_specular[0] = 0.0F;
            mat_specular[1] = 0.0F;
            mat_specular[2] = 0.0F;
            mat_specular[3] = 0.0F;
            mat_ambient[0] = 1.0F;
            mat_ambient[1] = 1.0F;
            mat_ambient[2] = 1.0F;
            mat_ambient[3] = 1.0F;
            mat_diffuse[0] = 1.0F;
            mat_diffuse[1] = 1.0F;
            mat_diffuse[2] = 1.0F;
            mat_diffuse[3] = 1.0F;
            mat_shininess[0] = 0.0F;

            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, mat_ambient);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, mat_specular);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, mat_diffuse);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, mat_shininess);

            IsGLInitialized = true;
        }

        public void Viewport_Resize()
        {
            if ( !App.ProgramInitialized )
            {
                return;
            }

            if ( GraphicsContext.CurrentContext != OpenGLControl.Context )
            {
                OpenGLControl.MakeCurrent();
            }
            GL.Viewport(0, 0, GLSize.X, GLSize.Y);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Flush();
            OpenGLControl.SwapBuffers();
            Refresh();

            DrawViewLater();
        }

        private void DrawView()
        {
            if ( !(DrawView_Enabled && IsGLInitialized) )
            {
                return;
            }

            if ( GraphicsContext.CurrentContext != OpenGLControl.Context )
            {
                OpenGLControl.MakeCurrent();
            }

            var Map = MainMap;
            var BGColour = new sRGB_sng();

            if ( Map == null )
            {
                BGColour.Red = 0.5F;
                BGColour.Green = 0.5F;
                BGColour.Blue = 0.5F;
            }
            else if ( Map.Tileset == null )
            {
                BGColour.Red = 0.5F;
                BGColour.Green = 0.5F;
                BGColour.Blue = 0.5F;
            }
            else
            {
                BGColour = Map.Tileset.BGColour;
            }

            GL.ClearColor(BGColour.Red, BGColour.Green, BGColour.Blue, 1.0F);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if ( Map != null )
            {
                Map.GLDraw();
            }

            GL.Flush();
            OpenGLControl.SwapBuffers();

            Refresh();
        }

        public void OpenGL_MouseMove(object sender, MouseEventArgs e)
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            Map.ViewInfo.Map.ViewInfo.MouseOver = new clsViewInfo.clsMouseOver();
            Map.ViewInfo.MouseOver.ScreenPos.X = e.X;
            Map.ViewInfo.MouseOver.ScreenPos.Y = e.Y;

            Map.ViewInfo.MouseOver_Pos_Calc();
        }

        public void Pos_Display_Update()
        {
            var Map = MainMap;
            var MouseOverTerrain = Map.ViewInfo.GetMouseOverTerrain();

            if ( MouseOverTerrain == null )
            {
                lblTile.Text = "";
                lblVertex.Text = "";
                lblPos.Text = "";
            }
            else
            {
                lblTile.Text = "Tile x:" + Convert.ToString(MouseOverTerrain.Tile.Normal.X) + ", y:" + Convert.ToString(MouseOverTerrain.Tile.Normal.Y);
                lblVertex.Text = "Vertex  x:" + Convert.ToString(MouseOverTerrain.Vertex.Normal.X) + ", y:" +
                                 Convert.ToString(MouseOverTerrain.Vertex.Normal.Y) + ", alt:" +
                                 Map.Terrain.Vertices[MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y].Height * Map.HeightMultiplier + " (" +
                                 Convert.ToString(Map.Terrain.Vertices[MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y].Height) + "x" +
                                 Convert.ToString(Map.HeightMultiplier) + ")";
                lblPos.Text = "Pos x:" + Convert.ToString(MouseOverTerrain.Pos.Horizontal.X) + ", y:" +
                              Convert.ToString(MouseOverTerrain.Pos.Horizontal.Y) + ", alt:" + Convert.ToString(MouseOverTerrain.Pos.Altitude) +
                              ", slope: " +
                              Convert.ToString(Math.Round(Map.GetTerrainSlopeAngle(MouseOverTerrain.Pos.Horizontal) / MathUtil.RadOf1Deg * 10.0D) / 10.0D) +
                              "°";
            }
        }

        public void OpenGL_LostFocus(Object eventSender, EventArgs eventArgs)
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            Map.SuppressMinimap = false;

            Map.ViewInfo.MouseOver = null;
            Map.ViewInfo.MouseLeftDown = null;
            Map.ViewInfo.MouseRightDown = null;

            App.ViewKeyDown_Clear();
        }

        private void ListSelect_Click(object Sender, ToolStripItemClickedEventArgs e)
        {
            var Button = e.ClickedItem;
            var Unit = (clsUnit)Button.Tag;

            if ( ListSelectIsPicker )
            {
                Program.frmMainInstance.ObjectPicker(Unit.TypeBase);
            }
            else
            {
                if ( Unit.MapSelectedUnitLink.IsConnected )
                {
                    Unit.MapDeselect();
                }
                else
                {
                    Unit.MapSelect();
                }
                Program.frmMainInstance.SelectedObject_Changed();
                DrawViewLater();
            }
        }

        private void ListSelect_Close(object sender, ToolStripDropDownClosedEventArgs e)
        {
            var A = 0;

            for ( A = 0; A <= ListSelectItems.GetUpperBound(0); A++ )
            {
                ListSelectItems[A].Tag = null;
                ListSelectItems[A].Dispose();
            }
            ListSelect.Items.Clear();
            ListSelectItems = new ToolStripItem[0];

            App.ViewKeyDown_Clear();
        }

        private void OpenGL_MouseDown(object sender, MouseEventArgs e)
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            Map.ViewInfo.MouseDown(e);
        }

        private void OpenGL_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            var matrixA = new Matrix3DMath.Matrix3D();
            var MouseOverTerrain = Map.ViewInfo.GetMouseOverTerrain();

            App.IsViewKeyDown.Keys[(int)e.KeyCode] = true;

            foreach ( Option<KeyboardControl> control in KeyboardManager.OptionsKeyboardControls.Options )
            {
                ((KeyboardControl)(KeyboardManager.KeyboardProfile.get_Value(control))).KeysChanged(App.IsViewKeyDown);
            }

            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Undo) )
            {
                var Message = "";
                if ( Map.UndoPosition > 0 )
                {
                    Message = "Undid: " + Map.Undos[Map.UndoPosition - 1].Name;
                    var MapMessage = new clsMessage();
                    MapMessage.Text = Message;
                    Map.Messages.Add(MapMessage);
                    Map.UndoPerform();
                    DrawViewLater();
                }
                else
                {
                    Message = "Nothing to undo";
                }
                DisplayUndoMessage(Message);
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Redo) )
            {
                var Message = "";
                if ( Map.UndoPosition < Map.Undos.Count )
                {
                    Message = "Redid: " + Map.Undos[Map.UndoPosition].Name;
                    var MapMessage = new clsMessage();
                    MapMessage.Text = Message;
                    Map.Messages.Add(MapMessage);
                    Map.RedoPerform();
                    DrawViewLater();
                }
                else
                {
                    Message = "Nothing to redo";
                }
                DisplayUndoMessage(Message);
            }
            if ( App.IsViewKeyDown.Keys[(int)Keys.ControlKey] )
            {
                if ( e.KeyCode == Keys.D1 )
                {
                    App.VisionRadius_2E = 6;
                }
                else if ( e.KeyCode == Keys.D2 )
                {
                    App.VisionRadius_2E = 7;
                }
                else if ( e.KeyCode == Keys.D3 )
                {
                    App.VisionRadius_2E = 8;
                }
                else if ( e.KeyCode == Keys.D4 )
                {
                    App.VisionRadius_2E = 9;
                }
                else if ( e.KeyCode == Keys.D5 )
                {
                    App.VisionRadius_2E = 10;
                }
                else if ( e.KeyCode == Keys.D6 )
                {
                    App.VisionRadius_2E = 11;
                }
                else if ( e.KeyCode == Keys.D7 )
                {
                    App.VisionRadius_2E = 12;
                }
                else if ( e.KeyCode == Keys.D8 )
                {
                    App.VisionRadius_2E = 13;
                }
                else if ( e.KeyCode == Keys.D9 )
                {
                    App.VisionRadius_2E = 14;
                }
                else if ( e.KeyCode == Keys.D0 )
                {
                    App.VisionRadius_2E = 15;
                }
                App.VisionRadius_2E_Changed();
            }

            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewMoveType) )
            {
                if ( App.ViewMoveType == enumView_Move_Type.Free )
                {
                    App.ViewMoveType = enumView_Move_Type.RTS;
                }
                else if ( App.ViewMoveType == enumView_Move_Type.RTS )
                {
                    App.ViewMoveType = enumView_Move_Type.Free;
                }
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewRotateType) )
            {
                App.RTSOrbit = !App.RTSOrbit;
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewReset) )
            {
                Map.ViewInfo.FOV_Multiplier_Set(SettingsManager.Settings.FOVDefault);
                if ( App.ViewMoveType == enumView_Move_Type.Free )
                {
                    Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
                    Map.ViewInfo.ViewAngleSet_Rotate(matrixA);
                }
                else if ( App.ViewMoveType == enumView_Move_Type.RTS )
                {
                    Matrix3DMath.MatrixSetToXAngle(matrixA, Math.Atan(2.0D));
                    Map.ViewInfo.ViewAngleSet_Rotate(matrixA);
                }
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewTextures) )
            {
                App.Draw_TileTextures = !App.Draw_TileTextures;
                DrawViewLater();
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewWireframe) )
            {
                App.Draw_TileWireframe = !App.Draw_TileWireframe;
                DrawViewLater();
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewUnits) )
            {
                App.Draw_Units = !App.Draw_Units;
                var X = 0;
                var Y = 0;
                var SectorNum = new XYInt();
                var Unit = default(clsUnit);
                var Connection = default(clsUnitSectorConnection);
                for ( Y = 0; Y <= Map.SectorCount.Y - 1; Y++ )
                {
                    for ( X = 0; X <= Map.SectorCount.X - 1; X++ )
                    {
                        foreach ( var tempLoopVar_Connection in Map.Sectors[X, Y].Units )
                        {
                            Connection = tempLoopVar_Connection;
                            Unit = Connection.Unit;
                            if ( Unit.TypeBase.Type == UnitType.PlayerStructure )
                            {
                                if ( ((StructureTypeBase)Unit.TypeBase).StructureBasePlate != null )
                                {
                                    SectorNum.X = X;
                                    SectorNum.Y = Y;
                                    Map.SectorGraphicsChanges.Changed(SectorNum);
                                    break;
                                }
                            }
                        }
                    }
                }
                Map.Update();
                DrawViewLater();
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewScriptMarkers) )
            {
                App.Draw_ScriptMarkers = !App.Draw_ScriptMarkers;
                DrawViewLater();
            }
            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.ViewLighting) )
            {
                if ( App.Draw_Lighting == enumDrawLighting.Off )
                {
                    App.Draw_Lighting = enumDrawLighting.Half;
                }
                else if ( App.Draw_Lighting == enumDrawLighting.Half )
                {
                    App.Draw_Lighting = enumDrawLighting.Normal;
                }
                else if ( App.Draw_Lighting == enumDrawLighting.Normal )
                {
                    App.Draw_Lighting = enumDrawLighting.Off;
                }
                DrawViewLater();
            }
            if ( modTools.Tool == modTools.Tools.TextureBrush )
            {
                if ( MouseOverTerrain != null )
                {
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Clockwise) )
                    {
                        Map.ViewInfo.Apply_Texture_Clockwise();
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.CounterClockwise) )
                    {
                        Map.ViewInfo.Apply_Texture_CounterClockwise();
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.TextureFlip) )
                    {
                        Map.ViewInfo.Apply_Texture_FlipX();
                    }
                    if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.TriFlip) )
                    {
                        Map.ViewInfo.Apply_Tri_Flip();
                    }
                }
            }
            if ( modTools.Tool == modTools.Tools.ObjectSelect )
            {
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.UnitDelete) )
                {
                    if ( Map.SelectedUnits.Count > 0 )
                    {
                        var Unit = default(clsUnit);
                        foreach ( var tempLoopVar_Unit in Map.SelectedUnits.GetItemsAsSimpleList() )
                        {
                            Unit = tempLoopVar_Unit;
                            Map.UnitRemoveStoreChange(Unit.MapLink.ArrayPosition);
                        }
                        Program.frmMainInstance.SelectedObject_Changed();
                        Map.UndoStepCreate("Object Deleted");
                        Map.Update();
                        Map.MinimapMakeLater();
                        DrawViewLater();
                    }
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.UnitMove) )
                {
                    if ( MouseOverTerrain != null )
                    {
                        if ( Map.SelectedUnits.Count > 0 )
                        {
                            var Centre = App.CalcUnitsCentrePos(Map.SelectedUnits.GetItemsAsSimpleList());
                            var Offset = new XYInt();
                            Offset.X = ((int)(Math.Round(Convert.ToDouble((MouseOverTerrain.Pos.Horizontal.X - Centre.X) / Constants.TerrainGridSpacing)))) *
                                       Constants.TerrainGridSpacing;
                            Offset.Y = ((int)(Math.Round(Convert.ToDouble((MouseOverTerrain.Pos.Horizontal.Y - Centre.Y) / Constants.TerrainGridSpacing)))) *
                                       Constants.TerrainGridSpacing;
                            var ObjectPosOffset = new clsObjectPosOffset();
                            ObjectPosOffset.Map = Map;
                            ObjectPosOffset.Offset = Offset;
                            Map.SelectedUnitsAction(ObjectPosOffset);

                            Map.UndoStepCreate("Objects Moved");
                            Map.Update();
                            Map.MinimapMakeLater();
                            Program.frmMainInstance.SelectedObject_Changed();
                            DrawViewLater();
                        }
                    }
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Clockwise) )
                {
                    var ObjectRotationOffset = new clsObjectRotationOffset();
                    ObjectRotationOffset.Map = Map;
                    ObjectRotationOffset.Offset = -90;
                    Map.SelectedUnitsAction(ObjectRotationOffset);
                    Map.Update();
                    Program.frmMainInstance.SelectedObject_Changed();
                    Map.UndoStepCreate("Object Rotated");
                    DrawViewLater();
                }
                if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.CounterClockwise) )
                {
                    var ObjectRotationOffset = new clsObjectRotationOffset();
                    ObjectRotationOffset.Map = Map;
                    ObjectRotationOffset.Offset = 90;
                    Map.SelectedUnitsAction(ObjectRotationOffset);
                    Map.Update();
                    Program.frmMainInstance.SelectedObject_Changed();
                    Map.UndoStepCreate("Object Rotated");
                    DrawViewLater();
                }
            }

            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.Deselect) )
            {
                modTools.Tool = modTools.Tools.ObjectSelect;
                DrawViewLater();
            }

            if ( KeyboardManager.KeyboardProfile.Active(KeyboardManager.PreviousTool) )
            {
                modTools.Tool = modTools.PreviousTool;
                DrawViewLater();
            }
        }

        private void OpenGL_KeyUp(object sender, KeyEventArgs e)
        {
            App.IsViewKeyDown.Keys[(int)e.KeyCode] = false;

            foreach ( Option<KeyboardControl> control in KeyboardManager.OptionsKeyboardControls.Options )
            {
                ((KeyboardControl)(KeyboardManager.KeyboardProfile.get_Value(control))).KeysChanged(App.IsViewKeyDown);
            }
        }

        private void OpenGL_MouseUp(object sender, MouseEventArgs e)
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            var MouseOverTerrain = Map.ViewInfo.GetMouseOverTerrain();

            Map.SuppressMinimap = false;

            if ( e.Button == MouseButtons.Left )
            {
                if ( Map.ViewInfo.GetMouseLeftDownOverMinimap() != null )
                {
                }
                else
                {
                    if ( modTools.Tool == modTools.Tools.TerrainBrush )
                    {
                        Map.UndoStepCreate("Ground Painted");
                    }
                    else if ( modTools.Tool == modTools.Tools.CliffTriangle )
                    {
                        Map.UndoStepCreate("Cliff Triangles");
                    }
                    else if ( modTools.Tool == modTools.Tools.CliffBrush )
                    {
                        Map.UndoStepCreate("Cliff Brush");
                    }
                    else if ( modTools.Tool == modTools.Tools.CliffRemove )
                    {
                        Map.UndoStepCreate("Cliff Remove Brush");
                    }
                    else if ( modTools.Tool == modTools.Tools.HeightChangeBrush )
                    {
                        Map.UndoStepCreate("Height Change");
                    }
                    else if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                    {
                        Map.UndoStepCreate("Height Set");
                    }
                    else if ( modTools.Tool == modTools.Tools.HeightSmoothBrush )
                    {
                        Map.UndoStepCreate("Height Smooth");
                    }
                    else if ( modTools.Tool == modTools.Tools.TextureBrush )
                    {
                        Map.UndoStepCreate("Texture");
                    }
                    else if ( modTools.Tool == modTools.Tools.RoadRemove )
                    {
                        Map.UndoStepCreate("Road Remove");
                    }
                    else if ( modTools.Tool == modTools.Tools.ObjectSelect )
                    {
                        if ( Map.Unit_Selected_Area_VertexA != null )
                        {
                            if ( MouseOverTerrain != null )
                            {
                                SelectUnits(Map.Unit_Selected_Area_VertexA, MouseOverTerrain.Vertex.Normal);
                            }
                            Map.Unit_Selected_Area_VertexA = null;
                        }
                    }
                }
                Map.ViewInfo.MouseLeftDown = null;
            }
            else if ( e.Button == MouseButtons.Right )
            {
                if ( Map.ViewInfo.GetMouseRightDownOverMinimap() != null )
                {
                }
                else
                {
                    if ( modTools.Tool == modTools.Tools.HeightChangeBrush )
                    {
                        Map.UndoStepCreate("Height Change");
                    }
                    else if ( modTools.Tool == modTools.Tools.HeightSetBrush )
                    {
                        Map.UndoStepCreate("Height Set");
                    }
                }
                Map.ViewInfo.MouseRightDown = null;
            }
        }

        private void SelectUnits(XYInt VertexA, XYInt VertexB)
        {
            var Map = MainMap;
            var MouseOverTerrain = Map.ViewInfo.GetMouseOverTerrain();
            var SectorNum = new XYInt();
            var Unit = default(clsUnit);
            var SectorStart = new XYInt();
            var SectorFinish = new XYInt();
            var StartPos = new XYInt();
            var FinishPos = new XYInt();
            var StartVertex = new XYInt();
            var FinishVertex = new XYInt();

            if ( Math.Abs(VertexA.X - VertexB.X) <= 1 &&
                 Math.Abs(VertexA.Y - VertexB.Y) <= 1 &&
                 MouseOverTerrain != null )
            {
                if ( MouseOverTerrain.Units.Count > 0 )
                {
                    if ( MouseOverTerrain.Units.Count == 1 )
                    {
                        Unit = MouseOverTerrain.Units[0];
                        if ( Unit.MapSelectedUnitLink.IsConnected )
                        {
                            Unit.MapDeselect();
                        }
                        else
                        {
                            Unit.MapSelect();
                        }
                    }
                    else
                    {
                        ListSelectBegin(false);
                    }
                }
            }
            else
            {
                MathUtil.ReorderXY(VertexA, VertexB, ref StartVertex, ref FinishVertex);
                StartPos.X = StartVertex.X * Constants.TerrainGridSpacing;
                StartPos.Y = StartVertex.Y * Constants.TerrainGridSpacing;
                FinishPos.X = FinishVertex.X * Constants.TerrainGridSpacing;
                FinishPos.Y = FinishVertex.Y * Constants.TerrainGridSpacing;
                SectorStart.X = Math.Min(StartVertex.X / Constants.SectorTileSize, Map.SectorCount.X - 1);
                SectorStart.Y = Math.Min(StartVertex.Y / Constants.SectorTileSize, Map.SectorCount.Y - 1);
                SectorFinish.X = Math.Min(FinishVertex.X / Constants.SectorTileSize, Map.SectorCount.X - 1);
                SectorFinish.Y = Math.Min(FinishVertex.Y / Constants.SectorTileSize, Map.SectorCount.Y - 1);
                for ( SectorNum.Y = SectorStart.Y; SectorNum.Y <= SectorFinish.Y; SectorNum.Y++ )
                {
                    for ( SectorNum.X = SectorStart.X; SectorNum.X <= SectorFinish.X; SectorNum.X++ )
                    {
                        var Connection = default(clsUnitSectorConnection);
                        foreach ( var tempLoopVar_Connection in Map.Sectors[SectorNum.X, SectorNum.Y].Units )
                        {
                            Connection = tempLoopVar_Connection;
                            Unit = Connection.Unit;
                            if ( Unit.Pos.Horizontal.X >= StartPos.X & Unit.Pos.Horizontal.Y >= StartPos.Y &
                                 Unit.Pos.Horizontal.X <= FinishPos.X & Unit.Pos.Horizontal.Y <= FinishPos.Y )
                            {
                                if ( !Unit.MapSelectedUnitLink.IsConnected )
                                {
                                    Unit.MapSelect();
                                }
                            }
                        }
                    }
                }
            }
            Program.frmMainInstance.SelectedObject_Changed();
            DrawViewLater();
        }

        private void tmrDrawDelay_Tick(Object sender, EventArgs e)
        {
            if ( DrawPending )
            {
                DrawPending = false;
                DrawView();
            }
            else
            {
                tmrDrawDelay.Enabled = false;
            }
        }

        public void pnlDraw_Resize(object sender, EventArgs e)
        {
            if ( OpenGLControl != null )
            {
                ResizeOpenGL();
            }
        }

        public void OpenGL_Resize(Object eventSender, EventArgs eventArgs)
        {
            var Map = MainMap;

            GLSize.X = OpenGLControl.Width;
            GLSize.Y = OpenGLControl.Height;
            //if (GLSize.Y != 0)
            //{
            //    GLSize_XPerY = (float)(GLSize.X / GLSize.Y);
            //}
            Viewport_Resize();
            if ( Map != null )
            {
                Map.ViewInfo.FOV_Calc();
            }
            DrawViewLater();
        }

        public void OpenGL_MouseEnter(object sender, EventArgs e)
        {
            if ( Form.ActiveForm == Program.frmMainInstance )
            {
                OpenGLControl.Focus();
            }
        }

        public void OpenGL_MouseWheel(object sender, MouseEventArgs e)
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            var Move = new XYZInt(0, 0, 0);
            var XYZ_dbl = default(XYZDouble);
            var A = 0;

            for ( A = 0; A <= (int)(Math.Abs(e.Delta / 120.0D)); A++ )
            {
                Matrix3DMath.VectorForwardsRotationByMatrix(Map.ViewInfo.ViewAngleMatrix,
                    Convert.ToDouble(Math.Sign(e.Delta) * Math.Max(Map.ViewInfo.ViewPos.Y, 512.0D) / 24.0D), ref XYZ_dbl);
                Move.Set_dbl(XYZ_dbl);
                Map.ViewInfo.ViewPosChange(Move);
            }
        }

        public GLFont CreateGLFont(Font BaseFont)
        {
            return new GLFont(new Font(BaseFont.FontFamily, 24.0F, BaseFont.Style, GraphicsUnit.Pixel));
        }

        public void RemoveUndoMessage(object sender, EventArgs e)
        {
            UndoMessageTimer.Enabled = false;
            lblUndo.Text = "";
        }

        public void DisplayUndoMessage(string Text)
        {
            lblUndo.Text = Text;
            UndoMessageTimer.Enabled = false;
            UndoMessageTimer.Enabled = true;
        }

        private void OpenGL_MouseLeave(object sender, EventArgs e)
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            Map.ViewInfo.MouseOver = null;
        }

        public void ListSelectBegin(bool isPicker)
        {
            var Map = MainMap;
            var MouseOverTerrain = Map.ViewInfo.GetMouseOverTerrain();

            if ( MouseOverTerrain == null )
            {
                Debugger.Break();
                return;
            }

            var A = 0;
            var Unit = default(clsUnit);

            ListSelect.Close();
            ListSelect.Items.Clear();
            ListSelectItems = new ToolStripItem[MouseOverTerrain.Units.Count];
            for ( A = 0; A <= MouseOverTerrain.Units.Count - 1; A++ )
            {
                Unit = MouseOverTerrain.Units[A];
                ListSelectItems[A] = new ToolStripButton(Unit.TypeBase.GetDisplayTextCode());
                ListSelectItems[A].Tag = Unit;
                ListSelect.Items.Add(ListSelectItems[A]);
            }
            ListSelectIsPicker = isPicker;
            ListSelect.Show(this, new Point(Map.ViewInfo.MouseOver.ScreenPos.X, Map.ViewInfo.MouseOver.ScreenPos.Y));
        }

        public void tabMaps_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if ( !tabMaps.Enabled )
            {
                return;
            }

            if ( tabMaps.SelectedTab == null )
            {
                _Owner.SetMainMap(null);
                return;
            }

            var Map = (clsMap)tabMaps.SelectedTab.Tag;

            _Owner.SetMainMap(Map);
        }

        public void btnClose_Click(Object sender, EventArgs e)
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }
            if ( !Map.frmMainLink.IsConnected )
            {
                MessageBox.Show("Error: Map should be closed already.");
                return;
            }

            if ( !Map.ClosePrompt() )
            {
                return;
            }

            Map.Deallocate();
        }

        public void UpdateTabs()
        {
            var Map = default(clsMap);

            tabMaps.Enabled = false;
            tabMaps.TabPages.Clear();
            foreach ( var tempLoopVar_Map in _Owner.LoadedMaps )
            {
                Map = tempLoopVar_Map;
                tabMaps.TabPages.Add(Map.MapView_TabPage);
            }
            Map = MainMap;
            if ( Map != null )
            {
                tabMaps.SelectedIndex = Map.frmMainLink.ArrayPosition;
            }
            else
            {
                tabMaps.SelectedIndex = -1;
            }
            tabMaps.Enabled = true;
        }
    }
}