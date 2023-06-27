#version 130

in uint vs_in_dat;

out vec3 vs_out_pos;
out vec3 vs_out_norm;
out vec2 vs_out_tex0;

uniform mat4 mvp;
uniform mat4 world;
uniform mat4 worldIT;

struct returnData
{
	vec3 coords;
	vec3 normal;
	int tex;
};

returnData parseDat(uint dat)
{
	uint divisors[7]; divisors[0] = 512u; divisors[1] = 8u; divisors[2] = 256u; divisors[3] = 8u; divisors[4] = 4u; divisors[5] = 4u; divisors[6] = 4u;

	int nx, ny, nz, x, y, z, t, s;
	uint div;
	
	div = 512u;
	t = int(dat % div);
	dat -= (dat % div);
	dat /= div;

	div = 32u;
	z = int(dat % div);
	dat -= (dat % div);
	dat /= div;

	div = 512u;
	y = int(dat % div);
	dat -= (dat % div);
	dat /= div;

	div = 32u;
	x = int(dat % div);
	dat -= (dat % div);
	dat /= div;

	div = 2u;
	nz = int(dat % div);
	dat -= (dat % div);
	dat /= div;

	div = 2u;
	ny = int(dat % div);
	dat -= (dat % div);
	dat /= div;

	div = 2u;
	nx = int(dat % div);
	dat -= (dat % div);
	dat /= div;

	div = 2u;
	s = int(dat % div);
	dat -= (dat % div);
	dat /= div;

	if(s==0) s = 1;
	else s = -1;

	returnData datt = returnData(vec3(x,y,z), vec3(s*nx,s*ny,s*nz), t);

	return datt;
}

void main()
{
	returnData data = parseDat(vs_in_dat);
	gl_Position = mvp * vec4(data.coords, 1.f);
	vs_out_pos = (world * vec4(data.coords,1)).xyz;
	vs_out_norm = (worldIT * vec4(vec3(data.normal), 0)).xyz;
	vs_out_tex0 = vec2((data.tex % 17) / 16.f, 1 - floor(data.tex / 17.f) / 16);
}