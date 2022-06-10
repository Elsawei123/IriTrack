function Save-Result([string]$folder)
{
    $count = (Get-ChildItem ".\$folder" | Where-Object {$_ -is [System.IO.DirectoryInfo]}).Count
    $count++
    New-Item ".\$folder\$count" -ItemType "Directory" | Out-Null
    Copy-Item ".\1_Generation\result.txt" ".\$folder\$count\pattern.txt"
    Copy-Item ".\2_Track\result.txt" ".\$folder\$count\track.txt"
    Copy-Item ".\3_Measure\result.txt" ".\$folder\$count\measure.txt"
}
$folder = "Results"
if($args.Count -gt 0) {
    $folder = $args[0]
}
Save-Result $folder