#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

// Self-contained Edit Mode smoke tests: Tools > Treasure Quest > Run Logic Tests.
// No external CI secrets or third-party test runner configuration needed.
public static class GameRulesSmokeTests
{
    [MenuItem("Tools/Treasure Quest/Run Logic Tests")]
    public static void Run()
    {
        MustEqual(0, GameRules.CalculateScore(0, 0, 0), "starting score");
        MustEqual(120, GameRules.CalculateScore(2, 1, 0), "coin + enemy score");
        MustEqual(20, GameRules.CalculateScore(2, 1, 100), "death penalty");
        MustEqual(0, GameRules.CalculateScore(0, 0, 500), "nonnegative score");
        MustEqual(int.MaxValue, GameRules.CalculateScore(int.MaxValue, int.MaxValue, 0), "overflow handling");
        int score;
        Must(GameRules.ValidLeaderboardEntry("Runner", "100", out score) && score == 100, "valid leaderboard entry");
        Must(!GameRules.ValidLeaderboardEntry("", "100", out score), "empty username rejected");
        Must(!GameRules.ValidLeaderboardEntry("Runner", "NaN", out score), "invalid score rejected");
        Must(!GameRules.ValidLeaderboardEntry("Runner", "-1", out score), "negative score rejected");
        Debug.Log("Treasure Quest: all 9 game-rule smoke tests passed.");
    }

    private static void Must(bool condition, string label)
    {
        if (!condition) throw new Exception("Game-rule test failed: " + label);
    }
    private static void MustEqual(int expected, int actual, string label)
    {
        Must(expected == actual, label + " expected " + expected + ", got " + actual);
    }
}
#endif
