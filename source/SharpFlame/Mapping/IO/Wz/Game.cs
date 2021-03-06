#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NLog;
using SharpFlame.FileIO;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Mapping.IO.Wz
{
    public class Game : Wz
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public Game(clsMap newMap) : base(newMap)
        {
        }

        public override clsResult Load(string path)
        {
            var returnResult =
                new clsResult("Loading game file from \"{0}\"".Format2(path), false);
            logger.Info("Loading game file from \"{0}\"", path);
            var subResult = new sResult();

            map.Tileset = null;

            map.TileType_Reset();
            map.SetPainterToDefaults();

            var gameSplitPath = new sSplitPath(path);
            var gameFilesPath = gameSplitPath.FilePath + gameSplitPath.FileTitleWithoutExtension + Convert.ToString(App.PlatformPathSeparator);
            var mapDirectory = "";
            FileStream file = null;

            subResult = IOUtil.TryOpenFileStream(path, ref file);
            if ( !subResult.Success )
            {
                returnResult.ProblemAdd("Game file not found: " + subResult.Problem);
                return returnResult;
            }

            var Map_Reader = new BinaryReader(file);
            subResult = read_WZ_gam(Map_Reader);
            Map_Reader.Close();

            if ( !subResult.Success )
            {
                returnResult.ProblemAdd(subResult.Problem);
                return returnResult;
            }

            subResult = IOUtil.TryOpenFileStream(gameFilesPath + "game.map", ref file);
            if ( !subResult.Success )
            {
                if ( MessageBox.Show("game.map file not found at \"{0}\"\n" +
                                     "Do you want to select another directory to load the underlying map from?".Format2(gameFilesPath),
                    "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel )
                {
                    returnResult.ProblemAdd("Aborted.");
                    return returnResult;
                }
                var DirectorySelect = new FolderBrowserDialog();
                DirectorySelect.SelectedPath = gameFilesPath;
                if ( DirectorySelect.ShowDialog() != DialogResult.OK )
                {
                    returnResult.ProblemAdd("Aborted.");
                    return returnResult;
                }
                mapDirectory = DirectorySelect.SelectedPath + Convert.ToString(App.PlatformPathSeparator);

                subResult = IOUtil.TryOpenFileStream(mapDirectory + "game.map", ref file);
                if ( !subResult.Success )
                {
                    returnResult.ProblemAdd("game.map file not found: " + subResult.Problem);
                    return returnResult;
                }
            }
            else
            {
                mapDirectory = gameFilesPath;
            }

            var Map_ReaderB = new BinaryReader(file);
            subResult = read_WZ_map(Map_ReaderB);
            Map_ReaderB.Close();

            if ( !subResult.Success )
            {
                returnResult.ProblemAdd(subResult.Problem);
                return returnResult;
            }

            var bjoUnits = new List<WZBJOUnit>();

            var iniFeatures = new List<IniFeature>();
            subResult = IOUtil.TryOpenFileStream(gameFilesPath + "feature.ini", ref file);
            if ( subResult.Success )
            {
                using ( var reader = new StreamReader(file) )
                {
                    var text = reader.ReadToEnd();
                    returnResult.Add(read_INI_Features(text, iniFeatures));
                }
            }

            if ( iniFeatures.Count == 0 ) // no feature.ini
            {
                var Result = new clsResult("feat.bjo", false);
                logger.Info("Loading feat.bjo");
                subResult = IOUtil.TryOpenFileStream(gameFilesPath + "feat.bjo", ref file);
                if ( !subResult.Success )
                {
                    Result.WarningAdd("file not found");
                }
                else
                {
                    var Features_Reader = new BinaryReader(file);
                    subResult = read_WZ_Features(Features_Reader, bjoUnits);
                    Features_Reader.Close();
                    if ( !subResult.Success )
                    {
                        Result.WarningAdd(subResult.Problem);
                    }
                }
                returnResult.Add(Result);
            }

            {
                var Result = new clsResult("ttypes.ttp", false);
                logger.Info("Loading ttypes.ttp");
                subResult = IOUtil.TryOpenFileStream(mapDirectory + "ttypes.ttp", ref file);
                if ( !subResult.Success )
                {
                    Result.WarningAdd("file not found");
                }
                else
                {
                    var TileTypes_Reader = new BinaryReader(file);
                    subResult = read_WZ_TileTypes(TileTypes_Reader);
                    TileTypes_Reader.Close();
                    if ( !subResult.Success )
                    {
                        Result.WarningAdd(subResult.Problem);
                    }
                }
                returnResult.Add(Result);
            }

            var iniStructures = new List<IniStructure>();
            subResult = IOUtil.TryOpenFileStream(gameFilesPath + "struct.ini", ref file);
            if ( subResult.Success )
            {
                using ( var reader = new StreamReader(file) )
                {
                    var text = reader.ReadToEnd();
                    returnResult.Add(read_INI_Structures(text, iniStructures));
                }
            }

            if ( iniStructures.Count == 0 ) // no struct.ini
            {
                var Result = new clsResult("struct.bjo", false);
                logger.Info("Loading struct.bjo");
                subResult = IOUtil.TryOpenFileStream(gameFilesPath + "struct.bjo", ref file);
                if ( !subResult.Success )
                {
                    Result.WarningAdd("struct.bjo file not found.");
                }
                else
                {
                    var Structures_Reader = new BinaryReader(file);
                    subResult = read_WZ_Structures(Structures_Reader, bjoUnits);
                    Structures_Reader.Close();
                    if ( !subResult.Success )
                    {
                        Result.WarningAdd(subResult.Problem);
                    }
                }
                returnResult.Add(Result);
            }

            var iniDroids = new List<IniDroid>();
            subResult = IOUtil.TryOpenFileStream(gameFilesPath + "droid.ini", ref file);
            if ( subResult.Success )
            {
                using ( var reader = new StreamReader(file) )
                {
                    var text = reader.ReadToEnd();
                    returnResult.Add(read_INI_Droids(text, iniDroids));
                }
            }

            if ( iniDroids.Count == 0 ) // no droid.ini
            {
                var Result = new clsResult("dinit.bjo", false);
                logger.Info("Loading dinit.bjo");
                subResult = IOUtil.TryOpenFileStream(gameFilesPath + "dinit.bjo", ref file);
                if ( !subResult.Success )
                {
                    Result.WarningAdd("dinit.bjo file not found.");
                }
                else
                {
                    var Droids_Reader = new BinaryReader(file);
                    subResult = read_WZ_Droids(Droids_Reader, bjoUnits);
                    Droids_Reader.Close();
                    if ( !subResult.Success )
                    {
                        Result.WarningAdd(subResult.Problem);
                    }
                }
                returnResult.Add(Result);
            }

            returnResult.Add(createWZObjects(bjoUnits, iniStructures, iniDroids, iniFeatures));

            //map objects are modified by this and must already exist
            subResult = IOUtil.TryOpenFileStream(gameFilesPath + "labels.ini", ref file);
            if ( subResult.Success )
            {
                using ( var reader = new StreamReader(file) )
                {
                    var text = reader.ReadToEnd();
                    returnResult.Add(read_INI_Labels(text));
                }
            }

            return returnResult;
        }
    }
}