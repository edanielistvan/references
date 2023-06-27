#include "MyApp.h"

#include <math.h>
#include <vector>

#include <array>
#include <list>
#include <tuple>

#include <random>

#include "imgui\imgui.h"

#include "Includes/ObjParser_OGL3.h"

CMyApp::CMyApp(void)
{
	m_camera.SetView(glm::vec3(5, 80, 5), glm::vec3(0, 0, 0), glm::vec3(0, 1, 0));
}

CMyApp::~CMyApp(void)
{
	for (int i = 0; i < chunks.size(); i++) delete chunks[i];

	Chunk::DeleteBlocks();
}

void CMyApp::generateHeightMap(std::vector<std::vector<short>>& vec, int size) {
	srand(time(NULL));

	int div = 5; int randDiv = 1;

	int a = rand() % div, b = rand() % div, c = rand() % div, d = rand() % div;
	vec[0][0] = a; vec[size - 1][0] = b; vec[0][size - 1] = c; vec[size - 1][size - 1] = d;

	for (int sideLength = size - 1; sideLength >= 2; sideLength /= 2)
	{
		//srand(time(NULL));

		int half = sideLength / 2;

		for (int i = 0; i < size - 1; i += sideLength)
		{
			for (int j = 0; j < size - 1; j += sideLength)
			{
				float avg = vec[i][j] + vec[i + sideLength][j] + vec[i][j + sideLength] + vec[i + sideLength][j + sideLength] + rand() % div;
				avg /= (5.0f + (float)rand() / (float)RAND_MAX * 2 * randDiv - randDiv);
				vec[i + half][j + half] = round(avg);
			}
		}

		for (int i = 0; i < size - 1; i += half)
		{
			for (int j = (i + half) % sideLength; j < size - 1; j += sideLength)
			{
				float avg = vec[(i - half + size - 1) % (size - 1)][j] + vec[(i + half) % (size - 1)][j] +
							 vec[i][(j + half) % (size - 1)] + vec[i][(j - half + size - 1) % (size - 1)] + rand() % div;
				avg /= (5.0f + (float)rand() / (float)RAND_MAX * 2 * randDiv - randDiv);
				vec[i][j] = round(avg);

				if (i == 0) vec[size - 1][j] = round(avg);
				if (j == 0) vec[i][size - 1] = round(avg);
			}
		}
	}
}

void CMyApp::InitChunks()
{
	Chunk::InitBlocks();

	std::vector<std::vector<short>> hm;
	hm.resize(N * 16 + 1);
	for (int i = 0; i < hm.size(); i++) { hm[i].resize(M * 16 + 1, 0.0f); }

	generateHeightMap(hm, hm.size());

	for (int i = 0; i < N; i++) {
		for (int j = 0; j < M; j++) {
			std::vector<std::vector<short>> heightmap;
			heightmap.resize(17);

			for (int a = 0; a < 16; a++) {
				heightmap[a + 1].resize(17);
				for (int b = 0; b < 16; b++) {
					heightmap[a + 1][b + 1] = hm[i * 16 + a][j * 16 + b];
				}
			}

			chunks.push_back(new Chunk(i - N / 2, j - M / 2, heightmap));
		}
	}

	for (int i = 0; i < N; i++) {
		for (int j = 0; j < M; j++) {
			if (i > 0) chunks[i * M + j]->connectNeighbor(chunks[(i - 1) * M + j], 0);
			if (i < N - 1) chunks[i * M + j]->connectNeighbor(chunks[(i + 1) * M + j], 1);
			if (j > 0) chunks[i * M + j]->connectNeighbor(chunks[i * M + j - 1], 2);
			if (j < M - 1) chunks[i * M + j]->connectNeighbor(chunks[i * M + j + 1], 3);
		}
	}
}

