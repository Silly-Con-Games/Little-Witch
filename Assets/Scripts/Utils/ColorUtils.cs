using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtils
{
	public static Color Desaturate(Color input, float fraction) {
		float h,s,v;
		Color.RGBToHSV(input, out h, out s, out v);
		s *= fraction;
		return Color.HSVToRGB(h, s, v);
	}

	public static Color Darken(Color input, float strength) {
		float h, s, v;
		Color.RGBToHSV(input, out h, out s, out v);
		v *= strength;
		return Color.HSVToRGB(h, s, v);
	}

	public static void SetColor(MeshRenderer mesh, Color color) {
		Material[] tempMaterials = new Material[mesh.sharedMaterials.Length];
		for (int i = 0; i < mesh.sharedMaterials.Length; i++) {
			tempMaterials[i] = new Material(mesh.sharedMaterials[i]);
			tempMaterials[i].SetColor("_Color", color);
		}
		mesh.sharedMaterials = tempMaterials;
	}

	public static void SetColors(MeshRenderer mesh, Color[] colors) {
		Material[] tempMaterials = new Material[colors.Length];
		for (int i = 0; i < colors.Length; i++) {
			tempMaterials[i] = new Material(mesh.sharedMaterials[i]);
			tempMaterials[i].SetColor("_Color", colors[i]);
		}
		mesh.sharedMaterials = tempMaterials;
	}

	public static void SetSaturation(MeshRenderer mesh, float value) {
		Material[] tempMaterials = new Material[mesh.sharedMaterials.Length];
		for (int i = 0; i < tempMaterials.Length; i++) {
			tempMaterials[i] = new Material(mesh.sharedMaterials[i]);
			tempMaterials[i].SetFloat("_Saturation", value);
		}
		mesh.sharedMaterials = tempMaterials;
	}

	public static Gradient GetGradient(Color from, Color to) {  // not used now, but might get useful later
		Gradient gradient = new Gradient();
		GradientColorKey[] colorKeys = new GradientColorKey[2];
		colorKeys[0].color = from;
		colorKeys[0].time = 0f;
		colorKeys[1].color = to;
		colorKeys[1].time = 1f;

		GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
		alphaKeys[0].alpha = 1f;
		alphaKeys[0].time = 0f;
		alphaKeys[1].alpha = 1f;
		alphaKeys[1].time = 1f;

		gradient.SetKeys(colorKeys, alphaKeys);

		return gradient;
	}
}
