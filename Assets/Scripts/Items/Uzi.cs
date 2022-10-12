namespace Assets.Scripts
{
    public class Uzi : RangeWeapon
    {
        protected override void ShotSound()
        {
            SoundEvents.UziShot();
        }

    }
}