void CMyApp::InitSkybox() {
	m_SkyboxProgram.Init({
		{GL_VERTEX_SHADER, "skybox.vert"},
		{GL_FRAGMENT_SHADER, "skybox.frag"}
	},
	{
		{ 0, "vs_in_pos" },
		{ 1, "vs_in_col"}
	});

	m_SkyboxVert.BufferData(std::vector<glm::vec3>{
		glm::vec3(-1, -1, -1),
		glm::vec3(-1, -1, 1),
		glm::vec3(1, -1, 1),
		glm::vec3(1, -1, -1),
		glm::vec3(-1, 1, -1),
		glm::vec3(-1, 1, 1),
		glm::vec3(1, 1, 1),
		glm::vec3(1, 1, -1),
		glm::vec3(-1,-0.1f,-1),
		glm::vec3(1,-0.1f,-1),
		glm::vec3(1,-0.1f,1),
		glm::vec3(-1,-0.1f,1),
		glm::vec3(-1, 0.1f, -1),
		glm::vec3(1, 0.1f, -1),
		glm::vec3(1, 0.1f, 1),
		glm::vec3(-1, 0.1f, 1)
	});

	m_SkyboxCol.BufferData(std::vector<glm::vec3> {
		glm::vec3(0.f, 0.4f, 0.8f),
		glm::vec3(0.f, 0.4f, 0.8f),
		glm::vec3(0.f, 0.4f, 0.8f),
		glm::vec3(0.f, 0.4f, 0.8f),
		glm::vec3(0.2f, 0.6f, 1.f),
		glm::vec3(0.2f, 0.6f, 1.f),
		glm::vec3(0.2f, 0.6f, 1.f),
		glm::vec3(0.2f, 0.6f, 1.f),
		glm::vec3(0.f, 0.4f, 0.8f),
		glm::vec3(0.f, 0.4f, 0.8f),
		glm::vec3(0.f, 0.4f, 0.8f),
		glm::vec3(0.f, 0.4f, 0.8f),
		glm::vec3(0.2f, 0.6f, 1.f),
		glm::vec3(0.2f, 0.6f, 1.f),
		glm::vec3(0.2f, 0.6f, 1.f),
		glm::vec3(0.2f, 0.6f, 1.f)
	});

	m_SkyboxInd.BufferData(std::vector<GLuint> {
		0,1,3,
		3,1,2,
		0,3,8,
		3,9,8,
		8,9,12,
		9,13,12,
		12,13,4,
		13,7,4,
		2,9,3,
		2,10,9,
		10,13,9,
		10,14,13,
		14,7,13,
		14,6,7,
		2,1,11,
		2,11,10,
		10,11,15,
		10,15,14,
		14,15,5,
		14,5,6,
		1,0,8,
		1,8,11,
		11,8,12,
		11,12,15,
		15,12,4,
		15,4,5,
		5,7,6,
		5,4,7
	});

	m_SkyboxVao.Init(
		{
			{ CreateAttribute<0, glm::vec3, 0, sizeof(glm::vec3)>, m_SkyboxVert},
			{ CreateAttribute<1, glm::vec3, 0, sizeof(glm::vec3)>, m_SkyboxCol}
		},
		m_SkyboxInd);
}

void CMyApp::InitSunMoon()
{
	m_SunMoonProgram.Init({
		{GL_VERTEX_SHADER, "sunmoon.vert"},
		{GL_FRAGMENT_SHADER, "sunmoon.frag"}
	},
	{
		{0,"vs_in_pos"},
		{1,"vs_in_tex"}
	});

	m_SunMoonVert.BufferData(std::vector<glm::vec3> {
		glm::vec3(-1, -1, 0),
		glm::vec3(-1, 1, 0),
		glm::vec3(1, 1, 0),
		glm::vec3(1, -1, 0)
	});

	m_SunMoonTex.BufferData(std::vector<glm::vec2> {
		glm::vec2(1, 0),
		glm::vec2(1, 1),
		glm::vec2(0, 1),
		glm::vec2(0, 0)
	});

	m_SunMoonInd.BufferData(std::vector<GLuint> {
		0, 3, 1,
		3, 2, 1
	});

	m_SunMoonVao.Init({
		{ CreateAttribute<0, glm::vec3, 0, sizeof(glm::vec3)>, m_SunMoonVert},
		{ CreateAttribute<1, glm::vec2, 0, sizeof(glm::vec2)>, m_SunMoonTex}
	},
	m_SunMoonInd);
}

bool CMyApp::Init()
{
	glClearColor(0.1f,0.1f,0.41f, 1);

	glLineWidth(1.5f);

	glEnable(GL_DEPTH_TEST);
	glEnable(GL_CULL_FACE);

	m_MinecraftCloneProgram.Init({
		{GL_VERTEX_SHADER,		"minecraft-clone.vert"},
		{GL_FRAGMENT_SHADER,	"minecraft-clone.frag"}
	},
	{
		{0, "vs_in_dat"}
	});

	eye = new float[3];
	at = new float[3];

	InitChunks();
	InitSkybox();
	InitSunMoon();
	
	//texturak
	m_textureAtlas.FromFile("Assets/atlas.png");
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST_MIPMAP_LINEAR);

	m_sunTexture.FromFile("Assets/sun.png");
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST_MIPMAP_LINEAR);

	m_moonTexture.FromFile("Assets/moon.png");
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST_MIPMAP_LINEAR);

	glBindTexture(GL_TEXTURE_2D, 0);

	// kamera
	//m_camera.SetView(glm::vec3(0.0f, 80.0f, 0.0f), glm::vec3(0.0f, 0.0f, 0.0f), glm::vec3(0.0f, 1.0f, 0.0f));
	m_camera.SetProj(45.0f, 640.0f / 480.0f, 0.01f, 1000.0f);

	return true;
}

void CMyApp::Clean()
{
}

