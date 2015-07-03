//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using System.Deployment.Internal;
//
//public class SteeringVisualizer : MonoBehaviour {
//    public float zOffset = 0f;
//    public float xyOffset = 1f;
//    public float drawScale = 0.5f;
//    public Gradient dangerGradient;
//    public Gradient interestGradient;
//
//    [HideInInspector]
//    public bool nodesShown = false;
//    private List<SteeringVisualizedNode> visualizedNodes;
//    private GameObject container;
//
//    void Start() {
//        steering = GetComponent<Entity>().engineSystem.contextMap;
//        visualizedNodes = new List<SteeringVisualizedNode>();
//        container = new GameObject();
//        container.transform.parent = transform;
//        container.transform.name = "Visual Steering Container";
//        container.transform.localPosition = Vector3.zero;
//        container.transform.localRotation = Quaternion.identity;
//        container.transform.localScale = Vector3.one;
//        nodesShown = false;
//        Rebuild();
//    }
//
//    private void Update() {
////        if (visualizedNodes.Count != steering.nodes.Count) {
////            Rebuild();
////        }
//        if (nodesShown) {
//            for (int i = 0; i < visualizedNodes.Count; i++) {
//                SteeringVisualizedNode node = visualizedNodes[i];
//                node.score = steering.nodes[i].score;
//                if (node.score >= 0) {
//                    node.color = interestGradient.Evaluate(node.score / steering.maxScore);
//                } else {
//                   node.color = dangerGradient.Evaluate((node.score * -1) / steering.minScore * -1);
//                }
//                //visualizedNodes[i].color = dangerGradient.Evaluate(node[i].score / steering.highest.score);
//
//                //            if (node.score >= 0) {
//                //                node.color = interestGradient.Evaluate(node.score / max);
//                //            } else {
//                //                node.color = dangerGradient.Evaluate((node.score * -1) / min * -1);
//                //            }
//            }
//            visualizedNodes[steering.highest.index].color = Color.magenta;
//        }
//    }
//
//    public void Rebuild() {
//        nodesShown = true;
//        for (var i = 0; i < visualizedNodes.Count; i++) {
//            Destroy(visualizedNodes[i]);
//        }
//        visualizedNodes.Clear();
//
//        float offset = (steering.resolution / 2);
//        if (steering.resolution % 2 == 0) {
//            offset -= 0.5f;
//        }
//        Vector3 drawOffset = new Vector3(offset, offset, 0);
//        for (var i = 0; i < steering.nodes.Length; i++) {
//            var x = steering.nodes[i].x;
//            var y = steering.nodes[i].y;
//            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
//            go.AddComponent<SteeringVisualizedNode>();
//            Destroy(go.GetComponent<Collider>());
//            go.transform.parent = container.transform;
//            go.transform.name = "index: " + steering.nodes[i].index + " x: " + steering.nodes[i].x + ", y: " + steering.nodes[i].y;
//            go.transform.localPosition = (new Vector3(x, y, 0) - drawOffset) * xyOffset;
//            go.transform.localScale = new Vector3(drawScale, drawScale, drawScale);
//            go.transform.localRotation = Quaternion.identity;
//            visualizedNodes.Add(go.GetComponent<SteeringVisualizedNode>());
//        }
//    }
//
//    public void HideNodes() {
//        foreach (var v in visualizedNodes) {
//            Destroy(v.gameObject);
//        }
//        visualizedNodes.Clear();
//        nodesShown = false;
//    }
//
//
//}
//
//[CustomEditor(typeof(SteeringVisualizer))]
//public class SteeringVisualizerEditor : Editor {
//    public override void OnInspectorGUI() {
//        DrawDefaultInspector();
//        SteeringVisualizer sv = (SteeringVisualizer)target;
//        if (sv.nodesShown) {
//            if (GUILayout.Button("Hide Nodes")) {
//                sv.HideNodes();
//            }
//        } else {
//            if (GUILayout.Button("Show Nodes")) {
//                sv.Rebuild();
//            }
//        }
//    }
//}//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using System.Deployment.Internal;
//
//public class SteeringVisualizer : MonoBehaviour {
//    public float zOffset = 0f;
//    public float xyOffset = 1f;
//    public float drawScale = 0.5f;
//    public Gradient dangerGradient;
//    public Gradient interestGradient;
//
//    [HideInInspector]
//    public bool nodesShown = false;
//    private List<SteeringVisualizedNode> visualizedNodes;
//    private GameObject container;
//
//    void Start() {
//        steering = GetComponent<Entity>().engineSystem.contextMap;
//        visualizedNodes = new List<SteeringVisualizedNode>();
//        container = new GameObject();
//        container.transform.parent = transform;
//        container.transform.name = "Visual Steering Container";
//        container.transform.localPosition = Vector3.zero;
//        container.transform.localRotation = Quaternion.identity;
//        container.transform.localScale = Vector3.one;
//        nodesShown = false;
//        Rebuild();
//    }
//
//    private void Update() {
////        if (visualizedNodes.Count != steering.nodes.Count) {
////            Rebuild();
////        }
//        if (nodesShown) {
//            for (int i = 0; i < visualizedNodes.Count; i++) {
//                SteeringVisualizedNode node = visualizedNodes[i];
//                node.score = steering.nodes[i].score;
//                if (node.score >= 0) {
//                    node.color = interestGradient.Evaluate(node.score / steering.maxScore);
//                } else {
//                   node.color = dangerGradient.Evaluate((node.score * -1) / steering.minScore * -1);
//                }
//                //visualizedNodes[i].color = dangerGradient.Evaluate(node[i].score / steering.highest.score);
//
//                //            if (node.score >= 0) {
//                //                node.color = interestGradient.Evaluate(node.score / max);
//                //            } else {
//                //                node.color = dangerGradient.Evaluate((node.score * -1) / min * -1);
//                //            }
//            }
//            visualizedNodes[steering.highest.index].color = Color.magenta;
//        }
//    }
//
//    public void Rebuild() {
//        nodesShown = true;
//        for (var i = 0; i < visualizedNodes.Count; i++) {
//            Destroy(visualizedNodes[i]);
//        }
//        visualizedNodes.Clear();
//
//        float offset = (steering.resolution / 2);
//        if (steering.resolution % 2 == 0) {
//            offset -= 0.5f;
//        }
//        Vector3 drawOffset = new Vector3(offset, offset, 0);
//        for (var i = 0; i < steering.nodes.Length; i++) {
//            var x = steering.nodes[i].x;
//            var y = steering.nodes[i].y;
//            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
//            go.AddComponent<SteeringVisualizedNode>();
//            Destroy(go.GetComponent<Collider>());
//            go.transform.parent = container.transform;
//            go.transform.name = "index: " + steering.nodes[i].index + " x: " + steering.nodes[i].x + ", y: " + steering.nodes[i].y;
//            go.transform.localPosition = (new Vector3(x, y, 0) - drawOffset) * xyOffset;
//            go.transform.localScale = new Vector3(drawScale, drawScale, drawScale);
//            go.transform.localRotation = Quaternion.identity;
//            visualizedNodes.Add(go.GetComponent<SteeringVisualizedNode>());
//        }
//    }
//
//    public void HideNodes() {
//        foreach (var v in visualizedNodes) {
//            Destroy(v.gameObject);
//        }
//        visualizedNodes.Clear();
//        nodesShown = false;
//    }
//
//
//}
//
//[CustomEditor(typeof(SteeringVisualizer))]
//public class SteeringVisualizerEditor : Editor {
//    public override void OnInspectorGUI() {
//        DrawDefaultInspector();
//        SteeringVisualizer sv = (SteeringVisualizer)target;
//        if (sv.nodesShown) {
//            if (GUILayout.Button("Hide Nodes")) {
//                sv.HideNodes();
//            }
//        } else {
//            if (GUILayout.Button("Show Nodes")) {
//                sv.Rebuild();
//            }
//        }
//    }
//}