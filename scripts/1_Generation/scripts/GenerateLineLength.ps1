function Generate-LineLength()
{
    $lengths = @(150, 200, 250)
    $weights = @(2, 3, 2)
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
    return $lengths[$index]
}

return Generate-LineLength