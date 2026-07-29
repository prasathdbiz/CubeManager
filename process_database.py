#!/usr/bin/env python3
# Database.cs Processor - Python version - fast and robust!
import re
import sys

input_file = r"c:\TRAE_PROJECT\PC_Updated Code\PC\CubeServer-v1.0.0\CubeServer\Data\Database.cs"
output_file = r"c:\TRAE_PROJECT\PC_Updated Code\PC\CubeServer-v1.0.0\CubeServer\Data\Database.cs.new"

with open(input_file, 'r', encoding='utf-8-sig') as f:
    lines = f.readlines()
total_lines = len(lines)
print(f"Read {total_lines} lines")

output_lines = []
wrapped_count = 0
already_count = 0
all_methods = 0
skipped_no_conn = 0

def find_matching_brace(start_line, start_char=0):
    """Find matching close brace - character-level with string/char handling."""
    depth = 0
    in_string = False
    in_char = False
    string_char = ''
    in_verbatim = False  # @"..." strings
    in_single_line_comment = False
    in_multi_line_comment = False
    
    for x in range(start_line, total_lines):
        line = lines[x]
        j = start_char if x == start_line else 0
        while j < len(line):
            ch = line[j]
            prev = line[j-1] if j > 0 else ''
            next_ch = line[j+1] if j+1 < len(line) else ''
            
            # Handle multi-line comment end
            if in_multi_line_comment:
                if ch == '*' and next_ch == '/':
                    in_multi_line_comment = False
                    j += 2
                    continue
                j += 1
                continue
            
            # Handle single-line comment
            if in_single_line_comment:
                j += 1
                continue
            
            # Check for comment start
            if ch == '/' and not in_string and not in_char:
                if next_ch == '/':
                    in_single_line_comment = True
                    j += 2
                    continue
                elif next_ch == '*':
                    in_multi_line_comment = True
                    j += 2
                    continue
            
            if not in_verbatim and ch == '\\' and in_string and not in_char:
                j += 2  # skip escaped char
                continue
            
            # Handle verbatim string @"..." 
            if ch == '@' and next_ch == '"' and not in_string and not in_char:
                in_string = True
                in_verbatim = True
                string_char = '"'
                j += 2
                continue
            
            # Handle quote
            if ch == '"' and not in_char:
                if in_verbatim:
                    # Verbatim string - look for "" as escape
                    if next_ch == '"':
                        j += 2
                        continue
                    else:
                        in_string = False
                        in_verbatim = False
                        string_char = ''
                else:
                    if in_string and string_char == '"':
                        in_string = False
                        string_char = ''
                    elif not in_string:
                        in_string = True
                        string_char = '"'
                j += 1
                continue
            
            if ch == "'" and not in_string:
                # Handle char literal - should only have one char but allow escapes
                if in_char:
                    in_char = False
                else:
                    in_char = True
                j += 1
                continue
            
            if not in_string and not in_char:
                if ch == '{':
                    depth += 1
                elif ch == '}':
                    depth -= 1
                    if depth == 0:
                        return (x, j)
            
            j += 1
        in_single_line_comment = False
    return (total_lines - 1, 0)


def get_ret_type_and_name(sig_lines_text):
    """Extract return type and method name from signature text."""
    s = sig_lines_text.strip()
    # Remove modifiers
    s = re.sub(r'^\s*public\s+', '', s)
    s = re.sub(r'^\s*async\s+', '', s)
    s = re.sub(r'^\s*static\s+', '', s)
    s = re.sub(r'^\s*unsafe\s+', '', s)
    s = re.sub(r'^\s*extern\s+', '', s)
    
    # Find the ( that starts parameter list (not return type tuple parens)
    param_paren_idx = -1
    paren_depth = 0
    for ci, c in enumerate(s):
        if c == '(':
            if paren_depth == 0 and ci > 0:
                param_paren_idx = ci
                break
            paren_depth += 1
        elif c == ')':
            if paren_depth > 0:
                paren_depth -= 1
    
    if param_paren_idx <= 0:
        return ("void", "Unknown")
    
    before_paren = s[:param_paren_idx].strip()
    # Method name is last identifier token
    match = re.search(r'(\w+)\s*$', before_paren)
    if match:
        mname = match.group(1)
        mn_idx = before_paren.rfind(mname)
        ret_type = before_paren[:mn_idx].strip() if mn_idx > 0 else ""
        return (ret_type, mname)
    return ("void", "Unknown")


