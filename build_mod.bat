copy /y ..\MtnBaseShaders.pak
move /y MtnBaseShaders.pak MovieNight.pak
cd ..\..\TagTool\Latest\net10.0-windows
type ..\..\..\Mods\BladeRunnerMovie\movie_build_script.cmds|TagTool.exe
move /y ..\..\..\Mods\BladeRunnerMovie\MovieNight.pak D:\Halo\Eldewrito\Game\mods\

pause