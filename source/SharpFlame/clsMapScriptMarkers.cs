using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace SharpFlame
{
	public partial class clsMap
	{
		
		public modLists.ConnectedList<clsScriptPosition, clsMap> ScriptPositions; 
		public modLists.ConnectedList<clsScriptArea, clsMap> ScriptAreas; 
		
		public string GetDefaultScriptLabel(string Prefix)
		{
			int Number = 1;
			modProgram.sResult Valid = new modProgram.sResult();
			string Label = "";
			
			do
			{
				Label = Prefix + modIO.InvariantToString_int(Number);
				Valid = ScriptLabelIsValid(Label);
				if (Valid.Success)
				{
					return Label;
				}
				Number++;
				if (Number >= 16384)
				{
MessageBox.Show("Error: Unable to set default script label.");
					return "";
				}
			} while (true);
		}
		
		public modProgram.sResult ScriptLabelIsValid(string Text)
		{
			modProgram.sResult ReturnResult = new modProgram.sResult();
			ReturnResult.Success = false;
			ReturnResult.Problem = "";
			
			if (Text == null)
			{
				ReturnResult.Problem = "Label cannot be nothing.";
				return ReturnResult;
			}
			
			string LCaseText = Text.ToLower();
			
			if (LCaseText.Length < 1)
			{
				ReturnResult.Problem = "Label cannot be nothing.";
				return ReturnResult;
			}
			
			char CurrentChar = (char) 0;
			bool Invalid = default(bool);
			
			Invalid = false;
			foreach (char tempLoopVar_CurrentChar in LCaseText)
			{
				CurrentChar = tempLoopVar_CurrentChar;
				if (!((CurrentChar >= 'a' && CurrentChar <= 'z') || (CurrentChar >= '0' && CurrentChar <= '9') || CurrentChar == '_'))
				{
					Invalid = true;
					break;
				}
			}
			if (Invalid)
			{
				ReturnResult.Problem = "Label contains invalid characters. Use only letters, numbers or underscores.";
				return ReturnResult;
			}
			
			clsUnit Unit = default(clsUnit);
			
			foreach (clsUnit tempLoopVar_Unit in Units)
			{
				Unit = tempLoopVar_Unit;
				if (Unit.Label != null)
				{
					if (LCaseText == Unit.Label.ToLower())
					{
						ReturnResult.Problem = "Label text is already in use.";
						return ReturnResult;
					}
				}
			}
			
			clsScriptPosition ScriptPosition = default(clsScriptPosition);
			
			foreach (clsScriptPosition tempLoopVar_ScriptPosition in ScriptPositions)
			{
				ScriptPosition = tempLoopVar_ScriptPosition;
				if (LCaseText == ScriptPosition.Label.ToLower())
				{
					ReturnResult.Problem = "Label text is already in use.";
					return ReturnResult;
				}
			}
			
			clsScriptArea ScriptArea = default(clsScriptArea);
			
			foreach (clsScriptArea tempLoopVar_ScriptArea in ScriptAreas)
			{
				ScriptArea = tempLoopVar_ScriptArea;
				if (LCaseText == ScriptArea.Label.ToLower())
				{
					ReturnResult.Problem = "Label text is already in use.";
					return ReturnResult;
				}
			}
			
			ReturnResult.Success = true;
			return ReturnResult;
		}
		
		public class clsScriptPosition
		{
			public clsScriptPosition()
			{
				
				_ParentMapLink = new modLists.ConnectedListLink<clsScriptPosition, clsMap>(this);
				
			}
			
			private modLists.ConnectedListLink<clsScriptPosition, clsMap> _ParentMapLink; 
public modLists.ConnectedListLink<clsScriptPosition, clsMap> ParentMap
			{
				get
				{
					return _ParentMapLink;
				}
			}
			
			private string _Label;
public string Label
			{
				get
				{
					return _Label;
				}
			}
			
			private modMath.sXY_int _Pos;
public int PosX
			{
				get
				{
					return _Pos.X;
				}
				set
				{
					_Pos.X = modMath.Clamp_int(value, 0, System.Convert.ToInt32(System.Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.X * modProgram.TerrainGridSpacing) - 1));
				}
			}
public int PosY
			{
				get
				{
					return _Pos.Y;
				}
				set
				{
					_Pos.Y = modMath.Clamp_int(value, 0, System.Convert.ToInt32(System.Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.Y * modProgram.TerrainGridSpacing) - 1));
				}
			}
			
			//private clsScriptPosition()
			//{
				
				//_ParentMapLink = new ConnectedListLink<clsScriptPosition, clsMap>(this);
				
				
				//}
				
				public static clsScriptPosition Create(clsMap Map)
				{
					clsScriptPosition Result = new clsScriptPosition();
					
					Result._Label = Map.GetDefaultScriptLabel("Position");
					
					Result._ParentMapLink.Connect(Map.ScriptPositions);
					
					return Result;
				}
				
				public void GLDraw()
				{
					
					clsMap.clsDrawHorizontalPosOnTerrain Drawer = new clsMap.clsDrawHorizontalPosOnTerrain();
					Drawer.Map = _ParentMapLink.Source;
					Drawer.Horizontal = _Pos;
					if (modMain.frmMainInstance.SelectedScriptMarker == this)
					{
						GL.LineWidth(4.5F);
						Drawer.Colour = new sRGBA_sng(1.0F, 1.0F, 0.5F, 1.0F);
					}
					else
					{
						GL.LineWidth(3.0F);
						Drawer.Colour = new sRGBA_sng(1.0F, 1.0F, 0.0F, 0.75F);
					}
					Drawer.ActionPerform();
				}
				
				public void MapResizing(modMath.sXY_int PosOffset)
				{
					
					PosX = _Pos.X - PosOffset.X;
					PosY = _Pos.Y - PosOffset.Y;
				}
				
				public void WriteWZ(clsINIWrite File)
				{
					
					File.SectionName_Append("position_" + modIO.InvariantToString_int(_ParentMapLink.ArrayPosition));
					File.Property_Append("pos", modIO.InvariantToString_int(_Pos.X) + ", " + modIO.InvariantToString_int(_Pos.Y));
					File.Property_Append("label", _Label);
					File.Gap_Append();
				}
				
				public modProgram.sResult SetLabel(string Text)
				{
					modProgram.sResult Result = new modProgram.sResult();
					
					Result = _ParentMapLink.Source.ScriptLabelIsValid(Text);
					if (Result.Success)
					{
						_Label = Text;
					}
					return Result;
				}
				
				public void Deallocate()
				{
					
					_ParentMapLink.Deallocate();
				}
			}
			
			public class clsScriptArea
			{
				public clsScriptArea()
				{
					
					_ParentMapLink = new modLists.ConnectedListLink<clsScriptArea, clsMap>(this);
					
				}
				
				private modLists.ConnectedListLink<clsScriptArea, clsMap> _ParentMapLink; 
public modLists.ConnectedListLink<clsScriptArea, clsMap> ParentMap
				{
					get
					{
						return _ParentMapLink;
					}
				}
				
				private string _Label;
public string Label
				{
					get
					{
						return _Label;
					}
				}
				
				private modMath.sXY_int _PosA;
				private modMath.sXY_int _PosB;
public modMath.sXY_int PosA
				{
					set
					{
						clsMap Map = _ParentMapLink.Source;
						_PosA.X = modMath.Clamp_int(value.X, 0, Map.Terrain.TileSize.X * modProgram.TerrainGridSpacing - 1);
						_PosA.Y = modMath.Clamp_int(value.Y, 0, Map.Terrain.TileSize.Y * modProgram.TerrainGridSpacing - 1);
						modMath.ReorderXY(_PosA, _PosB, _PosA, _PosB);
					}
				}
public modMath.sXY_int PosB
				{
					set
					{
						clsMap Map = _ParentMapLink.Source;
						_PosB.X = modMath.Clamp_int(value.X, 0, Map.Terrain.TileSize.X * modProgram.TerrainGridSpacing - 1);
						_PosB.Y = modMath.Clamp_int(value.Y, 0, Map.Terrain.TileSize.Y * modProgram.TerrainGridSpacing - 1);
						modMath.ReorderXY(_PosA, _PosB, _PosA, _PosB);
					}
				}
public int PosAX
				{
					get
					{
						return _PosA.X;
					}
					set
					{
						_PosA.X = modMath.Clamp_int(value, 0, System.Convert.ToInt32(System.Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.X * modProgram.TerrainGridSpacing) - 1));
						modMath.ReorderXY(_PosA, _PosB, _PosA, _PosB);
					}
				}
