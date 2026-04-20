using TagTool.Commands.Common;
using TagTool.Tags.Definitions.Common;

using (var stream = Cache.OpenCacheReadWrite())
{
    // Is there enough arguments?
    if (Args.Count != 6)
    {
        Console.WriteLine("TOO MANY OR TOO FEW ARGUMENTS PASSED IN");
        return false;
    }

    // This must exist first because of a try statement moving it out of scope
	// Order is: Crates, Scenery, Vehicles, Bipeds, Characters/Spawnpoints
    short[] script_objects = { 0, 0, 0, 0 };
	short spawn_points = 0;

    // Are the arguments after the tag name numerical?
    try
    {
        script_objects[0] = Int16.Parse(Args[1]);
        script_objects[1] = Int16.Parse(Args[2]);
        script_objects[2] = Int16.Parse(Args[3]);
        script_objects[3] = Int16.Parse(Args[4]);
		spawn_points = Int16.Parse(Args[5]);
    }
    catch (FormatException error)
    {
        Console.WriteLine(error.Message);
        return false;
    }

    // Which scenario tag do we want to perform this on?
    CachedTag scnr_tag = null;

    if (!Cache.TagCache.TryGetTag(Args[0], out CachedTag out_tag))
    {
        Console.WriteLine("COULDN'T FIND SCENARIO TAG OR INVALID INPUT");
        return false;
    }
    else
    {
        scnr_tag = out_tag;
    }

    // Access the tag's contents
    var scnr_def = Cache.Deserialize<TagTool.Tags.Definitions.Scenario>(stream, scnr_tag);

    // Make sure we have a list for each of these block fields
    if (scnr_def.ObjectNames.Count == 0)
        scnr_def.ObjectNames = new List<Scenario.ObjectName>();

    if (scnr_def.Crates.Count == 0)
        scnr_def.Crates = new List<Scenario.CrateInstance>();

    if (scnr_def.Scenery.Count == 0)
        scnr_def.Scenery = new List<Scenario.SceneryInstance>();

    if (scnr_def.Bipeds.Count == 0)
        scnr_def.Bipeds = new List<Scenario.BipedInstance>();

    if (scnr_def.Vehicles.Count == 0)
        scnr_def.Vehicles = new List<Scenario.VehicleInstance>();

    if (scnr_def.TriggerVolumes.Count == 0)
        scnr_def.TriggerVolumes = new List<Scenario.TriggerVolume>();

    if (scnr_def.ObjectReferenceFrames.Count == 0)
        scnr_def.ObjectReferenceFrames = new List<Scenario.ReferenceFrame>();

    if (scnr_def.Squads.Count == 0)
        scnr_def.Squads = new List<Scenario.Squad>();

    if (scnr_def.ScriptingData.Count == 0)
        scnr_def.ScriptingData = new List<Scenario.ScriptingDatum>();

    scnr_def.ScriptingData.Add(new Scenario.ScriptingDatum());

    scnr_def.ScriptingData[0].PointSets = new List<Scenario.ScriptingDatum.PointSet>();

    string[] point_set_names = { "cset", "sset", "vset", "bset" };

    for (short i = 0; i < point_set_names.Length; i++)
    {
        scnr_def.ScriptingData[0].PointSets.Add
        (
            new Scenario.ScriptingDatum.PointSet
            {
                Name = point_set_names[i],
                BspIndex = -1,
                ManualReferenceFrame = -1,
                EditorFolderIndex = -1
            }
        );
        scnr_def.ScriptingData[0].PointSets[i].Points = new List<Scenario.ScriptingDatum.PointSet.Point>();
    }

    // Keep track of current overall object index
    short current_object_index = 0;

    string[] object_name_prefixes = { "bloc", "scen", "vehi", "bipd" };
    string[] object_types = { "Crate", "Scenery", "Vehicle", "Biped" };

    for (short current_type = 0; current_type < object_types.Length; current_type++)
    {
        for (short i = 0; i < script_objects[current_type]; i++)
        {
            string current_object_name = object_name_prefixes[current_type] + i.ToString();

            scnr_def.ObjectNames.Add
            (
                new Scenario.ObjectName
                {
                    Name = current_object_name,
                    ObjectType = new GameObjectType16()
                    {
                        Halo3ODST = (GameObjectTypeHalo3ODST)Enum.Parse(typeof(GameObjectTypeHalo3ODST), object_types[current_type])
                    },
                    PlacementIndex = i
                }
            );

            scnr_def.TriggerVolumes.Add
            (
                new Scenario.TriggerVolume
                {
                    Name = Cache.StringTable.GetOrAddString(current_object_name),
                    ObjectName = current_object_index,
                    NodeName = Cache.StringTable.GetOrAddString("root"),
                    Forward = new RealVector3d(1, 0, 0),
                    Up = new RealVector3d(0, 0, 1),
                    Extents = object_types[current_type] switch
                    {
                        "Scenery" => new RealPoint3d(3, 3, 3),
                        _ => new RealPoint3d(0, 0, 0)
                    },
                    Position = object_types[current_type] switch
                    {
                        "Scenery" => new RealPoint3d(-1.5f, -1.5f, -1.5f),
                        _ => new RealPoint3d(0, 0, 0)
                    },
					C = object_types[current_type] switch
					{
						"Scenery" => (float)((Math.Sqrt(Math.Pow(3, 2) + Math.Pow(3, 2) + Math.Pow(3, 2))) / 2), // A sphere that encapsulates the rectangular prism (cuboid)
						_ => 0
					},
                    KillVolume = -1,
                    EditorFolderIndex = -1
                }
            );

            scnr_def.ScriptingData[0].PointSets[current_type].Points.Add
            (
                new Scenario.ScriptingDatum.PointSet.Point
                {
                    Name = "p" + i.ToString(),
                    Position = new RealPoint3d(0.1f, 0.1f, 0.1f),
                    ReferenceFrame = current_object_index,
                    BspIndex = -1,
                    ZoneIndex = -1
                }
            );

            scnr_def.ObjectReferenceFrames.Add
            (
                new Scenario.ReferenceFrame
                {
                    ObjectId = new ObjectIdentifier
                    {
                        OriginBspIndex = -1,
                        UniqueId = new DatumHandle((ushort)(40000 + current_object_index), (ushort)current_object_index),
                        Type = new GameObjectType8()
                        { 
                            Halo3ODST = (GameObjectTypeHalo3ODST)Enum.Parse(typeof(GameObjectTypeHalo3ODST), object_types[current_type]) 
                        },
                        Source = ObjectIdentifier.SourceValue.Editor
                    },
                    ProjectionAxis = 2
                }
            );

            switch (object_types[current_type])
            {
                case "Crate":
                    crate_add(current_object_index, i, scnr_def);
                    break;
                case "Scenery":
                    scenery_add(current_object_index, i, scnr_def);
                    break;
                case "Vehicle":
                    vehicle_add(current_object_index, i, scnr_def);
                    break;
                case "Biped":
                    biped_add(current_object_index, i, scnr_def);
                    break;
                default:
                    break;
            }

            current_object_index++;
        }
    }

    // Save new strings ids
    Cache.SaveStrings();

    // File, Tag Location, Tag Data
    Cache.Serialize(stream, scnr_tag, scnr_def);

    return true;

    void crate_add(short current_object_index, short i, Scenario scenario)
    {
        scnr_def.Crates.Add
        (
            new Scenario.CrateInstance
            {
                NameIndex = current_object_index,
                Position = new RealPoint3d((float)i / 10f, 0, 0),
                ObjectId = new ObjectIdentifier
                {
                    UniqueId = new DatumHandle((ushort)(40000 + current_object_index), (ushort)current_object_index),
                    OriginBspIndex = -1,
                    Type = new GameObjectType8() { Halo3ODST = GameObjectTypeHalo3ODST.Crate },
                    Source = ObjectIdentifier.SourceValue.Editor
                },
                EditingBoundToBsp = -1,
                EditorFolder = -1,
                ParentId = new ScenarioObjectParentStruct
                {
                    NameIndex = -1
                },
                CanAttachToBspFlags = 1,
                Multiplayer = new Scenario.MultiplayerObjectProperties
                {
                    Team = MultiplayerTeamDesignator.Neutral,
                    MapVariantParent = new ScenarioObjectParentStruct
                    {
                        NameIndex = -1
                    }
                }
            }
        );

        scnr_def.Squads.Add
        (
            new Scenario.Squad
            {
                Name = "sq" + i.ToString(),
                Flags = Scenario.SquadFlags.Deaf | Scenario.SquadFlags.Blind,
                Team = GameTeam.Player,
                ParentIndex = -1,
                InitialZoneIndex = -1,
                InitialObjectiveIndex = -1,
                InitialTaskIndex = -1,
                EditorFolderIndex = -1,
            }
        );
		
		scnr_def.Squads[i].SpawnPoints = new List<Scenario.Squad.SpawnPoint>();
		scnr_def.Squads[i].DesignerFireteams = new List<Scenario.Squad.Fireteam>();
		
		// Change for how many characters you want to use, don't forget to spawn the individual units by doing: squad/spawnpoint in the halo script
		for (short j = 0; j < spawn_points; j++)
		{
			scnr_def.Squads[i].SpawnPoints.Add
			(
				new Scenario.Squad.SpawnPoint
				{
					Name = Cache.StringTable.GetOrAddString("sp" + j.ToString()),
					FireteamIndex = j,
					Point = new Scenario.AiPoint3d
					{
						ReferenceFrame = current_object_index,
						BspIndex = -1
					},
					CharacterTypeIndex = -1,
					InitialPrimaryWeaponIndex = -1,
					InitialSecondaryWeaponIndex = -1,
					InitialEquipmentIndexNew = -1,
					VehicleTypeIndex = -1,
					EmitterVehicleIndex = -1,
					EmitterBipedIndex = -1,
					EmitterGiantIndex = -1,
					PlacementScriptIndex = -1,
					PointSetIndex = -1
				}
			);
	
			scnr_def.Squads[i].DesignerFireteams.Add
			(
				new Scenario.Squad.Fireteam
				{
					SpawnCount = 1,
					MajorUpgrade = -1,
					VehicleTypeIndex = -1,
					CommandScriptIndex = -1,
					PointSetIndex = -1
				}
			);
	
			scnr_def.Squads[i].DesignerFireteams[j].CharacterType = new List<Scenario.Squad.Fireteam.CharacterTypeBlock>();
			scnr_def.Squads[i].DesignerFireteams[j].CharacterType.Add
			(
				new Scenario.Squad.Fireteam.CharacterTypeBlock
				{
					CharacterTypeIndex = j,
					Chance = 1
				}
			);
		}
    }

    void scenery_add(short current_object_index, short i, Scenario scenario)
    {
        scnr_def.Scenery.Add
        (
            new Scenario.SceneryInstance
            {
                NameIndex = current_object_index,
                Position = new RealPoint3d((float)i / 10f, 1, 0),
                ObjectId = new ObjectIdentifier
                {
                    UniqueId = new DatumHandle((ushort)(40000 + current_object_index), (ushort)current_object_index),
                    OriginBspIndex = -1,
                    Type = new GameObjectType8() { Halo3ODST = GameObjectTypeHalo3ODST.Scenery },
                    Source = ObjectIdentifier.SourceValue.Editor
                },
                EditingBoundToBsp = -1,
                EditorFolder = -1,
                ParentId = new ScenarioObjectParentStruct
                {
                    NameIndex = -1
                },
                CanAttachToBspFlags = 1,
                HavokMoppIndex = -1,
                AiSpawningSquad = -1,
                Multiplayer = new Scenario.MultiplayerObjectProperties
                {
                    Team = MultiplayerTeamDesignator.Neutral,
                    MapVariantParent = new ScenarioObjectParentStruct
                    {
                        NameIndex = -1
                    }
                }
            }
        );
    }

    void vehicle_add(short current_object_index, short i, Scenario scenario)
    {
        scnr_def.Vehicles.Add
        (
            new Scenario.VehicleInstance
            {
                NameIndex = current_object_index,
                Position = new RealPoint3d((float)i / 10f, 2, 0),
                ObjectId = new ObjectIdentifier
                {
                    UniqueId = new DatumHandle((ushort)(40000 + current_object_index), (ushort)current_object_index),
                    OriginBspIndex = -1,
                    Type = new GameObjectType8() { Halo3ODST = GameObjectTypeHalo3ODST.Vehicle },
                    Source = ObjectIdentifier.SourceValue.Editor
                },
                EditingBoundToBsp = -1,
                EditorFolder = -1,
                ParentId = new ScenarioObjectParentStruct
                {
                    NameIndex = -1
                },
                CanAttachToBspFlags = 1,
                Multiplayer = new Scenario.MultiplayerObjectProperties
                {
                    Team = MultiplayerTeamDesignator.Neutral,
                    MapVariantParent = new ScenarioObjectParentStruct
                    {
                        NameIndex = -1
                    }
                }
            }
        );
    }

    void biped_add(short current_object_index, short i, Scenario scenario)
    {
        scnr_def.Bipeds.Add
        (
            new Scenario.BipedInstance
            {
                NameIndex = current_object_index,
                Position = new RealPoint3d((float)i / 10f, 3, 0),
                ObjectId = new ObjectIdentifier
                {
                    UniqueId = new DatumHandle((ushort)(40000 + current_object_index), (ushort)current_object_index),
                    OriginBspIndex = -1,
                    Type = new GameObjectType8() { Halo3ODST = GameObjectTypeHalo3ODST.Biped },
                    Source = ObjectIdentifier.SourceValue.Editor
                },
                EditingBoundToBsp = -1,
                EditorFolder = -1,
                ParentId = new ScenarioObjectParentStruct
                {
                    NameIndex = -1
                },
                CanAttachToBspFlags = 1
            }
        );
    }


}