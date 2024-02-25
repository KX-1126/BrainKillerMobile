using UnityEngine;
using System;

namespace Utilities
{
    public class GridLayoutSizeCalculator
    {
        public static Vector2 CalculateGridLayoutSize(Vector2 parentSize, int rowCount, int columnCount, int spacing, int padding)
        {
            int width = (int) (parentSize.x - (columnCount + 1) * spacing - 2 * padding) / columnCount;
            int height = (int) (parentSize.y - (rowCount + 1) * spacing - 2 * padding) / rowCount;
            return new Vector2(width, height);
        }
        
        public static Vector2 CalculateGridMinSquareSize(Vector2 parentSize, int rowCount, int columnCount, int spacing, int padding)
        {
            Vector2 size = CalculateGridLayoutSize(parentSize, rowCount, columnCount, spacing, padding);
            int length = (int) Mathf.Min(size.x, size.y);
            return new Vector2(length, length);
        }
    }
}