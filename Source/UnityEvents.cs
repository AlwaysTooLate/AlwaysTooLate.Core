// AlwaysTooLate.Core (c) 2018-2022 Always Too Late. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace AlwaysTooLate.Core
{
    [Serializable]
    public class UnityEventTransform : UnityEvent<Transform> { }

    [Serializable]
    public class UnityEventGameObject : UnityEvent<GameObject> { }

    [Serializable]
    public class UnityEventBoolean : UnityEvent<bool> { }

    [Serializable]
    public class UnityEventInt : UnityEvent<int> { }

    [Serializable]
    public class UnityEventFloat : UnityEvent<float> { }

    [Serializable]
    public class UnityEventString : UnityEvent<string> { }
}
