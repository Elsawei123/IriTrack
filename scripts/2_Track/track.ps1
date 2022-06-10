$path = $PWD.Path
$current = "$path\2_Track"
$patternPath = "$path\1_Generation\result.txt"
Set-Location $current

$speed = 500
$equalizeHist = "false"

Start-Process "$current\Track\ImageProcess.exe" -ArgumentList "$equalizeHist","null",$patternPath,$speed -WorkingDirectory "$current\Track" -RedirectStandardOutput "$current\result.txt" -Wait

Set-Location $path