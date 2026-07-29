# Database.cs Processor v3
$ErrorActionPreference = "Stop"

$inputFile = "c:\TRAE_PROJECT\PC_Updated Code\PC\CubeServer-v1.0.0\CubeServer\Data\Database.cs"
$outputFile = "c:\TRAE_PROJECT\PC_Updated Code\PC\CubeServer-v1.0.0\CubeServer\Data\Database.cs.new"

$lines = [string[]](Get-Content -Path $inputFile)
[int]$totalLines = $lines.Length
Write-Host "Read $totalLines lines"

$output2 = New-Object System.Collections.Generic.List[string]

$script:wrapped = 0
$script:already = 0

$BACKSLASH = [char]0x5C

function FindMatch {
    param(
        [string[]]$lns,
        [int]$sLine,
        [int]$sChar
    )
    [int]$depth = 0
    [bool]$inStr = $false
    [bool]$inCh = $false
    [char]$sCh = [char]0
    [int]$lineCount = $lns.Length

    for ([int]$x = $sLine; $x -lt $lineCount; $x++) {
        [string]$ln = $lns[$x]
        [int]$startJ = 0
        if ($x -eq $sLine -and $sChar -ge 0) { $startJ = $sChar }

        for ([int]$j = $startJ; $j -lt $ln.Length; $j++) {
            [char]$ch = $ln[$j]
            [char]$prev = [char]0
            if ($j -gt 0) { $prev = $ln[$j-1] }

            if ($prev -ne $BACKSLASH) {
                if (($ch -eq [char]'"' -or $ch -eq [char]"'") -and -not $inCh) {
                    if ($inStr -and $ch -eq $sCh) {
                        $inStr = $false
                        $sCh = [char]0
                    } elseif (-not $inStr) {
                        $inStr = $true
                        $sCh = $ch
                    }
                }
                if ($ch -eq [char]"'" -and -not $inStr) {
                    if ($inCh) { $inCh = $false } else { $inCh = $true }
                }
            }

            if (-not $inStr -and -not $inCh) {
                if ($ch -eq [char]'{') { $depth++ }
                elseif ($ch -eq [char]'}') {
                    $depth--
                    if ($depth -eq 0) {
                        return [int[]]@($x, $j)
                    }
                }
            }
        }
    }
    $lastIdx = [int]($lineCount - 1)
    return [int[]]@($lastIdx, 0)
}

function GetRetType {
    param([string]$sig)
    [string]$s = $sig.Trim()
    $s = $s -replace '^\s*public\s+', ''
    $s = $s -replace '^\s*async\s+', ''
    $s = $s -replace '^\s*static\s+', ''
    [int]$pi = $s.IndexOf('(')
    if ($pi -le 0) { return "unknown" }
    [string]$bp = $s.Substring(0, $pi).Trim()
    [string[]]$parts = $bp -split '\s+'
    if ($parts.Length -le 1) { return "void" }
    [string[]]$rtParts = $parts[0..([int]($parts.Length-2))]
    return ($rtParts -join ' ').Trim()
}

function GetMethName {
    param([string]$sig)
    [int]$pi = $sig.IndexOf('(')
    if ($pi -le 0) { return "Meth" }
    [string]$bp = $s.Substring(0, $pi).Trim()
    [string[]]$parts = $bp -split '\s+'
    if ($parts.Length -le 0) { return "Meth" }
    return $parts[[int]($parts.Length-1)].Trim()
}

function GetOutParamsArr {
    param([string]$sig)
    $results = @()
    [int]$opi = $sig.IndexOf('(')
    [int]$cpi = $sig.LastIndexOf(')')
    if ($opi -le 0 -or $cpi -le $opi) { return $results }
    [string]$pstr = $sig.Substring([int]($opi+1), [int]($cpi - $opi - 1))
    foreach ($pp in ($pstr -split ',')) {
        [string]$ppt = $pp.ToString().Trim()
        if ($ppt -match '^out\s+(.+?)\s+(\w+)\s*$') {
            $results += $Matches[2]
        }
    }
    return ,$results
}

