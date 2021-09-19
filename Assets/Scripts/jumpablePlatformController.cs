using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpablePlatformController : MonoBehaviour
{
    [SerializeField]
    private bool isNeedToBeJump = false;
    [SerializeField]
    private List<string> passedPlayers;
    public bool isBackward;

    private void Start()
    {
        passedPlayers = new List<string>();
    }
    public bool IsNeedToBeJump { get => isNeedToBeJump; }

    public void addMeToList(string player) 
    {
        if (passedPlayers.Contains(player))
            return;
        passedPlayers.Add(player);
    }
    public void deleteFromList(string player) 
    {
        if (!passedPlayers.Contains(player))
            return;
        passedPlayers.Remove(player);
    }
    public bool isContainsPlayer(string player) 
    {
        return passedPlayers.Contains(player);
    }
}
