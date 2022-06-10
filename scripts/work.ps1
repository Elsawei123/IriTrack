$skip = $false
if(($args.Count -gt 0) -and ($args[0] -eq $true)) {
    $skip = $true
}
$skip2 = $false
if(($args.Count -gt 1) -and ($args[1] -eq $true)) {
    $skip2 = $true
}
$skipGeneration = $skip
$skipTracking = $skip2

Write-Host "Generate Pattern"
if($skipGeneration -eq $false) {
    $outPattern = $false
    $generateBegin = [datetime]::Now
    $result = ($outPattern | .\1_Generation\generate.ps1)
    $generateEnd = [datetime]::Now
    $generateTime = ($generateEnd - $generateBegin).TotalMilliseconds
    Write-Host "times of network: $($result.NetTimes)"
    Write-Host "goodness        : $($result.Goodness)"
    Write-Host "time            : $generateTime"
}
else {
    Write-Host "skipped"
}
Write-Host


Write-Host "Start Tracking"
function Get-Time($line)
{
    return [double]::Parse($line.Split(',')[1])
}

if($skipTracking -eq $false) {
    .\2_Track\track.ps1
    $content = Get-Content "2_Track\result.txt"
    $len = $content.Length
    $first = $content[0]
    $last = $content[$len - 1]
    $duration = (Get-Time $last) - (Get-Time $first)
    Write-Host "time: $duration"
} else {
    Write-Host "skipped"
}
Write-Host

Write-Host "Measure Results"
$sum = .\3_Measure\measure.ps1
$sum | Out-Host
