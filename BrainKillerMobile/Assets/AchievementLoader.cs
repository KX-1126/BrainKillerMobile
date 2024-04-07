using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Network;

public class AchievementLoader : MonoBehaviour
{
    public AchievementListController gameAchievements;
    public AchievementListController scienceAchievements;
    
    private List<AchievementRecord> gameRecords = new List<AchievementRecord>();
    private List<AchievementRecord> scienceRecords = new List<AchievementRecord>();
    
    async void Start()
    {
        bool result = await GetRecords();
        if (!result)
        {
            Debug.LogError("fail to download achievements list");
            return;
        }
        
        gameAchievements.fillInAchievements(gameRecords);
        scienceAchievements.fillInAchievements(scienceRecords);
    }
    
    private async Task<bool> GetRecords()
    {
        RequestResult<string> result = await NetworkRequest.GetRequest(NetworkURL.GET_ACHIEVEMENTS);
        if (!result.Success)
        {
            Debug.LogError("get achievement list failed");
            return false;
        }
        
        print(result.Data);
        string jsonWithHeaders = "{\"records\":" + result.Data + "}";
        List<AchievementRecord> records = JsonUtility.FromJson<AchievementRecordList>(jsonWithHeaders).records;
        
        print($"fetch total {records.Count} records");
        

        foreach (var record in records)
        {
            if (record.type == "game")
            {
                gameRecords.Add(record);
            }else if (record.type == "science")
            {
                scienceRecords.Add(record);
            }
        }

        return true;
    }
}
