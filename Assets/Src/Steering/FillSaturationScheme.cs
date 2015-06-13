using UnityEngine;
using System.Collections.Generic;

public class FillSaturationScheme : MonoBehaviour {

    //each plane contributes 50% of total saturation
    //saturation amount is dependant on the absolute value of the difference between initial node's
    //"plane" value and current node's "plane" value. the more similar they are the higher the strength
    //if they are more than resolution - 1 / 2 nodes away the strength inverts
    public static void Saturate(Vector2 index, List<SteeringContextNode> list, int resolution, float strength, float dropOff) {
        int half = (resolution - 1) / 2;
        foreach (var node in list) {
            if (node.index == index) {
                node.score += strength;
            } else {
                var xDiff = (int)Mathf.Abs(index.x - node.index.x);
                var yDiff = (int)Mathf.Abs(index.y - node.index.y);
                float scale = Mathf.Pow(dropOff, xDiff);
                if (scale == 0) scale = 1;
                node.score += strength * scale * 0.5f;

                scale = Mathf.Pow(dropOff, yDiff);
                if (scale == 0) scale = 1;
                node.score += strength * scale * 0.5f;
            }
        }
    }

    public static void Saturate(SteeringNode origin, List<SteeringNode> list, int resolution, float strength, float dropOff) {
        int half = (resolution - 1) / 2;
        for(int i = 0; i < list.Count; i++) {
            var node = list[i];
            if (node == origin) {
                node.score += strength;
            } else {
                var xDiff = (int)Mathf.Abs(origin.x - node.x);
                var yDiff = (int)Mathf.Abs(origin.y - node.y);
                float scale = Mathf.Pow(dropOff, xDiff);
                if (scale == 0) scale = 1;
                node.score += strength * scale * 0.5f;

                scale = Mathf.Pow(dropOff, yDiff);
                if (scale == 0) scale = 1;
                node.score += strength * scale * 0.5f;
            }
        }
    }

    public static bool IsHorizontalEdge(Vector2 nodeId, int size) {
        return nodeId.x == 0 || nodeId.x == size - 1;
    }

    public static bool IsVerticalEdge(Vector2 nodeId, int size) {
        return nodeId.y == 0 || nodeId.y == size - 1;
    }

    public static bool IsCorner(Vector2 nodeId, int size) {
        var x = nodeId.x;
        var y = nodeId.y;
        var farEdge = size - 1;
        return (x == 0 && y == 0 || x == farEdge && y == farEdge || x == 0 && y == farEdge || x == farEdge && y == 0);
    }
}