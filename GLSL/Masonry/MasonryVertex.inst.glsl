#version 460 core
layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec2 VertexUV;

layout (location = 2) in vec4 Layout;

out vec2 FragmentUV;
flat out int InstID;

uniform mat4 MVP;

void main() {
	vec3 _ = (VertexPosition * vec3(Layout.zw,1)) + vec3(Layout.xy,0);

	gl_Position = vec4(_,1) * MVP;

	FragmentUV = VertexUV;
	InstID = gl_InstanceID;
}