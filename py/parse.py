import spacy
from spacy import displacy

nlp = spacy.load('en_core_web_sm')

text = {i[0]: i[2] for i in map(lambda i:i.split("::"), open("cards.txt").readlines())}
M = 'modifiers'

def find_arc(lst, *arcs, n=0):
    def _find_arc(l_, arc):
        def __find_arc(a):
            l = [i for i in l_ if i['arc']==a]
            if l:
                return l[n]
            return None
        if isinstance(arc, list):
            for i in arc:
                a = __find_arc(i)
                if a is not None:
                    return a
        return __find_arc(arc)
    a = lst
    for arc in arcs[:-1]:
        a = _find_arc(a, arc)[M]
    a = _find_arc(a, arcs[-1])
    return a

def parse(card):
    """
    There are several types of cards.
    1) Triggered.
       a) Direct. see PIN
       b) Conditional. see PLANE
    2) Global.
       a) Aggressive. see TOWER
       b) Passive. see BENCH
       c) Effective. see LAMPSHADE
    3) Replacement.
       a) Pure. see DOLL
       b) Augmentative. see CRANE
       (The difference between these may seem slight.
        Pure effects give the player a new option;
        Augmentative effects make the current option better.)
    Note: Any card may have a second part, the causative: "If you do..."
    """
    target = nlp(text[card])
    pt = target.print_tree()
    m = pt[0][M]
    start = find_arc(m, 'prep')
    trigger = find_arc(start[M], 'pobj')
    if trigger:
        event = find_arc(trigger[M], ['compound', 'amod', 'nmod'])
        cond = find_arc(trigger[M], 'mark')
        if cond: # Conditional
            check = find_arc(trigger[M], 'advcl')
        action = find_arc(m, 'prep', n=1)
    else:
        ...
    return action, start, trigger, event
    # event = find_arc_deep(m, 'prep', 'pobj', ['compound', 'amod', 'nmod'])
    # return event

def _parse_piece(piece):
    n = nlp(piece).print_tree()[0][M]
    if find_arc(n, 'pobj'): # event?
        ...
    

def parse(card):
    target = text[card].split(".")
    for sentence in target:
        pieces = sentence.split(",")
        parsed = list(map(_parse_piece, pieces))

def get_rendered_svg(phrase):
    open("test.svg", "w").write(displacy.render(nlp(phrase), options={'compact':True, 'page':True}))
