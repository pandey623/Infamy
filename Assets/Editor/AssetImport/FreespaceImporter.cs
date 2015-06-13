using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.AssetImport {
    /*
         Root/Faction/Class/AssetName(ship, container, entity, w/e)
    */
    class FreespaceImporter {
        private static string ROOT = "Freespace";
        private string[] importedAssets;
        private string mainAssetName;
        private string destinationRoot;
        private string sourceRoot;
        private UnityEngine.Object[] loadedAssets = null;
        private static List<String> TextureExtensions = new List<String>() {
            ".tga", ".tiff", ".jpg", ".jpeg", ".png", ".dds", ".bmp"
        };

        private Dictionary<string, Material> materialLookup = new Dictionary<string, Material>();
        private Dictionary<string, Mesh> meshLookup = new Dictionary<string, Mesh>();
        private Dictionary<string, Texture2D> textureLookup = new Dictionary<string, Texture2D>();

        public FreespaceImporter(string mainAssetName, string sourceRoot, string[] importedAssets) {
            this.mainAssetName = mainAssetName;
            this.importedAssets = importedAssets;
            this.sourceRoot = sourceRoot;
            Init();
        }

        private void Init() {
            destinationRoot = "Assets/" + ROOT + "/" + mainAssetName;
            if (Directory.Exists(destinationRoot)) {
                bool result = EditorUtility.DisplayDialog("Really overwrite assets?",
                    "Assets exist at " + destinationRoot + "\nAre you sure you want to overwrite?",
                    "Yes, overwrite", "No, take me back");
                if (!result) {
                    return;
                }
            }
            loadedAssets = AssetDatabase.LoadAllAssetsAtPath(GetAssetToLoadPath(sourceRoot));
            Directory.CreateDirectory(destinationRoot);
            ImportMaterials();
            ImportTextures();
            MatchTexturesToMaterials();
            ImportMeshes();
            CreatePrefabs();
            loadedAssets = null; //destruct just in case
        }

        private void ImportMeshes() {
            var meshRoot = destinationRoot + "/meshes/";
            Directory.CreateDirectory(meshRoot);
            Assets.Editor.AssetImport.ImportHelpers.WarmupAssetDatabase(meshRoot);
            for (int i = 0; i < loadedAssets.Length; i++) {
                UnityEngine.Object asset = loadedAssets[i];
                if (asset.GetType() == typeof(MeshFilter)) {
                    Mesh mesh = ImportHelpers.CloneMesh(((MeshFilter)asset).sharedMesh);
                    mesh.name = mainAssetName.ToLower() + "_" + mesh.name;
                    AssetDatabase.CreateAsset(mesh, meshRoot + mesh.name + ".asset");
                    meshLookup.Add(asset.name, mesh);
                }
            }
        }

        private void MatchTexturesToMaterials() //todo -- set normal maps, glow maps etc if possible
        {
            foreach (KeyValuePair<string, Material> matKvP in materialLookup) {
                Material mat = matKvP.Value;
                string matName = Path.GetFileNameWithoutExtension(matKvP.Key);
                Texture2D tex;
                if (textureLookup.TryGetValue(matName, out tex)) {
                    mat.mainTexture = tex;
                }
            }
        }

        private void ImportTextures() {
            var textureRoot = destinationRoot + "/textures/";
            Directory.CreateDirectory(textureRoot);
            ImportHelpers.WarmupAssetDatabase(textureRoot);
            foreach (string file in importedAssets) {
                if (TextureExtensions.Contains(Path.GetExtension(file).ToLower())) {
                    var name = mainAssetName.ToLower() + "_" + Path.GetFileName(file).ToLower();
                    AssetDatabase.MoveAsset(file, textureRoot + name);
                    var tex = AssetDatabase.LoadAssetAtPath(textureRoot + name, typeof(Texture2D)) as Texture2D;
                    textureLookup.Add(Path.GetFileNameWithoutExtension(name), tex);
                }
            }
        }

        private void ImportMaterials() {
            var materialRoot = destinationRoot + "/materials/";
            Directory.CreateDirectory(materialRoot);
            foreach (string file in importedAssets) {
                if (Path.GetExtension(file) == ".mat") {
                    var name = mainAssetName.ToLower() + "_" + Path.GetFileName(file).ToLower();
                    var newMat = new Material(Shader.Find("Diffuse"));
                    materialLookup.Add(Path.GetFileNameWithoutExtension(name), newMat);
                    AssetDatabase.CreateAsset(newMat, materialRoot + name);
                }
            }
        }


        private void CreatePrefabs() {
            for (int i = 0; i < loadedAssets.Length; i++) {
                var asset = loadedAssets[i];
                if (asset.GetType() == typeof(GameObject)) {
                    var go = asset as GameObject;
                    var name = go.name.ToLower();
                    if (name.StartsWith("debris")) {
                        ClonePrefabNode("debris", go);
                    } else if (name.StartsWith("cockpit")) {
                        ClonePrefabNode("cockpit", go);
                    } else if (name.StartsWith("detail")) {
                        ClonePrefabNode("detail", go);
                    } else if (name.StartsWith("shield")) {
                        ClonePrefabNode("shield", go);
                    } else if (name.StartsWith("thrusters")) {
                        CloneHeirarchyNode("thrusters", "thruster", go);
                    } else if (name.StartsWith("missilebank") && !name.Contains("-")) {
                        CloneHeirarchyNode("missilebanks", "missilebank", go);
                    } else if (name.StartsWith("gunbank") && !name.Contains("-")) {
                        CloneHeirarchyNode("gunbanks", "gunbank", go);
                    } else if (name.StartsWith("turret")) {

                    } else if (name.StartsWith("moi")) {

                    } else if (name.StartsWith("dockpoint")) {

                    }
                }
            }
        }

        private void CloneHeirarchyNode(string writeLocation, string childNameStartsWith, GameObject source) {
            Transform sourceTransform = source.transform;
            GameObject clone = ImportHelpers.CreateGameObjectFromTransform(sourceTransform);
            Directory.CreateDirectory(destinationRoot + "/" + writeLocation);
            int childCount = sourceTransform.childCount;
            for (int i = 0; i < childCount; i++) {
                var sourceChild = sourceTransform.GetChild(i);
                if (sourceChild.name.StartsWith(childNameStartsWith)) {
                    GameObject child = ImportHelpers.CreateMarkerFromTransform(sourceChild);
                    child.transform.SetParent(clone.transform);
                }
            }
            PrefabUtility.CreatePrefab(destinationRoot + "/" + writeLocation + "/" + clone.name + ".prefab", clone);
            UnityEngine.Object.DestroyImmediate(clone); //remove object from heirarchy (its added automatically)
        }

        private void ClonePrefabNode(string writeLocation, GameObject source) {
            source.name = mainAssetName.ToLower() + "_" + source.name;
            var dest = destinationRoot + "/" + writeLocation + "/" + source.name + ".prefab";
            GameObject clone = ImportHelpers.CreateGameObjectFromTransform(source.transform);
            clone.name = mainAssetName.ToLower() + "_" + clone.name;
            UseMeshFromSourcePrefab(clone, source);
            UseMeshRendererFromSourcePrefab(clone, source);
            Directory.CreateDirectory(destinationRoot + "/" + writeLocation);
            PrefabUtility.CreatePrefab(dest, clone);
            UnityEngine.Object.DestroyImmediate(clone); //remove object from heirarchy (its added automatically)
        }

        private static string GetAssetToLoadPath(string root) {
            return root + "/" + Path.GetFileName(root.Substring(0, root.Length - 4)).ToLower() + ".dae";
        }

        private void UseMeshFromSourcePrefab(GameObject clone, GameObject source) {
            var cloneMF = source.GetComponent<MeshFilter>();
            if (cloneMF != null) {
                MeshFilter mf = clone.AddComponent<MeshFilter>() as MeshFilter;
                Mesh mesh;
                if (meshLookup.TryGetValue(cloneMF.sharedMesh.name, out mesh)) {
                    mf.mesh = mesh;
                }
            }
        }

        private void UseMeshRendererFromSourcePrefab(GameObject clone, GameObject source) {
            var sourceRenderer = source.GetComponent<MeshRenderer>();
            if (sourceRenderer != null) {
                MeshRenderer mr = clone.AddComponent<MeshRenderer>();
                Material[] materials = new Material[sourceRenderer.sharedMaterials.Length];
                for (int i = 0; i < sourceRenderer.sharedMaterials.Length; i++) {
                    var sourceMat = sourceRenderer.sharedMaterials[i];
                    var sourceMatName = mainAssetName.ToLower() + "_" + sourceMat.name.ToLower();
                    Material mat;
                    if (materialLookup.TryGetValue(sourceMatName, out mat)) {
                        materials[i] = mat;
                    }
                }

                mr.materials = materials;
            }
        }
    }
}