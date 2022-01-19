// AlwaysTooLate.Core (c) 2018-2022 Always Too Late. All rights reserved.

using System;
using System.IO;
using UnityEngine;

namespace AlwaysTooLate.Core
{
    public static class UnityExtensions
    {
        /// <summary>
        ///     Smooth LookAt extension function.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="target">The target look at point.</param>
        /// <param name="t">The signed linear interpolation value.</param>
        public static void SmoothLookAt(this Transform transform, Vector3 target, float t)
        {
            var rotation = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, t);
        }

        /// <summary>
        ///     Sets layer recursively.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer">The layer to apply.</param>
        [Obsolete("Please use SetLayers(int) instead.")]
        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            // source: https://forum.unity.com/threads/change-gameobject-layer-at-run-time-wont-apply-to-child.10091/
            obj.layer = layer;

            foreach (Transform child in obj.transform) child.gameObject.SetLayerRecursively(layer);
        }

        /// <summary>
        ///     Sets layer on this game object and all of it's children.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer">The layer to apply.</param>
        public static void SetLayers(this GameObject obj, int layer)
        {
            // source: https://forum.unity.com/threads/change-gameobject-layer-at-run-time-wont-apply-to-child.10091/
            if (obj == null)
                return;

            obj.layer = layer;
            foreach (var trans in obj.GetComponentsInChildren<Transform>(true)) trans.gameObject.layer = layer;
        }

        /// <summary>
        ///     Snaps this vector by given value.
        /// </summary>
        public static Vector2 Snap(this Vector2 v, float value)
        {
            return new Vector2(v.x - v.x % value, v.y - v.y % value);
        }

        /// <summary>
        ///     Snaps this vector by given value.
        /// </summary>
        public static Vector3 Snap(this Vector3 v, float value)
        {
            return new Vector3(v.x - v.x % value, v.y - v.y % value, v.z - v.z % value);
        }

        /// <summary>
        ///     Converts Vector3 into Vector2.
        /// </summary>
        /// <returns>The constructed Vector2.</returns>
        public static Vector2 ToVector2(this Vector3 v)
        {
            return v;
        }

        /// <summary>
        ///     Converts Vector4 into Vector3.
        /// </summary>
        /// <returns>The constructed Vector3.</returns>
        public static Vector3 ToVector3(this Vector4 v)
        {
            return v;
        }

        /// <summary>
        ///     Squared distance between two vectors.
        /// </summary>
        /// <returns>The constructed Vector2.</returns>
        public static float DistanceSqr(this Vector2 left, Vector2 right)
        {
            var x = left.x - right.x;
            var y = left.y - right.y;

            return x * x + y * y;
        }

        /// <summary>
        ///     Squared distance between two vectors.
        /// </summary>
        /// <returns>The constructed Vector2.</returns>
        public static float DistanceSqr(this Vector3 left, Vector3 right)
        {
            var x = left.x - right.x;
            var y = left.y - right.y;
            var z = left.z - right.z;

            return x * x + y * y + z * z;
        }

        public static Guid ReadGUID(this BinaryReader reader)
        {
            return new Guid(reader.ReadString());
        }

        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static Vector4 ReadVector4(this BinaryReader reader)
        {
            return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public static Matrix4x4 ReadMatrix4x4(this BinaryReader reader)
        {
            return new Matrix4x4(reader.ReadVector4(), reader.ReadVector4(), reader.ReadVector4(),
                reader.ReadVector4());
        }

        public static void Write(this BinaryWriter writer, Guid guid)
        {
            writer.Write(guid.ToString("N"));
        }

        public static void Write(this BinaryWriter writer, Vector2 v)
        {
            writer.Write(v.x);
            writer.Write(v.y);
        }

        public static void Write(this BinaryWriter writer, Vector3 v)
        {
            writer.Write(v.x);
            writer.Write(v.y);
            writer.Write(v.z);
        }

        public static void Write(this BinaryWriter writer, Vector4 v)
        {
            writer.Write(v.x);
            writer.Write(v.y);
            writer.Write(v.z);
            writer.Write(v.w);
        }

        public static void Write(this BinaryWriter writer, Matrix4x4 v)
        {
            writer.Write(v.GetColumn(0));
            writer.Write(v.GetColumn(1));
            writer.Write(v.GetColumn(2));
            writer.Write(v.GetColumn(3));
        }

        public static Matrix4x4 Lerp(this Matrix4x4 from, Matrix4x4 to, float t)
        {
            return new Matrix4x4
            {
                m00 = Mathf.Lerp(from.m00, to.m00, t),
                m01 = Mathf.Lerp(from.m01, to.m01, t),
                m02 = Mathf.Lerp(from.m02, to.m02, t),
                m03 = Mathf.Lerp(from.m03, to.m03, t),
                m10 = Mathf.Lerp(from.m10, to.m10, t),
                m11 = Mathf.Lerp(from.m11, to.m11, t),
                m12 = Mathf.Lerp(from.m12, to.m12, t),
                m13 = Mathf.Lerp(from.m13, to.m13, t),
                m20 = Mathf.Lerp(from.m20, to.m20, t),
                m21 = Mathf.Lerp(from.m21, to.m21, t),
                m22 = Mathf.Lerp(from.m22, to.m22, t),
                m23 = Mathf.Lerp(from.m23, to.m23, t),
                m30 = Mathf.Lerp(from.m30, to.m30, t),
                m31 = Mathf.Lerp(from.m31, to.m31, t),
                m32 = Mathf.Lerp(from.m32, to.m32, t),
                m33 = Mathf.Lerp(from.m33, to.m33, t)
            };
        }
    }
}