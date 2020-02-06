package main

import (
	"fmt"
)

var mat = NewMaterials()

type Card struct {
	mat  Material
	name string
	text string
}

type Museum struct {
	sales      []Card
	helpers    []Card
	gift_shop  []Card
	gallery    []Card
	work_bench []Card

	task Card
}

func main() {
	c := Card{mat.PAPER, "Doll"}
	fmt.Println(c)
}
