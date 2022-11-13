using DarkRift;
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
    public List<PlayerState> PlayersInRadius;

    private bool Deactivated = false;

    public ActivatedPlayerSkill() {}

    public ActivatedPlayerSkill(string scene, PlayerState playerState, ISkill skill, List<ActivatedPlayerSkill> activatedPlayerSkills)
    {
        Scene = scene;
        PlayerState = playerState;
        Skill = skill;
        ActivatedPlayerSkills = activatedPlayerSkills;
        PlayersInRadius = new List<PlayerState>();
    }

    public void Update()
    {
        if(PlayerState == null) { Deactivate(); }
        if(PlayerState.IsDead) { Deactivate(); }
        if (ServerManager.Instance.Connections[PlayerState.ClientId].Client.ConnectionState 
            != ConnectionState.Connected) { Deactivate(); }

        if (!Deactivated)
        {
            foreach (PlayerState otherPlayer in ServerManager.Instance.StateManager.WorldState.Players)
            {
                if (otherPlayer.ClientId == PlayerState.ClientId) { continue; }
                if (otherPlayer.Scene.ToLower() != PlayerState.Scene.ToLower()) { continue; }

                if (Vector2.Distance(PlayerState.Location, otherPlayer.Location) < Skill.Radius)
                {
                    if (otherPlayer.ActiveSkills.Count(x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId) == 0)
                    {
                        otherPlayer.ActiveSkills.Add(new KeyValueState(
                            Skill.GetName(),
                            PlayerState.ClientId)
                        {
                            Index = PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value
                        });

                        ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(otherPlayer);

                        PlayersInRadius.Add(otherPlayer);

                        ServerManager.BroadcastNetworkMessage(
                            NetworkTags.ActivatePlayerSkill,
                            new SkillActivationData(
                                PlayerState.ClientId,
                                otherPlayer.ClientId,
                                Skill.GetName(),
                                PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value,
                                PlayerState.Scene
                            ));
                    }
                }
                else
                {
                    if (otherPlayer.ActiveSkills.Count(x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId) > 0)
                    {
                        otherPlayer.ActiveSkills.RemoveAll(x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId);

                        ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(otherPlayer);

                        PlayersInRadius.Remove(otherPlayer);

                        ServerManager.BroadcastNetworkMessage(
                            NetworkTags.DeactivatePlayerSkill,
                            new SkillActivationData(
                                PlayerState.ClientId,
                                otherPlayer.ClientId,
                                Skill.GetName(),
                                PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value,
                                PlayerState.Scene
                            ));
                    }
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
                    PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value,
                    PlayerState.Scene
                ));

        }

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
        Deactivated = true;

        PlayerState.ActiveSkills.RemoveAll(x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId);

        ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(PlayerState);

        ServerManager.BroadcastNetworkMessage(
            NetworkTags.DeactivatePlayerSkill,
            new SkillActivationData(
                PlayerState.ClientId,
                PlayerState.ClientId,
                Skill.GetName(),
                PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value,
                PlayerState.Scene
            ));

        foreach (PlayerState otherPlayer in PlayersInRadius)
        {
            otherPlayer.ActiveSkills.RemoveAll(
                x => x.Key == Skill.GetName() && x.Value == PlayerState.ClientId);

            ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(otherPlayer);

            ServerManager.BroadcastNetworkMessage(
                NetworkTags.DeactivatePlayerSkill,
                new SkillActivationData(
                    PlayerState.ClientId,
                    otherPlayer.ClientId,
                    Skill.GetName(),
                    PlayerState.Skills.First(x => x.Key == Skill.GetName()).Value,
                    PlayerState.Scene
                ));
        }

        ActivatedPlayerSkills.Remove(this);
    }
}

public class ActivatedEnemySkill
{
    public string Scene;
    public EnemyState EnemyState;
    public ISkill Skill;
    public List<ActivatedEnemySkill> ActivatedEnemySkills;
    public List<EnemyState> EnemiesInRadius;

    private bool Deactivated = false;

    public ActivatedEnemySkill() { }

    public ActivatedEnemySkill(string scene, EnemyState enemyState, ISkill skill, List<ActivatedEnemySkill> activatedEnemySkills)
    {
        Scene = scene;
        EnemyState = enemyState;
        Skill = skill;
        ActivatedEnemySkills = activatedEnemySkills;
        EnemiesInRadius = new List<EnemyState>();
    }

