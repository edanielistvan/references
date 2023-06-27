#ifndef BLOCK_H_
#define BLOCK_H_

#include "textureCoords.h"
#include <vector>

class Block
{
	private:
		std::vector<textureCoords*> textureIDs; //top, bottom, left, right, front, back
		int id;

		void initVector();
	public:
		enum TextureSide {TOP, BOTTOM, LEFT, RIGHT, FRONT, BACK};

		Block(int id); //levegohoz
		Block(int id, int ids[4]); //minden egyforma
		Block(int id, int idsTop[4], int idsSide[4]); //csak a teteje mas
		Block(int id, int idsTop[4], int idsSide[4], int idsBot[4]); //teteje/oldal/alja
		~Block();

		textureCoords* getTextureID(TextureSide tex) const;
		int getID() const;
};
#endif