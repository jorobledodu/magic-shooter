using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Dan.Main;

public class Leaderboard_Controller : MonoBehaviour
{
    public static Leaderboard_Controller Instance { get; private set; }

    public List<TextMeshProUGUI> ranking;
    public List<TextMeshProUGUI> usernames;
    public List<TextMeshProUGUI> scores;

    private void Start()
    {
        #region Singleton
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        #endregion


        LoadEntries();
    }

    public void LoadEntries()
    {
        Leaderboards.MagicShooterLeaderboard.GetEntries(entries =>
        {
            foreach (TextMeshProUGUI rank in ranking)
            {
                rank.text = "";
            }

            foreach (TextMeshProUGUI username in usernames)
            {
                username.text = "";
            }

            foreach (TextMeshProUGUI score in scores)
            {
                score.text = "";
            }

            float length = Mathf.Min(usernames.Count, entries.Length);

            for (int i = 0; i < length; i++)
            {
                ranking[i].text = entries[i].Rank.ToString();
                usernames[i].text = entries[i].Username;
                scores[i].text = entries[i].Score.ToString();
            }
        });
    }

    public void SetEntry(string username, int score)
    {
        Leaderboards.MagicShooterLeaderboard.UploadNewEntry(username, score, isSuccessful =>
        {
            if (isSuccessful)
            {
                LoadEntries();
            }
            else
            {
                Debug.Log("Server Error");
                return;
            }
        });
    }
}
