#ifndef FLOW_INCLUDED
#define FLOW_INCLUDED

float3 flowUV(
	float2 uv, float2 flowVector, float2 jump, 
	float tiling, float time,bool flowB
) {
	//使从(0,0)到(1,1)的变换 为(0,0)到(0.5,1) 和(0.5,1)到(1,1)
    //0 = k * 0 + b         1 = k * 0.5 +b;
    //1 = k * 0.5 + b;      0 = k * 1 + b;
    //y = 2x                y = -2x + 2

	//原函数关于x= 0.5对称的函数为 2-2x
	float phaseOffset = flowB ? 0.5 : 0;
	float progress = frac(time + phaseOffset);
	float3 uvw;
	//显示向前移动
	uvw.xy = uv - flowVector * progress;
	uvw.xy *= tiling;
	uvw.xy += phaseOffset;
	uvw.xy += (time - progress) * jump;
	//渐变系数，保存到z值上
	uvw.z = 1 - abs(1 - 2 * progress);
	return uvw;
}

#endif