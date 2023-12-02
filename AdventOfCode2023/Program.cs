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
            Day2.Solve();
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

    public class Helpers
    {
        public static string[] LoadInput(string fileName)
        {
            return File.ReadAllLines(Path.Combine("inputs", fileName));
        }
    }
}
