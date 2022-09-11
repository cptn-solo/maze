using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public partial class Player
    {
        private IEnumerator EmergencyExit()
        {
            yield return new WaitForSeconds(1.5f);

            if (targetPosition != default)
            {
                // player stacked in apex
                transform.position = targetPosition + Vector3.up * transform.position.y;
                targetPosition = default;
                TogglePhisBody(true);

                ReadLocalAxis();
            }
        }

        private IEnumerator ApexMove()
        {
            if ((targetPosition - center).sqrMagnitude < tresholdSqr * 2)
            {
                targetPosition += 2 * treshold * (targetPosition - center).normalized;
            }

            StartCoroutine(EmergencyExit());

            while (targetPosition != default)
            {
                var grounded = Grounded();
                
                TogglePhisBody(false);

                if ((targetPosition - grounded).sqrMagnitude > tresholdSqr * .1f)
                {
                    var current = (grounded - sideSwitchPivot).normalized;
                    var target = (targetPosition - sideSwitchPivot).normalized;
                    var step = sideSwitchPivot + sideSwitchRadius * Vector3.RotateTowards(
                        current, target, apexSpeed * Time.deltaTime, 0.0f);

                    var lookDir = (step - grounded).normalized;

                    var delta1 = Vector3.SignedAngle(
                        transform.forward,
                        lookDir,
                        transform.up);

                    transform.RotateAround(transform.position, transform.up, delta1);

                    transform.position = step + transform.up * transform.position.y;
                    //rb.AddForce((targetPosition - grounded).normalized * 2.0f, ForceMode.Force);
                }
                else
                {
                    transform.position = targetPosition + Vector3.up * transform.position.y;
                    targetPosition = default;

                    TogglePhisBody(true);

                    ReadLocalAxis();
                }

                yield return null;
            }
        }
    }
}