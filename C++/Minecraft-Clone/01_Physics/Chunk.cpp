#include "Chunk.h"

std::vector<Block*> Chunk::blocks;

Chunk::Chunk(int x, int z, std::vector<std::vector<short>>& heightmap) 
{
	chunkCoordinates = { x, z };

	this->heightmap = heightmap;
	generateChunk();

	changed = true;

	numOfIndices = 0;

	neighbors = { nullptr, nullptr, nullptr, nullptr }; //left, right, down, up|| -x, x, -z, z

	chunk_vao.Init({
			{ CreateAttribute<0, GLuint, 0, sizeof(GLuint)>, vertexes },
		},
		indices
	);
}

Chunk::~Chunk() {}

bool Chunk::isAir(int x, int y, int z) const {
	if (y > 256 || y < 1) return true;

	if (x < 1) {
		if (neighbors[0] != nullptr) {
			return neighbors[0]->isAir(16, y, z);
		}
		else {
			return true;
		}

	}

	if (x > 16) {
		if (neighbors[1] != nullptr) {
			return neighbors[1]->isAir(1, y, z);
		}
		else {
			return true;
		}
	}

	if (z < 1) {
		if (neighbors[2] != nullptr) {
			return neighbors[2]->isAir(x, y, 16);
		}
		else {
			return true;
		}

	}

	if (z > 16) {
		if (neighbors[3] != nullptr) {
			return neighbors[3]->isAir(x, y, 1);
		}
		else {
			return true;
		}

	}

	return blockIDs[x][z][y]->getID() <= 0;
}

void Chunk::connectNeighbor(Chunk* ch, const int i) {
	neighbors[i] = ch;
}

GLuint Chunk::calcDatValue(GLuint x, GLuint y, GLuint z, GLuint t, Block::TextureSide side) const {
	GLuint nx = 0, ny = 0, nz = 0;
	GLuint sign = 0;

	switch (side)
	{
		case Block::TOP: ny++; break;
		case Block::BOTTOM: ny++; sign=1; break;
		case Block::LEFT: nx++; sign=1; break;
		case Block::RIGHT: nx++; break;
		case Block::BACK: nz++; break;
		case Block::FRONT: nz++; sign=1; break;
		default: break;
	}

	//if (x - 1 > 15) std::cerr << "bad x: " << x << std::endl;
	if (y > 255) std::cerr << "bad y: " << y << std::endl;
	//if (z - 1 > 15) std::cerr << "bad z: " << z << std::endl;

	GLuint v = sign;
	v *= 2;
	v += nx;
	v *= 2;
	v += ny;
	v *= 2;
	v += nz;
	v *= 32;
	v += x;
	v *= 512;
	v += y;
	v *= 32;
	v += z;
	v *= 512;
	v += t;
	return v;
}

const std::vector<std::vector<short>>* Chunk::getHeightmap() const {
	return &heightmap;
}

