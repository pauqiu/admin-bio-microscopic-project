namespace UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;

public sealed record HeldPhotoPlacement(int Id, int? Stack, int? Position);

/// <summary>
/// Autoridad del armado de pilas. La posición 1 es la foto más lejana.
/// </summary>
public static class HeldPhotoLayout
{
    public const int MaxPerStack = 6;
    public const long MaxFileBytes = 10L * 1024 * 1024;

    public static IReadOnlyList<string> Validate(IReadOnlyList<HeldPhotoPlacement> photos)
    {
        var errors = new List<string>();
        var ids = photos.Select(photo => photo.Id).ToList();
        if (ids.Distinct().Count() != ids.Count)
            errors.Add("Una foto está repetida.");

        foreach (var photo in photos)
        {
            var hasStack = photo.Stack is not null;
            var hasPosition = photo.Position is not null;
            if (hasStack != hasPosition)
                errors.Add($"La foto {photo.Id} necesita pila y posición juntas.");
            if (photo.Position is int position && (position < 1 || position > MaxPerStack))
                errors.Add($"La foto {photo.Id} tiene una posición fuera de 1 a {MaxPerStack}.");
        }

        foreach (var group in photos.Where(photo => photo.Stack is not null && photo.Position is not null).GroupBy(photo => photo.Stack))
        {
            var positions = group.Select(photo => photo.Position!.Value).ToList();
            if (positions.Count > MaxPerStack)
                errors.Add($"La pila {group.Key} tiene {positions.Count} fotos. El máximo es {MaxPerStack}.");
            if (positions.Distinct().Count() != positions.Count)
                errors.Add($"La pila {group.Key} repite una posición.");
            var expected = Enumerable.Range(1, positions.Count).ToHashSet();
            if (!expected.SetEquals(positions))
                errors.Add($"La pila {group.Key} debe usar las posiciones 1 a {positions.Count} sin huecos.");
        }

        return errors;
    }
}