namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

/// <summary>
/// Reglas del zoom del visor. Cada foto de una lámina se tomó con un aumento distinto y el orden
/// va de la más lejana (índice 0) a la más cercana. Acercar y alejar pasan entre esas fotos.
/// </summary>
public static class ZoomSteps
{
    /// <summary>Cuánto se agranda una foto al dar paso a la siguiente.</summary>
    public const double TransitionScale = 2.5;

    public static int Clamp(int index, int count) => count <= 0 ? 0 : Math.Clamp(index, 0, count - 1);

    public static bool CanZoomIn(int index, int count) => index < count - 1;

    public static bool CanZoomOut(int index) => index > 0;

    /// <summary>
    /// Escala de una capa según su distancia a la foto actual. Las fotos ya pasadas (más lejanas)
    /// quedan agrandadas, como si se hubiera entrado en ellas; las que siguen (más cercanas)
    /// esperan reducidas, para crecer al acercar.
    /// </summary>
    public static double LayerScale(int offset) => offset switch
    {
        0 => 1,
        < 0 => TransitionScale,
        _ => 1 / TransitionScale
    };

    public static string Describe(int index, int count)
    {
        if (count <= 1) return "Una sola foto";
        var position = index == 0 ? " · la más lejana" : index == count - 1 ? " · la más cercana" : "";
        return $"Aumento {index + 1} de {count}{position}";
    }
}
