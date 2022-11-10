using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillsPanel : BasePanel
{

    private enum SkillsScreens
    {
        Attacks,
        Skills,
        Passives
    }
    private SkillsScreens SelectedScreen = SkillsScreens.Attacks;

    public Button AttacksButton, SkillsButton, PassivesButton, CloseButton;
    public GameObject Frame1, Frame2, Frame3, Frame4, Frame5, Frame6, Frame7, Frame8, Frame9, Frame10, Frame11, Frame12, Frame13, Frame14, Frame15, Frame16;

    public GameObject IconPrefab;
    // Start is called before the first frame update
    void Start()
    {
        AttacksButton.onClick.AddListener(AttacksClicked);
        SkillsButton.onClick.AddListener(SkillsClicked);
        PassivesButton.onClick.AddListener(PassivesClicked);
        CloseButton.onClick.AddListener(CloseClicked);
        DrawScreen();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        gameObject.transform.Find("ButtonAttacks").Find("Points").GetComponent<TextMeshProUGUI>().text = 
            ClientManager.Instance.StateManager.PlayerState.AttackPoints.ToString();

        gameObject.transform.Find("ButtonSkills").Find("Points").GetComponent<TextMeshProUGUI>().text =
            ClientManager.Instance.StateManager.PlayerState.SkillPoints.ToString();

        gameObject.transform.Find("ButtonPassives").Find("Points").GetComponent<TextMeshProUGUI>().text =
            ClientManager.Instance.StateManager.PlayerState.PassivePoints.ToString();

    }

    private void DrawScreen()
    {
        var playerType = CreateInstance.Character(ClientManager.Instance.StateManager.PlayerState.Type.ToString());

        var iconFrameCount = gameObject.transform.Find("IconFrames").childCount;

        for (var i = 0; i < iconFrameCount; i++)
        {
            var icon = gameObject.transform.Find("IconFrames").Find($"Frame{i + 1}").Find("Icon").GetComponent<Icon>();
            var level = gameObject.transform.Find("IconFrames").Find($"Frame{i + 1}").Find("Level").GetComponent<TextMeshProUGUI>();
            var frame = gameObject.transform.Find("IconFrames").Find($"Frame{i + 1}");
            frame.gameObject.SetActive(false);

            switch (SelectedScreen)
            {
                case SkillsScreens.Attacks:
                    if (playerType.Attacks.Count > i)
                    {
                        var attack = playerType.Attacks[i];

                        level.text = ClientManager.Instance.StateManager.PlayerState.Attacks.First(x => x.Key == attack.GetName()).Value.ToString();
                        icon.CurrentIcon = (Sprite)icon.GetType().GetField($"AttackIcon{attack.IconId}").GetValue(icon);
                        icon.IsDraggable = true;
                        icon.Type = IconType.Attack;
                        icon.TypeKey = playerType.Attacks[i].GetName();
                        frame.gameObject.SetActive(true);
                    }
                    break;
                case SkillsScreens.Passives:
                    if (playerType.Passives.Count > i)
                    {
                        var passive = playerType.Passives[i];

                        level.text = ClientManager.Instance.StateManager.PlayerState.Passives.First(x => x.Key == passive.GetName()).Value.ToString();
                        icon.CurrentIcon = (Sprite)icon.GetType().GetField($"PassiveIcon{passive.IconId}").GetValue(icon);
                        icon.IsDraggable = false;
                        icon.Type = IconType.Passive;
                        icon.TypeKey = playerType.Passives[i].GetName();
                        frame.gameObject.SetActive(true);
                    }
                    break;
                case SkillsScreens.Skills:
                    if (playerType.Skills.Count > i)
                    {
                        var skill = playerType.Skills[i];

                        level.text = ClientManager.Instance.StateManager.PlayerState.Skills.First(x => x.Key == skill.GetName()).Value.ToString();
                        icon.CurrentIcon = (Sprite)icon.GetType().GetField($"SkillIcon{skill.IconId}").GetValue(icon);
                        icon.IsDraggable = true;
                        icon.Type = IconType.Skill;
                        icon.TypeKey = playerType.Skills[i].GetName();
                        frame.gameObject.SetActive(true);
                    }
                    break;
            }


        }
    }

    private void AttacksClicked()
    {
        SelectedScreen = SkillsScreens.Attacks;
        AttacksButton.transform.Find("Clicked").gameObject.SetActive(true);
        Invoke("ResetButtons", 0.1f);
        DrawScreen();
    }

    private void SkillsClicked()
    {
        SelectedScreen = SkillsScreens.Skills;
        SkillsButton.transform.Find("Clicked").gameObject.SetActive(true);
        Invoke("ResetButtons", 0.1f);
        DrawScreen();
    }

    private void PassivesClicked()
    {
        SelectedScreen = SkillsScreens.Passives;
        PassivesButton.transform.Find("Clicked").gameObject.SetActive(true);
        Invoke("ResetButtons", 0.1f);
        DrawScreen();
    }

    private void ResetButtons()
    {
        AttacksButton.transform.Find("Clicked").gameObject.SetActive(false);
        SkillsButton.transform.Find("Clicked").gameObject.SetActive(false);
        PassivesButton.transform.Find("Clicked").gameObject.SetActive(false);
    }

    private void CloseClicked()
    {
        gameObject.SetActive(false);

    }

}

    