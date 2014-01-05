using System;
using System.Diagnostics;
using Matrix3D;
using Microsoft.VisualBasic;
using OpenTK.Graphics.OpenGL;

namespace SharpFlame
{
	public partial class clsMap
	{
		
		public clsViewInfo ViewInfo;
		
		public void GLDraw()
		{
			Matrix3D.Position.XYZ_dbl XYZ_dbl = default(Matrix3D.Position.XYZ_dbl);
			modMath.sXY_int Footprint;
			int X = 0;
			int Y = 0;
			int X2 = 0;
			int Y2 = 0;
			int A = 0;
			int B = 0;
			int C = 0;
			int D = 0;
			sRGBA_sng ColourA = new sRGBA_sng();
			sRGBA_sng ColourB = new sRGBA_sng();
			bool ShowMinimapViewPosBox = default(bool);
			Matrix3D.Position.XY_dbl ViewCorner0 = default(Matrix3D.Position.XY_dbl);
			Matrix3D.Position.XY_dbl ViewCorner1 = default(Matrix3D.Position.XY_dbl);
			Matrix3D.Position.XY_dbl ViewCorner2 = default(Matrix3D.Position.XY_dbl);
			Matrix3D.Position.XY_dbl ViewCorner3 = default(Matrix3D.Position.XY_dbl);
			double dblTemp = 0;
			Matrix3D.Position.XYZ_dbl Vertex0 = default(Matrix3D.Position.XYZ_dbl);
			Matrix3D.Position.XYZ_dbl Vertex1 = default(Matrix3D.Position.XYZ_dbl);
			Matrix3D.Position.XYZ_dbl Vertex2 = default(Matrix3D.Position.XYZ_dbl);
			Matrix3D.Position.XYZ_dbl Vertex3 = default(Matrix3D.Position.XYZ_dbl);
			modMath.sXY_int ScreenPos = new modMath.sXY_int();
			Matrix3D.Position.XYZ_dbl XYZ_dbl2 = default(Matrix3D.Position.XYZ_dbl);
			modProgram.sWorldPos WorldPos = new modProgram.sWorldPos();
			Matrix3D.Position.XY_dbl PosA = default(Matrix3D.Position.XY_dbl);
			Matrix3D.Position.XY_dbl PosB = default(Matrix3D.Position.XY_dbl);
			Matrix3D.Position.XY_dbl PosC = default(Matrix3D.Position.XY_dbl);
			Matrix3D.Position.XY_dbl PosD = default(Matrix3D.Position.XY_dbl);
			modMath.sXY_int MinimapSizeXY = new modMath.sXY_int();
			clsMap.clsUnit Unit = default(clsMap.clsUnit);
			modMath.sXY_int StartXY = new modMath.sXY_int();
			modMath.sXY_int FinishXY = new modMath.sXY_int();
			bool DrawIt = default(bool);
			clsBrush.sPosNum DrawCentreSector = new clsBrush.sPosNum();
			clsTextLabel SelectionLabel = new clsTextLabel();
			float[] light_position = new float[4];
			Matrix3DMath.Matrix3D matrixA = new Matrix3DMath.Matrix3D();
			Matrix3DMath.Matrix3D matrixB = new Matrix3DMath.Matrix3D();
			clsMap.clsAction MapAction = default(clsMap.clsAction);
			float ZNearFar = 0;
			ctrlMapView MapView = ViewInfo.MapView;
			modMath.sXY_int GLSize = ViewInfo.MapView.GLSize;
			Matrix3D.Position.XY_dbl DrawCentre = default(Matrix3D.Position.XY_dbl);
			double dblTemp2 = 0;
			
			dblTemp = modSettings.Settings.MinimapSize;
			ViewInfo.Tiles_Per_Minimap_Pixel = Math.Sqrt(Terrain.TileSize.X * Terrain.TileSize.X + Terrain.TileSize.Y * Terrain.TileSize.Y) / (modMath.RootTwo * dblTemp);
			if (Minimap_Texture_Size > 0 & ViewInfo.Tiles_Per_Minimap_Pixel > 0.0D)
			{
				MinimapSizeXY.X = (int) (Terrain.TileSize.X / ViewInfo.Tiles_Per_Minimap_Pixel);
				MinimapSizeXY.Y = (int) (Terrain.TileSize.Y / ViewInfo.Tiles_Per_Minimap_Pixel);
			}
			
			if (!ViewInfo.ScreenXY_Get_ViewPlanePos(new modMath.sXY_int((int) (GLSize.X / 2.0D), (int) (GLSize.Y / 2.0D)), dblTemp, DrawCentre))
			{
				Matrix3DMath.VectorForwardsRotationByMatrix(ViewInfo.ViewAngleMatrix, ref XYZ_dbl);
				dblTemp2 = modProgram.VisionRadius * 2.0D / Math.Sqrt(XYZ_dbl.X * XYZ_dbl.X + XYZ_dbl.Z * XYZ_dbl.Z);
				DrawCentre.X = ViewInfo.ViewPos.X + XYZ_dbl.X * dblTemp2;
				DrawCentre.Y = ViewInfo.ViewPos.Z + XYZ_dbl.Z * dblTemp2;
			}
			DrawCentre.X = modMath.Clamp_dbl(DrawCentre.X, 0.0D, Terrain.TileSize.X * modProgram.TerrainGridSpacing - 1.0D);
			DrawCentre.Y = modMath.Clamp_dbl(System.Convert.ToDouble(- DrawCentre.Y), 0.0D, Terrain.TileSize.Y * modProgram.TerrainGridSpacing - 1.0D);
			DrawCentreSector.Normal = GetPosSectorNum(new modMath.sXY_int((int) DrawCentre.X, (int) DrawCentre.Y));
			DrawCentreSector.Alignment = GetPosSectorNum(new modMath.sXY_int((int) (DrawCentre.X - modProgram.SectorTileSize * modProgram.TerrainGridSpacing / 2.0D), (int) (DrawCentre.Y - modProgram.SectorTileSize * modProgram.TerrainGridSpacing / 2.0D)));
			
			clsMap.clsDrawSectorObjects DrawObjects = new clsMap.clsDrawSectorObjects();
			DrawObjects.Map = this;
			DrawObjects.UnitTextLabels = new clsTextLabels(64);
			DrawObjects.Start();
			
			XYZ_dbl.X = DrawCentre.X - ViewInfo.ViewPos.X;
			XYZ_dbl.Y = 128 - ViewInfo.ViewPos.Y;
			XYZ_dbl.Z = - DrawCentre.Y - ViewInfo.ViewPos.Z;
			ZNearFar = (float) (XYZ_dbl.GetMagnitude());
			
			GL.Enable(EnableCap.DepthTest);
			GL.MatrixMode(MatrixMode.Projection);
			OpenTK.Matrix4 temp_mat = OpenTK.Matrix4.CreatePerspectiveFieldOfView(ViewInfo.FieldOfViewY, MapView.GLSize_XPerY, ZNearFar / 128.0F, ZNearFar * 128.0F);
			GL.LoadMatrix(ref temp_mat);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			
			Matrix3DMath.MatrixRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, modProgram.SunAngleMatrix, matrixB);
			Matrix3DMath.VectorForwardsRotationByMatrix(matrixB, ref XYZ_dbl);
			light_position[0] = (float) XYZ_dbl.X;
			light_position[1] = (float) XYZ_dbl.Y;
			light_position[2] = System.Convert.ToSingle(- XYZ_dbl.Z);
			light_position[3] = 0.0F;
			GL.Light(LightName.Light0, LightParameter.Position, light_position);
			GL.Light(LightName.Light1, LightParameter.Position, light_position);
			
			GL.Disable(EnableCap.Light0);
			GL.Disable(EnableCap.Light1);
			if (modProgram.Draw_Lighting != modProgram.enumDrawLighting.Off)
			{
				if (modProgram.Draw_Lighting == modProgram.enumDrawLighting.Half)
				{
					GL.Enable(EnableCap.Light0);
				}
				else if (modProgram.Draw_Lighting == modProgram.enumDrawLighting.Normal)
				{
					GL.Enable(EnableCap.Light1);
				}
				GL.Enable(EnableCap.Lighting);
			}
			else
			{
				GL.Disable(EnableCap.Lighting);
			}
			
			dblTemp = 127.5D * HeightMultiplier;
			if (ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(0, 0, dblTemp, ViewCorner0) 
				&& ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(GLSize.X, 0, dblTemp, ViewCorner1) 
				&& ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(GLSize.X, GLSize.Y, dblTemp, ViewCorner2) 
				&& ViewInfo.ScreenXY_Get_ViewPlanePos_ForwardDownOnly(0, GLSize.Y, dblTemp, ViewCorner3))
			{
				ShowMinimapViewPosBox = true;
			}
			else
			{
				ShowMinimapViewPosBox = false;
			}
			
			GL.Rotate((float) (ViewInfo.ViewAngleRPY.Roll / modMath.RadOf1Deg), 0.0F, 0.0F, -1.0F);
			GL.Rotate((float) (ViewInfo.ViewAngleRPY.Pitch / modMath.RadOf1Deg), 1.0F, 0.0F, 0.0F);
			GL.Rotate((float) (ViewInfo.ViewAngleRPY.Yaw / modMath.RadOf1Deg), 0.0F, 1.0F, 0.0F);
			GL.Translate(System.Convert.ToDouble(- ViewInfo.ViewPos.X), System.Convert.ToDouble(- ViewInfo.ViewPos.Y), ViewInfo.ViewPos.Z);
			
			GL.Enable(EnableCap.CullFace);
			
			DebugGLError("Matrix modes");
			
			if (modProgram.Draw_TileTextures)
			{
				GL.Color3(1.0F, 1.0F, 1.0F);
				GL.Enable(EnableCap.Texture2D);
				MapAction = new clsMap.clsDrawCallTerrain();
				MapAction.Map = this;
				modProgram.VisionSectors.PerformActionMapSectors(MapAction, DrawCentreSector);
				GL.Disable(EnableCap.Texture2D);
				
				DebugGLError("Tile textures");
			}
			
