using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    class Program
    {
        static void Main(string[] args)
        {
            Day1.Solve();
        }

        class Day1
        {
            static List<int> GetCaloriesPerElf(string[] inputLines)
            {
                var caloriesPerElf = new List<int>();
                var currentElf = 0;
                for (int i = 0; i < inputLines.Length; i++)
                {
                    if (inputLines[i] == "")
                    {
                        caloriesPerElf.Add(currentElf);
                        currentElf = 0;
                    } else
                    {
                        currentElf += int.Parse(inputLines[i]);
                    }
                }

                caloriesPerElf.Add(currentElf);
                return caloriesPerElf;
            }
            public static void Solve()
            {
                // input: "https://adventofcode.com/2022/day/1/input"
                var input = Helpers.LoadInput("input1.txt");
                var caloriesPerElf = GetCaloriesPerElf(input);
                Console.WriteLine($"Part 1: {caloriesPerElf.Max()}");
                Console.WriteLine($"Part 2: {caloriesPerElf.OrderByDescending(c => c).Take(3).Sum()}");
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
