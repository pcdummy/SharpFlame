﻿namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Collections;

    [StandardModule]
    public sealed class modGenerator
    {
        public static clsGeneratorTileset Generator_TilesetArizona;
        public static clsGeneratorTileset Generator_TilesetRockies;
        public static clsGeneratorTileset Generator_TilesetUrban;
        public static clsMap.sGenerateMasterTerrainArgs TerrainStyle_Arizona;
        public static clsMap.sGenerateMasterTerrainArgs TerrainStyle_Rockies;
        public static clsMap.sGenerateMasterTerrainArgs TerrainStyle_Urban;
        public static clsUnitType UnitType_1Tree;
        public static clsUnitType UnitType_1TreeSnowy;
        public static clsUnitType UnitType_4Trees;
        public static clsUnitType UnitType_4TreesSnowy;
        public static clsUnitType UnitType_5Trees;
        public static clsUnitType UnitType_5TreesSnowy;
        public static clsUnitType UnitType_Boulder1;
        public static clsUnitType UnitType_Boulder2;
        public static clsUnitType UnitType_Boulder3;
        public static clsUnitType UnitType_BuildingStump;
        public static clsUnitType UnitType_CommandCentre;
        public static clsUnitType UnitType_Crane;
        public static clsUnitType UnitType_CyborgFactory;
        public static clsUnitType UnitType_Derrick;
        public static clsUnitType UnitType_Factory;
        public static clsUnitType UnitType_FactoryModule;
        public static clsUnitType UnitType_HalfHighrise;
        public static clsUnitType UnitType_Highrise1;
        public static clsUnitType UnitType_Highrise2;
        public static clsUnitType UnitType_Highrise3;
        public static clsUnitType UnitType_HighriseStump1;
        public static clsUnitType UnitType_HighriseStump2;
        public static clsUnitType UnitType_LogCabin1;
        public static clsUnitType UnitType_LogCabin2;
        public static clsUnitType UnitType_LogCabin3;
        public static clsUnitType UnitType_LogCabin4;
        public static clsUnitType UnitType_LogCabin5;
        public static clsUnitType UnitType_OilDrum;
        public static clsUnitType UnitType_OilResource;
        public static clsUnitType UnitType_PowerGenerator;
        public static clsUnitType UnitType_PowerModule;
        public static clsUnitType UnitType_ResearchFacility;
        public static clsUnitType UnitType_ResearchModule;
        public static clsUnitType UnitType_Ruin1;
        public static clsUnitType UnitType_Ruin3;
        public static clsUnitType UnitType_Ruin4;
        public static clsUnitType UnitType_Ruin5;
        public static clsUnitType UnitType_SmallBuilding1;
        public static clsUnitType UnitType_SmallBuilding2;
        public static clsUnitType UnitType_SmallBuilding3;
        public static clsUnitType UnitType_Truck;

        public static void CreateGeneratorTilesets()
        {
            Generator_TilesetArizona = new clsGeneratorTileset();
            Generator_TilesetArizona.Tileset = modProgram.Tileset_Arizona;
            Generator_TilesetUrban = new clsGeneratorTileset();
            Generator_TilesetUrban.Tileset = modProgram.Tileset_Urban;
            Generator_TilesetRockies = new clsGeneratorTileset();
            Generator_TilesetRockies.Tileset = modProgram.Tileset_Rockies;
            UnitType_OilResource = GetUnitTypeFromCode("OilResource");
            UnitType_CommandCentre = GetUnitTypeFromCode("A0CommandCentre");
            UnitType_Truck = GetUnitTypeFromCode("ConstructionDroid");
            UnitType_Factory = GetUnitTypeFromCode("A0LightFactory");
            UnitType_FactoryModule = GetUnitTypeFromCode("A0FacMod1");
            UnitType_CyborgFactory = GetUnitTypeFromCode("A0CyborgFactory");
            UnitType_ResearchFacility = GetUnitTypeFromCode("A0ResearchFacility");
            UnitType_ResearchModule = GetUnitTypeFromCode("A0ResearchModule1");
            UnitType_PowerGenerator = GetUnitTypeFromCode("A0PowerGenerator");
            UnitType_PowerModule = GetUnitTypeFromCode("A0PowMod1");
            UnitType_Derrick = GetUnitTypeFromCode("A0ResourceExtractor");
            UnitType_Boulder1 = GetUnitTypeFromCode("Boulder1");
            UnitType_Boulder2 = GetUnitTypeFromCode("Boulder2");
            UnitType_Boulder3 = GetUnitTypeFromCode("Boulder3");
            UnitType_Ruin1 = GetUnitTypeFromCode("Ruin1");
            UnitType_Ruin3 = GetUnitTypeFromCode("Ruin3");
            UnitType_Ruin4 = GetUnitTypeFromCode("Ruin4");
            UnitType_Ruin5 = GetUnitTypeFromCode("Ruin5");
            UnitType_5Trees = GetUnitTypeFromCode("Tree1");
            UnitType_4Trees = GetUnitTypeFromCode("Tree2");
            UnitType_1Tree = GetUnitTypeFromCode("Tree3");
            UnitType_5TreesSnowy = GetUnitTypeFromCode("TreeSnow1");
            UnitType_4TreesSnowy = GetUnitTypeFromCode("TreeSnow2");
            UnitType_1TreeSnowy = GetUnitTypeFromCode("TreeSnow3");
            UnitType_Highrise1 = GetUnitTypeFromCode("building1");
            UnitType_Highrise2 = GetUnitTypeFromCode("building2");
            UnitType_Highrise3 = GetUnitTypeFromCode("building3");
            UnitType_HalfHighrise = GetUnitTypeFromCode("building11");
            UnitType_HighriseStump1 = GetUnitTypeFromCode("building7");
            UnitType_HighriseStump2 = GetUnitTypeFromCode("building8");
            UnitType_BuildingStump = GetUnitTypeFromCode("WreckedBuilding9");
            UnitType_SmallBuilding1 = GetUnitTypeFromCode("building10");
            UnitType_SmallBuilding2 = GetUnitTypeFromCode("building12");
            UnitType_SmallBuilding3 = GetUnitTypeFromCode("WreckedBuilding17");
            UnitType_LogCabin1 = GetUnitTypeFromCode("LogCabin1");
            UnitType_LogCabin2 = GetUnitTypeFromCode("LogCabin2");
            UnitType_LogCabin3 = GetUnitTypeFromCode("LogCabin3");
            UnitType_LogCabin4 = GetUnitTypeFromCode("LogCabin4");
            UnitType_LogCabin5 = GetUnitTypeFromCode("LogCabin5");
            UnitType_Crane = GetUnitTypeFromCode("Crane");
            UnitType_OilDrum = GetUnitTypeFromCode("OilDrum");
            clsGeneratorTileset.sUnitChance newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Boulder1, 1);
            Generator_TilesetArizona.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Boulder2, 1);
            Generator_TilesetArizona.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Boulder3, 1);
            Generator_TilesetArizona.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Ruin1, 1);
            Generator_TilesetArizona.ClusteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Ruin3, 1);
            Generator_TilesetArizona.ClusteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Ruin4, 1);
            Generator_TilesetArizona.ClusteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Ruin5, 1);
            Generator_TilesetArizona.ClusteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Highrise1, 3);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Highrise2, 3);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Highrise3, 3);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_HalfHighrise, 1);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_SmallBuilding1, 3);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_SmallBuilding2, 3);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_SmallBuilding3, 3);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_HighriseStump1, 1);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_HighriseStump2, 1);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_BuildingStump, 1);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_Crane, 2);
            Generator_TilesetUrban.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_5Trees, 1);
            Generator_TilesetRockies.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_4Trees, 1);
            Generator_TilesetRockies.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_1Tree, 2);
            Generator_TilesetRockies.ScatteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_LogCabin1, 3);
            Generator_TilesetRockies.ClusteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_LogCabin2, 1);
            Generator_TilesetRockies.ClusteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_LogCabin3, 1);
            Generator_TilesetRockies.ClusteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_LogCabin4, 1);
            Generator_TilesetRockies.ClusteredUnit_Add(newUnit);
            newUnit = new clsGeneratorTileset.sUnitChance(UnitType_LogCabin5, 3);
            Generator_TilesetRockies.ClusteredUnit_Add(newUnit);
            int index = 0;
            TerrainStyle_Arizona.LayerCount = 5;
            index = 0;
            TerrainStyle_Arizona.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Arizona.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Arizona.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[index].TileNum = 0x30;
            TerrainStyle_Arizona.Layers[index].TerrainmapDensity = 1f;
            TerrainStyle_Arizona.Layers[index].TerrainmapScale = 0f;
            TerrainStyle_Arizona.Layers[index].HeightMax = 256f;
            TerrainStyle_Arizona.Layers[index].AvoidLayers = new bool[(TerrainStyle_Arizona.LayerCount - 1) + 1];
            TerrainStyle_Arizona.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Arizona.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Arizona.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Arizona.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[index].TileNum = 11;
            TerrainStyle_Arizona.Layers[index].TerrainmapDensity = 0.5f;
            TerrainStyle_Arizona.Layers[index].TerrainmapScale = 2f;
            TerrainStyle_Arizona.Layers[index].HeightMax = 256f;
            TerrainStyle_Arizona.Layers[index].AvoidLayers = new bool[(TerrainStyle_Arizona.LayerCount - 1) + 1];
            TerrainStyle_Arizona.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Arizona.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Arizona.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Arizona.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[index].TileNum = 5;
            TerrainStyle_Arizona.Layers[index].TerrainmapDensity = 0.4f;
            TerrainStyle_Arizona.Layers[index].TerrainmapScale = 1.5f;
            TerrainStyle_Arizona.Layers[index].HeightMax = 256f;
            TerrainStyle_Arizona.Layers[index].AvoidLayers = new bool[(TerrainStyle_Arizona.LayerCount - 1) + 1];
            TerrainStyle_Arizona.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Arizona.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Arizona.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Arizona.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[index].TileNum = 0x17;
            TerrainStyle_Arizona.Layers[index].TerrainmapDensity = 0.75f;
            TerrainStyle_Arizona.Layers[index].TerrainmapScale = 1.5f;
            TerrainStyle_Arizona.Layers[index].HeightMax = 256f;
            TerrainStyle_Arizona.Layers[index].AvoidLayers = new bool[(TerrainStyle_Arizona.LayerCount - 1) + 1];
            TerrainStyle_Arizona.Layers[index].WithinLayer = index - 1;
            index++;
            TerrainStyle_Arizona.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Arizona.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Arizona.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Arizona.Layers[index].TileNum = 0x12;
            TerrainStyle_Arizona.Layers[index].TerrainmapDensity = 1f;
            TerrainStyle_Arizona.Layers[index].TerrainmapScale = 0f;
            TerrainStyle_Arizona.Layers[index].HeightMax = 256f;
            TerrainStyle_Arizona.Layers[index].IsCliff = true;
            TerrainStyle_Arizona.Layers[index].AvoidLayers = new bool[(TerrainStyle_Arizona.LayerCount - 1) + 1];
            TerrainStyle_Arizona.Layers[index].WithinLayer = -1;
            TerrainStyle_Arizona.Tileset = Generator_TilesetArizona;
            Generator_TilesetArizona.BorderTextureNum = 0x12;
            TerrainStyle_Urban.LayerCount = 6;
            index = 0;
            TerrainStyle_Urban.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Urban.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Urban.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[index].TileNum = 7;
            TerrainStyle_Urban.Layers[index].TerrainmapDensity = 1f;
            TerrainStyle_Urban.Layers[index].HeightMax = 256f;
            TerrainStyle_Urban.Layers[index].AvoidLayers = new bool[(TerrainStyle_Urban.LayerCount - 1) + 1];
            TerrainStyle_Urban.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Urban.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Urban.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Urban.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[index].TileNum = 0;
            TerrainStyle_Urban.Layers[index].TerrainmapDensity = 0.5f;
            TerrainStyle_Urban.Layers[index].TerrainmapScale = 1.5f;
            TerrainStyle_Urban.Layers[index].HeightMax = 256f;
            TerrainStyle_Urban.Layers[index].AvoidLayers = new bool[(TerrainStyle_Urban.LayerCount - 1) + 1];
            TerrainStyle_Urban.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Urban.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Urban.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Urban.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[index].TileNum = 0x16;
            TerrainStyle_Urban.Layers[index].TerrainmapDensity = 0.333f;
            TerrainStyle_Urban.Layers[index].TerrainmapScale = 1.5f;
            TerrainStyle_Urban.Layers[index].HeightMax = 256f;
            TerrainStyle_Urban.Layers[index].AvoidLayers = new bool[(TerrainStyle_Urban.LayerCount - 1) + 1];
            TerrainStyle_Urban.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Urban.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Urban.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Urban.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[index].TileNum = 50;
            TerrainStyle_Urban.Layers[index].TerrainmapDensity = 0.333f;
            TerrainStyle_Urban.Layers[index].TerrainmapScale = 1.5f;
            TerrainStyle_Urban.Layers[index].HeightMax = 256f;
            TerrainStyle_Urban.Layers[index].AvoidLayers = new bool[(TerrainStyle_Urban.LayerCount - 1) + 1];
            TerrainStyle_Urban.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Urban.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Urban.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Urban.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[index].TileNum = 0x33;
            TerrainStyle_Urban.Layers[index].TerrainmapDensity = 0.4f;
            TerrainStyle_Urban.Layers[index].TerrainmapScale = 1.5f;
            TerrainStyle_Urban.Layers[index].HeightMax = 256f;
            TerrainStyle_Urban.Layers[index].AvoidLayers = new bool[(TerrainStyle_Urban.LayerCount - 1) + 1];
            TerrainStyle_Urban.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Urban.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Urban.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Urban.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Urban.Layers[index].TileNum = 70;
            TerrainStyle_Urban.Layers[index].TerrainmapDensity = 1f;
            TerrainStyle_Urban.Layers[index].TerrainmapScale = 0f;
            TerrainStyle_Urban.Layers[index].HeightMax = 256f;
            TerrainStyle_Urban.Layers[index].IsCliff = true;
            TerrainStyle_Urban.Layers[index].AvoidLayers = new bool[(TerrainStyle_Urban.LayerCount - 1) + 1];
            TerrainStyle_Urban.Layers[index].WithinLayer = -1;
            TerrainStyle_Urban.Tileset = Generator_TilesetUrban;
            Generator_TilesetUrban.BorderTextureNum = 70;
            TerrainStyle_Rockies.LayerCount = 7;
            index = 0;
            TerrainStyle_Rockies.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Rockies.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Rockies.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[index].TileNum = 0;
            TerrainStyle_Rockies.Layers[index].TerrainmapDensity = 1f;
            TerrainStyle_Rockies.Layers[index].TerrainmapScale = 0f;
            TerrainStyle_Rockies.Layers[index].HeightMax = 256f;
            TerrainStyle_Rockies.Layers[index].AvoidLayers = new bool[(TerrainStyle_Rockies.LayerCount - 1) + 1];
            TerrainStyle_Rockies.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Rockies.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Rockies.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Rockies.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[index].TileNum = 0x35;
            TerrainStyle_Rockies.Layers[index].TerrainmapDensity = 0.4f;
            TerrainStyle_Rockies.Layers[index].TerrainmapScale = 1.5f;
            TerrainStyle_Rockies.Layers[index].HeightMax = 256f;
            TerrainStyle_Rockies.Layers[index].AvoidLayers = new bool[(TerrainStyle_Rockies.LayerCount - 1) + 1];
            TerrainStyle_Rockies.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Rockies.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Rockies.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Rockies.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[index].TileNum = 0x17;
            TerrainStyle_Rockies.Layers[index].TerrainmapDensity = 0.333f;
            TerrainStyle_Rockies.Layers[index].TerrainmapScale = 1.5f;
            TerrainStyle_Rockies.Layers[index].HeightMin = 85f;
            TerrainStyle_Rockies.Layers[index].HeightMax = 256f;
            TerrainStyle_Rockies.Layers[index].AvoidLayers = new bool[(TerrainStyle_Rockies.LayerCount - 1) + 1];
            TerrainStyle_Rockies.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Rockies.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Rockies.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Rockies.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[index].TileNum = 0x40;
            TerrainStyle_Rockies.Layers[index].TerrainmapDensity = 0.5f;
            TerrainStyle_Rockies.Layers[index].TerrainmapScale = 1f;
            TerrainStyle_Rockies.Layers[index].HeightMin = 85f;
            TerrainStyle_Rockies.Layers[index].HeightMax = 256f;
            TerrainStyle_Rockies.Layers[index].AvoidLayers = new bool[(TerrainStyle_Rockies.LayerCount - 1) + 1];
            TerrainStyle_Rockies.Layers[index].WithinLayer = index - 1;
            index++;
            TerrainStyle_Rockies.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Rockies.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Rockies.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[index].TileNum = 0x29;
            TerrainStyle_Rockies.Layers[index].TerrainmapDensity = 1f;
            TerrainStyle_Rockies.Layers[index].TerrainmapScale = 0f;
            TerrainStyle_Rockies.Layers[index].HeightMin = 170f;
            TerrainStyle_Rockies.Layers[index].HeightMax = 256f;
            TerrainStyle_Rockies.Layers[index].AvoidLayers = new bool[(TerrainStyle_Rockies.LayerCount - 1) + 1];
            TerrainStyle_Rockies.Layers[index].WithinLayer = -1;
            index++;
            TerrainStyle_Rockies.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Rockies.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Rockies.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[index].TileNum = 0x40;
            TerrainStyle_Rockies.Layers[index].TerrainmapDensity = 0.875f;
            TerrainStyle_Rockies.Layers[index].TerrainmapScale = 1f;
            TerrainStyle_Rockies.Layers[index].HeightMin = 170f;
            TerrainStyle_Rockies.Layers[index].HeightMax = 256f;
            TerrainStyle_Rockies.Layers[index].AvoidLayers = new bool[(TerrainStyle_Rockies.LayerCount - 1) + 1];
            TerrainStyle_Rockies.Layers[index].WithinLayer = index - 1;
            index++;
            TerrainStyle_Rockies.Layers = (clsMap.sGenerateMasterTerrainArgs.clsLayer[]) Utils.CopyArray((Array) TerrainStyle_Rockies.Layers, new clsMap.sGenerateMasterTerrainArgs.clsLayer[index + 1]);
            TerrainStyle_Rockies.Layers[index] = new clsMap.sGenerateMasterTerrainArgs.clsLayer();
            TerrainStyle_Rockies.Layers[index].TileNum = 30;
            TerrainStyle_Rockies.Layers[index].TerrainmapDensity = 1f;
            TerrainStyle_Rockies.Layers[index].TerrainmapScale = 0f;
            TerrainStyle_Rockies.Layers[index].HeightMax = 256f;
            TerrainStyle_Rockies.Layers[index].IsCliff = true;
            TerrainStyle_Rockies.Layers[index].AvoidLayers = new bool[(TerrainStyle_Rockies.LayerCount - 1) + 1];
            TerrainStyle_Rockies.Layers[index].WithinLayer = -1;
            TerrainStyle_Rockies.Tileset = Generator_TilesetRockies;
            Generator_TilesetRockies.BorderTextureNum = 30;
        }

        public static clsUnitType GetUnitTypeFromCode(string Code)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = modProgram.ObjectData.UnitTypes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsUnitType current = (clsUnitType) enumerator.Current;
                    string result = null;
                    if (current.GetCode(ref result) && (result == Code))
                    {
                        return current;
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            return null;
        }
    }
}

