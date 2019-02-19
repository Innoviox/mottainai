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

types = Types.__members__
points = {t: round(0.5 * types[t].value + 0.7) for t in types}
colors = {Types.PAPER: 'magenta',
          Types.STONE: 'green',
          Types.CLOTH: 'cyan',
          Types.METAL: 'blue',
          Types.CLAY: 'yellow'}
ons = {Types.PAPER: 'on_white',
       Types.STONE: 'on_blue',
       Types.CLOTH: 'on_magenta',
       Types.METAL: 'on_white',
       Types.CLAY: 'on_blue'}
workers = {Types.PAPER: 'clerk',
          Types.STONE: 'monk',
          Types.CLOTH: 'tailor',
          Types.METAL: 'smith',
          Types.CLAY: 'potter'}

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
                    s += tc.colored(workers[h.typ][i], colors[h.typ], ons[h.typ])
                s += " "

            s += " |" + " " * (box_width - 2) + "  | "

            for a in self.sales:
                if i >= len(a):
                    s += " "
                else:
                    s += tc.colored(a.name[i], colors[a.typ], ons[a.typ])
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
        
 #       s = s.upper()
        return s
        


class Human(Player):
    def take_turn(self):
        if len(self.hand) > 5:
            # discard down
            ...
        self.game.floor.append(self.task) # TODO: check chopsticks
        
        for work in self.gallery:
            work.morning("gallery")
        for work in self.gift_shop:
            work.morning("gift_shop")        
        

class Game:
    def __init__(self, n_players=3):
        self.deck = list(map(Card.of, open("cards.txt").readlines()))
        self.floor = []
        self.players = [Player(self) for _ in range(n_players)]
        self.current = 0

    def start_game(self):
        random.shuffle(self.deck)
        
        for p in self.players:
            p.hand = self.deck[1:5]
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

g.players[0].helpers.append(g.deck.pop())
g.players[0].sales.append(g.deck.pop())
g.players[0].craft_bench.append(g.deck.pop())
g.players[0].gift_shop.append(g.deck.pop())
g.players[0].gallery.append(g.deck.pop())
g.players[0].gallery.append(g.deck.pop())


print(g.players[0])
