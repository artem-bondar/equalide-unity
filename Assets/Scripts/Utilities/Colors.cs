using UnityEngine;

namespace Utilities
{
    public static class Colors
    {
        public static readonly Color backgroundColor;
        public static readonly Color topAppBarBackgroundColor;

        public static readonly Color solvedTileColor;
        public static readonly Color unsolvedTileColor;

        public static readonly Color[] cellColors = new Color[3];

        static Colors()
        {
            ColorUtility.TryParseHtmlString("#101616", out backgroundColor);
            ColorUtility.TryParseHtmlString("#212727", out topAppBarBackgroundColor);

            ColorUtility.TryParseHtmlString("#34739f", out solvedTileColor);
            ColorUtility.TryParseHtmlString("#223440", out unsolvedTileColor);

            ColorUtility.TryParseHtmlString("#E120F0", out cellColors[0]); // violet
            ColorUtility.TryParseHtmlString("#00F3F9", out cellColors[1]); // cyan
            ColorUtility.TryParseHtmlString("#FF016D", out cellColors[2]); // pink
        }
    }
}
