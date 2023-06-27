#version 130

in vec3 vs_out_pos;
in vec3 vs_out_tex0;

out vec4 fs_out_col;

uniform sampler2D texImage;

void main()
{
	fs_out_col = vec4(0.7f,0.7f,0.7f,1.f) * texture(texImage, vs_out_tex0.st);
}