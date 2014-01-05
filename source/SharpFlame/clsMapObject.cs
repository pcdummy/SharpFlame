using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace SharpFlame
{
	public partial class clsMap
	{
		
		public class clsUnit
		{
			public clsUnit()
			{
				
				MapLink = new modLists.ConnectedListLink<clsUnit, clsMap>(this);
				MapSelectedUnitLink = new modLists.ConnectedListLink<clsUnit, clsMap>(this);
				Sectors = new modLists.ConnectedList<clsUnitSectorConnection, clsUnit>(this);
				
			}
			public modLists.ConnectedListLink<clsUnit, clsMap> MapLink; 
			public modLists.ConnectedListLink<clsUnit, clsMap> MapSelectedUnitLink; 
			public modLists.ConnectedList<clsUnitSectorConnection, clsUnit> Sectors; 
			
			public UInt32 ID;
			public clsUnitType Type;
			public modProgram.sWorldPos Pos;
			public int Rotation;
			public clsUnitGroup UnitGroup;
			public int SavePriority;
			public double Health = 1.0D;
			public bool PreferPartsOutput = false;
			
			private string _Label;
			
			//public clsUnit()
			//{
				
				//MapLink = new ConnectedListLink<clsUnit, clsMap>(this);
				//MapSelectedUnitLink = new ConnectedListLink<clsUnit, clsMap>(this);
				//Sectors = new ConnectedList<clsUnitSectorConnection, clsUnit>(this);
				
				
				//}
				
				public clsUnit(clsUnit UnitToCopy, clsMap TargetMap)
				{
					
					MapLink = new modLists.ConnectedListLink<clsUnit, clsMap>(this);
					MapSelectedUnitLink = new modLists.ConnectedListLink<clsUnit, clsMap>(this);
					Sectors = new modLists.ConnectedList<clsUnitSectorConnection, clsUnit>(this);
					
					bool IsDesign = default(bool);
					
					if (UnitToCopy.Type.Type == clsUnitType.enumType.PlayerDroid)
					{
						IsDesign = !((clsDroidDesign) UnitToCopy.Type).IsTemplate;
					}
					else
					{
						IsDesign = false;
					}
					if (IsDesign)
					{
						clsDroidDesign DroidDesign = new clsDroidDesign();
						Type = DroidDesign;
						DroidDesign.CopyDesign((clsDroidDesign) UnitToCopy.Type);
						DroidDesign.UpdateAttachments();
					}
					else
					{
						Type = UnitToCopy.Type;
					}
					Pos = UnitToCopy.Pos;
					Rotation = UnitToCopy.Rotation;
					clsMap.clsUnitGroup OtherUnitGroup = default(clsMap.clsUnitGroup);
					OtherUnitGroup = UnitToCopy.UnitGroup;
					if (OtherUnitGroup.WZ_StartPos < 0)
					{
						UnitGroup = TargetMap.ScavengerUnitGroup;
					}
					else
					{
						UnitGroup = TargetMap.UnitGroups[OtherUnitGroup.WZ_StartPos];
					}
					SavePriority = UnitToCopy.SavePriority;
					Health = UnitToCopy.Health;
					PreferPartsOutput = UnitToCopy.PreferPartsOutput;
				}
				
public string Label
				{
					get
					{
						return _Label;
					}
				}
				
				public string GetINIPosition()
				{
					
					return modIO.InvariantToString_int(Pos.Horizontal.X) + ", " + modIO.InvariantToString_int(Pos.Horizontal.Y) + ", 0";
				}
				
				public string GetINIRotation()
				{
					int Rotation16 = 0;
					
					Rotation16 = (int) (Rotation * modProgram.INIRotationMax / 360.0D);
					if (Rotation16 >= modProgram.INIRotationMax)
					{
						Rotation16 -= modProgram.INIRotationMax;
					}
					else if (Rotation16 < 0)
					{
						Debugger.Break();
						Rotation16 += modProgram.INIRotationMax;
					}
					
					return modIO.InvariantToString_int(Rotation16) + ", 0, 0";
				}
				
				public string GetINIHealthPercent()
				{
					
					return modIO.InvariantToString_int((int) (modMath.Clamp_dbl(Health * 100.0D, 1.0D, 100.0D))) + "%";
				}
				
				public string GetPosText()
				{
					
					return modIO.InvariantToString_int(Pos.Horizontal.X) + ", " + modIO.InvariantToString_int(Pos.Horizontal.Y);
				}
				
				public modProgram.sResult SetLabel(string Text)
				{
					modProgram.sResult Result = new modProgram.sResult();
					
					if (Type.Type == clsUnitType.enumType.PlayerStructure)
					{
						clsStructureType StructureType = (clsStructureType) Type;
						clsStructureType.enumStructureType StructureTypeType = StructureType.StructureType;
						if (StructureTypeType == clsStructureType.enumStructureType.FactoryModule 
							| StructureTypeType == clsStructureType.enumStructureType.PowerModule 
							| StructureTypeType == clsStructureType.enumStructureType.ResearchModule)
						{
							Result.Problem = "Error: Trying to assign label to structure module.";
							return Result;
						}
					}
					
					if (!MapLink.IsConnected)
					{
						Debugger.Break();
						Result.Problem = "Error: Unit not on a map.";
						return Result;
					}
					
					if (Text == null)
					{
						_Label = null;
						Result.Success = true;
						Result.Problem = "";
						return Result;
					}
					else
					{
						Result = MapLink.Source.ScriptLabelIsValid(Text);
						if (Result.Success)
						{
							_Label = Text;
						}
						return Result;
					}
				}
				
				public void WriteWZLabel(clsINIWrite File, int PlayerCount)
				{
					
					if (_Label != null)
					{
						int TypeNum = 0;
						switch (Type.Type)
						{
							case clsUnitType.enumType.PlayerDroid:
								TypeNum = 0;
								break;
							case clsUnitType.enumType.PlayerStructure:
								TypeNum = 1;
								break;
							case clsUnitType.enumType.Feature:
								TypeNum = 2;
								break;
							default:
								return;
						}
						File.SectionName_Append("object_" + modIO.InvariantToString_int(MapLink.ArrayPosition));
						File.Property_Append("id", modIO.InvariantToString_uint(ID));
						if (PlayerCount >= 0) //not an FMap
						{
							File.Property_Append("type", modIO.InvariantToString_int(TypeNum));
							File.Property_Append("player", modIO.InvariantToString_int(UnitGroup.GetPlayerNum(PlayerCount)));
						}
						File.Property_Append("label", _Label);
						File.Gap_Append();
					}
				}
				
				public UInt32 GetBJOMultiplayerPlayerNum(int PlayerCount)
				{
					int PlayerNum = 0;
					
					if (UnitGroup == MapLink.Source.ScavengerUnitGroup || UnitGroup.WZ_StartPos < 0)
					{
						PlayerNum = Math.Max(PlayerCount, 7);
					}
					else
					{
						PlayerNum = UnitGroup.WZ_StartPos;
					}
					return (uint) PlayerNum;
				}
				
				public UInt32 GetBJOCampaignPlayerNum()
				{
					int PlayerNum = 0;
					
					if (UnitGroup == MapLink.Source.ScavengerUnitGroup || UnitGroup.WZ_StartPos < 0)
					{
						PlayerNum = 7;
					}
					else
					{
						PlayerNum = UnitGroup.WZ_StartPos;
					}
					return (uint) PlayerNum;
				}
				
				public void MapSelect()
				{
					
					if (MapSelectedUnitLink.IsConnected)
					{
						Debugger.Break();
						return;
					}
					
					MapSelectedUnitLink.Connect(MapLink.Source.SelectedUnits);
				}
				
				public void MapDeselect()
				{
					
					if (!MapSelectedUnitLink.IsConnected)
					{
						Debugger.Break();
						return;
					}
					
					MapSelectedUnitLink.Disconnect();
				}
				
				public void DisconnectFromMap()
				{
					
					if (MapLink.IsConnected)
					{
						MapLink.Disconnect();
					}
					if (MapSelectedUnitLink.IsConnected)
					{
						MapSelectedUnitLink.Disconnect();
					}
					Sectors.Clear();
				}
				
				public void Deallocate()
				{
					
					MapLink.Deallocate();
					MapSelectedUnitLink.Deallocate();
					Sectors.Deallocate();
				}
			}
			
			public modLists.ConnectedList<clsMap.clsUnit, clsMap> Units; 
			
			public class clsUnitSectorConnection
			{
				public clsUnitSectorConnection()
				{
					
					_UnitLink = new Link<clsMap.clsUnit>(this);
					_SectorLink = new Link<clsMap.clsSector>(this);
					
				}
				
				protected class Link<SourceType> : modLists.ConnectedListLink<clsUnitSectorConnection, SourceType> where SourceType : class
				{
					
					public Link(clsUnitSectorConnection Owner) : base(Owner)
					{
						
					}
					
					public override void AfterRemove()
					{
						base.AfterRemove();
						
						Item.Deallocate();
					}
				}
				
				protected Link<clsMap.clsUnit> _UnitLink; 
				protected Link<clsMap.clsSector> _SectorLink; 
				
public virtual clsMap.clsUnit Unit
				{
					get
					{
						return _UnitLink.Source;
					}
				}
				
public virtual clsMap.clsSector Sector
				{
					get
					{
						return _SectorLink.Source;
					}
				}
				
				public static clsUnitSectorConnection Create(clsMap.clsUnit Unit, clsMap.clsSector Sector)
				{
					
					if (Unit == null)
					{
						return null;
					}
					if (Unit.Sectors == null)
					{
						return null;
					}
					if (Unit.Sectors.IsBusy)
					{
						return null;
					}
					if (Sector == null)
					{
						return null;
					}
					if (Sector.Units == null)
					{
						return null;
					}
					if (Sector.Units.IsBusy)
					{
						return null;
					}
					
					clsUnitSectorConnection Result = new clsUnitSectorConnection();
					Result._UnitLink.Connect(Unit.Sectors);
					Result._SectorLink.Connect(Sector.Units);
					return Result;
				}
				
				//protected clsUnitSectorConnection()
				//{
					
					//_UnitLink = new Link<clsMap.clsUnit>(this);
					//_SectorLink = new Link<clsMap.clsSector>(this);
					
					
					
					//}
					
					public void Deallocate()
					{
						
						_UnitLink.Deallocate();
						_SectorLink.Deallocate();
					}
				}
				
				public class clsUnitGroupContainer
				{
					
					private clsUnitGroup _Item;
					
public clsUnitGroup Item
					{
						get
						{
							return _Item;
						}
						set
						{
							if (value == _Item)
							{
								return;
							}
							_Item = value;
							if (ChangedEvent != null)
								ChangedEvent();
						}
					}
					
					public delegate void ChangedEventHandler();
					private ChangedEventHandler ChangedEvent;
					
					public event ChangedEventHandler Changed
					{
						add
						{
							ChangedEvent = (ChangedEventHandler) System.Delegate.Combine(ChangedEvent, value);
						}
						remove
						{
							ChangedEvent = (ChangedEventHandler) System.Delegate.Remove(ChangedEvent, value);
						}
					}
					
				}
				
				private clsUnitGroupContainer _SelectedUnitGroup;
public clsUnitGroupContainer SelectedUnitGroup
				{
					get
					{
						return _SelectedUnitGroup;
					}
				}
				
				public class clsUnitGroup
				{
					public clsUnitGroup()
					{
						
						MapLink = new modLists.ConnectedListLink<clsUnitGroup, clsMap>(this);
						
					}
					
					public modLists.ConnectedListLink<clsUnitGroup, clsMap> MapLink; 
					
					public int WZ_StartPos = -1;
					
					public string GetFMapINIPlayerText()
					{
						
						if (WZ_StartPos < 0 | WZ_StartPos >= modProgram.PlayerCountMax)
						{
							return "scavenger";
						}
						else
						{
							return modIO.InvariantToString_int(WZ_StartPos);
						}
					}
					
					public string GetLNDPlayerText()
					{
						
						if (WZ_StartPos < 0 | WZ_StartPos >= modProgram.PlayerCountMax)
						{
							return modIO.InvariantToString_int(7);
						}
						else
						{
							return modIO.InvariantToString_int(WZ_StartPos);
						}
					}
					
					public int GetPlayerNum(int PlayerCount)
					{
						
						if (WZ_StartPos < 0 | WZ_StartPos >= modProgram.PlayerCountMax)
						{
							return Math.Max(PlayerCount, 7);
						}
						else
						{
							return WZ_StartPos;
						}
					}
				}
				
				public modLists.ConnectedList<clsUnitGroup, clsMap> UnitGroups; 
				public clsUnitGroup ScavengerUnitGroup;
				
				public UInt32 GetAvailableID()
				{
					clsUnit Unit = default(clsUnit);
					UInt32 ID = 0;
					
					ID = 1U;
					foreach (clsUnit tempLoopVar_Unit in Units)
					{
						Unit = tempLoopVar_Unit;
						if (Unit.ID >= ID)
						{
							ID = Unit.ID + 1U;
						}
					}
					
					return ID;
				}
				
				public class clsUnitAdd
				{
					public clsMap Map;
					public clsUnit NewUnit;
					public UInt32 ID = 0U;
					public string Label = null;
					public bool StoreChange = false;
					
					public bool Perform()
					{
						
						if (Map == null)
						{
							Debugger.Break();
							return false;
						}
						if (NewUnit == null)
						{
							Debugger.Break();
							return false;
						}
						
						if (NewUnit.MapLink.IsConnected)
						{
MessageBox.Show("Error: Added object already has a map assigned.");
							return false;
						}
						if (NewUnit.UnitGroup == null)
						{
MessageBox.Show("Error: Added object has no group.");
							NewUnit.UnitGroup = Map.ScavengerUnitGroup;
							return false;
						}
						if (NewUnit.UnitGroup.MapLink.Source != Map)
						{
MessageBox.Show("Error: Something terrible happened.");
							return false;
						}
						
						if (StoreChange)
						{
							clsMap.clsUnitChange UnitChange = new clsMap.clsUnitChange();
							UnitChange.Type = clsUnitChange.enumType.Added;
							UnitChange.Unit = NewUnit;
							Map.UnitChanges.Add(UnitChange);
						}
						
						if (ID <= 0U)
						{
							ID = Map.GetAvailableID();
						}
						else if (Map.IDUsage(ID) != null)
						{
							ID = Map.GetAvailableID();
						}
						
						NewUnit.ID = ID;
						
						NewUnit.MapLink.Connect(Map.Units);
						
						NewUnit.Pos.Horizontal.X = modMath.Clamp_int(NewUnit.Pos.Horizontal.X, 0, Map.Terrain.TileSize.X * modProgram.TerrainGridSpacing - 1);
						NewUnit.Pos.Horizontal.Y = modMath.Clamp_int(NewUnit.Pos.Horizontal.Y, 0, Map.Terrain.TileSize.Y * modProgram.TerrainGridSpacing - 1);
						NewUnit.Pos.Altitude = (int) (Math.Ceiling(Map.GetTerrainHeight(NewUnit.Pos.Horizontal)));
						
						if (Label != null)
						{
							NewUnit.SetLabel(Label);
						}
						
						Map.UnitSectorsCalc(NewUnit);
						
						if (Map.SectorGraphicsChanges != null)
						{
							Map.UnitSectorsGraphicsChanged(NewUnit);
						}
						
						return true;
					}
				}
				
				public void UnitRemoveStoreChange(int Num)
				{
					
					clsMap.clsUnitChange UnitChange = new clsMap.clsUnitChange();
					UnitChange.Type = clsUnitChange.enumType.Deleted;
					UnitChange.Unit = Units[Num];
					UnitChanges.Add(UnitChange);
					
					UnitRemove(Num);
				}
				
				public void UnitRemove(int Num)
				{
					clsMap.clsUnit Unit = default(clsMap.clsUnit);
					
					Unit = Units[Num];
					
					if (SectorGraphicsChanges != null)
					{
						UnitSectorsGraphicsChanged(Unit);
					}
					
					if (ViewInfo != null)
					{
						clsViewInfo.clsMouseOver.clsOverTerrain MouseOverTerrain = ViewInfo.GetMouseOverTerrain();
						if (MouseOverTerrain != null)
						{
							int Pos = MouseOverTerrain.Units.FindFirstItemPosition(Unit);
							if (Pos >= 0)
							{
								MouseOverTerrain.Units.Remove(Pos);
							}
						}
					}
					
					Unit.DisconnectFromMap();
				}
				
				public void UnitSwap(clsMap.clsUnit OldUnit, clsMap.clsUnit NewUnit)
				{
					
					if (OldUnit.MapLink.Source != this)
					{
						Debugger.Break();
						return;
					}
					
					UnitRemoveStoreChange(OldUnit.MapLink.ArrayPosition);
					clsMap.clsUnitAdd UnitAdd = new clsMap.clsUnitAdd();
					UnitAdd.Map = this;
					UnitAdd.StoreChange = true;
					UnitAdd.ID = OldUnit.ID;
					UnitAdd.NewUnit = NewUnit;
					UnitAdd.Label = OldUnit.Label;
					UnitAdd.Perform();
					modProgram.ErrorIDChange(OldUnit.ID, NewUnit, "UnitSwap");
				}
				
				public void MakeDefaultUnitGroups()
				{
					int A = 0;
					clsMap.clsUnitGroup NewGroup = default(clsMap.clsUnitGroup);
					
					UnitGroups.Clear();
					for (A = 0; A <= modProgram.PlayerCountMax - 1; A++)
					{
						NewGroup = new clsUnitGroup();
						NewGroup.WZ_StartPos = A;
						NewGroup.MapLink.Connect(UnitGroups);
					}
					ScavengerUnitGroup = new clsUnitGroup();
					ScavengerUnitGroup.MapLink.Connect(UnitGroups);
					ScavengerUnitGroup.WZ_StartPos = -1;
				}
				
				public sRGB_sng GetUnitGroupColour(clsUnitGroup ColourUnitGroup)
				{
					
					if (ColourUnitGroup.WZ_StartPos < 0)
					{
						return new sRGB_sng(1.0F, 1.0F, 1.0F);
					}
					else
					{
						return modProgram.PlayerColour[ColourUnitGroup.WZ_StartPos].Colour;
					}
				}
				
				public sRGB_sng GetUnitGroupMinimapColour(clsUnitGroup ColourUnitGroup)
				{
					
					if (ColourUnitGroup.WZ_StartPos < 0)
					{
						return new sRGB_sng(1.0F, 1.0F, 1.0F);
					}
					else
					{
						return modProgram.PlayerColour[ColourUnitGroup.WZ_StartPos].MinimapColour;
					}
				}
				
				public clsUnit IDUsage(UInt32 ID)
				{
					
					foreach (clsUnit Unit in Units)
					{
						if (Unit.ID == ID)
						{
							return Unit;
//							break;
						}
					}
					
					return null;
				}
				
				public class clsUnitCreate
				{
					
					public clsMap Map;
					public clsUnitType ObjectType;
					public modMath.sXY_int Horizontal;
					public clsUnitGroup UnitGroup;
					public bool AutoWalls = false;
					public int Rotation = 0;
					public bool RandomizeRotation = false;
					
					public clsUnit Perform()
					{
						
						if (AutoWalls)
						{
							if (ObjectType.Type == clsUnitType.enumType.PlayerStructure)
							{
								clsStructureType StructureType = (clsStructureType) ObjectType;
								if (StructureType.WallLink.IsConnected)
								{
									clsWallType AutoWallType = null;
									AutoWallType = StructureType.WallLink.Source;
									Map.PerformTileWall(AutoWallType, Map.GetPosTileNum(Horizontal), true);
									return null;
								}
							}
						}
						clsMap.clsUnit newUnit = new clsMap.clsUnit();
						if (RandomizeRotation)
						{
							newUnit.Rotation = (int) (Conversion.Int(VBMath.Rnd() * 360.0D));
						}
						else
						{
							newUnit.Rotation = Rotation;
						}
						newUnit.UnitGroup = UnitGroup;
						newUnit.Pos = Map.TileAlignedPosFromMapPos(Horizontal, ObjectType.get_GetFootprintSelected(newUnit.Rotation));
						newUnit.Type = ObjectType;
						clsUnitAdd UnitAdd = new clsUnitAdd();
						UnitAdd.Map = Map;
						UnitAdd.NewUnit = newUnit;
						UnitAdd.StoreChange = true;
						UnitAdd.Perform();
						return newUnit;
					}
				}
				
				public void SetObjectCreatorDefaults(clsMap.clsUnitCreate objectCreator)
				{
					
					objectCreator.Map = this;
					
					objectCreator.ObjectType = modMain.frmMainInstance.SingleSelectedObjectType;
					objectCreator.AutoWalls = modMain.frmMainInstance.cbxAutoWalls.Checked;
					objectCreator.UnitGroup = SelectedUnitGroup.Item;
					try
					{
						int Rotation = 0;
						modIO.InvariantParse_int(modMain.frmMainInstance.txtNewObjectRotation.Text, ref Rotation);
						if (Rotation < 0 | Rotation > 359)
						{
							objectCreator.Rotation = 0;
						}
						else
						{
							objectCreator.Rotation = Rotation;
						}
					}
					catch
					{
						objectCreator.Rotation = 0;
					}
					objectCreator.RandomizeRotation = modMain.frmMainInstance.cbxObjectRandomRotation.Checked;
				}
			}
			
		}