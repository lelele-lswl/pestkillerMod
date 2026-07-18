$src = 'D:\aMyGameSelf\ModMaker\Slay the Spire 2'
$files = Get-ChildItem -Path $src -Recurse -Filter "GodotSharp.dll" -ErrorAction SilentlyContinue
foreach ($f in $files) {
    [Console]::WriteLine($f.FullName)
}
[Console]::WriteLine("---")
$files2 = Get-ChildItem -Path $src -Recurse -Filter "Godot.dll" -ErrorAction SilentlyContinue
foreach ($f in $files2) {
    [Console]::WriteLine($f.FullName)
}