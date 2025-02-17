using System;
using System.Numerics;

namespace Trivial.Graph.Domain.Geometry;

/// <summary>
/// Bezier Spline methods
/// </summary>
/// <remarks>
/// Modified: Peter Lee (peterlee.com.cn < at > gmail.com)
///   Update: 2009-03-16
/// 
/// see also:
/// Draw a smooth curve through a set of 2D points with Bezier primitives
/// http://www.codeproject.com/KB/graphics/BezierSpline.aspx
/// By Oleg V. Polikarpotchkin
/// 
/// Algorithm Descripition:
/// 
/// To make a sequence of individual Bezier curves to be a spline, we
/// should calculate Bezier control points so that the spline curve
/// has two continuous derivatives at knot points.
/// 
/// Note: `[]` denotes subscript
///        `^` denotes supscript
///        `'` denotes first derivative
///       `''` denotes second derivative
	///       
/// A Bezier curve on a single interval can be expressed as:
/// 
/// B(t) = (1-t)^3 P0 + 3(1-t)^2 t P1 + 3(1-t)t^2 P2 + t^3 P3          (*)
/// 
/// where t is in [0,1], and
///     1. P0 - first knot point
///     2. P1 - first control point (close to P0)
///     3. P2 - second control point (close to P3)
///     4. P3 - second knot point
///     
/// The first derivative of (*) is:
/// 
/// B'(t) = -3(1-t)^2 P0 + 3(3t^2–4t+1) P1 + 3(2–3t)t P2 + 3t^2 P3
/// 
/// The second derivative of (*) is:
/// 
/// B''(t) = 6(1-t) P0 + 6(3t-2) P1 + 6(1–3t) P2 + 6t P3
/// 
/// Considering a set of piecewise Bezier curves with n+1 points
/// (Q[0..n]) and n subintervals, the (i-1)-th curve should connect
/// to the i-th one:
/// 
/// Q[0] = P0[1],
/// Q[1] = P0[2] = P3[1], ... , Q[i-1] = P0[i] = P3[i-1]  (i = 1..n)   (@)
/// 
/// At the i-th subinterval, the Bezier curve is:
/// 
/// B[i](t) = (1-t)^3 P0[i] + 3(1-t)^2 t P1[i] + 
///           3(1-t)t^2 P2[i] + t^3 P3[i]                 (i = 1..n)
/// 
/// applying (@):
/// 
/// B[i](t) = (1-t)^3 Q[i-1] + 3(1-t)^2 t P1[i] + 
///           3(1-t)t^2 P2[i] + t^3 Q[i]                  (i = 1..n)   (i)
///           
/// From (i), the first derivative at the i-th subinterval is:
/// 
/// B'[i](t) = -3(1-t)^2 Q[i-1] + 3(3t^2–4t+1) P1[i] +
///            3(2–3t)t P2[i] + 3t^2 Q[i]                 (i = 1..n)
/// 
/// Using the first derivative continuity condition:
/// 
/// B'[i-1](1) = B'[i](0)
/// 
/// we get:
/// 
/// P1[i] + P2[i-1] = 2Q[i-1]                             (i = 2..n)   (1)
/// 
/// From (i), the second derivative at the i-th subinterval is:
/// 
/// B''[i](t) = 6(1-t) Q[i-1] + 6(3t-2) P1[i] +
///             6(1-3t) P2[i] + 6t Q[i]                   (i = 1..n)
/// 
/// Using the second derivative continuity condition:
/// 
/// B''[i-1](1) = B''[i](0)
/// 
/// we get:
/// 
/// P1[i-1] + 2P1[i] = P2[i] + 2P2[i-1]                   (i = 2..n)   (2)
/// 
/// Then, using the so-called "natural conditions":
/// 
/// B''[1](0) = 0
/// 
/// B''[n](1) = 0
/// 
/// to the second derivative equations, and we get:
/// 
/// 2P1[1] - P2[1] = Q[0]                                              (3)
/// 
/// 2P2[n] - P1[n] = Q[n]                                              (4)
/// 
/// From (1)(2)(3)(4), we have 2n conditions for n first control points
/// P1[1..n], and n second control points P2[1..n].
/// 
/// Eliminating P2[1..n], we get (be patient to get :-) a set of n
/// equations for solving P1[1..n]:
/// 
///   2P1[1]   +  P1[2]   +            = Q[0] + 2Q[1]
///    P1[1]   + 4P1[2]   +    P1[3]   = 4Q[1] + 2Q[2]
///  ...
///    P1[i-1] + 4P1[i]   +    P1[i+1] = 4Q[i-1] + 2Q[i]
///  ...
///    P1[n-2] + 4P1[n-1] +    P1[n]   = 4Q[n-2] + 2Q[n-1]
///               P1[n-1] + 3.5P1[n]   = (8Q[n-1] + Q[n]) / 2
///  
/// From this set of equations, P1[1..n] are easy but tedious to solve.
/// </remarks>
	public static class BezierSpline
{
    /// <summary>
    /// Get open-ended Bezier Spline Control Points.
    /// </summary>
    /// <param name="Knots">Input Knot Bezier spline points.</param>
    /// <param name="FirstControlPoints">Output First Control points array of knots.Length - 1 length.</param>
    /// <param name="SecondControlPoints">Output Second Control points array of knots.Length - 1 length.</param>
    /// <exception cref="ArgumentNullException"><paramref name="Knots"/> parameter must be not null.</exception>
    /// <exception cref="ArgumentException"><paramref name="Knots"/> array must containg at least two points.</exception>
    public static void GetCurveControlPoints(Vector2[] Knots, out Vector2[] FirstControlPoints, out Vector2[] SecondControlPoints)
    {
        if (Knots == null)
            throw new ArgumentNullException("Knots");
        int t_N = Knots.Length - 1;
        if (t_N < 1)
            throw new ArgumentException("At least two knot points required", "Knots");
        if (t_N == 1)
        { // Special case: Bezier curve should be a straight line.
            FirstControlPoints = new Vector2[1];
            // 3P1 = 2P0 + P3
            FirstControlPoints[0] = new Vector2((2 * Knots[0].X + Knots[1].X) / 3, (2 * Knots[0].Y + Knots[1].Y) / 3);

            SecondControlPoints = new Vector2[1];
            // P2 = 2P1 – P0
            SecondControlPoints[0] = new Vector2(2 * FirstControlPoints[0].X - Knots[0].X, 2 * FirstControlPoints[0].Y - Knots[0].Y);
            return;
        }

        // Calculate first Bezier control points
        // Right hand side vector
        float[] t_Rhs = new float[t_N];

        // Set right hand side X values
        for (int t_I = 1; t_I < t_N - 1; ++t_I)
            t_Rhs[t_I] = 4 * Knots[t_I].X + 2 * Knots[t_I + 1].X;
        t_Rhs[0] = Knots[0].X + 2 * Knots[1].X;
        t_Rhs[t_N - 1] = (8 * Knots[t_N - 1].X + Knots[t_N].X) / 2.0f;
        // Get first control points X-values
        float[] t_X = GetFirstControlPoints(t_Rhs);

        // Set right hand side Y values
        for (int t_I = 1; t_I < t_N - 1; ++t_I)
            t_Rhs[t_I] = 4 * Knots[t_I].Y + 2 * Knots[t_I + 1].Y;
        t_Rhs[0] = Knots[0].Y + 2 * Knots[1].Y;
        t_Rhs[t_N - 1] = (8 * Knots[t_N - 1].Y + Knots[t_N].Y) / 2.0f;
        // Get first control points Y-values
        float[] t_Y = GetFirstControlPoints(t_Rhs);

        // Fill output arrays.
        FirstControlPoints = new Vector2[t_N];
        SecondControlPoints = new Vector2[t_N];
        for (int t_I = 0; t_I < t_N; ++t_I)
        {
            // First control point
            FirstControlPoints[t_I] = new Vector2(t_X[t_I], t_Y[t_I]);
            // Second control point
            if (t_I < t_N - 1)
                SecondControlPoints[t_I] = new Vector2(2 * Knots[t_I + 1].X - t_X[t_I + 1], 2 * Knots[t_I + 1].Y - t_Y[t_I + 1]);
            else
                SecondControlPoints[t_I] = new Vector2((Knots[t_N].X + t_X[t_N - 1]) / 2, (Knots[t_N].Y + t_Y[t_N - 1]) / 2);
        }
    }

    /// <summary>
    /// Solves a tridiagonal system for one of coordinates (x or y) of first Bezier control points.
    /// </summary>
    /// <param name="Rhs">Right hand side vector.</param>
    /// <returns>Solution vector.</returns>
    private static float[] GetFirstControlPoints(float[] Rhs)
    {
        int t_N = Rhs.Length;
        float[] t_X = new float[t_N]; // Solution vector.
        float[] t_Tmp = new float[t_N]; // Temp workspace.

        float t_B = 2.0f;
        t_X[0] = Rhs[0] / t_B;
        for (int t_I = 1; t_I < t_N; t_I++) // Decomposition and forward substitution.
        {
            t_Tmp[t_I] = 1 / t_B;
            t_B = (t_I < t_N - 1 ? 4.0f : 3.5f) - t_Tmp[t_I];
            t_X[t_I] = (Rhs[t_I] - t_X[t_I - 1]) / t_B;
        }
        for (int t_I = 1; t_I < t_N; t_I++)
            t_X[t_N - t_I - 1] -= t_Tmp[t_N - t_I] * t_X[t_N - t_I]; // Backsubstitution.

        return t_X;
    }
}
