# Database.cs Processor - Adds try/catch wrapping to public methods that open SqlConnection
$ErrorActionPreference = "Stop"

$inputFile = "c:\TRAE_PROJECT\PC_Updated Code\PC\CubeServer-v1.0.0\CubeServer\Data\Database.cs"
$outputFile = "c:\TRAE_PROJECT\PC_Updated Code\PC\CubeServer-v1.0.0\CubeServer\Data\Database.cs.new"
$backupFile = "c:\TRAE_PROJECT\PC_Updated Code\PC\CubeServer-v1.0.0\CubeServer\Data\Database.cs.bak"

# Read all lines
$lines = [System.IO.File]::ReadAllLines($inputFile)
$totalLines = $lines.Count
Write-Host "Read $totalLines lines from $inputFile"

# First, backup original
Copy-Item $inputFile $backupFile -Force
Write-Host "Backed up to $backupFile"

# We'll build the output as a list of lines
$output = New-Object System.Collections.Generic.List[string]

# Track method wrapping statistics
$global:wrappedCount = 0
$global:alreadyWrappedCount = 0
$global:skippedNoConnection = 0
$global:methodsProcessed = 0

# Helper: Find matching close brace for an open brace at a given position
function FindMatchingBrace {
    param(
        [string[]]$lines,
        [int]$startLine,    # line index where the open brace { is
        [int]$startChar     # char index of the open brace in that line, -1 if we just know the line
    )
    
    $depth = 0
    $inString = $false
    $inChar = $false
    $stringChar = ''
    
    for ($i = $startLine; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        $j = if ($i -eq $startLine -and $startChar -ge 0) { $startChar } else { 0 }
        
        while ($j -lt $line.Length) {
            $ch = $line[$j]
            $prev = if ($j -gt 0) { $line[$j-1] } else { [char]0 }
            
            # Handle escape sequences
            if ($prev -ne '\') {
                if (($ch -eq '"' -or $ch -eq "'") -and -not $inChar) {
                    if ($inString -and $ch -eq $stringChar) {
                        $inString = $false
                        $stringChar = ''
                    } elseif (-not $inString) {
                        $inString = $true
                        $stringChar = $ch
                    }
                }
                if ($ch -eq "'" -and -not $inString) {
                    if ($inChar) { $inChar = $false } else { $inChar = $true }
                }
            }
            
            if (-not $inString -and -not $inChar) {
                if ($ch -eq '{') { $depth++ }
                elseif ($ch -eq '}') { 
                    $depth--
                    if ($depth -eq 0) {
                        return @($i, $j)
                    }
                }
            }
            $j++
        }
    }
    return @($lines.Count-1, 0)
}

# Helper: Get return type from method signature
function GetReturnType {
    param([string]$signature)
    
    # Remove 'public' and 'async' keywords first
    $s = $signature.Trim()
    $s = $s -replace '^\s*public\s+', ''
    $s = $s -replace '^\s*async\s+', ''
    $s = $s -replace '^\s*static\s+', ''
    
    # Now the first token should be the return type (or void)
    # Find the method name - it's before the (
    $parenIdx = $s.IndexOf('(')
    if ($parenIdx -le 0) { return "unknown" }
    
    $beforeParen = $s.Substring(0, $parenIdx).Trim()
    # Split by space - last token is method name, rest is return type
    $parts = $beforeParen -split '\s+'
    if ($parts.Count -le 1) { return "void" }
    
    $returnTypeParts = $parts[0..($parts.Count-2)]
    $returnType = $returnTypeParts -join ' '
    return $returnType.Trim()
}

# Helper: Get method name from signature
function GetMethodName {
    param([string]$signature)
    
    $parenIdx = $signature.IndexOf('(')
    if ($parenIdx -le 0) { return "UnknownMethod" }
    
    $beforeParen = $signature.Substring(0, $parenIdx).Trim()
    $parts = $beforeParen -split '\s+'
    if ($parts.Count -le 0) { return "UnknownMethod" }
    return $parts[$parts.Count-1].Trim()
}

# Helper: Extract out parameter names
function GetOutParams {
    param([string]$signature)
    
    $parenIdx = $signature.IndexOf('(')
    $endParenIdx = $signature.LastIndexOf(')')
    if ($parenIdx -le 0 -or $endParenIdx -le $parenIdx) { return @() }
    
    $params = $signature.Substring($parenIdx+1, $endParenIdx - $parenIdx - 1)
    $results = @()
    
    foreach ($p in ($params -split ',')) {
        $pt = $p.Trim()
        if ($pt -match '^out\s+(.+?)\s+(\w+)\s*$') {
            $results = $results + @($Matches[2])
        } elseif ($pt -match '^out\s+(.+?)\s+(\w+)\s*=') {
            $results = $results + @($Matches[2])
        }
    }
    return $results
}

