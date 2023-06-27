#ifndef CHUNK_H_
#define CHUNK_H_

#include "Block.h"

class Chunk
{
private:
	bool changed;
	int numOfIndices;

	void generateChunk();
	void loadToDraw();

	GLuint Chunk::calcDatValue(GLuint x, GLuint y, GLuint z, GLuint t, Block::TextureSide side) const;

	std::vector<Chunk*> neighbors;
protected:
	VertexArrayObject chunk_vao;
	IndexBuffer indices;
	ArrayBuffer vertexes;
	std::vector<std::vector<std::vector<Block*>>> blockIDs; //sekely
	std::vector<std::vector<short>> heightmap;

	bool isAir(int x, int y, int z) const;
public:
	static std::vector<Block*> blocks;
	static void InitBlocks();
	static void DeleteBlocks();

	glm::vec2 chunkCoordinates;

	Chunk(int x, int z, std::vector<std::vector<short>>& heightmap);
	~Chunk();

	void drawChunk();
	void connectNeighbor(Chunk* ch, const int i);

	const std::vector<std::vector<short>>* getHeightmap() const;
};
#endif