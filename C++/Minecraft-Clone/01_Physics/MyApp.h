#pragma once

// C++ includes
#include <memory>

// GLEW
#include <GL/glew.h>

// SDL
#include <SDL.h>
#include <SDL_opengl.h>

#include <vector>

//Minecraft
#include "Chunk.h"

class CMyApp
{
	public:
		CMyApp(void);
		~CMyApp(void);

		void generateHeightMap(std::vector<std::vector<short>>& vec, int size);

		bool Init();
		void InitChunks();
		void InitSkybox();
		void InitSunMoon();
		void Clean();

		void Update();
		void Render();

		void KeyboardDown(SDL_KeyboardEvent&);
		void KeyboardUp(SDL_KeyboardEvent&);
		void MouseMove(SDL_MouseMotionEvent&);
		void MouseDown(SDL_MouseButtonEvent&);
		void MouseUp(SDL_MouseButtonEvent&);
		void MouseWheel(SDL_MouseWheelEvent&);
		void Resize(int, int);
	protected:
		ProgramObject m_SkyboxProgram;
		VertexArrayObject m_SkyboxVao;
		ArrayBuffer m_SkyboxVert;
		ArrayBuffer m_SkyboxCol;
		IndexBuffer m_SkyboxInd;

		ProgramObject m_MinecraftCloneProgram;
		Texture2D m_textureAtlas;

		ProgramObject m_SunMoonProgram;
		VertexArrayObject m_SunMoonVao;
		ArrayBuffer m_SunMoonVert;
		ArrayBuffer m_SunMoonTex;
		IndexBuffer m_SunMoonInd;

		Texture2D m_sunTexture;
		Texture2D m_moonTexture;

		int N = 10;
		int M = 10;

		std::vector<Chunk*> chunks;

		gCamera				m_camera;

		float currAngle = 0.f;
		float igtime = 0.f;
		int night = 0;

		bool mouseOn = true;
		float* eye;
		float* at;
};

