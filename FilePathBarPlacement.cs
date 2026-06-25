using Microsoft.VisualStudio.Text.Editor;

namespace FilePathBar;

internal static class FilePathBarPlacement
{
    public const string Top = PredefinedMarginNames.Top;
    public const string Bottom = PredefinedMarginNames.Bottom;

    // Change this constant to Bottom to place FilePathBar below the editor surface.
    public const string Current = Top;

    public static bool IsTop => Current == Top;
}
