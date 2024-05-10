using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.Scripts.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private PlayerMovementRb playerMovement;
        [SerializeField] private Animator animator;
        [SerializeField] private ShadowCaster2D normalShadowCaster, runShadowCaster;
        private static readonly int IsWalk = Animator.StringToHash("IsWalk");
        private static readonly int IsRun = Animator.StringToHash("IsRun");

        private void FixedUpdate()
        {
            CheckState();
        }

        private void CheckState()
        {
            Vector2 vel = playerMovement.CurrentVelocity;

            float velXAbs = Mathf.Abs(vel.x);
            if (velXAbs <= 0.01f) // Idle
                SetIdle();
            else if (!playerMovement.IsSprinting) // Walk
                SetWalk();
            else // Run
                SetRun();
        }

        private void SetIdle()
        {
            ActivateNormalShadowCaster();
            animator.SetBool(IsWalk, false);
            animator.SetBool(IsRun, false);
        }


        private void SetWalk()
        {
            ActivateNormalShadowCaster();
            animator.SetBool(IsWalk, true);
            animator.SetBool(IsRun, false);
        }

        private void SetRun()
        {
            ActivateRunShadowCaster();
            animator.SetBool(IsWalk, false);
            animator.SetBool(IsRun, true);
        }

        private void ActivateNormalShadowCaster()
        {
            normalShadowCaster.enabled = true;
            runShadowCaster.enabled = false;
        }

        private void ActivateRunShadowCaster()
        {
            normalShadowCaster.enabled = false;
            runShadowCaster.enabled = true;
        }
    }
}