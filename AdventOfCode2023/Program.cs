using System;
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
            Day8.Solve();
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

    public class Helpers
    {
        public static string[] LoadInput(string fileName)
        {
            return File.ReadAllLines(Path.Combine("inputs", fileName));
        }
    }
}
