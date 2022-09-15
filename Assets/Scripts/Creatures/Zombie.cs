using UnityEngine;

namespace Assets.Scripts
{
    public class Zombie : Movable
    {
        private const string AnimGoBool = "go";
        private const string AnimDieBool = "die";
        private const string AnimAttackBool = "attack";

        protected override void OnStart()
        {
            base.OnStart();

            var dirRandom = Random.Range(0, 2);
            moveDir = dirRandom == 0 ? Vector3.right : Vector3.left;
        }

        protected override void OnObjDisable()
        {
            base.OnObjDisable();

            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimAttackBool, false);
            animator.SetBool(AnimGoBool, false);
        }

        protected override void OnObjEnable()
        {
            base.OnObjEnable();
            
            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimAttackBool, false);
            animator.SetBool(AnimGoBool, true);
        }
    }
}