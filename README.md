![GBX.NET](logo_outline.png)

GBX.NET is a GameBox (.Gbx) file parser library written in C# for .NET software framework. This file type can be seen in many of the Nadeo games like TrackMania, ShootMania or Virtual Skipper.

For any questions, open an issue, join the [GameBox Sandbox Discord server](https://discord.gg/9wAAJvKYyE) or message me via DM: BigBang1112#9489.

[![Nuget](https://img.shields.io/nuget/v/GBX.NET?style=for-the-badge)](https://www.nuget.org/packages/GBX.NET/)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/BigBang1112/gbx-net?include_prereleases&style=for-the-badge)](https://github.com/BigBang1112/gbx-net/releases)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/BigBang1112/gbx-net?style=for-the-badge)](#)

- GBX.NET can recognize **entire Gbx files**, however **cannot read all of the existing files**. Gbx file is basically a serialized class from the GameBox engine, and all of these classes must be known to read. Exploring new chunks can be done by reverse engineering the games.
- GBX.NET can **write** most of the Gbx file types (those that can be read by the parser).
- All versions of Gbx are supported: ranging from TM1.0 to TM®, except the Gbx versions below 3 (which haven't been seen so far, even in the oldest game).
- **GBX.NET 0.10.0+ is separated into MIT and GPL3.0, see [License](#License)**.
- Reading text-formatted Gbx is not currently supported.
- Reading compressed reference tables is not currently supported.
- Reading Pak files is partially supported with the **GBX.NET.PAK** sublibrary, but it applies only to Paks from TMU/F.

Here are some of the useful classes/types to start with:

| Latest extension | Class | Can read | Can write | Other extension/s
| --- | --- | --- | --- | ---
| Map.Gbx | [CGameCtnChallenge](Src/GBX.NET/Engines/Game/CGameCtnChallenge.cs) | Yes | Yes | Challenge.Gbx
| Replay.Gbx | [CGameCtnReplayRecord](Src/GBX.NET/Engines/Game/CGameCtnReplayRecord.cs) | Yes | **No<sup>1</sup>**
| Ghost.Gbx | [CGameCtnGhost](Src/GBX.NET/Engines/Game/CGameCtnGhost.cs) | Yes | **Yes**
| Clip.Gbx | [CGameCtnMediaClip](Src/GBX.NET/Engines/Game/CGameCtnMediaClip.cs) | Yes | Yes
| Item.Gbx | [CGameItemModel](Src/GBX.NET/Engines/GameData/CGameItemModel.cs) | Yes | No
| Block.Gbx | [CGameItemModel](Src/GBX.NET/Engines/GameData/CGameItemModel.cs) | Yes | No
| Mat.Gbx | [CPlugMaterialUserInst](Src/GBX.NET/Engines/Plug/CPlugMaterialUserInst.cs) | Yes | Yes
| Mesh.Gbx | [CPlugSolid2Model](Src/GBX.NET/Engines/Plug/CPlugSolid2Model.cs) | Yes<sup>2</sup> | Yes<sup>2</sup>
| Shape.Gbx | [CPlugSurface](Src/GBX.NET/Engines/Plug/CPlugSurface.cs) | Yes | Yes
| Macroblock.Gbx | [CGameCtnMacroBlockInfo](Src/GBX.NET/Engines/Game/CGameCtnMacroBlockInfo.cs) | Yes | Yes
| LightMapCache.Gbx | [CHmsLightMapCache](Src/GBX.NET/Engines/Hms/CHmsLightMapCache.cs) | Yes | Yes
| SystemConfig.Gbx | [CSystemConfig](Src/GBX.NET/Engines/System/CSystemConfig.cs) | Yes | Yes
| FidCache.Gbx | [CMwRefBuffer](Src/GBX.NET/Engines/MwFoundations/CMwRefBuffer.cs) | Yes | Yes
| Scores.Gbx | [CGamePlayerScore](Src/GBX.NET/Engines/Game/CGamePlayerScore.cs) | Yes | No

- <sup>1</sup>Safety reasons. Consider extracting `CGameCtnGhost` from `CGameCtnReplayRecord`, transfer it over to `CGameCtnMediaBlockGhost`, add it to `CGameCtnMediaClip`, and save it as `.Clip.Gbx`, which you can then import in MediaTracker.
- <sup>2</sup>May not have perfect support for the `CPlugVisual3D` objects inside and TM2020 could be generally unstable as well.

**Full list of supported file types is available in the [SUPPORTED GBX FILE TYPES](SUPPORTED_GBX_FILE_TYPES.md)**.

## Compatibility and build

- GBX.NET is compatible down to **.NET Standard 2.0** and **.NET Framework 4.6.2**.
- Current C# language version is **12**.

To build the solution:
- Installing Visual Studio 2022 with default .NET tools, **.NET WebAssembly Build Tools**, **.NET Core 3.1 Runtime**, and **.NET Framework 4.6.2 Targeting Pack** is the easiest option.
- JetBrains Rider also works as it should. Visual Studio Code may work with a bit more setup.
- Make sure you have all the needed targetting packs installed (currently **.NET 8.0**, .NET 7.0, .NET 6.0, .NET Standard 2.1, .NET Standard 2.0, and .NET Framework 4.6.2).

*(reminder: you can just use the NuGet packages in any IDE or text editor that supports them)*

## Techniques

The Gbx format compatibility is extensible through the `GBX.NET.Engines` namespace.

Through the Gbx format, internal game objects (called **nodes**) are being serialized and deserialized through **chunks**. In GBX.NET, chunks are presented as node's **nested classes** and they are named using the pattern `Chunk[chunkId]` (where `chunkId` is formatted on 8 digits).

Nodes inherit `CMwNod` or other class that inherits `CMwNod` and each node class has a `NodeAttribute` with its **latest ID** and a **protected constructor**. Chunks inherit the `Chunk<T>`/`SkippableChunk<T>`/`HeaderChunk<T>` class based on the behaviour and each chunk class has a `ChunkAttribute` with its **latest ID**.

To keep the code clean while also performant, the library includes a source generated layer on top for effective reading and writing with minimal cold start.

- Better performance than reflection (mainly due to minimal cold start)
- `Parse...()` methods are thread safe.
- Con: Library is larger due to generated code (not a short one).

The library also speeds up parse time by ignoring unused skippable chunks with *discover* feature:

- Discover basically means "read a skippable chunk".
- Skippable chunks are read in-depth only if methods or properties request for them.
- Calling certain properties will discover all needed chunks synchronously before returning the value.
- You can pre-discover certain chunks on different threads to increase your code's performance.

## Benchmarks

Coming soon!

## Dependencies

### GBX.NET

- 0.0.1 - 0.4.1: SharpZipLib.NETStandard
- 0.1.0 - 0.4.1: Microsoft.CSharp
- 0.0.1 - 0.9.0: System.Drawing.Common
- 0.13.0+: TmEssentials
- 0.15.0+: Microsoft.Extensions.Logging.Abstractions
- 1.2.4+: System.IO.Hashing

### GBX.NET.Imaging
- System.Drawing.Common

### GBX.NET.Json
- Newtonsoft.Json

## Usage

Using the NuGet packages is recommended.

### Create a new GBX.NET project (lightweight)

1. Install .NET SDK 8.
    - Windows: `winget install Microsoft.DotNet.SDK.8` (make sure you have WinGet installed)
    - [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux) (just SDK)
2. Create directory for your project (anywhere), **go inside it**.
3. Create new console project: `dotnet new console`
4. Add the latest GBX.NET NuGet pckage: `dotnet add package GBX.NET`
5. Open `Program.cs` with your favorite text editor: `code . -g Program.cs`
6. Write code - see [Examples](#examples).
7. Use `dotnet run` to run the app.

Steps 2-7:
```
mkdir MyGbxProject
cd MyGbxProject
dotnet new console
dotnet add package GBX.NET
code . -g Program.cs
dotnet run
```

### Create a new GBX.NET project (Visual Studio Code)

1. Install C# Dev Kit extension.
2. Click on `Create .NET Project` button, or press <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd>, type `.NET: New Project`.
3. Select `Console App` and create your project.
4. Open a new terminal and type `dotnet add package GBX.NET` to add GBX.NET.
5. Write code - see [Examples](#examples).
6. Run and debug as usual, select C# if prompted.

### Create a new GBX.NET project (Visual Studio)

1. Create a new Console project
2. Under your project in Solution Explorer, right-click on Dependencies and select `Manage NuGet packages...`
3. Search `GBX.NET` and click install
4. Write code - see [Examples](#examples).

### Examples

To parse a Gbx with a known type:

```cs
using GBX.NET;
using GBX.NET.Engines.Game;

var map = GameBox.ParseNode<CGameCtnChallenge>("MyMap.Map.Gbx");
```

To parse a Gbx with an unknown type:

```cs
using GBX.NET;
using GBX.NET.Engines.Game;

var node = GameBox.ParseNode("MyMap.Map.Gbx");

if (node is CGameCtnChallenge map)
{
    // Node data is available in map
}
else if (node is CGameCtnReplayRecord replay)
{
    // Node data is available in replay
}

// C# 7+

switch (node)
{
    case CGameCtnChallenge map:
        // Node data is available in map
        break;
    case CGameCtnReplayRecord replay:
        // Node data is available in replay
        break;
}
```

To get the Gbx metadata, use the `node.GetGbx()` property (only possible on the main node).

To save changes of the parsed Gbx file:

```cs
using GBX.NET;
using GBX.NET.Engines.Game;

var node = GameBox.ParseNode("MyMap.Map.Gbx");

if (node is CGameCtnChallenge map)
{
    // Do changes with CGameCtnChallenge

    map.Save("MyMap.Map.Gbx");
}
else if (node is CGameCtnGhost ghost)
{
    // Do changes with CGameCtnGhost

    ghost.Save("MyGhost.Ghost.Gbx");
}
```

To save any supported `Node` to a Gbx file:

```cs
using GBX.NET;
using GBX.NET.Engines.Game;

var replay = GameBox.ParseNode<CGameCtnReplayRecord>("MyReplay.Replay.Gbx");

// Ghosts property can be null if you would use the ParseNodeHeader method.
if (replay.Ghosts is not null)
{
    foreach (CGameCtnGhost ghost in replay.Ghosts)
    {
        ghost.Save($"{ghost.GhostNickname}.Ghost.Gbx");
    }
}
```

Compressed Gbx files require to include the GBX.NET.LZO library (or any similar implementation, but there are no other compatible at the moment). In most of the cases, the LZO compression is automatically detected after just referencing the library in the project. You don't require to have a `using GBX.NET.LZO;` anywhere.

On specific platforms like Blazor WebAssembly though, the dependency system works differently and (currently) GBX.NET struggles to automatically detect the LZO library.

In these cases (if just following the `MissingLzoException` message didn't solve the problem), add this line above the first attempt of parsing the Gbx. It should be called just once.

```cs
GBX.NET.Lzo.SetLzo(typeof(GBX.NET.LZO.MiniLZO));
```

## Conventions

### This convention is no longer relevant in GBX.NET 0.11.0+ when using the ParseNode method.

Make the code cleaner by **aliasing** the `Node` from the parsed `GameBox`:

```cs
var gbx = GameBox.Parse<CGameCtnChallenge>("MyMap.Map.Gbx");
var map = gbx.Node; // Like this

var bronzeTime = gbx.Node.BronzeTime; // WRONG !!!
var silverTime = map.SilverTime; // Correct
```

## Trimming

**Since 1.2.1**, GBX.NET officially supports full trimming, but you have to be careful using it. You're not forced to use trimming in case of complications.

To make sure it works:

1. Explicitly setting LZO is **required**:

```cs
GBX.NET.Lzo.SetLzo(typeof(GBX.NET.LZO.MiniLZO));
```

2. After that, currently, you can just suppress warnings on the `GameBox.Parse...` and possibly other methods. Sadly, it's not possible to suppress warnings only for a specific library reference, so you have to do it this way for now. This will improve in GBX.NET 2. **Please, do not suppress the warning globally.**

```cs
#pragma warning disable IL2026 // add these lines
var gbx = GameBox.ParseNode<CGameCtnReplayRecord>(args[0], logger: logger);
#pragma warning restore IL2026 // add these lines
```

3. In case you wanna use reflection on GBX.NET, it is strongly recommended to simply turn off trimming of this library. **In case of Blazor WebAssembly specifically, it's worth noting that the release build trims automatically**, so in case you're using reflection, modify your library reference:

```xml
<PackageReference Include="GBX.NET">
    <IsTrimmable>false</IsTrimmable> <!-- add this line -->
</PackageReference>
```

## License

- **The sub-library GBX.NET.LZO is licensed with [GNU General Public License v3.0](Src/GBX.NET.LZO/LICENSE.GPL-3.0-or-later.md). If you're going to use this library, please license your work under GPL-3.0-or-later.**
- **GbxExplorer, and everything in the Tools and Samples folder is also licensed with [GNU General Public License v3.0](Samples/LICENSE.GPL-3.0-or-later.md).**
- The libraries GBX.NET, GBX.NET.Imaging, GBX.NET.Json, GBX.NET.Localization and the project DocGenerator **are licensed with MIT** and you can use them much more permissively.
- Information gathered from the project (chunk structure, parse examples, data structure, wiki information, markdown) is usable with [The Unlicense](https://unlicense.org/).
- Font of the logo is called [Raleway](https://fonts.google.com/specimen/Raleway) licensed under [SIL Open Font License](https://scripts.sil.org/cms/scripts/page.php?site_id=nrsi&id=OFL). You can use the logo as a part of your media.

### Conclusion

Your work doesn't have to fall under the GNU GPL license if you're interested in either reading the header data only, or reading certain uncompressed Gbx files (usually the internal ones inside PAK files). If you're looking to read the content of a **compressed Gbx body** (applies to maps, replays and other user generated content), you **have to license your work with GNU GPL v3.0 or later**.

## Special thanks

Without these people, this project wouldn't be what it is today:

- Stefan Baumann (Solux)
- Melissa (Miss)
- florenzius
- Kim
- tilman
- James Romeril
- frolad (Juice)
- Mika Kuijpers (TheMrMiku)
- donadigo

And many thanks to every bug reporter!

## Alternative Gbx parsers

- [gbx.js](https://github.com/ThaumicTom/gbx.js) by ThaumicTom (Gbx header parser for clientside JavaScript)
- [ManiaPlanetSharp](https://github.com/stefan-baumann/ManiaPlanetSharp) by Solux (C# toolkit for accessing ManiaPlanet data, including Gbx parser used by ManiaExchange)
- [pygbx](https://github.com/donadigo/pygbx) by Donadigo (Gbx parser for Python)
