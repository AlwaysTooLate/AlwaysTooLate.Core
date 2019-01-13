// AlwaysTooLate.Core (c) 2018-2019 Always Too Late. All rights reserved.

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace AlwaysTooLate.Core
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Smooth LookAt extension function.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="target">The target look at point.</param>
        /// <param name="t">The signed linear interpolation value.</param>
        public static void SmoothLookAt(this Transform transform, Vector3 target, float t)
        {
            var rotation = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, t);
        }

        // source: https://forum.unity.com/threads/change-gameobject-layer-at-run-time-wont-apply-to-child.10091/
        /// <summary>
        /// Sets layer recursively.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer">The layer to apply.</param>
        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        /// <summary>
        /// Snaps this vector by given value.
        /// </summary>
        public static Vector2 Snap(this Vector2 v, float value)
        {
            return new Vector2(v.x - v.x % value, v.y - v.y % value);
        }

        /// <summary>
        /// Snaps this vector by given value.
        /// </summary>
        public static Vector3 Snap(this Vector3 v, float value)
        {
            return new Vector3(v.x - v.x % value, v.y - v.y % value, v.z - v.z % value);
        }

        /// <summary>
        /// Converts Vector3 into Vector2.
        /// </summary>
        /// <returns>The constructed Vector2.</returns>
        public static Vector2 ToVector2(this Vector3 v)
        {
            return v;
        }

        /// <summary>
        /// Converts Vector4 into Vector3.
        /// </summary>
        /// <returns>The constructed Vector3.</returns>
        public static Vector3 ToVector3(this Vector4 v)
        {
            return v;
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
    }

    [Serializable]
    public class UnityEventTransform : UnityEvent<Transform> { }
    
    [Serializable]
    public class UnityEventInt : UnityEvent<int> { }

    [Serializable]
    public class UnityEventFloat : UnityEvent<float> { }

    [Serializable]
    public class UnityEventString : UnityEvent<string> { }
}
