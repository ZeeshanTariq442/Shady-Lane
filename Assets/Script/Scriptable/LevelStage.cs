using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelDesign
{
    public LevelData[] task;
    [Range(1,20)]
    public int trails;
    [Range(1, 20)]
    public int countdownTime;
    [Range(1, 300)]
    public int itemsTurn;
}
[System.Serializable]
public class LevelData
{
    public int totalItems;
    public GameManager.ItemColor[] bottleColors;
    public int trail;
    public bool showWrongTurn;
    public GameManager.Items ItemName;
}


[CreateAssetMenu(fileName = "NewLevel", menuName = "Level Design/Level Data")]
public class LevelStage : ScriptableObject
{
    public LevelDesign[] levels;

    #region Checking Basket & Items Index
    public Dictionary<int, int> GetUniqueItemCountsPerLevel()
    {
        Dictionary<int, int> uniqueItemCounts = new Dictionary<int, int>();

        for (int i = 0; i < levels.Length; i++)
        {
            int uniqueCount = 0;
            HashSet<string> uniqueItems = new HashSet<string>();
            foreach (var task in levels[i].task)
            {
                if (!uniqueItems.Contains(task.ItemName.ToString()))
                    uniqueItems.Add(task.ItemName.ToString());

                uniqueCount = uniqueItems.Count;
            }

            uniqueItemCounts[i] = uniqueCount;
        }

        return uniqueItemCounts;
    }

    public Dictionary<int, int> GetEachUniqueItemCountsPerLevel(int currentLevel)
    {
        Dictionary<int, int> uniqueItemCounts = new Dictionary<int, int>();
        HashSet<string> uniqueItems = new HashSet<string>();

        for (int i = 0; i < GetUniqueItemCountsPerLevel()[currentLevel]; i++)
        {
            int uniqueCount = 0;
            List<string> items = new List<string>();
            foreach (var task in levels[currentLevel].task)
            {
               
                if ((items.Contains(task.ItemName.ToString()) || items.Count == 0) && !uniqueItems.Contains(task.ItemName.ToString()))
                {
                    items.Add(task.ItemName.ToString());
                }
                uniqueCount = items.Count;
            }
            uniqueItems.Add(items[0]);
            uniqueItemCounts[i] = uniqueCount;
        }

        return uniqueItemCounts;
    }

    public List<GameManager.Items> GetItemImageIndex(int currentLevel)
    {
        HashSet<GameManager.Items> uniqueItems = new HashSet<GameManager.Items>();
        for (int i = 0; i < levels.Length; i++)
        {
            foreach (var task in levels[currentLevel].task)
            {
                uniqueItems.Add(task.ItemName);
            }
            
        }
        List<GameManager.Items> numberList = new List<GameManager.Items>(uniqueItems);
        return numberList;
    }

    public Dictionary<int,int> GetEachUniqueItemIndex(int currentLevel)
    {
        Dictionary<int, int> uniqueItemCounts = new Dictionary<int,int>();

        for (int i = 0; i < GetUniqueItemCountsPerLevel()[currentLevel]; i++)
        {
            List<string> items = new List<string>();
            foreach (var task in levels[currentLevel].task)
            {
                if (!items.Contains(task.ItemName.ToString()) || items.Count == 0)
                {
                    items.Add(task.ItemName.ToString());
                    uniqueItemCounts[i] = i;
                }
               
            }
           
        }
        return uniqueItemCounts;
    }

    #endregion
}
