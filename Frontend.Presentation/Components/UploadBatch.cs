namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

public sealed record UploadAttempt(int Index, string FileName, int Order, bool Succeeded, string? Error);

/// <summary>
/// Ejecuta la carga de una selección ya planificada. Una llamada por archivo.
/// Un archivo que falla no cancela a los demás.
/// </summary>
public static class UploadBatch
{
    public static async Task<IReadOnlyList<UploadAttempt>> RunAsync(
        IReadOnlyList<PlannedUpload> planned,
        Func<PlannedUpload, Task> upload)
    {
        var results = new List<UploadAttempt>();
        foreach (var item in planned)
        {
            try
            {
                await upload(item);
                results.Add(new UploadAttempt(item.Index, item.FileName, item.Order, true, null));
            }
            catch (Exception ex)
            {
                results.Add(new UploadAttempt(item.Index, item.FileName, item.Order, false, ex.Message));
            }
        }

        return results;
    }
}