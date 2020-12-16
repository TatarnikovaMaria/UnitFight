using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text infoText;
    public Text teamPCount;
    public Text teamZCount;

    public static UIManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            this.enabled = false;
    }
    public void ShowInfoText(string text)
    {
        if (infoText != null)
        {
            infoText.text = text;
            infoText.enabled = true;
        }
    }

    public void HideInfoText()
    {
        infoText.enabled = false;
    }

    public void SetTeamCount(UnitTeams team, int count)
    {
        if(team == UnitTeams.people)
        {
            if (teamPCount != null)
            {
                teamPCount.text = count.ToString();
            }
        }
        else if(teamZCount != null)
        {
            teamZCount.text = count.ToString();
        }
    }
}
