<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PivotDetail.aspx.cs" Inherits="DevPivotGrid.PivotDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>详细</title><link href='/CommonWebResource/Theme/Default/MiniUI/miniui.css' rel='stylesheet' type='text/css' />
    <link href='/CommonWebResource/Theme/Default/MiniUI/icons.css' rel='stylesheet' type='text/css' />
    <link href='/CommonWebResource/Theme/Default/MiniUI/miniextend.css' rel='stylesheet' type='text/css' />
    <script src="/CommonWebResource/CoreLib/Basic/jQuery/jquery-1.6.2.min.js" type="text/javascript"></script>
    <script src="/CommonWebResource/CoreLib/MiniUI/miniui.js"></script>
</head>
<body>
    <div class="mini-fit">
        <div id="dataGrid" class="mini-datagrid" style="width: 100%; height: 100%;" multiselect="true" pagesize="50" showcolumnsmenu="true">
            <div property="columns">
                <%=ColumnsString%>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        //获取地址栏参数,如果参数不存在则返回空字符串
        function getQueryString(key, url) {
            if (typeof (url) == "undefined")
                url = window.location.search;
            var re = new RegExp("[?&]" + key + "=([^\\&]*)", "i");
            var a = re.exec(url);
            if (a == null) return "";
            return a[1];
        }
    </script>
    <script type="text/javascript">
        mini.parse();
        var grid = mini.get("dataGrid");
        var url = "/DevPivotGrid/PivotGrid.aspx?method=getDetail&TmplCode=" + getQueryString("TmplCode") + "&colIndex=" + getQueryString('colIndex') + "&rowIndex=" + getQueryString('rowIndex');
        if (getQueryString("ID"))//自由透视表
            url = "/DevPivotGrid/PivotGrid.aspx?method=getDetail&ID=" + getQueryString("ID") + "&colIndex=" + getQueryString('colIndex') + "&rowIndex=" + getQueryString('rowIndex');
        grid.setUrl(url);
        grid.reload();
    </script>
</body>
</html>
