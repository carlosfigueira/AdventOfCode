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

    print("Part 1: ", total_score_part_1)

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

    print("Part 2: ", total_score_part_2)

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

    print("Part 1: ", sum_priorities)

    sum_priorities = 0
    for i in range(0, len(lines), 3):
        group = day3_string_to_set(lines[i].strip()).intersection(day3_string_to_set(lines[i+1].strip())).intersection(day3_string_to_set(lines[i+2].strip()))
        for letter in group:
            sum_priorities += day3_get_letter_priority(letter)

    print("Part 2: ", sum_priorities)

day3()
