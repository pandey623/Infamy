using UnityEngine;
using System.Collections;

public class DotHeadingTest : MonoBehaviour {
    public Transform target;
    public OrientedBoundingBox orientedBoundingBox;

    void Start() {
        orientedBoundingBox = new OrientedBoundingBox(transform, GetComponent<Collider>());
    }

    void Update() {

    }

    Vector3? RayPlaneIntersection(Ray ray, Vector3 planeNormal, Vector3 planePoint) {
        float dot = Vector3.Dot(planeNormal, ray.direction);
        if (dot == 0) return null;
        return ray.origin + ray.direction * (-Vector3.Dot(planeNormal, ray.origin - planePoint) / dot);
    }

    Vector3? RayTriangleIntersection(Ray ray, Vector3 normal, Vector3[] points) {
        Vector3? planeIntersection = RayPlaneIntersection(ray, normal, points[0]);
        if (planeIntersection == null) return null;
        if (Vector3.Dot(Vector3.Cross(points[1] - points[0], (Vector3)planeIntersection - points[0]), normal) < 0) return null;
        if (Vector3.Dot(Vector3.Cross(points[2] - points[1], (Vector3)planeIntersection - points[1]), normal) < 0) return null;
        if (Vector3.Dot(Vector3.Cross(points[0] - points[2], (Vector3)planeIntersection - points[2]), normal) < 0) return null;
        return planeIntersection;
    }

    Vector3? RayQuadIntersection(Ray ray, Vector3 normal, Vector3[] points) {
        Vector3? planeIntersection = RayPlaneIntersection(ray, normal, points[0]);
        if (planeIntersection == null) return null;
        if (Vector3.Dot(Vector3.Cross(points[1] - points[0], (Vector3)planeIntersection - points[0]), normal) < 0) return null;
        if (Vector3.Dot(Vector3.Cross(points[2] - points[1], (Vector3)planeIntersection - points[1]), normal) < 0) return null;
        if (Vector3.Dot(Vector3.Cross(points[3] - points[2], (Vector3)planeIntersection - points[2]), normal) < 0) return null;
        if (Vector3.Dot(Vector3.Cross(points[0] - points[3], (Vector3)planeIntersection - points[3]), normal) < 0) return null;
        return planeIntersection;
    }

    Vector3? RayPolygonIntersection(Ray ray, Vector3 normal, Vector3[] points) {
        Vector3? planeIntersection = RayPlaneIntersection(ray, normal, points[0]);
        if (planeIntersection == null) return null;
        int n = points.Length;
        for (int i = 0; i < points.Length; i++) {
            if (Vector3.Dot(Vector3.Cross(points[(i + 1) % n] - points[i], (Vector3)planeIntersection - points[i]), normal) < 0) return null;
        }
        return planeIntersection;
    }

    //if more precisions is needed just do more ray tests, seems cheap enough
    void OnDrawGizmos() {
        if (orientedBoundingBox == null) return;
        orientedBoundingBox.Update(transform.position + transform.forward * 7f, transform.rotation);
        var direction = target.forward;
        Vector3? intersection = orientedBoundingBox.GetLineSegmentIntersectionPoint(transform, new Ray(target.position, direction), 10f);
        if (intersection != null) {
            Gizmos.DrawWireSphere((Vector3)intersection, 0.25f);
            orientedBoundingBox.DrawAsGizmo(Color.green);
        } else {
            orientedBoundingBox.DrawAsGizmo(Color.white);
        }
        Gizmos.DrawRay(target.position, direction * 10f);
    }
}
