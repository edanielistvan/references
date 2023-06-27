#include "Block.h"

void Block::initVector() {
	textureIDs.resize(6);
}

Block::Block(int id) {
	this->id = id;
}

Block::Block(int id, int ids[4]) {
	initVector();
	this->id = id;

	for (int i = 0; i < textureIDs.size(); i++) {
		textureIDs[i] = new textureCoords(ids[0], ids[1], ids[2], ids[3]);
	}
}

Block::Block(int id, int idsTop[4], int idsSide[4]) {
	initVector();
	this->id = id;
	textureIDs[0] = new textureCoords(idsTop[0], idsTop[1], idsTop[2], idsTop[3]);

	for (int i = 1; i < textureIDs.size(); i++) {
		textureIDs[i] = new textureCoords(idsSide[0], idsSide[1], idsSide[2], idsSide[3]);
	}
}

Block::Block(int id, int idsTop[4], int idsSide[4], int idsBot[4]) {
	initVector();
	this->id = id;
	textureIDs[0] = new textureCoords(idsTop[0], idsTop[1], idsTop[2], idsTop[3]);
	textureIDs[1] = new textureCoords(idsBot[0], idsBot[1], idsBot[2], idsBot[3]);

	for (int i = 1; i < textureIDs.size(); i++) {
		textureIDs[i] = new textureCoords(idsSide[0], idsSide[1], idsSide[2], idsSide[3]);
	}
}

Block::~Block() {
	for (auto p : textureIDs) {
		delete p;
	}
}

textureCoords* Block::getTextureID(Block::TextureSide tex) const {
	return textureIDs[tex];
}

int Block::getID() const {
	return id;
}