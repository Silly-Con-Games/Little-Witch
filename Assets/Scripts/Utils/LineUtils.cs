using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;

// from https://gist.github.com/sinbad/68cb88e980eeaed0505210d052573724
public static class LineUtils
{
    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    public static bool Approximately(float a, float b, float tolerance = 1e-5f)
    {
        return Mathf.Abs(a - b) <= tolerance;
    }

    public static float CrossProduct2D(Vector2 a, Vector2 b)
    {
        return a.x * b.y - b.x * a.y;
    }

    public static bool IntersectLineSegments2D(Vector2 p1start, Vector2 p1end, Vector2 p2start, Vector2 p2end,
        out Vector2 intersection)
    {
        // Consider:
        //   p1start = p
        //   p1end = p + r
        //   p2start = q
        //   p2end = q + s
        // We want to find the intersection point where :
        //  p + t*r == q + u*s
        // So we need to solve for t and u
        var p = p1start;
        var r = p1end - p1start;
        var q = p2start;
        var s = p2end - p2start;
        var qminusp = q - p;

        float cross_rs = CrossProduct2D(r, s);

        if (Approximately(cross_rs, 0f))
        {
            // Parallel lines
            if (Approximately(CrossProduct2D(qminusp, r), 0f))
            {
                // Co-linear lines, could overlap
                float rdotr = Vector2.Dot(r, r);
                float sdotr = Vector2.Dot(s, r);
                // this means lines are co-linear
                // they may or may not be overlapping
                float t0 = Vector2.Dot(qminusp, r / rdotr);
                float t1 = t0 + sdotr / rdotr;
                if (sdotr < 0)
                {
                    // lines were facing in different directions so t1 > t0, swap to simplify check
                    Swap(ref t0, ref t1);
                }

                if (t0 <= 1 && t1 >= 0)
                {
                    // Nice half-way point intersection
                    float t = Mathf.Lerp(Mathf.Max(0, t0), Mathf.Min(1, t1), 0.5f);
                    intersection = p + t * r;
                    return true;
                }
                else
                {
                    // Co-linear but disjoint
                    intersection = Vector2.zero;
                    return false;
                }
            }
            else
            {
                // Just parallel in different places, cannot intersect
                intersection = Vector2.zero;
                return false;
            }
        }
        else
        {
            // Not parallel, calculate t and u
            float t = CrossProduct2D(qminusp, s) / cross_rs;
            float u = CrossProduct2D(qminusp, r) / cross_rs;
            if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
            {
                intersection = p + t * r;
                return true;
            }
            else
            {
                // Lines only cross outside segment range
                intersection = Vector2.zero;
                return false;
            }
        }
    }

    public static bool GetIntersectWithScreenEdges(Vector2 inter1, Vector2 inter2, out Vector2 intersection)
    {
        Vector2 topLeftCorner = new Vector2(0,  Screen.height);
        Vector2 topRightCorner = new Vector2(Screen.width, Screen.height);
        Vector2 bottomLeftCorner = new Vector2(0,0);
        Vector2 bottomRightCorner = new Vector2(Screen.width,0);

        if (
                IntersectLineSegments2D(inter1, inter2, topRightCorner, bottomRightCorner, out intersection) ||
                IntersectLineSegments2D(inter1, inter2, topRightCorner, topLeftCorner, out intersection) ||
                IntersectLineSegments2D(inter1, inter2, topLeftCorner, bottomLeftCorner, out intersection) ||
                IntersectLineSegments2D(inter1, inter2, bottomRightCorner, bottomLeftCorner, out intersection)
            )
        {
            return true;
        }
        
        return false;
    }
}