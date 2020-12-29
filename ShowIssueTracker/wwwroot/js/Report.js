var testHTML = '<html xmlns:v="urn:schemas-microsoft-com:vml" xmlns:o="urn:schemas-microsoft-com:office:office"' +
    'xmlns:w="urn:schemas-microsoft-com:office:word" xmlns:m="http://schemas.microsoft.com/office/2004/12/omml"' +
    'mlns="http://www.w3.org/TR/REC-html40">' +
    '<head><meta http-equiv=Content-Type content="text/html; charset=utf-8"><title></title>' +
    '<style>' +
    '@page' +
    '{' +
    '  border:solid navy 3.25pt; padding:24.0pt 24.0pt 24.0pt 24.0pt;' +
    '}' +
    '@page Section1 {' +
    '    border:solid navy 2.25pt; padding:24.0pt 24.0pt 24.0pt 24.0pt;' +
    '    mso-header-margin:.5in;' +
    '    mso-footer-margin:.5in;' +
    '    mso-header: h1;' +
    '    mso-footer: f1;' +
    '    }' +
    'div.Section1 { page:Section1; }' +
    'table#hrdftrtbl' +
    '{' +
    '    margin:0in 0in 0in 900in;' +
    '    width:1px;' +
    '    height:1px;' +
    '    overflow:hidden;' +
    '}' +
    'h4 {font-family: "Calibri (Body) (Body)"; font-size: 14pt;}    table{ border - collapse: collapse; width:100%;}' +
    'p.MsoFooter, li.MsoFooter, div.MsoFooter' +
    '{' +
    '    margin:0in;' +
    '    margin-bottom:.0001pt;' +
    '    mso-pagination:widow-orphan;' +
    '    tab-stops:center 3.0in right 6.0in;' +
    '    font-size:12.0pt;' +
    '}' +
    '</style>' +
    '<xml>' +
    '<w:WordDocument>' +
    '<w:View>Print</w:View>' +
    '<w:Zoom>75</w:Zoom>' +
    '<w:DoNotOptimizeForBrowser/>' +
    '</w:WordDocument>' +
    '</xml>' +
    '</head>' +
    '<body>' +
    '<div class="Section1">' +
    '    <p>####CONTENT####</p>' +
    '<br/>' +
    '    <table id="hrdftrtbl" border="0" cellspacing="0" cellpadding="0">' +
    '    <tr><td><div style="mso-element:header" id="h1" >' +
    '            <p class=MsoHeader >####HEADER####</p>' +
    '        </div>' +
    '    </td>' +
    '    <td>' +
    '   </span>' +
    '        </p>' +
    '    </div>' +
    '    <div style="mso-element:header" id="fh1">' +
    '        <p class="MsoHeader"><span lang="EN-US" style="mso-ansi-language:EN-U"></span></p>' +
    '        </div>' +
    '        <div style="mso-element:footer" id="ff">' +
    '        <p class="MsoFooter"><span lang="EN-US" style="mso-ansi-language:EN-US">&nbsp;<o:p></o:p></span></p>' +
    '    </div>' +
    '    </td></tr>' +
    '    </table>' +
    '</div>' +
    '</body></html>';
$(document).ready(function () {
    if (!window.Blob) {
        alert('Your legacy browser does not support this action.');
        return;
    }

    generateReport()
});

