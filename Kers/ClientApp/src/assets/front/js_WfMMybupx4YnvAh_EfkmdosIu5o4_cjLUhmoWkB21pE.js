/* global a2a*/
(function (Drupal) {
  'use strict';

  Drupal.behaviors.addToAny = {
    attach: function (context, settings) {
      // If not the full document (it's probably AJAX), and window.a2a exists
      if (context !== document && window.a2a) {
        a2a.init_all(); // Init all uninitiated AddToAny instances
      }
    }
  };

})(Drupal);
;
(window.webpackJsonp=window.webpackJsonp||[]).push([[0],[function(t,n,r){(function(n){var r=function(t){return t&&t.Math==Math&&t};t.exports=r("object"==typeof globalThis&&globalThis)||r("object"==typeof window&&window)||r("object"==typeof self&&self)||r("object"==typeof n&&n)||Function("return this")()}).call(this,r(37))},function(t,n){t.exports=function(t){try{return!!t()}catch(t){return!0}}},function(t,n){t.exports=function(t){return"object"==typeof t?null!==t:"function"==typeof t}},function(t,n){var r={}.hasOwnProperty;t.exports=function(t,n){return r.call(t,n)}},function(t,n,r){var e=r(6),o=r(15),i=r(12);t.exports=e?function(t,n,r){return o.f(t,n,i(1,r))}:function(t,n,r){return t[n]=r,t}},function(t,n){t.exports=function(t){if(null==t)throw TypeError("Can't call method on "+t);return t}},function(t,n,r){var e=r(1);t.exports=!e((function(){return 7!=Object.defineProperty({},"a",{get:function(){return 7}}).a}))},function(t,n,r){var e=r(19),o=r(5);t.exports=function(t){return e(o(t))}},function(t,n,r){var e=r(2);t.exports=function(t){if(!e(t))throw TypeError(String(t)+" is not an object");return t}},function(t,n,r){var e=r(17),o=Math.min;t.exports=function(t){return t>0?o(e(t),9007199254740991):0}},function(t,n,r){var e=r(0),o=r(24),i=r(3),u=r(25),c=r(32),f=r(55),a=o("wks"),s=e.Symbol,l=f?s:u;t.exports=function(t){return i(a,t)||(c&&i(s,t)?a[t]=s[t]:a[t]=l("Symbol."+t)),a[t]}},function(t,n,r){var e=r(0),o=r(18).f,i=r(4),u=r(21),c=r(16),f=r(44),a=r(51);t.exports=function(t,n){var r,s,l,p,v,h=t.target,x=t.global,y=t.stat;if(r=x?e:y?e[h]||c(h,{}):(e[h]||{}).prototype)for(s in n){if(p=n[s],l=t.noTargetGet?(v=o(r,s))&&v.value:r[s],!a(x?s:h+(y?".":"#")+s,t.forced)&&void 0!==l){if(typeof p==typeof l)continue;f(p,l)}(t.sham||l&&l.sham)&&i(p,"sham",!0),u(r,s,p,t)}}},function(t,n){t.exports=function(t,n){return{enumerable:!(1&t),configurable:!(2&t),writable:!(4&t),value:n}}},function(t,n){var r={}.toString;t.exports=function(t){return r.call(t).slice(8,-1)}},function(t,n,r){var e=r(2);t.exports=function(t,n){if(!e(t))return t;var r,o;if(n&&"function"==typeof(r=t.toString)&&!e(o=r.call(t)))return o;if("function"==typeof(r=t.valueOf)&&!e(o=r.call(t)))return o;if(!n&&"function"==typeof(r=t.toString)&&!e(o=r.call(t)))return o;throw TypeError("Can't convert object to primitive value")}},function(t,n,r){var e=r(6),o=r(20),i=r(8),u=r(14),c=Object.defineProperty;n.f=e?c:function(t,n,r){if(i(t),n=u(n,!0),i(r),o)try{return c(t,n,r)}catch(t){}if("get"in r||"set"in r)throw TypeError("Accessors not supported");return"value"in r&&(t[n]=r.value),t}},function(t,n,r){var e=r(0),o=r(4);t.exports=function(t,n){try{o(e,t,n)}catch(r){e[t]=n}return n}},function(t,n){var r=Math.ceil,e=Math.floor;t.exports=function(t){return isNaN(t=+t)?0:(t>0?e:r)(t)}},function(t,n,r){var e=r(6),o=r(38),i=r(12),u=r(7),c=r(14),f=r(3),a=r(20),s=Object.getOwnPropertyDescriptor;n.f=e?s:function(t,n){if(t=u(t),n=c(n,!0),a)try{return s(t,n)}catch(t){}if(f(t,n))return i(!o.f.call(t,n),t[n])}},function(t,n,r){var e=r(1),o=r(13),i="".split;t.exports=e((function(){return!Object("z").propertyIsEnumerable(0)}))?function(t){return"String"==o(t)?i.call(t,""):Object(t)}:Object},function(t,n,r){var e=r(6),o=r(1),i=r(39);t.exports=!e&&!o((function(){return 7!=Object.defineProperty(i("div"),"a",{get:function(){return 7}}).a}))},function(t,n,r){var e=r(0),o=r(4),i=r(3),u=r(16),c=r(22),f=r(40),a=f.get,s=f.enforce,l=String(String).split("String");(t.exports=function(t,n,r,c){var f=!!c&&!!c.unsafe,a=!!c&&!!c.enumerable,p=!!c&&!!c.noTargetGet;"function"==typeof r&&("string"!=typeof n||i(r,"name")||o(r,"name",n),s(r).source=l.join("string"==typeof n?n:"")),t!==e?(f?!p&&t[n]&&(a=!0):delete t[n],a?t[n]=r:o(t,n,r)):a?t[n]=r:u(n,r)})(Function.prototype,"toString",(function(){return"function"==typeof this&&a(this).source||c(this)}))},function(t,n,r){var e=r(23),o=Function.toString;"function"!=typeof e.inspectSource&&(e.inspectSource=function(t){return o.call(t)}),t.exports=e.inspectSource},function(t,n,r){var e=r(0),o=r(16),i=e["__core-js_shared__"]||o("__core-js_shared__",{});t.exports=i},function(t,n,r){var e=r(43),o=r(23);(t.exports=function(t,n){return o[t]||(o[t]=void 0!==n?n:{})})("versions",[]).push({version:"3.4.8",mode:e?"pure":"global",copyright:"© 2019 Denis Pushkarev (zloirock.ru)"})},function(t,n){var r=0,e=Math.random();t.exports=function(t){return"Symbol("+String(void 0===t?"":t)+")_"+(++r+e).toString(36)}},function(t,n){t.exports={}},function(t,n,r){var e=r(46),o=r(0),i=function(t){return"function"==typeof t?t:void 0};t.exports=function(t,n){return arguments.length<2?i(e[t])||i(o[t]):e[t]&&e[t][n]||o[t]&&o[t][n]}},function(t,n,r){var e=r(7),o=r(9),i=r(29),u=function(t){return function(n,r,u){var c,f=e(n),a=o(f.length),s=i(u,a);if(t&&r!=r){for(;a>s;)if((c=f[s++])!=c)return!0}else for(;a>s;s++)if((t||s in f)&&f[s]===r)return t||s||0;return!t&&-1}};t.exports={includes:u(!0),indexOf:u(!1)}},function(t,n,r){var e=r(17),o=Math.max,i=Math.min;t.exports=function(t,n){var r=e(t);return r<0?o(r+n,0):i(r,n)}},function(t,n,r){"use strict";var e=r(1);t.exports=function(t,n){var r=[][t];return!r||!e((function(){r.call(null,n||function(){throw 1},1)}))}},function(t,n,r){var e=r(13);t.exports=Array.isArray||function(t){return"Array"==e(t)}},function(t,n,r){var e=r(1);t.exports=!!Object.getOwnPropertySymbols&&!e((function(){return!String(Symbol())}))},function(t,n){t.exports="\t\n\v\f\r                　\u2028\u2029\ufeff"},function(t,n,r){"use strict";var e,o,i=r(71),u=RegExp.prototype.exec,c=String.prototype.replace,f=u,a=(e=/a/,o=/b*/g,u.call(e,"a"),u.call(o,"a"),0!==e.lastIndex||0!==o.lastIndex),s=void 0!==/()??/.exec("")[1];(a||s)&&(f=function(t){var n,r,e,o,f=this;return s&&(r=new RegExp("^"+f.source+"$(?!\\s)",i.call(f))),a&&(n=f.lastIndex),e=u.call(f,t),a&&e&&(f.lastIndex=f.global?e.index+e[0].length:n),s&&e&&e.length>1&&c.call(e[0],r,(function(){for(o=1;o<arguments.length-2;o++)void 0===arguments[o]&&(e[o]=void 0)})),e}),t.exports=f},,function(t,n,r){"use strict";var e=r(11),o=r(28).indexOf,i=r(30),u=[].indexOf,c=!!u&&1/[1].indexOf(1,-0)<0,f=i("indexOf");e({target:"Array",proto:!0,forced:c||f},{indexOf:function(t){return c?u.apply(this,arguments)||0:o(this,t,arguments.length>1?arguments[1]:void 0)}})},function(t,n){var r;r=function(){return this}();try{r=r||new Function("return this")()}catch(t){"object"==typeof window&&(r=window)}t.exports=r},function(t,n,r){"use strict";var e={}.propertyIsEnumerable,o=Object.getOwnPropertyDescriptor,i=o&&!e.call({1:2},1);n.f=i?function(t){var n=o(this,t);return!!n&&n.enumerable}:e},function(t,n,r){var e=r(0),o=r(2),i=e.document,u=o(i)&&o(i.createElement);t.exports=function(t){return u?i.createElement(t):{}}},function(t,n,r){var e,o,i,u=r(41),c=r(0),f=r(2),a=r(4),s=r(3),l=r(42),p=r(26),v=c.WeakMap;if(u){var h=new v,x=h.get,y=h.has,g=h.set;e=function(t,n){return g.call(h,t,n),n},o=function(t){return x.call(h,t)||{}},i=function(t){return y.call(h,t)}}else{var d=l("state");p[d]=!0,e=function(t,n){return a(t,d,n),n},o=function(t){return s(t,d)?t[d]:{}},i=function(t){return s(t,d)}}t.exports={set:e,get:o,has:i,enforce:function(t){return i(t)?o(t):e(t,{})},getterFor:function(t){return function(n){var r;if(!f(n)||(r=o(n)).type!==t)throw TypeError("Incompatible receiver, "+t+" required");return r}}}},function(t,n,r){var e=r(0),o=r(22),i=e.WeakMap;t.exports="function"==typeof i&&/native code/.test(o(i))},function(t,n,r){var e=r(24),o=r(25),i=e("keys");t.exports=function(t){return i[t]||(i[t]=o(t))}},function(t,n){t.exports=!1},function(t,n,r){var e=r(3),o=r(45),i=r(18),u=r(15);t.exports=function(t,n){for(var r=o(n),c=u.f,f=i.f,a=0;a<r.length;a++){var s=r[a];e(t,s)||c(t,s,f(n,s))}}},function(t,n,r){var e=r(27),o=r(47),i=r(50),u=r(8);t.exports=e("Reflect","ownKeys")||function(t){var n=o.f(u(t)),r=i.f;return r?n.concat(r(t)):n}},function(t,n,r){var e=r(0);t.exports=e},function(t,n,r){var e=r(48),o=r(49).concat("length","prototype");n.f=Object.getOwnPropertyNames||function(t){return e(t,o)}},function(t,n,r){var e=r(3),o=r(7),i=r(28).indexOf,u=r(26);t.exports=function(t,n){var r,c=o(t),f=0,a=[];for(r in c)!e(u,r)&&e(c,r)&&a.push(r);for(;n.length>f;)e(c,r=n[f++])&&(~i(a,r)||a.push(r));return a}},function(t,n){t.exports=["constructor","hasOwnProperty","isPrototypeOf","propertyIsEnumerable","toLocaleString","toString","valueOf"]},function(t,n){n.f=Object.getOwnPropertySymbols},function(t,n,r){var e=r(1),o=/#|\.prototype\./,i=function(t,n){var r=c[u(t)];return r==a||r!=f&&("function"==typeof n?e(n):!!n)},u=i.normalize=function(t){return String(t).replace(o,".").toLowerCase()},c=i.data={},f=i.NATIVE="N",a=i.POLYFILL="P";t.exports=i},function(t,n,r){"use strict";var e=r(11),o=r(2),i=r(31),u=r(29),c=r(9),f=r(7),a=r(53),s=r(54),l=r(10)("species"),p=[].slice,v=Math.max;e({target:"Array",proto:!0,forced:!s("slice")},{slice:function(t,n){var r,e,s,h=f(this),x=c(h.length),y=u(t,x),g=u(void 0===n?x:n,x);if(i(h)&&("function"!=typeof(r=h.constructor)||r!==Array&&!i(r.prototype)?o(r)&&null===(r=r[l])&&(r=void 0):r=void 0,r===Array||void 0===r))return p.call(h,y,g);for(e=new(void 0===r?Array:r)(v(g-y,0)),s=0;y<g;y++,s++)y in h&&a(e,s,h[y]);return e.length=s,e}})},function(t,n,r){"use strict";var e=r(14),o=r(15),i=r(12);t.exports=function(t,n,r){var u=e(n);u in t?o.f(t,u,i(0,r)):t[u]=r}},function(t,n,r){var e=r(1),o=r(10),i=r(56),u=o("species");t.exports=function(t){return i>=51||!e((function(){var n=[];return(n.constructor={})[u]=function(){return{foo:1}},1!==n[t](Boolean).foo}))}},function(t,n,r){var e=r(32);t.exports=e&&!Symbol.sham&&"symbol"==typeof Symbol()},function(t,n,r){var e,o,i=r(0),u=r(57),c=i.process,f=c&&c.versions,a=f&&f.v8;a?o=(e=a.split("."))[0]+e[1]:u&&(!(e=u.match(/Edge\/(\d+)/))||e[1]>=74)&&(e=u.match(/Chrome\/(\d+)/))&&(o=e[1]),t.exports=o&&+o},function(t,n,r){var e=r(27);t.exports=e("navigator","userAgent")||""},function(t,n,r){"use strict";var e=r(11),o=r(59).trim;e({target:"String",proto:!0,forced:r(60)("trim")},{trim:function(){return o(this)}})},function(t,n,r){var e=r(5),o="["+r(33)+"]",i=RegExp("^"+o+o+"*"),u=RegExp(o+o+"*$"),c=function(t){return function(n){var r=String(e(n));return 1&t&&(r=r.replace(i,"")),2&t&&(r=r.replace(u,"")),r}};t.exports={start:c(1),end:c(2),trim:c(3)}},function(t,n,r){var e=r(1),o=r(33);t.exports=function(t){return e((function(){return!!o[t]()||"​᠎"!="​᠎"[t]()||o[t].name!==t}))}},function(t,n,r){var e=r(0),o=r(62),i=r(63),u=r(4);for(var c in o){var f=e[c],a=f&&f.prototype;if(a&&a.forEach!==i)try{u(a,"forEach",i)}catch(t){a.forEach=i}}},function(t,n){t.exports={CSSRuleList:0,CSSStyleDeclaration:0,CSSValueList:0,ClientRectList:0,DOMRectList:0,DOMStringList:0,DOMTokenList:1,DataTransferItemList:0,FileList:0,HTMLAllCollection:0,HTMLCollection:0,HTMLFormElement:0,HTMLSelectElement:0,MediaList:0,MimeTypeArray:0,NamedNodeMap:0,NodeList:1,PaintRequestList:0,Plugin:0,PluginArray:0,SVGLengthList:0,SVGNumberList:0,SVGPathSegList:0,SVGPointList:0,SVGStringList:0,SVGTransformList:0,SourceBufferList:0,StyleSheetList:0,TextTrackCueList:0,TextTrackList:0,TouchList:0}},function(t,n,r){"use strict";var e=r(64).forEach,o=r(30);t.exports=o("forEach")?function(t){return e(this,t,arguments.length>1?arguments[1]:void 0)}:[].forEach},function(t,n,r){var e=r(65),o=r(19),i=r(67),u=r(9),c=r(68),f=[].push,a=function(t){var n=1==t,r=2==t,a=3==t,s=4==t,l=6==t,p=5==t||l;return function(v,h,x,y){for(var g,d,m=i(v),S=o(m),b=e(h,x,3),w=u(S.length),E=0,O=y||c,L=n?O(v,w):r?O(v,0):void 0;w>E;E++)if((p||E in S)&&(d=b(g=S[E],E,m),t))if(n)L[E]=d;else if(d)switch(t){case 3:return!0;case 5:return g;case 6:return E;case 2:f.call(L,g)}else if(s)return!1;return l?-1:a||s?s:L}};t.exports={forEach:a(0),map:a(1),filter:a(2),some:a(3),every:a(4),find:a(5),findIndex:a(6)}},function(t,n,r){var e=r(66);t.exports=function(t,n,r){if(e(t),void 0===n)return t;switch(r){case 0:return function(){return t.call(n)};case 1:return function(r){return t.call(n,r)};case 2:return function(r,e){return t.call(n,r,e)};case 3:return function(r,e,o){return t.call(n,r,e,o)}}return function(){return t.apply(n,arguments)}}},function(t,n){t.exports=function(t){if("function"!=typeof t)throw TypeError(String(t)+" is not a function");return t}},function(t,n,r){var e=r(5);t.exports=function(t){return Object(e(t))}},function(t,n,r){var e=r(2),o=r(31),i=r(10)("species");t.exports=function(t,n){var r;return o(t)&&("function"!=typeof(r=t.constructor)||r!==Array&&!o(r.prototype)?e(r)&&null===(r=r[i])&&(r=void 0):r=void 0),new(void 0===r?Array:r)(0===n?0:n)}},function(t,n,r){"use strict";var e=r(70),o=r(8),i=r(9),u=r(5),c=r(72),f=r(74);e("match",1,(function(t,n,r){return[function(n){var r=u(this),e=null==n?void 0:n[t];return void 0!==e?e.call(n,r):new RegExp(n)[t](String(r))},function(t){var e=r(n,t,this);if(e.done)return e.value;var u=o(t),a=String(this);if(!u.global)return f(u,a);var s=u.unicode;u.lastIndex=0;for(var l,p=[],v=0;null!==(l=f(u,a));){var h=String(l[0]);p[v]=h,""===h&&(u.lastIndex=c(a,i(u.lastIndex),s)),v++}return 0===v?null:p}]}))},function(t,n,r){"use strict";var e=r(4),o=r(21),i=r(1),u=r(10),c=r(34),f=u("species"),a=!i((function(){var t=/./;return t.exec=function(){var t=[];return t.groups={a:"7"},t},"7"!=="".replace(t,"$<a>")})),s=!i((function(){var t=/(?:)/,n=t.exec;t.exec=function(){return n.apply(this,arguments)};var r="ab".split(t);return 2!==r.length||"a"!==r[0]||"b"!==r[1]}));t.exports=function(t,n,r,l){var p=u(t),v=!i((function(){var n={};return n[p]=function(){return 7},7!=""[t](n)})),h=v&&!i((function(){var n=!1,r=/a/;return"split"===t&&((r={}).constructor={},r.constructor[f]=function(){return r},r.flags="",r[p]=/./[p]),r.exec=function(){return n=!0,null},r[p](""),!n}));if(!v||!h||"replace"===t&&!a||"split"===t&&!s){var x=/./[p],y=r(p,""[t],(function(t,n,r,e,o){return n.exec===c?v&&!o?{done:!0,value:x.call(n,r,e)}:{done:!0,value:t.call(r,n,e)}:{done:!1}})),g=y[0],d=y[1];o(String.prototype,t,g),o(RegExp.prototype,p,2==n?function(t,n){return d.call(t,this,n)}:function(t){return d.call(t,this)}),l&&e(RegExp.prototype[p],"sham",!0)}}},function(t,n,r){"use strict";var e=r(8);t.exports=function(){var t=e(this),n="";return t.global&&(n+="g"),t.ignoreCase&&(n+="i"),t.multiline&&(n+="m"),t.dotAll&&(n+="s"),t.unicode&&(n+="u"),t.sticky&&(n+="y"),n}},function(t,n,r){"use strict";var e=r(73).charAt;t.exports=function(t,n,r){return n+(r?e(t,n).length:1)}},function(t,n,r){var e=r(17),o=r(5),i=function(t){return function(n,r){var i,u,c=String(o(n)),f=e(r),a=c.length;return f<0||f>=a?t?"":void 0:(i=c.charCodeAt(f))<55296||i>56319||f+1===a||(u=c.charCodeAt(f+1))<56320||u>57343?t?c.charAt(f):i:t?c.slice(f,f+2):u-56320+(i-55296<<10)+65536}};t.exports={codeAt:i(!1),charAt:i(!0)}},function(t,n,r){var e=r(13),o=r(34);t.exports=function(t,n){var r=t.exec;if("function"==typeof r){var i=r.call(t,n);if("object"!=typeof i)throw TypeError("RegExp exec method returned something other than an Object or null");return i}if("RegExp"!==e(t))throw TypeError("RegExp#exec called on incompatible receiver");return o.call(t,n)}}]]);;
/**
* DO NOT EDIT THIS FILE.
* See the following change record for more information,
* https://www.drupal.org/node/2815083
* @preserve
**/
(function ($, Drupal) {
  Drupal.theme.progressBar = function (id) {
    return "<div id=\"".concat(id, "\" class=\"progress\" aria-live=\"polite\">") + '<div class="progress__label">&nbsp;</div>' + '<div class="progress__track"><div class="progress__bar"></div></div>' + '<div class="progress__percentage"></div>' + '<div class="progress__description">&nbsp;</div>' + '</div>';
  };
  Drupal.ProgressBar = function (id, updateCallback, method, errorCallback) {
    this.id = id;
    this.method = method || 'GET';
    this.updateCallback = updateCallback;
    this.errorCallback = errorCallback;
    this.element = $(Drupal.theme('progressBar', id));
  };
  $.extend(Drupal.ProgressBar.prototype, {
    setProgress: function setProgress(percentage, message, label) {
      if (percentage >= 0 && percentage <= 100) {
        $(this.element).find('div.progress__bar').css('width', "".concat(percentage, "%"));
        $(this.element).find('div.progress__percentage').html("".concat(percentage, "%"));
      }
      $('div.progress__description', this.element).html(message);
      $('div.progress__label', this.element).html(label);
      if (this.updateCallback) {
        this.updateCallback(percentage, message, this);
      }
    },
    startMonitoring: function startMonitoring(uri, delay) {
      this.delay = delay;
      this.uri = uri;
      this.sendPing();
    },
    stopMonitoring: function stopMonitoring() {
      clearTimeout(this.timer);
      this.uri = null;
    },
    sendPing: function sendPing() {
      if (this.timer) {
        clearTimeout(this.timer);
      }
      if (this.uri) {
        var pb = this;
        var uri = this.uri;
        if (uri.indexOf('?') === -1) {
          uri += '?';
        } else {
          uri += '&';
        }
        uri += '_format=json';
        $.ajax({
          type: this.method,
          url: uri,
          data: '',
          dataType: 'json',
          success: function success(progress) {
            if (progress.status === 0) {
              pb.displayError(progress.data);
              return;
            }
            pb.setProgress(progress.percentage, progress.message, progress.label);
            pb.timer = setTimeout(function () {
              pb.sendPing();
            }, pb.delay);
          },
          error: function error(xmlhttp) {
            var e = new Drupal.AjaxError(xmlhttp, pb.uri);
            pb.displayError("<pre>".concat(e.message, "</pre>"));
          }
        });
      }
    },
    displayError: function displayError(string) {
      var error = $('<div class="messages messages--error"></div>').html(string);
      $(this.element).before(error).hide();
      if (this.errorCallback) {
        this.errorCallback(this);
      }
    }
  });
})(jQuery, Drupal);;
/**
* DO NOT EDIT THIS FILE.
* See the following change record for more information,
* https://www.drupal.org/node/2815083
* @preserve
**/
(function ($, once) {
  var deprecatedMessageSuffix = "is deprecated in Drupal 9.3.0 and will be removed in Drupal 10.0.0. Use the core/once library instead. See https://www.drupal.org/node/3158256";
  var originalJQOnce = $.fn.once;
  var originalJQRemoveOnce = $.fn.removeOnce;
  $.fn.once = function jQueryOnce(id) {
    Drupal.deprecationError({
      message: "jQuery.once() ".concat(deprecatedMessageSuffix)
    });
    return originalJQOnce.apply(this, [id]);
  };
  $.fn.removeOnce = function jQueryRemoveOnce(id) {
    Drupal.deprecationError({
      message: "jQuery.removeOnce() ".concat(deprecatedMessageSuffix)
    });
    return originalJQRemoveOnce.apply(this, [id]);
  };
  var drupalOnce = once;
  function augmentedOnce(id, selector, context) {
    originalJQOnce.apply($(selector, context), [id]);
    return drupalOnce(id, selector, context);
  }
  function remove(id, selector, context) {
    originalJQRemoveOnce.apply($(selector, context), [id]);
    return drupalOnce.remove(id, selector, context);
  }
  window.once = Object.assign(augmentedOnce, drupalOnce, {
    remove: remove
  });
})(jQuery, once);;
loadjs=function(){var h=function(){},c={},u={},f={};function o(e,n){if(e){var r=f[e];if(u[e]=n,r)for(;r.length;)r[0](e,n),r.splice(0,1)}}function l(e,n){e.call&&(e={success:e}),n.length?(e.error||h)(n):(e.success||h)(e)}function d(r,t,s,i){var c,o,e=document,n=s.async,u=(s.numRetries||0)+1,f=s.before||h,l=r.replace(/[\?|#].*$/,""),a=r.replace(/^(css|img)!/,"");i=i||0,/(^css!|\.css$)/.test(l)?((o=e.createElement("link")).rel="stylesheet",o.href=a,(c="hideFocus"in o)&&o.relList&&(c=0,o.rel="preload",o.as="style")):/(^img!|\.(png|gif|jpg|svg|webp)$)/.test(l)?(o=e.createElement("img")).src=a:((o=e.createElement("script")).src=r,o.async=void 0===n||n),!(o.onload=o.onerror=o.onbeforeload=function(e){var n=e.type[0];if(c)try{o.sheet.cssText.length||(n="e")}catch(e){18!=e.code&&(n="e")}if("e"==n){if((i+=1)<u)return d(r,t,s,i)}else if("preload"==o.rel&&"style"==o.as)return o.rel="stylesheet";t(r,n,e.defaultPrevented)})!==f(r,o)&&e.head.appendChild(o)}function r(e,n,r){var t,s;if(n&&n.trim&&(t=n),s=(t?r:n)||{},t){if(t in c)throw"LoadJS";c[t]=!0}function i(n,r){!function(e,t,n){var r,s,i=(e=e.push?e:[e]).length,c=i,o=[];for(r=function(e,n,r){if("e"==n&&o.push(e),"b"==n){if(!r)return;o.push(e)}--i||t(o)},s=0;s<c;s++)d(e[s],r,n)}(e,function(e){l(s,e),n&&l({success:n,error:r},e),o(t,e)},s)}if(s.returnPromise)return new Promise(i);i()}return r.ready=function(e,n){return function(e,r){e=e.push?e:[e];var n,t,s,i=[],c=e.length,o=c;for(n=function(e,n){n.length&&i.push(e),--o||r(i)};c--;)t=e[c],(s=u[t])?n(t,s):(f[t]=f[t]||[]).push(n)}(e,function(e){l(n,e)}),r},r.done=function(e){o(e,[])},r.reset=function(){c={},u={},f={}},r.isDefined=function(e){return e in c},r}();;
/**
* DO NOT EDIT THIS FILE.
* See the following change record for more information,
* https://www.drupal.org/node/2815083
* @preserve
**/
function _toConsumableArray(arr) { return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _unsupportedIterableToArray(arr) || _nonIterableSpread(); }
function _nonIterableSpread() { throw new TypeError("Invalid attempt to spread non-iterable instance.\nIn order to be iterable, non-array objects must have a [Symbol.iterator]() method."); }
function _unsupportedIterableToArray(o, minLen) { if (!o) return; if (typeof o === "string") return _arrayLikeToArray(o, minLen); var n = Object.prototype.toString.call(o).slice(8, -1); if (n === "Object" && o.constructor) n = o.constructor.name; if (n === "Map" || n === "Set") return Array.from(o); if (n === "Arguments" || /^(?:Ui|I)nt(?:8|16|32)(?:Clamped)?Array$/.test(n)) return _arrayLikeToArray(o, minLen); }
function _iterableToArray(iter) { if (typeof Symbol !== "undefined" && iter[Symbol.iterator] != null || iter["@@iterator"] != null) return Array.from(iter); }
function _arrayWithoutHoles(arr) { if (Array.isArray(arr)) return _arrayLikeToArray(arr); }
function _arrayLikeToArray(arr, len) { if (len == null || len > arr.length) len = arr.length; for (var i = 0, arr2 = new Array(len); i < len; i++) { arr2[i] = arr[i]; } return arr2; }
(function ($, window, Drupal, drupalSettings, loadjs, _ref) {
  var isFocusable = _ref.isFocusable,
    tabbable = _ref.tabbable;
  Drupal.behaviors.AJAX = {
    attach: function attach(context, settings) {
      function loadAjaxBehavior(base) {
        var elementSettings = settings.ajax[base];
        if (typeof elementSettings.selector === 'undefined') {
          elementSettings.selector = "#".concat(base);
        }
        once('drupal-ajax', $(elementSettings.selector)).forEach(function (el) {
          elementSettings.element = el;
          elementSettings.base = base;
          Drupal.ajax(elementSettings);
        });
      }
      Object.keys(settings.ajax || {}).forEach(function (base) {
        return loadAjaxBehavior(base);
      });
      Drupal.ajax.bindAjaxLinks(document.body);
      once('ajax', '.use-ajax-submit').forEach(function (el) {
        var elementSettings = {};
        elementSettings.url = $(el.form).attr('action');
        elementSettings.setClick = true;
        elementSettings.event = 'click';
        elementSettings.progress = {
          type: 'throbber'
        };
        elementSettings.base = el.id;
        elementSettings.element = el;
        Drupal.ajax(elementSettings);
      });
    },
    detach: function detach(context, settings, trigger) {
      if (trigger === 'unload') {
        Drupal.ajax.expired().forEach(function (instance) {
          Drupal.ajax.instances[instance.instanceIndex] = null;
        });
      }
    }
  };
  Drupal.AjaxError = function (xmlhttp, uri, customMessage) {
    var statusCode;
    var statusText;
    var responseText;
    if (xmlhttp.status) {
      statusCode = "\n".concat(Drupal.t('An AJAX HTTP error occurred.'), "\n").concat(Drupal.t('HTTP Result Code: !status', {
        '!status': xmlhttp.status
      }));
    } else {
      statusCode = "\n".concat(Drupal.t('An AJAX HTTP request terminated abnormally.'));
    }
    statusCode += "\n".concat(Drupal.t('Debugging information follows.'));
    var pathText = "\n".concat(Drupal.t('Path: !uri', {
      '!uri': uri
    }));
    statusText = '';
    try {
      statusText = "\n".concat(Drupal.t('StatusText: !statusText', {
        '!statusText': xmlhttp.statusText.trim()
      }));
    } catch (e) {}
    responseText = '';
    try {
      responseText = "\n".concat(Drupal.t('ResponseText: !responseText', {
        '!responseText': xmlhttp.responseText.trim()
      }));
    } catch (e) {}
    responseText = responseText.replace(/<("[^"]*"|'[^']*'|[^'">])*>/gi, '');
    responseText = responseText.replace(/[\n]+\s+/g, '\n');
    var readyStateText = xmlhttp.status === 0 ? "\n".concat(Drupal.t('ReadyState: !readyState', {
      '!readyState': xmlhttp.readyState
    })) : '';
    customMessage = customMessage ? "\n".concat(Drupal.t('CustomMessage: !customMessage', {
      '!customMessage': customMessage
    })) : '';
    this.message = statusCode + pathText + statusText + customMessage + responseText + readyStateText;
    this.name = 'AjaxError';
  };
  Drupal.AjaxError.prototype = new Error();
  Drupal.AjaxError.prototype.constructor = Drupal.AjaxError;
  Drupal.ajax = function (settings) {
    if (arguments.length !== 1) {
      throw new Error('Drupal.ajax() function must be called with one configuration object only');
    }
    var base = settings.base || false;
    var element = settings.element || false;
    delete settings.base;
    delete settings.element;
    if (!settings.progress && !element) {
      settings.progress = false;
    }
    var ajax = new Drupal.Ajax(base, element, settings);
    ajax.instanceIndex = Drupal.ajax.instances.length;
    Drupal.ajax.instances.push(ajax);
    return ajax;
  };
  Drupal.ajax.instances = [];
  Drupal.ajax.expired = function () {
    return Drupal.ajax.instances.filter(function (instance) {
      return instance && instance.element !== false && !document.body.contains(instance.element);
    });
  };
  Drupal.ajax.bindAjaxLinks = function (element) {
    once('ajax', '.use-ajax', element).forEach(function (ajaxLink) {
      var $linkElement = $(ajaxLink);
      var elementSettings = {
        progress: {
          type: 'throbber'
        },
        dialogType: $linkElement.data('dialog-type'),
        dialog: $linkElement.data('dialog-options'),
        dialogRenderer: $linkElement.data('dialog-renderer'),
        base: $linkElement.attr('id'),
        element: ajaxLink
      };
      var href = $linkElement.attr('href');
      if (href) {
        elementSettings.url = href;
        elementSettings.event = 'click';
      }
      Drupal.ajax(elementSettings);
    });
  };
  Drupal.Ajax = function (base, element, elementSettings) {
    var defaults = {
      event: element ? 'mousedown' : null,
      keypress: true,
      selector: base ? "#".concat(base) : null,
      effect: 'none',
      speed: 'none',
      method: 'replaceWith',
      progress: {
        type: 'throbber',
        message: Drupal.t('Please wait...')
      },
      submit: {
        js: true
      }
    };
    $.extend(this, defaults, elementSettings);
    this.commands = new Drupal.AjaxCommands();
    this.instanceIndex = false;
    if (this.wrapper) {
      this.wrapper = "#".concat(this.wrapper);
    }
    this.element = element;
    this.element_settings = elementSettings;
    this.elementSettings = elementSettings;
    if (this.element && this.element.form) {
      this.$form = $(this.element.form);
    }
    if (!this.url) {
      var $element = $(this.element);
      if ($element.is('a')) {
        this.url = $element.attr('href');
      } else if (this.element && element.form) {
        this.url = this.$form.attr('action');
      }
    }
    var originalUrl = this.url;
    this.url = this.url.replace(/\/nojs(\/|$|\?|#)/, '/ajax$1');
    if (drupalSettings.ajaxTrustedUrl[originalUrl]) {
      drupalSettings.ajaxTrustedUrl[this.url] = true;
    }
    var ajax = this;
    ajax.options = {
      url: ajax.url,
      data: ajax.submit,
      isInProgress: function isInProgress() {
        return ajax.ajaxing;
      },
      beforeSerialize: function beforeSerialize(elementSettings, options) {
        return ajax.beforeSerialize(elementSettings, options);
      },
      beforeSubmit: function beforeSubmit(formValues, elementSettings, options) {
        ajax.ajaxing = true;
        return ajax.beforeSubmit(formValues, elementSettings, options);
      },
      beforeSend: function beforeSend(xmlhttprequest, options) {
        ajax.ajaxing = true;
        return ajax.beforeSend(xmlhttprequest, options);
      },
      success: function success(response, status, xmlhttprequest) {
        var _this = this;
        if (typeof response === 'string') {
          response = $.parseJSON(response);
        }
        if (response !== null && !drupalSettings.ajaxTrustedUrl[ajax.url]) {
          if (xmlhttprequest.getResponseHeader('X-Drupal-Ajax-Token') !== '1') {
            var customMessage = Drupal.t('The response failed verification so will not be processed.');
            return ajax.error(xmlhttprequest, ajax.url, customMessage);
          }
        }
        return Promise.resolve(ajax.success(response, status)).then(function () {
          ajax.ajaxing = false;
          $(document).trigger('ajaxSuccess', [xmlhttprequest, _this]);
          $(document).trigger('ajaxComplete', [xmlhttprequest, _this]);
          if (--$.active === 0) {
            $(document).trigger('ajaxStop');
          }
        });
      },
      error: function error(xmlhttprequest, status, _error) {
        ajax.ajaxing = false;
      },
      complete: function complete(xmlhttprequest, status) {
        if (status === 'error' || status === 'parsererror') {
          return ajax.error(xmlhttprequest, ajax.url);
        }
      },
      dataType: 'json',
      jsonp: false,
      type: 'POST'
    };
    if (elementSettings.dialog) {
      ajax.options.data.dialogOptions = elementSettings.dialog;
    }
    if (ajax.options.url.indexOf('?') === -1) {
      ajax.options.url += '?';
    } else {
      ajax.options.url += '&';
    }
    var wrapper = "drupal_".concat(elementSettings.dialogType || 'ajax');
    if (elementSettings.dialogRenderer) {
      wrapper += ".".concat(elementSettings.dialogRenderer);
    }
    ajax.options.url += "".concat(Drupal.ajax.WRAPPER_FORMAT, "=").concat(wrapper);
    $(ajax.element).on(elementSettings.event, function (event) {
      if (!drupalSettings.ajaxTrustedUrl[ajax.url] && !Drupal.url.isLocal(ajax.url)) {
        throw new Error(Drupal.t('The callback URL is not local and not trusted: !url', {
          '!url': ajax.url
        }));
      }
      return ajax.eventResponse(this, event);
    });
    if (elementSettings.keypress) {
      $(ajax.element).on('keypress', function (event) {
        return ajax.keypressResponse(this, event);
      });
    }
    if (elementSettings.prevent) {
      $(ajax.element).on(elementSettings.prevent, false);
    }
  };
  Drupal.ajax.WRAPPER_FORMAT = '_wrapper_format';
  Drupal.Ajax.AJAX_REQUEST_PARAMETER = '_drupal_ajax';
  Drupal.Ajax.prototype.execute = function () {
    if (this.ajaxing) {
      return;
    }
    try {
      this.beforeSerialize(this.element, this.options);
      return $.ajax(this.options);
    } catch (e) {
      this.ajaxing = false;
      window.alert("An error occurred while attempting to process ".concat(this.options.url, ": ").concat(e.message));
      return $.Deferred().reject();
    }
  };
  Drupal.Ajax.prototype.keypressResponse = function (element, event) {
    var ajax = this;
    if (event.which === 13 || event.which === 32 && element.type !== 'text' && element.type !== 'textarea' && element.type !== 'tel' && element.type !== 'number') {
      event.preventDefault();
      event.stopPropagation();
      $(element).trigger(ajax.elementSettings.event);
    }
  };
  Drupal.Ajax.prototype.eventResponse = function (element, event) {
    event.preventDefault();
    event.stopPropagation();
    var ajax = this;
    if (ajax.ajaxing) {
      return;
    }
    try {
      if (ajax.$form) {
        if (ajax.setClick) {
          element.form.clk = element;
        }
        ajax.$form.ajaxSubmit(ajax.options);
      } else {
        ajax.beforeSerialize(ajax.element, ajax.options);
        $.ajax(ajax.options);
      }
    } catch (e) {
      ajax.ajaxing = false;
      window.alert("An error occurred while attempting to process ".concat(ajax.options.url, ": ").concat(e.message));
    }
  };
  Drupal.Ajax.prototype.beforeSerialize = function (element, options) {
    if (this.$form && document.body.contains(this.$form.get(0))) {
      var settings = this.settings || drupalSettings;
      Drupal.detachBehaviors(this.$form.get(0), settings, 'serialize');
    }
    options.data[Drupal.Ajax.AJAX_REQUEST_PARAMETER] = 1;
    var pageState = drupalSettings.ajaxPageState;
    options.data['ajax_page_state[theme]'] = pageState.theme;
    options.data['ajax_page_state[theme_token]'] = pageState.theme_token;
    options.data['ajax_page_state[libraries]'] = pageState.libraries;
  };
  Drupal.Ajax.prototype.beforeSubmit = function (formValues, element, options) {};
  Drupal.Ajax.prototype.beforeSend = function (xmlhttprequest, options) {
    if (this.$form) {
      options.extraData = options.extraData || {};
      options.extraData.ajax_iframe_upload = '1';
      var v = $.fieldValue(this.element);
      if (v !== null) {
        options.extraData[this.element.name] = v;
      }
    }
    $(this.element).prop('disabled', true);
    if (!this.progress || !this.progress.type) {
      return;
    }
    var progressIndicatorMethod = "setProgressIndicator".concat(this.progress.type.slice(0, 1).toUpperCase()).concat(this.progress.type.slice(1).toLowerCase());
    if (progressIndicatorMethod in this && typeof this[progressIndicatorMethod] === 'function') {
      this[progressIndicatorMethod].call(this);
    }
  };
  Drupal.theme.ajaxProgressThrobber = function (message) {
    var messageMarkup = typeof message === 'string' ? Drupal.theme('ajaxProgressMessage', message) : '';
    var throbber = '<div class="throbber">&nbsp;</div>';
    return "<div class=\"ajax-progress ajax-progress-throbber\">".concat(throbber).concat(messageMarkup, "</div>");
  };
  Drupal.theme.ajaxProgressIndicatorFullscreen = function () {
    return '<div class="ajax-progress ajax-progress-fullscreen">&nbsp;</div>';
  };
  Drupal.theme.ajaxProgressMessage = function (message) {
    return "<div class=\"message\">".concat(message, "</div>");
  };
  Drupal.theme.ajaxProgressBar = function ($element) {
    return $('<div class="ajax-progress ajax-progress-bar"></div>').append($element);
  };
  Drupal.Ajax.prototype.setProgressIndicatorBar = function () {
    var progressBar = new Drupal.ProgressBar("ajax-progress-".concat(this.element.id), $.noop, this.progress.method, $.noop);
    if (this.progress.message) {
      progressBar.setProgress(-1, this.progress.message);
    }
    if (this.progress.url) {
      progressBar.startMonitoring(this.progress.url, this.progress.interval || 1500);
    }
    this.progress.element = $(Drupal.theme('ajaxProgressBar', progressBar.element));
    this.progress.object = progressBar;
    $(this.element).after(this.progress.element);
  };
  Drupal.Ajax.prototype.setProgressIndicatorThrobber = function () {
    this.progress.element = $(Drupal.theme('ajaxProgressThrobber', this.progress.message));
    $(this.element).after(this.progress.element);
  };
  Drupal.Ajax.prototype.setProgressIndicatorFullscreen = function () {
    this.progress.element = $(Drupal.theme('ajaxProgressIndicatorFullscreen'));
    $('body').append(this.progress.element);
  };
  Drupal.Ajax.prototype.commandExecutionQueue = function (response, status) {
    var _this2 = this;
    var ajaxCommands = this.commands;
    return Object.keys(response || {}).reduce(function (executionQueue, key) {
      return executionQueue.then(function () {
        var command = response[key].command;
        if (command && ajaxCommands[command]) {
          return ajaxCommands[command](_this2, response[key], status);
        }
      });
    }, Promise.resolve());
  };
  Drupal.Ajax.prototype.success = function (response, status) {
    var _this3 = this;
    if (this.progress.element) {
      $(this.progress.element).remove();
    }
    if (this.progress.object) {
      this.progress.object.stopMonitoring();
    }
    $(this.element).prop('disabled', false);
    var elementParents = $(this.element).parents('[data-drupal-selector]').addBack().toArray();
    var focusChanged = Object.keys(response || {}).some(function (key) {
      var _response$key = response[key],
        command = _response$key.command,
        method = _response$key.method;
      return command === 'focusFirst' || command === 'invoke' && method === 'focus';
    });
    return this.commandExecutionQueue(response, status).then(function () {
      if (!focusChanged && _this3.element && !$(_this3.element).data('disable-refocus')) {
        var target = false;
        for (var n = elementParents.length - 1; !target && n >= 0; n--) {
          target = document.querySelector("[data-drupal-selector=\"".concat(elementParents[n].getAttribute('data-drupal-selector'), "\"]"));
        }
        if (target) {
          $(target).trigger('focus');
        }
      }
      if (_this3.$form && document.body.contains(_this3.$form.get(0))) {
        var settings = _this3.settings || drupalSettings;
        Drupal.attachBehaviors(_this3.$form.get(0), settings);
      }
      _this3.settings = null;
    }).catch(function (error) {
      return console.error(Drupal.t('An error occurred during the execution of the Ajax response: !error', {
        '!error': error
      }));
    });
  };
  Drupal.Ajax.prototype.getEffect = function (response) {
    var type = response.effect || this.effect;
    var speed = response.speed || this.speed;
    var effect = {};
    if (type === 'none') {
      effect.showEffect = 'show';
      effect.hideEffect = 'hide';
      effect.showSpeed = '';
    } else if (type === 'fade') {
      effect.showEffect = 'fadeIn';
      effect.hideEffect = 'fadeOut';
      effect.showSpeed = speed;
    } else {
      effect.showEffect = "".concat(type, "Toggle");
      effect.hideEffect = "".concat(type, "Toggle");
      effect.showSpeed = speed;
    }
    return effect;
  };
  Drupal.Ajax.prototype.error = function (xmlhttprequest, uri, customMessage) {
    if (this.progress.element) {
      $(this.progress.element).remove();
    }
    if (this.progress.object) {
      this.progress.object.stopMonitoring();
    }
    $(this.wrapper).show();
    $(this.element).prop('disabled', false);
    if (this.$form && document.body.contains(this.$form.get(0))) {
      var settings = this.settings || drupalSettings;
      Drupal.attachBehaviors(this.$form.get(0), settings);
    }
    throw new Drupal.AjaxError(xmlhttprequest, uri, customMessage);
  };
  Drupal.theme.ajaxWrapperNewContent = function ($newContent, ajax, response) {
    return (response.effect || ajax.effect) !== 'none' && $newContent.filter(function (i) {
      return !($newContent[i].nodeName === '#comment' || $newContent[i].nodeName === '#text' && /^(\s|\n|\r)*$/.test($newContent[i].textContent));
    }).length > 1 ? Drupal.theme('ajaxWrapperMultipleRootElements', $newContent) : $newContent;
  };
  Drupal.theme.ajaxWrapperMultipleRootElements = function ($elements) {
    return $('<div></div>').append($elements);
  };
  Drupal.AjaxCommands = function () {};
  Drupal.AjaxCommands.prototype = {
    insert: function insert(ajax, response) {
      var $wrapper = response.selector ? $(response.selector) : $(ajax.wrapper);
      var method = response.method || ajax.method;
      var effect = ajax.getEffect(response);
      var settings = response.settings || ajax.settings || drupalSettings;
      var $newContent = $($.parseHTML(response.data, document, true));
      $newContent = Drupal.theme('ajaxWrapperNewContent', $newContent, ajax, response);
      switch (method) {
        case 'html':
        case 'replaceWith':
        case 'replaceAll':
        case 'empty':
        case 'remove':
          Drupal.detachBehaviors($wrapper.get(0), settings);
          break;
        default:
          break;
      }
      $wrapper[method]($newContent);
      if (effect.showEffect !== 'show') {
        $newContent.hide();
      }
      var $ajaxNewContent = $newContent.find('.ajax-new-content');
      if ($ajaxNewContent.length) {
        $ajaxNewContent.hide();
        $newContent.show();
        $ajaxNewContent[effect.showEffect](effect.showSpeed);
      } else if (effect.showEffect !== 'show') {
        $newContent[effect.showEffect](effect.showSpeed);
      }
      if ($newContent.parents('html').length) {
        $newContent.each(function (index, element) {
          if (element.nodeType === Node.ELEMENT_NODE) {
            Drupal.attachBehaviors(element, settings);
          }
        });
      }
    },
    remove: function remove(ajax, response, status) {
      var settings = response.settings || ajax.settings || drupalSettings;
      $(response.selector).each(function () {
        Drupal.detachBehaviors(this, settings);
      }).remove();
    },
    changed: function changed(ajax, response, status) {
      var $element = $(response.selector);
      if (!$element.hasClass('ajax-changed')) {
        $element.addClass('ajax-changed');
        if (response.asterisk) {
          $element.find(response.asterisk).append(" <abbr class=\"ajax-changed\" title=\"".concat(Drupal.t('Changed'), "\">*</abbr> "));
        }
      }
    },
    alert: function alert(ajax, response, status) {
      window.alert(response.text);
    },
    announce: function announce(ajax, response) {
      if (response.priority) {
        Drupal.announce(response.text, response.priority);
      } else {
        Drupal.announce(response.text);
      }
    },
    redirect: function redirect(ajax, response, status) {
      window.location = response.url;
    },
    css: function css(ajax, response, status) {
      $(response.selector).css(response.argument);
    },
    settings: function settings(ajax, response, status) {
      var ajaxSettings = drupalSettings.ajax;
      if (ajaxSettings) {
        Drupal.ajax.expired().forEach(function (instance) {
          if (instance.selector) {
            var selector = instance.selector.replace('#', '');
            if (selector in ajaxSettings) {
              delete ajaxSettings[selector];
            }
          }
        });
      }
      if (response.merge) {
        $.extend(true, drupalSettings, response.settings);
      } else {
        ajax.settings = response.settings;
      }
    },
    data: function data(ajax, response, status) {
      $(response.selector).data(response.name, response.value);
    },
    focusFirst: function focusFirst(ajax, response, status) {
      var focusChanged = false;
      var container = document.querySelector(response.selector);
      if (container) {
        var tabbableElements = tabbable(container);
        if (tabbableElements.length) {
          tabbableElements[0].focus();
          focusChanged = true;
        } else if (isFocusable(container)) {
          container.focus();
          focusChanged = true;
        }
      }
      if (ajax.hasOwnProperty('element') && !focusChanged) {
        ajax.element.focus();
      }
    },
    invoke: function invoke(ajax, response, status) {
      var $element = $(response.selector);
      $element[response.method].apply($element, _toConsumableArray(response.args));
    },
    restripe: function restripe(ajax, response, status) {
      $(response.selector).find('> tbody > tr:visible, > tr:visible').removeClass('odd even').filter(':even').addClass('odd').end().filter(':odd').addClass('even');
    },
    update_build_id: function update_build_id(ajax, response, status) {
      document.querySelectorAll("input[name=\"form_build_id\"][value=\"".concat(response.old, "\"]")).forEach(function (item) {
        item.value = response.new;
      });
    },
    add_css: function add_css(ajax, response, status) {
      $('head').prepend(response.data);
    },
    message: function message(ajax, response) {
      var messages = new Drupal.Message(document.querySelector(response.messageWrapperQuerySelector));
      if (response.clearPrevious) {
        messages.clear();
      }
      messages.add(response.message, response.messageOptions);
    },
    add_js: function add_js(ajax, response, status) {
      var parentEl = document.querySelector(response.selector || 'body');
      var settings = ajax.settings || drupalSettings;
      var allUniqueBundleIds = response.data.map(function (script) {
        var uniqueBundleId = script.src + ajax.instanceIndex;
        loadjs(script.src, uniqueBundleId, {
          async: false,
          before: function before(path, scriptEl) {
            Object.keys(script).forEach(function (attributeKey) {
              scriptEl.setAttribute(attributeKey, script[attributeKey]);
            });
            parentEl.appendChild(scriptEl);
            return false;
          }
        });
        return uniqueBundleId;
      });
      return new Promise(function (resolve, reject) {
        loadjs.ready(allUniqueBundleIds, {
          success: function success() {
            Drupal.attachBehaviors(parentEl, settings);
            resolve();
          },
          error: function error(depsNotFound) {
            var message = Drupal.t("The following files could not be loaded: @dependencies", {
              '@dependencies': depsNotFound.join(', ')
            });
            reject(message);
          }
        });
      });
    }
  };
  var stopEvent = function stopEvent(xhr, settings) {
    return xhr.getResponseHeader('X-Drupal-Ajax-Token') === '1' && settings.isInProgress && settings.isInProgress();
  };
  $.extend(true, $.event.special, {
    ajaxSuccess: {
      trigger: function trigger(event, xhr, settings) {
        if (stopEvent(xhr, settings)) {
          return false;
        }
      }
    },
    ajaxComplete: {
      trigger: function trigger(event, xhr, settings) {
        if (stopEvent(xhr, settings)) {
          $.active++;
          return false;
        }
      }
    }
  });
})(jQuery, window, Drupal, drupalSettings, loadjs, window.tabbable);;
/**
* DO NOT EDIT THIS FILE.
* See the following change record for more information,
* https://www.drupal.org/node/2815083
* @preserve
**/

(function (Drupal) {
  Drupal.theme.ajaxProgressBar = function ($element) {
    return $element.addClass('ajax-progress ajax-progress-bar');
  };
})(Drupal);;
/**
* DO NOT EDIT THIS FILE.
* See the following change record for more information,
* https://www.drupal.org/node/2815083
* @preserve
**/
(function ($, Drupal, _ref) {
  var isTabbable = _ref.isTabbable;
  $.extend($.expr[':'], {
    tabbable: function tabbable(element) {
      Drupal.deprecationError({
        message: 'The :tabbable selector is deprecated in Drupal 9.2.0 and will be removed in Drupal 11.0.0. Use the core/tabbable library instead. See https://www.drupal.org/node/3183730'
      });
      if (element.tagName === 'SUMMARY' || element.tagName === 'DETAILS') {
        var tabIndex = element.getAttribute('tabIndex');
        if (tabIndex === null || tabIndex < 0) {
          return false;
        }
      }
      return isTabbable(element);
    }
  });
})(jQuery, Drupal, window.tabbable);;
/**
* DO NOT EDIT THIS FILE.
* See the following change record for more information,
* https://www.drupal.org/node/2815083
* @preserve
**/
(function ($) {
  var cachedScrollbarWidth = null;
  var max = Math.max,
    abs = Math.abs;
  var regexHorizontal = /left|center|right/;
  var regexVertical = /top|center|bottom/;
  var regexOffset = /[+-]\d+(\.[\d]+)?%?/;
  var regexPosition = /^\w+/;
  var regexPercent = /%$/;
  var _position = $.fn.position;
  function getOffsets(offsets, width, height) {
    return [parseFloat(offsets[0]) * (regexPercent.test(offsets[0]) ? width / 100 : 1), parseFloat(offsets[1]) * (regexPercent.test(offsets[1]) ? height / 100 : 1)];
  }
  function parseCss(element, property) {
    return parseInt($.css(element, property), 10) || 0;
  }
  function getDimensions(elem) {
    var raw = elem[0];
    if (raw.nodeType === 9) {
      return {
        width: elem.width(),
        height: elem.height(),
        offset: {
          top: 0,
          left: 0
        }
      };
    }
    if ($.isWindow(raw)) {
      return {
        width: elem.width(),
        height: elem.height(),
        offset: {
          top: elem.scrollTop(),
          left: elem.scrollLeft()
        }
      };
    }
    if (raw.preventDefault) {
      return {
        width: 0,
        height: 0,
        offset: {
          top: raw.pageY,
          left: raw.pageX
        }
      };
    }
    return {
      width: elem.outerWidth(),
      height: elem.outerHeight(),
      offset: elem.offset()
    };
  }
  var collisions = {
    fit: {
      left: function left(position, data) {
        var within = data.within;
        var withinOffset = within.isWindow ? within.scrollLeft : within.offset.left;
        var outerWidth = within.width;
        var collisionPosLeft = position.left - data.collisionPosition.marginLeft;
        var overLeft = withinOffset - collisionPosLeft;
        var overRight = collisionPosLeft + data.collisionWidth - outerWidth - withinOffset;
        var newOverRight;
        if (data.collisionWidth > outerWidth) {
          if (overLeft > 0 && overRight <= 0) {
            newOverRight = position.left + overLeft + data.collisionWidth - outerWidth - withinOffset;
            position.left += overLeft - newOverRight;
          } else if (overRight > 0 && overLeft <= 0) {
            position.left = withinOffset;
          } else if (overLeft > overRight) {
            position.left = withinOffset + outerWidth - data.collisionWidth;
          } else {
            position.left = withinOffset;
          }
        } else if (overLeft > 0) {
          position.left += overLeft;
        } else if (overRight > 0) {
          position.left -= overRight;
        } else {
          position.left = max(position.left - collisionPosLeft, position.left);
        }
      },
      top: function top(position, data) {
        var within = data.within;
        var withinOffset = within.isWindow ? within.scrollTop : within.offset.top;
        var outerHeight = data.within.height;
        var collisionPosTop = position.top - data.collisionPosition.marginTop;
        var overTop = withinOffset - collisionPosTop;
        var overBottom = collisionPosTop + data.collisionHeight - outerHeight - withinOffset;
        var newOverBottom;
        if (data.collisionHeight > outerHeight) {
          if (overTop > 0 && overBottom <= 0) {
            newOverBottom = position.top + overTop + data.collisionHeight - outerHeight - withinOffset;
            position.top += overTop - newOverBottom;
          } else if (overBottom > 0 && overTop <= 0) {
            position.top = withinOffset;
          } else if (overTop > overBottom) {
            position.top = withinOffset + outerHeight - data.collisionHeight;
          } else {
            position.top = withinOffset;
          }
        } else if (overTop > 0) {
          position.top += overTop;
        } else if (overBottom > 0) {
          position.top -= overBottom;
        } else {
          position.top = max(position.top - collisionPosTop, position.top);
        }
      }
    },
    flip: {
      left: function left(position, data) {
        var within = data.within;
        var withinOffset = within.offset.left + within.scrollLeft;
        var outerWidth = within.width;
        var offsetLeft = within.isWindow ? within.scrollLeft : within.offset.left;
        var collisionPosLeft = position.left - data.collisionPosition.marginLeft;
        var overLeft = collisionPosLeft - offsetLeft;
        var overRight = collisionPosLeft + data.collisionWidth - outerWidth - offsetLeft;
        var myOffset = data.my[0] === 'left' ? -data.elemWidth : data.my[0] === 'right' ? data.elemWidth : 0;
        var atOffset = data.at[0] === 'left' ? data.targetWidth : data.at[0] === 'right' ? -data.targetWidth : 0;
        var offset = -2 * data.offset[0];
        var newOverRight;
        var newOverLeft;
        if (overLeft < 0) {
          newOverRight = position.left + myOffset + atOffset + offset + data.collisionWidth - outerWidth - withinOffset;
          if (newOverRight < 0 || newOverRight < abs(overLeft)) {
            position.left += myOffset + atOffset + offset;
          }
        } else if (overRight > 0) {
          newOverLeft = position.left - data.collisionPosition.marginLeft + myOffset + atOffset + offset - offsetLeft;
          if (newOverLeft > 0 || abs(newOverLeft) < overRight) {
            position.left += myOffset + atOffset + offset;
          }
        }
      },
      top: function top(position, data) {
        var within = data.within;
        var withinOffset = within.offset.top + within.scrollTop;
        var outerHeight = within.height;
        var offsetTop = within.isWindow ? within.scrollTop : within.offset.top;
        var collisionPosTop = position.top - data.collisionPosition.marginTop;
        var overTop = collisionPosTop - offsetTop;
        var overBottom = collisionPosTop + data.collisionHeight - outerHeight - offsetTop;
        var top = data.my[1] === 'top';
        var myOffset = top ? -data.elemHeight : data.my[1] === 'bottom' ? data.elemHeight : 0;
        var atOffset = data.at[1] === 'top' ? data.targetHeight : data.at[1] === 'bottom' ? -data.targetHeight : 0;
        var offset = -2 * data.offset[1];
        var newOverTop;
        var newOverBottom;
        if (overTop < 0) {
          newOverBottom = position.top + myOffset + atOffset + offset + data.collisionHeight - outerHeight - withinOffset;
          if (newOverBottom < 0 || newOverBottom < abs(overTop)) {
            position.top += myOffset + atOffset + offset;
          }
        } else if (overBottom > 0) {
          newOverTop = position.top - data.collisionPosition.marginTop + myOffset + atOffset + offset - offsetTop;
          if (newOverTop > 0 || abs(newOverTop) < overBottom) {
            position.top += myOffset + atOffset + offset;
          }
        }
      }
    },
    flipfit: {
      left: function left() {
        for (var _len = arguments.length, args = new Array(_len), _key = 0; _key < _len; _key++) {
          args[_key] = arguments[_key];
        }
        collisions.flip.left.apply(this, args);
        collisions.fit.left.apply(this, args);
      },
      top: function top() {
        for (var _len2 = arguments.length, args = new Array(_len2), _key2 = 0; _key2 < _len2; _key2++) {
          args[_key2] = arguments[_key2];
        }
        collisions.flip.top.apply(this, args);
        collisions.fit.top.apply(this, args);
      }
    }
  };
  $.position = {
    scrollbarWidth: function scrollbarWidth() {
      if (cachedScrollbarWidth !== undefined) {
        return cachedScrollbarWidth;
      }
      var div = $('<div ' + "style='display:block;position:absolute;width:50px;height:50px;overflow:hidden;'>" + "<div style='height:100px;width:auto;'></div></div>");
      var innerDiv = div.children()[0];
      $('body').append(div);
      var w1 = innerDiv.offsetWidth;
      div.css('overflow', 'scroll');
      var w2 = innerDiv.offsetWidth;
      if (w1 === w2) {
        w2 = div[0].clientWidth;
      }
      div.remove();
      cachedScrollbarWidth = w1 - w2;
      return cachedScrollbarWidth;
    },
    getScrollInfo: function getScrollInfo(within) {
      var overflowX = within.isWindow || within.isDocument ? '' : within.element.css('overflow-x');
      var overflowY = within.isWindow || within.isDocument ? '' : within.element.css('overflow-y');
      var hasOverflowX = overflowX === 'scroll' || overflowX === 'auto' && within.width < within.element[0].scrollWidth;
      var hasOverflowY = overflowY === 'scroll' || overflowY === 'auto' && within.height < within.element[0].scrollHeight;
      return {
        width: hasOverflowY ? $.position.scrollbarWidth() : 0,
        height: hasOverflowX ? $.position.scrollbarWidth() : 0
      };
    },
    getWithinInfo: function getWithinInfo(element) {
      var withinElement = $(element || window);
      var isWindow = $.isWindow(withinElement[0]);
      var isDocument = !!withinElement[0] && withinElement[0].nodeType === 9;
      var hasOffset = !isWindow && !isDocument;
      return {
        element: withinElement,
        isWindow: isWindow,
        isDocument: isDocument,
        offset: hasOffset ? $(element).offset() : {
          left: 0,
          top: 0
        },
        scrollLeft: withinElement.scrollLeft(),
        scrollTop: withinElement.scrollTop(),
        width: withinElement.outerWidth(),
        height: withinElement.outerHeight()
      };
    }
  };
  $.fn.position = function (options) {
    if (!options || !options.of) {
      return _position.apply(this, arguments);
    }
    options = $.extend({}, options);
    var within = $.position.getWithinInfo(options.within);
    var scrollInfo = $.position.getScrollInfo(within);
    var collision = (options.collision || 'flip').split(' ');
    var offsets = {};
    var target = typeof options.of === 'string' ? $(document).find(options.of) : $(options.of);
    var dimensions = getDimensions(target);
    var targetWidth = dimensions.width;
    var targetHeight = dimensions.height;
    var targetOffset = dimensions.offset;
    if (target[0].preventDefault) {
      options.at = 'left top';
    }
    var basePosition = $.extend({}, targetOffset);
    $.each(['my', 'at'], function () {
      var pos = (options[this] || '').split(' ');
      if (pos.length === 1) {
        pos = regexHorizontal.test(pos[0]) ? pos.concat(['center']) : regexVertical.test(pos[0]) ? ['center'].concat(pos) : ['center', 'center'];
      }
      pos[0] = regexHorizontal.test(pos[0]) ? pos[0] : 'center';
      pos[1] = regexVertical.test(pos[1]) ? pos[1] : 'center';
      var horizontalOffset = regexOffset.exec(pos[0]);
      var verticalOffset = regexOffset.exec(pos[1]);
      offsets[this] = [horizontalOffset ? horizontalOffset[0] : 0, verticalOffset ? verticalOffset[0] : 0];
      options[this] = [regexPosition.exec(pos[0])[0], regexPosition.exec(pos[1])[0]];
    });
    if (collision.length === 1) {
      collision[1] = collision[0];
    }
    if (options.at[0] === 'right') {
      basePosition.left += targetWidth;
    } else if (options.at[0] === 'center') {
      basePosition.left += targetWidth / 2;
    }
    if (options.at[1] === 'bottom') {
      basePosition.top += targetHeight;
    } else if (options.at[1] === 'center') {
      basePosition.top += targetHeight / 2;
    }
    var atOffset = getOffsets(offsets.at, targetWidth, targetHeight);
    basePosition.left += atOffset[0];
    basePosition.top += atOffset[1];
    return this.each(function () {
      var using;
      var elem = $(this);
      var elemWidth = elem.outerWidth();
      var elemHeight = elem.outerHeight();
      var marginLeft = parseCss(this, 'marginLeft');
      var marginTop = parseCss(this, 'marginTop');
      var collisionWidth = elemWidth + marginLeft + parseCss(this, 'marginRight') + scrollInfo.width;
      var collisionHeight = elemHeight + marginTop + parseCss(this, 'marginBottom') + scrollInfo.height;
      var position = $.extend({}, basePosition);
      var myOffset = getOffsets(offsets.my, elem.outerWidth(), elem.outerHeight());
      if (options.my[0] === 'right') {
        position.left -= elemWidth;
      } else if (options.my[0] === 'center') {
        position.left -= elemWidth / 2;
      }
      if (options.my[1] === 'bottom') {
        position.top -= elemHeight;
      } else if (options.my[1] === 'center') {
        position.top -= elemHeight / 2;
      }
      position.left += myOffset[0];
      position.top += myOffset[1];
      var collisionPosition = {
        marginLeft: marginLeft,
        marginTop: marginTop
      };
      $.each(['left', 'top'], function (i, dir) {
        if (collisions[collision[i]]) {
          collisions[collision[i]][dir](position, {
            targetWidth: targetWidth,
            targetHeight: targetHeight,
            elemWidth: elemWidth,
            elemHeight: elemHeight,
            collisionPosition: collisionPosition,
            collisionWidth: collisionWidth,
            collisionHeight: collisionHeight,
            offset: [atOffset[0] + myOffset[0], atOffset[1] + myOffset[1]],
            my: options.my,
            at: options.at,
            within: within,
            elem: elem
          });
        }
      });
      if (options.using) {
        using = function using(props) {
          var left = targetOffset.left - position.left;
          var right = left + targetWidth - elemWidth;
          var top = targetOffset.top - position.top;
          var bottom = top + targetHeight - elemHeight;
          var feedback = {
            target: {
              element: target,
              left: targetOffset.left,
              top: targetOffset.top,
              width: targetWidth,
              height: targetHeight
            },
            element: {
              element: elem,
              left: position.left,
              top: position.top,
              width: elemWidth,
              height: elemHeight
            },
            horizontal: right < 0 ? 'left' : left > 0 ? 'right' : 'center',
            vertical: bottom < 0 ? 'top' : top > 0 ? 'bottom' : 'middle'
          };
          if (targetWidth < elemWidth && abs(left + right) < targetWidth) {
            feedback.horizontal = 'center';
          }
          if (targetHeight < elemHeight && abs(top + bottom) < targetHeight) {
            feedback.vertical = 'middle';
          }
          if (max(abs(left), abs(right)) > max(abs(top), abs(bottom))) {
            feedback.important = 'horizontal';
          } else {
            feedback.important = 'vertical';
          }
          options.using.call(this, props, feedback);
        };
      }
      elem.offset($.extend(position, {
        using: using
      }));
    });
  };
  if (!$.hasOwnProperty('ui')) {
    $.ui = {};
  }
  $.ui.position = collisions;
})(jQuery);;
!function(t){function e(e){for(var s,r,u=e[0],h=e[1],a=e[2],l=0,p=[];l<u.length;l++)r=u[l],Object.prototype.hasOwnProperty.call(n,r)&&n[r]&&p.push(n[r][0]),n[r]=0;for(s in h)Object.prototype.hasOwnProperty.call(h,s)&&(t[s]=h[s]);for(c&&c(e);p.length;)p.shift()();return i.push.apply(i,a||[]),o()}function o(){for(var t,e=0;e<i.length;e++){for(var o=i[e],s=!0,u=1;u<o.length;u++){var h=o[u];0!==n[h]&&(s=!1)}s&&(i.splice(e--,1),t=r(r.s=o[0]))}return t}var s={},n={1:0},i=[];function r(e){if(s[e])return s[e].exports;var o=s[e]={i:e,l:!1,exports:{}};return t[e].call(o.exports,o,o.exports,r),o.l=!0,o.exports}r.m=t,r.c=s,r.d=function(t,e,o){r.o(t,e)||Object.defineProperty(t,e,{enumerable:!0,get:o})},r.r=function(t){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(t,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(t,"__esModule",{value:!0})},r.t=function(t,e){if(1&e&&(t=r(t)),8&e)return t;if(4&e&&"object"==typeof t&&t&&t.__esModule)return t;var o=Object.create(null);if(r.r(o),Object.defineProperty(o,"default",{enumerable:!0,value:t}),2&e&&"string"!=typeof t)for(var s in t)r.d(o,s,function(e){return t[e]}.bind(null,s));return o},r.n=function(t){var e=t&&t.__esModule?function(){return t.default}:function(){return t};return r.d(e,"a",e),e},r.o=function(t,e){return Object.prototype.hasOwnProperty.call(t,e)},r.p="";var u=window.webpackJsonp=window.webpackJsonp||[],h=u.push.bind(u);u.push=e,u=u.slice();for(var a=0;a<u.length;a++)e(u[a]);var c=h;i.push([76,0]),o()}({35:function(t,e){t.exports=Drupal},76:function(t,e,o){"use strict";o.r(e);o(36),o(52),o(58),o(61),o(69);var s={TAB:9,RETURN:13,ESC:27,SPACE:32,PAGEUP:33,PAGEDOWN:34,END:35,HOME:36,LEFT:37,UP:38,RIGHT:39,DOWN:40};function n(t,e){t.prototype=Object.create(e.prototype),t.prototype.constructor=t,t.__proto__=e}var i=function(){function t(t,e){this.menu=e,this.domNode=t,this.popupMenu=!1,this.isMenubarItem=!1,this.hasFocus=!1,this.hasHover=!1}var e=t.prototype;return e._isPrintableCharacter=function(t){return 1===t.length&&t.match(/\S/)},e.init=function(){var t=this.domNode.parentElement.querySelector("ul");t&&(this.domNode.setAttribute("aria-haspopup","true"),this.popupMenu=new p(t,this),this.popupMenu.init()),this.domNode.addEventListener("keydown",this.handleKeydown.bind(this)),this.domNode.addEventListener("focus",this.handleFocus.bind(this)),this.domNode.addEventListener("blur",this.handleBlur.bind(this)),this.domNode.addEventListener("mouseover",this.handleMouseover.bind(this)),this.domNode.addEventListener("mouseout",this.handleMouseout.bind(this))},e.setExpanded=function(t){t?this.domNode.setAttribute("aria-expanded","true"):this.domNode.setAttribute("aria-expanded","false")},e.getBoundaries=function(){return this.domNode.getBoundingClientRect()},e.getIsMenubarItem=function(){return this.isMenubarItem},e.getHover=function(){return this.hasHover},e.handleFocus=function(){this.menu.setFocus(!0)},e.handleBlur=function(){this.menu.setFocus(!1)},e.handleMouseover=function(){this.hasHover=!0,this.popupMenu&&(this.popupMenu.setHover(!0),this.popupMenu.open())},e.handleMouseout=function(){this.hasHover=!1,this.popupMenu&&(this.popupMenu.setHover(!1),setTimeout(this.popupMenu.close.bind(this.popupMenu,!1),300))},t}(),r=function(t){function e(e,o){var s;return(s=t.call(this,e,o)||this).isMenubarItem=!0,s}n(e,t);var o=e.prototype;return o.init=function(){t.prototype.init.call(this),this.domNode.tabIndex=-1},o.setTabIndex=function(t){this.domNode.tabIndex=t},o.focusOnSelf=function(){this.domNode.focus()},o.focusOnPreviousSibling=function(){this.menu.setFocusToPreviousItem(this)},o.focusOnNextSibling=function(){this.menu.setFocusToNextItem(this)},o.handleKeydown=function(t){var e=t.key,o=!1;switch(t.keyCode){case s.SPACE:case s.RETURN:case s.DOWN:this.popupMenu&&(this.popupMenu.open(),this.popupMenu.setFocusToFirstItem(),o=!0);break;case s.LEFT:this.menu.setFocusToPreviousItem(this),o=!0;break;case s.RIGHT:this.menu.setFocusToNextItem(this),o=!0;break;case s.UP:this.popupMenu&&(this.popupMenu.open(),this.popupMenu.setFocusToLastItem(),o=!0);break;case s.HOME:case s.PAGEUP:this.menu.setFocusToFirstItem(),o=!0;break;case s.END:case s.PAGEDOWN:this.menu.setFocusToLastItem(),o=!0;break;case s.TAB:case s.ESC:this.popupMenu&&this.popupMenu.close(!0);break;default:this._isPrintableCharacter(e)&&(this.menu.setFocusByFirstCharacter(this,e),o=!0)}o&&(t.stopPropagation(),t.preventDefault())},e}(i),u=function(t){function e(){return t.apply(this,arguments)||this}n(e,t);var o=e.prototype;return o.init=function(){t.prototype.init.call(this),this.domNode.addEventListener("click",this.handleClick.bind(this))},o.handleKeydown=function(t){var e,o=t.currentTarget,n=t.key,i=!1;switch(t.keyCode){case s.SPACE:case s.RETURN:if(this.popupMenu)this.popupMenu.open(),this.popupMenu.setFocusToFirstItem();else{try{e=new MouseEvent("click",{view:window,bubbles:!0,cancelable:!0})}catch(t){document.createEvent&&(e=document.createEvent("MouseEvents")).initEvent("click",!0,!0)}o.dispatchEvent(e)}i=!0;break;case s.UP:this.menu.setFocusToPreviousItem(this),i=!0;break;case s.DOWN:this.menu.setFocusToNextItem(this),i=!0;break;case s.LEFT:this.menu.setFocusToController("previous",!0),this.menu.close(!0),i=!0;break;case s.RIGHT:this.popupMenu?(this.popupMenu.open(),this.popupMenu.setFocusToFirstItem()):(this.menu.setFocusToController("next",!0),this.menu.close(!0)),i=!0;break;case s.HOME:case s.PAGEUP:this.menu.setFocusToFirstItem(),i=!0;break;case s.END:case s.PAGEDOWN:this.menu.setFocusToLastItem(),i=!0;break;case s.ESC:this.menu.setFocusToController(),this.menu.close(!0),i=!0;break;case s.TAB:this.menu.setFocusToController();break;default:this._isPrintableCharacter(n)&&(this.menu.setFocusByFirstCharacter(this,n),i=!0)}i&&(t.stopPropagation(),t.preventDefault())},o.handleClick=function(){this.menu.setFocusToController(),this.menu.close(!0)},o.handleBlur=function(){t.prototype.handleBlur.call(this),setTimeout(this.menu.close.bind(this.menu,!1),300)},o.handleMouseover=function(){this.menu.setHover(!0),this.menu.open(),t.prototype.handleMouseover.call(this)},o.handleMouseout=function(){t.prototype.handleMouseout.call(this),this.menu.setHover(!1),setTimeout(this.menu.close.bind(this.menu,!1),300)},e}(i),h=1200;function a(t,e){t.prototype=Object.create(e.prototype),t.prototype.constructor=t,t.__proto__=e}var c=function(){function t(t){this.domNode=t,this.menuItems=[],this.firstChars=[],this.firstItem=null,this.lastItem=null,this.hasFocus=!1,this.hasHover=!1,this.isMenubar=!1}var e=t.prototype;return e._createMenuItem=function(t){},e.init=function(){var t=this;this.domNode.children.length>0&&[].slice.call(this.domNode.children).forEach((function(e){var o=e.firstElementChild;if(o&&"A"===o.tagName){var s=t._createMenuItem(o);s.init(),t.menuItems.push(s);var n=o.textContent.trim();t.firstChars.push(n.substring(0,1).toLowerCase())}}));var e=this.menuItems.length;e>0&&(this.firstItem=this.menuItems[0],this.lastItem=this.menuItems[e-1])},e.setFocus=function(t){this.hasFocus=t},e.setHover=function(t){this.hasHover=t},e.setFocusToItem=function(t){},e.setFocusToFirstItem=function(){this.setFocusToItem(this.firstItem)},e.setFocusToLastItem=function(){this.setFocusToItem(this.lastItem)},e.setFocusToPreviousItem=function(t){var e=!1;if(t===this.firstItem)e=this.lastItem;else{var o=this.menuItems.indexOf(t);e=this.menuItems[o-1]}e&&this.setFocusToItem(e)},e.setFocusToNextItem=function(t){var e=!1;if(t===this.lastItem)e=this.firstItem;else{var o=this.menuItems.indexOf(t);e=this.menuItems[o+1]}e&&this.setFocusToItem(e)},e.setFocusByFirstCharacter=function(t,e){var o=e.toLowerCase(),s=this.menuItems.indexOf(t)+1;s===this.menuItems.length&&(s=0);var n=this._getIndexFirstChars(s,o);-1===n&&(n=this._getIndexFirstChars(0,o)),n>-1&&this.setFocusToItem(this.menuItems[n])},e._getIndexFirstChars=function(t,e){for(var o=t;1<this.firstChars.length;o++)if(e===this.firstChars[o])return o;return-1},t}(),l=function(t){function e(e){var o,s="Menubar constructor argument menuBarNode";if(!(e instanceof Element))throw new TypeError(s+" is not a Dom Element");if(0===e.childElementCount)throw new Error(s+" does not have element children");for(var n=e.firstElementChild;n;){var i=n.firstElementChild;if(n&&i&&"A"!==i.tagName)throw new Error(s+" has child elements are not A elements");n=n.nextElementSibling}return(o=t.call(this,e)||this).isMenubar=!0,o}a(e,t);var o=e.prototype;return o._createMenuItem=function(t){return new r(t,this)},o.init=function(){t.prototype.init.call(this),this.domNode.setAttribute("role","menubar"),this.firstItem.setTabIndex(0)},o.setFocusToItem=function(t){var e=!1;this.menuItems.forEach((function(t){0===t.domNode.tabIndex&&(e="true"===t.domNode.getAttribute("aria-expanded")),t.domNode.tabIndex=-1,t.popupMenu&&t.popupMenu.close()})),t.domNode.focus(),t.domNode.tabIndex=0,e&&t.popupMenu&&t.popupMenu.open()},e}(c),p=function(t){function e(e,o){var s;return(s=t.call(this,e)||this).controller=o,s}a(e,t);var o=e.prototype;return o._createMenuItem=function(t){return new u(t,this)},o.init=function(){t.prototype.init.call(this),this.domNode.setAttribute("role","menu"),this.domNode.addEventListener("mouseover",this.handleMouseover.bind(this)),this.domNode.addEventListener("mouseout",this.handleMouseout.bind(this))},o.setFocusToItem=function(t){t.domNode.focus()},o.setFocusToController=function(t){var e=t;"string"!=typeof e&&(e=""),""!==e?"previous"===e?this.controller.focusOnPreviousSibling():"next"===e&&this.controller.focusOnNextSibling():this.controller&&this.controller.focusOnSelf()},o.open=function(){var t=this.controller.getBoundaries();this.controller.getIsMenubarItem()?(this.domNode.style.display="block",this.domNode.style.position="absolute",this.domNode.style.top=t.height-1+"px",this.domNode.style.zIndex=h):(this.domNode.parentNode.style.position="relative",this.domNode.style.display="block",this.domNode.style.position="absolute",this.domNode.style.left=t.width+"px",this.domNode.style.zIndex=h),this.controller.setExpanded(!0)},o.close=function(t){for(var e=this.controller.getHover(),o=this.hasFocus,s=0;s<this.menuItems.length;s++){var n=this.menuItems[s];n.popupMenu&&(o=o||n.popupMenu.hasFocus)}this.controller.getIsMenubarItem()||(e=!1),!t&&(o||this.hasHover||e)||(this.domNode.style.display="none",this.domNode.style.zIndex="0",this.controller.setExpanded(!1))},o.handleMouseover=function(){this.hasHover=!0},o.handleMouseout=function(){this.hasHover=!1},e}(c),d=o(35);!function(t){t.behaviors.primaryNav={attach:function(t){var e=t.querySelector(".menu--main");e&&new l(e).init()}}}(o.n(d).a)}});;
jQuery(document).ready(function($) {

  $("div.full-width.white-bg").each(function(index) {
    if ($(this).next().hasClass("full-width white-bg") == true) {

      var childElement = $(this).children(":first").children(":first");

      if (childElement[0].classList.contains("cw-section") == true) {
        $(this).children(":first").children(":first").css("padding", "2em 0 0 0");
      }
    } else {
      return;
    }
  });
  
  $("div.full-width.midnight-blue-bg").each(function(index) {
    if ($(this).next().hasClass("full-width midnight-blue-bg") == true) {

      var childElement = $(this).children(":first").children(":first");

      if (childElement[0].classList.contains("cw-section") == true) {
        $(this).children(":first").children(":first").css("padding", "2em 0 0 0");
      }
    } else {
      return;
    }
  });
  
  $("div.full-width.wildcat-blue-bg").each(function(index) {
    if ($(this).next().hasClass("full-width wildcat-blue-bg") == true) {

      var childElement = $(this).children(":first").children(":first");

      if (childElement[0].classList.contains("cw-section") == true) {
        $(this).children(":first").children(":first").css("padding", "2em 0 0 0");
      }
    } else {
      return;
    }
  });  
  
  $("div.full-width.sky-blue-bg").each(function(index) {
    if ($(this).next().hasClass("full-width sky-blue-bg") == true) {

      var childElement = $(this).children(":first").children(":first");

      if (childElement[0].classList.contains("cw-section") == true) {
        $(this).children(":first").children(":first").css("padding", "2em 0 0 0");
      }
    } else {
      return;
    }
  });  
  
  $("div.full-width.black-bg").each(function(index) {
    if ($(this).next().hasClass("full-width black-bg") == true) {

      var childElement = $(this).children(":first").children(":first");

      if (childElement[0].classList.contains("cw-section") == true) {
        $(this).children(":first").children(":first").css("padding", "2em 0 0 0");
      }
    } else {
      return;
    }
  });  
  
});;
!function(e){var t={};function n(r){if(t[r])return t[r].exports;var o=t[r]={i:r,l:!1,exports:{}};return e[r].call(o.exports,o,o.exports,n),o.l=!0,o.exports}n.m=e,n.c=t,n.d=function(e,t,r){n.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:r})},n.r=function(e){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},n.t=function(e,t){if(1&t&&(e=n(e)),8&t)return e;if(4&t&&"object"==typeof e&&e&&e.__esModule)return e;var r=Object.create(null);if(n.r(r),Object.defineProperty(r,"default",{enumerable:!0,value:e}),2&t&&"string"!=typeof e)for(var o in e)n.d(r,o,function(t){return e[t]}.bind(null,o));return r},n.n=function(e){var t=e&&e.__esModule?function(){return e.default}:function(){return e};return n.d(t,"a",t),t},n.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},n.p="",n(n.s=75)}({75:function(e,t){!function(){"use strict";function e(){}var t=!1;window.addEventListener("resize",(function(){!1!==t&&clearTimeout(t),t=setTimeout(e,200)}))}()}});;
jQuery(document).ready(function ($) {
  

  $( document ).ready(function() {
    var mobile_menu_height = $('header').height();
    $('#upper-nav__mobile-menu-trigger').height(mobile_menu_height);
  });

    // Store the window width
    var windowWidth = $(window).width();

    // Resize Event
    $(window).resize(function(){

      // Check window width has actually changed and it's not just iOS triggering a resize event on scroll
      if ($(window).width() != windowWidth) {
          // Update the window width for next time
          windowWidth = $(window).width();

          var mobile_menu_height = $('header').height();
          $('#upper-nav__mobile-menu-trigger').height(mobile_menu_height);

          var search_block_status = $('.search-block').css('display');
          var mobile_navigation_status = $('.mobile-navigation').css('display');
          var lateral_navigation_status = $('.lateral-navigation').css('display');

          if(search_block_status == 'block') {
            $('.search-block').css('display', 'none');
            $('span.upper-nav__search-text').removeClass('is-active');
          }

          else if (mobile_navigation_status == 'block') {
            $('span.upper-nav__menu-text').removeClass('is-active');
            $('.mobile-navigation').css('display', 'none');
          } 
          
          else if (lateral_navigation_status == 'block') {
            $('span.upper-nav__menu-text').removeClass('is-active');
            $('.lateral-navigation').css('display', 'none');
            $('#main-content').css('transform', 'none');
          }
      }
    });

  var search_block = $('.search-block');

  $('.open-search-block').click(function() {
    var search_block_state = search_block.css('display');

    if (search_block_state == 'block') {
      $('.search-block').css('display', 'none');
      $('span.upper-nav__search-text').removeClass('is-active');
    }

    else {
      search_block.css('display', 'block');
    $('span.upper-nav__search-text').addClass('is-active');
    $('#search-block__search-field').get(0).focus();
    $('#search-block__search-field').trigger('click');
    }
    
    var mobile_menu_state = $('.mobile-navigation').css('display');

    if (mobile_menu_state == 'block') {
      $('span.upper-nav__menu-text').removeClass('is-active');
      $('.mobile-navigation').css('display', 'none');
    }

  });

  $('.search-block__close-icon').click(function() {
    search_block.css('display', 'none');
    $('span.upper-nav__search-text').removeClass('is-active');
  });

  var main_content = $('#main-content');
  var page_height = $('#main-content').height() + "px";
  var lateral_navigation = $('.lateral-navigation');

  $('.expand-lateral-menu').click(function() {
    var lateral_menu_state = lateral_navigation.css('display');

    if(lateral_menu_state == 'none') {
      $('span.upper-nav__menu-text').addClass('is-active');
      lateral_navigation.css("height", page_height);
      lateral_navigation.css("display", "block");
      main_content.css('transform', 'translateX(-350px)');
    }

    else if (lateral_menu_state == 'block') {
      $('span.upper-nav__menu-text').removeClass('is-active');
      lateral_navigation.css('display', 'none');
      main_content.css('transform', 'none');
    }

    else {
      return false;
    }
  });

  $('.lateral-navigation__close-block').click(function() {
    $('span.upper-nav__menu-text').removeClass('is-active');
      lateral_navigation.css('display', 'none');
      main_content.css('transform', 'none');
  });

  $('.mobile-naviation__main-menu').css('display', 'block');
  $('.mobile-naviation__quick-links').css('display', 'none');

  $('.mobile-navigation__menu-tab-quick a').click(function() {
    $('.mobile-naviation__main-menu').css('display', 'none');
    $('.mobile-naviation__quick-links').css('display', 'block');
    $('.mobile-navigation__menu-tab-quick a').addClass('mobile-navigation__menu-tab-hover');
    $('.mobile-navigation__menu-tab-main a').removeClass('mobile-navigation__menu-tab-hover');
  });

  $('.mobile-navigation__menu-tab-main a').click(function() {
    $('.mobile-naviation__main-menu').css('display', 'block');
    $('.mobile-naviation__quick-links').css('display', 'none');
    $('.mobile-navigation__menu-tab-quick a').removeClass('mobile-navigation__menu-tab-hover');
    $('.mobile-navigation__menu-tab-main a').addClass('mobile-navigation__menu-tab-hover');
  });

  $('#upper-nav__mobile-menu-trigger').click(function() {
    var mobile_menu_state = $('.mobile-navigation').css('display');

    if(mobile_menu_state == 'block') {
      $('span.upper-nav__menu-text').removeClass('is-active');
      $('.mobile-navigation').css('display', 'none');
    }
    else if (mobile_menu_state == 'none') {
      $('span.upper-nav__menu-text').addClass('is-active');
      $('.mobile-navigation').css('display', 'block');
    }
    else {
      return false;
    }
  });
});;
jQuery(document).ready(function($){
  var acc = document.getElementsByClassName("accordion");
  var i;

  for (i = 0; i < acc.length; i++) {
    acc[i].addEventListener("click", function() {
      this.classList.toggle("accordion-active");
      var accordion__panel = this.nextElementSibling;
      if (accordion__panel.style.maxHeight) {
        accordion__panel.style.maxHeight = null;
      } else {
        accordion__panel.style.maxHeight = accordion__panel.scrollHeight + "px";
      }
    });
  }
});
;
/*! Backstretch - v2.1.18 - 2019-09-18\n* Copyright (c) 2019 Scott Robbin;* Fork of improvements - by Daniel Cohen Gindi (danielgindi@gmail.com) Licensed MIT */!function(a,b,c){"use strict";function d(a){return m.hasOwnProperty(a)?a:"cover"}var e=/^.*(youtu\.be\/|youtube\.com\/v\/|youtube\.com\/embed\/|youtube\.com\/watch\?v=|youtube\.com\/watch\?.*\&v=)([^#\&\?]*).*/i;a.fn.backstretch=function(d,e){var f=arguments;0===a(b).scrollTop()&&b.scrollTo(0,0);var g;return this.each(function(b){var h=a(this),i=h.data("backstretch");if(i){if("string"==typeof f[0]&&"function"==typeof i[f[0]]){var j=i[f[0]].apply(i,Array.prototype.slice.call(f,1));return j===i&&(j=c),void(j!==c&&(g=g||[],g[b]=j))}e=a.extend(i.options,e),"object"==typeof i&&"destroy"in i&&i.destroy(!0)}if(!d||d&&0===d.length){var k=h.css("background-image");k&&"none"!==k?d=[{url:h.css("backgroundImage").replace(/url\(|\)|"|'/g,"")}]:a.error("No images were supplied for Backstretch, or element must have a CSS-defined background image.")}i=new n(this,d,e||{}),h.data("backstretch",i)}),g?1===g.length?g[0]:g:this},a.backstretch=function(b,c){return a("body").backstretch(b,c).data("backstretch")},a.expr[":"].backstretch=function(b){return a(b).data("backstretch")!==c},a.fn.backstretch.defaults={duration:5e3,transition:"fade",transitionDuration:0,animateFirst:!0,alignX:.5,alignY:.5,paused:!1,start:0,preload:2,preloadSize:1,resolutionRefreshRate:2500,resolutionChangeRatioThreshold:.1};var f={wrap:{left:0,top:0,overflow:"hidden",margin:0,padding:0,height:"100%",width:"100%",zIndex:-999999},itemWrapper:{position:"absolute",display:"none",margin:0,padding:0,border:"none",width:"100%",height:"100%",zIndex:-999999},item:{position:"absolute",margin:0,padding:0,border:"none",width:"100%",height:"100%",maxWidth:"none"}},g=function(){var c=function(a){for(var b=1;b<a.length;b++){for(var c=a[b],d=b;a[d-1]&&parseInt(a[d-1].width,10)>parseInt(c.width,10);)a[d]=a[d-1],--d;a[d]=c}return a},d=function(a,c,d){for(var e,f,g=b.devicePixelRatio||1,h=q(),i=(r(),c>a?"portrait":a>c?"landscape":"square"),j=0,k=0;k<d.length&&(f=d[k],"string"==typeof f&&(f=d[k]={url:f}),f.pixelRatio&&"auto"!==f.pixelRatio&&parseFloat(f.pixelRatio)!==g||f.deviceOrientation&&f.deviceOrientation!==h||f.windowOrientation&&f.windowOrientation!==h||f.orientation&&f.orientation!==i||(j=k,e=a,"auto"===f.pixelRatio&&(a*=g),!(f.width>=e)));k++);return d[Math.min(k,j)]},e=function(a,b){if("string"==typeof a)a=a.replace(/{{(width|height)}}/g,b);else if(a instanceof Array)for(var c=0;c<a.length;c++)a[c].src?a[c].src=e(a[c].src,b):a[c]=e(a[c],b);return a};return function(b,f){for(var g=b.width(),h=b.height(),i=[],j=function(a,b){return"width"===b?g:"height"===b?h:a},k=0;k<f.length;k++)if(a.isArray(f[k])){f[k]=c(f[k]);var l=d(g,h,f[k]);i.push(l)}else{"string"==typeof f[k]&&(f[k]={url:f[k]});var m=a.extend({},f[k]);m.url=e(m.url,j),i.push(m)}return i}}(),h=function(a){return e.test(a.url)||a.isVideo},i=function(b,c,d,e,f){var g=[],i=function(a){for(var b=0;b<g.length;b++)if(g[b].src===a.src)return g[b];return g.push(a),a},j=function(a,b,c){"function"==typeof b&&b.call(a,c)};return function b(c,d,e,f,g){if("undefined"!=typeof c){a.isArray(c)||(c=[c]),arguments.length<5&&"function"==typeof arguments[arguments.length-1]&&(g=arguments[arguments.length-1]),d="function"!=typeof d&&d?d:0,e="function"==typeof e||!e||e<0?c.length:Math.min(e,c.length),f="function"!=typeof f&&f?f:1,d>=c.length&&(d=0,e=0),f<0&&(f=e),f=Math.min(f,e);var k=c.slice(d+f,e-f);if(c=c.slice(d,f),e=c.length,!e)return void j(c,g,!0);for(var l,m=0,n=function(){m++,m===e&&(j(c,g,!k),b(k,0,0,f,g))},o=0;o<c.length;o++)h(c[o])||(l=new Image,l.src=c[o].url,l=i(l),l.complete?n():a(l).on("load error",n))}}}(),j=function(b){for(var c=[],d=0;d<b.length;d++)"string"==typeof b[d]?c.push({url:b[d]}):a.isArray(b[d])?c.push(j(b[d])):c.push(k(b[d]));return c},k=function(a,e){return(a.centeredX||a.centeredY)&&(b.console&&b.console.log&&b.console.log("jquery.backstretch: `centeredX`/`centeredY` is deprecated, please use `alignX`/`alignY`"),a.centeredX&&(a.alignX=.5),a.centeredY&&(a.alignY=.5)),a.speed!==c&&(b.console&&b.console.log&&b.console.log("jquery.backstretch: `speed` is deprecated, please use `transitionDuration`"),a.transitionDuration=a.speed,a.transition="fade"),a.resolutionChangeRatioTreshold!==c&&(b.console.log("jquery.backstretch: `treshold` is a typo!"),a.resolutionChangeRatioThreshold=a.resolutionChangeRatioTreshold),a.fadeFirst!==c&&(a.animateFirst=a.fadeFirst),a.fade!==c&&(a.transitionDuration=a.fade,a.transition="fade"),a.scale&&(a.scale=d(a.scale)),l(a)},l=function(a,b){return"left"===a.alignX?a.alignX=0:"center"===a.alignX?a.alignX=.5:"right"===a.alignX?a.alignX=1:(a.alignX!==c||b)&&(a.alignX=parseFloat(a.alignX),isNaN(a.alignX)&&(a.alignX=.5)),"top"===a.alignY?a.alignY=0:"center"===a.alignY?a.alignY=.5:"bottom"===a.alignY?a.alignY=1:(a.alignX!==c||b)&&(a.alignY=parseFloat(a.alignY),isNaN(a.alignY)&&(a.alignY=.5)),a},m={cover:"cover",fit:"fit","fit-smaller":"fit-smaller",fill:"fill"},n=function(c,d,e){this.options=a.extend({},a.fn.backstretch.defaults,e||{}),this.firstShow=!0,k(this.options,!0),this.images=j(a.isArray(d)?d:[d]),this.options.paused&&(this.paused=!0),this.options.start>=this.images.length&&(this.options.start=this.images.length-1),this.options.start<0&&(this.options.start=0),this.isBody=c===document.body;var h=a(b);this.$container=a(c),this.$root=this.isBody?s?h:a(document):this.$container,this.originalImages=this.images,this.images=g(this.options.alwaysTestWindowResolution?h:this.$root,this.originalImages),i(this.images,this.options.start||0,this.options.preload||1);var l=this.$container.children(".backstretch").first();if(this.$wrap=l.length?l:a('<div class="backstretch"></div>').css(this.options.bypassCss?{}:f.wrap).appendTo(this.$container),!this.options.bypassCss){if(!this.isBody){var m=this.$container.css("position"),n=this.$container.css("zIndex");this.$container.css({position:"static"===m?"relative":m,zIndex:"auto"===n?0:n}),this.$wrap.css({zIndex:-999998})}this.$wrap.css({position:this.isBody&&s?"fixed":"absolute"})}this.index=this.options.start,this.show(this.index),h.on("resize.backstretch",a.proxy(this.resize,this)).on("orientationchange.backstretch",a.proxy(function(){this.isBody&&0===b.pageYOffset&&(b.scrollTo(0,1),this.resize())},this))},o=function(b){var d=b.transition||"fade";"string"==typeof d&&d.indexOf("|")>-1&&(d=d.split("|")),d instanceof Array&&(d=d[Math.round(Math.random()*(d.length-1))]);var e=b.new,f=b.old?b.old:a([]);switch(d.toString().toLowerCase()){default:case"fade":e.fadeIn({duration:b.duration,complete:b.complete,easing:b.easing||c});break;case"fadeinout":case"fade_in_out":var g=function(){e.fadeIn({duration:b.duration/2,complete:b.complete,easing:b.easing||c})};f.length?f.fadeOut({duration:b.duration/2,complete:g,easing:b.easing||c}):g();break;case"pushleft":case"push_left":case"pushright":case"push_right":case"pushup":case"push_up":case"pushdown":case"push_down":case"coverleft":case"cover_left":case"coverright":case"cover_right":case"coverup":case"cover_up":case"coverdown":case"cover_down":var h=d.match(/^(cover|push)_?(.*)$/),i="left"===h[2]?"right":"right"===h[2]?"left":"down"===h[2]?"top":"up"===h[2]?"bottom":"right",j={display:""},k={};if(j[i]="-100%",k[i]=0,e.css(j).animate(k,{duration:b.duration,complete:function(){e.css(i,""),b.complete.apply(this,arguments)},easing:b.easing||c}),"push"===h[1]&&f.length){var l={};l[i]="100%",f.animate(l,{duration:b.duration,complete:function(){f.css("display","none")},easing:b.easing||c})}}};n.prototype={resize:function(){try{var e=this.options.alwaysTestWindowResolution?a(b):this.$root,f=e.width(),h=e.height(),j=f/(this._lastResizeContainerWidth||0),k=h/(this._lastResizeContainerHeight||0),l=this.options.resolutionChangeRatioThreshold||0;if((f!==this._lastResizeContainerWidth||h!==this._lastResizeContainerHeight)&&(Math.abs(j-1)>=l||isNaN(j)||Math.abs(k-1)>=l||isNaN(k))&&(this._lastResizeContainerWidth=f,this._lastResizeContainerHeight=h,this.images=g(e,this.originalImages),this.options.preload&&i(this.images,(this.index+1)%this.images.length,this.options.preload),1===this.images.length&&this._currentImage.url!==this.images[0].url)){var m=this;clearTimeout(m._selectAnotherResolutionTimeout),m._selectAnotherResolutionTimeout=setTimeout(function(){m.show(0)},this.options.resolutionRefreshRate)}var n,o,p={left:0,top:0,right:"auto",bottom:"auto"},q=this.isBody?this.$root.width():this.$root.innerWidth(),r=this.isBody?b.innerHeight?b.innerHeight:this.$root.height():this.$root.innerHeight(),s=this.$itemWrapper.data("width"),t=this.$itemWrapper.data("height"),u=s/t||1,v=this._currentImage.alignX===c?this.options.alignX:this._currentImage.alignX,w=this._currentImage.alignY===c?this.options.alignY:this._currentImage.alignY,x=d(this._currentImage.scale||this.options.scale);if("fit"===x||"fit-smaller"===x){if(n=s,o=t,n>q||o>r||"fit-smaller"===x){var y=q/r;y>u?(n=Math.floor(r*u),o=r):y<u?(n=q,o=Math.floor(q/u)):(n=q,o=r)}}else"fill"===x?(n=q,o=r):(n=Math.max(r*u,q),o=Math.max(n/u,r));p.top=-(o-r)*w,p.left=-(n-q)*v,p.width=n,p.height=o,this.options.bypassCss||this.$wrap.css({width:q,height:r}).find(">.backstretch-item").not(".deleteable").each(function(){var b=a(this);b.find("img,video,iframe").css(p)});var z=a.Event("backstretch.resize",{relatedTarget:this.$container[0]});this.$container.trigger(z,this)}catch(a){}return this},show:function(b,d){if(!(Math.abs(b)>this.images.length-1)){var e=this,g=e.$wrap.find(">.backstretch-item").addClass("deleteable"),i=e.videoWrapper,j={relatedTarget:e.$container[0]};e.$container.trigger(a.Event("backstretch.before",j),[e,b]),this.index=b;var k=e.images[b];clearTimeout(e._cycleTimeout),delete e.videoWrapper;var l=h(k);return l?(e.videoWrapper=new p(k),e.$item=e.videoWrapper.$video.css("pointer-events","none")):e.$item=a("<img />"),e.$itemWrapper=a('<div class="backstretch-item">').append(e.$item),this.options.bypassCss?e.$itemWrapper.css({display:"none"}):(e.$itemWrapper.css(f.itemWrapper),e.$item.css(f.item)),e.$item.bind(l?"canplay":"load",function(f){var h=a(this),k=h.parent(),m=k.data("options");d&&(m=a.extend({},m,d));var n=this.naturalWidth||this.videoWidth||this.width,p=this.naturalHeight||this.videoHeight||this.height;k.data("width",n).data("height",p);var q=function(a){return m[a]!==c?m[a]:e.options[a]},r=q("transition"),s=q("transitionEasing"),t=q("transitionDuration"),u=function(){i&&(i.stop(),i.destroy()),g.remove(),!e.paused&&e.images.length>1&&e.cycle(),e.options.bypassCss||e.isBody||e.$container.css("background-image","none"),a(["after","show"]).each(function(){e.$container.trigger(a.Event("backstretch."+this,j),[e,b])}),l&&e.videoWrapper.play()};e.firstShow&&!e.options.animateFirst||!t||!r?(k.show(),u()):o({new:k,old:g,transition:r,duration:t,easing:s,complete:u}),e.firstShow=!1,e.resize()}),e.$itemWrapper.appendTo(e.$wrap),e.$item.attr("alt",k.alt||""),e.$itemWrapper.data("options",k),l||e.$item.attr("src",k.url),e._currentImage=k,e}},current:function(){return this.index},next:function(){var a=Array.prototype.slice.call(arguments,0);return a.unshift(this.index<this.images.length-1?this.index+1:0),this.show.apply(this,a)},prev:function(){var a=Array.prototype.slice.call(arguments,0);return a.unshift(0===this.index?this.images.length-1:this.index-1),this.show.apply(this,a)},pause:function(){return this.paused=!0,this.videoWrapper&&this.videoWrapper.pause(),this},resume:function(){return this.paused=!1,this.videoWrapper&&this.videoWrapper.play(),this.cycle(),this},cycle:function(){if(this.images.length>1){clearTimeout(this._cycleTimeout);var b=this._currentImage&&this._currentImage.duration||this.options.duration,c=h(this._currentImage),d=function(){this.$item.off(".cycle"),this.paused||this.next()};if(c){if(!this._currentImage.loop){var e=0;this.$item.on("playing.cycle",function(){var b=a(this).data("player");clearTimeout(e),e=setTimeout(function(){b.pause(),b.$video.trigger("ended")},1e3*(b.getDuration()-b.getCurrentTime()))}).on("ended.cycle",function(){clearTimeout(e)})}this.$item.on("error.cycle initerror.cycle",a.proxy(d,this))}c&&!this._currentImage.duration?this.$item.on("ended.cycle",a.proxy(d,this)):this._cycleTimeout=setTimeout(a.proxy(d,this),b)}return this},destroy:function(c){a(b).off("resize.backstretch orientationchange.backstretch"),this.videoWrapper&&this.videoWrapper.destroy(),clearTimeout(this._cycleTimeout),c||this.$wrap.remove(),this.$container.removeData("backstretch")}};var p=function(){this.init.apply(this,arguments)};p.prototype.init=function(d){var f,g=this,h=function(){g.$video=f,g.video=f[0]},i="video";if(d.url instanceof Array||!e.test(d.url)||(i="youtube"),g.type=i,"youtube"===i){p.loadYoutubeAPI(),g.ytId=d.url.match(e)[2];var j="https://www.youtube.com/embed/"+g.ytId+"?rel=0&autoplay=0&showinfo=0&controls=0&modestbranding=1&cc_load_policy=0&disablekb=1&iv_load_policy=3&loop=0&enablejsapi=1&origin="+encodeURIComponent(b.location.origin);g.__ytStartMuted=!!d.mute||d.mute===c,f=a("<iframe />").attr({src_to_load:j}).css({border:0,margin:0,padding:0}).data("player",g),d.loop&&f.on("ended.loop",function(){g.__manuallyStopped||g.play()}),g.ytReady=!1,h(),b.YT&&b.YT.loaded?(g._initYoutube(),f.trigger("initsuccess")):a(b).one("youtube_api_load",function(){g._initYoutube(),f.trigger("initsuccess")})}else{f=a("<video>").prop("autoplay",!1).prop("controls",!1).prop("loop",!!d.loop).prop("muted",!!d.mute||d.mute===c).prop("preload","auto").prop("poster",d.poster||"");for(var k=d.url instanceof Array?d.url:[d.url],l=0;l<k.length;l++){var m=k[l];"string"==typeof m&&(m={src:m}),a("<source>").attr("src",m.src).attr("type",m.type||null).appendTo(f)}f[0].canPlayType&&k.length?f.trigger("initsuccess"):f.trigger("initerror"),h()}},p.prototype._initYoutube=function(){var c=this,d=b.YT;c.$video.attr("src",c.$video.attr("src_to_load")).removeAttr("src_to_load");var e=!!c.$video[0].parentNode;if(!e){var f=a("<div>").css("display","none !important").appendTo(document.body);c.$video.appendTo(f)}var g=new d.Player(c.video,{events:{onReady:function(){c.__ytStartMuted&&g.mute(),e||(c.$video[0].parentNode===f[0]&&c.$video.detach(),f.remove()),c.ytReady=!0,c._updateYoutubeSize(),c.$video.trigger("canplay")},onStateChange:function(a){switch(a.data){case d.PlayerState.PLAYING:c.$video.trigger("playing");break;case d.PlayerState.ENDED:c.$video.trigger("ended");break;case d.PlayerState.PAUSED:c.$video.trigger("pause");break;case d.PlayerState.BUFFERING:c.$video.trigger("waiting");break;case d.PlayerState.CUED:c.$video.trigger("canplay")}},onPlaybackQualityChange:function(){c._updateYoutubeSize(),c.$video.trigger("resize")},onError:function(a){c.hasError=!0,c.$video.trigger({type:"error",error:a})}}});return c.ytPlayer=g,c},p.prototype._updateYoutubeSize=function(){var a=this;switch(a.ytPlayer.getPlaybackQuality()||"medium"){case"small":a.video.videoWidth=426,a.video.videoHeight=240;break;case"medium":a.video.videoWidth=640,a.video.videoHeight=360;break;default:case"large":a.video.videoWidth=854,a.video.videoHeight=480;break;case"hd720":a.video.videoWidth=1280,a.video.videoHeight=720;break;case"hd1080":a.video.videoWidth=1920,a.video.videoHeight=1080;break;case"highres":a.video.videoWidth=2560,a.video.videoHeight=1440}return a},p.prototype.play=function(){var a=this;return a.__manuallyStopped=!1,"youtube"===a.type?a.ytReady&&(a.$video.trigger("play"),a.ytPlayer.playVideo()):a.video.play(),a},p.prototype.pause=function(){var a=this;return a.__manuallyStopped=!1,"youtube"===a.type?a.ytReady&&a.ytPlayer.pauseVideo():a.video.pause(),a},p.prototype.stop=function(){var a=this;return a.__manuallyStopped=!0,"youtube"===a.type?a.ytReady&&(a.ytPlayer.pauseVideo(),a.ytPlayer.seekTo(0)):(a.video.pause(),a.video.currentTime=0),a},p.prototype.destroy=function(){var a=this;return a.ytPlayer&&a.ytPlayer.destroy(),a.$video.remove(),a},p.prototype.getCurrentTime=function(a){var b=this;return"youtube"!==b.type?b.video.currentTime:b.ytReady?b.ytPlayer.getCurrentTime():0},p.prototype.setCurrentTime=function(a){var b=this;return"youtube"===b.type?b.ytReady&&b.ytPlayer.seekTo(a,!0):b.video.currentTime=a,b},p.prototype.getDuration=function(){var a=this;return"youtube"!==a.type?a.video.duration:a.ytReady?a.ytPlayer.getDuration():0},p.loadYoutubeAPI=function(){b.YT&&b.__yt_load_event_interval__||(b.YT||a("script[src*=www\\.youtube\\.com\\/iframe_api]").length||a('<script type="text/javascript" src="https://www.youtube.com/iframe_api">').appendTo("body"),b.__yt_load_event_interval__=setInterval(function(){b.YT&&b.YT.loaded&&(a(b).trigger("youtube_api_load"),clearTimeout(b.__yt_load_event_interval__),delete b.__yt_load_event_interval__)},50))};var q=function(){if("matchMedia"in b){if(b.matchMedia("(orientation: portrait)").matches)return"portrait";if(b.matchMedia("(orientation: landscape)").matches)return"landscape"}return screen.height>screen.width?"portrait":"landscape"},r=function(){return b.innerHeight>b.innerWidth?"portrait":b.innerWidth>b.innerHeight?"landscape":"square"},s=function(){var a=navigator.userAgent,c=navigator.platform,d=a.match(/AppleWebKit\/([0-9]+)/),e=!!d&&d[1],f=a.match(/Fennec\/([0-9]+)/),g=!!f&&f[1],h=a.match(/Opera Mobi\/([0-9]+)/),i=!!h&&h[1],j=a.match(/MSIE ([0-9]+)/),k=!!j&&j[1];return!((c.indexOf("iPhone")>-1||c.indexOf("iPad")>-1||c.indexOf("iPod")>-1)&&e&&e<534||b.operamini&&"[object OperaMini]"==={}.toString.call(b.operamini)||h&&i<7458||a.indexOf("Android")>-1&&e&&e<533||g&&g<6||"palmGetResource"in b&&e&&e<534||a.indexOf("MeeGo")>-1&&a.indexOf("NokiaBrowser/8.5.0")>-1||k&&k<=6)}()}(jQuery,window);;
jQuery(document).ready(function($){
  $(".alert__closer").click(function() {
    $(".alert__closer").closest('div.full-width').remove();
  });
});
;
$(function () {
  $('[data-container="tabs"]').each(function () {
      var $this = $(this);

      $this.find('[data-section="tabs"]').eq(0).addClass('is-active');
      $this.find('[data-action="tabs"]').eq(0).addClass('is-active');
  });

  $(document).on('click', '[data-action="tabs"]', function () {
      var $action = $(this),
          $container = $action.closest('[data-container="tabs"]'),
          $section = $('[data-section="tabs"]', $container);

      $action.addClass('is-active').siblings('[data-action="tabs"]').removeClass('is-active');
      $section.removeClass('is-active').siblings('[data-id="' + $action.data('type') + '"]').addClass('is-active');
  });
});
;
jQuery(document).ready(function($){
    if($('.event__view-horizontal-card-content').length){
        $('.event__view-horizontal-card-content').each(function(i) {
            var event_view_result_height = $(this).height();
            $(this).prev('a.event__view-horizontal-card-img-link').find('div.event__view-horizontal-card-img').css({height : 'calc(' + event_view_result_height + 'px' + ' + ' + '2em' + ')'});
        });
    };

    Drupal.behaviors.myBehavior = {
        attach: function (context, settings) {
            if($('.event__view-horizontal-card-content').length){
                $('.event__view-horizontal-card-content').each(function(i) {
                    var event_view_result_height = $(this).height();
                    $(this).prev('a.event__view-horizontal-card-img-link').find('div.event__view-horizontal-card-img').css({height : 'calc(' + event_view_result_height + 'px' + ' + ' + '2em' + ')'});
                });
            };
        }
    };
});
;
jQuery(document).ready(function($){
    if($('.person__view-horizontal-card-content').length){
        $('.person__view-horizontal-card-content').each(function(i) {
            var person_view_result_height = $(this).height();
            $(this).prev('a.person__view-horizontal-card-img-link').find('div.person__view-horizontal-card-img').css({height : 'calc(' + person_view_result_height + 'px' + ' + ' + '2em' + ')'});
        });
    };

    Drupal.behaviors.myBehavior = {
        attach: function (context, settings) {
            if($('.person__view-horizontal-card-content').length){
                $('.person__view-horizontal-card-content').each(function(i) {
                    var person_view_result_height = $(this).height();
                    $(this).prev('a.person__view-horizontal-card-img-link').find('div.person__view-horizontal-card-img').css({height : 'calc(' + person_view_result_height + 'px' + ' + ' + '2em' + ')'});
                });
            };
        }
    };
});
;
jQuery(document).ready(function ($) {
    $("div.layout--twocol-section").each(function(index) {
        if ($(this).children(":first").hasClass("layout__region--second") == true) {
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else {
            return false;
        }
    });

    $("div.layout--threecol-section").each(function(index) {
        if ($(this).children(":first").hasClass("layout__region--first") == true && $(this).children(":first").next().hasClass("layout__region--third") == true) {
            $(this).children(":first").after("<div class=\"layout__region layout__region--second\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--second") == true && $(this).children(":first").next().hasClass("layout__region--third") == true) {
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--third") == true  && $(this).children(":first").next().length == 0) {
            $(this).prepend("<div class=\"layout__region layout__region--second\" aria-hidden=\"true\"></div>");
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--second") == true && $(this).children(":first").next().length == 0) {
            $(this).children(":first").after("<div class=\"layout__region layout__region--third\" aria-hidden=\"true\"></div>");
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else {
            return false;
        }
    });

    $("div.layout--fourcol-section").each(function(index) {
        if ($(this).children(":first").hasClass("layout__region--second") == true && $(this).children(":first").next().hasClass("layout__region--third") == true && $(this).children(":first").next().next().hasClass("layout__region--fourth") == true) {
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--third") == true && $(this).children(":first").next().hasClass("layout__region--fourth") == true) {
            $(this).prepend("<div class=\"layout__region layout__region--second\" aria-hidden=\"true\"></div>");
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--fourth") == true && $(this).children(":first").next().length == 0) {
            $(this).prepend("<div class=\"layout__region layout__region--third\" aria-hidden=\"true\"></div>");
            $(this).prepend("<div class=\"layout__region layout__region--second\" aria-hidden=\"true\"></div>");
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--third") == true && $(this).children(":first").next().length == 0) {
            $(this).children(":first").after("<div class=\"layout__region layout__region--fourth\" aria-hidden=\"true\"></div>");
            $(this).prepend("<div class=\"layout__region layout__region--second\" aria-hidden=\"true\"></div>");
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--second") == true && $(this).children(":first").next().length == 0 && $(this).children(":first").next().next().length == 0) {
            $(this).children(":first").after("<div class=\"layout__region layout__region--fourth\" aria-hidden=\"true\"></div>");
            $(this).children(":first").after("<div class=\"layout__region layout__region--third\" aria-hidden=\"true\"></div>");
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--second") == true && $(this).children(":first").next().hasClass("layout__region--fourth") == true) {
            $(this).children(":first").after("<div class=\"layout__region layout__region--third\" aria-hidden=\"true\"></div>");
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--second") == true && $(this).children(":first").next().hasClass("layout__region--third") == true) {
            $(this).children(":first").next().after("<div class=\"layout__region layout__region--third\" aria-hidden=\"true\"></div>");
            $(this).prepend("<div class=\"layout__region layout__region--first\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--first") == true && $(this).children(":first").next().hasClass("layout__region--fourth") == true) {
            $(this).children(":first").after("<div class=\"layout__region layout__region--third\" aria-hidden=\"true\"></div>");
            $(this).children(":first").after("<div class=\"layout__region layout__region--second\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--first") == true && $(this).children(":first").next().hasClass("layout__region--third") == true && $(this).children(":first").next().next().length == 0) {
            $(this).children(":first").next().after("<div class=\"layout__region layout__region--fourth\" aria-hidden=\"true\"></div>");
            $(this).children(":first").after("<div class=\"layout__region layout__region--second\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--first") == true && $(this).children(":first").next().hasClass("layout__region--third") == true && $(this).children(":first").next().next().hasClass("layout__region--fourth") == true) {
            $(this).children(":first").after("<div class=\"layout__region layout__region--second\" aria-hidden=\"true\"></div>");
        }

        else if ($(this).children(":first").hasClass("layout__region--first") == true && $(this).children(":first").next().hasClass("layout__region--second") == true && $(this).children(":first").next().next().hasClass("layout__region--fourth") == true) {
            $(this).children(":first").next().after("<div class=\"layout__region layout__region--third\" aria-hidden=\"true\"></div>");
        }

        else {
            return false;
        }
    });
});
;
(function ($, Drupal) {

    orig_allowInteraction = $.ui.dialog.prototype._allowInteraction;
    $.ui.dialog.prototype._allowInteraction = function(event) {
       if ($(event.target).closest('.cke_dialog').length) {
          return true;
       }
       return orig_allowInteraction.apply(this, arguments);
    };
  
  })(jQuery, Drupal);;
/**
 * @file
 * Dropbutton feature.
 */

(function ($, Drupal) {

  'use strict';

  /**
   * Process elements with the .dropbutton class on page load.
   *
   * @type {Drupal~behavior}
   *
   * @prop {Drupal~behaviorAttach} attach
   *   Attaches dropButton behaviors.
   */
  Drupal.behaviors.dropButton = {
    attach: function (context, settings) {
      var $dropbuttons = $(context).find('.js-dropbutton').once('dropbutton');
      if ($dropbuttons.length) {
        // Adds the delegated handler that will toggle dropdowns on click.
        var $body = $('body').once('dropbutton-click');
        if ($body.length) {
          $body.on('click', '.dropbutton__toggle', dropbuttonClickHandler);
        }
        // Initialize all buttons.
        var il = $dropbuttons.length;
        for (var i = 0; i < il; i++) {
          DropButton.dropbuttons.push(new DropButton($dropbuttons[i], settings.dropbutton));
        }
      }
    }
  };

  /**
   * Delegated callback for opening and closing dropbutton secondary actions.
   *
   * @function Drupal.DropButton~dropbuttonClickHandler
   *
   * @param {jQuery.Event} e
   *   The event triggered.
   */
  function dropbuttonClickHandler(e) {
    e.preventDefault();
    $(e.target).closest('.js-dropbutton').toggleClass('is-open');
  }

  /**
   * A DropButton presents an HTML list as a button with a primary action.
   *
   * All secondary actions beyond the first in the list are presented in a
   * dropdown list accessible through a toggle arrow associated with the button.
   *
   * @constructor Drupal.DropButton
   *
   * @param {HTMLElement} dropbutton
   *   A DOM element.
   * @param {object} settings
   *   A list of options including:
   * @param {string} settings.title
   *   The text inside the toggle link element. This text is hidden
   *   from visual UAs.
   */
  function DropButton(dropbutton, settings) {
    // Merge defaults with settings.
    var options = $.extend({title: Drupal.t('List additional actions')}, settings);
    var $dropbutton = $(dropbutton);

    /**
     * @type {jQuery}
     */
    this.$dropbutton = $dropbutton;

    /**
     * @type {jQuery}
     */
    this.$list = $dropbutton.find('.js-dropbutton-list');

    /**
     * Find actions and mark them.
     *
     * @type {jQuery}
     */
    this.$actions = this.$list.find('li');

    // Add the special dropdown only if there are hidden actions.
    if (this.$actions.length > 1) {
      // Identify the first element of the collection.
      var $primary = this.$actions.slice(0, 1);
      $primary.addClass('is-action');
      // Identify the secondary actions.
      var $secondary = this.$actions.slice(1);
      $secondary.addClass('is-secondary-action');
      // Add toggle link.
      $primary.after(Drupal.theme('dropbuttonToggle', options));
      // Bind mouse events.
      this.$dropbutton
        .addClass('has-multiple')
        .on({

          /**
           * Adds a timeout to close the dropdown on mouseleave.
           *
           * @ignore
           */
          'mouseleave.dropbutton': $.proxy(this.hoverOut, this),

          /**
           * Clears timeout when mouseout of the dropdown.
           *
           * @ignore
           */
          'mouseenter.dropbutton': $.proxy(this.hoverIn, this),

          /**
           * Similar to mouseleave/mouseenter, but for keyboard navigation.
           *
           * @ignore
           */
          'focusout.dropbutton': $.proxy(this.focusOut, this),

          /**
           * @ignore
           */
          'focusin.dropbutton': $.proxy(this.focusIn, this)
        });
    }
    else {
      this.$dropbutton.addClass('is-single');
    }
  }

  /**
   * Extend the DropButton constructor.
   */
  $.extend(DropButton, /** @lends Drupal.DropButton */{
    /**
     * Store all processed DropButtons.
     *
     * @type {Array.<Drupal.DropButton>}
     */
    dropbuttons: []
  });

  /**
   * Extend the DropButton prototype.
   */
  $.extend(DropButton.prototype, /** @lends Drupal.DropButton# */{

    /**
     * Toggle the dropbutton open and closed.
     *
     * @param {bool} [show]
     *   Force the dropbutton to open by passing true or to close by
     *   passing false.
     */
    toggle: function (show) {
      var isBool = typeof show === 'boolean';
      show = isBool ? show : !this.$dropbutton.hasClass('is-open');
      this.$dropbutton.toggleClass('is-open', show);
    },

    /**
     * @method
     */
    hoverIn: function () {
      // Clear any previous timer we were using.
      if (this.timerID) {
        window.clearTimeout(this.timerID);
      }
    },

    /**
     * @method
     */
    hoverOut: function () {
      // Wait half a second before closing.
      this.timerID = window.setTimeout($.proxy(this, 'close'), 500);
    },

    /**
     * @method
     */
    open: function () {
      this.toggle(true);
    },

    /**
     * @method
     */
    close: function () {
      this.toggle(false);
    },

    /**
     * @param {jQuery.Event} e
     *   The event triggered.
     */
    focusOut: function (e) {
      this.hoverOut.call(this, e);
    },

    /**
     * @param {jQuery.Event} e
     *   The event triggered.
     */
    focusIn: function (e) {
      this.hoverIn.call(this, e);
    }
  });

  $.extend(Drupal.theme, /** @lends Drupal.theme */{

    /**
     * A toggle is an interactive element often bound to a click handler.
     *
     * @param {object} options
     *   Options object.
     * @param {string} [options.title]
     *   The HTML anchor title attribute and text for the inner span element.
     *
     * @return {string}
     *   A string representing a DOM fragment.
     */
    dropbuttonToggle: function (options) {
      return '<li class="dropbutton__toggle"><button class="dropbutton__toggle-button" type="button"><span class="visually-hidden">' + options.title + '</span></button></li>';
    }
  });

  // Expose constructor in the public space.
  Drupal.DropButton = DropButton;

})(jQuery, Drupal);
;
