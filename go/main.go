package main

import (
	"fmt"
)

type Material int

const (
	PAPER Material = 1
	CLOTH Material = 2
	STONE Material = 2
	METAL Material = 3
	CLAY  Material = 3
)

type Card struct {
	mat  Material
	name string
}

func main() {
	c := Card{PAPER, "Doll"}
	fmt.Println(c)
}
