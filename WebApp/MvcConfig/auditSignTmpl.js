//旧会签模板(无标题)
var auditSignTmpl_AuditSignTemplete = " <div  style='border:1px solid #828282'>"
                        + "<table cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;" +
                           "' width='100%'>$AuditItem</Table></div>";
var auditSignTmpl_AuditSignTempleteItem = "<tr><td style='width:21%'><td style='width:3%'></td><td style='width:21%'></td>"
    + "<td style='width:21%'></td><td style='width:3%'></td><td style='width:21%'></td></tr>"
    + "<tr><td align='left' colspan='3' >" + (CurrentLGID == "EN" ? "Opinion" : "意见") + "：</td></tr>"
    + "<tr><td align='left' colspan='6'>$SignComment</td></tr>"
    + "<tr>"
    + "<td align='right'>" + (CurrentLGID == "EN" ? "Sign" : "负责人签字") + ":</td><td align='center'>&nbsp;</td><td id='$ExecUserID' align='center'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\" width=\"80px\" height=\"30px\"></td>"
    + "<td align='right'>" + (CurrentLGID == "EN" ? "Date" : "日期") + ":</td><td align='center'>&nbsp;</td><td align='center'>$SignTime</td></tr>";
+"<tr><td colspan='6'><hr></td></tr>"; //尾行必须要

//新会签模板（带标题）
var auditMultiSignTmpl_AuditSignTemplete = " <div  style='border:0px solid #828282'>"
                        + "<table  cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;table-layout:fixed;" +
                           "' width='100%'> <tr  style='display:none'><td width='60%'></td><td width='15%'></td><td width='10%'></td><td width='15%'></td></tr> $AuditItem</Table></div>";
var auditMultiSignTmpl_AuditSignTempleteItem = "<tr><td align='left' colspan='4' >$StepName意见：</td></tr>"
    + "<tr><td align='left' colspan='4'>$SignComment</td></tr>"
    + "<tr>"
    + "<td align='right'>签字:&nbsp;&nbsp;&nbsp;</td><td  id='$ExecUserID' align='left'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\"  height=\"30px\"></td>"
    + "<td align='right'>日期:&nbsp;&nbsp;&nbsp;</td><td align='left'>$SignTime</td></tr>"
    + "<tr><td colspan='4'><hr></td></tr>"; //尾行必须要

//单签模板（竖）
var auditSignSingleTmpl_AuditSignTemplete = " <div  style='border:0px solid #828282'>"
                        + "<table cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;" +
                           "' width='100%'>$AuditItem</Table></div>";
var auditSignSingleTmpl_AuditSignTempleteItem = "<tr><td align='right'>$SignTitle：</td><td align='left'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\" width=\"80px\" height=\"30px\"></td></tr>"
    + "<tr><td align='right'>日期：</td><td align='left'>$SignTime</td></tr>"
    + "<tr><td colspan='6'></td></tr>"; //尾行必须要

//单签模板（横）
var auditSignSingleHorizontalTmpl_AuditSignTemplete = " <div  style='border:0px solid #828282'>"
                        + "<table cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;" +
                           "' width='100%'>$AuditItem</Table></div>";
var auditSignSingleHorizontalTmpl_AuditSignTempleteItem = "<tr><td align='right' style='width:20%'> 签字：</td><td align='left'  style='width:30%'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\" width=\"80px\" height=\"30px\"></td>"
    + "<td align='right' style='width:20%'>日期：</td><td align='left' style='width:30%'>$SignTime</td></tr>"
+ "<tr><td colspan='4'></td></tr>"; //尾行必须要

//仅图片
var auditSignOnlyUserTmpl_AuditSignTemplete = " <div  style='border:0px solid #828282'>"
                        + "<table cellSpacing='0' cellPadding='0' border='0' style='border-collapse: collapse;" +
                           "' width='100%'>$AuditItem</Table></div>";
var auditSignOnlyUserTmpl_AuditSignTempleteItem = "<tr><td align='right'>$SignTitle</td><td align='left'><img src=\"/MvcConfig/Image/GetSignPic?UserId=$ExecUserID\" width=\"80px\" height=\"30px\"></td></tr>"
+ "<tr><td colspan='6'></td></tr>"; //尾行必须要


