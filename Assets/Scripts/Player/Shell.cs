using UnityEngine;

namespace Assets.Scripts
{
    public class Shell : MonoBehaviour
    {
        [SerializeField] private float speed = 1.0f;

        private void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

    }
}