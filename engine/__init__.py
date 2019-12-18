from enum   import Enum
from random import shuffle

cards = {i[0]: i[2] for i in map(lambda i:i.split("::"), open("cards.txt").readlines())}

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

    def trigger_works(self, event: Events):
        ...

    def discard_down(self):
        ...

    def pick_task(self):
        ...

class Deck:
    def __init__(self):
        self._deck = list(cards) # note: deck is backwards - "top" of deck is last element of _deck
        shuffle(self._deck)

    def take(self, n):
        return [self._deck.pop() for i in n]

    def ret(self, card):
        self._deck.append(card)

class Game:
    def __init__(self):
        self.players = [Player(self) for _ in range(3)]
        self.current = None
        self._curr_player = 0
        self.deck = Deck()
        self.floor = []

    def start_game(self):
        for p in self.players:
            p.hand = self.deck.take(5)
            p.task = self.deck.take(1)
            self.floor.append(self.deck.take(1))

    def do_turn(self):
        self.current_player.discard_down()
        self.current_player.pick_task()
        self.current_player.trigger_works(Events.MORNING)
        

    @property
    def current_player(self) -> Player:
        return self.players[self._curr_player]