def get_out_params(sig_lines_text):
    """Get list of out parameter names."""
    opi = sig_lines_text.find('(')
    cpi = sig_lines_text.rfind(')')
    if opi < 0 or cpi <= opi:
        return []
    pstr = sig_lines_text[opi+1:cpi]
    results = []
    for p in pstr.split(','):
        pt = p.strip()
        m = re.match(r'^out\s+(.+?)\s+(\w+)\s*$', pt)
        if m:
            results.append(m.group(2))
    return results


def default_ret(ret_type):
    """Get default return value literal for a return type."""
    r = ret_type.strip()
    if r == 'void' or r == '':
        return ""
    low = r.lower()
    if low == 'bool': return "false"
    if low == 'string': return "null"
    if low == 'int': return "0"
    if low == 'double': return "0.0"
    if low == 'long': return "0"
    if low == 'short': return "0"
    if low == 'byte': return "0"
    if low == 'decimal': return "0"
    if low == 'float': return "0.0f"
    if low == 'datetime': return "DateTime.MinValue"
    if low == 'timespan': return "TimeSpan.Zero"
    if low == 'guid': return "Guid.Empty"
    if re.search(r'List<', r, re.IGNORECASE): return "new()"
    if re.search(r'Dictionary<', r, re.IGNORECASE): return "new()"
    if re.search(r'HashSet<', r, re.IGNORECASE): return "new()"
    if re.search(r'Queue<', r, re.IGNORECASE): return "new()"
    if re.search(r'Stack<', r, re.IGNORECASE): return "new()"
    if '[]' in r: return "null"
    if re.match(r'^\w+\?$', r): return "null"
    if re.match(r'^\(.*\)$', r): return "null"  # tuple types
    return "null"  # all other ref types (User, Session, etc.)


def default_out_param(pn, sig_lines_text):
    """Get default assignment line for an out parameter."""
    opi = sig_lines_text.find('(')
    cpi = sig_lines_text.rfind(')')
    if opi < 0 or cpi <= opi:
        return f"{pn} = 0;"
    pstr = sig_lines_text[opi+1:cpi]
    for p in pstr.split(','):
        pt = p.strip()
        m = re.search(r'out\s+(.+?)\s+' + re.escape(pn), pt)
        if m:
            pt_str = m.group(1).strip()
            dv = default_ret(pt_str)
            if dv == "":
                dv = "0"
            return f"{pn} = {dv};"
    return f"{pn} = 0;"


