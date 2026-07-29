# Database.cs Processor v4 - more robust method detection
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

function GetRetTypeAndName {
    param([string]$sig)
    [string]$s = $sig.Trim()
    
    # Remove modifiers
    $s = $s -replace '^\s*public\s+', ''
    $s = $s -replace '^\s*async\s+', ''
    $s = $s -replace '^\s*static\s+', ''
    $s = $s -replace '^\s*unsafe\s+', ''
    
    # Find first paren ( that starts the params
    # Need to skip over parens that are part of return type like (double, double)
    [int]$parenDepth = 0
    [int]$paramParenIdx = -1
    for ([int]$ci = 0; $ci -lt $s.Length; $ci++) {
        [char]$c = $s[$ci]
        if ($c -eq [char]'(') {
            if ($parenDepth -eq 0 -and $ci -gt 0) {
                # Check if this looks like start of params
                # (char before should be whitespace or identifier char close to type)
                $paramParenIdx = $ci
                break
            }
            $parenDepth++
        } elseif ($c -eq [char]')') {
            if ($parenDepth -gt 0) { $parenDepth-- }
        }
    }
    
    if ($paramParenIdx -le 0) { return @("void", "Unknown") }
    
    [string]$beforeParen = $s.Substring(0, $paramParenIdx).Trim()
    # Method name is last token, everything before is return type
    # Last token is sequence of letters/digits/_ at end
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
    # Tuple types like (double, double), etc. -> null (for value tuples default would be default, use null safe if nullable)
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

# Is this line a potential start of public method? Check pattern: public ... (...)
# { - open brace for method
function IsPublicMethodCandidate {
    param([string]$line)
    [string]$t = $line.Trim()
    # Must start with public (possibly after [attribute])
    if ($t -match '^\s*public\s') { return $true }
    return $false
}

[int]$i = 0
while ($i -lt $totalLines) {
    [string]$line = $lines[$i]
    [string]$trimmed = $line.Trim()

    if (IsPublicMethodCandidate $line) {
        # Look ahead to find open paren - multi-line signatures
        [int]$sigStart = $i
        [int]$foundParenLine = -1
        
        # Search for ( within next 5 lines
        for ([int]$fs = $i; $fs -lt [Math]::Min([int]($i+5), $totalLines); $fs++) {
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
        
        # Now find closing paren from foundParenLine onwards
        [int]$sigEnd = $foundParenLine
        [string[]]$sigLines = @()
        [int]$pd = 0
        [bool]$started = $false
        for ([int]$si = $i; $si -lt [Math]::Min([int]($i+10), $totalLines); $si++) {
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
        $sigLines = $lines[$sigStart..$sigEnd]
        
        [string]$fullSig = $sigLines -join " "
        
        # Get return type and method name
        $rtAndMn = GetRetTypeAndName $fullSig
        [string]$retType = $rtAndMn[0]
        [string]$mname = $rtAndMn[1]
        
        if ($mname -eq "Database" -or $mname -eq "Unknown") {
            # Constructor or unrecognized - output as-is
            # But might be just a field? e.g. public string connStr; - need to skip those
            # If the fullSig doesn't look like a method (no () after name with proper body), skip
            if ($mname -eq "Database") {
                $output2.Add($line)
                $i++
                continue
            }
            # Fall through - check if it has parens. If unknown and no sig, skip processing as method
        }
        
        # Make sure this looks like a method: the fullSig should have () after the method name
        # Also make sure it's not a field declaration like public enum CubeQueryOption { ... }
        if ($fullSig -notmatch '\w\s*\(' -or $fullSig -match ';\s*$') {
            # Not a method - enum declaration or field or delegate
            $output2.Add($line)
            $i++
            continue
        }
        
        $script:allMethods++

        # Find opening { of the method
        [int]$bodyStart = -1
        [int]$searchEnd = [Math]::Min([int]($sigEnd+5), [int]($totalLines-1))
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
            for ([int]$oi = $sigStart; $oi -le [Math]::Min($sigEnd+2, $totalLines-1); $oi++) {
                $output2.Add($lines[$oi])
            }
            $i = [Math]::Min($sigEnd + 3, $totalLines)
            continue
        }

        [int]$obc = $lines[$bodyStart].IndexOf('{')
        if ($obc -lt 0) { $obc = 0 }

        [int[]]$closeRes = FindMatch $lines $bodyStart $obc
        [int]$bodyEnd = $closeRes[0]
        [int]$cbc = $closeRes[1]

        if ($bodyEnd -le $bodyStart) {
            for ([int]$oi = $sigStart; $oi -le [Math]::Min([int]($bodyStart+20), [int]($totalLines-1)); $oi++) {
                $output2.Add($lines[$oi])
            }
            $i = [Math]::Min([int]($bodyStart + 21), $totalLines)
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
Write-Host "Total public methods detected: $script:allMethods"
Write-Host "Wrapped (added try/catch): $script:wrapped"
Write-Host "Already had try/catch: $script:already"
Write-Host "Output lines: $($output2.Count)"
Write-Host "Done."
