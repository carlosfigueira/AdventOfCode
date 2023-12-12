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
            Day1.Solve();
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

    public class Helpers
    {
        public static string[] LoadInput(string fileName)
        {
            return File.ReadAllLines(Path.Combine("inputs", fileName));
        }
    }
}
