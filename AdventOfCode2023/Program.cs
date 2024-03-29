﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    class Program
    {
        static void Main(string[] args)
        {
            // Day1.Solve();
            // Day2.Solve();
            // Day3.Solve();
            // Day4.Solve();
            // Day5.Solve();
            // Day6.Solve();
            // Day7.Solve();
            // Day8.Solve();
            // Day9.Solve();
            // Day10.Solve();
            // Day11.Solve();
            // Day12.Solve();
            // Day13.Solve();
            // Day14.Solve();
            // Day15.Solve();
            Day16.Solve();
        }
    }

    class Day1
    {
        public static void Solve()
        {
            var useSample = false;
            var lines = useSample ?
                new[] { "1abc2", "pqr3stu8vwx", "a1b2c3d4e5f", "treb7uchet" } :
                Helpers.LoadInput("day1.txt");
            var calibrationSum = 0;
            foreach (var line in lines)
            {
                int first = -1, last = -1;
                for (var i = 0; i < line.Length; i++)
                {
                    if (char.IsDigit(line[i]))
                    {
                        last = i;
                        if (first < 0) first = i;
                    }
                }

                var calibrationValue = 10 * (line[first] - '0') + line[last] - '0';
                calibrationSum += calibrationValue;
            }

            Console.WriteLine("Day 1, part 1: " + calibrationSum);

            if (useSample)
            {
                lines = new[] { "two1nine", "eightwothree", "abcone2threexyz", "xtwone3four", "4nineeightseven2", "zoneight234", "7pqrstsixteen" };
            }

            calibrationSum = 0;
            foreach (var line in lines)
            {
                int firstNumber = -1, lastNumber = -1;
                for (var i = 0; i < line.Length; i++)
                {
                    if (TryGetNumber(line, i, out int value))
                    {
                        lastNumber = value;
                        if (firstNumber < 0) firstNumber = value;
                    }
                }

                var calibrationValue = 10 * firstNumber + lastNumber;
                calibrationSum += calibrationValue;
            }

            Console.WriteLine("Day 1, part 2: " + calibrationSum);
        }

        static readonly string[] numbers = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        static bool TryGetNumber(string str, int index, out int value)
        {
            if (char.IsDigit(str[index]))
            {
                value = str[index] - '0';
                return true;
            }

            for (var i = 0; i < numbers.Length; i++)
            {
                if (str.Substring(index).StartsWith(numbers[i]))
                {
                    value = i + 1;
                    return true;
                }
            }

            value = -1;
            return false;
        }
    }

    class Day2
    {
        class GamePart
        {
            public int Red { get; set; }
            public int Green { get; set; }
            public int Blue { get; set; }
        }

        class Game
        {
            public int Number { get; }
            public List<GamePart> Parts { get; }
            public Game(int number)
            {
                Number = number;
                Parts = new List<GamePart>();
            }

            public int MaxBlue => Parts.Max(gp => gp.Blue);
            public int MaxRed => Parts.Max(gp => gp.Red);
            public int MaxGreen => Parts.Max(gp => gp.Green);

            static Regex GameRegex = new Regex(@"Game (?<number>\d+): (?<parts>.+)");
            static Regex PartRegex = new Regex(@"(?<number>\d+) (?<color>\w+)");
            public static Game Parse(string line)
            {
                var match = GameRegex.Match(line);
                var number = int.Parse(match.Groups["number"].Value);
                var result = new Game(number);
                var parts = match.Groups["parts"].Value.Split(';');
                foreach (var part in parts)
                {
                    var gamePart = new GamePart();
                    result.Parts.Add(gamePart);
                    var colors = part.Split(',');
                    foreach (var color in colors)
                    {
                        var partMatch = PartRegex.Match(color.Trim());
                        var partNumber = int.Parse(partMatch.Groups["number"].Value);
                        var partColor = partMatch.Groups["color"].Value;
                        switch (partColor)
                        {
                            case "blue": gamePart.Blue = partNumber; break;
                            case "red": gamePart.Red = partNumber; break;
                            case "green": gamePart.Green = partNumber; break;
                            default: throw new ArgumentException();
                        }
                    }
                }

                return result;
            }
        }

        public static void Solve()
        {
            var useSample = false;
            var lines = useSample ?
                new[]
                {
                    "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
                    "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
                    "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
                    "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
                    "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
                } :
                Helpers.LoadInput("day2.txt");

            var games = lines.Select(l => Game.Parse(l)).ToList();
            var idSumPart1 = 0;
            foreach (var game in games)
            {
                if (game.MaxBlue <= 14 && game.MaxGreen <= 13 && game.MaxRed <= 12)
                {
                    idSumPart1 += game.Number;
                }
            }

            Console.WriteLine("Day 2, part 1: " + idSumPart1);

            var powerSumPart2 = 0;
            foreach (var game in games)
            {
                powerSumPart2 += game.MaxBlue * game.MaxGreen * game.MaxRed;
            }

            Console.WriteLine("Day 2, part 2: " + powerSumPart2);
        }
    }

    class Day3
    {
        static bool IsSymbol(char c)
        {
            return !char.IsDigit(c) && c != '.';
        }

        class NumberDef
        {
            public int Value { get; }
            public int Line { get; }
            public int StartCol { get; }
            public int EndCol { get; }
            public bool IsPart { get; }
            public NumberDef(int value, int line, int startCol, int endCol, bool isPart)
            {
                Value = value;
                Line = line;
                StartCol = startCol;
                EndCol = endCol;
                IsPart = isPart;
            }
        }

        public static void Solve()
        {
            var useSample = false;
            var lines = useSample ?
                new[] { "467..114..", "...*......", "..35..633.", "......#...", "617*......", ".....+.58.", "..592.....", "......755.", "...$.*....", ".664.598.." } :
                Helpers.LoadInput("day3.txt");

            int sumParts = 0;
            var numberDefsPerLine = new List<List<NumberDef>>();

            for (int l = 0; l < lines.Length; l++)
            {
                numberDefsPerLine.Add(new List<NumberDef>());
                var line = lines[l];
                var startNumber = -1;
                var number = 0;
                for (int c = 0; c < line.Length; c++)
                {
                    if (char.IsDigit(line[c]))
                    {
                        if (startNumber < 0)
                        {
                            startNumber = c;
                        }

                        number *= 10;
                        number += line[c] - '0';
                    }

                    if (!char.IsDigit(line[c]) || c == line.Length - 1)
                    {
                        var minCol = Math.Max(0, startNumber - 1);
                        var maxCol = Math.Min(line.Length - 1, c);
                        var isPart = false;
                        if (startNumber > 0 && IsSymbol(lines[l][startNumber - 1]))
                        {
                            isPart = true;
                        }
                        else if (c < line.Length - 1 && IsSymbol(lines[l][c]))
                        {
                            isPart = true;
                        }
                        else
                        {
                            for (var col = minCol; col <= maxCol; col++)
                            {
                                if (l > 0 && IsSymbol(lines[l - 1][col]))
                                {
                                    isPart = true;
                                    break;
                                }

                                if (l < lines.Length - 1 && IsSymbol(lines[l + 1][col]))
                                {
                                    isPart = true;
                                    break;
                                }
                            }
                        }

                        if (startNumber >= 0)
                        {
                            numberDefsPerLine[l].Add(new NumberDef(number, l, startNumber, c - 1, isPart));
                        }

                        if (isPart)
                        {
                            sumParts += number;
                        }

                        startNumber = -1;
                        number = 0;
                    }
                }
            }

            Console.WriteLine("Day 3, part 1: " + sumParts);

            var sumGearRatios = 0;
            for (var l = 0; l < lines.Length; l++)
            {
                for (var c = 0; c < lines[l].Length; c++)
                {
                    if (lines[l][c] == '*')
                    {
                        var neighbooringNumbers = new List<NumberDef>();
                        var minLine = Math.Max(0, l - 1);
                        var maxLine = Math.Min(lines.Length - 1, l + 1);
                        for (var line = minLine; line <= maxLine; line++)
                        {
                            foreach (var numberDef in numberDefsPerLine[line])
                            {
                                if ((numberDef.StartCol - 1) <= c && c <= (numberDef.EndCol + 1))
                                {
                                    neighbooringNumbers.Add(numberDef);
                                }
                            }
                        }

                        if (neighbooringNumbers.Count == 2)
                        {
                            sumGearRatios += neighbooringNumbers[0].Value * neighbooringNumbers[1].Value;
                        }
                    }
                }
            }

            Console.WriteLine("Day 3, part 2: " + sumGearRatios);
        }
    }

    class Day4
    {
        class Card
        {
            public int Number { get; }
            public List<int> WinningNumbers { get; }
            public List<int> CardNumbers { get; }

            public Card(int number, IEnumerable<int> winningNumbers, IEnumerable<int> cardNumbers)
            {
                Number = number;
                WinningNumbers = new List<int>(winningNumbers);
                CardNumbers = new List<int>(cardNumbers);
            }

            static readonly Regex CardNumberRegex = new Regex(@"Card\s+(\d+)");
            public static Card Parse(string line)
            {
                int colonIndex = line.IndexOf(':');
                var number = int.Parse(CardNumberRegex.Match(line.Substring(0, colonIndex)).Groups[1].Value);
                line = line.Substring(colonIndex + 1);
                var barIndex = line.IndexOf('|');
                var winningNumbers = line.Substring(0, barIndex).Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n));
                var cardNumbers = line.Substring(barIndex + 1).Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n));
                return new Card(number, winningNumbers, cardNumbers);
            }

            public int WinningNumbersInCard()
            {
                int result = 0;
                foreach (var cardNumber in CardNumbers)
                {
                    if (WinningNumbers.Contains(cardNumber))
                    {
                        result++;
                    }
                }

                return result;
            }
        }

        public static void Solve()
        {
            var useSample = false;
            var lines = useSample ?
                new[] {
                    "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53",
                    "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19",
                    "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1",
                    "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83",
                    "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36",
                    "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11"
                } :
                Helpers.LoadInput("day4.txt");

            int part1 = 0;
            foreach (var line in lines)
            {
                var card = Card.Parse(line);
                var winningNumbers = card.WinningNumbersInCard();
                if (winningNumbers > 0)
                {
                    part1 += (1 << (winningNumbers - 1));
                }
            }

            Console.WriteLine("Day 4, part 1: " + part1);

            int[] copies = new int[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                copies[i] = 1;
            }

            var totalCards = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var card = Card.Parse(line);
                var winningNumbers = card.WinningNumbersInCard();
                totalCards += copies[i];
                if (winningNumbers > 0)
                {
                    for (int j = 0; j < winningNumbers; j++)
                    {
                        var lineToGetCopy = i + j + 1;
                        if (lineToGetCopy < lines.Length)
                        {
                            copies[lineToGetCopy] += copies[i];
                        }
                    }
                }
            }

            Console.WriteLine("Day 4, part 2: " + totalCards);
        }
    }

    class Day5
    {
        class CategoryMapEntry
        {
            public long FromRangeStart { get; }
            public long ToRangeStart { get; }
            public long RangeLength { get; }

            public CategoryMapEntry(long toRangeStart, long fromRangeStart, long rangeLength)
            {
                ToRangeStart = toRangeStart;
                FromRangeStart = fromRangeStart;
                RangeLength = rangeLength;
            }

            public long Delta()
            {
                return ToRangeStart - FromRangeStart;
            }

            public override string ToString()
            {
                return $"({FromRangeStart} to {FromRangeStart + RangeLength - 1}, {Delta()})";
            }
        }

        class CategoryMap
        {
            public string From { get; }
            public string To { get; }
            public List<CategoryMapEntry> Entries { get; }

            public CategoryMap(string from, string to)
            {
                Entries = new List<CategoryMapEntry>();
                From = from;
                To = to;
            }

            public long GetDestination(long fromValue)
            {
                foreach (var mapEntry in Entries)
                {
                    var first = mapEntry.FromRangeStart;
                    var last = first + mapEntry.RangeLength - 1;
                    if (first <= fromValue && fromValue <= last)
                    {
                        return mapEntry.ToRangeStart + (fromValue - mapEntry.FromRangeStart);
                    }
                }

                return fromValue;
            }

            public override string ToString()
            {
                return $"From {From} to {To}: {string.Join(", ", Entries)}";
            }

            private static Regex FromToRegex = new Regex(@"(?<from>[^\-]+)\-to\-(?<to>\S+) map:");
            public static CategoryMap Parse(string[] lines, ref int index)
            {
                var match = FromToRegex.Match(lines[index++]);
                if (!match.Success) throw new ArgumentException();
                var from = match.Groups["from"].Value;
                var to = match.Groups["to"].Value;
                var result = new CategoryMap(from, to);
                var tempEntries = new List<CategoryMapEntry>();
                while (index < lines.Length && lines[index].Length > 0)
                {
                    var parts = lines[index++].Split(' ').Select(p => long.Parse(p)).ToArray();
                    tempEntries.Add(new CategoryMapEntry(parts[0], parts[1], parts[2]));
                }

                if (index < lines.Length && lines[index].Length == 0) index++;
                tempEntries.Sort((me1, me2) => Math.Sign(me1.FromRangeStart - me2.FromRangeStart));
                long min = 0;
                foreach (var tempEntry in tempEntries)
                {
                    if (tempEntry.FromRangeStart > min)
                    {
                        result.Entries.Add(new CategoryMapEntry(min, min, tempEntry.FromRangeStart - min));
                    }

                    result.Entries.Add(tempEntry);
                    min = tempEntry.FromRangeStart + tempEntry.RangeLength;
                }

                if (min < long.MaxValue)
                {
                    result.Entries.Add(new CategoryMapEntry(min, min, long.MaxValue - min));
                }

                return result;
            }
        }

        class MappedRange
        {
            public long Start { get; }
            public long Length { get; }
            public MappedRange(long start, long length)
            {
                Start = start;
                Length = length;
            }

            public override string ToString()
            {
                return $"[{Start}, {Start + Length - 1}]";
            }
        }

        public static void Solve()
        {
            var useSample = false;
            var lines = useSample ?
                new[] {
                    "seeds: 79 14 55 13",
                    "",
                    "seed-to-soil map:",
                    "50 98 2",
                    "52 50 48",
                    "",
                    "soil-to-fertilizer map:",
                    "0 15 37",
                    "37 52 2",
                    "39 0 15",
                    "",
                    "fertilizer-to-water map:",
                    "49 53 8",
                    "0 11 42",
                    "42 0 7",
                    "57 7 4",
                    "",
                    "water-to-light map:",
                    "88 18 7",
                    "18 25 70",
                    "",
                    "light-to-temperature map:",
                    "45 77 23",
                    "81 45 19",
                    "68 64 13",
                    "",
                    "temperature-to-humidity map:",
                    "0 69 1",
                    "1 0 69",
                    "",
                    "humidity-to-location map:",
                    "60 56 37",
                    "56 93 4"
                } :
                Helpers.LoadInput("day5.txt");

            var seeds = lines[0]
                .Substring(lines[0].IndexOf(':') + 1)
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => long.Parse(p))
                .ToArray();

            var mapped = seeds;
            var maps = new List<CategoryMap>();
            var index = 2;
            while (index < lines.Length)
            {
                var map = CategoryMap.Parse(lines, ref index);
                maps.Add(map);
                mapped = mapped.Select(m => map.GetDestination(m)).ToArray();
            }

            Console.WriteLine("Day 5, part 1: " + mapped.Min());

            var mappedRanges = new List<MappedRange>();
            for (int i = 0; i < seeds.Length; i += 2)
            {
                mappedRanges.Add(new MappedRange(seeds[i], seeds[i + 1]));
            }

            mappedRanges.Sort((mr1, mr2) => Math.Sign(mr1.Start - mr2.Start));

            foreach (var map in maps)
            {
                List<MappedRange> newRanges = new List<MappedRange>();
                foreach (var range in mappedRanges)
                {
                    int rangeStartIndex = -1;
                    for (int i = 0; i < map.Entries.Count; i++)
                    {
                        if (map.Entries[i].FromRangeStart <= range.Start && range.Start <= map.Entries[i].FromRangeStart + map.Entries[i].RangeLength)
                        {
                            rangeStartIndex = i;
                            break;
                        }
                    }

                    long leftInRange = range.Length;
                    long leftInMapEntry = map.Entries[rangeStartIndex].RangeLength - (range.Start - map.Entries[rangeStartIndex].FromRangeStart);
                    long usedFromRange = 0;
                    while (leftInRange > 0)
                    {
                        long nextRangeSize = Math.Min(leftInRange, leftInMapEntry);
                        newRanges.Add(new MappedRange(range.Start + usedFromRange + map.Entries[rangeStartIndex].Delta(), nextRangeSize));
                        usedFromRange += nextRangeSize;
                        leftInRange -= nextRangeSize;

                        if (leftInRange > 0)
                        {
                            rangeStartIndex++;
                            leftInMapEntry = map.Entries[rangeStartIndex].RangeLength;
                        }
                    }
                }

                mappedRanges = newRanges;
            }

            mappedRanges.Sort((mr1, mr2) => Math.Sign(mr1.Start - mr2.Start));
            Console.WriteLine("Day 5, part 2: " + mappedRanges[0].Start);
        }
    }

    class Day6
    {
        public static void Solve()
        {
            var useSample = false;
            var input = useSample ?
                new[] { "Time:      7  15   30", "Distance:  9  40  200" } :
                Helpers.LoadInput("day6.txt");

            var timesDistances = input.Select(i =>
                i.Substring(i.IndexOf(':') + 1)
                .Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => int.Parse(t))
                .ToArray()).ToArray();
            var times = timesDistances[0];
            var distances = timesDistances[1];

            long result = 1;
            for (var i = 0; i < times.Length; i++)
            {
                var waysToWinRace = 0;
                for (var t = 1; t < times[i]; t++)
                {
                    var d = (times[i] - t) * t;
                    if (d > distances[i]) waysToWinRace++;
                }

                result *= waysToWinRace;
            }

            Console.WriteLine("Day 6, part 1: " + result);

            var largeTime = long.Parse(input[0].Substring(input[0].IndexOf(':') + 1).Replace(" ", ""));
            var largeDistance = long.Parse(input[1].Substring(input[1].IndexOf(':') + 1).Replace(" ", ""));

            // Values where beat distance: t(largeTime - t) - largeDistance > 0
            // -t^2 + largeTime*t - largeDistance
            // Solving using quadratic equation
            var delta = largeTime * largeTime - 4 * largeDistance;
            var sqrtDelta = Math.Sqrt(delta);
            var root1 = (largeTime - sqrtDelta) / 2;
            var root2 = (largeTime + sqrtDelta) / 2;
            var minTime = Math.Floor(root1);
            var minDistance = minTime * (largeTime - minTime);
            if (minDistance <= largeDistance) minTime++;
            var maxTime = Math.Ceiling(root2);
            var maxDistance = maxTime * (largeTime - maxTime);
            if (maxDistance <= largeDistance) maxTime--;

            Console.WriteLine("Day 6, part 2: " + (maxTime - minTime + 1));
        }
    }

    class Day7
    {
        private enum HandType
        {
            FiveOfAKind = 1,
            FourOfAKind = 2,
            FullHouse = 3,
            ThreeOfAKind = 4,
            TwoPair = 5,
            OnePair = 6,
            HighCard = 7,
        }

        private const string CardOrderNoJoker = "AKQJT98765432";
        private const string CardOrderWithJoker = "AKQT98765432J";

        class Hand : IComparable<Hand>
        {
            public string Cards { get; }
            public int Bid { get; }
            public HandType HandType { get; }
            private bool JIsJoker { get; }

            public Hand(string cards, int bid, bool jIsJoker)
            {
                Cards = cards;
                Bid = bid;
                JIsJoker = jIsJoker;

                HandType = CalculateHandType(cards);
            }

            public override string ToString()
            {
                return $"Hand[Cards={Cards}, Bid={Bid}, {HandType}]";
            }

            private HandType CalculateHandType(string cards)
            {
                var jokerCount = JIsJoker ? cards.Count(c => c == 'J') : 0;
                var counts = cards
                    .Where(c => !JIsJoker || c != 'J')
                    .GroupBy(c => c)
                    .Select(group => group.Count())
                    .OrderByDescending(g => g)
                    .ToArray();
                if (counts.Length == 0)
                {
                    // All jokers
                    return HandType.FiveOfAKind;
                }

                counts[0] += jokerCount;
                if (counts[0] == 5)
                {
                    return HandType.FiveOfAKind;
                }

                if (counts[0] == 4)
                {
                    return HandType.FourOfAKind;
                }

                if (counts[0] == 3)
                {
                    if (counts[1] == 2)
                    {
                        return HandType.FullHouse;
                    }
                    else
                    {
                        return HandType.ThreeOfAKind;
                    }
                }

                if (counts[0] == 2)
                {
                    if (counts[1] == 2)
                    {
                        return HandType.TwoPair;
                    }
                    else
                    {
                        return HandType.OnePair;
                    }
                }

                return HandType.HighCard;
            }

            public static Hand Parse(string line, bool jIsJoker)
            {
                var parts = line.Split(' ');
                var cards = parts[0];
                var bid = int.Parse(parts[1]);
                return new Hand(cards, bid, jIsJoker);
            }

            public int CompareTo(Hand other)
            {
                if (this.HandType != other.HandType)
                {
                    return this.HandType - other.HandType;
                }

                var CardOrder = JIsJoker ? CardOrderWithJoker : CardOrderNoJoker;
                for (var i = 0; i < this.Cards.Length; i++)
                {
                    var thisRank = CardOrder.IndexOf(this.Cards[i]);
                    var otherRank = CardOrder.IndexOf(other.Cards[i]);
                    if (thisRank != otherRank)
                    {
                        return thisRank - otherRank;
                    }
                }

                return 0;
            }
        }
        public static void Solve()
        {
            var useSample = false;
            var input = useSample ?
                new[] {
                    "32T3K 765",
                    "T55J5 684",
                    "KK677 28",
                    "KTJJT 220",
                    "QQQJA 483"
                } :
                Helpers.LoadInput("day7.txt");

            var allHands = input.Select(l => Hand.Parse(l, jIsJoker: false)).ToList();
            allHands.Sort();
            long totalWinnings = 0;
            var currentRank = allHands.Count;
            for (int i = 0; i < allHands.Count; i++)
            {
                totalWinnings += allHands[i].Bid * currentRank;
                currentRank--;
            }

            Console.WriteLine("Day 7, part 1: " + totalWinnings);

            allHands = input.Select(l => Hand.Parse(l, jIsJoker: true)).ToList();
            allHands.Sort();
            totalWinnings = 0;
            currentRank = allHands.Count;
            for (int i = 0; i < allHands.Count; i++)
            {
                totalWinnings += allHands[i].Bid * currentRank;
                currentRank--;
            }

            Console.WriteLine("Day 7, part 1: " + totalWinnings);
        }
    }

    class Day8
    {
        public static void Solve()
        {
            var useSample1 = false;
            var useSample2 = false;
            var input = useSample1 ?
                new[] { "RL", "", "AAA = (BBB, CCC)", "BBB = (DDD, EEE)", "CCC = (ZZZ, GGG)", "DDD = (DDD, DDD)", "EEE = (EEE, EEE)", "GGG = (GGG, GGG)", "ZZZ = (ZZZ, ZZZ)" } :
                useSample2 ?
                new[] { "LLR", "", "AAA = (BBB, BBB)", "BBB = (AAA, ZZZ)", "ZZZ = (ZZZ, ZZZ)" } :
                Helpers.LoadInput("day8.txt");

            var directions = input[0];
            var dict = new Dictionary<string, Tuple<string, string>>();
            for (int i = 2; i < input.Length; i++)
            {
                dict.Add(input[i].Substring(0, 3), Tuple.Create(input[i].Substring(7, 3), input[i].Substring(12, 3)));
            }

            var count = 0;
            var directionIndex = 0;
            var current = "AAA";
            while (current != "ZZZ")
            {
                count++;
                var next = dict[current];
                current = directions[directionIndex] == 'L' ? next.Item1 : next.Item2;
                directionIndex = (directionIndex + 1) % directions.Length;
            }

            Console.WriteLine("Day 8, part 1: " + count);

            // Large value of 'stepsToZ' is around 21000, so no need to have primes beyond Sqrt(21000) which is less than 150
            var primes = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151 };
            var primeFactors = new Dictionary<int, int>();

            var aNodes = dict.Keys.Where(n => n[2] == 'A').ToArray();
            var stepsToZ = new int[aNodes.Length];
            for (int i = 0; i < aNodes.Length; i++)
            {
                current = aNodes[i];
                count = 0;
                directionIndex = 0;
                while (current[2] != 'Z')
                {
                    count++;
                    var next = dict[current];
                    current = directions[directionIndex] == 'L' ? next.Item1 : next.Item2;
                    directionIndex = (directionIndex + 1) % directions.Length;
                }

                stepsToZ[i] = count;

                while (count > 1)
                {
                    foreach (var prime in primes)
                    {
                        var factor = 0;
                        while ((count % prime) == 0)
                        {
                            factor++;
                            count /= prime;
                        }

                        if (factor > 0)
                        {
                            if (!primeFactors.ContainsKey(prime))
                            {
                                primeFactors.Add(prime, factor);
                            }
                            else
                            {
                                primeFactors[prime] = Math.Max(factor, primeFactors[prime]);
                            }
                        }

                        if (count == 1) break;
                    }

                    if (count > 1)
                    {
                        primeFactors[count] = 1;
                        count = 1;
                    }
                }
            }

            long part2Result = 1;
            foreach (var prime in primeFactors.Keys)
            {
                for (var i = 0; i < primeFactors[prime]; i++) part2Result *= prime;
            }

            Console.WriteLine("Day 8, part 2: " + part2Result);
        }
    }

    class Day9
    {
        public static void Solve()
        {
            var useSample = false;
            var input = useSample ?
                new[] { "0 3 6 9 12 15", "1 3 6 10 15 21", "10 13 16 21 30 45" } :
                Helpers.LoadInput("day9.txt");

            var values = input.Select(l => l.Split(' ').Select(p => long.Parse(p)).ToList()).ToArray();

            long part1 = 0, part2 = 0;

            foreach (var line in values)
            {
                var newLines = new List<List<long>>();
                var currList = line;
                newLines.Add(line);
                for (int i = 0; i < line.Count; i++)
                {
                    var newList = new List<long>();
                    newLines.Add(newList);
                    var allZeros = true;
                    for (int j = 0; j < currList.Count - 1; j++)
                    {
                        var newValue = currList[j + 1] - currList[j];
                        newList.Add(newValue);
                        if (newValue != 0) allZeros = false;
                    }

                    currList = newList;
                    if (allZeros) break;
                }

                var extrapolateAfter = new List<long> { 0 };
                var extrapolateBefore = new List<long> { 0 };
                for (int i = newLines.Count - 2; i >= 0; i--)
                {
                    extrapolateAfter.Add(extrapolateAfter.Last() + newLines[i].Last());
                    extrapolateBefore.Add(newLines[i][0] - extrapolateBefore.Last());
                }

                part1 += extrapolateAfter.Last();
                part2 += extrapolateBefore.Last();
            }

            Console.WriteLine("Day 9, part 1: " + part1);
            Console.WriteLine("Day 9, part 2: " + part2);
        }
    }

    class Day10
    {
        enum Direction { North, East, South, West };
        static void NextStep(string[] input, ref int row, ref int col, ref Direction direction)
        {
            var currentInput = input[row][col];
            switch (direction)
            {
                case Direction.North:
                    switch(currentInput)
                    {
                        case '|': row--; break;
                        case '7': col--; direction = Direction.West; break;
                        case 'F': col++; direction = Direction.East; break;
                        default: throw new ArgumentException();
                    }

                    break;
                case Direction.South:
                    switch (currentInput)
                    {
                        case '|': row++; break;
                        case 'J': col--; direction = Direction.West; break;
                        case 'L': col++; direction = Direction.East; break;
                        default: throw new ArgumentException();
                    }

                    break;
                case Direction.East:
                    switch (currentInput)
                    {
                        case '-': col++; break;
                        case '7': row++; direction = Direction.South; break;
                        case 'J': row--; direction = Direction.North; break;
                        default: throw new ArgumentException();
                    }
                    break;
                case Direction.West:
                    switch (currentInput)
                    {
                        case '-': col--; break;
                        case 'F': row++; direction = Direction.South; break;
                        case 'L': row--; direction = Direction.North; break;
                        default: throw new ArgumentException();
                    }
                    break;
            }
        }

        public static void Solve()
        {
            var useSample1 = false;
            var useSample2 = false;
            var useSample3 = false;
            var input = useSample1 ?
                new[] { "-L|F7", "7S-7|", "L|7||", "-L-J|", "L|-JF" } :
                useSample2 ?
                new[] { "7-F7-", ".FJ|7", "SJLL7", "|F--J", "LJ.LJ" } :
                useSample3 ?
                new[] { "...........", ".S-------7.", ".|F-----7|.", ".||.....||.", ".||.....||.", ".|L-7.F-J|.", ".|..|.|..|.", ".L--J.L--J.", "..........." } :
                Helpers.LoadInput("day10.txt");

            var startDirection = useSample1 ? Direction.North : useSample2 ? Direction.North : Direction.North;
            var startLetter = useSample1 ? 'F' : useSample2 ? 'F' : useSample3 ? 'F' : '|';
            int startRow = -1, startCol = -1;
            for (var i = 0; i < input.Length; i++)
            {
                for (var j = 0; j < input.Length; j++)
                {
                    if (input[i][j] == 'S')
                    {
                        startRow = i;
                        startCol = j;
                        input[i] = input[i].Substring(0, j) + startLetter + input[i].Substring(j + 1);
                        break;
                    }
                }
            }

            var totalSteps = 0;
            var currentRow = startRow;
            var currentCol = startCol;
            var currentDirection = startDirection;

            var visited = new HashSet<Tuple<int, int>>();
            do
            {
                visited.Add(Tuple.Create(currentRow, currentCol));
                NextStep(input, ref currentRow, ref currentCol, ref currentDirection);
                totalSteps++;
            } while (currentRow != startRow || currentCol != startCol);

            Console.WriteLine("Day 10, part 1: " + (totalSteps / 2));

            // Idea for part 2 from https://www.reddit.com/r/adventofcode/comments/18evyu9/comment/kcqphay/
            var newInput = new List<string>();
            for (var i = 0; i < input.Length; i++)
            {
                var sb = new StringBuilder();
                for (var j = 0; j < input[i].Length; j++)
                {
                    if (visited.Contains(Tuple.Create(i, j)))
                    {
                        sb.Append(input[i][j]);
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }

                newInput.Add(
                    Regex.Replace(
                        Regex.Replace(
                            sb.ToString(),
                            "F-*7|L-*J",
                            ""),
                        "F-*J|L-*7",
                        "|"));
            }

            var part2 = 0;
            foreach (var line in newInput)
            {
                var inside = false;
                foreach (var c in line)
                {
                    if (c == '|') inside = !inside;
                    if (c == '.' && inside) part2++;
                }
            }

            Console.WriteLine("Day 10, part 2: " + part2);
        }
    }

    class Day11
    {
        public static void Solve()
        {
            var useSample = false;
            var input = useSample ?
                new[] { "...#......", ".......#..", "#.........", "..........", "......#...", ".#........", ".........#", "..........", ".......#..", "#...#....." } :
                Helpers.LoadInput("day11.txt");
            var rows = input.Length;
            var cols = input[0].Length;

            var emptyRows = new HashSet<int>();
            for (var i = 0; i < rows; i++)
            {
                var isEmpty = true;
                for (var j = 0; j < cols; j++)
                {
                    if (input[i][j] == '#')
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty) emptyRows.Add(i);
            }

            var emptyCols = new HashSet<int>();
            for (var j = 0; j < cols; j++)
            {
                var isEmpty = true;
                for (var i = 0; i < rows; i++)
                {
                    if (input[i][j] == '#')
                    {
                        isEmpty = false;
                        break;
                    }
                }

                if (isEmpty) emptyCols.Add(j);
            }

            var expandedRows = new List<string>();
            for (int i = 0; i < rows; i++)
            {
                expandedRows.Add(input[i]);
                if (emptyRows.Contains(i))
                {
                    expandedRows.Add(input[i]);
                }
            }

            rows = expandedRows.Count;

            for (int i = 0; i < rows; i++)
            {
                var sb = new StringBuilder();
                for (int j = 0; j < cols; j++)
                {
                    sb.Append(expandedRows[i][j]);
                    if (emptyCols.Contains(j)) sb.Append('.');
                }

                expandedRows[i] = sb.ToString();
            }

            cols = expandedRows[0].Length;

            List<Tuple<int, int>> stars = new List<Tuple<int, int>>();
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    if (expandedRows[i][j] == '#')
                    {
                        stars.Add(Tuple.Create(i, j));
                    }
                }
            }

            int part1 = 0;
            for (int i = 0; i < stars.Count; i++)
            {
                var star1 = stars[i];
                for (int j = i + 1; j < stars.Count; j++)
                {
                    var star2 = stars[j];
                    part1 += Math.Abs(star1.Item1 - star2.Item1) + Math.Abs(star1.Item2 - star2.Item2);
                }
            }

            Console.WriteLine("Day 11, part 1: " + part1);

            long part2 = 0;
            List<Tuple<int, int>> originalStars = new List<Tuple<int, int>>();
            rows = input.Length;
            cols = input[0].Length;
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    if (input[i][j] == '#')
                    {
                        originalStars.Add(Tuple.Create(i, j));
                    }
                }
            }

            int expansionFactor = 1000000;
            for (int i = 0; i < originalStars.Count; i++)
            {
                var star1 = originalStars[i];
                for (int j = i + 1; j < originalStars.Count; j++)
                {
                    var star2 = originalStars[j];
                    int fromRow = Math.Min(star1.Item1, star2.Item1);
                    int toRow = Math.Max(star1.Item1, star2.Item1);
                    int fromCol = Math.Min(star1.Item2, star2.Item2);
                    int toCol = Math.Max(star1.Item2, star2.Item2);
                    for (int row = fromRow + 1; row <= toRow; row++)
                    {
                        if (emptyRows.Contains(row))
                        {
                            part2 += expansionFactor;
                        }
                        else
                        {
                            part2 += 1;
                        }
                    }
                    for (int col = fromCol + 1; col <= toCol; col++)
                    {
                        if (emptyCols.Contains(col))
                        {
                            part2 += expansionFactor;
                        }
                        else
                        {
                            part2 += 1;
                        }
                    }
                }
            }

            Console.WriteLine("Day 11, part 2: " + part2);
        }
    }

    class Day12
    {
        class SpringRow
        {
            public bool?[] Damaged { get; }
            public int[] GroupSizes { get; }

            private int _wildcardCount;
            private string _line;

            public SpringRow(string line, bool?[] damaged, int[] groupSizes)
            {
                Damaged = damaged;
                GroupSizes = groupSizes;
                _line = line;

                _wildcardCount = damaged.Where(c => c == null).Count();
            }

            public static SpringRow Parse(string line, bool unfold = false)
            {
                var parts = line.Split(' ');
                var damaged = parts[0].Select(c => c == '#' ? (bool?)true : c == '.' ? (bool?)false : null).ToArray();
                var groupSizes = parts[1].Split(',').Select(s => int.Parse(s)).ToArray();
                if (!unfold)
                {
                    return new SpringRow(line, damaged, groupSizes);
                }
                else
                {
                    var newDamaged = new bool?[damaged.Length * 5 + 4];
                    var newGroupSizes = new int[groupSizes.Length * 5];
                    for (int i = 0; i < 5; i++)
                    {
                        Array.Copy(damaged, 0, newDamaged, (damaged.Length + 1) * i, damaged.Length);
                        Array.Copy(groupSizes, 0, newGroupSizes, groupSizes.Length * i, groupSizes.Length);
                    }

                    return new SpringRow(line, newDamaged, newGroupSizes);
                }
            }

            public int PossibleArrangements()
            {
                int max = 1 << _wildcardCount;
                var bits = new bool[_wildcardCount];
                var validArrangements = 0;
                for (int i = 0; i < max; i++)
                {
                    for (int j = 0; j < _wildcardCount; j++) bits[j] = (i & (1 << j)) != 0;

                    var inGroup = false;
                    var groupCount = 0;
                    var wildcardIndex = 0;
                    int groupSizesIndex = 0;
                    var isValidArrangement = true;
                    for (int j = 0; j < Damaged.Length; j++)
                    {
                        if ((Damaged[j].HasValue && Damaged[j].Value) || (!Damaged[j].HasValue && bits[wildcardIndex++]))
                        {
                            if (!inGroup)
                            {
                                inGroup = true;
                                if (groupSizesIndex == GroupSizes.Length)
                                {
                                    isValidArrangement = false;
                                    break;
                                }
                            }

                            groupCount++;
                        }
                        else
                        {
                            if (inGroup)
                            {
                                inGroup = false;
                                if (groupCount != GroupSizes[groupSizesIndex++])
                                {
                                    isValidArrangement = false;
                                    break;
                                }

                                groupCount = 0;
                            }
                        }
                    }

                    if (isValidArrangement)
                    {
                        if (groupSizesIndex > GroupSizes.Length)
                        {
                            isValidArrangement = false;
                        }
                        else if (inGroup && groupCount != GroupSizes[groupSizesIndex++])
                        {
                            isValidArrangement = false;
                        }
                        else if (groupSizesIndex != GroupSizes.Length)
                        {
                            isValidArrangement = false;
                        }
                    }

                    if (isValidArrangement)
                    {
                        validArrangements++;
                    }
                }

                return validArrangements;
            }
        }

        public static void Solve()
        {
            var useSample = true;
            var input = useSample ?
                new[] { "???.### 1,1,3", ".??..??...?##. 1,1,3", "?#?#?#?#?#?#?#? 1,3,1,6", "????.#...#... 4,1,1", "????.######..#####. 1,6,5", "?###???????? 3,2,1" } :
                Helpers.LoadInput("day12.txt");

            var part1 = 0;
            foreach (var line in input)
            {
                var springRow = SpringRow.Parse(line);
                var arrangementsForLine = springRow.PossibleArrangements();
                part1 += arrangementsForLine;
                //Console.WriteLine($"[debug] {line}: {arrangementsForLine}");
            }

            Console.WriteLine("Day 12, part 1: " + part1);

            var part2 = 0;
            foreach (var line in input)
            {
                var springRow = SpringRow.Parse(line, true);
                var arrangementsForLine = springRow.PossibleArrangements();
                part2 += arrangementsForLine;
                Console.WriteLine($"[debug] {line}: {arrangementsForLine}");
            }

            Console.WriteLine("Day 12, part 2: " + part2);
        }
    }

    public class Day13
    {
        class Pattern
        {
            public string[] Lines { get; }
            public string[] TransposedLines { get; }
            public int Rows { get; }
            public int Cols { get; }

            public Pattern(string[] lines)
            {
                Lines = lines;
                Rows = lines.Length;
                Cols = lines[0].Length;
                var transposed = new StringBuilder[lines[0].Length];
                for (var i = 0; i < transposed.Length; i++)
                {
                    transposed[i] = new StringBuilder();
                    for (var j = 0; j < lines.Length; j++)
                    {
                        transposed[i].Append(lines[j][i]);
                    }
                }

                TransposedLines = transposed.Select(sb => sb.ToString()).ToArray();
            }

            public static bool TryParse(string[] input, ref int index, out Pattern pattern)
            {
                var list = new List<string>();
                while (index < input.Length && input[index] != "")
                {
                    list.Add(input[index++]);
                }

                if (list.Count > 0)
                {
                    pattern = new Pattern(list.ToArray());
                    return true;
                }

                pattern = null;
                return false;
            }

            public static List<Pattern> ParseInput(string[] input)
            {
                var result = new List<Pattern>();
                var index = 0;
                while (TryParse(input, ref index, out var pattern))
                {
                    result.Add(pattern);
                    index++;
                }

                return result;
            }

            public int VerticalReflectionLine()
            {
                return ReflectionLine(Lines);
            }

            public int HorizontalReflectionLine()
            {
                return ReflectionLine(TransposedLines);
            }

            public int VerticalSmudgeReflectionLine()
            {
                for (int i = 0; i < Lines.Length; i++)
                {
                    for (int j = 0; j < Lines[i].Length; j++)
                    {
                        var old = Lines[i];
                        Lines[i] = old.Substring(0, j) + (old[j] == '#' ? '.' : '#') + old.Substring(j + 1);
                        var temp = ReflectionLine(Lines);
                        if (temp > 0) return temp;
                        Lines[i] = old;
                    }
                }

                return -1;
            }

            public int HorizontalSmudgeReflectionLine()
            {
                for (int i = 0; i < TransposedLines.Length; i++)
                {
                    for (int j = 0; j < TransposedLines[i].Length; j++)
                    {
                        var old = TransposedLines[i];
                        TransposedLines[i] = old.Substring(0, j) + (old[j] == '#' ? '.' : '#') + old.Substring(j + 1);
                        var temp = ReflectionLine(Lines);
                        if (temp > 0) return temp;
                        TransposedLines[i] = old;
                    }
                }

                return -1;
            }

            private int ReflectionLine(string[] lines)
            {
                for (int row = 1; row < lines.Length; row++)
                {
                    int rowsBefore = row;
                    int rowsAfter = lines.Length - row;
                    int toReflect = Math.Min(rowsBefore, rowsAfter);
                    var isReflection = true;
                    for (int reflected = 0; reflected < toReflect; reflected++)
                    {
                        var left = row - reflected - 1;
                        var right = row + reflected;
                        if (lines[left] != lines[right])
                        {
                            isReflection = false;
                            break;
                        }
                    }

                    if (isReflection)
                    {
                        return row;
                    }
                }

                return -1;
            }
        }

        public static void Solve()
        {
            var useSample = true;
            var input = useSample ?
                new[] { "#.##..##.", "..#.##.#.", "##......#", "##......#", "..#.##.#.", "..##..##.", "#.#.##.#.", "", "#...##..#", "#....#..#", "..##..###", "#####.##.", "#####.##.", "..##..###", "#....#..#" } :
                Helpers.LoadInput("day13.txt");

            var patterns = Pattern.ParseInput(input);
            var part1 = 0;
            foreach (var pattern in patterns)
            {
                var horizLine = pattern.HorizontalReflectionLine();
                if (horizLine > 0)
                {
                    part1 += horizLine;
                }
                else
                {
                    part1 += 100 * pattern.VerticalReflectionLine();
                }
            }

            Console.WriteLine("Day 13, part 1: " + part1);

            // Still incorrect
            var part2 = 0;
            foreach (var pattern in patterns)
            {
                var horizLine = pattern.VerticalSmudgeReflectionLine();
                if (horizLine > 0)
                {
                    part2 += horizLine * 100;
                }
                else
                {
                    part2 += pattern.HorizontalSmudgeReflectionLine();
                }
            }

            Console.WriteLine("Day 13, part 2: " + part2);
        }
    }

    class Day14
    {
        class Field
        {
            public char[][] Cols { get; }
            public Field(string[] Rows)
            {
                Cols = new char[Rows[0].Length][];
                for (int i = 0; i < Cols.Length; i++)
                {
                    Cols[i] = new char[Rows.Length];
                    for (int j = 0; j < Rows.Length; j++)
                    {
                        Cols[i][j] = Rows[j][i];
                    }
                }
            }

            public char[][] TiltNorth()
            {
                var result = new char[Cols.Length][];
                for (int i = 0; i < Cols.Length; i++)
                {
                    var col = Cols[i];
                    result[i] = new char[col.Length];
                    Array.Copy(col, result[i], col.Length);
                    var lastPlaceToGo = 0;
                    for (int j = 0; j < col.Length; j++)
                    {
                        switch (col[j])
                        {
                            case '.': break;
                            case '#': lastPlaceToGo = j + 1; break;
                            case 'O':
                                if (j != lastPlaceToGo)
                                {
                                    if (result[i][lastPlaceToGo] != '.') throw new Exception();
                                    result[i][j] = '.';
                                    result[i][lastPlaceToGo] = 'O';
                                }

                                lastPlaceToGo++;
                                break;
                        }
                    }
                }

                return result;
            }
        }

        public static void Solve()
        {
            var useSample = false;
            var input = useSample ?
                new[] { "O....#....", "O.OO#....#", ".....##...", "OO.#O....O", ".O.....O#.", "O.#..O.#.#", "..O..#O..O", ".......O..", "#....###..", "#OO..#...." } :
                Helpers.LoadInput("day14.txt");

            var field = new Field(input);
            var northTilt = field.TiltNorth();
            var part1 = 0;
            for (var col = 0; col < northTilt.Length; col++)
            {
                var currentCol = northTilt[col];
                for (var row = 0; row < currentCol.Length; row++)
                {
                    if (currentCol[row] == 'O')
                    {
                        part1 += currentCol.Length - row;
                    }
                }
            }

            Console.WriteLine("Day 14, part 1: " + part1);
        }
    }

    class Day15
    {
        public static void Solve()
        {
            var useSample = false;
            var input = (useSample ?
                "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7" :
                Helpers.LoadInput("day15.txt")[0]).Split(',');

            var part1 = 0;
            foreach (var part in input)
            {
                part1 += Hash(part);
            }

            Console.WriteLine("Day 15, part 1: " + part1);

            var part2 = 0L;
            Regex regex = new Regex(@"(\w+)(\=|\-)(\d+)?");
            var boxList = Enumerable.Range(0, 256).Select(_ => new List<BoxContent>()).ToArray();
            foreach (var part in input)
            {
                var match = regex.Match(part);
                var label = match.Groups[1].Value;
                var operation = match.Groups[2].Value[0];
                var box = Hash(label);
                var index = GetIndexInBox(boxList[box], label);
                switch (operation)
                {
                    case '=':
                        var focalLength = int.Parse(match.Groups[3].Value);
                        if (index < 0)
                        {
                            boxList[box].Add(new BoxContent(label, focalLength));
                        }
                        else
                        {
                            boxList[box][index].FocalLength = focalLength;
                        }

                        break;
                    case '-':
                        if (index >= 0)
                        {
                            boxList[box].RemoveAt(index);
                        }

                        break;
                }
            }

            for (int i = 0; i < boxList.Length; i++)
            {
                for (int j = 0; j < boxList[i].Count; j++)
                {
                    var focusingPower = (i + 1) * (j + 1) * boxList[i][j].FocalLength;
                    part2 += focusingPower;
                }
            }

            Console.WriteLine("Day 15, part 2: " + part2);
        }

        static int GetIndexInBox(List<BoxContent> contents, string label)
        {
            for (int i = 0; i < contents.Count; i++)
            {
                if (contents[i].Label == label)
                {
                    return i;
                }
            }

            return -1;
        }

        class BoxContent
        {
            public string Label { get; }
            public int FocalLength { get; set; }
            public BoxContent(string label, int focalLength)
            {
                Label = label;
                FocalLength = focalLength;
            }
        }

        private static int Hash(string value)
        {
            int result = 0;
            foreach (var c in value)
            {
                result += (int)c;
                result *= 17;
                result = result % 256;
            }

            return result;
        }
    }

    class Day16
    {
        public enum Direction { North, South, East, West }

        public class Coord
        {
            public int Row { get; }
            public int Col { get; }
            public Coord(int row, int col)
            {
                Row = row;
                Col = col;
            }

            public override int GetHashCode()
            {
                return Row.GetHashCode() ^ Col.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is Coord other
                    && Row == other.Row
                    && Col == other.Col;
            }
        }

        public class CoordAndDirection
        {
            public int Row { get; }
            public int Col { get; }
            public Direction Direction { get; }
            public CoordAndDirection(int row, int col, Direction direction)
            {
                Row = row;
                Col = col;
                Direction = direction;
            }

            public override int GetHashCode()
            {
                return Row.GetHashCode() ^ Col.GetHashCode() ^ Direction.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is CoordAndDirection other
                    && Row == other.Row
                    && Col == other.Col
                    && Direction == other.Direction;
            }
        }

        public static void Solve()
        {
            var useSample = false;
            var input = useSample ?
                new[] { ".|...\\....", "|.-.\\.....", ".....|-...", "........|.", "..........", ".........\\", "..../.\\\\..", ".-.-/..|..", ".|....-|.\\", "..//.|...." } :
                Helpers.LoadInput("day16.txt");

            var rowCount = input.Length;
            var colCount = input[0].Length;

            int part1 = FindEnergizedTiles(input, 0, 0, rowCount, colCount, Direction.East);

            Console.WriteLine("Day 16, part 1: " + part1);

            int part2 = 0;
            for (int r = 0; r < rowCount; r++)
            {
                var temp = FindEnergizedTiles(input, r, 0, rowCount, colCount, Direction.East);
                if (temp > part2) part2 = temp;

                temp = FindEnergizedTiles(input, r, colCount - 1, rowCount, colCount, Direction.West);
                if (temp > part2) part2 = temp;
            }

            for (int c = 0; c < colCount; c++)
            {
                var temp = FindEnergizedTiles(input, 0, c, rowCount, colCount, Direction.South);
                if (temp > part2) part2 = temp;

                temp = FindEnergizedTiles(input, rowCount - 1, c, rowCount, colCount, Direction.North);
                if (temp > part2) part2 = temp;
            }

            Console.WriteLine("Day 16, part 2: " + part2);
        }

        private static int FindEnergizedTiles(string[] input, int initialRow, int initialCol, int rowCount, int colCount, Direction initialDirection)
        {
            var queue = new Queue<CoordAndDirection>();
            queue.Enqueue(new CoordAndDirection(initialRow, initialCol, initialDirection));
            HashSet<CoordAndDirection> visitedWithDirection = new HashSet<CoordAndDirection>();
            HashSet<Coord> visited = new HashSet<Coord>();
            while (queue.Count > 0)
            {
                var next = queue.Dequeue();
                var row = next.Row;
                var col = next.Col;
                var dir = next.Direction;
                visited.Add(new Coord(row, col));
                visitedWithDirection.Add(new CoordAndDirection(row, col, dir));
                int nextRow = row, nextCol = col;
                int? nextRow2 = null, nextCol2 = null;
                Direction? nextDir2 = null;
                Direction nextDir = dir;
                switch (input[row][col])
                {
                    case '.':
                        switch (dir)
                        {
                            case Direction.East:
                                nextCol++;
                                break;
                            case Direction.West:
                                nextCol--;
                                break;
                            case Direction.North:
                                nextRow--;
                                break;
                            case Direction.South:
                                nextRow++;
                                break;
                        }
                        break;

                    case '\\':
                        switch (dir)
                        {
                            case Direction.East:
                                nextRow++;
                                nextDir = Direction.South;
                                break;
                            case Direction.West:
                                nextRow--;
                                nextDir = Direction.North;
                                break;
                            case Direction.North:
                                nextCol--;
                                nextDir = Direction.West;
                                break;
                            case Direction.South:
                                nextCol++;
                                nextDir = Direction.East;
                                break;
                        }
                        break;

                    case '/':
                        switch (dir)
                        {
                            case Direction.East:
                                nextRow--;
                                nextDir = Direction.North;
                                break;
                            case Direction.West:
                                nextRow++;
                                nextDir = Direction.South;
                                break;
                            case Direction.North:
                                nextCol++;
                                nextDir = Direction.East;
                                break;
                            case Direction.South:
                                nextCol--;
                                nextDir = Direction.West;
                                break;
                        }
                        break;

                    case '-':
                        switch (dir)
                        {
                            case Direction.East:
                                nextCol++;
                                break;
                            case Direction.West:
                                nextCol--;
                                break;
                            case Direction.North:
                            case Direction.South:
                                nextCol++;
                                nextDir = Direction.East;
                                nextRow2 = row;
                                nextCol2 = col - 1;
                                nextDir2 = Direction.West;
                                break;
                        }
                        break;

                    case '|':
                        switch (dir)
                        {
                            case Direction.East:
                            case Direction.West:
                                nextRow++;
                                nextDir = Direction.South;
                                nextRow2 = row - 1;
                                nextCol2 = col;
                                nextDir2 = Direction.North;
                                break;
                            case Direction.North:
                                nextRow--;
                                break;
                            case Direction.South:
                                nextRow++;
                                break;
                        }
                        break;
                }

                if (0 <= nextRow && nextRow < rowCount && 0 <= nextCol && nextCol < rowCount)
                {
                    var toEnqueue = new CoordAndDirection(nextRow, nextCol, nextDir);
                    if (!visitedWithDirection.Contains(toEnqueue))
                    {
                        queue.Enqueue(toEnqueue);
                    }
                }

                if (nextRow2.HasValue && 0 <= nextRow2.Value && nextRow2.Value < rowCount && 0 <= nextCol2.Value && nextCol2.Value < colCount)
                {
                    var toEnqueue = new CoordAndDirection(nextRow2.Value, nextCol2.Value, nextDir2.Value);
                    if (!visitedWithDirection.Contains(toEnqueue))
                    {
                        queue.Enqueue(toEnqueue);
                    }
                }
            }

            return visited.Count;
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