function DefaultRet {
    param([string]$rt)
    [string]$r = $rt.Trim()
    if ($r -eq 'void' -or $r -eq '') { return "" }
    if ($r -eq 'bool') { return "false" }
    if ($r -eq 'string') { return "null" }
    if ($r -eq 'int') { return "0" }
    if ($r -eq 'double') { return "0.0" }
    if ($r -eq 'long') { return "0" }
    if ($r -eq 'short') { return "0" }
    if ($r -eq 'byte') { return "0" }
    if ($r -eq 'decimal') { return "0" }
    if ($r -eq 'float') { return "0.0f" }
    if ($r -eq 'DateTime') { return "DateTime.MinValue" }
    if ($r -eq 'TimeSpan') { return "TimeSpan.Zero" }
    if ($r -eq 'Guid') { return "Guid.Empty" }
    if ($r -match 'List<') { return "new()" }
    if ($r -match 'Dictionary<') { return "new()" }
    if ($r -match 'HashSet<') { return "new()" }
    if ($r -match 'Queue<') { return "new()" }
    if ($r -match 'Stack<') { return "new()" }
    if ($r -match '\[\]') { return "null" }
    if ($r -match '^(\w+)\?$') { return "null" }
    return "null"
}

function DefaultOutParam {
    param([string]$pn, [string]$sig)
    [int]$opi = $sig.IndexOf('(')
    [int]$cpi = $sig.LastIndexOf(')')
    if ($opi -le 0 -or $cpi -le $opi) { return "$pn = 0;" }
    [string]$pstr = $sig.Substring([int]($opi+1), [int]($cpi - $opi - 1))
    foreach ($pp in ($pstr -split ',')) {
        [string]$ppt = $pp.ToString().Trim()
        if ($ppt -match "out\s+(.+?)\s+$pn") {
            [string]$pt = $Matches[1].Trim()
            [string]$dv = DefaultRet $pt
            if ($dv -eq "") { $dv = "0" }
            return "$pn = $dv;"
        }
    }
    return "$pn = 0;"
}

