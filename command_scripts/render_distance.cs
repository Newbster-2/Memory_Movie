// Args[0] = threshold value (float)  
// Args[1] = comparison type: gt, gte, lt, lte  
//           TRUE  = BoundingRadius satisfies condition -> skip  
//           FALSE = BoundingRadius does NOT satisfy condition -> set hlmt distances  
// Args[2] = new distance value (float) to assign to DisappearDistance and BeginFadeDistance  
  
if (Args.Count < 3)  
{  
    Console.WriteLine("Usage: CS set_hlmt_distances.cs <threshold> <compType> <newDist>");  
    Console.WriteLine("  compType: gt, gte, lt, lte");  
    return;  
}  
  
float threshold = float.Parse(Args[0]);  
string compType = Args[1].ToLower();  
float newDist   = float.Parse(Args[2]);  
  
bool PassesComparison(float radius) => compType switch  
{  
    "gt"  => radius > threshold,  
    "gte" => radius >= threshold,  
    "lt"  => radius < threshold,  
    "lte" => radius <= threshold,  
    _     => throw new ArgumentException($"Unknown comparison type '{compType}'. Use: gt, gte, lt, lte")  
};  
  
// All concrete object-type group tags (Gen3 / HaloOnline)  
// Each is deserialized as its registered type (Biped, Vehicle, etc.)  
// and cast to the common base class GameObject.  
string[] objectGroups =  
{  
    "bipd",  // Biped    : Unit : GameObject  
    "vehi",  // Vehicle  : Unit : GameObject  
    "weap",  // Weapon   : Item : GameObject  
    "eqip",  // Equipment: Item : GameObject  
    "scen",  // Scenery  : GameObject  
    "bloc",  // Crate    : GameObject  
    "crea",  // Creature : GameObject  
    "gint",  // Giant    : Unit : GameObject  
    "efsc",  // EffectScenery : GameObject  
    "mach",  // DeviceMachine : Device : GameObject  
    "ctrl",  // DeviceControl : Device : GameObject  
};  
  
// Track modified hlmt indices to avoid redundant re-serialization  
// when multiple object tags share the same hlmt.  
var modifiedHlmts = new HashSet<int>();  
int skipped = 0, modified = 0;  
  
using (var stream = Cache.OpenCacheReadWrite())  
{  
    foreach (var groupTag in objectGroups)  
    {  
        foreach (var tag in Cache.TagCache.FindAllInGroup(new Tag(groupTag)))  
        {  
            // Deserialize using the auto-detected registered type for this group.  
            // Safe to cast to GameObject because all object types inherit from it.  
            var objDef = (GameObject)Cache.Deserialize(stream, tag);  
  
            if (PassesComparison(objDef.BoundingRadius))  
            {  
                // Condition is TRUE -> ignore  
                skipped++;  
                continue;  
            }  
  
            // Condition is FALSE -> set hlmt distances (if a model is linked)  
            if (objDef.Model == null)  
                continue;  
  
            if (modifiedHlmts.Contains(objDef.Model.Index))  
                continue; // already updated this hlmt from another object tag  
  
            var hlmtDef = Cache.Deserialize<Model>(stream, objDef.Model);  
            hlmtDef.DisappearDistance = newDist;  
            hlmtDef.BeginFadeDistance = newDist;  
            Cache.Serialize(stream, objDef.Model, hlmtDef);  
  
            modifiedHlmts.Add(objDef.Model.Index);  
            modified++;  
  
            Console.WriteLine(  
                $"[{tag.Group}] {tag.Name ?? $"0x{tag.Index:X4}"} " +  
                $"BoundingRadius={objDef.BoundingRadius:F4} -> " +  
                $"Set hlmt distances on: {objDef.Model.Name ?? $"0x{objDef.Model.Index:X4}"}");  
        }  
    }  
}  
  
Console.WriteLine($"\nDone. {modified} hlmt(s) updated, {skipped} object(s) skipped.");