<div id="{|ID|}" class="box box-primary" style="padding-bottom:{|padding-bottom|}px">
    <div class="box-header">
        <h3 class="box-title">
            {|Title|}
        </h3>
        <div class="box-tools pull-right">
            <button type="button" class="btn btn-box-tool" data-widget="collapse">
                <i class="fa fa-minus"></i>
            </button>
            <button type="button" class="btn btn-box-tool" data-widget="remove" onclick="$('#{|ID|}').data('hidden', 'true');dashboardChanged('{|ID|}');">
                <i class="fa fa-times"></i>
            </button>
        </div>
    </div>
    <div class="box-body" style="height:{|Height|}">
        <iframe src="{|LinkUrl|}" width="100%" height="100%" frameborder="0">
        </iframe>     
    </div>
</div>
<script>
</script>
