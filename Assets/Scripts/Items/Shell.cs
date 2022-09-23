using UnityEngine;

namespace Assets.Scripts
{
    public class Shell : MonoBehaviour
    {
        [SerializeField] private float speed = 1.0f;
        private Damage damage;

        public LayerMask DamageTo => damage != null ? damage.DamageTo : default;

        public Vector3 TargetDir { get; internal set; }

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
            transform.position += speed * Time.deltaTime * TargetDir;
        }

    }
}