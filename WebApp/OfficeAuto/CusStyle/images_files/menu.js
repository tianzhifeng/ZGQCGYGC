function  currentMenu(i){
		$("#menu ul li a").removeClass("currentNav");
	//curMenu.find("a").addClass("currentAdmin");
	$("#qtsurvey"+i).addClass("currentNav");
}
function  currentMenuIntr(i){
	$("#introMenu ul li a").removeClass("currentNav");
	$("#introMenu ul li:eq("+i+")").find("a").addClass("currentNav");
}