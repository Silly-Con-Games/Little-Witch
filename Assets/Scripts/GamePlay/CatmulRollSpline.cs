using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CatmulRollSpline 
{
	Vector3[] controlPoints;
	int segmentCnt;

	public CatmulRollSpline(Vector3[] controlPoints)
	{
		Assert.IsTrue(controlPoints.Length >= 4);
		this.controlPoints = controlPoints;

		segmentCnt = controlPoints.Length - 3;
	}

	public Vector3 PointAt(float t)
	{
		Assert.IsTrue(t >= 0 && t <= 1);

		int segmentIndex = (int)(segmentCnt * t) + 1;
		if (segmentIndex == segmentCnt + 1)
			segmentIndex -= 1;

		return GetCatmullRomPosition(t * segmentCnt - (segmentIndex - 1), controlPoints[segmentIndex - 1], controlPoints[segmentIndex], controlPoints[segmentIndex + 1], controlPoints[segmentIndex + 2]);
	}

    public Vector3 FirstDerivativeAt(float t)
    {
        Assert.IsTrue(t >= 0 && t <= 1);

		int segmentIndex = (int)(segmentCnt * t) + 1;
		if (segmentIndex == segmentCnt + 1)
			segmentIndex -= 1;

		return GetCatmullRomDerivative(t * segmentCnt - (segmentIndex - 1), controlPoints[segmentIndex - 1], controlPoints[segmentIndex], controlPoints[segmentIndex + 1], controlPoints[segmentIndex + 2]);
    }

    // https://www.mvps.org/directx/articles/catmull/
    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		//The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
		Vector3 a = 2f * p1;
		Vector3 b = p2 - p0;
		Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

		//The cubic polynomial: a + b * t + c * t^2 + d * t^3
		Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

		return pos;
	}

	Vector3 GetCatmullRomDerivative(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		//The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
		Vector3 a = 2f * p1;
		Vector3 b = p2 - p0;
		Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
		Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

		//The cubic polynomial: a + b * t + c * t^2 + d * t^3
		Vector3 pos = 0.5f * b  + (c * 2 * t ) + (d * 3 * t * t);

		return pos;
	}
}
