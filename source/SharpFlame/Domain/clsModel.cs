#region

using System;
using System.IO;
using NLog;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core.Domain;

#endregion

namespace SharpFlame.Domain
{
    public class clsModel
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public int ConnectorCount;
        public XYZDouble[] Connectors = new XYZDouble[0];

        public int GLTextureNum;

        public int QuadCount;
        public sQuad[] Quads;
        public int TriangleCount;
        public sTriangle[] Triangles;

        public int GLList_Create()
        {
            var Result = 0;

            Result = GL.GenLists(1);
            if ( Result == 0 )
            {
                return Result;
            }

            GL.NewList(Result, ListMode.Compile);
            GLDraw();
            GL.EndList();

            return Result;
        }

        public void GLDraw()
        {
            var A = 0;

            GL.BindTexture(TextureTarget.Texture2D, GLTextureNum);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
            GL.AlphaFunc(AlphaFunction.Greater, 0.5f);
            GL.Enable(EnableCap.AlphaTest);

            GL.Begin(BeginMode.Triangles);
            for ( A = 0; A <= TriangleCount - 1; A++ )
            {
                var with_1 = Triangles[A];
                GL.TexCoord2(with_1.TexCoordA.X, with_1.TexCoordA.Y);
                GL.Vertex3(with_1.PosA.X, with_1.PosA.Y, Convert.ToDouble(- with_1.PosA.Z));
                GL.TexCoord2(with_1.TexCoordB.X, with_1.TexCoordB.Y);
                GL.Vertex3(with_1.PosB.X, with_1.PosB.Y, Convert.ToDouble(- with_1.PosB.Z));
                GL.TexCoord2(with_1.TexCoordC.X, with_1.TexCoordC.Y);
                GL.Vertex3(with_1.PosC.X, with_1.PosC.Y, Convert.ToDouble(- with_1.PosC.Z));
            }
            GL.End();
            GL.Begin(BeginMode.Quads);
            for ( A = 0; A <= QuadCount - 1; A++ )
            {
                var with_2 = Quads[A];
                GL.TexCoord2(with_2.TexCoordA.X, with_2.TexCoordA.Y);
                GL.Vertex3(with_2.PosA.X, with_2.PosA.Y, Convert.ToDouble(- with_2.PosA.Z));
                GL.TexCoord2(with_2.TexCoordB.X, with_2.TexCoordB.Y);
                GL.Vertex3(with_2.PosB.X, with_2.PosB.Y, Convert.ToDouble(- with_2.PosB.Z));
                GL.TexCoord2(with_2.TexCoordC.X, with_2.TexCoordC.Y);
                GL.Vertex3(with_2.PosC.X, with_2.PosC.Y, Convert.ToDouble(- with_2.PosC.Z));
                GL.TexCoord2(with_2.TexCoordD.X, with_2.TexCoordD.Y);
                GL.Vertex3(with_2.PosD.X, with_2.PosD.Y, Convert.ToDouble(- with_2.PosD.Z));
            }
            GL.End();
        }

