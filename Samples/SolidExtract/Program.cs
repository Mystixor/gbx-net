﻿using GBX.NET;
using GBX.NET.Engines.Plug;
using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Length == 0)
    return;

var rootPath = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
var fileName = args[0];

var node = GameBox.ParseNode<CPlugSolid>(fileName);

if (node.Tree is null)
    return;

var dirName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(fileName));

Directory.CreateDirectory(dirName);

Recurse(node.Tree);

void Recurse(CPlugTree? tree, float? distance = null)
{
    if (tree is null)
        return;

    foreach (var plug in tree.Children)
    {
        Recurse(plug, distance);
    }

    if (tree is CPlugTreeVisualMip mip)
    {
        foreach (var level in mip.Levels)
        {
            Recurse(level.Value, distance: level.Key);
        }
    }

    var visual = tree.Visual;

    if (visual is null)
        return;

    var fileName = tree.Name + ".obj";
    var fullDirectory = Path.Combine(rootPath, dirName, distance.ToString() ?? "");
    var fullFileName = Path.Combine(fullDirectory, fileName);

    Directory.CreateDirectory(fullDirectory);

    using var w = new StreamWriter(fullFileName);

    if (visual is CPlugVisualIndexed indexed)
    {
        foreach (var vertex in indexed.Vertices)
        {
            w.WriteLine("v {0} {1} {2}", vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
        }

        if (indexed.TexCoords is not null)
        {
            foreach (var uv in indexed.TexCoords)
            {
                w.WriteLine("vt {0} {1}", uv.X, uv.Y);
            }
        }

        foreach (var indicies in indexed.Indicies.Chunk(3))
        {
            var a = indicies[0] + 1;
            var b = indicies[1] + 1;
            var c = indicies[2] + 1;

            if (indexed.TexCoords is null)
            {
                w.WriteLine("f {0} {1} {2}", a, b, c);
                continue;
            }

            w.WriteLine("f {0}/{0} {1}/{1} {2}/{2}", a, b, c);
        }
    }
}