# ===== MAIN PROCESSING =====
i = 0
while i < total_lines:
    line = lines[i]
    trimmed = line.lstrip()
    
    is_public_method = False
    if trimmed.startswith('public '):
        # Quick exclusion checks
        is_field = line.rstrip().endswith(';')
        is_enum = ' enum ' in line or re.match(r'^\s*public\s+enum\s', line)
        is_class = ' class ' in line or re.match(r'^\s*public\s+(?:partial\s+)?(?:sealed\s+)?(?:abstract\s+)?class\s', line)
        is_struct = ' struct ' in line or re.match(r'^\s*public\s+(?:partial\s+)?struct\s', line)
        is_interface = ' interface ' in line or re.match(r'^\s*public\s+(?:partial\s+)?interface\s', line)
        is_delegate = ' delegate ' in line or re.match(r'^\s*public\s+delegate\s', line)
        
        if not (is_field or is_enum or is_class or is_struct or is_interface or is_delegate):
            # Check if it has ( anywhere in next 10 lines
            found_paren = False
            search_end = min(i + 10, total_lines)
            for si in range(i, search_end):
                if '(' in lines[si]:
                    found_paren = True
                    break
            if found_paren:
                is_public_method = True
    
    if not is_public_method:
        output_lines.append(line)
        i += 1
        continue
    
    # Found a public method candidate!
    # Collect signature - find matching close paren from first open paren
    sig_start = i
    first_paren_line = -1
    first_paren_char = -1
    for si in range(i, min(i + 10, total_lines)):
        ci = lines[si].find('(')
        if ci >= 0:
            first_paren_line = si
            first_paren_char = ci
            break
    
    if first_paren_line < 0:
        output_lines.append(line)
        i += 1
        continue
    
    # Find matching close paren starting from first paren position
    sig_end = first_paren_line
    pdepth = 0
    started_p = False
    for si in range(first_paren_line, min(first_paren_line + 15, total_lines)):
        ln = lines[si]
        cstart = first_paren_char if si == first_paren_line else 0
        for ci in range(cstart, len(ln)):
            cc = ln[ci]
            if cc == '(':
                pdepth += 1
                started_p = True
            elif cc == ')':
                if started_p:
                    pdepth -= 1
                    if pdepth == 0:
                        sig_end = si
                        break
        if started_p and pdepth == 0:
            break
    
    sig_lines_list = lines[sig_start:sig_end+1]
    sig_text = ' '.join(l.rstrip('\n').rstrip('\r') for l in sig_lines_list)
    
    # Skip constructor
    rt_and_mn = get_ret_type_and_name(sig_text)
    ret_type = rt_and_mn[0]
    mname = rt_and_mn[1]
    
    if mname == "Database" or mname == "Unknown":
        # Constructor or unrecognized - output as-is
        # But find actual body to skip correctly
        # Find { next
        body_s = -1
        for bsi in range(sig_end + 1, min(sig_end + 6, total_lines)):
            if re.match(r'^\s*\{', lines[bsi].lstrip()):
                body_s = bsi
                break
        if body_s < 0 and '{' in lines[sig_end]:
            body_s = sig_end
        
        if body_s >= 0:
            body_e, _ = find_matching_brace(body_s)
            for oi in range(sig_start, min(body_e + 1, total_lines)):
                output_lines.append(lines[oi])
            i = body_e + 1
            continue
        else:
            output_lines.append(line)
            i += 1
            continue
    
    # Make sure params exist and not something like field
    if not re.search(r'\w\s*\(', sig_text):
        output_lines.append(line)
        i += 1
        continue
    
    all_methods += 1
    
    # Find method body opening {
    body_start = -1
    for bsi in range(sig_end + 1, min(sig_end + 6, total_lines)):
        if re.match(r'^\s*\{', lines[bsi].lstrip()):
            body_start = bsi
            break
    if body_start < 0 and '{' in lines[sig_end]:
        body_start = sig_end
    
    if body_start < 0:
        # Something weird - output lines and move on
        for oi in range(sig_start, min(sig_end + 3, total_lines)):
            output_lines.append(lines[oi])
        i = min(sig_end + 3, total_lines)
        continue
    
    # Find matching } for the method body
    body_end_result = find_matching_brace(body_start)
    body_end = body_end_result[0]
    
    if body_end <= body_start:
        body_end = min(body_start + 150, total_lines - 1)
    
    # Full method content
    full_meth_lines = lines[sig_start:body_end+1]
    full_meth_text = ''.join(full_meth_lines)
    body_inside = lines[body_start+1:body_end] if body_end > body_start + 1 else []
    
    # Check for new SqlConnection in method (not just parameter)
    has_new_connection = bool(re.search(r'new\s+SqlConnection\s*\(', full_meth_text))
    # Check if SqlConnection is passed IN as parameter
    conn_in_param = bool(re.search(r'SqlConnection\s+\w+', sig_text))
    
    # Get indent
    indent_match = re.match(r'^(\s*)', line)
    indent_str = indent_match.group(1) if indent_match else ""
    indent1 = indent_str + "    "
    indent2 = indent1 + "    "
    
    if not has_new_connection or conn_in_param:
        skipped_no_conn += 1
        # Output unchanged
        for oi in range(sig_start, body_end + 1):
            output_lines.append(lines[oi])
        i = body_end + 1
        continue
    
    # Check for existing full wrapping: try appears before new SqlConnection
    body_flat = ''.join(body_inside)
    # Remove comments for analysis
    nc = body_flat
    nc = re.sub(r'/\*[\s\S]*?\*/', ' ', nc)
    nc = re.sub(r'//[^\n]*', '', nc)
    
    already_wrapped = False
    try_idx = nc.find("try {")
    if try_idx < 0:
        try_idx = nc.find("try\n")
    if try_idx < 0:
        try_idx = nc.find("try\r")
    if try_idx < 0:
        try_idx = nc.find("try\t{")
    conn_idx = nc.find("new SqlConnection")
    
    if try_idx >= 0 and conn_idx >= 0 and try_idx < conn_idx:
        catch_idx = nc.rfind("catch")
        if catch_idx >= 0 and catch_idx > conn_idx:
            already_wrapped = True
    
    if already_wrapped:
        print(f"ALREADY: {mname} line {sig_start+1} [{ret_type}]")
        already_count += 1
        for oi in range(sig_start, body_end + 1):
            output_lines.append(lines[oi])
        i = body_end + 1
        continue
    
    # ========= WRAP THE METHOD =========
    print(f"WRAP: {mname} line {sig_start+1} [{ret_type}]")
    wrapped_count += 1
    
    dr = default_ret(ret_type)
    out_params = get_out_params(sig_text)
    
    # 1. Output signature lines
    for oi in range(sig_start, body_start):
        output_lines.append(lines[oi])
    
    # 2. Output method opening brace
    if body_start != sig_start:
        output_lines.append(lines[body_start])
    else:
        # Brace on same line as signature
        sl = lines[sig_start].rstrip('\n').rstrip('\r')
        m = re.match(r'^(.*?)\{\s*$', sl)
        if m:
            output_lines.append(m.group(1).rstrip() + '\n')
            output_lines.append(indent_str + "{\n")
        else:
            output_lines.append(lines[sig_start])
    
    # 3. Out parameter defaults before try
    for op in out_params:
        def_line = default_out_param(op, sig_text)
        output_lines.append(indent1 + def_line + '\n')
    
    # 4. try {
    output_lines.append(indent1 + "try\n")
    output_lines.append(indent1 + "{\n")
    
    # 5. Original body inside try with extra indentation
    for bl in body_inside:
        bt = bl.rstrip('\n').rstrip('\r')
        if bt.strip() == '':
            output_lines.append('\n')
        else:
            output_lines.append(indent1 + bt + '\n')
    
    # 6. Close try, open catch
    output_lines.append(indent1 + "}\n")
    output_lines.append(indent1 + "catch (Exception ex)\n")
    output_lines.append(indent1 + "{\n")
    output_lines.append(indent2 + 'Global.logger?.LogMessageEx("Error", "DB {0}: {1}", System.Reflection.MethodBase.GetCurrentMethod()?.Name ?? "method", ex.Message);\n')
    if ret_type != 'void' and ret_type != '' and dr != '':
        output_lines.append(indent2 + f"return {dr};\n")
    output_lines.append(indent1 + "}\n")
    
    # 7. Close method
    output_lines.append(indent_str + "}\n")
    
    i = body_end + 1

# Write output
print("Writing...")
with open(output_file, 'w', encoding='utf-8') as f:
    f.writelines(output_lines)

print(f"\n===== RESULTS =====")
print(f"Total public methods: {all_methods}")
print(f"Methods wrapped (NEW try/catch): {wrapped_count}")
print(f"Already had try/catch: {already_count}")
print(f"Skipped (no new SqlConnection): {skipped_no_conn}")
print(f"Output lines: {len(output_lines)}")
print("DONE!")
