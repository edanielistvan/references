#ifndef TEXTURECOORDS_H_
#define TEXTURECOORDS_H_

// GLM
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtx/transform2.hpp>

#include "Includes/ProgramObject.h"
#include "Includes/BufferObject.h"
#include "Includes/VertexArrayObject.h"
#include "Includes/TextureObject.h"

#include "Includes/Mesh_OGL3.h"
#include "Includes/gCamera.h"

struct textureCoords {
	private:
		GLuint topLeft;
		GLuint topRight;
		GLuint bottomLeft;
		GLuint bottomRight;
	public:
		textureCoords(GLuint tL, GLuint tR, GLuint bL, GLuint bR);
		~textureCoords() {};

		GLuint getTopLeft() const;
		GLuint getTopRight() const;
		GLuint getBotLeft() const;
		GLuint getBotRight() const;
};
#endif