import random
import termcolor as tc
import textwrap as tw

from enum     import Enum

import os
os.system('color')
"""
Parsing notes
~: any of the cards from a list
.: the current card
?: may
>: ":" on mtg
T(): transfer
r: return
"""

MAX_ABILITY_LEN = 30

class Types(Enum):
    PAPER = 1
    STONE = 2
    CLOTH = 3
    CLAY = 4
    METAL = 5

class Events(Enum):
    MORNING = 0
    NOON = 1
    NIGHT = 2

    CLERK = 3
    MONK = 4
    TAILOR = 5
    POTTER = 6
    SMITH = 7

    PRAYER = 8
    CRAFT = 9
types = Types.__members__
points = {t: round(0.5 * types[t].value + 0.7) for t in types}
colors = {Types.PAPER: 'magenta',
          Types.STONE: 'green',
          Types.CLOTH: 'cyan',
          Types.METAL: 'blue',
          Types.CLAY: 'yellow'}
ons = {Types.PAPER: 'on_blue',
       Types.STONE: 'on_blue',
       Types.CLOTH: 'on_blue',
       Types.METAL: 'on_white',
       Types.CLAY: 'on_blue'}
workers = {Types.PAPER: 'clerk',
          Types.STONE: 'monk',
          Types.CLOTH: 'tailor',
          Types.METAL: 'smith',
          Types.CLAY: 'potter'}
tasks = {Types.PAPER: Events.CLERK,
         Types.STONE: Events.MONK,
         Types.CLOTH: Events.TAILOR,
         Types.CLAY: Events.POTTER,
         Types.METAL: Events.SMITH}

def str_list(l, f=lambda i:i.name):
    return ", ".join(tc.colored(f(i), colors[i.typ]) for i in l)

class Card:
    def __init__(self, typ, name, ability):
        self.typ = typ
        self.name = name
        self.ability = ability

    def morning(self, side):
        # TODO: morning effects
        ...

    def __len__(self):
        return len(self.name)
    
    @classmethod
    def of(cls, string):
        n, t, a = string.split("::")
        return Card(types[t], n, a)

    def __str__(self):
        s = ""
        s += self.name.ljust(MAX_ABILITY_LEN) + "\n" + "\n".join(i.ljust(MAX_ABILITY_LEN) for i in tw.wrap(self.ability, MAX_ABILITY_LEN))

        return tc.colored(s, colors[self.typ])


class Player:
    def __init__(self, game):
        self.gallery = []
        self.gift_shop = []

        self.sales = []
        self.helpers = []
        self.craft_bench = []

        self.task = None

        self.hand = []

        self.game = game

    def take_turn(self):
        raise NotImplementedError()
    
    def __repr__(self):
        s = ""

        lpad = (len(self.helpers) + 1) * 2
        if self.gallery:
            lpad += (MAX_ABILITY_LEN + 1) * len(self.gallery)
        lpad = " " * lpad
        s += lpad

        if self.task is not None:
            task_name = workers[self.task.typ]
            task_color = colors[self.task.typ]
        else:
            task_name = "None"
            task_color = "black"
        s += tc.colored(task_name, task_color) + "\n"

        box_width = len(task_name)
        if self.craft_bench:
            box_width = max(box_width, max(map(lambda i: len(i.typ.name), self.craft_bench)))
            
        box_height = 1
        if self.helpers:
            box_height = max(box_height, max(map(lambda i: len(workers[i.typ]), self.helpers)))
        if self.sales:
            box_height = max(box_height, max(map(len, self.sales)))
        if self.gift_shop:
            box_height = max(box_height, max(map(lambda i: len(str(i).split("\n")), self.gift_shop)))
        if self.gallery:
            box_height = max(box_height, max(map(lambda i: len(str(i).split("\n")), self.gallery)))

        s += lpad + '-' * box_width + "\n"
        for i in range(box_height):
            for w in self.gallery:
                g = str(w).split("\n")
                if i < len(g):
                    k = g[i]
                    s += tc.colored(k, colors[w.typ]) + " "
                else:
                    s += " " * (MAX_ABILITY_LEN + 1)
                
            for h in self.helpers:
                if i >= len(workers[h.typ]):
                    s += " "
                else:
                    s += tc.colored(workers[h.typ][i], colors[h.typ]) #, ons[h.typ])
                s += " "

            s += " |" + " " * (box_width - 2) + "  | "

            for a in self.sales:
                if i >= len(a):
                    s += " "
                else:
                    s += tc.colored(a.name[i], colors[a.typ]) #, ons[a.typ])
                s += " "

            for w in self.gift_shop:
                g = str(w).split("\n")
                if i < len(g):
                    s += tc.colored(g[i], colors[w.typ])
                else:
                    s += " " * max(map(len, g))

            s += "\n"
        s += lpad + '-' * box_width + "\n"

        for c in self.craft_bench:
            s += lpad + tc.colored(c.typ.name, colors[c.typ]) + "\n"

        s += "\n\n"
        s += ", ".join(tc.colored(i.name, colors[i.typ]) for i in self.hand)
        s += "\n"
        
 #       s = s.upper()
        return s
        


