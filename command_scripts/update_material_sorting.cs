using TagTool.Commands.Common;
using TagTool.Tags.Definitions.Common;

using (var stream = Cache.OpenCacheReadWrite())
{
    if (Args.Count != 1)
    {
        Console.WriteLine("YOU SHOULD ONLY BE PASSING A SKY BOX RENDER MODEL TAG!");
        return false;
    }

    CachedTag mode_tag = null;

    if (!Cache.TagCache.TryGetTag(Args[0], out CachedTag out_tag))
    {
        Console.WriteLine("COULDN'T FIND RENDER MODEL TAG OR INVALID INPUT");
        return false;
    }
    else
    {
        mode_tag = out_tag;
    }

    var mode_def = Cache.Deserialize<TagTool.Tags.Definitions.RenderModel>(stream, mode_tag);

    if (mode_def.Materials.Count == 0)
    {
        Console.WriteLine("THIS RENDER MODEL TAG HAS NO MATERIALS!");
        return false;
    }

    foreach (var material in mode_def.Materials)  
	{  
		if (material.RenderMethod != null)  
		{  
			// Create the specific shader type instance  
			var shaderType = Cache.TagCache.TagDefinitions.GetTagDefinitionType(material.RenderMethod.Group);  
			var shader_definition = (RenderMethod)Activator.CreateInstance(shaderType);  
			
			// Deserialize to the specific type  
			shader_definition = (RenderMethod)Cache.Deserialize(stream, material.RenderMethod, shader_definition);  
			
			// Set the common property  
			shader_definition.SortLayer = TagTool.Shaders.SortingLayerValue.PrePass;  
			
			// Serialize back  
			Cache.Serialize(stream, material.RenderMethod, shader_definition);  
		}  
	}
	
    return true;
}