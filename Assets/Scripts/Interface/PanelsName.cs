public enum PanelsName
{
    MainMenu,
    Settings,
    KeyBinds,
    Loading,
}

public static class PanelsNameExtensions
{
    public static string GetPanelName(this PanelsName name)
    {
        return $"{name} Panel";
    }
}