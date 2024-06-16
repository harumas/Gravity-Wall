using System;

namespace StageEditor
{
    public static class SpanExtension
    {
        public static void Sort<T>(this Span<T> span) where T : IComparable<T>
        {
            QuickSortInternal(span, 0, span.Length - 1);
        }

        private static void QuickSortInternal<T>(Span<T> span, int low, int high) where T : IComparable<T>
        {
            if (low < high)
            {
                int pivotIndex = Partition(span, low, high);
                QuickSortInternal(span, low, pivotIndex - 1);
                QuickSortInternal(span, pivotIndex + 1, high);
            }
        }

        private static int Partition<T>(Span<T> span, int low, int high) where T : IComparable<T>
        {
            T pivot = span[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (span[j].CompareTo(pivot) <= 0)
                {
                    i++;
                    Swap(span, i, j);
                }
            }

            Swap(span, i + 1, high);
            return i + 1;
        }

        private static void Swap<T>(Span<T> span, int i, int j)
        {
            if (i != j)
            {
                (span[i], span[j]) = (span[j], span[i]);
            }
        }
    }
}