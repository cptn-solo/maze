using UnityEngine;

namespace Assets.Scripts
{
    public class Zombie : Movable
    {
        private const string AnimGoBool = "go";

        protected override void OnStart()
        {
            base.OnStart();

            var dirRandom = Random.Range(0, 2);
            moveDir = dirRandom == 0 ? Vector3.right : Vector3.left;

            animator.SetBool(AnimGoBool, true);
        }

    }
}