using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelType { Normal, Shop, Arena }

public class DataManager : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private PlayerData defaultPlayer;
    [SerializeField] private int maxRooms = 7;

    [Header("Dynamic Data")]
    [SerializeField] private PlayerData player;
    [SerializeField] private int roomNumber = 1;
    [SerializeField] private int stageNumber = 1;
    [SerializeField] private LevelType currentLevelType;

    public static DataManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (DataManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public void CreateNewPlayer()
    {
        // Set player to a copy of the template
        player = (PlayerData)defaultPlayer.Copy();

        // Reset progess
        stageNumber = 1;
        roomNumber = 1;
        currentLevelType = LevelType.Normal;
    }

    public PlayerData GetPlayer()
    {
        return player;
    }

    public void SetNextRoom()
    {
        // Increment room number
        roomNumber++;
        if (roomNumber > maxRooms)
        {
            stageNumber++;
            roomNumber = 1;
        }

        // Shops on Room 3 and 6
        if (roomNumber == 3 || roomNumber == 6)
        {
            currentLevelType = LevelType.Shop;
        }
        // Boss on Room 7
        else if (roomNumber == maxRooms)
        {
            currentLevelType = LevelType.Arena;
        }
        else
        {
            currentLevelType = LevelType.Normal;
        }
    }

    public LevelType GetCurrentRoom()
    {
        return currentLevelType;
    }

    public string GetRoomDescription()
    {
        return "Stage " + stageNumber + " - " + roomNumber;
    }

    public int GetRoomNumber()
    {
        return roomNumber;
    }

    public LevelType GetNextRoom()
    {
        // Shops on Room 3 and 6
        int next = roomNumber + 1;
        if (next == 3 || next == 6)
        {
            return LevelType.Shop;
        }
        else if (next == maxRooms) // Boss on Room 7
        {
            return LevelType.Arena;
        }
        else
        {
            return LevelType.Normal;
        }
    }
}
