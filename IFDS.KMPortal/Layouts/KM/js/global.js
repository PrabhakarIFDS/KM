
$(document).ready(function(){
// faq collapse
$('.collapse').on('shown.bs.collapse', function(){
$(this).parent().find(".glyphicon-chevron-right").removeClass("glyphicon-chevron-right").addClass("glyphicon-chevron-down");
}).on('hidden.bs.collapse', function(){
$(this).parent().find(".glyphicon-chevron-down").removeClass(" glyphicon-chevron-down").addClass("glyphicon-chevron-right");
});
// end faq collaps


//my favs viewall
$("#view-news").click(function(){
    $("#home").hide();
    $("#favAll").show();
});
//discussion forum view all

$("#view-forum").click(function(){
    $("#home").hide();
    $("#discussAll").show();
});

//top contibuters view all
$("#view-topcontributer").click(function(){
   $("#home").hide();
    $("#top-contributer").show();
}); 

//Faqs view all

$("#view-faqs").click(function(){
    $("#home").hide();
    $("#faqsAll").show();
});

$("#search").click(function(){
    $("#searchResult").hide();
    $("#resultPage").show();
});
$("#BusinessSearch a").click(function(){
    $("#searchResult").hide();
    $("#resultPage").show();
});



});