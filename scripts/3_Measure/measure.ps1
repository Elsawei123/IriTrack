$path = $PWD.Path
$pointsPath = "$path\2_Track\result.txt"
$patternPath = "$path\1_Generation\result.txt"
$current = "$path\3_Measure"
Set-Location $current

if ((Get-Content $pointsPath).Count -eq 0) {
    return @{
        LeftCost = 0
        RightCost = 0
        Time = 0
    }
}

Start-Process ".\PointAngleCS.exe" -ArgumentList $patternPath,$pointsPath,"false" -NoNewWindow -RedirectStandardOutput "result.txt" -Wait
$content = Get-Content "result.txt"
$index = $content.IndexOf("======")
$leftSum = [double]::Parse($content[$index - 1])
$rightSum = [double]::Parse($content[$content.Length - 2])
$time = [double]::Parse($content[$content.Length - 1])
Set-Location $path
return @{
    LeftCost = $leftSum
    RightCost = $rightSum
    Time = $time
}