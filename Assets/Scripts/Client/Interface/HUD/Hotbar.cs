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
            if (Input.GetKeyUp(KeyCode.Alpha1)) { transform.Find("Skill1").GetComponent<HotbarSkill>().Activate(); }
            if (Input.GetKeyUp(KeyCode.Alpha2)) { transform.Find("Skill2").GetComponent<HotbarSkill>().Activate(); }
            if (Input.GetKeyUp(KeyCode.Alpha3)) { transform.Find("Skill3").GetComponent<HotbarSkill>().Activate(); }
            if (Input.GetKeyUp(KeyCode.Alpha4)) { transform.Find("Skill4").GetComponent<HotbarSkill>().Activate(); }
            if (Input.GetKeyUp(KeyCode.Alpha5)) { transform.Find("Skill5").GetComponent<HotbarSkill>().Activate(); }
            if (Input.GetKeyUp(KeyCode.Alpha6)) { transform.Find("Skill6").GetComponent<HotbarSkill>().Activate(); }

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
