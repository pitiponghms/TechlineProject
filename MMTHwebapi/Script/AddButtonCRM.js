﻿function InitFunction() {
	// update record in CRM (RO Case)
	Xrm.Page.getAttribute("hms_newsystemstatus").addOnChange(updateShippingMethod);

	// new page button
	//chatmessage_onload();
}

function updateShippingMethod() {
	var api = "https://10.146.138.166:44300/api/rocase/update";

	var formdata = new FormData();
	var text = Xrm.Page.getAttribute("hms_newsystemstatus").getText();
	var value = Xrm.Page.getAttribute("hms_newsystemstatus").getValue();
	var stvalue=1;
	if(value=='177980001'){
		stvalue=2;
	}if(value=='177980002'){
		stvalue=3;
	}if(value=='177980003'){
		stvalue=4;
	}if(value=='177980004'){
		stvalue=5;
	}if(value=='177980005'){
		stvalue=6;
	}if(value=='177980006'){
		stvalue=7;
	}

	var data = {
		'Id': Xrm.Page.getAttribute("hms_rocaseid").getValue(),
		//'CaseId': Xrm.Page.getAttribute("hms_rocaseid").getValue(),
		//'A_code' : null,
		//'B_code' : "",
		//'C_code' : 'C_code3',
		'StatusCode':stvalue,
		'ModifiedBy': '999',
		'MicrosoftTeamLink': Xrm.Page.getAttribute("hms_microsoftteamlink").getValue(),
		'SolutionForDealer': Xrm.Page.getAttribute("hms_solutionfordealer").getValue(),
		//'StatusCode' : '2',

		//'LevelofProblem' : 'Level2',
		//'CaseTitle' : 'CaseTitle2',
		//'CaseType' : 'Type2',
		//'CaseTitle': text,
		//'CaseType': value,
	};
	/*
	var data = {
		'Id':34,
		'StatusCode':'4',
		'ModifiedBy': '999',
	};
	*/
	//prompt('',JSON.stringify(data));

	formdata.append('Model', JSON.stringify(data));

	$.ajax({
		type: 'POST',
		url: api,
		data: formdata,
		contentType: false,
		processData: false,
		success: function (result) {
			if (result != null) {
				alert(result.Message);
			}
		},
		error: function (xhr, status, error) {
			alert(xhr.responseText);
			alert(status);
			alert(error);
		}
	});
}
function chatmessage_onload() {
	// Web resource: 'WebResource_btn_chatmessage'
	ConvertToButton('WebResource_chat', chatmessage_onclick);
}

function ConvertToButton(fieldname, clickevent) {
	//check if object exists; else return
	var control = Xrm.Page.getControl(fieldname);

	if (control != null) {
		control.getObject().onclick = clickevent;
	}
}

function chatmessage_onclick() {
	var viewportwidth = screen.availWidth;
	var viewportheight = screen.availHeight;
	//window.resizeBy(-300,0);
	window.moveTo(0, 0);
	
	window.open('https://thcrmweb01.ad-mmth.th.mitsubishi-motors.com/MMTHQAS/WebResources/hms_roChatPage.html',
		"mywindow",
		"width=750, height=430, left=" + (viewportwidth - 750) + ",top=" + (viewportheight - 430)
	);
}