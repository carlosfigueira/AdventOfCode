using System;
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
            Day14.Solve();
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
    
    class Day6
    {
        public static void Solve()
        {
            var input = Helpers.LoadInput("input6.txt");
            int start = 0;
            int end = 1;
            var groups = new List<Group>();
            while (end < input.Length)
            {
                while (end < input.Length && !string.IsNullOrWhiteSpace(input[end])) end++;
                if (end < input.Length)
                {
                    groups.Add(new Group { Answers = input.Skip(start).Take(end - start).ToArray() });
                    start = end + 1;
                    end = start + 1;
                }
            }

            groups.Add(new Group { Answers = input.Skip(start).Take(end - start).ToArray() });

            int part1 = groups.Sum(g => g.QuestionsAnyoneAnsweredYes());
            Console.WriteLine($"Part 1: {part1}");

            int part2 = groups.Sum(g => g.QuestionsEveryoneAnsweredYes());
            Console.WriteLine($"Part 2: {part2}");
        }

        class Group
        {
            public string[] Answers;

            public int QuestionsAnyoneAnsweredYes()
            {
                bool[] questions = new bool[26];
                foreach (var answer in Answers)
                {
                    foreach (var c in answer)
                    {
                        questions[c - 'a'] = true;
                    }
                }

                return questions.Sum(q => q ? 1 : 0);
            }

            public int QuestionsEveryoneAnsweredYes()
            {
                int[] questions = new int[26];
                foreach (var answer in Answers)
                {
                    foreach (var c in answer)
                    {
                        questions[c - 'a']++;
                    }
                }

                return questions.Sum(q => q == Answers.Length ? 1 : 0);
            }
        }
    }

    class Day7
    {
        public static void Solve()
        {
            var testRules = new[]
            {
                "light red bags contain 1 bright white bag, 2 muted yellow bags.",
                "dark orange bags contain 3 bright white bags, 4 muted yellow bags.",
                "bright white bags contain 1 shiny gold bag.",
                "muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.",
                "shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.",
                "dark olive bags contain 3 faded blue bags, 4 dotted black bags.",
                "vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.",
                "faded blue bags contain no other bags.",
                "dotted black bags contain no other bags."
            };
            var input =
                //testRules
                Helpers.LoadInput("input7.txt")
                .Select(l => Bag.Parse(l))
                .ToArray();
            var carries = input.ToDictionary(b => b.Color, b => b.Holds.Select(t => t.Item1).ToArray());
            var canBeCarried = new Dictionary<string, List<string>>();
            foreach (var bag in input)
            {
                foreach (var held in bag.Holds)
                {
                    if (!canBeCarried.ContainsKey(held.Item1))
                    {
                        canBeCarried.Add(held.Item1, new List<string>());
                    }

                    canBeCarried[held.Item1].Add(bag.Color);
                }
            }
            HashSet<string> visited = new HashSet<string>();
            visited.Add("shiny gold");
            Queue<string> toVisit = new Queue<string>();
            toVisit.Enqueue("shiny gold");
            int part1 = 0;
            while (toVisit.Count > 0)
            {
                var next = toVisit.Dequeue();
                if (carries.ContainsKey(next) && next != "shiny gold")
                {
                    // Found a bag that can carry a shiny gold
                    part1++;
                }

                if (canBeCarried.ContainsKey(next))
                {
                    foreach (var nextToEnqueue in canBeCarried[next])
                    {
                        if (!visited.Contains(nextToEnqueue))
                        {
                            visited.Add(nextToEnqueue);
                            toVisit.Enqueue(nextToEnqueue);
                        }
                    }
                }
            }

            Console.WriteLine($"Part 1: {part1}");

            int part2 = BagsInside(input.ToDictionary(b => b.Color), "shiny gold");
            Console.WriteLine($"Part 2: {part2}");
        }

        private static int BagsInside(Dictionary<string, Bag> allBags, string bagColor)
        {
            var bag = allBags[bagColor];
            if (bag.Holds.Count == 0) return 0;
            var result = 0;
            foreach (var heldBag in bag.Holds)
            {
                result += (1 + BagsInside(allBags, heldBag.Item1)) * heldBag.Item2;
            }

            return result;
        }

        class Bag
        {
            public string Color { get; set; }
            public List<Tuple<string, int>> Holds { get; set; }

            public static Bag Parse(string definition)
            {
                string separator = " bags contain";
                int sepIndex = definition.IndexOf(separator);
                string color = definition.Substring(0, sepIndex);
                string holdsDef = definition.Substring(sepIndex + separator.Length).TrimEnd('.');
                var holds = new List<Tuple<string, int>>();
                if (holdsDef != " no other bags")
                {
                    foreach (var holdDef in holdsDef.Split(','))
                    {
                        var def = holdDef.Substring(1)
                            .TrimEnd('b', 'a', 'g', 's')
                            .TrimEnd(' '); // remove space and 'bag(s)'
                        var spaceIndex = def.IndexOf(' ');
                        var holdNumber = int.Parse(def.Substring(0, spaceIndex));
                        var holdColor = def.Substring(spaceIndex + 1);
                        holds.Add(Tuple.Create(holdColor, holdNumber));
                    }
                }

                return new Bag { Holds = holds, Color = color };
            }
        }
    }

    class Day8
    {
        public static void Solve()
        {
            var testInput = new[]
            {
                "nop +0",
                "acc +1",
                "jmp +4",
                "acc +3",
                "jmp -3",
                "acc -99",
                "acc +1",
                "jmp -4",
                "acc +6"
            };
            var input =
                //testInput
                Helpers.LoadInput("input8.txt")
                .Select(l => Instruction.Parse(l))
                .ToArray();

            var computer = new Computer(input);
            computer.RunUntilLoop();
            Console.WriteLine($"Part 1: {computer.Accumulator}");

            computer.RunSwappingOneJmpNop();
            Console.WriteLine($"Part 2: {computer.Accumulator}");
        }

        enum InstructionType { Acc, Jmp, Nop }
        class Instruction
        {
            public InstructionType Type;
            public int Arg;

            public static Instruction Parse(string line)
            {
                var parts = line.Split(' ');
                var type = Enum.Parse<InstructionType>(parts[0], true);
                var arg = int.Parse(parts[1]);
                return new Instruction { Type = type, Arg = arg };
            }
        }

        class Computer
        {
            public int Accumulator { get; private set; } = 0;
            private Instruction[] instructions;
            public Computer(Instruction[] instructions)
            {
                this.instructions = instructions;
            }

            public void RunUntilLoop()
            {
                this.Accumulator = 0;
                int ip = 0;
                bool[] visited = new bool[this.instructions.Length];
                while (!visited[ip])
                {
                    visited[ip] = true;
                    var nextInstruction = this.instructions[ip];
                    RunInstruction(nextInstruction, ref ip);
                }
            }

            private void RunInstruction(Instruction instruction, ref int ip)
            {
                switch (instruction.Type)
                {
                    case InstructionType.Acc:
                        this.Accumulator += instruction.Arg;
                        ip++;
                        break;
                    case InstructionType.Nop:
                        ip++;
                        break;
                    case InstructionType.Jmp:
                        ip += instruction.Arg;
                        break;
                }
            }

            public void RunSwappingOneJmpNop()
            {
                for (int changedIndex = 0; changedIndex < this.instructions.Length; changedIndex++)
                {
                    var instructionType = this.instructions[changedIndex].Type;
                    if (instructionType == InstructionType.Acc)
                    {
                        continue; // Cannot change acc
                    }

                    try
                    {
                        this.instructions[changedIndex].Type = instructionType == InstructionType.Jmp ? InstructionType.Nop : InstructionType.Jmp;
                        int ip = 0;
                        bool[] visited = new bool[this.instructions.Length];
                        this.Accumulator = 0;
                        while (ip < visited.Length && !visited[ip])
                        {
                            visited[ip] = true;
                            RunInstruction(this.instructions[ip], ref ip);
                        }

                        if (ip >= visited.Length)
                        {
                            // Completed
                            break;
                        }
                    }
                    finally
                    {
                        this.instructions[changedIndex].Type = instructionType;
                    }
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

    class Day17
    {
        public static void Solve()
        {
            var testInputs = new[] { ".#.", "..#", "###" };
            var input =
                //testInputs
                Helpers.LoadInput("input17.txt")
                ;
            var board = new Board3D(input);
            Console.WriteLine($"After 0 iterations, there are {board.ActiveCells} live cells");
            for (int i = 1; i <= 6; i++)
            {
                board.Iterate();
                Console.WriteLine($"After {i} iterations, there are {board.ActiveCells} live cells");
            }

            Console.WriteLine($"Part 1: {board.ActiveCells}");

            var board2 = new Board4D(input);
            Console.WriteLine($"After 0 iterations, there are {board2.ActiveCells} live cells");
            for (int i = 1; i <= 6; i++)
            {
                board2.Iterate();
                Console.WriteLine($"After {i} iterations, there are {board2.ActiveCells} live cells");
            }

            Console.WriteLine($"Part 2: {board2.ActiveCells}");
        }

        class Board3D
        {
            HashSet<Tuple<int, int, int>> liveCells;
            int minX, maxX, minY, maxY, minZ, maxZ;

            public int ActiveCells => this.liveCells.Count;

            public Board3D(string[] initialState)
            {
                this.minZ = this.maxZ = 0;
                this.minX = this.minY = int.MaxValue;
                this.maxX = this.maxY = int.MinValue;
                this.liveCells = new HashSet<Tuple<int, int, int>>();
                for (var y = 0; y < initialState.Length; y++)
                {
                    for (int x = 0; x < initialState[y].Length; x++)
                    {
                        if (initialState[y][x] == '#')
                        {
                            this.liveCells.Add(Tuple.Create(x, y, 0));
                            this.minX = Math.Min(this.minX, x);
                            this.maxX = Math.Max(this.maxX, x);
                            this.minY = Math.Min(this.minY, y);
                            this.maxY = Math.Max(this.maxY, y);
                        }
                    }
                }
            }

            public void Iterate()
            {
                var newCells = new HashSet<Tuple<int, int, int>>();
                int newMinX = int.MaxValue;
                int newMinY = int.MaxValue;
                int newMinZ = int.MaxValue;
                int newMaxX = int.MinValue;
                int newMaxY = int.MinValue;
                int newMaxZ = int.MinValue;
                for (int z = this.minZ - 1; z <= this.maxZ + 1; z++)
                {
                    for (int y = this.minY - 1; y <= this.maxY + 1; y++)
                    {
                        for (int x = this.minX - 1; x <= this.maxX + 1; x++)
                        {
                            var currentPosition = Tuple.Create(x, y, z);
                            bool currentCell = this.liveCells.Contains(currentPosition);
                            int neighbors = this.CountLiveNeighbors(this.liveCells, x, y, z);
                            if (neighbors == 3 || (neighbors == 2 && currentCell))
                            {
                                newCells.Add(currentPosition);
                                newMinX = Math.Min(x, newMinX);
                                newMinY = Math.Min(y, newMinY);
                                newMinZ = Math.Min(z, newMinZ);
                                newMaxX = Math.Max(x, newMaxX);
                                newMaxY = Math.Max(y, newMaxY);
                                newMaxZ = Math.Max(z, newMaxZ);
                            }
                        }
                    }
                }

                this.liveCells = newCells;
                this.minX = newMinX;
                this.minY = newMinY;
                this.minZ = newMinZ;
                this.maxX = newMaxX;
                this.maxY = newMaxY;
                this.maxZ = newMaxZ;
            }

            private int CountLiveNeighbors(HashSet<Tuple<int, int, int>> liveCells, int x, int y, int z)
            {
                int result = 0;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int k = -1; k <= 1; k++)
                        {
                            if (i != 0 || j != 0 || k != 0)
                            {
                                var neighbor = Tuple.Create(x + i, y + j, z + k);
                                if (liveCells.Contains(neighbor))
                                {
                                    result++;
                                }
                            }
                        }
                    }
                }

                return result;
            }
        }

        class Board4D
        {
            HashSet<Tuple<int, int, int, int>> liveCells;
            int minX, maxX, minY, maxY, minZ, maxZ, minW, maxW;

            public int ActiveCells => this.liveCells.Count;

            public Board4D(string[] initialState)
            {
                this.minZ = this.maxZ = this.minW = this.maxW = 0;
                this.minX = this.minY = int.MaxValue;
                this.maxX = this.maxY = int.MinValue;
                this.liveCells = new HashSet<Tuple<int, int, int, int>>();
                for (var y = 0; y < initialState.Length; y++)
                {
                    for (int x = 0; x < initialState[y].Length; x++)
                    {
                        if (initialState[y][x] == '#')
                        {
                            this.liveCells.Add(Tuple.Create(x, y, 0, 0));
                            this.minX = Math.Min(this.minX, x);
                            this.maxX = Math.Max(this.maxX, x);
                            this.minY = Math.Min(this.minY, y);
                            this.maxY = Math.Max(this.maxY, y);
                        }
                    }
                }
            }

            public void Iterate()
            {
                var newCells = new HashSet<Tuple<int, int, int, int>>();
                int newMinX = int.MaxValue;
                int newMinY = int.MaxValue;
                int newMinZ = int.MaxValue;
                int newMinW = int.MaxValue;
                int newMaxX = int.MinValue;
                int newMaxY = int.MinValue;
                int newMaxZ = int.MinValue;
                int newMaxW = int.MinValue;
                for (int w = this.minW - 1; w <= this.maxW + 1; w++)
                {
                    for (int z = this.minZ - 1; z <= this.maxZ + 1; z++)
                    {
                        for (int y = this.minY - 1; y <= this.maxY + 1; y++)
                        {
                            for (int x = this.minX - 1; x <= this.maxX + 1; x++)
                            {
                                var currentPosition = Tuple.Create(x, y, z, w);
                                bool currentCell = this.liveCells.Contains(currentPosition);
                                int neighbors = this.CountLiveNeighbors(this.liveCells, x, y, z, w);
                                if (neighbors == 3 || (neighbors == 2 && currentCell))
                                {
                                    newCells.Add(currentPosition);
                                    newMinX = Math.Min(x, newMinX);
                                    newMinY = Math.Min(y, newMinY);
                                    newMinZ = Math.Min(z, newMinZ);
                                    newMinW = Math.Min(w, newMinW);
                                    newMaxX = Math.Max(x, newMaxX);
                                    newMaxY = Math.Max(y, newMaxY);
                                    newMaxZ = Math.Max(z, newMaxZ);
                                    newMaxW = Math.Max(w, newMaxW);
                                }
                            }
                        }
                    }
                }

                this.liveCells = newCells;
                this.minX = newMinX;
                this.minY = newMinY;
                this.minZ = newMinZ;
                this.minW = newMinW;
                this.maxX = newMaxX;
                this.maxY = newMaxY;
                this.maxZ = newMaxZ;
                this.maxW = newMaxW;
            }

            private int CountLiveNeighbors(HashSet<Tuple<int, int, int, int>> liveCells, int x, int y, int z, int w)
            {
                int result = 0;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int k = -1; k <= 1; k++)
                        {
                            for (int l = -1; l <= 1; l++)
                            {
                                if (i != 0 || j != 0 || k != 0 || l != 0)
                                {
                                    var neighbor = Tuple.Create(x + i, y + j, z + k, w + l);
                                    if (liveCells.Contains(neighbor))
                                    {
                                        result++;
                                    }
                                }
                            }
                        }
                    }
                }

                return result;
            }
        }
    }

    class Day9
    {
        public static void Solve()
        {
            var testInput = new[]
            {
                "35", "20", "15", "25", "47",
                "40", "62", "55", "65", "95",
                "102", "117", "150", "182", "127",
                "219", "299", "277", "309", "576"
            };
            var useTestInput = false;
            var input =
                (useTestInput ? testInput : Helpers.LoadInput("input9.txt"))
                .Select(l => long.Parse(l))
                .ToArray();
            var patternSize = useTestInput ? 5 : 25;
            int part1Index = -1;
            long part1 = 0;
            for (int i = patternSize; i < input.Length; i++)
            {
                if (!HasValue(input, i - patternSize, i - 1, input[i]))
                {
                    part1Index = i; 
                    part1 = input[i];
                    break;
                }
            }

            Console.WriteLine($"Part 1: {part1}");

            long part2 = -1;
            // 11828200 is too low

            // For part 2, the input size is small enough that we can use a naive n^2 approach
            for (int start = 0; start < part1Index; start++)
            {
                long sum = 0;
                long smaller = long.MaxValue;
                long larger = long.MinValue;
                for (int end = start; end < part1Index; end++)
                {
                    sum += input[end];
                    if (input[end] < smaller) smaller = input[end];
                    if (input[end] > larger) larger = input[end];
                    if (sum > part1)
                    {
                        // Not this number
                        break;
                    }

                    if (sum == part1)
                    {
                        part2 = smaller + larger;
                        break;
                    }
                }

                if (part2 > 0) break;
            }

            Console.WriteLine($"Part 2: {part2}");
        }

        private static bool HasValue(long[] array, int lowIndex, int highIndex, long value)
        {
            long[] searched = new long[highIndex - lowIndex + 1];
            Array.Copy(array, lowIndex, searched, 0, searched.Length);
            Array.Sort(searched);
            int low = 0, high = searched.Length - 1;
            while (low < high)
            {
                long candidate = searched[low] + searched[high];
                if (candidate == value) return true;
                if (candidate < value)
                {
                    low++;
                }
                else
                {
                    high--;
                }
            }

            return false;
        }
    }

    class Day16
    {
        public static void Solve()
        {
            var testInput1 = new[]
            {
                "class: 1-3 or 5-7",
                "row: 6-11 or 33-44",
                "seat: 13-40 or 45-50",
                "",
                "your ticket:",
                "7,1,14",
                "",
                "nearby tickets:",
                "7,3,47",
                "40,4,50",
                "55,2,20",
                "38,6,12"
            };
            var useTestInput = false;
            var input = useTestInput ? testInput1 : Helpers.LoadInput("input16.txt");
            ParseInput(input, out List<Rule> rules, out int[] myTicket, out List<int[]> otherTickets);

            int part1 = 0;
            List<int> invalidTicketIndices = new List<int>();
            for (var i = 0; i < otherTickets.Count; i++)
            {
                var other = otherTickets[i];
                var isValid = true;
                foreach (var field in other)
                {
                    if (!rules.Any(r => r.IsValid(field)))
                    {
                        part1 += field;
                        isValid = false;
                    }
                }

                if (!isValid)
                {
                    invalidTicketIndices.Add(i);
                }
            }

            Console.WriteLine($"Part 1: {part1}");

            for (var i = invalidTicketIndices.Count - 1; i >= 0; i--)
            {
                otherTickets.RemoveAt(invalidTicketIndices[i]);
            }


            if (useTestInput)
            {
                var testInput2 = new[]
                {
                    "class: 0-1 or 4-19",
                    "row: 0-5 or 8-19",
                    "seat: 0-13 or 16-19",
                    "",
                    "your ticket:",
                    "11,12,13",
                    "",
                    "nearby tickets:",
                    "3,9,18",
                    "15,1,5",
                    "5,14,9"
                };
                ParseInput(testInput2, out rules, out myTicket, out otherTickets);
            }

            List<int>[] ruleToPositionMapping = new List<int>[rules.Count];
            for (var i = 0; i < rules.Count; i++)
            {
                ruleToPositionMapping[i] = new List<int>();
                for (int candidateField = 0; candidateField < rules.Count; candidateField++)
                {
                    bool allValid = true;
                    foreach (var ticket in otherTickets)
                    {
                        if (!rules[i].IsValid(ticket[candidateField]))
                        {
                            allValid = false;
                            break;
                        }
                    }

                    if (allValid)
                    {
                        ruleToPositionMapping[i].Add(candidateField);
                    }
                }
            }

            //for (int i = 0; i < rules.Count; i++)
            //{
            //    Console.WriteLine($"There are {ruleToPositionMapping[i].Count} possibilities for rule {i}");
            //}

            List<Tuple<int, List<int>>> allRuleMappings =
                ruleToPositionMapping.Select((mappings, index) => Tuple.Create(index, mappings))
                .OrderBy(m => m.Item2.Count)
                .ToList();

            Dictionary<int, int> ruleMappings = new Dictionary<int, int>();
            List<int> alreadyUsed = new List<int>();
            var myFields = new Dictionary<string, int>();
            for (int i = 0; i < allRuleMappings.Count; i++)
            {
                var ruleIndex = allRuleMappings[i].Item1;
                var possibleMappings = allRuleMappings[i].Item2.Except(alreadyUsed).ToArray();
                var target = possibleMappings[0];
                ruleMappings.Add(ruleIndex, target);
                var rule = rules[ruleIndex];
                myFields.Add(rule.Name, myTicket[target]);
                // Console.WriteLine($"Rule {rule.Name} ({rule.Ranges}) maps to {target}: {myTicket[target]}");
                alreadyUsed.Add(target);
            }

            long part2 = 1;
            int departureCount = 0;
            foreach (var rule in myFields.Keys)
            {
                if (rule.StartsWith("departure"))
                {
                    departureCount++;
                    part2 *= myFields[rule];
                }
            }

            Console.WriteLine($"Part 2: product={part2}, for {departureCount} rules");
        }

        private static void ParseInput(string[] input, out List<Rule> rules, out int[] myTicket, out List<int[]> otherTickets)
        {
            rules = new List<Rule>();
            int i = 0;
            while (!string.IsNullOrWhiteSpace(input[i]))
            {
                rules.Add(new Rule(input[i]));
                i++;
            }

            i += 2;
            myTicket = input[i].Split(',').Select(p => int.Parse(p)).ToArray();

            i += 3;
            otherTickets = new List<int[]>();
            while (i < input.Length)
            {
                otherTickets.Add(input[i].Split(',').Select(p => int.Parse(p)).ToArray());
                i++;
            }
        }

        class Rule
        {
            private int min1, min2, max1, max2;
            public string Name { get; private set; }
            public string Ranges => $"{min1}-{max1} or {min2}-{max2}";

            private static Regex ruleRegex = new Regex(@"(?<name>[^\:]+)\: (?<min1>\d+)\-(?<max1>\d+) or (?<min2>\d+)\-(?<max2>\d+)");
            public Rule(string ruleSpec)
            {
                var match = ruleRegex.Match(ruleSpec);
                if (!match.Success) throw new Exception();
                this.Name = match.Groups["name"].Value;
                this.min1 = int.Parse(match.Groups["min1"].Value);
                this.max1 = int.Parse(match.Groups["max1"].Value);
                this.min2 = int.Parse(match.Groups["min2"].Value);
                this.max2 = int.Parse(match.Groups["max2"].Value);
            }

            public bool IsValid(int number)
            {
                return (min1 <= number && number <= max1) || (min2 <= number && number <= max2);
            }
        }
    }

    class Day10
    {
        public static void Solve()
        {
            var testInput1 = new[] { "16", "10", "15", "5", "1", "11", "7", "19", "6", "12", "4" };
            var testInput2 = new[] { 
                "28", "33", "18", "42", "31", "14", "46",
                "20", "48", "47", "24", "23", "49", "45",
                "19", "38", "39", "11", "1", "32", "25",
                "35", "8", "17", "7", "9", "4", "2", "34",
                "10", "3"
            };
            var input =
                //testInput1
                //testInput2
                Helpers.LoadInput("input10.txt")
                .Select(l => int.Parse(l))
                .ToArray();

            var sorted = input.OrderBy(v => v).ToArray();
            int ones = 0, twos = 0, threes = 0;
            int last = 0;
            for (int i = 0; i < sorted.Length; i++)
            {
                switch(sorted[i] - last)
                {
                    case 1: ones++; break;
                    case 2: twos++; break;
                    case 3: threes++; break;
                }

                last = sorted[i];
            }
            threes++; // last jolt
            Console.WriteLine($"Part 1: {ones} * {threes} = {ones * threes}");

            List<int> seriesOfOne = new List<int>();
            int currentSeries = 1;
            if (sorted[0] == 1) currentSeries++;
            for (int i = 1; i < sorted.Length; i++)
            {
                if (sorted[i] - sorted[i - 1] == 1)
                {
                    currentSeries++;
                }
                else
                {
                    if (currentSeries > 1)
                    {
                        seriesOfOne.Add(currentSeries);
                    }

                    currentSeries = 1;
                }
            }

            if (currentSeries > 1) seriesOfOne.Add(currentSeries);
            int maxSeries = seriesOfOne.Max();

            int[] possibilities = new int[maxSeries + 1];
            possibilities[0] = 0;
            possibilities[1] = 1;
            possibilities[2] = 1;
            for (int i = 3; i < possibilities.Length; i++)
            {
                possibilities[i] = possibilities[i - 1] + possibilities[i - 2] + possibilities[i - 3];
            }

            long result = 1;
            foreach (var series in seriesOfOne)
            {
                result *= possibilities[series];
            }

            Console.WriteLine($"Part 2: {result}");
        }
    }

    class Day18
    {
        public static void Solve()
        {
            var testInput = new string[]
            {
                "1 + 2 * 3 + 4 * 5 + 6",
                "2 * 3 + (4 * 5)",
                "5 + (8 * 3 + 9 + 3 * 4 * 3)",
                "5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))",
                "((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2"
            };
            var useTestInput = false;
            var input = useTestInput ? testInput : Helpers.LoadInput("input18.txt");

            long part1 = 0;
            IEvaluator evaluator = new EvaluatorWithoutPrecedence();
            foreach (var expr in input)
            {
                var result = evaluator.Evaluate(expr);
                Console.WriteLine($"{expr} = {result}");
                part1 += result;
            }

            Console.WriteLine($"Part 1: {part1}");

            long part2 = 0;
            evaluator = new EvaluatorWithPrecedence();
            foreach (var expr in input)
            {
                var result = evaluator.Evaluate(expr);
                Console.WriteLine($"{expr} = {result}");
                part2 += result;
            }

            Console.WriteLine($"Part 2: {part2}");

        }

        interface IEvaluator
        {
            long Evaluate(string expression);
        }

        abstract class EvaluatorBase: IEvaluator
        {
            protected char? ReadOperatorOrCloseParenOrEof(string expr, ref int index)
            {
                this.SkipSpaces(expr, ref index);
                if (index >= expr.Length) return null;
                // Should be a '+' or a '*'
                switch (expr[index])
                {
                    case '+':
                    case '*':
                        return expr[index++];
                    case ')':
                        return expr[index]; // Do not consume it
                    default:
                        throw new Exception($"Expr[{index}] = '{expr[index]}', expected '+' or '*'");
                }
            }
            protected void SkipSpaces(string expr, ref int index)
            {
                while (index < expr.Length && char.IsWhiteSpace(expr[index])) index++;
            }

            public abstract long Evaluate(string expression);
        }

        class EvaluatorWithoutPrecedence : EvaluatorBase
        {
            public override long Evaluate(string expr)
            {
                int index = 0;
                return this.Evaluate(expr, ref index);
            }

            private long Evaluate(string expr, ref int index)
            {
                long result = this.ReadValue(expr, ref index);
                this.SkipSpaces(expr, ref index);
                while (index < expr.Length)
                {
                    char? op = this.ReadOperatorOrCloseParenOrEof(expr, ref index);
                    if (op == null || op == ')') break;
                    long operand = ReadValue(expr, ref index);
                    if (op == '+')
                    {
                        result += operand;
                    }
                    else
                    {
                        result *= operand;
                    }
                }

                return result;
            }

            private long ReadValue(string expr, ref int index)
            {
                SkipSpaces(expr, ref index);
                if (char.IsDigit(expr[index]))
                {
                    return expr[index++] - '0';
                }

                // Should be a '('
                if (expr[index] != '(') throw new Exception($"Expr[{index}] = '{expr[index]}', expected '('");
                index++;
                var result = Evaluate(expr, ref index);
                SkipSpaces(expr, ref index);
                // Should be a ')'
                if (expr[index] != ')') throw new Exception($"Expr[{index}] = '{expr[index]}', expected ')'");
                index++;
                return result;
            }
        }

        class EvaluatorWithPrecedence : EvaluatorBase
        {
            public override long Evaluate(string expr)
            {
                int index = 0;
                return this.Evaluate(expr, ref index);
            }

            private long Evaluate(string expr, ref int index)
            {
                return this.ReadMultiplication(expr, ref index);
            }

            private long ReadMultiplication(string expr, ref int index)
            {
                long result = this.ReadAddition(expr, ref index);
                while (index < expr.Length)
                {
                    char? op = this.ReadOperatorOrCloseParenOrEof(expr, ref index);
                    if (op != '*') break;
                    long operand = ReadAddition(expr, ref index);
                    result *= operand;
                }

                return result;
            }

            private long ReadAddition(string expr, ref int index)
            {
                long result = this.ReadValue(expr, ref index);
                while (index < expr.Length)
                {
                    char? op = this.ReadOperatorOrCloseParenOrEof(expr, ref index);
                    if (op == '*') index--; // so they can be consumed by the caller
                    if (op != '+') break;
                    long operand = ReadValue(expr, ref index);
                    result += operand;
                }

                return result;
            }

            private long ReadValue(string expr, ref int index)
            {
                SkipSpaces(expr, ref index);
                if (char.IsDigit(expr[index]))
                {
                    return expr[index++] - '0';
                }

                // Should be a '('
                if (expr[index] != '(') throw new Exception($"Expr[{index}] = '{expr[index]}', expected '('");
                index++;
                var result = this.Evaluate(expr, ref index);
                this.SkipSpaces(expr, ref index);
                // Should be a ')'
                if (expr[index] != ')') throw new Exception($"Expr[{index}] = '{expr[index]}', expected ')'");
                index++;
                return result;
            }
        }
    }

    class Day19
    {
        public static void Solve()
        {
            var testInputs = new string[]
            {
                "0: 4 1 5",
                "1: 2 3 | 3 2",
                "2: 4 4 | 5 5",
                "3: 4 5 | 5 4",
                "4: \"a\"",
                "5: \"b\"",
                "",
                "ababbb",
                "bababa",
                "abbbab",
                "aaabbb",
                "aaaabbb",
            };
            var useTestInput = false;
            var input = useTestInput ? testInputs : Helpers.LoadInput("input19.txt");
            bool parsingRules = true;
            var ruleSet = new RuleSet();
            var messages = new List<string>();
            foreach (var line in input)
            {
                if (parsingRules)
                {
                    if (line == "")
                    {
                        parsingRules = false;
                    }
                    else
                    {
                        var rule = Rule.Parse(line);
                        ruleSet.Add(rule);
                    }
                }
                else
                {
                    messages.Add(line);
                }
            }

            string regexExpr = "^" + CreateRegex(ruleSet, 0) + "$";
            var regex = new Regex(regexExpr);
            int part1 = 0;
            foreach (var message in messages)
            {
                Console.WriteLine($"Message {message}: {regex.IsMatch(message)}");
                if (regex.IsMatch(message))
                {
                    part1++;
                }
            }

            Console.WriteLine($"Part 1: {part1}");
        }

        private static string CreateRegex(RuleSet ruleSet, int ruleId)
        {
            var rule = ruleSet.Get(ruleId);
            if (rule.IsLeaf)
            {
                return rule.Letter.ToString();
            }

            if (rule.NextIds.Length == 1)
            {
                return string.Join("", rule.NextIds[0].Select(id => CreateRegex(ruleSet, id)));
            }

            return "(" +
                string.Join(
                    "|",
                    rule.NextIds.Select(
                        ids => "(" + string.Join("", ids.Select(id => CreateRegex(ruleSet, id))) + ")"))
                + ")";
        }

        class RuleSet
        {
            Dictionary<int, Rule> rules = new Dictionary<int, Rule>();
            public void Add(Rule rule)
            {
                this.rules.Add(rule.Id, rule);
            }
            public Rule Get(int id)
            {
                return this.rules[id];
            }
            //public bool IsValid(string message)
            //{
            //    Rule initial = this.rules[0];
            //    int index = 0;
            //}
            //private bool IsValid(Rule rule, string message, int index)
            //{
            //    if (rule.IsLeaf)
            //    {
            //        return index == message.Length - 1 && message[index] == rule.Letter;
            //    }

            //    for
            //}
        }

        class Rule
        {
            public int Id;
            public bool IsLeaf;
            public char Letter;
            public int[][] NextIds;
            public Rule(int id, char letter)
            {
                this.Id = id;
                this.IsLeaf = true;
                this.Letter = letter;
            }
            public Rule(int id, int[][] nextIds)
            {
                this.Id = id;
                this.IsLeaf = false;
                this.NextIds = nextIds;
            }
            public static Rule Parse(string line)
            {
                int colonIndex = line.IndexOf(':');
                int id = int.Parse(line.Substring(0, colonIndex));
                if (line[colonIndex + 2] == '\"')
                {
                    // Letter
                    return new Rule(id, line[colonIndex + 3]);
                }

                var nextIds = line.Substring(colonIndex + 1).Trim()
                    .Split('|')
                    .Select(p => p.Trim().Split(' ').Select(n => int.Parse(n)).ToArray())
                    .ToArray();
                return new Rule(id, nextIds);
            }
        }
    }

    class Day14
    {
        public static void Solve()
        {
            var testInput = new[]
            {
                "mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X",
                "mem[8] = 11",
                "mem[7] = 101",
                "mem[8] = 0"
            };
            var useTestInput = false;
            var input = useTestInput ? testInput : Helpers.LoadInput("input14.txt");

            var memory = new Dictionary<int, long>();
            var maskRegex = new Regex(@"^mask \= (?<mask>[01X]{36})$");
            var memSetRegex = new Regex(@"^mem\[(?<address>\d+)\] = (?<value>\d+)");
            string currentMask = "";
            for (int i = 0; i < input.Length; i++)
            {
                var maskMatch = maskRegex.Match(input[i]);
                if (maskMatch.Success)
                {
                    currentMask = maskMatch.Groups["mask"].Value;
                }
                else
                {
                    var memSetMatch = memSetRegex.Match(input[i]);
                    if (!memSetMatch.Success) throw new ArgumentException("Invalid line: " + input[i]);
                    int address = int.Parse(memSetMatch.Groups["address"].Value);
                    long unmaskedValue = long.Parse(memSetMatch.Groups["value"].Value);
                    long value = 0;
                    long multiplier = 1;
                    for (int b = 0; b < 36; b++)
                    {
                        var bitSet = currentMask[35 - b] == '1' || (currentMask[35 - b] == 'X' && (unmaskedValue & 1) == 1);
                        if (bitSet) value += multiplier;
                        unmaskedValue /= 2;
                        multiplier *= 2;
                    }

                    memory[address] = value;
                }
            }

            var part1 = memory.Values.Sum();
            Console.WriteLine($"Part 1: {part1}");

            var testInput2 = new[]
            {
                "mask = 000000000000000000000000000000X1001X",
                "mem[42] = 100",
                "mask = 00000000000000000000000000000000X0XX",
                "mem[26] = 1"
            };
            if (useTestInput)
            {
                input = testInput2;
            }

            var memory2 = new Dictionary<long, long>();
            for (int i = 0; i < input.Length; i++)
            {
                var maskMatch = maskRegex.Match(input[i]);
                if (maskMatch.Success)
                {
                    currentMask = maskMatch.Groups["mask"].Value;
                }
                else
                {
                    var memSetMatch = memSetRegex.Match(input[i]);
                    if (!memSetMatch.Success) throw new ArgumentException("Invalid line: " + input[i]);
                    int unmaskedAddress = int.Parse(memSetMatch.Groups["address"].Value);
                    long value = long.Parse(memSetMatch.Groups["value"].Value);
                    var floatingBits = new List<long>();
                    long multiplier = 1;
                    for (int b = 0; b < 36; b++)
                    {
                        if (currentMask[35 - b] == 'X')
                        {
                            floatingBits.Add(multiplier);
                        }

                        multiplier *= 2;
                    }

                    long address = 0;
                    multiplier = 1;
                    long baseMemoryAddress = 0;
                    for (int b = 0; b < 36; b++)
                    {
                        var bitSet = currentMask[35 - b] == '1' || (currentMask[35 - b] == '0' && (unmaskedAddress & 1) == 1);
                        if (bitSet) baseMemoryAddress += multiplier;
                        unmaskedAddress /= 2;
                        multiplier *= 2;
                    }

                    foreach (var floatingValue in GetFloatingValues(floatingBits))
                    {
                        memory2[floatingValue + baseMemoryAddress] = value;
                    }
                }
            }

            var part2 = memory2.Values.Sum();
            Console.WriteLine($"Part 2: {part2}");
        }

        static IEnumerable<long> GetFloatingValues(List<long> floatingBits)
        {
            int[] bits = new int[floatingBits.Count];
            bool end = false;
            while (!end)
            {
                long result = 0;
                int i;
                for (i = 0; i < bits.Length; i++)
                {
                    if (bits[i] == 1) result += floatingBits[i];
                    yield return result;
                }

                // Move to next

                for (i = bits.Length - 1; i >= 0; i--)
                {
                    bits[i]++;
                    if (bits[i] == 1) break;
                    bits[i] = 0;
                    end = i == 0;
                }
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
