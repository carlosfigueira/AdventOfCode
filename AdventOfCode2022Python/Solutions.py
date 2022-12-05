import re
from copy import deepcopy
from functools import reduce

def read_input(fileName: str):
    theFile = open(fileName, "r")
    return theFile.readlines()

def day2():
    lines = read_input("input2.txt")
    total_score_part_1 = 0
    hand_scores = {"R":1, "P":2, "S":3}
    game_scores = {
        ("R","R"):3, ("R","P"):0, ("R","S"):6,
        ("P","R"):6, ("P","P"):3, ("P","S"):0,
        ("S","R"):0, ("S","P"):6, ("S","S"):3
    }
    opponent_code = {"A":"R", "B":"P", "C":"S"}
    my_code_part_1 = {"X":"R", "Y":"P", "Z":"S"}
    for line in lines:
        parts = line.split()
        opponent_play = opponent_code[parts[0]]
        my_play = my_code_part_1[parts[1]]
        total_score_part_1 += hand_scores[my_play] + game_scores[(my_play, opponent_play)]

    print("Day 2, part 1: ", total_score_part_1)

    total_score_part_2 = 0
    my_code_part_2 = {
        ("X","R"):"S", ("X","P"):"R", ("X","S"):"P", # I lose
        ("Y","R"):"R", ("Y","P"):"P", ("Y","S"):"S", # draw
        ("Z","R"):"P", ("Z","P"):"S", ("Z","S"):"R", # I win
    }

    for line in lines:
        parts = line.split()
        opponent_play = opponent_code[parts[0]]
        my_play = my_code_part_2[(parts[1], opponent_play)]
        total_score_part_2 += hand_scores[my_play] + game_scores[(my_play, opponent_play)]

    print("Day 2, part 2: ", total_score_part_2)

def day3_get_letter_priority(letter):
    if 'a' <= letter and letter <= 'z':
        return ord(letter) - ord('a') + 1
    return ord(letter) - ord('A') + 1 + 26

def day3_string_to_set(word: str):
    result = set()
    for letter in word:
        result.add(letter)
    return result

def day3():
    lines = read_input("input3.txt")
    #lines = ["vJrwpWtwJgWrhcsFMMfFFhFp","jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL","PmmdzqPrVvPwwTWBwg","wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn","ttgJtRGJQctTZtZT","CrZsJsPPZsGzwwsLwLmpwMDw"]
    sum_priorities = 0
    for line in lines:
        line_len = len(line)
        mid_line = int(line_len / 2)
        part1 = line[0:mid_line];
        part2 = line[mid_line:line_len]
        set1 = day3_string_to_set(part1)
        set2 = day3_string_to_set(part2)
        for letter in set1.intersection(set2):
            sum_priorities += day3_get_letter_priority(letter)

    print("Day 3, part 1: ", sum_priorities)

    sum_priorities = 0
    for i in range(0, len(lines), 3):
        group = day3_string_to_set(lines[i].strip()).intersection(day3_string_to_set(lines[i+1].strip())).intersection(day3_string_to_set(lines[i+2].strip()))
        for letter in group:
            sum_priorities += day3_get_letter_priority(letter)

    print("Day 3, part 2: ", sum_priorities)

def day4_between(number: int, min: int, max: int) -> bool:
    return min <= number and number <= max

def day4():
    lines = read_input("input4.txt")
    #lines = ["2-4,6-8","2-3,4-5","5-7,7-9","2-8,3-7","6-6,4-6","2-6,4-8"]
    fully_contained = 0
    overlaps = 0
    for line in lines:
        pair = line.split(",")
        min1 = int(pair[0].split('-')[0]);
        max1 = int(pair[0].split('-')[1]);
        min2 = int(pair[1].split('-')[0]);
        max2 = int(pair[1].split('-')[1]);
        if (min1 <= min2 and max2 <= max1) or (min2 <= min1 and max1 <= max2):
            fully_contained += 1
        if day4_between(min1, min2, max2) or day4_between(max1, min2, max2) or day4_between(min2, min1, max1) or day4_between(max2, min1, max1):
            overlaps += 1

    print("Day 4, part 1: ", fully_contained)
    print("Day 4, part 2: ", overlaps)

def day5():
    lines = read_input("input5.txt")
    #lines = ["    [D]    ","[N] [C]    ","[Z] [M] [P]"," 1   2   3 ","","move 1 from 2 to 1","move 3 from 1 to 3","move 2 from 2 to 1","move 1 from 1 to 2"]

    crates = []
    initialization = True
    num_crates = 0
    moves = []
    for i in range(10):
        crates.append([])

    for line in lines:
        if line.strip() == "":
            initialization = False
            initial_state = deepcopy(crates)
            continue
        if initialization:
            crate_number = 0
            num_crates = max(num_crates, int((len(line) + 1) / 4))
            for i in range(1, len(line), 4):
                crate_number += 1
                if ord(line[i]) >= ord('A'):
                    crates[crate_number].insert(0, line[i])
        else:
            moves.append(line)

    for line in moves:
        match_obj = re.match(r'move (\d+) from (\d) to (\d)', line)
        crates_to_move = int(match_obj.group(1))
        move_from = int(match_obj.group(2))
        move_to = int(match_obj.group(3))
        for i in range(crates_to_move):
            crates[move_to].append(crates[move_from].pop())

    part1_answer = ""
    for crate in crates:
        if len(crate) > 0:
            part1_answer += crate.pop()
    print("Day 5, part 1: ", part1_answer)

    # Restoring initial state
    crates = deepcopy(initial_state)
    for line in moves:
        match_obj = re.match(r'move (\d+) from (\d) to (\d)', line)
        crates_to_move = int(match_obj.group(1))
        move_from = int(match_obj.group(2))
        move_to = int(match_obj.group(3))
        temp = []
        for i in range(crates_to_move):
            temp.append(crates[move_from].pop())
        for i in range(crates_to_move):
            crates[move_to].append(temp.pop())

    part2_answer = reduce(lambda a, b: a + b.pop(), crates[1:num_crates + 1], "")
    print("Day 5, part 2: ", part2_answer)
day5()
