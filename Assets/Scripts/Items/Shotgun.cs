using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Shotgun : RangeWeapon
    {
        public override void AfterEachShot()
        {
            base.AfterEachShot();
            
            Attack(false);
        }

        protected override void ShotSound()
        {
            SoundEvents.BombExplode();
        }
    }
}