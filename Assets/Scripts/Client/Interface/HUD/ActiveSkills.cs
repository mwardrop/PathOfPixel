using UnityEngine;

public class ActiveSkills : MonoBehaviour
{

    public PlayerState PlayerState;

    // Start is called before the first frame update
    void Start()
    {
        try { PlayerState = ClientManager.Instance.StateManager.PlayerState; } catch { PlayerState = null; }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerState != null)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var iconObject = transform.GetChild(i).gameObject;

                if (PlayerState.ActiveSkills.Count > i)
                {
                    var icon = iconObject.GetComponent<Icon>();
                    var skill = CreateInstance.Skill(PlayerState.ActiveSkills[i].Key, 0);

                    icon.CurrentIcon = (Sprite)icon.GetType().GetField($"SkillIcon{skill.IconId}").GetValue(icon);

                    iconObject.SetActive(true);
                }
                else
                {
                    iconObject.SetActive(false);
                }
            }
        }
    }
}
