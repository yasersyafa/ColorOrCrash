using UnityEngine;

namespace ColorOrCrash.Vin.VFX
{
    public class BallCollectionEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem collectionParticles;

        public void PlayEffect(Vector3 position, Color color)
        {
            if (collectionParticles != null)
            {
                collectionParticles.transform.position = position;

                var main = collectionParticles.main;
                main.startColor = color;

                collectionParticles.Play();
            }
        }
    }
}
