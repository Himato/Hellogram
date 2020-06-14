function play(filename) {
    var audio = new Audio("/Content/Audio/" + filename + ".mp3");
    audio.play();
}

function fail(message) {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    toastr["error"](message, "Failed");
}

function success(message, title) {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    toastr["success"](message, title);
}

function notify(message) {
    play("notify");
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": true,
        "onclick": function() {
            window.location.replace("/Notifications");
        },
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    toastr["info"](message, "");
}

function follow(sender) {
    var id = sender.attr("data-user-id");

    $(".js-post-follow").each(function () {
        if ($(this).attr("data-user-id") === id) {
            var text = $(this).children(".js-post-follow-text").first();
            if (text.text().trim() === "Follow") {
                text.text("Unfollow");
            } else {
                text.text("Follow");
            }
        }
    });

    $(".js-profile-container").each(function () {
        if ($(this).attr("data-user-id") === id) {
            var btn = $(this).find(".js-card-follow").first();
            var number = $(this).find(".js-followers-number").first();
            if (btn.text().trim() === "Follow") {
                btn.text("Unfollow");
                btn.removeClass("px-3");
                btn.removeClass("btn-primary");
                btn.addClass("btn-neutral");
                if (!number.text().trim().includes("K")) {
                    number.text(+number.text() + 1);
                }
            } else {
                btn.text("Follow");
                btn.removeClass("btn-neutral");
                btn.addClass("px-3");
                btn.addClass("btn-primary");
                if (!number.text().trim().includes("K")) {
                    number.text(+number.text() - 1);
                }
            }
        }
    });
    $.ajax({
        url: "/api/followings?id=" + sender.attr("data-user-id"),
        type: "PUT",
        data: {}
    })
        .fail(function () {
            fail("Something went wrong");
        });
}

function react(id, like) {
    $.ajax({
        type: "PUT",
        url: "/api/posts/react/" + id + "?isLike=" + like,
        data: {}
    }).fail(function () {
        fail("Something went wrong");
    });
}

function reactEffect(object, numberSelector, color, otherSelector, otherNumberSelector, otherColor) {
    var number = object.parents(".js-post-container").first().find(numberSelector).first();
    if (!number.text().trim().includes("K")) {
        if (object.hasClass(color)) {
            number.text(+number.text() - 1);
        } else {
            number.text(+number.text() + 1);
        }
    }

    var other = object.parents(".js-react").first().find(otherSelector).first();
    var number2 = object.parents(".js-post-container").first().find(otherNumberSelector).first();
    if (!number2.text().trim().includes("K")) {
        if (other.hasClass(otherColor)) {
            number2.text(+number2.text() - 1);
        }
    }

    other.removeClass(otherColor);
    object.toggleClass(color);
}

function readMore(container) {
    if (container) {
        container = $($.parseHTML(container));
        container.find(".buffer").each(function () {
            var parent = $(this);
            var text = $(this).text();
            if (text.split(String.fromCharCode(10)).length - 1 >= 3) {
                var splits = text.split(String.fromCharCode(10));
                parent.text(splits[0] + String.fromCharCode(10) + splits[1] + String.fromCharCode(10) + splits[2]).append('<a href="#" class="show-more"> Show More</a>');
            } else {
                parent.text(text.substr(0, 120)).append('<a href="#" class="show-more"> Show More</a>');
            }
            parent.find(".show-more").first().click(function() {
                event.preventDefault();
                $(this).remove();
                parent.text(text);
            });
        });
        $("#home-post-container").append(container);
    } else {
        $(".buffer").each(function () {
            var parent = $(this);
            var text = $(this).text();
            if (text.split(String.fromCharCode(10)).length - 1 >= 3) {
                var splits = text.split(String.fromCharCode(10));
                parent.text(splits[0] + String.fromCharCode(10) + splits[1] + String.fromCharCode(10) + splits[2]).append('<a href="#" class="show-more"> Show More</a>');
            } else {
                parent.text(text.substr(0, 120)).append('<a href="#" class="show-more"> Show More</a>');
            }
            parent.find(".show-more").first().click(function () {
                event.preventDefault();
                $(this).remove();
                parent.text(text);
            });
        });
    }
}

