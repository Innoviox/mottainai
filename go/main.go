package main

import (
	"fmt"
)

var materials = NewMaterials()
var deck = NewDeck()

type Museum struct {
	sales      []Card
	helpers    []Card
	gift_shop  []Card
	gallery    []Card
	work_bench []Card

	task Card
}

func main() {
	for idx, el := range deck.cards {
		fmt.Println(idx, el.name)
	}
}
