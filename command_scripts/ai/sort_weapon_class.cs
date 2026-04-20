using (var stream = Cache.OpenCacheReadWrite())
{
    if (Args.Count != 2)
    {
        Console.WriteLine("YOU SHOULD BE PASSING AN ANIMATION GRAPH TAG AND WEAPON CLASS FILTERS STRING");
        return false;
    }

    CachedTag jmad_tag = null;
    string[] class_filters = Args[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

    if (!Cache.TagCache.TryGetTag(Args[0], out CachedTag out_tag))
    {
        Console.WriteLine("COULDN'T FIND RENDER ANIMATION GRAPH TAG OR INVALID INPUT");
        return false;
    }
    else
    {
        jmad_tag = out_tag;
    }

    var jmad_def = Cache.Deserialize<TagTool.Tags.Definitions.ModelAnimationGraph>(stream, jmad_tag);

    if (jmad_def.Modes.Count == 0)
    {
        Console.WriteLine("THIS ANIMATION GRAPH TAG HAS NO MODES!");
        return false;
    }

    int counter = 0;

    foreach (var mode in jmad_def.Modes)
    {
        foreach (var wpnclass in mode.WeaponClass)
        {
            string class_name = Cache.StringTable.GetString(wpnclass.Label);

            if (class_filters.Length > 0)
            {
                bool isMatch = class_filters.Any(f => f.Trim().Equals(class_name, StringComparison.OrdinalIgnoreCase));

                if (!isMatch) continue;
            }

            foreach (var wpntype in wpnclass.WeaponType)
            {
                wpntype.Set.Overlays.Sort((a, b) => a.Label.CompareTo(b.Label));
            }

            counter++;
        }
    }

    Console.WriteLine("SORTED {0} WEAPON TYPES", counter);

    Cache.SaveStrings();
    Cache.Serialize(stream, jmad_tag, jmad_def);

    return true;
}