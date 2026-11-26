namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

public sealed record UploadCandidate(int Index, string FileName, long Length);

public sealed record PlannedUpload(int Index, string FileName, int Order);

public sealed record RejectedUpload(int Index, string FileName, string Reason);

public sealed record UploadPlanResult(
    IReadOnlyList<PlannedUpload> Accepted,
    IReadOnlyList<RejectedUpload> Rejected);

/// <summary>
/// Decide, antes de llamar al API, qué fotos de una selección entran en la lámina.
/// El orden de la lista es el orden de la selección. La primera que entra ocupa el aumento libre más bajo.
/// </summary>
public static class UploadPlan
{
    public const int MaxImages = 6;
    public const long MaxFileBytes = 10 * 1024 * 1024;

    public static UploadPlanResult Plan(IReadOnlyCollection<int> usedOrders, IReadOnlyList<UploadCandidate> files)
    {
        var free = Enumerable.Range(1, MaxImages).Where(order => !usedOrders.Contains(order)).ToList();
        var accepted = new List<PlannedUpload>();
        var rejected = new List<RejectedUpload>();

        foreach (var file in files)
        {
            if (file.Length <= 0)
            {
                rejected.Add(new RejectedUpload(file.Index, file.FileName, "El archivo está vacío."));
                continue;
            }

            if (file.Length > MaxFileBytes)
            {
                rejected.Add(new RejectedUpload(file.Index, file.FileName, "Supera 10 MB."));
                continue;
            }

            if (accepted.Count >= free.Count)
            {
                rejected.Add(new RejectedUpload(file.Index, file.FileName, "La lámina ya llega a 6 fotos."));
                continue;
            }

            accepted.Add(new PlannedUpload(file.Index, file.FileName, free[accepted.Count]));
        }

        return new UploadPlanResult(accepted, rejected);
    }

    /// <summary>
    /// Filtro de la sección Carga. No aplica el tope de 6 fotos.
    /// Los aceptados salen con <see cref="PlannedUpload.Order"/> 0 porque todavía no tienen lámina.
    /// </summary>
    public static UploadPlanResult Screen(IReadOnlyList<UploadCandidate> files)
    {
        var accepted = new List<PlannedUpload>();
        var rejected = new List<RejectedUpload>();
        foreach (var file in files)
        {
            if (file.Length <= 0)
                rejected.Add(new RejectedUpload(file.Index, file.FileName, "El archivo está vacío."));
            else if (file.Length > MaxFileBytes)
                rejected.Add(new RejectedUpload(file.Index, file.FileName, "Supera 10 MB."));
            else
                accepted.Add(new PlannedUpload(file.Index, file.FileName, 0));
        }

        return new UploadPlanResult(accepted, rejected);
    }
}