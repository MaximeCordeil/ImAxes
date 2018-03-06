using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Curver
{

    public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness)
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (smoothness < 1.0f) smoothness = 1.0f;

        pointsLength = arrayToCurve.Length;

        curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

            points = new List<Vector3>(arrayToCurve);

            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }

            curvedPoints.Add(points[0]);
        }

        return (curvedPoints.ToArray());
    }

    List<Vector3> CatmulRom(float amountOfPoints, float alpha)
    {
        List<Vector3> newPoints = new List<Vector3>();

        //Vector2 p0 = new Vector2(points[0].transform.position.x, points[0].transform.position.y);
        //Vector2 p1 = new Vector2(points[1].transform.position.x, points[1].transform.position.y);
        //Vector2 p2 = new Vector2(points[2].transform.position.x, points[2].transform.position.y);
        //Vector2 p3 = new Vector2(points[3].transform.position.x, points[3].transform.position.y);

        //float t0 = 0.0f;
        //float t1 = GetT(t0, p0, p1, alpha);
        //float t2 = GetT(t1, p1, p2, alpha);
        //float t3 = GetT(t2, p2, p3, alpha);

        //for (float t = t1; t < t2; t += ((t2 - t1) / amountOfPoints))
        //{
        //    Vector2 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
        //    Vector2 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
        //    Vector2 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

        //    Vector2 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
        //    Vector2 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

        //    Vector2 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

        //    newPoints.Add(C);
        //}
        return newPoints;
    }

    float GetT(float t, Vector2 p0, Vector2 p1, float alpha)
    {
        float a = Mathf.Pow((p1.x - p0.x), 2.0f) + Mathf.Pow((p1.y - p0.y), 2.0f);
        float b = Mathf.Pow(a, 0.5f);
        float c = Mathf.Pow(b, alpha);

        return (c + t);
    }

}