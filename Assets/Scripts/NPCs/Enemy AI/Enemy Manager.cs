using System.Collections.Generic;

public static class EnemyManager
{
    public static HashSet<string> DeadEnemies = new HashSet<string>();

    public static void MarkDead(string enemyID)
    {
        if (!string.IsNullOrEmpty(enemyID))
        {
            DeadEnemies.Add(enemyID);
        }
    }

    public static void Clear()
    {
        DeadEnemies.Clear();
    }
}