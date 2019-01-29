$('.clickable').click(function () {
    console.log($(this).attr('id'))
    $(this).toggleClass("selected");
    
    console.log($(this).attr('class'))
    $.ajax({
        type: "POST",
        url: "RSSFeed/Index",
        data: jQuery.param({ RSSURL: $(this).attr('title') }),
        error: function (jqXHR, textStatus, errorThrown) {
            alert(jqXHR);
        },
        success: function (result) {
            
            $("body").html(result); 
            
        }
         
    });
   
    console.log($(this).attr('class'))
});
$(document).ready(function () {
    $('a').click(function () {
        $(this).toggleClass("selected");
    });
});
