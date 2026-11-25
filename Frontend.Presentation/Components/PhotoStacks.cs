namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

public sealed record StackPlacement(int Id, int? Stack, int? Position);

/// <summary>Foto que sí se guardó, con el índice que tenía en la selección.</summary>
public sealed record UploadedSelection(int Id, int SelectionIndex);

/// <summary>
/// Una pila será una lámina. La posición 1 es la foto más lejana.
/// Una foto sin pila tiene pila y posición nulas. Las dos van juntas.
/// </summary>
public static class PhotoStacks
{
    public const int MaxPerStack = 6;

    /// <summary>
    /// Arma pilas sin correr las fotos hacia adelante cuando falta un índice de la selección.
    /// La pila es <paramref name="firstPile"/> + índice / 6. Dentro de cada pila las posiciones son 1..n, sin huecos.
    /// </summary>
    public static IReadOnlyList<StackPlacement> Pack(IReadOnlyList<UploadedSelection> uploaded, int firstPile) =>
        uploaded
            .GroupBy(photo => firstPile + photo.SelectionIndex / MaxPerStack)
            .OrderBy(group => group.Key)
            .SelectMany(group => group
                .OrderBy(photo => photo.SelectionIndex)
                .Select((photo, index) => new StackPlacement(photo.Id, group.Key, index + 1)))
            .ToList();

    public static IReadOnlyList<string> Validate(IReadOnlyList<StackPlacement> photos)
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