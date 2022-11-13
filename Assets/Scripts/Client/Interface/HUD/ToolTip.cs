using TMPro;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    public string Label;
    public string Description;

    private TextMeshProUGUI TextLabel { get { return gameObject.transform.Find("Canvas").Find("TextLabel").GetComponent<TextMeshProUGUI>();  } }
    private TextMeshProUGUI TextDescription { get { return gameObject.transform.Find("Canvas").Find("TextDescription").GetComponent<TextMeshProUGUI>(); } }
    private RectTransform Background { get { return gameObject.transform.Find("Canvas").Find("Background").GetComponent<RectTransform>(); } }
    private bool IsActive { get { 
            try
            {
                return gameObject.transform.Find("Canvas").Find("Background").gameObject.activeSelf;
            } catch
            {
                return false;
            }
            
        } }

    private Vector2 anchoredPosition;
    private Vector2 sizeDelta;

    void Start()
    {
        anchoredPosition = new Vector2(Background.anchoredPosition.x, Background.anchoredPosition.y);
        sizeDelta = new Vector2(Background.sizeDelta.x, Background.sizeDelta.y); 
    }

    void Update()
    {
        if (IsActive)
        {
            TextLabel.text = Label;
            TextDescription.text = Description;

            float lineCount = TextDescription.textInfo.lineCount;
            float offset = 12f + (5.5f * (lineCount - 1));

            Background.anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y - offset);
            Background.sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y + offset);
        }

    }
}
