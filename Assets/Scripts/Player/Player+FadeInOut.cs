using System.Collections;

namespace Assets.Scripts
{
    public partial class Player
    {
        private const string shaderColorKey = "_BaseColor";

        private bool fadingOut;
        private bool fadedOut;

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

                foreach (var m in ren.materials)
                {
                    var c = m.GetColor(shaderColorKey);
                    c.a = a;
                    m.SetColor(shaderColorKey, c);
                }
                yield return null;
            }
            ren.enabled = false;
            fadedOut = true;
        }

        internal void FadeIn()
        {
            TogglePhisBody(true);

            foreach (var m in ren.materials)
            {
                var c = m.GetColor(shaderColorKey);
                c.a = 1.0f;
                m.SetColor(shaderColorKey, c);
            }

            ren.enabled = true;

            fadingOut = false;
            fadedOut = false;
        }
    }
}