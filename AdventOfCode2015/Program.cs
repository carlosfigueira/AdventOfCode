using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2015
{
    class Program
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
            var input = Helpers.LoadInput("day1.txt");

            var floor = 0;
            var firstBasement = -1;
            for (var i = 0; i < input[0].Length; i++)
            {
                if (input[0][i] == '(')
                {
                    floor++;
                }
                else
                {
                    floor--;
                }

                if (floor == -1 && firstBasement < 0)
                {
                    firstBasement = i + 1;
                }
            }

            Console.WriteLine("Day 1, part 1: " + floor);
            Console.WriteLine("Day 1, part 2: " + firstBasement);
        }
    }

    class Day2
    {
        public static void Solve()
        {
            var input = Helpers.LoadInput("day2.txt");
            long part1 = 0;
            long part2 = 0;
            foreach (var line in input)
            {
                var numbers = line.Split('x').Select(n => int.Parse(n)).OrderBy(n => n).ToArray();
                part1 += 3 * numbers[0] * numbers[1] + 2 * numbers[0] * numbers[2] + 2 * numbers[1] * numbers[2];
                part2 += 2 * (numbers[0] + numbers[1]) + numbers[0] * numbers[1] * numbers[2];
            }

            Console.WriteLine("Day 2, part 1: " + part1);
            Console.WriteLine("Day 2, part 2: " + part2);
        }
    }

    class Day3
    {
        public static void Solve()
        {
            var input = Helpers.LoadInput("day3.txt");
            var directions = input[0];

            var visited = new HashSet<Coord2D>();
            var x = 0;
            var y = 0;
            visited.Add(new Coord2D(x, y));
            foreach (var c in directions)
            {
                switch (c)
                {
                    case '^': y--; break;
                    case 'v': y++; break;
                    case '>': x++; break;
                    case '<': x--; break;
                }

                visited.Add(new Coord2D(x, y));
            }

            Console.WriteLine("Day 3, part 1: " + visited.Count);

            visited.Clear();
            x = 0;
            y = 0;
            var x2 = 0;
            var y2 = 0;
            var useFirst = true;
            visited.Add(new Coord2D(x, y));
            foreach (var c in directions)
            {
                if (useFirst)
                {
                    switch (c)
                    {
                        case '^': y--; break;
                        case 'v': y++; break;
                        case '>': x++; break;
                        case '<': x--; break;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '^': y2--; break;
                        case 'v': y2++; break;
                        case '>': x2++; break;
                        case '<': x2--; break;
                    }
                }

                visited.Add(new Coord2D(x, y));
                visited.Add(new Coord2D(x2, y2));
                useFirst = !useFirst;
            }

            Console.WriteLine("Day 3, part 2: " + visited.Count);
        }
    }

    class Day4
    {
        public static void Solve()
        {
            var input = "ckczppom";
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                for (int i = 0; i < 100000000; i++)
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input + i);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    if (hashBytes[0] == 0 && hashBytes[1] == 0 && (hashBytes[2] & 0xF0) == 0)
                    {
                        Console.WriteLine("Day 4, part 1: " + i);
                        break;
                    }
                }
            }
        }
    }

    public class Coord2D
    {
        public int X;
        public int Y;
        public Coord2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Coord2D;
            return other != null && this.X == other.X && this.Y == other.Y;
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
