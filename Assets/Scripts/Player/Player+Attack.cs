
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public partial class Player
    {
        private bool attackRunning;
        private bool weaponSelectRunning;
        private bool item1SelectRunning;
        private bool item2SelectRunning;

        private WeaponType currentWeapon = WeaponType.Shuriken;
        private WeaponType stowedWeapon = WeaponType.Minigun;

        private void OnWeaponSelect()
        {
            if (!weaponSelectRunning)
                StartCoroutine(ToggleWeapon());
        }

        private void OnItem1Select()
        {
            if (!item1SelectRunning)
                StartCoroutine(UseItem1());
        }

        private void OnItem2Select()
        {
            if (!item2SelectRunning)
                StartCoroutine(UseItem2());
        }

        private IEnumerator UseItem1()
        {
            item1SelectRunning = true;
            yield return new WaitForSeconds(.3f);
            item1SelectRunning = true;
        }

        private IEnumerator UseItem2()
        {
            item2SelectRunning = true;
            yield return new WaitForSeconds(.3f);
            item2SelectRunning = true;
        }

        private IEnumerator ToggleWeapon()
        {
            weaponSelectRunning = true;

            (stowedWeapon, currentWeapon) = (currentWeapon, stowedWeapon);

            OnWeaponSelected?.Invoke(currentWeapon);

            minigun.gameObject.SetActive(currentWeapon == WeaponType.Minigun);
            animator.SetBool(AnimMinigunBool, currentWeapon == WeaponType.Minigun);

            yield return new WaitForSeconds(.3f);

            weaponSelectRunning = false;
        }

        private void OnAttack(bool toggle)
        {
            Debug.Log($"OnAttack {toggle}");
            inAttackState = toggle;
            if (inAttackState && !attackRunning)
                StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            attackRunning = true;
            var ammo = 0;
            var ammoCollectable = PlayerBalanceService.CollectableForWeapon(currentWeapon);
            if (ammoCollectable != CollectableType.NA)
            {
                if (Balances.CurrentBalance(ammoCollectable) is int beginBalance &&
                    beginBalance > 0)
                {
                    ammo = beginBalance;
                    aim.Engage(true);
                }
                else
                {
                    SoundEvents.OutOfAmmo();
                    yield return new WaitForSeconds(.1f);
                    attackRunning = false;
                }
            }
            else
            {
                aim.Engage(true);
            }

            while (attackRunning && inAttackState && !fadingOut)
            {

                aim.TryGetAttackTarget(true);

                switch (currentWeapon)
                {
                    case WeaponType.Minigun:
                        {
                            if (ammo <= 0)
                            {
                                minigun.Attack(false);
                                SoundEvents.OutOfAmmo();

                                break;
                            }

                            minigun.Attack(true);
                            SoundEvents.MinigunShot();
                            ammo--;
                            OnActiveWeaponAttack?.Invoke(currentWeapon, ammo);

                            break;
                        }
                    default:
                        {
                            animator.SetBool(AnimAttackBool, true);

                            shell.transform.SetParent(null, true);
                            shell.TargetDir = aim.AttackTarget != null ?
                                (aim.AttackTarget.transform.position - transform.position).normalized :
                                transform.forward;
                            shell.gameObject.SetActive(true);

                            SoundEvents.PlayerAttack();

                            yield return new WaitForSeconds(.3f);
                            
                            animator.SetBool(AnimAttackBool, false);

                            shell.gameObject.SetActive(false);
                            shell.transform.SetParent(launcher, false);
                            shell.transform.localPosition = Vector3.zero;
                            shell.transform.localRotation = Quaternion.identity;

                            break;
                        }
                }

                yield return new WaitForSeconds(.1f);
            }

            aim.Engage(false);

            switch (currentWeapon)
            {
                case WeaponType.Minigun:
                    {
                        Balances.SetBalance(ammoCollectable, ammo);
                        minigun.Attack(false);
                        break;
                    }
                default:
                    {
                        animator.SetBool(AnimAttackBool, false);
                        break;

                    }
                    
            }

            attackRunning = false;
        }

    }
}