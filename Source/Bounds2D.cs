// AlwaysTooLate.Core (c) 2018-2022 Always Too Late. All rights reserved.

using UnityEngine;

namespace AlwaysTooLate.Core
{
    /// <summary>
    ///     2D bounds structure.
    /// </summary>
    public readonly struct Bounds2D
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public Bounds2D(Vector2 center, Vector2 size)
        {
            Center = center;
            Size = size;
        }

        /// <summary>
        ///     Returns true if this bounds contain given point.
        /// </summary>
        /// <param name="point">The point</param>
        /// <returns>True if contains given point.</returns>
        public bool Contains(Vector2 point)
        {
            var min = Min;
            var max = Max;

            return point.x >= min.x && point.x <= max.x && point.y >= min.y && point.y <= max.y;
        }

        /// <summary>
        ///     Returns true if given bounding box intersects with this bounds.
        /// </summary>
        /// <param name="bounds">The bounds</param>
        /// <returns>True if given bounding box intersects with this</returns>
        public bool Intersects(Bounds2D bounds)
        {
            return Mathf.Abs(Center.x - bounds.Center.x) * 2 < Size.x + bounds.Size.x &&
                   Mathf.Abs(Center.y - bounds.Center.y) * 2 < Size.y + bounds.Size.y;
        }


        /// <summary>
        ///     Returns true if given ray intersects with this bounds.
        /// </summary>
        /// <param name="ray">The ray</param>
        /// <param name="distance">The resulting distance</param>
        /// <returns>True if given ray intersects with this</returns>
        public bool Intersects(Ray2D ray, out float distance)
        {
            distance = 0f;
            var tmax = float.MaxValue;

            if (Mathf.Approximately(ray.direction.x, 0.0f))
            {
                if (ray.origin.x < Min.x || ray.origin.x > Max.x)
                {
                    distance = 0f;
                    return false;
                }
            }
            else
            {
                var inverse = 1.0f / ray.direction.x;
                var t1 = (Min.x - ray.origin.x) * inverse;
                var t2 = (Max.x - ray.origin.x) * inverse;

                if (t1 > t2)
                {
                    var temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                distance = Mathf.Max(t1, distance);
                tmax = Mathf.Min(t2, tmax);

                if (distance > tmax)
                {
                    distance = 0f;
                    return false;
                }
            }

            if (Mathf.Approximately(ray.direction.y, 0.0f))
            {
                if (ray.origin.y < Min.y || ray.origin.y > Max.y)
                {
                    distance = 0f;
                    return false;
                }
            }
            else
            {
                var inverse = 1.0f / ray.direction.y;
                var t1 = (Min.y - ray.origin.y) * inverse;
                var t2 = (Max.y - ray.origin.y) * inverse;

                if (t1 > t2)
                {
                    var temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                distance = Mathf.Max(t1, distance);
                tmax = Mathf.Min(t2, tmax);

                if (distance > tmax)
                {
                    distance = 0f;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Returns the distance between given point and the nearest point on the bound's edge.
        /// </summary>
        /// <param name="point">The point to check the distance.</param>
        /// <returns>The calculated distance.</returns>
        public float DistanceToEdge(Vector2 point)
        {
            var dx = Mathf.Max(Min.x - point.x, 0, point.x - Max.x);
            var dy = Mathf.Max(Min.y - point.y, 0, point.y - Max.y);
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static implicit operator Bounds2D(Bounds bounds3D)
        {
            var size = new Vector2(bounds3D.size.x, bounds3D.size.y);
            return new Bounds2D(bounds3D.center, size);
        }

        /// <summary>
        ///     The center point of this bounds.
        /// </summary>
        public Vector2 Center { get; }

        /// <summary>
        ///     The size of this bounds.
        /// </summary>
        public Vector2 Size { get; }

        /// <summary>
        ///     The min point of this bounds.
        /// </summary>
        public Vector2 Min => Center - Extents;

        /// <summary>
        ///     The max point of this bounds.
        /// </summary>
        public Vector2 Max => Center + Extents;

        /// <summary>
        ///     The extents of this bounds.
        /// </summary>
        public Vector2 Extents => Size * 0.5f;

        /// <summary>
        ///     Top Left point of this bounds.
        /// </summary>
        public Vector2 TopLeft => Center + new Vector2(-Extents.x, Extents.y);

        /// <summary>
        ///     Top Left point of this bounds
        /// </summary>
        public Vector2 BottomRight => Center + new Vector2(Extents.x, -Extents.y);

        /// <summary>
        ///     Bounds with size of 0.
        /// </summary>
        public static Bounds2D Zero => new Bounds2D(Vector2.zero, Vector2.zero);

        /// <summary>
        ///     Bounds with size of 1.
        /// </summary>
        public static Bounds2D One => new Bounds2D(Vector2.zero, Vector2.one);
    }
}