using (var stream = Cache.OpenCacheReadWrite())
{
    if (Args.Count < 3 || Args.Count > 4)
    {
        Console.WriteLine("YOU SHOULD BE PASSING AN ANIMATION GRAPH TAG, MATCH STRING, AND REPLACE STRING! (OPTIONAL WEAPON CLASS FILTERS SEPERATED BY COMMAS)");
        return false;
    }

    CachedTag jmad_tag = null;
    string match = Args[1];
    string replace = Args[2];
    byte flags = 0;

    bool invert_filters = false;
    string filter_arg = Args.Count == 4 ? Args[3] : string.Empty;

    if (filter_arg.StartsWith("!"))
    {
        invert_filters = true;
        filter_arg = filter_arg.Substring(1); // Remove the '!' for parsing
    }

    string[] class_filters = filter_arg.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

    if (match.Contains('^')) flags |= 0b_100;
    if (match.EndsWith('*')) flags |= 0b_010;
    if (match.StartsWith('*')) flags |= 0b_001;

    match = match.Trim('^', '*');

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

                if (invert_filters ? isMatch : !isMatch)
                    continue;
            }

            foreach (var wpntype in wpnclass.WeaponType)
            {
                foreach (var overlay in wpntype.Set.Overlays)
                {   
                    if (overlay.Label == StringId.Empty || overlay.Label == StringId.Invalid)
                    {
                        continue;
                    }

                    string label = Cache.StringTable.GetString(overlay.Label);

                    bool hit = (flags & 0b_011) switch
                    {
                        0b_010 => label.StartsWith(match, StringComparison.OrdinalIgnoreCase),
                        0b_001 => label.EndsWith(match, StringComparison.OrdinalIgnoreCase),
                        _ => label.Contains(match, StringComparison.OrdinalIgnoreCase)
                    };

                    if (hit)
                    {
                        if ((flags & 0b_100) != 0)
                        {
                            label = label.Replace(match, replace, StringComparison.OrdinalIgnoreCase);
                            overlay.Label = Cache.StringTable.GetOrAddString(label);
                        }
                        else
                        {
                            overlay.Label = Cache.StringTable.GetOrAddString(replace);
                        }
                        counter++;
                        break; // Since this was only to fix ai melee, there ain't much point in changing all of labels that have melee in them
                    }
                }
            }
        }
    }

    Console.WriteLine("REPLACED {0} LABELS", counter);

    Cache.SaveStrings();
    Cache.Serialize(stream, jmad_tag, jmad_def);

    return true;
}