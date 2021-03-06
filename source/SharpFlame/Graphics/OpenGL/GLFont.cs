#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Colors;
using SharpFlame.Core.Domain;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

#endregion

namespace SharpFlame.Graphics.OpenGL
{
    public class GLFont
    {
        public Font BaseFont;

        public sCharacter[] Character = new sCharacter[256];
        public int Height;

        public GLFont(Font BaseFont)
        {
            GLTextures_Generate(BaseFont);
        }

        private void GLTextures_Generate(Font NewBaseFont)
        {
            var A = 0;
            var X = 0;
            var Y = 0;
            var TempBitmap = default(Bitmap);
            var gfx = default(System.Drawing.Graphics);
            var BitmapData = default(BitmapData);
            var NewSizeX = 0;
            var StartX = 0;
            var FinishX = 0;
            var TexBitmap = default(Bitmap);
            var Text = "";

            BaseFont = NewBaseFont;
            Height = BaseFont.Height;
            for ( A = 0; A <= 255; A++ )
            {
                Text = ((char)A).ToString();
                TempBitmap = new Bitmap(Height * 2, Height, PixelFormat.Format32bppArgb);
                gfx = System.Drawing.Graphics.FromImage(TempBitmap);
                gfx.Clear(Color.Transparent);
                gfx.DrawString(Text, BaseFont, Brushes.White, 0.0F, 0.0F);
                gfx.Dispose();
                for ( X = 0; X <= TempBitmap.Width - 1; X++ )
                {
                    for ( Y = 0; Y <= TempBitmap.Height - 1; Y++ )
                    {
                        if ( TempBitmap.GetPixel(X, Y).A > 0 )
                        {
                            break;
                        }
                    }
                    if ( Y < TempBitmap.Height )
                    {
                        break;
                    }
                }
                StartX = X;
                for ( X = TempBitmap.Width - 1; X >= 0; X-- )
                {
                    for ( Y = 0; Y <= TempBitmap.Height - 1; Y++ )
                    {
                        if ( TempBitmap.GetPixel(X, Y).A > 0 )
                        {
                            break;
                        }
                    }
                    if ( Y < TempBitmap.Height )
                    {
                        break;
                    }
                }
                FinishX = X;
                NewSizeX = FinishX - StartX + 1;
                if ( NewSizeX <= 0 )
                {
                    NewSizeX = Math.Max((int)(Math.Round(Height / 4.0F)), 1);
                    Character[A].TexSize = (int)(Math.Round(Math.Pow(2.0D, Math.Ceiling(Math.Log(Math.Max(NewSizeX, TempBitmap.Height)) / Math.Log(2.0D)))));
                    TexBitmap = new Bitmap(Character[A].TexSize, Convert.ToInt32(Character[A].TexSize), PixelFormat.Format32bppArgb);
                    gfx = System.Drawing.Graphics.FromImage(TexBitmap);
                    gfx.Clear(Color.Transparent);
                    gfx.Dispose();
                    BitmapData = TexBitmap.LockBits(new Rectangle(0, 0, TexBitmap.Width, TexBitmap.Height), ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb);
                    GL.GenTextures(1, out Character[A].GLTexture);
                    GL.BindTexture(TextureTarget.Texture2D, Convert.ToInt32(Character[A].GLTexture));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TexBitmap.Width, TexBitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, BitmapData.Scan0);
                    TexBitmap.UnlockBits(BitmapData);
                    Character[A].Width = NewSizeX;
                }
                else
                {
                    Character[A].TexSize = (int)(Math.Round(Math.Pow(2.0D, Math.Ceiling(Math.Log(Math.Max(NewSizeX, TempBitmap.Height)) / Math.Log(2.0D)))));
                    TexBitmap = new Bitmap(Convert.ToInt32(Character[A].TexSize), Character[A].TexSize, PixelFormat.Format32bppArgb);
                    gfx = System.Drawing.Graphics.FromImage(TexBitmap);
                    gfx.Clear(Color.Transparent);
                    gfx.Dispose();
                    for ( Y = 0; Y <= TempBitmap.Height - 1; Y++ )
                    {
                        for ( X = StartX; X <= FinishX; X++ )
                        {
                            TexBitmap.SetPixel(X - StartX, Y, TempBitmap.GetPixel(X, Y));
                        }
                    }
                    BitmapData = TexBitmap.LockBits(new Rectangle(0, 0, TexBitmap.Width, TexBitmap.Height), ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb);
                    GL.GenTextures(1, out Character[A].GLTexture);
                    GL.BindTexture(TextureTarget.Texture2D, Character[A].GLTexture);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TexBitmap.Width, TexBitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                        PixelType.UnsignedByte, BitmapData.Scan0);
                    TexBitmap.UnlockBits(BitmapData);
                    Character[A].Width = NewSizeX;
                }
                TempBitmap.Dispose();
                TexBitmap.Dispose();
            }
        }

        public void Deallocate()
        {
            var A = 0;

            for ( A = 0; A <= 255; A++ )
            {
                GL.DeleteTexture(Convert.ToInt32(Character[A].GLTexture));
            }
        }

        public struct sCharacter
        {
            public int GLTexture;
            public int TexSize;
            public int Width;
        }
    }

    public class clsTextLabel
    {
        public sRGBA_sng Colour;
        public XYInt Pos;
        public float SizeY;
        public string Text;
        public GLFont TextFont;

        public float GetSizeX()
        {
            float SizeX = 0;
            float CharWidth = 0;
            var CharSpacing = SizeY / 10.0F;
            var CharSize = SizeY / TextFont.Height;
            var A = 0;

            for ( A = 0; A <= Text.Length - 1; A++ )
            {
                CharWidth = TextFont.Character[Text[A]].Width * CharSize;
                SizeX += CharWidth;
            }
            SizeX += CharSpacing * (Text.Length - 1);

            return SizeX;
        }

        public void Draw()
        {
            if ( Text == null )
            {
                return;
            }
            if ( Text.Length == 0 )
            {
                return;
            }
            if ( TextFont == null )
            {
                return;
            }

            var CharCode = 0;
            float CharWidth = 0;
            var TexRatio = new XYDouble();
            float LetterPosA = 0;
            float LetterPosB = 0;
            float PosY1 = 0;
            float PosY2 = 0;
            float CharSpacing = 0;
            var A = 0;

            GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
            PosY1 = Pos.Y;
            PosY2 = Pos.Y + SizeY;
            CharSpacing = SizeY / 10.0F;
            LetterPosA = Pos.X;
            for ( A = 0; A <= Text.Length - 1; A++ )
            {
                CharCode = Text[A];
                if ( CharCode >= 0 & CharCode <= 255 )
                {
                    CharWidth = SizeY * TextFont.Character[CharCode].Width / TextFont.Height;
                    TexRatio.X = (float)((double)TextFont.Character[CharCode].Width / TextFont.Character[CharCode].TexSize);
                    TexRatio.Y = (float)((double)TextFont.Height / TextFont.Character[CharCode].TexSize);
                    LetterPosB = LetterPosA + CharWidth;
                    GL.BindTexture(TextureTarget.Texture2D, TextFont.Character[CharCode].GLTexture);
                    GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
                    GL.Begin(BeginMode.Quads);
                    GL.TexCoord2(0.0F, 0.0F);
                    GL.Vertex2(LetterPosA, PosY1);
                    GL.TexCoord2(0.0F, TexRatio.Y);
                    GL.Vertex2(LetterPosA, PosY2);
                    GL.TexCoord2(TexRatio.X, TexRatio.Y);
                    GL.Vertex2(LetterPosB, PosY2);
                    GL.TexCoord2(TexRatio.X, 0.0F);
                    GL.Vertex2(LetterPosB, PosY1);
                    GL.End();
                    LetterPosA = LetterPosB + CharSpacing;
                }
            }
        }
    }

    public class clsTextLabels
    {
        public int ItemCount = 0;
        public clsTextLabel[] Items;
        public int MaxCount;

        public clsTextLabels(int MaxItemCount)
        {
            MaxCount = MaxItemCount;
            Items = new clsTextLabel[MaxCount];
        }

        public bool AtMaxCount()
        {
            return ItemCount >= MaxCount;
        }

        public void Add(clsTextLabel NewItem)
        {
            if ( ItemCount == MaxCount )
            {
                Debugger.Break();
                return;
            }

            Items[ItemCount] = NewItem;
            ItemCount++;
        }

        public void Draw()
        {
            var A = 0;

            for ( A = 0; A <= ItemCount - 1; A++ )
            {
                Items[A].Draw();
            }
        }
    }
}