using UnityEngine;
using ECellDive.Interfaces;

namespace ECellDive.Utility
{
    /// <summary>
    /// A simple component to always call <see cref="ILookAt.LookAt"/>
    /// on update.
    /// </summary>
    [RequireComponent(typeof(ILookAt))]
    public class AlwaysLookAt : MonoBehaviour
    {
        private ILookAt refLookAtComponent;
        // Start is called before the first frame update
        void Start()
        {
            refLookAtComponent = GetComponent<ILookAt>();
        }

        // Update is called once per frame
        void Update()
        {
            refLookAtComponent.LookAt();
        }
    }
}

