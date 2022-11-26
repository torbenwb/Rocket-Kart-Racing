using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Leaderboard 
{
    const string LB_ENTRY_COUNT_TAG = "LB_ENTRY_COUNT";
    const string LB_ENTRY_TAG = "LG_ENTRY_";


    public static void NewLeaderboardEntry(float score){
        int leaderboardEntries = PlayerPrefs.GetInt(LB_ENTRY_COUNT_TAG, 0);
        leaderboardEntries++;
        PlayerPrefs.SetFloat(LeaderboardEntryTag(leaderboardEntries),score);
        PlayerPrefs.SetInt(LB_ENTRY_COUNT_TAG, leaderboardEntries);
    }

    public static void Save(){
        PlayerPrefs.Save();
    }

    public static string LeaderboardEntryTag(int num){
        return $"{LB_ENTRY_TAG}{num}";
    }

    public static void PrintLB(){
        Debug.Log("Print Leaderboard");
        int leaderboardEntries = PlayerPrefs.GetInt(LB_ENTRY_COUNT_TAG);
        List<float> entries = new List<float>();
        for (int i = 0; i < leaderboardEntries; i++){
            entries.Add(PlayerPrefs.GetFloat(LeaderboardEntryTag(i)));
        }

        entries.Sort();
        for(int i = 0; i < entries.Count; i++){
            Debug.Log(entries[i]);
        }
    }
}
