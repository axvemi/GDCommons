using Godot;
using System;

namespace Axvemi;
public static class Logger
{
    public static void LogInfo(string msg, string category = "")
    {
        string result = string.Join(" ", "[AXVEMI -", category + "]", "(" + DateTime.Now + ")", msg);
        GD.Print(result);
    }

    public static void LogWarning(string msg, string category = "")
    {
        string result = string.Join(" ", "[WARNING]", "[AXVEMI -", category + "]", "(" + DateTime.Now + ")", msg);
        GD.Print(result);
    }

    public static void LogError(string msg, string category = "")
    {
        string result = string.Join(" ", "[ERROR]", "[AXVEMI -", category + "]", "(" + DateTime.Now + ")", msg);
        GD.PrintErr(result);
    }
}
