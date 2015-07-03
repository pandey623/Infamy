using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MonoContextMap : MonoBehaviour {

    public Transform monoPrefab;
    public Dictionary<Vector3, MonoContextNode> nodeMap;
    public List<MonoContextNode> nodeList = new List<MonoContextNode>();
    public int subdivisions;
    public float spacing = 3;
    public float boxScale = 0.2f;
    public bool diagonalNeighbors = false;
    public bool additiveMode = true;
    public float rearNodePenalty = 0.2f;
    public bool renderNodes = true;

	void Start () {
        nodeMap = CreateContextMap(subdivisions, ref nodeList);    
	}
	
	void Update () {
  //      foreach (var node in nodeList) {
  //          DrawArrow.ForDebug(transform.position, node.weightedInterestedVector, Color.green);
 //           DrawArrow.ForDebug(transform.position, node.rawInterestVector, Color.yellow);
//            DrawArrow.ForDebug(transform.position, node.GetAggregateInterestVector() * 5, Color.magenta);
 //       }
	}

    public MonoContextNode GetContextNode(Vector3 normalizedDirection) {
        var index = Vector3.zero;
        index.x = GetIndex(subdivisions, Vector3.Dot(transform.right, normalizedDirection));
        index.y = GetIndex(subdivisions, Vector3.Dot(transform.up, normalizedDirection));
        index.z = GetIndex(subdivisions, Vector3.Dot(transform.forward, normalizedDirection));
        MonoContextNode slot;
        if (nodeMap.TryGetValue(index, out slot)) {
            return slot;
        }
        return null;
    }

    public static int GetIndex(int subdvisions, float input) {
        if (input == 1) return subdvisions - 1;
        var percent = (input + 1) / 2;
        return (int)(subdvisions * percent);
    }

    public void AlignInSphere() {
        var drawOffset = GetDrawOffset();
        foreach (var node in nodeList) {
            var direction = (new Vector3(node.id.x, node.id.y, node.id.z) - drawOffset).normalized;
            node.transform.position = direction * spacing;

        }
    }

    public void AlignInCube() {
        var drawOffset = GetDrawOffset();

        foreach(var node in nodeList) {
            node.transform.position = (new Vector3(node.id.x, node.id.y, node.id.z) - drawOffset) * spacing;
            node.transform.localScale = new Vector3(boxScale, boxScale, boxScale);
        }

    }

    public Vector3 GetDrawOffset() {
        float offset = (subdivisions / 2);
        if (subdivisions % 2 == 0) {
            offset -= 0.5f;
        }
        return new Vector3(offset, offset, offset);
    }
    
    public Dictionary<Vector3, MonoContextNode> CreateContextMap(int subdivisions, ref List<MonoContextNode> nodeList) {
        var nodeMap = new Dictionary<Vector3, MonoContextNode>();
        if (nodeList == null) {
            nodeList = new List<MonoContextNode>();
        }

        //float offset = (subdivisions / 2);
        //if (subdivisions % 2 == 0) {
        //    offset -= 0.5f;
        //}
        Vector3 drawOffset = GetDrawOffset();

        var maxEdge = subdivisions - 1;
        var counter = 0;
        for (var x = 0; x < subdivisions; x++) {
            for (var y = 0; y < subdivisions; y++) {
                for (var z = 0; z < subdivisions; z++) {
                    var v = new Vector3(x, y, z);
                    if (x == maxEdge || y == maxEdge || z == maxEdge || z == 0 || x == 0 || y == 0) {
                        var n = Instantiate(monoPrefab);
                        var ctx = n.GetComponent<MonoContextNode>();
                        ctx.id = new Vector3(x, y, z);
                        nodeMap[ctx.id] = ctx;
                        ctx.transform.name = "ContextNode" + (++counter);
                        ctx.transform.parent = transform;
                        ctx.offset = drawOffset;
                        ctx.additiveMode = additiveMode;
                        ctx.rearNodePenalty = rearNodePenalty;
                        nodeList.Add(ctx);
                    }
                }
            }
        }

        var centerValue = 0;
        if (subdivisions % 2 == 0) {
            centerValue = subdivisions / 2;
        } else {
            centerValue = (subdivisions - 1) / 2;
        }


        if (diagonalNeighbors) {
            //all neighbors used here
            foreach (var node in nodeList) {
                Vector3[] array = new Vector3[26];
                var rightIndex = node.id.x;
                var upIndex = node.id.y;
                var forwardIndex = node.id.z;
                array[0] = new Vector3(rightIndex + 1, upIndex + 1, forwardIndex);       //top center right
                array[1] = new Vector3(rightIndex, upIndex + 1, forwardIndex);           //top center center
                array[2] = new Vector3(rightIndex - 1, upIndex + 1, forwardIndex);       //top center left

                array[3] = new Vector3(rightIndex + 1, upIndex + 1, forwardIndex + 1);   //top forward right
                array[4] = new Vector3(rightIndex, upIndex + 1, forwardIndex + 1);       //top forward center
                array[5] = new Vector3(rightIndex - 1, upIndex + 1, forwardIndex + 1);   //top forward left

                array[6] = new Vector3(rightIndex + 1, upIndex + 1, forwardIndex - 1);   //top back right
                array[7] = new Vector3(rightIndex, upIndex + 1, forwardIndex - 1);       //top back center
                array[8] = new Vector3(rightIndex - 1, upIndex + 1, forwardIndex - 1);   //top back left

                array[9] = new Vector3(rightIndex + 1, upIndex, forwardIndex + 1);       //center forward right
                array[10] = new Vector3(rightIndex, upIndex, forwardIndex + 1);          //center forward center
                array[11] = new Vector3(rightIndex - 1, upIndex, forwardIndex + 1);      //center forward left

                array[12] = new Vector3(rightIndex + 1, upIndex, forwardIndex);           //center center right
                array[13] = new Vector3(rightIndex - 1, upIndex, forwardIndex);          //center center left

                array[14] = new Vector3(rightIndex + 1, upIndex, forwardIndex - 1);      //center back right
                array[15] = new Vector3(rightIndex, upIndex, forwardIndex - 1);          //center back center
                array[16] = new Vector3(rightIndex - 1, upIndex, forwardIndex - 1);      //center back left

                array[17] = new Vector3(rightIndex + 1, upIndex - 1, forwardIndex);      //bottom center right
                array[18] = new Vector3(rightIndex, upIndex - 1, forwardIndex);          //bottom center center
                array[19] = new Vector3(rightIndex - 1, upIndex - 1, forwardIndex);      //bottom center left

                array[20] = new Vector3(rightIndex + 1, upIndex - 1, forwardIndex + 1);  //bottom forward right
                array[21] = new Vector3(rightIndex, upIndex - 1, forwardIndex + 1);      //bottom forward center
                array[22] = new Vector3(rightIndex - 1, upIndex - 1, forwardIndex + 1);  //bottom forward left

                array[23] = new Vector3(rightIndex + 1, upIndex - 1, forwardIndex - 1);  //bottom back right
                array[24] = new Vector3(rightIndex, upIndex - 1, forwardIndex - 1);      //bottom back center
                array[25] = new Vector3(rightIndex - 1, upIndex - 1, forwardIndex - 1);  //bottom back left

                foreach (var id in array) {
                    MonoContextNode neighbor;
                    if (nodeMap.TryGetValue(id, out neighbor)) {
                        node.neighbors.Add(neighbor);
                    }
                }
            }
        } else {
            //only non diagonal adjacent neighbors used here
            foreach (var node in nodeList) {
                Vector3[] array = new Vector3[6];
                var rightIndex = node.id.x;
                var upIndex = node.id.y;
                var forwardIndex = node.id.z;
                array[0] = new Vector3(rightIndex, upIndex + 1, forwardIndex);           //top center center
                array[1] = new Vector3(rightIndex, upIndex, forwardIndex + 1);          //center forward center
                array[2] = new Vector3(rightIndex + 1, upIndex, forwardIndex);           //center center right
                array[3] = new Vector3(rightIndex - 1, upIndex, forwardIndex);          //center center left
                array[4] = new Vector3(rightIndex, upIndex, forwardIndex - 1);          //center back center
                array[5] = new Vector3(rightIndex, upIndex - 1, forwardIndex);          //bottom center center

                for (var i = 0; i < array.Length; i++) {
                    MonoContextNode neighbor;
                    var id = array[i];
                    if (id == Vector3.zero) continue;
                    if (nodeMap.TryGetValue(id, out neighbor)) {
                        node.neighbors.Add(neighbor);
                    }
                }
    
            }
        }

        AlignInCube();
        //no neighbors on the center node
        var centerNode = Instantiate(monoPrefab);
        var centerCtx = centerNode.GetComponent<MonoContextNode>();
        centerCtx.id = new Vector3(centerValue, centerValue, centerValue);
        centerCtx.GetComponent<MeshRenderer>().enabled = false;
        nodeMap[centerCtx.id] = centerCtx;
        return nodeMap;
    }
}


[CustomEditor(typeof(MonoContextMap))]
public class MonoContextMapEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        MonoContextMap map = (MonoContextMap)target;
        if (GUILayout.Button("Rebuild Context Map")) {
            map.nodeList.ForEach((node) => Destroy(node.gameObject));
            map.nodeList.Clear();
            map.nodeMap = map.CreateContextMap(map.subdivisions, ref map.nodeList);
        }

        if (GUILayout.Button("Align in Sphere")) {
            map.AlignInSphere();
        }

        if (GUILayout.Button("Align in Cube")) {
            map.AlignInCube();
        }

        if (GUILayout.Button("Reset All Nodes")) {
            map.nodeList.ForEach((node) => node.Reset());
        }
    }
}