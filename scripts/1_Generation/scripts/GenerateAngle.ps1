function Generate-Angle()
{
    $angles = @(30, 60, 90, 120, 150)
    $weights = @(0.766, 0.775, 0.717, 0.792, 0.808)
    [double]$weightSum = (($weights | Measure-Object -Sum).Sum * 1.0)
    $pas = $weights | ForEach-Object { $_ / $weightSum}
    $prob = Get-Double
    $index = 0
    foreach($pa in $pas)
    {
        if($pa -ge $prob) { break }
        $prob -= $pa
        $index++
    }
    return @($angles[$index], $weights[$index], $pas[$index])
}
return Generate-Angle