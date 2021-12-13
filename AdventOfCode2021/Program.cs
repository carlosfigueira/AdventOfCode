using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace AdventOfCode2021
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Day10.Solve();
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
                for (int i = 0; i < 100; i++) {
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
                        for (int j= 0; j < this.cols; j++)
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
    }

    public class Helpers
    {
        public static string[] LoadInput(string fileName)
        {
            return File.ReadAllLines(Path.Combine("inputs", fileName));
        }
    }
}
