using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Assets.Editor.AssetImport
{
    public class ImportHelpers
    {
        private static Quaternion qRotateX = Quaternion.AngleAxis(90.0f, Vector3.right);
        private static Quaternion qRotateZ = Quaternion.AngleAxis(180.0f, Vector3.forward);

        //warm up the asset database, smells like a bug
        public static void WarmupAssetDatabase(string location)
        {
            AssetDatabase.CreateAsset(new Material(Shader.Find("Diffuse")), location + "tmp.mat");
            AssetDatabase.DeleteAsset(location + "tmp.mat");
        }


        public static GameObject CreateMarkerFromTransform(Transform toClone)
        {
            Vector3 transformedPosition = new Vector3();
            transformedPosition.x =  toClone.position.x;
            transformedPosition.y =  toClone.position.y;
            transformedPosition.z = -toClone.position.z;

            GameObject clone = new GameObject();
            clone.transform.name = toClone.name;
            clone.transform.position = transformedPosition;
            clone.transform.rotation = Quaternion.identity;
            clone.transform.localScale = new Vector3(1, 1, 1);
            return clone;
        }

        public static GameObject CreateGameObjectFromTransform(Transform toClone/*, bool shouldZeroRotation = true*/)
        {
            //Quaternion rotation = shouldZeroRotation ? Quaternion.identity : toClone.rotation;

            Vector3 pos = qRotateX * toClone.position;
            pos = qRotateZ * pos;

            GameObject clone = new GameObject();
            clone.transform.name = toClone.name;
            clone.transform.position = toClone.position;
            clone.transform.rotation = Quaternion.identity;
            clone.transform.localScale = toClone.localScale;
            return clone;
        }

        //todo this really needs to be threaded, some meshes are quite large
        public static Mesh CloneMesh(Mesh mesh)
        {
            Mesh newmesh = new Mesh();
            newmesh.name = mesh.name;
            var newverts = new Vector3[mesh.vertices.Length];
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                var vertex = mesh.vertices[i];
                Vector3 v = qRotateX * vertex;
                newverts[i] = qRotateZ * v;
            }
            newmesh.vertices = newverts;
            newmesh.subMeshCount = mesh.subMeshCount;
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] triangles = mesh.GetTriangles(i);
                newmesh.SetTriangles(triangles, i);
            }
            newmesh.uv = mesh.uv;
            newmesh.uv2 = mesh.uv2;
            newmesh.uv2 = mesh.uv2;
            newmesh.normals = mesh.normals;
            newmesh.colors = mesh.colors;
            newmesh.tangents = mesh.tangents;

            return newmesh;
        }
    }
}

//flip y and z
//invert y
//zero rotation
//reset scale to 1