public int PosAY
				{
					get
					{
						return _PosA.Y;
					}
					set
					{
						_PosA.Y = modMath.Clamp_int(value, 0, System.Convert.ToInt32(System.Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.Y * modProgram.TerrainGridSpacing) - 1));
						modMath.ReorderXY(_PosA, _PosB, _PosA, _PosB);
					}
				}
public int PosBX
				{
					get
					{
						return _PosB.X;
					}
					set
					{
						_PosB.X = modMath.Clamp_int(value, 0, System.Convert.ToInt32(System.Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.X * modProgram.TerrainGridSpacing) - 1));
						modMath.ReorderXY(_PosA, _PosB, _PosA, _PosB);
					}
				}
public int PosBY
				{
					get
					{
						return _PosB.Y;
					}
					set
					{
						_PosB.Y = modMath.Clamp_int(value, 0, System.Convert.ToInt32(System.Convert.ToInt32(_ParentMapLink.Source.Terrain.TileSize.Y * modProgram.TerrainGridSpacing) - 1));
						modMath.ReorderXY(_PosA, _PosB, _PosA, _PosB);
					}
				}
				
				//protected clsScriptArea()
				//{
					
					//_ParentMapLink = new ConnectedListLink<clsScriptArea, clsMap>(this);
					
					
					//}
					
					public static clsScriptArea Create(clsMap Map)
					{
						clsScriptArea Result = new clsScriptArea();
						
						Result._Label = Map.GetDefaultScriptLabel("Area");
						
						Result._ParentMapLink.Connect(Map.ScriptAreas);
						
						return Result;
					}
					
					public void SetPositions(modMath.sXY_int PosA, modMath.sXY_int PosB)
					{
						clsMap Map = _ParentMapLink.Source;
						
						PosA.X = modMath.Clamp_int(PosA.X, 0, Map.Terrain.TileSize.X * modProgram.TerrainGridSpacing - 1);
						PosA.Y = modMath.Clamp_int(PosA.Y, 0, Map.Terrain.TileSize.Y * modProgram.TerrainGridSpacing - 1);
						PosB.X = modMath.Clamp_int(PosB.X, 0, Map.Terrain.TileSize.X * modProgram.TerrainGridSpacing - 1);
						PosB.Y = modMath.Clamp_int(PosB.Y, 0, Map.Terrain.TileSize.Y * modProgram.TerrainGridSpacing - 1);
						
						modMath.ReorderXY(PosA, PosB, _PosA, _PosB);
					}
					
					public void GLDraw()
					{
						
						clsMap.clsDrawTerrainLine Drawer = new clsMap.clsDrawTerrainLine();
						Drawer.Map = _ParentMapLink.Source;
						if (modMain.frmMainInstance.SelectedScriptMarker == this)
						{
							GL.LineWidth(4.5F);
							Drawer.Colour = new sRGBA_sng(1.0F, 1.0F, 0.5F, 0.75F);
						}
						else
						{
							GL.LineWidth(3.0F);
							Drawer.Colour = new sRGBA_sng(1.0F, 1.0F, 0.0F, 0.5F);
						}
						
						Drawer.StartXY = _PosA;
						Drawer.FinishXY.X = _PosB.X;
						Drawer.FinishXY.Y = _PosA.Y;
						Drawer.ActionPerform();
						
						Drawer.StartXY = _PosA;
						Drawer.FinishXY.X = _PosA.X;
						Drawer.FinishXY.Y = _PosB.Y;
						Drawer.ActionPerform();
						
						Drawer.StartXY.X = _PosB.X;
						Drawer.StartXY.Y = _PosA.Y;
						Drawer.FinishXY = _PosB;
						Drawer.ActionPerform();
						
						Drawer.StartXY.X = _PosA.X;
						Drawer.StartXY.Y = _PosB.Y;
						Drawer.FinishXY = _PosB;
						Drawer.ActionPerform();
					}
					
					public void MapResizing(modMath.sXY_int PosOffset)
					{
						
						SetPositions(new modMath.sXY_int(_PosA.X - PosOffset.X, _PosA.Y - PosOffset.Y), new modMath.sXY_int(_PosB.X - PosOffset.X, _PosB.Y - PosOffset.Y));
					}
					
					public void WriteWZ(clsINIWrite File)
					{
						
						File.SectionName_Append("area_" + modIO.InvariantToString_int(_ParentMapLink.ArrayPosition));
						File.Property_Append("pos1", modIO.InvariantToString_int(_PosA.X) + ", " + modIO.InvariantToString_int(_PosA.Y));
						File.Property_Append("pos2", modIO.InvariantToString_int(_PosB.X) + ", " + modIO.InvariantToString_int(_PosB.Y));
						File.Property_Append("label", _Label);
						File.Gap_Append();
					}
					
					public modProgram.sResult SetLabel(string Text)
					{
						modProgram.sResult Result = new modProgram.sResult();
						
						Result = _ParentMapLink.Source.ScriptLabelIsValid(Text);
						if (Result.Success)
						{
							_Label = Text;
						}
						return Result;
					}
					
					public void Deallocate()
					{
						
						_ParentMapLink.Deallocate();
					}
				}
			}
			
		}