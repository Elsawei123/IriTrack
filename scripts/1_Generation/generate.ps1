function Calculate-G([System.Collections.Generic.IList[double]]$pas)
{
    $count = $pas.Count
    $value = [Math]::Pow(6.0, $count)
    $value /= ($count + 1)
    foreach($pa in $pas)
    {
        $value *= $pa
    }
    return $value
}
function Get-PL([int]$n)
{
    if($n -lt 3){ return 1 }
    else { return 1.0 / ($n - 3) }
}
function Get-Double()
{
    $min = 0
    $max = 10000
    $url = "https://www.random.org/integers/?num=1&min=$min&max=$max&col=1&base=10&format=plain&rnd=new"
    # $result = [int]::Parse((Invoke-WebRequest -Uri $url).Content)
    $result = Get-Random -Maximum $max -Minimum $min
    return ($result * 1.0) / ($max - $min)
}
function Get-IsNextDotNeeded([int]$n)
{
    $prob = Get-Double
    $pl = Get-PL $n
    if($prob -le $pl) { return $true }
    else { return $false }
}
function GeneratePattern()
{
    $angles = New-Object System.Collections.Generic.List[int]
    $weights = New-Object System.Collections.Generic.List[int]
    $anglePAs = New-Object System.Collections.Generic.List[double]
    $lines = New-Object System.Collections.Generic.List[int]
    $goodness = 0
    $times = 0
    while($true)
    {
        $angles.Clear()
        $weights.Clear()
        $anglePAs.Clear()
        $lines.Clear()

        $lines.Add((.\scripts\GenerateLineLength.ps1))
        $times += 1
        while((Get-IsNextDotNeeded ($angles.Count + 2)) -eq $true)
        {
            $angleResult = .\scripts\GenerateAngle.ps1
            $angles.Add($angleResult[0])
            $weights.Add($angleResult[1])
            $anglePAS.Add($angleResult[2])
            $lines.Add((.\scripts\GenerateLineLength.ps1))
            $times += 3
        }
        $goodness = Calculate-G $anglePAs
        if($goodness -ge 0.14) { break }
    }
    return @{
        Angles = $angles
        Lengths = $lines
        Weights = $weights
        Goodness = $goodness
        NetTimes = $times
    }
}

$path = $PWD.Path
$current = "$path\1_Generation"
Set-Location $current

$result = GeneratePattern
[System.Text.StringBuilder]$builder = New-Object System.Text.StringBuilder
$builder.AppendLine($result.Angles.Count) | Out-Null
$builder.AppendLine() | Out-Null
foreach($angle in $result.Angles)
{
    $builder.AppendLine($angle) | Out-Null
}
$builder.AppendLine() | Out-Null
foreach($weight in $result.Weights)
{
    $builder.AppendLine($weight) | Out-Null
}
$builder.AppendLine() | Out-Null
foreach($line in $result.Lengths)
{
    $builder.AppendLine($line) | Out-Null
}
$builder.AppendLine() | Out-Null

$builder.AppendLine($result.NetTimes) | Out-Null
$builder.AppendLine($result.Goodness) | Out-Null

$builder.ToString() | Out-File "result.txt" -Encoding ascii
if($input -eq $true)
{
    Write-Host $builder.ToString()
}
Set-Location $path
return $result
