<html>
<head>
    <meta charset="utf-8">
    <link href="monthly_report.css" rel="stylesheet">
    <title>Monthly Report</title>
    <meta name="description" content="Monthly Summary Report">
</head>

<header>
    <div id="title">Concrete Cube Testing Monthly Report ({{ report.start_date | date.to_string "%F" }} &ndash; {{ report.end_date | date.to_string "%F" }})</div>
    <table id="top_header">
        <tr>
            <td>
                <table id="left_header">
                    <tr>
                        <td>PROJECT ID:</td>
                        <td>{{ report.project_id }}</td>
                    </tr>
                    <tr>
                        <td>PROJECT:</td>
                        <td>{{ report.project_name }}</td>
                    </tr>
                    <tr>
                        <td>COMPANY:</td>
                        <td>{{ report.company_name }}</td>
                    </tr>
                    <tr>
                        <td>ADDRESS:</td>
                        <td>{{ report.company_address }}</td>
                    </tr>
                    <tr>
                        <td>ATTENTION:</td>
                        <td>{{ report.attn_to }}</td>
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
<footer>
    <div class="remarks">
        <table>
            <tr>
                <td><b>Remarks:</b> </td>
                <td>
                    P - Comply With Specification, F - Does Not Comply With Specification
                </td>
            </tr>
        </table>
    </div>
</footer>

<body>
    
    {{  s = 1
        for section in report.sections }}
    <table class="center_header">
        <tr>
            <td>SCO No.: {{ section.sco_num }}</td>
            <td>Supplier: {{ section.supplier }}</td>
            <td>Specification: {{ section.specification }}</td>
            <td>Testing Standard: {{ section.test_standard }}</td>
            <td>Grade: {{ section.concrete_grade }}</td>
        </tr>
    </table>
    <table width="100%" class="cubetable">
        <thead>
        <tr>
            <th>Set</th>
            <th>Test Date</th>
            <th>Batch</th>
            <th>Sample Ref</th>
            <th>Date Cast</th>
            <th>Age At Test</th>
            <th>Cube Count</th>
            <th>Dimension<br>(mm)</th>
            <th>Average Strength<br>(N/mm2)</th>
            <th>Rolling Avg Strength<br>(N/mm2)</th>
            <th>Criteria<br>B&emsp;A</th>
        </tr>
        </thead>
        {{ i = 1
           for cs in section.cube_set_reports
                for b in cs.batch_reports }}
                <tr>
                    <td>{{ i++ }}</td>
                    <td>{{ b.cubes[0].actual_test_date | date.to_string "%F" }}</td>
                    <td>{{ b.batch.batch_num }}</td>
                    <td>{{ b.cubes[0].sample_ref }}</td>
                    <td>{{ b.batch.casting_date | date.to_string "%F" }}</td>
                    <td>{{ (b.cubes[0].actual_test_date - b.batch.casting_date).days}}</td>
                    <td>{{ b.cubes.size }}</td>
                    <td>{{ b.batch.dimension }}</td>
                    <td>{{ b.batch.avg_strength | math.format "0.0" }}</td>
                    {{ if b.batch.rolling_avg_strength == 0 }}
                    <td> &mdash; </td>
                    {{ else }}
                    <td>{{ b.batch.rolling_avg_strength | math.format "0.0" }}</td>
                    {{ end }}
                    <td>{{ b.batch.criterion_a }} &emsp; {{ b.batch.criterion_b }}</td>
                </tr>
             {{ end }}
        {{ end }}
    </table>
    <table class="subtotal">
        <tr>
            <td>SUB TOTAL</td>
            <td>Total Sets: {{ section.cube_set_reports.size }}</td>
            <td>Total No. of Cubes: {{ section.cube_cnt }}</td>
        </tr>
    </table>
    {{ if s++ < report.sections.size }}
        <p style="page-break-before: always" ></p>
    {{ end }}
 {{ end }}
</body>

</html>