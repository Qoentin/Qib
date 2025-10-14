#version 460 core
layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec2 VertexUV;

layout (location = 2) in vec3 InstOffset;

out vec2 FragmentUV;

uniform mat4 MVP;

void main() {
	gl_Position = vec4(VertexPosition + InstOffset, 1.0) * MVP;

	FragmentUV = vec2(VertexUV.x, 1-VertexUV.y);
}