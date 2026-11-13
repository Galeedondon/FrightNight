namespace MyAntDesignApp.CommonService;

public static class AvatarCatalog
{
    // Explicit allowlist avoids case-sensitive filesystem misses and arbitrary path construction.
    private static readonly Dictionary<string, string> Originals = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Andy"] = "Andy", ["Chu"] = "Chu", ["Cindy"] = "Cindy", ["Eileen"] = "Eileen",
        ["Jane"] = "Jane", ["Jerry"] = "Jerry", ["Ray"] = "Ray", ["Rocky"] = "Rocky",
        ["Rona"] = "Rona", ["Sandy"] = "Sandy", ["Sharon"] = "Sharon"
    };
    public static string Image(string account)
    {
        var key = account.Trim();
        if (Originals.TryGetValue(key, out var name)) return $"/img/avatar/{name}.png";
        // A stable pseudo-random assignment means a new member keeps the same face on every device.
        uint hash = 2166136261;
        foreach (var character in key.ToLowerInvariant()) hash = unchecked((hash ^ character) * 16777619);
        return $"/img/avatar/generated/portrait-{hash % 4 + 1}.png";
    }
    public static bool IsOriginal(string account) => Originals.ContainsKey(account.Trim());
}
