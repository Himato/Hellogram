(function($) {

    "use strict";

    Dropzone.autoDiscover = false;

    $(document).ready(function() {

        //
        // Detect mobile devices
        //

        var isMobile = {
            Android: function() {
                return navigator.userAgent.match(/Android/i);
            },
            BlackBerry: function() {
                return navigator.userAgent.match(/BlackBerry/i);
            },
            iOS: function() {
                return navigator.userAgent.match(/iPhone|iPod|iPad/i);
            },
            Opera: function() {
                return navigator.userAgent.match(/Opera Mini/i);
            },
            Windows: function() {
                return navigator.userAgent.match(/IEMobile/i) || navigator.userAgent.match(/WPDesktop/i);
            },
            any: function() {
                return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
            }
        };

        //
        // Emoji
        //

        if ( !isMobile.any() ) {
            [].forEach.call(document.querySelectorAll("[data-emoji-form]"), function (form) {
                var button = form.querySelector("[data-emoji-btn]");

                var picker = new EmojiButton({
                    position: "top",
                    zIndex: 1020
                });

                picker.on("emoji", function(emoji) {
                    form.querySelector("[data-emoji-input]").value += emoji;
                });

                button.addEventListener("click", function () {
                    picker.pickerVisible ? picker.hidePicker() : picker.showPicker(button);
                });
            });
        } else {
            [].forEach.call(document.querySelectorAll("[data-emoji-form]"), function (form) {
                form.querySelector("[data-emoji-btn]").style.display = "none";
            });
        }

        //
        // Toggle chat
        //

        [].forEach.call(document.querySelectorAll('[data-chat="open"]'), function (a) {
            a.addEventListener("click", function () {
                document.querySelector(".main").classList.toggle("main-visible");
            }, false );
        });

        //
        // Toggle chat`s sidebar
        //

        [].forEach.call(document.querySelectorAll("[data-chat-sidebar-toggle]"), function (e) {
            e.addEventListener("click", function (event) {
                event.preventDefault();
                var chatSidebarId = e.getAttribute("data-chat-sidebar-toggle");
                var chatSidebar = document.querySelector(chatSidebarId);

                if (typeof(chatSidebar) != "undefined" && chatSidebar != null) {
                    if ( chatSidebar.classList.contains("chat-sidebar-visible") ) {
                        chatSidebar.classList.remove("chat-sidebar-visible")
                    } else {
                        [].forEach.call(document.querySelectorAll(".chat-sidebar"), function (e) {
                            e.classList.remove("chat-sidebar-visible");
                        });
                        chatSidebar.classList.add("chat-sidebar-visible");
                    }
                }

            });
        });

        //
        // Close all chat`s sidebars
        //

        [].forEach.call(document.querySelectorAll("[data-chat-sidebar-close]"), function (a) {
            a.addEventListener("click", function (event) {
                event.preventDefault();
                [].forEach.call(document.querySelectorAll(".chat-sidebar"), function (a) {
                    a.classList.remove("chat-sidebar-visible");
                });
            }, false );
        });

        //
        // Mobile screen height minus toolbar height
        //

        function mobileScreenHeight() {
            if ( document.querySelectorAll(".navigation").length && document.querySelectorAll(".sidebar").length ) {
                document.querySelector(".sidebar").style.height = windowHeight - document.querySelector(".navigation").offsetHeight + "px";
            }
        }

        if ( isMobile.any() && (document.documentElement.clientWidth < 1024) ) {
            var windowHeight = document.documentElement.clientHeight;
            mobileScreenHeight();

            window.addEventListener("resize", function(event){
                if (document.documentElement.clientHeight != windowHeight) {
                    windowHeight = document.documentElement.clientHeight;
                    mobileScreenHeight();
                }
            });
        }

        //
        // Autosize
        //

        autosize(document.querySelectorAll('[data-autosize="true"]'));

        //
        // SVG inject
        //

        SVGInjector(document.querySelectorAll("img[data-inject-svg]"));

    });

})(jQuery);

function changeTheme(sender) {
    if (sender.prop("checked")) {
        $("#theme-style").attr("href", "/Content/Messenger/Assets/main.dark.css");
    } else {
        $("#theme-style").attr("href", "/Content/Messenger/Assets/main.css");
    }
}

function changeActiveState(sender) {
    if (sender.prop("checked")) {
        $("#user-avatar").addClass("avatar-online");
    } else {
        $("#user-avatar").removeClass("avatar-online");
    }
}

function changePreferences() {
    $.ajax({
        url: "/api/messenger/preferences/",
        type: "PUT",
        data: {
            notificationSound: $("#notifications-sound-switch").prop("checked"),
            darkTheme: $("#dark-theme-switch").prop("checked"),
            activeState: $("#active-state-switch").prop("checked")
        }
    });
}

function play(filename) {
    try {
        if ($("#notifications-sound-switch").prop("checked")) {
            var audio = new Audio("/Content/Audio/" + filename + ".mp3");
            audio.play();
        }
    } catch (exception) {
        console.log("interact with the page");
    }
}

