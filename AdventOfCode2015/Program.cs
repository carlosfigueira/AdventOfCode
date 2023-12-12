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
            Day2.Solve();
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
            foreach (var line in input)
            {
                var numbers = line.Split('x').Select(n => int.Parse(n)).OrderBy(n => n).ToArray();
                part1 += 3 * numbers[0] * numbers[1] + 2 * numbers[0] * numbers[2] + 2 * numbers[1] * numbers[2];
            }

            Console.WriteLine("Day 2, part 1: " + part1);
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
