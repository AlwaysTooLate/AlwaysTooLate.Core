// AlwaysTooLate.Console (c) 2018-2019 Always Too Late.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AlwaysTooLate.Core
{
    /// <summary>
    ///     Simple GameObjectPool for specialized usage.
    ///     It is specialized, because it's GameObjects have pre-added components and should not get new one,
    ///     so it can be used in a special cases.
    ///     Mainly built to be used with Console, as it needs a lot of GameObjects being used as Text lines.
    /// </summary>
    [Obsolete("GameObjectPool is no longer supported. " +
              "Please use new 'AlwaysTooLate.Core.Pooling.GameObjectPool<Transform> or GameObjectPool2'. " +
              "This class will be replaced in the future.")]
    public class GameObjectPool : IDisposable
    {
        private const string GameObjectName = "Pooled GameObject";

        private readonly ConcurrentStack<GameObject> _freeGameObjects;
        private readonly List<GameObject> _gameObjects;
        private readonly Transform _root;
        private readonly Type[] _components;
        private readonly int _dynamicAllocationCount;

        public bool AllowDynamicAllocation = true;

        public GameObjectPool(int numObjects, Type[] components = null, int dynamicAllocationCount = 32)
        {
            Debug.Assert(dynamicAllocationCount > 0, "Dynamic allocation count must be higher than 0 (default: 32)!");

            _root = new GameObject("GameObjectPool Root").transform;
            _root.position = Vector3.zero;
            _root.rotation = Quaternion.identity;

            _components = components;
            _dynamicAllocationCount = dynamicAllocationCount;

            Object.DontDestroyOnLoad(_root);

            _freeGameObjects = new ConcurrentStack<GameObject>();
            _gameObjects = new List<GameObject>();

            // Allocate initial pool
            Allocate(numObjects);
        }

        public void Dispose()
        {
            foreach (var gameObject in _gameObjects) Object.Destroy(gameObject);

            if (_root.gameObject)
                Object.Destroy(_root.gameObject);

            _gameObjects.Clear();
        }

        public GameObject Acquire()
        {
            // Try to acquire game object
            if(_freeGameObjects.TryPop(out var gameObject))
            {
                gameObject.SetActive(true);
                return gameObject;
            }

            if (!AllowDynamicAllocation)
            {
                Debug.LogError("Could not allocate game object! Dynamic allocation is disabled and we're out of objects!");
                return null;
            }

            // No free objects left
            // Allocate new ones if possible
            Allocate(_dynamicAllocationCount);

            // Try to pop. This should work.
            if (_freeGameObjects.TryPop(out gameObject))
            {
                gameObject.SetActive(true);
                return gameObject;
            }

            // We're dead.
            throw new InvalidOperationException("We're out of game objects. Something went terribly wrong!");
        }

        public void Release(GameObject gameObject)
        {
            Debug.Assert(gameObject);
#if DEBUG
            Debug.Assert(_gameObjects.Contains(gameObject), "This game object is not owned by this GameObject pool!");
#endif
            // Reset transform
            var transform = gameObject.transform;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.SetParent(_root);
            gameObject.SetActive(false);

            // Push to the stack
            _freeGameObjects.Push(gameObject);
        }

        private void Allocate(int numObjects)
        {
            for (var i = 0; i < numObjects; i++)
            {
                var gameObject = new GameObject(GameObjectName);

                _gameObjects.Add(gameObject);
                _freeGameObjects.Push(gameObject);

                if (_components != null)
                    foreach (var component in _components)
                        gameObject.AddComponent(component);

                gameObject.transform.SetParent(_root);
                gameObject.SetActive(false);
            }
        }
    }
}