<html>
<head>
    <meta charset="utf-8">
    <link href="cube_test_report.css" rel="stylesheet">
    <title>Cube Test Report</title>
    <meta name="description" content="Cube Test Report">
</head>

<header>
    <div id="title">TEST REPORT: COMPRESSION TEST ON CONCRETE CUBES</div>
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
    <div id="accreditation">
        <img src="footer.png" id="accred_img" />
    </div>
</footer>

<body>
    {{  s = 1
        for section in report.sections }}
    <table class="center_header">
        <tr>
            <td>SCO No.: {{ section.sco_num }}</td>
            <td>Test Date: {{ report.test_date }}</td>
            <td>Supplier: {{ section.supplier }}</td>
            <td>Specification: {{ section.specification }}</td>
            <td>Testing Standard: {{ section.test_standard }}</td>
        </tr>
    </table>
    <table width="100%" class="cubetable">
        <thead>
        <tr>
            <th>Set</th>
            <th>Batch</th>
            <th>Date Cast</th>
            <th>Age At Test</th>
            <th>Grade</th>
            <th>Cube Dimension<br>(mm)</th>
            <th>Concrete Type</th>
            <th>Average Strength<br>(N/mm2)</th>
            <th>Sample Ref</th>
            <th>Compressive Strength<br>At Failure (N/mm2)</th>
            <th>Mode of Failure</th>
        </tr>
        </thead>
        {{ i = 1
           for cs in section.cube_set_reports
                for b in cs.batch_reports }}
                <tr>
                    <td>{{ i++ }}</td>
                    <td>{{ b.batch.batch_num }}</td>
                    <td>{{ b.batch.casting_date | date.to_string "%F" }}</td>
                    <td>{{ b.batch.test_age }}</td>
                    <td>{{ cs.cube_set.concrete_grade }}</td>
                    <td>{{ b.batch.dimension }}</td>
                    <td>{{ cs.cube_set.concrete_type }}</td>
                    <td>{{ b.batch.avg_strength | math.format "0.0" }}</td>
                    <td>{{ for c in b.cubes }}
                            {{ c.sample_ref }}<br>
                        {{ end }}</td>
                    <td>{{ for c in b.cubes }}
                            {{ c.measured_strength | math.format "0.0" }}<br>
                        {{ end }}</td>
                    <td>{{ for c in b.cubes }}
                            {{ c.test_result == 1 ? "SATISFACTORY" : "NOT SATISFACTORY" }}<br>
                        {{ end }}</td>
                </tr>
                <tr>
                    <td colspan="11"><div style="float:left;">Client's Location: {{ cs.cube_set.location }}</div></td>
                </tr>
                {{ end }}
        {{ end }}
        </table>
    {{ end }}
</body>

</html>
