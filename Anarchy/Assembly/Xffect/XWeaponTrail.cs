using Anarchy.Configuration;
using System.Collections.Generic;
using UnityEngine;

namespace Xft
{
    public class XWeaponTrail : MonoBehaviour
    {
        private static float fps;
        private float deltaTime;
        public static float FadeTime;
        public int Granularity = 60;
        public int MaxFrame;
        protected float mElapsedTime;
        protected float mFadeElapsedime;
        protected float mFadeT = 1f;
        protected float mFadeTime = 1f;
        protected Element mHeadElem = new Element();
        protected bool mInited;
        protected bool mIsFading;
        protected GameObject mMeshObj;
        protected List<Element> mSnapshotList = new List<Element>();
        protected Spline mSpline = new Spline();
        protected float mTrailWidth;
        protected VertexPool mVertexPool;
        protected VertexPool.VertexSegment mVertexSegment;
        public Color MyColor = Color.white;
        public Material MyMaterial;
        public Transform PointEnd;
        public Transform PointStart;
        public static string Version = "1.0.1";

        public void Activate()
        {
            FadeTime = VideoSettings.InfiniteTrail.Value ? -1f : 0.3f;
            MaxFrame = Mathf.RoundToInt(VideoSettings.TrailFPS.Value);
            Init();
            if (mMeshObj == null)
            {
                InitMeshObj();
            }
            else
            {
                gameObject.SetActive(true);
                if (mMeshObj != null)
                {
                    mMeshObj.SetActive(true);
                }

                mFadeT = 1f;
                mIsFading = false;
                mFadeTime = 1f;
                mFadeElapsedime = 0f;
                mElapsedTime = 0f;
                for (var i = 0; i < mSnapshotList.Count; i++)
                {
                    mSnapshotList[i].PointStart = PointStart.position;
                    mSnapshotList[i].PointEnd = PointEnd.position;
                    mSpline.ControlPoints[i].Position = mSnapshotList[i].Pos;
                    mSpline.ControlPoints[i].Normal = mSnapshotList[i].PointEnd - mSnapshotList[i].PointStart;
                }

                RefreshSpline();
                UpdateVertex();
            }
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            if (mMeshObj != null)
            {
                mMeshObj.SetActive(false);
            }
        }

        public void Init()
        {
            if (!mInited)
            {
                var vector = PointStart.position - PointEnd.position;
                mTrailWidth = vector.magnitude;
                InitMeshObj();
                InitOriginalElements();
                InitSpline();
                mInited = true;
            }
        }

        private void InitMeshObj()
        {
            mMeshObj = new GameObject("_XWeaponTrailMesh: " + gameObject.name);
            mMeshObj.layer = gameObject.layer;
            mMeshObj.SetActive(true);
            var filter = mMeshObj.AddComponent<MeshFilter>();
            var renderer = mMeshObj.AddComponent<MeshRenderer>();
            renderer.castShadows = false;
            renderer.receiveShadows = false;
            renderer.renderer.sharedMaterial = MyMaterial;
            filter.sharedMesh = new Mesh();
            mVertexPool = new VertexPool(filter.sharedMesh, MyMaterial);
            mVertexSegment = mVertexPool.GetVertices(Granularity * 3, (Granularity - 1) * 12);
            UpdateIndices();
        }

        private void InitOriginalElements()
        {
            mSnapshotList.Clear();
            mSnapshotList.Add(new Element(PointStart.position, PointEnd.position));
            mSnapshotList.Add(new Element(PointStart.position, PointEnd.position));
        }

        private void InitSpline()
        {
            mSpline.Granularity = Granularity;
            mSpline.Clear();
            for (var i = 0; i < MaxFrame; i++)
            {
                mSpline.AddControlPoint(CurHeadPos, PointStart.position - PointEnd.position);
            }
        }

        public void lateUpdate()
        {
            if (mInited)
            {
                mVertexPool.LateUpdate();
            }
        }

        private void OnDrawGizmos()
        {
            if (PointEnd != null && PointStart != null)
            {
                var vector = PointStart.position - PointEnd.position;
                var magnitude = vector.magnitude;
                if (magnitude >= float.Epsilon)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(PointStart.position, magnitude * 0.04f);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(PointEnd.position, magnitude * 0.04f);
                }
            }
        }

        private void RecordCurElem()
        {
            var item = new Element(PointStart.position, PointEnd.position);
            if (mSnapshotList.Count < MaxFrame)
            {
                mSnapshotList.Insert(1, item);
            }
            else
            {
                mSnapshotList.RemoveAt(mSnapshotList.Count - 1);
                mSnapshotList.Insert(1, item);
            }
        }

