using UnityEngine;

namespace Gazeus.DesafioMatch3.Project.Script.Utils
{
    public class ParticleSystemDestroyer : MonoBehaviour
    {
        // This function can be called from an event to destroy the GameObject
        public void DestroyGameObject()
        {
            Destroy(gameObject);
        }
    }
}