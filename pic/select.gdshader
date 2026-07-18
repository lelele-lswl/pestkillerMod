shader_type canvas_item;

// 极坐标效果参数
uniform bool polar_coordinates = false;
uniform vec2 polar_center = vec2(0.5);
uniform float polar_zoom = 1.0;
uniform float polar_repeat = 1.0;

// 旋转和偏移参数
uniform highp float spin_rotation = 0.0; 
uniform highp float spin_speed = 1.0;
uniform highp vec2 offset = vec2(0.0, 0.0); 

// 颜色配置 - 使用更柔和的配色
uniform highp vec4 colour_1 : source_color = vec4(0.15, 0.25, 0.4, 1.0);
uniform highp vec4 colour_2 : source_color = vec4(0.25, 0.2, 0.45, 1.0);
uniform highp vec4 colour_3 : source_color = vec4(0.4, 0.3, 0.35, 1.0);

// 效果强度参数
uniform highp float contrast = 1.2;
uniform highp float spin_amount = 0.2;
uniform highp float pixel_filter = 1920.0; // 提升到1920像素级别

// 透明度控制
uniform float overlay_opacity : hint_range(0.0, 1.0) = 0.3;

#define SPIN_EASE 1.0

vec4 effect(vec2 screenSize, vec2 screen_coords){
	// 去掉像素效果，直接使用原始坐标
    highp vec2 uv = screen_coords - offset;
    highp float uv_len = length(uv);
	
	// 中心漩涡效果，随时间变化
    highp float speed = (spin_rotation * SPIN_EASE * 0.2) + 302.2;
    highp float new_pixel_angle = (atan(uv.y, uv.x)) + speed - SPIN_EASE * 20.0 * (1.0 * spin_amount * uv_len + (1.0 - 1.0 * spin_amount));
    highp vec2 mid = (screenSize.xy / length(screenSize.xy)) / 2.0;
    uv = (vec2((uv_len * cos(new_pixel_angle) + mid.x), (uv_len * sin(new_pixel_angle) + mid.y)) - mid);
	
	// 添加流体效果
    uv *= 30.0;
    speed = TIME * (spin_speed);
	highp vec2 uv2 = vec2(uv.x + uv.y);
	
	for(int i = 0; i < 5; i++) {
		uv2 += sin(max(uv.x, uv.y)) + uv;
		uv  += 0.5 * vec2(cos(5.1123314 + 0.353 * uv2.y + speed * 0.131121), sin(uv2.x - 0.113 * speed));
		uv  -= 1.0 * cos(uv.x + uv.y) - 1.0 * sin(uv.x * 0.711 - uv.y);
	}
	
	// 计算颜色混合
    highp float contrast_mod = (0.25 * contrast + 0.5 * spin_amount + 1.2);
	highp float paint_res = min(2.0, max(0.0, length(uv) * (0.035) * contrast_mod));
    highp float c1p = max(0.0, 1.0 - contrast_mod * abs(1.0 - paint_res));
    highp float c2p = max(0.0, 1.0 - contrast_mod * abs(paint_res));
    highp float c3p = 1.0 - min(1.0, c1p + c2p);
	
	highp vec4 ret_col = (0.3/contrast) * colour_1 + (1.0 - 0.3/contrast) * (colour_1 * c1p + colour_2 * c2p + vec4(c3p * colour_3.rgb, c3p * colour_1.a));
	return ret_col;
}

vec2 polar_coords(vec2 uv, vec2 center, float zoom, float repeat){
	vec2 dir = uv - center;
	float radius = length(dir) * 2.0;
	float angle = atan(dir.y, dir.x) * 1.0 / (PI * 2.0);
	return mod(vec2(radius * zoom, angle * repeat), 1.0);
}

void fragment() {
	vec2 polarCoords = UV;
	if (polar_coordinates){
		polarCoords = polar_coords(UV.xy, polar_center, polar_zoom, polar_repeat);
	}
	
	// 应用效果并设置透明度
	vec4 effectColor = effect(TEXTURE_PIXEL_SIZE, polarCoords);
	COLOR = vec4(effectColor.rgb, effectColor.a * overlay_opacity);
}