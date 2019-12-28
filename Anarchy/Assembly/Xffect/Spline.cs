using System.Collections.Generic;
using UnityEngine;

namespace Xft
{
    public class Spline
    {
        private List<SplineControlPoint> mControlPoints = new List<SplineControlPoint>();
        private List<SplineControlPoint> mSegments = new List<SplineControlPoint>();
        public int Granularity = 20;

        public List<SplineControlPoint> ControlPoints
        {
            get
            {
                return this.mControlPoints;
            }
        }

        public List<SplineControlPoint> Segments
        {
            get
            {
                return this.mSegments;
            }
        }

        public SplineControlPoint this[int index]
        {
            get
            {
                if (index > -1 && index < this.mSegments.Count)
                {
                    return this.mSegments[index];
                }
                return null;
            }
        }

        private void RefreshDistance()
        {
            if (this.mSegments.Count < 1)
            {
                return;
            }
            this.mSegments[0].Dist = 0f;
            for (int i = 1; i < this.mSegments.Count; i++)
            {
                float magnitude = (this.mSegments[i].Position - this.mSegments[i - 1].Position).magnitude;
                this.mSegments[i].Dist = this.mSegments[i - 1].Dist + magnitude;
            }
        }

        public static Vector3 CatmulRom(Vector3 T0, Vector3 P0, Vector3 P1, Vector3 T1, float f)
        {
            double num = -0.5;
            double num2 = 1.5;
            double num3 = -1.5;
            double num4 = 0.5;
            double num5 = -2.5;
            double num6 = 2.0;
            double num7 = -0.5;
            double num8 = -0.5;
            double num9 = 0.5;
            double num10 = num * (double)T0.x + num2 * (double)P0.x + num3 * (double)P1.x + num4 * (double)T1.x;
            double num11 = (double)T0.x + num5 * (double)P0.x + num6 * (double)P1.x + num7 * (double)T1.x;
            double num12 = num8 * (double)T0.x + num9 * (double)P1.x;
            double num13 = (double)P0.x;
            double num14 = num * (double)T0.y + num2 * (double)P0.y + num3 * (double)P1.y + num4 * (double)T1.y;
            double num15 = (double)T0.y + num5 * (double)P0.y + num6 * (double)P1.y + num7 * (double)T1.y;
            double num16 = num8 * (double)T0.y + num9 * (double)P1.y;
            double num17 = (double)P0.y;
            double num18 = num * (double)T0.z + num2 * (double)P0.z + num3 * (double)P1.z + num4 * (double)T1.z;
            double num19 = (double)T0.z + num5 * (double)P0.z + num6 * (double)P1.z + num7 * (double)T1.z;
            double num20 = num8 * (double)T0.z + num9 * (double)P1.z;
            double num21 = (double)P0.z;
            float x = (float)(((num10 * (double)f + num11) * (double)f + num12) * (double)f + num13);
            float y = (float)(((num14 * (double)f + num15) * (double)f + num16) * (double)f + num17);
            float z = (float)(((num18 * (double)f + num19) * (double)f + num20) * (double)f + num21);
            return new Vector3(x, y, z);
        }

        public SplineControlPoint AddControlPoint(Vector3 pos, Vector3 up)
        {
            SplineControlPoint splineControlPoint = new SplineControlPoint();
            splineControlPoint.Init(this);
            splineControlPoint.Position = pos;
            splineControlPoint.Normal = up;
            this.mControlPoints.Add(splineControlPoint);
            splineControlPoint.ControlPointIndex = this.mControlPoints.Count - 1;
            return splineControlPoint;
        }

        public void Clear()
        {
            this.mControlPoints.Clear();
        }

        public Vector3 InterpolateByLen(float tl)
        {
            float localF;
            SplineControlPoint splineControlPoint = this.LenToSegment(tl, out localF);
            return splineControlPoint.Interpolate(localF);
        }

        public Vector3 InterpolateNormalByLen(float tl)
        {
            float localF;
            SplineControlPoint splineControlPoint = this.LenToSegment(tl, out localF);
            return splineControlPoint.InterpolateNormal(localF);
        }

        public SplineControlPoint LenToSegment(float t, out float localF)
        {
            SplineControlPoint splineControlPoint = null;
            t = Mathf.Clamp01(t);
            float num = t * this.mSegments[this.mSegments.Count - 1].Dist;
            int i;
            for (i = 0; i < this.mSegments.Count; i++)
            {
                if (this.mSegments[i].Dist >= num)
                {
                    splineControlPoint = this.mSegments[i];
                    break;
                }
            }
            if (i == 0)
            {
                localF = 0f;
                return splineControlPoint;
            }
            int index = splineControlPoint.SegmentIndex - 1;
            SplineControlPoint splineControlPoint2 = this.mSegments[index];
            float num2 = splineControlPoint.Dist - splineControlPoint2.Dist;
            localF = (num - splineControlPoint2.Dist) / num2;
            return splineControlPoint2;
        }

        public SplineControlPoint NextControlPoint(SplineControlPoint controlpoint)
        {
            if (this.mControlPoints.Count == 0)
            {
                return null;
            }
            int num = controlpoint.ControlPointIndex + 1;
            if (num >= this.mControlPoints.Count)
            {
                return null;
            }
            return this.mControlPoints[num];
        }

        public Vector3 NextNormal(SplineControlPoint controlpoint)
        {
            SplineControlPoint splineControlPoint = this.NextControlPoint(controlpoint);
            if (splineControlPoint != null)
            {
                return splineControlPoint.Normal;
            }
            return controlpoint.Normal;
        }

        public Vector3 NextPosition(SplineControlPoint controlpoint)
        {
            SplineControlPoint splineControlPoint = this.NextControlPoint(controlpoint);
            if (splineControlPoint != null)
            {
                return splineControlPoint.Position;
            }
            return controlpoint.Position;
        }

        public SplineControlPoint PreviousControlPoint(SplineControlPoint controlpoint)
        {
            if (this.mControlPoints.Count == 0)
            {
                return null;
            }
            int num = controlpoint.ControlPointIndex - 1;
            if (num < 0)
            {
                return null;
            }
            return this.mControlPoints[num];
        }

        public Vector3 PreviousNormal(SplineControlPoint controlpoint)
        {
            SplineControlPoint splineControlPoint = this.PreviousControlPoint(controlpoint);
            if (splineControlPoint != null)
            {
                return splineControlPoint.Normal;
            }
            return controlpoint.Normal;
        }

        public Vector3 PreviousPosition(SplineControlPoint controlpoint)
        {
            SplineControlPoint splineControlPoint = this.PreviousControlPoint(controlpoint);
            if (splineControlPoint != null)
            {
                return splineControlPoint.Position;
            }
            return controlpoint.Position;
        }

        public void RefreshSpline()
        {
            this.mSegments.Clear();
            for (int i = 0; i < this.mControlPoints.Count; i++)
            {
                if (this.mControlPoints[i].IsValid)
                {
                    this.mSegments.Add(this.mControlPoints[i]);
                    this.mControlPoints[i].SegmentIndex = this.mSegments.Count - 1;
                }
            }
            this.RefreshDistance();
        }
    }
}