    public void Update()
    {
        if (EnemyState == null) { Deactivate(); }
        if (EnemyState.IsDead) { Deactivate(); }

        if (!Deactivated)
        {
            var otherEnemies =
                ServerManager.Instance.StateManager.WorldState.Scenes.First(x => x.Name.ToLower() == Scene.ToLower()).Enemies;

            foreach (EnemyState otherEnemy in otherEnemies)
            {
                if (otherEnemy.EnemyGuid.GetHashCode() == EnemyState.EnemyGuid.GetHashCode()) { continue; }

                if (Vector2.Distance(EnemyState.Location, otherEnemy.Location) < Skill.Radius)
                {
                    if (otherEnemy.ActiveSkills.Count(x => x.Key == Skill.GetName() && x.Value == EnemyState.EnemyGuid.GetHashCode()) == 0)
                    {
                        otherEnemy.ActiveSkills.Add(new KeyValueState(
                            Skill.GetName(),
                            EnemyState.EnemyGuid.GetHashCode())
                        {
                            Index = EnemyState.Skills.First(x => x.Key == Skill.GetName()).Value
                        });

                        ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(otherEnemy);

                        EnemiesInRadius.Add(otherEnemy);

                        ServerManager.BroadcastNetworkMessage(
                            NetworkTags.ActivateEnemySkill,
                            new SkillActivationData(
                                EnemyState.EnemyGuid.GetHashCode(),
                                otherEnemy.EnemyGuid.GetHashCode(),
                                Skill.GetName(),
                                EnemyState.Skills.First(x => x.Key == Skill.GetName()).Value,
                                Scene
                            ));
                    }
                }
                else
                {
                    if (otherEnemy.ActiveSkills.Count(x => x.Key == Skill.GetName() && x.Value == EnemyState.EnemyGuid.GetHashCode()) > 0)
                    {
                        otherEnemy.ActiveSkills.RemoveAll(x => x.Key == Skill.GetName() && x.Value == EnemyState.EnemyGuid.GetHashCode());

                        ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(otherEnemy);

                        EnemiesInRadius.Remove(otherEnemy);

                        ServerManager.BroadcastNetworkMessage(
                            NetworkTags.DeactivateEnemySkill,
                            new SkillActivationData(
                                EnemyState.EnemyGuid.GetHashCode(),
                                otherEnemy.EnemyGuid.GetHashCode(),
                                Skill.GetName(),
                                EnemyState.Skills.First(x => x.Key == Skill.GetName()).Value,
                                Scene
                            ));
                    }
                }
            }
        }
    }

    public ActivatedEnemySkill Activate()
    {

        if (EnemyState.ActiveSkills.Count(x => x.Key == Skill.GetName() && x.Value == EnemyState.EnemyGuid.GetHashCode()) == 0)
        {
            EnemyState.ActiveSkills.Add(new KeyValueState(
                Skill.GetName(),
                EnemyState.EnemyGuid.GetHashCode())
            {
                Index = EnemyState.Skills.First(x => x.Key == Skill.GetName()).Value
            });

            ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(EnemyState);

            ServerManager.BroadcastNetworkMessage(
                NetworkTags.ActivateEnemySkill,
                new SkillActivationData(
                    EnemyState.EnemyGuid.GetHashCode(),
                    EnemyState.EnemyGuid.GetHashCode(),
                    Skill.GetName(),
                    EnemyState.Skills.First(x => x.Key == Skill.GetName()).Value,
                    Scene
                ));

        }

        return this;
    }

    public void Deactivate()
    {
        Deactivated = true;

        foreach (EnemyState otherEnemy in EnemiesInRadius)
        {
            otherEnemy.ActiveSkills.RemoveAll(
                x => x.Key == Skill.GetName() && x.Value == EnemyState.EnemyGuid.GetHashCode());

            ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(otherEnemy);

            ServerManager.BroadcastNetworkMessage(
                NetworkTags.DeactivateEnemySkill,
                new SkillActivationData(
                    EnemyState.EnemyGuid.GetHashCode(),
                    otherEnemy.EnemyGuid.GetHashCode(),
                    Skill.GetName(),
                    EnemyState.Skills.First(x => x.Key == Skill.GetName()).Value,
                    Scene
                ));

        }

        EnemyState.ActiveSkills.RemoveAll(x => x.Key == Skill.GetName() && x.Value == EnemyState.EnemyGuid.GetHashCode());

        ServerManager.Instance.StateManager.StateCalculator.CalcCharacterState(EnemyState);

        ServerManager.BroadcastNetworkMessage(
            NetworkTags.DeactivateEnemySkill,
            new SkillActivationData(
                EnemyState.EnemyGuid.GetHashCode(),
                EnemyState.EnemyGuid.GetHashCode(),
                Skill.GetName(),
                EnemyState.Skills.First(x => x.Key == Skill.GetName()).Value,
                Scene
            ));

        ActivatedEnemySkills.Remove(this);

    }
}