			GL.Disable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Lighting);
			
			if (modProgram.Draw_TileWireframe)
			{
				GL.Color3(0.0F, 1.0F, 0.0F);
				GL.LineWidth(1.0F);
				clsMap.clsDrawCallTerrainWireframe DrawCallTerrainWireframe = new clsMap.clsDrawCallTerrainWireframe();
				DrawCallTerrainWireframe.Map = this;
				modProgram.VisionSectors.PerformActionMapSectors(DrawCallTerrainWireframe, DrawCentreSector);
				
				DebugGLError("Wireframe");
			}
			
			//draw tile orientation markers
			
			if (modProgram.DisplayTileOrientation)
			{
				
				GL.Disable(EnableCap.CullFace);
				
				GL.Begin(BeginMode.Triangles);
				GL.Color3(1.0F, 1.0F, 0.0F);
				MapAction = new clsMap.clsDrawTileOrientation();
				MapAction.Map = this;
				modProgram.VisionSectors.PerformActionMapSectors(MapAction, DrawCentreSector);
				GL.End();
				
				GL.Enable(EnableCap.CullFace);
				
				DebugGLError("Tile orientation");
			}
			
			//draw painted texture terrain type markers
			
			sRGB_sng RGB_sng = new sRGB_sng();
			
			clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = ViewInfo.GetMouseOverTerrain();
			
			if (modProgram.Draw_VertexTerrain)
			{
				GL.LineWidth(1.0F);
				clsMap.clsDrawVertexTerrain DrawVertexTerran = new clsMap.clsDrawVertexTerrain();
				DrawVertexTerran.Map = this;
				DrawVertexTerran.ViewAngleMatrix = ViewInfo.ViewAngleMatrix;
				modProgram.VisionSectors.PerformActionMapSectors(DrawVertexTerran, DrawCentreSector);
				DebugGLError("Terrain type markers");
			}
			
			SelectionLabel.Text = "";
			
			if (Selected_Area_VertexA != null)
			{
				DrawIt = false;
				if (Selected_Area_VertexB != null)
				{
					//area is selected
					modMath.ReorderXY(Selected_Area_VertexA.XY, Selected_Area_VertexB.XY, StartXY, FinishXY);
					XYZ_dbl.X = Selected_Area_VertexB.X * modProgram.TerrainGridSpacing - ViewInfo.ViewPos.X;
					XYZ_dbl.Z = - Selected_Area_VertexB.Y * modProgram.TerrainGridSpacing - ViewInfo.ViewPos.Z;
					XYZ_dbl.Y = GetVertexAltitude(Selected_Area_VertexB.XY) - ViewInfo.ViewPos.Y;
					DrawIt = true;
				}
				else if (modTools.Tool == modTools.Tools.TerrainSelect)
				{
					if (MouseOverTerrain != null)
					{
						//selection is changing under pointer
						modMath.ReorderXY(Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal, StartXY, FinishXY);
						XYZ_dbl.X = MouseOverTerrain.Vertex.Normal.X * modProgram.TerrainGridSpacing - ViewInfo.ViewPos.X;
						XYZ_dbl.Z = - MouseOverTerrain.Vertex.Normal.Y * modProgram.TerrainGridSpacing - ViewInfo.ViewPos.Z;
						XYZ_dbl.Y = GetVertexAltitude(MouseOverTerrain.Vertex.Normal) - ViewInfo.ViewPos.Y;
						DrawIt = true;
					}
				}
				if (DrawIt)
				{
					Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, XYZ_dbl, ref XYZ_dbl2);
					if (ViewInfo.Pos_Get_Screen_XY(XYZ_dbl2, ScreenPos))
					{
						if (ScreenPos.X >= 0 & ScreenPos.X <= GLSize.X & ScreenPos.Y >= 0 & ScreenPos.Y <= GLSize.Y)
						{
							SelectionLabel.Colour.Red = 1.0F;
							SelectionLabel.Colour.Green = 1.0F;
							SelectionLabel.Colour.Blue = 1.0F;
							SelectionLabel.Colour.Alpha = 1.0F;
							SelectionLabel.TextFont = modProgram.UnitLabelFont;
							SelectionLabel.SizeY = modSettings.Settings.FontSize;
							SelectionLabel.Pos = ScreenPos;
							SelectionLabel.Text = FinishXY.X - StartXY.X + "x" + System.Convert.ToString(FinishXY.Y - StartXY.Y);
						}
					}
					GL.LineWidth(3.0F);
					clsMap.clsDrawTileAreaOutline DrawSelection = new clsMap.clsDrawTileAreaOutline();
					DrawSelection.Map = this;
					DrawSelection.StartXY = StartXY;
					DrawSelection.FinishXY = FinishXY;
					DrawSelection.Colour = new sRGBA_sng(1.0F, 1.0F, 1.0F, 1.0F);
					DrawSelection.ActionPerform();
				}
				
