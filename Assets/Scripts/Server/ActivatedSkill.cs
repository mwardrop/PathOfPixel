using Data.Skills;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

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
    }

    public void Update()
    {
        if(PlayerState == null) { Deactivate(); }
        if(PlayerState.IsDead) { Deactivate(); }

        foreach(PlayerState otherPlayer in ServerManager.Instance.StateManager.WorldState.Players)
        {
            if (otherPlayer.ClientId == PlayerState.ClientId) { continue; }

            if (Vector2.Distance(PlayerState.Location, otherPlayer.Location) < Skill.Radius)
            {
                if (otherPlayer.ActiveSkills.Count(x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId) == 0)
                {
                    otherPlayer.ActiveSkills.Add(new KeyValueState(
                        Skill.GetName(),
                        PlayerState.ClientId) {  
                            Index = PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value
                    });

                    ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(otherPlayer);

                    PlayersInRadius.Add(otherPlayer.ClientId);
     
                    //ServerManager.SendNetworkMessage(
                    //    otherPlayer.ClientId,
                    //    NetworkTags.ActivatePlayerSkill,
                    //    new StringIntegerData(Skill.GetName(), PlayerState.ClientId));

                    ServerManager.BroadcastNetworkMessage(
                        NetworkTags.ActivatePlayerSkill,
                        new SkillActivationData(
                            PlayerState.ClientId,
                            otherPlayer.ClientId,
                            Skill.GetName(),
                            PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value
                        ));
                }
            }
            else
            {
                if (otherPlayer.ActiveSkills.Count(x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId) > 0)
                {
                    otherPlayer.ActiveSkills.RemoveAll(x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId);
                    
                    ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(otherPlayer);

                    PlayersInRadius.Remove(otherPlayer.ClientId);

                    //ServerManager.SendNetworkMessage(
                    //    otherPlayer.ClientId,
                    //    NetworkTags.DeactivatePlayerSkill,
                    //    new StringIntegerData(Skill.GetName(), PlayerState.ClientId));

                    ServerManager.BroadcastNetworkMessage(
                        NetworkTags.DeactivatePlayerSkill,
                        new SkillActivationData(
                            PlayerState.ClientId,
                            otherPlayer.ClientId,
                            Skill.GetName(),
                            PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value
                        ));
                }
            }
        }
    }

    public ActivatedPlayerSkill Activate()
    {

        if (PlayerState.ActiveSkills.Count(x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId) == 0)
        {
            PlayerState.ActiveSkills.Add(new KeyValueState(
                Skill.GetName(),
                PlayerState.ClientId) { 
                    Index = PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value 
                });

            ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(PlayerState);

            ServerManager.BroadcastNetworkMessage(
                NetworkTags.ActivatePlayerSkill,
                new SkillActivationData(
                    PlayerState.ClientId,
                    PlayerState.ClientId,
                    Skill.GetName(),
                    PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value
                ));

        }

        //ServerManager.SendNetworkMessage(
        //    PlayerState.ClientId,
        //    NetworkTags.ActivatePlayerSkill,
        //    new StringIntegerData(Skill.GetName(), PlayerState.ClientId));

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

        ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(PlayerState);

        ServerManager.BroadcastNetworkMessage(
            NetworkTags.DeactivatePlayerSkill,
            new SkillActivationData(
                PlayerState.ClientId,
                PlayerState.ClientId,
                Skill.GetName(),
                PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value
            ));

        foreach (int playerId in PlayersInRadius)
        {
            var otherPlayer = ServerManager.Instance.StateManager.WorldState.GetPlayerState(playerId);
            otherPlayer.ActiveSkills.RemoveAll(
                x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId);

            ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(otherPlayer);

            ServerManager.BroadcastNetworkMessage(
                NetworkTags.DeactivatePlayerSkill,
                new SkillActivationData(
                    PlayerState.ClientId,
                    otherPlayer.ClientId,
                    Skill.GetName(),
                    PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value
                ));
        }

        //ServerManager.BroadcastNetworkMessage(
        //    NetworkTags.DeactivatePlayerSkill,
        //    new StringIntegerData(Skill.GetName(), PlayerState.ClientId));

        ActivatedPlayerSkills.Remove(this);
    }
}

