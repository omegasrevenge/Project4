using System;

[Flags]
public enum TutorialStep : int
{
    InitGame = 1,
    NextFight = 2,
    FirstLevelUp = 4,
    SecondCrafting = 8,
    FirstNCEPlayer = 16,
    FirstVENGEAPlayer = 32,
    UsedDriod = 64,
    SecondLevelUp = 128,
    FirstTimeNCE = 256,
    None = 0
}
public static class Tutorial
{
    public static bool IsFlagSet(this TutorialStep value, TutorialStep flag)
    {
        return flag == (value & flag);
    }
    public static bool IsAnyFlagSet(this TutorialStep value, TutorialStep flag)
    {
        return 0 != (value & flag);
    }
    public static TutorialStep SetFlag(this TutorialStep value, TutorialStep flag)
    {
        return value | flag;
    }
    public static TutorialStep SetFlag(this TutorialStep value, TutorialStep flag, bool on)
    {
        if (on)
            return value | flag;
        else
            return value & ~flag;
    }
}