# Helper: Get default return value for a return type
function GetDefaultReturnValue {
    param([string]$returnType)
    
    $rt = $returnType.Trim()
    
    if ($rt -eq 'void' -or $rt -eq '') { return "" }
    if ($rt -eq 'bool') { return "false" }
    if ($rt -eq 'string') { return "null" }
    if ($rt -eq 'int') { return "0" }
    if ($rt -eq 'double') { return "0.0" }
    if ($rt -eq 'long') { return "0" }
    if ($rt -eq 'short') { return "0" }
    if ($rt -eq 'byte') { return "0" }
    if ($rt -eq 'decimal') { return "0" }
    if ($rt -eq 'float') { return "0.0f" }
    if ($rt -eq 'char') { return "'\\0'" }
    if ($rt -eq 'DateTime') { return "DateTime.MinValue" }
    if ($rt -eq 'TimeSpan') { return "TimeSpan.Zero" }
    if ($rt -eq 'Guid') { return "Guid.Empty" }
    
    # List<T>, Dictionary, etc.
    if ($rt -match 'List<') { return "new()" }
    if ($rt -match 'Dictionary<') { return "new()" }
    if ($rt -match 'HashSet<') { return "new()" }
    if ($rt -match 'Queue<') { return "new()" }
    if ($rt -match 'Stack<') { return "new()" }
    if ($rt -match 'ICollection<') { return "null" }
    if ($rt -match 'IEnumerable<') { return "null" }
    if ($rt -match 'IList<') { return "null" }
    if ($rt -match 'IDictionary<') { return "null" }
    
    # Array types
    if ($rt -match '\[\]') { return "null" }
    
    # Nullable value types (int?, double?, etc.)
    if ($rt -match '^(\w+)\?$') { return "null" }
    
    # All other reference types (User, Session, Cube, etc.) - null
    # Value types like struct would need default, but most model types are classes
    return "null"
}

# Helper: Get default assignment for an out param type
function GetOutParamDefault {
    param([string]$paramName, [string]$signature)
    
    # Look for "out type name" pattern
    $parenIdx = $signature.IndexOf('(')
    $endParenIdx = $signature.LastIndexOf(')')
    if ($parenIdx -le 0 -or $endParenIdx -le $parenIdx) { return "$paramName = 0;" }
    
    $params = $signature.Substring($parenIdx+1, $endParenIdx - $parenIdx - 1)
    
    foreach ($p in ($params -split ',')) {
        $pt = $p.Trim()
        if ($pt -match "out\s+(.+?)\s+$paramName") {
            $ptype = $Matches[1].Trim()
            $defaultVal = GetDefaultReturnValue $ptype
            if ($defaultVal -eq "") { $defaultVal = "0" }
            return "$paramName = $defaultVal;"
        }
    }
    return "$paramName = 0;"
}

# Check if method body has a try/catch that wraps from SqlConnection open to end
function AlreadyHasFullTryCatch {
    param(
        [string[]]$methodLines,  # method body lines AFTER the opening {
        [int]$indentLevel
    )
    
    $bodyText = $methodLines -join "`n"
    
    # Check for SqlConnection usage first
    if ($bodyText -notmatch 'SqlConnection\s+') { return $true }  # No connection = treat as wrapped (skip)
    
    # Now check pattern: skip leading variable declarations, then try { should appear
    # We need to check if the very first meaningful code (after var declarations) is a try block
    # that wraps the rest of the method
    
    $sawUsingOrCode = $false
    $sawTry = $false
    $tryDepth = -1
    
    # Simple heuristic: Look for "try {" appearing before "using (SqlConnection" 
    # and the corresponding catch appears near the end before the final }
    
    # Find first 'try {' token and check if it wraps everything
    $cleanBody = ($methodLines -join "`n").Trim()
    
    # Remove comments for analysis
    $noComments = $cleanBody
    # Remove /* */ comments (simplified)
    while ($noComments -match '/\*.*?\*/') {
        $noComments = $noComments -replace '/\*.*?\*/', ' '
    }
    # Remove // comments
    $noComments = $noComments -replace '//[^\n]*', ''
    
    # Check if try { is one of the first statements (before SqlConnection)
    $firstTryIdx = $noComments.IndexOf("try {")
    if ($firstTryIdx -lt 0) { $firstTryIdx = $noComments.IndexOf("try`n") }
    if ($firstTryIdx -lt 0) { $firstTryIdx = $noComments.IndexOf("try`r") }
    
    $sqlConnIdx = $noComments.IndexOf("SqlConnection")
    
    if ($firstTryIdx -ge 0 -and $sqlConnIdx -ge 0 -and $firstTryIdx -lt $sqlConnIdx) {
        # try appears before SqlConnection - check if catch appears near end
        # Find catch { near end
        $lastCatchIdx = $noComments.LastIndexOf("catch")
        $bodyLength = $noComments.Length
        
        if ($lastCatchIdx -ge 0 -and $lastCatchIdx -gt $sqlConnIdx) {
            # catch appears after connection, likely wrapping
            Write-Host "  -> Already has try/catch wrapping"
            return $true
        }
    }
    
    return $false
}

