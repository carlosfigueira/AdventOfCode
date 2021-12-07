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
            Day7.Solve();
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
            // Part 2 94814673 is too high
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
    }

    public class Helpers
    {
        public static string[] LoadInput(string fileName)
        {
            return File.ReadAllLines(Path.Combine("inputs", fileName));
        }
    }
}
