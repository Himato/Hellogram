
var comment = $.connection.commentsHub;
comment.client.notify = function (commentId) {
    $.ajax({
        type: "GET",
        url: "/mvc/posts/comment?commentId=" + commentId,
        data: {},
        success: function (response) {
            var post = $(".js-post-container").first();
            var container = post.find(".js-post-comments-container").first();
            container.prepend(response);

            post.find(".js-react").first().removeClass("mb-2");
            post.find(".js-post-divider").first().removeClass("hide");
            post.find(".js-post-comment-container").removeClass("hide");
            container.find(".js-post-comment-container").removeClass("hide");

            var number = post.find(".js-post-comment-number").first();
            if (!number.text().trim().includes("K")) {
                if (number.text().trim().includes("No")) {
                    number.text("1");
                } else {
                    number.text(+number.text() + 1);
                }
            }
            $(".js-post-comment-container").tooltip("enable");
        }
    });
};
$.connection.hub.start().done(function () {
    comment.server.register($(".js-post-container").first().attr("data-post-id"));
});