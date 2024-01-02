using Base.Core.Managers;
using UnityEngine;

namespace Base.Core.Components
{
    public class MyMonoBehaviour : MonoBehaviour
    {
        public GameManager GameManager => GameManager.Instance;
    }
}
