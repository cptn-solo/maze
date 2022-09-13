using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class Player
    {
        private const string shaderColorKey = "_BaseColor";

        private bool fadingOut;
        private bool fadedOut;

        private void TogglePhisBody(bool toggle)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.ResetCenterOfMass();
            rb.ResetInertiaTensor();

            if (toggle)
            {
                col.enabled = toggle;
                rb.mass = toggle ? 1.0f : 0.0f;
                rb.useGravity = toggle;
            }
            else
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.ResetCenterOfMass();
                rb.ResetInertiaTensor();
                rb.useGravity = toggle;
                rb.mass = toggle ? 1.0f : 0.0f;
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

            var a = 1.0f;
            while (a > 0)
            {
                a -= .05f;
                this.transform.localScale = Vector3.forward * a + Vector3.right * a + Vector3.up;
                //this.transform.localScale = Vector3.forward + Vector3.right + Vector3.up * a;

                yield return null;
            }

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
            while (a < 1.0f)
            {
                a += .05f;
                this.transform.localScale = Vector3.forward * a + Vector3.right * a + Vector3.up;
                //this.transform.localScale = Vector3.forward + Vector3.right + Vector3.up * a;
                yield return null;
            }

            fadingOut = false;
            fadedOut = false;

            TogglePhisBody(true);
        }
    }
}