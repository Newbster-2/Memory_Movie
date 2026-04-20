using TagTool.Commands.Common;
using TagTool.Tags.Definitions.Common;

using (var stream = Cache.OpenCacheReadWrite())
{
    if (Args.Count != 5)
    {
        Console.WriteLine("TOO MANY OR TOO FEW ARGUMENTS PASSED IN");
        return false;
    }

    short GridSizeX = 0;
    short GridSizeY = 0;
    float GridSpacingX = 0.0f;
    float GridSpacingY = 0.0f;

    try
    {
        GridSizeX = Int16.Parse(Args[1]);
        GridSizeY = Int16.Parse(Args[2]);
        GridSpacingX = float.Parse(Args[3]);
        GridSpacingY = float.Parse(Args[4]);
    }
    catch (FormatException error)
    {
        Console.WriteLine(error.Message);
        return false;
    }

    CachedTag clwd_tag = null;

    if (!Cache.TagCache.TryGetTag(Args[0], out CachedTag out_tag))
    {
        Console.WriteLine("COULDN'T FIND CLOTH TAG OR INVALID INPUT");
        return false;
    }
    else
    {
        clwd_tag = out_tag;
    }

    var clwd_def = Cache.Deserialize<TagTool.Tags.Definitions.Cloth>(stream, clwd_tag);

    clwd_def.GridXDimension = GridSizeX;
    clwd_def.GridYDimension = GridSizeY;
    clwd_def.GridSpacingX = GridSpacingX;
    clwd_def.GridSpacingY = GridSpacingY;

    generate_vertice_blocks(GridSizeX, GridSizeY, GridSpacingX, GridSpacingY, clwd_def);
    generate_indice_blocks(GridSizeX, GridSizeY, clwd_def);
    generate_link_blocks(GridSizeX, GridSizeY, GridSpacingX, GridSpacingY, clwd_def);

    Cache.Serialize(stream, clwd_tag, clwd_def);

    return true;
}

void generate_vertice_blocks(short GridX, short GridY, float SpacingX, float SpacingY, Cloth cloth)
{
    short GridArea = (short)(GridX * GridY);
    float Uv_I = 1.0f / (GridX - 1);
    float Uv_J = 1.0f / (GridY - 1);

    if (cloth.Vertices.Count == 0)
        cloth.Vertices = new List<Cloth.Vertex>();
    else
        cloth.Vertices.Clear();

    for (short i = 0; i < GridArea; i++)
    {
        cloth.Vertices.Add
        (
            new Cloth.Vertex
            {
                InitialPosition = new RealPoint3d(SpacingX * (i % GridX), 0f, SpacingY * -(i / GridX)),
                Uv = new RealVector2d(Uv_I * (i % GridX), 1 - (Uv_J * (i / GridX)))
            }
        );
    }
}

void generate_indice_blocks(short GridX, short GridY, Cloth cloth)
{
    short RectangleGrid = (short)((GridX - 1) * (GridY - 1));
    short TriangleVertices = (short)(RectangleGrid * 2 * 3);

    if (cloth.Indices.Count == 0)
        cloth.Indices = new List<Cloth.ClothIndex>();
    else
        cloth.Indices.Clear();

    for (short i = 0; i < RectangleGrid; i += GridX)
    {
        for (short j = 0; j < GridX - 1; j++)
        {
            cloth.Indices.Add(new Cloth.ClothIndex { Index = (short)(j + i) });
            cloth.Indices.Add(new Cloth.ClothIndex { Index = (short)(j + i + GridX) });
            cloth.Indices.Add(new Cloth.ClothIndex { Index = (short)(j + i + 1) });

            cloth.Indices.Add(new Cloth.ClothIndex { Index = (short)(j + i + 1) });
            cloth.Indices.Add(new Cloth.ClothIndex { Index = (short)(j + i + GridX) });
            cloth.Indices.Add(new Cloth.ClothIndex { Index = (short)(j + i + GridX + 1) });
        }
    }
}

void generate_link_blocks(short GridX, short GridY, float SpacingX, float SpacingY, Cloth cloth)
{
    short RectangleGrid = (short)((GridX - 1) * (GridY - 1));
    short LinksCount = (short)((RectangleGrid * 4) + ((GridY - 1) * (GridX - 2)) + (GridX - 1) + (GridY - 1));
    float SlopeDistance = (float)Math.Sqrt(Math.Pow(SpacingX, 2) + Math.Pow(SpacingY, 2));
    float SpecialSlopeDistance = (float)Math.Sqrt(Math.Pow(SpacingX * 2, 2) + Math.Pow(SpacingY, 2));

    if (cloth.Links.Count == 0)
        cloth.Links = new List<Cloth.Link>();
    else
        cloth.Links.Clear();

    for (short i = 0; i < RectangleGrid; i += GridX)
    {
        for (short j = 0; j < GridX - 1; j++)
        {
            cloth.Links.Add
            (
                new Cloth.Link
                {
                    Index1 = (short)(j + i),
                    Index2 = (short)(j + i + GridX),
                    DefaultDistance = SpacingY
                }
            );

            cloth.Links.Add
            (
                new Cloth.Link
                {
                    Index1 = (short)(j + i),
                    Index2 = (short)(j + i + 1),
                    DefaultDistance = SpacingX
                }
            );

            cloth.Links.Add
            (
                new Cloth.Link
                {
                    Index1 = (short)(j + i),
                    Index2 = (short)(j + i + GridX + 1),
                    DefaultDistance = SlopeDistance
                }
            );

            cloth.Links.Add
            (
                new Cloth.Link
                {
                    Index1 = (short)(j + i + GridX),
                    Index2 = (short)(j + i + 1),
                    DefaultDistance = SlopeDistance
                }
            );

            if (!(j >= GridX - 2))
                cloth.Links.Add
                (
                    new Cloth.Link
                    {
                        Index1 = (short)(j + i + GridX),
                        Index2 = (short)(j + i + 2),
                        DefaultDistance = SpecialSlopeDistance
                    }
                );

            if (j == GridX - 2)
                cloth.Links.Add
                (
                    new Cloth.Link
                    {
                        Index1 = (short)(j + i + 1),
                        Index2 = (short)(j + i + GridX + 1),
                        DefaultDistance = SpacingY
                    }
                );

            if (i / GridX == GridY - 2)
                cloth.Links.Add
                (
                    new Cloth.Link
                    {
                        Index1 = (short)(j + i + GridX),
                        Index2 = (short)(j + i + GridX + 1),
                        DefaultDistance = SpacingX
                    }
                );
        }
    }
}