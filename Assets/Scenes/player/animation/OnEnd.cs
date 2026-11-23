
using UnityEngine;

public class OnEnd : StateMachineBehaviour
{
    [Header("Next Animation")]
    [SerializeField] private string nextAnimation = "";

    [Header("Combat")]
    [SerializeField] private bool notifyCombatController = false;

    private bool hasTriggered;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,int layerIndex)
    {
        hasTriggered = false;
    }

    public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hasTriggered)
            return;

        if (stateInfo.normalizedTime < 1f ||animator.IsInTransition(layerIndex))
            return;

        hasTriggered = true;

        if (notifyCombatController)
        {
            CombatController combat = animator.GetComponentInParent<CombatController>();

            if (combat != null)
                combat.OnCombatAnimationFinished();
            else
                Debug.LogError("OnEnd: CombatController not found!");

            return;
        }

        AnimScript anim = animator.GetComponentInParent<AnimScript>();

        if (anim == null)
            anim = animator.GetComponentInChildren<AnimScript>();

        if (anim == null)
        {
            Debug.LogError("OnEnd: AnimScript not found!");
            return;
        }

        if (!string.IsNullOrEmpty(nextAnimation))
            anim.ChangeAnimation(nextAnimation);
        else
            anim.AnimationFinished();
    }
}
