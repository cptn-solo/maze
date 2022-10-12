
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    public partial class Player
    {
        private bool attackRunning;
        private bool weaponSelectRunning;
        private bool item1SelectRunning;
        private bool item2SelectRunning;

        private WeaponType currentWeapon = WeaponType.Shuriken;
        private WeaponType stowedWeapon = WeaponType.NA;
        
        [SerializeField] private GameObject bombPrefab;
        [SerializeField] private GameObject landminePrefab;

        public int PerkRateOfFire { get; private set; } = 1;

        public void SelectWeapon(WeaponType weapon)
        {
            if (weapon != currentWeapon)
            {
                stowedWeapon = weapon;
                OnWeaponSelect();
            }
        }
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

            if (Balances.CurrentBalance(CollectableType.Bomb) is int ammo &&
                ammo > 0)
            {
                ammo--;
                Balances.SetBalance(CollectableType.Bomb, ammo);
                var bomb = Instantiate(bombPrefab,
                    transform.position + transform.up * .05f,
                    Quaternion.LookRotation(transform.forward, transform.up)).GetComponent<Bomb>();

                bomb.SoundEvents = SoundEvents;
            }
            else
            {
                SoundEvents.OutOfAmmo();
            }

            yield return new WaitForSeconds(.3f);
            item1SelectRunning = false;
        }

        private IEnumerator UseItem2()
        {
            item2SelectRunning = true;
            if (Balances.CurrentBalance(CollectableType.Landmine) is int ammo &&
                ammo > 0)
            {
                ammo--;
                Balances.SetBalance(CollectableType.Landmine, ammo);
                var landmine = Instantiate(landminePrefab,
                    transform.position + transform.up * .05f,
                    Quaternion.LookRotation(transform.forward, transform.up)).GetComponent<Landmine>();

                landmine.OwnerPlayerId = hitbox.PlayerId;
                landmine.SoundEvents = SoundEvents;

            }
            else
            {
                SoundEvents.OutOfAmmo();
            }

            yield return new WaitForSeconds(.3f);
            item2SelectRunning = false;
        }

        private IEnumerator ToggleWeapon()
        {
            weaponSelectRunning = true;

            (stowedWeapon, currentWeapon) = (currentWeapon, stowedWeapon);

            OnWeaponSelected?.Invoke(currentWeapon);

            var perkLevel = Perks.CurrentPerk(currentWeapon);
            PerkRateOfFire = PerkROF(currentWeapon, perkLevel);

            minigun.gameObject.SetActive(currentWeapon == WeaponType.Minigun);
            shotgun.gameObject.SetActive(currentWeapon == WeaponType.Shotgun);
            uzi.gameObject.SetActive(currentWeapon == WeaponType.Uzi);

            animator.SetBool(AnimMinigunBool, currentWeapon == WeaponType.Minigun);
            animator.SetBool(AnimShotgunBool, currentWeapon == WeaponType.Shotgun);
            animator.SetBool(AnimUziBool, currentWeapon == WeaponType.Uzi);

            yield return new WaitForSeconds(.3f);

            weaponSelectRunning = false;
        }

        private void OnAttack(bool toggle)
        {
            inAttackState = toggle;
            if (inAttackState && !attackRunning)
                StartCoroutine(AttackCoroutine());
        }

        private IEnumerator AttackCoroutine()
        {
            attackRunning = true;

            RangeWeapon rangeWeapon = currentWeapon switch
            {
                WeaponType.Minigun => minigun,
                WeaponType.Shotgun => shotgun,
                WeaponType.Uzi => uzi,
                _ => null
            };
            
            if (rangeWeapon != null)
                rangeWeapon.SoundEvents = SoundEvents;

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
                    if (rangeWeapon != null)
                        rangeWeapon.OutOfAmmo();
                    else
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
                    case WeaponType.Shotgun:
                    case WeaponType.Uzi:
                        {
                            float seconds = 1 / (float)PerkRateOfFire;
                            Debug.Log(seconds);
                            if (ammo <= 0)
                            {
                                rangeWeapon.Attack(false);
                                rangeWeapon.OutOfAmmo();
                                
                                yield return new WaitForSeconds(seconds);
                                
                                break;
                            }

                            rangeWeapon.Attack(true);
                            animator.SetBool(AnimAttackBool, true);

                            yield return new WaitForSeconds(.05f);
                            
                            rangeWeapon.AfterEachShot();
                            animator.SetBool(AnimAttackBool, false);

                            ammo--;
                            OnActiveWeaponAttack?.Invoke(currentWeapon, ammo);

                            yield return new WaitForSeconds(seconds);

                            break;
                        }
                    default:
                        {
                            float seconds = 1 / (float)PerkRateOfFire;

                            animator.SetBool(AnimAttackBool, true);

                            shell.transform.SetParent(null, true);
                            shell.TargetDir = aim.AttackTarget != null ?
                                (aim.AttackTarget.transform.position - transform.position).normalized :
                                transform.forward;
                            shell.gameObject.SetActive(true);

                            SoundEvents.PlayerAttack();

                            yield return new WaitForSeconds(.01f);
                            
                            animator.SetBool(AnimAttackBool, false);

                            yield return new WaitForSeconds(seconds - .01f);

                            shell.gameObject.SetActive(false);
                            shell.transform.SetParent(launcher, false);
                            shell.transform.localPosition = Vector3.zero;
                            shell.transform.localRotation = Quaternion.identity;

                            break;
                        }
                }

            }

            aim.Engage(false);

            switch (currentWeapon)
            {
                case WeaponType.Minigun:
                case WeaponType.Shotgun:
                case WeaponType.Uzi:
                    {
                        Balances.SetBalance(ammoCollectable, ammo);
                        
                        rangeWeapon.Attack(false);
                        
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