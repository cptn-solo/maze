using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Zombie : MovableUnit
    {
        private const string AnimGoBool = "go";
        private const string AnimDieBool = "die";
        private const string AnimAttackBool = "attack";
        private const string AnimDamageBool = "damage";
        
        private Damage damage;

        protected override void OnAwake()
        {
            base.OnAwake();
            damage = GetComponentInChildren<Damage>();
        }
        protected override void OnTakingDamage(bool critical)
        {
            base.OnTakingDamage(critical);

            StartCoroutine(DamageAnimation());
        }

        private IEnumerator DamageAnimation()
        {
            animator.SetBool(AnimDamageBool, true);

            yield return new WaitForSeconds(.3f);

            animator.SetBool(AnimDamageBool, false);

        }

        protected override void OnGotKilled()
        {
            base.OnGotKilled();

            damage.Active = false;

            animator.SetBool(AnimDieBool, true);
            animator.SetBool(AnimGoBool, false);
            animator.SetBool(AnimAttackBool, false);
        }

        protected override void OnResurrected()
        {
            base.OnResurrected();
            
            var dirRandom = Random.Range(0, 2);
            moveDir = dirRandom == 0 ? Vector3.right : Vector3.left;

            damage.Active = true;

            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimGoBool, true);
            animator.SetBool(AnimAttackBool, false);
        }

        protected override void OnObjDisable()
        {
            base.OnObjDisable();

            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimGoBool, false);
            animator.SetBool(AnimAttackBool, false);
        }

        protected override void OnObjEnable()
        {
            base.OnObjEnable();
            
            animator.SetBool(AnimDieBool, false);
            animator.SetBool(AnimGoBool, true);
            animator.SetBool(AnimAttackBool, false);
        }
    }
}