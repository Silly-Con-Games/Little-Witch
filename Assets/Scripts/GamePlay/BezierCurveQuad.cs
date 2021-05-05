
using UnityEngine;
using UnityEngine.Assertions;

public class BezierCurveQuad 
{
    Vector3[] controlPoints;
    int segmentCnt;

    public BezierCurveQuad(Vector3[] points)
    {
        Assert.IsTrue(points.Length >= 3);
        controlPoints = points;

        segmentCnt = points.Length - 2;
    }

    public Vector3 PointAt(float t)
    {
        Assert.IsTrue(t >= 0 && t <= 1);

        int segmentIndex = (int) (segmentCnt * t);
        if (segmentIndex == segmentCnt)
            segmentIndex -= 1;

        return GetPoint(controlPoints[segmentIndex], controlPoints[segmentIndex + 1], controlPoints[segmentIndex + 2], t * segmentCnt - segmentIndex);
    }

    public Vector3 FirstDerivativeAt(float t)
    {
        Assert.IsTrue(t >= 0 && t <= 1);

        int segmentIndex = (int)(segmentCnt * t);
        if (segmentIndex == segmentCnt)
            segmentIndex -= 1;

        return GetFirstDerivative(controlPoints[segmentIndex], controlPoints[segmentIndex + 1], controlPoints[segmentIndex + 2], t * segmentCnt - segmentIndex);
    }

    Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            2f * (1f - t) * (p1 - p0) +
            2f * t * (p2 - p1);
    }
}
