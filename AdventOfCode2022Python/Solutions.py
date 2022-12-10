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

def day6_first_unique_n_characters(input: str, n: int) -> int:
    dict = {}
    result = 0
    for i in range(0,n):
        ch = input[i]
        cnt = dict.get(ch)
        if cnt == None:
            dict[ch] = 1
        else:
            dict[ch] = cnt + 1
    if len(dict) == n:
        result = n
    else:
        for i in range(n, len(input)):
            prev = input[i - n]
            next = input[i]
            cnt = dict[prev]
            if cnt == 1:
                dict.pop(prev)
            else:
                dict[prev] -= 1
            cnt = dict.get(next)
            if cnt == None:
                dict[next] = 1
            else:
                dict[next] = cnt + 1
            if len(dict) == n:
                result = i + 1 # index is 0-based, answer is 1-based
                break
    return result

def day6():
    lines = read_input("input6.txt")
    #lines = ["mjqjpqmgbljsphdztnvjfqwrcgsmlb"]
    line = lines[0]

    day6_part1 = day6_first_unique_n_characters(line, 4)
    print("Day 6, part 1: ", day6_part1)

    day6_part2 = day6_first_unique_n_characters(line, 14)
    print("Day 6, part 2: ", day6_part2)

def day8():
    lines = read_input("input8.txt")
    #lines = ["30373", "25512", "65332", "33549", "35390"]
    visible_trees: list[list[int]] = []
    rows = len(lines)
    cols = len(lines[0].strip())
    for row in range(rows):
        visible_trees.append([])
        for col in range(cols):
            if row == 0 or row == rows - 1 or col == 0 or col == cols - 1:
                visible_trees[row].append(int(lines[row][col]))
            else:
                visible_trees[row].append(-1)

    for row in range(1, rows - 1):
        max_left = int(lines[row][0])
        for col in range(1, cols - 1):
            curr_tree = int(lines[row][col])
            if max_left < curr_tree:
                max_left = curr_tree
                visible_trees[row][col] = curr_tree
        max_right = int(lines[row][cols - 1])
        for col in range(cols - 2, 0, -1):
            curr_tree = int(lines[row][col])
            if max_right < curr_tree:
                visible_trees[row][col] = curr_tree
                max_right = curr_tree
    for col in range(1, cols - 1):
        max_top = int(lines[0][col])
        for row in range(1, rows - 1):
            curr_tree = int(lines[row][col])
            if max_top < curr_tree:
                visible_trees[row][col] = curr_tree
                max_top = curr_tree
        max_bottom = int(lines[rows - 1][col])
        for row in range(rows - 2, 0, -1):
            curr_tree = int(lines[row][col])
            if max_bottom < curr_tree:
                visible_trees[row][col] = curr_tree
                max_bottom = curr_tree

    day8_part1 = 0
    for row in range(rows):
        for col in range(cols):
            if visible_trees[row][col] >= 0:
                day8_part1 += 1

    print("Day 8, part 1: ", day8_part1)

    viewing_distances_left: list[list[int]] = []
    viewing_distances_right: list[list[int]] = []
    viewing_distances_top: list[list[int]] = []
    viewing_distances_bottom: list[list[int]] = []

    for row in range(rows):
        viewing_distances_left.append([])
        viewing_distances_right.append([])
        viewing_distances_top.append([])
        viewing_distances_bottom.append([])
        for col in range(cols):
            viewing_distances_left[row].append(0)
            viewing_distances_right[row].append(0)
            viewing_distances_top[row].append(0)
            viewing_distances_bottom[row].append(0)
            visible_trees[row][col] = int(lines[row][col])

    for row in range(rows):
        for col in range(1, cols - 1):
            right_col = cols - col - 1
            c = col - 1
            visible_trees_to_left = 0
            while True:
                visible_trees_to_left += 1
                if c == 0 or visible_trees[row][c] >= visible_trees[row][col]:
                    break
                c -= 1
            viewing_distances_left[row][col] = visible_trees_to_left

            c = right_col + 1
            visible_trees_to_right = 0
            while True:
                visible_trees_to_right += 1
                if c == cols - 1 or visible_trees[row][c] >= visible_trees[row][right_col]:
                    break
                c += 1
            viewing_distances_right[row][right_col] = visible_trees_to_right

    for col in range(cols):
        for row in range(1, rows - 1):
            bottom_row = rows - row - 1
            r = row - 1
            visible_trees_to_top = 0
            while True:
                visible_trees_to_top += 1
                if r == 0 or visible_trees[r][col] >= visible_trees[row][col]:
                    break
                r -= 1
            viewing_distances_top[row][col] = visible_trees_to_top

            r = bottom_row + 1
            visible_trees_to_bottom = 0
            while True:
                visible_trees_to_bottom += 1
                if r == rows - 1 or visible_trees[r][col] >= visible_trees[bottom_row][col]:
                    break
                r += 1
            viewing_distances_bottom[bottom_row][col] = visible_trees_to_bottom

    day8_part2 = 0
    for row in range(1, rows - 1):
        for col in range(1, cols - 1):
            viewing_distance = viewing_distances_left[row][col] * viewing_distances_right[row][col] * viewing_distances_top[row][col] * viewing_distances_bottom[row][col]
            if (day8_part2 < viewing_distance):
                day8_part2 = viewing_distance

    print("Day 8, part 2: ", day8_part2)

day8()