void Chunk::loadToDraw() {
	std::vector<GLuint> vert;
	std::vector<GLuint> ind;

	for (GLuint i = 1; i < 17u; i++)
	{
		for (GLuint j = 1; j < 17u; j++)
		{
			for (GLuint l = 1; l < 257u; l++)
			{
				if (blockIDs[i][j][l]->getID() > 0) {
					if (isAir(i, l + 1, j)) {
						textureCoords* t = blockIDs[i][j][l]->getTextureID(Block::TOP);

						vert.push_back(calcDatValue(i, l, j, t->getBotLeft(), Block::TOP));
						vert.push_back(calcDatValue(i, l, j + 1, t->getTopLeft(), Block::TOP));
						vert.push_back(calcDatValue(i + 1, l, j + 1, t->getTopRight(), Block::TOP));
						vert.push_back(calcDatValue(i + 1, l, j, t->getBotRight(), Block::TOP));

						int n = vert.size();

						ind.push_back(n - 4); ind.push_back(n - 3); ind.push_back(n - 1);
						ind.push_back(n - 1); ind.push_back(n - 3); ind.push_back(n - 2);
					}

					if (isAir(i, l - 1, j)) {
						textureCoords* t = blockIDs[i][j][l]->getTextureID(Block::BOTTOM);
						vert.push_back(calcDatValue(i, l - 1, j, t->getTopLeft(), Block::BOTTOM));
						vert.push_back(calcDatValue(i, l - 1, j + 1, t->getBotLeft(), Block::BOTTOM));
						vert.push_back(calcDatValue(i + 1, l - 1, j + 1, t->getBotRight(), Block::BOTTOM));
						vert.push_back(calcDatValue(i + 1, l - 1, j, t->getTopRight(), Block::BOTTOM));

						int n = vert.size();

						ind.push_back(n - 4); ind.push_back(n - 1); ind.push_back(n - 3);
						ind.push_back(n - 1); ind.push_back(n - 2); ind.push_back(n - 3);
					}

					if (isAir(i + 1, l, j)) {
						textureCoords* t = blockIDs[i][j][l]->getTextureID(Block::RIGHT);
						vert.push_back(calcDatValue(i + 1, l, j, t->getTopLeft(), Block::RIGHT));
						vert.push_back(calcDatValue(i + 1, l, j + 1, t->getTopRight(), Block::RIGHT));
						vert.push_back(calcDatValue(i + 1, l - 1, j + 1, t->getBotRight(), Block::RIGHT));
						vert.push_back(calcDatValue(i + 1, l - 1, j, t->getBotLeft(), Block::RIGHT));

						int n = vert.size();

						ind.push_back(n - 4); ind.push_back(n - 3); ind.push_back(n - 1);
						ind.push_back(n - 1); ind.push_back(n - 3); ind.push_back(n - 2);
					}

					if (isAir(i - 1, l, j)) {
						textureCoords* t = blockIDs[i][j][l]->getTextureID(Block::LEFT);
						vert.push_back(calcDatValue(i, l, j, t->getTopRight(), Block::LEFT));
						vert.push_back(calcDatValue(i, l, j + 1, t->getTopLeft(), Block::LEFT));
						vert.push_back(calcDatValue(i, l - 1, j + 1, t->getBotLeft(), Block::LEFT));
						vert.push_back(calcDatValue(i, l - 1, j, t->getBotRight(), Block::LEFT));

						int n = vert.size();

						ind.push_back(n - 4); ind.push_back(n - 1); ind.push_back(n - 3);
						ind.push_back(n - 1); ind.push_back(n - 2); ind.push_back(n - 3);
					}

					if (isAir(i, l, j + 1)) {
						textureCoords* t = blockIDs[i][j][l]->getTextureID(Block::BACK);
						vert.push_back(calcDatValue(i + 1, l, j + 1, t->getTopLeft(), Block::BACK));
						vert.push_back(calcDatValue(i, l, j + 1, t->getTopRight(), Block::BACK));
						vert.push_back(calcDatValue(i, l - 1, j + 1, t->getBotRight(), Block::BACK));
						vert.push_back(calcDatValue(i + 1, l - 1, j + 1, t->getBotLeft(), Block::BACK));

						int n = vert.size();

						ind.push_back(n - 4); ind.push_back(n - 3); ind.push_back(n - 1);
						ind.push_back(n - 1); ind.push_back(n - 3); ind.push_back(n - 2);
					}

					if (isAir(i, l, j - 1)) {
						textureCoords* t = blockIDs[i][j][l]->getTextureID(Block::FRONT);

						vert.push_back(calcDatValue(i + 1, l, j, t->getTopRight(), Block::FRONT));
						vert.push_back(calcDatValue(i, l, j, t->getTopLeft(), Block::FRONT));
						vert.push_back(calcDatValue(i, l - 1, j, t->getBotLeft(), Block::FRONT));
						vert.push_back(calcDatValue(i + 1, l - 1, j, t->getBotRight(), Block::FRONT));

						int n = vert.size();

						ind.push_back(n - 4); ind.push_back(n - 1); ind.push_back(n - 3);
						ind.push_back(n - 1); ind.push_back(n - 2); ind.push_back(n - 3);
					}
				}
			}
		}

		numOfIndices = ind.size();

		vertexes.BufferData(vert);
		indices.BufferData(ind);

		changed = false;
	}
}

void Chunk::drawChunk() {
	if (changed) { loadToDraw(); }

	chunk_vao.Bind();
	glDrawElements(GL_TRIANGLES, numOfIndices, GL_UNSIGNED_INT, 0);
	chunk_vao.Unbind();
}

void Chunk::generateChunk() {
	float off_x = chunkCoordinates.x * 16;
	float off_z = chunkCoordinates.y * 16;

	blockIDs.resize(17);
	for (int i = 1; i < 17; i++)
	{
		blockIDs[i].resize(17);
		for (int j = 1; j < 17; j++)
		{
			blockIDs[i][j].resize(257);
			
			int height = 68 + heightmap[i][j];

			for (int l = 1; l < 257; l++)
			{
				if (l == height) {
					blockIDs[i][j][l] = blocks[1];
				}
				else if (l < height && l > height - 4) {
					blockIDs[i][j][l] = blocks[2];
				}
				else if (l <= height - 4) {
					blockIDs[i][j][l] = blocks[3];
				}
				else {
					blockIDs[i][j][l] = blocks[0];
				}
			}
		}
	}
}

void Chunk::InitBlocks() {
	int grassTop[4] = { 0,1,17,18 };
	int grassSide[4] = { 3,4,20,21 };
	int dirt[4] = { 2,3,19,20 };
	int stone[4] = { 1,2,18,19 };
	int woodTop[4] = { 22,23,39,40 };
	int woodSide[4] = { 21,22,38,39 };
	int planks[4] = { 4,5,21,22 };

	blocks.push_back(new Block(0));
	blocks.push_back(new Block(1, grassTop, grassSide, dirt)); //fu blokk
	blocks.push_back(new Block(2, dirt)); //fold
	blocks.push_back(new Block(3, stone)); //ko
	blocks.push_back(new Block(4, woodTop, woodSide, woodTop)); //fa
	blocks.push_back(new Block(5, planks)); //megmunkalt fa
}

void Chunk::DeleteBlocks() {
	for (auto a : blocks) {
		delete a;
	}
}