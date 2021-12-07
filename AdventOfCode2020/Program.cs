﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Day 15:");
            //Day1.Solve();
            //Day2.Solve();
            //Day3.Solve();
            //Day4.Solve();
            Day5.Solve();
            //Day15.Solve();
        }
    }

    class Day1
    {
        public static void Solve()
        {
            var inputs = Helpers.LoadInput("input1.txt").Select(s => int.Parse(s)).ToArray();
            Array.Sort(inputs);
            (int, int) FindIndicesThatAddTo(int value) => FindIndicesThatAddToPartial(value, 0, inputs.Length - 1);
            (int, int) FindIndicesThatAddToPartial(int value, int minIndex, int maxIndex)
            {
                int min = minIndex, max = maxIndex;
                int sum = 0;
                while (min < max && (sum = inputs[min] + inputs[max]) != value)
                {
                    if (sum < value)
                    {
                        min++;
                    }
                    else if (sum > value)
                    {
                        max--;
                    }
                }

                if (sum == value)
                {
                    return (min, max);
                }
                else
                {
                    return (-1, -1);
                }
            }

            var (min, max) = FindIndicesThatAddTo(2020);

            Console.WriteLine($"Part 1: {inputs[min]} * {inputs[max]} = {inputs[min] * inputs[max]}");

            for (int i = 0; i < inputs.Length - 2; i++)
            {
                var (mid, high) = FindIndicesThatAddToPartial(2020 - inputs[i], i + 1, inputs.Length - 1);
                if (mid > 0)
                {
                    Console.WriteLine($"Part 2: {inputs[i]} * {inputs[mid]} * {inputs[high]} = {inputs[i] * inputs[mid] * inputs[high]}");
                }
            }
        }
    }

    class Day2
    {
        public static void Solve()
        {
            var inputs = Helpers.LoadInput("input2.txt");
            Regex regex = new Regex(@"(?<min>\d+)\-(?<max>\d+) (?<char>[a-z])\: (?<pwd>\w+)");
            int validPasswords = 0;
            foreach (var input in inputs)
            {
                var match = regex.Match(input);
                int min = int.Parse(match.Groups["min"].Value);
                int max = int.Parse(match.Groups["max"].Value);
                char c = match.Groups["char"].Value[0];
                string pwd = match.Groups["pwd"].Value;
                int cCount = pwd.Sum(pwdChar => (pwdChar == c ? 1 : 0));
                if (min <= cCount && cCount <= max) validPasswords++;
            }

            Console.WriteLine($"Part 1: {validPasswords}");

            regex = new Regex(@"(?<pos1>\d+)\-(?<pos2>\d+) (?<char>[a-z])\: (?<pwd>\w+)");
            validPasswords = 0;
            foreach (var input in inputs)
            {
                var match = regex.Match(input);
                int pos1 = int.Parse(match.Groups["pos1"].Value);
                int pos2 = int.Parse(match.Groups["pos2"].Value);
                char c = match.Groups["char"].Value[0];
                string pwd = match.Groups["pwd"].Value;
                if (pwd[pos1 - 1] == c ^ pwd[pos2 - 1] == c)
                {
                    validPasswords++;
                }
            }

            Console.WriteLine($"Part 2: {validPasswords}");
        }
    }

    class Day3
    {
        public static void Solve()
        {
            var inputs = Helpers.LoadInput("input3.txt");
            int col = 0;
            int trees = 0;
            for (int row = 0; row < inputs.Length; row++, col += 3)
            {
                if (inputs[row][col % inputs[row].Length] == '#') trees++;
            }

            Console.WriteLine($"Part 1: {trees}");

            int slope2 = trees;

            col = 0; trees = 0;
            for (int row = 0; row < inputs.Length; row++, col += 1)
            {
                if (inputs[row][col % inputs[row].Length] == '#') trees++;
            }

            int slope1 = trees;

            col = 0; trees = 0;
            for (int row = 0; row < inputs.Length; row++, col += 5)
            {
                if (inputs[row][col % inputs[row].Length] == '#') trees++;
            }

            int slope3 = trees;

            col = 0; trees = 0;
            for (int row = 0; row < inputs.Length; row++, col += 7)
            {
                if (inputs[row][col % inputs[row].Length] == '#') trees++;
            }

            int slope4 = trees;

            col = 0; trees = 0;
            for (int row = 0; row < inputs.Length; row += 2, col += 1)
            {
                if (inputs[row][col % inputs[row].Length] == '#') trees++;
            }

            int slope5 = trees;
            Console.WriteLine($"Part 2: {1L * slope1 * slope2 * slope3 * slope4 * slope5}");
        }
    }

    class Day4
    {
        public static void Solve()
        {
            var input = Helpers.LoadInput("input4.txt");
            int start = 0;
            int end = 1;
            var passports = new List<Passport>();
            while (end < input.Length)
            {
                while (end < input.Length && !string.IsNullOrWhiteSpace(input[end])) end++;
                if (end < input.Length)
                {
                    passports.Add(Passport.Parse(input, start, end));
                    start = end + 1;
                    end = start + 1;
                }
            }

            passports.Add(Passport.Parse(input, start, end));
            int part1 = 0;
            foreach (var passport in passports)
            {
                if (passport.IsValid()) part1++;
            }

            Console.WriteLine($"Part 1: {part1}");

            int part2 = 0;
            foreach (var passport in passports)
            {
                if (passport.IsReallyValid()) part2++;
            }

            Console.WriteLine($"Part 2: {part2}");
        }

        class Passport
        {
            //public string byr;
            //public string iyr;
            //public string eyr;
            //public string hgt;
            //public string hcl;
            //public string ecl;
            //public string pid;
            //public string cid;

            public Dictionary<string, string> fields;

            public bool IsValid()
            {
                var requiredFields = new[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
                foreach (var requiredField in requiredFields)
                {
                    if (!fields.ContainsKey(requiredField)) return false;
                }

                return true;
            }

            private static Dictionary<string, Regex> validations = new Dictionary<string, Regex>
            {
                { "byr", new Regex(@"^(?<value>\d\d\d\d)$") },
                { "iyr", new Regex(@"^(?<value>\d\d\d\d)$") },
                { "eyr", new Regex(@"^(?<value>\d\d\d\d)$") },
                { "hgt", new Regex(@"^(?<value>\d{2,3})((cm)|(in))$") },
                { "hcl", new Regex(@"^\#[0-9a-f]{6}$") },
                { "ecl", new Regex(@"^((amb)|(blu)|(brn)|(gry)|(grn)|(hzl)|(oth))$") },
                { "pid", new Regex(@"^\d{9}$") },
            };

            public bool IsReallyValid()
            {
                if (!IsValid()) return false;
                foreach (var field in validations.Keys)
                {
                    var match = validations[field].Match(this.fields[field]);
                    if (!match.Success) return false;
                    int value = -1;
                    if (match.Groups["value"].Success)
                    {
                        value = int.Parse(match.Groups["value"].Value);
                    }

                    switch (field)
                    {
                        case "byr":
                            if (value < 1920 || value > 2002) return false;
                            break;
                        case "iyr":
                            if (value < 2010 || value > 2020) return false;
                            break;
                        case "eyr":
                            if (value < 2020 || value > 2030) return false;
                            break;
                        case "hgt":
                            if (this.fields[field].EndsWith("in") && (value < 59 || value > 76)) return false;
                            if (this.fields[field].EndsWith("cm") && (value < 150 || value > 193)) return false;
                            break;
                    }
                }

                return true;
            }

            public static Passport Parse(string[] lines, int startIndex, int endIndex)
            {
                var sb = new StringBuilder();
                for (int i = startIndex; i < endIndex; i++)
                {
                    sb.Append(lines[i]);
                    sb.Append(" ");
                }

                var result = new Dictionary<string, string>();
                foreach (var pair in sb.ToString().Split(new[] { ' '}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = pair.Split(':');
                    result.Add(parts[0], parts[1]);
                }

                return new Passport { fields = result };
            }
        }
    }

    class Day5
    {
        public static void Solve()
        {
            var input = Helpers.LoadInput("input5.txt");
            //input = new[] { "BFFFBBFRRR", "FFFBBBFRRR", "BBFFBBFRLL" };
            var seatIds = input.Select(line =>
            {
                int row = 0;
                for (int i = 0; i < 7; i++)
                {
                    row *= 2;
                    row += line[i] == 'B' ? 1 : 0;
                }

                int col = 0;
                for (int i = 0; i < 3; i++)
                {
                    col *= 2;
                    col += line[i + 7] == 'R' ? 1 : 0;
                }

                return col + row * 8;
            }).ToArray();
            Array.Sort(seatIds);

            var maxSeatId = seatIds[seatIds.Length - 1];
            Console.WriteLine($"Part 1: {maxSeatId}");

            for (int i = 1; i < seatIds.Length; i++)
            {
                if (seatIds[i - 1] + 2 == seatIds[i])
                {
                    Console.WriteLine($"Part 2: {seatIds[i] - 1}");
                    break;
                }
            }
        }
    }

    class Day15
    {
        public static void Solve()
        {
            var numbers = new List<int> { 5, 1, 9, 18, 13, 8, 0 };
            //numbers = new List<int> { 0, 3, 6 };

            int targetTurn = 30000000;
            Dictionary<int, int> lastTurn = new Dictionary<int, int>();
            for (var i = 0; i < numbers.Count; i++)
            {
                lastTurn[numbers[i]] = i + 1;
            }
            int turn = numbers.Count + 1;
            int nextNumber = 0;
            while (turn <= targetTurn)
            {
                int followingNumber = lastTurn.ContainsKey(nextNumber) ?
                    turn - lastTurn[nextNumber] :
                    0;
                lastTurn[nextNumber] = turn;

                if (turn == targetTurn || turn == 2020)
                {
                    Console.WriteLine($"Turn {turn}: {nextNumber}");
                }

                nextNumber = followingNumber;
                turn++;
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
