#region

using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.Graphics.OpenGL;
using SharpFlame.Mapping;
using SharpFlame.Mapping.Tiles;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Controls
{
    public partial class TextureViewControl
    {
        private readonly frmMain _Owner;
        private readonly Timer tmrDraw;
        private readonly Timer tmrDrawDelay;
        public bool DisplayTileNumbers = false;
        public bool DisplayTileTypes = false;

        public bool DrawPending;

        public bool DrawView_Enabled = false;

        private Timer GLInitializeDelayTimer;
        public XYInt GLSize;
        public double GLSize_XPerY;
        public bool IsGLInitialized = false;

        public GLControl OpenGLControl;
        public XYInt TextureCount;
        public int TextureYOffset;
        public XYInt View_Pos;

        public TextureViewControl(frmMain Owner)
        {
            _Owner = Owner;
            TextureCount = new XYInt(0, 0);
            GLSize = new XYInt(0, 0);

            InitializeComponent();

            OpenGLControl = Program.OpenGL2;
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
        }

        private clsMap MainMap
        {
            get { return _Owner.MainMap; }
        }

        public void OpenGL_Size_Calc()
        {
            if ( OpenGLControl.Context == null )
            {
                return;
            }

            OpenGLControl.Width = pnlDraw.Width;
            OpenGLControl.Height = pnlDraw.Height;

            Viewport_Resize();
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
                DrawPending = false;
                DrawView();
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

            OpenGL_Size_Calc();

            OpenGLControl.MouseDown += OpenGL_MouseDown;
            OpenGLControl.Resize += OpenGL_Resize;

            if ( GraphicsContext.CurrentContext != OpenGLControl.Context )
            {
                OpenGLControl.MakeCurrent();
            }

            GL.ClearColor(0.0F, 0.0F, 0.0F, 1.0F);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.Enable(EnableCap.CullFace);

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

            GL.Clear(ClearBufferMask.ColorBufferBit);

            var Map = MainMap;

            if ( Map == null )
            {
                GL.Flush();
                OpenGLControl.SwapBuffers();
                Refresh();
                return;
            }

            var X = 0;
            var Y = 0;
            var Num = 0;
            var XY_int = new XYInt();
            var A = 0;
            var Vertex0 = new XYDouble();
            var Vertex1 = new XYDouble();
            var Vertex2 = new XYDouble();
            var UnrotatedPos = new XYDouble();
            var TexCoord0 = new XYDouble();
            var TexCoord1 = new XYDouble();
            var TexCoord2 = new XYDouble();
            var TexCoord3 = new XYDouble();

            GL.MatrixMode(MatrixMode.Projection);
            var temp_mat = Matrix4.CreateOrthographicOffCenter(0.0F, GLSize.X, GLSize.Y, 0.0F, -1.0F, 1.0F);
            GL.LoadMatrix(ref temp_mat);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            if ( Map.Tileset != null )
            {
                TileUtil.GetTileRotatedTexCoords(App.TextureOrientation, ref TexCoord0, ref TexCoord1, ref TexCoord2, ref TexCoord3);

                GL.Enable(EnableCap.Texture2D);
                GL.Color4(0.0F, 0.0F, 0.0F, 1.0F);

                for ( Y = 0; Y <= TextureCount.Y - 1; Y++ )
                {
                    for ( X = 0; X <= TextureCount.X - 1; X++ )
                    {
                        Num = (TextureYOffset + Y) * TextureCount.X + X;
                        if ( Num >= Map.Tileset.TileCount )
                        {
                            goto EndOfTextures1;
                        }
                        A = Map.Tileset.Tiles[Num].TextureViewGlTextureNum;
                        GL.BindTexture(TextureTarget.Texture2D, A);
                        GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Decal);
                        GL.Begin(BeginMode.Quads);
                        GL.TexCoord2(TexCoord0.X, TexCoord0.Y);
                        GL.Vertex2(X * 64, Y * 64); // Top Left
                        GL.TexCoord2(TexCoord1.X, TexCoord1.Y);
                        GL.Vertex2(X * 64 + 64, Y * 64); // Bottom Left
                        GL.TexCoord2(TexCoord3.X, TexCoord3.Y);
                        GL.Vertex2(X * 64 + 64, Y * 64 + 64); // Bottom right
                        GL.TexCoord2(TexCoord2.X, TexCoord2.Y);
                        GL.Vertex2(X * 64, Y * 64 + 64); // Top right

                        GL.End();
                    }
                }

                EndOfTextures1:

                GL.Disable(EnableCap.Texture2D);

                if ( DisplayTileTypes )
                {
                    GL.Begin(BeginMode.Quads);
                    for ( Y = 0; Y <= TextureCount.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= TextureCount.X - 1; X++ )
                        {
                            Num = (TextureYOffset + Y) * TextureCount.X + X;
                            if ( Num >= Map.Tileset.TileCount )
                            {
                                goto EndOfTextures2;
                            }
                            A = Map.Tile_TypeNum[Num];
                            GL.Color3(App.TileTypes[A].DisplayColour.Red, App.TileTypes[A].DisplayColour.Green, App.TileTypes[A].DisplayColour.Blue);
                            GL.Vertex2(X * 64 + 24, Y * 64 + 24);
                            GL.Vertex2(X * 64 + 24, Y * 64 + 40);
                            GL.Vertex2(X * 64 + 40, Y * 64 + 40);
                            GL.Vertex2(X * 64 + 40, Y * 64 + 24);
                        }
                    }
                    EndOfTextures2:
                    GL.End();
                }

                if ( App.DisplayTileOrientation )
                {
                    GL.Disable(EnableCap.CullFace);

                    UnrotatedPos.X = 0.25F;
                    UnrotatedPos.Y = 0.25F;
                    Vertex0 = TileUtil.GetTileRotatedPos_sng(App.TextureOrientation, UnrotatedPos);
                    UnrotatedPos.X = 0.5F;
                    UnrotatedPos.Y = 0.25F;
                    Vertex1 = TileUtil.GetTileRotatedPos_sng(App.TextureOrientation, UnrotatedPos);
                    UnrotatedPos.X = 0.5F;
                    UnrotatedPos.Y = 0.5F;
                    Vertex2 = TileUtil.GetTileRotatedPos_sng(App.TextureOrientation, UnrotatedPos);

                    GL.Begin(BeginMode.Triangles);
                    GL.Color3(1.0F, 1.0F, 0.0F);
                    for ( Y = 0; Y <= TextureCount.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= TextureCount.X - 1; X++ )
                        {
                            Num = (TextureYOffset + Y) * TextureCount.X + X;
                            if ( Num >= Map.Tileset.TileCount )
                            {
                                goto EndOfTextures3;
                            }
                            GL.Vertex2(X * 64 + Vertex0.X * 64, Y * 64 + Vertex0.Y * 64);
                            GL.Vertex2(X * 64 + Vertex2.X * 64, Y * 64 + Vertex2.Y * 64);
                            GL.Vertex2(X * 64 + Vertex1.X * 64, Y * 64 + Vertex1.Y * 64);
                        }
                    }
                    EndOfTextures3:
                    GL.End();

                    GL.Enable(EnableCap.CullFace);
                }

                if ( DisplayTileNumbers && App.UnitLabelFont != null ) //TextureViewFont IsNot Nothing Then
                {
                    var TextLabel = default(clsTextLabel);
                    GL.Enable(EnableCap.Texture2D);
                    for ( Y = 0; Y <= TextureCount.Y - 1; Y++ )
                    {
                        for ( X = 0; X <= TextureCount.X - 1; X++ )
                        {
                            Num = (TextureYOffset + Y) * TextureCount.X + X;
                            if ( Num >= Map.Tileset.TileCount )
                            {
                                goto EndOfTextures4;
                            }
                            TextLabel = new clsTextLabel();
                            TextLabel.Text = Num.ToStringInvariant();
                            TextLabel.SizeY = 24.0F;
                            TextLabel.Colour.Red = 1.0F;
                            TextLabel.Colour.Green = 1.0F;
                            TextLabel.Colour.Blue = 0.0F;
                            TextLabel.Colour.Alpha = 1.0F;
                            TextLabel.Pos.X = X * 64;
                            TextLabel.Pos.Y = Y * 64;
                            TextLabel.TextFont = App.UnitLabelFont; //TextureViewFont
                            TextLabel.Draw();
                        }
                    }
                    EndOfTextures4:
                    GL.Disable(EnableCap.Texture2D);
                }

                if ( App.SelectedTextureNum >= 0 & TextureCount.X > 0 )
                {
                    A = App.SelectedTextureNum - TextureYOffset * TextureCount.X;
                    XY_int.X = A - A / TextureCount.X * TextureCount.X;
                    XY_int.Y = A / TextureCount.X;
                    GL.Begin(BeginMode.LineLoop);
                    GL.Color3(1.0F, 1.0F, 0.0F);
                    GL.Vertex2(XY_int.X * 64, XY_int.Y * 64);
                    GL.Vertex2(XY_int.X * 64, XY_int.Y * 64.0D + 64);
                    GL.Vertex2(XY_int.X * 64 + 64, XY_int.Y * 64 + 64);
                    GL.Vertex2(XY_int.X * 64 + 64, XY_int.Y * 64);
                    GL.End();
                }
            }

            GL.Flush();
            OpenGLControl.SwapBuffers();

            Refresh();
        }

        public void OpenGL_MouseDown(object sender, MouseEventArgs e)
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            if ( Map == null )
            {
                App.SelectedTextureNum = -1;
            }
            else if ( Map.Tileset == null )
            {
                App.SelectedTextureNum = -1;
            }
            else if ( e.X >= 0 & e.X < TextureCount.X * 64 & e.Y >= 0 & e.Y < TextureCount.Y * 64 )
            {
                App.SelectedTextureNum = (int)((TextureYOffset + (int)(e.Y / 64.0D)) * TextureCount.X + (e.X / 64.0D));
                if ( App.SelectedTextureNum >= Map.Tileset.TileCount )
                {
                    App.SelectedTextureNum = -1;
                }
                else
                {
                    modTools.Tool = modTools.Tools.TextureBrush;
                }
            }
            else
            {
                App.SelectedTextureNum = -1;
            }

            if ( App.SelectedTextureNum >= 0 )
            {
                Program.frmMainInstance.cboTileType.Enabled = false;
                Program.frmMainInstance.cboTileType.SelectedIndex = Map.Tile_TypeNum[App.SelectedTextureNum];
                Program.frmMainInstance.cboTileType.Enabled = true;
            }
            else
            {
                Program.frmMainInstance.cboTileType.Enabled = false;
                Program.frmMainInstance.cboTileType.SelectedIndex = -1;
            }

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
                OpenGL_Size_Calc();
                TextureCount.X = (int)(Math.Floor(GLSize.X / 64.0D));
                TextureCount.Y = (int)(Math.Ceiling(GLSize.Y / 64.0D));
            }
            else
            {
                TextureCount.X = 0;
                TextureCount.Y = 0;
            }

            ScrollUpdate();
        }

        public void ScrollUpdate()
        {
            var Map = MainMap;

            if ( Map == null )
            {
                return;
            }

            var Flag = default(bool);

            if ( TextureCount.X > 0 & TextureCount.Y > 0 )
            {
                if ( Map == null )
                {
                    Flag = true;
                }
                else if ( Map.Tileset == null )
                {
                    Flag = true;
                }
                else
                {
                    Flag = false;
                }
            }
            else
            {
                Flag = true;
            }
            if ( Flag )
            {
                TextureScroll.Maximum = 0;
                TextureScroll.LargeChange = 0;
                TextureScroll.Enabled = false;
            }
            else
            {
                TextureScroll.Maximum = (int)(Math.Ceiling(((double)Map.Tileset.TileCount / TextureCount.X)));
                TextureScroll.LargeChange = TextureCount.Y;
                TextureScroll.Enabled = true;
            }
        }

        public void OpenGL_Resize(object sender, EventArgs e)
        {
            GLSize.X = OpenGLControl.Width;
            GLSize.Y = OpenGLControl.Height;
            if ( GLSize.Y != 0 )
            {
                GLSize_XPerY = (double)GLSize.X / GLSize.Y;
            }
            Viewport_Resize();
        }

        public void TextureScroll_ValueChanged(object sender, EventArgs e)
        {
            TextureYOffset = TextureScroll.Value;

            DrawViewLater();
        }
    }
}