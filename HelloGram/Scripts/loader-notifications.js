
function markRead(id, href) {
    $.ajax({
        type: "PUT",
        url: "/api/mark_read?id=" + id,
        data: {},
        success: function () {
            if (href != null) {
                window.location.href = href;
            }
        }
    });
};

$(document).ready(function () {
    $(".js-notifications-number").each(function() {
        $(this).addClass("hide");
    });
    $(".js-mark-as-read").click(function () {
        $(".js-notification > .notification-body").each(function () {
            $(this).removeClass("new-notification");
        });
        $(".js-notifications-number").each(function() {
            $(this).text(0 + "");
            $(this).addClass("hide");
        });
        markRead(null, null);
    });

    $(".js-notification").click(function (e) {
        e.preventDefault();
        $(this).find(".notification-body").first().removeClass("new-notification");
        markRead($(this).attr("data-notification-id"), $(this).attr("href"));
    });
});