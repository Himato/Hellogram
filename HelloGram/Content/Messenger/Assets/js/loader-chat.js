function register() {
    var messenger = $.connection.messengerHub;
    messenger.client.receive = function (username, ids) {
        receive(username, ids);
        markAsRead(username);
    };
    messenger.client.changeSeen = function (list) {
        changeSeen(list);
    };

    var login = $.connection.loginHub;
    login.client.changeState = function (username, online, lastLogin) {
        changeState(username, online, lastLogin);
    };

    $.connection.hub.start().done(function () {
        messenger.server.register();
        login.server.register();
    });

    $.connection.hub.reconnecting(function () {
        $(".js-lost-connection").removeClass("hide");
    });

    $.connection.hub.reconnected(function () {
        $(".js-lost-connection").addClass("hide");
    });

    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 5000); // Restart connection after 5 seconds.
    });

    markAsRead(getUsername());
    seenEffect();
}

function markAsRead(username) {
    if (username === getUsername()) {
        $.ajax({
                url: "/api/messenger/mark_read?username=" + getUsername(),
                type: "PUT",
                data: {},
                success: function () {
                    $("#tab-content-dialogs").find(".card").each(function () {
                        if ($(this).attr("data-username") === getUsername()) {
                            $(this).find(".badge").first().addClass("hide").find("span").first().text(0);
                        }
                    });

                    activeChats();
                }
            })
            .fail(function () {
                markAsRead(username);
            });
    }
}

function changeSeen(list) {
    for (var i = 0; i < list.length; i++) {
        var contents = list[i].split("#");
        var id = contents[0];
        var tooltip = contents[1];
        $(".message-content").each(function () {
            if ($(this).attr("data-message-id") === id) {
                $(this).attr("title", tooltip);
                $(this).attr("data-message-seen", "true");
            }
        });
    }

    seenEffect();
}

function seenEffect() {
    $(".message-footer").each(function() {
        $(this).addClass("hide");
    });

    $(".message-right").last()
        .find(".message-content[data-message-seen='true']").last()
        .siblings(".message-footer").first().removeClass("hide");
}

function receive(username, ids) {
    var opened = getUsername();

    if (opened === username) {
        getMessage(ids, false);
    } else {
        $.ajax({
            type: "GET",
            url: "/mvc/messenger?messageIds=" + ids,
            data: {},
            success: function(messages) {
                changeMember($($.parseHTML(messages)), false);
                play("notify");
            }
        });
    }
}

function receiveMessage(content) {
    var parent = $(".js-messages-container").first();
    var last = parent.find(".message").last();
    var body = last.find(".messages-body-container").first();

    if (content.hasClass("message-right")) {
        if (last.hasClass("message-right")) {
            body.append(content.find(".messages-body-container").first().html());
        } else {
            parent.append(content);
        }

        changeMember(content, true);
    } else {
        if (!last.hasClass("message-right")) {
            body.append(content.find(".messages-body-container").first().html());
        } else {
            parent.append(content);
        }

        changeMember(content, false);
    }
    document.querySelector(".end-of-chat").scrollIntoView();
}

function getMessage(ids, sent) {
    $.ajax({
        type: "GET",
        url: "/mvc/messenger?messageIds=" + ids,
        data: {},
        success: function (messages) {
            var content = $($.parseHTML(messages));
            var isFile = content.find(".media-body").length > 0;

            if (isFile) {
                $.ajax({
                    type: "GET",
                    url: "/mvc/messenger/files?messageIds=" + ids,
                    data: {},
                    success: function(files) {
                        $(".js-files-deprecated").remove();
                        $(".js-files").prepend($($.parseHTML(files)));
                    }
                });
            }

            receiveMessage(content);
            $("#chat-form").removeClass("message-failed");
            if (!sent) {
                play("notify");
            }
        }
    }).fail(function () {
        if (sent) {
            $("#chat-form").addClass("message-failed");
        }
    });
}

function sendMessage(message) {
    $("#chat-input").val("");
    var username = getUsername();

    $.post("/api/messenger/messages?username=" + username, {
        content: message
    }, "json")
        .done(function (response) {
            getMessage(response + "", true);
        })
        .fail(function (data) {
            console.log("failed: " + data);
        });

    $(".js-start-messaging").remove();
    $(".js-start-chats").remove();
}

function chat() {
    // Dropzone

    if (document.querySelector("#dropzone-template-js")) {
        var template = document.querySelector("#dropzone-template-js");
        var templateElement = document.querySelector("#dropzone-template-js");
        templateElement.parentNode.removeChild(templateElement);
    }

    [].forEach.call(document.querySelectorAll(".js-chat"), function (el) {

        var button = el.querySelector(".dropzone-button-js")
        var clickable = button.id;
        var url = button.getAttribute("data-dz-url");
        var previewsContainer = el.querySelector(".dropzone-previews-js");

        var myDropzone = new Dropzone(el, {
            url: url,
            previewTemplate: template.innerHTML,
            previewsContainer: previewsContainer,
            autoProcessQueue: false,
            clickable: "#" + clickable,
            init: function () {
                var submitButton = $("#chat-btn");
                myDropzone = this;
                var input = $("#chat-input");

                function send() {
                    if (input.val()) {
                        sendMessage(input.val());
                    }
                    myDropzone.processQueue(); // Tell Dropzone to process all queued files.
                }

                $("#chat-input").on("keypress", function (e) {
                    if (e.keyCode === 13 && !e.shiftKey) {
                        send();
                        e.preventDefault();
                    }
                });

                submitButton.on("click", function (e) {
                    send();
                    e.preventDefault();
                });
            },
            success: function(file, response) {
                //var args = Array.prototype.slice.call(arguments);
                // Look at the output in you browser console, if there is something interesting
                file.previewElement.parentNode.removeChild(file.previewElement);
                getMessage(response, true);
            }
        });
    });
}

$(document).ready(function () {
    register();

    chat();

    if (document.querySelector(".end-of-chat")) {
        document.querySelector(".end-of-chat").scrollIntoView();
    }
});