using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{

    public partial class MovableUnit
    {
        protected bool fadingOut;
        protected bool fadedOut;

        protected void TogglePhisBody(bool toggle)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.ResetCenterOfMass();
            rb.ResetInertiaTensor();
            desiredVelocity = Vector3.zero;

            if (toggle)
            {
                col.enabled = toggle;
                rb.mass = toggle ? 1.0f : 0.0f;
                rb.useGravity = toggle;

                transform.localScale = Vector3.one * SizeScale;
            }
            else
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.ResetCenterOfMass();
                rb.ResetInertiaTensor();
                rb.useGravity = toggle;
                rb.mass = toggle ? 1.0f * SizeScale : 0.0f;
                col.enabled = toggle;
            }
        }

        internal bool FadeOut()
        {
            if (!fadingOut)
            {
                fadedOut = false;
                fadingOut = true;
                StartCoroutine(FadeMeOut());
            }
            return fadedOut;
        }

        private IEnumerator FadeMeOut()
        {
            TogglePhisBody(false);

            var a = 1.0f * SizeScale;
            while (a > 0)
            {
                a -= fadeSpeed * Time.deltaTime;
                this.transform.localScale = Vector3.forward * a + Vector3.right * a + Vector3.up * SizeScale;

                yield return null;
            }

            this.transform.localScale = Vector3.forward * 0 + Vector3.right * 0 + Vector3.up * SizeScale;
            yield return null;

            ren.enabled = false;
            fadedOut = true;
        }

        internal void FadeIn()
        {
            StartCoroutine(FadeMeIn());
        }

        private IEnumerator FadeMeIn()
        {

            ren.enabled = true;

            var a = 0.0f;
            while (a < 1.0f * SizeScale)
            {
                a += fadeSpeed * Time.deltaTime;
                this.transform.localScale = Vector3.forward * a + Vector3.right * a + Vector3.up * SizeScale;

                yield return null;
            }

            this.transform.localScale = Vector3.one * SizeScale;

            fadingOut = false;
            fadedOut = false;

            TogglePhisBody(true);
        }
    }
}