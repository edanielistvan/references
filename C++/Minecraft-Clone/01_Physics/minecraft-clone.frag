#version 130

in vec3 vs_out_pos;
in vec3 vs_out_norm;
in vec2 vs_out_tex0;

out vec4 fs_out_color;

vec4 color = vec4(0.7f, 0.7f, 0.7f, 1);

uniform vec3 eye_pos;

//sun
uniform vec3 sunlight_pos = vec3(10,10,10);

uniform vec4 Lsa = vec4(0.3f, 0.3f, 0.3f, 1);
uniform vec4 Lsd = vec4(0.6f, 0.6f, 0.4f, 1);
uniform vec4 Lss = vec4(1, 1, 0.7f, 1);

//moon
uniform vec3 moonlight_pos = vec3(10,10,10);

uniform vec4 Lma = vec4(0.1f, 0.1f, 0.1f, 1);
uniform vec4 Lmd = vec4(0.2f, 0.2f, 0.2f, 1);
uniform vec4 Lms = vec4(0.3f, 0.3f, 0.3f, 1);

//material
uniform vec4 Ka = vec4(1, 1, 1, 0);
uniform vec4 Kd = vec4(0.75f, 0.75f, 0.75f, 1);
uniform vec4 Ks = vec4(0, 0, 0, 0);
uniform float specular_power = 32;

uniform sampler2D texImage;

uniform int night = 0;

void main()
{
	vec3 normal = normalize(vs_out_norm);

	if(night == 0)
	{
		//ambiens
		vec4 sambient = Lsa * Ka;

		//diffuz
		vec3 toSLight = normalize(sunlight_pos); //minden blokkot ugyanonnan vilagitunk meg -> lightPos = vs_out_pos + sunlight_pos, lightPos - vs_out_pos = sunlight_pos
		float dis = clamp(dot(toSLight,normal),0.0f,1.0f);
		vec4 sdiffuse = vec4(Lsd.rgb * Kd.rgb * dis, Kd.a);

		//spekularis
		vec4 sspecular = vec4(0); 

		if(dis > 0) 
		{
			vec3 e = normalize( eye_pos - vs_out_pos );
			vec3 r = reflect( -toSLight, normal );
			float si = pow( clamp( dot(e, r), 0.0f, 1.0f ), specular_power );
			sspecular = Lss*Ks*si;
		}

		fs_out_color = (sambient + sdiffuse + sspecular) * texture(texImage, vs_out_tex0.st);
	}
	else
	{
		//ambiens
		vec4 mambient = Lma * Ka;

		//diffuz
		vec3 toMLight = normalize(moonlight_pos);
		float dim = clamp(dot(toMLight,normal),0.0f,1.0f);
		vec4 mdiffuse = vec4(Lmd.rgb * Kd.rgb * dim, Kd.a);

		//spekularis
		vec4 mspecular = vec4(0);

		if(dim > 0)
		{
			vec3 e = normalize( eye_pos - vs_out_pos );
			vec3 r = reflect( -toMLight, normal );
			float si = pow( clamp( dot(e, r), 0.0f, 1.0f ), specular_power );
			mspecular = Lms*Ks*si;
		}

		fs_out_color = (mambient + mdiffuse + mspecular) * texture(texImage, vs_out_tex0.st);
	}
}