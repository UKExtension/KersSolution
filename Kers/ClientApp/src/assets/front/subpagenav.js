$(document).ready(function () {
    $(".mobile-naviation__quick-links").css("display", "none");
    var isLateralOpen = false;
    var isMobileOpen = false;
    var isMobileMainOpen = true;
    var isMobileResourcesOpen = false;
    $(".expand-lateral-menu").on('click',function(event){
        event.preventDefault();
        if(isLateralOpen){
            $(".lateral-navigation").css("display", "none");
            $("#main-content").css('transform', 'translateX(0px)');
        }else{
            $(".lateral-navigation").css("display", "block");
            $("#main-content").css('transform', 'translateX(-350px)');
        }
        isLateralOpen = !isLateralOpen;
    });
    $(".lateral-navigation__closer").css("cursor","pointer").on("click",function(){
        $(".lateral-navigation").css("display", "none");
        $("#main-content").css('transform', 'translateX(0px)');
        isLateralOpen = false;
    })

    $("#upper-nav__mobile-menu-trigger").on('click',function(event){
        if(isMobileOpen){
            $(".mobile-navigation").css("display", "none");
        }else{
            $(".mobile-navigation").css("display", "block");
        }
        isMobileOpen = !isMobileOpen;
    });
    $( window ).on( "resize", function() {
        $(".mobile-navigation").css("display", "none");
        isMobileOpen =false;
      } );


    $(".mobile-navigation__menu-tab-quick").on("click", function(){
        if(!isMobileResourcesOpen){
            $(".mobile-naviation__quick-links").css("display", "block");
            $(".mobile-naviation__main-menu").css("display", "none");
            $(".mobile-navigation__menu-tab-quick").children("a").addClass("mobile-navigation__menu-tab-hover");
            $(".mobile-navigation__menu-tab-main").children("a").removeClass("mobile-navigation__menu-tab-hover");
            isMobileMainOpen=false;
            isMobileResourcesOpen=true;
        }
    });
    $(".mobile-navigation__menu-tab-main").on("click", function(){
        if(!isMobileMainOpen){
            $(".mobile-naviation__main-menu").css("display", "block");
            $(".mobile-naviation__quick-links").css("display", "none");
            $(".mobile-navigation__menu-tab-quick").children("a").removeClass("mobile-navigation__menu-tab-hover");
            $(".mobile-navigation__menu-tab-main").children("a").addClass("mobile-navigation__menu-tab-hover");
            isMobileMainOpen=true;
            isMobileResourcesOpen=false;
        }
    });



      /* 

    $("div.subpagenav__link").click(function () {
        $(this).next().toggleClass("sub-open");
        $(this).children("i").toggleClass("fa-angle-double-right fa-angle-double-down");
    });

    $("div.subpagenav__sub-parent-link").click(function () {
        if ($(this).next().is("ul.subpagenav__sub-level-1")) {
            $(this).next().toggleClass("sub-open");
            $(this).children("i").toggleClass("fa-angle-double-right fa-angle-double-down");
        } else if ($(this).next().is("ul.subpagenav__sub-level-2")) {
            $(this).next().toggleClass("sub-open");
            $(this).children("i").toggleClass("fa-angle-double-right fa-angle-double-down");
        } else if ($(this).next().is("ul.subpagenav__sub-level-3")) {
            $(this).next().toggleClass("sub-open");
            $(this).children("i").toggleClass("fa-angle-double-right  fa-angle-double-down");
        } else if ($(this).next().is("ul.subpagenav__sub-level-4")) {
            $(this).next().toggleClass("sub-open");
            $(this).children("i").toggleClass("fa-angle-double-right fa-angle-double-down");
        } else if ($(this).next().is("ul.subpagenav__sub-level-5")) {
            $(this).next().toggleClass("sub-open");
            $(this).children("i").toggleClass("fa-angle-double-right fa-angle-double-down");
        } else if ($(this).next().is("ul.subpagenav__sub-level-6")) {
            $(this).next().toggleClass("sub-open");
            $(this).children("i").toggleClass("fa-angle-double-right fa-angle-double-down");
        } else if ($(this).next().is("ul.subpagenav__sub-level-7")) {
            $(this).next().toggleClass("sub-open");
            $(this).children("i").toggleClass("fa-angle-double-right fa-angle-double-down");
        } else if ($(this).next().is("ul.subpagenav__sub-level-8")) {
            $(this).next().toggleClass("sub-open");
            $(this).children("i").toggleClass("fa-angle-double-right fa-angle-double-down");
        } else if ($(this).next().is("ul.subpagenav__sub-level-9")) {
            $(this).next().toggleClass("sub-open");
            $(this).children("i").toggleClass("fa-angle-double-right fa-angle-double-down");
        } 
    });*/
});
