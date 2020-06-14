
function loadMoreEffect(on) {
    if (on) {
        $("#load-more-btn > span").text("");
        $("#load-more-btn").addClass("spinner");
    } else {
        $("#load-more-btn > span").text("Load More");
        $("#load-more-btn").removeClass("spinner");
    }
};

$(document).ready(function () {
    $("#load-more-btn").click(function () {
        loadMoreEffect(true);
        var str = "";
        $(".js-post-container").each(function () {
            str = str.concat($(this).attr("data-post-id")).concat("_");
        });
        $.ajax({
            type: "GET",
            url: "/mvc/posts/home",
            data: {
                "loaded": str
            },
            success: function (response) {
                readMore(response);
                
                if (response.trim() === "") {
                    $("#home-post-container").append('<div class="card shadow mb-4"> <div class="card-body"> <p class="post-text text-center">There is no more posts in here.</p> </div></div>');
                    $("#load-more-btn").remove();
                }
                loadMoreEffect(false);
            }
        }).fail(function () {
            loadMoreEffect(false);
            fail("Something went wrong");
        });
    });
});