# Database.cs Processor v6 - Line-level brace matching
$ErrorActionPreference = "Stop"

$inputFile = "c:\TRAE_PROJECT\PC_Updated Code\PC\CubeServer-v1.0.0\CubeServer\Data\Database.cs"
$outputFile = "c:\TRAE_PROJECT\PC_Updated Code\PC\CubeServer-v1.0.0\CubeServer\Data\Database.cs.new"

$lines = [string[]](Get-Content -Path $inputFile)
[int]$totalLines = $lines.Length
Write-Host "Read $totalLines lines"

$output2 = New-Object System.Collections.Generic.List[string]

$script:wrapped = 0
$script:already = 0
$script:allMethods = 0

$BSLASH = [char]0x5C
$DQ = [char]0x22
$OB = [char]0x7B
$CB = [char]0x7D

function FindMethodEnd {
    param(
        [string[]]$lns,
        [int]$startLine
    )
    
    [int]$depth = 0
    [bool]$inStr = $false
    
    for ([int]$x = $startLine; $x -lt $lns.Length; $x++) {
        [string]$ln = $lns[$x]
        [bool]$esc = $false
        
        for ([int]$ci = 0; $ci -lt $ln.Length; $ci++) {
            [char]$c = $ln[$ci]
            
            if ($c -eq $BSLASH) {
                if (-not $esc) { $esc = $true; continue }
                else { $esc = $false; continue }
            }
            if ($esc) { $esc = $false; continue }
            
            if ($c -eq $DQ) {
                $inStr = -not $inStr
                continue
            }
            
            if (-not $inStr) {
                if ($c -eq $OB) { $depth++ }
                elseif ($c -eq $CB) {
                    $depth--
                    if ($depth -eq 0 -and $x -gt $startLine) {
                        return $x
                    }
                }
            }
        }
    }
    return [int]($lns.Length - 1)
}

function GetRetTypeAndName {
    param([string]$sig)
    [string]$s = $sig.Trim()
    $s = $s -replace '^\s*public\s+', ''
    $s = $s -replace '^\s*async\s+', ''
    $s = $s -replace '^\s*static\s+', ''
    $s = $s -replace '^\s*unsafe\s+', ''
    
    [int]$pd2 = 0
    [int]$paramParenIdx = -1
    for ([int]$ci = 0; $ci -lt $s.Length; $ci++) {
        [char]$c = $s[$ci]
        if ($c -eq [char]'(') {
            if ($pd2 -eq 0 -and $ci -gt 0) {
                $paramParenIdx = $ci
                break
            }
            $pd2++
        } elseif ($c -eq [char]')') {
            if ($pd2 -gt 0) { $pd2-- }
        }
    }
    
    if ($paramParenIdx -le 0) { return @("void", "Unknown") }
    
    [string]$beforeParen = $s.Substring(0, $paramParenIdx).Trim()
    if ($beforeParen -match '(\w+)\s*$') {
        [string]$mname = $Matches[1]
        [int]$mnIdx = $beforeParen.LastIndexOf($mname)
        [string]$retType = ""
        if ($mnIdx -gt 0) {
            $retType = $beforeParen.Substring(0, $mnIdx).Trim()
        }
        return @($retType, $mname)
    }
    return @("void", "Unknown")
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
    if ($r -match '^\(.*\)$') { return "null" }
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

    if ($trimmed -match '^\s*public\s') {
        if ($trimmed -match ';\s*$' -or $trimmed -match '^\s*public\s+enum\s' -or $trimmed -match '^\s*public\s+class\s' -or $trimmed -match '^\s*public\s+struct\s' -or $trimmed -match '^\s*public\s+interface\s' -or $trimmed -match '^\s*public\s+delegate\s') {
            $output2.Add($line)
            $i++
            continue
        }
        
        [int]$sigStart = $i
        [int]$foundParenLine = -1
        for ([int]$fs = $i; $fs -lt [Math]::Min([int]($i+8), $totalLines); $fs++) {
            if ($lines[$fs] -match '\(') {
                $foundParenLine = $fs
                break
            }
        }
        if ($foundParenLine -lt 0) {
            $output2.Add($line)
            $i++
            continue
        }
        
        [int]$sigEnd = $foundParenLine
        [int]$pd = 0
        [bool]$started = $false
        for ([int]$si = $i; $si -lt [Math]::Min([int]($i+12), $totalLines); $si++) {
            [string]$sl = $lines[$si]
            for ([int]$ci = 0; $ci -lt $sl.Length; $ci++) {
                if ($sl[$ci] -eq [char]'(') { $pd++; $started = $true }
                elseif ($sl[$ci] -eq [char]')') {
                    if ($started) {
                        $pd--
                        if ($pd -eq 0) {
                            $sigEnd = $si
                            break
                        }
                    }
                }
            }
            if ($started -and $pd -eq 0) { break }
        }
        
        [string[]]$sigLines = $lines[$sigStart..$sigEnd]
        [string]$fullSig = $sigLines -join " "
        
        if ($fullSig -notmatch '\w\s*\(') {
            $output2.Add($line)
            $i++
            continue
        }
        [string]$justCheck = $fullSig.Trim()
        $justCheck = $justCheck -replace '^\s*public\s+', ''
        if ($justCheck -match '^Database\s*\(') {
            $output2.Add($line)
            $i++
            continue
        }
        
        $script:allMethods++
        $rtAndMn = GetRetTypeAndName $fullSig
        [string]$retType = $rtAndMn[0]
        [string]$mname = $rtAndMn[1]
        
        [int]$bodyStart = -1
        [int]$searchEnd = [Math]::Min([int]($sigEnd+5), [int]($totalLines-1))
        for ([int]$mi = [int]($sigEnd + 1); $mi -le $searchEnd; $mi++) {
            if ($lines[$mi].Trim() -match '^\s*\{') {
                $bodyStart = $mi
                break
            }
        }
        if ($bodyStart -lt 0 -and $lines[$sigEnd] -match '\{') {
            $bodyStart = $sigEnd
        }
        if ($bodyStart -lt 0) {
            for ([int]$oi = $sigStart; $oi -le [Math]::Min($sigEnd+2, $totalLines-1); $oi++) {
                $output2.Add($lines[$oi])
            }
            $i = [Math]::Min($sigEnd + 3, $totalLines)
            continue
        }
        
        [int]$bodyEnd = FindMethodEnd $lines $bodyStart
        
        if ($bodyEnd -le $bodyStart) {
            $bodyEnd = [Math]::Min($bodyStart + 100, $totalLines - 1)
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
            Write-Host "ALREADY: $mname line $($sigStart+1) [$retType]"
            $script:already++
            for ([int]$oi = $sigStart; $oi -le $bodyEnd; $oi++) {
                $output2.Add($lines[$oi])
            }
            $i = [int]($bodyEnd + 1)
            continue
        }

        Write-Host "WRAP: $mname line $($sigStart+1) [$retType]"
        $script:wrapped++

        [string]$dr = DefaultRet $retType
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
        if ($retType -ne 'void' -and $retType -ne '' -and $dr -ne "") {
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
Write-Host "Total public methods: $script:allMethods"
Write-Host "Wrapped (NEW try/catch): $script:wrapped"
Write-Host "Already had try/catch: $script:already"
Write-Host "Output lines: $($output2.Count)"
Write-Host "Done."
