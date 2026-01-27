using NocturneThree.ServiceLocator;
using Unity.Cinemachine;
using UnityEngine;

namespace ColorOrCrash.Features.Camera.Components
{
    [Service]
    public class CameraShakeController : MonoBehaviour, IGameService
    {
        private CinemachineImpulseSource _source;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _source = GetComponent<CinemachineImpulseSource>();
        }

        public void Shake(float force = 1f)
        {
            _source.GenerateImpulseWithVelocity(Vector3.one * force);
        }
    }
}
