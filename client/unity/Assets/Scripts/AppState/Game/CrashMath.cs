using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class CrashMath
{
    public static Plane GetCreatePlaneFromQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, bool reverse)
    {
        Vector3[] tris = new Vector3[4];
        tris[0] = reverse ? p3 : p0;
        tris[1] = reverse ? p2 : p1;
        tris[2] = reverse ? p1 : p2;
        tris[3] = reverse ? p0 : p3;
        Plane plane;
        GeometryUtility.TryCreatePlaneFromPolygon(tris, out plane);
        return plane;
    }

    public static void GetFrustumPlanes(Camera camera, Rect rectSS, Plane[] frustumPlanes)
    {
        var frustumPoints = new Vector3[8];
        var cameraViewportRect = new Rect(
            new Vector2(rectSS.center.x / camera.pixelWidth, rectSS.center.y / camera.pixelHeight),
            new Vector2(rectSS.width / camera.pixelWidth, rectSS.height / camera.pixelHeight)
        );
        cameraViewportRect.position -= cameraViewportRect.size / 2;
            
        camera.CalculateFrustumCorners(cameraViewportRect, camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumPoints);
        Array.Copy(frustumPoints, 0, frustumPoints, 4, 4);
        camera.CalculateFrustumCorners(cameraViewportRect, camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumPoints);
        for (int i = 0; i < frustumPoints.Length; i++)
            frustumPoints[i] = camera.transform.TransformPoint(frustumPoints[i]);
            
        frustumPlanes[0] = GetCreatePlaneFromQuad(frustumPoints[0], frustumPoints[1], frustumPoints[5], frustumPoints[4], false);
        frustumPlanes[1] = GetCreatePlaneFromQuad(frustumPoints[2], frustumPoints[3], frustumPoints[7], frustumPoints[6], false);
        frustumPlanes[2] = GetCreatePlaneFromQuad(frustumPoints[0], frustumPoints[3], frustumPoints[7], frustumPoints[4], true);
        frustumPlanes[3] = GetCreatePlaneFromQuad(frustumPoints[1], frustumPoints[2], frustumPoints[6], frustumPoints[5], false);
        frustumPlanes[4] = GetCreatePlaneFromQuad(frustumPoints[0], frustumPoints[1], frustumPoints[2], frustumPoints[3], true);
        frustumPlanes[5] = GetCreatePlaneFromQuad(frustumPoints[4], frustumPoints[5], frustumPoints[6], frustumPoints[7], false);
    }

    public static Vector3Int EncodePosition(Vector3 position)
    {
        return new Vector3Int(Mathf.FloorToInt(position.x * 1000.0f), Mathf.FloorToInt(position.y * 1000.0f),Mathf.FloorToInt(position.z * 1000.0f));
    }
    public static Vector3 DecodePosition(Vector3Int position)
    {
        return new Vector3(position.x / 1000.0f, position.y / 1000.0f, position.z / 1000.0f);
    }
}