function postMethods() {
    readMore();
    $(document).delegate(".js-post-edit", "click", function () {
        $.getJSON("/api/posts/" + $(this).parents(".js-post-container").attr("data-post-id"),
            {},
            function (post) {
                $("#edit-post-text").val(post.text);
                $("#edit-post-category option").each(function () {
                    if ($(this).text() === post.category.name) {
                        $(this).attr("selected", "selected");
                    }
                });
                $("#edit-post-modal").attr("data-post-id", post.id);
                if (post.image) {
                    $("#edit-post-image-preview").attr("src", "/Content/Images/Posts/" + post.image);
                    $("#edit-post-image-preview-container").removeClass("hide");
                } else {
                    $("#edit-post-image-container").removeClass("hide");
                }
            })
            .fail(function () {
                window.location.reload();
            });
    });
    $(document).delegate(".js-post-copy-url", "click", function () {
        var id = window.location.protocol + "//" +
            window.location.host + "/Users/" +
            $(this).parents(".js-post-container").attr("data-user-name") + "/" + $(this).parents(".js-post-container").attr("data-post-id");
        var el = document.createElement("textarea");
        el.value = id;
        document.body.appendChild(el);
        el.select();
        document.execCommand("copy");
        document.body.removeChild(el);
    });
    $(document).delegate(".js-post-delete", "click", function () {
        $.ajax({
            url: "/api/posts/" + $(this).parents(".js-post-container").attr("data-post-id"),
            type: "DELETE",
            data: {},
            success: function () {
                window.location.replace("/Trash");
            }
        })
            .fail(function (data) {
                fail(data.responseJSON.message);
            });
    });
    $(document).delegate(".js-post-re-post", "click", function () {
        $.ajax({
            url: "/api/posts/" + $(this).parents(".js-post-container").attr("data-post-id") + "?deleted=true",
            type: "PUT",
            data: {},
            success: function (response) {
                window.location.replace("/Users/" + $(".js-post-container").attr("data-user-name") + "/" + response);
            }
        })
            .fail(function (data) {
                fail(data.responseJSON.message);
            });
    });
    $(".js-post-delete-permanently").click(function () {
        var id = $(this).parents(".js-post-container").attr("data-post-id");
        $("#modal-delete-confirmation").attr("data-post-id", id);
    });
    $("#js-post-delete-confirmation").click(function () {
        $("#js-post-delete-confirmation > span").text("");
        $("#js-post-delete-confirmation").addClass("spinner");
        $.ajax({
            url: "/api/posts/" + $(this).parents("#modal-delete-confirmation").attr("data-post-id"),
            type: "DELETE",
            data: {},
            success: function () {
                $("#js-post-delete-confirmation > span").text("");
                $("#js-post-delete-confirmation").addClass("spinner");
                window.location.replace("/Trash");
            }
        })
            .fail(function (data) {
                $("#js-post-delete-confirmation > span").text("Ok, Got it");
                $("#js-post-delete-confirmation").removeClass("spinner");
                fail(data.responseJSON.message);
            });
    });
    $(document).delegate(".js-post-save", "click", function () {
        var text = $(this).children(".js-post-save-text").first();
        if (text.text().trim() === "Save Post") {
            text.text("Unsave Post");
        } else {
            text.text("Save Post");
        }
        $.ajax({
            url: "/api/posts/saves?id=" + $(this).parents(".js-post-container").attr("data-post-id"),
            type: "PUT",
            data: {}
        })
            .fail(function () {
                fail("Something went wrong");
            });
    });
    $(document).delegate(".js-post-follow", "click", function () {
        follow($(this));
    });
    $(document).delegate(".js-card-follow", "click", function () {
        follow($(this).parents(".js-profile-container").first());
    });
    $(document).delegate(".js-post-like", "click", function () {
        reactEffect($(this), ".js-post-like-number", "text-green", ".js-post-dislike", ".js-post-dislike-number", "text-red");

        var id = $(this).parents(".js-post-container").first().attr("data-post-id");
        react(id, true);
    });
    $(document).delegate(".js-post-dislike", "click", function () {
        reactEffect($(this), ".js-post-dislike-number", "text-red", ".js-post-like", ".js-post-like-number", "text-green");

        var id = $(this).parents(".js-post-container").first().attr("data-post-id");
        react(id, false);
    });
    $(document).delegate(".js-post-comment-btn", "click", function () {
        $(this).parents(".js-react").first().toggleClass("mb-2");
        $(this).parents(".js-post-container").first().find(".js-post-divider").first().toggleClass("hide");
        $(this).parents(".js-post-container").first().find(".js-post-comment-container").toggleClass("hide");
    });
    $(document).delegate(".js-post-comment", "keydown", function (event) {
        if (event.keyCode === 13 && !event.shiftKey) {
            var text = $(this).val().trim();
            if (text.length < 2 || text.length > 500) {
                $(this).addClass("border-danger");
                event.preventDefault();
                return;
            } else {
                $(this).removeClass("border-danger");
            }
            var post = $(this).parents(".js-post-container").first();
            $.post("/mvc/posts/comments", {
                postId: post.attr("data-post-id"),
                text: $(this).val()
            }, "json")
                .done(function (response) {
                    // Checking for the UserPost page
                    var n = $(".js-post-container").length;
                    if (n === 2) {
                        return;
                    }
                    var container = post.find(".js-post-comments-container").first();
                    container.prepend(response);
                    container.find(".js-post-comment-container").removeClass("hide");
                    var number = post.find(".js-post-comment-number").first();
                    if (number.text().trim().includes("No")) {
                        number.text("1");
                    } else {
                        if (!number.text().trim().includes("K")) {
                            number.text(+number.text() + 1);
                        }
                    }
                    post.find(".js-turn-on-notifications").first().text("Turn off notifications")
                        .attr("data-state", "True");
                }).fail(function () {
                    $(this).addClass("border-danger");
                    event.preventDefault();
                    return;
                });
            $(this).val("");
            event.preventDefault();
            return;
        }
    });
    $(document).delegate(".js-post-view-more", "click", function () {
        $(this).children(".js-post-view-more-content").first().text("");
        $(this).addClass("spinner");
        var post = $(this).parents(".js-post-container").first();
        $.ajax({
            type: "GET",
            url: "/mvc/posts/comments?postId=" + post.attr("data-post-id"),
            data: {},
            success: function (response) {
                var container = post.find(".js-post-comments-container").first();
                container.empty();
                container.append(response);
                container.find(".js-post-comment-container").removeClass("hide");
                var n = 0;
                container.find(".js-post-comment-container").each(function () {
                    n++;
                });
                if (n === 0) {
                    post.find(".js-post-comment-number").first().text("No");
                } else {
                    if (n >= 1000) {
                        n = n / 1000;
                        post.find(".js-post-comment-number").first().text(n.toFixed(1) + "K");
                    } else {
                        post.find(".js-post-comment-number").first().text(n);
                    }
                }
                $(".js-post-comment-container").tooltip("enable");
            }
        }).fail(function () {
            post.find(".js-post-comments-container").first().empty();
        });
    });
    $(document).delegate(".js-comment-delete", "click", function () {
        var container = $(this).parents(".js-post-comment-container").first();
        var id = container.attr("data-comment-id");
        var number = container.parents(".js-post-container").first().find(".js-post-comment-number").first();
        if (!number.text().trim().includes("K")) {
            if (+number.text().trim() === 1) {
                number.text("No");
            } else {
                number.text(+number.text() - 1);
            }
        }
        container.remove();
        $.ajax({
            type: "DELETE",
            url: "/api/posts/comments?id=" + id,
            data: {}
        });
    });
    $(document).delegate(".js-comment-edit", "click", function () {
        var parent = $(this).parents(".js-post-container").first();
        var container = $(this).parents(".js-post-comment-container").first();
        container.find(".js-comment-dropdown").first().dropdown("toggle");

        var disabled = $(this).parents(".post-comment-disabled").first();
        var text = container.find(".post-comment-text").first().text().trim();
        disabled.remove();

        var col = parent.find(".js-post-textarea").first().clone().appendTo(container);
        var textarea = col.find(".js-post-comment").first();

        textarea.removeClass("js-post-comment");
        textarea.val(text);
        textarea.addClass("js-post-comment-temp");

        $(document).delegate(".js-post-comment-temp", "keydown", function (event) {
            if (event.keyCode === 13 && !event.shiftKey) {
                var value = $(this).val().trim();
                if (value.length < 2 || value.length > 500) {
                    $(this).addClass("border-danger");
                    event.preventDefault();
                    return;
                } else {
                    $(this).removeClass("border-danger");
                }
                var me = $(this);
                $.ajax({
                    type: "PUT",
                    url: "/api/posts/comments?id=" + container.attr("data-comment-id") + "&text=" + value,
                    data: {},
                    success: function () {
                        me.parents(".js-post-textarea").first().remove();
                        container.append(disabled);
                        disabled.find(".post-comment-text").first().text(value);
                    }
                }).fail(function () {
                    $(this).addClass("border-danger");
                    event.preventDefault();
                    return;
                });
                event.preventDefault();
                return;
            }
        });
    });
    $(document).delegate(".js-turn-on-notifications", "click", function () {
        var state = $(this).attr("data-state");
        if (state === "True") {
            $(this).text("Turn on notifications");
            $(this).attr("data-state", "False");
        } else {
            $(this).text("Turn off notifications");
            $(this).attr("data-state", "True");
        }
        $.ajax({
            type: "PUT",
            url: "/api/posts/subscribe?id=" + $(this).parents(".js-post-container").first().attr("data-post-id"),
            data: {}
        });
    });
    $(document).delegate(".js-report", "click", function() {
        success("You report will be consider by our team soon.", "Report");
    });
}

