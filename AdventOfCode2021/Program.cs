using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2021
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Day18.Solve();
        }

        static void Day1Part1()
        {
            // input: "https://adventofcode.com/2021/day/1/input"
            var input = Helpers.LoadInput("input1.txt");
            int incrementCount = 0;
            int prevValue = int.Parse(input[0]);
            for (int i = 1; i < input.Length; i++)
            {
                var currValue = int.Parse(input[i]);
                if (currValue > prevValue) incrementCount++;
                prevValue = currValue;
            }

            Console.WriteLine(incrementCount);
        }

        static void Day1Part2()
        {
            // input: "https://adventofcode.com/2021/day/1/input"
            var input = Helpers.LoadInput("input1.txt");
            int incrementCount = 0;
            int w0 = int.Parse(input[0]);
            int w1 = int.Parse(input[1]);
            int w2 = int.Parse(input[2]);
            int prevWindow = w0 + w1 + w2;
            for (int i = 3; i < input.Length; i++)
            {
                var currValue = int.Parse(input[i]);
                int currWindow = prevWindow - w0 + currValue;
                if (currWindow > prevWindow) incrementCount++;
                prevWindow = currWindow;
                w0 = w1;
                w1 = w2;
                w2 = currValue;
            }

            Console.WriteLine(incrementCount);
        }

        static void Day2Part1()
        {
            // input: https://adventofcode.com/2021/day/2/input
            var input = Helpers.LoadInput("input2.txt");
            int horizontal = 0;
            int depth = 0;
            for (int i = 0; i < input.Length; i++)
            {
                var order = input[i];
                var parts = order.Split(' ');
                var direction = parts[0];
                var amount = int.Parse(parts[1]);
                switch (direction)
                {
                    case "forward": horizontal += amount; break;
                    case "down": depth += amount; break;
                    case "up": depth -= amount; break;
                    default: throw new Exception("Invalid order: " + order);
                }
            }

            Console.WriteLine($"horizontal={horizontal}, depth={depth}, product={horizontal * depth}");
        }

        static void Day2Part2()
        {
            // input: https://adventofcode.com/2021/day/2/input
            var input = Helpers.LoadInput("input2.txt");
            int horizontal = 0;
            int depth = 0;
            int aim = 0;
            for (int i = 0; i < input.Length; i++)
            {
                var order = input[i];
                var parts = order.Split(' ');
                var direction = parts[0];
                var amount = int.Parse(parts[1]);
                switch (direction)
                {
                    case "forward":
                        horizontal += amount;
                        depth += aim * amount;
                        break;
                    case "down": aim += amount; break;
                    case "up": aim -= amount; break;
                    default: throw new Exception("Invalid order: " + order);
                }
            }

            Console.WriteLine($"horizontal={horizontal}, depth={depth}, product={horizontal * depth}");
        }

        static void Day3()
        {
            // input: "https://adventofcode.com/2021/day/3/input"
            //var input = new string[] { "00100", "11110", "10110", "10111", "10101", "01111", "00111", "11100", "10000", "11001", "00010", "01010" };
            var input = Helpers.LoadInput("input3.txt");
            int bitLength = input[0].Length;
            int[] oneCounts = new int[bitLength];
            for (int i = 0; i < input.Length; i++)
            {
                for (int j = 0; j < bitLength; j++)
                {
                    if (input[i][j] == '1') oneCounts[j]++;
                }
            }

            int p2 = 1;
            int gamma = 0;
            int epsilon = 0;
            for (int i = bitLength - 1; i >= 0; i--)
            {
                if (oneCounts[i] > input.Length / 2)
                {
                    gamma += p2;
                }
                else
                {
                    epsilon += p2;
                }

                p2 *= 2;
            }

            Console.WriteLine($"Part 1: {gamma * epsilon}");

            string findRating(bool takeMostCommon, bool oneIfTie)
            {
                int bit = 0;
                string[] currentList = input;
                while (currentList.Count() > 1)
                {
                    int oneBits = currentList.Sum(s => s[bit] == '1' ? 1 : 0);
                    int zeroBits = currentList.Count() - oneBits;
                    char bitToTake;
                    if (oneBits > zeroBits)
                    {
                        bitToTake = takeMostCommon ? '1' : '0';
                    }
                    else if (oneBits < zeroBits)
                    {
                        bitToTake = takeMostCommon ? '0' : '1';
                    }
                    else
                    {
                        bitToTake = oneIfTie ? '1' : '0';
                    }
                    currentList = currentList.Where(s => s[bit] == bitToTake).ToArray();
                    bit++;
                }

                return currentList.First();
            }
            var oxygenGeneratorRating = Convert.ToInt32(findRating(true, true), 2);
            var co2ScrubberRating = Convert.ToInt32(findRating(false, false), 2);
            Console.WriteLine($"Part 2: {oxygenGeneratorRating * co2ScrubberRating}");
        }

        class Day4
        {
            class Board
            {
                public int[,] Numbers;
                private bool[,] Called;
                public Board(string[] lines)
                {
                    this.Numbers = new int[5, 5];
                    this.Called = new bool[5, 5];
                    for (int i = 0; i < 5; i++)
                    {
                        var lineNumbers = lines[i]
                            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => int.Parse(s))
                            .ToArray();
                        for (int j = 0; j < 5; j++)
                        {
                            this.Numbers[i, j] = lineNumbers[j];
                        }
                    }
                }

                public void ResetBoard()
                {
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            this.Called[i, j] = false;
                        }
                    }
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="number"></param>
                /// <returns><code>true</code> if it is a winning board, <code>false</code> otherwise</returns>
                public bool CallNumber(int number)
                {
                    int row = -1, col = -1;
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (this.Numbers[i, j] == number)
                            {
                                row = i;
                                col = j;
                                break;
                            }
                        }

                        if (row >= 0) break;
                    }

                    if (row < 0) return false;
                    this.Called[row, col] = true;
                    bool winningRow = true, winningCol = true;
                    for (int i = 0; i < 5; i++)
                    {
                        if (!this.Called[i, col])
                        {
                            winningCol = false;
                        }

                        if (!this.Called[row, i])
                        {
                            winningRow = false;
                        }
                    }

                    return (winningRow || winningCol);
                }

                public IEnumerable<int> UncalledNumbers()
                {
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (!this.Called[i, j])
                            {
                                yield return this.Numbers[i, j];
                            }
                        }
                    }
                }
            }
            public static void Solve()
            {
                // input: "https://adventofcode.com/2021/day/4/input"
                var input = Helpers.LoadInput("input4.txt");
                var calledNumbers = input[0].Split(',').Select(s => int.Parse(s)).ToArray();
                var boards = new List<Board>();
                for (int i = 2; i < input.Length; i += 6)
                {
                    var boardLines = new string[] { input[i], input[i + 1], input[i + 2], input[i + 3], input[i + 4] };
                    boards.Add(new Board(boardLines));
                }

                int result = -1;
                for (int i = 0; i < calledNumbers.Length; i++)
                {
                    foreach (var board in boards)
                    {
                        bool winning = board.CallNumber(calledNumbers[i]);
                        if (winning)
                        {
                            var uncalledNumbersSum = board.UncalledNumbers().Sum();
                            result = uncalledNumbersSum * calledNumbers[i];
                            break;
                        }
                    }

                    if (result >= 0)
                    {
                        break;
                    }
                }

                Console.WriteLine($"Part 1: {result}");

                foreach (var board in boards)
                {
                    board.ResetBoard();
                }

                result = -1;
                var allBoards = new List<Board>(boards);
                for (int i = 0; i < calledNumbers.Length; i++)
                {
                    List<int> winningBoardIndices = new List<int>();
                    for (int boardIndex = 0; boardIndex < allBoards.Count; boardIndex++)
                    {
                        var board = allBoards[boardIndex];
                        bool winning = board.CallNumber(calledNumbers[i]);
                        if (winning)
                        {
                            winningBoardIndices.Add(boardIndex);
                            result = board.UncalledNumbers().Sum() * calledNumbers[i];
                        }
                    }

                    for (int j = winningBoardIndices.Count - 1; j >= 0; j--)
                    {
                        allBoards.RemoveAt(winningBoardIndices[j]);
                    }

                    if (allBoards.Count == 0)
                    {
                        break;
                    }
                }

                Console.WriteLine($"Part 2: {result}");
            }
        }

        class Day5
        {
            public static void Solve()
            {
                // input: "https://adventofcode.com/2021/day/5/input"
                var input = Helpers.LoadInput("input5.txt");
                var vents = input.Select(d => Vent.Parse(d)).ToArray();
                var maxX = vents.Max(v => Math.Max(v.xFrom, v.xTo));
                var maxY = vents.Max(v => Math.Max(v.yFrom, v.yTo));
                int[,] field = new int[maxX + 1, maxY + 1];
                foreach (var vent in vents)
                {
                    if (vent.IsHorizontal)
                    {
                        int min = Math.Min(vent.xFrom, vent.xTo);
                        int max = Math.Max(vent.xFrom, vent.xTo);
                        for (int i = min; i <= max; i++)
                        {
                            field[i, vent.yFrom]++;
                        }
                    }
                    else if (vent.IsVertical)
                    {
                        int min = Math.Min(vent.yFrom, vent.yTo);
                        int max = Math.Max(vent.yFrom, vent.yTo);
                        for (int i = min; i <= max; i++)
                        {
                            field[vent.xFrom, i]++;
                        }
                    }
                }

                int resultPart1 = 0;
                for (int y = 0; y < maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        if (field[x, y] > 1) resultPart1++;
                    }
                }

                Console.WriteLine($"Day 5 Part 1: {resultPart1}");

                foreach (var vent in vents)
                {
                    if (!vent.IsHorizontal && !vent.IsVertical)
                    {
                        int xDirection = Math.Sign(vent.xTo - vent.xFrom);
                        int yDirection = Math.Sign(vent.yTo - vent.yFrom);
                        int length = Math.Abs(vent.xFrom - vent.xTo);
                        for (int i = 0; i <= length; i++)
                        {
                            int x = vent.xFrom + i * xDirection;
                            int y = vent.yFrom + i * yDirection;
                            field[x, y]++;
                        }
                    }
                }

                int resultPart2 = 0;
                for (int y = 0; y < maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        if (field[x, y] > 1) resultPart2++;
                    }
                }

                Console.WriteLine($"Day 5 Part 2: {resultPart2}");
            }

            class Vent
            {
                public int xFrom;
                public int yFrom;
                public int xTo;
                public int yTo;

                private static Regex defRegex = new Regex(@"(?<xFrom>\d+),(?<yFrom>\d+) \-\> (?<xTo>\d+),(?<yTo>\d+)");
                public static Vent Parse(string definition)
                {
                    Match match = defRegex.Match(definition);
                    return new Vent
                    {
                        xFrom = int.Parse(match.Groups["xFrom"].Value),
                        yFrom = int.Parse(match.Groups["yFrom"].Value),
                        xTo = int.Parse(match.Groups["xTo"].Value),
                        yTo = int.Parse(match.Groups["yTo"].Value),
                    };
                }

                public bool IsHorizontal
                {
                    get { return yFrom == yTo; }
                }

                public bool IsVertical
                {
                    get { return xFrom == xTo; }
                }
            }
        }

        class Day6
        {
            public static void Solve()
            {
                var inputs = Helpers.LoadInput("input6.txt");
                var initialState = inputs[0].Split(',').Select(s => int.Parse(s)).ToArray();
                //var initialState = new[] { 3, 4, 3, 1, 2 };
                long[] spawningOn = new long[270];
                long current = initialState.Length;
                for (int i = 0; i < initialState.Length; i++)
                {
                    spawningOn[initialState[i] + 1]++;
                }

                long part1 = 0, part2 = 0;
                for (int day = 1; day <= 256; day++)
                {
                    current += spawningOn[day];
                    spawningOn[day + 7] += spawningOn[day];
                    spawningOn[day + 9] += spawningOn[day];

                    Console.WriteLine($"Day {day}, total {current}");

                    if (day == 80) part1 = current;
                    if (day == 256) part2 = current;
                }

                Console.WriteLine($"Part 1: {part1}");
                Console.WriteLine($"Part 2: {part2}");
            }
        }

        class Day7
        {
            public static void Solve()
            {
                var input = Helpers.LoadInput("input7.txt")
                    .First()
                    .Split(',')
                    .Select(s => int.Parse(s))
                    .ToArray();
                //input = new[] { 16, 1, 2, 0, 4, 2, 7, 1, 2, 14 };

                int[] sortedInput = input.OrderBy(i => i).ToArray();
                int cost;
                if ((sortedInput.Length % 2) == 1)
                {
                    var median = sortedInput[sortedInput.Length / 2];
                    cost = sortedInput.Sum(v => Math.Abs(v - median));
                }
                else
                {
                    int indexLow = sortedInput.Length / 2 - 1;
                    int indexHigh = sortedInput.Length / 2 - 1;
                    int costLow = sortedInput.Sum(v => Math.Abs(v - sortedInput[indexLow]));
                    int costHigh = sortedInput.Sum(v => Math.Abs(v - sortedInput[indexHigh]));
                    cost = Math.Min(costLow, costHigh);
                }

                Console.WriteLine($"Part 1: {cost}");

                // For part 2, cost is d*(d+1)/2; mean would be best for d*d, so looking near the mean
                var mean = (int)sortedInput.Average();
                long minCostPart2 = long.MaxValue;
                for (int i = Math.Max(0, mean - 3); i <= mean + 3; i++)
                {
                    var cost2 = sortedInput.Sum(value =>
                    {
                        var dist = Math.Abs(value - i);
                        return 1L * dist * (dist + 1) / 2;
                    });
                    if (cost2 < minCostPart2)
                    {
                        minCostPart2 = cost2;
                    }
                }

                Console.WriteLine($"Part 2: {minCostPart2}");
            }
        }

        class Day8
        {
            public static void Solve()
            {
                var testInput = new[]
                {
                    "be cfbegad cbdgef fgaecd cgeb fdcge agebfd fecdb fabcd edb | fdgacbe cefdb cefbgd gcbe",
                    "edbfga begcd cbg gc gcadebf fbgde acbgfd abcde gfcbed gfec | fcgedb cgb dgebacf gc",
                    "fgaebd cg bdaec gdafb agbcfd gdcbef bgcad gfac gcb cdgabef | cg cg fdcagb cbg",
                    "fbegcd cbd adcefb dageb afcb bc aefdc ecdab fgdeca fcdbega | efabcd cedba gadfec cb",
                    "aecbfdg fbg gf bafeg dbefa fcge gcbea fcaegb dgceab fcbdga | gecf egdcabf bgf bfgea",
                    "fgeab ca afcebg bdacfeg cfaedg gcfdb baec bfadeg bafgc acf | gebdcfa ecba ca fadegcb",
                    "dbcfg fgd bdegcaf fgec aegbdf ecdfab fbedc dacgb gdcebf gf | cefg dcbef fcge gbcadfe",
                    "bdfegc cbegaf gecbf dfcage bdacg ed bedf ced adcbefg gebcd | ed bcgafe cdgba cbgef",
                    "egadfb cdbfeg cegd fecab cgb gbdefca cg fgcdab egfdb bfceg | gbdfcae bgc cg cgb",
                    "gcafb gcf dcaebfg ecagb gf abcdeg gaef cafbge fdbac fegbdc | fgae cfgab fg bagce"
                };
                var input =
                    //testInput
                    Helpers.LoadInput("input8.txt")
                    .Select(line => Case.Parse(line))
                    .ToArray();

                var part1 = input.Sum(c => c.NumberOf1_4_7_8());
                Console.WriteLine($"Part 1: {part1}");

                var part2 = input.Sum(c => c.DecodedOutput());
                Console.WriteLine($"Part 2: {part2}");
            }

            public class Case
            {
                public string[] SignalPatterns;
                public string[] Outputs;
                private Dictionary<string, int> decoding;

                public Case(string[] patterns, string[] outputs)
                {
                    this.SignalPatterns = patterns;
                    this.Outputs = outputs;
                    this.DecodePatterns();
                }

                /// <summary>
                /// Patterns:
                /// 
                ///  aaaa     0: abc efg (6)
                /// b    c    1:   c  f  (2)
                /// b    c    2: a cde g (5)
                /// b    c    3: a cd fg (5)
                ///  dddd     4:  bcd f  (4)
                /// e    f    5: ab d fg (5)
                /// e    f    6: ab defg (6)
                /// e    f    7: a c  f  (3)
                ///  gggg     8: abcdefg (7)
                ///           9: abcd fg (6)
                /// </summary>
                private void DecodePatterns()
                {
                    this.decoding = new Dictionary<string, int>();
                    char a, b, c = ' ', d = ' ', e, f = ' ', g = ' ';
                    var one = this.SignalPatterns.First(p => p.Length == 2);
                    var seven = this.SignalPatterns.First(p => p.Length == 3);
                    var four = this.SignalPatterns.First(p => p.Length == 4);
                    var eight = this.SignalPatterns.First(p => p.Length == 7);
                    string zero = "", two = "", three = "", five = "", six = "", nine = "";

                    AddDecoding(one, 1);
                    AddDecoding(seven, 7);
                    AddDecoding(four, 4);
                    AddDecoding(eight, 8);

                    // Easy one: segment 'a'
                    a = seven.Except(one).First();
                    var cfPossibilities = one;
                    var bdPossibilities = string.Join("", four.Except(one));
                    var fiveSegments = this.SignalPatterns.Where(p => p.Length == 5).ToArray();
                    foreach (var fiveSegment in fiveSegments)
                    {
                        if (fiveSegment.Contains(bdPossibilities[0]) && fiveSegment.Contains(bdPossibilities[1]))
                        {
                            // It's a 5
                            five = fiveSegment;
                            AddDecoding(five, 5);
                            var fgPossibilities = fiveSegment.Except(bdPossibilities).Except(new[] { a }).ToArray();
                            if (fgPossibilities[0] == cfPossibilities[0] || fgPossibilities[0] == cfPossibilities[1])
                            {
                                f = fgPossibilities[0];
                                g = fgPossibilities[1];
                            }
                            else
                            {
                                f = fgPossibilities[1];
                                g = fgPossibilities[0];
                            }

                            c = cfPossibilities[0] == f ? cfPossibilities[1] : cfPossibilities[0];
                            break;
                        }
                    }

                    // At this point we know a, c, f, g; looking at the other 5-segment numbers (2, 3)
                    foreach (var fiveSegment in fiveSegments.Except(new[] { five }))
                    {
                        var notACFG = fiveSegment.Except(new[] { a, c, f, g }).ToArray();
                        if (notACFG.Length == 1)
                        {
                            // It's a 3, other segment is 'd'
                            d = notACFG[0];
                            three = fiveSegment;
                            AddDecoding(three, 3);
                            break;
                        }
                    }

                    two = fiveSegments.Except(new[] { three, five }).First();
                    AddDecoding(two, 2);
                    e = two.Except(new[] { a, c, d, f, g }).First();

                    // At this point we know a, c, d, e, f, g; getting b is trivial
                    b = eight.Except(new[] { a, c, d, e, f, g }).First();

                    var sixSegments = this.SignalPatterns.Where(p => p.Length == 6).ToArray();
                    zero = sixSegments.First(s => !s.Contains(d));
                    six = sixSegments.First(s => !s.Contains(c));
                    nine = sixSegments.First(s => !s.Contains(e));
                    AddDecoding(zero, 0);
                    AddDecoding(six, 6);
                    AddDecoding(nine, 9);
                }

                private void AddDecoding(string pattern, int value)
                {
                    this.decoding.Add(
                        string.Join("", pattern.OrderBy(c => c)),
                        value);
                }

                private int GetDecoding(string pattern)
                {
                    return this.decoding[string.Join("", pattern.OrderBy(c => c))];
                }

                public static Case Parse(string line)
                {
                    string[] parts = line.Split('|');
                    var patterns = parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var outputs = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    return new Case(patterns, outputs);
                }

                public int NumberOf1_4_7_8()
                {
                    int result = 0;
                    foreach (var output in this.Outputs)
                    {
                        switch (output.Length)
                        {
                            case 2:
                            case 3:
                            case 4:
                            case 7:
                                result++;
                                break;
                        }
                    }

                    return result;
                }

                public int DecodedOutput()
                {
                    return 1000 * GetDecoding(this.Outputs[0]) +
                        100 * GetDecoding(this.Outputs[1]) +
                        10 * GetDecoding(this.Outputs[2]) +
                        1 * GetDecoding(this.Outputs[3]);
                }
            }
        }

        class Day9
        {
            public static void Solve()
            {
                var testInput = new[]
                {
                    "2199943210",
                    "3987894921",
                    "9856789892",
                    "8767896789",
                    "9899965678"
                };
                var input =
                    //testInput
                    Helpers.LoadInput("input9.txt")
                    ;

                var lowPoints = new List<Tuple<int, int>>();
                int rows = input.Length;
                int cols = input[0].Length;
                int totalPoints = rows * cols;
                int numberOfNines = 0; // used to validate algorithm
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (input[i][j] == '9') numberOfNines++;
                        var isLow = true;
                        if (j > 0 && input[i][j] >= input[i][j - 1])
                        {
                            isLow = false;
                            continue;
                        }
                        if (j < cols - 1 && input[i][j] >= input[i][j + 1])
                        {
                            isLow = false;
                            continue;
                        }
                        if (i > 0 && input[i][j] >= input[i - 1][j])
                        {
                            isLow = false;
                            continue;
                        }
                        if (i < rows - 1 && input[i][j] >= input[i + 1][j])
                        {
                            isLow = false;
                            continue;
                        }

                        if (isLow)
                        {
                            //Console.WriteLine($"Low point: {i},{j}: {input[i][j]}");
                            lowPoints.Add(Tuple.Create(i, j));
                        }
                    }
                }

                Console.WriteLine($"Part 1: {lowPoints.Sum(p => input[p.Item1][p.Item2] - '0' + 1)}");

                var allBasinSizes = new List<int>();
                foreach (var lowPoint in lowPoints)
                {
                    var basinSize = 0;
                    var queue = new Queue<Tuple<int, int>>();
                    queue.Enqueue(lowPoint);
                    var basinPoints = new List<Tuple<int, int>>();
                    bool[,] visited = new bool[rows, cols];
                    visited[lowPoint.Item1, lowPoint.Item2] = true;
                    while (queue.Count > 0)
                    {
                        var next = queue.Dequeue();
                        basinPoints.Add(next);
                        basinSize++;
                        int i = next.Item1, j = next.Item2;
                        if (i > 0 && input[i][j] < input[i - 1][j] && input[i - 1][j] != '9' && !visited[i - 1, j])
                        {
                            visited[i - 1, j] = true;
                            queue.Enqueue(Tuple.Create(i - 1, j));
                        }
                        if (i < rows - 1 && input[i][j] < input[i + 1][j] && input[i + 1][j] != '9' && !visited[i + 1, j])
                        {
                            visited[i + 1, j] = true;
                            queue.Enqueue(Tuple.Create(i + 1, j));
                        }
                        if (j > 0 && input[i][j] < input[i][j - 1] && input[i][j - 1] != '9' && !visited[i, j - 1])
                        {
                            visited[i, j - 1] = true;
                            queue.Enqueue(Tuple.Create(i, j - 1));
                        }
                        if (j < cols - 1 && input[i][j] < input[i][j + 1] && input[i][j + 1] != '9' && !visited[i, j + 1])
                        {
                            visited[i, j + 1] = true;
                            queue.Enqueue(Tuple.Create(i, j + 1));
                        }
                    }

                    //Console.WriteLine($"Basin points ({basinPoints.Count}, {basinSize}): " + String.Join(" - ", basinPoints.Select(p => $"{p.Item1},{p.Item2}")));
                    allBasinSizes.Add(basinSize);
                }

                var top3 = allBasinSizes.OrderByDescending(s => s).Take(3).ToArray();
                var part2 = top3[0] * top3[1] * top3[2];
                Console.WriteLine($"Part 2: {part2}. Validation: {numberOfNines} + {allBasinSizes.Sum()} = {numberOfNines + allBasinSizes.Sum()}, should be {totalPoints}");
            }
        }

        class Day10
        {
            public static void Solve()
            {
                var testInput = new[]
                {
                    "[({(<(())[]>[[{[]{<()<>>",
                    "[(()[<>])]({[<{<<[]>>(",
                    "{([(<{}[<>[]}>{[]{[(<()>",
                    "(((({<>}<{<{<>}{[]{[]{}",
                    "[[<[([]))<([[{}[[()]]]",
                    "[{[{({}]{}}([{[{{{}}([]",
                    "{<[[]]>}<{[{[{[]{()[[[]",
                    "[<(<(<(<{}))><([]([]()",
                    "<{([([[(<>()){}]>(<<{{",
                    "<{([{{}}[<[[[<>{}]]]>[]]"
                };
                var useTestInput = false;
                var input = useTestInput ? testInput : Helpers.LoadInput("input10.txt");
                var invalidCharValues = new Dictionary<char, int>
                {
                    {')',3 }, {']', 57 },{ '}', 1197 }, {'>', 25137}
                };

                var part1 = input.Sum(line =>
                {
                    var c = FirstInvalidChar(line);
                    return c.HasValue ? invalidCharValues[c.Value] : 0;
                });
                Console.WriteLine($"Part 1: {part1}");

                var validInputs = input.Where(l => FirstInvalidChar(l) == null).ToArray();
                var validCharValues = new Dictionary<char, int>
                {
                    {')', 1 }, {']', 2 },{ '}', 3 }, {'>', 4 }
                };
                var validScores = new long[validInputs.Length];
                for (var i = 0; i < validInputs.Length; i++)
                {
                    var score = 0L;
                    var completionChars = GetCompletionChars(validInputs[i]);
                    foreach (var c in completionChars)
                    {
                        score *= 5;
                        score += validCharValues[c];
                    }

                    validScores[i] = score;
                }

                Array.Sort(validScores);
                var part2 = validScores[validScores.Length / 2];
                Console.WriteLine($"Part 2: {part2}");
            }

            static string GetCompletionChars(string line)
            {
                Stack<char> stack = new Stack<char>();
                string openChars = "[({<";
                string closeChars = "])}>";
                foreach (var c in line)
                {
                    if (openChars.Contains(c))
                    {
                        var correspondingCloseChar = closeChars[openChars.IndexOf(c)];
                        stack.Push(correspondingCloseChar);
                    }
                    else
                    {
                        stack.Pop();
                    }
                }

                var sb = new StringBuilder();
                while (stack.Count > 0)
                {
                    sb.Append(stack.Pop());
                }

                return sb.ToString();
            }

            static char? FirstInvalidChar(string line)
            {
                Stack<char> stack = new Stack<char>();
                string openChars = "[({<";
                string closeChars = "])}>";
                foreach (var c in line)
                {
                    if (openChars.Contains(c))
                    {
                        stack.Push(c);
                    }
                    else
                    {
                        if (stack.Count == 0) return c;
                        var expectedOpenChar = openChars[closeChars.IndexOf(c)];
                        var correspondingOpenChar = stack.Pop();
                        if (correspondingOpenChar != expectedOpenChar) return c;
                    }
                }

                return null;
            }
        }

        class Day11
        {
            public static void Solve()
            {
                var testInput1 = new[] { "11111", "19991", "19191", "19991", "11111" };
                var testInput2 = new[]
                {
                    "5483143223",
                    "2745854711",
                    "5264556173",
                    "6141336146",
                    "6357385478",
                    "4167524645",
                    "2176841721",
                    "6882881134",
                    "4846848554",
                    "5283751526"
                };
                var inputToUse = 3;
                var input = inputToUse == 1 ? testInput1 : (inputToUse == 2 ? testInput2 : Helpers.LoadInput("input11.txt"));
                Board board = new Board(input);
                for (int i = 0; i < 100; i++)
                {
                    //Console.WriteLine($"Iteration {i} (current flash count = {board.FlashCount})");
                    //board.Print();
                    board.Iterate();
                }
                Console.WriteLine($"Part 1: {board.FlashCount}");

                int prevCount = board.FlashCount;
                int currentStep = 100;
                while (true)
                {
                    board.Iterate();
                    currentStep++;
                    if (board.FlashCount == prevCount + 100)
                    {
                        Console.WriteLine($"Part 2: {currentStep}");
                        break;
                    }

                    prevCount = board.FlashCount;
                }
            }

            class Board
            {
                private int rows;
                private int cols;
                private int[,] energyLevels;
                public int FlashCount { get; private set; }
                public Board(string[] initialState)
                {
                    this.rows = initialState.Length;
                    this.cols = initialState[0].Length;
                    this.energyLevels = new int[rows, cols];
                    for (int i = 0; i < this.rows; i++)
                    {
                        for (int j = 0; j < this.cols; j++)
                        {
                            this.energyLevels[i, j] = initialState[i][j] - '0';
                        }
                    }
                }
                public void Print()
                {
                    for (int i = 0; i < this.rows; i++)
                    {
                        for (int j = 0; j < this.cols; j++)
                        {
                            Console.Write(this.energyLevels[i, j]);
                        }

                        Console.WriteLine();
                    }
                }
                public void Iterate()
                {
                    Queue<Tuple<int, int>> toIncrease = new Queue<Tuple<int, int>>();
                    for (int i = 0; i < this.rows; i++)
                    {
                        for (int j = 0; j < this.cols; j++)
                        {
                            this.energyLevels[i, j]++;
                            if (this.energyLevels[i, j] == 10)
                            {
                                this.FlashCount++;
                                toIncrease.Enqueue(Tuple.Create(i, j));
                            }
                        }
                    }

                    while (toIncrease.Count > 0)
                    {
                        var next = toIncrease.Dequeue();
                        int row = next.Item1, col = next.Item2;
                        for (int i = Math.Max(0, row - 1); i <= Math.Min(row + 1, this.rows - 1); i++)
                        {
                            if (i < 0 || i >= this.rows) continue;
                            for (int j = Math.Max(0, col - 1); j <= Math.Min(col + 1, this.cols - 1); j++)
                            {
                                if (i == row && j == col) continue;
                                this.energyLevels[i, j]++;
                                if (this.energyLevels[i, j] == 10)
                                {
                                    this.FlashCount++;
                                    toIncrease.Enqueue(Tuple.Create(i, j));
                                }
                            }
                        }
                    }

                    for (int i = 0; i < this.rows; i++)
                    {
                        for (int j = 0; j < this.cols; j++)
                        {
                            if (this.energyLevels[i, j] > 9)
                            {
                                this.energyLevels[i, j] = 0;
                            }
                        }
                    }
                }
            }
        }

        class Day12
        {
            public static void Solve()
            {
                var testInput1 = new[]
                {
                    "start-A",
                    "start-b",
                    "A-c",
                    "A-b",
                    "b-d",
                    "A-end",
                    "b-end"
                };

                var testInput2 = new[]
                {
                    "dc-end",
                    "HN-start",
                    "start-kj",
                    "dc-start",
                    "dc-HN",
                    "LN-dc",
                    "HN-end",
                    "kj-sa",
                    "kj-HN",
                    "kj-dc"
                };

                var testInput3 = new[]
                {
                    "fs-end",
                    "he-DX",
                    "fs-he",
                    "start-DX",
                    "pj-DX",
                    "end-zg",
                    "zg-sl",
                    "zg-pj",
                    "pj-he",
                    "RW-he",
                    "fs-DX",
                    "pj-RW",
                    "zg-RW",
                    "start-pj",
                    "he-WI",
                    "zg-he",
                    "pj-fs",
                    "start-RW"
                };

                var inputToUse = 4;
                string[] input = null;
                switch (inputToUse)
                {
                    case 1: input = testInput1; break;
                    case 2: input = testInput2; break;
                    case 3: input = testInput3; break;
                    default: input = Helpers.LoadInput("input12.txt"); break;
                }

                var paths = new Dictionary<string, List<string>>();
                foreach (var line in input)
                {
                    var parts = line.Split('-');
                    var end1 = parts[0];
                    var end2 = parts[1];
                    if (!paths.ContainsKey(end1)) paths.Add(end1, new List<string>());
                    if (!paths.ContainsKey(end2)) paths.Add(end2, new List<string>());
                    paths[end1].Add(end2);
                    paths[end2].Add(end1);
                }

                int part1 = 0;
                foreach (var startPath in paths["start"])
                {
                    HashSet<string> visited = new HashSet<string>();
                    visited.Add("start");
                    part1 += CountPathsToEnd(startPath, paths, visited);
                }

                Console.WriteLine($"Part 1: {part1}");

                int part2 = 0;
                var lowerCaseCaves = paths.Keys.Where(c => c != "start" && c != "end" && char.IsLower(c[0])).ToArray();

                foreach (var startPath in paths["start"])
                {
                    HashSet<string> visited = new HashSet<string>();
                    visited.Add("start");
                    part2 += CountPathsToEnd2(startPath, paths, false, $"start-{startPath}-");
                }

                Console.WriteLine($"Part 2: {part2}");
            }

            static int CountPathsToEnd2(string cave, Dictionary<string, List<string>> paths, bool visitedLowerTwice, string path)
            {
                int result = 0;
                foreach (var nextCave in paths[cave])
                {
                    if (nextCave == "end")
                    {
                        //Console.WriteLine($"Path: {path}end");
                        result++;
                        continue;
                    }

                    if (nextCave == "start")
                    {
                        continue;
                    }

                    bool canVisit;
                    bool secondLowerVisit = false;
                    if (char.IsUpper(nextCave[0]))
                    {
                        canVisit = true;
                    }
                    else
                    {
                        bool alreadyVisited = path.Contains("-" + nextCave + "-");
                        if (!alreadyVisited)
                        {
                            canVisit = true;
                        }
                        else
                        {
                            canVisit = !visitedLowerTwice;
                            secondLowerVisit = true;
                        }
                    }

                    if (canVisit)
                    {
                        result += CountPathsToEnd2(nextCave, paths, secondLowerVisit || visitedLowerTwice, path + nextCave + "-");
                    }
                }

                return result;
            }

            static int CountPathsToEnd(string cave, Dictionary<string, List<string>> paths, HashSet<string> visited)
            {
                int result = 0;
                foreach (var nextCave in paths[cave])
                {
                    if (nextCave == "end")
                    {
                        result++;
                        continue;
                    }

                    if (!visited.Contains(nextCave))
                    {
                        if (!char.IsUpper(cave[0]))
                        {
                            visited.Add(cave);
                        }

                        result += CountPathsToEnd(nextCave, paths, visited);

                        if (!char.IsUpper(cave[0]))
                        {
                            visited.Remove(cave);
                        }
                    }
                }

                return result;
            }
        }

        class Day13
        {
            public static void Solve()
            {
                var testInput = new[]
                {
                    "6,10",
                    "0,14",
                    "9,10",
                    "0,3",
                    "10,4",
                    "4,11",
                    "6,0",
                    "6,12",
                    "4,1",
                    "0,13",
                    "10,12",
                    "3,4",
                    "3,0",
                    "8,4",
                    "1,10",
                    "2,14",
                    "8,10",
                    "9,0",
                    "",
                    "fold along y=7",
                    "fold along x=5"
                };
                var useTestInput = false;
                var input = useTestInput ? testInput : Helpers.LoadInput("input13.txt");
                ParseInput(input, out var points, out var folds);

                var board = points;
                int foldCount = 0;
                foreach (var fold in folds)
                {
                    var toRemove = new List<int>();
                    for (var i = 0; i < board.Count; i++)
                    {
                        int x = board[i].X;
                        int y = board[i].Y;
                        int foldAt = fold.Item2;
                        if (fold.Item1 == 'y')
                        {
                            if (y > foldAt)
                            {
                                var newPoint = new Coord(x, 2 * foldAt - y);
                                if (board.Contains(newPoint))
                                {
                                    toRemove.Add(i);
                                }
                                else
                                {
                                    board[i] = newPoint;
                                }
                            }
                        }
                        else
                        {
                            if (x > foldAt)
                            {
                                var newPoint = new Coord(2 * foldAt - x, y);
                                if (board.Contains(newPoint))
                                {
                                    toRemove.Add(i);
                                }
                                else
                                {
                                    board[i] = newPoint;
                                }
                            }
                        }
                    }

                    for (var i = toRemove.Count - 1; i >= 0; i--)
                    {
                        board.RemoveAt(toRemove[i]);
                    }

                    foldCount++;
                    Console.WriteLine($"After {foldCount} folds, {board.Count} dots are visible");
                }

                int maxX = board.Max(c => c.X);
                int maxY = board.Max(c => c.Y);
                for (int y = 0; y <= maxY; y++)
                {
                    for (int x = 0; x <= maxX; x++)
                    {
                        if (board.Contains(new Coord(x, y)))
                        {
                            Console.Write("#");
                        }
                        else
                        {
                            Console.Write(".");
                        }
                    }

                    Console.WriteLine();
                }
            }

            [DebuggerDisplay("Coord({X},{Y})")]
            class Coord
            {
                public int X { get; private set; }
                public int Y { get; private set; }
                public Coord(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }
                public override bool Equals(object obj)
                {
                    return obj is Coord && this.X == ((Coord)obj).X && this.Y == ((Coord)obj).Y;
                }
                public override int GetHashCode()
                {
                    return (this.X * 10000 + this.Y).GetHashCode();
                }
            }

            static void ParseInput(string[] input, out List<Coord> points, out List<Tuple<char, int>> folds)
            {
                int i = 0;
                points = new List<Coord>();
                folds = new List<Tuple<char, int>>();
                while (!string.IsNullOrEmpty(input[i]))
                {
                    var parts = input[i].Split(',');
                    points.Add(new Coord(int.Parse(parts[0]), int.Parse(parts[1])));
                    i++;
                }

                i++;
                while (i < input.Length)
                {
                    var fold = input[i].Substring("fold along ".Length);
                    var parts = fold.Split('=');
                    folds.Add(Tuple.Create(parts[0][0], int.Parse(parts[1])));
                    i++;
                }
            }
        }

        class Day14
        {
            public static void Solve()
            {
                var testInput = new[]
                {
                    "NNCB",
                    "",
                    "CH -> B",
                    "HH -> N",
                    "CB -> H",
                    "NH -> C",
                    "HB -> C",
                    "HC -> B",
                    "HN -> C",
                    "NN -> C",
                    "BH -> H",
                    "NC -> B",
                    "NB -> B",
                    "BN -> B",
                    "BB -> N",
                    "BC -> B",
                    "CC -> N",
                    "CN -> C"
                };
                var useTestInput = false;
                var input = useTestInput ? testInput : Helpers.LoadInput("input14.txt");
                var template = input[0];
                var rules = input.Skip(2).ToDictionary(k => k.Substring(0, 2), v => v[6]);

                //var sb = new StringBuilder(template);
                //for (int i = 1; i <= 10; i++)
                //{
                //    int j = 1;
                //    while (j < sb.Length)
                //    {
                //        var s = new string(new[] { sb[j - 1], sb[j] });
                //        if (rules.ContainsKey(s))
                //        {
                //            sb.Insert(j, rules[s]);
                //            j += 2;
                //        }
                //        else
                //        {
                //            j++;
                //        }
                //    }

                //    Console.WriteLine($"After {i} steps: {sb.Length}");
                //}

                //var counts = new Dictionary<char, int>();
                //for (int i = 0; i < sb.Length; i++)
                //{
                //    char c = sb[i];
                //    if (counts.ContainsKey(c))
                //    {
                //        counts[c]++;
                //    }
                //    else
                //    {
                //        counts.Add(c, 1);
                //    }
                //}
                //var values = counts.Values.OrderBy(v => v).ToArray();
                //Console.WriteLine($"Part 1: {values[values.Length - 1] - values[0]}");

                // For part 2 we cannot go brute force; storing pairs instead
                var pairs = new Dictionary<string, long>();
                Action<Dictionary<string, long>, string, long> insert = (dic, pair, quantity) =>
                {
                    if (dic.ContainsKey(pair))
                    {
                        dic[pair] += quantity;
                    }
                    else
                    {
                        dic.Add(pair, quantity);
                    }
                };

                for (int i = 0; i < template.Length - 1; i++)
                {
                    var pair = template.Substring(i, 2);
                    insert(pairs, pair, 1);
                }

                for (int i = 0; i < 40; i++)
                {
                    var newPairs = new Dictionary<string, long>();
                    foreach (var pair in pairs.Keys)
                    {
                        var mid = rules[pair];
                        var pair1 = $"{pair[0]}{mid}";
                        var pair2 = $"{mid}{pair[1]}";
                        insert(newPairs, pair1, pairs[pair]);
                        insert(newPairs, pair2, pairs[pair]);
                    }

                    pairs = newPairs;

                    var letterCounts = new Dictionary<char, long>();
                    letterCounts.Add(template[0], 1);
                    foreach (var pair in pairs.Keys)
                    {
                        char c = pair[1];
                        if (letterCounts.ContainsKey(c))
                        {
                            letterCounts[c] += pairs[pair];
                        }
                        else
                        {
                            letterCounts.Add(c, pairs[pair]);
                        }
                    }

                    var letterCountStr = string.Join(
                        ", ",
                        letterCounts.Keys.OrderBy(k => k).Select(c => $"{c}:{letterCounts[c]}"));
                    var onlyCounts = letterCounts.Values.OrderBy(v => v).ToArray();
                    var maxMinusMin = onlyCounts[onlyCounts.Length - 1] - onlyCounts[0];
                    Console.WriteLine($"After step {i + 1}, letter counts: {letterCountStr} ({letterCounts.Values.Sum()}) - {maxMinusMin}");
                }
            }
        }

        class Day15
        {
            public static void Solve()
            {
                var testInput = new[]
                {
                    "1163751742",
                    "1381373672",
                    "2136511328",
                    "3694931569",
                    "7463417111",
                    "1319128137",
                    "1359912421",
                    "3125421639",
                    "1293138521",
                    "2311944581"
                };
                var useTestInput = false;
                var input =
                    (useTestInput ? testInput : Helpers.LoadInput("input15.txt"))
                    .Select(row => row.Select(c => c - '0').ToArray())
                    .ToArray();
                int rows = input.Length;
                int cols = input[0].Length;

                PriorityQueue queue = new PriorityQueue();
                queue.Enqueue(0, 0, 0);
                var visited = new bool[rows, cols];
                while (!queue.IsEmpty)
                {
                    var nextNode = queue.Dequeue();
                    var row = nextNode.Row;
                    var col = nextNode.Col;
                    if (visited[row, col])
                    {
                        // Already visited it in a cheaper configuration
                        continue;
                    }

                    visited[row, col] = true;
                    if (row == rows - 1 && col == cols - 1)
                    {
                        Console.WriteLine($"Part 1: {nextNode.Priority}");
                        break;
                    }

                    // Visit neighbors
                    if (row > 0 && !visited[row - 1, col])
                    {
                        queue.Enqueue(nextNode.Priority + input[row - 1][col], row - 1, col);
                    }
                    if (row < rows - 1 && !visited[row + 1, col])
                    {
                        queue.Enqueue(nextNode.Priority + input[row + 1][col], row + 1, col);
                    }
                    if (col > 0 && !visited[row, col - 1])
                    {
                        queue.Enqueue(nextNode.Priority + input[row][col - 1], row, col - 1);
                    }
                    if (col < cols - 1 && !visited[row, col + 1])
                    {
                        queue.Enqueue(nextNode.Priority + input[row][col + 1], row, col + 1);
                    }
                }

                var part2Rows = rows * 5;
                var part2Cols = cols * 5;
                queue = new PriorityQueue();
                queue.Enqueue(0, 0, 0);
                visited = new bool[part2Rows, part2Cols];
                while (!queue.IsEmpty)
                {
                    var nextNode = queue.Dequeue();
                    var row = nextNode.Row;
                    var col = nextNode.Col;
                    if (visited[row, col])
                    {
                        // Already visited it in a cheaper configuration
                        continue;
                    }

                    visited[row, col] = true;
                    if (row == part2Rows - 1 && col == part2Cols - 1)
                    {
                        Console.WriteLine($"Part 2: {nextNode.Priority}");
                        break;
                    }

                    Func<int, int, int, int> getUpdatedValue = (int r, int c, int v) =>
                    {
                        var increment = r / rows + c / cols;
                        var newValue = v + increment;
                        while (newValue > 9)
                        {
                            newValue -= 9;
                        }

                        return newValue;
                    };

                    // Visit neighbors
                    if (row > 0 && !visited[row - 1, col])
                    {
                        queue.Enqueue(nextNode.Priority + getUpdatedValue(row - 1, col, input[(row - 1) % rows][col % cols]), row - 1, col);
                    }
                    if (row < part2Rows - 1 && !visited[row + 1, col])
                    {
                        queue.Enqueue(nextNode.Priority + getUpdatedValue(row + 1, col, input[(row + 1) % rows][col % cols]), row + 1, col);
                    }
                    if (col > 0 && !visited[row, col - 1])
                    {
                        queue.Enqueue(nextNode.Priority + getUpdatedValue(row, col - 1, input[row % rows][(col - 1) % cols]), row, col - 1);
                    }
                    if (col < part2Cols - 1 && !visited[row, col + 1])
                    {
                        queue.Enqueue(nextNode.Priority + getUpdatedValue(row, col + 1, input[row % rows][(col + 1) % cols]), row, col + 1);
                    }
                }
            }

            class Node
            {
                public int Priority;
                public Node Next;
                public int Row;
                public int Col;

                public Node(int priority, int row, int col, Node next = null)
                {
                    this.Priority = priority;
                    this.Row = row;
                    this.Col = col;
                    this.Next = next;
                }
            }

            class PriorityQueue
            {
                private Node head;
                public PriorityQueue()
                {
                    this.head = null;
                }
                public bool IsEmpty => this.head == null;
                public void Enqueue(int priority, int row, int col)
                {
                    if (this.head == null)
                    {
                        this.head = new Node(priority, row, col);
                        return;
                    }

                    if (this.head.Priority >= priority)
                    {
                        this.head = new Node(priority, row, col, this.head);
                        return;
                    }
                    Node prev = this.head;
                    Node next = this.head.Next;
                    while (next != null && next.Priority < priority)
                    {
                        prev = next;
                        next = next.Next;
                    }

                    prev.Next = new Node(priority, row, col, next);
                }
                public Node Dequeue()
                {
                    if (this.head == null) throw new InvalidOperationException();
                    var result = this.head;
                    this.head = this.head.Next;
                    return result;
                }
            }
        }

        class Day16
        {
            public static void Solve()
            {
                var testInput1 = "D2FE28"; // Lit(V6)
                var testInput2 = "38006F45291200"; // Op(V1, I0, Lit(, 10), Lit(, 20))
                var testInput3 = "EE00D40C823060"; // Op(V7, I1, Lit(, 1), Lit(, 2), Lit(, 3))
                var testInput4 = "8A004A801A8002F478"; // Op(V4, Op(V4, Lit(V6))) - vsum=16
                var testInput5 = "620080001611562C8802118E34"; // Op(V3, I1, Op(, , Lit(, ), Lit(, )), Op(, , Lit(, ), Lit(, ))) - vsum=12
                var testInput6 = "C0015000016115A2E0802F182340"; // Op(V6, I0, Op(, , Lit(, ), Lit(, )), Op(, , Lit(, ), Lit(, ))) - vsum=23
                var testInput7 = "A0016C880162017C3686B18A3D4780"; // Op(, , Op(, , Lit(, ), Lit(, ), Lit(, ), Lit(, ), Lit(, )) - vsum=31
                var testInputs = new[] { testInput1, testInput2, testInput3, testInput4, testInput5, testInput6, testInput7 };
                var inputToUse = 8;
                string input = HexToBinary(inputToUse <= 7 ? testInputs[inputToUse - 1] : Helpers.LoadInput("input16.txt")[0]);

                var packet = Packet.Parse(input);
                int part1 = SumVersions(packet);
                Console.WriteLine($"Part 1: {part1}");

                Console.WriteLine($"Part 2: {packet.Evaluate()}");
            }

            static int SumVersions(Packet packet)
            {
                int result = packet.Version;
                if (packet.IsOperator)
                {
                    result += packet.As<OperatorPacket>().Children.Sum(p => SumVersions(p));
                }

                return result;
            }

            static Dictionary<char, string> hexValues = new Dictionary<char, string>
            {
                { '0', "0000" }, { '1', "0001" }, { '2', "0010" }, { '3', "0011" },
                { '4', "0100" }, { '5', "0101" }, { '6', "0110" }, { '7', "0111" },
                { '8', "1000" }, { '9', "1001" }, { 'A', "1010" }, { 'B', "1011" },
                { 'C', "1100" }, { 'D', "1101" }, { 'E', "1110" }, { 'F', "1111" },
            };

            static string HexToBinary(string hex)
            {
                return string.Join("", hex.Select(h => hexValues[h]));
            }

            public abstract class Packet
            {
                public int Version { get; private set; }
                public virtual bool IsLiteral => false;
                public virtual bool IsOperator => false;
                public Packet(int version)
                {
                    this.Version = version;
                }

                public static Packet Parse(string binary)
                {
                    int index = 0;
                    return Parse(binary, ref index);
                }

                private static Packet Parse(string binary, ref int index)
                {
                    int version = ReadBinaryValue(binary, 3, ref index);
                    int type = ReadBinaryValue(binary, 3, ref index);
                    if (type == 4)
                    {
                        // Literal value
                        long value = 0;
                        bool isLast;
                        do
                        {
                            value *= 16;
                            isLast = ReadBinaryValue(binary, 1, ref index) == 0;
                            int nibble = ReadBinaryValue(binary, 4, ref index);
                            value += nibble;
                        } while (!isLast);

                        return new LiteralPacket(version, value);
                    }
                    else
                    {
                        // Operator
                        int id = ReadBinaryValue(binary, 1, ref index);
                        List<Packet> children = new List<Packet>();
                        if (id == 0)
                        {
                            var totalLength = ReadBinaryValue(binary, 15, ref index);
                            int currentIndex = index;
                            while (index - currentIndex < totalLength)
                            {
                                children.Add(Parse(binary, ref index));
                            }
                        }
                        else
                        {
                            var childCount = ReadBinaryValue(binary, 11, ref index);
                            for (int i = 0; i < childCount; i++)
                            {
                                children.Add(Parse(binary, ref index));
                            }
                        }

                        return new OperatorPacket(version, type, id, children);
                    }
                }

                static int ReadBinaryValue(string binary, int bits, ref int index)
                {
                    int result = 0;
                    for (int i = 0; i < bits; i++)
                    {
                        result *= 2;
                        result += binary[index] - '0';
                        index++;
                    }

                    return result;
                }

                public T As<T>() where T : Packet
                {
                    return (T)this;
                }

                public abstract long Evaluate();
            }

            [DebuggerDisplay("Literal[Version={Version},Value={Value}]")]
            public class LiteralPacket : Packet
            {
                public override bool IsLiteral => true;
                public long Value { get; private set; }
                public LiteralPacket(int version, long value) : base(version)
                {
                    this.Value = value;
                }

                public override long Evaluate()
                {
                    return this.Value;
                }
            }

            [DebuggerDisplay("Operator[Version={Version},Type={Type},Id={Id},Children={ChildCount}]")]
            public class OperatorPacket : Packet
            {
                public override bool IsOperator => true;
                public int Type { get; private set; }
                public int Id { get; private set; }
                private List<Packet> children;
                public IEnumerable<Packet> Children => this.children.AsEnumerable();
                public int ChildCount => this.children.Count;
                public OperatorPacket(int version, int type, int id, IEnumerable<Packet> children) : base(version)
                {
                    this.Type = type;
                    this.Id = id;
                    this.children = children.ToList();
                }

                public override long Evaluate()
                {
                    switch (this.Type)
                    {
                        case 0: return this.children.Sum(p => p.Evaluate());
                        case 1:
                            long prod = 1;
                            foreach (var child in this.children)
                            {
                                prod *= child.Evaluate();
                            }

                            return prod;
                        case 2: return this.children.Min(p => p.Evaluate());
                        case 3: return this.children.Max(p => p.Evaluate());
                        case 5: return this.children[0].Evaluate() > this.children[1].Evaluate() ? 1 : 0;
                        case 6: return this.children[0].Evaluate() < this.children[1].Evaluate() ? 1 : 0;
                        case 7: return this.children[0].Evaluate() == this.children[1].Evaluate() ? 1 : 0;
                        default: throw new InvalidOperationException();
                    }
                }
            }
        }

        class Day17
        {
            public static void Solve()
            {
                var testInput = "target area: x=20..30, y=-10..-5";
                var realInput = "target area: x=206..250, y=-105..-57";
                var useTestInput = false;
                var inputRegex = new Regex(@"x=(\d+)\.\.(\d+), y=([\-\d]+)\.\.([\-\d]+)");
                var inputMatch = inputRegex.Match(useTestInput ? testInput : realInput);
                var xMin = int.Parse(inputMatch.Groups[1].Value);
                var xMax = int.Parse(inputMatch.Groups[2].Value);
                var yMin = int.Parse(inputMatch.Groups[3].Value);
                var yMax = int.Parse(inputMatch.Groups[4].Value);

                // Part 1: math only: the higher y velocity will be the one that after going
                //   up will go down and hit the target area (assuming that there is an initial
                //   x velocity that will reach 0 by the area
                // If there are integers that satisfy vx(vx+1)/2 between 206..250, we can use it,
                //   which is the case (20 or 21)
                // For y, it will start with vy, and will pass by y=0 downwards with v=-vy, and
                //   the next step will be (-vy+1). The higher value that will reach the target is
                //   then 104 (-105 after passing by 0). The higher point is given again by
                //   vy(vy+1)/2 = 5460
                Console.WriteLine($"Part 1: {(useTestInput ? 45 : 5460)}");

                // Part 2: - smallest value of vx that will reach the target: 20 (or 6 for test input)
                //   - largest value of vx: xMax (reaching in 1 step)
                //   - smallest value of vy: yMin
                //   - highest value of vy (-yMin-1)
                var allVelocities = new List<string>();
                int maxHeight = 0;
                for (int vx0 = (useTestInput ? 6 : 20); vx0 <= xMax; vx0++)
                {
                    for (int vy0 = yMin; vy0 < -yMin; vy0++)
                    {
                        int vx = vx0;
                        int vy = vy0;
                        int x = 0, y = 0;
                        int maxHeightThisVelocity = int.MinValue;
                        while (x <= xMax && y >= yMin)
                        {
                            if (y > maxHeightThisVelocity) maxHeightThisVelocity = y;
                            if (xMin <= x && x <= xMax && yMin <= y && y <= yMax)
                            {
                                if (maxHeightThisVelocity > maxHeight) maxHeight = maxHeightThisVelocity;
                                allVelocities.Add($"{vx0},{vy0}");
                                break;
                            }

                            x += vx;
                            y += vy;
                            if (vx > 0) vx--;
                            vy--;
                        }
                    }
                }

                Console.WriteLine($"Part 2: {allVelocities.Count} (max height = {maxHeight})");
            }
        }

        class Day18
        {
            public static void Solve()
            {
                //var reduceTests = new List<Tuple<string, string>> {
                //    Tuple.Create("[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]"),
                //    Tuple.Create("[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]"),
                //    Tuple.Create("[[6,[5,[4,[3,2]]]],1]", "[[6,[5,[7,0]]],3]"),
                //    Tuple.Create("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]"),
                //};

                //foreach (var test in reduceTests)
                //{
                //    Console.WriteLine("Testing Reduce algo 1");
                //    var result = Reduce(test.Item1);
                //    if (result == test.Item2)
                //    {
                //        Console.WriteLine("  PASS");
                //    }
                //    else
                //    {
                //        Console.WriteLine("  FAIL - expected: " + test.Item2);
                //    }

                //    Console.WriteLine("Testing Explode algo 2");
                //    var node = Node.Parse(test.Item1);
                //    node.Reduce();
                //    if (node.ToString() == test.Item2)
                //    {
                //        Console.WriteLine("  PASS");
                //    }
                //    else
                //    {
                //        Console.WriteLine("  FAIL - expected: " + test.Item2);
                //    }
                //}

                //var sumTests = new[] {
                //    "[[[[4,3],4],4],[7,[[8,4],9]]]+[1,1]=[[[[0,7],4],[[7,8],[6,0]]],[8,1]]",
                //    "[1,1]+[2,2]+[3,3]+[4,4]=[[[[1,1],[2,2]],[3,3]],[4,4]]",
                //    "[1,1]+[2,2]+[3,3]+[4,4]+[5,5]=[[[[3,0],[5,3]],[4,4]],[5,5]]",
                //    "[1,1]+[2,2]+[3,3]+[4,4]+[5,5]+[6,6]=[[[[5,0],[7,4]],[5,5]],[6,6]]",
                //    "[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]+[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]+[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]+[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]+[7,[5,[[3,8],[1,4]]]]+[[2,[2,2]],[8,[8,1]]]+[2,9]+[1,[[[9,3],9],[[9,0],[0,7]]]]+[[[5,[7,4]],7],1]+[[[[4,2],2],6],[8,7]]=[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]"
                //};
                //foreach (var sumTest in sumTests)
                //{
                //    var split1 = sumTest.Split('=');
                //    var expected = split1[1];
                //    Console.WriteLine(split1[0]);
                //    var terms = split1[0].Split('+').Select(t => Node.Parse(t)).ToList();
                //    while (terms.Count > 1)
                //    {
                //        var sum = Node.Add(terms[0], terms[1]);
                //        terms.RemoveAt(1);
                //        terms[0] = sum;
                //    }

                //    if (terms[0].ToString() == expected)
                //    {
                //        Console.WriteLine("  PASS");
                //    }
                //    else
                //    {
                //        Console.WriteLine("  FAIL - expected " + expected);
                //    }
                //}

                var testInput = new[]
                {
                    "[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]",
                    "[[[5,[2,8]],4],[5,[[9,9],0]]]",
                    "[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]",
                    "[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]",
                    "[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]",
                    "[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]",
                    "[[[[5,4],[7,7]],8],[[8,3],8]]",
                    "[[9,3],[[9,9],[6,[4,9]]]]",
                    "[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]",
                    "[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]"
                };

                var useTestInput = false;
                var input = (useTestInput ? testInput : Helpers.LoadInput("input18.txt"))
                    .Select(l => Node.Parse(l))
                    .ToList();

                var part1Input = input;
                while (part1Input.Count > 1)
                {
                    var sum = Node.Add(part1Input[0], part1Input[1]);
                    part1Input.RemoveAt(0);
                    part1Input[0] = sum;
                }

                Console.WriteLine($"Part 1 result: {part1Input[0]} with magnitude {part1Input[0].Magnitude()}");

                var part2Input = (useTestInput ? testInput : Helpers.LoadInput("input18.txt"))
                    .Select(l => Node.Parse(l))
                    .ToList();
                long maxMagnitude = long.MinValue;
                for (int i = 0; i < part2Input.Count - 1; i++)
                {
                    for (int j = i + 1; j < part2Input.Count; j++)
                    {
                        var sum1 = Node.Add(part2Input[i], part2Input[j]);
                        var sum2 = Node.Add(part2Input[j], part2Input[i]);
                        var mag1 = sum1.Magnitude();
                        var mag2 = sum2.Magnitude();
                        if (mag1 > maxMagnitude) maxMagnitude = mag1;
                        if (mag2 > maxMagnitude) maxMagnitude = mag2;
                    }
                }
                Console.WriteLine($"Part 2: {maxMagnitude}");
            }

            private static string Add(string number1, string number2)
            {
                var addition = $"[{number1},{number2}]";
                return Reduce(addition);
            }

            private static string Reduce(string number)
            {
                bool keepReducing = true;
                Console.WriteLine("Reducing " + number);
                while (keepReducing)
                {
                    keepReducing = false;
                    int depth = 0;
                    for (int i = 0; i < number.Length; i++)
                    {
                        if (number[i] == '[')
                        {
                            depth++;
                            if (depth > 4)
                            {
                                // next should be a pair of numbers
                                keepReducing = true;
                                var newNumber = "";
                                AssertPair(number, i + 1);
                                var first = number[i + 1] - '0';
                                var second = number[i + 3] - '0';
                                int j;
                                for (j = i - 1; j >= 0; j--)
                                {
                                    if (char.IsDigit(number[j]))
                                    {
                                        first += number[j] - '0';
                                        break;
                                    }
                                }

                                if (j < 0)
                                {
                                    newNumber = number.Substring(0, i);
                                }
                                else
                                {
                                    newNumber = number.Substring(0, j) + first + number.Substring(j + 1, i - (j + 1));
                                }

                                newNumber += '0';

                                for (j = i + 5; j < number.Length; j++)
                                {
                                    if (char.IsDigit(number[j]))
                                    {
                                        second += number[j] - '0';
                                        break;
                                    }
                                }

                                if (j == number.Length)
                                {
                                    newNumber += number.Substring(i + 5);
                                }
                                else
                                {
                                    newNumber += number.Substring(i + 5, j - (i + 5)) + second + number.Substring(j + 1);
                                }

                                number = newNumber;
                                Console.WriteLine("Exploded to " + number);
                                break;
                            }
                        }
                        else if (number[i] == ']')
                        {
                            depth--;
                        }
                        else if (char.IsDigit(number[i]))
                        {
                            if (char.IsDigit(number[i + 1]))
                            {
                                keepReducing = true;
                                var numberValue = (number[i] - '0') + number[i + 1] - '0';
                                var left = numberValue / 2;
                                var right = (numberValue + 1) / 2;
                                number = number.Substring(0, i) +
                                    '[' + left + ',' + right + ']' +
                                    number.Substring(i + 2);
                                Console.WriteLine("Split to " + number);
                                break;
                            }
                        }
                    }
                }

                return number;
            }

            static void AssertPair(string number, int index)
            {
                if (char.IsDigit(number[index]) && number[index + 1] == ',' && char.IsDigit(number[index + 2]) && number[index + 3] == ']')
                {
                    // all good
                }
                else
                {
                    throw new Exception($"Not a pair for '{number}' at index {index}");
                }
            }

            class Node
            {
                private long _value;
                private Node _left;
                private Node _right;
                public bool IsLiteral { get; private set; }
                public Node Parent { get; private set; }
                public int Depth { get; private set; }
                public long Value
                {
                    get
                    {
                        if (!this.IsLiteral) throw new InvalidOperationException();
                        return this._value;
                    }
                }

                public Node Left
                {
                    get
                    {
                        if (this.IsLiteral) throw new InvalidOperationException();
                        return this._left;
                    }
                }

                public Node Right
                {
                    get
                    {
                        if (this.IsLiteral) throw new InvalidOperationException();
                        return this._right;
                    }
                }

                public Node(long value)
                {
                    this.IsLiteral = true;
                    this._value = value;
                }

                public Node(Node left, Node right)
                {
                    this.IsLiteral = false;
                    this._left = left;
                    this._right = right;
                }

                public Node Clone()
                {
                    if (this.IsLiteral) return new Node(this.Value);
                    return new Node(this.Left.Clone(), this.Right.Clone());
                }

                public long Magnitude()
                {
                    if (this.IsLiteral) return this.Value;
                    return 3 * this.Left.Magnitude() + 2 * this.Right.Magnitude();
                }

                public override string ToString()
                {
                    if (this.IsLiteral)
                    {
                        return this.Value.ToString();
                    }
                    else
                    {
                        return $"[{this.Left},{this.Right}]";
                    }
                }

                public void Reduce()
                {
                    bool reduced;
                    do
                    {
                        reduced = this.TryExplode();
                        //if (reduced) Console.WriteLine("after explode:  " + this.ToString());
                        if (!reduced)
                        {
                            reduced = this.TrySplit();
                            //if (reduced) Console.WriteLine("after split:    " + this.ToString());
                        }

                        if (reduced)
                        {
                            UpdateOtherProperties(this);
                        }
                    } while (reduced);
                }

                public bool TryExplode()
                {
                    if (this.IsLiteral) return false;
                    if (this.Depth < 4)
                    {
                        if (this.Left.TryExplode()) return true;
                        if (this.Right.TryExplode()) return true;
                        return false;
                    }
                    var leftNeighbor = this.FindLeftNeighbor();
                    var rightNeighbor = this.FindRightNeighbor();
                    if (leftNeighbor != null)
                    {
                        leftNeighbor._value += this.Left.Value;
                    }

                    if (rightNeighbor != null)
                    {
                        rightNeighbor._value += this.Right.Value;
                    }

                    var isLeftChild = this == this.Parent.Left;
                    if (isLeftChild)
                    {
                        this.Parent._left = new Node(0);
                    }
                    else
                    {
                        this.Parent._right = new Node(0);
                    }

                    return true;
                }

                public bool TrySplit()
                {
                    if (!this.IsLiteral)
                    {
                        if (this.Left.TrySplit()) return true;
                        if (this.Right.TrySplit()) return true;
                        return false;
                    }

                    if (this.Value < 10) return false;
                    var newNode = new Node(new Node(this.Value / 2), new Node((this.Value + 1) / 2));
                    var isLeftChild = this == this.Parent.Left;
                    if (isLeftChild)
                    {
                        this.Parent._left = newNode;
                    }
                    else
                    {
                        this.Parent._right = newNode;
                    }

                    return true;
                }

                private Node FindLeftNeighbor()
                {
                    if (this.Parent == null) return null;
                    if (this == this.Parent.Right)
                    {
                        Node result = this.Parent.Left;
                        while (!result.IsLiteral) result = result.Right;
                        return result;
                    }
                    return this.Parent.FindLeftNeighbor();
                }

                private Node FindRightNeighbor()
                {
                    if (this.Parent == null) return null;
                    if (this == this.Parent.Left)
                    {
                        Node result = this.Parent.Right;
                        while (!result.IsLiteral) result = result.Left;
                        return result;
                    }
                    return this.Parent.FindRightNeighbor();
                }

                public static Node Parse(string nodeStr)
                {
                    int index = 0;
                    var result = Parse(nodeStr, ref index);
                    UpdateOtherProperties(result);
                    return result;
                }

                public static Node Add(Node node1, Node node2)
                {
                    var term1 = node1.Clone();
                    UpdateOtherProperties(term1);
                    var term2 = node2.Clone();
                    UpdateOtherProperties(term2);
                    var result = new Node(term1, term2);
                    UpdateOtherProperties(result);
                    result.Reduce();
                    return result;
                }

                private static Node Parse(string nodeStr, ref int index)
                {
                    if (char.IsDigit(nodeStr[index]))
                    {
                        return new Node(nodeStr[index++] - '0');
                    }

                    AssertChar(nodeStr, index, '[');
                    index++;
                    var left = Parse(nodeStr, ref index);
                    AssertChar(nodeStr, index, ',');
                    index++;
                    var right = Parse(nodeStr, ref index);
                    AssertChar(nodeStr, index, ']');
                    index++;
                    return new Node(left, right);
                }

                private static void AssertChar(string str, int index, char expectedChar)
                {
                    if (str[index] != expectedChar) throw new Exception($"str[{index}] = {str[index]}, expected {expectedChar}");
                }

                private static void UpdateOtherProperties(Node node, int depth = 0, Node parent = null)
                {
                    node.Parent = parent;
                    node.Depth = depth;
                    if (!node.IsLiteral)
                    {
                        UpdateOtherProperties(node._left, depth + 1, node);
                        UpdateOtherProperties(node._right, depth + 1, node);
                    }
                }
            }
        }

        class Day20
        {
            public static void Solve()
            {
                var testInput = new[]
                {
                    "..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#",
                    "",
                    "#..#.",
                    "#....",
                    "##..#",
                    "..#..",
                    "..###"
                };
                var useTestInput = false;
                var input = useTestInput ? testInput : Helpers.LoadInput("input20.txt");
                var algorithm = input[0];
                int totalIterations = 50; // 2 for part 1
                var image = input.Skip(2).Select(l => (new string('.', totalIterations) + l + new string('.', totalIterations)).ToCharArray()).ToList();
                var imageWidth = image[0].Length;
                for (int i = 0; i < totalIterations; i++)
                {
                    image.Insert(0, new string('.', imageWidth).ToCharArray());
                    image.Add(new string('.', imageWidth).ToCharArray());
                }

                bool outside0 = true;
                int numRows = image.Count;
                int numCols = imageWidth;
                for (int row = 0; row < numRows; row++)
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        if (useTestInput) Console.Write(image[row][col]);
                    }

                    if (useTestInput) Console.WriteLine();
                }

                Console.WriteLine();

                for (int algoIteration = 0; algoIteration < totalIterations; algoIteration++)
                {
                    var newImage = Enumerable.Range(0, numRows).Select(_ => Enumerable.Range(0, numCols).Select(_2 => '.').ToArray()).ToList();
                    for (int row = 0; row < numRows; row++)
                    {
                        for (int col = 0; col < numCols; col++)
                        {
                            int pixelValue = 0;
                            int power2 = 256;
                            for (int r = row - 1; r <= row + 1; r++)
                            {
                                for (int c = col - 1; c <= col + 1; c++)
                                {
                                    bool pixelOn = (r < 0 || r >= numRows || c < 0 || c >= numCols) ? !outside0 : (image[r][c] == '#');
                                    if (pixelOn) pixelValue += power2;
                                    power2 /= 2;
                                }
                            }

                            newImage[row][col] = algorithm[pixelValue];
                        }
                    }

                    image = newImage;

                    if (outside0 && algorithm[0] == '#')
                    {
                        outside0 = false;
                    }
                    else if (!outside0 && algorithm[511] == '.')
                    {
                        outside0 = true;
                    }

                    int litPixels = 0;
                    for (int row = 0; row < numRows; row++)
                    {
                        for (int col = 0; col < numCols; col++)
                        {
                            if (useTestInput) Console.Write(image[row][col]);
                            if (image[row][col] == '#') litPixels++;
                        }

                        if (useTestInput) Console.WriteLine();
                    }

                    if (useTestInput) Console.WriteLine();

                    Console.WriteLine($"After {algoIteration + 1} iterations, {litPixels} lit pixels");
                }
            }
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
