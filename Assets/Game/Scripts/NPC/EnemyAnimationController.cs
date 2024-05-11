using UnityEngine;

namespace NPC
{
    public class EnemyAnimationController : MonoBehaviour
    {
        [Header("Animation")] [SerializeField] public Animator animator;

        public int MovingAnimation
        {
            get => Animator.StringToHash("IsWalk");
            set
            {
                if (value > 0)
                {
                    animator.SetBool(MovingAnimation, true);
                    animator.SetBool(IdleAnimation, false);
                }
                else
                {
                    IdleAnimation = 1;
                }
            }
        }

        public int IdleAnimation
        {
            get => Animator.StringToHash("IsIdle");
            set
            {
                if (value > 0)
                {
                    animator.SetBool(MovingAnimation, false);
                    animator.SetBool(IdleAnimation, true);
                }
            }
        }

        public int StunAnimation
        {
            get => Animator.StringToHash("IsIdle");
            set
            {
                if (value > 0)
                {
                    animator.SetBool(MovingAnimation, false);
                    animator.SetBool(IdleAnimation, true);
                }
            }
        }

        public int AttackAnimation
        {
            get => Animator.StringToHash("IsWalk");
            set
            {
                if (value > 0)
                {
                    animator.SetBool(MovingAnimation, false);
                    animator.SetBool(IdleAnimation, false);
                }
            }
        }
    }
}