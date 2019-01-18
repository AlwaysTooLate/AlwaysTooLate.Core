// AlwaysTooLate.Console (c) 2018-2019 Always Too Late.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlwaysTooLate.Core
{
    using Object = UnityEngine.Object;

    /// <summary>
    /// Simple GameObjectPool for specialized usage.
    /// It is specialized, because it's GameObjects have pre-added components and should not get new one,
    /// so it can be used in a special cases.
    /// Mainly built to be used with Console, as it needs a lot of GameObjects being used as Text lines.
    /// </summary>
    public class GameObjectPool : IDisposable
    {
        private const string GameObjectName = "Pooled GameObject";

        private readonly Transform _root;
        private readonly List<GameObject> _gameObjects;
        private readonly Stack<GameObject> _freeGameObjects;

        public GameObjectPool(int numObjects, Type[] components = null)
        {
            _root = new GameObject("GameObjectPool Root").transform;
            _root.position = Vector3.zero;
            _root.rotation = Quaternion.identity;

            Object.DontDestroyOnLoad(_root);
            
            _freeGameObjects = new Stack<GameObject>();
            _gameObjects = new List<GameObject>();

            for (var i = 0; i < numObjects; i++)
            {
                var gameObject = new GameObject(GameObjectName);

                _gameObjects.Add(gameObject);
                _freeGameObjects.Push(gameObject);

                if (components != null)
                {
                    foreach (var component in components)
                        gameObject.AddComponent(component);
                }

                gameObject.transform.SetParent(_root);
                gameObject.SetActive(false);
            }
        }
        
        public void Dispose()
        {
            foreach (var gameObject in _gameObjects)
            {
                Object.Destroy(gameObject);
            }
            Object.Destroy(_root);
            _gameObjects.Clear();
        }

        public GameObject Acquire()
        {
            lock (_freeGameObjects)
            {
                Debug.Assert(_freeGameObjects.Count > 0, "The GameObject Pool is empty!");

                var gameObject = _freeGameObjects.Pop();
                gameObject.SetActive(true);
                return gameObject;
            }
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

            // Lock and push to the stack
            lock (_freeGameObjects)
            {
                _freeGameObjects.Push(gameObject);
            }
        }
    }
}
