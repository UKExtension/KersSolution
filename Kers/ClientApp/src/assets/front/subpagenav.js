$(document).ready(function () {
        console.log('loaded');
    $("div.mobile-navigation__menu-tab-quick .button").on('click',function(event){
        event.preventDefault();
        console.log('clicked');
        $(".mobile_main").show();
    });




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
    });
});
