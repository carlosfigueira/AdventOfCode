namespace AdventOfCode2024
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Day1.Solve();
            //Day2.Solve();
            Day3.Solve();
        }
    }

    class Day1
    {
        public static void Solve()
        {
            var sampleInput = new[] { "3   4", "4   3", "2   5", "1   3", "3   9", "3   3" };

            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day1.txt");

            var ids = lines
                .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToArray())
                .Select(arr => new { left = arr[0], right = arr[1] });
            var leftIds = ids.Select(i => int.Parse(i.left)).ToList();
            leftIds.Sort();
            var rightIds = ids.Select(i => int.Parse(i.right)).ToList();
            rightIds.Sort();
            var sum = 0;
            for (int i = 0; i < leftIds.Count; i++)
            {
                sum += Math.Abs(leftIds[i] - rightIds[i]);
            }

            Console.WriteLine(sum);

            var similarity = 0;
            var previous = int.MinValue;
            var previousSimilarity = -1;
            for (var i = 0; i < leftIds.Count; i++)
            {
                var left = leftIds[i];
                if (left == previous)
                {
                    similarity += previousSimilarity;
                    continue;
                }

                var rightIndex = rightIds.BinarySearch(left);
                if (rightIndex < 0)
                {
                    previousSimilarity = -1;
                    previous = int.MinValue;
                    continue;
                }

                var before = 0;
                for (var j = rightIndex - 1; j >= 0 && rightIds[j] == rightIds[rightIndex]; j--) before++;
                var after = 0;
                for (var j = rightIndex + 1; j < rightIds.Count && rightIds[j] == rightIds[rightIndex]; j++) after++;
                var currentSimilarity = (before + 1 + after) * left;

                similarity += currentSimilarity;

                previous = left;
                previousSimilarity = currentSimilarity;
            }

            Console.WriteLine(similarity);
        }
    }

    class Day2
    {
        public static void Solve()
        {
            var sampleInput = new[]
            {
                "7 6 4 2 1",
                "1 2 7 8 9",
                "9 7 6 2 1",
                "1 3 2 4 5",
                "8 6 4 4 1",
                "1 3 6 7 9"
            };

            var useSample = false;
            var lines = useSample ? sampleInput : Helpers.LoadInput("day2.txt");

            var values = lines
                .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i.Trim())).ToArray());

            var safeCount = 0;
            foreach (var line in values)
            {
                if (IsSafe(line)) safeCount++;
            }

            Console.WriteLine(safeCount);

            var safeWithDampenerCount = 0;
            // Could be smarter, but works well enough for this input
            foreach (var line in values)
            {
                if (IsSafe(line))
                {
                    safeWithDampenerCount++;
                }
                else
                {
                    var isSafe = false;
                    for (var i = 0; i < line.Length; i++)
                    {
                        if (IsSafe(line.Where((value, index) => index != i).ToArray()))
                        {
                            isSafe = true;
                            break;
                        }
                    }

                    if (isSafe)
                    {
                        safeWithDampenerCount++;
                    }
                }
            }

            Console.WriteLine(safeWithDampenerCount);
        }

        static bool IsSafe(int[] array)
        {
            if (array.Length < 2) return true;
            var ascending = array[0] < array[1];
            for (int i = 1; i < array.Length; i++)
            {
                var diff = ascending ? (array[i] - array[i - 1]) : (array[i - 1] - array[i]);
                if (diff < 1 || diff > 3) return false;
            }

            return true;
        }
    }

    class Day3
    {
        public static void Solve()
        {
            var sampleInput = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
            var useSample = false;
            var input = useSample ? sampleInput : string.Join("", Helpers.LoadInput("day3.txt").Where(l => !string.IsNullOrEmpty(l)));

            int result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != 'm') continue;
                i++; if (i >= input.Length || input[i] != 'u') continue;
                i++; if (i >= input.Length || input[i] != 'l') continue;
                i++; if (i >= input.Length || input[i] != '(') continue;
                i++; if (i >= input.Length) continue;
                if (!char.IsAsciiDigit(input[i])) continue;
                var op1 = input[i] - '0';
                var op1Start = i;
                var invalid = false;
                i++;
                while (i < input.Length && char.IsAsciiDigit(input[i]))
                {
                    op1 *= 10;
                    op1 += input[i] - '0';
                    i++;
                    if (i - op1Start > 3)
                    {
                        invalid = true;
                        break;
                    }
                }

                if (invalid) continue;
                if (i >= input.Length || input[i] != ',') continue;
                i++; if (i >= input.Length) continue;
                if (!char.IsAsciiDigit(input[i])) continue;
                var op2 = input[i] - '0';
                var op2Start = i;
                invalid = false;
                i++;
                while (i < input.Length && char.IsAsciiDigit(input[i]))
                {
                    op2 *= 10;
                    op2 += input[i] - '0';
                    i++;
                    if (i - op2Start > 3)
                    {
                        invalid = true;
                        break;
                    }
                }

                if (i >= input.Length || input[i] != ')') continue;
                result += op1 * op2;
            }

            Console.WriteLine(result);
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
