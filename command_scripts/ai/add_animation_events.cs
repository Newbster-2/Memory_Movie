using (var stream = Cache.OpenCacheReadWrite())
{
    if (Args.Count != 4)
    {
        Console.WriteLine("YOU SHOULD BE PASSING AN ANIMATION GRAPH TAG, EFFECT TAG, EFFECT MARKER, AND LABEL FILTERS STRING");
        return false;
    }

    CachedTag jmad_tag = null;
    CachedTag effe_tag = null;
    string effe_marker = Args[2];
    string[] tar_labels = Args[3].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

    if (!Cache.TagCache.TryGetTag(Args[0], out CachedTag out_tag))
    {
        Console.WriteLine("COULDN'T FIND RENDER ANIMATION GRAPH TAG OR INVALID INPUT");
        return false;
    }
    else
    {
        jmad_tag = out_tag;
    }

    if (!Cache.TagCache.TryGetTag(Args[1], out CachedTag out_tag2))
    {
        Console.WriteLine("COULDN'T FIND EFFECT TAG OR INVALID INPUT");
        return false;
    }
    else
    {
        effe_tag = out_tag2;
    }

    var jmad_def = Cache.Deserialize<TagTool.Tags.Definitions.ModelAnimationGraph>(stream, jmad_tag);

    if (jmad_def.Modes.Count == 0 || jmad_def.Animations.Count == 0)
    {
        Console.WriteLine("THIS ANIMATION GRAPH TAG HAS NO MODES OR NO ANIMATIONS!");
        return false;
    }

    if (jmad_def.EffectReferences.Count == 0)
        jmad_def.EffectReferences = new List<ModelAnimationGraph.AnimationTagReference>();

    jmad_def.EffectReferences.Add
    (
        new ModelAnimationGraph.AnimationTagReference
        {
            Reference = effe_tag
        }
    );

    int counter = 0;
    var processedAnimations = new HashSet<int>();

    foreach (var mode in jmad_def.Modes)
    {
        foreach (var wpnclass in mode.WeaponClass)
        {
            foreach (var wpntype in wpnclass.WeaponType)
            {
                foreach (var action in wpntype.Set.Actions)
                {
                    if (action.Label == StringId.Empty || action.Label == StringId.Invalid || action.GraphIndex != -1)
                    {
                        continue;
                    }

                    string label = Cache.StringTable.GetString(action.Label);

                    if (tar_labels.Any(f => f.Trim().Equals(label, StringComparison.OrdinalIgnoreCase)) && !processedAnimations.Contains(action.Animation))
                    {
                        if (jmad_def.Animations[action.Animation].AnimationData.EffectEvents.Count == 0)
                            jmad_def.Animations[action.Animation].AnimationData.EffectEvents = new List<ModelAnimationGraph.Animation.EffectEvent>();
                        jmad_def.Animations[action.Animation].AnimationData.EffectEvents.Add
                        (
                            new ModelAnimationGraph.Animation.EffectEvent
                            {
                                Effect = (short)(jmad_def.EffectReferences.Count - 1),
                                MarkerName = Cache.StringTable.GetOrAddString(effe_marker)
                            }
                        );
                        processedAnimations.Add(action.Animation);
                        counter++;
                    }
                }
                foreach (var overlay in wpntype.Set.Overlays)
                {
                    if (overlay.Label == StringId.Empty || overlay.Label == StringId.Invalid || overlay.GraphIndex != -1)
                    {
                        continue;
                    }

                    string label = Cache.StringTable.GetString(overlay.Label);

                    if (tar_labels.Any(f => f.Trim().Equals(label, StringComparison.OrdinalIgnoreCase)) && !processedAnimations.Contains(overlay.Animation))
                    {
                        if (jmad_def.Animations[overlay.Animation].AnimationData.EffectEvents.Count == 0)
                            jmad_def.Animations[overlay.Animation].AnimationData.EffectEvents = new List<ModelAnimationGraph.Animation.EffectEvent>();
                        jmad_def.Animations[overlay.Animation].AnimationData.EffectEvents.Add
                        (
                            new ModelAnimationGraph.Animation.EffectEvent
                            {
                                Effect = (short)(jmad_def.EffectReferences.Count - 1),
                                MarkerName = Cache.StringTable.GetOrAddString(effe_marker)
                            }
                        );
                        processedAnimations.Add(overlay.Animation);
                        counter++;
                    }
                }
            }
        }
    }

    Console.WriteLine("ADDED {0} ANIMATION EFFECT EVENTS", counter);

    Cache.SaveStrings();
    Cache.Serialize(stream, jmad_tag, jmad_def);

    return true;
}