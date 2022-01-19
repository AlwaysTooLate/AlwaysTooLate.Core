// AlwaysTooLate.Core (c) 2018-2022 Always Too Late. All rights reserved.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace AlwaysTooLate.Core.Pooling
{
    using Object = UnityEngine.Object;

    /// <summary>
    ///     GameObject pool with handle.
    /// </summary>
    /// <typeparam name="THandleComponent">The handle, has to inherit from Component (MonoBehaviour) class.</typeparam>
    public class GameObjectPool<THandleComponent> : IDisposable 
        where THandleComponent : Component
    {
        private readonly Transform _root;
        private readonly GameObject _prefab;
        private readonly bool _enableDynamicAllocation;
        private readonly int _poolDynamicAllocationSize;

        private readonly Stack<THandleComponent> _freeGameObjects;
        private readonly List<THandleComponent> _gameObjectHandles;

        private bool _disposed = false;

        /// <summary>
        ///     Unsafe access to the internal game object handles list.
        ///     Use with caution, this is not thread safe.
        /// </summary>
        public IReadOnlyList<THandleComponent> UnsafeObjectHandles => _gameObjectHandles;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="prefab">
        ///     The prefab that will be used for the pooling.
        ///     Pass null for default GameObject.
        /// </param>
        /// <param name="initialPoolSize">The initial pool size.</param>
        /// <param name="enableDynamicAllocation">Enables or disabled dynamic allocation.</param>
        /// <param name="poolDynamicAllocationSize">
        ///     When dynamic allocation is enabled, this value will be used,
        ///     to dynamically allocate new <see cref="prefab"/> instances.
        /// </param>
        public GameObjectPool(GameObject prefab = null,
            int initialPoolSize = 512,
            bool enableDynamicAllocation = true,
            int poolDynamicAllocationSize = 32)
        {
            _freeGameObjects = new Stack<THandleComponent>();
            _gameObjectHandles = new List<THandleComponent>();

            if (prefab == null)
            {
                prefab = new GameObject("GameObjectPool-DefaultPrefab");
                prefab.SetActive(false);
            }

            _prefab = prefab;
            _enableDynamicAllocation = enableDynamicAllocation;
            _poolDynamicAllocationSize = poolDynamicAllocationSize;

            // Create our root.
            _root = new GameObject("GameObjectPool Root").transform;
            _root.position = Vector3.zero;
            _root.rotation = Quaternion.identity;

            Object.DontDestroyOnLoad(_root);

            // Allocate initial pool
            Allocate(initialPoolSize); // We don't have to call the lock in ctor... yes?

            // Register for automatic exit
            // This is required, because when we're exiting playmode or the game itself,
            // someone would call Dispose from OnDestroy, which is not ideal and will case errors, sometimes.
            Application.wantsToQuit += OnApplicationExit;
        }

        private bool OnApplicationExit()
        {
            Dispose();
            return true;
        }

        /// <summary>
        ///     Acquires new handle.
        /// </summary>
        public THandleComponent Acquire()
        {
            if (_disposed) return null;

            lock (_gameObjectHandles)
            {
                // Try to pop. This should work.
                THandleComponent handle;
                do
                {
                    // When empty, allocate more game objects
                    if (_freeGameObjects.Count == 0)
                    {
                        Assert.IsTrue(_enableDynamicAllocation, "Dynamic allocation is disabled, and this pool is out of instances!");
                        Allocate(_poolDynamicAllocationSize);
                    }

                    // Pop object from the stack
                    handle = _freeGameObjects.Pop();

                    // Check if it exists, this might happen,
                    // when some idiot destroys pooled game object manually or
                    // doesn't release it when before scene change.
                    if (handle == null)
                    {
                        Debug.LogError("Detected destroyed pooled game object! " +
                                       "You cannot destroy pooled game objects manually!");
                    }
                } while (!handle);

                if (!handle) 
                    throw new Exception("WTF?"); // Sanity check.

                var gameObject = handle.gameObject;
                gameObject.SetActive(true);
                return handle;

            }
        }

        /// <summary>
        ///     Releases game object to the pool.
        /// </summary>
        public void Release(THandleComponent handle)
        {
            if (_disposed) return;

            Assert.IsNotNull(handle);

            // Object has been destroyed
            if (handle.gameObject == null) return;

            lock (_gameObjectHandles)
            {
                Assert.IsTrue(_gameObjectHandles.Contains(handle), "This game object is not owned by this GameObject pool!");

                // Cleanup
                var transform = handle.transform;
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                transform.SetParent(_root);

                // Disable the object, so we don't make some weird bugs in someone's game.
                handle.gameObject.SetActive(false);

                // Push to the stack
                _freeGameObjects.Push(handle);
            }
        }

        /// <summary>
        ///     Disposes the pool. This will destroy all allocated game objects etc.
        ///     Make sure, that you're no longer using any of allocated game objects!
        /// </summary>
        public void Dispose()
        {
            lock (_gameObjectHandles)
            {
                foreach (var handle in _gameObjectHandles)
                {
                    // Destroy only when it's not null
                    // This is common issue, where object lives inside other object, that has been already destroyed.
                    if (handle != null)
                    {
                        Object.Destroy(handle.gameObject);
                    }
                }

                if (_root)
                {
                    Object.Destroy(_root.gameObject);
                }

                // Cleanup
                _freeGameObjects.Clear();
                _gameObjectHandles.Clear();
                _disposed = true;
            }
        }

        /// <summary>
        ///     Unsafe. Please lock _freeGameObjects before calling this.
        /// </summary>
        /// <param name="numObjects"></param>
        private void Allocate(int numObjects)
        {
            for (var i = 0; i < numObjects; i++)
            {
                var instance = Object.Instantiate(_prefab, _root, false);
                instance.SetActive(false);

                // Get handle
                var handle = instance.GetComponent<THandleComponent>();

                _gameObjectHandles.Add(handle);
                _freeGameObjects.Push(handle);
            }
        }
    }

    /// <summary>
    ///     Simple implementation of <see cref="GameObjectPool{THandleComponent}"/>, uses Transform as the handle.
    /// </summary>
    public class GameObjectPool2 : GameObjectPool<Transform>
    {
        public GameObjectPool2(GameObject prefab,
            int initialPoolSize = 512,
            bool enableDynamicAllocation = true,
            int poolDynamicAllocationSize = 32) : base(prefab,
            initialPoolSize,
            enableDynamicAllocation,
            poolDynamicAllocationSize)
        {
        }
    }
}
