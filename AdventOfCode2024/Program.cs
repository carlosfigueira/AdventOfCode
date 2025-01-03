﻿using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Day1.Solve();
            //Day2.Solve();
            //Day3.Solve();
            //Day4.Solve();
            //Day5.Solve();
            //Day6.Solve();
            //Day7.Solve();
            //Day8.Solve();
            //Day9.Solve();
            //Day10.Solve();
            //Day11.Solve();
            //Day12.Solve();
            //Day13.Solve();
            //Day14.Solve();
            //Day15.Solve();
            //Day16.Solve();
            //Day17.Solve();
            //Day18.Solve();
            //Day19.Solve();
            //Day20.Solve();
            Day22.Solve();
            //Day23.Solve();
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

    class Day5
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "47|53",
                "97|13",
                "97|61",
                "97|47",
                "75|29",
                "61|13",
                "75|53",
                "29|13",
                "97|29",
                "53|29",
                "61|53",
                "97|53",
                "61|29",
                "47|13",
                "75|47",
                "97|75",
                "47|61",
                "75|61",
                "47|29",
                "75|13",
                "53|13",
                "",
                "75,47,61,53,29",
                "97,61,53,29,13",
                "75,29,13",
                "75,97,47,61,53",
                "61,13,29",
                "97,13,75,29,47"
            };

            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day5.txt");
            var mustBeBefore = new Dictionary<int, HashSet<int>>();
            var orderPairs = new List<Tuple<int, int>>();
            var i = 0;
            for (; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    // End of first part of input
                    i++;
                    break;
                }

                var values = lines[i].Split('|').Select(v => int.Parse(v)).ToArray();
                var before = values[0];
                var after = values[1];
                orderPairs.Add(Tuple.Create(before, after));
                if (mustBeBefore.TryGetValue(before, out var afters))
                {
                    afters.Add(after);
                }
                else
                {
                    mustBeBefore.Add(before, new HashSet<int> { after });
                }
            }

            var tests = new List<int[]>();
            for (; i < lines.Length; i++)
            {
                tests.Add(lines[i].Split(',').Select(n => int.Parse(n)).ToArray());
            }

            var part1Result = 0;
            foreach (var test in tests)
            {
                if (IsValidPart1(test, mustBeBefore))
                {
                    part1Result += GetMiddleValue(test);
                }
            }

            Console.WriteLine(part1Result);

            var part2Result = 0;
            foreach (var test in tests)
            {
                if (IsValidPart2(test, mustBeBefore, out var middleValue))
                {
                    part2Result += middleValue;
                }
            }

            Console.WriteLine(part2Result);
        }

        static int GetMiddleValue(int[] pages)
        {
            return pages[pages.Length / 2];
        }

        static bool IsValidPart1(int[] pages, Dictionary<int, HashSet<int>> orderingRules)
        {
            for (int i = 1; i < pages.Length; i++)
            {
                var after = pages[i];
                if (!orderingRules.ContainsKey(after)) continue;
                for (int j = 0; j < i; j++)
                {
                    var before = pages[j];
                    if (orderingRules[after].Contains(before)) return false;
                }
            }

            return true;
        }

        static bool IsValidPart2(int[] pages, Dictionary<int, HashSet<int>> orderingRules, out int middleNumber)
        {
            var trimmedRules = new Dictionary<int, HashSet<int>>();
            foreach (var before in orderingRules.Keys)
            {
                if (!pages.Contains(before)) continue;
                var after = orderingRules[before].Where(n => pages.Contains(n)).ToArray();
                if (after.Length == 0) continue;
                trimmedRules.Add(before, new HashSet<int>(after));
            }

            var changed = false;
            for (int i = 0; i < pages.Length - 1; i++)
            {
                var changeMade = false;
                for (int j = i + 1; j < pages.Length; j++)
                {
                    if (trimmedRules.TryGetValue(pages[j], out var afters) && afters.Contains(pages[i]))
                    {
                        changed = true;
                        changeMade = true;
                        var temp = pages[i]; pages[i] = pages[j]; pages[j] = temp;
                        break;
                    }
                }

                if (changeMade)
                {
                    // Try again
                    i--;
                    continue;
                }
            }

            middleNumber = 0;
            if (changed)
            {
                middleNumber = GetMiddleValue(pages);
            }

            return changed;
        }
    }

    class Day6
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "....#.....",
                ".........#",
                "..........",
                "..#.......",
                ".......#..",
                "..........",
                ".#..^.....",
                "........#.",
                "#.........",
                "......#..."
            };
            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day6.txt");
            int startRow = -1, startCol = -1;
            var rows = lines.Length;
            var cols = lines[0].Length;
            char direction = '^';

            var map = new char[rows, cols];
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    map[r, c] = lines[r][c];
                }
            }

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    if (map[r, c] == '^')
                    {
                        startRow = r;
                        startCol = c;
                        break;
                    }
                }

                if (startRow >= 0) break;
            }

            var row = startRow;
            var col = startCol;
            var visited = new HashSet<int>();
            visited.Add(CoordsToValue(row, col));
            var endOfMap = false;
            while (!endOfMap)
            {
                if (TryMove(map, rows, cols, ref row, ref col, ref direction, ref endOfMap))
                {
                    visited.Add(CoordsToValue(row, col));
                }
            }

            Console.WriteLine(visited.Count);

            var possiblePlaces = 0;
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    if (map[r, c] != '.') continue;
                    var visited2 = new HashSet<(int row, int col, char direction)>();
                    row = startRow;
                    col = startCol;
                    direction = '^';
                    visited2.Add((row, col, direction));
                    map[r, c] = '#';
                    endOfMap = false;
                    while (!endOfMap)
                    {
                        if (TryMove(map, rows, cols, ref row, ref col, ref direction, ref endOfMap))
                        {
                            if (visited2.Contains((row, col, direction)))
                            {
                                // Added a loop
                                possiblePlaces++;
                                break;
                            }

                            visited2.Add((row, col, direction));
                        }
                    }

                    map[r, c] = '.';
                }
            }

            Console.WriteLine(possiblePlaces);
        }

        static int CoordsToValue(int row, int col)
        {
            return row * 1000 + col;
        }

        static char ChangeDirection(char direction)
        {
            switch (direction)
            {
                case '^': return '>';
                case '>': return 'v';
                case 'v': return '<';
                default: return '^';
            }
        }

        static bool TryMove(char[,] map, int rows, int cols, ref int row, ref int col, ref char direction, ref bool endOfMap)
        {
            endOfMap = false;
            switch (direction)
            {
                case '^':
                    if (row == 0)
                    {
                        endOfMap = true;
                        return false;
                    }

                    if (map[row - 1, col] == '#')
                    {
                        direction = ChangeDirection(direction);
                        return false;
                    }

                    row--;
                    return true;
                case 'v':
                    if (row == rows - 1)
                    {
                        endOfMap = true;
                        return false;
                    }

                    if (map[row + 1, col] == '#')
                    {
                        direction = ChangeDirection(direction);
                        return false;
                    }

                    row++;
                    return true;
                case '<':
                    if (col == 0)
                    {
                        endOfMap = true;
                        return false;
                    }

                    if (map[row, col - 1] == '#')
                    {
                        direction = ChangeDirection(direction);
                        return false;
                    }

                    col--;
                    return true;
                case '>':
                    if (col == cols - 1)
                    {
                        endOfMap = true;
                        return false;
                    }

                    if (map[row, col + 1] == '#')
                    {
                        direction = ChangeDirection(direction);
                        return false;
                    }

                    col++;
                    return true;
            }

            return false;
        }
    }

    class Day7
    {
        class LineInput
        {
            public long TestValue { get; set; }
            public long[] Operands { get; set; }

            public static LineInput Parse(string line)
            {
                var parts = line.Split(':');
                var testValue = long.Parse(parts[0].Trim());
                var operands = parts[1].Trim().Split(' ').Select(n => long.Parse(n)).ToArray();
                return new LineInput { TestValue = testValue, Operands = operands };
            }
        }

        static bool[] ToBitArray(long l, int size)
        {
            var result = new bool[size];
            for (var i = 0; i < size; i++)
            {
                if ((l & 1) != 0)
                {
                    result[i] = true;
                }

                l /= 2;
            }

            return result;
        }

        static int[] ToNAry(long l, int baseN, int size)
        {
            var result = new int[size];
            for (var i = 0; i < size; i++)
            {
                result[i] = (int)(l % baseN);
                l /= baseN;
            }

            return result;
        }

        public static void Solve()
        {
            var sampleInput = new[]
            {
                "190: 10 19",
                "3267: 81 40 27",
                "83: 17 5",
                "156: 15 6",
                "7290: 6 8 6 15",
                "161011: 16 10 13",
                "192: 17 8 14",
                "21037: 9 7 18 13",
                "292: 11 6 16 20"
            };
            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day7.txt");
            var inputs = lines.Select(l => LineInput.Parse(l)).ToArray();

            long totalCalibration = 0;
            foreach (var input in inputs)
            {
                var operatorCount = input.Operands.Length - 1;
                var totalPossibleOperators = 1 << operatorCount;
                for (var possibleOperators = 0; possibleOperators < totalPossibleOperators; possibleOperators++)
                {
                    long currentValue = input.Operands[0];
                    var operations = ToNAry(possibleOperators, 2, operatorCount);
                    for (var i = 0; i < operations.Length; i++)
                    {
                        if (operations[i] == 0)
                        {
                            currentValue += input.Operands[i + 1];
                        }
                        else
                        {
                            currentValue *= input.Operands[i + 1];
                        }

                        if (currentValue > input.TestValue)
                        {
                            break;
                        }
                    }

                    if (currentValue == input.TestValue)
                    {
                        totalCalibration += input.TestValue;
                        break;
                    }
                }
            }

            Console.WriteLine(totalCalibration);

            totalCalibration = 0;
            foreach (var input in inputs)
            {
                var operatorCount = input.Operands.Length - 1;
                var totalPossibleOperators = 1;
                for (var i = 0; i < operatorCount; i++) totalPossibleOperators *= 3;
                for (var possibleOperators = 0; possibleOperators < totalPossibleOperators; possibleOperators++)
                {
                    long currentValue = input.Operands[0];
                    var operations = ToNAry(possibleOperators, 3, operatorCount);
                    for (var i = 0; i < operations.Length; i++)
                    {
                        if (operations[i] == 0)
                        {
                            currentValue += input.Operands[i + 1];
                        }
                        else if (operations[i] == 1)
                        {
                            currentValue *= input.Operands[i + 1];
                        }
                        else
                        {
                            currentValue = long.Parse(currentValue.ToString() + input.Operands[i + 1].ToString());
                        }

                        if (currentValue > input.TestValue)
                        {
                            break;
                        }
                    }

                    if (currentValue == input.TestValue)
                    {
                        totalCalibration += input.TestValue;
                        break;
                    }
                }
            }

            Console.WriteLine(totalCalibration);
        }
    }

    class Day8
    {
        [DebuggerDisplay("{Row},{Col}")]
        class Coord
        {
            public int Row { get; set; }
            public int Col { get; set; }

            public override int GetHashCode()
            {
                return this.Row * 100 + this.Col;
            }

            public override bool Equals(object? obj)
            {
                return obj is Coord other && this.Row == other.Row && this.Col == other.Col;
            }

            public Coord Distance(Coord other)
            {
                return new Coord { Row = other.Row - this.Row, Col = other.Col - this.Col };
            }

            public Coord Add(Coord other, int multiplier)
            {
                return new Coord { Row = this.Row + other.Row * multiplier, Col = this.Col + other.Col * multiplier };
            }
        }

        public static void Solve()
        {
            var sampleInput = new[]
            {
                "............",
                "........0...",
                ".....0......",
                ".......0....",
                "....0.......",
                "......A.....",
                "............",
                "............",
                "........A...",
                ".........A..",
                "............",
                "............"
            };
            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day8.txt");

            var types = new Dictionary<char, List<Coord>>();
            var rows = lines.Length;
            var cols = lines[0].Length;
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    var type = lines[row][col];
                    if (type == '.') continue;
                    if (types.TryGetValue(type, out var positions))
                    {
                        positions.Add(new Coord { Row = row, Col = col });
                    }
                    else
                    {
                        types.Add(type, new List<Coord> { new Coord { Row = row, Col = col } });
                    }
                }
            }

            var antiNodes = new HashSet<Coord>();
            bool isValid(Coord coord)
            {
                return coord.Row >= 0 && coord.Col >= 0 && coord.Row < rows && coord.Col < cols;
            }
            void add(Coord coord)
            {
                if (!antiNodes.Contains(coord))
                {
                    antiNodes.Add(coord);
                }
            }
            void addIfValid(Coord coord)
            {
                if (isValid(coord))
                {
                    add(coord);
                }
            }

            foreach (var type in types.Keys)
            {
                var antennas = types[type];
                for (var i = 0; i < antennas.Count - 1; i++)
                {
                    for (var j = i + 1; j < antennas.Count; j++)
                    {
                        var dist = antennas[i].Distance(antennas[j]);
                        var firstAntiNode = antennas[j].Add(dist, 1);
                        var secondAntiNode = antennas[i].Add(dist, -1);
                        addIfValid(firstAntiNode);
                        addIfValid(secondAntiNode);
                    }
                }
            }

            Console.WriteLine(antiNodes.Count);

            antiNodes.Clear();
            foreach (var type in types.Keys)
            {
                var antennas = types[type];
                for (var i = 0; i < antennas.Count; i++)
                {
                    add(antennas[i]);
                    for (var j = i + 1; j < antennas.Count; j++)
                    {
                        var dist = antennas[i].Distance(antennas[j]);
                        for (var k = 1; k < 100; k++)
                        {
                            var possibleAntiNode = antennas[j].Add(dist, k);
                            if (!isValid(possibleAntiNode)) break;
                            add(possibleAntiNode);
                        }

                        for (var k = 1; k < 100; k++)
                        {
                            var possibleAntiNode = antennas[i].Add(dist, -k);
                            if (!isValid(possibleAntiNode)) break;
                            add(possibleAntiNode);
                        }
                    }
                }
            }

            Console.WriteLine(antiNodes.Count);
        }
    }

    class Day9
    {
        public static void Solve()
        {
            var sampleInput = "2333133121414131402";
            var useSample = true;
            var input = useSample ? sampleInput : Helpers.LoadInput("day9.txt")[0];

            var list = new List<int>();
            var id = 0;
            for (var i = 0; i < input.Length; i += 2)
            {
                var fileBlocks = input[i] - '0';
                var freeBlocks = i + 1 < input.Length ? input[i + 1] - '0' : 0;
                for (var j = 0; j < fileBlocks; j++) list.Add(id);
                for (var j = 0; j < freeBlocks; j++) list.Add(-1);
                id++;
            }

            int left = 0; int right = list.Count - 1;
            while (left < list.Count && list[left] >= 0) left++;
            while (right >= 0 && list[right] < 0) right--;

            while (left < right)
            {
                var temp = list[left]; list[left] = list[right]; list[right] = temp;
                left++;
                while (left < list.Count && list[left] >= 0) left++;
                right--;
                while (right >= 0 && list[right] < 0) right--;
            }

            var checksum = 0L;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] < 0) break;
                checksum += list[i] * i;
            }

            Console.WriteLine(checksum);
        }
    }

    class Day10
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "89010123",
                "78121874",
                "87430965",
                "96549874",
                "45678903",
                "32019012",
                "01329801",
                "10456732"
            };
            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day10.txt");

            var totalScore = 0;
            for (var r = 0; r < lines.Length; r++)
            {
                for (var c = 0; c < lines[r].Length; c++)
                {
                    if (lines[r][c] == '0')
                    {
                        totalScore += CalculateTrailheadScore(lines, r, c);
                    }
                }
            }

            Console.WriteLine(totalScore);

            totalScore = 0;
            for (var r = 0; r < lines.Length; r++)
            {
                for (var c = 0; c < lines[r].Length; c++)
                {
                    if (lines[r][c] == '0')
                    {
                        totalScore += CalculateTrailheadScorePart2(lines, r, c);
                    }
                }
            }

            Console.WriteLine(totalScore);
        }

        static int CalculateTrailheadScore(string[] map, int startRow, int startCol)
        {
            var result = 0;
            var rows = map.Length;
            var cols = map[0].Length;
            var endPoints = new bool[rows, cols];
            var queue = new Queue<(int, int)>();

            queue.Enqueue((startRow, startCol));
            var rowDirections = new[] { -1, 1, 0, 0 };
            var colDirections = new[] { 0, 0, -1, 1 };
            while (queue.Count > 0)
            {
                (int row, int col) = queue.Dequeue();
                for (var i = 0; i < 4; i++)
                {
                    int nextRow = row + rowDirections[i];
                    if (nextRow < 0 || nextRow >= rows) continue;
                    int nextCol = col + colDirections[i];
                    if (nextCol < 0 || nextCol >= cols) continue;
                    if (map[row][col] + 1 == map[nextRow][nextCol])
                    {
                        if (map[nextRow][nextCol] == '9')
                        {
                            if (!endPoints[nextRow, nextCol])
                            {
                                result++;
                            }

                            endPoints[nextRow, nextCol] = true;
                        }
                        else
                        {
                            queue.Enqueue((nextRow, nextCol));
                        }
                    }
                }
            }

            return result;
        }

        static int CalculateTrailheadScorePart2(string[] map, int startRow, int startCol)
        {
            var result = 0;
            var rows = map.Length;
            var cols = map[0].Length;
            var queue = new Queue<(int, int)>();

            queue.Enqueue((startRow, startCol));
            var rowDirections = new[] { -1, 1, 0, 0 };
            var colDirections = new[] { 0, 0, -1, 1 };
            while (queue.Count > 0)
            {
                (int row, int col) = queue.Dequeue();
                for (var i = 0; i < 4; i++)
                {
                    int nextRow = row + rowDirections[i];
                    if (nextRow < 0 || nextRow >= rows) continue;
                    int nextCol = col + colDirections[i];
                    if (nextCol < 0 || nextCol >= cols) continue;
                    if (map[row][col] + 1 == map[nextRow][nextCol])
                    {
                        if (map[nextRow][nextCol] == '9')
                        {
                            result++;
                        }
                        else
                        {
                            queue.Enqueue((nextRow, nextCol));
                        }
                    }
                }
            }

            return result;
        }
    }

    class Day11
    {
        class PartialResult
        {
            public string Value { get; set; }
            public int RoundsLeft { get; set; }
            public override bool Equals(object? obj)
            {
                return obj is PartialResult other && this.Value == other.Value && this.RoundsLeft == other.RoundsLeft;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(this.Value, this.RoundsLeft);
            }
        }

        public static void SolveNaive()
        {
            var input = "125 17";
            var list = input.Split(' ').ToList();
            Console.WriteLine(string.Join(" ", list));
            for (int i = 0; i < 15; i++)
            {
                var newList = new List<string>();
                foreach (var item in list)
                {
                    newList.AddRange(Expand(item));
                }

                list = newList;
                Console.WriteLine($"({list.Count}) " + string.Join(" ", list));
            }
        }

        static IEnumerable<string> Expand(string value)
        {
            if (value == "0")
            {
                yield return "1";
            }
            else if ((value.Length % 2) == 1)
            {
                yield return (long.Parse(value) * 2024).ToString();
            }
            else
            {
                var part1 = value.Substring(0, value.Length / 2).TrimStart('0');
                var part2 = value.Substring(value.Length / 2).TrimStart('0'); ;
                if (part1 == "") part1 = "0";
                if (part2 == "") part2 = "0";
                yield return part1;
                yield return part2;
            }
        }
        public static void Solve()
        {
            var sampleInput = "125 17";
            var useSample = false;
            var input = useSample ? sampleInput : Helpers.LoadInput("day11.txt")[0].Trim();

            Dictionary<(string, int), long> partialResultsCache = new Dictionary<(string, int), long>();
            var parts = input.Split(' ');
            var rounds = 25;
            long result = 0;
            foreach (var part in parts)
            {
                result += CountStonesAfter(partialResultsCache, part, rounds);
            }

            Console.WriteLine(result);

            rounds = 75;
            result = 0;
            foreach (var part in parts)
            {
                result += CountStonesAfter(partialResultsCache, part, rounds);
            }

            Console.WriteLine(result);
        }

        static long CountStonesAfter(Dictionary<(string, int), long> partialResultsCache, string stone, int blinksLeft)
        {
            if (blinksLeft == 0) return 1;
            if (partialResultsCache.TryGetValue((stone, blinksLeft), out var cachedResult))
            {
                return cachedResult;
            }

            if (stone == "0")
            {
                if (blinksLeft == 1)
                {
                    partialResultsCache[(stone, 1)] = 1;
                    return 1;
                }

                var result = CountStonesAfter(partialResultsCache, "1", blinksLeft - 1);
                partialResultsCache.Add((stone, blinksLeft), result);
                return result;
            }
            else if ((stone.Length % 2) == 0)
            {
                var part1 = stone.Substring(0, stone.Length / 2).TrimStart('0');
                if (part1.Length == 0) part1 = "0";
                var part2 = stone.Substring(stone.Length / 2).TrimStart('0');
                if (part2.Length == 0) part2 = "0";
                if (blinksLeft == 1)
                {
                    partialResultsCache[(stone, 1)] = 2;
                    return 2;
                }

                long result1, result2;
                if (partialResultsCache.TryGetValue((part1, blinksLeft - 1), out var resultPart1))
                {
                    result1 = resultPart1;
                }
                else
                {
                    result1 = CountStonesAfter(partialResultsCache, part1, blinksLeft - 1);
                }

                if (partialResultsCache.TryGetValue((part2, blinksLeft - 1), out var resultPart2))
                {
                    result2 = resultPart2;
                }
                else
                {
                    result2 = CountStonesAfter(partialResultsCache, part2, blinksLeft - 1);
                }

                var result = result1 + result2;
                partialResultsCache.Add((stone, blinksLeft), result);
                return result;
            }
            else
            {
                if (blinksLeft == 1)
                {
                    partialResultsCache[(stone, 1)] = 1;
                    return 1;
                }

                var newStone = (long.Parse(stone) * 2024).ToString();
                if (partialResultsCache.TryGetValue((newStone, blinksLeft - 1), out var result))
                {
                    return result;
                }

                result = CountStonesAfter(partialResultsCache, newStone, blinksLeft - 1);
                partialResultsCache.Add((stone, blinksLeft), result);
                return result;
            }
        }
    }

    class Day12
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "RRRRIICCFF",
                "RRRRIICCCF",
                "VVRRRCCFFF",
                "VVRCCCJFFF",
                "VVVVCJJCFE",
                "VVIVCCJJEE",
                "VVIIICJJEE",
                "MIIIIIJJEE",
                "MIIISIJEEE",
                "MMMISSJEEE"
            };
            var useSample = false;
            var input = useSample ? sampleInput : Helpers.LoadInput("day12.txt");

            var rows = input.Length;
            var cols = input[0].Length;
            var visited = new bool[rows, cols];
            var totalCost = 0L;
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    if (!visited[row, col])
                    {
                        totalCost += CalculateCost(input, rows, cols, row, col, visited);
                    }
                }
            }

            Console.WriteLine(totalCost);

            totalCost = 0;
            visited = new bool[rows, cols];
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    if (!visited[row, col])
                    {
                        totalCost += CalculateCostPart2(input, rows, cols, row, col, visited);
                    }
                }
            }

            Console.WriteLine(totalCost);
        }

        private static long CalculateCost(string[] input, int rows, int cols, int startRow, int startCol, bool[,] visited)
        {
            var letter = input[startRow][startCol];
            var queue = new Queue<(int, int)>();
            queue.Enqueue((startRow, startCol));
            visited[startRow, startCol] = true;
            var perimeter = 0;
            var area = 0;
            while (queue.Count > 0)
            {
                (int r, int c) = queue.Dequeue();
                area++;
                if (r == 0 || input[r - 1][c] != letter)
                {
                    perimeter++;
                }
                else
                {
                    if (!visited[r - 1, c])
                    {
                        visited[r - 1, c] = true;
                        queue.Enqueue((r - 1, c));
                    }
                }

                if (r == rows - 1 || input[r + 1][c] != letter)
                {
                    perimeter++;
                }
                else
                {
                    if (!visited[r + 1, c])
                    {
                        visited[r + 1, c] = true;
                        queue.Enqueue((r + 1, c));
                    }
                }

                if (c == 0 || input[r][c - 1] != letter)
                {
                    perimeter++;
                }
                else
                {
                    if (!visited[r, c - 1])
                    {
                        visited[r, c - 1] = true;
                        queue.Enqueue((r, c - 1));
                    }
                }

                if (c == cols - 1 || input[r][c + 1] != letter)
                {
                    perimeter++;
                }
                else
                {
                    if (!visited[r, c + 1])
                    {
                        visited[r, c + 1] = true;
                        queue.Enqueue((r, c + 1));
                    }
                }
            }

            return perimeter * area;
        }

        private static long CalculateCostPart2(string[] input, int rows, int cols, int startRow, int startCol, bool[,] visited)
        {
            var letter = input[startRow][startCol];
            var queue = new Queue<(int, int)>();
            queue.Enqueue((startRow, startCol));
            visited[startRow, startCol] = true;
            var sides = 0;
            var area = 0;
            while (queue.Count > 0)
            {
                (int r, int c) = queue.Dequeue();
                area++;
                if (IsCorner(input, rows, cols, r, c, -1, -1)) sides++;
                if (IsCorner(input, rows, cols, r, c, -1, 1)) sides++;
                if (IsCorner(input, rows, cols, r, c, 1, -1)) sides++;
                if (IsCorner(input, rows, cols, r, c, 1, 1)) sides++;

                if (r > 0 && input[r - 1][c] == letter && !visited[r - 1, c])
                {
                    visited[r - 1, c] = true;
                    queue.Enqueue((r - 1, c));
                }

                if (r < rows - 1 && input[r + 1][c] == letter && !visited[r + 1, c])
                {
                    visited[r + 1, c] = true;
                    queue.Enqueue((r + 1, c));
                }

                if (c > 0 && input[r][c - 1] == letter && !visited[r, c - 1])
                {
                    visited[r, c - 1] = true;
                    queue.Enqueue((r, c - 1));
                }

                if (c < cols - 1 && input[r][c + 1] == letter && !visited[r, c + 1])
                {
                    visited[r, c + 1] = true;
                    queue.Enqueue((r, c + 1));
                }
            }

            return sides * area;
        }

        private static bool IsCorner(string[] input, int rows, int cols, int row, int col, int cornerRowDirection, int cornerColDirection)
        {
            int firstNeighborRow = row + cornerRowDirection;
            int secondNeighborCol = col + cornerColDirection;
            var current = input[row][col];
            var firstNeighbor = firstNeighborRow >= 0 && firstNeighborRow < rows ? input[firstNeighborRow][col] : '.';
            var secondNeighbor = secondNeighborCol >= 0 && secondNeighborCol < cols ? input[row][secondNeighborCol] : '.';
            var diagonalNeigbor = firstNeighborRow >= 0 && firstNeighborRow < rows && secondNeighborCol >= 0 && secondNeighborCol < cols ? input[firstNeighborRow][secondNeighborCol] : '.';
            return (current != firstNeighbor && current != secondNeighbor) || (current == firstNeighbor && current == secondNeighbor && current != diagonalNeigbor);
        }
    }

    class Day13
    {
        public class Game
        {
            public (int X, int Y) ButtonA { get; set; }
            public (int X, int Y) ButtonB { get; set; }
            public (int X, int Y) Prize { get; set; }

            static readonly Regex ButtonABRegex = new Regex(@"Button [A|B]: X\+(\d+), Y\+(\d+)");
            static readonly Regex PrizeRegex = new Regex(@"Prize: X=(\d+), Y=(\d+)");
            public static List<Game> Parse(string[] input)
            {
                var result = new List<Game>();
                for (int i = 0; i < input.Length; i++)
                {
                    if (string.IsNullOrEmpty(input[i])) continue;
                    var matchA = ButtonABRegex.Match(input[i++]);
                    var matchB = ButtonABRegex.Match(input[i++]);
                    var matchPrize = PrizeRegex.Match(input[i++]);
                    result.Add(new Game
                    {
                        ButtonA = GetValues(matchA),
                        ButtonB = GetValues(matchB),
                        Prize = GetValues(matchPrize)
                    });
                }

                return result;
            }
            static (int X, int Y) GetValues(Match match)
            {
                return (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            }
        }

        public static void Solve()
        {
            var sampleInput = new[]
            {
                "Button A: X+94, Y+34",
                "Button B: X+22, Y+67",
                "Prize: X=8400, Y=5400",
                "",
                "Button A: X+26, Y+66",
                "Button B: X+67, Y+21",
                "Prize: X=12748, Y=12176",
                "",
                "Button A: X+17, Y+86",
                "Button B: X+84, Y+37",
                "Prize: X=7870, Y=6450",
                "",
                "Button A: X+69, Y+23",
                "Button B: X+27, Y+71",
                "Prize: X=18641, Y=10279",
            };
            var useSample = false;
            var input = useSample ? sampleInput : Helpers.LoadInput("day13.txt");

            var games = Game.Parse(input);
            int totalTokens = 0;
            foreach (var game in games)
            {
                if (TrySolve(game, out var tokens))
                {
                    totalTokens += tokens;
                }
            }

            Console.WriteLine(totalTokens);

            long totalTokensPart2 = 0;
            foreach (var game in games)
            {
                if (TrySolvePart2(game, out var tokens))
                {
                    totalTokensPart2 += tokens;
                }
            }

            Console.WriteLine(totalTokensPart2);
        }

        static bool TrySolve(Game game, out int tokens)
        {
            var bP = (game.ButtonA.Y * game.Prize.X - game.ButtonA.X * game.Prize.Y) / (game.ButtonA.Y * game.ButtonB.X - game.ButtonA.X * game.ButtonB.Y);
            var aP = (game.Prize.X - bP * game.ButtonB.X) / game.ButtonA.X;
            if (aP >= 0 && bP >= 0 &&
                game.Prize.X == aP * game.ButtonA.X + bP * game.ButtonB.X &&
                game.Prize.Y == aP * game.ButtonA.Y + bP * game.ButtonB.Y)
            {
                tokens = 3 * aP + bP;
                return true;
            }

            tokens = -1;
            return false;
        }

        static bool TrySolvePart2(Game game, out long tokens)
        {
            long ax = game.ButtonA.X, ay = game.ButtonA.Y,
                bx = game.ButtonB.X, by = game.ButtonB.Y,
                px = game.Prize.X + 10000000000000L, py = game.Prize.Y + 10000000000000L;

            var bP = (ay * px - ax * py) / (ay * bx - ax * by);
            var aP = (px - bP * bx) / ax;
            if (aP >= 0 && bP >= 0 &&
                px == aP * ax + bP * bx &&
                py == aP * ay + bP * by)
            {
                tokens = 3 * aP + bP;
                return true;
            }

            tokens = -1;
            return false;
        }
    }

    class Day14
    {
        class Robot
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int DX { get; set; }
            public int DY { get; set; }

            static readonly Regex LineRegex = new Regex(@"p=(?<x>\d+),(?<y>\d+) v=(?<dx>-?\d+),(?<dy>-?\d+)");
            public static Robot Parse(string line)
            {
                var match = LineRegex.Match(line);
                return new Robot
                {
                    X = int.Parse(match.Groups["x"].Value),
                    Y = int.Parse(match.Groups["y"].Value),
                    DX = int.Parse(match.Groups["dx"].Value),
                    DY = int.Parse(match.Groups["dy"].Value),
                };
            }
        }


        public static void Solve()
        {
            var sampleInput = new[] {
                "p=0,4 v=3,-3",
                "p=6,3 v=-1,-3",
                "p=10,3 v=-1,2",
                "p=2,0 v=2,-1",
                "p=0,0 v=1,3",
                "p=3,0 v=-2,-2",
                "p=7,6 v=-1,-3",
                "p=3,0 v=-1,-2",
                "p=9,3 v=2,3",
                "p=7,3 v=-1,2",
                "p=2,4 v=2,-3",
                "p=9,5 v=-3,-3",
            };

            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day14.txt");
            var input = lines.Select(l => Robot.Parse(l)).ToArray();

            var rows = useSample ? 7 : 103;
            var cols = useSample ? 11 : 101;

            var midLineRows = rows / 2;
            var midLineCols = cols / 2;

            var quadrantSizes = new int[4];

            foreach (var robot in input)
            {
                var newX = robot.X + robot.DX * 100 + cols * 100;
                var newY = robot.Y + robot.DY * 100 + rows * 100;
                newX = newX % cols;
                newY = newY % rows;

                if (newX == midLineCols || newY == midLineRows) continue; // on the axis, no quadrants
                if (newY < midLineRows)
                {
                    if (newX > midLineCols)
                    {
                        quadrantSizes[0]++;
                    }
                    else
                    {
                        quadrantSizes[1]++;
                    }
                }
                else
                {
                    if (newX < midLineCols)
                    {
                        quadrantSizes[2]++;
                    }
                    else
                    {
                        quadrantSizes[3]++;
                    }
                }
            }

            Console.WriteLine(quadrantSizes[0] * quadrantSizes[1] * quadrantSizes[2] * quadrantSizes[3]);

            if (useSample)
            {
                Console.WriteLine("Part 2 only with real input");
                return;
            }

            var seconds = 0;
            var treeFound = false;
            while (!treeFound)
            {
                seconds++;
                var grid = new bool[rows, cols];

                // Move all robots
                foreach (var robot in input)
                {
                    var newCol = (robot.X + robot.DX + cols) % cols;
                    var newRow = (robot.Y + robot.DY + rows) % rows;
                    robot.X = newCol;
                    robot.Y = newRow;
                    grid[newRow, newCol] = true;
                }

                var clusterSize = 10;
                // Look for cluster of {clusterSize} vertical robots
                var searched = new bool[rows, cols];
                foreach (var robot in input)
                {
                    var col = robot.X;
                    var row = robot.Y;
                    if (searched[row, col]) continue;
                    int line = 1;
                    searched[row, col] = true;
                    row--;
                    while (row >= 0)
                    {
                        if (grid[row, col] && !searched[row, col])
                        {
                            line++;
                            searched[row, col] = true;
                            row--;
                        }
                        else
                        {
                            break;
                        }
                    }

                    row = robot.Y + 1;
                    while (row < rows)
                    {
                        if (grid[row, col] && !searched[row, col])
                        {
                            line++;
                            searched[row, col] = true;
                            row++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (line >= clusterSize)
                    {
                        for (int r = 0; r < rows; r++)
                        {
                            for (int c = 0; c < cols; c++)
                            {
                                Console.Write(grid[r, c] ? '#' : '.');
                            }

                            Console.WriteLine();
                        }

                        Console.WriteLine();
                        Console.Write("Is this a tree (Y/N)? ");
                        var answer = Console.ReadLine();
                        if (answer?.ToUpper() == "Y")
                        {
                            treeFound = true;
                            Console.WriteLine($"Found in {seconds} seconds.");
                        }

                        break;
                    }
                }
            }
        }
    }

    class Day15
    {
        static bool IsDebug = false;
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "##########",
                "#..O..O.O#",
                "#......O.#",
                "#.OO..O.O#",
                "#..O@..O.#",
                "#O#..O...#",
                "#O..O..O.#",
                "#.OO.O.OO#",
                "#....O...#",
                "##########",
                "",
                "<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^",
                "vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v",
                "><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<",
                "<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^",
                "^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><",
                "^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^",
                ">^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^",
                "<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>",
                "^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>",
                "v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^",
            };

            //sampleInput = new[]
            //{
            //    "########",
            //    "#..O.O.#",
            //    "##@.O..#",
            //    "#...O..#",
            //    "#.#.O..#",
            //    "#...O..#",
            //    "#......#",
            //    "########",
            //    "",
            //    "<^^>>>vv<v>>v<<",
            //};

            //sampleInput = new[]
            //{
            //    "#######",
            //    "#...#.#",
            //    "#.....#",
            //    "#..OO@#",
            //    "#..O..#",
            //    "#.....#",
            //    "#######",
            //    "",
            //    "<vv<<^^<<^^",
            //};

            var useSample = false;
            var input = useSample ? sampleInput : Helpers.LoadInput("day15.txt");

            List<string> map1 = new List<string>();
            int i;
            int rows = 0;
            int cols = input[0].Length;
            for (i = 0; i < input.Length; i++)
            {
                if (string.IsNullOrEmpty(input[i]))
                {
                    rows = i;
                    break;
                }

                map1.Add(input[i]);
            }

            var map = new char[rows, cols];
            int initialRow = -1, initialCol = -1;
            int r, c;
            for (r = 0; r < rows; r++)
            {
                for (c = 0; c < cols; c++)
                {
                    map[r, c] = map1[r][c];
                    if (map1[r][c] == '@')
                    {
                        initialRow = r;
                        initialCol = c;
                    }
                }
            }

            DumpMap(map, true);

            i++;
            var sb = new StringBuilder();
            for (; i < input.Length; i++)
            {
                sb.Append(input[i].Trim());
            }

            var instructions = sb.ToString();
            r = initialRow;
            c = initialCol;
            foreach (var instruction in instructions)
            {
                int dr, dc;
                if (IsDebug) Console.WriteLine("Move: " + instruction);
                switch (instruction)
                {
                    case '^': dr = -1; dc = 0; break;
                    case 'v': dr = 1; dc = 0; break;
                    case '>': dr = 0; dc = 1; break;
                    case '<': dr = 0; dc = -1; break;
                    default: throw new Exception();
                }

                if (map[r + dr, c + dc] == '.')
                {
                    map[r + dr, c + dc] = '@';
                    map[r, c] = '.';
                    r = r + dr;
                    c = c + dc;
                }
                else
                {
                    var boxesToPush = 1;
                    while (map[r + dr * boxesToPush, c + dc * boxesToPush] == 'O')
                    {
                        boxesToPush++;
                    }

                    if (map[r + dr * boxesToPush, c + dc * boxesToPush] == '#')
                    {
                        // Cannot push
                        if (IsDebug) { Console.WriteLine("No changes"); Console.WriteLine(); }
                        continue;
                    }

                    map[r + dr * boxesToPush, c + dc * boxesToPush] = 'O';
                    map[r + dr, c + dc] = '@';
                    map[r, c] = '.';
                    r = r + dr;
                    c = c + dc;
                }

                DumpMap(map);
            }

            int result = 0;
            for (r = 0; r < rows; r++)
            {
                for (c = 0; c < cols; c++)
                {
                    if (map[r, c] == 'O')
                    {
                        result += 100 * r + c;
                    }
                }
            }

            Console.WriteLine(result);

            // ************** Part 2 *************

            map = new char[rows, cols * 2];
            initialRow = -1; initialCol = -1;
            for (r = 0; r < rows; r++)
            {
                for (c = 0; c < cols; c++)
                {
                    map[r, 2 * c] = map[r, 2 * c + 1] = map1[r][c];
                    if (map1[r][c] == '@')
                    {
                        initialRow = r;
                        initialCol = 2 * c;
                        map[r, 2 * c + 1] = '.';
                    }
                    else if (map1[r][c] == 'O')
                    {
                        map[r, 2 * c] = '[';
                        map[r, 2 * c + 1] = ']';
                    }
                }
            }

            cols *= 2;

            // IsDebug = true;
            r = initialRow;
            c = initialCol;
            Console.WriteLine("Initial state:");
            DumpMap(map);
            foreach (var instruction in instructions)
            {
                int dr, dc;
                if (IsDebug) Console.WriteLine("Move: " + instruction);
                switch (instruction)
                {
                    case '^': dr = -1; dc = 0; break;
                    case 'v': dr = 1; dc = 0; break;
                    case '>': dr = 0; dc = 1; break;
                    case '<': dr = 0; dc = -1; break;
                    default: throw new Exception();
                }

                if (TryMove(map, r, c, dr, dc, true))
                {
                    r += dr;
                    c += dc;
                }

                DumpMap(map);
            }

            result = 0;
            for (r = 0; r < rows; r++)
            {
                for (c = 0; c < cols; c++)
                {
                    if (map[r, c] == '[')
                    {
                        result += 100 * r + c;
                    }
                }
            }

            Console.WriteLine("Part 2: " + result);
        }

        static bool TryMove(char[,] map, int r, int c, int dr, int dc, bool doMove)
        {
            var newR = r + dr;
            var newC = c + dc;
            char next = map[newR, newC];
            if (next == '.')
            {
                if (doMove)
                {
                    map[newR, newC] = map[r, c];
                    map[r, c] = '.';
                    r = newR;
                    c = newC;
                }

                return true;
            }
            else if (next == '[' || next == ']')
            {
                if (dr == 0)
                {
                    // Lateral push
                    if (TryMove(map, newR, newC, dr, dc, doMove: false))
                    {
                        if (doMove)
                        {
                            TryMove(map, newR, newC, dr, dc, doMove: true);
                            map[newR, newC] = map[r, c];
                            map[r, c] = '.';
                            r = newR;
                            c = newC;
                        }

                        return true;
                    }
                }
                else
                {
                    int otherBoxColumn = next == ']' ? c - 1 : c + 1;
                    if (TryMove(map, newR, newC, dr, dc, doMove: false) && TryMove(map, newR, otherBoxColumn, dr, dc, doMove: false))
                    {
                        if (doMove)
                        {
                            var newR2 = newR;
                            var newC2 = newC;
                            TryMove(map, newR2, newC2, dr, dc, doMove: true);
                            newR2 = newR;
                            newC2 = otherBoxColumn;
                            TryMove(map, newR2, newC2, dr, dc, doMove: true);

                            map[newR, newC] = map[r, c];
                            map[r, c] = '.';
                            r = newR;
                            c = newC;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        static void DumpMap(char[,] map, bool overrideDebug = false)
        {
            if (IsDebug || overrideDebug)
            {
                for (var r = 0; r < map.GetLength(0); r++)
                {
                    for (var c = 0; c < map.GetLength(1); c++)
                    {
                        Console.Write(map[r, c]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
    }

    public class Day16
    {
        public static void Solve()
        {
            var sampleInput = new[] {
                "###############",
                "#.......#....E#",
                "#.#.###.#.###.#",
                "#.....#.#...#.#",
                "#.###.#####.#.#",
                "#.#.#.......#.#",
                "#.#.#####.###.#",
                "#...........#.#",
                "###.#.#####.#.#",
                "#...#.....#.#.#",
                "#.#.#.###.#.#.#",
                "#.....#...#.#.#",
                "#.###.#.#.#.#.#",
                "#S..#.....#...#",
                "###############",
            };

            sampleInput = new[]
            {
                "#################",
                "#...#...#...#..E#",
                "#.#.#.#.#.#.#.#.#",
                "#.#.#.#...#...#.#",
                "#.#.#.#.###.#.#.#",
                "#...#.#.#.....#.#",
                "#.#.#.#.#.#####.#",
                "#.#...#.#.#.....#",
                "#.#.#####.#.###.#",
                "#.#.#.......#...#",
                "#.#.###.#####.###",
                "#.#.#...#.....#.#",
                "#.#.#.#####.###.#",
                "#.#.#.........#.#",
                "#.#.#.#########.#",
                "#S#.............#",
                "#################",
            };

            var useSample = false;
            var input = useSample ? sampleInput : Helpers.LoadInput("day16.txt");

            var rows = input.Length;
            var cols = input[0].Length;
            int endRow = 1, endCol = cols - 2;
            int startRow = rows - 2, startCol = 1;
            if (input[startRow][startCol] != 'S') throw new Exception();
            if (input[endRow][endCol] != 'E') throw new Exception();

            var queue = new PriorityQueue<(int, int, char, int), int>();
            Dictionary<(int, int, char), int> lowerCost = new Dictionary<(int, int, char), int>();

            queue.Enqueue((startRow, startCol, '>', 0), 0);
            lowerCost.Add((startRow, startCol, '>'), 0);
            while (queue.Count > 0)
            {
                (var row, var col, var direction, var cost) = queue.Dequeue();
                (var dr, var dc) = DirectionToDeltas(direction);
                if (input[row + dr][col + dc] == 'E')
                {
                    Console.WriteLine("Part 1: " + (cost + 1));
                    break;
                }

                if (input[row + dr][col + dc] == '.')
                {
                    if (!lowerCost.TryGetValue((row + dr, col + dc, direction), out var nextCost) || (cost + 1) < nextCost)
                    {
                        lowerCost[(row + dr, col + dc, direction)] = cost + 1;
                        queue.Enqueue((row + dr, col + dc, direction, cost + 1), cost + 1);
                    }
                }

                foreach (var newDirection in new[] { RotateClockwise(direction), RotateCounterclockwise(direction) })
                {
                    (var newDR, var newDC) = DirectionToDeltas(newDirection);
                    if (input[row + newDR][col + newDC] != '#')
                    {
                        if (!lowerCost.TryGetValue((row, col, newDirection), out var nextCost) || (cost + 1000) < nextCost)
                        {
                            lowerCost[(row, col, newDirection)] = cost + 1000;
                            queue.Enqueue((row, col, newDirection, cost + 1000), cost + 1000);
                        }
                    }
                }
            }
        }

        static char RotateClockwise(char direction)
        {
            switch (direction)
            {
                case 'v': return '<';
                case '<': return '^';
                case '^': return '>';
                default: return 'v';
            }
        }

        static char RotateCounterclockwise(char direction)
        {
            switch (direction)
            {
                case 'v': return '>';
                case '>': return '^';
                case '^': return '<';
                default: return 'v';
            }
        }

        static (int deltaRow, int deltaCol) DirectionToDeltas(char direction)
        {
            var dr = direction == '^' ? -1 : direction == 'v' ? 1 : 0;
            var dc = direction == '<' ? -1 : direction == '>' ? 1 : 0;
            return (dr, dc);
        }
    }

    public class Day17
    {
        enum OpCode
        {
            adv = 0,
            bxl = 1,
            bst = 2,
            jnz = 3,
            bxc = 4,
            @out = 5,
            bdv = 6,
            cdv = 7
        }

        class Computer
        {
            public static bool IsDebug = true;

            public int RegA { get; set; }
            public int RegB { get; set; }
            public int RegC { get; set; }
            public int IP { get; set; }
            public List<int> Instructions { get; set; }

            public Computer(int regA, int regB, int regC, IEnumerable<int> instructions)
            {
                this.RegA = regA;
                this.RegB = regB;
                this.RegC = regC;
                this.IP = 0;
                this.Instructions = new List<int>(instructions);
            }

            void DumpState(IEnumerable<int> outputs)
            {
                Console.WriteLine($"  Registers: {RegA},{RegB},{RegC}");
                Console.Write("  ");
                for (int i = 0; i < Instructions.Count; i++)
                {
                    if (i > 0) Console.Write(",");
                    if (i == IP) Console.Write("(");
                    if ((i % 2) == 0)
                    {
                        Console.Write((OpCode)Instructions[i]);
                    }
                    else
                    {
                        Console.Write(Instructions[i]);
                    }
                    if (i == IP) Console.Write(")");
                }
                Console.WriteLine();
                Console.WriteLine("  Outputs: " + string.Join(",", outputs));
                Console.WriteLine();
            }

            public List<int> Run()
            {
                var result = new List<int>();
                if (IsDebug)
                {
                    Console.WriteLine($"Initial state");
                    DumpState(result);
                }

                while (this.IP < this.Instructions.Count)
                {
                    var opcode = (OpCode)this.Instructions[IP];
                    var arg = this.Instructions[IP + 1];
                    this.IP += 2;
                    switch (opcode)
                    {
                        case OpCode.@out:
                            result.Add(this.GetComboOperand(arg) % 8);
                            break;
                        case OpCode.adv:
                            this.RegA /= 1 << this.GetComboOperand(arg);
                            break;
                        case OpCode.bdv:
                            this.RegB = this.RegA / (1 << this.GetComboOperand(arg));
                            break;
                        case OpCode.cdv:
                            this.RegC = this.RegA / (1 << this.GetComboOperand(arg));
                            break;
                        case OpCode.jnz:
                            if (this.RegA != 0)
                            {
                                this.IP = arg;
                            }

                            break;
                        case OpCode.bst:
                            this.RegB = this.GetComboOperand(arg) % 8;
                            break;
                        case OpCode.bxl:
                            this.RegB ^= arg;
                            break;
                        case OpCode.bxc:
                            this.RegB ^= this.RegC;
                            break;
                        default:
                            throw new Exception();
                    }

                    if (IsDebug) DumpState(result);
                }

                return result;
            }

            private int GetComboOperand(int value)
            {
                switch (value)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        return value;
                    case 4:
                        return this.RegA;
                    case 5:
                        return this.RegB;
                    case 6:
                        return this.RegC;
                    default:
                        throw new Exception();
                }
            }
        }

        static void TestCases()
        {
            Computer.IsDebug = false;
            var tests = new List<(int, int, int, string)>
            {
                ( 0, 0, 9, "2,6" ),
                ( 10, 0, 0, "5,0,5,1,5,4" ),
                ( 2024, 0, 0, "0,1,5,4,3,0" ),
                ( 0, 29, 0, "1,7" ),
                ( 0, 2024, 43690, "4,0" )
            };
            foreach (var test in tests)
            {
                (var a, var b, var c, var instr) = test;
                var comp = new Computer(a, b, c, instr.Split(',').Select(p => int.Parse(p)));
                var output = comp.Run();
                Console.WriteLine($"Result: {comp.RegA}, {comp.RegB}, {comp.RegC} - {string.Join(",", output)}");
            }
        }
        public static void Solve()
        {
            //TestCases();
            var sampleInput = new[]
            {
                "Register A: 729",
                "Register B: 0",
                "Register C: 0",
                "",
                "Program: 0,1,5,4,3,0"
            };
            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day17.txt");

            var registers = lines.Take(3).Select(l => int.Parse(l.Substring(l.IndexOf(':') + 1))).ToArray();
            if (registers.Length != 3) throw new Exception();
            var program = lines.Skip(4).First();
            if (!program.StartsWith("Program: ")) throw new Exception();
            var instructions = program.Substring("Program: ".Length).Split(',').Select(p => int.Parse(p));
            var computer = new Computer(registers[0], registers[1], registers[2], instructions);

            Console.WriteLine(string.Join(",", computer.Run()));
        }
    }

    class Day18
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "5,4",
                "4,2",
                "4,5",
                "3,0",
                "2,1",
                "6,3",
                "2,4",
                "1,5",
                "0,6",
                "3,3",
                "2,6",
                "5,1",
                "1,2",
                "5,5",
                "2,5",
                "6,5",
                "1,4",
                "0,4",
                "6,4",
                "1,1",
                "6,1",
                "1,0",
                "0,5",
                "1,6",
                "2,0",
            };

            var useSample = false;
            var input = useSample ? sampleInput : Helpers.LoadInput("day18.txt");
            var byteAddresses = input.Select(row =>
            {
                var parts = row.Split(',');
                return (int.Parse(parts[0]), int.Parse(parts[1]));
            }).ToArray();

            var rows = useSample ? 7 : 71;
            var cols = useSample ? 7 : 71;
            var bytesFallen = useSample ? 12 : 1024;

            var map = new char[rows, cols];
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    map[r, c] = '.';
                }
            }

            foreach (var byteAddress in byteAddresses.Take(bytesFallen))
            {
                (var row, var col) = byteAddress;
                map[row, col] = '#';
            }

            var dumpMap = () =>
            {
                for (var r = 0; r < rows; r++)
                {
                    for (var c = 0; c < cols; c++)
                    {
                        Console.Write(map[r, c]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            };

            var queue = new Queue<(int, int, int)>();
            queue.Enqueue((0, 0, 0));
            var directions = new List<(int dr, int dc)> { (-1, 0), (1, 0), (0, -1), (0, 1) };
            var resultPart1 = -1;
            while (queue.Count > 0)
            {
                //dumpMap();
                (var curRow, var curCol, var curCost) = queue.Dequeue();
                foreach (var direction in directions)
                {
                    (var dr, var dc) = direction;
                    var nextRow = curRow + dr;
                    var nextCol = curCol + dc;
                    var nextCost = curCost + 1;
                    if (nextRow == rows - 1 && nextCol == cols - 1)
                    {
                        resultPart1 = nextCost;
                        break;
                    }

                    if (0 <= nextRow && nextRow < rows && 0 <= nextCol && nextCol < cols && map[nextRow, nextCol] == '.')
                    {
                        map[nextRow, nextCol] = 'O';
                        queue.Enqueue((nextRow, nextCol, nextCost));
                    }
                }

                if (resultPart1 > 0) break;
            }

            Console.WriteLine(resultPart1);

            int resultPart2;
            do
            {
                map = new char[rows, cols];
                for (var r = 0; r < rows; r++)
                {
                    for (var c = 0; c < cols; c++)
                    {
                        map[r, c] = '.';
                    }
                }

                foreach (var byteAddress in byteAddresses.Take(bytesFallen))
                {
                    (var row, var col) = byteAddress;
                    map[row, col] = '#';
                }

                queue = new Queue<(int, int, int)>();
                queue.Enqueue((0, 0, 0));
                resultPart2 = -1;

                while (queue.Count > 0)
                {
                    //dumpMap();
                    (var curRow, var curCol, var curCost) = queue.Dequeue();
                    foreach (var direction in directions)
                    {
                        (var dr, var dc) = direction;
                        var nextRow = curRow + dr;
                        var nextCol = curCol + dc;
                        var nextCost = curCost + 1;
                        if (nextRow == rows - 1 && nextCol == cols - 1)
                        {
                            resultPart2 = nextCost;
                            break;
                        }

                        if (0 <= nextRow && nextRow < rows && 0 <= nextCol && nextCol < cols && map[nextRow, nextCol] == '.')
                        {
                            map[nextRow, nextCol] = 'O';
                            queue.Enqueue((nextRow, nextCol, nextCost));
                        }
                    }

                    if (resultPart2 > 0) break;
                }

                if (resultPart2 > 0)
                {
                    bytesFallen++;
                }
            } while (resultPart2 > 0);

            (var lastRow, var lastCol) = byteAddresses[bytesFallen - 1];
            Console.WriteLine($"Part 2: {lastRow},{lastCol}");
        }
    }

    class Day19
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "r, wr, b, g, bwu, rb, gb, br",
                "",
                "brwrr",
                "bggr",
                "gbbr",
                "rrbgbr",
                "ubwu",
                "bwurrg",
                "brgr",
                "bbrgwb",
            };
            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day19.txt");
            var patterns = lines[0].Split(',').Select(p => p.Trim()).ToArray();
            var tests = lines.Skip(2).ToArray();

            var part1Result = 0;
            for (var iTest = 0; iTest < tests.Length; iTest++)
            {
                var test = tests[iTest];
                var canCreateUpTo = new bool[test.Length + 1];
                canCreateUpTo[0] = true;
                for (var i = 1; i <= test.Length; i++)
                {
                    foreach (var pattern in patterns)
                    {
                        var patternLength = pattern.Length;
                        if (patternLength <= i && canCreateUpTo[i - patternLength] && test.Substring(i - patternLength, patternLength) == pattern)
                        {
                            canCreateUpTo[i] = true;
                            break;
                        }
                    }
                }
                var pass = canCreateUpTo[test.Length];
                Console.WriteLine($"Test {iTest + 1} of {tests.Length}: {pass}");

                if (pass)
                {
                    part1Result++;
                }
            }

            Console.WriteLine(part1Result);

            var part2Result = 0L;
            for (var iTest = 0; iTest < tests.Length; iTest++)
            {
                var test = tests[iTest];
                var waysToCreateUpTo = new long[test.Length + 1];
                waysToCreateUpTo[0] = 1;
                for (var i = 1; i <= test.Length; i++)
                {
                    foreach (var pattern in patterns)
                    {
                        var patternLength = pattern.Length;
                        if (patternLength <= i && waysToCreateUpTo[i - patternLength] > 0 && test.Substring(i - patternLength, patternLength) == pattern)
                        {
                            waysToCreateUpTo[i] += waysToCreateUpTo[i - patternLength];
                        }
                    }
                }
                var ways = waysToCreateUpTo[test.Length];
                Console.WriteLine($"Test {iTest + 1} of {tests.Length}: {ways}");

                part2Result += ways;
            }

            Console.WriteLine(part2Result);
        }
    }

    class Day20
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "###############",
                "#...#...#.....#",
                "#.#.#.#.#.###.#",
                "#S#...#.#.#...#",
                "#######.#.#.###",
                "#######.#.#...#",
                "#######.#.###.#",
                "###..E#...#...#",
                "###.#######.###",
                "#...###...#...#",
                "#.#####.#.###.#",
                "#.#...#.#.#...#",
                "#.#.#.#.#.#.###",
                "#...#...#...###",
                "###############",
            };
            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day20.txt");

            var rows = lines.Length;
            var cols = lines[0].Length;
            var costs = new int[rows, cols];
            int startRow = -1, startCol = -1;
            int endRow = -1, endCol = -1;
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    costs[r, c] = -1;
                    if (lines[r][c] == 'S')
                    {
                        startRow = r;
                        startCol = c;
                    }
                    else if (lines[r][c] == 'E')
                    {
                        endRow = r;
                        endCol = c;
                    }
                }
            }

            var prev = new Dictionary<(int row, int col), (int row, int col)>();
            prev.Add((startRow, startCol), (-1, -1));
            var queue = new Queue<(int, int)>();
            queue.Enqueue((startRow, startCol));
            while (queue.Count > 0)
            {
                var (r, c) = queue.Dequeue();
                if (r == endRow && c == endCol) break;

                if (c >= 1 && lines[r][c - 1] != '#' && !prev.ContainsKey((r, c - 1)))
                {
                    prev.Add((r, c - 1), (r, c));
                    queue.Enqueue((r, c - 1));
                }

                if (c < cols - 1 && lines[r][c + 1] != '#' && !prev.ContainsKey((r, c + 1)))
                {
                    prev.Add((r, c + 1), (r, c));
                    queue.Enqueue((r, c + 1));
                }

                if (r >= 1 && lines[r - 1][c] != '#' && !prev.ContainsKey((r - 1, c)))
                {
                    prev.Add((r - 1, c), (r, c));
                    queue.Enqueue((r - 1, c));
                }

                if (r < rows - 1 && lines[r + 1][c] != '#' && !prev.ContainsKey((r + 1, c)))
                {
                    prev.Add((r + 1, c), (r, c));
                    queue.Enqueue((r + 1, c));
                }
            }

            var current = (endRow, endCol);
            var cost = 0;
            var path = new List<(int row, int col)>();

            while (true)
            {
                var (r, c) = current;
                costs[r, c] = cost;
                path.Add(current);
                if (r == startRow && c == startCol) break;
                current = prev[current];
                cost++;
            }

            path.Reverse();

            var cheats = new Dictionary<(int r1, int c1, int r2, int c2), int>();
            foreach (var step in path)
            {
                var (r, c) = step;
                if (c > 1 && lines[r][c - 1] == '#' && costs[r, c - 2] >= 0 && costs[r, c] > costs[r, c - 2])
                {
                    // Cheat: skip from (r,c) to (r,c-2)
                    cheats.Add((r, c, r, c - 2), costs[r, c] - costs[r, c - 2] - 2);
                }
                if (c < cols - 2 && lines[r][c + 1] == '#' && costs[r, c + 2] >= 0 && costs[r, c] > costs[r, c + 2])
                {
                    // Cheat: skip from (r,c) to (r,c+2)
                    cheats.Add((r, c, r, c + 2), costs[r, c] - costs[r, c + 2] - 2);
                }
                if (r > 1 && lines[r - 1][c] == '#' && costs[r - 2, c] >= 0 && costs[r, c] > costs[r - 2, c])
                {
                    // Cheat: skip from (r,c) to (r-2,c)
                    cheats.Add((r, c, r - 2, c), costs[r, c] - costs[r - 2, c] - 2);
                }
                if (r < rows - 2 && lines[r + 1][c] == '#' && costs[r + 2, c] >= 0 && costs[r, c] > costs[r + 2, c])
                {
                    // Cheat: skip from (r,c) to (r+2,c)
                    cheats.Add((r, c, r + 2, c), costs[r, c] - costs[r + 2, c] - 2);
                }
            }

            var pathReductionThreshold = useSample ? 10 : 100;
            var part1Result = 0;
            foreach (var cheat in cheats)
            {
                if (cheat.Value >= pathReductionThreshold)
                {
                    part1Result++;
                }
            }

            Console.WriteLine(part1Result);

            // Part 2
            pathReductionThreshold = useSample ? 70 : 100;
            var part2Result = 0;
            cheats.Clear();
            foreach (var step in path)
            {
                var (r, c) = step;
                for (var dr = -20; dr <= 20; dr++)
                {
                    for (var dc = -20 + Math.Abs(dr); dc <= 20 - Math.Abs(dr); dc++)
                    {
                        var r2 = r + dr;
                        var c2 = c + dc;
                        if (0 <= r2 && r2 < rows && 0 <= c2 && c2 < cols && costs[r2, c2] >= 0 && costs[r, c] > costs[r2, c2])
                        {
                            // Cheat: skip from (r,c) to (r2,c2)
                            cheats.Add((r, c, r2, c2), costs[r, c] - costs[r2, c2] - Math.Abs(dr) - Math.Abs(dc));
                        }
                    }
                }
            }
            foreach (var cheat in cheats)
            {
                if (cheat.Value >= pathReductionThreshold)
                {
                    part2Result++;
                }
            }

            Console.WriteLine(part2Result);
        }
    }

    class Day22
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "1",
                "10",
                "100",
                "2024"
            };
            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day22.txt");

            var resultPart1 = 0L;
            foreach (var line in lines)
            {
                resultPart1 += GenerateSecret(long.Parse(line), 2000);
            }

            Console.WriteLine(resultPart1);

            // Sequence => (iLine => max value)
            var allSequences = new Dictionary<(int, int, int, int), Dictionary<int, int>>();
            for (var iLine = 0; iLine < lines.Length; iLine++)
            {
                var line = lines[iLine];
                long value = long.Parse(line);
                int v0, v1, v2, v3, v4;
                v0 = (int)(value % 10);
                value = Iterate(value);
                v1 = (int)(value % 10);
                value = Iterate(value);
                v2 = (int)(value % 10);
                value = Iterate(value);
                v3 = (int)(value % 10);
                for (int i = 3; i < 2000; i++)
                {
                    value = Iterate(value);
                    v4 = (int)(value % 10);

                    var sequence = (v1 - v0, v2 - v1, v3 - v2, v4 - v3);
                    if (!allSequences.TryGetValue(sequence, out var dict))
                    {
                        dict = new Dictionary<int, int> { { iLine, v4 } };
                        allSequences.Add(sequence, dict);
                    }

                    if (!dict.ContainsKey(iLine))
                    {
                        dict[iLine] = v4;
                    }

                    v0 = v1;
                    v1 = v2;
                    v2 = v3;
                    v3 = v4;
                }
            }

            var currMaxSum = 0;
            (int, int, int, int) currMaxSequence = default;
            foreach (var seq in allSequences)
            {
                var dict = seq.Value;
                var (d1, d2, d3, d4) = seq.Key;
                var sum = dict.Sum(pair => pair.Value);
                if (sum > currMaxSum)
                {
                    currMaxSum = sum;
                    currMaxSequence = seq.Key;
                }
            }

            Console.WriteLine(currMaxSequence);
            Console.WriteLine(currMaxSum);
        }

        private static long Iterate(long value)
        {
            value = MixAndPrune(value, v => v * 64);
            value = MixAndPrune(value, v => v / 32);
            value = MixAndPrune(value, v => v * 2048);
            return value;
        }

        private static long GenerateSecret(long initial, int iterations)
        {
            long value = initial;
            // Console.WriteLine("Iteration 0: " + value);
            for (var i = 0; i < iterations; i++)
            {
                value = Iterate(value);
                // Console.WriteLine($"Iteration {i + 1}: {value}");
            }

            return value;
        }

        static long MixAndPrune(long value, Func<long, long> operation)
        {
            var other = operation(value);
            value ^= other;
            return value & 0xFFFFFF;
        }
    }

    class Day23
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "kh-tc",
                "qp-kh",
                "de-cg",
                "ka-co",
                "yn-aq",
                "qp-ub",
                "cg-tb",
                "vc-aq",
                "tb-ka",
                "wh-tc",
                "yn-cg",
                "kh-ub",
                "ta-co",
                "de-co",
                "tc-td",
                "tb-wq",
                "wh-td",
                "ta-ka",
                "td-qp",
                "aq-cg",
                "wq-ub",
                "ub-vc",
                "de-ta",
                "wq-aq",
                "wq-vc",
                "wh-yn",
                "ka-de",
                "kh-ta",
                "co-tc",
                "wh-qp",
                "tb-vc",
                "td-yn",
            };

            var useSample = false;
            var input = useSample ? sampleInput : Helpers.LoadInput("day23.txt");

            var computerNames = new HashSet<string>();
            var connections = new Dictionary<string, List<string>>();
            foreach (var connection in input)
            {
                var c1 = connection.Substring(0, 2);
                var c2 = connection.Substring(3, 2);
                computerNames.Add(c1);
                computerNames.Add(c2);
                if (!connections.TryGetValue(c1, out var conn1))
                {
                    conn1 = new List<string>();
                    connections.Add(c1, conn1);
                }
                conn1.Add(c2);
                if (!connections.TryGetValue(c2, out var conn2))
                {
                    conn2 = new List<string>();
                    connections.Add(c2, conn2);
                }
                conn2.Add(c1);
            }

            foreach (var conn in connections)
            {
                conn.Value.Sort();
            }

            var sortedComputerNames = computerNames.OrderBy(c => c).ToArray();
            var connectedGroups = new List<List<string>>();
            var groupsWithT = 0;
            for (var i = 0; i < sortedComputerNames.Length - 2; i++)
            {
                var c1 = sortedComputerNames[i];
                foreach (var c2 in connections[c1])
                {
                    if (c1.CompareTo(c2) > 0) continue;
                    foreach (var c3 in connections[c2])
                    {
                        if (c2.CompareTo(c3) > 0) continue;
                        if (connections[c1].Contains(c3))
                        {
                            connectedGroups.Add(new List<string> { c1, c2, c3 });
                            if (c1[0] == 't' || c2[0] == 't' || c3[0] == 't') groupsWithT++;
                        }
                    }
                }
            }

            Console.WriteLine(groupsWithT);
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
