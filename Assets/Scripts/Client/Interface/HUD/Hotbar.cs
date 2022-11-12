using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public Slider LevelBar;
    public GameObject HealthOrbWave;
    private RectTransform HealthTransform { get { return HealthOrbWave.GetComponent<RectTransform>(); } }

    public GameObject ManaOrbWave;
    private RectTransform ManaTransform {  get { return ManaOrbWave.GetComponent<RectTransform>(); } }

    private HotbarSkill Skill1 { get { return transform.Find("Skill1").GetComponent<HotbarSkill>(); } }
    private HotbarSkill Skill2 { get { return transform.Find("Skill2").GetComponent<HotbarSkill>(); } }
    private HotbarSkill Skill3 { get { return transform.Find("Skill3").GetComponent<HotbarSkill>(); } }
    private HotbarSkill Skill4 { get { return transform.Find("Skill4").GetComponent<HotbarSkill>(); } }
    private HotbarSkill Skill5 { get { return transform.Find("Skill5").GetComponent<HotbarSkill>(); } }
    private HotbarSkill Skill6 { get { return transform.Find("Skill6").GetComponent<HotbarSkill>(); } }

    private PlayerState PlayerState
    {
        get
        {
            try { return ClientManager.Instance.StateManager.PlayerState; } catch { return null; }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerState != null)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1)) { Skill1.Activate(); }
            if (Input.GetKeyUp(KeyCode.Alpha2)) { Skill2.Activate(); }
            if (Input.GetKeyUp(KeyCode.Alpha3)) { Skill3.Activate(); }
            if (Input.GetKeyUp(KeyCode.Alpha4)) { Skill4.Activate(); }
            if (Input.GetKeyUp(KeyCode.Alpha5)) { Skill5.Activate(); }
            if (Input.GetKeyUp(KeyCode.Alpha6)) { Skill6.Activate(); }

            float maxExp = (100 * PlayerState.Level) * PlayerState.Level;
            float currentExp = PlayerState.Experience;

            LevelBar.value = currentExp / maxExp;

            HealthTransform.anchoredPosition = new Vector2(
                HealthTransform.anchoredPosition.x,
                ((PlayerState.Health / PlayerState.MaxHealth) * 100) - 40);

            ManaTransform.anchoredPosition = new Vector2(
                ManaTransform.anchoredPosition.x,
                ((PlayerState.Mana / PlayerState.MaxMana) * 100) - 40);

        }
    }
}
