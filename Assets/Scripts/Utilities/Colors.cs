using UnityEngine;

namespace Utilities
{
    public static class Colors
    {
        public static readonly Color backgroundColor;
        public static readonly Color topAppBarBackgroundColor;

        public static readonly Color solvedTileColor;
        public static readonly Color unsolvedTileColor;

        public static readonly Color[] cellColors = new Color[4];

        static Colors()
        {
            ColorUtility.TryParseHtmlString("#101616", out backgroundColor);
            ColorUtility.TryParseHtmlString("#212727", out topAppBarBackgroundColor);

            ColorUtility.TryParseHtmlString("#34739f", out solvedTileColor);
            ColorUtility.TryParseHtmlString("#223440", out unsolvedTileColor);

            ColorUtility.TryParseHtmlString("#4285F4", out cellColors[0]); // blue
            ColorUtility.TryParseHtmlString("#34A853", out cellColors[1]); // green
            ColorUtility.TryParseHtmlString("#FBBC05", out cellColors[2]); // yellow
            ColorUtility.TryParseHtmlString("#EA4335", out cellColors[3]); // red
        }
    }
}
