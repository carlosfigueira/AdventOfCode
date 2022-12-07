using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            //Day1.Solve();
            Day7.Solve();
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

        class Day7
        {
            [DebuggerDisplay("Folder {name}, {subFolders.Count} folders, {files.Count} files")]
            class Folder
            {
                private string name;
                private Dictionary<string, Folder> subFolders;
                private Dictionary<string, long> files;
                private long cachedTotalSize = -1;
                public Folder(string name)
                {
                    this.name = name;
                    this.subFolders = new Dictionary<string, Folder>();
                    this.files = new Dictionary<string, long>();
                }

                public void AddFile(string name, long size)
                {
                    this.files[name] = size;
                }

                public Folder AddFolder(string name)
                {
                    if (!this.subFolders.ContainsKey(name))
                    {
                        this.subFolders.Add(name, new Folder(name));
                    }

                    return this.subFolders[name];
                }

                public IEnumerable<Folder> SubFolders => this.subFolders.Values;

                public long GetTotalSize()
                {
                    if (this.cachedTotalSize < 0)
                    {
                        this.cachedTotalSize = 0;
                        foreach (var file in this.files)
                        {
                            this.cachedTotalSize += file.Value;
                        }

                        foreach (var subFolder in this.subFolders)
                        {
                            this.cachedTotalSize += subFolder.Value.GetTotalSize();
                        }
                    }

                    return this.cachedTotalSize;
                }

                public void Print(string indent = "")
                {
                    Console.WriteLine($"{indent}{this.name}/ ({this.GetTotalSize()} total bytes)");
                    indent += "    ";
                    foreach (var file in this.files)
                    {
                        Console.WriteLine($"{indent}{file.Key} - {file.Value} bytes");
                    }

                    foreach (var folder in this.subFolders)
                    {
                        folder.Value.Print(indent);
                    }
                }
            }

            static Folder ReadFolders(string[] lines)
            {
                Debug.Assert(lines[0] == "$ cd /");
                var rootFolder = new Folder("/");
                var folderStack = new Stack<Folder>();
                var currentFolder = rootFolder;
                var listingFiles = false;
                foreach (var line in lines.Skip(1))
                {
                    if (line.StartsWith("$"))
                    {
                        listingFiles = false;
                    }

                    if (line == "$ ls")
                    {
                        listingFiles = true;
                        continue;
                    }

                    if (line == "$ cd /")
                    {
                        folderStack.Clear();
                        currentFolder = rootFolder;
                        continue;
                    }

                    if (line == "$ cd ..")
                    {
                        currentFolder = folderStack.Pop();
                        continue;
                    }

                    if (line.StartsWith("$ cd "))
                    {
                        var folderName = line.Substring("$ cd ".Length);
                        folderStack.Push(currentFolder);
                        currentFolder = currentFolder.AddFolder(folderName);
                        continue;
                    }

                    if (listingFiles)
                    {
                        if (line.StartsWith("dir "))
                        {
                            currentFolder.AddFolder(line.Substring("dir ".Length));
                        }
                        else
                        {
                            var parts = line.Split(' ');
                            currentFolder.AddFile(parts[1], long.Parse(parts[0]));
                        }
                    }
                }

                return rootFolder;
            }

            static long SumSizesAtMost(Folder folder, long limit)
            {
                long result = 0;
                foreach (var subFolder in folder.SubFolders)
                {
                    result += SumSizesAtMost(subFolder, limit);
                }

                var folderTotalSize = folder.GetTotalSize();
                if (folderTotalSize <= limit)
                {
                    result += folderTotalSize;
                }

                return result;
            }

            public static void Solve()
            {
                var lines = Helpers.LoadInput("input7.txt");
                //lines = new[] { "$ cd /", "$ ls", "dir a", "14848514 b.txt", "8504156 c.dat", "dir d", "$ cd a", "$ ls", "dir e", "29116 f", "2557 g", "62596 h.lst", "$ cd e", "$ ls", "584 i", "$ cd ..", "$ cd ..", "$ cd d", "$ ls", "4060174 j", "8033020 d.log", "5626152 d.ext", "7214296 k" };
                var rootFolder = ReadFolders(lines);

                //rootFolder.Print();

                var totalSize = rootFolder.GetTotalSize(); // Calculate all sizes
                Console.WriteLine($"Day 7, part 1: {SumSizesAtMost(rootFolder, 100000)}");

                var currentFreeSpace = 70000000 - totalSize;
                var needToDelete = 30000000 - currentFreeSpace;
                Console.WriteLine($"Day 7, part 2: {FindSmallestFolderGreaterThan(rootFolder, needToDelete)}");
            }

            private static long FindSmallestFolderGreaterThan(Folder folder, long needToDelete)
            {
                long result = -1;
                foreach (var subFolder in folder.SubFolders)
                {
                    var subFolderTotalSize = FindSmallestFolderGreaterThan(subFolder, needToDelete);
                    if (subFolderTotalSize >= needToDelete)
                    {
                        if (result < 0 || result > subFolderTotalSize)
                        {
                            result = subFolderTotalSize;
                        }
                    }
                }

                var folderTotalSize = folder.GetTotalSize();
                if (result == -1 && folderTotalSize >= needToDelete)
                {
                    result = folderTotalSize;
                }

                return result;
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
