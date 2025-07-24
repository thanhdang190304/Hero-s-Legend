using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyGame // Adjust this namespace to match your project's structure
{
    public class MissionUIManager : MonoBehaviour
    {
        public GameObject missionButtonPrefab;
        public Transform missionListParent;

        void Start()
        {
            LoadMissionsUI();
        }

        void OnEnable()
        {
            LoadMissionsUI();
        }

        public void RefreshMissionsUI()
        {
            LoadMissionsUI();
            Debug.Log("[MissionUIManager] Mission UI refreshed.");
        }

        private void LoadMissionsUI()
        {
            // Clear existing mission UI elements
            foreach (Transform child in missionListParent)
                Destroy(child.gameObject);

            foreach (var mission in MissionManager.instance.missions)
            {
                Debug.Log($"[MissionUIManager] Loading mission: {mission.description}, IsComplete={mission.IsComplete}, IsClaimed={mission.isClaimed}, levelIndex={mission.levelIndex}");

                // Instantiate the mission UI element
                GameObject missionGO = Instantiate(missionButtonPrefab, missionListParent);
                Button[] buttons = missionGO.GetComponentsInChildren<Button>();
                TextMeshProUGUI[] texts = missionGO.GetComponentsInChildren<TextMeshProUGUI>();

                Button descButton = buttons[0];
                Button claimButton = buttons[1];
                TextMeshProUGUI descText = texts[0];
                TextMeshProUGUI claimText = texts[1];

                // Set the description text
                descText.text = mission.description;

                // Ensure text wrapping and disable auto-sizing
                descText.enableWordWrapping = true;
                descText.overflowMode = TextOverflowModes.Ellipsis;
                descText.alignment = TextAlignmentOptions.Left;
                descText.enableAutoSizing = false;
                descText.fontSize = 15;
                Debug.Log($"[MissionUIManager] Description font size set to: {descText.fontSize}");

                // Adjust the RectTransform of the description text
                RectTransform descTextRect = descText.GetComponent<RectTransform>();
                descTextRect.sizeDelta = new Vector2(descTextRect.sizeDelta.x, descText.preferredHeight);

                // Stretch the description button to cover the text area
                RectTransform descButtonRect = descButton.GetComponent<RectTransform>();
                descButtonRect.sizeDelta = new Vector2(300, descText.preferredHeight);
                descButtonRect.anchorMin = new Vector2(0, 1);
                descButtonRect.anchorMax = new Vector2(0.7f, 1);
                descButtonRect.pivot = new Vector2(0.5f, 1);
                descButtonRect.anchoredPosition = new Vector2(0, -descTextRect.anchoredPosition.y);

                // Adjust the claim button
                RectTransform claimButtonRect = claimButton.GetComponent<RectTransform>();
                claimButtonRect.anchorMin = new Vector2(0.75f, 1);
                claimButtonRect.anchorMax = new Vector2(1, 1);
                claimButtonRect.pivot = new Vector2(1, 1);
                claimButtonRect.anchoredPosition = new Vector2(-10, -descTextRect.anchoredPosition.y);
                claimButtonRect.sizeDelta = new Vector2(80, descText.preferredHeight);

                // Set the claim button text and interactability
                if (mission.isClaimed)
                {
                    claimText.text = "Claimed!";
                    claimButton.interactable = false;
                }
                else
                {
                    bool canClaim = mission.IsComplete;
                    claimText.text = canClaim ? "Claim" : "Incomplete";
                    claimText.enableAutoSizing = false;
                    claimText.fontSize = 15;
                    Debug.Log($"[MissionUIManager] Claim font size set to: {claimText.fontSize}");
                    claimButton.interactable = canClaim;
                }

                // Description button click listener
                descButton.onClick.AddListener(() =>
                {
                    Debug.Log($"[Mission] {mission.description} - {(mission.requiresNoDamage ? $"No damage condition, levelIndex={mission.levelIndex}" : $"{mission.currentKills}/{mission.killTarget}")}, Claimed={mission.isClaimed}, IsComplete={mission.IsComplete}");
                });

                // Claim button click listener
                claimButton.onClick.AddListener(() =>
                {
                    bool claimed = MissionManager.instance.ClaimMission(mission);
                    if (claimed)
                    {
                        claimText.text = "Claimed!";
                        claimButton.interactable = false;
                        Debug.Log($"[MissionUIManager] Mission '{mission.description}' claimed successfully.");
                    }
                    else
                    {
                        Debug.Log($"[MissionUIManager] Mission '{mission.description}' not claimed: IsComplete={mission.IsComplete}, IsClaimed={mission.isClaimed}");
                    }
                    RefreshMissionsUI();
                });

                // Ensure the mission UI element's height adjusts
                RectTransform missionGORect = missionGO.GetComponent<RectTransform>();
                missionGORect.sizeDelta = new Vector2(missionGORect.sizeDelta.x, descText.preferredHeight + 10);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(missionListParent.GetComponent<RectTransform>());
        }
    }
}