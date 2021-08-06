function chatmessage_onload() {
	// update record in CRM (RO Case)
	//Xrm.Page.getAttribute("hms_newsystemstatus").addOnChange(updateShippingMethod);
	
	// Web resource: 'WebResource_btn_chatmessage'
	//ConvertToButton('WebResource_img_chat', chatmessage_onclick);
	ConvertToButton('WebResource_chat', chatmessage_onclick);
	ConvertToButton('WebResource_rating', rating_onclick);
	
	disabledSystemStatus_onload();
}

function updateShippingMethod() {
	//var api = "https://localhost:44369/api/rocase/updatecrm";
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
		stvalue=5;
	}if(value=='177980004'){
		stvalue=4;
	}if(value=='177980005'){
		stvalue=6;
	}if(value=='177980006'){
		stvalue=7;
	}

		var data = {
		
                 'Id': Xrm.Page.getAttribute("hms_rocaseid").getValue(),
		'StatusCode':stvalue,
		'ModifiedBy': '999',
		'MicrosoftTeamLink': Xrm.Page.getAttribute("hms_microsoftteamlink").getValue(),
		'SolutionForDealer': Xrm.Page.getAttribute("hms_solutionfordealer").getValue(),
	};
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
		}
	});
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
	/*
	window.open('http://phoebe.hms-cloud.com:5555/MMTHQAS/WebResources/hms_html_rochat',
		"mywindow",
		"width=750, height=430, left=" + (viewportwidth - 750) + ",top=" + (viewportheight - 430)
	);
*/
	window.open('https://thcrmweb01.ad-mmth.th.mitsubishi-motors.com/MMTHQAS/WebResources/hms_roChatPage.html',
		"mywindow",
		"width=750, height=430, left=" + (viewportwidth - 750) + ",top=" + (viewportheight - 430)
	);

}

function rating_onclick() {
	var viewportwidth = screen.availWidth;
	var viewportheight = screen.availHeight;
	//window.resizeBy(-300,0);
	window.moveTo(0, 0);
	/*
	window.open('http://phoebe.hms-cloud.com:5555/MMTHQAS/WebResources/hms_html_rating',
		"mywindow",
		"width=750, height=430, left=" + (viewportwidth/5) + ",top=" + (viewportheight/5)
	);*/
	window.open('https://thcrmweb01.ad-mmth.th.mitsubishi-motors.com/MMTHQAS/WebResources/hms_html_rating',
		"mywindow",
		"width=750, height=430, left=" + (viewportwidth - 750) + ",top=" + (viewportheight - 430)
	);
}

function disabledSystemStatus_onload(){
	var fieldname = "hms_newsystemstatus";
	var status = Xrm.Page.getAttribute(fieldname).getValue();
	
	if(status == null){
		Xrm.Page.getAttribute(fieldname).setValue(177980000);	
	}
	else if(status == 177980000){
		Xrm.Page.getControl(fieldname).setDisabled(true);		
	}
	else if(status == 177980001){
		Xrm.Page.getControl(fieldname).removeOption(177980000);
		Xrm.Page.getControl(fieldname).removeOption(177980002);
		Xrm.Page.getControl(fieldname).removeOption(177980004);
		Xrm.Page.getControl(fieldname).removeOption(177980005);
		Xrm.Page.getControl(fieldname).removeOption(177980006);
	}
	else if(status == 177980002){
		Xrm.Page.getControl(fieldname).removeOption(177980000);
		Xrm.Page.getControl(fieldname).removeOption(177980001);
		Xrm.Page.getControl(fieldname).removeOption(177980003);
		Xrm.Page.getControl(fieldname).removeOption(177980004);
		Xrm.Page.getControl(fieldname).removeOption(177980006);
	}
	else if(status == 177980003){
		Xrm.Page.getControl(fieldname).setDisabled(true);	
	}
	else if(status == 177980004){
		Xrm.Page.getControl(fieldname).removeOption(177980000);
		Xrm.Page.getControl(fieldname).removeOption(177980001);
		Xrm.Page.getControl(fieldname).removeOption(177980002);
		Xrm.Page.getControl(fieldname).removeOption(177980003);
		Xrm.Page.getControl(fieldname).removeOption(177980005);
	}
	else if(status == 177980005){
		Xrm.Page.getControl(fieldname).setDisabled(true);	
	}
	else if(status == 177980006){
		Xrm.Page.getControl(fieldname).setDisabled(true);	
	}
}