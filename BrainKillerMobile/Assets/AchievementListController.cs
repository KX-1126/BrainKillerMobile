using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct AchievementRecord
{
    public string name;
    public string description;
    public bool unlocked;
    public string typeName;
}
public class AchievementListController : MonoBehaviour
{
    public TextMeshProUGUI title;
    private List<AchievementRecord> achievementRecords;

    public void GetRecords()
    {
        achievementRecords = new List<AchievementRecord>();
        achievementRecords.Add(new AchievementRecord() { name = "Achievement 1", description = "Achievement 1 Description",unlocked = true, typeName = "Game" });
        achievementRecords.Add(new AchievementRecord() { name = "Achievement 2", description = "Achievement 2 Description",unlocked = true,  typeName = "Game" });
        achievementRecords.Add(new AchievementRecord() { name = "Achievement 3", description = "Achievement 3 Description",unlocked = false,  typeName = "Game" });
    }

    public void fillInAchievements(AchievementRecord record)
    {
        title.text = CapitalizeFirstLetter(record.typeName) + " Achievements";
        GameObject achievementItem = new GameObject();
        if (record.unlocked)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/AchievementItemUnlocked");
            achievementItem = Instantiate(prefab, this.transform);
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/AchievementItemLocked");
            achievementItem = Instantiate(prefab, this.transform);
        }
        achievementItem.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = record.name;
        achievementItem.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = record.description;
    }
    
    void Start()
    {
        GetRecords();
        foreach (var record in achievementRecords)
        {
            fillInAchievements(record);
        }
    }
    
    public static string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Capitalize the first letter and add the rest of the string
        return char.ToUpper(input[0]) + input.Substring(1);
    }


}
