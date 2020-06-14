
$(document).ready(function () {
    $(".js-category-subscribe").click(function () {
        var number = $(this).parents("#category-container").find(".js-subscribe-number").first();
        if ($(this).text().trim() === "Subscribe") {
            $(this).text("Unsubscribe");
            $(this).removeClass("btn-primary");
            $(this).addClass("btn-neutral");
            if (!number.text().includes("K")) {
                number.text(+number.text() + 1);
            }
        } else {
            $(this).text("Subscribe");
            $(this).addClass("btn-primary");
            $(this).removeClass("btn-neutral");
            if (!number.text().includes("K")) {
                number.text(+number.text() - 1);
            }
        }
        $.ajax({
            url: "/api/subscriptions?id=" + $(this).parents("#category-container").attr("data-category-id"),
            type: "PUT",
            data: {}
        })
            .fail(function () {
                fail("Something went wrong");
            });
    });
});