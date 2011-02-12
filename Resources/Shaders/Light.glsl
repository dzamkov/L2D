uniform vec3 SunDirection;
uniform float Diffuse;
uniform float Ambient;

uniform sampler2D Material;

varying vec3 Normal;

#ifdef _VERTEX_
void main()
{
	Normal = normalize(gl_NormalMatrix * gl_Normal);
	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
}
#endif

#ifdef _FRAGMENT_
void main()
{
	vec2 uv = gl_TexCoord[0];
	vec4 col = texture2D(Material, uv);
	
	vec3 diffuse = col.rgb;
	float light = dot(Normal, SunDirection) + 0.5;
	gl_FragColor = vec4(diffuse * light, 1.0);
}
#endif