class Human(Player):
    def take_turn(self):
        #### MORNING ####
        if len(self.hand) > 5:
            # discard down
            ...
            
        self.game.floor.append(self.task) # TODO: check chopsticks
        
        self.do_works(Events.MORNING)
        
        print("Your hand is:", str_list(self.hand))
        self.task = self.hand[int(input("Enter new task (0-indexed): "))] # TODO: input by name

        #### NOON ####
        for p in self.game.players:
            self.do_task(workers[p.task.typ]) # TODO: Hidden tasks

    def do_works(self, event):
        print("Processing for:", event.name)
        for l in [self.gift_shop, self.gallery]:
            for work in l:
                n = event.name.lower()
                if event.value > 2:
                    n += " action"
                if n in work.ability.lower():
                    print("Found", work.name)

    def do_task(self, task):
        print("Doing task:", task)
        self.do_works(task)
        for _ in range(1 + self.count_helpers(task)):
            if task == Events.CLERK:
                print("Your craft bench is:", str_list(self.craft_bench, f=lambda i:i.typ.name))
                c = int(input("Enter craft to sell (0-indexed): "))
                self.sales.append(self.craft_bench.pop(c))
            elif task == Events.MONK:
                print("The floor is:", str_list(self.game.floor))
                c = int(input("Enter helper to get (0-indexed): "))
                self.helpers.append(self.game.floor.pop(c))
            elif task == Events.TAILOR:
                print("Your hand is:", str_list(self.hand))
                c = list(map(int, input("Enter indexes of cards you want to cycle: ").split()))
                # TODO: Tailor
            elif task == Events.POTTER:
                print("The floor is:", str_list(self.game.floor, f=lambda i:i.typ.name))
                c = int(input("Enter material to get (0-indexed): "))
                self.craft_bench.append(self.game.floor.pop(c))
            elif task == Events.SMITH:
                ... # TODO: SMITH
            # TODO: Switch for prayer / craft

    def count_helpers(self, task):
        return sum(1 for h in self.helpers if workers[h.typ].upper() == task.name)


class Game:
    def __init__(self, n_players=3):
        self.deck = list(map(Card.of, open("cards.txt").readlines()))
        self.floor = []
        self.players = [Human(self) for _ in range(n_players)]
        self.current = 0

    def start_game(self):
        random.shuffle(self.deck)
        
        for p in self.players:
            p.hand = self.deck[:5]
            self.deck = self.deck[5:]
            
        for p in self.players:
            p.task = self.deck.pop()
            
        first_card = "ZZZZZ"
        for i, p in enumerate(self.players):
            c = self.deck.pop()
            if c.name < first_card:
                self.current = i
                first_card = c.name
        
g = Game()
g.start_game()

print(g.players)
g.players[0].take_turn()