        private void RefreshSpline()
        {
            for (var i = 0; i < mSnapshotList.Count; i++)
            {
                mSpline.ControlPoints[i].Position = mSnapshotList[i].Pos;
                mSpline.ControlPoints[i].Normal = mSnapshotList[i].PointEnd - mSnapshotList[i].PointStart;
            }

            mSpline.RefreshSpline();
        }

        private void Start()
        {
            Init();
        }

        public void StopSmoothly(float fadeTime)
        {
            mIsFading = true;
            mFadeTime = FadeTime;
        }

        public void update()
        {
            if (mInited)
            {
                if (mMeshObj == null)
                {
                    InitMeshObj();
                }
                else
                {
                    UpdateHeadElem();
                    mElapsedTime += Time.deltaTime;
                    if (mElapsedTime >= UpdateInterval)
                    {
                        mElapsedTime -= UpdateInterval;
                        RecordCurElem();
                        RefreshSpline();
                        UpdateFade();
                        UpdateVertex();
                    }
                }
            }

            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;
        }

        private void UpdateFade()
        {
            if (mIsFading)
            {
                mFadeElapsedime += Time.deltaTime;
                var num = mFadeElapsedime / mFadeTime;
                mFadeT = 1f - num;
                if (mFadeT < 0f)
                {
                    Deactivate();
                }
            }
        }

        private void UpdateHeadElem()
        {
            mSnapshotList[0].PointStart = PointStart.position;
            mSnapshotList[0].PointEnd = PointEnd.position;
        }

        private void UpdateIndices()
        {
            var pool = mVertexSegment.Pool;
            for (var i = 0; i < Granularity - 1; i++)
            {
                var num2 = mVertexSegment.VertStart + i * 3;
                var num3 = mVertexSegment.VertStart + (i + 1) * 3;
                var index = mVertexSegment.IndexStart + i * 12;
                pool.Indices[index] = num3;
                pool.Indices[index + 1] = num3 + 1;
                pool.Indices[index + 2] = num2;
                pool.Indices[index + 3] = num3 + 1;
                pool.Indices[index + 4] = num2 + 1;
                pool.Indices[index + 5] = num2;
                pool.Indices[index + 6] = num3 + 1;
                pool.Indices[index + 7] = num3 + 2;
                pool.Indices[index + 8] = num2 + 1;
                pool.Indices[index + 9] = num3 + 2;
                pool.Indices[index + 10] = num2 + 2;
                pool.Indices[index + 11] = num2 + 1;
            }

            pool.IndiceChanged = true;
        }

        private void UpdateVertex()
        {
            var pool = mVertexSegment.Pool;
            for (var i = 0; i < Granularity; i++)
            {
                var index = mVertexSegment.VertStart + i * 3;
                var num3 = i / (float)Granularity;
                var tl = num3 * mFadeT;
                var zero = Vector2.zero;
                var vector2 = mSpline.InterpolateByLen(tl);
                var vector3 = mSpline.InterpolateNormalByLen(tl);
                var vector4 = vector2 + vector3.normalized * mTrailWidth * 0.5f;
                var vector5 = vector2 - vector3.normalized * mTrailWidth * 0.5f;
                pool.Vertices[index] = vector4;
                pool.Colors[index] = MyColor;
                zero.x = 0f;
                zero.y = num3;
                pool.UVs[index] = zero;
                pool.Vertices[index + 1] = vector2;
                pool.Colors[index + 1] = MyColor;
                zero.x = 0.5f;
                zero.y = num3;
                pool.UVs[index + 1] = zero;
                pool.Vertices[index + 2] = vector5;
                pool.Colors[index + 2] = MyColor;
                zero.x = 1f;
                zero.y = num3;
                pool.UVs[index + 2] = zero;
            }

            mVertexSegment.Pool.UVChanged = true;
            mVertexSegment.Pool.VertChanged = true;
            mVertexSegment.Pool.ColorChanged = true;
        }

        public Vector3 CurHeadPos
        {
            get { return (PointStart.position + PointEnd.position) / 2f; }
        }

        public float TrailWidth
        {
            get { return mTrailWidth; }
        }

        public float UpdateInterval
        {
            get { return 1f / fps; }
        }

        public class Element
        {
            public Vector3 PointEnd;
            public Vector3 PointStart;

            public Element()
            {
            }

            public Element(Vector3 start, Vector3 end)
            {
                PointStart = start;
                PointEnd = end;
            }

            public Vector3 Pos
            {
                get { return (PointStart + PointEnd) / 2f; }
            }
        }
    }
}