using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)) { transform.Find("Skill1").GetComponent<HotbarSkill>().Activate(); }
        if (Input.GetKeyUp(KeyCode.Alpha2)) { transform.Find("Skill2").GetComponent<HotbarSkill>().Activate(); }
        if (Input.GetKeyUp(KeyCode.Alpha3)) { transform.Find("Skill3").GetComponent<HotbarSkill>().Activate(); }
        if (Input.GetKeyUp(KeyCode.Alpha4)) { transform.Find("Skill4").GetComponent<HotbarSkill>().Activate(); }
        if (Input.GetKeyUp(KeyCode.Alpha5)) { transform.Find("Skill5").GetComponent<HotbarSkill>().Activate(); }
        if (Input.GetKeyUp(KeyCode.Alpha6)) { transform.Find("Skill6").GetComponent<HotbarSkill>().Activate(); }
    }
}
