/// <reference path="jquery-1.6.4-vsdoc.js" />
/// <reference path="jQuery.tmpl.js" />
/// <reference path="json2.js" />
/// <reference path="jquery.signalR.js" />

/* Author:
		Korroz
*/

window.chatApplication = function () {
	var app = this;
	var originalTitle = document.title;

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
		unreadCount: ko.observable(0),
		unread: null, // dependentObservable
		title: null, // dependentObservable
		windowActive: ko.observable(true),
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

			if (!this.windowActive()) {
				var count = this.unread();
				this.unread(++count);
			}
		}
	};
	this.viewModel.unread = ko.dependentObservable({
		read: function () {
			if (this.windowActive())
				this.unreadCount(0);
			return this.unreadCount();
		},
		write: function (value) {
			this.unreadCount(value);
		},
		owner: this.viewModel
	});
	this.viewModel.title = ko.dependentObservable(function () {
		var count = this.unread();
		return (count) ? "(".concat(count, ")", originalTitle) : originalTitle;
	}, this.viewModel);

	this.start = function () {
		$(document).ready(function () {
			// Knockout
			ko.applyBindings(app.viewModel);
			app.viewModel.title.subscribe(function (newTitle) {
				document.title = newTitle;
			});

			// Setup events
			$("#msg").keyup(function (event) {
				if (event.keyCode == 13)
					$("#send").click();
			});

			$("#msg").focus();

			$(window).blur(function () {
				app.viewModel.windowActive(false);
			});
			$(window).focus(function () {
				app.viewModel.windowActive(true);
			});

			// SignalR
			$.connection.hub.start();
		});
	};
};