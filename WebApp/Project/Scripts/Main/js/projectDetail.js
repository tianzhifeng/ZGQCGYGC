$(document).ready(function(){
			$(".section .title ul li").on("click",function(){
				$(this).addClass("active").siblings().removeClass("active");

				var index=$(this).attr("for");
				$(".table-ing,.table-end,.table-design,.table-plan").hide();
				$("."+index).show();
			})
		})