[int]$i = 0
while ($i -lt $totalLines) {
    [string]$line = $lines[$i]
    [string]$trimmed = $line.Trim()

    if ($trimmed -match '^\s*public\s+(?:async\s+)?(?:static\s+)?(?:[a-zA-Z0-9_<>.,\[\]\s?]+)\s+([a-zA-Z0-9_]+)\s*\(') {
        [string]$mname = $Matches[1]
        if ($mname -eq 'Database') {
            $output2.Add($line)
            $i++
            continue
        }

        [int]$sigStart = $i
        [int]$sigEnd = $i
        [string[]]$sigLines = @($line)
        [int]$pd = 0
        for ([int]$si = $i; $si -lt $totalLines; $si++) {
            [string]$sl = $lines[$si]
            for ([int]$ci = 0; $ci -lt $sl.Length; $ci++) {
                if ($sl[$ci] -eq [char]'(') { $pd++ }
                elseif ($sl[$ci] -eq [char]')') {
                    $pd--
                    if ($pd -eq 0) {
                        $sigEnd = $si
                        if ($si -gt $i) {
                            [int]$len = [int]($si - $i + 1)
                            $sigLines = $lines[$i..$si]
                        }
                        break
                    }
                }
            }
            if ($pd -eq 0) { break }
        }

        [string]$fullSig = $sigLines -join " "

        [int]$bodyStart = -1
        [int]$searchEnd = [Math]::Min([int]($sigEnd+4), [int]($totalLines-1))
        for ([int]$mi = [int]($sigEnd + 1); $mi -le $searchEnd; $mi++) {
            if ($lines[$mi].Trim() -match '^\s*\{') {
                $bodyStart = $mi
                break
            }
        }
        if ($bodyStart -lt 0) {
            if ($lines[$sigEnd] -match '\{') {
                $bodyStart = $sigEnd
            }
        }
        if ($bodyStart -lt 0) {
            $output2.Add($line)
            $i++
            continue
        }

        [int]$obc = $lines[$bodyStart].IndexOf('{')
        if ($obc -lt 0) { $obc = 0 }

        [int[]]$closeRes = FindMatch $lines $bodyStart $obc
        [int]$bodyEnd = $closeRes[0]
        [int]$cbc = $closeRes[1]

        if ($bodyEnd -le $bodyStart) {
            for ([int]$oi = $sigStart; $oi -le [Math]::Min([int]($sigStart+20), [int]($totalLines-1)); $oi++) {
                $output2.Add($lines[$oi])
            }
            $i = [Math]::Min([int]($sigStart + 21), $totalLines)
            continue
        }

        [string[]]$fullMethodLines = $lines[$sigStart..$bodyEnd]
        [string]$fullMethText = $fullMethodLines -join "`n"
        [string[]]$bodyInside = @()
        if ([int]($bodyEnd-1) -ge [int]($bodyStart+1)) {
            $bodyInside = $lines[[int]($bodyStart+1)..[int]($bodyEnd-1)]
        }

        [bool]$hasConn = $false
        if ($fullMethText -match 'new\s+SqlConnection\s*\(') {
            $hasConn = $true
        }

        [bool]$connInParam = $false
        if ($fullSig -match 'SqlConnection\s+\w+') {
            $connInParam = $true
        }

        [string]$indentStr = ""
        if ($line -match '^(\s*)') { $indentStr = $Matches[1] }
        [string]$indent1 = $indentStr + "    "
        [string]$indent2 = $indent1 + "    "

        if (-not $hasConn -or $connInParam) {
            for ([int]$oi = $sigStart; $oi -le $bodyEnd; $oi++) {
                $output2.Add($lines[$oi])
            }
            $i = [int]($bodyEnd + 1)
            continue
        }

        [string]$bodyFlat = ($bodyInside -join "`n")
        [string]$nc = $bodyFlat
        $nc = $nc -replace '/\*[\s\S]*?\*/', ' '
        $nc = $nc -replace '//[^\n]*', ''

        [bool]$alreadyWrap = $false
        [int]$tryIdx = $nc.IndexOf("try {")
        if ($tryIdx -lt 0) { $tryIdx = $nc.IndexOf("try`n") }
        if ($tryIdx -lt 0) { $tryIdx = $nc.IndexOf("try`r") }
        [int]$connIdx = $nc.IndexOf("new SqlConnection")

        if ($tryIdx -ge 0 -and $connIdx -ge 0 -and $tryIdx -lt $connIdx) {
            [int]$catchIdx = $nc.LastIndexOf("catch")
            if ($catchIdx -ge 0 -and $catchIdx -gt $connIdx) {
                $alreadyWrap = $true
            }
        }

        if ($alreadyWrap) {
            Write-Host "ALREADY: $mname line $($sigStart+1)"
            $script:already++
            for ([int]$oi = $sigStart; $oi -le $bodyEnd; $oi++) {
                $output2.Add($lines[$oi])
            }
            $i = [int]($bodyEnd + 1)
            continue
        }

        Write-Host "WRAP: $mname line $($sigStart+1)"
        $script:wrapped++

        [string]$rt = GetRetType $fullSig
        [string]$mn = $mname
        [string]$dr = DefaultRet $rt
        $outParamsArr = GetOutParamsArr $fullSig

        for ([int]$oi = $sigStart; $oi -lt $bodyStart; $oi++) {
            $output2.Add($lines[$oi])
        }

        if ($bodyStart -ne $sigStart) {
            $output2.Add($lines[$bodyStart])
        } else {
            [string]$sLine = $lines[$sigStart]
            if ($sLine -match '^(.*?)\{\s*$') {
                $output2.Add($Matches[1].TrimEnd())
                $output2.Add("$indentStr{")
            } else {
                $output2.Add($sLine)
            }
        }

        foreach ($op in $outParamsArr) {
            [string]$opn = $op.ToString()
            [string]$defLine = DefaultOutParam $opn $fullSig
            $output2.Add("$indent1$defLine")
        }

        $output2.Add("$indent1" + "try")
        $output2.Add("$indent1" + "{")

        foreach ($bl in $bodyInside) {
            [string]$bt = $bl.ToString()
            if ($bt.Trim() -eq '') {
                $output2.Add('')
            } else {
                $output2.Add("$indent1" + $bt)
            }
        }

        $output2.Add("$indent1" + "}")
        $output2.Add("$indent1" + "catch (Exception ex)")
        $output2.Add("$indent1" + "{")
        $output2.Add("$indent2" + 'Global.logger?.LogMessageEx("Error", "DB {0}: {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name ?? "method", ex.Message);')
        if ($rt -ne 'void' -and $rt -ne '' -and $dr -ne "") {
            $output2.Add("$indent2" + "return $dr;")
        }
        $output2.Add("$indent1" + "}")
        $output2.Add("$indentStr" + "}")

        $i = [int]($bodyEnd + 1)
        continue
    }

    $output2.Add($line)
    $i++
}

Write-Host "Writing..."
[System.IO.File]::WriteAllLines($outputFile, $output2)
Write-Host ""
Write-Host "=== RESULTS ==="
Write-Host "Wrapped (added try/catch): $script:wrapped"
Write-Host "Already had try/catch: $script:already"
Write-Host "Output lines: $($output2.Count)"
Write-Host "Done."
