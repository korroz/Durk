/// <reference path="jquery-1.6.4-vsdoc.js" />
/// <reference path="jQuery.tmpl.js" />
/// <reference path="json2.js" />
/// <reference path="jquery.signalR.js" />

/* Author:
		Korroz
*/

window.chatApplication = function () {
	var app = this;

	var user = function (name) {
		this.name = name;
	};

	this.hub = $.connection.chat;
	this.hub.addMessage = function (jsonChatMessage) {
		app.viewModel.addMessage(JSON.parse(jsonChatMessage));
	};
	this.hub.userEntered = function (jsonUser) {
	};

	this.viewModel = {
		users: ko.observableArray([new user("Doom"), new user("Lolzorz")]),
		sendMessage: function () {
			var msg = $("#msg");
			app.hub.send(msg.val()).fail(function (e) { alert(e); });
			msg.val("");
		},
		addMessage: function (chatMessage) {
			var time = new Date();
			chatMessage.Time = time.format("HH:MM");
			var messages = $("#messages");
			$("#msgTmpl").tmpl(chatMessage).appendTo(messages);
			messages.scrollTop(messages[0].scrollHeight);
		}
	};

	this.start = function () {
		$(document).ready(function () {
			// Knockout
			ko.applyBindings(app.viewModel);

			$("#msg").keyup(function (event) {
				if (event.keyCode == 13)
					$("#send").click();
			});

			$("#msg").focus();

			// SignalR
			$.connection.hub.start();
		});
	};
};