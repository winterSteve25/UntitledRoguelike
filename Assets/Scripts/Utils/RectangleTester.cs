using UnityEngine;

namespace Utils
{
    public class RectangleTester
    {
        public static bool InBound(Vector2Int size, Vector2Int position, int x, int y, bool inclusive, bool upsideDown)
        {
            return InBound(size, position, x, y, 0, 0, inclusive, upsideDown);
        }

        public static bool InBound(Vector2Int size, Vector2Int position, int x, int y, int width, int height,
            bool inclusive, bool upsideDown)
        {
            if (upsideDown)
            {
                return InBoundUpsideDown(size, position, x, y, width, height, inclusive);
            }

            if (inclusive)
                return x >= position.x && x + width <= position.x + size.x && y >= position.y &&
                       y + height <= position.y + size.y;

            return x >= position.x && x + width < position.x + size.x && y >= position.y &&
                   y + height < position.y + size.y;
        }

        private static bool InBoundUpsideDown(Vector2Int size, Vector2Int position, int x, int y, int width, int height,
            bool inclusive)
        {
            if (inclusive)
            {
                return x + 1 <= position.x + size.x && x + 1 - width >= position.x
                                                && y + 1 <= position.y + size.y && y + 1 - height >= position.y;
            }

            return x + 1 < position.x + size.x && x + 1 - width >= position.x
                                            && y + 1 < position.y + size.y && y + 1 - height >= position.y;
        }

        public static bool AreRectanglesOverlapping(int x1, int y1, int width1, int height1,
            int x2, int y2, int width2, int height2)
        {
            // Rectangle 1 bounds
            int left1 = x1;
            int right1 = x1 + width1;
            int bottom1 = y1;
            int top1 = y1 + height1;

            // Rectangle 2 bounds
            int left2 = x2;
            int right2 = x2 + width2;
            int bottom2 = y2;
            int top2 = y2 + height2;

            // If one rectangle is to the left of the other
            if (right1 <= left2 || right2 <= left1)
                return false;

            // If one rectangle is below the other
            if (top1 <= bottom2 || top2 <= bottom1)
                return false;

            // Rectangles overlap
            return true;
        }
    }
}