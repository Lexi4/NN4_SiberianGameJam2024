using UnityEngine;

namespace NPC
{
    public class EnemyAnimationController : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] public Animator animator;
        public int MovingAnimation
        {
            get => Animator.StringToHash("MovingAnimation");
            set
            {
                if (value > 0)
                {
                    animator.SetBool(MovingAnimation, true);
                    animator.SetBool(IdleAnimation, false);
                    animator.SetBool(StunAnimation, false);
                    animator.SetBool(AttackAnimation, false);
                }
                else
                {
                    IdleAnimation = 1;
                }
            }
        }
        public int IdleAnimation
        {
            get => Animator.StringToHash("IdleAnimation");
            set
            {
                if (value > 0)
                {
                    animator.SetBool(MovingAnimation, false);
                    animator.SetBool(IdleAnimation, true);
                    animator.SetBool(StunAnimation, false);
                    animator.SetBool(AttackAnimation, false);
                }
            }
        }
        public int StunAnimation
        {
            get => Animator.StringToHash("StunAnimation");
            set
            {
                if (value > 0)
                {
                    animator.SetBool(MovingAnimation, false);
                    animator.SetBool(IdleAnimation, true);
                    animator.SetBool(StunAnimation, false);
                    animator.SetBool(AttackAnimation, false);
                }
            }
        }
        public int AttackAnimation
        {
            get => Animator.StringToHash("AttackAnimation");
            set
            {
                if (value > 0)
                {
                    animator.SetBool(MovingAnimation, false);
                    animator.SetBool(IdleAnimation, false);
                    animator.SetBool(StunAnimation, false);
                    animator.SetBool(AttackAnimation, true);
                }
            }
        }
    }
}