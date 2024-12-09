﻿using System.Diagnostics;

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
            Day7.Solve();
            //Day8.Solve();
            //Day9.Solve();
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
            int row = -1, col = -1;
            char direction = '^';
            for (var r = 0; r < lines.Length; r++)
            {
                for (var c = 0; c < lines[r].Length; c++)
                {
                    if (lines[r][c] == '^')
                    {
                        row = r;
                        col = c;
                        break;
                    }
                }

                if (row >= 0) break;
            }

            var visited = new HashSet<int>();
            visited.Add(CoordsToValue(row, col));
            var endOfMap = false;
            while (!endOfMap)
            {
                if (TryMove(lines, ref row, ref col, ref direction, ref endOfMap))
                {
                    visited.Add(CoordsToValue(row, col));
                }
            }

            Console.WriteLine(visited.Count);
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

        static bool TryMove(string[] map, ref int row, ref int col, ref char direction, ref bool endOfMap)
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

                    if (map[row - 1][col] == '#')
                    {
                        direction = ChangeDirection(direction);
                        return false;
                    }

                    row--;
                    return true;
                case 'v':
                    if (row == map.Length - 1)
                    {
                        endOfMap = true;
                        return false;
                    }

                    if (map[row + 1][col] == '#')
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

                    if (map[row][col - 1] == '#')
                    {
                        direction = ChangeDirection(direction);
                        return false;
                    }

                    col--;
                    return true;
                case '>':
                    if (col == map[0].Length - 1)
                    {
                        endOfMap = true;
                        return false;
                    }

                    if (map[row][col + 1] == '#')
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
                    var operations = ToBitArray(possibleOperators, operatorCount);
                    for (var i = 0; i < operations.Length; i++)
                    {
                        if (operations[i])
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

    public class Helpers
    {
        public static string[] LoadInput(string fileName)
        {
            return File.ReadAllLines(Path.Combine("inputs", fileName));
        }
    }

}
