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
		unread: ko.observable(0),
		title: null, // dependentObservable
		windowActive: ko.observable(true),
		users: ko.observableArray([new user("Doom"), new user("Lolzorz")]),
		sendMessage: function () {
			var msg = $("#msg");
			if(msg.val().trim()) {
				app.hub.send(msg.val()).fail(function (e) { alert(e); });
			}
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
		},
		focusInput: function () {
			$("#msg").focus();
		},
		addBookmarkLine: function () {
			this.removeBookmarkLine();
			$("<hr id='bookmarkLine'/>").appendTo($("#messages"));
		},
		removeBookmarkLine: function () {
			$("#bookmarkLine").remove();
		}
	};
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
			app.viewModel.windowActive.subscribe(function (itIs) {
				if (itIs) {
					app.viewModel.unread(0);
				} else {
					app.viewModel.addBookmarkLine();
				}
			});

			// Setup events
			$("#msg").keyup(function (event) {
				if (event.keyCode == 13) {
					app.viewModel.sendMessage();
					app.viewModel.focusInput();
					event.preventDefault();
				}
			}).keydown(function (event) {
				if (event.keyCode == 13)
					event.preventDefault();
			});

			$(window).blur(function () {
				app.viewModel.windowActive(false);
			});
			$(window).focus(function () {
				app.viewModel.windowActive(true);
			});

			// Start state
			app.viewModel.focusInput();

			// SignalR
			$.connection.hub.start();
		});
	};
};