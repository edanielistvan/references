#include "textureCoords.h"

textureCoords::textureCoords(GLuint tL, GLuint tR, GLuint bL, GLuint bR) : topLeft(tL), topRight(tR), bottomLeft(bL), bottomRight(bR) {}

GLuint textureCoords::getTopLeft() const {
	return topLeft;
}

GLuint textureCoords::getTopRight() const {
	return topRight;
}

GLuint textureCoords::getBotLeft() const {
	return bottomLeft;
}

GLuint textureCoords::getBotRight() const {
	return bottomRight;
}