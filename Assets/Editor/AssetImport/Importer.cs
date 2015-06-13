using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Editor.AssetImport;

public class Importer : AssetPostprocessor
{
    private static bool importingFreespace = false;
    private static List<string> skipFiles = new List<string>() { ".DS_STORE", ".txt", ".tbm", ".pof" };
    private static string freespaceImportDestPath;
    private static string freespaceImportAssetName;

    [MenuItem("Assets/Import Freespace")]
    static void ImportFreespace()
    {
        var path = EditorUtility.OpenFolderPanel("Choose Freespace Model Directory", "C:\\Users\\Matt\\Desktop\\Freespace Models", "");
        if (path != string.Empty)
        {
            importingFreespace = true;
            var dirName = Path.GetFileName(path);
            freespaceImportDestPath = "Assets/" + dirName + "_tmp";
            freespaceImportAssetName = dirName;
            Directory.CreateDirectory(freespaceImportDestPath);
            string[] files = Directory.GetFiles(path);
            foreach(string s in files)
            {
                var fileName = Path.GetFileName(s);
                var destFile = Path.Combine(freespaceImportDestPath, fileName);
                var ext = Path.GetExtension(s);
                if (!skipFiles.Contains(ext))
                {
                    File.Copy(s, destFile, true);
                }
            }
            AssetDatabase.Refresh();
        }
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (importingFreespace)
        {
            importingFreespace = false;
            new FreespaceImporter(freespaceImportAssetName, freespaceImportDestPath, importedAssets);
            AssetDatabase.DeleteAsset(freespaceImportDestPath);
            ImportHelpers.WarmupAssetDatabase("Assets/");
            AssetDatabase.Refresh();
            freespaceImportDestPath = null;
            freespaceImportAssetName = null;
        }

    }


}

/* 
 Root
    Materials
    LOD
    Debris
    Meshes
    Shield
    Thrusters
    Gunbanks
    MissileBanks
 */