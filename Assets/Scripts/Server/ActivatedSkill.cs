using Data.Skills;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActivatedPlayerSkill
{
    public string Scene;
    public PlayerState PlayerState;
    public ISkill Skill;
    public List<ActivatedPlayerSkill> ActivatedPlayerSkills;
    public List<int> PlayersInRadius;

    public ActivatedPlayerSkill()
    {

    }

    public ActivatedPlayerSkill(string scene, PlayerState playerState, ISkill skill, List<ActivatedPlayerSkill> activatedPlayerSkills)
    {
        Scene = scene;
        PlayerState = playerState;
        Skill = skill;
        ActivatedPlayerSkills = activatedPlayerSkills;
        PlayersInRadius = new List<int>();

        Activate();
    }

    public ActivatedPlayerSkill Activate()
    {

        if (PlayerState.ActiveSkills.Count(x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId) == 0)
        {
            PlayerState.ActiveSkills.Add(new KeyValueState(
                Skill.GetName(),
                PlayerState.ClientId));
        }

        ServerManager.SendNetworkMessage(
            PlayerState.ClientId,
            NetworkTags.ActivatePlayerSkill,
            new StringIntegerData(Skill.GetName(), PlayerState.ClientId));

        ServerManager.Instance.StartCoroutine(DeactivateSkillCoroutine());

        IEnumerator DeactivateSkillCoroutine()
        {
            yield return new WaitForSeconds(Skill.Duration);
            Deactivate();

        }

        return this;
    }

    public void Deactivate()
    {
        PlayerState.ActiveSkills.RemoveAll(x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId);

        foreach(int playerId in PlayersInRadius)
        {
            ServerManager.Instance.StateManager.WorldState.GetPlayerState(playerId).ActiveSkills.RemoveAll(
                x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId);
        }

        ServerManager.BroadcastNetworkMessage(
            NetworkTags.DeactivatePlayerSkill,
            new StringIntegerData(Skill.GetName(), PlayerState.ClientId));

        ActivatedPlayerSkills.Remove(this);
    }
}

