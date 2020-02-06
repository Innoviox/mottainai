package main

import (
	"strings"
)

type Material struct {
	String string
	value  int
}

type Materials struct {
	PAPER Material
	CLOTH Material
	STONE Material
	METAL Material
	CLAY  Material
}

func NewMaterials() Materials {
	p := Material{"Paper", 1}
	c := Material{"Cloth", 2}
	s := Material{"Stone", 2}
	m := Material{"Metal", 3}
	l := Material{"Clay", 3}
	return Materials{p, c, s, m, l}
}

func (m Materials) NewMaterial(s string) Material {
	switch strings.ToLower(s) {
	case "paper":
		return m.PAPER
	case "cloth":
		return m.CLOTH
	case "stone":
		return m.STONE
	case "metal":
		return m.METAL
	default: // case "clay"
		return m.CLAY
	}
}