function generateReport() {
    $('#content').html("");
    var meetingId = $('#meetingId').val();

    $.ajax({
        url: "/api/Reports/GetIDCReport/" + meetingId,
        type: 'GET',
        contentType: 'application/json',
        dataType: 'json'
    }).done(function (result) {
        if (result.success === false) {
            swal("Error!", result.value, "error");
            return;
        }
        //Report Header
        var headerdiv = ' <div style="mso-element: header" id=h1>     <p class=MsoHeader><span style="mso-tab - count: 1"></span>  <span style="mso-field - code:PAGE"></span><div class="row" id="rptHeader"></div>';
        var head = '<p></p><p></p><div class="col s12" style="text-align:center;"><img src="https://ShowIssueTracker.azurewebsites.net/NADA.png" title="logo" align="left" /><p style="font-family:Calibri (Body);color:#002060;font-size:25.0pt;">'
            + result.meeting.dealership + '</p></div><p></p><p></p>' +
             '</p> </div>';



        var header = $(headerdiv).html(head);

        // $('#content').append(header);

        // Report Body
        var bodydiv = '<div class="row" id="rptBody"><div class="col s12" id="rptContetnArea"></div></div>';
        var bodycontent = '';

 
        bodycontent += '<div class="row"><span style="font-family:Calibri (Body);color:#002060;font-weight:bold;font-size:12.0pt;">'
            + 'Follow-Up Report' + '</span> <br/>';

        bodycontent += '<span style="font-family:Calibri (Body);color: #002060; font-size:12.0pt;">In-Dealership Consulting</span><br/>' +
            '<span style="font-family:Calibri (Body);color: #002060; font-size:12.0pt;">'
            + formatDate2(result.meeting.meetingDate) + ' - ' + formatDate2(result.meeting.reportDate) +
            '</span><br/>' +
            '<span style="font-family:Calibri (Body);color: #002060; font-size:12.0pt;">Visit '
            + result.meeting.meetingNo + '</span><br/></div>';



        if (result.meeting.personalMessage === "" || result.meeting.personalMessage === undefined || result.meeting.personalMessage === 'NaN' || !isNaN(result.meeting.personalMessage))
        {

            bodycontent += '<p><span style="font-family:Calibri (Body);color: #002060;font-size:12.0pt;">'
                + 'Sir' + '</span></p>';

            bodycontent += '<p><span style="font-family:Calibri (Body);color: #002060;font-size:12.0pt;">'
                + 'It was a pleasure working with you and your staff. This is a follow-up letter for my visit to your store to make sure everyone is on the same page going forward.'
                + '</span></p>';


        }

        else {
            bodycontent += '<div style="font-family:Calibri (Body);color: #002060;font-size:12.0pt;">'
                + result.meeting.personalMessage + '</div>';
            
        }

        /*   bodycontent += '<p><span style="font-family:Calibri (Body);color: #002060;font-weight: bold; font-size: 14.0pt;">'
               + 'Overview' + '</span></p>';*/

        bodycontent += '<p style="font-family:Calibri (Body);"><u><span style="font-family:Calibri (Body);color: #002060;font-weight:bold;font-size:14.0pt;">'
            + 'Variable Departments:' + '</span></u></p>';

        bodycontent += '<div style="font-family:Calibri (Body);color: #002060;font-size:12.0pt;"> '
            + result.meeting.fbVariableOperations + ' </div>';

        bodycontent += '<p style="font-family:Calibri (Body);"><u><span style="font-family:Calibri (Body);color: #002060;font-weight:bold;font-size:14.0pt;">'
            + 'Fixed Departments:' + '</span></u></p>';

        bodycontent += '<div style="font-family:Calibri (Body);color: #002060; font-size:12.0pt;">'
            + result.meeting.fbFixedOperations + '</div>';

        bodycontent += '<p style="font-family:Calibri (Body);"><span style="font-family:Calibri (Body);color: #002060;font-size:11.0pt;">'
            + 'Thank you,' + '</span></p > ';

        bodycontent += '<p style="margin-top:0in;margin-right:0in;margin-bottom:6.0pt;margin-left:0in;mso-add-space:auto;font-family:Calibri (Body);">'
            + '<b><span style="font-family:Calibri (Body);font-size: 12.0pt;color: #333333;">'
            + result.meeting.consultant + '</span></b></p>';

        bodycontent += '<p style="margin-top:0in;margin-right:0in;margin-bottom:6.0pt;margin-left:0in;mso-add-space:auto;font-family:Calibri (Body);">'
            + '<b><span style="font-family:Calibri (Body);font-size: 12.0pt;color:#7F7F7F;">'
            + 'Dealership Management Consultant 20 Group' + '</span></b></p>';

        bodycontent += '<p style="margin-top:0in;margin-right:0in;margin-bottom:6.0pt;margin-left:0in;mso-add-space:auto;font-family:Calibri (Body);">'
            + '<b><span style="font-family:Calibri (Body);font-size: 12.0pt;color:#7F7F7F;">'
            + 'National Automobile Dealers Association' + '</span></b></p>';

        if (result.consultant.email) {
            bodycontent += '<p style="margin-top:0in;margin-right:0in;margin-bottom:6.0pt;margin-left:0in;mso-add-space:auto;font-family:Calibri (Body);">'
                + '<a href="mailto:' + result.consultant.email + '">'
                + '<span style="font-family:Calibri (Body);font-size:10.0pt;color:#0563C1;">'
                + result.consultant.email + '</span></a></p > ';
        }

        if (result.consultant.workPhone) {
            bodycontent += '<p style="margin-top:0in;margin-right:0in;margin-bottom:6.0pt;margin-left:0in;mso-add-space:auto;font-family:Calibri (Body);">'
                + '<span style="font-family:Calibri (Body);font-size:10.0pt;color:#333333;">'
                + 'o ' + result.consultant.workPhone + '</span></p>';
        }

        if (result.consultant.mobilePhone) {
            bodycontent += '<p style="margin-top:0in;margin-right:0in;margin-bottom:6.0pt;margin-left:0in;mso-add-space:auto;font-family:Calibri (Body);">'
                + '<span style="font-family:Calibri (Body);font-size:10.0pt;color:#333333;">'
                + 'm ' + result.consultant.mobilePhone + '</span></p>';
        }

        if (result.consultant.fax) {
            bodycontent += '<p style="margin-top:0in;margin-right:0in;margin-bottom:6.0pt;margin-left:0in;mso-add-space:auto;font-family:Calibri (Body);">'
                + '<span style="font-family:Calibri (Body);font-size:10.0pt;color:#333333;">'
                + 'f ' + result.consultant.fax + '</span></p>';
        }

        bodycontent += '<p style="margin-top:0in;margin-right:0in;margin-bottom:6.0pt;margin-left:0in;mso-add-space:auto;">'
            + '<span style="font-family: Calibri (Body); font-size: 10.0pt; color: #333333;">8484 Westpark Drive</span></p>'
            + '<p style="margin-top:0in;margin-right:0in;margin-bottom:6.0pt;margin-left:0in;mso-add-space:auto;">'
            + '<span style="font-family:Calibri (Body);font-size:10.0pt;color:#333333;">Tysons, VA 22102</span></p>'
            + '<p style="margin-top:0in;margin-right:0in;margin-bottom:6.0pt;margin-left:0in;mso-add-space:auto;">'
            + '<b><span style="font-family:Calibri (Body);font-size:10.0pt;color:#7F7F7F;"><a href="https://www.nada.org/">nada.org</a></span></b></p>';

        // ActionItems
        var count = 1;
        var currentActionItem = "";
        if (result.actionItems !== undefined && result.actionItems.length > 0) {

            bodycontent += '<p></p>'
                + '<p align="center" style="text-align:center;font-family:Calibri (Body);font-size:16.0pt;">'
                + '<span style="font-family:Calibri (Body);color: #002060;font-weight: bold;  " class="break">Action Plans</span></p>';

            var tableHeaderCols = '<th valign="top" style=" width:1%;  padding: 0in 1.4pt;border:solid 1px;font-size:16.0pt;">'
                + '<p align="center" style="text-align:center;"><b><span style=" font-family:Calibri (Body);color:#002060;">Completed</span></b></p></th>';

            tableHeaderCols += '<th valign="top" style="width:1%;  padding: 0in 1.4pt;border:solid 1px;font-size:16.0pt;">'
                + '<p align = "center" style = "text-align:center;" > <b><span style=" font-family:Calibri (Body);color:#002060">Who</span></b></p></th>';

            tableHeaderCols += '<th valign="top" style=" width:10%;  padding: 0in 1.4pt;border:solid 1px;font-size:16.0pt;">'
                + '<p align="center" style="text-align:center;"><b><span style=" font-family:Calibri (Body);color:#002060">When</span></b></p></th>';

            /* tableHeaderCols += '<th valign="top" style="width:10%;   padding: 0in 1.4pt;border:solid 1px;">'
                 + '<p align="center" style="text-align:center;"><b><span style="font-size:12.0pt;font-family:Calibri (Body);color:#002060">Type</span></b></p></th>'; */

            tableHeaderCols += '<th valign="top" style=" width:88%;  padding: 0in 1.4pt;border:solid 1px; font-size:16.0pt;">'
                + '<p align="center" style="text-align:center;"><b><span style="font-family:Calibri (Body);color:#002060">What&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></b></p></th>';



            /*  var tableHeaderCols = '<th valign="top" style="  padding: 0in 1.4pt;border:solid 1px;">'
                  + '<p align="center" style="text-align:center;"><b><span style="font-size:14.0pt;font-family:Calibri (Body);color:#002060;">#</span></b></p></th>';*/

            /*   tableHeaderCols += '<th valign="top" style="  padding: 0in 1.4pt;border:solid 1px;">'
        + '<p align="center" style="text-align:center;"><b><span style="font-size:14.0pt;font-family:Calibri (Body);color:#002060">After</span></b></p></th>';
        */

            var tableHeaderRow = '<tr  class="calibre3" style="background-color: #EDEDED;">' + tableHeaderCols + '</tr>';

            var tableContentRows = ''; var tableContentRowsBody = ''; var tableContentRowsFI = ''; var tableContentRowsFixed = ''; var tableContentRowsMisc = '';
            var tableContentRowsAccounting = ''; var tableContentRowsNew = ''; var tableContentRowsParts = ''; var tableContentService = ''; var tableContentRowsUsed = ''; var tableContentRowsVariable = '';
            $.each(result.actionItems, function (index, actionItem) {

                var deptSelected = actionItem.department.name;
                switch (actionItem.department.name) {
                    case "Accounting":

                        var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';
                        /* tableContentRows += '<td style="width:1%; padding: 0in 1.4pt;border:solid 1px;">'
                             + '<p align="center" style="text-align:center;">'
                             + '<b><span style="font-size:12.0pt;font-family:Calibri (Body);">' + count + '</span></b></p></td>';*/
                        tableContentRowsAccounting += ' <tr>';
                        tableContentRowsAccounting += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="border:2px solid black;width:25px;height:15px;">' + completed + '</span> </b></p></td>';


                        // Action Items from selected meeting
                        if (actionItem.meeting.id === meetingId) {

                            tableContentRowsAccounting += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who.replace(/style="[^"]*"/, "")
                                + '</div></td>';

                            tableContentRowsAccounting += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';


                            /*  tableContentRowsAccounting += '<td style="width:10%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                  + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                  + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                  + actionItem.department.name
                                  + '</span></p></td>'; */

                            tableContentRowsAccounting += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsAccounting += '<td style="  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                        }
                        // Unfinished Action Items from previous meetings (Display in red)
                        else {
                            tableContentRowsAccounting += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who
                                + '</div></td>';

                            tableContentRowsAccounting += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';
                            /*tableContentRowsAccounting += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + actionItem.department.name
                                + '</span></p></td>';*/

                            tableContentRowsAccounting += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsAccounting += '<td style=" padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after  //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                           
                        }
                        tableContentRowsAccounting += '</tr>';
                        break;
                    case "Body Shop":
                        var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';
                        /* tableContentRows += '<td style="width:1%; padding: 0in 1.4pt;border:solid 1px;">'
                             + '<p align="left" style="text-align:left;">'
                             + '<b><span style="font-size:12.0pt;font-family:Calibri (Body);">' + count + '</span></b></p></td>';*/
                        tableContentRowsBody += ' <tr>';
                        tableContentRowsBody += '  < td style = " width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);" > '
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="border:2px solid black;width:25px;height:15px;">' + completed + '</span> </b></p></td>';

                        // Action Items from selected meeting
                        if (actionItem.meeting.id == meetingId) {

                            tableContentRowsBody += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who.replace(/style="[^"]*"/, "")
                                + '</div></td>';

                            tableContentRowsBody += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';


                            /*   tableContentRowsBody  += '<td style="width:10%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                   + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                   + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                   + actionItem.department.name
                                   + '</span></p></td>'; */

                            tableContentRowsBody += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsBody  += '<td style="  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                        }
                        // Unfinished Action Items from previous meetings (Display in red)
                        else {
                            tableContentRowsBody += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who
                                + '</div></td>';

                            tableContentRowsBody += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';
                            /*tableContentRowsBody  += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.department.name
                                 + '</span></p></td>';*/

                            tableContentRowsBody += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsBody  += '<td style=" padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after  //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                           
                        }
                        tableContentRowsBody += '</tr>';
                        break;
                    case "F&I":
                        var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';
                        /* tableContentRows += '<td style="width:1%; padding: 0in 1.4pt;border:solid 1px;">'
                             + '<p align="left" style="text-align:left;">'
                             + '<b><span style="font-size:12.0pt;font-family:Calibri (Body);">' + count + '</span></b></p></td>';*/
                        tableContentRowsFI += ' <tr>';
                        tableContentRowsFI += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="border:2px solid black;width:25px;height:15px;">' + completed + '</span> </b></p></td>';

                        // Action Items from selected meeting
                        if (actionItem.meeting.id === meetingId) {

                            tableContentRowsFI += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who.replace(/style="[^"]*"/, "")
                                + '</div></td>';

                            tableContentRowsFI += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';


                            /* tableContentRowsFI  += '<td style="width:10%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                 + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.department.name
                                 + '</span></p></td>'; */

                            tableContentRowsFI += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsFI  += '<td style="  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                        }
                        // Unfinished Action Items from previous meetings (Display in red)
                        else {
                            tableContentRowsFI += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who
                                + '</div></td>';

                            tableContentRowsFI += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';
                            /* tableContentRowsFI  += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                  + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                  + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                  + actionItem.department.name
                                  + '</span></p></td>';*/

                            tableContentRowsFI += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsFI  += '<td style=" padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after  //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                            tableContentRowsFI += '</tr>';
                        }
                        tableContentRowsFI += '</tr>';
                        break;
                    case "Fixed Ops":
                        var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';
                        /* tableContentRows += '<td style="width:1%; padding: 0in 1.4pt;border:solid 1px;">'
                             + '<p align="left" style="text-align:left;">'
                             + '<b><span style="font-size:12.0pt;font-family:Calibri (Body);">' + count + '</span></b></p></td>';*/
                        tableContentRowsFixed += ' <tr>';
                        tableContentRowsFixed += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="border:2px solid black;width:25px;height:15px;">' + completed + '</span> </b></p></td>';

                        // Action Items from selected meeting
                        if (actionItem.meeting.id === meetingId) {

                            tableContentRowsFixed += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who.replace(/style="[^"]*"/, "")
                                + '</div></td>';

                            tableContentRowsFixed += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';


                            /*  tableContentRowsFixed  += '<td style="width:10%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                  + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                  + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                  + actionItem.department.name
                                  + '</span></p></td>'; */

                            tableContentRowsFixed += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsFixed  += '<td style="  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                        }
                        // Unfinished Action Items from previous meetings (Display in red)
                        else {
                            tableContentRowsFixed += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who
                                + '</div></td>';

                            tableContentRowsFixed += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';
                            /* tableContentRowsFixed  += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.department.name
                                 + '</span></p></td>';*/

                            tableContentRowsFixed += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsFixed  += '<td style=" padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after  //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/
 
                        }
                        tableContentRowsFixed += '</tr>';
                        break;
                    case "Misc":
                        var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';
                        /* tableContentRows += '<td style="width:1%; padding: 0in 1.4pt;border:solid 1px;">'
                             + '<p align="left" style="text-align:left;">'
                             + '<b><span style="font-size:12.0pt;font-family:Calibri (Body);">' + count + '</span></b></p></td>';*/
                        tableContentRowsMisc += ' <tr>';
                        tableContentRowsMisc += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="border:2px solid black;width:25px;height:15px;">' + completed + '</span> </b></p></td>';

                        // Action Items from selected meeting
                        if (actionItem.meeting.id === meetingId) {

                            tableContentRowsMisc += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who.replace(/style="[^"]*"/, "")
                                + '</div></td>';

                            tableContentRowsMisc += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';


                            /*  tableContentRowsMisc  += '<td style="width:10%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                  + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                  + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                  + actionItem.department.name
                                  + '</span></p></td>';*/

                            tableContentRowsMisc += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsMisc  += '<td style="  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                        }
                        // Unfinished Action Items from previous meetings (Display in red)
                        else {
                            tableContentRowsMisc += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who
                                + '</div></td>';

                            tableContentRowsMisc += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';
                            /* tableContentRowsMisc  += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.department.name
                                 + '</span></p></td>';*/

                            tableContentRowsMisc += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsMisc  += '<td style=" padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after  //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                           
                        }
                        tableContentRowsMisc += '</tr>';
                        break;
                    case "New Vehicles":
                        var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';

                        tableContentRowsNew += ' <tr>';
                        tableContentRowsNew += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="border:2px solid black;width:25px;height:15px;">' + completed + '</span> </b></p></td>';

                        // Action Items from selected meeting
                        if (actionItem.meeting.id === meetingId) {

                            tableContentRowsNew += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who//.replace(/style="[^"]*"/, "")
                                + '</div></td>';

                            tableContentRowsNew += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';




                            tableContentRowsNew += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';


                        }
                        // Unfinished Action Items from previous meetings (Display in red)
                        else {
                            tableContentRowsNew += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.who
                                + '</div></td>';

                            tableContentRowsNew += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';



                            tableContentRowsNew += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';


                        }
                        tableContentRowsNew += '</tr>';
                        break;

                    case "Parts":
                       /*  var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';
                        tableContentRows += '<td style="width:1%; padding: 0in 1.4pt;border:solid 1px;">'
                             + '<p align="left" style="text-align:left;">'
                             + '<b><span style="font-size:12.0pt;font-family:Calibri (Body);">' + count + '</span></b></p></td>';
                        tableContentRowsParts += ' <tr>';
                        tableContentRowsParts += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="font-size:12.0pt;font-family:Wingdings;color:#00B050">' + completed + '</span></b></p></td>';
                            */
                        var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';

                        tableContentRowsParts += ' <tr>';
                        tableContentRowsParts += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="border:2px solid black;width:25px;height:15px;">' + completed + '</span> </b></p></td>';


                        // Action Items from selected meeting
                        if (actionItem.meeting.id === meetingId) {

                            tableContentRowsParts += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who//.replace(/style="[^"]*"/, "")
                                + '</div></td>';

                            tableContentRowsParts += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';


                            /* tableContentRowsParts  += '<td style="width:10%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                 + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.department.name
                                 + '</span></p></td>';*/

                            tableContentRowsParts += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsParts  += '<td style="  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                        }
                        // Unfinished Action Items from previous meetings (Display in red)
                        else {
                            tableContentRowsParts += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who
                                + '</div></td>';

                            tableContentRowsParts += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';
                            /* tableContentRowsParts  += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.department.name
                                 + '</span></p></td>';*/

                            tableContentRowsParts += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsParts  += '<td style=" padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after  //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                       
                        }
                        tableContentRowsParts += '</tr>';
                        break;
                    case "Service":
                        var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';
                        /* tableContentRows += '<td style="width:1%; padding: 0in 1.4pt;border:solid 1px;">'
                             + '<p align="left" style="text-align:left;">'
                             + '<b><span style="font-size:12.0pt;font-family:Calibri (Body);">' + count + '</span></b></p></td>';*/
                        tableContentService += ' <tr>';
                        tableContentService += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="border:2px solid black;width:25px;height:15px;">' + completed + '</span> </b></p></td>';

                        // Action Items from selected meeting
                        if (actionItem.meeting.id === meetingId) {

                            tableContentService += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who.replace(/style="[^"]*"/, "")
                                + '</div></td>';

                            tableContentService += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';


                            /*tableContentService  += '<td style="width:10%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                 + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.department.name
                                 + '</span></p></td>';*/

                            tableContentService += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentService  += '<td style="  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                        }
                        // Unfinished Action Items from previous meetings (Display in red)
                        else {
                            tableContentService += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who
                                + '</div></td>';

                            tableContentService += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';
                            /* tableContentService  += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.department.name
                                 + '</span></p></td>';*/

                            tableContentService += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentService  += '<td style=" padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after  //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                            
                        }
                        tableContentService += '</tr>';
                        break;
                    case "Used Vehicle":
                        var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';
                        /* tableContentRows += '<td style="width:1%; padding: 0in 1.4pt;border:solid 1px;">'
                             + '<p align="left" style="text-align:left;">'
                             + '<b><span style="font-size:12.0pt;font-family:Calibri (Body);">' + count + '</span></b></p></td>';*/
                        tableContentRowsUsed += ' <tr>';
                        tableContentRowsUsed += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="border:2px solid black;width:25px;height:15px;">' + completed + '</span> </b></p></td>';

                        // Action Items from selected meeting
                        if (actionItem.meeting.id === meetingId) {

                            tableContentRowsUsed += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who.replace(/style="[^"]*"/, "")
                                + '</div></td>';

                            tableContentRowsUsed += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';


                            /*tableContentRowsUsed  += '<td style="width:10%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + actionItem.department.name
                                + '</span></p></td>';*/

                            tableContentRowsUsed += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsUsed  += '<td style="  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                        }
                        // Unfinished Action Items from previous meetings (Display in red)
                        else {
                            tableContentRowsUsed += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who
                                + '</div></td>';

                            tableContentRowsUsed += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';
                            /* tableContentRowsUsed  += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                  + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                  + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                  + actionItem.department.name
                                  + '</span></p></td>';*/

                            tableContentRowsUsed += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsUsed  += '<td style=" padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after  //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                           
                        }
                        tableContentRowsUsed += '</tr>';
                        break;
                    case "Variable Ops":
                        var completed = '&nbsp;&nbsp;&nbsp;';
                        if (actionItem.completed === true)
                            completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';
                        /* tableContentRows += '<td style="width:1%; padding: 0in 1.4pt;border:solid 1px;">'
                             + '<p align="left" style="text-align:left;">'
                             + '<b><span style="font-size:12.0pt;font-family:Calibri (Body);">' + count + '</span></b></p></td>';*/
                        tableContentRowsVariable += ' <tr>';
                        tableContentRowsVariable += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                            + '<p align="center" style="text-align:center;">'
                            + '<b><span style="border:2px solid black;width:25px;height:15px;">' + completed + '</span> </b></p></td>';

                        // Action Items from selected meeting
                        if (actionItem.meeting.id === meetingId) {

                            tableContentRowsVariable += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who.replace(/style="[^"]*"/, "")
                                + '</div></td>';

                            tableContentRowsVariable += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';


                            /*  tableContentRowsVariable   += '<td style="width:10%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                                  + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                                  + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                  + actionItem.department.name
                                  + '</span></p></td>';*/

                            tableContentRowsVariable += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsVariable   += '<td style="  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                        }
                        // Unfinished Action Items from previous meetings (Display in red)
                        else {
                            tableContentRowsVariable += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                                + actionItem.who
                                + '</div></td>';

                            tableContentRowsVariable += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                + formatDate(actionItem.when)
                                + '</span></p></td>';
                            /* tableContentRowsVariable   += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.department.name
                                 + '</span></p></td>';*/

                            tableContentRowsVariable += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                                + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                                + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                                + '</div></td>';
                            /* tableContentRowsVariable   += '<td style=" padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                                 + '<p>'
                                 + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                                 + actionItem.after  //.replace(/(<([^>]+)>)/gi, "")
                                 + '</span></p></td>';*/

                           
                        }
                        tableContentRowsVariable += '</tr>';
                        break;
                   
          /* case "New Vehicles":
               var completed = '&nbsp;&nbsp;&nbsp;';
               if (actionItem.completed === true)
                   completed = '<span style = "font-size:12.0pt;font-family:Wingdings;color:#00B050"> ' + 'ü' + '</span>';
            
               tableContentRowsNew += ' <tr>';
               tableContentRowsNew += ' <td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;font-size:12.0pt;font-family:Calibri (Body);">'
                   + '<p align="center" style="text-align:center;">'
                   + '<b><span style="border:2px solid black;width:25px;height:15px;">&nbsp;&nbsp;&nbsp;</span><span style="font-size:12.0pt;font-family:Wingdings;color:#00B050">' + completed + '</span></b></p></td>';


               // Action Items from selected meeting
               if (actionItem.meeting.id === meetingId) {

                   tableContentRowsNew += '<td style=" width:1%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                       + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                       + actionItem.who//.replace(/style="[^"]*"/, "")
                       + '</div></td>';

                   tableContentRowsNew += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060;font-size:12.0pt;font-family:Calibri (Body);">'
                       + '<p align="left" style="text-align:left;font-family:Calibri (Body);">'
                       + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                       + formatDate(actionItem.when)
                       + '</span></p></td>';


                 

                   tableContentRowsNew += '<td style=" width:88%; padding: 0in 1.4pt;border:solid 1px;border-color:black;color:#002060; font-size:12.0pt;font-family:Calibri (Body);"> '
                       + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                       + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                       + '</div></td>';
                 

               }
               // Unfinished Action Items from previous meetings (Display in red)
               else {
                   tableContentRowsNew += '<td style="width:1%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                       + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'

                       + actionItem.who
                       + '</div></td>';

                   tableContentRowsNew += '<td style="width:10%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;">'
                       + '<p align="left" style="text-align:left;font-size:12.0pt;font-family:Calibri (Body);">'
                       + '<span style="font-size:12.0pt;font-family:Calibri (Body);">'
                       + formatDate(actionItem.when)
                       + '</span></p></td>';
                  

                   tableContentRowsNew += '<td style=" width:88%;  padding: 0in 1.4pt;border:solid 1px;border-color:black;color:red;font-size:12.0pt;font-family:Calibri (Body);"> '
                       + '<div align="left" style="text-align:left;font-family:Calibri (Body);font-size:12.0pt;">'
                       + actionItem.what//.replace(/(<([^>]+)>)/gi, "")
                       + '</div></td>';
                 

                
               }
                  tableContentRowsNew += ' </tr>';
               break;*/
                }

                count = count + 1;
            });

            /* bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                 + tableHeaderRow
                 + tableContentRows
                 + '</tbody></table> ';*/
           if (tableContentRowsAccounting !== '') {
                bodycontent += '<p></p>'
                bodycontent += "<p style='font - family: Calibri(Body); color: #002060; font-size: 14.0pt;'>Accounting</p>";

                bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                    + tableHeaderRow
                    + tableContentRowsAccounting
                    + '</tbody></table> ';

                bodycontent += '<p></p>'
            }
            if (tableContentRowsBody !== '') {
                bodycontent += '<p></p>'
                bodycontent += "<p style='font - family: Calibri(Body); color: #002060; font-size: 14.0pt;'>Body Shop</p>";
                bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                    + tableHeaderRow
                    + tableContentRowsBody
                    + '</tbody></table> ';
                bodycontent += '<p></p>'
            }
            if (tableContentRowsFixed !== '') {
                bodycontent += '<p></p>'
                bodycontent += "<p style='font - family: Calibri(Body); color: #002060; font-size: 14.0pt;'>Fixed Ops</p>";
                bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                    + tableHeaderRow
                    + tableContentRowsFixed
                    + '</tbody></table> ';
                bodycontent += '<p></p>'
            }

            if (tableContentRowsFI !== '') {
                bodycontent += '<p></p>'
                bodycontent += "<p style='font - family: Calibri(Body); color: #002060; font-size: 14.0pt;'>F&I</p>";
                bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                    + tableHeaderRow
                    + tableContentRowsFI
                    + '</tbody></table> ';
            }

          
            if (tableContentRowsMisc !== '') {
                bodycontent += '<p></p>'
                bodycontent += "<p style='font - family: Calibri(Body); color: #002060; font-size: 14.0pt;'>Misc</p>";
                bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                    + tableHeaderRow
                    + tableContentRowsMisc
                    + '</tbody></table> ';
                bodycontent += '<p></p>'
            }
             
            if (tableContentService !== '') {
                bodycontent += '<p></p>'
                bodycontent += "<p style='font - family: Calibri(Body); color: #002060; font-size: 14.0pt;'>Service</p>";
                bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                    + tableHeaderRow
                    + tableContentService
                    + '</tbody></table> ';
                bodycontent += '<p></p>'
            }

            if (tableContentRowsUsed !== '') {
                bodycontent += '<p></p>'
                bodycontent += "<p style='font - family: Calibri(Body); color: #002060; font-size: 14.0pt;'>Used Vehicles</p>";
                bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                    + tableHeaderRow
                    + tableContentRowsUsed
                    + '</tbody></table> ';
                bodycontent += '<p></p>'
            }
            if (tableContentRowsVariable !== '') {
                bodycontent += '<p></p>'
                bodycontent += "<p style='font - family: Calibri(Body); color: #002060; font-size: 14.0pt;'>Variable Ops</p>";
                bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                    + tableHeaderRow
                    + tableContentRowsVariable
                    + '</tbody></table> ';
                bodycontent += '<p></p>'
            }

            if (tableContentRowsParts !== '') {
                bodycontent += '<p></p>'
                bodycontent += "<p style='font - family: Calibri(Body); color: #002060; font-size: 14.0pt;'>Parts</p>";
                bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                    + tableHeaderRow
                    + tableContentRowsParts
                    + '</tbody></table> ';
                bodycontent += '<p></p>'
            } 
            if (tableContentRowsNew !== '') {
                bodycontent += '<p></p>'
                bodycontent += "<p style='font - family: Calibri(Body); color: #002060; font-size: 14.0pt;'>New Vehicles</p>";
                bodycontent += '<table style="border:solid 1px;border-color:black; display:table; float:left;page-break-before: always;" cellspacing="0" cellpadding="0" class="fixed printTable table"><tbody>'
                    + tableHeaderRow
                    + tableContentRowsNew
                    + '</tbody></table> ';
                bodycontent += '<p></p>'
            }
           

            
            bodycontent += '<p style="font-size:10pt;"><font style="color:red;">* </font>Unfinished action-items from previous meetings are displayed in red.</p>';
        }

        if (result.files !== undefined && result.files.length > 0) {

            bodycontent += '<p></p>'
                + '<p align="left" style="text-align:center">'
                + '<span style="font-family:Calibri (Body);color: #002060;font-weight: bold; font-size: 14.0pt;" class="break">Uploaded Files</span></p>';

            var tableHeaderCols = '<td valign="top" style="width:10%; padding: 0in 1.4pt;border:solid 1px;">'
                + '<p align="left" style="text-align:left;"><b><span style="font-size:12.0pt;font-family:Calibri (Body);color:#002060;">File</span></b></p></td>';


            var tableHeaderRow = '<tr>' + tableHeaderCols + '</tr>';

            var tableContentRows = '';
            $.each(result.files, function (index, file) {



                tableContentRows += '<tr><td style="width:10%; padding: 0in 1.4pt;border:solid 1px;">'
                    + '<p align="left" style="text-align:left;">'
                    + '<b><span style="font-size:12.0pt;font-family:Calibri (Body);color:#00B050"><a href="' + file.path + '">' + file.fileName + '</a></span></b></p></td></tr>';


            });

            bodycontent += '<table style="border:solid 1px;border-color:black;" cellspacing="0" cellpadding="0"  class="printTable"><tbody>'
                + tableHeaderRow
                + tableContentRows
                + '</tbody></table>';


        }
        $('#rptContetnArea').append(bodycontent);

        var body = $(bodydiv).html(bodycontent);
        $('#content').append(body);

        $('.removeStyle').removeAttr('style');
        $("#removeStyle").removeAttr("style");
        console.log("style remove");

    }).done(function (result) {
        if (result.success == false) {
            swal("Error!", result.value, "error");
            return;
        }
        setTimeout(function () {

            var html, link, blob, url, css;
            var headerdiv = ' <div style="mso-element: header" id=h1>     <p class=MsoHeader><span style="mso-tab - count: 1"></span>  <span style="mso-field - code:PAGE"></span><div class="row" id="rptHeader"></div>';

            var head = '<p>&nbsp;</p><p style="font-family:Calibri (Body);color:#ff0000;font-size:24.0pt;"><img style="margin-top:40px; display: inline-block;float: left;" src="https://ShowIssueTracker.azurewebsites.net/NADA.png" title="logo"/> &nbsp; &nbsp;'
                + $.trim(result.meeting.dealership) + '</p><p>&nbsp;</p>' +
                ' </p>   </div>';


            var footer = "</body></html>";
            css = (
                '<style>' +
                'h4 {font-family: "Calibri (Body) (Body)"; font-size: 14pt;}    table{ border - collapse: collapse; width:100%;} td{ border: 1px gray solid; width: 5em; padding: 2px; }' +
                + ' @page{size: 11.0in 8.5in;mso-page-orientation: landscape;}  .break {page-break-after: always !important;}' +
                + ' .layer{size: 11.0in 8.5in;mso-page-orientation: landscape; border:1px; solid red;}   ' +
                + 'table.fixed { table - layout: fixed; width: 100%;  } ' +
                'table{border-collapse:collapse;}td{border:1px red solid;width:5em;padding:2px;}' +
                + 'table.fixed td { overflow: hidden; } ' +
                + 'table.fixed td: nth - of - type(1) { width: 10%; } ' +
                + 'table.fixed td: nth - of - type(2) { width: 10%; } ' +
                + 'table.fixed td: nth - of - type(3) { width: 10%; } ' +
                + 'table.fixed td: nth - of - type(4) { width: 10%; } ' +
                + 'table.fixed td: nth - of - type(5) { width: 50%; } ' +
                + 'table.fixed td: nth - of - type(6) { width: 10%; } ' +
                +'.table {  page -break-after: always;   }' +
                + '  p.MsoHeader, li.MsoHeader, div.MsoHeader' +
                + '    {' +
                + '  margin: 0in;' +
                + ' margin - bottom: .0001pt;' +
                + ' mso - pagination: widow - orphan;' +
                + 'tab - stops: center 3.0in right 6.0in;' +
                + 'font - size: 12.0pt;' +
                + ' } p { color: #002060; }' +
                + '  .calibre3 {  display: table - row;  vertical - align: inherit ;background-color: #EDEDED;  }' +

                + ' div.header {  ' +
                + '        display: block; text-align: center; ' +
                + '       position: running(header); ' +
                + '   } ' +
                + '   div.footer { ' +
                + '       display: block; text-align: center; ' +
                + '     position: running(footer); ' +
                + '  }' +
                + '  @page {' +
                + '       @top-center { content: element(header) }' +
                + '  }' +
                + '   @page {' +
                + '      @bottom-center { content: element(footer) }' +
                + '  } .custom-footer-page-number:after { content: counter(page); }              #header { position: fixed; width: 100 %; top: 0; left: 0; right: 0; } #footer { position: fixed; width: 100 %; bottom: 0; left: 0; right: 0; }' +
                '</style>'


            );
            
            html = testHTML.replace('####HEADER####', head ).replace('####CONTENT####', document.getElementById('content').innerHTML);//.replace('####CSS####',css);// + footer; 
             blob = new Blob(['\ufeff', css + html], {
                type: 'application/msword'
            });

            url = URL.createObjectURL(blob);
            link = document.createElement('A');
            link.href = url;
            // Set default file name.
            // Word will append file extension - do not add an extension here.
            link.download = result.meeting.dealership + ' Follow Up & Action Plan IDC_' + result.meeting.consultant + '_' + formatDate2(result.meeting.meetingDate);
            document.body.appendChild(link);
            if (navigator.msSaveOrOpenBlob) navigator.msSaveOrOpenBlob(blob, link.download); // IE10-11
            else link.click();  // other browsers
            document.body.removeChild(link); 

            console.log(html);


        }, 5000);


    }).fail(function (data) {
        swal("Error!", data.value, "error");
    });
}
function exportHTML(header, footer) {

    var sourceHTML = header + document.getElementById("content").innerHTML + footer;

    var source = 'data:application/vnd.ms-word;charset=utf-8,' + encodeURIComponent(sourceHTML);
    //var fileDownload = document.createElement("a");
    //document.body.appendChild(fileDownload);
    //fileDownload.href = source;
    //fileDownload.download = 'document.doc';
    //fileDownload.click();
    //document.body.removeChild(fileDownload);
    var my_file = new Blob([source]);
    getBase64(my_file);
}

function getBase64(file) {
    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
        console.log(reader.result);
    };
    reader.onerror = function (error) {
        console.log('Error: ', error);
    };
}

// Function - formatDate
function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [month, day, year].join('/');
}

// Function - formatDate2
function formatDate2(date) {
    const monthNames = ["January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    ];

    var d = new Date(date),
        month = monthNames[d.getMonth()],
        day = '' + d.getDate(),
        year = d.getFullYear();

    var result = month + ' ' + day + ', ' + year;
    return result;
}

function printpdf(elementId) {
    var getMyFrame = document.getElementById(elementId);
    getMyFrame.focus();
    getMyFrame.contentWindow.print();
}
function saveDoc() {

    if (!window.Blob) {
        alert('Your legacy browser does not support this action.');
        return;
    }

    var html, link, blob, url, css;

    // EU A4 use: size: 841.95pt 595.35pt;
    // US Letter use: size:11.0in 8.5in;

    css = ('\
   <style>\
   @page WordSection1{size: 841.95pt 595.35pt;mso-page-orientation: portrait;}\
   div.WordSection1 {page: WordSection1;}\
   h1 {font-family: "Times New Roman", Georgia, Serif; font-size: 16pt;}\
   p {font-family: "Times New Roman", Georgia, Serif; font-size: 14pt;}\
   </style>\
  ');

    var rightAligned = document.getElementsByClassName("sm-align-right");
    for (var i = 0, max = rightAligned.length; i < max; i++) {
        rightAligned[i].style = "text-align: right;"
    }

    var centerAligned = document.getElementsByClassName("sm-align-center");
    for (var i = 0, max = centerAligned.length; i < max; i++) {
        centerAligned[i].style = "text-align: center;"
    }

    html = document.getElementById('content').innerHTML;
    html = '\
  <html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:w="urn:schemas-microsoft-com:office:word" xmlns="http://www.w3.org/TR/REC-html40">\
  <head>\
    <title>Document Titleasdsadasd</title>\
    <xml>\
      <w:worddocument xmlns:w="#unknown">\
        <w:view>Print</w:view>\
        <w:zoom>90</w:zoom>\
        <w:donotoptimizeforbrowser />\
      </w:worddocument>\
    </xml>\
  </head>\
  <body lang=RU-ru style="tab-interval:.5in">\
    <div class="Section1">' + html + '</div>\
  </body>\
  </html>'

    blob = new Blob(['\ufeff', css + html], {
        type: 'application/msword'
    });

    url = URL.createObjectURL(blob);
    link = document.createElement('A');
    link.href = url;

    filename = 'filename';

    // Set default file name.
    // Word will append file extension - do not add an extension here.
    link.download = filename;

    document.body.appendChild(link);

    if (navigator.msSaveOrOpenBlob) {
        navigator.msSaveOrOpenBlob(blob, filename + '.doc'); // IE10-11
    } else {
        link.click(); // other browsers
    }

    document.body.removeChild(link);
};