        public clsResult ReadPIE(StreamReader File, clsObjectData Owner)
        {
            var ReturnResult = new clsResult("Reading PIE", false);
            logger.Debug("Reading PIE");

            var A = 0;
            var B = 0;
            var strTemp = "";
            string[] SplitText = null;
            var LevelCount = 0;
            var NewQuadCount = 0;
            var NewTriCount = 0;
            var C = 0;
            var TextureName = "";
            sPIELevel[] Levels = null;
            var LevelNum = 0;
            var GotText = default(bool);
            string strTemp2;
            var D = 0;
            var PIEVersion = 0;
            var Count = 0;

            Levels = new sPIELevel[0];
            LevelNum = -1;
            do
            {
                strTemp = File.ReadLine();
                if ( strTemp == null )
                {
                    goto FileFinished;
                }
                Reeval:
                if ( strTemp.Substring(0, 3) == "PIE" )
                {
                    PIEVersion = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 4), strTemp.Length - 4));
                    if ( PIEVersion != 2 & PIEVersion != 3 )
                    {
                        ReturnResult.ProblemAdd("Version is unknown.");
                        return ReturnResult;
                    }
                }
                else if ( strTemp.Substring(0, 4) == "TYPE" )
                {
                }
                else if ( strTemp.Substring(0, 7) == "TEXTURE" )
                {
                    TextureName = strTemp.Substring(strTemp.Length - (strTemp.Length - 10), strTemp.Length - 10);
                    A = TextureName.LastIndexOf(" ");
                    if ( A > 0 )
                    {
                        A = TextureName.LastIndexOf(" ", A - 1) + 1;
                    }
                    else
                    {
                        ReturnResult.ProblemAdd("Bad texture name.");
                        return ReturnResult;
                    }

                    if ( A > 0 )
                    {
                        TextureName = TextureName.Substring(0, A - 1);
                    }
                    else
                    {
                        ReturnResult.ProblemAdd("Bad texture name.");
                        return ReturnResult;
                    }
                }
                else if ( strTemp.Substring(0, 6) == "LEVELS" )
                {
                    LevelCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 7), strTemp.Length - 7));
                    Levels = new sPIELevel[LevelCount];
                }
                else if ( strTemp.Substring(0, 6) == "LEVEL " )
                {
                    LevelNum = (int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 6), strTemp.Length - 6))) - 1;
                    if ( LevelNum >= LevelCount )
                    {
                        ReturnResult.ProblemAdd("Level number >= number of levels.");
                        return ReturnResult;
                    }
                }
                else if ( strTemp.Substring(0, 6) == "POINTS" )
                {
                    Levels[LevelNum].PointCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 7), strTemp.Length - 7));
                    Levels[LevelNum].Point = new XYZDouble[Levels[LevelNum].PointCount];
                    A = 0;
                    do
                    {
                        strTemp = File.ReadLine();
                        if ( strTemp == null )
                        {
                            goto FileFinished;
                        }

                        strTemp2 = strTemp.Left(1);
                        if ( char.Parse(strTemp2) == '\t' || strTemp2 == " " )
                        {
                            SplitText = new string[3];
                            C = 0;
                            SplitText[0] = "";
                            GotText = false;
                            for ( B = 0; B <= strTemp.Length - 1; B++ )
                            {
                                if ( strTemp[B] != ' ' && strTemp[B] != '\t' )
                                {
                                    GotText = true;
                                    SplitText[C] += strTemp[B].ToString();
                                }
                                else
                                {
                                    if ( GotText )
                                    {
                                        C++;
                                        if ( C == 3 )
                                        {
                                            break;
                                        }
                                        SplitText[C] = "";
                                        GotText = false;
                                    }
                                }
                            }

                            try
                            {
                                Levels[LevelNum].Point[A].X = float.Parse(SplitText[0]);
                                Levels[LevelNum].Point[A].Y = float.Parse(SplitText[1]);
                                Levels[LevelNum].Point[A].Z = float.Parse(SplitText[2]);
                            }
                            catch ( Exception )
                            {
                                ReturnResult.ProblemAdd("Bad point " + Convert.ToString(A));
                                return ReturnResult;
                            }
                            A++;
                        }
                        else if ( string.IsNullOrEmpty(strTemp2) )
                        {
                        }
                        else
                        {
                            goto Reeval;
                        }
                    } while ( true );
                }
                else if ( strTemp.Substring(0, 8) == "POLYGONS" )
                {
                    Levels[LevelNum].PolygonCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 9), strTemp.Length - 9));
                    Levels[LevelNum].Polygon = new sPIELevel.sPolygon[Levels[LevelNum].PolygonCount];
                    A = 0;
                    do
                    {
                        strTemp = File.ReadLine();
                        if ( strTemp == null )
                        {
                            goto FileFinished;
                        }

                        strTemp2 = strTemp.Left(1);
                        if ( char.Parse(strTemp2) == '\t' || strTemp2 == " " )
                        {
                            C = 0;
                            SplitText = new string[C + 1];
                            SplitText[C] = "";
                            for ( B = 0; B <= strTemp.Length - 1; B++ )
                            {
                                if ( strTemp[B] == ' ' || strTemp[B] == '\t' )
                                {
                                    if ( SplitText[C].Length > 0 )
                                    {
                                        C++;
                                        Array.Resize(ref SplitText, C + 1);
                                        SplitText[C] = "";
                                    }
                                }
                                else
                                {
                                    SplitText[C] += strTemp[B].ToString();
                                }
                            }
                            if ( SplitText[C].Length == 0 )
                            {
                                Array.Resize(ref SplitText, C);
                            }
                            else
                            {
                                C++;
                            }

                            if ( PIEVersion == 3 )
                            {
                                //200, pointcount, points, texcoords
                                if ( C < 2 )
                                {
                                    ReturnResult.ProblemAdd("Too few fields for polygon " + Convert.ToString(A));
                                    return ReturnResult;
                                }
                                try
                                {
                                    Count = int.Parse(SplitText[1]);
                                }
                                catch ( Exception ex )
                                {
                                    ReturnResult.ProblemAdd("Bad polygon point count: " + ex.Message);
                                    return ReturnResult;
                                }
                                Levels[LevelNum].Polygon[A].PointCount = Count;
                                Levels[LevelNum].Polygon[A].PointNum = new int[Count];
                                Levels[LevelNum].Polygon[A].TexCoord = new XYDouble[Count];
                                if ( Count == 3 )
                                {
                                    NewTriCount++;
                                }
                                else if ( Count == 4 )
                                {
                                    NewQuadCount++;
                                }
                                if ( SplitText.GetUpperBound(0) + 1 == 0 )
                                {
                                    goto Reeval;
                                }
                                if ( SplitText.GetUpperBound(0) + 1 != (2 + Count * 3) )
                                {
                                    ReturnResult.ProblemAdd("Wrong number of fields (" + Convert.ToString(SplitText.GetUpperBound(0) + 1) + ") for polygon " +
                                                            Convert.ToString(A));
                                    return ReturnResult;
                                }
                                for ( B = 0; B <= Count - 1; B++ )
                                {
                                    try
                                    {
                                        Levels[LevelNum].Polygon[A].PointNum[B] = int.Parse(SplitText[2 + B]);
                                    }
                                    catch ( Exception ex )
                                    {
                                        ReturnResult.ProblemAdd("Bad polygon point: " + ex.Message);
                                        return ReturnResult;
                                    }

                                    try
                                    {
                                        Levels[LevelNum].Polygon[A].TexCoord[B].X = float.Parse(SplitText[2 + Count + 2 * B]);
                                    }
                                    catch ( Exception ex )
                                    {
                                        ReturnResult.ProblemAdd("Bad polygon x tex coord: " + ex.Message);
                                        return ReturnResult;
                                    }
                                    try
                                    {
                                        Levels[LevelNum].Polygon[A].TexCoord[B].Y = float.Parse(SplitText[2 + Count + 2 * B + 1]);
                                    }
                                    catch ( Exception ex )
                                    {
                                        ReturnResult.ProblemAdd("Bad polygon y tex coord: " + ex.Message);
                                        return ReturnResult;
                                    }
                                }
                                A++;
                            }
                            else if ( PIEVersion == 2 )
                            {
                                D = 0;
                                do
                                {
                                    //flag, numpoints, points[], x4 ignore if animated, texcoord[]xy
                                    Levels[LevelNum].Polygon[A].PointCount = int.Parse(SplitText[D + 1]);
                                    Levels[LevelNum].Polygon[A].PointNum = new int[Levels[LevelNum].Polygon[A].PointCount];
                                    Levels[LevelNum].Polygon[A].TexCoord = new XYDouble[Levels[LevelNum].Polygon[A].PointCount];
                                    if ( Levels[LevelNum].Polygon[A].PointCount == 3 )
                                    {
                                        NewTriCount++;
                                    }
                                    else if ( Levels[LevelNum].Polygon[A].PointCount == 4 )
                                    {
                                        NewQuadCount++;
                                    }
                                    for ( B = 0; B <= Levels[LevelNum].Polygon[A].PointCount - 1; B++ )
                                    {
                                        Levels[LevelNum].Polygon[A].PointNum[B] = int.Parse(SplitText[D + 2 + B]);
                                    }
                                    C = D + 2 + Levels[LevelNum].Polygon[A].PointCount;
                                    if ( SplitText[D] == "4200" || SplitText[D] == "4000" || SplitText[D] == "6a00" || SplitText[D] == "4a00" || SplitText[D] == "6200" ||
                                         SplitText[D] == "14200" || SplitText[D] == "14a00" || SplitText[D] == "16a00" )
                                    {
                                        C += 4;
                                    }
                                    for ( B = 0; B <= Levels[LevelNum].Polygon[A].PointCount - 1; B++ )
                                    {
                                        Levels[LevelNum].Polygon[A].TexCoord[B].X = float.Parse(SplitText[C]);
                                        Levels[LevelNum].Polygon[A].TexCoord[B].Y = float.Parse(SplitText[C + 1]);
                                        C += 2;
                                    }
                                    D = C;
                                    A++;
                                } while ( D < SplitText.GetUpperBound(0) );
                            }
                        }
                        else if ( string.IsNullOrEmpty(strTemp2) )
                        {
                        }
                        else
                        {
                            goto Reeval;
                        }
                    } while ( true );
                }
                else if ( strTemp.Substring(0, 10) == "CONNECTORS" )
                {
                    ConnectorCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 11), strTemp.Length - 11));
                    Connectors = new XYZDouble[ConnectorCount];
                    A = 0;
                    do
                    {
                        strTemp = File.ReadLine();
                        if ( strTemp == null )
                        {
                            goto FileFinished;
                        }

                        strTemp2 = strTemp.Left(1);
                        if ( char.Parse(strTemp2) == '\t' || strTemp2 == " " )
                        {
                            SplitText = new string[3];
                            C = 0;
                            SplitText[0] = "";
                            GotText = false;
                            for ( B = 0; B <= strTemp.Length - 1; B++ )
                            {
                                if ( strTemp[B] != ' ' && strTemp[B] != '\t' )
                                {
                                    GotText = true;
                                    SplitText[C] += strTemp[B].ToString();
                                }
                                else
                                {
                                    if ( GotText )
                                    {
                                        C++;
                                        if ( C == 3 )
                                        {
                                            break;
                                        }
                                        SplitText[C] = "";
                                        GotText = false;
                                    }
                                }
                            }

                            try
                            {
                                Connectors[A].X = float.Parse(SplitText[0]);
                                Connectors[A].Y = float.Parse(SplitText[2]);
                                Connectors[A].Z = float.Parse(SplitText[1]);
                            }
                            catch ( Exception )
                            {
                                ReturnResult.ProblemAdd("Bad connector " + Convert.ToString(A));
                                return ReturnResult;
                            }
                            A++;
                        }
                        else if ( string.IsNullOrEmpty(strTemp2) )
                        {
                        }
                        else
                        {
                            goto Reeval;
                        }
                    } while ( true );
                }
            } while ( true );
            FileFinished:

            GLTextureNum = Owner.Get_TexturePage_GLTexture(TextureName.Substring(0, TextureName.Length - 4));
            if ( GLTextureNum == 0 )
            {
                ReturnResult.WarningAdd("Texture \"{0}\" was not loaded".Format2(TextureName));
            }

            TriangleCount = NewTriCount;
            QuadCount = NewQuadCount;
            Triangles = new sTriangle[TriangleCount];
            Quads = new sQuad[QuadCount];
            NewTriCount = 0;
            NewQuadCount = 0;
            for ( LevelNum = 0; LevelNum <= LevelCount - 1; LevelNum++ )
            {
                for ( A = 0; A <= Levels[LevelNum].PolygonCount - 1; A++ )
                {
                    if ( Levels[LevelNum].Polygon[A].PointCount == 3 )
                    {
                        Triangles[NewTriCount].PosA = Levels[LevelNum].Point[Levels[LevelNum].Polygon[A].PointNum[0]];
                        Triangles[NewTriCount].PosB = Levels[LevelNum].Point[Levels[LevelNum].Polygon[A].PointNum[1]];
                        Triangles[NewTriCount].PosC = Levels[LevelNum].Point[Levels[LevelNum].Polygon[A].PointNum[2]];
                        if ( PIEVersion == 2 )
                        {
                            Triangles[NewTriCount].TexCoordA.X = (float)(Levels[LevelNum].Polygon[A].TexCoord[0].X / 255.0D);
                            Triangles[NewTriCount].TexCoordA.Y = (float)(Levels[LevelNum].Polygon[A].TexCoord[0].Y / 255.0D);
                            Triangles[NewTriCount].TexCoordB.X = (float)(Levels[LevelNum].Polygon[A].TexCoord[1].X / 255.0D);
                            Triangles[NewTriCount].TexCoordB.Y = (float)(Levels[LevelNum].Polygon[A].TexCoord[1].Y / 255.0D);
                            Triangles[NewTriCount].TexCoordC.X = (float)(Levels[LevelNum].Polygon[A].TexCoord[2].X / 255.0D);
                            Triangles[NewTriCount].TexCoordC.Y = (float)(Levels[LevelNum].Polygon[A].TexCoord[2].Y / 255.0D);
                        }
                        else if ( PIEVersion == 3 )
                        {
                            Triangles[NewTriCount].TexCoordA = Levels[LevelNum].Polygon[A].TexCoord[0];
                            Triangles[NewTriCount].TexCoordB = Levels[LevelNum].Polygon[A].TexCoord[1];
                            Triangles[NewTriCount].TexCoordC = Levels[LevelNum].Polygon[A].TexCoord[2];
                        }
                        NewTriCount++;
                    }
                    else if ( Levels[LevelNum].Polygon[A].PointCount == 4 )
                    {
                        Quads[NewQuadCount].PosA = Levels[LevelNum].Point[Levels[LevelNum].Polygon[A].PointNum[0]];
                        Quads[NewQuadCount].PosB = Levels[LevelNum].Point[Levels[LevelNum].Polygon[A].PointNum[1]];
                        Quads[NewQuadCount].PosC = Levels[LevelNum].Point[Levels[LevelNum].Polygon[A].PointNum[2]];
                        Quads[NewQuadCount].PosD = Levels[LevelNum].Point[Levels[LevelNum].Polygon[A].PointNum[3]];
                        if ( PIEVersion == 2 )
                        {
                            Quads[NewQuadCount].TexCoordA.X = (float)(Levels[LevelNum].Polygon[A].TexCoord[0].X / 255.0D);
                            Quads[NewQuadCount].TexCoordA.Y = (float)(Levels[LevelNum].Polygon[A].TexCoord[0].Y / 255.0D);
                            Quads[NewQuadCount].TexCoordB.X = (float)(Levels[LevelNum].Polygon[A].TexCoord[1].X / 255.0D);
                            Quads[NewQuadCount].TexCoordB.Y = (float)(Levels[LevelNum].Polygon[A].TexCoord[1].Y / 255.0D);
                            Quads[NewQuadCount].TexCoordC.X = (float)(Levels[LevelNum].Polygon[A].TexCoord[2].X / 255.0D);
                            Quads[NewQuadCount].TexCoordC.Y = (float)(Levels[LevelNum].Polygon[A].TexCoord[2].Y / 255.0D);
                            Quads[NewQuadCount].TexCoordD.X = (float)(Levels[LevelNum].Polygon[A].TexCoord[3].X / 255.0D);
                            Quads[NewQuadCount].TexCoordD.Y = (float)(Levels[LevelNum].Polygon[A].TexCoord[3].Y / 255.0D);
                        }
                        else if ( PIEVersion == 3 )
                        {
                            Quads[NewQuadCount].TexCoordA = Levels[LevelNum].Polygon[A].TexCoord[0];
                            Quads[NewQuadCount].TexCoordB = Levels[LevelNum].Polygon[A].TexCoord[1];
                            Quads[NewQuadCount].TexCoordC = Levels[LevelNum].Polygon[A].TexCoord[2];
                            Quads[NewQuadCount].TexCoordD = Levels[LevelNum].Polygon[A].TexCoord[3];
                        }
                        NewQuadCount++;
                    }
                }
            }

            return ReturnResult;
        }

        public struct sPIELevel
        {
            public XYZDouble[] Point;
            public int PointCount;
            public sPolygon[] Polygon;
            public int PolygonCount;

            public struct sPolygon
            {
                public int PointCount;
                public int[] PointNum;
                public XYDouble[] TexCoord;
            }
        }

        public struct sQuad
        {
            public XYZDouble PosA;
            public XYZDouble PosB;
            public XYZDouble PosC;
            public XYZDouble PosD;
            public XYDouble TexCoordA;
            public XYDouble TexCoordB;
            public XYDouble TexCoordC;
            public XYDouble TexCoordD;
        }

        public struct sTriangle
        {
            public XYZDouble PosA;
            public XYZDouble PosB;
            public XYZDouble PosC;
            public XYDouble TexCoordA;
            public XYDouble TexCoordB;
            public XYDouble TexCoordC;
        }
    }
}