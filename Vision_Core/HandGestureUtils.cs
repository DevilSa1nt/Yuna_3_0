using System;
using System.Collections.Generic;
using System.Linq;

namespace Vision_Core
{
    public static class HandGestureUtils
    {
        public static bool IsThumbsUp(List<HandLandmark> points)
        {
            var thumbTip = points[4];
            var indexTip = points[8];
            return thumbTip.Y < indexTip.Y && Math.Abs(thumbTip.X - indexTip.X) < 0.1;
        }

        public static string DetectCustomGesture(List<HandLandmark> points)
        {
            if (IsThumbsUp(points)) return "👍 Thumbs Up";
            return "✋ Unknown Gesture";
        }
    }
}