function getUsername() {
    return $(".js-chat").first().attr("data-username");
}

function register() {
    var messenger = $.connection.messengerHub;
    messenger.client.receive = function (username, ids) {
        receive(ids);
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
}

function receive(ids) {
    $.ajax({
        type: "GET",
        url: "/mvc/messenger?messageIds=" + ids,
        data: {},
        success: function(messages) {
            changeMember($($.parseHTML(messages.responseText)), false);
            play("notify");
        }
    });
}

function changeMember(content, sent) {
    var username = content.attr("data-username");
    var lastMessage = content.find(".message-body").last();

    var message;
    if (lastMessage.find(".message-content > p").length > 0) {
        message = lastMessage.find(".message-content > p").first().text();
    } else {
        message = "Sent an Attachment";
    }

    if (sent) {
        message = "You: " + message;
        username = getUsername();
    }

    var open = username === getUsername();

    var changed = false;

    $("#tab-content-dialogs").find(".card").each(function () {
        if ($(this).attr("data-username") === username) {

            $(this).find(".js-last-message").first().text(message);

            var badge = $(this).find(".badge").first();
            if (open) {
                badge.addClass("hide");
                badge.find("span").first().text(0);
            } else {
                badge.removeClass("hide");
                badge.find("span").first().text(+badge.text() + 1);
            }

            var time = lastMessage.find(".message-content").first().attr("data-sent-time");
            $(this).find(".js-last-message-time").first().text(time);

            var me = $(this).clone(true);
            $(this).remove();
            $("#tab-content-dialogs").find(".nav").first().prepend(me);

            changed = true;
        }
    });

    // Member doesn't exist
    if (!changed) {
        $.ajax({
            type: "GET",
            url: "/mvc/messenger/members?username=" + username,
            data: {},
            success: function(messages) {
                $("#tab-content-dialogs").find(".nav").first().prepend(messages);
            }
        });
    }

    activeChats();
}

function activeChats() {
    var b = $("#menu").find(".nav-item").find(".badge").first();
    b.addClass("hide");

    $("#tab-content-dialogs").find(".card").each(function () {
        var badge = $(this).find(".badge > span").first();
        if (badge.text() != 0) {
            b.removeClass("hide");
        }
    });
}

function changeState(username, online, lastLogin) {

    function changeLastLogin(member, isChat) {
        if (lastLogin) {
            var text = member.find(".js-last-login").first();
            if (text.length !== 0) {
                text.text(lastLogin);
            } else {
                if (!isChat) {
                    member.find(".media-body").first()
                        .append('<small class="text-muted js-last-login">' + lastLogin + "</small>");
                }
            }
        } else {
            var buffer = member.find(".js-last-login").first();
            if (buffer) {
                buffer.remove();
            }
        }
    }

    function changeMemberState(member, isChat) {
        if (online) {
            member.find(".avatar").first().addClass("avatar-online");
            lastLogin = "Online";
        } else {
            member.find(".avatar").first().removeClass("avatar-online");
        }

        changeLastLogin(member, isChat);
    };

    if (getUsername() === username) {
        changeMemberState($(".js-chat").find(".js-chat-photo").first(), false);
    }

    $("#tab-content-dialogs").find(".card").each(function () {
        if ($(this).attr("data-username") === username) {
            changeMemberState($(this), true);
        }
    });

    $("#tab-content-people").find(".card").each(function () {
        if ($(this).attr("data-username") === username) {
            changeMemberState($(this), false);
        }
    });
}

function search(text) {
    $.ajax({
        type: "GET",
        url: "/mvc/messenger/search?q=" + text,
        data: {},
        success: function(messages) {
            var result = $($.parseHTML(messages));
            $("#tab-content-people").find("nav").remove().append(result);
            $("#people-container").append(result);
        }
    });
}

$(document).ready(function () {
    register();
    changeTheme($("#dark-theme-switch"));

    $(".js-logout").click(function () { $(this).parent("#logoutForm").submit(); });

    $("#notifications-sound-switch").change(function (e) {
        changePreferences();
        e.preventDefault();
    });

    $("#dark-theme-switch").change(function (e) {
        changeTheme($(this));
        changePreferences();
        e.preventDefault();
    });

    $("#active-state-switch").change(function (e) {
        changeActiveState($(this));
        changePreferences();
        e.preventDefault();
    });

    $(".js-search").click(function () {
        var isPeopleSearch = $(this).parent("#tab-content-people").length !== 0;

        if (!isPeopleSearch) {
            $(".nav-link[title='People']").trigger("click");
            setTimeout(function() {
                    $("#tab-content-people").find(".js-search").first().focus();
                },
                1000);
        }
    });

    $(".js-search").keyup(function (event) {
        var isWordCharacter = event.key.length === 1;
        var isBackspaceOrDelete = event.keyCode === 8 || event.keyCode === 46;

        if (isWordCharacter || isBackspaceOrDelete) {
            search($(this).val());
        }
    });
});