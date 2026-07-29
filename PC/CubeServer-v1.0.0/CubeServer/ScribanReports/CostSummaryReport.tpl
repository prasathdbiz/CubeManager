<html>
<head>
    <meta charset="utf-8">
    <link href="cost_summary_report.css" rel="stylesheet">
    <title>Cost Summary Report</title>
    <meta name="description" content="Cost Summary Report">
</head>

<header>
    <div id="title">COST SUMMARY FOR CONCRETE CUBE TESTING</div>
    <table id="top_header">
        <tr>
            <td>
                <table id="left_header">
                    <tr>
                        <td>Report Period:</td>
                        <td>{{ report.start_date | date.to_string "%F" }} &ndash; {{ report.end_date | date.to_string "%F" }}</td>
                    </tr>
                </table>
            </td>
            <td>
                <table id="right_header">
                    <tr>
                        <td>
                            <img src="tuvsud_logo.png" id="logo" />
                        </td>
                        <td>
                            <b>TÜV SÜD PSB Pte. Ltd.</b><br />
                            60 Tuas South Street 1<br />
                            Singapore 639925<br />
                            Business Reg No. 199002667R<br />
                            <br />
                            Tel: 6240 0217<br />
                            Fax: 6240 0201
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div style="padding-top:10px"></div>

</header>

<body>    
    <table width="100%" class="costtable">
        <tr>
            <th>S/No</th>
            <th>Client Name</th>
            <th>Project ID</th>
            <th>Total Sets</th>
            <th>Total Cubes</th>
            <th>Unit Price</th>
            <th>Cost</th>
        </tr>
        {{ i = 1
           for pr in report.client_projs }}
            <tr>
                <td rowspan="{{ pr.value.size }}">{{ i++ }}</td>
                <td rowspan="{{ pr.value.size }}">{{ pr.key }}</td>
           {{ for p in pr.value }}
                <td>{{ p.project.project_code }}</td>
                <td>{{ p.cube_sets.size }}</td>
                <td>{{ p.cubes.size }}</td>
                <td>&dollar;{{ p.project.price_per_cube | math.format "N2" }}</td>
                <td>&dollar;{{ p.project.price_per_cube * p.cubes.size | math.format "N2" }}</td>
            </tr>
           {{ end }}
        {{ end }}
    </table>
</body>
<footer>
    
</footer>
</html>