using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct AchievementRecord
{
    public string name;
    public string description;
    [FormerlySerializedAs("unlocked")] public bool unlock;
    public string type;
}

//"name":"Flip Master","description":"Successfully finished 10 flip levels","unlock":true,"type":"game"

[System.Serializable]
public struct AchievementRecordList
{
    public List<AchievementRecord> records;
}
public class AchievementListController : MonoBehaviour
{
    public TextMeshProUGUI title;
    
    public void fillInAchievements(List<AchievementRecord> records)
    {
        foreach (var record in records)
        {
            fillInAchievement(record);
        }
    }

    private void fillInAchievement(AchievementRecord record)
    {
        title.text = CapitalizeFirstLetter(record.type) + " Achievements";
        GameObject achievementItem = new GameObject();
        if (record.unlock)
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