# Now process line by line
$i = 0
while ($i -lt $totalLines) {
    $line = $lines[$i]
    $trimmed = $line.Trim()
    
    # Look for public method declarations
    # Pattern: public [async] [static] returnType MethodName(params)
    if ($trimmed -match '^\s*public\s+(?:async\s+)?(?:static\s+)?(?:[a-zA-Z0-9_<>.,\[\]\s?]+)\s+([a-zA-Z0-9_]+)\s*\(') {
        
        # Is it the constructor? (Database)
        $methodName = $Matches[1]
        if ($methodName -eq 'Database') {
            $output.Add($line)
            $i++
            continue
        }
        
        # Check for closing paren on same line or continuation lines
        $sigStartLine = $i
        $sigEndLine = $i
        $sigLines = @($line)
        $parenDepth = 0
        for ($si = $i; $si -lt $totalLines; $si++) {
            $sl = $lines[$si]
            for ($ci = 0; $ci -lt $sl.Length; $ci++) {
                if ($sl[$ci] -eq '(') { $parenDepth++ }
                elseif ($sl[$ci] -eq ')') { 
                    $parenDepth--
                    if ($parenDepth -eq 0) {
                        $sigEndLine = $si
                        if ($si -gt $i) {
                            $sigLines = $lines[$i..$si]
                        }
                        break
                    }
                }
            }
            if ($parenDepth -eq 0) { break }
        }
        
        $fullSig = $sigLines -join " "
        
        # Now find opening { of the method
        $methodBodyStart = -1
        for ($mi = $sigEndLine + 1; $mi -lt [Math]::Min($sigEndLine+5, $totalLines); $mi++) {
            if ($lines[$mi].Trim() -match '^\s*\{') {
                $methodBodyStart = $mi
                break
            }
        }
        if ($methodBodyStart -lt 0) {
            # Check if it's on the same line as signature
            for ($mi = $sigEndLine; $mi -le $sigEndLine; $mi++) {
                if ($lines[$mi] -match '\{') {
                    $methodBodyStart = $mi
                    break
                }
            }
        }
        
        if ($methodBodyStart -lt 0) {
            $output.Add($line)
            $i++
            continue
        }
        
        # Find the char position of the open brace
        $openBraceChar = $lines[$methodBodyStart].IndexOf('{')
        if ($openBraceChar -lt 0) { $openBraceChar = 0 }
        
        # Find matching close brace for the method
        $closeResult = FindMatchingBrace $lines $methodBodyStart $openBraceChar
        $methodBodyEnd = $closeResult[0]
        $closeBraceChar = $closeResult[1]
        
        # Get the method body lines (between { and })
        if ($methodBodyEnd -le $methodBodyStart) {
            $output.Add($line)
            $i++
            continue
        }
        
        # Extract method content
        $fullMethodLines = $lines[$sigStartLine..$methodBodyEnd]
        $bodyInsideBraces = if ($methodBodyEnd -gt $methodBodyStart) { $lines[($methodBodyStart+1)..($methodBodyEnd-1)] } else { @() }
        $fullMethodText = $fullMethodLines -join "`n"
        
        # Check: does this method open a SqlConnection? (using new SqlConnection)
        $hasSqlConnection = $false
        if ($fullMethodText -match 'new\s+SqlConnection\s*\(') {
            $hasSqlConnection = $true
        }
        
        # Also check for methods that RECEIVE SqlConnection as parameter (like LogActivity) - skip those
        $isConnectionPassedIn = $false
        if ($fullSig -match 'SqlConnection\s+') {
            $isConnectionPassedIn = $true
        }
        
        # Get indent level - number of leading spaces on the method signature
        $indentStr = if ($line -match '^(\s*)') { $Matches[1] } else { "" }
        $oneMoreIndent = $indentStr + "    "
        $twoMoreIndent = $oneMoreIndent + "    "
        
        if (-not $hasSqlConnection -or $isConnectionPassedIn) {
            # Skip - no new SqlConnection, or receives it as param
            if ($hasSqlConnection) {
                Write-Host "SKIP (passes conn): $methodName at line $($sigStartLine+1)"
                $global:skippedNoConnection++
            } else {
                Write-Host "SKIP (no conn): $methodName at line $($sigStartLine+1)"
                $global:skippedNoConnection++
            }
            # Output unchanged
            for ($oi = $sigStartLine; $oi -le $methodBodyEnd; $oi++) {
                $output.Add($lines[$oi])
            }
            $i = $methodBodyEnd + 1
            $global:methodsProcessed++
            continue
        }
        
        # Check if already has full try/catch wrapping
        $alreadyWrapped = AlreadyHasFullTryCatch $bodyInsideBraces 4
        
        if ($alreadyWrapped) {
            Write-Host "ALREADY WRAPPED: $methodName at line $($sigStartLine+1)"
            $global:alreadyWrappedCount++
            # Output unchanged
            for ($oi = $sigStartLine; $oi -le $methodBodyEnd; $oi++) {
                $output.Add($lines[$oi])
            }
            $i = $methodBodyEnd + 1
            $global:methodsProcessed++
            continue
        }
        
        # NEEDS WRAPPING!
        Write-Host "WRAPPING: $methodName at line $($sigStartLine+1)"
        $global:wrappedCount++
        
        $returnType = GetReturnType $fullSig
        $actualMethodName = GetMethodName $fullSig
        $defaultRet = GetDefaultReturnValue $returnType
        $outParams = GetOutParams $fullSig
        
        # Write signature line(s)
        for ($oi = $sigStartLine; $oi -le $methodBodyStart; $oi++) {
            # The last line will be the one with { - we write lines before it normally, 
            # and handle the { with our wrapping
            if ($oi -lt $methodBodyStart) {
                $output.Add($lines[$oi])
            }
        }
        
        # Write the opening { of the method (may be on last sig line or separate)
        if ($methodBodyStart -ne $sigStartLine -and $lines[$methodBodyStart].Trim() -eq '{') {
            $output.Add($lines[$methodBodyStart])
        } elseif ($methodBodyStart -eq $sigStartLine) {
            # { on same line as sig - write sig with { on its own line
            $sigLine = $lines[$sigStartLine]
            if ($sigLine -match '^(.*?)\{\s*$') {
                $output.Add($Matches[1].TrimEnd())
                $output.Add("$indentStr{")
            } else {
                $output.Add($sigLine)
            }
        }
        
        # Write out param defaults BEFORE the try block
        foreach ($op in $outParams) {
            $defaultLine = GetOutParamDefault $op $fullSig
            $output.Add("$oneMoreIndent$defaultLine")
        }
        
        # Write try {
        $output.Add("$oneMoreIndent" + "try")
        $output.Add("$oneMoreIndent" + "{")
        
        # Write the existing method body inside the try, with one more level of indentation
        foreach ($bl in $bodyInsideBraces) {
            if ($bl.Trim() -eq '') {
                $output.Add('')  # Preserve empty lines
            } else {
                $output.Add($oneMoreIndent + $bl)
            }
        }
        
        # Close try, open catch
        $output.Add("$oneMoreIndent" + "}")
        $output.Add("$oneMoreIndent" + "catch (Exception ex)")
        $output.Add("$oneMoreIndent" + "{")
        $output.Add("$twoMoreIndent" + 'Global.logger?.LogMessageEx("Error", "DB {0}: {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name ?? "method", ex.Message);')
        
        # Write return statement
        if ($returnType -ne 'void' -and $returnType -ne '') {
            $output.Add("$twoMoreIndent" + "return $defaultRet;")
        }
        
        # Close catch
        $output.Add("$oneMoreIndent" + "}")
        
        # Close method
        $output.Add("$indentStr" + "}")
        
        $i = $methodBodyEnd + 1
        $global:methodsProcessed++
        continue
    }
    
    # Regular line - add as is
    $output.Add($line)
    $i++
}

# Write output
Write-Host ""
Write-Host "Writing output to $outputFile..."
[System.IO.File]::WriteAllLines($outputFile, $output)

Write-Host ""
Write-Host "====== SUMMARY ======"
Write-Host "Total methods processed: $global:methodsProcessed"
Write-Host "Methods wrapped (added try/catch): $global:wrappedCount"
Write-Host "Already had try/catch: $global:alreadyWrappedCount"
Write-Host "Skipped (no new SqlConnection): $global:skippedNoConnection"
Write-Host "Output lines: $($output.Count)"
Write-Host ""
Write-Host "Backup saved to: $backupFile"
Write-Host "New file saved to: $outputFile"
Write-Host ""
