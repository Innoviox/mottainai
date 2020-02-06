package main

import (
	"bufio"
	"log"
	"math/rand"
	"os"
	"strings"
	"time"
)

type Card struct {
	mat  Material
	name string
	text string
}

func NewCard(line string) Card {
	x := strings.Split(line, "::")

	return Card{materials.NewMaterial(x[1]), x[0], x[2]}
}

type Deck struct {
	cards [54]Card
}

func NewDeck() Deck {
	var cards [54]Card

	file, err := os.Open("cards.txt")
	if err != nil {
		log.Fatal(err)
	}

	defer file.Close()

	scanner := bufio.NewScanner(file)
	i := 0
	for scanner.Scan() {
		cards[i] = NewCard(scanner.Text())
		i++
	}

	if err := scanner.Err(); err != nil {
		log.Fatal(err)
	}

	// fisher-yates shuffle
	rand.Seed(time.Now().UnixNano())
	rand.Shuffle(len(cards), func(i, j int) {
		cards[i], cards[j] = cards[j], cards[i]
	})

	return Deck{cards}
}