function modalMethods() {
    function preview(input, target) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function(e) {
                $(target).attr("src", e.target.result);
            };
            reader.readAsDataURL(input.files[0]);
        }
    }
    $("#new-post-image").change(function () {
        preview(this, "#new-post-image-preview");
        $("#new-post-image-container").addClass("hide");
        $("#new-post-image-preview-container").removeClass("hide");
    });
    $("#new-post-delete-image").click(function () {
        $("#new-post-image-container").removeClass("hide");
        $("#new-post-image-preview-container").addClass("hide");
        var parent = $(this).parents("#new-post-modal").first();
        parent.find(".custom-file-label").first().text("Add an image");
        parent.find(".custom-file-input").first().val("");
    });
    $("#new-post-btn").click(function () {
        $("#new-post-btn > span").text("");
        $("#new-post-btn").addClass("spinner");
        $.post("/api/posts", {
            text: $("#new-post-text").val(),
            category: $("#new-post-category option:selected").text()
        }, "json")
            .done(function (response) {
                var formData = new FormData();
                var image = $("#new-post-image")[0].files[0];
                if (image) {
                    formData.append("image", image);
                    $.ajax({
                        type: "POST",
                        url: "/api/posts/" + response,
                        data: formData,
                        contentType: false,
                        processData: false
                    })
                        .done(function () {
                            window.location.replace("/Users/" +
                                $("#new-post-modal").attr("data-user-name") +
                                "/" +
                                response);
                        })
                        .fail(function (data) {
                            $("#new-post-btn > span").text("Post");
                            $("#new-post-btn").removeClass("spinner");
                            $("#new-post-validation-message").text(data.responseJSON.message);
                        });
                } else {
                    window.location.replace("/Users/" +
                        $("#new-post-modal").attr("data-user-name") +
                        "/" +
                        response);
                }
            })
            .fail(function (data) {
                $("#new-post-btn > span").text("Post");
                $("#new-post-btn").removeClass("spinner");
                $("#new-post-validation-message").text(data.responseJSON.message);
            });
    });

    $("#edit-post-image").change(function () {
        preview(this, "#edit-post-image-preview");
        $("#edit-post-image-container").addClass("hide");
        $("#edit-post-image-preview-container").removeClass("hide");
        $(this).parents("#edit-post-modal").first().attr("data-image-update", "1");
    });
    $("#edit-post-delete-image").click(function () {
        $("#edit-post-image-container").removeClass("hide");
        $("#edit-post-image-preview-container").addClass("hide");
        var parent = $(this).parents("#edit-post-modal").first();
        parent.find(".custom-file-label").first().text("Add an image");
        parent.find(".custom-file-input").first().val("");
        parent.attr("data-image-update", "1");
    });
    $("#edit-post-btn").click(function () {
        var parent = $(this).parents("#edit-post-modal").first();
        $("#edit-post-btn > span").text("");
        $("#edit-post-btn").addClass("spinner");
        $.ajax({
            url: "/api/posts/" + $("#edit-post-modal").attr("data-post-id"),
            type: "PUT",
            data: {
                text: $("#edit-post-text").val(),
                category: $("#edit-post-category option:selected").text()
            },
            success: function (response) {
                $("#edit-post-btn > span").text("");
                $("#edit-post-btn").addClass("spinner");
                var formData = new FormData();
                var image = $("#edit-post-image")[0].files[0];
                if (parent.attr("data-image-update") === "1") {
                    formData.append("image", image);
                    $.ajax({
                        type: "POST",
                        url: "/api/posts/" + response.id,
                        data: formData,
                        contentType: false,
                        processData: false
                    })
                        .done(function () {
                            window.location.replace("/Users/" + $("#edit-post-modal").attr("data-user-name") + "/" + response);
                        })
                        .fail(function (data) {
                            $("#edit-post-btn > span").text("Post");
                            $("#edit-post-btn").removeClass("spinner");
                            $("#edit-post-validation-message").text(data.responseJSON.message);
                        });
                } else {
                    window.location.replace("/Users/" + $("#edit-post-modal").attr("data-user-name") + "/" + response);
                }
            }
        })
            .fail(function (data) {
                $("#edit-post-btn > span").text("Edit");
                $("#edit-post-btn").removeClass("spinner");
                $("#edit-post-validation-message").text(data.responseJSON.message);
            });
    });

    $("#image-upload-form-btn").click(function () {
        $("#image-upload-form-btn > span").text("");
        $("#image-upload-form-btn").addClass("spinner");
        var formData = new FormData();
        formData.append("image", $("#image-upload-form")[0].files[0]);
        $.ajax({
            type: "PUT",
            url: "/Users/Images",
            data: formData,
            contentType: false,
            processData: false
        })
            .done(function () {
                $("#image-upload-form-btn > span").text("");
                $("#image-upload-form-btn").addClass("spinner");
                window.location.reload();
            })
            .fail(function (data) {
                $("#image-upload-form-btn > span").text("Upload");
                $("#image-upload-form-btn").removeClass("spinner");
                $("#image-upload-form-validation-message").text(data.responseJSON.message);
            });
    });

    $(".custom-file-input").on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
    });
}

function register() {
    var notification = $.connection.notificationsHub;
    notification.client.notify = function (message, number) {
        $(".js-notifications-number").each(function () {
            $(this).text(number);
            $(this).removeClass("hide");
        });
        notify(message);
    };

    var messenger = $.connection.messengerHub;
    messenger.client.notify = function () {
        play("notify");
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-bottom-right",
            "preventDuplicates": true,
            "onclick": function () {
                window.location.replace("/Messenger");
            },
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        toastr["success"]("There's are more new messages. Check your Messenger.", "Messenger");
    };

    $.connection.hub.start().done(function () {
        notification.server.register();
        messenger.server.register();
    });
}

register();

$(document).ready(function () {
    postMethods(), modalMethods(), $(".js-logout").click(function() {
        $(this).parent("#logoutForm").submit();
    });
});