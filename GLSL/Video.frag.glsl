#version 460 core
#extension GL_ARB_bindless_texture : enable
#extension GL_ARB_gpu_shader_int64 : enable

in vec2 FragmentUV;

out vec4 FinalColor;

layout (std430, binding = 1) buffer ActiveFrame {
	uint64_t ActiveFrameHandle;
};

void main() {
	sampler2D Sampler = sampler2D(ActiveFrameHandle);

	vec4 o = vec4(1,1,1,1);
	vec4 t = texture2D(Sampler, FragmentUV);

	FinalColor = t;
}