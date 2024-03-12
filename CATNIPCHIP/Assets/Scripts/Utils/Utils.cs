using UnityEngine;

namespace Utility
{
	[System.Serializable]
	public struct MinMax
	{
		public float min, max;

		public float GenerateRandom()
		{
			return Random.Range(min, max);
		}
	}

	[System.Serializable]
	public struct MinMaxInt
	{
		public int min, max;

		public int GenerateRandom()
		{
			return Random.Range(min, max);
		}
	}

	[System.Serializable]
	public struct CenterRect
	{
		public Vector3 position;
		public float top, down, left, right;

		public void DrawGizmo(Color color = default)
		{
			Gizmos.color = color;
			Vector3 size = new Vector2(left + right, down + top);
			Vector3 center = new Vector2((-left + right) / 2, (-down + top) / 2);
			Vector3 offsetCenter = center + position;
			Gizmos.DrawWireCube(offsetCenter, size);
		}
	}

	public static class Utils
	{
		public static Vector3 GetMouseWorldPosition(Camera worldCamera = null)
		{
			if (worldCamera == null)
			{
				worldCamera = Camera.main;
			}

			Vector3 mousePosWithZ = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.0f);
			return worldCamera.ScreenToWorldPoint(mousePosWithZ);
		}

		public static int LoopedAdd(int value, int max, int min = 0, int addAmount = 1)
		{
			if (value + addAmount < max)
			{
				return value + addAmount;
			}
			else
			{
				return min;
			}
		}

		public static int LoopedSubtract(int value, int max, int min = 0, int subtractAmount = 1)
		{
			if (value - subtractAmount >= min)
			{
				return value - subtractAmount;
			}
			else
			{
				return max;
			}
		}

		public static Color IntColor(int r, int g, int b, int a = 255)
		{
			return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
		}

		public static Vector3 ClampVector3(Vector3 value, Vector3 min, Vector3 max)
		{
			return new Vector3(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y),
				Mathf.Clamp(value.z, min.z, max.z));
		}

		public static bool ArrayContainsNullReference(object[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					return true;
				}
			}

			return false;
		}

		public static Vector2 WorldToCanvasPoint(Vector3 worldPoint, Camera camera, Canvas canvas)
		{
			RectTransform canvasTransform = canvas.GetComponent<RectTransform>();
			Vector2 adjustedPosition = camera.WorldToScreenPoint(worldPoint);
			adjustedPosition.x *= canvasTransform.rect.width / (float)camera.pixelWidth;
			adjustedPosition.y *= canvasTransform.rect.height / (float)camera.pixelHeight;
			return adjustedPosition - canvasTransform.sizeDelta / 2f;
		}
	}
}

