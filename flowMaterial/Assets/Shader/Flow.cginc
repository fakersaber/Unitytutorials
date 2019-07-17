#ifndef FLOW_INCLUDED
#define FLOW_INCLUDED

float3 flowUV(float2 uv, float2 flowVector,float time) {
	//ʹ��(0,0)��(1,1)�ı任 Ϊ(0,0)��(0.5,1)
    //0 = k * 0 + b
    //1 = k * 0.5 + b;
    //y = 2x
	float progress = frac(time);
	float3 uvw;
	//��ʾ��ǰ�ƶ�
	uvw.xy = uv - flowVector * progress;
	//����ϵ��
	uvw.z = 1;


	return uvw;
}

#endif