using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RuleSelector : MonoBehaviour
{
    [SerializeField] Selectable[] ruleTitle;
    [SerializeField] GameObject[] ruleText;

    void Update()
    {
        if (EventSystem.current == null) return;

        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < ruleTitle.Length; i++)
        {
            if (selectedObj == ruleTitle[i].gameObject)
            {
                Debug.Log($"{ruleTitle[i].name} が選択中！");
                SwtichText(i);
            }
        }
    }

    void SwtichText(int n)
    {
        for (int i = 0; i < ruleText.Length; i++)
        {
            if (ruleText[i] != null)
            {
                ruleText[i].SetActive(i == n);
            }
        }
    }
}
