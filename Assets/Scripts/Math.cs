using UnityEngine;
namespace Virulent{
	public static class Math : System.Object {
		/// <summary>
		/// Smoothly interpolate between original and target.
		/// </summary>
		/// <param name="original">
		/// A <see cref="Vector3"/>
		/// </param>
		/// <param name="target">
		/// A <see cref="Vector3"/>
		/// </param>
		/// <param name="t">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <returns>
		/// A <see cref="Vector3"/>
		/// </returns>
		public static Vector3 SmoothLerp(Vector3 original, Vector3 target, float t) {
			t = Mathf.Clamp01(t);
			float weight = -2.0f * t * t * t + 3 * t * t; // -2x^3 + 3x^2
			Vector3 delta = target - original;
			
			return original + weight * delta;
		}
		
		/// <summary>
		/// Smoothly interpolate between original and target.
		/// </summary>
		/// <param name="original">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <param name="target">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <param name="t">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Single"/>
		/// </returns>
		public static float SmoothLerp(float original, float target, float t) {
			t = Mathf.Clamp01(t);
			float weight = -2.0f * t * t * t + 3 * t * t; // -2x^3 + 3x^2
			float delta = target - original;
			
			return original + weight * delta;
		}
	}
}
