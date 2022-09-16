using UnityEngine;

namespace Ouroboros
{
    /// <summary>
    /// Rotates the GameObject by AngularSpeed in Degress per second.
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 angularSpeed;

        private void Update()
        {
            transform.Rotate(angularSpeed * Time.deltaTime);
        }
    }
}