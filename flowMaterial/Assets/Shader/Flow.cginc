#ifndef FLOW_INCLUDED
#define FLOW_INCLUDED

float3 flowUV(float2 uv, float2 flowVector,float time) {
	//使从(0,0)到(1,1)的变换 为(0,0)到(0.5,1)
    //0 = k * 0 + b
    //1 = k * 0.5 + b;
    //y = 2x
	float progress = frac(time);
	float3 uvw;
	//显示向前移动
	uvw.xy = uv - flowVector * progress;
	//渐变系数
	uvw.z = 1;


	return uvw;
}

#endif