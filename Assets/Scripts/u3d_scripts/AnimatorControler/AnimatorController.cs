using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class AnimatorController : StateMachineBehaviour
{
    public List<int> common;
    public List<int> battle;
    public bool inBattle = false;

    private void Awake()
    {
        common = new List<int>();
        battle = new List<int>();

        common.Add(Animator.StringToHash("Base Layer.Common.Idle"));
        battle.Add(Animator.StringToHash("Base Layer.Battle.Idle"));
        common.Add(Animator.StringToHash("Base Layer.Common.WalkAndRun.MoveLoop"));
        battle.Add(Animator.StringToHash("Base Layer.Battle.WalkAndRun.MoveLoop"));
        common.Add(Animator.StringToHash("Base Layer.Common.WalkAndRun.MoveStart"));
        battle.Add(Animator.StringToHash("Base Layer.Battle.WalkAndRun.MoveStart"));
        common.Add(Animator.StringToHash("Base Layer.Common.WalkAndRun.MoveEnd"));
        battle.Add(Animator.StringToHash("Base Layer.Battle.WalkAndRun.MoveEnd"));
    }

    private float getCurrentClipOffset(Animator animator)
    {
        var StateInfos =  animator.GetCurrentAnimatorStateInfo(0);
        return StateInfos.normalizedTime;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var inBattleNew = animator.GetBool("InBattle");
        var fullPathHash = stateInfo.fullPathHash;

        if (inBattle!= inBattleNew)
        {
            if (inBattleNew)
            {
                var index = common.FindIndex((x) => x == fullPathHash);
                animator.CrossFadeInFixedTime(battle[index], 0.25f, 0, getCurrentClipOffset(animator));
            }
            else
            {
                var index = battle.FindIndex((x) => x == fullPathHash);
                animator.CrossFadeInFixedTime(common[index], 0.25f, 0, getCurrentClipOffset(animator));
            }
        }
        inBattle = inBattleNew;
    }

    public void playerSkillAttackAnimatior(Animator animator, string name, int index = 0, float fixTransDuration = 0.0f, float offset = 0.0f)
    {
        StringBuilder MyStringBuilder = new StringBuilder("Base Layer.Skill.", 10);
        MyStringBuilder.Append(name);
        MyStringBuilder.Append(".");
        MyStringBuilder.Append(index);
        int clip = Animator.StringToHash(MyStringBuilder.ToString());
        animator.CrossFadeInFixedTime(clip, fixTransDuration, 0, offset);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var fullPathHash = stateInfo.fullPathHash;
        if (fullPathHash == Animator.StringToHash("Base Layer.Skill.Attack.End"))
        {
            int clip = Animator.StringToHash("Base Layer.Battle.Idle");
            animator.CrossFadeInFixedTime(clip, 0.0f, 0, 0);
        }

    }
}
