namespace AdventOfCode2024
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Day1.Solve();
            //Day2.Solve();
            //Day3.Solve();
            Day4.Solve();
        }
    }

    class Day1
    {
        public static void Solve()
        {
            var sampleInput = new[] { "3   4", "4   3", "2   5", "1   3", "3   9", "3   3" };

            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day1.txt");

            var ids = lines
                .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToArray())
                .Select(arr => new { left = arr[0], right = arr[1] });
            var leftIds = ids.Select(i => int.Parse(i.left)).ToList();
            leftIds.Sort();
            var rightIds = ids.Select(i => int.Parse(i.right)).ToList();
            rightIds.Sort();
            var sum = 0;
            for (int i = 0; i < leftIds.Count; i++)
            {
                sum += Math.Abs(leftIds[i] - rightIds[i]);
            }

            Console.WriteLine(sum);

            var similarity = 0;
            var previous = int.MinValue;
            var previousSimilarity = -1;
            for (var i = 0; i < leftIds.Count; i++)
            {
                var left = leftIds[i];
                if (left == previous)
                {
                    similarity += previousSimilarity;
                    continue;
                }

                var rightIndex = rightIds.BinarySearch(left);
                if (rightIndex < 0)
                {
                    previousSimilarity = -1;
                    previous = int.MinValue;
                    continue;
                }

                var before = 0;
                for (var j = rightIndex - 1; j >= 0 && rightIds[j] == rightIds[rightIndex]; j--) before++;
                var after = 0;
                for (var j = rightIndex + 1; j < rightIds.Count && rightIds[j] == rightIds[rightIndex]; j++) after++;
                var currentSimilarity = (before + 1 + after) * left;

                similarity += currentSimilarity;

                previous = left;
                previousSimilarity = currentSimilarity;
            }

            Console.WriteLine(similarity);
        }
    }

    class Day2
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "7 6 4 2 1",
                "1 2 7 8 9",
                "9 7 6 2 1",
                "1 3 2 4 5",
                "8 6 4 4 1",
                "1 3 6 7 9"
            };

            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day2.txt");

            var values = lines
                .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i.Trim())).ToArray());

            var safeCount = 0;
            foreach (var line in values)
            {
                if (IsSafe(line)) safeCount++;
            }

            Console.WriteLine(safeCount);

            var safeWithDampenerCount = 0;
            // Could be smarter, but works well enough for this input
            foreach (var line in values)
            {
                if (IsSafe(line))
                {
                    safeWithDampenerCount++;
                }
                else
                {
                    var isSafe = false;
                    for (var i = 0; i < line.Length; i++)
                    {
                        if (IsSafe(line.Where((value, index) => index != i).ToArray()))
                        {
                            isSafe = true;
                            break;
                        }
                    }

                    if (isSafe)
                    {
                        safeWithDampenerCount++;
                    }
                }
            }

            Console.WriteLine(safeWithDampenerCount);
        }

        static bool IsSafe(int[] array)
        {
            if (array.Length < 2) return true;
            var ascending = array[0] < array[1];
            for (int i = 1; i < array.Length; i++)
            {
                var diff = ascending ? (array[i] - array[i - 1]) : (array[i - 1] - array[i]);
                if (diff < 1 || diff > 3) return false;
            }

            return true;
        }
    }

    class Day3
    {
        public static void Solve()
        {
            var sampleInput = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
            var useSample = false;
            var input = useSample ? sampleInput : string.Join("", Helpers.LoadInput("day3.txt").Where(l => !string.IsNullOrEmpty(l)));

            int resultPart1 = 0;
            int resultPart2 = 0;
            var enabled = true;
            for (int i = 0; i < input.Length; i++)
            {
                if (i < input.Length - "do()".Length && input[i] == 'd' && input[i + 1] == 'o' && input[i + 2] == '(' && input[i + 3] == ')')
                {
                    enabled = true;
                    i += "do(".Length;
                    continue;
                }

                if (i < input.Length - "don't()".Length && input[i] == 'd' && input[i + 1] == 'o' && input[i + 2] == 'n' && input[i + 3] == '\'' && input[i + 4] == 't' && input[i + 5] == '(' && input[i + 6] == ')')
                {
                    enabled = false;
                    i += "do(".Length;
                    continue;
                }

                if (input[i] != 'm') continue;
                i++; if (i >= input.Length || input[i] != 'u') continue;
                i++; if (i >= input.Length || input[i] != 'l') continue;
                i++; if (i >= input.Length || input[i] != '(') continue;
                i++; if (i >= input.Length) continue;
                if (!char.IsAsciiDigit(input[i])) continue;
                var op1 = input[i] - '0';
                var op1Start = i;
                var invalid = false;
                i++;
                while (i < input.Length && char.IsAsciiDigit(input[i]))
                {
                    op1 *= 10;
                    op1 += input[i] - '0';
                    i++;
                    if (i - op1Start > 3)
                    {
                        invalid = true;
                        break;
                    }
                }

                if (invalid) continue;
                if (i >= input.Length || input[i] != ',') continue;
                i++; if (i >= input.Length) continue;
                if (!char.IsAsciiDigit(input[i])) continue;
                var op2 = input[i] - '0';
                var op2Start = i;
                invalid = false;
                i++;
                while (i < input.Length && char.IsAsciiDigit(input[i]))
                {
                    op2 *= 10;
                    op2 += input[i] - '0';
                    i++;
                    if (i - op2Start > 3)
                    {
                        invalid = true;
                        break;
                    }
                }

                if (i >= input.Length || input[i] != ')') continue;
                resultPart1 += op1 * op2;
                if (enabled)
                {
                    resultPart2 += op1 * op2;
                }
            }

            Console.WriteLine(resultPart1);
            Console.WriteLine(resultPart2);
        }
    }

    class Day4
    {
        const int PadSize = 4;
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "MMMSXXMASM",
                "MSAMXMSMSA",
                "AMXSXMAAMM",
                "MSAMASMSMX",
                "XMASAMXAMM",
                "XXAMMXXAMA",
                "SMSMSASXSS",
                "SAXAMASAAA",
                "MAMMMXMMMM",
                "MXMXAXMASX"
            };

            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day4.txt");
            var topBottomPadding = Enumerable.Range(0, PadSize).Select(_ => new string('.', lines[0].Length));
            lines = topBottomPadding.Concat(lines).Concat(topBottomPadding).ToArray();
            lines = lines.Select(l => new string('.', PadSize) + l + new string('.', PadSize)).ToArray();

            var result = 0;
            int rows = lines.Length;
            int cols = lines[0].Length;
            for (int row = PadSize; row < rows - PadSize; row++)
            {
                for (int col = PadSize; col < cols - PadSize; col++)
                {
                    if (lines[row][col] == 'X')
                    {
                        // Horizontal, left-to-right
                        if (Check(lines, row, col, 0, 1)) result++;
                        // Horizontal, right-to-left
                        if (Check(lines, row, col, 0, -1)) result++;
                        // Vertical, top-to-bottom
                        if (Check(lines, row, col, 1, 0)) result++;
                        // Vertical, bottom-to-top
                        if (Check(lines, row, col, -1, 0)) result++;
                        // Diagonal, topleft-to-bottomright
                        if (Check(lines, row, col, 1, 1)) result++;
                        // Diagonal, bottomeft-to-topright
                        if (Check(lines, row, col, -1, 1)) result++;
                        // Diagonal, topright-to-bottomleft
                        if (Check(lines, row, col, 1, -1)) result++;
                        // Diagonal, bottomright-to-topleft
                        if (Check(lines, row, col, -1, -1)) result++;
                    }
                }
            }

            Console.WriteLine(result);

            result = 0;
            for (int row = PadSize; row < rows - PadSize; row++)
            {
                for (int col = PadSize; col < cols - PadSize; col++)
                {
                    if (lines[row][col] == 'A')
                    {
                        // M's to the left
                        if (Check2(lines, row, col, mDirection: Direction.Left)) result++;
                        // M's to the right
                        if (Check2(lines, row, col, mDirection: Direction.Right)) result++;
                        // M's to the top
                        if (Check2(lines, row, col, mDirection: Direction.Top)) result++;
                        // M's to the bottom
                        if (Check2(lines, row, col, mDirection: Direction.Bottom)) result++;
                    }
                }
            }

            Console.WriteLine(result);
        }

        static bool Check(string[] lines, int row, int col, int rowDirection, int colDirection)
        {
            var result =
                lines[row + 1 * rowDirection][col + 1 * colDirection] == 'M' &&
                lines[row + 2 * rowDirection][col + 2 * colDirection] == 'A' &&
                lines[row + 3 * rowDirection][col + 3 * colDirection] == 'S';
            //if (result)
            //{
            //    Console.WriteLine($"{DirectionFromDeltas(rowDirection, colDirection)}, from ({row - PadSize},{col - PadSize})");
            //}
            return result;
        }

        enum Direction { Left, Right, Top, Bottom }
        static bool Check2(string[] lines, int row, int col, Direction mDirection)
        {
            switch (mDirection)
            {
                case Direction.Left:
                    return lines[row - 1][col - 1] == 'M' &&
                        lines[row + 1][col - 1] == 'M' &&
                        lines[row - 1][col + 1] == 'S' &&
                        lines[row + 1][col + 1] == 'S';
                case Direction.Right:
                    return lines[row - 1][col + 1] == 'M' &&
                        lines[row + 1][col + 1] == 'M' &&
                        lines[row - 1][col - 1] == 'S' &&
                        lines[row + 1][col - 1] == 'S';
                case Direction.Top:
                    return lines[row - 1][col - 1] == 'M' &&
                        lines[row - 1][col + 1] == 'M' &&
                        lines[row + 1][col - 1] == 'S' &&
                        lines[row + 1][col + 1] == 'S';
                case Direction.Bottom:
                    return lines[row + 1][col - 1] == 'M' &&
                        lines[row + 1][col + 1] == 'M' &&
                        lines[row - 1][col - 1] == 'S' &&
                        lines[row - 1][col + 1] == 'S';
            }

            return false;
        }

        static string DirectionFromDeltas(int rowDelta, int colDelta)
        {
            if (rowDelta == 1 && colDelta == 0) return "top-to-bottom";
            if (rowDelta == -1 && colDelta == 0) return "bottom-to-top";
            if (rowDelta == 0 && colDelta == 1) return "left-to-right";
            if (rowDelta == 0 && colDelta == -1) return "right-to-left";
            if (rowDelta == 1 && colDelta == 1) return "topleft-to-bottomright";
            if (rowDelta == 1 && colDelta == -1) return "topright-to-bottomleft";
            if (rowDelta == -1 && colDelta == 1) return "bottomleft-to-topright";
            if (rowDelta == -1 && colDelta == -1) return "bottomright-to-topleft";
            return "unknown";
        }
    }

    public class Helpers
    {
        public static string[] LoadInput(string fileName)
        {
            return File.ReadAllLines(Path.Combine("inputs", fileName));
        }
    }

}
