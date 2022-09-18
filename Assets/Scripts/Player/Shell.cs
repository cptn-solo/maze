using UnityEngine;

namespace Assets.Scripts
{
    public class Shell : MonoBehaviour
    {
        [SerializeField] private float speed = 1.0f;
        private Damage damage;

        private void Awake()
        {
            damage = GetComponent<Damage>();
        }

        private void OnEnable()
        {
            damage.Active = true;
        }
        private void OnDisable()
        {
            damage.Active = false;
        }

        private void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

    }
}