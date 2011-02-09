
uniform vec3 SunDirection;
uniform float Diffuse;
uniform float Ambient;

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
	vec3 diffuse = vec3(2.0, 2.0, 2.0);
	float light = dot(Normal, SunDirection) + 0.5;
	gl_FragColor = vec4(diffuse * light, 1.0);
}
#endif