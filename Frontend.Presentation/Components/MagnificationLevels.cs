namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

/// <summary>
/// Niveles de aumento del visor, como los objetivos de un microscopio de laboratorio.
/// El primero muestra la foto completa; cada nivel la agranda en proporción a su aumento.
/// </summary>
public static class MagnificationLevels
{
    public static IReadOnlyList<int> Objectives { get; } = [4, 10, 40, 100];

    public static int Count => Objectives.Count;

    public static string Label(int level) => $"{Objectives[Clamp(level)]}x";

    /// <summary>Veces que se agranda la foto respecto a la vista completa.</summary>
    public static double Scale(int level) => (double)Objectives[Clamp(level)] / Objectives[0];

    public static int Clamp(int level) => Math.Clamp(level, 0, Objectives.Count - 1);
}
