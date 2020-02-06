package main

import (
	"fmt"
)

var mat = NewMaterials()

type Card struct {
	mat  Material
	name string
}

func main() {
	c := Card{mat.PAPER, "Doll"}
	fmt.Println(c)
}
