using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CharacterHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(3, 10)] // Sets a multi-line text box with 3 minimum and 10 maximum lines
    public string characterInfo = "Default Info"; // Unique info for each character
    public GameObject infoBox; // Assign the Panel GameObject
    public TextMeshProUGUI infoText; // Assign the TextMeshPro component

    void Start()
    {
        if (infoBox != null)
            infoBox.SetActive(false); // Hide info box initially
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (infoBox != null && infoText != null)
        {
            infoBox.SetActive(true); // Show info box
            infoText.text = characterInfo; // Use the Inspector-set value
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoBox != null)
            infoBox.SetActive(false); // Hide info box
    }
}