				DebugGLError("Terrain selection box");
			}
			
			if (modTools.Tool == modTools.Tools.TerrainSelect)
			{
				if (MouseOverTerrain != null)
				{
					//draw mouseover vertex
					GL.LineWidth(3.0F);
					
					Vertex0.X = MouseOverTerrain.Vertex.Normal.X * modProgram.TerrainGridSpacing;
					Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y].Height * HeightMultiplier);
					Vertex0.Z = - MouseOverTerrain.Vertex.Normal.Y * modProgram.TerrainGridSpacing;
					GL.Begin(BeginMode.Lines);
					GL.Color3(1.0F, 1.0F, 1.0F);
					GL.Vertex3(Vertex0.X - 8.0D, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
					GL.Vertex3(Vertex0.X + 8.0D, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
					GL.Vertex3(Vertex0.X, Vertex0.Y, - Vertex0.Z - 8.0D);
					GL.Vertex3(Vertex0.X, Vertex0.Y, - Vertex0.Z + 8.0D);
					GL.End();
				}
				DebugGLError("Terrain selection vertex");
			}
			
			clsGateway Gateway = default(clsGateway);
			
			if (modProgram.Draw_Gateways)
			{
				GL.LineWidth(2.0F);
				foreach (clsGateway tempLoopVar_Gateway in Gateways)
				{
					Gateway = tempLoopVar_Gateway;
					if (Gateway.PosA.X == Gateway.PosB.X)
					{
						if (Gateway.PosA.Y <= Gateway.PosB.Y)
						{
							C = Gateway.PosA.Y;
							D = Gateway.PosB.Y;
						}
						else
						{
							C = Gateway.PosB.Y;
							D = Gateway.PosA.Y;
						}
						X2 = Gateway.PosA.X;
						for (Y2 = C; Y2 <= D; Y2++)
						{
							Vertex0.X = X2 * modProgram.TerrainGridSpacing;
							Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
							Vertex0.Z = - Y2 * modProgram.TerrainGridSpacing;
							Vertex1.X = (X2 + 1) * modProgram.TerrainGridSpacing;
							Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
							Vertex1.Z = - Y2 * modProgram.TerrainGridSpacing;
							Vertex2.X = X2 * modProgram.TerrainGridSpacing;
							Vertex2.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
							Vertex2.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
							Vertex3.X = (X2 + 1) * modProgram.TerrainGridSpacing;
							Vertex3.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
							Vertex3.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
							GL.Begin(BeginMode.LineLoop);
							GL.Color3(0.75F, 1.0F, 0.0F);
							GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
							GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
							GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToDouble(- Vertex3.Z));
							GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToDouble(- Vertex2.Z));
							GL.End();
						}
					}
					else if (Gateway.PosA.Y == Gateway.PosB.Y)
					{
						if (Gateway.PosA.X <= Gateway.PosB.X)
						{
							C = Gateway.PosA.X;
							D = Gateway.PosB.X;
						}
						else
						{
							C = Gateway.PosB.X;
							D = Gateway.PosA.X;
						}
						Y2 = Gateway.PosA.Y;
						for (X2 = C; X2 <= D; X2++)
						{
							Vertex0.X = X2 * modProgram.TerrainGridSpacing;
							Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
							Vertex0.Z = - Y2 * modProgram.TerrainGridSpacing;
							Vertex1.X = (X2 + 1) * modProgram.TerrainGridSpacing;
							Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
							Vertex1.Z = - Y2 * modProgram.TerrainGridSpacing;
							Vertex2.X = X2 * modProgram.TerrainGridSpacing;
							Vertex2.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
							Vertex2.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
							Vertex3.X = (X2 + 1) * modProgram.TerrainGridSpacing;
							Vertex3.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
							Vertex3.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
							GL.Begin(BeginMode.LineLoop);
							GL.Color3(0.75F, 1.0F, 0.0F);
							GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
							GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
							GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToDouble(- Vertex3.Z));
							GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToDouble(- Vertex2.Z));
							GL.End();
						}
					}
					else
					{
						//draw invalid gateways as red tile borders
						X2 = Gateway.PosA.X;
						Y2 = Gateway.PosA.Y;
						
						Vertex0.X = X2 * modProgram.TerrainGridSpacing;
						Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
						Vertex0.Z = - Y2 * modProgram.TerrainGridSpacing;
						Vertex1.X = (X2 + 1) * modProgram.TerrainGridSpacing;
						Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
						Vertex1.Z = - Y2 * modProgram.TerrainGridSpacing;
						Vertex2.X = X2 * modProgram.TerrainGridSpacing;
						Vertex2.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
						Vertex2.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
						Vertex3.X = (X2 + 1) * modProgram.TerrainGridSpacing;
						Vertex3.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
						Vertex3.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
						GL.Begin(BeginMode.LineLoop);
						GL.Color3(1.0F, 0.0F, 0.0F);
						GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
						GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
						GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToDouble(- Vertex3.Z));
						GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToDouble(- Vertex2.Z));
						GL.End();
						
						X2 = Gateway.PosB.X;
						Y2 = Gateway.PosB.Y;
						
						Vertex0.X = X2 * modProgram.TerrainGridSpacing;
						Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
						Vertex0.Z = - Y2 * modProgram.TerrainGridSpacing;
						Vertex1.X = (X2 + 1) * modProgram.TerrainGridSpacing;
						Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
						Vertex1.Z = - Y2 * modProgram.TerrainGridSpacing;
						Vertex2.X = X2 * modProgram.TerrainGridSpacing;
						Vertex2.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
						Vertex2.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
						Vertex3.X = (X2 + 1) * modProgram.TerrainGridSpacing;
						Vertex3.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
						Vertex3.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
						GL.Begin(BeginMode.LineLoop);
						GL.Color3(1.0F, 0.0F, 0.0F);
						GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
						GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
						GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToDouble(- Vertex3.Z));
						GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToDouble(- Vertex2.Z));
						GL.End();
					}
				}
				DebugGLError("Gateways");
			}
			
			if (MouseOverTerrain != null)
			{
				
				if (modTools.Tool == modTools.Tools.ObjectSelect)
				{
					if (Unit_Selected_Area_VertexA != null)
					{
						//selection is changing under pointer
						modMath.ReorderXY(Unit_Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal, StartXY, FinishXY);
						GL.LineWidth(2.0F);
						GL.Color3(0.0F, 1.0F, 1.0F);
						for (X = StartXY.X; X <= FinishXY.X - 1; X++)
						{
							Vertex0.X = X * modProgram.TerrainGridSpacing;
							Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[X, StartXY.Y].Height * HeightMultiplier);
							Vertex0.Z = - StartXY.Y * modProgram.TerrainGridSpacing;
							Vertex1.X = (X + 1) * modProgram.TerrainGridSpacing;
							Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[X + 1, StartXY.Y].Height * HeightMultiplier);
							Vertex1.Z = - StartXY.Y * modProgram.TerrainGridSpacing;
							GL.Begin(BeginMode.Lines);
							GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
							GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
							GL.End();
						}
						for (X = StartXY.X; X <= FinishXY.X - 1; X++)
						{
							Vertex0.X = X * modProgram.TerrainGridSpacing;
							Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[X, FinishXY.Y].Height * HeightMultiplier);
							Vertex0.Z = - FinishXY.Y * modProgram.TerrainGridSpacing;
							Vertex1.X = (X + 1) * modProgram.TerrainGridSpacing;
							Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[X + 1, FinishXY.Y].Height * HeightMultiplier);
							Vertex1.Z = - FinishXY.Y * modProgram.TerrainGridSpacing;
							GL.Begin(BeginMode.Lines);
							GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
							GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
							GL.End();
						}
						for (Y = StartXY.Y; Y <= FinishXY.Y - 1; Y++)
						{
							Vertex0.X = StartXY.X * modProgram.TerrainGridSpacing;
							Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[StartXY.X, Y].Height * HeightMultiplier);
							Vertex0.Z = - Y * modProgram.TerrainGridSpacing;
							Vertex1.X = StartXY.X * modProgram.TerrainGridSpacing;
							Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[StartXY.X, Y + 1].Height * HeightMultiplier);
							Vertex1.Z = - (Y + 1) * modProgram.TerrainGridSpacing;
							GL.Begin(BeginMode.Lines);
							GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
							GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
							GL.End();
						}
						for (Y = StartXY.Y; Y <= FinishXY.Y - 1; Y++)
						{
							Vertex0.X = FinishXY.X * modProgram.TerrainGridSpacing;
							Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[FinishXY.X, Y].Height * HeightMultiplier);
							Vertex0.Z = - Y * modProgram.TerrainGridSpacing;
							Vertex1.X = FinishXY.X * modProgram.TerrainGridSpacing;
							Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[FinishXY.X, Y + 1].Height * HeightMultiplier);
							Vertex1.Z = - (Y + 1) * modProgram.TerrainGridSpacing;
							GL.Begin(BeginMode.Lines);
							GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
							GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
							GL.End();
						}
						
						DebugGLError("Object selection box");
					}
					else
					{
						GL.LineWidth(2.0F);
						GL.Color3(0.0F, 1.0F, 1.0F);
						GL.Begin(BeginMode.Lines);
						GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X - 16.0D, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y - 16.0D);
						GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X + 16.0D, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y + 16.0D);
						GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X + 16.0D, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y - 16.0D);
						GL.Vertex3(MouseOverTerrain.Pos.Horizontal.X - 16.0D, MouseOverTerrain.Pos.Altitude, MouseOverTerrain.Pos.Horizontal.Y + 16.0D);
						GL.End();
						
						DebugGLError("Mouse over position");
					}
				}
				
				if (modTools.Tool == modTools.Tools.RoadPlace)
				{
					GL.LineWidth(2.0F);
					
					if (MouseOverTerrain.Side_IsV)
					{
						Vertex0.X = MouseOverTerrain.Side_Num.X * modProgram.TerrainGridSpacing;
						Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y].Height * HeightMultiplier);
						Vertex0.Z = - MouseOverTerrain.Side_Num.Y * modProgram.TerrainGridSpacing;
						Vertex1.X = MouseOverTerrain.Side_Num.X * modProgram.TerrainGridSpacing;
						Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y + 1].Height * HeightMultiplier);
						Vertex1.Z = - (MouseOverTerrain.Side_Num.Y + 1) * modProgram.TerrainGridSpacing;
					}
					else
					{
						Vertex0.X = MouseOverTerrain.Side_Num.X * modProgram.TerrainGridSpacing;
						Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X, MouseOverTerrain.Side_Num.Y].Height * HeightMultiplier);
						Vertex0.Z = - MouseOverTerrain.Side_Num.Y * modProgram.TerrainGridSpacing;
						Vertex1.X = (MouseOverTerrain.Side_Num.X + 1) * modProgram.TerrainGridSpacing;
						Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Side_Num.X + 1, MouseOverTerrain.Side_Num.Y].Height * HeightMultiplier);
						Vertex1.Z = - MouseOverTerrain.Side_Num.Y * modProgram.TerrainGridSpacing;
					}
					
					GL.Begin(BeginMode.Lines);
					GL.Color3(0.0F, 1.0F, 1.0F);
					GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
					GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
					GL.End();
					
					DebugGLError("Road place brush");
				}
				else if (modTools.Tool == modTools.Tools.RoadLines || modTools.Tool == modTools.Tools.Gateways || modTools.Tool == modTools.Tools.ObjectLines)
				{
					GL.LineWidth(2.0F);
					
					if (Selected_Tile_A != null)
					{
						X2 = Selected_Tile_A.X;
						Y2 = Selected_Tile_A.Y;
						
						Vertex0.X = X2 * modProgram.TerrainGridSpacing;
						Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
						Vertex0.Z = - Y2 * modProgram.TerrainGridSpacing;
						Vertex1.X = (X2 + 1) * modProgram.TerrainGridSpacing;
						Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
						Vertex1.Z = - Y2 * modProgram.TerrainGridSpacing;
						Vertex2.X = X2 * modProgram.TerrainGridSpacing;
						Vertex2.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
						Vertex2.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
						Vertex3.X = (X2 + 1) * modProgram.TerrainGridSpacing;
						Vertex3.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
						Vertex3.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
						GL.Begin(BeginMode.LineLoop);
						GL.Color3(0.0F, 1.0F, 1.0F);
						GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
						GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
						GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToDouble(- Vertex3.Z));
						GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToDouble(- Vertex2.Z));
						GL.End();
						
						if (MouseOverTerrain.Tile.Normal.X == Selected_Tile_A.X)
						{
							if (MouseOverTerrain.Tile.Normal.Y <= Selected_Tile_A.Y)
							{
								A = MouseOverTerrain.Tile.Normal.Y;
								B = Selected_Tile_A.Y;
							}
							else
							{
								A = Selected_Tile_A.Y;
								B = MouseOverTerrain.Tile.Normal.Y;
							}
							X2 = Selected_Tile_A.X;
							for (Y2 = A; Y2 <= B; Y2++)
							{
								Vertex0.X = X2 * modProgram.TerrainGridSpacing;
								Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
								Vertex0.Z = - Y2 * modProgram.TerrainGridSpacing;
								Vertex1.X = (X2 + 1) * modProgram.TerrainGridSpacing;
								Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
								Vertex1.Z = - Y2 * modProgram.TerrainGridSpacing;
								Vertex2.X = X2 * modProgram.TerrainGridSpacing;
								Vertex2.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
								Vertex2.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
								Vertex3.X = (X2 + 1) * modProgram.TerrainGridSpacing;
								Vertex3.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
								Vertex3.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
								GL.Begin(BeginMode.LineLoop);
								GL.Color3(0.0F, 1.0F, 1.0F);
								GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
								GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
								GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToDouble(- Vertex3.Z));
								GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToDouble(- Vertex2.Z));
								GL.End();
							}
						}
						else if (MouseOverTerrain.Tile.Normal.Y == Selected_Tile_A.Y)
						{
							if (MouseOverTerrain.Tile.Normal.X <= Selected_Tile_A.X)
							{
								A = MouseOverTerrain.Tile.Normal.X;
								B = Selected_Tile_A.X;
							}
							else
							{
								A = Selected_Tile_A.X;
								B = MouseOverTerrain.Tile.Normal.X;
							}
							Y2 = Selected_Tile_A.Y;
							for (X2 = A; X2 <= B; X2++)
							{
								Vertex0.X = X2 * modProgram.TerrainGridSpacing;
								Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
								Vertex0.Z = - Y2 * modProgram.TerrainGridSpacing;
								Vertex1.X = (X2 + 1) * modProgram.TerrainGridSpacing;
								Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
								Vertex1.Z = - Y2 * modProgram.TerrainGridSpacing;
								Vertex2.X = X2 * modProgram.TerrainGridSpacing;
								Vertex2.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
								Vertex2.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
								Vertex3.X = (X2 + 1) * modProgram.TerrainGridSpacing;
								Vertex3.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
								Vertex3.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
								GL.Begin(BeginMode.LineLoop);
								GL.Color3(0.0F, 1.0F, 1.0F);
								GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
								GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
								GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToDouble(- Vertex3.Z));
								GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToDouble(- Vertex2.Z));
								GL.End();
							}
						}
					}
					else
					{
						X2 = MouseOverTerrain.Tile.Normal.X;
						Y2 = MouseOverTerrain.Tile.Normal.Y;
						
						Vertex0.X = X2 * modProgram.TerrainGridSpacing;
						Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2].Height * HeightMultiplier);
						Vertex0.Z = - Y2 * modProgram.TerrainGridSpacing;
						Vertex1.X = (X2 + 1) * modProgram.TerrainGridSpacing;
						Vertex1.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2].Height * HeightMultiplier);
						Vertex1.Z = - Y2 * modProgram.TerrainGridSpacing;
						Vertex2.X = X2 * modProgram.TerrainGridSpacing;
						Vertex2.Y = System.Convert.ToDouble(Terrain.Vertices[X2, Y2 + 1].Height * HeightMultiplier);
						Vertex2.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
						Vertex3.X = (X2 + 1) * modProgram.TerrainGridSpacing;
						Vertex3.Y = System.Convert.ToDouble(Terrain.Vertices[X2 + 1, Y2 + 1].Height * HeightMultiplier);
						Vertex3.Z = - (Y2 + 1) * modProgram.TerrainGridSpacing;
						GL.Begin(BeginMode.LineLoop);
						GL.Color3(0.0F, 1.0F, 1.0F);
						GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
						GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
						GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToDouble(- Vertex3.Z));
						GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToDouble(- Vertex2.Z));
						GL.End();
					}
					DebugGLError("Line brush");
				}
				
				//draw mouseover tiles
				
				clsBrush ToolBrush = default(clsBrush);
				
				if (modTools.Tool == modTools.Tools.TextureBrush)
				{
					ToolBrush = modProgram.TextureBrush;
				}
				else if (modTools.Tool == modTools.Tools.CliffBrush)
				{
					ToolBrush = modProgram.CliffBrush;
				}
				else if (modTools.Tool == modTools.Tools.CliffRemove)
				{
					ToolBrush = modProgram.CliffBrush;
				}
				else if (modTools.Tool == modTools.Tools.RoadRemove)
				{
					ToolBrush = modProgram.CliffBrush;
				}
				else
				{
					ToolBrush = null;
				}
				
				if (ToolBrush != null)
				{
					GL.LineWidth(2.0F);
					clsMap.clsDrawTileOutline DrawTileOutline = new clsMap.clsDrawTileOutline();
					DrawTileOutline.Map = this;
					DrawTileOutline.Colour.Red = 0.0F;
					DrawTileOutline.Colour.Green = 1.0F;
					DrawTileOutline.Colour.Blue = 1.0F;
					DrawTileOutline.Colour.Alpha = 1.0F;
					ToolBrush.PerformActionMapTiles(DrawTileOutline, MouseOverTerrain.Tile);
					
					DebugGLError("Brush tiles");
				}
				
				//draw mouseover vertex
				if (modTools.Tool == modTools.Tools.TerrainFill)
				{
					GL.LineWidth(2.0F);
					
					Vertex0.X = MouseOverTerrain.Vertex.Normal.X * modProgram.TerrainGridSpacing;
					Vertex0.Y = System.Convert.ToDouble(Terrain.Vertices[MouseOverTerrain.Vertex.Normal.X, MouseOverTerrain.Vertex.Normal.Y].Height * HeightMultiplier);
					Vertex0.Z = - MouseOverTerrain.Vertex.Normal.Y * modProgram.TerrainGridSpacing;
					GL.Begin(BeginMode.Lines);
					GL.Color3(0.0F, 1.0F, 1.0F);
					GL.Vertex3(Vertex0.X - 8.0D, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
					GL.Vertex3(Vertex0.X + 8.0D, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
					GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z - 8.0D));
					GL.Vertex3(Vertex0.X, Vertex0.Y, - Vertex0.Z + 8.0D);
					GL.End();
					
					DebugGLError("Mouse over vertex");
				}
				
				if (modTools.Tool == modTools.Tools.TerrainBrush)
				{
					ToolBrush = modProgram.TerrainBrush;
				}
				else if (modTools.Tool == modTools.Tools.HeightSetBrush)
				{
					ToolBrush = modProgram.HeightBrush;
				}
				else if (modTools.Tool == modTools.Tools.HeightChangeBrush)
				{
					ToolBrush = modProgram.HeightBrush;
				}
				else if (modTools.Tool == modTools.Tools.HeightSmoothBrush)
				{
					ToolBrush = modProgram.HeightBrush;
				}
				else
				{
					ToolBrush = null;
				}
				
				if (ToolBrush != null)
				{
					GL.LineWidth(2.0F);
					clsMap.clsDrawVertexMarker DrawVertexMarker = new clsMap.clsDrawVertexMarker();
					DrawVertexMarker.Map = this;
					DrawVertexMarker.Colour.Red = 0.0F;
					DrawVertexMarker.Colour.Green = 1.0F;
					DrawVertexMarker.Colour.Blue = 1.0F;
					DrawVertexMarker.Colour.Alpha = 1.0F;
					ToolBrush.PerformActionMapVertices(DrawVertexMarker, MouseOverTerrain.Vertex);
					
					DebugGLError("Brush vertices");
				}
			}
			
			GL.Enable(EnableCap.DepthTest);
			
			GL.Disable(EnableCap.CullFace);
			
			GL.LoadIdentity();
			GL.Rotate((float) (ViewInfo.ViewAngleRPY.Roll / modMath.RadOf1Deg), 0.0F, 0.0F, -1.0F);
			GL.Rotate((float) (ViewInfo.ViewAngleRPY.Pitch / modMath.RadOf1Deg), 1.0F, 0.0F, 0.0F);
			GL.Rotate((float) (ViewInfo.ViewAngleRPY.Yaw / modMath.RadOf1Deg), 0.0F, 1.0F, 0.0F);
			
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			
			DebugGLError("Object matrix modes");
			
			if (modProgram.Draw_Units)
			{
				GL.Color3(1.0F, 1.0F, 1.0F);
				GL.Enable(EnableCap.Texture2D);
				modProgram.VisionSectors.PerformActionMapSectors(DrawObjects, DrawCentreSector);
				GL.Disable(EnableCap.Texture2D);
				DebugGLError("Objects");
			}
			
			if (MouseOverTerrain != null)
			{
				GL.Enable(EnableCap.Texture2D);
				if (modTools.Tool == modTools.Tools.ObjectPlace)
				{
					clsUnitType placeObject = modMain.frmMainInstance.SingleSelectedObjectType;
					if (placeObject != null)
					{
						int Rotation = 0;
						try
						{
							modIO.InvariantParse_int(modMain.frmMainInstance.txtNewObjectRotation.Text, ref Rotation);
							if (Rotation < 0 | Rotation > 359)
							{
								Rotation = 0;
							}
						}
						catch
						{
							Rotation = 0;
						}
						WorldPos = TileAlignedPosFromMapPos(MouseOverTerrain.Pos.Horizontal, placeObject.get_GetFootprintSelected(Rotation));
						GL.PushMatrix();
						GL.Translate(WorldPos.Horizontal.X - ViewInfo.ViewPos.X, WorldPos.Altitude - ViewInfo.ViewPos.Y + 2.0D, ViewInfo.ViewPos.Z + WorldPos.Horizontal.Y);
						placeObject.GLDraw(Rotation);
						GL.PopMatrix();
					}
				}
				GL.Disable(EnableCap.Texture2D);
				DebugGLError("Mouse over object");
			}
			
			GL.Disable(EnableCap.DepthTest);
			
			clsTextLabels ScriptMarkerTextLabels = new clsTextLabels(256);
			clsTextLabel TextLabel = default(clsTextLabel);
			if (modProgram.Draw_ScriptMarkers)
			{
				clsScriptPosition ScriptPosition = default(clsScriptPosition);
				clsScriptArea ScriptArea = default(clsScriptArea);
				GL.PushMatrix();
				GL.Translate(System.Convert.ToDouble(- ViewInfo.ViewPos.X), System.Convert.ToDouble(- ViewInfo.ViewPos.Y), ViewInfo.ViewPos.Z);
				foreach (clsScriptPosition tempLoopVar_ScriptPosition in ScriptPositions)
				{
					ScriptPosition = tempLoopVar_ScriptPosition;
					ScriptPosition.GLDraw();
				}
				foreach (clsScriptArea tempLoopVar_ScriptArea in ScriptAreas)
				{
					ScriptArea = tempLoopVar_ScriptArea;
					ScriptArea.GLDraw();
				}
				foreach (clsScriptPosition tempLoopVar_ScriptPosition in ScriptPositions)
				{
					ScriptPosition = tempLoopVar_ScriptPosition;
					if (ScriptMarkerTextLabels.AtMaxCount())
					{
						break;
					}
					XYZ_dbl.X = ScriptPosition.PosX - ViewInfo.ViewPos.X;
					XYZ_dbl.Z = - ScriptPosition.PosY - ViewInfo.ViewPos.Z;
					XYZ_dbl.Y = GetTerrainHeight(new modMath.sXY_int(ScriptPosition.PosX, ScriptPosition.PosY)) - ViewInfo.ViewPos.Y;
					Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, XYZ_dbl, ref XYZ_dbl2);
					if (ViewInfo.Pos_Get_Screen_XY(XYZ_dbl2, ScreenPos))
					{
						if (ScreenPos.X >= 0 & ScreenPos.X <= GLSize.X & ScreenPos.Y >= 0 & ScreenPos.Y <= GLSize.Y)
						{
							TextLabel = new clsTextLabel();
							TextLabel.Colour.Red = 1.0F;
							TextLabel.Colour.Green = 1.0F;
							TextLabel.Colour.Blue = 0.5F;
							TextLabel.Colour.Alpha = 0.75F;
							TextLabel.TextFont = modProgram.UnitLabelFont;
							TextLabel.SizeY = modSettings.Settings.FontSize;
							TextLabel.Pos = ScreenPos;
							TextLabel.Text = ScriptPosition.Label;
							ScriptMarkerTextLabels.Add(TextLabel);
						}
					}
				}
				DebugGLError("Script positions");
				foreach (clsScriptArea tempLoopVar_ScriptArea in ScriptAreas)
				{
					ScriptArea = tempLoopVar_ScriptArea;
					if (ScriptMarkerTextLabels.AtMaxCount())
					{
						break;
					}
					XYZ_dbl.X = ScriptArea.PosAX - ViewInfo.ViewPos.X;
					XYZ_dbl.Z = - ScriptArea.PosAY - ViewInfo.ViewPos.Z;
					XYZ_dbl.Y = GetTerrainHeight(new modMath.sXY_int(ScriptArea.PosAX, ScriptArea.PosAY)) - ViewInfo.ViewPos.Y;
					Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, XYZ_dbl, ref XYZ_dbl2);
					if (ViewInfo.Pos_Get_Screen_XY(XYZ_dbl2, ScreenPos))
					{
						if (ScreenPos.X >= 0 & ScreenPos.X <= GLSize.X & ScreenPos.Y >= 0 & ScreenPos.Y <= GLSize.Y)
						{
							TextLabel = new clsTextLabel();
							TextLabel.Colour.Red = 1.0F;
							TextLabel.Colour.Green = 1.0F;
							TextLabel.Colour.Blue = 0.5F;
							TextLabel.Colour.Alpha = 0.75F;
							TextLabel.TextFont = modProgram.UnitLabelFont;
							TextLabel.SizeY = modSettings.Settings.FontSize;
							TextLabel.Pos = ScreenPos;
							TextLabel.Text = ScriptArea.Label;
							ScriptMarkerTextLabels.Add(TextLabel);
						}
					}
				}
				GL.PopMatrix();
				
				DebugGLError("Script areas");
			}
			
			clsTextLabels MessageTextLabels = new clsTextLabels(24);
			
			B = 0;
			for (A = Math.Max(Messages.Count - MessageTextLabels.MaxCount, 0); A <= Messages.Count - 1; A++)
			{
				if (!MessageTextLabels.AtMaxCount())
				{
					TextLabel = new clsTextLabel();
					TextLabel.Colour.Red = 0.875F;
					TextLabel.Colour.Green = 0.875F;
					TextLabel.Colour.Blue = 1.0F;
					TextLabel.Colour.Alpha = 1.0F;
					TextLabel.TextFont = modProgram.UnitLabelFont;
					TextLabel.SizeY = modSettings.Settings.FontSize;
					TextLabel.Pos.X = 32 + MinimapSizeXY.X;
					TextLabel.Pos.Y = 32 + (int) (Math.Ceiling((decimal) (B * TextLabel.SizeY)));
					TextLabel.Text = System.Convert.ToString(Messages[A].Text);
					MessageTextLabels.Add(TextLabel);
					B++;
				}
			}
			
			//draw unit selection
			
			GL.Begin(BeginMode.Quads);
			foreach (clsMap.clsUnit tempLoopVar_Unit in SelectedUnits)
			{
				Unit = tempLoopVar_Unit;
				Footprint = Unit.Type.get_GetFootprintSelected(Unit.Rotation);
				RGB_sng = GetUnitGroupColour(Unit.UnitGroup);
				ColourA = new sRGBA_sng((1.0F + RGB_sng.Red) / 2.0F, (1.0F + RGB_sng.Green) / 2.0F, (1.0F + RGB_sng.Blue) / 2.0F, 0.75F);
				ColourB = new sRGBA_sng(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue, 0.75F);
				DrawUnitRectangle(Unit, 8, ColourA, ColourB);
			}
			if (MouseOverTerrain != null)
			{
				foreach (clsMap.clsUnit tempLoopVar_Unit in MouseOverTerrain.Units)
				{
					Unit = tempLoopVar_Unit;
					if (Unit != null && modTools.Tool == modTools.Tools.ObjectSelect)
					{
						RGB_sng = GetUnitGroupColour(Unit.UnitGroup);
						GL.Color4((0.5F + RGB_sng.Red) / 1.5F, (0.5F + RGB_sng.Green) / 1.5F, (0.5F + RGB_sng.Blue) / 1.5F, 0.75F);
						Footprint = Unit.Type.get_GetFootprintSelected(Unit.Rotation);
						ColourA = new sRGBA_sng((1.0F + RGB_sng.Red) / 2.0F, (1.0F + RGB_sng.Green) / 2.0F, (1.0F + RGB_sng.Blue) / 2.0F, 0.75F);
						ColourB = new sRGBA_sng(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue, 0.875F);
						DrawUnitRectangle(Unit, 16, ColourA, ColourB);
					}
				}
			}
			GL.End();
			
			DebugGLError("Unit selection");
			
			GL.MatrixMode(MatrixMode.Projection);
			OpenTK.Matrix4 temp_mat2 = OpenTK.Matrix4.CreateOrthographicOffCenter(0.0F, GLSize.X, GLSize.Y, 0.0F, -1.0F, 1.0F);
			GL.LoadMatrix(ref temp_mat2);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			
			DebugGLError("Text label matrix modes");
			
			GL.Enable(EnableCap.Texture2D);
			
			ScriptMarkerTextLabels.Draw();
			DrawObjects.UnitTextLabels.Draw();
			SelectionLabel.Draw();
			MessageTextLabels.Draw();
			
			DebugGLError("Text labels");
			
			GL.Disable(EnableCap.Texture2D);
			
			GL.Disable(EnableCap.Blend);
			
			//draw minimap
			
			GL.MatrixMode(MatrixMode.Projection);
			OpenTK.Matrix4 temp_mat3 = OpenTK.Matrix4.CreateOrthographicOffCenter(0.0F, GLSize.X, 0.0F, GLSize.Y, -1.0F, 1.0F);
			GL.LoadMatrix(ref temp_mat3);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			
			DebugGLError("Minimap matrix modes");
			
			if (Minimap_Texture_Size > 0 & ViewInfo.Tiles_Per_Minimap_Pixel > 0.0D)
			{
				
				GL.Translate(0.0F, GLSize.Y - MinimapSizeXY.Y, 0.0F);
				
				XYZ_dbl.X = Terrain.TileSize.X / Minimap_Texture_Size;
				XYZ_dbl.Z = Terrain.TileSize.Y / Minimap_Texture_Size;
				
				if (Minimap_GLTexture > 0)
				{
					
					GL.Enable(EnableCap.Texture2D);
					GL.BindTexture(TextureTarget.Texture2D, Minimap_GLTexture);
					GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Decal);
					
					GL.Begin(BeginMode.Quads);
					
					GL.TexCoord2(0.0F, 0.0F);
					GL.Vertex2(0, MinimapSizeXY.Y);
					
					GL.TexCoord2((float) XYZ_dbl.X, 0.0F);
					GL.Vertex2(MinimapSizeXY.X, MinimapSizeXY.Y);
					
					GL.TexCoord2((float) XYZ_dbl.X, (float) XYZ_dbl.Z);
					GL.Vertex2(MinimapSizeXY.X, 0);
					
					GL.TexCoord2(0.0F, (float) XYZ_dbl.Z);
					GL.Vertex2(0, 0);
					
					GL.End();
					
					GL.Disable(EnableCap.Texture2D);
					
					DebugGLError("Minimap");
				}
				
				//draw minimap border
				
				GL.LineWidth(1.0F);
				GL.Begin(BeginMode.Lines);
				GL.Color3(0.75F, 0.75F, 0.75F);
				GL.Vertex2(MinimapSizeXY.X, 0.0F);
				GL.Vertex2(MinimapSizeXY.X, MinimapSizeXY.Y);
				GL.Vertex2(0.0F, 0.0F);
				GL.Vertex2(MinimapSizeXY.X, 0.0F);
				GL.End();
				
				DebugGLError("Minimap border");
				
				//draw minimap view pos box
				
				if (ShowMinimapViewPosBox)
				{
					dblTemp = modProgram.TerrainGridSpacing * ViewInfo.Tiles_Per_Minimap_Pixel;
					
					PosA.X = ViewCorner0.X / dblTemp;
					PosA.Y = MinimapSizeXY.Y + ViewCorner0.Y / dblTemp;
					PosB.X = ViewCorner1.X / dblTemp;
					PosB.Y = MinimapSizeXY.Y + ViewCorner1.Y / dblTemp;
					PosC.X = ViewCorner2.X / dblTemp;
					PosC.Y = MinimapSizeXY.Y + ViewCorner2.Y / dblTemp;
					PosD.X = ViewCorner3.X / dblTemp;
					PosD.Y = MinimapSizeXY.Y + ViewCorner3.Y / dblTemp;
					
					GL.LineWidth(1.0F);
					GL.Begin(BeginMode.LineLoop);
					GL.Color3(1.0F, 1.0F, 1.0F);
					GL.Vertex2(PosA.X, PosA.Y);
					GL.Vertex2(PosB.X, PosB.Y);
					GL.Vertex2(PosC.X, PosC.Y);
					GL.Vertex2(PosD.X, PosD.Y);
					GL.End();
					
					DebugGLError("Minimap view position polygon");
				}
				
				if (Selected_Area_VertexA != null)
				{
					DrawIt = false;
					if (Selected_Area_VertexB != null)
					{
						//area is selected
						modMath.ReorderXY(Selected_Area_VertexA.XY, Selected_Area_VertexB.XY, StartXY, FinishXY);
						DrawIt = true;
					}
					else if (modTools.Tool == modTools.Tools.TerrainSelect)
					{
						if (MouseOverTerrain != null)
						{
							//selection is changing under mouse
							modMath.ReorderXY(Selected_Area_VertexA.XY, MouseOverTerrain.Vertex.Normal, StartXY, FinishXY);
							DrawIt = true;
						}
					}
					if (DrawIt)
					{
						GL.LineWidth(1.0F);
						PosA.X = StartXY.X / ViewInfo.Tiles_Per_Minimap_Pixel;
						PosA.Y = MinimapSizeXY.Y - StartXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel;
						PosB.X = FinishXY.X / ViewInfo.Tiles_Per_Minimap_Pixel;
						PosB.Y = MinimapSizeXY.Y - StartXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel;
						PosC.X = FinishXY.X / ViewInfo.Tiles_Per_Minimap_Pixel;
						PosC.Y = MinimapSizeXY.Y - FinishXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel;
						PosD.X = StartXY.X / ViewInfo.Tiles_Per_Minimap_Pixel;
						PosD.Y = MinimapSizeXY.Y - FinishXY.Y / ViewInfo.Tiles_Per_Minimap_Pixel;
						GL.Begin(BeginMode.LineLoop);
						GL.Color3(1.0F, 1.0F, 1.0F);
						GL.Vertex2(PosA.X, PosA.Y);
						GL.Vertex2(PosB.X, PosB.Y);
						GL.Vertex2(PosC.X, PosC.Y);
						GL.Vertex2(PosD.X, PosD.Y);
						GL.End();
						
						DebugGLError("Minimap selection box");
					}
				}
			}
		}
		
		private void DebugGLError(string Name)
		{
			
			if (modProgram.Debug_GL)
			{
				if (Messages.Count < 8)
				{
					if (GL.GetError() != ErrorCode.NoError)
					{
						clsMessage NewMessage = new clsMessage();
						NewMessage.Text = "OpenGL Error (" + Name + ")";
						Messages.Add(NewMessage);
					}
				}
			}
		}
		
		public void DrawUnitRectangle(clsMap.clsUnit Unit, int BorderInsideThickness, sRGBA_sng InsideColour, sRGBA_sng OutsideColour)
		{
			modMath.sXY_int PosA = new modMath.sXY_int();
			modMath.sXY_int PosB = new modMath.sXY_int();
			int A = 0;
			int Altitude = Unit.Pos.Altitude - ViewInfo.ViewPos.Y;
			
			GetFootprintTileRangeClamped(Unit.Pos.Horizontal, Unit.Type.get_GetFootprintSelected(Unit.Rotation), PosA, PosB);
			A = PosA.Y;
			PosA.X = (int) ((PosA.X + 0.125D) * modProgram.TerrainGridSpacing - ViewInfo.ViewPos.X);
			PosA.Y = (int) ((PosB.Y + 0.875D) * - modProgram.TerrainGridSpacing - ViewInfo.ViewPos.Z);
			PosB.X = (int) ((PosB.X + 0.875D) * modProgram.TerrainGridSpacing - ViewInfo.ViewPos.X);
			PosB.Y = (int) ((A + 0.125D) * - modProgram.TerrainGridSpacing - ViewInfo.ViewPos.Z);
			
			GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
			GL.Vertex3(PosB.X, Altitude, System.Convert.ToInt32(- PosA.Y));
			GL.Vertex3(PosA.X, Altitude, System.Convert.ToInt32(- PosA.Y));
			GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
			GL.Vertex3(PosA.X + BorderInsideThickness, Altitude, System.Convert.ToInt32(- (PosA.Y + BorderInsideThickness)));
			GL.Vertex3(PosB.X - BorderInsideThickness, Altitude, System.Convert.ToInt32(- (PosA.Y + BorderInsideThickness)));
			
			GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
			GL.Vertex3(PosA.X, Altitude, System.Convert.ToInt32(- PosA.Y));
			GL.Vertex3(PosA.X, Altitude, System.Convert.ToInt32(- PosB.Y));
			GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
			GL.Vertex3(PosA.X + BorderInsideThickness, Altitude, System.Convert.ToInt32(- (PosB.Y - BorderInsideThickness)));
			GL.Vertex3(PosA.X + BorderInsideThickness, Altitude, System.Convert.ToInt32(- (PosA.Y + BorderInsideThickness)));
			
			GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
			GL.Vertex3(PosB.X, Altitude, System.Convert.ToInt32(- PosB.Y));
			GL.Vertex3(PosB.X, Altitude, System.Convert.ToInt32(- PosA.Y));
			GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
			GL.Vertex3(PosB.X - BorderInsideThickness, Altitude, - (PosA.Y + BorderInsideThickness));
			GL.Vertex3(PosB.X - BorderInsideThickness, Altitude, - (PosB.Y - BorderInsideThickness));
			
			GL.Color4(OutsideColour.Red, OutsideColour.Green, OutsideColour.Blue, OutsideColour.Alpha);
			GL.Vertex3(PosA.X, Altitude, System.Convert.ToInt32(- PosB.Y));
			GL.Vertex3(PosB.X, Altitude, System.Convert.ToInt32(- PosB.Y));
			GL.Color4(InsideColour.Red, InsideColour.Green, InsideColour.Blue, InsideColour.Alpha);
			GL.Vertex3(PosB.X - BorderInsideThickness, Altitude, - (PosB.Y - BorderInsideThickness));
			GL.Vertex3(PosA.X + BorderInsideThickness, Altitude, System.Convert.ToInt32(- (PosB.Y - BorderInsideThickness)));
		}
		
		public class clsDrawTileOutline : clsMap.clsAction
		{
			
			public sRGBA_sng Colour;
			
			private modMath.sXYZ_int Vertex0;
			private modMath.sXYZ_int Vertex1;
			private modMath.sXYZ_int Vertex2;
			private modMath.sXYZ_int Vertex3;
			
			public override void ActionPerform()
			{
				
				Vertex0.X = PosNum.X * modProgram.TerrainGridSpacing;
				Vertex0.Y = System.Convert.ToInt32(Map.Terrain.Vertices[PosNum.X, PosNum.Y].Height * Map.HeightMultiplier);
				Vertex0.Z = - PosNum.Y * modProgram.TerrainGridSpacing;
				Vertex1.X = (PosNum.X + 1) * modProgram.TerrainGridSpacing;
				Vertex1.Y = System.Convert.ToInt32(Map.Terrain.Vertices[PosNum.X + 1, PosNum.Y].Height * Map.HeightMultiplier);
				Vertex1.Z = - PosNum.Y * modProgram.TerrainGridSpacing;
				Vertex2.X = PosNum.X * modProgram.TerrainGridSpacing;
				Vertex2.Y = System.Convert.ToInt32(Map.Terrain.Vertices[PosNum.X, PosNum.Y + 1].Height * Map.HeightMultiplier);
				Vertex2.Z = - (PosNum.Y + 1) * modProgram.TerrainGridSpacing;
				Vertex3.X = (PosNum.X + 1) * modProgram.TerrainGridSpacing;
				Vertex3.Y = System.Convert.ToInt32(Map.Terrain.Vertices[PosNum.X + 1, PosNum.Y + 1].Height * Map.HeightMultiplier);
				Vertex3.Z = - (PosNum.Y + 1) * modProgram.TerrainGridSpacing;
				GL.Begin(BeginMode.LineLoop);
				GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
				GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z));
				GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToInt32(- Vertex1.Z));
				GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToInt32(- Vertex3.Z));
				GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToInt32(- Vertex2.Z));
				GL.End();
			}
		}
		
		public class clsDrawTileAreaOutline
		{
			//does not inherit action
			
			public clsMap Map;
			public sRGBA_sng Colour;
			public modMath.sXY_int StartXY;
			public modMath.sXY_int FinishXY;
			
			private modMath.sXYZ_int Vertex0;
			private modMath.sXYZ_int Vertex1;
			
			public void ActionPerform()
			{
				int X = 0;
				int Y = 0;
				
				GL.Begin(BeginMode.Lines);
				GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
				for (X = StartXY.X; X <= FinishXY.X - 1; X++)
				{
					Vertex0.X = X * modProgram.TerrainGridSpacing;
					Vertex0.Y = System.Convert.ToInt32(Map.Terrain.Vertices[X, StartXY.Y].Height * Map.HeightMultiplier);
					Vertex0.Z = - StartXY.Y * modProgram.TerrainGridSpacing;
					Vertex1.X = (X + 1) * modProgram.TerrainGridSpacing;
					Vertex1.Y = System.Convert.ToInt32(Map.Terrain.Vertices[X + 1, StartXY.Y].Height * Map.HeightMultiplier);
					Vertex1.Z = - StartXY.Y * modProgram.TerrainGridSpacing;
					GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z));
					GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToInt32(- Vertex1.Z));
				}
				for (X = StartXY.X; X <= FinishXY.X - 1; X++)
				{
					Vertex0.X = X * modProgram.TerrainGridSpacing;
					Vertex0.Y = System.Convert.ToInt32(Map.Terrain.Vertices[X, FinishXY.Y].Height * Map.HeightMultiplier);
					Vertex0.Z = - FinishXY.Y * modProgram.TerrainGridSpacing;
					Vertex1.X = (X + 1) * modProgram.TerrainGridSpacing;
					Vertex1.Y = System.Convert.ToInt32(Map.Terrain.Vertices[X + 1, FinishXY.Y].Height * Map.HeightMultiplier);
					Vertex1.Z = - FinishXY.Y * modProgram.TerrainGridSpacing;
					GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z));
					GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToInt32(- Vertex1.Z));
				}
				for (Y = StartXY.Y; Y <= FinishXY.Y - 1; Y++)
				{
					Vertex0.X = StartXY.X * modProgram.TerrainGridSpacing;
					Vertex0.Y = System.Convert.ToInt32(Map.Terrain.Vertices[StartXY.X, Y].Height * Map.HeightMultiplier);
					Vertex0.Z = - Y * modProgram.TerrainGridSpacing;
					Vertex1.X = StartXY.X * modProgram.TerrainGridSpacing;
					Vertex1.Y = System.Convert.ToInt32(Map.Terrain.Vertices[StartXY.X, Y + 1].Height * Map.HeightMultiplier);
					Vertex1.Z = - (Y + 1) * modProgram.TerrainGridSpacing;
					GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z));
					GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToInt32(- Vertex1.Z));
				}
				for (Y = StartXY.Y; Y <= FinishXY.Y - 1; Y++)
				{
					Vertex0.X = FinishXY.X * modProgram.TerrainGridSpacing;
					Vertex0.Y = System.Convert.ToInt32(Map.Terrain.Vertices[FinishXY.X, Y].Height * Map.HeightMultiplier);
					Vertex0.Z = - Y * modProgram.TerrainGridSpacing;
					Vertex1.X = FinishXY.X * modProgram.TerrainGridSpacing;
					Vertex1.Y = System.Convert.ToInt32(Map.Terrain.Vertices[FinishXY.X, Y + 1].Height * Map.HeightMultiplier);
					Vertex1.Z = - (Y + 1) * modProgram.TerrainGridSpacing;
					GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z));
					GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToInt32(- Vertex1.Z));
				}
				GL.End();
			}
		}
		
		public class clsDrawTerrainLine
		{
			//does not inherit action
			
			public clsMap Map;
			public sRGBA_sng Colour;
			public modMath.sXY_int StartXY;
			public modMath.sXY_int FinishXY;
			
			private modMath.sXYZ_int Vertex;
			private modMath.sXY_int StartTile;
			private modMath.sXY_int FinishTile;
			private modMath.sIntersectPos IntersectX;
			private modMath.sIntersectPos IntersectY;
			private modMath.sXY_int TileEdgeStart;
			private modMath.sXY_int TileEdgeFinish;
			private int LastXTile;
			private modMath.sXY_int Horizontal;
			
			public void ActionPerform()
			{
				int X = 0;
				int Y = 0;
				
				GL.Begin(BeginMode.LineStrip);
				GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
				
				StartTile.Y = (int) (Conversion.Int(StartXY.Y / modProgram.TerrainGridSpacing));
				FinishTile.Y = Conversion.Int(FinishXY.Y / modProgram.TerrainGridSpacing);
				LastXTile = Conversion.Int(StartXY.X / modProgram.TerrainGridSpacing);
				
				Horizontal = StartXY;
				Vertex.X = Horizontal.X;
				Vertex.Y = (int) (Map.GetTerrainHeight(Horizontal));
				Vertex.Z = System.Convert.ToInt32(- Horizontal.Y);
				GL.Vertex3(Vertex.X, Vertex.Y, System.Convert.ToInt32(- Vertex.Z));
				
				if (StartTile.Y + 1 <= FinishTile.Y)
				{
					for (Y = StartTile.Y + 1; Y <= FinishTile.Y; Y++)
					{
						TileEdgeStart.X = 0;
						TileEdgeStart.Y = Y * modProgram.TerrainGridSpacing;
						TileEdgeFinish.X = Map.Terrain.TileSize.X * modProgram.TerrainGridSpacing;
						TileEdgeFinish.Y = Y * modProgram.TerrainGridSpacing;
						IntersectY = modMath.GetLinesIntersectBetween(StartXY, FinishXY, TileEdgeStart, TileEdgeFinish);
						if (IntersectY.Exists)
						{
							StartTile.X = LastXTile;
							FinishTile.X = (int) (Conversion.Int(IntersectY.Pos.X / modProgram.TerrainGridSpacing));
							
							for (X = StartTile.X + 1; X <= FinishTile.X; X++)
							{
								TileEdgeStart.X = X * modProgram.TerrainGridSpacing;
								TileEdgeStart.Y = 0;
								TileEdgeFinish.X = X * modProgram.TerrainGridSpacing;
								TileEdgeFinish.Y = Map.Terrain.TileSize.Y * modProgram.TerrainGridSpacing;
								IntersectX = modMath.GetLinesIntersectBetween(StartXY, FinishXY, TileEdgeStart, TileEdgeFinish);
								if (IntersectX.Exists)
								{
									Horizontal = IntersectX.Pos;
									Vertex.X = Horizontal.X;
									Vertex.Y = (int) (Map.GetTerrainHeight(Horizontal));
									Vertex.Z = System.Convert.ToInt32(- Horizontal.Y);
									GL.Vertex3(Vertex.X, Vertex.Y, System.Convert.ToInt32(- Vertex.Z));
								}
							}
							
							LastXTile = FinishTile.X;
							
							Horizontal = IntersectY.Pos;
							Vertex.X = Horizontal.X;
							Vertex.Y = (int) (Map.GetTerrainHeight(Horizontal));
							Vertex.Z = System.Convert.ToInt32(- Horizontal.Y);
							GL.Vertex3(Vertex.X, Vertex.Y, System.Convert.ToInt32(- Vertex.Z));
						}
					}
				}
				else
				{
					StartTile.X = LastXTile;
					FinishTile.X = Conversion.Int(FinishXY.X / modProgram.TerrainGridSpacing);
					for (X = StartTile.X + 1; X <= FinishTile.X; X++)
					{
						TileEdgeStart.X = X * modProgram.TerrainGridSpacing;
						TileEdgeStart.Y = 0;
						TileEdgeFinish.X = X * modProgram.TerrainGridSpacing;
						TileEdgeFinish.Y = Map.Terrain.TileSize.Y * modProgram.TerrainGridSpacing;
						IntersectX = modMath.GetLinesIntersectBetween(StartXY, FinishXY, TileEdgeStart, TileEdgeFinish);
						if (IntersectX.Exists)
						{
							Horizontal = IntersectX.Pos;
							Vertex.X = Horizontal.X;
							Vertex.Y = (int) (Map.GetTerrainHeight(Horizontal));
							Vertex.Z = System.Convert.ToInt32(- Horizontal.Y);
							GL.Vertex3(Vertex.X, Vertex.Y, System.Convert.ToInt32(- Vertex.Z));
						}
					}
				}
				
				Horizontal = FinishXY;
				Vertex.X = Horizontal.X;
				Vertex.Y = (int) (Map.GetTerrainHeight(Horizontal));
				Vertex.Z = System.Convert.ToInt32(- Horizontal.Y);
				GL.Vertex3(Vertex.X, Vertex.Y, System.Convert.ToInt32(- Vertex.Z));
				
				GL.End();
			}
		}
		
		public class clsDrawCallTerrain : clsMap.clsAction
		{
			
			public override void ActionPerform()
			{
				
				GL.CallList(Map.Sectors[PosNum.X, PosNum.Y].GLList_Textured);
			}
		}
		
		public class clsDrawCallTerrainWireframe : clsMap.clsAction
		{
			
			public override void ActionPerform()
			{
				
				GL.CallList(Map.Sectors[PosNum.X, PosNum.Y].GLList_Wireframe);
			}
		}
		
		public class clsDrawTileOrientation : clsMap.clsAction
		{
			
			public override void ActionPerform()
			{
				int X = 0;
				int Y = 0;
				
				for (Y = PosNum.Y * modProgram.SectorTileSize; Y <= Math.Min((PosNum.Y + 1) * modProgram.SectorTileSize - 1, Map.Terrain.TileSize.Y - 1); Y++)
				{
					for (X = PosNum.X * modProgram.SectorTileSize; X <= Math.Min((PosNum.X + 1) * modProgram.SectorTileSize - 1, Map.Terrain.TileSize.X - 1); X++)
					{
						Map.DrawTileOrientation(new modMath.sXY_int(X, Y));
					}
				}
			}
		}
		
		public class clsDrawVertexTerrain : clsMap.clsAction
		{
			
			public Matrix3DMath.Matrix3D ViewAngleMatrix;
			
			private sRGB_sng RGB_sng;
			private sRGB_sng RGB_sng2;
			private Matrix3D.Position.XYZ_dbl XYZ_dbl;
			private Matrix3D.Position.XYZ_dbl XYZ_dbl2;
			private Matrix3D.Position.XYZ_dbl XYZ_dbl3;
			private Matrix3D.Position.XYZ_dbl Vertex0;
			private Matrix3D.Position.XYZ_dbl Vertex1;
			private Matrix3D.Position.XYZ_dbl Vertex2;
			private Matrix3D.Position.XYZ_dbl Vertex3;
			
			public override void ActionPerform()
			{
				int X = 0;
				int Y = 0;
				int A = 0;
				
				for (Y = PosNum.Y * modProgram.SectorTileSize; Y <= Math.Min(System.Convert.ToInt32((PosNum.Y + 1) * modProgram.SectorTileSize - 1), Map.Terrain.TileSize.Y); Y++)
				{
					for (X = PosNum.X * modProgram.SectorTileSize; X <= Math.Min((PosNum.X + 1) * modProgram.SectorTileSize - 1, Map.Terrain.TileSize.X); X++)
					{
						if (Map.Terrain.Vertices[X, Y].Terrain != null)
						{
							A = Map.Terrain.Vertices[X, Y].Terrain.Num;
							if (A < Map.Painter.TerrainCount)
							{
								if (Map.Painter.Terrains[A].Tiles.TileCount >= 1)
								{
									RGB_sng = Map.Tileset.Tiles[Map.Painter.Terrains[A].Tiles.Tiles[0].TextureNum].AverageColour;
									if (RGB_sng.Red + RGB_sng.Green + RGB_sng.Blue < 1.5F)
									{
										RGB_sng2.Red = (RGB_sng.Red + 1.0F) / 2.0F;
										RGB_sng2.Green = (RGB_sng.Green + 1.0F) / 2.0F;
										RGB_sng2.Blue = (RGB_sng.Blue + 1.0F) / 2.0F;
									}
									else
									{
										RGB_sng2.Red = RGB_sng.Red / 2.0F;
										RGB_sng2.Green = RGB_sng.Green / 2.0F;
										RGB_sng2.Blue = RGB_sng.Blue / 2.0F;
									}
									XYZ_dbl.X = X * modProgram.TerrainGridSpacing;
									XYZ_dbl.Y = System.Convert.ToDouble(Map.Terrain.Vertices[X, Y].Height * Map.HeightMultiplier);
									XYZ_dbl.Z = - Y * modProgram.TerrainGridSpacing;
									XYZ_dbl2.X = 10.0D;
									XYZ_dbl2.Y = 10.0D;
									XYZ_dbl2.Z = 0.0D;
									Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, ref XYZ_dbl3);
									Vertex0.X = XYZ_dbl.X + XYZ_dbl3.X;
									Vertex0.Y = XYZ_dbl.Y + XYZ_dbl3.Y;
									Vertex0.Z = XYZ_dbl.Z + XYZ_dbl3.Z;
									XYZ_dbl2.X = -10.0D;
									XYZ_dbl2.Y = 10.0D;
									XYZ_dbl2.Z = 0.0D;
									Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, ref XYZ_dbl3);
									Vertex1.X = XYZ_dbl.X + XYZ_dbl3.X;
									Vertex1.Y = XYZ_dbl.Y + XYZ_dbl3.Y;
									Vertex1.Z = XYZ_dbl.Z + XYZ_dbl3.Z;
									XYZ_dbl2.X = -10.0D;
									XYZ_dbl2.Y = -10.0D;
									XYZ_dbl2.Z = 0.0D;
									Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, ref XYZ_dbl3);
									Vertex2.X = XYZ_dbl.X + XYZ_dbl3.X;
									Vertex2.Y = XYZ_dbl.Y + XYZ_dbl3.Y;
									Vertex2.Z = XYZ_dbl.Z + XYZ_dbl3.Z;
									XYZ_dbl2.X = 10.0D;
									XYZ_dbl2.Y = -10.0D;
									XYZ_dbl2.Z = 0.0D;
									Matrix3DMath.VectorRotationByMatrix(ViewAngleMatrix, XYZ_dbl2, ref XYZ_dbl3);
									Vertex3.X = XYZ_dbl.X + XYZ_dbl3.X;
									Vertex3.Y = XYZ_dbl.Y + XYZ_dbl3.Y;
									Vertex3.Z = XYZ_dbl.Z + XYZ_dbl3.Z;
									GL.Begin(BeginMode.Quads);
									GL.Color3(RGB_sng.Red, RGB_sng.Green, RGB_sng.Blue);
									GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
									GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
									GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToDouble(- Vertex2.Z));
									GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToDouble(- Vertex3.Z));
									GL.End();
									GL.Begin(BeginMode.LineLoop);
									GL.Color3(RGB_sng2.Red, RGB_sng2.Green, RGB_sng2.Blue);
									GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToDouble(- Vertex0.Z));
									GL.Vertex3(Vertex1.X, Vertex1.Y, System.Convert.ToDouble(- Vertex1.Z));
									GL.Vertex3(Vertex2.X, Vertex2.Y, System.Convert.ToDouble(- Vertex2.Z));
									GL.Vertex3(Vertex3.X, Vertex3.Y, System.Convert.ToDouble(- Vertex3.Z));
									GL.End();
								}
							}
						}
					}
				}
			}
		}
		
		public class clsDrawVertexMarker : clsMap.clsAction
		{
			
			public sRGBA_sng Colour;
			
			private modMath.sXYZ_int Vertex0;
			
			public override void ActionPerform()
			{
				
				Vertex0.X = PosNum.X * modProgram.TerrainGridSpacing;
				Vertex0.Y = System.Convert.ToInt32(Map.Terrain.Vertices[PosNum.X, PosNum.Y].Height * Map.HeightMultiplier);
				Vertex0.Z = - PosNum.Y * modProgram.TerrainGridSpacing;
				GL.Begin(BeginMode.Lines);
				GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
				GL.Vertex3(Vertex0.X - 8, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z));
				GL.Vertex3(Vertex0.X + 8, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z));
				GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z - 8));
				GL.Vertex3(Vertex0.X, Vertex0.Y, - Vertex0.Z + 8);
				GL.End();
			}
		}
		
		public class clsDrawHorizontalPosOnTerrain
		{
			//does not inherit action
			
			public clsMap Map;
			
			public modMath.sXY_int Horizontal;
			public sRGBA_sng Colour;
			
			private modMath.sXYZ_int Vertex0;
			
			public void ActionPerform()
			{
				
				Vertex0.X = Horizontal.X;
				Vertex0.Y = (int) (Map.GetTerrainHeight(Horizontal));
				Vertex0.Z = System.Convert.ToInt32(- Horizontal.Y);
				GL.Begin(BeginMode.Lines);
				GL.Color4(Colour.Red, Colour.Green, Colour.Blue, Colour.Alpha);
				GL.Vertex3(Vertex0.X - 8, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z));
				GL.Vertex3(Vertex0.X + 8, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z));
				GL.Vertex3(Vertex0.X, Vertex0.Y, System.Convert.ToInt32(- Vertex0.Z - 8));
				GL.Vertex3(Vertex0.X, Vertex0.Y, - Vertex0.Z + 8);
				GL.End();
			}
		}
		
		public class clsDrawSectorObjects : clsMap.clsAction
		{
			
			public clsTextLabels UnitTextLabels;
			
			private bool[] UnitDrawn;
			private bool Started;
			
			public void Start()
			{
				
				UnitDrawn = new bool[Map.Units.Count - 1 + 1];
				
				Started = true;
			}
			
			public override void ActionPerform()
			{
				
				if (!Started)
				{
					Debugger.Break();
					return;
				}
				
				clsUnit Unit = default(clsUnit);
				clsSector Sector = Map.Sectors[PosNum.X, PosNum.Y];
				bool DrawUnitLabel = default(bool);
				clsViewInfo ViewInfo = Map.ViewInfo;
				clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = ViewInfo.GetMouseOverTerrain();
				clsTextLabel TextLabel = default(clsTextLabel);
				Matrix3D.Position.XYZ_dbl XYZ_dbl = default(Matrix3D.Position.XYZ_dbl);
				Matrix3D.Position.XYZ_dbl XYZ_dbl2 = default(Matrix3D.Position.XYZ_dbl);
				modMath.sXY_int ScreenPos = new modMath.sXY_int();
				clsUnitSectorConnection Connection = default(clsUnitSectorConnection);
				
				foreach (clsUnitSectorConnection tempLoopVar_Connection in Sector.Units)
				{
					Connection = tempLoopVar_Connection;
					Unit = Connection.Unit;
					if (!UnitDrawn[Unit.MapLink.ArrayPosition])
					{
						UnitDrawn[Unit.MapLink.ArrayPosition] = true;
						XYZ_dbl.X = Unit.Pos.Horizontal.X - ViewInfo.ViewPos.X;
						XYZ_dbl.Y = Unit.Pos.Altitude - ViewInfo.ViewPos.Y;
						XYZ_dbl.Z = - Unit.Pos.Horizontal.Y - ViewInfo.ViewPos.Z;
						DrawUnitLabel = false;
						if (Unit.Type.IsUnknown)
						{
							DrawUnitLabel = true;
						}
						else
						{
							GL.PushMatrix();
							GL.Translate(XYZ_dbl.X, XYZ_dbl.Y, System.Convert.ToDouble(- XYZ_dbl.Z));
							Unit.Type.GLDraw(Unit.Rotation);
							GL.PopMatrix();
							if (Unit.Type.Type == clsUnitType.enumType.PlayerDroid)
							{
								if (((clsDroidDesign) Unit.Type).AlwaysDrawTextLabel)
								{
									DrawUnitLabel = true;
								}
							}
							if (MouseOverTerrain != null)
							{
								if (MouseOverTerrain.Units.Count > 0)
								{
									if (MouseOverTerrain.Units[0] == Unit)
									{
										DrawUnitLabel = true;
									}
								}
							}
						}
						if (DrawUnitLabel && !UnitTextLabels.AtMaxCount())
						{
							Matrix3DMath.VectorRotationByMatrix(ViewInfo.ViewAngleMatrix_Inverted, XYZ_dbl, ref XYZ_dbl2);
							if (ViewInfo.Pos_Get_Screen_XY(XYZ_dbl2, ScreenPos))
							{
								if (ScreenPos.X >= 0 & ScreenPos.X <= ViewInfo.MapView.GLSize.X & ScreenPos.Y >= 0 & ScreenPos.Y <= ViewInfo.MapView.GLSize.Y)
								{
									TextLabel = new clsTextLabel();
									TextLabel.TextFont = modProgram.UnitLabelFont;
									TextLabel.SizeY = modSettings.Settings.FontSize;
									TextLabel.Colour.Red = 1.0F;
									TextLabel.Colour.Green = 1.0F;
									TextLabel.Colour.Blue = 1.0F;
									TextLabel.Colour.Alpha = 1.0F;
									TextLabel.Pos.X = ScreenPos.X + 32;
									TextLabel.Pos.Y = ScreenPos.Y;
									TextLabel.Text = Unit.Type.GetDisplayTextCode();
									UnitTextLabels.Add(TextLabel);
								}
							}
						}
					}
				}
			}
		}
	}
	
}