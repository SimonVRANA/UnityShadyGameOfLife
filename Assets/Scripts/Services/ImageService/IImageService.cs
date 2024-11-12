// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System;
using UnityEngine;

public interface IImageService
{
	public const int minimumNumberOfColumns = 6;
	public const int maximumNumberOfColumns = 1800;
	public const int columnsIncrementNumber = 6;

	/// <summary>
	/// Multiply the number of colums by this to get the corresponding number of rows.
	/// </summary>
	public const float columnsToRowsRatio = 1.0f / 2.0f;

	public int NumberOfColumns { get; set; }

	public event EventHandler OnNumberOfColumnsChanged;

	public void ApplyShader(Material material);

	public void RandomizePixels(Color deadColor, Color aliveColor);
}