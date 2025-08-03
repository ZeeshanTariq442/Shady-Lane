using System.Collections.Generic;
using UnityEngine;

public class RandomNumberGenerator
{
    private List<int> numbers = new List<int>();
    private int currentIndex = 0;

    public RandomNumberGenerator(int min, int max)
    {
        
        numbers.Clear();
        for (int i = min; i <= max; i++)
        {
            numbers.Add(i);
        }

        for (int i = 0; i < numbers.Count; i++)
        {
            int randomIndex = Random.Range(0, numbers.Count - 1);
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;


        }

        currentIndex = 0;
    }

    public int GetNextUniqueNumber()
    {
        if (currentIndex >= numbers.Count)
        {
            Debug.LogWarning("No more unique numbers available.");
            return 0; 
        }
        return numbers[currentIndex++];
    }
}
