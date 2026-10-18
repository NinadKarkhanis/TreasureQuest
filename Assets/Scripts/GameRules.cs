using System;

public static class GameRules
{
    public static int CalculateScore(int coins, int enemies, int penalties)
    {
        long total = (long)Math.Max(0, coins) * 10L + (long)Math.Max(0, enemies) * 100L - Math.Max(0, penalties);
        return (int)Math.Min(int.MaxValue, Math.Max(0L, total));
    }

    public static bool ValidLeaderboardEntry(string name, string scoreText, out int score)
    {
        score = 0;
        return !string.IsNullOrWhiteSpace(name) && name.Trim().Length >= 2 &&
               name.Trim().Length <= 18 && int.TryParse(scoreText, out score) && score >= 0;
    }
}