void CMyApp::Update()
{
	static Uint32 last_time = SDL_GetTicks();
	float delta_time = (SDL_GetTicks() - last_time) / 1000.0f;

	currAngle += delta_time * 2 * M_PI / 30.f;
	igtime += delta_time;

	if (igtime >= 15.f) {
		igtime -= 15.f;
		if (night == 0) night = 1;
		else night = 0;
	}

	m_camera.Update(delta_time);

	last_time = SDL_GetTicks();
}

void CMyApp::Render()
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

	glm::mat4 viewProj = m_camera.GetViewProj();

	glm::vec3 e = m_camera.GetEye();
	glm::vec3 a = m_camera.GetAt();

	m_MinecraftCloneProgram.Use();

	glm::mat4 smcons = glm::translate<float>(glm::vec3(0, 0, -10));
	glm::mat4 srotate = glm::rotate<float>(currAngle, glm::vec3(1, 0, 0));
	glm::mat4 mrotate = glm::rotate<float>(currAngle + M_PI, glm::vec3(1, 0, 0));
	//glm::mat4 skyrotate = glm::rotate<float>(currAngle - (M_PI / 2.f), glm::vec3(1, 0, 0));

	m_MinecraftCloneProgram.SetUniform("eye_pos", e);
	m_MinecraftCloneProgram.SetUniform("night", night);

	m_MinecraftCloneProgram.SetTexture("texImage", 0, m_textureAtlas);

	for (int i = 0; i < N; i++) {
		for (int j = 0; j < M; j++) {
			float x = chunks[i * M + j]->chunkCoordinates.x;
			float z = chunks[i * M + j]->chunkCoordinates.y;
			glm::mat4 world = glm::translate<float>(glm::vec3(x * 16, 0, z * 16));
			m_MinecraftCloneProgram.SetUniform("mvp", viewProj * world);
			m_MinecraftCloneProgram.SetUniform("world",world);
			m_MinecraftCloneProgram.SetUniform("worldIT", glm::transpose(glm::inverse(world)));

			chunks[i * M + j]->drawChunk();
		}
	}

	m_MinecraftCloneProgram.Unuse();

	glm::mat4 constant = viewProj * glm::translate(e);

	m_SkyboxProgram.Use();
	GLint prevDp;
	glGetIntegerv(GL_DEPTH_FUNC, &prevDp);
	glDepthFunc(GL_LEQUAL);

	m_SkyboxVao.Bind();

	m_SkyboxProgram.SetUniform("mvp", constant /** skyrotate*/);
	glDrawElements(GL_TRIANGLES, 84, GL_UNSIGNED_INT, 0);

	m_SkyboxVao.Unbind();
	m_SkyboxProgram.Unuse();

	m_SunMoonProgram.Use();
	m_SunMoonVao.Bind();

	glm::mat4 sworld = srotate * smcons;
	m_SunMoonProgram.SetUniform("mvp", constant * sworld);
	m_SunMoonProgram.SetTexture("texImage", 0, m_sunTexture);
	glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
	
	glm::mat4 mworld = mrotate * smcons;
	m_SunMoonProgram.SetUniform("mvp", constant * mworld);
	m_SunMoonProgram.SetTexture("texImage", 0, m_moonTexture);
	glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);

	m_SunMoonVao.Unbind();
	m_SunMoonProgram.Unuse();

	glDepthFunc(prevDp);

	eye[0] = e.x; eye[1] = e.y; eye[2] = e.z;
	at[0] = a.x; at[1] = a.y; at[2] = a.z;

	//ImGui::ShowTestWindow();

	if (ImGui::Begin("Coordinates")) {
		ImGui::InputFloat3("Location",  eye);
		ImGui::InputFloat3("Look-at", at);
	}

	ImGui::End();
}

void CMyApp::KeyboardDown(SDL_KeyboardEvent& key)
{
	if (key.keysym.sym == SDLK_ESCAPE) {
		if (mouseOn) SDL_SetRelativeMouseMode(SDL_FALSE);
		else  SDL_SetRelativeMouseMode(SDL_TRUE);

		mouseOn = !mouseOn;
	}

	if(mouseOn) m_camera.KeyboardDown(key);
}

void CMyApp::KeyboardUp(SDL_KeyboardEvent& key)
{
	if(mouseOn) m_camera.KeyboardUp(key);
}

void CMyApp::MouseMove(SDL_MouseMotionEvent& mouse)
{
	if (mouseOn) {
		m_camera.MouseMove(mouse);
	}
}

void CMyApp::MouseDown(SDL_MouseButtonEvent& mouse)
{
}

void CMyApp::MouseUp(SDL_MouseButtonEvent& mouse)
{
}

void CMyApp::MouseWheel(SDL_MouseWheelEvent& wheel)
{
}

// a két paraméterbe az új ablakméret szélessége (_w) és magassága (_h) található
void CMyApp::Resize(int _w, int _h)
{
	glViewport(0, 0, _w, _h );

	m_camera.Resize(_w, _h);
}