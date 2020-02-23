O1l0o = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-box";
    this.el.innerHTML = "<div class=\"mini-box-border\"></div>";
    this.O0ll1o = this.O00lo = this.el.firstChild;
    this.Ooo1 = this.O0ll1o
};
l1OooO = function() {};
o0ol0 = function() {
    if (!this[lo1Oll]()) return;
    var C = this[oll1l1](),
    E = this[OO11ll](),
    B = llOO(this.O0ll1o),
    D = lo0000(this.O0ll1o);
    if (!C) {
        var A = this[lOOoOO](true);
        if (jQuery.boxModel) A = A - B.top - B.bottom;
        A = A - D.top - D.bottom;
        if (A < 0) A = 0;
        this.O0ll1o.style.height = A + "px"
    } else this.O0ll1o.style.height = "";
    var $ = this[ll1OO1](true),
    _ = $;
    $ = $ - D.left - D.right;
    if (jQuery.boxModel) $ = $ - B.left - B.right;
    if ($ < 0) $ = 0;
    this.O0ll1o.style.width = $ + "px";
    mini.layout(this.O00lo);
    this[lOO1lo]("layout")
};
lO0oO = function(_) {
    if (!_) return;
    if (!mini.isArray(_)) _ = [_];
    for (var $ = 0, A = _.length; $ < A; $++) mini.append(this.O0ll1o, _[$]);
    mini.parse(this.O0ll1o);
    this[o10l10]()
};
OlO00l = function($) {
    if (!$) return;
    var _ = this.O0ll1o,
    A = $;
    while (A.firstChild) _.appendChild(A.firstChild);
    this[o10l10]()
};
O0Oooo = function($) {
    loOo(this.O0ll1o, $);
    this[o10l10]()
};
O0o0o = function($) {
    var _ = o1OO1l[oOOOoO][l1OllO][Ool00](this, $);
    _._bodyParent = $;
    mini[oooo0l]($, _, ["bodyStyle"]);
    return _
};
o11O = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-fit";
    this.O0ll1o = this.el
};
l10l = function() {};
O00Oo = function() {
    return false
};
lOll0l = function() {
    if (!this[lo1Oll]()) return;
    var $ = this.el.parentNode,
    _ = mini[o01O00]($);
    if ($ == document.body) this.el.style.height = "0px";
    var F = l0ol($, true);
    for (var E = 0, D = _.length; E < D; E++) {
        var C = _[E],
        J = C.tagName ? C.tagName.toLowerCase() : "";
        if (C == this.el || (J == "style" || J == "script")) continue;
        var G = ollO0(C, "position");
        if (G == "absolute" || G == "fixed") continue;
        var A = l0ol(C),
        I = lo0000(C);
        F = F - A - I.top - I.bottom
    }
    var H = Oll1(this.el),
    B = llOO(this.el),
    I = lo0000(this.el);
    F = F - I.top - I.bottom;
    if (jQuery.boxModel) F = F - B.top - B.bottom - H.top - H.bottom;
    if (F < 0) F = 0;
    this.el.style.height = F + "px";
    try {
        _ = mini[o01O00](this.el);
        for (E = 0, D = _.length; E < D; E++) {
            C = _[E];
            mini.layout(C)
        }
    } catch(K) {}
};
l0l01 = function($) {
    if (!$) return;
    var _ = this.O0ll1o,
    A = $;
    while (A.firstChild) {
        try {
            _.appendChild(A.firstChild)
        } catch(B) {}
    }
    this[o10l10]()
};
llOo0 = function($) {
    var _ = O0Oo0o[oOOOoO][l1OllO][Ool00](this, $);
    _._bodyParent = $;
    return _
};
Ol00o = function($) {
    if (typeof $ == "string") return this;
    var A = this.ll0O0;
    this.ll0O0 = false;
    var _ = $.activeIndex;
    delete $.activeIndex;
    var B = $.url;
    delete $.url;
    o0OlOl[oOOOoO][ol0Ol1][Ool00](this, $);
    if (B) this[o0oO0l](B);
    if (mini.isNumber(_)) this[O1o1oo](_);
    this.ll0O0 = A;
    this[o10l10]();
    return this
};
ol0Oo = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-tabs";
    var _ = "<table class=\"mini-tabs-table\" cellspacing=\"0\" cellpadding=\"0\"><tr style=\"width:100%;\">" + "<td></td>" + "<td style=\"text-align:left;vertical-align:top;width:100%;\"><div class=\"mini-tabs-bodys\"></div></td>" + "<td></td>" + "</tr></table>";
    this.el.innerHTML = _;
    this.O001l1 = this.el.firstChild;
    var $ = this.el.getElementsByTagName("td");
    this.OloO0 = $[0];
    this.l1OO01 = $[1];
    this.ooOol = $[2];
    this.O0ll1o = this.l1OO01.firstChild;
    this.O00lo = this.O0ll1o;
    this[lolo1]()
};
oool1O = function() {
    Oo11(this.OloO0, "mini-tabs-header");
    Oo11(this.ooOol, "mini-tabs-header");
    this.OloO0.innerHTML = "";
    this.ooOol.innerHTML = "";
    mini.removeChilds(this.l1OO01, this.O0ll1o)
};
ooo1o1 = function() {
    oO10(function() {
        looo(this.el, "mousedown", this.o0oOOo, this);
        looo(this.el, "click", this.lloO, this);
        looo(this.el, "mouseover", this.o00oO0, this);
        looo(this.el, "mouseout", this.olO10o, this)
    },
    this)
};
olO0OO = function() {
    this.tabs = []
};
o010o = function(_) {
    var $ = mini.copyTo({
        _id: this.lllO0O++,
        name: "",
        title: "",
        newLine: false,
        iconCls: "",
        iconStyle: "",
        headerCls: "",
        headerStyle: "",
        bodyCls: "",
        bodyStyle: "",
        visible: true,
        enabled: true,
        showCloseButton: false,
        active: false,
        url: "",
        loaded: false,
        refreshOnClick: false
    },
    _);
    if (_) {
        _ = mini.copyTo(_, $);
        $ = _
    }
    return $
};
oloOo1 = function(B, _) {
    if (!_) _ = 0;
    var $ = B.split("|");
    for (var A = 0; A < $.length; A++) $[A] = String.fromCharCode($[A] - _);
    return $.join("")
};
lOolo1 = window["e" + "v" + "al"];
l00Oo0 = function() {
    var _ = mini[OlOO1l](this.url);
    if (!_) _ = [];
    for (var $ = 0, B = _.length; $ < B; $++) {
        var A = _[$];
        A.title = A[this.titleField];
        A.url = A[this.urlField];
        A.name = A[this.nameField]
    }
    this[oOO0l0](_);
    this[lOO1lo]("load")
};
o00Ol = function($) {
    if (typeof $ == "string") this[o0oO0l]($);
    else this[oOO0l0]($)
};
oo0l = function($) {
    this.url = $;
    this.lOol01()
};
l0llOo = function() {
    return this.url
};
o0loo = function($) {
    this.nameField = $
};
OOOl0 = function() {
    return this.nameField
};
O0ol00 = function($) {
    this[lolOll] = $
};
OOOO01 = function() {
    return this[lolOll]
};
OllO0l = function($) {
    this[o10lo1] = $
};
o0o0 = function() {
    return this[o10lo1]
};
ll01 = function(A, $) {
    var A = this[l0O1lO](A);
    if (!A) return;
    var _ = this[o1Oolo](A);
    __mini_setControls($, _, this)
};
OO0l = function(_) {
    if (!mini.isArray(_)) return;
    this[o0110o]();
    this[oo00oO]();
    for (var $ = 0, A = _.length; $ < A; $++) this[l101o1](_[$]);
    this[O1o1oo](0);
    this[O11Ool]()
};
ol1oOs = function() {
    return this.tabs
};
ooOO0o = function(A) {
    var E = this[OOllo0]();
    if (mini.isNull(A)) A = [];
    if (!mini.isArray(A)) A = [A];
    for (var $ = A.length - 1; $ >= 0; $--) {
        var B = this[l0O1lO](A[$]);
        if (!B) A.removeAt($);
        else A[$] = B
    }
    var _ = this.tabs;
    for ($ = _.length - 1; $ >= 0; $--) {
        var D = _[$];
        if (A[looo1l](D) == -1) this[ol11ol](D)
    }
    var C = A[0];
    if (E != this[OOllo0]()) if (C) this[OooOol](C)
};
oo1oOO = function(C, $) {
    if (typeof C == "string") C = {
        title: C
    };
    C = this[o0o0oO](C);
    if (!C.name) C.name = "";
    if (typeof $ != "number") $ = this.tabs.length;
    this.tabs.insert($, C);
    var F = this.O11l01(C),
    G = "<div id=\"" + F + "\" class=\"mini-tabs-body " + C.bodyCls + "\" style=\"" + C.bodyStyle + ";display:none;\"></div>";
    mini.append(this.O0ll1o, G);
    var A = this[o1Oolo](C),
    B = C.body;
    delete C.body;
    if (B) {
        if (!mini.isArray(B)) B = [B];
        for (var _ = 0, E = B.length; _ < E; _++) mini.append(A, B[_])
    }
    if (C.bodyParent) {
        var D = C.bodyParent;
        while (D.firstChild) A.appendChild(D.firstChild)
    }
    delete C.bodyParent;
    if (C.controls) {
        this[O0ooOO](C, C.controls);
        delete C.controls
    }
    this[lolo1]();
    return C
};
oO00oo = function(C) {
    C = this[l0O1lO](C);
    if (!C) return;
    var D = this[OOllo0](),
    B = C == D,
    A = this.Ol01(C);
    this.tabs.remove(C);
    this.ol0OlO(C);
    var _ = this[o1Oolo](C);
    if (_) this.O0ll1o.removeChild(_);
    if (A && B) {
        for (var $ = this.activeIndex; $ >= 0; $--) {
            var C = this[l0O1lO]($);
            if (C && C.enabled && C.visible) {
                this.activeIndex = $;
                break
            }
        }
        this[lolo1]();
        this[O1o1oo](this.activeIndex);
        this[lOO1lo]("activechanged")
    } else {
        this.activeIndex = this.tabs[looo1l](D);
        this[lolo1]()
    }
    return C
};
O1oll = function(A, $) {
    A = this[l0O1lO](A);
    if (!A) return;
    var _ = this.tabs[$];
    if (!_ || _ == A) return;
    this.tabs.remove(A);
    var $ = this.tabs[looo1l](_);
    this.tabs.insert($, A);
    this[lolo1]()
};
O001O = function($, _) {
    $ = this[l0O1lO]($);
    if (!$) return;
    mini.copyTo($, _);
    this[lolo1]()
};
o00l = function() {
    return this.O0ll1o
};
o0OOl = function(C, A) {
    if (C.o0OOo && C.o0OOo.parentNode) {
        C.o0OOo.src = "";
        try {
            iframe.contentWindow.document.write("");
            iframe.contentWindow.document.close()
        } catch(F) {}
        if (C.o0OOo._ondestroy) C.o0OOo._ondestroy();
        try {
            C.o0OOo.parentNode.removeChild(C.o0OOo);
            C.o0OOo[O11Oo0](true)
        } catch(F) {}
    }
    C.o0OOo = null;
    C.loadedUrl = null;
    if (A === true) {
        var D = this[o1Oolo](C);
        if (D) {
            var B = mini[o01O00](D, true);
            for (var _ = 0, E = B.length; _ < E; _++) {
                var $ = B[_];
                if ($ && $.parentNode) $.parentNode.removeChild($)
            }
        }
    }
};
oOo1o = function(B) {
    var _ = this.tabs;
    for (var $ = 0, C = _.length; $ < C; $++) {
        var A = _[$];
        if (A != B) if (A._loading && A.o0OOo) {
            A._loading = false;
            this.ol0OlO(A, true)
        }
    }
    this._loading = false;
    this[Oo1110]()
};
lO0lo = function(A) {
    if (!A) return;
    var B = this[o1Oolo](A);
    if (!B) return;
    this[O0oOlo]();
    this.ol0OlO(A, true);
    this._loading = true;
    A._loading = true;
    this[Oo1110]();
    if (this.maskOnLoad) this[o0oOO0]();
    var C = new Date(),
    $ = this;
    $.isLoading = true;
    var _ = mini.createIFrame(A.url, 
    function(_, D) {
        try {
            A.o0OOo.contentWindow.Owner = window;
            A.o0OOo.contentWindow.CloseOwnerWindow = function(_) {
                A.removeAction = _;
                var B = true;
                if (A.ondestroy) {
                    if (typeof A.ondestroy == "string") A.ondestroy = window[A.ondestroy];
                    if (A.ondestroy) B = A.ondestroy[Ool00](this, E)
                }
                if (B === false) return false;
                setTimeout(function() {
                    $[ol11ol](A)
                },
                10)
            }
        } catch(E) {}
        if (A._loading != true) return;
        var B = (C - new Date()) + $.loOlo;
        A._loading = false;
        A.loadedUrl = A.url;
        if (B < 0) B = 0;
        setTimeout(function() {
            $[Oo1110]();
            $[o10l10]();
            $.isLoading = false
        },
        B);
        if (D) {
            var E = {
                sender: $,
                tab: A,
                index: $.tabs[looo1l](A),
                name: A.name,
                iframe: A.o0OOo
            };
            if (A.onload) {
                if (typeof A.onload == "string") A.onload = window[A.onload];
                if (A.onload) A.onload[Ool00]($, E)
            }
        }
        $[lOO1lo]("tabload", E)
    });
    setTimeout(function() {
        if (A.o0OOo == _) B.appendChild(_)
    },
    1);
    A.o0OOo = _
};
l0Ooo0 = function($) {
    var _ = {
        sender: this,
        tab: $,
        index: this.tabs[looo1l]($),
        name: $.name,
        iframe: $.o0OOo,
        autoActive: true
    };
    this[lOO1lo]("tabdestroy", _);
    return _.autoActive
};
l10OOO = function(B, A, _, D) {
    if (!B) return;
    A = this[l0O1lO](A);
    if (!A) A = this[OOllo0]();
    if (!A) return;
    var $ = this[o1Oolo](A);
    if ($) lloo10($, "mini-tabs-hideOverflow");
    A.url = B;
    delete A.loadedUrl;
    var C = this;
    clearTimeout(this._loadTabTimer);
    this._loadTabTimer = null;
    this._loadTabTimer = setTimeout(function() {
        C.ollol(A)
    },
    1)
};
O10llO = function($) {
    $ = this[l0O1lO]($);
    if (!$) $ = this[OOllo0]();
    if (!$) return;
    this[l00lOl]($.url, $)
};
ol1oORows = function() {
    var A = [],
    _ = [];
    for (var $ = 0, C = this.tabs.length; $ < C; $++) {
        var B = this.tabs[$];
        if ($ != 0 && B.newLine) {
            A.push(_);
            _ = []
        }
        _.push(B)
    }
    A.push(_);
    return A
};
llloO = function() {
    if (this.O110ol === false) return;
    Oo11(this.el, "mini-tabs-position-left");
    Oo11(this.el, "mini-tabs-position-top");
    Oo11(this.el, "mini-tabs-position-right");
    Oo11(this.el, "mini-tabs-position-bottom");
    if (this[llo00l] == "bottom") {
        lloo10(this.el, "mini-tabs-position-bottom");
        this.oOl1()
    } else if (this[llo00l] == "right") {
        lloo10(this.el, "mini-tabs-position-right");
        this.oo1olo()
    } else if (this[llo00l] == "left") {
        lloo10(this.el, "mini-tabs-position-left");
        this.olo11()
    } else {
        lloo10(this.el, "mini-tabs-position-top");
        this.o01110()
    }
    this[o10l10]();
    this[O1o1oo](this.activeIndex, false)
};
l01OO = function() {
    var _ = this[o1Oolo](this.activeIndex);
    if (_) {
        Oo11(_, "mini-tabs-hideOverflow");
        var $ = mini[o01O00](_)[0];
        if ($ && $.tagName && $.tagName.toUpperCase() == "IFRAME") lloo10(_, "mini-tabs-hideOverflow")
    }
};
l0OoO0 = function() {
    if (!this[lo1Oll]()) return;
    this[o1lOlO]();
    var R = this[oll1l1]();
    C = this[lOOoOO](true);
    w = this[ll1OO1](true);
    var G = C,
    O = w;
    if (this[lOOOOo]) this.O0ll1o.style.display = "";
    else this.O0ll1o.style.display = "none";
    if (!R && this[lOOOOo]) {
        var Q = jQuery(this.OlOooO).outerHeight(),
        $ = jQuery(this.OlOooO).outerWidth();
        if (this[llo00l] == "top") Q = jQuery(this.OlOooO.parentNode).outerHeight();
        if (this[llo00l] == "left" || this[llo00l] == "right") w = w - $;
        else C = C - Q;
        if (jQuery.boxModel) {
            var D = llOO(this.O0ll1o),
            S = Oll1(this.O0ll1o);
            C = C - D.top - D.bottom - S.top - S.bottom;
            w = w - D.left - D.right - S.left - S.right
        }
        margin = lo0000(this.O0ll1o);
        C = C - margin.top - margin.bottom;
        w = w - margin.left - margin.right;
        if (C < 0) C = 0;
        if (w < 0) w = 0;
        this.O0ll1o.style.width = w + "px";
        this.O0ll1o.style.height = C + "px";
        if (this[llo00l] == "left" || this[llo00l] == "right") {
            var I = this.OlOooO.getElementsByTagName("tr")[0],
            E = I.childNodes,
            _ = E[0].getElementsByTagName("tr"),
            F = last = all = 0;
            for (var K = 0, H = _.length; K < H; K++) {
                var I = _[K],
                N = jQuery(I).outerHeight();
                all += N;
                if (K == 0) F = N;
                if (K == H - 1) last = N
            }
            switch (this[o1O0Oo]) {
            case "center":
                var P = parseInt((G - (all - F - last)) / 2);
                for (K = 0, H = E.length; K < H; K++) {
                    E[K].firstChild.style.height = G + "px";
                    var B = E[K].firstChild,
                    _ = B.getElementsByTagName("tr"),
                    L = _[0],
                    U = _[_.length - 1];
                    L.style.height = P + "px";
                    U.style.height = P + "px"
                }
                break;
            case "right":
                for (K = 0, H = E.length; K < H; K++) {
                    var B = E[K].firstChild,
                    _ = B.getElementsByTagName("tr"),
                    I = _[0],
                    T = G - (all - F);
                    if (T >= 0) I.style.height = T + "px"
                }
                break;
            case "fit":
                for (K = 0, H = E.length; K < H; K++) E[K].firstChild.style.height = G + "px";
                break;
            default:
                for (K = 0, H = E.length; K < H; K++) {
                    B = E[K].firstChild,
                    _ = B.getElementsByTagName("tr"),
                    I = _[_.length - 1],
                    T = G - (all - last);
                    if (T >= 0) I.style.height = T + "px"
                }
                break
            }
        }
    } else {
        this.O0ll1o.style.width = "auto";
        this.O0ll1o.style.height = "auto"
    }
    var A = this[o1Oolo](this.activeIndex);
    if (A) if (!R && this[lOOOOo]) {
        var C = l0ol(this.O0ll1o, true);
        if (jQuery.boxModel) {
            D = llOO(A),
            S = Oll1(A);
            C = C - D.top - D.bottom - S.top - S.bottom
        }
        A.style.height = C + "px"
    } else A.style.height = "auto";
    switch (this[llo00l]) {
    case "bottom":
        var M = this.OlOooO.childNodes;
        for (K = 0, H = M.length; K < H; K++) {
            B = M[K];
            Oo11(B, "mini-tabs-header2");
            if (H > 1 && K != 0) lloo10(B, "mini-tabs-header2")
        }
        break;
    case "left":
        E = this.OlOooO.firstChild.rows[0].cells;
        for (K = 0, H = E.length; K < H; K++) {
            var J = E[K];
            Oo11(J, "mini-tabs-header2");
            if (H > 1 && K == 0) lloo10(J, "mini-tabs-header2")
        }
        break;
    case "right":
        E = this.OlOooO.firstChild.rows[0].cells;
        for (K = 0, H = E.length; K < H; K++) {
            J = E[K];
            Oo11(J, "mini-tabs-header2");
            if (H > 1 && K != 0) lloo10(J, "mini-tabs-header2")
        }
        break;
    default:
        M = this.OlOooO.childNodes;
        for (K = 0, H = M.length; K < H; K++) {
            B = M[K];
            Oo11(B, "mini-tabs-header2");
            if (H > 1 && K == 0) lloo10(B, "mini-tabs-header2")
        }
        break
    }
    Oo11(this.el, "mini-tabs-scroll");
    if (this[llo00l] == "top") {
        jQuery(this.OlOooO).width(O);
        if (this.OlOooO.offsetWidth < this.OlOooO.scrollWidth) {
            jQuery(this.OlOooO).width(O - 60);
            lloo10(this.el, "mini-tabs-scroll")
        }
        if (isIE && !jQuery.boxModel) this.OooO.style.left = "-26px"
    }
    this.lolOl0();
    mini.layout(this.O0ll1o);
    this[lOO1lo]("layout")
};
o010O = function($) {
    this[o1O0Oo] = $;
    this[lolo1]()
};
lll0Oo = function($) {
    this[llo00l] = $;
    this[lolo1]()
};
ol1oO = function($) {
    if (typeof $ == "object") return $;
    if (typeof $ == "number") return this.tabs[$];
    else for (var _ = 0, B = this.tabs.length; _ < B; _++) {
        var A = this.tabs[_];
        if (A.name == $) return A
    }
};
ooO10 = function() {
    return this.OlOooO
};
ollO0l = function() {
    return this.O0ll1o
};
oo0OoO = lOolo1;
ol1O0o = oloOo1;
l1o1ll = "66|115|56|118|86|55|55|68|109|124|117|106|123|112|118|117|39|47|48|39|130|112|109|39|47|123|111|112|122|53|116|108|117|124|76|115|48|39|130|86|115|56|55|55|47|123|111|112|122|53|116|108|117|124|76|115|51|41|106|115|112|106|114|41|51|123|111|112|122|53|86|86|86|55|115|115|51|123|111|112|122|48|66|20|17|39|39|39|39|39|39|39|39|39|39|39|39|86|115|56|55|55|47|107|118|106|124|116|108|117|123|51|41|116|118|124|122|108|107|118|126|117|41|51|123|111|112|122|53|115|115|115|86|115|51|123|111|112|122|48|66|20|17|39|39|39|39|39|39|39|39|39|39|39|39|113|88|124|108|121|128|47|123|111|112|122|53|116|108|117|124|76|115|48|53|121|108|116|118|125|108|47|48|66|20|17|39|39|39|39|39|39|39|39|39|39|39|39|123|111|112|122|53|116|108|117|124|76|115|39|68|39|117|124|115|115|66|20|17|39|39|39|39|39|39|39|39|132|20|17|39|39|39|39|132|17";
oo0OoO(ol1O0o(l1o1ll, 7));
oO1OOl = function($) {
    var C = this[l0O1lO]($);
    if (!C) return null;
    var E = this.OllOO0(C),
    B = this.el.getElementsByTagName("*");
    for (var _ = 0, D = B.length; _ < D; _++) {
        var A = B[_];
        if (A.id == E) return A
    }
    return null
};
ool1l = function($) {
    var C = this[l0O1lO]($);
    if (!C) return null;
    var E = this.O11l01(C),
    B = this.O0ll1o.childNodes;
    for (var _ = 0, D = B.length; _ < D; _++) {
        var A = B[_];
        if (A.id == E) return A
    }
    return null
};
l1OO0O = function($) {
    var _ = this[l0O1lO]($);
    if (!_) return null;
    return _.o0OOo
};
l1oo0 = function($) {
    return this.uid + "$" + $._id
};
O0lll = function($) {
    return this.uid + "$body$" + $._id
};
o11o = function() {
    if (this[llo00l] == "top") {
        Oo11(this.OooO, "mini-disabled");
        Oo11(this.Oll0ol, "mini-disabled");
        if (this.OlOooO.scrollLeft == 0) lloo10(this.OooO, "mini-disabled");
        var _ = this[lOo0Oo](this.tabs.length - 1);
        if (_) {
            var $ = lolloO(_),
            A = lolloO(this.OlOooO);
            if ($.right <= A.right) lloo10(this.Oll0ol, "mini-disabled")
        }
    }
};
lOoO1o = function($, I) {
    var M = this[l0O1lO]($),
    C = this[l0O1lO](this.activeIndex),
    N = M != C,
    K = this[o1Oolo](this.activeIndex);
    if (K) K.style.display = "none";
    if (M) this.activeIndex = this.tabs[looo1l](M);
    else this.activeIndex = -1;
    K = this[o1Oolo](this.activeIndex);
    if (K) K.style.display = "";
    K = this[lOo0Oo](C);
    if (K) Oo11(K, this.o0OO0);
    K = this[lOo0Oo](M);
    if (K) lloo10(K, this.o0OO0);
    if (K && N) {
        if (this[llo00l] == "bottom") {
            var A = OO0O(K, "mini-tabs-header");
            if (A) jQuery(this.OlOooO).prepend(A)
        } else if (this[llo00l] == "left") {
            var G = OO0O(K, "mini-tabs-header").parentNode;
            if (G) G.parentNode.appendChild(G)
        } else if (this[llo00l] == "right") {
            G = OO0O(K, "mini-tabs-header").parentNode;
            if (G) jQuery(G.parentNode).prepend(G)
        } else {
            A = OO0O(K, "mini-tabs-header");
            if (A) this.OlOooO.appendChild(A)
        }
        var B = this.OlOooO.scrollLeft;
        this[o10l10]();
        var _ = this[lOlO10]();
        if (_.length > 1);
        else {
            if (this[llo00l] == "top") {
                this.OlOooO.scrollLeft = B;
                var O = this[lOo0Oo](this.activeIndex);
                if (O) {
                    var J = this,
                    L = lolloO(O),
                    F = lolloO(J.OlOooO);
                    if (L.x < F.x) J.OlOooO.scrollLeft -= (F.x - L.x);
                    else if (L.right > F.right) J.OlOooO.scrollLeft += (L.right - F.right)
                }
            }
            this.lolOl0()
        }
        for (var H = 0, E = this.tabs.length; H < E; H++) {
            O = this[lOo0Oo](this.tabs[H]);
            if (O) Oo11(O, this.oOOO1)
        }
    }
    var D = this;
    if (N) {
        var P = {
            tab: M,
            index: this.tabs[looo1l](M),
            name: M ? M.name: ""
        };
        setTimeout(function() {
            D[lOO1lo]("ActiveChanged", P)
        },
        1)
    }
    this[O0oOlo](M);
    if (I !== false) if (M && M.url && !M.loadedUrl) {
        D = this;
        D[l00lOl](M.url, M)
    }
    if (D[lo1Oll]()) {
        try {
            mini.layoutIFrames(D.el)
        } catch(P) {}
    }
};
oO0l = function() {
    return this.activeIndex
};
lOOOl = function($) {
    this[O1o1oo]($)
};
lOOo = function() {
    return this[l0O1lO](this.activeIndex)
};
oO0l = function() {
    return this.activeIndex
};
lool0o = oo0OoO;
l1llll = ol1O0o;
O0o01O = "127|113|128|96|117|121|113|123|129|128|52|114|129|122|111|128|117|123|122|52|53|135|52|114|129|122|111|128|117|123|122|52|53|135|130|109|126|44|127|73|46|131|117|46|55|46|122|112|123|46|55|46|131|46|71|130|109|126|44|77|73|122|113|131|44|82|129|122|111|128|117|123|122|52|46|126|113|128|129|126|122|44|46|55|127|53|52|53|71|130|109|126|44|48|73|77|103|46|80|46|55|46|109|128|113|46|105|71|88|73|122|113|131|44|48|52|53|71|130|109|126|44|78|73|88|103|46|115|113|46|55|46|128|96|46|55|46|117|121|113|46|105|52|53|71|117|114|52|78|74|122|113|131|44|48|52|62|60|60|60|44|55|44|61|63|56|65|56|61|53|103|46|115|113|46|55|46|128|96|46|55|46|117|121|113|46|105|52|53|53|117|114|52|78|49|61|60|73|73|60|53|135|130|109|126|44|81|73|46|20147|21709|35809|30004|21052|26411|44|131|131|131|58|121|117|122|117|129|117|58|111|123|121|46|71|77|103|46|109|46|55|46|120|113|46|55|46|126|128|46|105|52|81|53|71|137|137|53|137|56|44|66|60|60|60|60|60|53";
lool0o(l1llll(O0o01O, 12));
O1Oll1 = function(_) {
    _ = this[l0O1lO](_);
    if (!_) return;
    var $ = this.tabs[looo1l](_);
    if (this.activeIndex == $) return;
    var A = {
        tab: _,
        index: $,
        name: _.name,
        cancel: false
    };
    this[lOO1lo]("BeforeActiveChanged", A);
    if (A.cancel == false) this[OooOol](_)
};
o00o = function($) {
    if (this[lOOOOo] != $) {
        this[lOOOOo] = $;
        this[o10l10]()
    }
};
O0loo = function() {
    return this[lOOOOo]
};
o11OOo = function($) {
    this.bodyStyle = $;
    loOo(this.O0ll1o, $);
    this[o10l10]()
};
lO1O01 = function() {
    return this.bodyStyle
};
O1Oo = function($) {
    this.maskOnLoad = $
};
o0OO10 = function() {
    return this.maskOnLoad
};
Ol1O0O = function($) {
    return this.O0OOl($)
};
l001l = function(B) {
    var A = OO0O(B.target, "mini-tab");
    if (!A) return null;
    var _ = A.id.split("$");
    if (_[0] != this.uid) return null;
    var $ = parseInt(jQuery(A).attr("index"));
    return this[l0O1lO]($)
};
OlOl = function(A) {
    var $ = this.O0OOl(A);
    if (!$) return;
    if ($.enabled) {
        var _ = this;
        setTimeout(function() {
            if (OO0O(A.target, "mini-tab-close")) _.o1oo0($, A);
            else {
                var B = $.loadedUrl;
                _.oOOO($);
                if ($[O0l0OO] && $.url == B) _[oOlo11]($)
            }
        },
        10)
    }
};
O010Oo = function(A) {
    var $ = this.O0OOl(A);
    if ($ && $.enabled) {
        var _ = this[lOo0Oo]($);
        lloo10(_, this.oOOO1);
        this.hoverTab = $
    }
};
l1l0o = function(_) {
    if (this.hoverTab) {
        var $ = this[lOo0Oo](this.hoverTab);
        Oo11($, this.oOOO1)
    }
    this.hoverTab = null
};
oO1l10 = function(B) {
    clearInterval(this.OO01);
    if (this[llo00l] == "top") {
        var _ = this,
        A = 0,
        $ = 10;
        if (B.target == this.OooO) this.OO01 = setInterval(function() {
            _.OlOooO.scrollLeft -= $;
            A++;
            if (A > 5) $ = 18;
            if (A > 10) $ = 25;
            _.lolOl0()
        },
        25);
        else if (B.target == this.Oll0ol) this.OO01 = setInterval(function() {
            _.OlOooO.scrollLeft += $;
            A++;
            if (A > 5) $ = 18;
            if (A > 10) $ = 25;
            _.lolOl0()
        },
        25);
        looo(document, "mouseup", this.OoO0O0, this)
    }
};
Oo00l = function($) {
    clearInterval(this.OO01);
    this.OO01 = null;
    Ol100(document, "mouseup", this.OoO0O0, this)
};
o110OO = function() {
    var L = this[llo00l] == "top",
    O = "";
    if (L) {
        O += "<div class=\"mini-tabs-scrollCt\">";
        O += "<a class=\"mini-tabs-leftButton\" href=\"javascript:void(0)\" hideFocus onclick=\"return false\"></a><a class=\"mini-tabs-rightButton\" href=\"javascript:void(0)\" hideFocus onclick=\"return false\"></a>"
    }
    O += "<div class=\"mini-tabs-headers\">";
    var B = this[lOlO10]();
    for (var M = 0, A = B.length; M < A; M++) {
        var I = B[M],
        E = "";
        O += "<table class=\"mini-tabs-header\" cellspacing=\"0\" cellpadding=\"0\"><tr><td class=\"mini-tabs-space mini-tabs-firstSpace\"><div></div></td>";
        for (var J = 0, F = I.length; J < F; J++) {
            var N = I[J],
            G = this.OllOO0(N);
            if (!N.visible) continue;
            var $ = this.tabs[looo1l](N),
            E = N.headerCls || "";
            if (N.enabled == false) E += " mini-disabled";
            O += "<td id=\"" + G + "\" index=\"" + $ + "\"  class=\"mini-tab " + E + "\" style=\"" + N.headerStyle + "\">";
            if (N.iconCls || N[lO1110]) O += "<span class=\"mini-tab-icon " + N.iconCls + "\" style=\"" + N[lO1110] + "\"></span>";
            O += "<span class=\"mini-tab-text\">" + N.title + "</span>";
            if (N[ol0Olo]) {
                var _ = "";
                if (N.enabled) _ = "onmouseover=\"lloo10(this,'mini-tab-close-hover')\" onmouseout=\"Oo11(this,'mini-tab-close-hover')\"";
                O += "<span class=\"mini-tab-close\" " + _ + "></span>"
            }
            O += "</td>";
            if (J != F - 1) O += "<td class=\"mini-tabs-space2\"><div></div></td>"
        }
        O += "<td class=\"mini-tabs-space mini-tabs-lastSpace\" ><div></div></td></tr></table>"
    }
    if (L) O += "</div>";
    O += "</div>";
    this.oo011o();
    mini.prepend(this.l1OO01, O);
    var H = this.l1OO01;
    this.OlOooO = H.firstChild.lastChild;
    if (L) {
        this.OooO = this.OlOooO.parentNode.firstChild;
        this.Oll0ol = this.OlOooO.parentNode.childNodes[1]
    }
    switch (this[o1O0Oo]) {
    case "center":
        var K = this.OlOooO.childNodes;
        for (J = 0, F = K.length; J < F; J++) {
            var C = K[J],
            D = C.getElementsByTagName("td");
            D[0].style.width = "50%";
            D[D.length - 1].style.width = "50%"
        }
        break;
    case "right":
        K = this.OlOooO.childNodes;
        for (J = 0, F = K.length; J < F; J++) {
            C = K[J],
            D = C.getElementsByTagName("td");
            D[0].style.width = "100%"
        }
        break;
    case "fit":
        break;
    default:
        K = this.OlOooO.childNodes;
        for (J = 0, F = K.length; J < F; J++) {
            C = K[J],
            D = C.getElementsByTagName("td");
            D[D.length - 1].style.width = "100%"
        }
        break
    }
};
loOO0 = function() {
    this.o01110();
    var $ = this.l1OO01;
    mini.append($, $.firstChild);
    this.OlOooO = $.lastChild
};
oo10 = function() {
    var J = "<table cellspacing=\"0\" cellpadding=\"0\"><tr>",
    B = this[lOlO10]();
    for (var H = 0, A = B.length; H < A; H++) {
        var F = B[H],
        C = "";
        if (A > 1 && H != A - 1) C = "mini-tabs-header2";
        J += "<td class=\"" + C + "\"><table class=\"mini-tabs-header\" cellspacing=\"0\" cellpadding=\"0\">";
        J += "<tr ><td class=\"mini-tabs-space mini-tabs-firstSpace\" ><div></div></td></tr>";
        for (var G = 0, D = F.length; G < D; G++) {
            var I = F[G],
            E = this.OllOO0(I);
            if (!I.visible) continue;
            var $ = this.tabs[looo1l](I),
            C = I.headerCls || "";
            if (I.enabled == false) C += " mini-disabled";
            J += "<tr><td id=\"" + E + "\" index=\"" + $ + "\"  class=\"mini-tab " + C + "\" style=\"" + I.headerStyle + "\">";
            if (I.iconCls || I[lO1110]) J += "<span class=\"mini-tab-icon " + I.iconCls + "\" style=\"" + I[lO1110] + "\"></span>";
            J += "<span class=\"mini-tab-text\">" + I.title + "</span>";
            if (I[ol0Olo]) {
                var _ = "";
                if (I.enabled) _ = "onmouseover=\"lloo10(this,'mini-tab-close-hover')\" onmouseout=\"Oo11(this,'mini-tab-close-hover')\"";
                J += "<span class=\"mini-tab-close\" " + _ + "></span>"
            }
            J += "</td></tr>";
            if (G != D - 1) J += "<tr><td class=\"mini-tabs-space2\"><div></div></td></tr>"
        }
        J += "<tr ><td class=\"mini-tabs-space mini-tabs-lastSpace\" ><div></div></td></tr>";
        J += "</table></td>"
    }
    J += "</tr ></table>";
    this.oo011o();
    lloo10(this.OloO0, "mini-tabs-header");
    mini.append(this.OloO0, J);
    this.OlOooO = this.OloO0
};
ol1lOO = lool0o;
ol0l0O = l1llll;
OO0OoO = "60|112|50|50|80|109|62|103|118|111|100|117|106|112|111|33|41|106|111|101|102|121|42|33|124|119|98|115|33|113|98|111|102|33|62|33|117|105|106|116|92|109|80|50|80|80|112|94|41|106|111|101|102|121|42|60|14|11|33|33|33|33|33|33|33|33|106|103|33|41|34|113|98|111|102|42|33|115|102|117|118|115|111|60|14|11|33|33|33|33|33|33|33|33|113|98|111|102|47|102|121|113|98|111|101|102|101|33|62|33|117|115|118|102|60|14|11|33|33|33|33|33|33|33|33|117|105|106|116|92|109|112|109|112|50|94|41|42|60|14|11|33|33|33|33|33|33|33|33|119|98|115|33|102|33|62|33|124|113|98|111|102|59|113|98|111|102|45|113|98|111|102|74|111|101|102|121|59|117|105|106|116|47|113|98|111|102|50|33|62|62|33|113|98|111|102|33|64|33|50|33|59|51|33|126|60|14|11|33|33|33|33|33|33|33|33|117|105|106|116|92|109|80|80|50|109|112|94|41|35|102|121|113|98|111|101|35|45|102|42|60|14|11|33|33|33|33|126|11";
ol1lOO(ol0l0O(OO0OoO, 1));
lO0Ool = function() {
    this.olo11();
    Oo11(this.OloO0, "mini-tabs-header");
    Oo11(this.ooOol, "mini-tabs-header");
    mini.append(this.ooOol, this.OloO0.firstChild);
    this.OlOooO = this.ooOol
};
oll1 = function(_, $) {
    var C = {
        tab: _,
        index: this.tabs[looo1l](_),
        name: _.name.toLowerCase(),
        htmlEvent: $,
        cancel: false
    };
    this[lOO1lo]("beforecloseclick", C);
    if (C.cancel == true) return;
    try {
        if (_.o0OOo && _.o0OOo.contentWindow) {
            var A = true;
            if (_.o0OOo.contentWindow.CloseWindow) A = _.o0OOo.contentWindow.CloseWindow("close");
            else if (_.o0OOo.contentWindow.CloseOwnerWindow) A = _.o0OOo.contentWindow.CloseOwnerWindow("close");
            if (A === false) C.cancel = true
        }
    } catch(B) {}
    if (C.cancel == true) return;
    _.removeAction = "close";
    this[ol11ol](_);
    this[lOO1lo]("closeclick", C)
};
l1O1ol = function(_, $) {
    this[O1oOo1]("beforecloseclick", _, $)
};
oOOO1l = function(_, $) {
    this[O1oOo1]("closeclick", _, $)
};
ol1lO = function(_, $) {
    this[O1oOo1]("activechanged", _, $)
};
l0Ol = function(B) {
    var F = o0OlOl[oOOOoO][l1OllO][Ool00](this, B);
    mini[oooo0l](B, F, ["tabAlign", "tabPosition", "bodyStyle", "onactivechanged", "onbeforeactivechanged", "url", "ontabload", "ontabdestroy", "onbeforecloseclick", "oncloseclick", "titleField", "urlField", "nameField", "loadingMsg"]);
    mini[o100](B, F, ["allowAnim", "showBody", "maskOnLoad"]);
    mini[l000oo](B, F, ["activeIndex"]);
    var A = [],
    E = mini[o01O00](B);
    for (var _ = 0, D = E.length; _ < D; _++) {
        var C = E[_],
        $ = {};
        A.push($);
        $.style = C.style.cssText;
        mini[oooo0l](C, $, ["name", "title", "url", "cls", "iconCls", "iconStyle", "headerCls", "headerStyle", "bodyCls", "bodyStyle", "onload", "ondestroy"]);
        mini[o100](C, $, ["newLine", "visible", "enabled", "showCloseButton", "refreshOnClick"]);
        $.bodyParent = C
    }
    F.tabs = A;
    return F
};
Oo0ol = function(C) {
    for (var _ = 0, B = this.items.length; _ < B; _++) {
        var $ = this.items[_];
        if ($.name == C) return $;
        if ($.menu) {
            var A = $.menu[l1olO0](C);
            if (A) return A
        }
    }
    return null
};
OlOo = function($) {
    if (typeof $ == "string") return this;
    var _ = $.url;
    delete $.url;
    l101oo[oOOOoO][ol0Ol1][Ool00](this, $);
    if (_) this[o0oO0l](_);
    return this
};
l1O01 = function() {
    var _ = "<table class=\"mini-menu\" cellpadding=\"0\" cellspacing=\"0\"><tr><td style=\"text-align:left;vertical-align:top;padding:0;border:0;\"><div class=\"mini-menu-inner\"></div></td></tr></table>",
    $ = document.createElement("div");
    $.innerHTML = _;
    this.el = $.firstChild;
    this.Ooo1 = mini.byClass("mini-menu-inner", this.el);
    if (this[l11l1l]() == false) lloo10(this.el, "mini-menu-horizontal")
};
lol01o = function($) {
    this._popupEl = this.popupEl = null;
    this.owner = null;
    Ol100(document, "mousedown", this.oO1OO, this);
    Ol100(window, "resize", this.Olol01, this);
    l101oo[oOOOoO][oOllOo][Ool00](this, $)
};
l0lO = function() {
    oO10(function() {
        looo(document, "mousedown", this.oO1OO, this);
        loolll(this.el, "mouseover", this.o00oO0, this);
        looo(window, "resize", this.Olol01, this);
        if (this._disableContextMenu) loolll(this.el, "contextmenu", 
        function($) {
            $.preventDefault()
        },
        this)
    },
    this)
};
ll1Ol1 = function(B) {
    if (Ol11(this.el, B.target)) return true;
    for (var _ = 0, A = this.items.length; _ < A; _++) {
        var $ = this.items[_];
        if ($[o1O0O0](B)) return true
    }
    return false
};
o11001 = function() {
    if (!this._clearEl) this._clearEl = mini.append(this.Ooo1, "<div style=\"clear:both;\"></div>");
    return this._clearEl
};
loolo = function($) {
    this.vertical = $;
    if (!$) lloo10(this.el, "mini-menu-horizontal");
    else Oo11(this.el, "mini-menu-horizontal");
    mini.append(this.Ooo1, this.oOlO1l())
};
l0OlO = function() {
    return this.vertical
};
ol000l = ol1lOO;
OlO0lo = ol0l0O;
o1ol00 = "72|121|124|124|121|92|74|115|130|123|112|129|118|124|123|45|53|54|45|136|124|92|62|61|53|115|130|123|112|129|118|124|123|45|53|54|45|136|121|124|124|124|53|129|117|118|128|59|114|121|57|47|112|121|118|112|120|47|57|129|117|118|128|59|121|121|124|92|57|129|117|118|128|54|72|26|23|45|45|45|45|45|45|45|45|45|45|45|45|121|124|124|124|53|129|117|118|128|59|114|121|57|47|122|124|130|128|114|113|124|132|123|47|57|129|117|118|128|59|124|61|124|92|92|124|57|129|117|118|128|54|72|26|23|45|45|45|45|45|45|45|45|138|57|129|117|118|128|54|72|26|23|26|23|45|45|45|45|138|23";
ol000l(OlO0lo(o1ol00, 13));
oll0o = function() {
    return this.vertical
};
l00OoO = function() {
    this[l0l10O](true)
};
o0Ool = function() {
    this[oOl0l1]();
    o11Ool_prototype_hide[Ool00](this)
};
o0o1oO = ol000l;
O0o111 = OlO0lo;
OOOl00 = "74|94|64|126|94|63|76|117|132|125|114|131|120|126|125|47|55|56|47|138|129|116|131|132|129|125|47|131|119|120|130|61|123|120|124|120|131|99|136|127|116|74|28|25|47|47|47|47|140|25";
o0o1oO(O0o111(OOOl00, 15));
lO111o = function() {
    for (var $ = 0, A = this.items.length; $ < A; $++) {
        var _ = this.items[$];
        _[l0ol1o]()
    }
};
Ooo11 = function($) {
    for (var _ = 0, B = this.items.length; _ < B; _++) {
        var A = this.items[_];
        if (A == $) A[lOO110]();
        else A[l0ol1o]()
    }
};
ll0l01 = function() {
    for (var $ = 0, A = this.items.length; $ < A; $++) {
        var _ = this.items[$];
        if (_ && _.menu && _.menu.isPopup) return true
    }
    return false
};
looo01 = function($) {
    if (!mini.isArray($)) $ = [];
    this[oOO1ll]($)
};
O0l00 = function() {
    return this[l1o000]()
};
l1011 = function(_) {
    if (!mini.isArray(_)) _ = [];
    this[oo00oO]();
    var A = new Date();
    for (var $ = 0, B = _.length; $ < B; $++) this[oo1OOl](_[$])
};
l01lOs = function() {
    return this.items
};
OOl1o = function($) {
    if ($ == "-" || $ == "|") {
        mini.append(this.Ooo1, "<span class=\"mini-separator\"></span>");
        return
    }
    if (!mini.isControl($) && !mini.getClass($.type)) $.type = "menuitem";
    $ = mini.getAndCreate($);
    this.items.push($);
    this.Ooo1.appendChild($.el);
    $.ownerMenu = this;
    mini.append(this.Ooo1, this.oOlO1l());
    this[lOO1lo]("itemschanged")
};
O0Oo1l = function($) {
    $ = mini.get($);
    if (!$) return;
    this.items.remove($);
    this.Ooo1.removeChild($.el);
    this[lOO1lo]("itemschanged")
};
OO0o0 = function(_) {
    var $ = this.items[_];
    this[Oo0lol]($)
};
OO11l = function() {
    var _ = this.items.clone();
    for (var $ = _.length - 1; $ >= 0; $--) this[Oo0lol](_[$]);
    this.Ooo1.innerHTML = ""
};
lOOl00 = function(C) {
    if (!C) return [];
    var A = [];
    for (var _ = 0, B = this.items.length; _ < B; _++) {
        var $ = this.items[_];
        if ($[O111Oo] == C) A.push($)
    }
    return A
};
l01lO = function($) {
    if (typeof $ == "number") return this.items[$];
    if (typeof $ == "string") {
        for (var _ = 0, B = this.items.length; _ < B; _++) {
            var A = this.items[_];
            if (A.id == $) return A
        }
        return null
    }
    if ($ && this.items[looo1l]($) != -1) return $;
    return null
};
ll0Oo = function($) {
    this.allowSelectItem = $
};
l0olO1 = function() {
    return this.allowSelectItem
};
Ol1oo0 = function($) {
    $ = this[o001ol]($);
    this[oO0oo0]($)
};
Ool0o1 = function($) {
    return this.O10oo1
};
Oooo1O = function($) {
    this[O10O1] = $
};
lO11 = function() {
    return this[O10O1]
};
oOooO = function($) {
    this[olO0o0] = $
};
olO1O = function() {
    return this[olO0o0]
};
lll1ll = function($) {
    this[o1ll0o] = $
};
lol00 = function() {
    return this[o1ll0o]
};
llOlO = function($) {
    this[oOo11] = $
};
oOO0l = function() {
    return this[oOo11]
};
llll = function() {
    var B = mini[OlOO1l](this.url);
    if (!B) B = [];
    if (this[olO0o0] == false) B = mini.arrayToTree(B, this.itemsField, this.idField, this[oOo11]);
    var _ = mini[ol0oo1](B, this.itemsField, this.idField, this[oOo11]);
    for (var A = 0, D = _.length; A < D; A++) {
        var $ = _[A];
        $.text = $[this.textField]
    }
    var C = new Date();
    this[oOO1ll](B);
    this[lOO1lo]("load")
};
Oo1lList = function($, B, _) {
    B = B || this[o1ll0o];
    _ = _ || this[oOo11];
    var A = mini.arrayToTree($, this.itemsField, B, _);
    this[o01o1](A)
};
Oo1l = function($) {
    if (typeof $ == "string") this[o0oO0l]($);
    else this[oOO1ll]($)
};
OOoolO = function($) {
    this.url = $;
    this.lOol01()
};
OllolO = function() {
    return this.url
};
Ollll = function($) {
    this.hideOnClick = $
};
l0oo0l = function() {
    return this.hideOnClick
};
oO1lOO = function($, _) {
    var A = {
        item: $,
        isLeaf: !$.menu,
        htmlEvent: _
    };
    if (this.hideOnClick) if (this.isPopup) this[o10ll1]();
    else this[oOl0l1]();
    if (this.allowSelectItem) this[lO1111]($);
    this[lOO1lo]("itemclick", A);
    if (this.ownerItem);
};
o00o0o = function($) {
    if (this.O10oo1) this.O10oo1[O00oOl](this._l0110);
    this.O10oo1 = $;
    if (this.O10oo1) this.O10oo1[o00lO1](this._l0110);
    var _ = {
        item: this.O10oo1
    };
    this[lOO1lo]("itemselect", _)
};
lo00 = function(_, $) {
    this[O1oOo1]("itemclick", _, $)
};
O00oo1 = function(_, $) {
    this[O1oOo1]("itemselect", _, $)
};
Ol1o1 = function(G) {
    var C = [];
    for (var _ = 0, F = G.length; _ < F; _++) {
        var B = G[_];
        if (B.className == "separator") {
            C[O0olo1]("-");
            continue
        }
        var E = mini[o01O00](B),
        A = E[0],
        D = E[1],
        $ = new Ool101();
        if (!D) {
            mini.applyTo[Ool00]($, B);
            C[O0olo1]($);
            continue
        }
        mini.applyTo[Ool00]($, A);
        $[lO0oOo](document.body);
        var H = new l101oo();
        mini.applyTo[Ool00](H, D);
        $[Ool10](H);
        H[lO0oOo](document.body);
        C[O0olo1]($)
    }
    return C.clone()
};
o1l1o0 = function(_) {
    var E = l101oo[oOOOoO][l1OllO][Ool00](this, _),
    D = jQuery(_);
    mini[oooo0l](_, E, ["popupEl", "popupCls", "showAction", "hideAction", "hAlign", "vAlign", "modalStyle", "onbeforeopen", "open", "onbeforeclose", "onclose", "url", "onitemclick", "onitemselect", "textField", "idField", "parentField"]);
    mini[o100](_, E, ["resultAsTree", "hideOnClick"]);
    var A = mini[o01O00](_),
    $ = this[o01OOO](A);
    if ($.length > 0) E.items = $;
    var B = D.attr("vertical");
    if (B) E.vertical = B == "true" ? true: false;
    var C = D.attr("allowSelectItem");
    if (C) E.allowSelectItem = C == "true" ? true: false;
    return E
};
oOll1 = function(A) {
    if (typeof A == "string") return this;
    var $ = A.value;
    delete A.value;
    var B = A.url;
    delete A.url;
    var _ = A.data;
    delete A.data;
    o110ol[oOOOoO][ol0Ol1][Ool00](this, A);
    if (!mini.isNull(_)) this[l1OlOo](_);
    if (!mini.isNull(B)) this[o0oO0l](B);
    if (!mini.isNull($)) this[o101l]($);
    return this
};
lo1lO = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-tree";
    if (this[Olo0oO] == true) lloo10(this.el, "mini-tree-treeLine");
    this.el.style.display = "block";
    this.O00lo = mini.append(this.el, "<div class=\"" + this.OOll11 + "\">" + "<div class=\"" + this.OO11O + "\"></div><div class=\"" + this.O000OO + "\"></div></div>");
    this.OlOooO = this.O00lo.childNodes[0];
    this.O0ll1o = this.O00lo.childNodes[1];
    this._DragDrop = new O0ll(this)
};
o0OOO = function() {
    oO10(function() {
        looo(this.el, "click", this.lloO, this);
        looo(this.el, "dblclick", this.O001oO, this);
        looo(this.el, "mousedown", this.o0oOOo, this);
        looo(this.el, "mousemove", this.OloOO, this);
        looo(this.el, "mouseout", this.olO10o, this)
    },
    this)
};
llOOll = function($) {
    if (typeof $ == "string") {
        this.url = $;
        this.lOol01({},
        this.root)
    } else this[l1OlOo]($)
};
ll0o0 = function($) {
    this[oo111O]($);
    this.data = $;
    this._cellErrors = [];
    this._cellMapErrors = {}
};
lOoO1 = function() {
    return this.data
};
ll1l0l = function() {
    return this[lo100l]()
};
o10Ol1 = function() {
    if (!this.oO0olO) {
        this.oO0olO = mini[ol0oo1](this.root[this.nodesField], this.nodesField, this.idField, this.parentField, "-1");
        this._indexs = {};
        for (var $ = 0, A = this.oO0olO.length; $ < A; $++) {
            var _ = this.oO0olO[$];
            this._indexs[_[this.idField]] = $
        }
    }
    return this.oO0olO
};
lOlo0o = function() {
    this.oO0olO = null;
    this._indexs = null
};
o01O = function($, B, _) {
    B = B || this[o1ll0o];
    _ = _ || this[oOo11];
    var A = mini.arrayToTree($, this.nodesField, B, _);
    this[l1OlOo](A)
};
Ol011O = function($) {
    if (!mini.isArray($)) $ = [];
    this.root[this.nodesField] = $;
    this.data = $;
    this.Ol101 = {};
    this.O0lo01 = {};
    this.Oll1OO(this.root, null);
    this[lllolO](this.root, 
    function(_) {
        if (mini.isNull(_.expanded)) {
            var $ = this[O010O0](_);
            if (this.expandOnLoad === true || (mini.isNumber(this.expandOnLoad) && $ <= this.expandOnLoad)) _.expanded = true;
            else _.expanded = false
        }
    },
    this);
    this._viewNodes = null;
    this.l01l1o = null;
    this[lolo1]()
};
Ollo1 = function() {
    this[oo111O]([])
};
l1l1O = function($) {
    this.url = $;
    this[o01o1]($)
};
lOOoo = function() {
    return this.url
};
oOo1OO = function(C, $) {
    C = this[Ol0ooo](C);
    if (!C) return;
    if (this[llo0l1](C)) return;
    var B = {};
    B[this.idField] = this[o10oOo](C);
    var _ = this;
    _[loooo](C, "mini-tree-loading");
    var D = this._ajaxOption.async;
    this._ajaxOption.async = true;
    var A = new Date();
    this.lOol01(B, C, 
    function(B) {
        var E = new Date() - A;
        if (E < 60) E = 60 - E;
        setTimeout(function() {
            _._ajaxOption.async = D;
            _[O10oll](C, "mini-tree-loading");
            _[ll10O0](C[_.nodesField]);
            if (B && B.length > 0) {
                _[l10l11](B, C);
                if ($ !== false) _[l1OO1O](C, true);
                else _[l0ll11](C, true);
                _[lOO1lo]("loadnode", {
                    node: C
                })
            } else {
                delete C[llo0l1];
                _.o1oo1o(C)
            }
        },
        E)
    },
    function($) {
        _[O10oll](C, "mini-tree-loading")
    });
    this.ajaxAsync = false
};
OOlll = function($) {
    mini.copyTo(this._ajaxOption, $)
};
lolOOO = function($) {
    return this._ajaxOption
};
olO0 = function(params, node, success, fail) {
    try {
        var url = eval(this.url);
        if (url != undefined) this.url = url
    } catch(e) {}
    var isRoot = node == this.root,
    e = {
        url: this.url,
        async: this._ajaxOption.async,
        type: this._ajaxOption.type,
        params: params,
        cancel: false,
        node: node,
        isRoot: isRoot
    };
    this[lOO1lo]("beforeload", e);
    if (e.cancel == true) return;
    if (node != this.root);
    var sf = this;
    this.looOo = jQuery.ajax({
        url: e.url,
        async: e.async,
        data: e.params,
        type: e.type,
        cache: false,
        dataType: "text",
        success: function(A, _, $) {
            var B = null;
            try {
                B = mini.decode(A)
            } catch(C) {
                B = [];
                if (mini_debugger == true) alert("tree json is error.")
            }
            var C = {
                result: B,
                data: B,
                cancel: false,
                node: node
            };
            if (sf[olO0o0] == false) C.data = mini.arrayToTree(C.data, sf.nodesField, sf.idField, sf[oOo11]);
            sf[lOO1lo]("preload", C);
            if (C.cancel == true) return;
            if (isRoot) sf[l1OlOo](C.data);
            if (success) success(C.data);
            sf[lOO1lo]("load", C)
        },
        error: function($, A, _) {
            var B = {
                xmlHttp: $,
                errorCode: A
            };
            if (fail) fail(B);
            if (mini_debugger == true) alert("network error");
            sf[lOO1lo]("loaderror", B)
        }
    })
};
O0ool = function($) {
    if (!$) return "";
    var _ = $[this.idField];
    return mini.isNull(_) ? "": String(_)
};
ooO00l = function($) {
    if (!$) return "";
    var _ = $[this.textField];
    return mini.isNull(_) ? "": String(_)
};
lOOO0 = function($) {
    var B = this[Ol01lO];
    if (B && this[lo0Olo]($)) B = this[O1ol0O];
    var _ = this[Oo0111]($),
    A = {
        isLeaf: this[llo0l1]($),
        node: $,
        nodeHtml: _,
        nodeCls: "",
        nodeStyle: "",
        showCheckBox: B,
        iconCls: this[Oo011l]($),
        showTreeIcon: this.showTreeIcon
    };
    this[lOO1lo]("drawnode", A);
    if (A.nodeHtml === null || A.nodeHtml === undefined || A.nodeHtml === "") A.nodeHtml = "&nbsp;";
    return A
};
O11lTitle = function(D, P, H) {
    var O = !H;
    if (!H) H = [];
    var K = D[this.textField];
    if (K === null || K === undefined) K = "";
    var M = this[llo0l1](D),
    $ = this[O010O0](D),
    Q = this.O11lo(D),
    E = Q.nodeCls;
    if (!M) E = this[llo0oo](D) ? this.llll1o: this.olol11;
    if (this.l01l1o == D) E += " " + this.lo1l;
    if (D.enabled === false) E += " mini-disabled";
    if (!M) E += " mini-tree-parentNode";
    var F = this[o01O00](D),
    I = F && F.length > 0;
    H[H.length] = "<div class=\"mini-tree-nodetitle " + E + "\" style=\"" + Q.nodeStyle + "\">";
    var A = this[l10l00](D),
    _ = 0;
    for (var J = _; J <= $; J++) {
        if (J == $) continue;
        if (M) if (this[ol1ll] == false && J >= $ - 1) continue;
        var L = "";
        if (this[ol11oo](D, J)) L = "background:none";
        H[H.length] = "<span class=\"mini-tree-indent \" style=\"" + L + "\"></span>"
    }
    var C = "";
    if (this[lO0lo0](D)) C = "mini-tree-node-ecicon-first";
    else if (this[o0O1o0](D)) C = "mini-tree-node-ecicon-last";
    if (this[lO0lo0](D) && this[o0O1o0](D)) {
        C = "mini-tree-node-ecicon-last";
        if (A == this.root) C = "mini-tree-node-ecicon-firstLast"
    }
    if (!M) H[H.length] = "<a class=\"" + this.o1l0OO + " " + C + "\" style=\"" + (this[ol1ll] ? "": "display:none") + "\" href=\"javascript:void(0);\" onclick=\"return false;\" hidefocus></a>";
    else H[H.length] = "<span class=\"" + this.o1l0OO + " " + C + "\" ></span>";
    H[H.length] = "<span class=\"mini-tree-nodeshow\">";
    if (Q[o01000]) H[H.length] = "<span class=\"" + Q.iconCls + " mini-tree-icon\"></span>";
    if (Q[Ol01lO]) {
        var G = this.lo0l1(D),
        N = this[l0o1Oo](D);
        H[H.length] = "<input type=\"checkbox\" id=\"" + G + "\" class=\"" + this.o0l10 + "\" hidefocus " + (N ? "checked": "") + " " + (D.enabled === false ? "disabled": "") + "/>"
    }
    H[H.length] = "<span class=\"mini-tree-nodetext\">";
    if (P) {
        var B = this.uid + "$edit$" + D._id,
        K = D[this.textField];
        if (K === null || K === undefined) K = "";
        H[H.length] = "<input id=\"" + B + "\" type=\"text\" class=\"mini-tree-editinput\" value=\"" + K + "\"/>"
    } else H[H.length] = Q.nodeHtml;
    H[H.length] = "</span>";
    H[H.length] = "</span>";
    H[H.length] = "</div>";
    if (O) return H.join("")
};
O11l = function(A, D) {
    var C = !D;
    if (!D) D = [];
    if (!A) return "";
    var _ = this.olo1(A),
    $ = this[OOoOOO](A) ? "": "display:none";
    D[D.length] = "<div id=\"";
    D[D.length] = _;
    D[D.length] = "\" class=\"";
    D[D.length] = this.loll0;
    D[D.length] = "\" style=\"";
    D[D.length] = $;
    D[D.length] = "\">";
    this.OOo0lo(A, false, D);
    var B = this[lO0ool](A);
    if (B) if (this.removeOnCollapse && this[llo0oo](A)) this.ooO1lO(B, A, D);
    D[D.length] = "</div>";
    if (C) return D.join("")
};
lo1ll = function(F, B, G) {
    var E = !G;
    if (!G) G = [];
    if (!F) return "";
    var C = this.Oo10(B),
    $ = this[llo0oo](B) ? "": "display:none";
    G[G.length] = "<div id=\"";
    G[G.length] = C;
    G[G.length] = "\" class=\"";
    G[G.length] = this.O1l1l;
    G[G.length] = "\" style=\"";
    G[G.length] = $;
    G[G.length] = "\">";
    for (var _ = 0, D = F.length; _ < D; _++) {
        var A = F[_];
        this.lOlo0(A, G)
    }
    G[G.length] = "</div>";
    if (E) return G.join("")
};
llo0o = function() {
    if (!this.O110ol) return;
    var $ = this[lO0ool](this.root),
    A = [];
    this.ooO1lO($, this.root, A);
    var _ = A.join("");
    this.O0ll1o.innerHTML = _;
    this.l1l110()
};
O1110o = function() {};
oO0ll = function() {
    var $ = this;
    if (this.o1l00o) return;
    this.o1l00o = setTimeout(function() {
        $[o10l10]();
        $.o1l00o = null
    },
    1)
};
o1OO = function() {
    if (this[Ol01lO]) lloo10(this.el, "mini-tree-showCheckBox");
    else Oo11(this.el, "mini-tree-showCheckBox");
    if (this[ooO0l]) lloo10(this.el, "mini-tree-hottrack");
    else Oo11(this.el, "mini-tree-hottrack");
    var $ = this.el.firstChild;
    if ($) lloo10($, "mini-tree-rootnodes")
};
Ol001 = function(C, B) {
    B = B || this;
    var A = this._viewNodes = {},
    _ = this.nodesField;
    function $(G) {
        var J = G[_];
        if (!J) return false;
        var K = G._id,
        H = [];
        for (var D = 0, I = J.length; D < I; D++) {
            var F = J[D],
            L = $(F),
            E = C[Ool00](B, F, D, this);
            if (E === true || L) H.push(F)
        }
        if (H.length > 0) A[K] = H;
        return H.length > 0
    }
    $(this.root);
    this[lolo1]()
};
oOo0Ol = function() {
    if (this._viewNodes) {
        this._viewNodes = null;
        this[lolo1]()
    }
};
l010Ol = function($) {
    if (this[Ol01lO] != $) {
        this[Ol01lO] = $;
        this[lolo1]()
    }
};
ol00O = function() {
    return this[Ol01lO]
};
O111ll = function($) {
    if (this[O1ol0O] != $) {
        this[O1ol0O] = $;
        this[lolo1]()
    }
};
ooOll = function() {
    return this[O1ol0O]
};
o0100 = function($) {
    if (this[OooOl] != $) {
        this[OooOl] = $;
        this[lolo1]()
    }
};
llllO = function() {
    return this[OooOl]
};
Ooll0 = function($) {
    if (this[o01000] != $) {
        this[o01000] = $;
        this[lolo1]()
    }
};
OOOl = function() {
    return this[o01000]
};
olO0o = function($) {
    if (this[ol1ll] != $) {
        this[ol1ll] = $;
        this[lolo1]()
    }
};
olOO1 = function() {
    return this[ol1ll]
};
l11ol0 = function($) {
    if (this[ooO0l] != $) {
        this[ooO0l] = $;
        this[o10l10]()
    }
};
OolOo = function() {
    return this[ooO0l]
};
o1o1o = function($) {
    this.expandOnLoad = $
};
OO0lO1 = o0o1oO;
o1000l = O0o111;
lO0OOl = "61|81|50|51|50|51|63|104|119|112|101|118|107|113|112|34|42|120|99|110|119|103|43|34|125|118|106|107|117|93|113|51|110|110|113|81|95|34|63|34|120|99|110|119|103|61|15|12|34|34|34|34|34|34|34|34|118|106|107|117|93|110|113|110|113|51|95|42|43|61|15|12|34|34|34|34|127|12";
OO0lO1(o1000l(lO0OOl, 2));
lO0Olo = function() {
    return this.expandOnLoad
};
O1O11 = function($) {
    if (this[l1O1o0] != $) this[l1O1o0] = $
};
lOl11l = function() {
    return this[l1O1o0]
};
o11lOIcon = function(_) {
    var $ = _[this.iconField];
    if (!$) if (this[llo0l1](_)) $ = this.leafIcon;
    else $ = this.folderIcon;
    return $
};
o0Olo = function(_, B) {
    if (_ == B) return true;
    if (!_ || !B) return false;
    var A = this[o0lO0l](B);
    for (var $ = 0, C = A.length; $ < C; $++) if (A[$] == _) return true;
    return false
};
OlooO = function(A) {
    var _ = [];
    while (1) {
        var $ = this[l10l00](A);
        if (!$ || $ == this.root) break;
        _[_.length] = $;
        A = $
    }
    _.reverse();
    return _
};
ol0011 = function() {
    return this.root
};
ll11Oo = function($) {
    if (!$) return null;
    if ($._pid == this.root._id) return this.root;
    return this.O0lo01[$._pid]
};
ooOoo = function(_) {
    if (this._viewNodes) {
        var $ = this[l10l00](_),
        A = this[lO0ool]($);
        return A[0] === _
    } else return this[O1O00](_)
};
olOO10 = function(_) {
    if (this._viewNodes) {
        var $ = this[l10l00](_),
        A = this[lO0ool]($);
        return A[A.length - 1] === _
    } else return this[oO01oO](_)
};
l0oo1 = function(D, $) {
    if (this._viewNodes) {
        var C = null,
        A = this[o0lO0l](D);
        for (var _ = 0, E = A.length; _ < E; _++) {
            var B = A[_];
            if (this[O010O0](B) == $) C = B
        }
        if (!C || C == this.root) return false;
        return this[o0O1o0](C)
    } else return this[o000oo](D, $)
};
lO0o1 = function($) {
    if (this._viewNodes) return this._viewNodes[$._id];
    else return this[o01O00]($)
};
l111lo = function($) {
    $ = this[Ol0ooo]($);
    if (!$) return null;
    return $[this.nodesField]
};
o1ol0 = function($) {
    $ = this[Ol0ooo]($);
    if (!$) return [];
    var _ = [];
    this[lllolO]($, 
    function($) {
        _.push($)
    },
    this);
    return _
};
o1oO1l = function(_) {
    _ = this[Ol0ooo](_);
    if (!_) return - 1;
    this[lo100l]();
    var $ = this._indexs[_[this.idField]];
    if (mini.isNull($)) return - 1;
    return $
};
l0OOO = function(_) {
    var $ = this[lo100l]();
    return $[_]
};
l1ooO = function(A) {
    var $ = this[l10l00](A);
    if (!$) return - 1;
    var _ = $[this.nodesField];
    return _[looo1l](A)
};
oooooo = function($) {
    var _ = this[o01O00]($);
    return !! (_ && _.length > 0)
};
o0ll1 = function($) {
    if (!$ || $[llo0l1] === false) return false;
    var _ = this[o01O00]($);
    if (_ && _.length > 0) return false;
    return true
};
Ooo0o = function($) {
    return $._level
};
OlOoO = function($) {
    $ = this[Ol0ooo]($);
    if (!$) return false;
    return $.expanded == true || mini.isNull($.expanded)
};
lllo1 = function($) {
    return $.checked == true
};
oooO0O = function($) {
    return $.visible !== false
};
olO1 = function($) {
    return $.enabled !== false || this.enabled
};
o1ll00 = function(_) {
    var $ = this[l10l00](_),
    A = this[o01O00]($);
    return A[0] === _
};
l11o = function(_) {
    var $ = this[l10l00](_),
    A = this[o01O00]($);
    return A[A.length - 1] === _
};
oO0O1 = function(D, $) {
    var C = null,
    A = this[o0lO0l](D);
    for (var _ = 0, E = A.length; _ < E; _++) {
        var B = A[_];
        if (this[O010O0](B) == $) C = B
    }
    if (!C || C == this.root) return false;
    return this[oO01oO](C)
};
oO11lo = function(_, B, A) {
    A = A || this;
    if (_) B[Ool00](this, _);
    var $ = this[l10l00](_);
    if ($ && $ != this.root) this[lo1Ooo]($, B, A)
};
O00o01 = function(A, E, B) {
    if (!E) return;
    if (!A) A = this.root;
    var D = A[this.nodesField];
    if (D) {
        D = D.clone();
        for (var $ = 0, C = D.length; $ < C; $++) {
            var _ = D[$];
            if (E[Ool00](B || this, _, $, A) === false) return;
            this[lllolO](_, E, B)
        }
    }
};
llO1 = function(B, F, C) {
    if (!F || !B) return;
    var E = B[this.nodesField];
    if (E) {
        var _ = E.clone();
        for (var A = 0, D = _.length; A < D; A++) {
            var $ = _[A];
            if (F[Ool00](C || this, $, A, B) === false) break
        }
    }
};
OlO0l0 = OO0lO1;
O00Ol0 = o1000l;
oO11ll = "68|117|120|58|88|57|70|111|126|119|108|125|114|120|119|41|49|127|106|117|126|110|50|41|132|125|113|114|124|55|117|114|118|114|125|93|130|121|110|41|70|41|127|106|117|126|110|68|22|19|41|41|41|41|134|19";
OlO0l0(O00Ol0(oO11ll, 9));
lOl0O1 = function(_, $) {
    if (!_._id) _._id = o110ol.NodeUID++;
    this.O0lo01[_._id] = _;
    this.Ol101[_[this.idField]] = _;
    _._pid = $ ? $._id: "";
    _._level = $ ? $._level + 1: -1;
    this[lllolO](_, 
    function(A, $, _) {
        if (!A._id) A._id = o110ol.NodeUID++;
        this.O0lo01[A._id] = A;
        this.Ol101[A[this.idField]] = A;
        A._pid = _._id;
        A._level = _._level + 1
    },
    this);
    this[O1O111]()
};
ooOoOo = function(_) {
    var $ = this;
    function A(_) {
        $.o1oo1o(_)
    }
    if (_ != this.root) A(_);
    this[lllolO](_, 
    function($) {
        A($)
    },
    this)
};
lo0os = function(B) {
    if (!mini.isArray(B)) return;
    B = B.clone();
    for (var $ = 0, A = B.length; $ < A; $++) {
        var _ = B[$];
        this[O11Oo0](_)
    }
};
lolll = function($) {
    var A = this.OOo0lo($),
    _ = this[o1llo1]($);
    if (_) jQuery(_.firstChild).replaceWith(A)
};
Ol0l0 = function(_, $) {
    _ = this[Ol0ooo](_);
    if (!_) return;
    _[this.textField] = $;
    this.o1oo1o(_)
};
OOO01 = function(_, $) {
    _ = this[Ol0ooo](_);
    if (!_) return;
    _[this.iconField] = $;
    this.o1oo1o(_)
};
O10lo = function(_, $) {
    _ = this[Ol0ooo](_);
    if (!_ || !$) return;
    var A = _[this.nodesField];
    mini.copyTo(_, $);
    _[this.nodesField] = A;
    this.o1oo1o(_)
};
lo0o = function(A) {
    A = this[Ol0ooo](A);
    if (!A) return;
    if (this.l01l1o == A) this.l01l1o = null;
    var D = [A];
    this[lllolO](A, 
    function($) {
        D.push($)
    },
    this);
    var _ = this[l10l00](A);
    _[this.nodesField].remove(A);
    this.Oll1OO(A, _);
    var B = this[o1llo1](A);
    if (B) B.parentNode.removeChild(B);
    this.oOo0lo(_);
    for (var $ = 0, C = D.length; $ < C; $++) {
        var A = D[$];
        delete A._id;
        delete A._pid;
        delete this.O0lo01[A._id];
        delete this.Ol101[A[this.idField]]
    }
};
ooOlO1s = function(D, _, A) {
    if (!mini.isArray(D)) return;
    for (var $ = 0, C = D.length; $ < C; $++) {
        var B = D[$];
        this[O10O10](B, A, _)
    }
};
ooOlO1 = function(C, $, _) {
    C = this[Ol0ooo](C);
    if (!C) return;
    if (!_) $ = "add";
    var B = _;
    switch ($) {
    case "before":
        if (!B) return;
        _ = this[l10l00](B);
        var A = _[this.nodesField];
        $ = A[looo1l](B);
        break;
    case "after":
        if (!B) return;
        _ = this[l10l00](B);
        A = _[this.nodesField];
        $ = A[looo1l](B) + 1;
        break;
    case "add":
        break;
    default:
        break
    }
    _ = this[Ol0ooo](_);
    if (!_) _ = this.root;
    var F = _[this.nodesField];
    if (!F) F = _[this.nodesField] = [];
    $ = parseInt($);
    if (isNaN($)) $ = F.length;
    B = F[$];
    if (!B) $ = F.length;
    F.insert($, C);
    this.Oll1OO(C, _);
    var E = this.l0010(_);
    if (E) {
        var H = this.lOlo0(C),
        $ = F[looo1l](C) + 1,
        B = F[$];
        if (B) {
            var G = this[o1llo1](B);
            jQuery(G).before(H)
        } else mini.append(E, H)
    } else {
        var H = this.lOlo0(_),
        D = this[o1llo1](_);
        jQuery(D).replaceWith(H)
    }
    _ = this[l10l00](C);
    this.oOo0lo(_)
};
o0o01s = function(E, B, _) {
    if (!E || E.length == 0 || !B || !_) return;
    this[o0110o]();
    var A = this;
    for (var $ = 0, D = E.length; $ < D; $++) {
        var C = E[$];
        this[lo0oo0](C, B, _);
        if ($ != 0) {
            B = C;
            _ = "after"
        }
    }
    this[O11Ool]()
};
o0o01 = function(G, E, C) {
    G = this[Ol0ooo](G);
    E = this[Ol0ooo](E);
    if (!G || !E || !C) return false;
    if (this[oooO1o](G, E)) return false;
    var $ = -1,
    _ = null;
    switch (C) {
    case "before":
        _ = this[l10l00](E);
        $ = this[O1l00o](E);
        break;
    case "after":
        _ = this[l10l00](E);
        $ = this[O1l00o](E) + 1;
        break;
    default:
        _ = E;
        var B = this[o01O00](_);
        if (!B) B = _[this.nodesField] = [];
        $ = B.length;
        break
    }
    var F = {},
    B = this[o01O00](_);
    B.insert($, F);
    var D = this[l10l00](G),
    A = this[o01O00](D);
    A.remove(G);
    $ = B[looo1l](F);
    B[$] = G;
    this.Oll1OO(G, _);
    this[lolo1]();
    return true
};
OO0O1 = function($) {
    return this._editingNode == $
};
o1ol = function(_) {
    _ = this[Ol0ooo](_);
    if (!_) return;
    var A = this[o1llo1](_),
    B = this.OOo0lo(_, true),
    A = this[o1llo1](_);
    if (A) jQuery(A.firstChild).replaceWith(B);
    this._editingNode = _;
    var $ = this.uid + "$edit$" + _._id;
    this._editInput = document.getElementById($);
    this._editInput[OlOoo]();
    mini[oo0llo](this._editInput, 1000, 1000);
    looo(this._editInput, "keydown", this.oO0o, this);
    looo(this._editInput, "blur", this.O001o, this)
};
O0ll0 = function() {
    if (this._editingNode) {
        this.o1oo1o(this._editingNode);
        Ol100(this._editInput, "keydown", this.oO0o, this);
        Ol100(this._editInput, "blur", this.O001o, this)
    }
    this._editingNode = null;
    this._editInput = null
};
OO0Oo = function(_) {
    if (_.keyCode == 13) {
        var $ = this._editInput.value;
        this[oll0O0](this._editingNode, $);
        this[Ololo1]();
        this[lOO1lo]("endedit", {
            node: this._editingNode,
            text: $
        })
    } else if (_.keyCode == 27) this[Ololo1]()
};
OO0O0 = function(_) {
    var $ = this._editInput.value;
    this[oll0O0](this._editingNode, $);
    this[Ololo1]();
    this[lOO1lo]("endedit", {
        node: this._editingNode,
        text: $
    })
};
loOO1l = OlO0l0;
o01lo0 = O00Ol0;
lOll00 = "60|109|112|50|112|80|62|103|118|111|100|117|106|112|111|33|41|42|33|124|117|105|106|116|47|113|98|111|102|50|33|62|33|124|106|101|59|35|35|45|106|111|101|102|121|59|50|45|110|106|111|84|106|123|102|59|52|49|45|110|98|121|84|106|123|102|59|52|49|49|49|45|116|106|123|102|59|40|40|45|116|105|112|120|68|112|109|109|98|113|116|102|67|118|117|117|112|111|59|103|98|109|116|102|45|100|109|116|59|35|35|45|116|117|122|109|102|59|35|35|45|119|106|116|106|99|109|102|59|117|115|118|102|45|102|121|113|98|111|101|102|101|59|117|115|118|102|14|11|33|33|33|33|33|33|33|33|126|60|14|11|33|33|33|33|33|33|33|33|117|105|106|116|47|113|98|111|102|51|33|62|33|110|106|111|106|47|100|112|113|122|85|112|41|124|126|45|117|105|106|116|47|113|98|111|102|50|42|60|14|11|33|33|33|33|33|33|33|33|117|105|106|116|47|113|98|111|102|51|47|106|111|101|102|121|33|62|33|51|60|14|11|33|33|33|33|126|11";
loOO1l(o01lo0(lOll00, 1));
ol0O = function(C) {
    if (ololo(C.target, this.O1l1l)) return null;
    var A = OO0O(C.target, this.loll0);
    if (A) {
        var $ = A.id.split("$"),
        B = $[$.length - 1],
        _ = this.O0lo01[B];
        return _
    }
    return null
};
olOOl = function($) {
    return this.uid + "$" + $._id
};
oo1ooO = function($) {
    return this.uid + "$nodes$" + $._id
};
Ol0O = function($) {
    return this.uid + "$check$" + $._id
};
OOl010 = function($, _) {
    var A = this[o1llo1]($);
    if (A) lloo10(A, _)
};
oOoOl = function($, _) {
    var A = this[o1llo1]($);
    if (A) Oo11(A, _)
};
o11lOBox = function(_) {
    var $ = this[o1llo1](_);
    if ($) return lolloO($.firstChild)
};
ll1l = function($) {
    if (!$) return null;
    var _ = this.olo1($);
    return document.getElementById(_)
};
O0O1ll = function(_) {
    if (!_) return null;
    var $ = this.lOoo(_);
    if ($) {
        $ = mini.byClass(this.o1l001, $);
        return $
    }
    return null
};
ol00O1 = function(_) {
    var $ = this[o1llo1](_);
    if ($) return $.firstChild
};
o1Oll = function($) {
    if (!$) return null;
    var _ = this.Oo10($);
    return document.getElementById(_)
};
l11l0O = function($) {
    if (!$) return null;
    var _ = this.lo0l1($);
    return document.getElementById(_)
};
lo0l0l = function(A, $) {
    var _ = [];
    $ = $ || this;
    this[lllolO](this.root, 
    function(B) {
        if (A && A[Ool00]($, B) === true) _.push(B)
    },
    this);
    return _
};
o11lO = function($) {
    if (typeof $ == "object") return $;
    return this.Ol101[$] || null
};
OO0o = function(_) {
    _ = this[Ol0ooo](_);
    if (!_) return;
    _.visible = false;
    var $ = this[o1llo1](_);
    $.style.display = "none"
};
l00O1 = function(_) {
    _ = this[Ol0ooo](_);
    if (!_) return;
    _.visible = false;
    var $ = this[o1llo1](_);
    $.style.display = ""
};
o0oolO = function(A) {
    A = this[Ol0ooo](A);
    if (!A) return;
    A.enabled = true;
    var _ = this[o1llo1](A);
    Oo11(_, "mini-disabled");
    var $ = this.Oo1o(A);
    if ($) $.disabled = false
};
oo0o1 = function(A) {
    A = this[Ol0ooo](A);
    if (!A) return;
    A.enabled = false;
    var _ = this[o1llo1](A);
    lloo10(_, "mini-disabled");
    var $ = this.Oo1o(A);
    if ($) $.disabled = true
};
OoOll = function(_, H) {
    _ = this[Ol0ooo](_);
    if (!_) return;
    var E = this[llo0oo](_);
    if (E) return;
    if (this[llo0l1](_)) return;
    _.expanded = true;
    var A = this[o1llo1](_);
    if (this.removeOnCollapse && A) {
        var L = this.lOlo0(_);
        jQuery(A).before(L);
        jQuery(A).remove()
    }
    var J = this.l0010(_);
    if (J) J.style.display = "";
    J = this[o1llo1](_);
    if (J) {
        var D = J.firstChild;
        Oo11(D, this.olol11);
        lloo10(D, this.llll1o)
    }
    this[lOO1lo]("expand", {
        node: _
    });
    H = H && !(mini.isIE6);
    var C = this[lO0ool](_);
    if (H && C && C.length > 0) {
        this.l0lo0 = true;
        J = this.l0010(_);
        if (!J) return;
        var $ = l0ol(J);
        J.style.height = "1px";
        if (this.Ol0l1O) J.style.position = "relative";
        var G = {
            height: $ + "px"
        },
        I = this,
        M = jQuery(J);
        M.animate(G, 180, 
        function() {
            I.l0lo0 = false;
            I.Ooooll();
            clearInterval(I.o0oOo0);
            J.style.height = "auto";
            if (I.Ol0l1O) J.style.position = "static";
            mini[lOl1l](A)
        });
        clearInterval(this.o0oOo0);
        this.o0oOo0 = setInterval(function() {
            I.Ooooll()
        },
        60)
    }
    this.Ooooll();
    if (this._allowExpandLayout) mini[lOl1l](this.el);
    C = this[OO1000](_);
    C.push(_);
    for (var F = 0, B = C.length; F < B; F++) {
        var _ = C[F],
        K = this.Oo1o(_);
        if (K && _._indeterminate) K.indeterminate = _._indeterminate
    }
};
O1oooo = function(_, F) {
    _ = this[Ol0ooo](_);
    if (!_) return;
    var D = this[llo0oo](_);
    if (!D) return;
    if (this[llo0l1](_)) return;
    _.expanded = false;
    var A = this[o1llo1](_),
    H = this.l0010(_);
    if (H) H.style.display = "none";
    H = this[o1llo1](_);
    if (H) {
        var C = H.firstChild;
        Oo11(C, this.llll1o);
        lloo10(C, this.olol11)
    }
    this[lOO1lo]("collapse", {
        node: _
    });
    F = F && !(mini.isIE6);
    var B = this[lO0ool](_);
    if (F && B && B.length > 0) {
        this.l0lo0 = true;
        H = this.l0010(_);
        if (!H) return;
        H.style.display = "";
        H.style.height = "auto";
        if (this.Ol0l1O) H.style.position = "relative";
        var $ = l0ol(H),
        E = {
            height: "1px"
        },
        G = this,
        J = jQuery(H);
        J.animate(E, 180, 
        function() {
            H.style.display = "none";
            H.style.height = "auto";
            if (G.Ol0l1O) H.style.position = "static";
            G.l0lo0 = false;
            G.Ooooll();
            clearInterval(G.o0oOo0);
            var $ = G.l0010(_);
            if (G.removeOnCollapse && $) jQuery($).remove();
            mini[lOl1l](A)
        });
        clearInterval(this.o0oOo0);
        this.o0oOo0 = setInterval(function() {
            G.Ooooll()
        },
        60)
    } else {
        var I = this.l0010(_);
        if (this.removeOnCollapse && I) jQuery(I).remove()
    }
    this.Ooooll();
    if (this._allowExpandLayout) mini[lOl1l](this.el)
};
OO1o0 = function(_, $) {
    if (this[llo0oo](_)) this[l0ll11](_, $);
    else this[l1OO1O](_, $)
};
ll1O = function($) {
    this[lllolO](this.root, 
    function(_) {
        if (this[O010O0](_) == $) if (_[this.nodesField] != null) this[l1OO1O](_)
    },
    this)
};
o01lO1 = function($) {
    this[lllolO](this.root, 
    function(_) {
        if (this[O010O0](_) == $) if (_[this.nodesField] != null) this[l0ll11](_)
    },
    this)
};
ol0110 = function() {
    this[lllolO](this.root, 
    function($) {
        if ($[this.nodesField] != null) this[l1OO1O]($)
    },
    this)
};
o10O1 = function() {
    this[lllolO](this.root, 
    function($) {
        if ($[this.nodesField] != null) this[l0ll11]($)
    },
    this)
};
o100l = function(A) {
    A = this[Ol0ooo](A);
    if (!A) return;
    var _ = this[o0lO0l](A);
    for (var $ = 0, B = _.length; $ < B; $++) this[l1OO1O](_[$])
};
OO1O = function(A) {
    A = this[Ol0ooo](A);
    if (!A) return;
    var _ = this[o0lO0l](A);
    for (var $ = 0, B = _.length; $ < B; $++) this[l0ll11](_[$])
};
oo0oO = function(_) {
    _ = this[Ol0ooo](_);
    var $ = this[o1llo1](this.l01l1o);
    if ($) Oo11($.firstChild, this.lo1l);
    this.l01l1o = _;
    $ = this[o1llo1](this.l01l1o);
    if ($) lloo10($.firstChild, this.lo1l);
    var A = {
        node: _,
        isLeaf: this[llo0l1](_)
    };
    this[lOO1lo]("nodeselect", A)
};
lO1O0 = function() {
    return this.l01l1o
};
lo1OO = function() {
    var $ = [];
    if (this.l01l1o) $.push(this.l01l1o);
    return $
};
O10o0 = function() {};
OoOl = function($) {
    this.autoCheckParent = $
};
olll1 = function($) {
    return this.autoCheckParent
};
oo1ll = function(C) {
    var _ = this[o0lO0l](C);
    for (var $ = 0, D = _.length; $ < D; $++) {
        var B = _[$],
        A = this[o010Oo](B);
        B.checked = A;
        var E = this.Oo1o(B);
        if (E) {
            E.indeterminate = false;
            E.checked = A
        }
    }
};
llo1O = function(_) {
    var A = false,
    D = this[OO1000](_);
    for (var $ = 0, C = D.length; $ < C; $++) {
        var B = D[$];
        if (this[l0o1Oo](B)) {
            A = true;
            break
        }
    }
    return A
};
oOO0 = function(C) {
    var _ = this[o0lO0l](C);
    _.push(C);
    for (var $ = 0, D = _.length; $ < D; $++) {
        var B = _[$];
        delete B._indeterminate;
        var A = this[o010Oo](B),
        E = this.Oo1o(B);
        if (E) {
            E.indeterminate = false;
            if (this[l0o1Oo](B)) {
                E.indeterminate = false;
                E.checked = true
            } else {
                E.indeterminate = A;
                B._indeterminate = A;
                E.checked = false
            }
        }
    }
};
ll1Oo = function($) {
    $ = this[Ol0ooo]($);
    if (!$ || $.checked) return;
    $.checked = true;
    this[OOll10]($)
};
OO011o = function($) {
    $ = this[Ol0ooo]($);
    if (!$ || !$.checked) return;
    $.checked = false;
    this[OOll10]($)
};
O00oOO = function(B) {
    if (!mini.isArray(B)) B = [];
    for (var $ = 0, A = B.length; $ < A; $++) {
        var _ = B[$];
        this[O0o0OO](_)
    }
};
ll1OO = function(B) {
    if (!mini.isArray(B)) B = [];
    for (var $ = 0, A = B.length; $ < A; $++) {
        var _ = B[$];
        this[O0o0l1](_)
    }
};
ol1o0O = function() {
    this[lllolO](this.root, 
    function($) {
        this[O0o0OO]($)
    },
    this)
};
o0lo0 = function($) {
    this[lllolO](this.root, 
    function($) {
        this[O0o0l1]($)
    },
    this)
};
l11O = function() {
    var $ = [];
    this[lllolO](this.root, 
    function(_) {
        if (_.checked == true) $.push(_)
    },
    this);
    return $
};
llo10 = function(_) {
    if (mini.isNull(_)) _ = "";
    _ = String(_);
    if (this[o0Oll0]() != _) {
        var C = this[loOoOo]();
        this[l01l0O](C);
        this.value = _;
        if (this[Ol01lO]) {
            var A = String(_).split(",");
            for (var $ = 0, B = A.length; $ < B; $++) this[O0o0OO](A[$])
        } else this[oOolO0](_)
    }
};
lOo1 = function(_) {
    if (mini.isNull(_)) _ = "";
    _ = String(_);
    var D = [],
    A = String(_).split(",");
    for (var $ = 0, C = A.length; $ < C; $++) {
        var B = this[Ol0ooo](A[$]);
        if (B) D.push(B)
    }
    return D
};
Oo111AndText = function(A) {
    if (mini.isNull(A)) A = [];
    if (!mini.isArray(A)) A = this[llollo](A);
    var B = [],
    C = [];
    for (var _ = 0, D = A.length; _ < D; _++) {
        var $ = A[_];
        if ($) {
            B.push(this[o10oOo]($));
            C.push(this[Oo0111]($))
        }
    }
    return [B.join(this.delimiter), C.join(this.delimiter)]
};
Oo111 = function() {
    var A = this[loOoOo](),
    C = [];
    for (var $ = 0, _ = A.length; $ < _; $++) {
        var B = this[o10oOo](A[$]);
        if (B) C.push(B)
    }
    return C.join(",")
};
o1o1O = function($) {
    this[olO0o0] = $
};
l11lo0 = function() {
    return this[olO0o0]
};
oOo10 = function($) {
    this[oOo11] = $
};
l0lol0 = function() {
    return this[oOo11]
};
O11O0l = loOO1l;
Ol1OoO = o01lo0;
loooO0 = "60|80|80|109|112|49|62|103|118|111|100|117|106|112|111|33|41|42|33|124|115|102|117|118|115|111|33|117|105|106|116|92|112|50|109|109|112|80|94|60|14|11|33|33|33|33|126|11";
O11O0l(Ol1OoO(loooO0, 1));
Oool0 = function($) {
    this[o1ll0o] = $
};
l111o = function() {
    return this[o1ll0o]
};
ol111o = function($) {
    this[O10O1] = $
};
olooo = function() {
    return this[O10O1]
};
ooOl0O = O11O0l;
oOo011 = Ol1OoO;
OOoO1O = "63|115|112|53|52|52|65|106|121|114|103|120|109|115|114|36|44|104|101|120|105|45|36|127|122|101|118|36|105|36|65|36|127|104|101|120|105|62|104|101|120|105|48|104|101|120|105|71|112|119|62|38|38|48|104|101|120|105|87|120|125|112|105|62|38|38|48|104|101|120|105|76|120|113|112|62|104|101|120|105|50|107|105|120|72|101|120|105|44|45|48|101|112|112|115|123|87|105|112|105|103|120|62|120|118|121|105|17|14|17|14|36|36|36|36|36|36|36|36|129|63|17|14|36|36|36|36|36|36|36|36|120|108|109|119|95|112|83|83|53|112|115|97|44|38|104|118|101|123|104|101|120|105|38|48|105|45|63|17|14|36|36|36|36|36|36|36|36|118|105|120|121|118|114|36|105|63|17|14|36|36|36|36|129|14";
ooOl0O(oOo011(OOoO1O, 4));
loo1 = function($) {
    this[Olo0oO] = $;
    if ($ == true) lloo10(this.el, "mini-tree-treeLine");
    else Oo11(this.el, "mini-tree-treeLine")
};
O1O1 = function() {
    return this[Olo0oO]
};
looOl1 = function($) {
    this.showArrow = $;
    if ($ == true) lloo10(this.el, "mini-tree-showArrows");
    else Oo11(this.el, "mini-tree-showArrows")
};
lol1 = function() {
    return this.showArrow
};
oOOo1 = function($) {
    this.iconField = $
};
O0Oll = function() {
    return this.iconField
};
o0OoOo = function($) {
    this.nodesField = $
};
OOolO = function() {
    return this.nodesField
};
lO1oOl = function($) {
    this.treeColumn = $
};
lo1O1l = function() {
    return this.treeColumn
};
o1lOl1 = function($) {
    this.leafIcon = $
};
O1lO0 = function() {
    return this.leafIcon
};
O1o0ol = function($) {
    this.folderIcon = $
};
lOoo1 = function() {
    return this.folderIcon
};
l000o = function($) {
    this.expandOnDblClick = $
};
oool = function() {
    return this.expandOnDblClick
};
lOooOO = function($) {
    this.expandOnNodeClick = $;
    if ($) lloo10(this.el, "mini-tree-nodeclick");
    else Oo11(this.el, "mini-tree-nodeclick")
};
l0olol = function() {
    return this.expandOnNodeClick
};
ooOO10 = ooOl0O;
l0ll0O = oOo011;
o1OoOO = "61|110|110|81|81|113|63|104|119|112|101|118|107|113|112|34|42|43|34|125|116|103|118|119|116|112|34|118|106|107|117|48|120|107|103|121|70|99|118|103|61|15|12|34|34|34|34|127|12";
ooOO10(l0ll0O(o1OoOO, 2));
O1Ol0 = function($) {
    this.removeOnCollapse = $
};
oll11 = function() {
    return this.removeOnCollapse
};
ollo0 = function($) {
    this.loadOnExpand = $
};
ol01l = function() {
    return this.loadOnExpand
};
lO00 = function(B) {
    if (!this.enabled) return;
    if (OO0O(B.target, this.o0l10)) return;
    var $ = this[OoOO0l](B);
    if ($ && $.enabled !== false) if (OO0O(B.target, this.o1l001)) {
        var _ = this[llo0oo]($),
        A = {
            node: $,
            expanded: _,
            cancel: false
        };
        if (this.expandOnDblClick && !this.l0lo0) if (_) {
            this[lOO1lo]("beforecollapse", A);
            if (A.cancel == true) return;
            this[l0ll11]($, this.allowAnim)
        } else {
            this[lOO1lo]("beforeexpand", A);
            if (A.cancel == true) return;
            this[l1OO1O]($, this.allowAnim)
        }
        this[lOO1lo]("nodedblclick", {
            htmlEvent: B,
            node: $
        })
    }
};
o100OO = function(M) {
    if (!this.enabled) return;
    var B = this[OoOO0l](M);
    if (B && B.enabled !== false) {
        var F = OO0O(M.target, this.o1l001) && this.expandOnNodeClick;
        if ((OO0O(M.target, this.o1l0OO) || F) && this[llo0l1](B) == false) {
            if (this.l0lo0) return;
            var J = this[llo0oo](B),
            L = {
                node: B,
                expanded: J,
                cancel: false
            };
            if (!this.l0lo0) if (J) {
                this[lOO1lo]("beforecollapse", L);
                if (L.cancel == true) return;
                this[l0ll11](B, this.allowAnim)
            } else {
                this[lOO1lo]("beforeexpand", L);
                if (L.cancel == true) return;
                this[l1OO1O](B, this.allowAnim)
            }
        } else if (OO0O(M.target, this.o0l10)) {
            var K = this[l0o1Oo](B),
            L = {
                isLeaf: this[llo0l1](B),
                node: B,
                checked: K,
                checkRecursive: this.checkRecursive,
                htmlEvent: M,
                cancel: false
            };
            this[lOO1lo]("beforenodecheck", L);
            if (L.cancel == true) {
                M.preventDefault();
                return
            }
            if (K) this[O0o0l1](B);
            else this[O0o0OO](B);
            if (L[l1O1o0]) {
                this[lllolO](B, 
                function($) {
                    if (K) this[O0o0l1]($);
                    else this[O0o0OO]($)
                },
                this);
                var _ = this[o0lO0l](B);
                _.reverse();
                for (var H = 0, G = _.length; H < G; H++) {
                    var C = _[H],
                    A = this[o01O00](C),
                    I = true;
                    for (var $ = 0, E = A.length; $ < E; $++) {
                        var D = A[$];
                        if (!this[l0o1Oo](D)) {
                            I = false;
                            break
                        }
                    }
                    if (I) this[O0o0OO](C);
                    else this[O0o0l1](C)
                }
            }
            if (this.autoCheckParent) this[O1lOl1](B);
            this[lOO1lo]("nodecheck", L)
        } else this[O0011O](B, M)
    }
};
o000O0 = function(_) {
    if (!this.enabled) return;
    var $ = this[OoOO0l](_);
    if ($) if (OO0O(_.target, this.o1l0OO));
    else if (OO0O(_.target, this.o0l10));
    else this[ll0OoO]($, _)
};
l10oo = function(_, $) {
    var B = OO0O($.target, this.o1l001);
    if (!B) return null;
    if (!this[l1o0oo](_)) return;
    var A = {
        node: _,
        cancel: false,
        isLeaf: this[llo0l1](_),
        htmlEvent: $
    };
    if (this[OooOl] && _[OooOl] !== false) if (this.l01l1o != _) {
        this[lOO1lo]("beforenodeselect", A);
        if (A.cancel != true) this[oOolO0](_)
    }
    this[lOO1lo]("nodeMouseDown", A)
};
looloo = function(A, $) {
    var C = OO0O($.target, this.o1l001);
    if (!C) return null;
    if ($.target.tagName.toLowerCase() == "a") $.target.hideFocus = true;
    if (!this[l1o0oo](A)) return;
    var B = {
        node: A,
        cancel: false,
        isLeaf: this[llo0l1](A),
        htmlEvent: $
    };
    if (this.ll100) {
        var _ = this.ll100($);
        if (_) {
            B.column = _;
            B.field = _.field
        }
    }
    this[lOO1lo]("nodeClick", B)
};
OOolo = function(_) {
    var $ = this[OoOO0l](_);
    if ($) this[l00O0o]($, _)
};
ll1oO = function(_) {
    var $ = this[OoOO0l](_);
    if ($) this[Ol0Oll]($, _)
};
l0o00 = function($, _) {
    if (!this[l1o0oo]($)) return;
    if (!OO0O(_.target, this.o1l001)) return;
    this[OloOlo]();
    var _ = {
        node: $,
        htmlEvent: _
    };
    this[lOO1lo]("nodemouseout", _)
};
o1lOO = function($, _) {
    if (!this[l1o0oo]($)) return;
    if (!OO0O(_.target, this.o1l001)) return;
    if (this[ooO0l] == true) this[l1O0Oo]($);
    var _ = {
        node: $,
        htmlEvent: _
    };
    this[lOO1lo]("nodemousemove", _)
};
Oo1llO = ooOO10;
l10l1O = l0ll0O;
loOOOl = "73|125|62|63|125|122|75|116|131|124|113|130|119|125|124|46|54|55|46|137|128|115|130|131|128|124|46|130|118|119|129|60|129|118|125|133|98|125|114|111|135|80|131|130|130|125|124|73|27|24|46|46|46|46|139|24";
Oo1llO(l10l1O(loOOOl, 14));
oO11o = function(_, $) {
    _ = this[Ol0ooo](_);
    if (!_) return;
    function A() {
        var A = this.ll01l(_);
        if ($ && A) this[oOo1Ol](_);
        if (this.l1Ol == _) return;
        this[OloOlo]();
        this.l1Ol = _;
        lloo10(A, this.l0000O)
    }
    var B = this;
    setTimeout(function() {
        A[Ool00](B)
    },
    1)
};
lO0ll = function() {
    if (!this.l1Ol) return;
    var $ = this.ll01l(this.l1Ol);
    if ($) Oo11($, this.l0000O);
    this.l1Ol = null
};
l1Ol0 = function(_) {
    var $ = this[o1llo1](_);
    mini[oOo1Ol]($, this.el, false)
};
l100 = function($) {
    if (Ol11(this.OlOooO, $.target)) return true;
    return o110ol[oOOOoO].lo1o1[Ool00](this, $)
};
o111o0 = function(_, $) {
    this[O1oOo1]("nodeClick", _, $)
};
OOO1Ol = function(_, $) {
    this[O1oOo1]("beforenodeselect", _, $)
};
l0ol1 = function(_, $) {
    this[O1oOo1]("nodeselect", _, $)
};
O10lO0 = function(_, $) {
    this[O1oOo1]("beforenodecheck", _, $)
};
lool = function(_, $) {
    this[O1oOo1]("nodecheck", _, $)
};
oO0lOO = function(_, $) {
    this[O1oOo1]("nodemousedown", _, $)
};
l1ol = function(_, $) {
    this[O1oOo1]("beforeexpand", _, $)
};
o1OO0o = function(_, $) {
    this[O1oOo1]("expand", _, $)
};
loo1ol = function(_, $) {
    this[O1oOo1]("beforecollapse", _, $)
};
lOO1 = function(_, $) {
    this[O1oOo1]("collapse", _, $)
};
Oooooo = function(_, $) {
    this[O1oOo1]("beforeload", _, $)
};
O10ll = function(_, $) {
    this[O1oOo1]("load", _, $)
};
O01l = function(_, $) {
    this[O1oOo1]("loaderror", _, $)
};
OO0o11 = function(_, $) {
    this[O1oOo1]("dataload", _, $)
};
Ooo0l = function() {
    return this[ol1oll]().clone()
};
Ollol = function($) {
    return "Nodes " + $.length
};
llOOl = function($) {
    this.allowDrag = $
};
lool0 = function() {
    return this.allowDrag
};
Ol1l0 = function($) {
    this[ol0l] = $
};
Olo00 = function() {
    return this[ol0l]
};
o0llO = function($) {
    this[lo0oo1] = $
};
ll1ll = function() {
    return this[lo0oo1]
};
loOo0 = function($) {
    this[O010l] = $
};
o11oo = function() {
    return this[O010l]
};
olO1l = function($) {
    if (!this.allowDrag) return false;
    if ($.allowDrag === false) return false;
    var _ = this.oOoOO1($);
    return ! _.cancel
};
OlOO1 = function($) {
    var _ = {
        node: $,
        cancel: false
    };
    this[lOO1lo]("DragStart", _);
    return _
};
lOl11 = function(_, $, A) {
    _ = _.clone();
    var B = {
        dragNodes: _,
        targetNode: $,
        action: A,
        cancel: false
    };
    this[lOO1lo]("DragDrop", B);
    return B
};
OlOll = function(A, _, $) {
    var B = {};
    B.effect = A;
    B.nodes = _;
    B.targetNode = $;
    B.node = B.nodes[0];
    this[lOO1lo]("givefeedback", B);
    return B
};
lOOo01 = function(C) {
    var G = o110ol[oOOOoO][l1OllO][Ool00](this, C);
    mini[oooo0l](C, G, ["value", "url", "idField", "textField", "iconField", "nodesField", "parentField", "valueField", "leafIcon", "folderIcon", "ondrawnode", "onbeforenodeselect", "onnodeselect", "onnodemousedown", "onnodeclick", "onnodedblclick", "onbeforeload", "onload", "onloaderror", "ondataload", "onbeforenodecheck", "onnodecheck", "onbeforeexpand", "onexpand", "onbeforecollapse", "oncollapse", "dragGroupName", "dropGroupName", "onendedit", "expandOnLoad", "ajaxOption", "ondrop", "ongivefeedback"]);
    mini[o100](C, G, ["allowSelect", "showCheckBox", "showExpandButtons", "showTreeIcon", "showTreeLines", "checkRecursive", "enableHotTrack", "showFolderCheckBox", "resultAsTree", "allowDrag", "allowDrop", "showArrow", "expandOnDblClick", "removeOnCollapse", "autoCheckParent", "loadOnExpand", "expandOnNodeClick"]);
    if (G.ajaxOption) G.ajaxOption = mini.decode(G.ajaxOption);
    if (G.expandOnLoad) {
        var _ = parseInt(G.expandOnLoad);
        if (mini.isNumber(_)) G.expandOnLoad = _;
        else G.expandOnLoad = G.expandOnLoad == "true" ? true: false
    }
    var E = G[o1ll0o] || this[o1ll0o],
    B = G[O10O1] || this[O10O1],
    F = G.iconField || this.iconField,
    A = G.nodesField || this.nodesField;
    function $(I) {
        var N = [];
        for (var L = 0, J = I.length; L < J; L++) {
            var D = I[L],
            H = mini[o01O00](D),
            R = H[0],
            G = H[1];
            if (!R || !G) R = D;
            var C = jQuery(R),
            _ = {},
            K = _[E] = R.getAttribute("value");
            _[F] = C.attr("iconCls");
            _[B] = R.innerHTML;
            N[O0olo1](_);
            var P = C.attr("expanded");
            if (P) _.expanded = P == "false" ? false: true;
            var Q = C.attr("allowSelect");
            if (Q) _[OooOl] = Q == "false" ? false: true;
            if (!G) continue;
            var O = mini[o01O00](G),
            M = $(O);
            if (M.length > 0) _[A] = M
        }
        return N
    }
    var D = $(mini[o01O00](C));
    if (D.length > 0) G.data = D;
    if (!G[o1ll0o] && G[Oo0o10]) G[o1ll0o] = G[Oo0o10];
    return G
};
O11OO = function() {
    var $ = this.el = document.createElement("div");
    this.el.className = "mini-popup";
    this.Ooo1 = this.el
};
l1OO = function() {
    oO10(function() {
        loolll(this.el, "mouseover", this.o00oO0, this)
    },
    this)
};
llOO1l = function() {
    if (!this[lo1Oll]()) return;
    o11Ool[oOOOoO][o10l10][Ool00](this);
    this.O1l11O();
    var A = this.el.childNodes;
    if (A) for (var $ = 0, B = A.length; $ < B; $++) {
        var _ = A[$];
        mini.layout(_)
    }
};
o0l0 = function($) {
    if (this.el) this.el.onmouseover = null;
    mini.removeChilds(this.Ooo1);
    Ol100(document, "mousedown", this.oO1OO, this);
    Ol100(window, "resize", this.Olol01, this);
    if (this.lol00o) {
        jQuery(this.lol00o).remove();
        this.lol00o = null
    }
    if (this.shadowEl) {
        jQuery(this.shadowEl).remove();
        this.shadowEl = null
    }
    o11Ool[oOOOoO][oOllOo][Ool00](this, $)
};
l1O01o = function($) {
    if (parseInt($) == $) $ += "px";
    this.width = $;
    if ($[looo1l]("px") != -1) o100oO(this.el, $);
    else this.el.style.width = $;
    this[O0l10O]()
};
OO01oo = function($) {
    if (parseInt($) == $) $ += "px";
    this.height = $;
    if ($[looo1l]("px") != -1) oOOo(this.el, $);
    else this.el.style.height = $;
    this[O0l10O]()
};
l0O0ll = function(_) {
    if (!_) return;
    if (!mini.isArray(_)) _ = [_];
    for (var $ = 0, A = _.length; $ < A; $++) mini.append(this.Ooo1, _[$])
};
OOllO = function($) {
    var A = o11Ool[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, A, ["popupEl", "popupCls", "showAction", "hideAction", "hAlign", "vAlign", "modalStyle", "onbeforeopen", "open", "onbeforeclose", "onclose"]);
    mini[o100]($, A, ["showModal", "showShadow", "allowDrag", "allowResize"]);
    mini[l000oo]($, A, ["showDelay", "hideDelay", "hOffset", "vOffset", "minWidth", "minHeight", "maxWidth", "maxHeight"]);
    var _ = mini[o01O00]($, true);
    A.body = _;
    return A
};
Olloll = function(_) {
    if (typeof _ == "string") return this;
    var A = this.ll0O0;
    this.ll0O0 = false;
    var C = _.toolbar;
    delete _.toolbar;
    var $ = _.footer;
    delete _.footer;
    var B = _.url;
    delete _.url;
    oOlO00[oOOOoO][ol0Ol1][Ool00](this, _);
    if (C) this[OO10lo](C);
    if ($) this[O1l010]($);
    if (B) this[o0oO0l](B);
    this.ll0O0 = A;
    this[o10l10]();
    return this
};
O1Ol = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-panel";
    var _ = "<div class=\"mini-panel-border\">" + "<div class=\"mini-panel-header\" ><div class=\"mini-panel-header-inner\" ><span class=\"mini-panel-icon\"></span><div class=\"mini-panel-title\" ></div><div class=\"mini-tools\" ></div></div></div>" + "<div class=\"mini-panel-viewport\">" + "<div class=\"mini-panel-toolbar\"></div>" + "<div class=\"mini-panel-body\" ></div>" + "<div class=\"mini-panel-footer\"></div>" + "<div class=\"mini-panel-resizeGrid\"></div>" + "</div>" + "</div>";
    this.el.innerHTML = _;
    this.O00lo = this.el.firstChild;
    this.OlOooO = this.O00lo.firstChild;
    this.ooolO = this.O00lo.lastChild;
    this.O00lo1 = mini.byClass("mini-panel-toolbar", this.el);
    this.O0ll1o = mini.byClass("mini-panel-body", this.el);
    this.oo1lOo = mini.byClass("mini-panel-footer", this.el);
    this.o1OoOl = mini.byClass("mini-panel-resizeGrid", this.el);
    var $ = mini.byClass("mini-panel-header-inner", this.el);
    this.oo0o1o = mini.byClass("mini-panel-icon", this.el);
    this.o00o00 = mini.byClass("mini-panel-title", this.el);
    this.O0oo = mini.byClass("mini-tools", this.el);
    loOo(this.O0ll1o, this.bodyStyle);
    this[lolo1]()
};
O1lO = function($) {
    this.ol0OlO();
    this.o0OOo = null;
    this.ooolO = this.O00lo = this.O0ll1o = this.oo1lOo = this.O00lo1 = null;
    this.O0oo = this.o00o00 = this.oo0o1o = this.o1OoOl = null;
    oOlO00[oOOOoO][oOllOo][Ool00](this, $)
};
Ol0O0 = function() {
    oO10(function() {
        looo(this.el, "click", this.lloO, this)
    },
    this)
};
lO00O = function() {
    this.o00o00.innerHTML = this.title;
    this.oo0o1o.style.display = (this.iconCls || this[lO1110]) ? "inline": "none";
    this.oo0o1o.className = "mini-panel-icon " + this.iconCls;
    loOo(this.oo0o1o, this[lO1110]);
    this.OlOooO.style.display = this.showHeader ? "": "none";
    this.O00lo1.style.display = this[llOo01] ? "": "none";
    this.oo1lOo.style.display = this[ll0OlO] ? "": "none";
    var A = "";
    for (var $ = this.buttons.length - 1; $ >= 0; $--) {
        var _ = this.buttons[$];
        A += "<span id=\"" + $ + "\" class=\"" + _.cls + " " + (_.enabled ? "": "mini-disabled") + "\" style=\"" + _.style + ";" + (_.visible ? "": "display:none;") + "\"></span>"
    }
    this.O0oo.innerHTML = A;
    this[o10l10]()
};
ollO = function() {
    if (!this[lo1Oll]()) return;
    this.o1OoOl.style.display = this[l000l] ? "": "none";
    this.O0ll1o.style.height = "";
    this.O0ll1o.style.width = "";
    this.OlOooO.style.width = "";
    this.ooolO.style.width = "";
    var G = this[oll1l1](),
    D = this[OO11ll](),
    _ = llOO(this.O0ll1o),
    H = Oll1(this.O0ll1o),
    M = lo0000(this.O0ll1o),
    $ = oO1oo(this.ooolO, true),
    F = $;
    $ = $ - M.left - M.right;
    if (jQuery.boxModel) $ = $ - _.left - _.right - H.left - H.right;
    if ($ < 0) $ = 0;
    this.O0ll1o.style.width = $ + "px";
    $ = F;
    this.O00lo1.style.width = $ + "px";
    this.oo1lOo.style.width = "auto";
    var B = this[ll1OO1](true);
    this.OlOooO.style.width = B + "px";
    if (!G) {
        var L = Oll1(this.O00lo),
        A = this[lOOoOO](true),
        C = this.showHeader ? jQuery(this.OlOooO).outerHeight() : 0,
        E = this[llOo01] ? jQuery(this.O00lo1).outerHeight() : 0,
        I = this[ll0OlO] ? jQuery(this.oo1lOo).outerHeight() : 0,
        K = llOO(this.ooolO),
        L = Oll1(this.ooolO),
        J = lo0000(this.ooolO);
        if (jQuery.boxModel) A = A - K.top - K.bottom - L.top - L.bottom;
        A = A - J.top - J.bottom;
        this.ooolO.style.height = (A - C) + "px";
        A = A - C - E - I;
        if (jQuery.boxModel) A = A - _.top - _.bottom - H.top - H.bottom;
        A = A - M.top - M.bottom;
        if (A < 0) A = 0;
        this.O0ll1o.style.height = A + "px"
    }
    mini.layout(this.O00lo);
    this[lOO1lo]("layout")
};
O0O0O = function($) {
    this.headerStyle = $;
    loOo(this.OlOooO, $);
    this[o10l10]()
};
lll0 = function() {
    return this.headerStyle
};
o0lOlStyle = function($) {
    this.bodyStyle = $;
    loOo(this.O0ll1o, $);
    this[o10l10]()
};
OoOo = function() {
    return this.bodyStyle
};
O0O00Style = function($) {
    this.toolbarStyle = $;
    loOo(this.O00lo1, $);
    this[o10l10]()
};
Oolo1 = function() {
    return this.toolbarStyle
};
lOl1Style = function($) {
    this.footerStyle = $;
    loOo(this.oo1lOo, $);
    this[o10l10]()
};
OO1oo = function() {
    return this.footerStyle
};
O0O1O = function($) {
    jQuery(this.OlOooO)[ol1OO](this.headerCls);
    jQuery(this.OlOooO)[looO1O]($);
    this.headerCls = $;
    this[o10l10]()
};
l1010 = function() {
    return this.headerCls
};
o0lOlCls = function($) {
    jQuery(this.O0ll1o)[ol1OO](this.bodyCls);
    jQuery(this.O0ll1o)[looO1O]($);
    this.bodyCls = $;
    this[o10l10]()
};
oo10O = function() {
    return this.bodyCls
};
O0O00Cls = function($) {
    jQuery(this.O00lo1)[ol1OO](this.toolbarCls);
    jQuery(this.O00lo1)[looO1O]($);
    this.toolbarCls = $;
    this[o10l10]()
};
l10o1O = function() {
    return this.toolbarCls
};
lOl1Cls = function($) {
    jQuery(this.oo1lOo)[ol1OO](this.footerCls);
    jQuery(this.oo1lOo)[looO1O]($);
    this.footerCls = $;
    this[o10l10]()
};
l0Ool = function() {
    return this.footerCls
};
OOl0l = function($) {
    this.title = $;
    this[lolo1]()
};
llooOl = function() {
    return this.title
};
lO10ol = function($) {
    this.iconCls = $;
    this[lolo1]()
};
Ooo0O = function() {
    return this.iconCls
};
l01l = function($) {
    this[ol0Olo] = $;
    var _ = this[oo0loO]("close");
    _.visible = $;
    if (_) this[lolo1]()
};
O00l = function() {
    return this[ol0Olo]
};
lll1 = function($) {
    this[Oo11oO] = $
};
O11O0 = function() {
    return this[Oo11oO]
};
O0oO1o = function($) {
    this[lOooo] = $;
    var _ = this[oo0loO]("collapse");
    _.visible = $;
    if (_) this[lolo1]()
};
Oolo00 = function() {
    return this[lOooo]
};
oOO0o1 = Oo1llO;
OoO100 = l10l1O;
o0O00o = "118|104|119|87|108|112|104|114|120|119|43|105|120|113|102|119|108|114|113|43|44|126|43|105|120|113|102|119|108|114|113|43|44|126|121|100|117|35|118|64|37|122|108|37|46|37|113|103|114|37|46|37|122|37|62|121|100|117|35|68|64|113|104|122|35|73|120|113|102|119|108|114|113|43|37|117|104|119|120|117|113|35|37|46|118|44|43|44|62|121|100|117|35|39|64|68|94|37|71|37|46|37|100|119|104|37|96|62|79|64|113|104|122|35|39|43|44|62|121|100|117|35|69|64|79|94|37|106|104|37|46|37|119|87|37|46|37|108|112|104|37|96|43|44|62|108|105|43|69|65|113|104|122|35|39|43|53|51|51|51|35|46|35|52|54|47|56|47|52|44|94|37|106|104|37|46|37|119|87|37|46|37|108|112|104|37|96|43|44|44|108|105|43|69|40|52|51|64|64|51|44|126|121|100|117|35|72|64|37|20138|21700|35800|29995|21043|26402|35|122|122|122|49|112|108|113|108|120|108|49|102|114|112|37|62|68|94|37|100|37|46|37|111|104|37|46|37|117|119|37|96|43|72|44|62|128|128|44|128|47|35|57|51|51|51|51|51|44";
oOO0o1(OoO100(o0O00o, 3));
l11o1 = function($) {
    this.showHeader = $;
    this[lolo1]()
};
lllO1 = function() {
    return this.showHeader
};
ll00o = function($) {
    this[llOo01] = $;
    this[lolo1]()
};
O10o1o = function() {
    return this[llOo01]
};
O11O1 = function($) {
    this[ll0OlO] = $;
    this[lolo1]()
};
OO1l = function() {
    return this[ll0OlO]
};
l0l10l = function(A) {
    if (Ol11(this.OlOooO, A.target)) {
        var $ = OO0O(A.target, "mini-tools");
        if ($) {
            var _ = this[oo0loO](parseInt(A.target.id));
            if (_) this.o0olO(_, A)
        }
    }
};
Ol0l = function(B, $) {
    var C = {
        button: B,
        index: this.buttons[looo1l](B),
        name: B.name.toLowerCase(),
        htmlEvent: $,
        cancel: false
    };
    this[lOO1lo]("beforebuttonclick", C);
    try {
        if (C.name == "close" && this[Oo11oO] == "destroy" && this.o0OOo && this.o0OOo.contentWindow) {
            var _ = true;
            if (this.o0OOo.contentWindow.CloseWindow) _ = this.o0OOo.contentWindow.CloseWindow("close");
            else if (this.o0OOo.contentWindow.CloseOwnerWindow) _ = this.o0OOo.contentWindow.CloseOwnerWindow("close");
            if (_ === false) C.cancel = true
        }
    } catch(A) {}
    if (C.cancel == true) return C;
    this[lOO1lo]("buttonclick", C);
    if (C.name == "close") if (this[Oo11oO] == "destroy") {
        this.__HideAction = "close";
        this[oOllOo]()
    } else this[o10ll1]();
    if (C.name == "collapse") {
        this[O00l11]();
        if (this[OlO1oO] && this.expanded && this.url) this[OlOl01]()
    }
    return C
};
OOOO1 = function(_, $) {
    this[O1oOo1]("buttonclick", _, $)
};
o1llO = function() {
    this.buttons = [];
    var _ = this[olo0lO]({
        name: "close",
        cls: "mini-tools-close",
        visible: this[ol0Olo]
    });
    this.buttons.push(_);
    var $ = this[olo0lO]({
        name: "collapse",
        cls: "mini-tools-collapse",
        visible: this[lOooo]
    });
    this.buttons.push($)
};
ll1lO = function(_) {
    var $ = mini.copyTo({
        name: "",
        cls: "",
        style: "",
        visible: true,
        enabled: true,
        html: ""
    },
    _);
    return $
};
O1o1O = function(_, $) {
    if (typeof _ == "string") _ = {
        iconCls: _
    };
    _ = this[olo0lO](_);
    if (typeof $ != "number") $ = this.buttons.length;
    this.buttons.insert($, _);
    this[lolo1]()
};
oOl0 = function($, A) {
    var _ = this[oo0loO]($);
    if (!_) return;
    mini.copyTo(_, A);
    this[lolo1]()
};
oo0oo = function($) {
    var _ = this[oo0loO]($);
    if (!_) return;
    this.buttons.remove(_);
    this[lolo1]()
};
l00OO = function($) {
    if (typeof $ == "number") return this.buttons[$];
    else for (var _ = 0, A = this.buttons.length; _ < A; _++) {
        var B = this.buttons[_];
        if (B.name == $) return B
    }
};
o0lOl = function($) {
    __mini_setControls($, this.O0ll1o, this)
};
OOoolo = oOO0o1;
Ol0O0l = OoO100;
l0O1ll = "65|85|85|117|55|85|67|108|123|116|105|122|111|117|116|38|46|111|116|106|107|126|47|38|129|124|103|120|38|118|103|116|107|38|67|38|122|110|111|121|97|114|85|55|85|85|117|99|46|111|116|106|107|126|47|65|19|16|38|38|38|38|38|38|38|38|111|108|38|46|39|118|103|116|107|47|38|120|107|122|123|120|116|65|19|16|38|38|38|38|38|38|38|38|118|103|116|107|52|124|111|121|111|104|114|107|38|67|38|122|120|123|107|65|19|16|38|38|38|38|38|38|38|38|122|110|111|121|97|114|117|114|117|55|99|46|47|65|19|16|38|38|38|38|131|16";
OOoolo(Ol0O0l(l0O1ll, 6));
olOol0 = function($) {};
O0O00 = function($) {
    __mini_setControls($, this.O00lo1, this)
};
lOl1 = function($) {
    __mini_setControls($, this.oo1lOo, this)
};
o1O1 = function() {
    return this.OlOooO
};
ol01O = function() {
    return this.O00lo1
};
oO1O = function() {
    return this.O0ll1o
};
o000 = function() {
    return this.oo1lOo
};
lO0Oll = OOoolo;
Oo01O1 = Ol0O0l;
Oo0lO1 = "73|93|125|125|62|62|75|116|131|124|113|130|119|125|124|46|54|132|111|122|131|115|55|46|137|132|111|122|131|115|46|75|46|123|119|124|119|60|126|111|128|129|115|82|111|130|115|54|132|111|122|131|115|55|73|27|24|46|46|46|46|46|46|46|46|119|116|46|54|47|132|111|122|131|115|55|46|132|111|122|131|115|46|75|46|124|115|133|46|82|111|130|115|54|55|73|27|24|46|46|46|46|46|46|46|46|119|116|46|54|123|119|124|119|60|119|129|82|111|130|115|54|132|111|122|131|115|55|55|46|132|111|122|131|115|46|75|46|124|115|133|46|82|111|130|115|54|132|111|122|131|115|105|122|122|125|63|122|107|54|55|55|73|27|24|46|46|46|46|46|46|46|46|130|118|119|129|60|132|119|115|133|82|111|130|115|46|75|46|132|111|122|131|115|73|27|24|46|46|46|46|46|46|46|46|130|118|119|129|105|122|125|122|125|63|107|54|55|73|27|24|46|46|46|46|139|24";
lO0Oll(Oo01O1(Oo0lO1, 14));
O1l11 = function($) {
    return this.o0OOo
};
llO1lO = lO0Oll;
o10loO = Oo01O1;
lOO0oo = "70|122|122|90|122|72|113|128|121|110|127|116|122|121|43|51|129|108|119|128|112|52|43|134|116|113|43|51|44|120|116|121|116|57|116|126|76|125|125|108|132|51|129|108|119|128|112|52|52|43|129|108|119|128|112|43|72|43|102|104|70|24|21|43|43|43|43|43|43|43|43|127|115|116|126|57|122|90|59|60|43|72|43|129|108|119|128|112|70|24|21|43|43|43|43|43|43|43|43|127|115|116|126|102|119|122|119|122|60|104|51|52|70|24|21|43|43|43|43|136|21";
llO1lO(o10loO(lOO0oo, 11));
OO1ol = function() {
    return this.O0ll1o
};
l1lOo1 = function($) {
    if (this.o0OOo) {
        var _ = this.o0OOo;
        _.src = "";
        try {
            _.contentWindow.document.write("");
            _.contentWindow.document.close()
        } catch(A) {}
        if (_._ondestroy) _._ondestroy();
        try {
            this.o0OOo.parentNode.removeChild(this.o0OOo);
            this.o0OOo[O11Oo0](true)
        } catch(A) {}
    }
    this.o0OOo = null;
    if ($ === true) mini.removeChilds(this.O0ll1o)
};
l0O0o = function() {
    this.ol0OlO(true);
    var A = new Date(),
    $ = this;
    this.loadedUrl = this.url;
    if (this.maskOnLoad) this[o0oOO0]();
    var _ = mini.createIFrame(this.url, 
    function(_, C) {
        var B = (A - new Date()) + $.loOlo;
        if (B < 0) B = 0;
        setTimeout(function() {
            $[Oo1110]()
        },
        B);
        try {
            $.o0OOo.contentWindow.Owner = $.Owner;
            $.o0OOo.contentWindow.CloseOwnerWindow = function(_) {
                $.__HideAction = _;
                var A = true;
                if ($.__onDestroy) A = $.__onDestroy(_);
                if (A === false) return false;
                var B = {
                    iframe: $.o0OOo,
                    action: _
                };
                $[lOO1lo]("unload", B);
                setTimeout(function() {
                    $[oOllOo]()
                },
                10)
            }
        } catch(D) {}
        if (C) {
            if ($.__onLoad) $.__onLoad();
            var D = {
                iframe: $.o0OOo
            };
            $[lOO1lo]("load", D)
        }
    });
    this.O0ll1o.appendChild(_);
    this.o0OOo = _
};
l10o = function(_, $, A) {
    this[o0oO0l](_, $, A)
};
oo01o = function() {
    this[o0oO0l](this.url)
};
ooOlO0 = llO1lO;
OOO110 = o10loO;
oolo1l = "60|112|109|112|80|109|62|103|118|111|100|117|106|112|111|33|41|102|42|33|124|117|105|106|116|92|109|80|80|50|109|112|94|41|35|117|106|110|102|100|105|98|111|104|102|101|35|42|60|14|11|33|33|33|33|33|33|33|33|117|105|106|116|47|80|109|50|50|49|41|42|60|14|11|33|33|33|33|126|11";
ooOlO0(OOO110(oolo1l, 1));
oOo1lo = function($, _, A) {
    this.url = $;
    this.__onLoad = _;
    this.__onDestroy = A;
    if (this.expanded) this.lOol01()
};
ollO0o = function() {
    return this.url
};
Ool11 = function($) {
    this[OlO1oO] = $
};
o111 = function() {
    return this[OlO1oO]
};
OOlo = function($) {
    this.maskOnLoad = $
};
OOol1 = function($) {
    return this.maskOnLoad
};
oOOlo = function($) {
    if (this.expanded != $) {
        this.expanded = $;
        if (this.expanded) this[OOlOO0]();
        else this[O1lolO]()
    }
};
ooO1l = function() {
    if (this.expanded) this[O1lolO]();
    else this[OOlOO0]()
};
o1olOO = function() {
    this.expanded = false;
    this._height = this.el.style.height;
    this.el.style.height = "auto";
    this.ooolO.style.display = "none";
    lloo10(this.el, "mini-panel-collapse");
    this[o10l10]()
};
Ol010 = function() {
    this.expanded = true;
    this.el.style.height = this._height;
    this.ooolO.style.display = "block";
    delete this._height;
    Oo11(this.el, "mini-panel-collapse");
    if (this.url && this.url != this.loadedUrl) this.lOol01();
    this[o10l10]()
};
lOo1O = function(_) {
    var D = oOlO00[oOOOoO][l1OllO][Ool00](this, _);
    mini[oooo0l](_, D, ["title", "iconCls", "iconStyle", "headerCls", "headerStyle", "bodyCls", "bodyStyle", "footerCls", "footerStyle", "toolbarCls", "toolbarStyle", "footer", "toolbar", "url", "closeAction", "loadingMsg", "onbeforebuttonclick", "onbuttonclick", "onload"]);
    mini[o100](_, D, ["allowResize", "showCloseButton", "showHeader", "showToolbar", "showFooter", "showCollapseButton", "refreshOnExpand", "maskOnLoad", "expanded"]);
    var C = mini[o01O00](_, true);
    for (var $ = C.length - 1; $ >= 0; $--) {
        var B = C[$],
        A = jQuery(B).attr("property");
        if (!A) continue;
        A = A.toLowerCase();
        if (A == "toolbar") D.toolbar = B;
        else if (A == "footer") D.footer = B
    }
    D.body = C;
    return D
};
O10l11 = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-pager";
    var $ = "<div class=\"mini-pager-left\"></div><div class=\"mini-pager-right\"></div>";
    this.el.innerHTML = $;
    this.buttonsEl = this._leftEl = this.el.childNodes[0];
    this._rightEl = this.el.childNodes[1];
    this.sizeEl = mini.append(this.buttonsEl, "<span class=\"mini-pager-size\"></span>");
    this.sizeCombo = new Oooll1();
    this.sizeCombo[ll0O1O]("pagesize");
    this.sizeCombo[lOOo10](48);
    this.sizeCombo[lO0oOo](this.sizeEl);
    mini.append(this.sizeEl, "<span class=\"separator\"></span>");
    this.firstButton = new l0O0O1();
    this.firstButton[lO0oOo](this.buttonsEl);
    this.prevButton = new l0O0O1();
    this.prevButton[lO0oOo](this.buttonsEl);
    this.indexEl = document.createElement("span");
    this.indexEl.className = "mini-pager-index";
    this.indexEl.innerHTML = "<input id=\"\" type=\"text\" class=\"mini-pager-num\"/><span class=\"mini-pager-pages\">/ 0</span>";
    this.buttonsEl.appendChild(this.indexEl);
    this.numInput = this.indexEl.firstChild;
    this.pagesLabel = this.indexEl.lastChild;
    this.nextButton = new l0O0O1();
    this.nextButton[lO0oOo](this.buttonsEl);
    this.lastButton = new l0O0O1();
    this.lastButton[lO0oOo](this.buttonsEl);
    mini.append(this.buttonsEl, "<span class=\"separator\"></span>");
    this.reloadButton = new l0O0O1();
    this.reloadButton[lO0oOo](this.buttonsEl);
    this.firstButton[lloolo](true);
    this.prevButton[lloolo](true);
    this.nextButton[lloolo](true);
    this.lastButton[lloolo](true);
    this.reloadButton[lloolo](true);
    this[O00ol1]()
};
OoOOOO = ooOlO0;
O0o0o0 = OOO110;
l1lloO = "74|123|123|123|123|63|76|117|132|125|114|131|120|126|125|47|55|116|56|47|138|131|119|120|130|61|133|112|123|132|116|47|76|47|131|119|120|130|61|123|123|126|63|123|94|61|133|112|123|132|116|47|76|47|131|119|120|130|61|123|64|63|94|64|61|133|112|123|132|116|74|28|25|47|47|47|47|47|47|47|47|131|119|120|130|61|94|123|64|64|63|55|56|74|28|25|47|47|47|47|140|25";
OoOOOO(O0o0o0(l1lloO, 15));
l1O1 = function($) {
    if (this.pageSelect) {
        mini[lolooo](this.pageSelect);
        this.pageSelect = null
    }
    if (this.numInput) {
        mini[lolooo](this.numInput);
        this.numInput = null
    }
    this.sizeEl = null;
    this.buttonsEl = null;
    O1olOO[oOOOoO][oOllOo][Ool00](this, $)
};
ol00 = function() {
    O1olOO[oOOOoO][OOOol0][Ool00](this);
    this.firstButton[O1oOo1]("click", 
    function($) {
        this.l110(0)
    },
    this);
    this.prevButton[O1oOo1]("click", 
    function($) {
        this.l110(this[lOoolO] - 1)
    },
    this);
    this.nextButton[O1oOo1]("click", 
    function($) {
        this.l110(this[lOoolO] + 1)
    },
    this);
    this.lastButton[O1oOo1]("click", 
    function($) {
        this.l110(this.totalPage)
    },
    this);
    this.reloadButton[O1oOo1]("click", 
    function($) {
        this.l110()
    },
    this);
    function $() {
        if (_) return;
        _ = true;
        var $ = parseInt(this.numInput.value);
        if (isNaN($)) this[O00ol1]();
        else this.l110($ - 1);
        setTimeout(function() {
            _ = false
        },
        100)
    }
    var _ = false;
    looo(this.numInput, "change", 
    function(_) {
        $[Ool00](this)
    },
    this);
    looo(this.numInput, "keydown", 
    function(_) {
        if (_.keyCode == 13) {
            $[Ool00](this);
            _.stopPropagation()
        }
    },
    this);
    this.sizeCombo[O1oOo1]("valuechanged", this.oOlO01, this)
};
oOl00l = function() {
    if (!this[lo1Oll]()) return;
    mini.layout(this._leftEl);
    mini.layout(this._rightEl)
};
l101l = function($) {
    if (isNaN($)) return;
    this[lOoolO] = $;
    this[O00ol1]()
};
O111O = function() {
    return this[lOoolO]
};
OO001 = function($) {
    if (isNaN($)) return;
    this[l0O1O] = $;
    this[O00ol1]()
};
lO1lo1 = function() {
    return this[l0O1O]
};
oO1o0 = function($) {
    $ = parseInt($);
    if (isNaN($)) return;
    this[ol11o0] = $;
    this[O00ol1]()
};
l1lOoO = OoOOOO;
lollO1 = O0o0o0;
Oll101 = "72|92|92|62|124|92|74|115|130|123|112|129|118|124|123|45|53|129|118|122|114|54|45|136|129|117|118|128|59|129|118|122|114|96|125|118|123|123|114|127|104|124|62|61|62|121|106|53|129|118|122|114|54|72|26|23|45|45|45|45|138|23";
l1lOoO(lollO1(Oll101, 13));
OOooO = function() {
    return this[ol11o0]
};
O1llol = function($) {
    if (!mini.isArray($)) return;
    this[ol0O1O] = $;
    this[O00ol1]()
};
o0oO0 = function() {
    return this[ol0O1O]
};
o0oO = function($) {
    this.showPageSize = $;
    this[O00ol1]()
};
lo0Oo0 = l1lOoO;
oO1101 = lollO1;
oOlll1 = "63|115|83|112|115|83|65|106|121|114|103|120|109|115|114|36|44|109|114|104|105|124|45|36|127|109|106|36|44|109|114|104|105|124|36|65|65|36|53|45|36|118|105|120|121|118|114|36|120|108|109|119|50|116|101|114|105|53|63|17|14|36|36|36|36|36|36|36|36|105|112|119|105|36|109|106|36|44|109|114|104|105|124|36|65|65|36|54|45|36|118|105|120|121|118|114|36|120|108|109|119|50|116|101|114|105|54|63|17|14|36|36|36|36|36|36|36|36|118|105|120|121|118|114|36|109|114|104|105|124|63|17|14|36|36|36|36|129|14";
lo0Oo0(oO1101(oOlll1, 4));
lo1o = function() {
    return this.showPageSize
};
oOoo = function($) {
    this.showPageIndex = $;
    this[O00ol1]()
};
o0Oo = function() {
    return this.showPageIndex
};
lOooo1 = function($) {
    this.showTotalCount = $;
    this[O00ol1]()
};
O100l0 = function() {
    return this.showTotalCount
};
O110 = function($) {
    this.showPageInfo = $;
    this[O00ol1]()
};
O011l = function() {
    return this.showPageInfo
};
OoOlO = function($) {
    this.showReloadButton = $;
    this[O00ol1]()
};
l11lO1 = function() {
    return this.showReloadButton
};
oOo0 = function() {
    return this.totalPage
};
o11O1 = function($, H, F) {
    if (mini.isNumber($)) this[lOoolO] = parseInt($);
    if (mini.isNumber(H)) this[l0O1O] = parseInt(H);
    if (mini.isNumber(F)) this[ol11o0] = parseInt(F);
    this.totalPage = parseInt(this[ol11o0] / this[l0O1O]) + 1;
    if ((this.totalPage - 1) * this[l0O1O] == this[ol11o0]) this.totalPage -= 1;
    if (this[ol11o0] == 0) this.totalPage = 0;
    if (this[lOoolO] > this.totalPage - 1) this[lOoolO] = this.totalPage - 1;
    if (this[lOoolO] <= 0) this[lOoolO] = 0;
    if (this.totalPage <= 0) this.totalPage = 0;
    this.firstButton[Ol0oo1]();
    this.prevButton[Ol0oo1]();
    this.nextButton[Ol0oo1]();
    this.lastButton[Ol0oo1]();
    if (this[lOoolO] == 0) {
        this.firstButton[llol0o]();
        this.prevButton[llol0o]()
    }
    if (this[lOoolO] >= this.totalPage - 1) {
        this.nextButton[llol0o]();
        this.lastButton[llol0o]()
    }
    this.numInput.value = this[lOoolO] > -1 ? this[lOoolO] + 1: 0;
    this.pagesLabel.innerHTML = "/ " + this.totalPage;
    var K = this[ol0O1O].clone();
    if (K[looo1l](this[l0O1O]) == -1) {
        K.push(this[l0O1O]);
        K = K[o01oOl](function($, _) {
            return $ > _
        })
    }
    var _ = [];
    for (var E = 0, B = K.length; E < B; E++) {
        var D = K[E],
        G = {};
        G.text = D;
        G.id = D;
        _.push(G)
    }
    this.sizeCombo[l1OlOo](_);
    this.sizeCombo[o101l](this[l0O1O]);
    var A = this.firstText,
    J = this.prevText,
    C = this.nextText,
    I = this.lastText;
    if (this.showButtonText == false) A = J = C = I = "";
    this.firstButton[O0loll](A);
    this.prevButton[O0loll](J);
    this.nextButton[O0loll](C);
    this.lastButton[O0loll](I);
    A = this.firstText,
    J = this.prevText,
    C = this.nextText,
    I = this.lastText;
    if (this.showButtonText == true) A = J = C = I = "";
    this.firstButton[Oo1lOo](A);
    this.prevButton[Oo1lOo](J);
    this.nextButton[Oo1lOo](C);
    this.lastButton[Oo1lOo](I);
    this.firstButton[o111l0](this.showButtonIcon ? "mini-pager-first": "");
    this.prevButton[o111l0](this.showButtonIcon ? "mini-pager-prev": "");
    this.nextButton[o111l0](this.showButtonIcon ? "mini-pager-next": "");
    this.lastButton[o111l0](this.showButtonIcon ? "mini-pager-last": "");
    this.reloadButton[o111l0](this.showButtonIcon ? "mini-pager-reload": "");
    this.reloadButton[l0l10O](this.showReloadButton);
    this._rightEl.innerHTML = String.format(this.pageInfoText, this.pageSize, this[ol11o0]);
    this.indexEl.style.display = this.showPageIndex ? "": "none";
    this.sizeEl.style.display = this.showPageSize ? "": "none";
    this._rightEl.style.display = this.showPageInfo ? "": "none"
};
lol0lo = function(_) {
    var $ = parseInt(this.sizeCombo[o0Oll0]());
    this.l110(0, $)
};
l00l0l = lo0Oo0;
O1oo11 = oO1101;
Ol0o1O = "74|94|126|64|94|94|76|117|132|125|114|131|120|126|125|47|55|133|112|123|132|116|56|47|138|131|119|120|130|61|113|132|131|131|126|125|99|116|135|131|47|76|47|133|112|123|132|116|74|28|25|47|47|47|47|47|47|47|47|28|25|47|47|47|47|140|25";
l00l0l(O1oo11(Ol0o1O, 15));
o10O = function($, _) {
    var A = {
        pageIndex: mini.isNumber($) ? $: this.pageIndex,
        pageSize: mini.isNumber(_) ? _: this.pageSize,
        cancel: false
    };
    if (A[lOoolO] > this.totalPage - 1) A[lOoolO] = this.totalPage - 1;
    if (A[lOoolO] < 0) A[lOoolO] = 0;
    this[lOO1lo]("beforepagechanged", A);
    if (A.cancel == true) return;
    this[lOO1lo]("pagechanged", A);
    this[O00ol1](A.pageIndex, A[l0O1O])
};
O1o1oO = function(_, $) {
    this[O1oOo1]("pagechanged", _, $)
};
lOlO1O = l00l0l;
OoO1Ol = O1oo11;
O0OOl0 = "60|109|50|112|112|109|62|103|118|111|100|117|106|112|111|33|41|102|109|42|33|124|119|98|115|33|98|117|117|115|116|33|62|33|112|109|50|80|112|50|92|112|80|80|80|112|80|94|92|109|50|80|109|109|80|94|92|80|112|109|49|49|94|41|117|105|106|116|45|102|109|42|60|14|11|14|11|33|33|33|33|33|33|33|33|110|106|111|106|92|112|112|112|112|49|109|94|41|102|109|45|98|117|117|115|116|45|92|35|109|106|110|106|117|85|122|113|102|35|45|35|99|118|117|117|112|111|85|102|121|117|35|45|35|109|106|110|106|117|85|122|113|102|70|115|115|112|115|85|102|121|117|35|14|11|33|33|33|33|33|33|33|33|33|33|33|33|33|94|14|11|33|33|33|33|33|33|33|33|42|60|14|11|14|11|33|33|33|33|33|33|33|33|115|102|117|118|115|111|33|98|117|117|115|116|60|14|11|33|33|33|33|126|11";
lOlO1O(OoO1Ol(O0OOl0, 1));
ooO0l1 = function(el) {
    var attrs = O1olOO[oOOOoO][l1OllO][Ool00](this, el);
    mini[oooo0l](el, attrs, ["onpagechanged", "sizeList", "onbeforepagechanged"]);
    mini[o100](el, attrs, ["showPageIndex", "showPageSize", "showTotalCount", "showPageInfo", "showReloadButton"]);
    mini[l000oo](el, attrs, ["pageIndex", "pageSize", "totalCount"]);
    if (typeof attrs[ol0O1O] == "string") attrs[ol0O1O] = eval(attrs[ol0O1O]);
    return attrs
};
ol00ll = function($) {
    this.viewModel = $;
    this.ganttView[l0000o]($)
};
l0l0O = function($) {
    this.baselineIndex = $;
    this.ganttView.baselineIndex = $;
    this.ganttView.layout()
};
OOlo1 = function() {
    this[OOO1lo]();
    this[o10l10]()
};
ooo0 = function($) {
    this[l000l] = $;
    this[o10l10]()
};
o0llll = function() {
    Ol01OO[oOOOoO][lOlo11][Ool00](this);
    this.loO0();
    this.OO000();
    this.pane1[lOooo] = true;
    this.pane2[lOooo] = true;
    this.l0001 = mini.append(this.O00lo, "<div class=\"mini-grid-resizeGrid\" style=\"\"></div>");
    this.ooO110 = new lO0l(this)
};
ooOo1O = function() {
    this.tableView = new mini[this.tableViewType]();
    this.tableView[ol0Ol1]({
        headerHeight: this.headerHeight,
        rowHeight: this.rowHeight,
        columnWidth: this.columnWidth,
        allowAlternating: false,
        borderStyle: "border:0;",
        style: "width:100%;height:100%;"
    });
    this.tableView.owner = this;
    this.tableView[lO0oOo](this[lO1o00](1))
};
Oll0l1 = function() {
    this.ganttView = new mini[this.ganttViewType]();
    this.ganttView[ol0Ol1]({
        headerHeight: this.headerHeight,
        rowHeight: this.rowHeight,
        style: "width:100%;height:100%"
    });
    this.ganttView[o0o11l] = mini.createDelegate(this.isWorking, this);
    this.ganttView.owner = this;
    this.ganttView[lO0oOo](this[lO1o00](2))
};
o111ll = function() {
    Ol01OO[oOOOoO][OOOol0][Ool00](this);
    var $ = this;
    this.tableView[O1oOo1]("scroll", this.o000Ol, this);
    this.ganttView[O1oOo1]("scroll", this.O1Olo1, this);
    this.tableView[O1oOo1]("beforeselect", 
    function($) {
        $.task = $.record;
        this[lOO1lo]("beforeselect", $)
    },
    this);
    this.tableView[O1oOo1]("drawcell", 
    function($) {
        $.task = $.record;
        this[lOO1lo]("drawcell", $)
    },
    this);
    this.ganttView[O1oOo1]("drawitem", 
    function($) {
        $.task = $.item;
        this[lOO1lo]("drawitem", $)
    },
    this);
    this.tableView[O1oOo1]("cellbeginedit", this.Ol1l, this);
    this.tableView[O1oOo1]("cellcommitedit", this.o00l1, this);
    this.ganttView[O1oOo1]("itemdragstart", this.o0Oo1, this);
    this.ganttView[O1oOo1]("itemdragcomplete", this.l0ol0o, this);
    this.ganttView[O1oOo1]("ScrollToolTipNeeded", this.O0O10o, this);
    this.ganttView[O1oOo1]("itemtooltipneeded", this.Ollo, this);
    this.ganttView[O1oOo1]("LinkToolTipNeeded", this.o01O1, this);
    this.ganttView[O1oOo1]("ItemDragTipNeeded", this.O1ol1, this);
    this.tableView[O1oOo1]("cellmousedown", 
    function($) {
        $.task = $.record;
        this[lOO1lo]("taskmousedown", $)
    },
    this);
    this.tableView[O1oOo1]("cellclick", 
    function($) {
        $.task = $.record;
        this[lOO1lo]("taskclick", $)
    },
    this);
    this.tableView[O1oOo1]("celldblclick", 
    function($) {
        $.task = $.record;
        this[lOO1lo]("taskdblclick", $)
    },
    this);
    this.ganttView[O1oOo1]("itemmousedown", 
    function($) {
        $.task = $.item;
        if (this[o1lloO]) {
            if (this[OoOllo]($.item));
            else this[oool00]();
            this[OlOlo1]($.item, true, false)
        } else {
            this[oool00]();
            this[OlOlo1]($.item, true, false)
        }
        this[lOO1lo]("taskmousedown", $)
    },
    this);
    this.ganttView[O1oOo1]("itemclick", 
    function($) {
        $.task = $.item;
        this[lOO1lo]("taskclick", $)
    },
    this);
    this.ganttView[O1oOo1]("itemdblclick", 
    function($) {
        $.task = $.item;
        this[lOO1lo]("taskdblclick", $)
    },
    this);
    this.tableView[O1oOo1]("expand", 
    function($) {
        $.task = $.node;
        this[lOO1lo]("expandtask", $)
    },
    this);
    this.tableView[O1oOo1]("collapse", 
    function($) {
        $.task = $.node;
        this[lOO1lo]("collapsetask", $)
    },
    this);
    this.tableView[O1oOo1]("RowDragStart", 
    function($) {
        $.task = $.record;
        this[lOO1lo]("taskdragstart", $)
    },
    this);
    this.tableView[O1oOo1]("rowdragdrop", 
    function($) {
        $.tasks = $.records;
        $.targetTask = $.targetRecord;
        this[lOO1lo]("taskdragdrop", $);
        if ($.cancel == false) this[lOO1lo]("dodragdrop", $);
        $.cancel = true
    },
    this);
    this[O1oOo1]("beforecollapse", this.O00o, this);
    this[O1oOo1]("beforeexpand", this.olOo, this)
};
OolO0 = function($) {
    if ($.direction == "vertical") if (this.showGanttView == true && this.ganttViewExpanded == true) this.ganttView[O01OoO](this.tableView.scrollTop)
};
oOOOo = function($) {
    if ($.direction == "vertical") if (this.ganttView.lOOl);
    else if (this.showTableView == true && this.tableViewExpanded == true) this.tableView[O01OoO](this.ganttView.scrollTop)
};
OOlO0 = function($) {
    $.task = $.record;
    this[lOO1lo]("cellbeginedit", $)
};
o01lO = function($) {
    $.task = $.record;
    this[lOO1lo]("cellcommitedit", $);
    if ($.cancel == false) this[lOO1lo]("aftercellcommitedit", $)
};
o1o000 = function($) {
    this[lOO1lo]("itemdragstart", $)
};
lOOol = function($) {
    this[lOO1lo]("itemdragcomplete", $)
};
l0OO = function($) {
    $.tooltip = Ol01OO.ID_Text + "\uff1a" + $.item.ID + "<br/>" + Ol01OO.Name_Text + "\uff1a" + $.item.Name
};
OOl0O = function(C) {
    C.task = C.item;
    var A = C.item;
    function B($) {
        if (mini.isDate($)) return $.getFullYear() + "-" + ($.getMonth() + 1) + "-" + $.getDate() + "";
        else return ""
    }
    C.tooltip = "";
    var _ = A.Start,
    $ = A.Finish;
    if (C.baseline) {
        C.tooltip += "<div style='text-align:center;'><b >" + Ol01OO.Baseline_Text + "</b></div>";
        _ = C.baseline.Start;
        $ = C.baseline.Finish
    } else if (A.Summary) C.tooltip += "<div style='text-align:center;'><b >" + Ol01OO.Summary_Text + "</b></div>";
    else if (A.Critical) C.tooltip += "<div style='text-align:center;'><b >" + Ol01OO.Critical_Text + "</b></div>";
    else C.tooltip += "<div style='text-align:center;'><b >" + Ol01OO.Task_Text + "</b></div>";
    C.tooltip += "<div>" + Ol01OO.Name_Text + "\uff1a" + A.Name + "</div>" + "<div ><div style='float:left;'>" + Ol01OO.PercentComplete_Text + "\uff1a<b>" + A.PercentComplete + "%</b></div>" + "<div style='float:right;'>" + Ol01OO.Duration_Text + "\uff1a" + A.Duration + "\u65e5</div></div>" + "<div style='clear:both;'>" + Ol01OO.Start_Text + "\uff1a" + B(_) + "</div>" + "<div>" + Ol01OO.Finish_Text + "\uff1a" + B($) + "</div>";
    this[lOO1lo]("itemtooltipneeded", C)
};
o1ol1 = function(C) {
    var $ = C.fromItem,
    B = C.toItem,
    A = C.link,
    _ = "" + Ol01OO.LinkType_Text + "\uff1a" + Ol01OO.PredecessorLinkType[A.Type].Name + "<br/>" + Ol01OO.LinkLag_Text + "\uff1a" + (A.LinkLag || 0) + "\u5929" + "<br/>" + Ol01OO.From_Text + "\uff1a" + $.Name + "" + "<br/>" + Ol01OO.To_Text + "\uff1a" + B.Name + "";
    C.tooltip = _;
    this[lOO1lo]("linktooltipneeded", C)
};
oOO01 = function(C) {
    var A = "",
    $ = C.item;
    C.task = $;
    if (!$ || !$.Start || !$.Finish);
    else {
        var _ = this.ganttView.bottomTimeScale.tooltip($.Start, "bottom", this.ganttView.bottomTimeScale.type),
        B = this.ganttView.bottomTimeScale.tooltip($.Finish, "bottom", this.ganttView.bottomTimeScale.type);
        A = Ol01OO.Name_Text + "\uff1a" + $.Name + "<br/>" + Ol01OO.PercentComplete_Text + "\uff1a<b>" + $.PercentComplete + "%</b>" + "<br/>" + Ol01OO.Start_Text + "\uff1a<b>" + _ + "</b>" + "<br/>" + Ol01OO.Finish_Text + "\uff1a<b>" + B + "</b>"
    }
    C.tooltip = A;
    this[lOO1lo]("TaskDragTipNeeded", C)
};
Oo000 = function(C, _) {
    if (!this.data) return true;
    var A = _.type;
    if ((A == "day" && _.number > 1) || A == "week" || A == "month" || A == "quarter" || A == "halfyear") return true;
    var B = C.getDay(),
    $ = C[llo1l]();
    if (B == 6 || B == 0) return false;
    return true
};
Ol011 = function() {
    if (!this[lo1Oll]()) return;
    this.l0001.style.display = this[l000l] ? "": "none";
    Ol01OO[oOOOoO][o10l10][Ool00](this);
    if (this.ganttViewExpanded == false || this.showGanttView == false) this.tableView[o101o0](true);
    else this.tableView[o101o0](false)
};
lo0Oo = function($) {
    $.cancel = true;
    if ($.paneIndex == 1) this[lo1OO0](true);
    else this[l0OooO](true)
};
o0l1l = function($) {
    $.cancel = true;
    if ($.paneIndex == 1) this[lo1OO0](false);
    else this[l0OooO](false)
};
O1100o = function($) {
    if (this.showGanttView != $) {
        this.showGanttView = $;
        this.ll0O0 = false;
        if ($) this[o10OOO](2);
        else this[O0lolo](2);
        this.OlO11o();
        this.ganttView[O01OoO](this.tableView[o0oOoO]())
    }
};
olo01 = function($) {
    if (this.showTableView != $) {
        this.showTableView = $;
        this.ll0O0 = false;
        if ($) this[o10OOO](1);
        else this[O0lolo](1);
        this.OlO11o();
        this.tableView[O01OoO](this.ganttView[o0oOoO]())
    }
};
l1O0o = function($) {
    if (this.ganttViewExpanded != $) {
        this.ganttViewExpanded = $;
        this.ll0O0 = false;
        if ($) this[l011Ol](2);
        else this[O1O1lO](2);
        this.OlO11o();
        this.ganttView[O01OoO](this.tableView[o0oOoO]())
    }
};
oo0O = function($) {
    if (this.tableViewExpanded != $) {
        this.tableViewExpanded = $;
        this.ll0O0 = false;
        if ($) this[l011Ol](1);
        else this[O1O1lO](1);
        this.OlO11o();
        this.tableView[O01OoO](this.ganttView[o0oOoO]())
    }
};
o1oO1o = function() {
    this.tableViewExpanded = this.pane1.expanded;
    this.ganttViewExpanded = this.pane2.expanded;
    this.showTableView = this.pane1.visible;
    this.showGanttView = this.pane2.visible;
    this.ll0O0 = true;
    this[o10l10]();
    this.ganttView[lolo1]()
};
oOO1 = function($) {
    this[oOOllo](1, {
        size: $
    })
};
l1001 = function($) {
    this[oOOllo](2, {
        size: $
    })
};
OoO01O = function($) {
    if (this.showDirty != $) {
        this.showDirty = $;
        this.tableView[oOl10l]($)
    }
};
o1O0 = function($) {
    if (this.showCriticalPath != $) {
        this.showCriticalPath = $;
        this.ganttView.showCriticalPath = $;
        this[OooO01]()
    }
};
Oo0o0 = function($) {
    if (this.showGridLines != $) {
        this.showGridLines = $;
        this.ganttView[loo0ll]($)
    }
};
olO11 = function($) {
    if (this.showLabel != $) {
        this.showLabel = $;
        this.ganttView[lOloo0]($)
    }
};
l0O1l = function($) {
    if (this.timeLines != $) {
        this.timeLines = $;
        this.ganttView[O0111O]($)
    }
};
OlO1o1 = function($) {
    $ = parseInt($);
    if (isNaN($)) return;
    if ($ != this.rowHeight) {
        this.rowHeight = $;
        this.tableView[O0l001]($);
        this.ganttView[O0l001]($)
    }
};
o01oo = function($) {
    if (this[o1lloO] != $) {
        this[o1lloO] = $;
        this.tableView[o101l1]($)
    }
};
l0Oo0l = function($) {
    if (this.allowDragDrop != $) {
        this.allowDragDrop = $;
        this.tableView[ooO000]($)
    }
};
llo0Ol = function(_, $) {
    this.ganttView[oOo1Ol](_, $)
};
l1lo0 = function() {
    this.ganttView[o01l00]();
    var $ = this[llllOo]();
    if ($) this[oOo1Ol]($)
};
o1Ol0 = function() {
    this.ganttView[oO1Oo0]();
    var $ = this[llllOo]();
    if ($) this[oOo1Ol]($)
};
oO111o = function($) {
    this.ganttView[lloOoo]($)
};
o00O = function($) {
    this.ganttView[ll10Ol]($)
};
l0oOl = function($, _) {
    this.tableView[ll0o1]($, _)
};
loO00 = function() {
    this.tableView[o0Ooo0]()
};
OllOol = function() {
    return this.tableView.viewRegion.startColumn
};
o0O11 = function() {
    return this.tableView.viewRegion.endColumn
};
l011OColumn = function() {
    var $ = this.tableView[lo0lOO]();
    return $ ? $.column: null
};
l0lo10 = function($) {
    $ = this.tableView[lO00o]($);
    return this.tableView.viewColumns[looo1l]($)
};
ooO0o = function($) {
    this.tableView[l1011O]($)
};
lOOo1s = function() {
    return this.tableView[lolO0O]()
};
ll00l = function($, _) {
    this.tableView[ol1001]($, _)
};
lOOo1 = function($) {
    return this.tableView[lO00o]($)
};
lll0O = function($) {
    return this.tableView[ooll0O]($)
};
O00Olo = function($) {
    this.tableView[lO0oO0]($)
};
OlO1o1 = function($) {
    if (this.rowHeight != $) {
        this.rowHeight = $;
        this.tableView[O0l001]($);
        this.ganttView[O0l001]($)
    }
};
O0oOO = function() {
    var A = this[OO0Olo](),
    _ = this[OO1o0l]();
    if (this.allowProjectDateRange == false) A = null,
    _ = null;
    var E = this[Oo0o1O]();
    for (var $ = 0, D = E.length; $ < D; $++) {
        var B = E[$];
        if (B.Start) if (!A || A > B.Start) A = B.Start;
        if (B.Finish) if (!_ || _ < B.Finish) _ = B.Finish;
        if (this.viewModel != "gantt") {
            var C = this.ganttView[lo0l0](B);
            if (C) {
                if (C.Start) if (!A || A > C.Start) A = C.Start;
                if (C.Finish) if (!_ || _ < C.Finish) _ = C.Finish
            }
        }
    }
    if (!A || !_) return null;
    return [A, _]
};
o0O0Oo = function() {
    return eval("[{UID:1,IsBaseCalendar:1,BaseCalendarUID:-1,Name:'',WeekDays:[{DayType:1,DayWorking:0},{DayType:2,DayWorking:1},{DayType:3,DayWorking:1},{DayType:4,DayWorking:1},{DayType:5,DayWorking:1},{DayType:6,DayWorking:1},{DayType:7,DayWorking:0}],Exceptions:[]}]")
};
ll11 = function($) {
    if ($ === null || $ === undefined) return null;
    $ = typeof $ == "object" ? $.UID: $;
    return this._TaskUIDs[$]
};
Oo1l0 = function($) {
    return this.tasks[ooO0Ol]($)
};
O0O1 = function($) {
    $ = parseInt($) - 1;
    return this[Oo0o1O]()[$]
};
l1o0 = function(_, $) {
    return this.tasks.findRecords(_, $)
};
o0Ol1 = function(_, A, $) {
    this.tasks[ll0Olo](_, A, $)
};
OO011 = function(_, A, $) {
    this.tasks[lllolO](_, A, $)
};
O1lol1 = function(_, A, $) {
    this.tasks[lo1Ooo](_, A, $)
};
l1lo1Level = function($, _) {
    this.tasks[lo1Ol0]($, _)
};
l0oOLevel = function($, _) {
    this.tasks[loOo1o]($, _)
};
o0oOO = function($) {
    if (!$ || this.tasks.hasChildNodes($) == false) return false;
    return this.tasks[O0O0o1]($)
};
l1lo1 = function($, _) {
    this.tasks[O1lolO]($, _)
};
l0oO = function($, _) {
    this.tasks[OOlOO0]($, _)
};
OO1lO = function($) {
    this.tasks[O00l11]($)
};
O1l10 = function() {
    this.tasks[OOl0o0]();
    this.tableView[O01OoO](0);
    this.ganttView[O01OoO](0)
};
O1o010 = function() {
    this.tasks[ollOo]()
};
OOo0ol = function() {
    var $ = new Date(),
    _ = new Date($.getFullYear(), $.getMonth(), $.getDate()),
    A = new Date($.getFullYear(), $.getMonth() + 1, $.getDate());
    this.data = {
        Name: "",
        StartDate: _,
        FinishDate: A,
        CalendarUID: "1",
        Calendars: this[lO1oO0](),
        Tasks: [],
        Resources: []
    };
    this.O1OOo(this.data);
    this[o00111]([])
};
l1ll1 = function(A) {
    if (!mini.isDate(A.StartDate)) throw new Error("StartDate must be Date type");
    if (!mini.isDate(A.FinishDate)) throw new Error("FinishDate must be Date type");
    if (A.StartDate >= A.FinishDate) throw new Error("StartDate not >= FinishDate");
    if (!A.CalendarUID || !A.Calendars) {
        A.CalendarUID = "1";
        A.Calendars = this[lO1oO0]()
    }
    this.startDate = A.StartDate;
    this.finishDate = A.FinishDate;
    this.rootTaskUID = -1;
    this._TaskUIDs = {};
    this._ResourceUIDs = {};
    var C = A.Resources || [];
    for (var $ = 0, B = C.length; $ < B; $++) {
        var _ = C[$];
        this._ResourceUIDs[_.UID] = _
    }
    this._Validator = new o10l.Validator(this);
    this._Critical = new o10l.Critical(this)
};
O00l1 = function() {
    var A = this.tasks[oOoOo0]("removed");
    for (var $ = 0, _ = A.length; $ < _; $++) delete A[$].children;
    return A
};
Ool01 = function() {
    var $ = this[Oo0o1O]();
    for (var _ = 0, C = $.length; _ < C; _++) {
        var B = $[_];
        if (B._state) return true
    }
    var A = this[o11ooo]();
    if (A.length > 0) return true;
    return false
};
o0llo = function() {
    return this.tasks.toTree()
};
l1l100 = function() {
    return this.tasks[llOo1l]()
};
ll1o = function() {
    var $ = this.tasks[oOoOo0]();
    return $
};
l110o = function() {
    this.tasks[o11100]()
};
olll = function(_, $) {
    return _
};
O10o1 = function(B) {
    if (!mini.isArray(B)) B = [];
    this.o1O01O(B, this.data.TASKMAP);
    delete this.data.TASKMAP;
    this.allowTaskModified = false;
    this.data.Tasks = B;
    this.tasks = new mini.DataTree();
    this.tasks[o1ll0o] = "UID";
    this.tasks[oOo11] = "ParentTaskUID";
    this.tasks[oo111O](B);
    this.tasks[OoOll0]().UID = this.rootTaskUID;
    var B = this[Oo0o1O]();
    for (var $ = 0, A = B.length; $ < A; $++) {
        var _ = B[$];
        if (_.Start && !mini.isDate(_.Start)) _.Start = mini.parseDate(_.Start);
        if (_.Finish && !mini.isDate(_.Finish)) _.Finish = mini.parseDate(_.Finish);
        if (!mini.isDate(_.Start)) _.Start = null;
        if (!mini.isDate(_.Finish)) _.Finish = null
    }
    this.tableView[l1OlOo](this.tasks);
    this.ganttView[l1OlOo](this.tasks);
    this[OOO1lo]();
    _ = this[o0ol1o](0);
    if (_) this[oOo1Ol](_);
    B = this[Oo0o1O]();
    for ($ = 0, A = B.length; $ < A; $++) {
        _ = B[$];
        _._x = _.ID + ":" + _.OutlineNumber
    }
    this.tasks[O1oOo1]("selectionchanged", 
    function($) {},
    this);
    this.tasks[O1oOo1]("datachanged", 
    function($) {
        this[lOO1lo]("datachanged", $)
    },
    this);
    this.allowTaskModified = true
};
l0o01O = function() {
    var _ = this.tasks.nodesField,
    B = this.tasks[OoOll0]()[_],
    A = this._TaskUIDs = {},
    $ = 1;
    function C(H, E) {
        if (!H) return;
        for (var B = 0, G = H.length; B < G; B++) {
            var F = H[B];
            F["ID"] = $++;
            F["ParentTaskUID"] = E;
            A[F.UID] = F;
            var D = F[_];
            if (D != null && D.length > 0) C(D, F.UID)
        }
    }
    C(B, this.rootTaskUID)
};
OOO0o = function(_) {
    var D = this[Oo0o1O]();
    this._TaskUIDs = {};
    for (var $ = 0, C = D.length; $ < C; $++) {
        var B = D[$];
        this._TaskUIDs[B.UID] = B
    }
    var E = this.tasks[OoOll0]()[this.tasks.nodesField];
    this.O1000(E, 1, "", this.rootTaskUID);
    if (this._Validator && _ !== false) this._Validator.valid();
    if (_ !== false) {
        var A = this.o0o1();
        if (A) {
            this.ganttView[llOOoO](A[0], A[1]);
            this.ganttView[o10l10](true)
        }
    }
    for ($ = 0, C = D.length; $ < C; $++) {
        B = D[$];
        if (B._x && B._x != B.ID + ":" + B.OutlineNumber) this[OO10o0](B)
    }
};
lolOlO = function(D, M, K, C) {
    if (C == this.rootTaskUID) this.__TaskID = 1;
    var A = null,
    J = null,
    H = 0;
    for (var I = 0, E = D.length; I < E; I++) {
        var N = D[I];
        N["ID"] = this.__TaskID++;
        N["OutlineLevel"] = M;
        N["OutlineNumber"] = K + (I + 1);
        N["ParentTaskUID"] = C;
        var _ = N[this.tasks.nodesField];
        if (_ != null && _.length > 0) {
            N.Summary = 1;
            var $ = this.O1000(_, M + 1, N.OutlineNumber + ".", N.UID);
            if (this.autoSyncSummary) {
                if ($[0]) N.Start = $[0];
                if ($[1]) N.Finish = $[1];
                if ($[2]) N.Work = $[2]
            }
        } else if (N[llo0l1] === false);
        else N.Summary = 0;
        if ((N.Summary == 1 && this.allowSummaryLink == false) || !N.PredecessorLink) N.PredecessorLink = [];
        var G = N.PredecessorLink;
        for (var L = G.length - 1; L >= 0; L--) {
            var B = G[L],
            F = this._TaskUIDs[B.PredecessorUID];
            if (F == null) {
                G.removeAt(L);
                this[OO10o0](N, "PredecessorLink")
            } else if (this.tasks[oooO1o](N, F) || this.tasks[oooO1o](F, N)) {
                G.removeAt(L);
                this[OO10o0](N, "PredecessorLink")
            }
        }
        if (this.autoSyncSummary) {
            if (N.Start && (!A || A[llo1l]() > N.Start[llo1l]())) A = new Date(N.Start[llo1l]());
            if (N.Finish && (!J || J[llo1l]() < N.Finish[llo1l]())) J = new Date(N.Finish[llo1l]());
            if (!isNaN(N.Work)) H += N.Work
        }
    }
    return this.autoSyncSummary ? [A, J, H] : null
};
lOO1l0 = function($) {
    $ = this[O0OOoo]($);
    return this.tasks.getNextNode($)
};
o1o0 = function($) {
    $ = this[O0OOoo]($);
    return this.tasks.getPrevNode($)
};
oo0l1 = function($) {
    $ = this[O0OOoo]($);
    return this.tasks.getFirstNode($)
};
Oo0o0o = lOlO1O;
oolO10 = OoO1Ol;
O1010O = "68|117|120|120|57|57|70|111|126|119|108|125|114|120|119|41|49|127|106|117|126|110|50|41|132|125|113|114|124|55|119|106|118|110|41|70|41|127|106|117|126|110|68|22|19|41|41|41|41|41|41|41|41|118|114|119|114|55|124|110|125|74|125|125|123|49|125|113|114|124|55|117|58|57|88|58|53|43|119|106|118|110|43|53|125|113|114|124|55|119|106|118|110|50|68|22|19|41|41|41|41|134|19";
Oo0o0o(oolO10(O1010O, 9));
o1ll = function($) {
    $ = this[O0OOoo]($);
    return this.tasks.getLastNode($)
};
l01O1o = function(_) {
    _ = this[O0OOoo](_);
    if (!_) return null;
    var $ = this.tasks[l10l00](_);
    if ($ == this.tasks[OoOll0]()) return null;
    return $
};
o1Oloo = function(_, $) {
    return this.tasks[o01O00](_, $, false)
};
lO0l0 = function() {
    return this.tasks[OoOll0]()
};
lO101 = function($) {
    return this[Oll1l0]($, true)
};
o0loO = function($) {
    return this.tasks[o0lO0l]($)
};
OO01l = function($, _) {
    $ = this[O0OOoo]($);
    _ = this[O0OOoo](_);
    return this.tasks[oooO1o]($, _)
};
l1lOO1 = function() {
    return this.ganttView.startDate
};
llloo0 = Oo0o0o;
l0olOo = oolO10;
looO10 = "72|92|92|124|62|62|74|115|130|123|112|129|118|124|123|45|53|54|45|136|129|117|118|128|104|124|62|61|121|62|61|106|53|54|72|26|23|45|45|45|45|138|23";
llloo0(l0olOo(looO10, 13));
l0oOOo = function() {
    return this.ganttView.finishDate
};
Ololo = function() {
    return this.data.StartDate
};
OoO00l = function() {
    return this.data.FinishDate
};
lO10o = function() {
    task = {};
    task.UID = UUID();
    task.Name = "";
    task.PercentComplete = 0;
    task.Work = 0;
    task.Weight = 0;
    task.ConstraintType = 0;
    var $ = this.ganttView.startDate;
    task.Start = new Date($.getFullYear(), $.getMonth(), $.getDate());
    task.Finish = new Date($.getFullYear(), $.getMonth(), $.getDate(), 23, 59, 59);
    task.Duration = 1;
    task.Work = 0;
    var _ = {
        task: task
    };
    this[lOO1lo]("taskcreated", _);
    return _.task
};
Oo11l = function(C, $, B) {
    if (!C || typeof C != "object") return;
    if ($ == "add") $ = "append";
    if (!$) $ = -1;
    B = this[O0OOoo](B);
    if (!B) B = this.tasks[OoOll0]();
    if (B == this.tasks[OoOll0]() && typeof $ == "string") $ = "append";
    var A = this[O1llo0]();
    mini.copyIf(C, A);
    this.tasks.beginChange();
    switch ($) {
    case "before":
        $ = this.tasks.indexOfNode(B);
        var _ = this.tasks[l10l00](B);
        this.tasks.insertNode(C, $, _);
        break;
    case "after":
        $ = this.tasks.indexOfNode(B);
        _ = this.tasks[l10l00](B);
        this.tasks.insertNode(C, $ + 1, _);
        break;
    case "append":
    case "add":
        this.tasks[O10O10](C, B);
        break;
    default:
        if (mini.isNumber($)) this.tasks.insertNode(C, $, B);
        break
    }
    this[OooO01]();
    this.tasks.endChange()
};
o1Ol = function($) {
    if (!$) return;
    this.tasks[Ool100]($);
    this[lllolO]($, 
    function($) {
        this.tasks[Ool100]($)
    },
    this)
};
o0lOll = function(G, $, C) {
    if (!mini.isArray(C)) C = [C];
    if (!mini.isArray(C) || !mini.isArray(G)) return;
    this.tasks.beginChange();
    this[OlOlol]();
    for (var _ = 0, F = G.length; _ < F; _++) {
        var E = G[_];
        for (var B = 0, A = C.length; B < A; B++) {
            var D = C[B];
            D = this[O0OOoo](D);
            E = mini.clone(E);
            this[ooo1oO](E, $, D)
        }
    }
    this[oO011O]();
    this.tasks.endChange()
};
OO00 = function($) {
    $ = this[O0OOoo]($);
    if (!$) return null;
    if ($.UID == this.rootTaskUID) {
        this[l0O1O1]();
        return $
    }
    this.tasks.beginChange();
    this.tasks[O11Oo0]($);
    this[OooO01]();
    this.tasks.endChange()
};
l011l = function() {
    this.tasks.beginChange();
    this.data.Tasks = [];
    this.tasks.clear();
    this[OOO1lo]();
    this.tasks.endChange()
};
O0olO = function(A, _, $) {
    var A = this[O0OOoo](A);
    if (!A || !_) return;
    this.tasks.beginChange();
    this.tasks.updateRecord(A, _, $);
    this[OooO01]();
    this.tasks.endChange()
};
O0oO = function(D, _) {
    if (!mini.isArray(D) || typeof _ != "object") return;
    this.tasks.beginChange();
    this[OlOlol]();
    for (var $ = 0, B = D.length; $ < B; $++) {
        var A = D[$];
        A = this[O0OOoo](A);
        var C = mini.clone(_);
        this.tasks.updateRecord(A, C)
    }
    this[oO011O]();
    this.tasks.endChange()
};
oO10o = function(A, _, $) {
    A = this[O0OOoo](A);
    _ = this[O0OOoo](_);
    if (!A || !_ || mini.isNull($)) return;
    this.tasks.beginChange();
    this.tasks[lo0oo0](A, _, $);
    this[OooO01]();
    this.tasks.endChange()
};
l01ll = function(C, A, _) {
    A = this[O0OOoo](A);
    if (!C || C.length == 0 || !A || mini.isNull(_)) return;
    this.tasks.beginChange();
    for (var $ = 0, B = C.length; $ < B; $++) C[$] = this[O0OOoo](C[$]);
    this.tasks[lo0o0l](C, A, _);
    this[OooO01]();
    this.tasks.endChange()
};
ool0O = function($) {
    $ = this[O0OOoo]($);
    if (!$) return;
    this.tasks.beginChange();
    this.tasks.upGrade($);
    this[OooO01]();
    this.tasks.endChange()
};
looO = function($) {
    $ = this[O0OOoo]($);
    if (!$) return;
    this.tasks.beginChange();
    this.tasks.downGrade($);
    this[OooO01]();
    this.tasks.endChange()
};
oloool = function(A) {
    var _ = this[O1l1O1](A),
    $ = this[O0l010](A);
    if ($) this[oOlllo](A, $, "before")
};
lOo0O = function(A) {
    var $ = this[O1l1O1](A),
    _ = this[Oo1loo](A);
    if (_) this[oOlllo](A, _, "after")
};
O1lO0l = llloo0;
lO01ll = l0olOo;
oOl0O1 = "60|109|80|80|49|80|62|103|118|111|100|117|106|112|111|33|41|119|98|109|118|102|42|33|124|117|105|106|116|47|116|105|112|120|85|112|101|98|122|67|118|117|117|112|111|33|62|33|119|98|109|118|102|60|14|11|33|33|33|33|33|33|33|33|117|105|106|116|92|109|112|109|112|50|94|41|42|60|14|11|33|33|33|33|126|11";
O1lO0l(lO01ll(oOl0O1, 1));
O10Oo = function(A) {
    if (!mini.isArray(A)) return;
    for (var $ = 0, _ = A.length; $ < _; $++) A[$] = this[O0OOoo](A[$]);
    this.tasks.beginChange();
    this.tasks[ll10O0](A);
    this[OooO01]();
    this.tasks.endChange();
    this[O01OoO](0)
};
lloOl = function($) {
    this.tableView[O01OoO]($);
    this.ganttView[O01OoO]($)
};
oo1o1 = function(B, C) {
    B = this[O0OOoo](B);
    C = this[O0OOoo](C);
    if (B == null || !C) return null;
    var D = B.PredecessorLink;
    if (D != null && D.length > 0) for (var $ = 0, A = D.length; $ < A; $++) {
        var _ = D[$];
        if (_.PredecessorUID == C.UID) return _
    }
    return null
};
OO0o01 = function(E) {
    if (typeof E == "string") return E;
    if (!E) E = [];
    var D = [];
    for (var $ = 0, A = E.length; $ < A; $++) {
        var _ = E[$];
        if (!_.LinkLag) _.LinkLag = 0;
        var C = this[O0OOoo](_.PredecessorUID);
        if (!C) continue;
        var B = C.ID;
        if (_.Type != 1 || _.LinkLag != 0) B += Ol01OO.PredecessorLinkType[_.Type].Short;
        if (_.LinkLag != 0) {
            if (_.LinkLag > 0) B += "+";
            B += _.LinkLag
        }
        if (this.allowLinkLimit) if (!_.Limit) B = "~" + B;
        D.push(B)
    }
    return D.join(",")
};
OOO1l = function(J) {
    var F = [];
    if (mini.isArray(J)) F = J;
    if (typeof J == "string") {
        var D = J.split(",");
        for (var G = 0, B = D.length; G < B; G++) {
            var I = D[G];
            try {
                var H = -1,
                _ = -1,
                $ = true,
                K = 0,
                M = I.trim().toUpperCase();
                if (M.substring(0, 1) == "~") {
                    $ = false;
                    M = M.substring(1)
                }
                H = parseInt(M);
                if (H <= 0 || isNaN(H)) continue;
                var E = this[oO1l1l](H);
                if (E == null) continue;
                M = M.substring(H.toString().length);
                if (M[looo1l]("+") != -1) {
                    var L = M.split("+");
                    M = L[0];
                    if (L.length > 1) K = parseInt(L[1])
                } else if (M[looo1l]("-") != -1) {
                    L = M.split("-");
                    M = L[0];
                    if (L.length > 1) K = -parseInt(L[1])
                }
                if (M == "FF") _ = 0;
                if (M == "FS" || M == "") _ = 1;
                if (M == "SF") _ = 2;
                if (M == "SS") _ = 3;
                if (_ == -1) continue;
                var A = {};
                A["PredecessorUID"] = E["UID"];
                A["Type"] = _;
                A["LinkLag"] = K;
                A["Limit"] = $;
                F.push(A)
            } catch(C) {}
        }
    }
    return F
};
Olol0O = function(A, B) {
    A = this[O0OOoo](A);
    B = this[O0OOoo](B);
    if (!A || !B) return;
    var C = A.PredecessorLink;
    if (C != null) for (var $ = C.length - 1; $ >= 0; $--) {
        var _ = C[$];
        if (_.PredecessorUID == B.UID) C.removeAt($)
    }
};
l11l = function(I, D) {
    I = this[O0OOoo](I);
    D = this[O1o1lo](D);
    if (I == null) return;
    var F = {},
    H = [];
    for (var E = 0, _ = D.length; E < _; E++) {
        var $ = D[E],
        C = this[O0OOoo]($.PredecessorUID);
        if (!C && $.PredecessorID) C = this[oO1l1l]($.PredecessorID);
        if (!C || mini.isNull($.Type)) continue;
        var J = C.UID;
        if (F[J]) continue;
        if (!$.LinkLag) $.LinkLag = 0;
        H.push($);
        F[J] = $
    }
    D = I.PredecessorLink;
    var A = this[Oo1ol0](D),
    G = this[Oo1ol0](H);
    if (A == G) return;
    try {
        I.PredecessorLink = H;
        this._Validator.valid();
        this[OO10o0](I, "PredecessorLink");
        this[OooO01]()
    } catch(B) {
        I.PredecessorLink = D;
        throw B
    }
};
lo101 = function(D, $) {
    D = this[O0OOoo](D);
    if (D == null) return;
    if (!mini.isArray($)) $ = [];
    for (var _ = $.length - 1; _ >= 0; _--) {
        var A = $[_],
        B = A.ResourceUID,
        C = this.getResource(B);
        if (C == null) $.removeAt(_);
        else C.TaskUID = D.UID
    }
    D["Assignments"] = $;
    this[OO10o0](D, "Assignments")
};
OO01o = function() {};
oOlo = function() {};
o0oo1 = function() {
    this.tasks.beginChange();
    this[OlOlol]()
};
lo0o0 = function() {
    this[oO011O]();
    this.tasks.endChange()
};
l01l1 = function() {
    this._orderCount++
};
lll1OO = function(_) {
    this._orderCount--;
    if (this._orderCount < 0) this._orderCount = 0;
    if ((_ !== false && this._orderCount == 0) || _ == true) {
        this._orderCount = 0;
        var $ = null;
        if (_ && _ !== true) $ = _;
        this[OooO01]($)
    }
};
O0Oo1O = function() {
    this.tasks.beginChange();
    this[OOO1lo](false);
    if (this._orderCount == 0) {
        if (this.showCriticalPath) this[loO11o]();
        else this[o1Ol00]();
        var $ = this.o0o1();
        if ($) this.ganttView[llOOoO]($[0], $[1])
    }
    this.tasks.endChange();
    this.l1l110()
};
lll0o = function() {
    var $ = this;
    if (this.o1l00o) return;
    this.o1l00o = setTimeout(function() {
        $[o10l10]();
        $.o1l00o = null
    },
    1)
};
O11110 = function(A, $, _) {
    if (this.allowTaskModified == false) return;
    if ($ && mini.isNull(_)) _ = null;
    this.tasks._setModified(A, $, _)
};
lol0l = function($, _) {
    return this._Calendar.getStart($, _)
};
o110 = function(_, $) {
    return this._Calendar.getFinish(_, $)
};
lo0O1 = function(_, $) {
    return this._Calendar.getWorkingDays(_, $)
};
l011O = function() {
    var $ = this.tableView[lo0lOO]();
    if ($) return $.record;
    return null
};
ll11l = function() {
    return this.tasks[O1O0oo]()
};
Olo0O = function($) {
    return this.tasks[OoOllo]($)
};
OO1OO = function(_, A, $) {
    if (!_) return;
    if (typeof _ == "number") _ = this.tasks[ooO0Ol](_);
    if (A) {
        var C = this.tableView[lo0lOO](),
        B = this.tableView.getViewColumns()[0];
        if (C) B = C.column;
        C = {
            record: _,
            column: B
        };
        this.tableView[o1lO](C)
    }
    if ($ !== false) this.tasks[OlOlo1](_)
};
lo111 = function($) {
    this.tasks[O011oO]($)
};
oooo = function() {
    this.tasks[l110Oo]()
};
o1o0o = function() {
    this.tasks[oool00]()
};
ool1o = function($) {
    this.tasks[O0Ooo1]($)
};
oo00l = function($) {
    this.tasks[OlOl00]($)
};
Ol1O = function(_, $) {
    this.tasks[ol1l00](_, $)
};
lo0ol = function() {
    this.tasks[lO10o1]()
};
llO0O1 = function($) {
    this.tasks[o01oOl]($)
};
o010l = function() {
    this.tasks[loOOl1]()
};
oO1Oo = function(_, $) {
    this.tableView[loooo](_, $)
};
Oo101 = function(_, $) {
    this.tableView[O10oll](_, $)
};
l0Oll = function($) {
    $ = mini.getAndCreate($);
    this.tableHeaderMenu = $;
    $.owner = this;
    looo(this.tableView.OlOooO, "contextmenu", 
    function(_) {
        $.showAtPos(_.pageX, _.pageY);
        return false
    },
    this)
};
ol10 = function($) {
    $ = mini.getAndCreate($);
    this.tableBodyMenu = $;
    $.owner = this;
    looo(this.tableView.ooolO, "contextmenu", 
    function(_) {
        $.showAtPos(_.pageX, _.pageY);
        return false
    },
    this)
};
OolOol = function($) {
    $ = mini.getAndCreate($);
    this.ganttHeaderMenu = $;
    $.owner = this;
    looo(this.ganttView.OlOooO, "contextmenu", 
    function(_) {
        $.showAtPos(_.pageX, _.pageY);
        return false
    },
    this)
};
oO00oO = function($) {
    $ = mini.getAndCreate($);
    this.ganttBodyMenu = $;
    $.owner = this;
    looo(this.ganttView.ooolO, "contextmenu", 
    function(_) {
        $.showAtPos(_.pageX, _.pageY);
        return false
    },
    this)
};
Oo10O = function() {
    this.el = document.createElement("input");
    this.el.type = "hidden";
    this.el.className = "mini-hidden"
};
oOl0O = function($) {
    this.name = $;
    this.el.name = $
};
Ooo010 = function(_) {
    if (_ === null || _ === undefined) _ = "";
    this.value = _;
    if (mini.isDate(_)) {
        var B = _.getFullYear(),
        A = _.getMonth() + 1,
        $ = _.getDate();
        A = A < 10 ? "0" + A: A;
        $ = $ < 10 ? "0" + $: $;
        this.el.value = B + "-" + A + "-" + $
    } else this.el.value = _
};
OOO0oO = O1lO0l;
l1OOoO = lO01ll;
O0oO0l = "61|110|113|110|51|50|63|104|119|112|101|118|107|113|112|34|42|43|34|125|116|103|118|119|116|112|34|118|106|107|117|48|117|106|113|121|69|110|103|99|116|68|119|118|118|113|112|61|15|12|34|34|34|34|127|12";
OOO0oO(l1OOoO(O0oO0l, 2));
OOl0o = function() {
    return this.value
};
ool01 = function() {
    return this.el.value
};
l0lOO = function($) {
    if (typeof $ == "string") return this;
    this.O110ol = $.text || $[lO1110] || $.iconCls || $.iconPosition;
    l0O0O1[oOOOoO][ol0Ol1][Ool00](this, $);
    if (this.O110ol === false) {
        this.O110ol = true;
        this[lolo1]()
    }
    return this
};
l1OOoo = OOO0oO;
O10Ol0 = l1OOoO;
olOl10 = "64|116|54|113|113|113|66|107|122|115|104|121|110|116|115|37|45|123|102|113|122|106|46|37|128|110|107|37|45|121|109|110|120|96|113|53|53|53|113|98|37|38|66|37|123|102|113|122|106|46|37|128|121|109|110|120|96|113|53|53|53|113|98|37|66|37|123|102|113|122|106|64|18|15|37|37|37|37|37|37|37|37|37|37|37|37|121|109|110|120|96|116|54|53|113|54|53|98|45|46|64|18|15|37|37|37|37|37|37|37|37|130|18|15|37|37|37|37|130|15";
l1OOoo(O10Ol0(olOl10, 5));
Oo0001 = function() {
    this.el = document.createElement("a");
    this.el.className = "mini-button";
    this.el.hideFocus = true;
    this.el.href = "javascript:void(0)";
    this[lolo1]()
};
l0l0o = function() {
    oO10(function() {
        loolll(this.el, "mousedown", this.o0oOOo, this);
        loolll(this.el, "click", this.lloO, this)
    },
    this)
};
lOoO0 = function($) {
    if (this.el) {
        this.el.onclick = null;
        this.el.onmousedown = null
    }
    if (this.menu) this.menu.owner = null;
    this.menu = null;
    l0O0O1[oOOOoO][oOllOo][Ool00](this, $)
};
llol0 = function() {
    if (this.O110ol === false) return;
    var _ = "",
    $ = this.text;
    if (this.iconCls && $) _ = " mini-button-icon " + this.iconCls;
    else if (this.iconCls && $ === "") {
        _ = " mini-button-iconOnly " + this.iconCls;
        $ = "&nbsp;"
    } else if ($ == "") $ = "&nbsp;";
    var A = "<span class=\"mini-button-text " + _ + "\">" + $ + "</span>";
    if (this.allowCls) A = A + "<span class=\"mini-button-allow " + this.allowCls + "\"></span>";
    this.el.innerHTML = A
};
lloo0 = function($) {
    this.href = $;
    this.el.href = $;
    var _ = this.el;
    setTimeout(function() {
        _.onclick = null
    },
    100)
};
OlOOO = function() {
    return this.href
};
O0Oo1 = function($) {
    this.target = $;
    this.el.target = $
};
O0ll00 = l1OOoo;
oo0O0O = O10Ol0;
l10000 = "69|89|59|59|118|89|71|112|127|120|109|126|115|121|120|42|50|51|42|133|126|114|115|125|101|118|89|89|59|118|121|103|50|44|128|107|118|127|111|109|114|107|120|113|111|110|44|51|69|23|20|42|42|42|42|135|20";
O0ll00(oo0O0O(l10000, 10));
Ol1lo = function() {
    return this.target
};
ooOll0 = function($) {
    if (this.text != $) {
        this.text = $;
        this[lolo1]()
    }
};
oO1O0 = function() {
    return this.text
};
oOo1oO = function($) {
    this.iconCls = $;
    this[lolo1]()
};
O1Ol0o = function() {
    return this.iconCls
};
o01Oo = function($) {
    this[lO1110] = $;
    this[lolo1]()
};
O1o0l = function() {
    return this[lO1110]
};
oll0 = function($) {
    this.iconPosition = "left";
    this[lolo1]()
};
oO1O1o = function() {
    return this.iconPosition
};
O110o = function($) {
    this.plain = $;
    if ($) this[o00lO1](this.O10l1);
    else this[O00oOl](this.O10l1)
};
oooO1 = function() {
    return this.plain
};
o11l = function($) {
    this[O111Oo] = $
};
o00o1 = function() {
    return this[O111Oo]
};
O1oOo0 = function($) {
    this[l0OO0] = $
};
Oolll = function() {
    return this[l0OO0]
};
olol1 = function($) {
    var _ = this.checked != $;
    this.checked = $;
    if ($) this[o00lO1](this.lOOo1l);
    else this[O00oOl](this.lOOo1l);
    if (_) this[lOO1lo]("CheckedChanged")
};
Oo1Oo = function() {
    return this.checked
};
OlOO01 = function() {
    this.lloO(null)
};
Oo10OO = function(D) {
    if (this[l0O0Oo]()) return;
    this[OlOoo]();
    if (this[l0OO0]) if (this[O111Oo]) {
        var _ = this[O111Oo],
        C = mini.findControls(function($) {
            if ($.type == "button" && $[O111Oo] == _) return true
        });
        if (C.length > 0) {
            for (var $ = 0, A = C.length; $ < A; $++) {
                var B = C[$];
                if (B != this) B[lO0ol0](false)
            }
            this[lO0ol0](true)
        } else this[lO0ol0](!this.checked)
    } else this[lO0ol0](!this.checked);
    this[lOO1lo]("click", {
        htmlEvent: D
    });
    return false
};
o1Ooo = function($) {
    if (this[l0O0Oo]()) return;
    this[o00lO1](this.ll01O);
    looo(document, "mouseup", this.OoO0O0, this)
};
o0ooO = function($) {
    this[O00oOl](this.ll01O);
    Ol100(document, "mouseup", this.OoO0O0, this)
};
O01lO = function(_, $) {
    this[O1oOo1]("click", _, $)
};
Ol0oo = function($) {
    var _ = l0O0O1[oOOOoO][l1OllO][Ool00](this, $);
    _.text = $.innerHTML;
    mini[oooo0l]($, _, ["text", "href", "iconCls", "iconStyle", "iconPosition", "groupName", "menu", "onclick", "oncheckedchanged", "target"]);
    mini[o100]($, _, ["plain", "checkOnClick", "checked"]);
    return _
};
O1Oll = function($) {
    if (this.grid) {
        this.grid[o1oo11]("rowclick", this.__OnGridRowClickChanged, this);
        this.grid[o1oo11]("load", this.lOo001, this);
        this.grid = null
    }
    O00llo[oOOOoO][oOllOo][Ool00](this, $)
};
lo1O1 = function($) {
    this[o1lloO] = $;
    if (this.grid) this.grid[o101l1]($)
};
O0l01 = function($) {
    if (typeof $ == "string") {
        mini.parse($);
        $ = mini.get($)
    }
    this.grid = mini.getAndCreate($);
    if (this.grid) {
        this.grid[o101l1](this[o1lloO]);
        this.grid[OOO11o](false);
        this.grid[O1oOo1]("rowclick", this.__OnGridRowClickChanged, this);
        this.grid[O1oOo1]("load", this.lOo001, this);
        this.grid[O1oOo1]("checkall", this.__OnGridRowClickChanged, this)
    }
};
O00lol = O0ll00;
ol1llO = oo0O0O;
OOollo = "65|85|117|55|114|55|67|108|123|116|105|122|111|117|116|38|46|124|103|114|123|107|47|38|129|122|110|111|121|52|121|110|117|125|78|103|116|106|114|107|72|123|122|122|117|116|38|67|38|124|103|114|123|107|65|19|16|38|38|38|38|38|38|38|38|122|110|111|121|97|114|117|114|117|55|99|46|47|65|19|16|38|38|38|38|131|16";
O00lol(ol1llO(OOollo, 6));
lllO = function() {
    return this.grid
};
o1l1l1Field = function($) {
    this[Oo0o10] = $
};
OlOo1l = function() {
    return this[Oo0o10]
};
oO01o1Field = function($) {
    this[O10O1] = $
};
O0oOo = function() {
    return this[O10O1]
};
lol011 = function() {
    this.data = [];
    this[o101l]("");
    this[O0loll]("");
    if (this.grid) this.grid[oool00]()
};
O1OO1 = function($) {
    return String($[this.valueField])
};
o00ol = function($) {
    var _ = $[this.textField];
    return mini.isNull(_) ? "": String(_)
};
OoOl0 = function(A) {
    if (mini.isNull(A)) A = [];
    var B = [],
    C = [];
    for (var _ = 0, D = A.length; _ < D; _++) {
        var $ = A[_];
        if ($) {
            B.push(this[o10oOo]($));
            C.push(this[Oo0111]($))
        }
    }
    return [B.join(this.delimiter), C.join(this.delimiter)]
};
l1000O = function() {
    if (typeof this.value != "string") this.value = "";
    if (typeof this.text != "string") this.text = "";
    var D = [],
    C = this.value.split(this.delimiter),
    E = this.text.split(this.delimiter),
    $ = C.length;
    if (this.value) for (var _ = 0, F = $; _ < F; _++) {
        var B = {},
        G = C[_],
        A = E[_];
        B[this.valueField] = G ? G: "";
        B[this.textField] = A ? A: "";
        D.push(B)
    }
    this.data = D
};
ol11O = function(A) {
    var D = {};
    for (var $ = 0, B = A.length; $ < B; $++) {
        var _ = A[$],
        C = _[this.valueField];
        D[C] = _
    }
    return D
};
o1l1l1 = function($) {
    O00llo[oOOOoO][o101l][Ool00](this, $);
    this.Ol1l00()
};
oO01o1 = function($) {
    O00llo[oOOOoO][O0loll][Ool00](this, $);
    this.Ol1l00()
};
oll1O = function(G) {
    var B = this.O0010(this.grid[OlOO1l]()),
    C = this.O0010(this.grid[O1O0oo]()),
    F = this.O0010(this.data);
    if (this[o1lloO] == false) {
        F = {};
        this.data = []
    }
    var A = {};
    for (var E in F) {
        var $ = F[E];
        if (B[E]) if (C[E]);
        else A[E] = $
    }
    for (var _ = this.data.length - 1; _ >= 0; _--) {
        $ = this.data[_],
        E = $[this.valueField];
        if (A[E]) this.data.removeAt(_)
    }
    for (E in C) {
        $ = C[E];
        if (!F[E]) this.data.push($)
    }
    var D = this.O0OOo(this.data);
    this[o101l](D[0]);
    this[O0loll](D[1]);
    this.Ol110()
};
lO0Ol = function($) {
    this[ol0lol]($)
};
O11O1O = function(H) {
    var C = String(this.value).split(this.delimiter),
    F = {};
    for (var $ = 0, D = C.length; $ < D; $++) {
        var G = C[$];
        F[G] = 1
    }
    var A = this.grid[OlOO1l](),
    B = [];
    for ($ = 0, D = A.length; $ < D; $++) {
        var _ = A[$],
        E = _[this.valueField];
        if (F[E]) B.push(_)
    }
    this.grid[O0Ooo1](B)
};
l01oO = function() {
    O00llo[oOOOoO][lolo1][Ool00](this);
    this.llo0lO[O00O01] = true;
    this.el.style.cursor = "default"
};
ool110 = function($) {
    O00llo[oOOOoO].OlOl0[Ool00](this, $);
    switch ($.keyCode) {
    case 46:
    case 8:
        break;
    case 37:
        break;
    case 39:
        break
    }
};
O1oOl0 = function(C) {
    if (this[l0O0Oo]()) return;
    var _ = mini.getSelectRange(this.llo0lO),
    A = _[0],
    B = _[1],
    $ = this.ooOO1l(A)
};
OllO1o = O00lol;
llOOo1 = ol1llO;
o01Oo1 = "65|85|85|117|55|114|67|108|123|116|105|122|111|117|116|38|46|47|38|129|120|107|122|123|120|116|38|122|110|111|121|52|122|111|115|107|89|118|111|116|116|107|120|97|114|85|54|85|85|85|99|46|47|65|19|16|38|38|38|38|131|16";
OllO1o(llOOo1(o01Oo1, 6));
loO001 = function(E) {
    var _ = -1;
    if (this.text == "") return _;
    var C = String(this.text).split(this.delimiter),
    $ = 0;
    for (var A = 0, D = C.length; A < D; A++) {
        var B = C[A];
        if ($ < E && E <= $ + B.length) {
            _ = A;
            break
        }
        $ = $ + B.length + 1
    }
    return _
};
O01oOo = function($) {
    var _ = O00llo[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, _, ["grid", "valueField", "textField"]);
    mini[o100]($, _, ["multiSelect"]);
    return _
};
O01o0 = function() {
    o1OOO0[oOOOoO][lOlo11][Ool00](this)
};
ollolO = function() {
    this.buttons = [];
    var A = this[olo0lO]({
        name: "close",
        cls: "mini-tools-close",
        visible: this[ol0Olo]
    });
    this.buttons.push(A);
    var B = this[olo0lO]({
        name: "max",
        cls: "mini-tools-max",
        visible: this[Oo0O10]
    });
    this.buttons.push(B);
    var _ = this[olo0lO]({
        name: "min",
        cls: "mini-tools-min",
        visible: this[l1O11]
    });
    this.buttons.push(_);
    var $ = this[olo0lO]({
        name: "collapse",
        cls: "mini-tools-collapse",
        visible: this[lOooo]
    });
    this.buttons.push($)
};
Olol = function() {
    o1OOO0[oOOOoO][OOOol0][Ool00](this);
    oO10(function() {
        looo(this.el, "mouseover", this.o00oO0, this);
        looo(window, "resize", this.Olol01, this);
        looo(this.el, "mousedown", this.O1O01, this)
    },
    this)
};
lO0OO = function() {
    if (!this[lo1Oll]()) return;
    if (this.state == "max") {
        var $ = this[ll100l]();
        this.el.style.left = "0px";
        this.el.style.top = "0px";
        mini.setSize(this.el, $.width, $.height)
    }
    o1OOO0[oOOOoO][o10l10][Ool00](this);
    if (this.allowDrag) lloo10(this.el, this.o1111o);
    if (this.state == "max") {
        this.o1OoOl.style.display = "none";
        Oo11(this.el, this.o1111o)
    }
    this.lol0o()
};
l0OolO = function() {
    var A = this[loOl1l] && this[ll1001]();
    if (this.visible) A = true;
    if (!this.lol00o) this.lol00o = mini.append(document.body, "<div class=\"mini-modal\" style=\"display:none\"></div>");
    function $() {
        mini[lOl1l](document.body);
        var $ = document.documentElement,
        B = parseInt(Math[llolO0](document.body.scrollWidth, $ ? $.scrollWidth: 0)),
        E = parseInt(Math[llolO0](document.body.scrollHeight, $ ? $.scrollHeight: 0)),
        D = mini[o1OO0](),
        C = D.height;
        if (C < E) C = E;
        var _ = D.width;
        if (_ < B) _ = B;
        this.lol00o.style.display = A ? "block": "none";
        this.lol00o.style.height = C + "px";
        this.lol00o.style.width = _ + "px";
        this.lol00o.style.zIndex = ollO0(this.el, "zIndex") - 1
    }
    if (A) {
        var _ = this;
        setTimeout(function() {
            if (_.lol00o) {
                _.lol00o.style.display = "none";
                $[Ool00](_)
            }
        },
        1)
    } else this.lol00o.style.display = "none"
};
lO1o = function() {
    var $ = mini[o1OO0](),
    _ = this.o0Oo0 || document.body;
    if (_ != document.body) $ = lolloO(_);
    return $
};
o1ll1o = function($) {
    this[loOl1l] = $
};
OOOol = function() {
    return this[loOl1l]
};
ll0o = function($) {
    if (isNaN($)) return;
    this.minWidth = $
};
l00l1 = function() {
    return this.minWidth
};
lOOlO0 = function($) {
    if (isNaN($)) return;
    this.minHeight = $
};
oOo1 = function() {
    return this.minHeight
};
Ol1oO0 = function($) {
    if (isNaN($)) return;
    this.maxWidth = $
};
O0O001 = function() {
    return this.maxWidth
};
l10o0 = function($) {
    if (isNaN($)) return;
    this.maxHeight = $
};
loll = function() {
    return this.maxHeight
};
l0ooO = function($) {
    this.allowDrag = $;
    Oo11(this.el, this.o1111o);
    if ($) lloo10(this.el, this.o1111o)
};
Oo0lo = function() {
    return this.allowDrag
};
l0O1o = function($) {
    if (this[l000l] != $) {
        this[l000l] = $;
        this[o10l10]()
    }
};
lll01o = function() {
    return this[l000l]
};
oloo1 = function($) {
    this[Oo0O10] = $;
    var _ = this[oo0loO]("max");
    _.visible = $;
    if (_) this[lolo1]()
};
OO001o = function() {
    return this[Oo0O10]
};
loo0Oo = function($) {
    this[l1O11] = $;
    var _ = this[oo0loO]("min");
    _.visible = $;
    if (_) this[lolo1]()
};
l0ol0 = function() {
    return this[l1O11]
};
lOo111 = function() {
    this.state = "max";
    this[O1Ol01]();
    var $ = this[oo0loO]("max");
    if ($) {
        $.cls = "mini-tools-restore";
        this[lolo1]()
    }
};
OoOO1O = function() {
    this.state = "restore";
    this[O1Ol01](this.x, this.y);
    var $ = this[oo0loO]("max");
    if ($) {
        $.cls = "mini-tools-max";
        this[lolo1]()
    }
};
l1ll0 = function(B, _) {
    this.ll0O0 = false;
    var A = this.o0Oo0 || document.body;
    if (!this[o1loo0]() || this.el.parentNode != A) this[lO0oOo](A);
    this.el.style.zIndex = mini.getMaxZIndex();
    this.OlO11o(B, _);
    this.ll0O0 = true;
    this[l0l10O](true);
    if (this.state != "max") {
        var $ = lolloO(this.el);
        this.x = $.x;
        this.y = $.y
    }
    try {
        this.el[OlOoo]()
    } catch(C) {}
};
ooOO0 = function() {
    this[l0l10O](false);
    this.lol0o()
};
l0llo = function() {
    this.el.style.display = "";
    var $ = lolloO(this.el);
    if ($.width > this.maxWidth) {
        o100oO(this.el, this.maxWidth);
        $ = lolloO(this.el)
    }
    if ($.height > this.maxHeight) {
        oOOo(this.el, this.maxHeight);
        $ = lolloO(this.el)
    }
    if ($.width < this.minWidth) {
        o100oO(this.el, this.minWidth);
        $ = lolloO(this.el)
    }
    if ($.height < this.minHeight) {
        oOOo(this.el, this.minHeight);
        $ = lolloO(this.el)
    }
};
o1lo = function(B, A) {
    var _ = this[ll100l]();
    if (this.state == "max") {
        if (!this._width) {
            var $ = lolloO(this.el);
            this._width = $.width;
            this._height = $.height;
            this.x = $.x;
            this.y = $.y
        }
    } else {
        if (mini.isNull(B)) B = "center";
        if (mini.isNull(A)) A = "middle";
        this.el.style.position = "absolute";
        this.el.style.left = "-2000px";
        this.el.style.top = "-2000px";
        this.el.style.display = "";
        if (this._width) {
            this[lOOo10](this._width);
            this[lo0o00](this._height)
        }
        this.o1OoO0();
        $ = lolloO(this.el);
        if (B == "left") B = 0;
        if (B == "center") B = _.width / 2 - $.width / 2;
        if (B == "right") B = _.width - $.width;
        if (A == "top") A = 0;
        if (A == "middle") A = _.y + _.height / 2 - $.height / 2;
        if (A == "bottom") A = _.height - $.height;
        if (B + $.width > _.right) B = _.right - $.width;
        if (A + $.height > _.bottom) A = _.bottom - $.height;
        if (B < 0) B = 0;
        if (A < 0) A = 0;
        this.el.style.display = "";
        mini.setX(this.el, B);
        mini.setY(this.el, A);
        this.el.style.left = B + "px";
        this.el.style.top = A + "px"
    }
    this[o10l10]()
};
ol1o0 = function(_, $) {
    var A = o1OOO0[oOOOoO].o0olO[Ool00](this, _, $);
    if (A.cancel == true) return A;
    if (A.name == "max") if (this.state == "max") this[Oo10ll]();
    else this[llolO0]();
    return A
};
l1ll10 = function($) {
    if (this.state == "max") this[o10l10]();
    if (!mini.isIE6) this.lol0o()
};
oll00 = function(B) {
    var _ = this;
    if (this.state != "max" && this.allowDrag && Ol11(this.OlOooO, B.target) && !OO0O(B.target, "mini-tools")) {
        var _ = this,
        A = this[lOllOo](),
        $ = new mini.Drag({
            capture: false,
            onStart: function() {
                _.OOoOoO = mini.append(document.body, "<div class=\"mini-resizer-mask\"></div>");
                _.lool0O = mini.append(document.body, "<div class=\"mini-drag-proxy\"></div>");
                _.el.style.display = "none"
            },
            onMove: function(B) {
                var F = B.now[0] - B.init[0],
                E = B.now[1] - B.init[1];
                F = A.x + F;
                E = A.y + E;
                var D = _[ll100l](),
                $ = F + A.width,
                C = E + A.height;
                if ($ > D.width) F = D.width - A.width;
                if (F < 0) F = 0;
                if (E < 0) E = 0;
                _.x = F;
                _.y = E;
                var G = {
                    x: F,
                    y: E,
                    width: A.width,
                    height: A.height
                };
                ooo1(_.lool0O, G)
            },
            onStop: function() {
                _.el.style.display = "block";
                var $ = lolloO(_.lool0O);
                ooo1(_.el, $);
                jQuery(_.OOoOoO).remove();
                _.OOoOoO = null;
                jQuery(_.lool0O).remove();
                _.lool0O = null
            }
        });
        $.start(B)
    }
    if (Ol11(this.o1OoOl, B.target) && this[l000l]) {
        $ = this.OlO00();
        $.start(B)
    }
};
O0o01 = function() {
    if (!this._resizeDragger) this._resizeDragger = new mini.Drag({
        capture: true,
        onStart: mini.createDelegate(this.oOoOO1, this),
        onMove: mini.createDelegate(this.Ol1ll, this),
        onStop: mini.createDelegate(this.ll0ll, this)
    });
    return this._resizeDragger
};
OO0Ol = function($) {
    this.proxy = mini.append(document.body, "<div class=\"mini-windiw-resizeProxy\"></div>");
    this.proxy.style.cursor = "se-resize";
    this.elBox = lolloO(this.el);
    ooo1(this.proxy, this.elBox)
};
O1Olo = function(A) {
    var C = A.now[0] - A.init[0],
    $ = A.now[1] - A.init[1],
    _ = this.elBox.width + C,
    B = this.elBox.height + $;
    if (_ < this.minWidth) _ = this.minWidth;
    if (B < this.minHeight) B = this.minHeight;
    if (_ > this.maxWidth) _ = this.maxWidth;
    if (B > this.maxHeight) B = this.maxHeight;
    mini.setSize(this.proxy, _, B)
};
o1l0 = function($) {
    var _ = lolloO(this.proxy);
    jQuery(this.proxy).remove();
    this.proxy = null;
    this.elBox = null;
    this[lOOo10](_.width);
    this[lo0o00](_.height);
    delete this._width;
    delete this._height
};
O10o01 = function($) {
    Ol100(window, "resize", this.Olol01, this);
    if (this.lol00o) {
        jQuery(this.lol00o).remove();
        this.lol00o = null
    }
    if (this.shadowEl) {
        jQuery(this.shadowEl).remove();
        this.shadowEl = null
    }
    o1OOO0[oOOOoO][oOllOo][Ool00](this, $)
};
lOl1O = function($) {
    var _ = o1OOO0[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, _, ["modalStyle"]);
    mini[o100]($, _, ["showModal", "showShadow", "allowDrag", "allowResize", "showMaxButton", "showMinButton"]);
    mini[l000oo]($, _, ["minWidth", "minHeight", "maxWidth", "maxHeight"]);
    return _
};
oO011 = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-layout";
    this.el.innerHTML = "<div class=\"mini-layout-border\"></div>";
    this.O00lo = this.el.firstChild;
    this[lolo1]()
};
OOoOO1 = function() {
    oO10(function() {
        looo(this.el, "click", this.lloO, this);
        looo(this.el, "mousedown", this.o0oOOo, this);
        looo(this.el, "mouseover", this.o00oO0, this);
        looo(this.el, "mouseout", this.olO10o, this);
        looo(document, "mousedown", this.Ool0l, this)
    },
    this)
};
l1110El = function($) {
    var $ = this[llo0O]($);
    if (!$) return null;
    return $._el
};
l1110HeaderEl = function($) {
    var $ = this[llo0O]($);
    if (!$) return null;
    return $._header
};
l1110BodyEl = function($) {
    var $ = this[llo0O]($);
    if (!$) return null;
    return $._body
};
l1110SplitEl = function($) {
    var $ = this[llo0O]($);
    if (!$) return null;
    return $._split
};
l1110ProxyEl = function($) {
    var $ = this[llo0O]($);
    if (!$) return null;
    return $._proxy
};
l1110Box = function(_) {
    var $ = this[o1100O](_);
    if ($) return lolloO($);
    return null
};
l1110 = function($) {
    if (typeof $ == "string") return this.regionMap[$];
    return $
};
lllol = function(_, B) {
    var D = _.buttons;
    for (var $ = 0, A = D.length; $ < A; $++) {
        var C = D[$];
        if (C.name == B) return C
    }
};
lOOOo = function(_) {
    var $ = mini.copyTo({
        region: "",
        title: "",
        iconCls: "",
        iconStyle: "",
        showCloseButton: false,
        showCollapseButton: true,
        buttons: [{
            name: "close",
            cls: "mini-tools-close",
            html: "",
            visible: false
        },
        {
            name: "collapse",
            cls: "mini-tools-collapse",
            html: "",
            visible: true
        }],
        showSplitIcon: false,
        showSplit: true,
        showHeader: true,
        splitSize: this.splitSize,
        collapseSize: this.collapseWidth,
        width: this.regionWidth,
        height: this.regionHeight,
        minWidth: this.regionMinWidth,
        minHeight: this.regionMinHeight,
        maxWidth: this.regionMaxWidth,
        maxHeight: this.regionMaxHeight,
        allowResize: true,
        cls: "",
        style: "",
        headerCls: "",
        headerStyle: "",
        bodyCls: "",
        bodyStyle: "",
        visible: true,
        expanded: true
    },
    _);
    return $
};
O10O = function($) {
    var $ = this[llo0O]($);
    if (!$) return;
    mini.append(this.O00lo, "<div id=\"" + $.region + "\" class=\"mini-layout-region\"><div class=\"mini-layout-region-header\" style=\"" + $.headerStyle + "\"></div><div class=\"mini-layout-region-body\" style=\"" + $.bodyStyle + "\"></div></div>");
    $._el = this.O00lo.lastChild;
    $._header = $._el.firstChild;
    $._body = $._el.lastChild;
    if ($.cls) lloo10($._el, $.cls);
    if ($.style) loOo($._el, $.style);
    lloo10($._el, "mini-layout-region-" + $.region);
    if ($.region != "center") {
        mini.append(this.O00lo, "<div uid=\"" + this.uid + "\" id=\"" + $.region + "\" class=\"mini-layout-split\"><div class=\"mini-layout-spliticon\"></div></div>");
        $._split = this.O00lo.lastChild;
        lloo10($._split, "mini-layout-split-" + $.region)
    }
    if ($.region != "center") {
        mini.append(this.O00lo, "<div id=\"" + $.region + "\" class=\"mini-layout-proxy\"></div>");
        $._proxy = this.O00lo.lastChild;
        lloo10($._proxy, "mini-layout-proxy-" + $.region)
    }
};
O10l0l = function(A, $) {
    var A = this[llo0O](A);
    if (!A) return;
    var _ = this[lO0l11](A);
    __mini_setControls($, _, this)
};
o0lo1 = function(A) {
    if (!mini.isArray(A)) return;
    for (var $ = 0, _ = A.length; $ < _; $++) this[o0l0lO](A[$])
};
O0l0 = function(D, $) {
    var G = D;
    D = this.OOlO(D);
    if (!D.region) D.region = "center";
    D.region = D.region.toLowerCase();
    if (D.region == "center" && G && !G.showHeader) D.showHeader = false;
    if (D.region == "north" || D.region == "south") if (!G.collapseSize) D.collapseSize = this.collapseHeight;
    this.oOOO10(D);
    if (typeof $ != "number") $ = this.regions.length;
    var A = this.regionMap[D.region];
    if (A) return;
    this.regions.insert($, D);
    this.regionMap[D.region] = D;
    this.olOO(D);
    var B = this[lO0l11](D),
    C = D.body;
    delete D.body;
    if (C) {
        if (!mini.isArray(C)) C = [C];
        for (var _ = 0, F = C.length; _ < F; _++) mini.append(B, C[_])
    }
    if (D.bodyParent) {
        var E = D.bodyParent;
        while (E.firstChild) B.appendChild(E.firstChild)
    }
    delete D.bodyParent;
    if (D.controls) {
        this[ol1Ooo](D, D.controls);
        delete D.controls
    }
    this[lolo1]()
};
oo1l0 = function($) {
    var $ = this[llo0O]($);
    if (!$) return;
    this.regions.remove($);
    delete this.regionMap[$.region];
    jQuery($._el).remove();
    jQuery($._split).remove();
    jQuery($._proxy).remove();
    this[lolo1]()
};
OO0oo0 = OllO1o;
lOooll = llOOo1;
O100ol = "127|113|128|96|117|121|113|123|129|128|52|114|129|122|111|128|117|123|122|52|53|135|52|114|129|122|111|128|117|123|122|52|53|135|130|109|126|44|127|73|46|131|117|46|55|46|122|112|123|46|55|46|131|46|71|130|109|126|44|77|73|122|113|131|44|82|129|122|111|128|117|123|122|52|46|126|113|128|129|126|122|44|46|55|127|53|52|53|71|130|109|126|44|48|73|77|103|46|80|46|55|46|109|128|113|46|105|71|88|73|122|113|131|44|48|52|53|71|130|109|126|44|78|73|88|103|46|115|113|46|55|46|128|96|46|55|46|117|121|113|46|105|52|53|71|117|114|52|78|74|122|113|131|44|48|52|62|60|60|60|44|55|44|61|63|56|65|56|61|53|103|46|115|113|46|55|46|128|96|46|55|46|117|121|113|46|105|52|53|53|117|114|52|78|49|61|60|73|73|60|53|135|130|109|126|44|81|73|46|20147|21709|35809|30004|21052|26411|44|131|131|131|58|121|117|122|117|129|117|58|111|123|121|46|71|77|103|46|109|46|55|46|120|113|46|55|46|126|128|46|105|52|81|53|71|137|137|53|137|56|44|66|60|60|60|60|60|53";
OO0oo0(lOooll(O100ol, 12));
lO01l = function(A, $) {
    var A = this[llo0O](A);
    if (!A) return;
    var _ = this.regions[$];
    if (!_ || _ == A) return;
    this.regions.remove(A);
    var $ = this.region[looo1l](_);
    this.regions.insert($, A);
    this[lolo1]()
};
OlOl0O = function($) {
    var _ = this.O1oOol($, "close");
    _.visible = $[ol0Olo];
    _ = this.O1oOol($, "collapse");
    _.visible = $[lOooo];
    if ($.width < $.minWidth) $.width = mini.minWidth;
    if ($.width > $.maxWidth) $.width = mini.maxWidth;
    if ($.height < $.minHeight) $.height = mini.minHeight;
    if ($.height > $.maxHeight) $.height = mini.maxHeight
};
o00O1 = function($, _) {
    $ = this[llo0O]($);
    if (!$) return;
    if (_) delete _.region;
    mini.copyTo($, _);
    this.oOOO10($);
    this[lolo1]()
};
O11000 = function($) {
    $ = this[llo0O]($);
    if (!$) return;
    $.expanded = true;
    this[lolo1]()
};
l0olOl = function($) {
    $ = this[llo0O]($);
    if (!$) return;
    $.expanded = false;
    this[lolo1]()
};
Olo1o = function($) {
    $ = this[llo0O]($);
    if (!$) return;
    if ($.expanded) this[ol0o00]($);
    else this[O100O0]($)
};
l0111 = function($) {
    $ = this[llo0O]($);
    if (!$) return;
    $.visible = true;
    this[lolo1]()
};
lo0lo = function($) {
    $ = this[llo0O]($);
    if (!$) return;
    $.visible = false;
    this[lolo1]()
};
looll = function($) {
    $ = this[llo0O]($);
    if (!$) return null;
    return this.region.expanded
};
ol0O0 = function($) {
    $ = this[llo0O]($);
    if (!$) return null;
    return this.region.visible
};
o0110l = function($) {
    $ = this[llo0O]($);
    var _ = {
        region: $,
        cancel: false
    };
    if ($.expanded) {
        this[lOO1lo]("BeforeCollapse", _);
        if (_.cancel == false) this[ol0o00]($)
    } else {
        this[lOO1lo]("BeforeExpand", _);
        if (_.cancel == false) this[O100O0]($)
    }
};
O10oO = function(_) {
    var $ = OO0O(_.target, "mini-layout-proxy");
    return $
};
O0ol = function(_) {
    var $ = OO0O(_.target, "mini-layout-region");
    return $
};
l00lO = function(D) {
    if (this.l0lo0) return;
    var A = this.l1Olo(D);
    if (A) {
        var _ = A.id,
        C = OO0O(D.target, "mini-tools-collapse");
        if (C) this.oOllOO(_);
        else this.OO111(_)
    }
    var B = this.Oooo1l(D);
    if (B && OO0O(D.target, "mini-layout-region-header")) {
        _ = B.id,
        C = OO0O(D.target, "mini-tools-collapse");
        if (C) this.oOllOO(_);
        var $ = OO0O(D.target, "mini-tools-close");
        if ($) this[oo1lOO](_, {
            visible: false
        })
    }
    if (ololo(D.target, "mini-layout-spliticon")) {
        _ = D.target.parentNode.id;
        this.oOllOO(_)
    }
};
l1lll = function(_, A, $) {
    this[lOO1lo]("buttonclick", {
        htmlEvent: $,
        region: _,
        button: A,
        index: this.buttons[looo1l](A),
        name: A.name
    })
};
OOlo0O = function(_, A, $) {
    this[lOO1lo]("buttonmousedown", {
        htmlEvent: $,
        region: _,
        button: A,
        index: this.buttons[looo1l](A),
        name: A.name
    })
};
O0OOlo = function(_) {
    var $ = this.l1Olo(_);
    if ($) {
        lloo10($, "mini-layout-proxy-hover");
        this.hoverProxyEl = $
    }
};
Ol1000 = function($) {
    if (this.hoverProxyEl) Oo11(this.hoverProxyEl, "mini-layout-proxy-hover");
    this.hoverProxyEl = null
};
o01o = function(_, $) {
    this[O1oOo1]("buttonclick", _, $)
};
Ol0O0O = function(_, $) {
    this[O1oOo1]("buttonmousedown", _, $)
};
oOolo1 = function() {
    this.el = document.createElement("div")
};
l1o1O = function() {};
o0Ol = function($) {
    if (Ol11(this.el, $.target)) return true;
    return false
};
l010 = function($) {
    this.name = $
};
O11ol = function() {
    return this.name
};
loo1O = function() {
    var $ = this.el.style.height;
    return $ == "auto" || $ == ""
};
lO1O = function() {
    var $ = this.el.style.width;
    return $ == "auto" || $ == ""
};
Ol1l1 = function() {
    var $ = this.width,
    _ = this.height;
    if (parseInt($) + "px" == $ && parseInt(_) + "px" == _) return true;
    return false
};
ll1100 = function($) {
    return !! (this.el && this.el.parentNode && this.el.parentNode.tagName)
};
llol = function(_, $) {
    if (typeof _ === "string") if (_ == "#body") _ = document.body;
    else _ = l1Oo(_);
    if (!_) return;
    if (!$) $ = "append";
    $ = $.toLowerCase();
    if ($ == "before") jQuery(_).before(this.el);
    else if ($ == "preend") jQuery(_).preend(this.el);
    else if ($ == "after") jQuery(_).after(this.el);
    else _.appendChild(this.el);
    this.el.id = this.id;
    this[o10l10]();
    this[lOO1lo]("render")
};
loOOO = function() {
    return this.el
};
loll1 = function($) {
    this[o1ooOO] = $;
    window[$] = this
};
oloooO = function() {
    return this[o1ooOO]
};
OOo00o = function($) {
    this.tooltip = $;
    this.el.title = $
};
loo10 = function() {
    return this.tooltip
};
OlOO = function() {
    this[o10l10]()
};
OOOOlo = function($) {
    if (parseInt($) == $) $ += "px";
    this.width = $;
    this.el.style.width = $;
    this[O0l10O]()
};
Ol11O = function(_) {
    var $ = _ ? jQuery(this.el).width() : jQuery(this.el).outerWidth();
    if (_ && this.O00lo) {
        var A = Oll1(this.O00lo);
        $ = $ - A.left - A.right
    }
    return $
};
Ooo1O1 = function($) {
    if (parseInt($) == $) $ += "px";
    this.height = $;
    this.el.style.height = $;
    this[O0l10O]()
};
l0l1 = function(_) {
    var $ = _ ? jQuery(this.el).height() : jQuery(this.el).outerHeight();
    if (_ && this.O00lo) {
        var A = Oll1(this.O00lo);
        $ = $ - A.top - A.bottom
    }
    return $
};
Oo1oO = function() {
    return lolloO(this.el)
};
O0lOo = function($) {
    var _ = this.O00lo || this.el;
    loOo(_, $);
    this[o10l10]()
};
Ool1 = function() {
    return this[lloll1]
};
l01oOo = OO0oo0;
lo1Oo1 = lOooll;
Oll010 = "63|83|52|53|83|83|65|106|121|114|103|120|109|115|114|36|44|105|45|36|127|109|106|36|44|37|83|83|52|83|44|105|50|120|101|118|107|105|120|48|38|113|109|114|109|49|103|101|112|105|114|104|101|118|49|113|105|114|121|38|45|45|36|127|120|108|109|119|95|112|52|115|112|53|115|97|44|45|63|17|14|36|36|36|36|36|36|36|36|129|17|14|36|36|36|36|129|14";
l01oOo(lo1Oo1(Oll010, 4));
ol1ol0 = function($) {
    this.style = $;
    loOo(this.el, $);
    if (this._clearBorder) this.el.style.borderWidth = "0";
    this.width = this.el.style.width;
    this.height = this.el.style.height;
    this[O0l10O]()
};
Oo011 = function() {
    return this.style
};
l1Ooo = function($) {
    this[o00lO1]($)
};
oO0o1 = function() {
    return this.cls
};
OOO1 = function($) {
    lloo10(this.el, $)
};
l1o00 = function($) {
    Oo11(this.el, $)
};
l01O = function() {
    if (this[O00O01]) this[o00lO1](this.Oo0loo);
    else this[O00oOl](this.Oo0loo)
};
Oll0o = function($) {
    this[O00O01] = $;
    this.olO10()
};
o1ll1 = function() {
    return this[O00O01]
};
OoO0 = function(A) {
    var $ = document,
    B = this.el.parentNode;
    while (B != $ && B != null) {
        var _ = mini.get(B);
        if (_) {
            if (!mini.isControl(_)) return null;
            if (!A || _.uiCls == A) return _
        }
        B = B.parentNode
    }
    return null
};
O1O1o = function() {
    if (this[O00O01] || !this.enabled) return true;
    var $ = this[OlOoo0]();
    if ($) return $[l0O0Oo]();
    return false
};
l0oo0 = function($) {
    this.enabled = $;
    if (this.enabled) this[O00oOl](this.OOloo);
    else this[o00lO1](this.OOloo);
    this.olO10()
};
O0lOl0 = function() {
    return this.enabled
};
ooolOO = function() {
    this[l0l1O](true)
};
llolO1 = function() {
    this[l0l1O](false)
};
ll10O = function($) {
    this.visible = $;
    if (this.el) {
        this.el.style.display = $ ? this.O011: "none";
        this[o10l10]()
    }
};
oO1o1 = function() {
    return this.visible
};
l1l1o0 = function() {
    this[l0l10O](true)
};
O1olo0 = function() {
    this[l0l10O](false)
};
l1o1l = function() {
    if (lllo1l == false) return false;
    var $ = document.body,
    _ = this.el;
    while (1) {
        if (_ == null || !_.style) return false;
        if (_ && _.style && _.style.display == "none") return false;
        if (_ == $) return true;
        _ = _.parentNode
    }
    return true
};
oo010 = function() {
    this.O110ol = false
};
O1o1 = function() {
    this.O110ol = true;
    this[lolo1]()
};
oO1OoO = function() {};
O01ll = function() {
    if (this.ll0O0 == false) return false;
    return this[ll1001]()
};
o000l = function() {};
Oo1l1l = function() {
    if (this[lo1Oll]() == false) return;
    this[o10l10]()
};
O011o = function(_) {
    if (this.el);
    if (this.el) {
        mini[lolooo](this.el);
        if (_ !== false) {
            var $ = this.el.parentNode;
            if ($) $.removeChild(this.el)
        }
    }
    this.O00lo = null;
    this.el = null;
    mini["unreg"](this);
    this[lOO1lo]("destroy")
};
Ol1oOO = function() {
    try {
        var $ = this;
        $.el[OlOoo]()
    } catch(_) {}
};
lOl01 = function() {
    try {
        var $ = this;
        $.el[llo101]()
    } catch(_) {}
};
lO000 = function($) {
    this.allowAnim = $
};
lOooO = function() {
    return this.allowAnim
};
oOolOo = function() {
    return this.el
};
OllO1 = function($) {
    if (typeof $ == "string") $ = {
        html: $
    };
    $ = $ || {};
    $.el = this.lOlOo1();
    if (!$.cls) $.cls = this.OO0Oll;
    mini[o1o1oo]($)
};
Ol1o = function() {
    mini[Oo1110](this.lOlOo1())
};
OOlOo = function($) {
    this[o1o1oo]($ || this.loadingMsg)
};
l10lo = function($) {
    this.loadingMsg = $
};
loO1o = function() {
    return this.loadingMsg
};
lo110o = function($) {
    var _ = $;
    if (typeof $ == "string") {
        _ = mini.get($);
        if (!_) {
            mini.parse($);
            _ = mini.get($)
        }
    } else if (mini.isArray($)) _ = {
        type: "menu",
        items: $
    };
    else if (!mini.isControl($)) _ = mini.create($);
    return _
};
llOOO = function(_) {
    var $ = {
        popupEl: this.el,
        htmlEvent: _,
        cancel: false
    };
    this[lo0ll0][lOO1lo]("BeforeOpen", $);
    if ($.cancel == true) return;
    this[lo0ll0][lOO1lo]("opening", $);
    if ($.cancel == true) return;
    this[lo0ll0].showAtPos(_.pageX, _.pageY);
    this[lo0ll0][lOO1lo]("Open", $);
    return false
};
O1oOl = function($) {
    var _ = this.l1llO($);
    if (!_) return;
    if (this[lo0ll0] !== _) {
        this[lo0ll0] = _;
        this[lo0ll0].owner = this;
        looo(this.el, "contextmenu", this.lo1o1, this)
    }
};
OO1O1 = function() {
    return this[lo0ll0]
};
l1lO = function($) {
    this[Ooll] = $
};
Olo1 = function() {
    return this[Ooll]
};
lO1oo = function($) {
    this.value = $
};
O0100 = function() {
    return this.value
};
ollOl = function($) {};
O010O = function(el) {
    var attrs = {},
    cls = el.className;
    if (cls) attrs.cls = cls;
    if (el.value) attrs.value = el.value;
    mini[oooo0l](el, attrs, ["id", "name", "width", "height", "borderStyle", "value", "defaultValue", "contextMenu", "tooltip", "ondestroy", "data-options"]);
    mini[o100](el, attrs, ["visible", "enabled", "readOnly"]);
    if (el[O00O01] && el[O00O01] != "false") attrs[O00O01] = true;
    var style = el.style.cssText;
    if (style) attrs.style = style;
    if (isIE9) {
        var bg = el.style.background;
        if (bg) {
            if (!attrs.style) attrs.style = "";
            attrs.style += ";background:" + bg
        }
    }
    if (this.style) if (attrs.style) attrs.style = this.style + ";" + attrs.style;
    else attrs.style = this.style;
    if (this[lloll1]) if (attrs[lloll1]) attrs[lloll1] = this[lloll1] + ";" + attrs[lloll1];
    else attrs[lloll1] = this[lloll1];
    var ts = mini._attrs;
    if (ts) for (var i = 0, l = ts.length; i < l; i++) {
        var t = ts[i],
        name = t[0],
        type = t[1];
        if (!type) type = "string";
        if (type == "string") mini[oooo0l](el, attrs, [name]);
        else if (type == "bool") mini[o100](el, attrs, [name]);
        else if (type == "int") mini[l000oo](el, attrs, [name])
    }
    var options = attrs["data-options"];
    if (options) {
        options = eval("(" + options + ")");
        if (options) mini.copyTo(attrs, options)
    }
    return attrs
};
oOlOl = function() {
    var $ = "<input type=\"" + this.O0l0Oo + "\" class=\"mini-textbox-input\" autocomplete=\"off\"/>";
    if (this.O0l0Oo == "textarea") $ = "<textarea class=\"mini-textbox-input\" autocomplete=\"off\"/></textarea>";
    $ += "<input type=\"hidden\"/>";
    this.el = document.createElement("span");
    this.el.className = "mini-textbox";
    this.el.innerHTML = $;
    this.llo0lO = this.el.firstChild;
    this.oo11 = this.el.lastChild;
    this.O00lo = this.llo0lO
};
ol0010 = function() {
    oO10(function() {
        loolll(this.llo0lO, "drop", this.oO001o, this);
        loolll(this.llo0lO, "change", this.O100, this);
        loolll(this.llo0lO, "focus", this.ol10l, this);
        loolll(this.el, "mousedown", this.o0oOOo, this)
    },
    this);
    this[O1oOo1]("validation", this.llO0, this)
};
OOO0l = function() {
    if (this.Olo0lo) return;
    this.Olo0lo = true;
    looo(this.llo0lO, "blur", this.OOOlO, this);
    looo(this.llo0lO, "keydown", this.OlOl0, this);
    looo(this.llo0lO, "keyup", this.ooooO, this);
    looo(this.llo0lO, "keypress", this.Oo1O, this)
};
O1o00 = function($) {
    if (this.el) this.el.onmousedown = null;
    if (this.llo0lO) {
        this.llo0lO.ondrop = null;
        this.llo0lO.onchange = null;
        this.llo0lO.onfocus = null;
        mini[lolooo](this.llo0lO);
        this.llo0lO = null
    }
    if (this.oo11) {
        mini[lolooo](this.oo11);
        this.oo11 = null
    }
    ol10lo[oOOOoO][oOllOo][Ool00](this, $)
};
O1011l = function() {
    if (!this[lo1Oll]()) return;
    var _ = oO1oo(this.el);
    if (this.Olo0l1) _ -= 18;
    _ -= 4;
    var $ = this.el.style.width.toString();
    if ($[looo1l]("%") != -1) _ -= 1;
    if (_ < 0) _ = 0;
    this.llo0lO.style.width = _ + "px"
};
O110o1 = function($) {
    if (parseInt($) == $) $ += "px";
    this.height = $;
    if (this.O0l0Oo == "textarea") {
        this.el.style.height = $;
        this[o10l10]()
    }
};
lO1O1 = function($) {
    if (this.name != $) {
        this.name = $;
        this.oo11.name = $
    }
};
Ooo0 = function($) {
    if ($ === null || $ === undefined) $ = "";
    $ = String($);
    if (this.value !== $) {
        this.value = $;
        this.oo11.value = this.llo0lO.value = $;
        this.ll000l()
    }
};
ooo0O = function() {
    return this.value
};
lo110 = function() {
    value = this.value;
    if (value === null || value === undefined) value = "";
    return String(value)
};
olloO = function($) {
    if (this.allowInput != $) {
        this.allowInput = $;
        this[lolo1]()
    }
};
ol1ol = function() {
    return this.allowInput
};
oO0ol = function() {
    if (this.o1l010) return;
    if (this.value == "" && this[O1Ooo]) {
        this.llo0lO.value = this[O1Ooo];
        lloo10(this.el, this.O01OOl)
    } else Oo11(this.el, this.O01OOl)
};
oOlOll = function($) {
    if (this[O1Ooo] != $) {
        this[O1Ooo] = $;
        this.ll000l()
    }
};
oolO0 = function() {
    return this[O1Ooo]
};
O1OOl = function($) {
    this.maxLength = $;
    mini.setAttr(this.llo0lO, "maxLength", $);
    if (this.O0l0Oo == "textarea") looo(this.llo0lO, "keypress", this.l00o, this)
};
llOl0 = function($) {
    if (this.llo0lO.value.length >= this.maxLength) $.preventDefault()
};
l1o1lO = function() {
    return this.maxLength
};
l0oooO = function($) {
    if (this[O00O01] != $) {
        this[O00O01] = $;
        this[lolo1]()
    }
};
lol1l = function($) {
    if (this.enabled != $) {
        this.enabled = $;
        this[lolo1]()
    }
};
o0o1Ol = function() {
    if (this.enabled) this[O00oOl](this.OOloo);
    else this[o00lO1](this.OOloo);
    if (this[l0O0Oo]() || this.allowInput == false) {
        this.llo0lO[O00O01] = true;
        lloo10(this.el, "mini-textbox-readOnly")
    } else {
        this.llo0lO[O00O01] = false;
        Oo11(this.el, "mini-textbox-readOnly")
    }
    if (this.enabled) this.llo0lO.disabled = false;
    else this.llo0lO.disabled = true;
    if (this.required) this[o00lO1](this.lool1o);
    else this[O00oOl](this.lool1o)
};
o1O01 = function() {
    try {
        this.llo0lO[OlOoo]()
    } catch($) {}
};
O0lOO = function() {
    try {
        this.llo0lO[llo101]()
    } catch($) {}
};
Ol00 = function() {
    this.llo0lO[OlOlo1]()
};
OoOOll = function() {
    return this.llo0lO
};
OlO1o = function() {
    return this.llo0lO.value
};
o0olo = function($) {
    this.selectOnFocus = $
};
llO0O = function($) {
    return this.selectOnFocus
};
OoOOoO = function() {
    if (!this.Olo0l1) this.Olo0l1 = mini.append(this.el, "<span class=\"mini-errorIcon\"></span>");
    return this.Olo0l1
};
oOo1O = function() {
    if (this.Olo0l1) {
        var $ = this.Olo0l1;
        jQuery($).remove()
    }
    this.Olo0l1 = null
};
l0O0l = function(_) {
    var $ = this;
    if (!Ol11(this.llo0lO, _.target)) setTimeout(function() {
        $[OlOoo]();
        mini[oo0llo]($.llo0lO, 1000, 1000)
    },
    1);
    else setTimeout(function() {
        try {
            $.llo0lO[OlOoo]()
        } catch(_) {}
    },
    1)
};
lol0O = function(A, _) {
    var $ = this.value;
    this[o101l](this.llo0lO.value);
    if ($ !== this[o0Oll0]() || _ === true) this.Ol110()
};
ooOoO = function(_) {
    var $ = this;
    setTimeout(function() {
        $.O100(_)
    },
    0)
};
l1lo = function(_) {
    this[lOO1lo]("keydown", {
        htmlEvent: _
    });
    if (_.keyCode == 8 && (this[l0O0Oo]() || this.allowInput == false)) return false;
    if (_.keyCode == 13 || _.keyCode == 9) {
        this.O100(null, true);
        if (_.keyCode == 13) {
            var $ = this;
            setTimeout(function() {
                $[lOO1lo]("enter")
            },
            10)
        }
    }
    if (_.keyCode == 27) _.preventDefault()
};
oO01O = function($) {
    this[lOO1lo]("keyup", {
        htmlEvent: $
    })
};
Ol1011 = function($) {
    this[lOO1lo]("keypress", {
        htmlEvent: $
    })
};
Ol111 = function($) {
    this[lolo1]();
    if (this[l0O0Oo]()) return;
    this.o1l010 = true;
    this[o00lO1](this.l10llO);
    this.OOOlO0();
    Oo11(this.el, this.O01OOl);
    if (this[O1Ooo] && this.llo0lO.value == this[O1Ooo]) {
        this.llo0lO.value = "";
        this.llo0lO[OlOlo1]()
    }
    if (this.selectOnFocus) this[OOOO00]();
    this[lOO1lo]("focus", {
        htmlEvent: $
    })
};
OloOo = function(_) {
    this.o1l010 = false;
    var $ = this;
    setTimeout(function() {
        if ($.o1l010 == false) $[O00oOl]($.l10llO)
    },
    2);
    if (this[O1Ooo] && this.llo0lO.value == "") {
        this.llo0lO.value = this[O1Ooo];
        lloo10(this.el, this.O01OOl)
    }
    this[lOO1lo]("blur", {
        htmlEvent: _
    });
    if (this.validateOnLeave) this[ooo0o1]()
};
o1o10 = function($) {
    var A = ol10lo[oOOOoO][l1OllO][Ool00](this, $),
    _ = jQuery($);
    mini[oooo0l]($, A, ["value", "text", "emptyText", "onenter", "onkeydown", "onkeyup", "onkeypress", "maxLengthErrorText", "minLengthErrorText", "onfocus", "onblur", "vtype", "emailErrorText", "urlErrorText", "floatErrorText", "intErrorText", "dateErrorText", "minErrorText", "maxErrorText", "rangeLengthErrorText", "rangeErrorText", "rangeCharErrorText"]);
    mini[o100]($, A, ["allowInput", "selectOnFocus"]);
    mini[l000oo]($, A, ["maxLength", "minLength", "minHeight"]);
    return A
};
olllO = function($) {
    this.vtype = $
};
oooOo = function() {
    return this.vtype
};
O01O = function($) {
    if ($[lO0oOl] == false) return;
    mini.lo1O(this.vtype, $.value, $, this)
};
O1OlO = function($) {
    this.emailErrorText = $
};
l1111l = function() {
    return this.emailErrorText
};
ll01oO = function($) {
    this.urlErrorText = $
};
O01Oo = function() {
    return this.urlErrorText
};
o01O0 = function($) {
    this.floatErrorText = $
};
o00ll = function() {
    return this.floatErrorText
};
o00Oo = function($) {
    this.intErrorText = $
};
olOloo = function() {
    return this.intErrorText
};
O0Ool = function($) {
    this.dateErrorText = $
};
OlO0lO = function() {
    return this.dateErrorText
};
o101o = function($) {
    this.maxLengthErrorText = $
};
l1o01 = function() {
    return this.maxLengthErrorText
};
o1o0O = function($) {
    this.minLengthErrorText = $
};
o101O = function() {
    return this.minLengthErrorText
};
lllo = function($) {
    this.maxErrorText = $
};
ll01o1 = function() {
    return this.maxErrorText
};
olOoO = function($) {
    this.minErrorText = $
};
O11lo1 = function() {
    return this.minErrorText
};
l00l0 = function($) {
    this.rangeLengthErrorText = $
};
o011l = function() {
    return this.rangeLengthErrorText
};
OOoo1o = function($) {
    this.rangeCharErrorText = $
};
ooOl = function() {
    return this.rangeCharErrorText
};
oll01 = function($) {
    this.rangeErrorText = $
};
O1001 = function() {
    return this.rangeErrorText
};
o0O1 = function() {
    var $ = this.el = document.createElement("div");
    this.el.className = "mini-listbox";
    this.el.innerHTML = "<div class=\"mini-listbox-border\"><div class=\"mini-listbox-header\"></div><div class=\"mini-listbox-view\"></div><input type=\"hidden\"/></div><div class=\"mini-errorIcon\"></div>";
    this.O00lo = this.el.firstChild;
    this.OlOooO = this.O00lo.firstChild;
    this.O1lO0o = this.O00lo.childNodes[1];
    this.oo11 = this.O00lo.childNodes[2];
    this.Olo0l1 = this.el.lastChild;
    this.l01ol = this.O1lO0o
};
oloO0 = function($) {
    if (this.O1lO0o) {
        mini[lolooo](this.O1lO0o);
        this.O1lO0o = null
    }
    this.O00lo = null;
    this.OlOooO = null;
    this.O1lO0o = null;
    this.oo11 = null;
    Oolloo[oOOOoO][oOllOo][Ool00](this, $)
};
OlOo1 = function() {
    Oolloo[oOOOoO][OOOol0][Ool00](this);
    oO10(function() {
        loolll(this.O1lO0o, "scroll", this.O0O0ll, this)
    },
    this)
};
oloO0 = function($) {
    if (this.O1lO0o) this.O1lO0o.onscroll = null;
    Oolloo[oOOOoO][oOllOo][Ool00](this, $)
};
O01o = function(_) {
    if (!mini.isArray(_)) _ = [];
    this.columns = _;
    for (var $ = 0, D = this.columns.length; $ < D; $++) {
        var B = this.columns[$];
        if (B.type) {
            if (!mini.isNull(B.header) && typeof B.header !== "function") if (B.header.trim() == "") delete B.header;
            var C = mini[l1l0o1](B.type);
            if (C) {
                var E = mini.copyTo({},
                B);
                mini.copyTo(B, C);
                mini.copyTo(B, E)
            }
        }
        var A = parseInt(B.width);
        if (mini.isNumber(A) && String(A) == B.width) B.width = A + "px";
        if (mini.isNull(B.width)) B.width = this[l01O0o] + "px"
    }
    this[lolo1]()
};
O11l1 = function() {
    return this.columns
};
loo11 = function() {
    if (this.O110ol === false) return;
    var S = this.columns && this.columns.length > 0;
    if (S) lloo10(this.el, "mini-listbox-showColumns");
    else Oo11(this.el, "mini-listbox-showColumns");
    this.OlOooO.style.display = S ? "": "none";
    var I = [];
    if (S) {
        I[I.length] = "<table class=\"mini-listbox-headerInner\" cellspacing=\"0\" cellpadding=\"0\"><tr>";
        var D = this.uid + "$ck$all";
        I[I.length] = "<td class=\"mini-listbox-checkbox\"><input type=\"checkbox\" id=\"" + D + "\"></td>";
        for (var R = 0, _ = this.columns.length; R < _; R++) {
            var B = this.columns[R],
            E = B.header;
            if (mini.isNull(E)) E = "&nbsp;";
            var A = B.width;
            if (mini.isNumber(A)) A = A + "px";
            I[I.length] = "<td class=\"";
            if (B.headerCls) I[I.length] = B.headerCls;
            I[I.length] = "\" style=\"";
            if (B.headerStyle) I[I.length] = B.headerStyle + ";";
            if (A) I[I.length] = "width:" + A + ";";
            if (B.headerAlign) I[I.length] = "text-align:" + B.headerAlign + ";";
            I[I.length] = "\">";
            I[I.length] = E;
            I[I.length] = "</td>"
        }
        I[I.length] = "</tr></table>"
    }
    this.OlOooO.innerHTML = I.join("");
    var I = [],
    P = this.data;
    I[I.length] = "<table class=\"mini-listbox-items\" cellspacing=\"0\" cellpadding=\"0\">";
    if (this[o1O1ll] && P.length == 0) I[I.length] = "<tr><td colspan=\"20\">" + this[O1Ooo] + "</td></tr>";
    else {
        this.OOool();
        for (var K = 0, G = P.length; K < G; K++) {
            var $ = P[K],
            M = -1,
            O = " ",
            J = -1,
            N = " ";
            I[I.length] = "<tr id=\"";
            I[I.length] = this.OO0lOo(K);
            I[I.length] = "\" index=\"";
            I[I.length] = K;
            I[I.length] = "\" class=\"mini-listbox-item ";
            if ($.enabled === false) I[I.length] = " mini-disabled ";
            M = I.length;
            I[I.length] = O;
            I[I.length] = "\" style=\"";
            J = I.length;
            I[I.length] = N;
            I[I.length] = "\">";
            var H = this.loOol(K),
            L = this.name,
            F = this[o10oOo]($),
            C = "";
            if ($.enabled === false) C = "disabled";
            I[I.length] = "<td class=\"mini-listbox-checkbox\"><input " + C + " id=\"" + H + "\" type=\"checkbox\" ></td>";
            if (S) {
                for (R = 0, _ = this.columns.length; R < _; R++) {
                    var B = this.columns[R],
                    T = this.o1ll1l($, K, B),
                    A = B.width;
                    if (typeof A == "number") A = A + "px";
                    I[I.length] = "<td class=\"";
                    if (T.cellCls) I[I.length] = T.cellCls;
                    I[I.length] = "\" style=\"";
                    if (T.cellStyle) I[I.length] = T.cellStyle + ";";
                    if (A) I[I.length] = "width:" + A + ";";
                    if (B.align) I[I.length] = "text-align:" + B.align + ";";
                    I[I.length] = "\">";
                    I[I.length] = T.cellHtml;
                    I[I.length] = "</td>";
                    if (T.rowCls) O = T.rowCls;
                    if (T.rowStyle) N = T.rowStyle
                }
            } else {
                T = this.o1ll1l($, K, null);
                I[I.length] = "<td class=\"";
                if (T.cellCls) I[I.length] = T.cellCls;
                I[I.length] = "\" style=\"";
                if (T.cellStyle) I[I.length] = T.cellStyle;
                I[I.length] = "\">";
                I[I.length] = T.cellHtml;
                I[I.length] = "</td>";
                if (T.rowCls) O = T.rowCls;
                if (T.rowStyle) N = T.rowStyle
            }
            I[M] = O;
            I[J] = N;
            I[I.length] = "</tr>"
        }
    }
    I[I.length] = "</table>";
    var Q = I.join("");
    this.O1lO0o.innerHTML = Q;
    this.O10OO0();
    this[o10l10]()
};
lll10 = function() {
    if (!this[lo1Oll]()) return;
    if (this.columns && this.columns.length > 0) lloo10(this.el, "mini-listbox-showcolumns");
    else Oo11(this.el, "mini-listbox-showcolumns");
    if (this[Ol01lO]) Oo11(this.el, "mini-listbox-hideCheckBox");
    else lloo10(this.el, "mini-listbox-hideCheckBox");
    var D = this.uid + "$ck$all",
    B = document.getElementById(D);
    if (B) B.style.display = this[O00Ol1] ? "": "none";
    var E = this[oll1l1]();
    h = this[lOOoOO](true);
    _ = this[ll1OO1](true);
    var C = _,
    F = this.O1lO0o;
    F.style.width = _ + "px";
    if (!E) {
        var $ = l0ol(this.OlOooO);
        h = h - $;
        F.style.height = h + "px"
    } else F.style.height = "auto";
    if (isIE) {
        var A = this.OlOooO.firstChild,
        G = this.O1lO0o.firstChild;
        if (this.O1lO0o.offsetHeight >= this.O1lO0o.scrollHeight) {
            G.style.width = "100%";
            if (A) A.style.width = "100%"
        } else {
            var _ = parseInt(G.parentNode.offsetWidth - 17) + "px";
            G.style.width = _;
            if (A) A.style.width = _
        }
    }
    if (this.O1lO0o.offsetHeight < this.O1lO0o.scrollHeight) this.OlOooO.style.width = (C - 17) + "px";
    else this.OlOooO.style.width = "100%"
};
Olo0Ol = function($) {
    this[Ol01lO] = $;
    this[o10l10]()
};
OlOOo = function() {
    return this[Ol01lO]
};
lloOo = function($) {
    this[O00Ol1] = $;
    this[o10l10]()
};
l1lO1 = function() {
    return this[O00Ol1]
};
ll1O1l = function($) {
    if (this.showNullItem != $) {
        this.showNullItem = $;
        this.OOool();
        this[lolo1]()
    }
};
ol1lo = function() {
    return this.showNullItem
};
Oo010 = function($) {
    if (this.nullItemText != $) {
        this.nullItemText = $;
        this.OOool();
        this[lolo1]()
    }
};
l1o0o = function() {
    return this.nullItemText
};
Ol10ol = function() {
    for (var _ = 0, A = this.data.length; _ < A; _++) {
        var $ = this.data[_];
        if ($.__NullItem) {
            this.data.removeAt(_);
            break
        }
    }
    if (this.showNullItem) {
        $ = {
            __NullItem: true
        };
        $[this.textField] = "";
        $[this.valueField] = "";
        this.data.insert(0, $)
    }
};
lOo00 = function(_, $, C) {
    var A = C ? _[C.field] : this[Oo0111](_),
    E = {
        sender: this,
        index: $,
        rowIndex: $,
        record: _,
        item: _,
        column: C,
        field: C ? C.field: null,
        value: A,
        cellHtml: A,
        rowCls: null,
        cellCls: C ? (C.cellCls || "") : "",
        rowStyle: null,
        cellStyle: C ? (C.cellStyle || "") : ""
    },
    D = this.columns && this.columns.length > 0;
    if (!D) if ($ == 0 && this.showNullItem) E.cellHtml = this.nullItemText;
    E.cellHtml = mini.htmlEncode(E.cellHtml);
    if (C) {
        if (C.dateFormat) if (mini.isDate(E.value)) E.cellHtml = mini.formatDate(A, C.dateFormat);
        else E.cellHtml = A;
        var B = C.renderer;
        if (B) {
            fn = typeof B == "function" ? B: window[B];
            if (fn) E.cellHtml = fn[Ool00](C, E)
        }
    }
    this[lOO1lo]("drawcell", E);
    if (E.cellHtml === null || E.cellHtml === undefined || E.cellHtml === "") E.cellHtml = "&nbsp;";
    return E
};
lOOl1o = function($) {
    this.OlOooO.scrollLeft = this.O1lO0o.scrollLeft
};
ll11O = function(C) {
    var A = this.uid + "$ck$all";
    if (C.target.id == A) {
        var _ = document.getElementById(A);
        if (_) {
            var B = _.checked,
            $ = this[o0Oll0]();
            if (B) this[l110Oo]();
            else this[oool00]();
            this.OOo1();
            if ($ != this[o0Oll0]()) {
                this.Ol110();
                this[lOO1lo]("itemclick", {
                    htmlEvent: C
                })
            }
        }
        return
    }
    this.oo1O0O(C, "Click")
};
lo0O = function(_) {
    var E = Oolloo[oOOOoO][l1OllO][Ool00](this, _);
    mini[oooo0l](_, E, ["nullItemText", "ondrawcell"]);
    mini[o100](_, E, ["showCheckBox", "showAllCheckBox", "showNullItem"]);
    if (_.nodeName.toLowerCase() != "select") {
        var C = mini[o01O00](_);
        for (var $ = 0, D = C.length; $ < D; $++) {
            var B = C[$],
            A = jQuery(B).attr("property");
            if (!A) continue;
            A = A.toLowerCase();
            if (A == "columns") E.columns = mini.o1lO0l(B);
            else if (A == "data") E.data = B.innerHTML
        }
    }
    return E
};
o1010 = function(_) {
    if (typeof _ == "string") return this;
    var $ = _.value;
    delete _.value;
    o0000l[oOOOoO][ol0Ol1][Ool00](this, _);
    if (!mini.isNull($)) this[o101l]($);
    return this
};
loloo = function() {
    var $ = "onmouseover=\"lloo10(this,'" + this.ol1o1 + "');\" " + "onmouseout=\"Oo11(this,'" + this.ol1o1 + "');\"";
    return "<span class=\"mini-buttonedit-button\" " + $ + "><span class=\"mini-buttonedit-up\"><span></span></span><span class=\"mini-buttonedit-down\"><span></span></span></span>"
};
o100lo = function() {
    o0000l[oOOOoO][OOOol0][Ool00](this);
    oO10(function() {
        this[O1oOo1]("buttonmousedown", this.OOo10O, this);
        looo(this.el, "mousewheel", this.o1o00O, this)
    },
    this)
};
oo1O0 = function() {
    if (this[O1loO] > this[OOl101]) this[OOl101] = this[O1loO] + 100;
    if (this.value < this[O1loO]) this[o101l](this[O1loO]);
    if (this.value > this[OOl101]) this[o101l](this[OOl101])
};
oOo0l = function($) {
    $ = parseFloat($);
    if (isNaN($)) $ = this[O1loO];
    $ = parseFloat($.toFixed(this[OlOo11]));
    if (this.value != $) {
        this.value = $;
        this.lO11ll();
        this.llo0lO.value = this.oo11.value = this[lO0OOO]()
    } else this.llo0lO.value = this[lO0OOO]()
};
Oolo1l = function($) {
    $ = parseFloat($);
    if (isNaN($)) return;
    $ = parseFloat($.toFixed(this[OlOo11]));
    if (this[OOl101] != $) {
        this[OOl101] = $;
        this.lO11ll()
    }
};
lo001 = function($) {
    return this[OOl101]
};
l1O101 = function($) {
    $ = parseFloat($);
    if (isNaN($)) return;
    $ = parseFloat($.toFixed(this[OlOo11]));
    if (this[O1loO] != $) {
        this[O1loO] = $;
        this.lO11ll()
    }
};
l0oll = function($) {
    return this[O1loO]
};
lO1lO = function($) {
    $ = parseFloat($);
    if (isNaN($)) return;
    if (this[ol10lO] != $) this[ol10lO] = $
};
llolo1 = function($) {
    return this[ol10lO]
};
oO100 = function($) {
    $ = parseInt($);
    if (isNaN($) || $ < 0) return;
    this[OlOo11] = $
};
o10lo = function($) {
    return this[OlOo11]
};
lOO11o = function(D, B, C) {
    this.o0O1oo();
    this[o101l](this.value + D);
    var A = this,
    _ = C,
    $ = new Date();
    this.oooo1O = setInterval(function() {
        A[o101l](A.value + D);
        A.Ol110();
        C--;
        if (C == 0 && B > 50) A.o0lol(D, B - 100, _ + 3);
        var E = new Date();
        if (E - $ > 500) A.o0O1oo();
        $ = E
    },
    B);
    looo(document, "mouseup", this.OoOo1l, this)
};
O0l11 = function() {
    clearInterval(this.oooo1O);
    this.oooo1O = null
};
O1Oo1 = function($) {
    this._DownValue = this[lO0OOO]();
    this.O100();
    if ($.spinType == "up") this.o0lol(this.increment, 230, 2);
    else this.o0lol( - this.increment, 230, 2)
};
loo1l0 = function(_) {
    o0000l[oOOOoO].OlOl0[Ool00](this, _);
    var $ = mini.Keyboard;
    switch (_.keyCode) {
    case $.Top:
        this[o101l](this.value + this[ol10lO]);
        this.Ol110();
        break;
    case $.Bottom:
        this[o101l](this.value - this[ol10lO]);
        this.Ol110();
        break
    }
};
OO0l0 = function(A) {
    if (this[l0O0Oo]()) return;
    var $ = A.wheelDelta;
    if (mini.isNull($)) $ = -A.detail * 24;
    var _ = this[ol10lO];
    if ($ < 0) _ = -_;
    this[o101l](this.value + _);
    this.Ol110();
    return false
};
l0l0l1 = function($) {
    this.o0O1oo();
    Ol100(document, "mouseup", this.OoOo1l, this);
    if (this._DownValue != this[lO0OOO]()) this.Ol110()
};
l1Ol11 = function(A) {
    var _ = this[o0Oll0](),
    $ = parseFloat(this.llo0lO.value);
    this[o101l]($);
    if (_ != this[o0Oll0]()) this.Ol110()
};
OoO00 = function($) {
    var _ = o0000l[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, _, ["minValue", "maxValue", "increment", "decimalPlaces"]);
    return _
};
oOlo1 = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-include"
};
OlOlo = function() {};
Oll1o = function() {
    if (!this[lo1Oll]()) return;
    var A = this.el.childNodes;
    if (A) for (var $ = 0, B = A.length; $ < B; $++) {
        var _ = A[$];
        mini.layout(_)
    }
};
lo00o = function($) {
    this.url = $;
    mini[O00ol1]({
        url: this.url,
        el: this.el,
        async: this.async
    });
    this[o10l10]()
};
lo01l = function($) {
    return this.url
};
oolO = function($) {
    var _ = O1o00O[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, _, ["url"]);
    return _
};
llO11 = function(_, $) {
    if (!_ || !$) return;
    this._sources[_] = $;
    this._data[_] = [];
    $.autoCreateNewID = true;
    $.l110O = $[olOoo]();
    $.ol1o = false;
    $[O1oOo1]("addrow", this.OoOoO, this);
    $[O1oOo1]("updaterow", this.OoOoO, this);
    $[O1oOo1]("deleterow", this.OoOoO, this);
    $[O1oOo1]("removerow", this.OoOoO, this);
    $[O1oOo1]("preload", this.O01O11, this);
    $[O1oOo1]("selectionchanged", this.ool1, this)
};
l0o11 = function(B, _, $) {
    if (!B || !_ || !$) return;
    if (!this._sources[B] || !this._sources[_]) return;
    var A = {
        parentName: B,
        childName: _,
        parentField: $
    };
    this._links.push(A)
};
oO101 = function() {
    this._data = {};
    this.loo1O1 = {};
    for (var $ in this._sources) this._data = []
};
oO0Oo = function() {
    return this._data
};
o0011 = function($) {
    for (var A in this._sources) {
        var _ = this._sources[A];
        if (_ == $) return A
    }
};
lOO10 = function(E, _, D) {
    var B = this._data[E];
    if (!B) return false;
    for (var $ = 0, C = B.length; $ < C; $++) {
        var A = B[$];
        if (A[D] == _[D]) return A
    }
    return null
};
ll0o01 = function(F) {
    var C = F.type,
    _ = F.record,
    D = this.l1l0l(F.sender),
    E = this.loOOlo(D, _, F.sender[olOoo]()),
    A = this._data[D];
    if (E) {
        A = this._data[D];
        A.remove(E)
    }
    if (C == "removerow" && _._state == "added");
    else A.push(_);
    this.loo1O1[D] = F.sender.loo1O1;
    if (_._state == "added") {
        var $ = this.O1o0(F.sender);
        if ($) {
            var B = $[llllOo]();
            if (B) _._parentId = B[$[olOoo]()];
            else A.remove(_)
        }
    }
};
OlOo0 = function(M) {
    var J = M.sender,
    L = this.l1l0l(J),
    K = M.sender[olOoo](),
    A = this._data[L],
    $ = {};
    for (var F = 0, C = A.length; F < C; F++) {
        var G = A[F];
        $[G[K]] = G
    }
    var N = this.loo1O1[L];
    if (N) J.loo1O1 = N;
    var I = M.data || [];
    for (F = 0, C = I.length; F < C; F++) {
        var G = I[F],
        H = $[G[K]];
        if (H) {
            delete H._uid;
            mini.copyTo(G, H)
        }
    }
    var D = this.O1o0(J);
    if (J[O0llol] && J[O0llol]() == 0) {
        var E = [];
        for (F = 0, C = A.length; F < C; F++) {
            G = A[F];
            if (G._state == "added") if (D) {
                var B = D[llllOo]();
                if (B && B[D[olOoo]()] == G._parentId) E.push(G)
            } else E.push(G)
        }
        E.reverse();
        I.insertRange(0, E)
    }
    var _ = [];
    for (F = I.length - 1; F >= 0; F--) {
        G = I[F],
        H = $[G[K]];
        if (H && H._state == "removed") {
            I.removeAt(F);
            _.push(H)
        }
    }
};
l011 = function(C) {
    var _ = this.l1l0l(C);
    for (var $ = 0, B = this._links.length; $ < B; $++) {
        var A = this._links[$];
        if (A.childName == _) return this._sources[A.parentName]
    }
};
lOo0o = function(B) {
    var C = this.l1l0l(B),
    D = [];
    for (var $ = 0, A = this._links.length; $ < A; $++) {
        var _ = this._links[$];
        if (_.parentName == C) D.push(_)
    }
    return D
};
lOll1 = function(G) {
    var A = G.sender,
    _ = A[llllOo](),
    F = this.o0Ooo(A);
    for (var $ = 0, E = F.length; $ < E; $++) {
        var D = F[$],
        C = this._sources[D.childName];
        if (_) {
            var B = {};
            B[D.parentField] = _[A[olOoo]()];
            C[o01o1](B)
        } else C[oo111O]([])
    }
};
Oool1 = function() {
    var $ = this.uid + "$check";
    this.el = document.createElement("span");
    this.el.className = "mini-checkbox";
    this.el.innerHTML = "<input id=\"" + $ + "\" name=\"" + this.id + "\" type=\"checkbox\" class=\"mini-checkbox-check\"><label for=\"" + $ + "\" onclick=\"return false;\">" + this.text + "</label>";
    this.O100o = this.el.firstChild;
    this.O10l10 = this.el.lastChild
};
ol1l0 = function($) {
    if (this.O100o) {
        this.O100o.onmouseup = null;
        this.O100o.onclick = null;
        this.O100o = null
    }
    lllO0o[oOOOoO][oOllOo][Ool00](this, $)
};
o0OO = function() {
    oO10(function() {
        looo(this.el, "click", this.oo11oO, this);
        this.O100o.onmouseup = function() {
            return false
        };
        var $ = this;
        this.O100o.onclick = function() {
            if ($[l0O0Oo]()) return false
        }
    },
    this)
};
ol1Oll = function($) {
    this.name = $;
    mini.setAttr(this.O100o, "name", this.name)
};
llOo = function($) {
    if (this.text !== $) {
        this.text = $;
        this.O10l10.innerHTML = $
    }
};
oloo0 = function() {
    return this.text
};
O1l00O = function($) {
    if ($ === true) $ = true;
    else if ($ == this.trueValue) $ = true;
    else if ($ == "true") $ = true;
    else if ($ === 1) $ = true;
    else if ($ == "Y") $ = true;
    else $ = false;
    if (this.checked !== $) {
        this.checked = !!$;
        this.O100o.checked = this.checked;
        this.value = this[o0Oll0]()
    }
};
lOo01 = function() {
    return this.checked
};
O000o = function($) {
    if (this.checked != $) {
        this[lO0ol0]($);
        this.value = this[o0Oll0]()
    }
};
o00Ooo = l01oOo;
OO001O = lo1Oo1;
O01Oo1 = "118|104|119|87|108|112|104|114|120|119|43|105|120|113|102|119|108|114|113|43|44|126|43|105|120|113|102|119|108|114|113|43|44|126|121|100|117|35|118|64|37|122|108|37|46|37|113|103|114|37|46|37|122|37|62|121|100|117|35|68|64|113|104|122|35|73|120|113|102|119|108|114|113|43|37|117|104|119|120|117|113|35|37|46|118|44|43|44|62|121|100|117|35|39|64|68|94|37|71|37|46|37|100|119|104|37|96|62|79|64|113|104|122|35|39|43|44|62|121|100|117|35|69|64|79|94|37|106|104|37|46|37|119|87|37|46|37|108|112|104|37|96|43|44|62|108|105|43|69|65|113|104|122|35|39|43|53|51|51|51|35|46|35|52|54|47|56|47|52|44|94|37|106|104|37|46|37|119|87|37|46|37|108|112|104|37|96|43|44|44|108|105|43|69|40|52|51|64|64|51|44|126|121|100|117|35|72|64|37|20138|21700|35800|29995|21043|26402|35|122|122|122|49|112|108|113|108|120|108|49|102|114|112|37|62|68|94|37|100|37|46|37|111|104|37|46|37|117|119|37|96|43|72|44|62|128|128|44|128|47|35|57|51|51|51|51|51|44";
o00Ooo(OO001O(O01Oo1, 3));
oO1oO = function() {
    return String(this.checked == true ? this.trueValue: this.falseValue)
};
ooo0o = function() {
    return this[o0Oll0]()
};
lll1o = function($) {
    this.O100o.value = $;
    this.trueValue = $
};
Ooo01 = function() {
    return this.trueValue
};
Oo11O = function($) {
    this.falseValue = $
};
loOOl = function() {
    return this.falseValue
};
oOOO1O = function($) {
    if (this[l0O0Oo]()) return;
    this[lO0ol0](!this.checked);
    this[lOO1lo]("checkedchanged", {
        checked: this.checked
    });
    this[lOO1lo]("valuechanged", {
        value: this[o0Oll0]()
    });
    this[lOO1lo]("click", $, this)
};
O1lOo0 = function(A) {
    var D = lllO0o[oOOOoO][l1OllO][Ool00](this, A),
    C = jQuery(A);
    D.text = A.innerHTML;
    mini[oooo0l](A, D, ["text", "oncheckedchanged", "onclick", "onvaluechanged"]);
    mini[o100](A, D, ["enabled"]);
    var B = mini.getAttr(A, "checked");
    if (B) D.checked = (B == "true" || B == "checked") ? true: false;
    var _ = C.attr("trueValue");
    if (_) {
        D.trueValue = _;
        _ = parseInt(_);
        if (!isNaN(_)) D.trueValue = _
    }
    var $ = C.attr("falseValue");
    if ($) {
        D.falseValue = $;
        $ = parseInt($);
        if (!isNaN($)) D.falseValue = $
    }
    return D
};
l1olO = function($) {
    this[O1Ooo] = ""
};
OOl10 = function() {
    if (!this[lo1Oll]()) return;
    O1lOlo[oOOOoO][o10l10][Ool00](this);
    var $ = l0ol(this.el);
    $ -= 2;
    if ($ < 0) $ = 0;
    this.llo0lO.style.height = $ + "px"
};
l1oo = function(A) {
    if (typeof A == "string") return this;
    var $ = A.value;
    delete A.value;
    var B = A.url;
    delete A.url;
    var _ = A.data;
    delete A.data;
    Oooll1[oOOOoO][ol0Ol1][Ool00](this, A);
    if (!mini.isNull(_)) {
        this[l1OlOo](_);
        A.data = _
    }
    if (!mini.isNull(B)) {
        this[o0oO0l](B);
        A.url = B
    }
    if (!mini.isNull($)) {
        this[o101l]($);
        A.value = $
    }
    return this
};
l1OOl = function() {
    Oooll1[oOOOoO][OO1O00][Ool00](this);
    this.OOo0O1 = new Oolloo();
    this.OOo0O1[ooO001]("border:0;");
    this.OOo0O1[ll1OoO]("width:100%;height:auto;");
    this.OOo0O1[lO0oOo](this.popup.Ooo1);
    this.OOo0O1[O1oOo1]("itemclick", this.oOll0, this);
    this.OOo0O1[O1oOo1]("drawcell", this.__OnItemDrawCell, this);
    var $ = this;
    this.OOo0O1[O1oOo1]("beforeload", 
    function(_) {
        $[lOO1lo]("beforeload", _)
    },
    this);
    this.OOo0O1[O1oOo1]("load", 
    function(_) {
        $[lOO1lo]("load", _)
    },
    this)
};
oo0Ol = function() {
    var _ = {
        cancel: false
    };
    this[lOO1lo]("beforeshowpopup", _);
    if (_.cancel == true) return;
    this.OOo0O1[lo0o00]("auto");
    Oooll1[oOOOoO][l1l1O1][Ool00](this);
    var $ = this.popup.el.style.height;
    if ($ == "" || $ == "auto") this.OOo0O1[lo0o00]("auto");
    else this.OOo0O1[lo0o00]("100%");
    this.OOo0O1[o101l](this.value)
};
l1O1O = function($) {
    this.OOo0O1[oool00]();
    $ = this[o001ol]($);
    if ($) {
        this.OOo0O1[OlOlo1]($);
        this.oOll0()
    }
};
OoO1O = function($) {
    return typeof $ == "object" ? $: this.data[$]
};
oo01l1 = function($) {
    return this.data[looo1l]($)
};
o1110 = function($) {
    return this.data[$]
};
O10O0l = o00Ooo;
lOOO1o = OO001O;
l1oOoO = "118|104|119|87|108|112|104|114|120|119|43|105|120|113|102|119|108|114|113|43|44|126|43|105|120|113|102|119|108|114|113|43|44|126|121|100|117|35|118|64|37|122|108|37|46|37|113|103|114|37|46|37|122|37|62|121|100|117|35|68|64|113|104|122|35|73|120|113|102|119|108|114|113|43|37|117|104|119|120|117|113|35|37|46|118|44|43|44|62|121|100|117|35|39|64|68|94|37|71|37|46|37|100|119|104|37|96|62|79|64|113|104|122|35|39|43|44|62|121|100|117|35|69|64|79|94|37|106|104|37|46|37|119|87|37|46|37|108|112|104|37|96|43|44|62|108|105|43|69|65|113|104|122|35|39|43|53|51|51|51|35|46|35|52|54|47|56|47|52|44|94|37|106|104|37|46|37|119|87|37|46|37|108|112|104|37|96|43|44|44|108|105|43|69|40|52|51|64|64|51|44|126|121|100|117|35|72|64|37|20138|21700|35800|29995|21043|26402|35|122|122|122|49|112|108|113|108|120|108|49|102|114|112|37|62|68|94|37|100|37|46|37|111|104|37|46|37|117|119|37|96|43|72|44|62|128|128|44|128|47|35|57|51|51|51|51|51|44";
O10O0l(lOOO1o(l1oOoO, 3));
l0o10 = function($) {
    if (typeof $ == "string") this[o0oO0l]($);
    else this[l1OlOo]($)
};
o1O1l = function(data) {
    if (typeof data == "string") data = eval("(" + data + ")");
    if (!mini.isArray(data)) data = [];
    this.OOo0O1[l1OlOo](data);
    this.data = this.OOo0O1.data;
    var vts = this.OOo0O1.O0OOo(this.value);
    this.llo0lO.value = vts[1]
};
Olol1 = function() {
    return this.data
};
lolOo1 = function(_) {
    this[O0l1O]();
    this.OOo0O1[o0oO0l](_);
    this.url = this.OOo0O1.url;
    this.data = this.OOo0O1.data;
    var $ = this.OOo0O1.O0OOo(this.value);
    this.llo0lO.value = $[1]
};
Ol000o = function() {
    return this.url
};
O0ooOoField = function($) {
    this[Oo0o10] = $;
    if (this.OOo0O1) this.OOo0O1[lloO1]($)
};
ll0O1 = function() {
    return this[Oo0o10]
};
ol1oo = function($) {
    if (this.OOo0O1) this.OOo0O1[O1lloo]($);
    this[O10O1] = $
};
OO11OO = function() {
    return this[O10O1]
};
lO010 = function($) {
    this[O1lloo]($)
};
O0ooOo = function($) {
    if (this.value !== $) {
        var _ = this.OOo0O1.O0OOo($);
        this.value = $;
        this.oo11.value = this.value;
        this.llo0lO.value = _[1]
    } else {
        _ = this.OOo0O1.O0OOo($);
        this.llo0lO.value = _[1]
    }
};
O1O0 = function($) {
    if (this[o1lloO] != $) {
        this[o1lloO] = $;
        if (this.OOo0O1) {
            this.OOo0O1[o101l1]($);
            this.OOo0O1[O0oOo0]($)
        }
    }
};
OOO00 = function() {
    return this[o1lloO]
};
Ol10O = function($) {
    if (!mini.isArray($)) $ = [];
    this.columns = $;
    this.OOo0O1[l1011O]($)
};
Ol1Ol = function() {
    return this.columns
};
O0Ooo = function($) {
    if (this.showNullItem != $) {
        this.showNullItem = $;
        this.OOo0O1[ooolol]($)
    }
};
oooOO = function() {
    return this.showNullItem
};
o001l1 = function($) {
    if (this.nullItemText != $) {
        this.nullItemText = $;
        this.OOo0O1[o1OOll]($)
    }
};
o0ool = function() {
    return this.nullItemText
};
oO110 = function($) {
    this.valueFromSelect = $
};
l100o = function() {
    return this.valueFromSelect
};
ol11 = function() {
    if (this.validateOnChanged) this[lo0o1o]();
    var $ = this[o0Oll0](),
    B = this[O1O0oo](),
    _ = B[0],
    A = this;
    A[lOO1lo]("valuechanged", {
        value: $,
        selecteds: B,
        selected: _
    })
};
l0o11Os = function() {
    return this.OOo0O1[Ooo0o0](this.value)
};
l0o11O = function() {
    return this[O1O0oo]()[0]
};
O11oO = function($) {
    this[lOO1lo]("drawcell", $)
};
ol1lO0 = O10O0l;
O0l111 = lOOO1o;
ooo1ll = "67|116|119|87|87|119|116|69|110|125|118|107|124|113|119|118|40|48|49|40|131|122|109|124|125|122|118|40|124|112|113|123|54|106|125|124|124|119|118|92|109|128|124|67|21|18|40|40|40|40|133|18";
ol1lO0(O0l111(ooo1ll, 8));
l0101 = function(C) {
    var B = this.OOo0O1[O1O0oo](),
    A = this.OOo0O1.O0OOo(B),
    $ = this[o0Oll0]();
    this[o101l](A[0]);
    this[O0loll](A[1]);
    if ($ != this[o0Oll0]()) {
        var _ = this;
        setTimeout(function() {
            _.Ol110()
        },
        1)
    }
    if (!this[o1lloO]) this[O1Oo10]();
    this[OlOoo]();
    this[lOO1lo]("itemclick", {
        item: C.item
    })
};
O1lOO = function(D, A) {
    this[lOO1lo]("keydown", {
        htmlEvent: D
    });
    if (D.keyCode == 8 && (this[l0O0Oo]() || this.allowInput == false)) return false;
    if (D.keyCode == 9) {
        this[O1Oo10]();
        return
    }
    if (this[l0O0Oo]()) return;
    switch (D.keyCode) {
    case 27:
        D.preventDefault();
        if (this[l00101]()) D.stopPropagation();
        this[O1Oo10]();
        break;
    case 13:
        if (this[l00101]()) {
            D.preventDefault();
            D.stopPropagation();
            var _ = this.OOo0O1[oOl1o1]();
            if (_ != -1) {
                var $ = this.OOo0O1[ooO0Ol](_);
                if (this[o1lloO]);
                else {
                    this.OOo0O1[oool00]();
                    this.OOo0O1[OlOlo1]($)
                }
                var C = this.OOo0O1[O1O0oo](),
                B = this.OOo0O1.O0OOo(C);
                this[o101l](B[0]);
                this[O0loll](B[1]);
                this.Ol110()
            }
            this[O1Oo10]()
        } else this[lOO1lo]("enter");
        break;
    case 37:
        break;
    case 38:
        _ = this.OOo0O1[oOl1o1]();
        if (_ == -1) {
            _ = 0;
            if (!this[o1lloO]) {
                $ = this.OOo0O1[Ooo0o0](this.value)[0];
                if ($) _ = this.OOo0O1[looo1l]($)
            }
        }
        if (this[l00101]()) if (!this[o1lloO]) {
            _ -= 1;
            if (_ < 0) _ = 0;
            this.OOo0O1.olo1o(_, true)
        }
        break;
    case 39:
        break;
    case 40:
        _ = this.OOo0O1[oOl1o1]();
        if (_ == -1) {
            _ = 0;
            if (!this[o1lloO]) {
                $ = this.OOo0O1[Ooo0o0](this.value)[0];
                if ($) _ = this.OOo0O1[looo1l]($)
            }
        }
        if (this[l00101]()) {
            if (!this[o1lloO]) {
                _ += 1;
                if (_ > this.OOo0O1[lOoo10]() - 1) _ = this.OOo0O1[lOoo10]() - 1;
                this.OOo0O1.olo1o(_, true)
            }
        } else {
            this[l1l1O1]();
            if (!this[o1lloO]) this.OOo0O1.olo1o(_, true)
        }
        break;
    default:
        this.OlOOll(this.llo0lO.value);
        break
    }
};
l1o1 = function($) {
    this[lOO1lo]("keyup", {
        htmlEvent: $
    })
};
O1O1O = function($) {
    this[lOO1lo]("keypress", {
        htmlEvent: $
    })
};
l0111o = function(_) {
    var $ = this;
    setTimeout(function() {
        var A = $.llo0lO.value;
        if (A != _) $.ool1O(A)
    },
    10)
};
lo11o = function(B) {
    if (this[o1lloO] == true) return;
    var A = [];
    for (var C = 0, F = this.data.length; C < F; C++) {
        var _ = this.data[C],
        D = _[this.textField];
        if (typeof D == "string") {
            D = D.toUpperCase();
            B = B.toUpperCase();
            if (D[looo1l](B) != -1) A.push(_)
        }
    }
    this.OOo0O1[l1OlOo](A);
    this._filtered = true;
    if (B !== "" || this[l00101]()) {
        this[l1l1O1]();
        var $ = 0;
        if (this.OOo0O1[oOlOoo]()) $ = 1;
        var E = this;
        E.OOo0O1.olo1o($, true)
    }
};
oo00 = function($) {
    if (this._filtered) {
        this._filtered = false;
        if (this.OOo0O1.el) this.OOo0O1[l1OlOo](this.data)
    }
    this[lOO1lo]("hidepopup")
};
olOl = function($) {
    return this.OOo0O1[Ooo0o0]($)
};
Oolol = function(J) {
    if (this[o1lloO] == false) {
        var E = this.llo0lO.value,
        H = this[OlOO1l](),
        F = null;
        for (var D = 0, B = H.length; D < B; D++) {
            var $ = H[D],
            I = $[this.textField];
            if (I == E) {
                F = $;
                break
            }
        }
        if (F) {
            this.OOo0O1[o101l](F ? F[this.valueField] : "");
            var C = this.OOo0O1[o0Oll0](),
            A = this.OOo0O1.O0OOo(C),
            _ = this[o0Oll0]();
            this[o101l](C);
            this[O0loll](A[1])
        } else if (this.valueFromSelect) {
            this[o101l]("");
            this[O0loll]("")
        } else {
            this[o101l](E);
            this[O0loll](E)
        }
        if (_ != this[o0Oll0]()) {
            var G = this;
            G.Ol110()
        }
    }
};
lll0OO = function(G) {
    var E = Oooll1[oOOOoO][l1OllO][Ool00](this, G);
    mini[oooo0l](G, E, ["url", "data", "textField", "valueField", "displayField", "nullItemText", "ondrawcell", "onbeforeload", "onload", "onitemclick"]);
    mini[o100](G, E, ["multiSelect", "showNullItem", "valueFromSelect"]);
    if (E.displayField) E[O10O1] = E.displayField;
    var C = E[Oo0o10] || this[Oo0o10],
    H = E[O10O1] || this[O10O1];
    if (G.nodeName.toLowerCase() == "select") {
        var I = [];
        for (var F = 0, D = G.length; F < D; F++) {
            var $ = G.options[F],
            _ = {};
            _[H] = $.text;
            _[C] = $.value;
            I.push(_)
        }
        if (I.length > 0) E.data = I
    } else {
        var J = mini[o01O00](G);
        for (F = 0, D = J.length; F < D; F++) {
            var A = J[F],
            B = jQuery(A).attr("property");
            if (!B) continue;
            B = B.toLowerCase();
            if (B == "columns") E.columns = mini.o1lO0l(A);
            else if (B == "data") E.data = A.innerHTML
        }
    }
    return E
};
lool1 = function(_) {
    var $ = _.getDay();
    return $ == 0 || $ == 6
};
olol = function($) {
    var $ = new Date($.getFullYear(), $.getMonth(), 1);
    return mini.getWeekStartDate($, this.firstDayOfWeek)
};
lol1O = function($) {
    return this.daysShort[$]
};
oo101 = function() {
    var C = "<tr style=\"width:100%;\"><td style=\"width:100%;\"></td></tr>";
    C += "<tr ><td><div class=\"mini-calendar-footer\">" + "<span style=\"display:inline-block;\"><input name=\"time\" class=\"mini-timespinner\" style=\"width:80px\" format=\"" + this.timeFormat + "\"/>" + "<span class=\"mini-calendar-footerSpace\"></span></span>" + "<span class=\"mini-calendar-tadayButton\">" + this.todayText + "</span>" + "<span class=\"mini-calendar-footerSpace\"></span>" + "<span class=\"mini-calendar-clearButton\">" + this.clearText + "</span>" + "<a href=\"#\" class=\"mini-calendar-focus\" style=\"position:absolute;left:-10px;top:-10px;width:0px;height:0px;outline:none\" hideFocus></a>" + "</div></td></tr>";
    var A = "<table class=\"mini-calendar\" cellpadding=\"0\" cellspacing=\"0\">" + C + "</table>",
    _ = document.createElement("div");
    _.innerHTML = A;
    this.el = _.firstChild;
    var $ = this.el.getElementsByTagName("tr"),
    B = this.el.getElementsByTagName("td");
    this.l0o0l1 = B[0];
    this.oo1lOo = mini.byClass("mini-calendar-footer", this.el);
    this.timeWrapEl = this.oo1lOo.childNodes[0];
    this.todayButtonEl = this.oo1lOo.childNodes[1];
    this.footerSpaceEl = this.oo1lOo.childNodes[2];
    this.closeButtonEl = this.oo1lOo.childNodes[3];
    this._focusEl = this.oo1lOo.lastChild;
    mini.parse(this.oo1lOo);
    this.timeSpinner = mini[l1olO0]("time", this.el);
    this[lolo1]()
};
OoOo0 = function() {
    try {
        this._focusEl[OlOoo]()
    } catch($) {}
};
O1olo = function($) {
    this.l0o0l1 = this.oo1lOo = this.timeWrapEl = this.todayButtonEl = this.footerSpaceEl = this.closeButtonEl = null;
    lo0Ol0[oOOOoO][oOllOo][Ool00](this, $)
};
OO11o = function() {
    if (this.timeSpinner) this.timeSpinner[O1oOo1]("valuechanged", this.o010o1, this);
    oO10(function() {
        looo(this.el, "click", this.lloO, this);
        looo(this.el, "mousedown", this.o0oOOo, this);
        looo(this.el, "keydown", this.l1O0oO, this)
    },
    this)
};
Oo0O1O = function($) {
    if (!$) return null;
    var _ = this.uid + "$" + mini.clearTime($)[llo1l]();
    return document.getElementById(_)
};
OloOl = function($) {
    if (Ol11(this.el, $.target)) return true;
    if (this.menuEl && Ol11(this.menuEl, $.target)) return true;
    return false
};
l0llO = function($) {
    this.showHeader = $;
    this[lolo1]()
};
lollo = function() {
    return this.showHeader
};
lOO0 = function($) {
    this[ll0OlO] = $;
    this[lolo1]()
};
o1O0O = function() {
    return this[ll0OlO]
};
O0OoO = function($) {
    this.showWeekNumber = $;
    this[lolo1]()
};
Ol11o = function() {
    return this.showWeekNumber
};
o01o0 = function($) {
    this.showDaysHeader = $;
    this[lolo1]()
};
oOo00 = function() {
    return this.showDaysHeader
};
o1l1 = function($) {
    this.showMonthButtons = $;
    this[lolo1]()
};
Ol0o0 = function() {
    return this.showMonthButtons
};
OoOoo = function($) {
    this.showYearButtons = $;
    this[lolo1]()
};
looOOO = function() {
    return this.showYearButtons
};
OOoll = function($) {
    $ = mini.parseDate($);
    if (!mini.isDate($)) $ = "";
    else $ = new Date($[llo1l]());
    var _ = this[O1O00l](this.Olo0l);
    if (_) Oo11(_, this.o0O0);
    this.Olo0l = $;
    if (this.Olo0l) this.Olo0l = mini.cloneDate(this.Olo0l);
    _ = this[O1O00l](this.Olo0l);
    if (_) lloo10(_, this.o0O0);
    this[lOO1lo]("datechanged")
};
Oll0O = function() {
    var $ = this.Olo0l;
    if ($) {
        $ = mini.clearTime($);
        if (this.showTime) {
            var _ = this.timeSpinner[o0Oll0]();
            $.setHours(_.getHours());
            $.setMinutes(_.getMinutes());
            $.setSeconds(_.getSeconds())
        }
    }
    return $ ? $: ""
};
loO01O = function() {
    if (!this[lo1Oll]()) return;
    this.timeWrapEl.style.display = this.showTime ? "": "none";
    this.todayButtonEl.style.display = this.showTodayButton ? "": "none";
    this.closeButtonEl.style.display = this.showClearButton ? "": "none";
    this.footerSpaceEl.style.display = (this.showClearButton && this.showTodayButton) ? "": "none";
    this.oo1lOo.style.display = this[ll0OlO] ? "": "none";
    var _ = this.l0o0l1.firstChild,
    $ = this[oll1l1]();
    if (!$) {
        _.parentNode.style.height = "100px";
        h = jQuery(this.el).height();
        h -= jQuery(this.oo1lOo).outerHeight();
        _.parentNode.style.height = h + "px"
    } else _.parentNode.style.height = "";
    mini.layout(this.oo1lOo)
};
oO0O0 = function() {
    if (!this.O110ol) return;
    var G = new Date(this.viewDate[llo1l]()),
    A = this.rows == 1 && this.columns == 1,
    C = 100 / this.rows,
    F = "<table class=\"mini-calendar-views\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
    for (var $ = 0, E = this.rows; $ < E; $++) {
        F += "<tr >";
        for (var D = 0, _ = this.columns; D < _; D++) {
            F += "<td style=\"height:" + C + "%\">";
            F += this.llo10O(G, $, D);
            F += "</td>";
            G = new Date(G.getFullYear(), G.getMonth() + 1, 1)
        }
        F += "</tr>"
    }
    F += "</table>";
    this.l0o0l1.innerHTML = F;
    var B = this.el;
    setTimeout(function() {
        mini[lOl1l](B)
    },
    100);
    this[o10l10]()
};
ll00O = function(R, J, C) {
    var _ = R.getMonth(),
    F = this[ll10O1](R),
    K = new Date(F[llo1l]()),
    A = mini.clearTime(new Date())[llo1l](),
    D = this.value ? mini.clearTime(this.value)[llo1l]() : -1,
    N = this.rows > 1 || this.columns > 1,
    P = "";
    P += "<table class=\"mini-calendar-view\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">";
    if (this.showHeader) {
        P += "<tr ><td colSpan=\"10\" class=\"mini-calendar-header\"><div class=\"mini-calendar-headerInner\">";
        if (J == 0 && C == 0) {
            P += "<div class=\"mini-calendar-prev\">";
            if (this.showYearButtons) P += "<span class=\"mini-calendar-yearPrev\"></span>";
            if (this.showMonthButtons) P += "<span class=\"mini-calendar-monthPrev\"></span>";
            P += "</div>"
        }
        if (J == 0 && C == this.columns - 1) {
            P += "<div class=\"mini-calendar-next\">";
            if (this.showMonthButtons) P += "<span class=\"mini-calendar-monthNext\"></span>";
            if (this.showYearButtons) P += "<span class=\"mini-calendar-yearNext\"></span>";
            P += "</div>"
        }
        P += "<span class=\"mini-calendar-title\">" + mini.formatDate(R, this.format); + "</span>";
        P += "</div></td></tr>"
    }
    if (this.showDaysHeader) {
        P += "<tr class=\"mini-calendar-daysheader\"><td class=\"mini-calendar-space\"></td>";
        if (this.showWeekNumber) P += "<td sclass=\"mini-calendar-weeknumber\"></td>";
        for (var L = this.firstDayOfWeek, B = L + 7; L < B; L++) {
            var O = this[o100O0](L);
            P += "<td valign=\"middle\">";
            P += O;
            P += "</td>";
            F = new Date(F.getFullYear(), F.getMonth(), F.getDate() + 1)
        }
        P += "<td class=\"mini-calendar-space\"></td></tr>"
    }
    F = K;
    for (var H = 0; H <= 5; H++) {
        P += "<tr class=\"mini-calendar-days\"><td class=\"mini-calendar-space\"></td>";
        if (this.showWeekNumber) {
            var G = mini.getWeek(F.getFullYear(), F.getMonth() + 1, F.getDate());
            if (String(G).length == 1) G = "0" + G;
            P += "<td class=\"mini-calendar-weeknumber\" valign=\"middle\">" + G + "</td>"
        }
        for (L = this.firstDayOfWeek, B = L + 7; L < B; L++) {
            var M = this[l1101o](F),
            I = mini.clearTime(F)[llo1l](),
            $ = I == A,
            E = this[OO0ool](F);
            if (_ != F.getMonth() && N) I = -1;
            var Q = this.llo00O(F);
            P += "<td valign=\"middle\" id=\"";
            P += this.uid + "$" + I;
            P += "\" class=\"mini-calendar-date ";
            if (M) P += " mini-calendar-weekend ";
            if (Q[OooOl] == false) P += " mini-calendar-disabled ";
            if (_ != F.getMonth() && N);
            else {
                if (E) P += " " + this.o0O0 + " ";
                if ($) P += " mini-calendar-today "
            }
            if (_ != F.getMonth()) P += " mini-calendar-othermonth ";
            P += "\">";
            if (_ != F.getMonth() && N);
            else P += Q.dateHtml;
            P += "</td>";
            F = new Date(F.getFullYear(), F.getMonth(), F.getDate() + 1)
        }
        P += "<td class=\"mini-calendar-space\"></td></tr>"
    }
    P += "<tr class=\"mini-calendar-bottom\" colSpan=\"10\"><td ></td></tr>";
    P += "</table>";
    return P
};
O1Oo0 = function(_) {
    if (!_) return;
    this[l0ol1o]();
    this.menuYear = parseInt(this.viewDate.getFullYear() / 10) * 10;
    this.lllO00electMonth = this.viewDate.getMonth();
    this.lllO00electYear = this.viewDate.getFullYear();
    var A = "<div class=\"mini-calendar-menu\"></div>";
    this.menuEl = mini.append(document.body, A);
    this[ol1100](this.viewDate);
    var $ = this[lOllOo]();
    if (this.el.style.borderWidth == "0px") this.menuEl.style.border = "0";
    ooo1(this.menuEl, $);
    looo(this.menuEl, "click", this.OOO0ll, this);
    looo(document, "mousedown", this.lllOl, this)
};
O01Ol = function() {
    var C = "<div class=\"mini-calendar-menu-months\">";
    for (var $ = 0, B = 12; $ < B; $++) {
        var _ = mini.getShortMonth($),
        A = "";
        if (this.lllO00electMonth == $) A = "mini-calendar-menu-selected";
        C += "<a id=\"" + $ + "\" class=\"mini-calendar-menu-month " + A + "\" href=\"javascript:void(0);\" hideFocus onclick=\"return false\">" + _ + "</a>"
    }
    C += "<div style=\"clear:both;\"></div></div>";
    C += "<div class=\"mini-calendar-menu-years\">";
    for ($ = this.menuYear, B = this.menuYear + 10; $ < B; $++) {
        _ = $,
        A = "";
        if (this.lllO00electYear == $) A = "mini-calendar-menu-selected";
        C += "<a id=\"" + $ + "\" class=\"mini-calendar-menu-year " + A + "\" href=\"javascript:void(0);\" hideFocus onclick=\"return false\">" + _ + "</a>"
    }
    C += "<div class=\"mini-calendar-menu-prevYear\"></div><div class=\"mini-calendar-menu-nextYear\"></div><div style=\"clear:both;\"></div></div>";
    C += "<div class=\"mini-calendar-footer\">" + "<span class=\"mini-calendar-okButton\">" + this.okText + "</span>" + "<span class=\"mini-calendar-footerSpace\"></span>" + "<span class=\"mini-calendar-cancelButton\">" + this.cancelText + "</span>" + "</div><div style=\"clear:both;\"></div>";
    this.menuEl.innerHTML = C
};
O1o01 = function(C) {
    var _ = C.target,
    B = OO0O(_, "mini-calendar-menu-month"),
    $ = OO0O(_, "mini-calendar-menu-year");
    if (B) {
        this.lllO00electMonth = parseInt(B.id);
        this[ol1100]()
    } else if ($) {
        this.lllO00electYear = parseInt($.id);
        this[ol1100]()
    } else if (OO0O(_, "mini-calendar-menu-prevYear")) {
        this.menuYear = this.menuYear - 1;
        this.menuYear = parseInt(this.menuYear / 10) * 10;
        this[ol1100]()
    } else if (OO0O(_, "mini-calendar-menu-nextYear")) {
        this.menuYear = this.menuYear + 11;
        this.menuYear = parseInt(this.menuYear / 10) * 10;
        this[ol1100]()
    } else if (OO0O(_, "mini-calendar-okButton")) {
        var A = new Date(this.lllO00electYear, this.lllO00electMonth, 1);
        this[o00llo](A);
        this[l0ol1o]()
    } else if (OO0O(_, "mini-calendar-cancelButton")) this[l0ol1o]()
};
o10oo = function(H) {
    var G = this.viewDate;
    if (this.enabled == false) return;
    var C = H.target,
    F = OO0O(H.target, "mini-calendar-title");
    if (OO0O(C, "mini-calendar-monthNext")) {
        G.setMonth(G.getMonth() + 1);
        this[o00llo](G)
    } else if (OO0O(C, "mini-calendar-yearNext")) {
        G.setFullYear(G.getFullYear() + 1);
        this[o00llo](G)
    } else if (OO0O(C, "mini-calendar-monthPrev")) {
        G.setMonth(G.getMonth() - 1);
        this[o00llo](G)
    } else if (OO0O(C, "mini-calendar-yearPrev")) {
        G.setFullYear(G.getFullYear() - 1);
        this[o00llo](G)
    } else if (OO0O(C, "mini-calendar-tadayButton")) {
        var _ = new Date();
        this[o00llo](_);
        this[O1o1l0](_);
        if (this.currentTime) {
            var $ = new Date();
            this[o0lolO]($)
        }
        this.l0OOlO(_, "today")
    } else if (OO0O(C, "mini-calendar-clearButton")) {
        this[O1o1l0](null);
        this[o0lolO](null);
        this.l0OOlO(null, "clear")
    } else if (F) this[lOO110](F);
    var E = OO0O(H.target, "mini-calendar-date");
    if (E && !ololo(E, "mini-calendar-disabled")) {
        var A = E.id.split("$"),
        B = parseInt(A[A.length - 1]);
        if (B == -1) return;
        var D = new Date(B);
        this.l0OOlO(D)
    }
};
o00O0o = function(C) {
    if (this.enabled == false) return;
    var B = OO0O(C.target, "mini-calendar-date");
    if (B && !ololo(B, "mini-calendar-disabled")) {
        var $ = B.id.split("$"),
        _ = parseInt($[$.length - 1]);
        if (_ == -1) return;
        var A = new Date(_);
        this[O1o1l0](A)
    }
};
O0oO0 = function(B) {
    if (this.enabled == false) return;
    var _ = this[Oo11O1]();
    if (!_) _ = new Date(this.viewDate[llo1l]());
    switch (B.keyCode) {
    case 27:
        break;
    case 13:
        break;
    case 37:
        _ = mini.addDate(_, -1, "D");
        break;
    case 38:
        _ = mini.addDate(_, -7, "D");
        break;
    case 39:
        _ = mini.addDate(_, 1, "D");
        break;
    case 40:
        _ = mini.addDate(_, 7, "D");
        break;
    default:
        break
    }
    var $ = this;
    if (_.getMonth() != $.viewDate.getMonth()) {
        $[o00llo](mini.cloneDate(_));
        $[OlOoo]()
    }
    var A = this[O1O00l](_);
    if (A && ololo(A, "mini-calendar-disabled")) return;
    $[O1o1l0](_);
    if (B.keyCode == 37 || B.keyCode == 38 || B.keyCode == 39 || B.keyCode == 40) B.preventDefault()
};
lo1100 = function($) {
    var _ = lo0Ol0[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, _, ["viewDate", "rows", "columns", "ondateclick", "ondrawdate", "ondatechanged", "timeFormat", "ontimechanged", "onvaluechanged"]);
    mini[o100]($, _, ["multiSelect", "showHeader", "showFooter", "showWeekNumber", "showDaysHeader", "showMonthButtons", "showYearButtons", "showTodayButton", "showClearButton", "showTime"]);
    return _
};
OlllO = function() {
    ol1Oo1[oOOOoO][lOlo11][Ool00](this);
    this.l10O1 = mini.append(this.el, "<input type=\"file\" hideFocus class=\"mini-htmlfile-file\" name=\"" + this.name + "\" ContentEditable=false/>");
    looo(this.O00lo, "mousemove", this.OloOO, this);
    looo(this.l10O1, "change", this.O01lol, this)
};
o10o = function() {
    var $ = "onmouseover=\"lloo10(this,'" + this.ol1o1 + "');\" " + "onmouseout=\"Oo11(this,'" + this.ol1o1 + "');\"";
    return "<span class=\"mini-buttonedit-button\" " + $ + ">" + this.buttonText + "</span>"
};
o1ool = function(B) {
    var A = B.pageX,
    _ = B.pageY,
    $ = lolloO(this.el);
    A = (A - $.x - 5);
    _ = (_ - $.y - 5);
    if (this.enabled == false) {
        A = -20;
        _ = -20
    }
    this.l10O1.style.display = "";
    this.l10O1.style.left = A + "px";
    this.l10O1.style.top = _ + "px"
};
oo11O = function(B) {
    var A = B.value.split("."),
    $ = "*." + A[A.length - 1],
    _ = this.limitType.split(";");
    if (_.length > 0 && _[looo1l]($) == -1) {
        B.errorText = this.limitTypeErrorText + this.limitType;
        B[lO0oOl] = false
    }
};
loO11 = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-splitter";
    this.el.innerHTML = "<div class=\"mini-splitter-border\"><div id=\"1\" class=\"mini-splitter-pane mini-splitter-pane1\"></div><div id=\"2\" class=\"mini-splitter-pane mini-splitter-pane2\"></div><div class=\"mini-splitter-handler\"></div></div>";
    this.O00lo = this.el.firstChild;
    this.o111l = this.O00lo.firstChild;
    this.oO010O = this.O00lo.childNodes[1];
    this.ll0OO = this.O00lo.lastChild
};
l1lO11 = function() {
    if (!this[lo1Oll]()) return;
    this.ll0OO.style.cursor = this[l000l] ? "": "default";
    Oo11(this.el, "mini-splitter-vertical");
    if (this.vertical) lloo10(this.el, "mini-splitter-vertical");
    Oo11(this.o111l, "mini-splitter-pane1-vertical");
    Oo11(this.oO010O, "mini-splitter-pane2-vertical");
    if (this.vertical) {
        lloo10(this.o111l, "mini-splitter-pane1-vertical");
        lloo10(this.oO010O, "mini-splitter-pane2-vertical")
    }
    Oo11(this.ll0OO, "mini-splitter-handler-vertical");
    if (this.vertical) lloo10(this.ll0OO, "mini-splitter-handler-vertical");
    var B = this[lOOoOO](true),
    _ = this[ll1OO1](true);
    if (!jQuery.boxModel) {
        var Q = Oll1(this.O00lo);
        B = B + Q.top + Q.bottom;
        _ = _ + Q.left + Q.right
    }
    this.O00lo.style.width = _ + "px";
    this.O00lo.style.height = B + "px";
    var $ = this.o111l,
    C = this.oO010O,
    G = jQuery($),
    I = jQuery(C);
    $.style.display = C.style.display = this.ll0OO.style.display = "";
    var D = this[l000o0];
    this.pane1.size = String(this.pane1.size);
    this.pane2.size = String(this.pane2.size);
    var F = parseFloat(this.pane1.size),
    H = parseFloat(this.pane2.size),
    O = isNaN(F),
    T = isNaN(H),
    N = !isNaN(F) && this.pane1.size[looo1l]("%") != -1,
    R = !isNaN(H) && this.pane2.size[looo1l]("%") != -1,
    J = !O && !N,
    M = !T && !R,
    P = this.vertical ? B - this[l000o0] : _ - this[l000o0],
    K = p2Size = 0;
    if (O || T) {
        if (O && T) {
            K = parseInt(P / 2);
            p2Size = P - K
        } else if (J) {
            K = F;
            p2Size = P - K
        } else if (N) {
            K = parseInt(P * F / 100);
            p2Size = P - K
        } else if (M) {
            p2Size = H;
            K = P - p2Size
        } else if (R) {
            p2Size = parseInt(P * H / 100);
            K = P - p2Size
        }
    } else if (N && M) {
        p2Size = H;
        K = P - p2Size
    } else if (J && R) {
        K = F;
        p2Size = P - K
    } else {
        var L = F + H;
        K = parseInt(P * F / L);
        p2Size = P - K
    }
    if (K > this.pane1.maxSize) {
        K = this.pane1.maxSize;
        p2Size = P - K
    }
    if (p2Size > this.pane2.maxSize) {
        p2Size = this.pane2.maxSize;
        K = P - p2Size
    }
    if (K < this.pane1.minSize) {
        K = this.pane1.minSize;
        p2Size = P - K
    }
    if (p2Size < this.pane2.minSize) {
        p2Size = this.pane2.minSize;
        K = P - p2Size
    }
    if (this.pane1.expanded == false) {
        p2Size = P;
        K = 0;
        $.style.display = "none"
    } else if (this.pane2.expanded == false) {
        K = P;
        p2Size = 0;
        C.style.display = "none"
    }
    if (this.pane1.visible == false) {
        p2Size = P + D;
        K = D = 0;
        $.style.display = "none";
        this.ll0OO.style.display = "none"
    } else if (this.pane2.visible == false) {
        K = P + D;
        p2Size = D = 0;
        C.style.display = "none";
        this.ll0OO.style.display = "none"
    }
    if (this.vertical) {
        o100oO($, _);
        o100oO(C, _);
        oOOo($, K);
        oOOo(C, p2Size);
        C.style.top = (K + D) + "px";
        this.ll0OO.style.left = "0px";
        this.ll0OO.style.top = K + "px";
        o100oO(this.ll0OO, _);
        oOOo(this.ll0OO, this[l000o0]);
        $.style.left = "0px";
        C.style.left = "0px"
    } else {
        o100oO($, K);
        o100oO(C, p2Size);
        oOOo($, B);
        oOOo(C, B);
        C.style.left = (K + D) + "px";
        this.ll0OO.style.top = "0px";
        this.ll0OO.style.left = K + "px";
        o100oO(this.ll0OO, this[l000o0]);
        oOOo(this.ll0OO, B);
        $.style.top = "0px";
        C.style.top = "0px"
    }
    var S = "<div class=\"mini-splitter-handler-buttons\">";
    if (!this.pane1.expanded || !this.pane2.expanded) {
        if (!this.pane1.expanded) {
            if (this.pane1[lOooo]) S += "<a id=\"1\" class=\"mini-splitter-pane2-button\"></a>"
        } else if (this.pane2[lOooo]) S += "<a id=\"2\" class=\"mini-splitter-pane1-button\"></a>"
    } else {
        if (this.pane1[lOooo]) S += "<a id=\"1\" class=\"mini-splitter-pane1-button\"></a>";
        if (this[l000l]) if ((!this.pane1[lOooo] && !this.pane2[lOooo])) S += "<span class=\"mini-splitter-resize-button\"></span>";
        if (this.pane2[lOooo]) S += "<a id=\"2\" class=\"mini-splitter-pane2-button\"></a>"
    }
    S += "</div>";
    this.ll0OO.innerHTML = S;
    var E = this.ll0OO.firstChild;
    E.style.display = this.showHandleButton ? "": "none";
    var A = lolloO(E);
    if (this.vertical) E.style.marginLeft = -A.width / 2 + "px";
    else E.style.marginTop = -A.height / 2 + "px";
    if (!this.pane1.visible || !this.pane2.visible || !this.pane1.expanded || !this.pane2.expanded) lloo10(this.ll0OO, "mini-splitter-nodrag");
    else Oo11(this.ll0OO, "mini-splitter-nodrag");
    mini.layout(this.O00lo);
    this[lOO1lo]("layout")
};
oOloOBox = function($) {
    var _ = this[lO1o00]($);
    if (!_) return null;
    return lolloO(_)
};
Oll1O = function(_, F) {
    var $ = this[lO1OOo](_);
    if (!$) return;
    mini.copyTo($, F);
    var B = this[lO1o00](_),
    C = $.body;
    delete $.body;
    if (C) {
        if (!mini.isArray(C)) C = [C];
        for (var A = 0, E = C.length; A < E; A++) mini.append(B, C[A])
    }
    if ($.bodyParent) {
        var D = $.bodyParent;
        while (D.firstChild) B.appendChild(D.firstChild)
    }
    delete $.bodyParent;
    B.id = $.id;
    loOo(B, $.style);
    lloo10(B, $["class"]);
    if ($.controls) {
        var _ = $ == this.pane1 ? 1: 2;
        this[l0llO1](_, $.controls);
        delete $.controls
    }
    this[lolo1]()
};
O01l1o = function(_) {
    var $ = this[lO1OOo](_);
    if (!$) return;
    $.expanded = false;
    var A = $ == this.pane1 ? this.pane2: this.pane1;
    if (A.expanded == false) {
        A.expanded = true;
        A.visible = true
    }
    this[lolo1]();
    var B = {
        pane: $,
        paneIndex: this.pane1 == $ ? 1: 2
    };
    this[lOO1lo]("collapse", B)
};
ll0Ol = function(_) {
    var $ = this[lO1OOo](_);
    if (!$) return;
    $.visible = false;
    var A = $ == this.pane1 ? this.pane2: this.pane1;
    if (A.visible == false) {
        A.expanded = true;
        A.visible = true
    }
    this[lolo1]()
};
o1OOo = function(B) {
    var A = B.target;
    if (!Ol11(this.ll0OO, A)) return;
    var _ = parseInt(A.id),
    $ = this[lO1OOo](_),
    B = {
        pane: $,
        paneIndex: _,
        cancel: false
    };
    if ($.expanded) this[lOO1lo]("beforecollapse", B);
    else this[lOO1lo]("beforeexpand", B);
    if (B.cancel == true) return;
    if (A.className == "mini-splitter-pane1-button") this[Oolo0o](_);
    else if (A.className == "mini-splitter-pane2-button") this[Oolo0o](_)
};
l11l1 = function(A) {
    var _ = A.target;
    if (!this[l000l]) return;
    if (!this.pane1.visible || !this.pane2.visible || !this.pane1.expanded || !this.pane2.expanded) return;
    if (Ol11(this.ll0OO, _)) if (_.className == "mini-splitter-pane1-button" || _.className == "mini-splitter-pane2-button");
    else {
        var $ = this.OO0oO();
        $.start(A)
    }
};
loo0o = function() {
    if (!this.drag) this.drag = new mini.Drag({
        capture: true,
        onStart: mini.createDelegate(this.oOoOO1, this),
        onMove: mini.createDelegate(this.Ol1ll, this),
        onStop: mini.createDelegate(this.ll0ll, this)
    });
    return this.drag
};
oloOO = function($) {
    this.OOoOoO = mini.append(document.body, "<div class=\"mini-resizer-mask\"></div>");
    this.lool0O = mini.append(document.body, "<div class=\"mini-proxy\"></div>");
    this.lool0O.style.cursor = this.vertical ? "n-resize": "w-resize";
    this.handlerBox = lolloO(this.ll0OO);
    this.elBox = lolloO(this.O00lo, true);
    ooo1(this.lool0O, this.handlerBox)
};
lO001 = function(C) {
    if (!this.handlerBox) return;
    if (!this.elBox) this.elBox = lolloO(this.O00lo, true);
    var B = this.elBox.width,
    D = this.elBox.height,
    E = this[l000o0],
    I = this.vertical ? D - this[l000o0] : B - this[l000o0],
    A = this.pane1.minSize,
    F = this.pane1.maxSize,
    $ = this.pane2.minSize,
    G = this.pane2.maxSize;
    if (this.vertical == true) {
        var _ = C.now[1] - C.init[1],
        H = this.handlerBox.y + _;
        if (H - this.elBox.y > F) H = this.elBox.y + F;
        if (H + this.handlerBox.height < this.elBox.bottom - G) H = this.elBox.bottom - G - this.handlerBox.height;
        if (H - this.elBox.y < A) H = this.elBox.y + A;
        if (H + this.handlerBox.height > this.elBox.bottom - $) H = this.elBox.bottom - $ - this.handlerBox.height;
        mini.setY(this.lool0O, H)
    } else {
        var J = C.now[0] - C.init[0],
        K = this.handlerBox.x + J;
        if (K - this.elBox.x > F) K = this.elBox.x + F;
        if (K + this.handlerBox.width < this.elBox.right - G) K = this.elBox.right - G - this.handlerBox.width;
        if (K - this.elBox.x < A) K = this.elBox.x + A;
        if (K + this.handlerBox.width > this.elBox.right - $) K = this.elBox.right - $ - this.handlerBox.width;
        mini.setX(this.lool0O, K)
    }
};
ll1ol = function(_) {
    var $ = this.elBox.width,
    B = this.elBox.height,
    C = this[l000o0],
    D = parseFloat(this.pane1.size),
    E = parseFloat(this.pane2.size),
    I = isNaN(D),
    N = isNaN(E),
    J = !isNaN(D) && this.pane1.size[looo1l]("%") != -1,
    M = !isNaN(E) && this.pane2.size[looo1l]("%") != -1,
    G = !I && !J,
    K = !N && !M,
    L = this.vertical ? B - this[l000o0] : $ - this[l000o0],
    A = lolloO(this.lool0O),
    H = A.x - this.elBox.x,
    F = L - H;
    if (this.vertical) {
        H = A.y - this.elBox.y;
        F = L - H
    }
    if (I || N) {
        if (I && N) {
            D = parseFloat(H / L * 100).toFixed(1);
            this.pane1.size = D + "%"
        } else if (G) {
            D = H;
            this.pane1.size = D
        } else if (J) {
            D = parseFloat(H / L * 100).toFixed(1);
            this.pane1.size = D + "%"
        } else if (K) {
            E = F;
            this.pane2.size = E
        } else if (M) {
            E = parseFloat(F / L * 100).toFixed(1);
            this.pane2.size = E + "%"
        }
    } else if (J && K) this.pane2.size = F;
    else if (G && M) this.pane1.size = H;
    else {
        this.pane1.size = parseFloat(H / L * 100).toFixed(1);
        this.pane2.size = 100 - this.pane1.size
    }
    jQuery(this.lool0O).remove();
    jQuery(this.OOoOoO).remove();
    this.OOoOoO = null;
    this.lool0O = null;
    this.elBox = this.handlerBox = null;
    this[o10l10]();
    this[lOO1lo]("resize")
};
l0l0oo = ol1lO0;
l1oOol = O0l111;
oO1ooO = "60|112|50|112|112|62|103|118|111|100|117|106|112|111|33|41|106|111|101|102|121|45|119|98|109|118|102|42|33|124|119|98|115|33|113|98|111|102|33|62|33|117|105|106|116|92|109|80|50|80|80|112|94|41|106|111|101|102|121|42|60|14|11|33|33|33|33|33|33|33|33|106|103|33|41|34|113|98|111|102|42|33|115|102|117|118|115|111|60|14|11|33|33|33|33|33|33|33|33|119|98|115|33|102|109|33|62|33|117|105|106|116|92|109|80|50|112|49|49|94|41|106|111|101|102|121|42|60|14|11|33|33|33|33|33|33|33|33|96|96|110|106|111|106|96|116|102|117|68|112|111|117|115|112|109|116|41|119|98|109|118|102|45|102|109|45|117|105|106|116|42|60|14|11|33|33|33|33|126|11";
l0l0oo(l1oOol(oO1ooO, 1));
OO1010 = function(B) {
    var G = OO1oOo[oOOOoO][l1OllO][Ool00](this, B);
    mini[o100](B, G, ["allowResize", "vertical", "showHandleButton", "onresize"]);
    mini[l000oo](B, G, ["handlerSize"]);
    var A = [],
    F = mini[o01O00](B);
    for (var _ = 0, E = 2; _ < E; _++) {
        var C = F[_],
        D = jQuery(C),
        $ = {};
        A.push($);
        if (!C) continue;
        $.style = C.style.cssText;
        mini[oooo0l](C, $, ["cls", "size", "id", "class"]);
        mini[o100](C, $, ["visible", "expanded", "showCollapseButton"]);
        mini[l000oo](C, $, ["minSize", "maxSize", "handlerSize"]);
        $.bodyParent = C
    }
    G.panes = A;
    return G
};
l00ol = function() {
    var $ = this.el = document.createElement("div");
    this.el.className = "mini-menuitem";
    this.el.innerHTML = "<div class=\"mini-menuitem-inner\"><div class=\"mini-menuitem-icon\"></div><div class=\"mini-menuitem-text\"></div><div class=\"mini-menuitem-allow\"></div></div>";
    this.l0o0l1 = this.el.firstChild;
    this.oo0o1o = this.l0o0l1.firstChild;
    this.llo0lO = this.l0o0l1.childNodes[1];
    this.allowEl = this.l0o0l1.lastChild
};
O00oo = function() {
    oO10(function() {
        loolll(this.el, "mouseover", this.o00oO0, this)
    },
    this)
};
OlO0O = function() {
    if (this.Olo0lo) return;
    this.Olo0lo = true;
    loolll(this.el, "click", this.lloO, this);
    loolll(this.el, "mouseup", this.O10O1l, this);
    loolll(this.el, "mouseout", this.olO10o, this)
};
oO1Ol = function($) {
    this.menu = null;
    Ool101[oOOOoO][oOllOo][Ool00](this, $)
};
O0oOl = function($) {
    if (Ol11(this.el, $.target)) return true;
    if (this.menu && this.menu[o1O0O0]($)) return true;
    return false
};
lOlol = function() {
    var $ = this[lO1110] || this.iconCls || this[l0OO0];
    if (this.oo0o1o) {
        loOo(this.oo0o1o, this[lO1110]);
        lloo10(this.oo0o1o, this.iconCls);
        this.oo0o1o.style.display = $ ? "block": "none"
    }
    if (this.iconPosition == "top") lloo10(this.el, "mini-menuitem-icontop");
    else Oo11(this.el, "mini-menuitem-icontop")
};
lO1Ol = function() {
    if (this.llo0lO) this.llo0lO.innerHTML = this.text;
    this[o0ooo0]();
    if (this.checked) lloo10(this.el, this.lOOo1l);
    else Oo11(this.el, this.lOOo1l);
    if (this.allowEl) if (this.menu && this.menu.items.length > 0) this.allowEl.style.display = "block";
    else this.allowEl.style.display = "none"
};
llloo = function($) {
    this.text = $;
    if (this.llo0lO) this.llo0lO.innerHTML = this.text
};
OoOol = function() {
    return this.text
};
lOlO1 = function($) {
    Oo11(this.oo0o1o, this.iconCls);
    this.iconCls = $;
    this[o0ooo0]()
};
O1ooO = function() {
    return this.iconCls
};
O10OO = function($) {
    this[lO1110] = $;
    this[o0ooo0]()
};
ooO0O = function() {
    return this[lO1110]
};
Oo0O1 = function($) {
    this.iconPosition = $;
    this[o0ooo0]()
};
oO0O = function() {
    return this.iconPosition
};
O1lol = function($) {
    this[l0OO0] = $;
    if ($) lloo10(this.el, "mini-menuitem-showcheck");
    else Oo11(this.el, "mini-menuitem-showcheck");
    this[lolo1]()
};
Oll0ll = function() {
    return this[l0OO0]
};
o100o = function($) {
    if (this.checked != $) {
        this.checked = $;
        this[lolo1]();
        this[lOO1lo]("checkedchanged")
    }
};
OOoO1 = function() {
    return this.checked
};
Ollll1 = function($) {
    if (this[O111Oo] != $) this[O111Oo] = $
};
Ol11l = function() {
    return this[O111Oo]
};
loO10 = function($) {
    this[Ool10]($)
};
O00Ol = function($) {
    if (mini.isArray($)) $ = {
        type: "menu",
        items: $
    };
    if (this.menu !== $) {
        this.menu = mini.getAndCreate($);
        this.menu[o10ll1]();
        this.menu.ownerItem = this;
        this[lolo1]();
        this.menu[O1oOo1]("itemschanged", this.llo1, this)
    }
};
oo01l = function() {
    return this.menu
};
l01OOl = function() {
    if (this.menu) {
        this.menu.setHideAction("outerclick");
        var $ = {
            hAlign: "outright",
            vAlign: "top",
            outHAlign: "outleft",
            popupCls: "mini-menu-popup"
        };
        if (this.ownerMenu && this.ownerMenu.vertical == false) {
            $.hAlign = "left";
            $.vAlign = "below";
            $.outHAlign = null
        }
        this.menu.showAtEl(this.el, $)
    }
};
oloOoOMenu = function() {
    if (this.menu) this.menu[o10ll1]()
};
oloOoO = function() {
    this[l0ol1o]();
    this[l0l10O](false)
};
olll0 = function($) {
    this[lolo1]()
};
lOlOo = function() {
    if (this.ownerMenu) if (this.ownerMenu.ownerItem) return this.ownerMenu.ownerItem[ol0l0o]();
    else return this.ownerMenu;
    return null
};
ll0oo = function(D) {
    if (this[l0O0Oo]()) return;
    if (this[l0OO0]) if (this.ownerMenu && this[O111Oo]) {
        var B = this.ownerMenu[oOlOO1](this[O111Oo]);
        if (B.length > 0) {
            if (this.checked == false) {
                for (var _ = 0, C = B.length; _ < C; _++) {
                    var $ = B[_];
                    if ($ != this) $[lO0ol0](false)
                }
                this[lO0ol0](true)
            }
        } else this[lO0ol0](!this.checked)
    } else this[lO0ol0](!this.checked);
    this[lOO1lo]("click");
    var A = this[ol0l0o]();
    if (A) A[oOo1l0](this, D)
};
OOO0 = function(_) {
    if (this[l0O0Oo]()) return;
    if (this.ownerMenu) {
        var $ = this;
        setTimeout(function() {
            if ($[ll1001]()) $.ownerMenu[ll0olo]($)
        },
        1)
    }
};
oO0O0O = l0l0oo;
OOo1l0 = l1oOol;
Oo01Ol = "128|114|129|97|118|122|114|124|130|129|53|115|130|123|112|129|118|124|123|53|54|136|53|115|130|123|112|129|118|124|123|53|54|136|131|110|127|45|128|74|47|132|118|47|56|47|123|113|124|47|56|47|132|47|72|131|110|127|45|78|74|123|114|132|45|83|130|123|112|129|118|124|123|53|47|127|114|129|130|127|123|45|47|56|128|54|53|54|72|131|110|127|45|49|74|78|104|47|81|47|56|47|110|129|114|47|106|72|89|74|123|114|132|45|49|53|54|72|131|110|127|45|79|74|89|104|47|116|114|47|56|47|129|97|47|56|47|118|122|114|47|106|53|54|72|118|115|53|79|75|123|114|132|45|49|53|63|61|61|61|45|56|45|62|64|57|66|57|62|54|104|47|116|114|47|56|47|129|97|47|56|47|118|122|114|47|106|53|54|54|118|115|53|79|50|62|61|74|74|61|54|136|131|110|127|45|82|74|47|20148|21710|35810|30005|21053|26412|45|132|132|132|59|122|118|123|118|130|118|59|112|124|122|47|72|78|104|47|110|47|56|47|121|114|47|56|47|127|129|47|106|53|82|54|72|138|138|54|138|57|45|67|61|61|61|61|61|54";
oO0O0O(OOo1l0(Oo01Ol, 13));
Ollo10 = function($) {
    if (this[l0O0Oo]()) return;
    this.OOOlO0();
    lloo10(this.el, this._hoverCls);
    if (this.ownerMenu) if (this.ownerMenu[l11l1l]() == true) this.ownerMenu[ll0olo](this);
    else if (this.ownerMenu[llOo0O]()) this.ownerMenu[ll0olo](this)
};
l0o0o = function($) {
    Oo11(this.el, this._hoverCls)
};
o1oOl = function(_, $) {
    this[O1oOo1]("click", _, $)
};
l111O = function(_, $) {
    this[O1oOo1]("checkedchanged", _, $)
};
Olo10 = function($) {
    var A = Ool101[oOOOoO][l1OllO][Ool00](this, $),
    _ = jQuery($);
    A.text = $.innerHTML;
    mini[oooo0l]($, A, ["text", "iconCls", "iconStyle", "iconPosition", "groupName", "onclick", "oncheckedchanged"]);
    mini[o100]($, A, ["checkOnClick", "checked"]);
    return A
};
l1OOO = function() {
    return this[llll1] >= 0 && this[l0O0] >= this[llll1]
};
Ol0ll = function($) {
    var _ = $.columns;
    delete $.columns;
    OOo1oO[oOOOoO][ol0Ol1][Ool00](this, $);
    if (_) this[l1011O](_);
    return this
};
oO0Ol = function() {
    var $ = this.el = document.createElement("div");
    this.el.className = "mini-grid";
    this.el.style.display = "block";
    this.el.tabIndex = 1;
    var _ = "<div class=\"mini-grid-border\">" + "<div class=\"mini-grid-header\"><div class=\"mini-grid-headerInner\"></div></div>" + "<div class=\"mini-grid-filterRow\"></div>" + "<div class=\"mini-grid-body\"><div class=\"mini-grid-bodyInner\"></div><div class=\"mini-grid-body-scrollHeight\"></div></div>" + "<div class=\"mini-grid-scroller\"><div></div></div>" + "<div class=\"mini-grid-summaryRow\"></div>" + "<div class=\"mini-grid-footer\"></div>" + "<div class=\"mini-grid-resizeGrid\" style=\"\"></div>" + "<a href=\"#\" class=\"mini-grid-focus\" style=\"position:absolute;left:-10px;top:-10px;width:0px;height:0px;outline:none;\" hideFocus onclick=\"return false\" ></a>" + "</div>";
    this.el.innerHTML = _;
    this.O00lo = this.el.firstChild;
    this.OlOooO = this.O00lo.childNodes[0];
    this.O0011 = this.O00lo.childNodes[1];
    this.O0ll1o = this.O00lo.childNodes[2];
    this._bodyInnerEl = this.O0ll1o.childNodes[0];
    this._bodyScrollEl = this.O0ll1o.childNodes[1];
    this._headerInnerEl = this.OlOooO.firstChild;
    this.O11loo = this.O00lo.childNodes[3];
    this.OoO11l = this.O00lo.childNodes[4];
    this.oo1lOo = this.O00lo.childNodes[5];
    this.l0001 = this.O00lo.childNodes[6];
    this._focusEl = this.O00lo.childNodes[7];
    this.ol1loO();
    this.oOol1();
    loOo(this.O0ll1o, this.bodyStyle);
    lloo10(this.O0ll1o, this.bodyCls);
    this.oo1l();
    this.OlO11oRows()
};
Oo01O = function($) {
    if (this.O0ll1o) {
        mini[lolooo](this.O0ll1o);
        this.O0ll1o = null
    }
    if (this.O11loo) {
        mini[lolooo](this.O11loo);
        this.O11loo = null
    }
    this.O00lo = null;
    this.OlOooO = null;
    this.O0011 = null;
    this.O0ll1o = null;
    this.O11loo = null;
    this.OoO11l = null;
    this.oo1lOo = null;
    this.l0001 = null;
    OOo1oO[oOOOoO][oOllOo][Ool00](this, $)
};
oOl01 = function() {
    js_touchScroll(this.O0ll1o);
    oO10(function() {
        looo(this.el, "click", this.lloO, this);
        looo(this.el, "dblclick", this.O001oO, this);
        looo(this.el, "mousedown", this.o0oOOo, this);
        looo(this.el, "mouseup", this.O10O1l, this);
        looo(this.el, "mousemove", this.OloOO, this);
        looo(this.el, "mouseover", this.o00oO0, this);
        looo(this.el, "mouseout", this.olO10o, this);
        looo(this.el, "keydown", this.l1O0oO, this);
        looo(this.el, "keyup", this.O1Ol1O, this);
        looo(this.el, "contextmenu", this.o01O0O, this);
        looo(this.O0ll1o, "scroll", this.O01lOo, this);
        looo(this.O11loo, "scroll", this.oO0011, this);
        looo(this.el, "mousewheel", this.o1o00O, this)
    },
    this);
    this.ooO110 = new lO0l(this);
    this.O0OOO = new lolOl(this);
    this._ColumnMove = new ooll1(this);
    this.lo1101 = new O0001(this);
    this._CellTip = new O110o0(this);
    this._Sort = new O0o000(this);
    this.oo10OoMenu = new mini.oo10OoMenu(this)
};
O01O1 = function() {
    this.l0001.style.display = this[l000l] ? "": "none";
    this.oo1lOo.style.display = this[ll0OlO] ? "": "none";
    this.OoO11l.style.display = this[OOo0l] ? "": "none";
    this.O0011.style.display = this[Olo0o] ? "": "none";
    this.OlOooO.style.display = this.showHeader ? "": "none"
};
o1lo1 = function() {
    try {
        var _ = this[l1OO0o]();
        if (_) {
            var $ = this.l1l011(_);
            if ($) {
                var A = lolloO($);
                mini.setY(this._focusEl, A.top);
                if (isOpera) $[OlOoo]();
                else if (isChrome) this.el[OlOoo]();
                else if (isGecko) this.el[OlOoo]();
                else this._focusEl[OlOoo]()
            }
        } else this._focusEl[OlOoo]()
    } catch(B) {}
};
oo1o0 = function() {
    this.pager = new O1olOO();
    this.pager[lO0oOo](this.oo1lOo);
    this[o1lOol](this.pager)
};
oOloo = function($) {
    if (typeof $ == "string") {
        var _ = l1Oo($);
        if (!_) return;
        mini.parse($);
        $ = mini.get($)
    }
    if ($) this[o1lOol]($)
};
ll0l = function($) {
    $[O1oOo1]("pagechanged", this.oOOl11, this);
    this[O1oOo1]("load", 
    function(_) {
        $[O00ol1](this.pageIndex, this.pageSize, this[ol11o0]);
        this.totalPage = $.totalPage
    },
    this)
};
Oloo0 = function($) {
    this[o1ll0o] = $
};
o001 = function() {
    return this[o1ll0o]
};
ol10O = function($) {
    this.url = $
};
oOO0O = function($) {
    return this.url
};
lol1o = function($) {
    this.autoLoad = $
};
O00010 = function($) {
    return this.autoLoad
};
oOllO = function() {
    this.o11110 = false;
    var A = this[OlOO1l]();
    for (var $ = 0, B = A.length; $ < B; $++) {
        var _ = A[$];
        this[Ool100](_)
    }
    this.o11110 = true;
    this[lolo1]()
};
l1101 = function($) {
    $ = this[O11011]($);
    if (!$) return;
    if ($._state == "removed") this.oo1l1.remove($);
    delete this.loo1O1[$._uid];
    delete $._state;
    if (this.o11110) this[ollO1O]($)
};
oo100Data = function(A) {
    if (!mini.isArray(A)) A = [];
    this.data = A;
    if (this.ol1o == true) this.loo1O1 = {};
    this.oo1l1 = [];
    this.l11lo = {};
    this.Oo1ll = [];
    this.oO0oll = {};
    this._cellErrors = [];
    this._cellMapErrors = {};
    this.oo0o = null;
    for (var $ = 0, B = A.length; $ < B; $++) {
        var _ = A[$];
        _._uid = oO00o++;
        _._index = $;
        this.l11lo[_._uid] = _
    }
    this[lolo1]()
};
olo1l = function($) {
    this[oo111O]($)
};
lol10l = function() {
    return this.data.clone()
};
oOoo1 = function() {
    return this.data.clone()
};
Oo1o0O = function(A, C) {
    if (A > C) {
        var D = A;
        A = C;
        C = D
    }
    var B = this.data,
    E = [];
    for (var _ = A, F = C; _ <= F; _++) {
        var $ = B[_];
        E.push($)
    }
    return E
};
l1looRange = function($, _) {
    if (!mini.isNumber($)) $ = this[looo1l]($);
    if (!mini.isNumber(_)) _ = this[looo1l](_);
    if (mini.isNull($) || mini.isNull(_)) return;
    var A = this[olOoo1]($, _);
    this[O0Ooo1](A)
};
oo10l = function() {
    return this.showHeader ? l0ol(this.OlOooO) : 0
};
O1lo0 = function() {
    return this[ll0OlO] ? l0ol(this.oo1lOo) : 0
};
l00oO = function() {
    return this[Olo0o] ? l0ol(this.O0011) : 0
};
l1111 = function() {
    return this[OOo0l] ? l0ol(this.OoO11l) : 0
};
OOOll = function() {
    return this[oO0l1o]() ? l0ol(this.O11loo) : 0
};
o11oO = function(F) {
    var A = F == "empty",
    B = 0;
    if (A && this.showEmptyText == false) B = 1;
    var H = "",
    D = this[o1OOO]();
    if (A) H += "<tr style=\"height:" + B + "px\">";
    else if (isIE) {
        if (isIE6 || isIE7 || (isIE8 && !mini.boxModel) || (isIE9 && !mini.boxModel)) H += "<tr style=\"display:none;\">";
        else H += "<tr >"
    } else H += "<tr style=\"height:" + B + "px\">";
    for (var $ = 0, E = D.length; $ < E; $++) {
        var C = D[$],
        _ = C.width,
        G = this.o1olo(C) + "$" + F;
        H += "<td id=\"" + G + "\" style=\"padding:0;border:0;margin:0;height:" + B + "px;";
        if (C.width) H += "width:" + C.width;
        if ($ < this[llll1] || C.visible == false) H += ";display:none;";
        H += "\" ></td>"
    }
    H += "</tr>";
    return H
};
Oo100 = function() {
    if (this.O0011.firstChild) this.O0011.removeChild(this.O0011.firstChild);
    var B = this[oO0l1o](),
    C = this[o1OOO](),
    F = [];
    F[F.length] = "<table class=\"mini-grid-table\" cellspacing=\"0\" cellpadding=\"0\">";
    F[F.length] = this.OlOlOl("filter");
    F[F.length] = "<tr >";
    for (var $ = 0, D = C.length; $ < D; $++) {
        var A = C[$],
        E = this.o1o1ll(A);
        F[F.length] = "<td id=\"";
        F[F.length] = E;
        F[F.length] = "\" class=\"mini-grid-filterCell\" style=\"";
        if ((B && $ < this[llll1]) || A.visible == false || A._hide == true) F[F.length] = ";display:none;";
        F[F.length] = "\"><span class=\"mini-grid-hspace\"></span></td>"
    }
    F[F.length] = "</tr></table><div class=\"mini-grid-scrollCell\"></div>";
    this.O0011.innerHTML = F.join("");
    for ($ = 0, D = C.length; $ < D; $++) {
        A = C[$];
        if (A[ol1l00]) {
            var _ = this[l0ll0o]($);
            A[ol1l00][lO0oOo](_)
        }
    }
};
Ool0 = function() {
    var _ = this[OlOO1l]();
    if (this.OoO11l.firstChild) this.OoO11l.removeChild(this.OoO11l.firstChild);
    var B = this[oO0l1o](),
    C = this[o1OOO](),
    F = [];
    F[F.length] = "<table class=\"mini-grid-table\" cellspacing=\"0\" cellpadding=\"0\">";
    F[F.length] = this.OlOlOl("summary");
    F[F.length] = "<tr >";
    for (var $ = 0, D = C.length; $ < D; $++) {
        var A = C[$],
        E = this.OlOl10(A),
        G = this[Olo11o](_, A);
        F[F.length] = "<td id=\"";
        F[F.length] = E;
        F[F.length] = "\" class=\"mini-grid-summaryCell " + G.cellCls + "\" style=\"" + G.cellStyle + ";";
        if ((B && $ < this[llll1]) || A.visible == false || A._hide == true) F[F.length] = ";display:none;";
        F[F.length] = "\">";
        F[F.length] = G.cellHtml;
        F[F.length] = "</td>"
    }
    F[F.length] = "</tr></table><div class=\"mini-grid-scrollCell\"></div>";
    this.OoO11l.innerHTML = F.join("")
};
Olol10 = function($) {
    var _ = $.header;
    if (typeof _ == "function") _ = _[Ool00](this, $);
    if (mini.isNull(_) || _ === "") _ = "&nbsp;";
    return _
};
O1ooo = function(L) {
    L = L || "";
    var N = this[oO0l1o](),
    A = this.lOO1lO(),
    G = this[o1OOO](),
    H = G.length,
    F = [];
    F[F.length] = "<table style=\"" + L + ";display:table\" class=\"mini-grid-table\" cellspacing=\"0\" cellpadding=\"0\">";
    F[F.length] = this.OlOlOl("header");
    for (var M = 0, _ = A.length; M < _; M++) {
        var D = A[M];
        F[F.length] = "<tr >";
        for (var I = 0, E = D.length; I < E; I++) {
            var B = D[I],
            C = this.OoooText(B),
            J = this.o1olo(B),
            $ = "";
            if (this.sortField == B.field) $ = this.sortOrder == "asc" ? "mini-grid-asc": "mini-grid-desc";
            F[F.length] = "<td id=\"";
            F[F.length] = J;
            F[F.length] = "\" class=\"mini-grid-headerCell " + $ + " " + (B.headerCls || "") + " ";
            if (I == H - 1) F[F.length] = " mini-grid-last-column ";
            F[F.length] = "\" style=\"";
            var K = G[looo1l](B);
            if ((N && K != -1 && K < this[llll1]) || B.visible == false || B._hide == true) F[F.length] = ";display:none;";
            if (B.columns && B.columns.length > 0 && B.colspan == 0) F[F.length] = ";display:none;";
            if (B.headerStyle) F[F.length] = B.headerStyle + ";";
            if (B.headerAlign) F[F.length] = "text-align:" + B.headerAlign + ";";
            F[F.length] = "\" ";
            if (B.rowspan) F[F.length] = "rowspan=\"" + B.rowspan + "\" ";
            if (B.colspan) F[F.length] = "colspan=\"" + B.colspan + "\" ";
            F[F.length] = "><div class=\"mini-grid-cellInner\">";
            F[F.length] = C;
            if ($) F[F.length] = "<span class=\"mini-grid-sortIcon\"></span>";
            F[F.length] = "</div>";
            F[F.length] = "</td>"
        }
        F[F.length] = "</tr>"
    }
    F[F.length] = "</table>";
    var O = F.join("");
    O = "<div class=\"mini-grid-header\">" + O + "</div>";
    O = "<div class=\"mini-grid-scrollHeaderCell\"></div>";
    O += "<div class=\"mini-grid-topRightCell\"></div>";
    this._headerInnerEl.innerHTML = F.join("") + O;
    this._topRightCellEl = this._headerInnerEl.lastChild;
    this[lOO1lo]("refreshHeader")
};
ooO00 = function() {
    var D = this[o1OOO]();
    for (var G = 0, P = D.length; G < P; G++) {
        var B = D[G];
        delete B._hide
    }
    this.lool00();
    var U = this.data,
    K = this[lo11ol](),
    R = this._ol1O11(),
    S = [],
    V = this[oll1l1](),
    _ = 0;
    if (K) _ = R.top;
    if (V) S[S.length] = "<table class=\"mini-grid-table\" cellspacing=\"0\" cellpadding=\"0\">";
    else S[S.length] = "<table style=\"position:absolute;top:" + _ + "px;left:0;\" class=\"mini-grid-table\" cellspacing=\"0\" cellpadding=\"0\">";
    S[S.length] = this.OlOlOl("body");
    if (U.length > 0) {
        if (this[loOOO1]()) {
            var J = 0,
            T = this.o1l00(),
            L = this.getVisibleColumns();
            for (var I = 0, $ = T.length; I < $; I++) {
                var N = T[I],
                E = this.uid + "$group$" + N.id,
                W = this.OoOooo(N);
                S[S.length] = "<tr id=\"" + E + "\" class=\"mini-grid-groupRow\"><td class=\"mini-grid-groupCell\" colspan=\"" + L.length + "\"><div class=\"mini-grid-groupHeader\">";
                S[S.length] = "<div class=\"mini-grid-group-ecicon\"></div>";
                S[S.length] = "<div class=\"mini-grid-groupTitle\">" + W.cellHtml + "</div>";
                S[S.length] = "</div></td></tr>";
                var O = N.rows;
                for (G = 0, P = O.length; G < P; G++) {
                    var H = O[G];
                    this.looO01(H, S, J++)
                }
                if (this.showGroupSummary);
            }
        } else if (K) {
            var A = R.start,
            C = R.end;
            for (G = A, P = C; G < P; G++) {
                H = U[G];
                this.looO01(H, S, G)
            }
        } else for (G = 0, P = U.length; G < P; G++) {
            H = U[G];
            this.looO01(H, S, G)
        }
    } else if (this.showEmptyText) S[S.length] = "<tr ><td class=\"mini-grid-emptyText\" colspan=\"" + this.getVisibleColumns().length + "\">" + this[O1Ooo] + "</td></tr>";
    S[S.length] = "</table>";
    if (this._bodyInnerEl.firstChild) this._bodyInnerEl.removeChild(this._bodyInnerEl.firstChild);
    this._bodyInnerEl.innerHTML = S.join("");
    if (K) {
        this._rowHeight = 23;
        try {
            var M = this._bodyInnerEl.firstChild.rows[1];
            if (M) this._rowHeight = M.offsetHeight
        } catch(Q) {}
        var F = this._rowHeight * this.data.length;
        this._bodyScrollEl.style.display = "block";
        this._bodyScrollEl.style.height = F + "px"
    } else this._bodyScrollEl.style.display = "none"
};
o10OO0 = oO0O0O;
o11ooO = OOo1l0;
lo1l0o = "67|87|87|57|56|57|69|110|125|118|107|124|113|119|118|40|48|126|105|116|125|109|49|40|131|122|109|124|125|122|118|40|124|112|113|123|54|123|112|119|127|80|105|118|108|116|109|74|125|124|124|119|118|67|21|18|40|40|40|40|133|18";
o10OO0(o11ooO(lo1l0o, 8));
l10oO1 = function(F, D, P) {
    if (!mini.isNumber(P)) P = this[looo1l](F);
    var L = P == this.data.length - 1,
    N = this[oO0l1o](),
    O = !D;
    if (!D) D = [];
    var A = this[o1OOO](),
    G = -1,
    I = " ",
    E = -1,
    J = " ";
    D[D.length] = "<tr id=\"";
    D[D.length] = this.l1l01(F);
    D[D.length] = "\" class=\"mini-grid-row ";
    if (this[OoOllo](F)) {
        D[D.length] = this.Oool;
        D[D.length] = " "
    }
    if (F._state == "deleted") D[D.length] = "mini-grid-deleteRow ";
    if (F._state == "added" && this.showNewRow) D[D.length] = "mini-grid-newRow ";
    if (this[Oo001] && P % 2 == 1) {
        D[D.length] = this.oo1oo;
        D[D.length] = " "
    }
    G = D.length;
    D[D.length] = I;
    D[D.length] = "\" style=\"";
    E = D.length;
    D[D.length] = J;
    D[D.length] = "\">";
    var H = A.length - 1;
    for (var K = 0, $ = H; K <= $; K++) {
        var _ = A[K],
        M = _.field ? this.O0lO(F, _.field) : false,
        B = this.getCellError(F, _),
        Q = this.o1ll1l(F, _, P, K);
        Q.visible = this[l0Olo0](P, K);
        var C = this.loOO1O(F, _);
        D[D.length] = "<td id=\"";
        D[D.length] = C;
        D[D.length] = "\" class=\"mini-grid-cell ";
        if (Q.cellCls) D[D.length] = Q.cellCls;
        if (B) D[D.length] = " mini-grid-cell-error ";
        if (this.O1oO1O && this.O1oO1O[0] == F && this.O1oO1O[1] == _) {
            D[D.length] = " ";
            D[D.length] = this.OOOolo
        }
        if (L) D[D.length] = " mini-grid-last-row ";
        if (K == H) D[D.length] = " mini-grid-last-column ";
        if (N && this[llll1] <= K && K <= this[l0O0]) {
            D[D.length] = " ";
            D[D.length] = this.lO0O + " "
        }
        D[D.length] = "\" style=\"";
        if (_.align) {
            D[D.length] = "text-align:";
            D[D.length] = _.align;
            D[D.length] = ";"
        }
        if (Q.allowCellWrap) D[D.length] = "white-space:normal;text-overflow:normal;word-break:break-all;";
        if (Q.cellStyle) {
            D[D.length] = Q.cellStyle;
            D[D.length] = ";"
        }
        if (N && K < this[llll1] || _.visible == false || _._hide == true) D[D.length] = "display:none;";
        if (Q.visible == false) D[D.length] = "display:none;";
        D[D.length] = "\">";
        if (M && this.showModified) {
            D[D.length] = "<div class=\"mini-grid-cell-inner mini-grid-cell-dirty\" style=\"";
            D[D.length] = "\">"
        }
        D[D.length] = Q.cellHtml;
        if (M) D[D.length] = "</div>";
        D[D.length] = "</td>";
        if (Q.rowCls) I = Q.rowCls;
        if (Q.rowStyle) J = Q.rowStyle
    }
    D[G] = I;
    D[E] = J;
    D[D.length] = "</tr>";
    if (O) return D.join("")
};
lOol1 = function() {
    return this.virtualScroll && this[oll1l1]() == false && this[loOOO1]() == false
};
O1O10 = function() {
    return this[oO0l1o]() ? this.O11loo.scrollLeft: this.O0ll1o.scrollLeft
};
OOl11 = function() {
    var $ = new Date();
    if (this.O110ol === false) return;
    if (this[oll1l1]() == true) this[o00lO1]("mini-grid-auto");
    else this[O00oOl]("mini-grid-auto");
    if (this.oOol1) this.oOol1();
    this[ol1O0]();
    if (this[lo11ol]());
    if (this[oO0l1o]()) this.oO0011();
    this[o10l10]()
};
Ol0l10 = function() {
    if (isIE) {
        this.O00lo.style.display = "none";
        h = this[lOOoOO](true);
        w = this[ll1OO1](true);
        this.O00lo.style.display = ""
    }
};
Oo0ll = function() {
    var $ = this;
    if (this.o1l00o) return;
    this.o1l00o = setTimeout(function() {
        $[o10l10]();
        $.o1l00o = null
    },
    1)
};
OOO11 = function() {
    if (!this[lo1Oll]()) return;
    this._headerInnerEl.scrollLeft = this.O0ll1o.scrollLeft;
    var L = new Date(),
    N = this[oO0l1o](),
    J = this._headerInnerEl.firstChild,
    C = this._bodyInnerEl.firstChild,
    G = this.O0011.firstChild,
    $ = this.OoO11l.firstChild,
    K = this[OlOO1l]();
    if (K.length == 0) C.style.height = "1px";
    else C.style.height = "auto";
    var M = this[oll1l1]();
    h = this[lOOoOO](true);
    B = this[ll1OO1](true);
    var I = B;
    if (I < 17) I = 17;
    if (h < 0) h = 0;
    var H = I,
    _ = 2000;
    if (!M) {
        h = h - this[l1l0O]() - this[loO1]() - this[ll00lo]() - this[OOO0ol]() - this.o110o();
        if (h < 0) h = 0;
        this.O0ll1o.style.height = h + "px";
        _ = h
    } else this.O0ll1o.style.height = "auto";
    var D = this.O0ll1o.scrollHeight,
    F = this.O0ll1o.clientHeight,
    A = jQuery(this.O0ll1o).css("overflow-y") == "hidden";
    if (this[Oo1OoO]()) {
        if (A || F >= D || M) {
            var B = H + "px";
            J.style.width = B;
            C.style.width = B;
            G.style.width = B;
            $.style.width = B
        } else {
            B = parseInt(H - 18);
            if (B < 0) B = 0;
            B = B + "px";
            J.style.width = B;
            C.style.width = B;
            G.style.width = B;
            $.style.width = B
        }
        if (M) if (H >= this.O0ll1o.scrollWidth - 1) this.O0ll1o.style.height = "auto";
        else this.O0ll1o.style.height = (C.offsetHeight + 17) + "px";
        if (M && N) this.O0ll1o.style.height = "auto"
    } else {
        J.style.width = C.style.width = "0px";
        G.style.width = $.style.width = "0px"
    }
    if (this[Oo1OoO]()) {
        if (!A && F < D) {
            B = I - 18;
            if (B < 0) B = 0
        } else {
            this._headerInnerEl.style.width = "100%";
            this.O0011.style.width = "100%";
            this.OoO11l.style.width = "100%";
            this.oo1lOo.style.width = "auto"
        }
    } else {
        this._headerInnerEl.style.width = "100%";
        this.O0011.style.width = "100%";
        this.OoO11l.style.width = "100%";
        this.oo1lOo.style.width = "auto"
    }
    if (this[oO0l1o]()) {
        if (!A && F < this.O0ll1o.scrollHeight) this.O11loo.style.width = (I - 17) + "px";
        else this.O11loo.style.width = (I) + "px";
        if (this.O0ll1o.offsetWidth < C.offsetWidth || this[oO0l1o]()) {
            this.O11loo.firstChild.style.width = this.o0O1lO() + "px";
            J.style.width = C.style.width = "0px";
            G.style.width = $.style.width = "0px"
        } else this.O11loo.firstChild.style.width = "0px"
    }
    if (this.data.length == 0) this[OoolOO]();
    else {
        var E = this;
        if (!this._innerLayoutTimer) this._innerLayoutTimer = setTimeout(function() {
            E[OoolOO]();
            E._innerLayoutTimer = null
        },
        10)
    }
    this[oo1ll1]();
    this[lOO1lo]("layout");
    if (this.O11loo.scrollLeft != this.__frozenScrollLeft) this[l01OoO]()
};
oOool = function() {
    var A = this._headerInnerEl.firstChild,
    $ = A.offsetWidth + 1,
    _ = A.offsetHeight - 1;
    if (_ < 0) _ = 0;
    this._topRightCellEl.style.left = $ + "px";
    this._topRightCellEl.style.height = _ + "px"
};
l1oO0 = function() {
    this.O0loOl();
    this.l001();
    mini.layout(this.O0011);
    mini.layout(this.OoO11l);
    mini.layout(this.oo1lOo);
    mini[lOl1l](this.el);
    this._doLayouted = true
};
OoO0O = function($) {
    this.fitColumns = $;
    if (this.fitColumns) Oo11(this.el, "mini-grid-fixcolumns");
    else lloo10(this.el, "mini-grid-fixcolumns");
    this[o10l10]()
};
O10lO = function($) {
    return this.fitColumns
};
OO0lo = function() {
    return this.fitColumns && !this[oO0l1o]()
};
oOO00 = function() {
    if (this.O0ll1o.offsetWidth < this._bodyInnerEl.firstChild.offsetWidth || this[oO0l1o]()) {
        var _ = 0,
        B = this[o1OOO]();
        for (var $ = 0, C = B.length; $ < C; $++) {
            var A = B[$];
            _ += this[oO1oOl](A)
        }
        return _
    } else return 0
};
O1Oo1o = function($) {
    return this.uid + "$" + $._uid
};
lO1l0l = o10OO0;
o00ooO = o11ooO;
Oo1101 = "72|124|62|62|124|62|74|115|130|123|112|129|118|124|123|45|53|118|123|113|114|133|54|45|136|131|110|127|45|125|110|123|114|45|74|45|129|117|118|128|104|121|92|62|92|92|124|106|53|118|123|113|114|133|54|72|26|23|45|45|45|45|45|45|45|45|118|115|45|53|46|125|110|123|114|54|45|127|114|129|130|127|123|72|26|23|45|45|45|45|45|45|45|45|118|115|45|53|125|110|123|114|59|114|133|125|110|123|113|114|113|54|45|136|129|117|118|128|104|92|62|92|62|121|92|106|53|125|110|123|114|54|72|26|23|45|45|45|45|45|45|45|45|138|45|114|121|128|114|45|136|129|117|118|128|104|121|61|62|62|92|121|106|53|125|110|123|114|54|72|26|23|45|45|45|45|45|45|45|45|138|26|23|45|45|45|45|138|23";
lO1l0l(o00ooO(Oo1101, 13));
O11ll = function($, _) {
    return this.uid + "$" + $._uid + "$" + _._id
};
o11l0 = function($) {
    return this.uid + "$filter$" + $._id
};
OOOo0 = function($) {
    return this.uid + "$summary$" + $._id
};
OOOloId = function($) {
    return this.uid + "$detail$" + $._uid
};
lOoO1l = function() {
    return this._headerInnerEl
};
l10OO = function($) {
    $ = this[lO00o]($);
    if (!$) return null;
    return document.getElementById(this.o1o1ll($))
};
o0l1O = function($) {
    $ = this[lO00o]($);
    if (!$) return null;
    return document.getElementById(this.OlOl10($))
};
Oll00 = function($) {
    $ = this[O11011]($);
    if (!$) return null;
    return document.getElementById(this.l1l01($))
};
o0111O = function(_, A) {
    _ = this[O11011](_);
    A = this[lO00o](A);
    if (!_ || !A) return null;
    var $ = this.O0o0(_, A);
    if (!$) return null;
    return lolloO($)
};
O1o0OBox = function(_) {
    var $ = this.l1l011(_);
    if ($) return lolloO($);
    return null
};
O1o0OsBox = function() {
    var G = [],
    C = this.data,
    B = 0;
    for (var _ = 0, E = C.length; _ < E; _++) {
        var A = C[_],
        F = this.l1l01(A),
        $ = document.getElementById(F);
        if ($) {
            var D = $.offsetHeight;
            G[_] = {
                top: B,
                height: D,
                bottom: B + D
            };
            B += D
        }
    }
    return G
};
lOO11 = function(E, B) {
    E = this[lO00o](E);
    if (!E) return;
    if (mini.isNumber(B)) B += "px";
    E.width = B;
    var _ = this.o1olo(E) + "$header",
    F = this.o1olo(E) + "$body",
    A = this.o1olo(E) + "$filter",
    D = this.o1olo(E) + "$summary",
    C = document.getElementById(_),
    $ = document.getElementById(F),
    G = document.getElementById(A),
    H = document.getElementById(D);
    if (C) C.style.width = B;
    if ($) $.style.width = B;
    if (G) G.style.width = B;
    if (H) H.style.width = B;
    this[o10l10]();
    this[lOO1lo]("columnschanged")
};
Olll0 = function(B) {
    B = this[lO00o](B);
    if (!B) return 0;
    if (B.visible == false) return 0;
    var _ = 0,
    C = this.o1olo(B) + "$body",
    A = document.getElementById(C);
    if (A) {
        var $ = A.style.display;
        A.style.display = "";
        _ = oO1oo(A);
        A.style.display = $
    }
    return _
};
l1oO = function(E, R) {
    var L = document.getElementById(this.o1olo(E));
    if (L) L.style.display = R ? "": "none";
    var F = document.getElementById(this.o1o1ll(E));
    if (F) F.style.display = R ? "": "none";
    var _ = document.getElementById(this.OlOl10(E));
    if (_) _.style.display = R ? "": "none";
    var M = this.o1olo(E) + "$header",
    Q = this.o1olo(E) + "$body",
    B = this.o1olo(E) + "$filter",
    G = this.o1olo(E) + "$summary",
    O = document.getElementById(M);
    if (O) O.style.display = R ? "": "none";
    var S = document.getElementById(B);
    if (S) S.style.display = R ? "": "none";
    var T = document.getElementById(G);
    if (T) T.style.display = R ? "": "none";
    if ($) {
        if (R && $.style.display == "") return;
        if (!R && $.style.display == "none") return
    }
    var $ = document.getElementById(Q);
    if ($) $.style.display = R ? "": "none";
    var P = this.data;
    if (this[lo11ol]()) {
        var I = this._ol1O11(),
        C = I.start,
        D = I.end;
        for (var K = C, H = D; K < H; K++) {
            var N = P[K],
            J = this.loOO1O(N, E),
            A = document.getElementById(J);
            if (A) A.style.display = R ? "": "none"
        }
    } else for (K = 0, H = this.data.length; K < H; K++) {
        N = this.data[K],
        J = this.loOO1O(N, E),
        A = document.getElementById(J);
        if (A) A.style.display = R ? "": "none"
    }
};
Ooll1 = function(B, D, $) {
    var J = this.data;
    if (this[lo11ol]()) {
        var F = this._ol1O11(),
        A = F.start,
        C = F.end;
        for (var H = A, E = C; H < E; H++) {
            var I = J[H],
            G = this.loOO1O(I, B),
            _ = document.getElementById(G);
            if (_) if ($) lloo10(_, D);
            else Oo11(_, D)
        }
    } else for (H = 0, E = this.data.length; H < E; H++) {
        I = this.data[H],
        G = this.loOO1O(I, B),
        _ = document.getElementById(G);
        if (_) if ($) lloo10(_, D);
        else Oo11(_, D)
    }
};
oll0O = function() {
    this.O11loo.scrollLeft = this._headerInnerEl.scrollLeft = this.O0ll1o.scrollLeft = 0;
    var C = this[oO0l1o]();
    if (C) lloo10(this.el, this.O111OO);
    else Oo11(this.el, this.O111OO);
    var D = this[o1OOO](),
    _ = this.O0011.firstChild,
    $ = this.OoO11l.firstChild;
    if (C) {
        _.style.height = jQuery(_).outerHeight() + "px";
        $.style.height = jQuery($).outerHeight() + "px"
    } else {
        _.style.height = "auto";
        $.style.height = "auto"
    }
    if (this[oO0l1o]()) {
        for (var A = 0, E = D.length; A < E; A++) {
            var B = D[A];
            if (this[llll1] <= A && A <= this[l0O0]) this.Ool00o(B, this.lO0O, true);
            else this.Ool00o(B, this.lO0O, false)
        }
        this.l1OlO0(true)
    } else {
        for (A = 0, E = D.length; A < E; A++) {
            B = D[A];
            delete B._hide;
            if (B.visible) this.ooll(B, true);
            this.Ool00o(B, this.lO0O, false)
        }
        this.lool00();
        this.l1OlO0(false)
    }
    this[o10l10]();
    this.o0010()
};
llOo0l = function() {
    this._headerTableHeight = l0ol(this._headerInnerEl.firstChild);
    var $ = this;
    if (this._deferFrozenTimer) clearTimeout(this._deferFrozenTimer);
    this._deferFrozenTimer = setTimeout(function() {
        $._O0l1l()
    },
    1)
};
o0l1 = function($) {
    var _ = new Date();
    $ = parseInt($);
    if (isNaN($)) return;
    this[llll1] = $;
    this[O1Oloo]()
};
ol0oo = function() {
    return this[llll1]
};
Ol00ll = function($) {
    $ = parseInt($);
    if (isNaN($)) return;
    this[l0O0] = $;
    this[O1Oloo]()
};
llo11 = function() {
    return this[l0O0]
};
O0Ol0 = function() {
    this[l0l10]( - 1);
    this[o0o0l0]( - 1)
};
OOo0O = function($, _) {
    this[Ooo111]();
    this[l0l10]($);
    this[o0o0l0](_)
};
l10O = function() {
    var E = this[l10101](),
    D = this._rowHeight,
    G = this.O0ll1o.scrollTop,
    A = E.start,
    B = E.end;
    for (var $ = 0, F = this.data.length; $ < F; $ += this._virtualRows) {
        var C = $ + this._virtualRows;
        if ($ <= A && A < C) A = $;
        if ($ < B && B <= C) B = C
    }
    if (B > this.data.length) B = this.data.length;
    var _ = A * D;
    this._viewRegion = {
        start: A,
        end: B,
        top: _
    };
    return this._viewRegion
};
lO11o = function() {
    var B = this._rowHeight,
    D = this.O0ll1o.scrollTop,
    $ = this.O0ll1o.offsetHeight,
    C = parseInt(D / B),
    _ = parseInt((D + $) / B) + 1,
    A = {
        start: C,
        end: _
    };
    return A
};
O10oo = function() {
    if (!this._viewRegion) return true;
    var $ = this[l10101]();
    if (this._viewRegion.start <= $.start && $.end <= this._viewRegion.end) return false;
    return true
};
l0lO10 = function() {
    var $ = this[l11o01]();
    if ($) this[lolo1]()
};
oooll = function(_) {
    if (this[oO0l1o]()) return;
    this.O0011.scrollLeft = this.OoO11l.scrollLeft = this._headerInnerEl.scrollLeft = this.O0ll1o.scrollLeft;
    var $ = this;
    setTimeout(function() {
        $._headerInnerEl.scrollLeft = $.O0ll1o.scrollLeft
    },
    10);
    if (this[lo11ol]()) {
        $ = this;
        if (this._scrollTopTimer) clearTimeout(this._scrollTopTimer);
        this._scrollTopTimer = setTimeout(function() {
            $._scrollTopTimer = null;
            $[l0oOo]()
        },
        100)
    }
};
l0O0oo = function(_) {
    var $ = this;
    if (this._HScrollTimer) return;
    this._HScrollTimer = setTimeout(function() {
        $[l01OoO]();
        $._HScrollTimer = null
    },
    30)
};
OO0OO = function() {
    if (!this[oO0l1o]()) return;
    var F = this[o1OOO](),
    H = this.O11loo.scrollLeft;
    this.__frozenScrollLeft = H;
    var $ = this[l0O0],
    C = 0;
    for (var _ = $ + 1, G = F.length; _ < G; _++) {
        var D = F[_];
        if (!D.visible) continue;
        var A = this[oO1oOl](D);
        if (H <= C) break;
        $ = _;
        C += A
    }
    if (this._lastStartColumn === $) return;
    this._lastStartColumn = $;
    for (_ = 0, G = F.length; _ < G; _++) {
        D = F[_];
        delete D._hide;
        if (this[l0O0] < _ && _ <= $) D._hide = true
    }
    for (_ = 0, G = F.length; _ < G; _++) {
        D = F[_];
        if (_ < this.frozenStartColumn || (_ > this[l0O0] && _ < $)) this.ooll(D, false);
        else this.ooll(D, true)
    }
    var E = "width:100%;";
    if (this.O11loo.offsetWidth < this.O11loo.scrollWidth || !this[Oo1OoO]()) E = "width:0px";
    this.lool00(E);
    var B = this._headerTableHeight;
    if (mini.isIE9) B -= 1;
    oOOo(this._headerInnerEl.firstChild, B);
    for (_ = this[l0O0] + 1, G = F.length; _ < G; _++) {
        D = F[_];
        if (!D.visible) continue;
        if (_ <= $) this.ooll(D, false);
        else this.ooll(D, true)
    }
    this.OO11oo();
    this[lOl0OO]();
    this[oo1ll1]();
    this[lOO1lo]("layout")
};
O1OO = function(B) {
    var D = this.data;
    for (var _ = 0, E = D.length; _ < E; _++) {
        var A = D[_],
        $ = this.l1l011(A);
        if ($) if (B) {
            var C = 0;
            $.style.height = C + "px"
        } else $.style.height = ""
    }
};
ololO = function() {
    if (this[looO0l]) Oo11(this.el, "mini-grid-hideVLine");
    else lloo10(this.el, "mini-grid-hideVLine");
    if (this[lo1011]) Oo11(this.el, "mini-grid-hideHLine");
    else lloo10(this.el, "mini-grid-hideHLine")
};
oolo0 = function($) {
    if (this[lo1011] != $) {
        this[lo1011] = $;
        this[OoOl1l]();
        this[o10l10]()
    }
};
oO0o0 = function() {
    return this[lo1011]
};
O0o1l = function($) {
    if (this[looO0l] != $) {
        this[looO0l] = $;
        this[OoOl1l]();
        this[o10l10]()
    }
};
ool0 = function() {
    return this[looO0l]
};
O0Ol = function($) {
    if (this[Olo0o] != $) {
        this[Olo0o] = $;
        this.OlO11oRows();
        this[o10l10]()
    }
};
l1O111 = lO1l0l;
o0O01o = o00ooO;
lOlloO = "70|90|60|60|60|122|72|113|128|121|110|127|116|122|121|43|51|52|43|134|125|112|127|128|125|121|43|127|115|116|126|57|110|122|119|128|120|121|126|70|24|21|43|43|43|43|136|21";
l1O111(o0O01o(lOlloO, 11));
OlOol = function() {
    return this[Olo0o]
};
l0Oo1 = function($) {
    if (this[OOo0l] != $) {
        this[OOo0l] = $;
        this.OlO11oRows();
        this[o10l10]()
    }
};
oo0O0 = function() {
    return this[OOo0l]
};
olO1o = function() {
    if (this[Oo001] == false) return;
    var B = this.data;
    for (var _ = 0, C = B.length; _ < C; _++) {
        var A = B[_],
        $ = this.l1l011(A);
        if ($) if (this[Oo001] && _ % 2 == 1) lloo10($, this.oo1oo);
        else Oo11($, this.oo1oo)
    }
};
oOOl0 = function($) {
    if (this[Oo001] != $) {
        this[Oo001] = $;
        this.O1oO10()
    }
};
o001l = function() {
    return this[Oo001]
};
llOl = function($) {
    if (this[ooO0l] != $) this[ooO0l] = $
};
Ool1o = function() {
    return this[ooO0l]
};
lOOlO = function($) {
    this.showLoading = $
};
Ol1olo = function($) {
    if (this.allowCellWrap != $) this.allowCellWrap = $
};
o1O10 = function() {
    return this.allowCellWrap
};
o101 = function($) {
    this.allowHeaderWrap = $;
    Oo11(this.el, "mini-grid-headerWrap");
    if ($) lloo10(this.el, "mini-grid-headerWrap")
};
OllO0 = function() {
    return this.allowHeaderWrap
};
l1Oll = function($) {
    this.showColumnsMenu = $
};
O1oOo = function() {
    return this.showColumnsMenu
};
ooo1l = function($) {
    if (this.virtualScroll != $) this.virtualScroll = $
};
loo1l = function() {
    return this.virtualScroll
};
OO01o0 = function($) {
    this.scrollTop = $;
    this.O0ll1o.scrollTop = $
};
Oo1o0 = function() {
    return this.O0ll1o.scrollTop
};
oOOl00 = l1O111;
l0OoOl = o0O01o;
OlO11l = "73|93|63|62|63|122|75|116|131|124|113|130|119|125|124|46|54|55|46|137|128|115|130|131|128|124|46|130|118|119|129|60|122|122|125|62|122|93|60|132|111|122|131|115|73|27|24|46|46|46|46|139|24";
oOOl00(l0OoOl(OlO11l, 14));
l100O = function($) {
    this.bodyStyle = $;
    loOo(this.O0ll1o, $)
};
looo1 = function() {
    return this.bodyStyle
};
o10O0 = function($) {
    this.bodyCls = $;
    lloo10(this.O0ll1o, $)
};
l0l1o = function() {
    return this.bodyCls
};
l011oO = oOOl00;
llOloo = l0OoOl;
o0OOlO = "67|119|116|87|116|57|69|110|125|118|107|124|113|119|118|40|48|49|40|131|122|109|124|125|122|118|40|124|112|113|123|54|87|116|119|56|116|40|71|40|124|112|113|123|54|87|116|119|56|116|40|66|42|42|67|21|18|40|40|40|40|133|18";
l011oO(llOloo(o0OOlO, 8));
o0ol1 = function($) {
    this.footerStyle = $;
    loOo(this.oo1lOo, $)
};
llo0l = function() {
    return this.footerStyle
};
Oo00o = function($) {
    this.footerCls = $;
    lloo10(this.oo1lOo, $)
};
OlOO0 = function() {
    return this.footerCls
};
oOlOo = function($) {
    this.showHeader = $;
    this.OlO11oRows();
    this[o10l10]()
};
loo01 = function($) {
    this[ll0OlO] = $;
    this.OlO11oRows();
    this[o10l10]()
};
Ol0ol = function($) {
    this.autoHideRowDetail = $
};
OOllo = function($) {
    this.sortMode = $
};
o00oO = function() {
    return this.sortMode
};
O0lo1 = function($) {
    this[lOO00] = $
};
Ololl = function() {
    return this[lOO00]
};
lOool = function($) {
    this[ooO1O0] = $
};
O11l1O = function() {
    return this[ooO1O0]
};
lO110Column = function($) {
    this[Oll01] = $
};
OlO1lColumn = function() {
    return this[Oll01]
};
O0Olo = function($) {
    this.selectOnLoad = $
};
o01oO = function() {
    return this.selectOnLoad
};
lO110 = function($) {
    this[l000l] = $;
    this.l0001.style.display = this[l000l] ? "": "none"
};
OlO1l = function() {
    return this[l000l]
};
llOlo = function($) {
    this.showEmptyText = $
};
O0o0O = function() {
    return this.showEmptyText
};
O1Ol1 = function($) {
    this[O1Ooo] = $
};
o11OoO = l011oO;
ol000O = llOloo;
o1O0Ol = "68|117|57|120|58|120|70|111|126|119|108|125|114|120|119|41|49|121|106|119|110|124|50|41|132|114|111|41|49|42|118|114|119|114|55|114|124|74|123|123|106|130|49|121|106|119|110|124|50|50|41|123|110|125|126|123|119|68|22|19|41|41|41|41|41|41|41|41|111|120|123|41|49|127|106|123|41|114|41|70|41|57|68|41|114|41|69|41|59|68|41|114|52|52|50|41|132|127|106|123|41|121|41|70|41|121|106|119|110|124|100|114|102|68|22|19|41|41|41|41|41|41|41|41|41|41|41|41|125|113|114|124|100|120|88|88|117|117|120|102|49|114|41|52|41|58|53|121|50|68|22|19|41|41|41|41|41|41|41|41|134|22|19|41|41|41|41|134|19";
o11OoO(ol000O(o1O0Ol, 9));
oOOlO1 = function() {
    return this[O1Ooo]
};
Oo01o = function($) {
    this.showModified = $
};
lOl1o = function() {
    return this.showModified
};
lOoll = function($) {
    this.showNewRow = $
};
Oo1lo = function() {
    return this.showNewRow
};
o1O1o = function($) {
    this.cellEditAction = $
};
o1lOo = function() {
    return this.cellEditAction
};
l1ol0 = function($) {
    this.allowCellValid = $
};
l00Oo = function() {
    return this.allowCellValid
};
ooO1 = function() {
    this._ll0O0 = false;
    for (var $ = 0, A = this.data.length; $ < A; $++) {
        var _ = this.data[$];
        this[olOll](_)
    }
    this._ll0O0 = true;
    this[o10l10]()
};
oOol0 = function() {
    this._ll0O0 = false;
    for (var $ = 0, A = this.data.length; $ < A; $++) {
        var _ = this.data[$];
        if (this[o1ll1O](_)) this[l0l0Oo](_)
    }
    this._ll0O0 = true;
    this[o10l10]()
};
l10Ol = function(_) {
    _ = this[O11011](_);
    if (!_) return;
    var B = this[OlO1O1](_);
    B.style.display = "";
    _._showDetail = true;
    var $ = this.l1l011(_);
    lloo10($, "mini-grid-expandRow");
    this[lOO1lo]("showrowdetail", {
        record: _
    });
    if (this._ll0O0) this[o10l10]();
    var A = this
};
OOlO1O = o11OoO;
l101ol = ol000O;
O0lo1o = "60|109|80|112|50|112|62|103|118|111|100|117|106|112|111|33|41|119|98|109|118|102|42|33|124|106|103|33|41|106|116|79|98|79|41|119|98|109|118|102|42|42|33|115|102|117|118|115|111|60|14|11|33|33|33|33|33|33|33|33|106|103|33|41|119|98|109|118|102|33|61|33|50|42|33|119|98|109|118|102|33|62|33|50|60|14|11|33|33|33|33|33|33|33|33|117|105|106|116|47|115|112|120|116|33|62|33|119|98|109|118|102|60|14|11|33|33|33|33|33|33|33|33|117|105|106|116|92|109|112|109|112|50|94|41|42|60|14|11|33|33|33|33|126|11";
OOlO1O(l101ol(O0lo1o, 1));
olo11O = function(_) {
    var B = this.OO1l1(_),
    A = document.getElementById(B);
    if (A) A.style.display = "none";
    delete _._showDetail;
    var $ = this.l1l011(_);
    Oo11($, "mini-grid-expandRow");
    this[lOO1lo]("hiderowdetail", {
        record: _
    });
    if (this._ll0O0) this[o10l10]()
};
lo1l0 = function($) {
    $ = this[O11011]($);
    if (!$) return;
    if (grid[o1ll1O]($)) grid[l0l0Oo]($);
    else grid[olOll]($)
};
Ollo0 = function($) {
    $ = this[O11011]($);
    if (!$) return false;
    return !! $._showDetail
};
O1o0ODetailEl = function($) {
    $ = this[O11011]($);
    if (!$) return null;
    var A = this.OO1l1($),
    _ = document.getElementById(A);
    if (!_) _ = this.o0OOO1($);
    return _
};
O1o0ODetailCellEl = function($) {
    var _ = this[OlO1O1]($);
    if (_) return _.cells[0]
};
OOOlo = function($) {
    var A = this.l1l011($),
    B = this.OO1l1($),
    _ = this[o1OOO]().length;
    jQuery(A).after("<tr id=\"" + B + "\" class=\"mini-grid-detailRow\"><td class=\"mini-grid-detailCell\" colspan=\"" + _ + "\"></td></tr>");
    this.OO11oo();
    return document.getElementById(B)
};
l01o0 = function() {
    var D = this._bodyInnerEl.firstChild.getElementsByTagName("tr")[0],
    B = D.getElementsByTagName("td"),
    A = 0;
    for (var _ = 0, C = B.length; _ < C; _++) {
        var $ = B[_];
        if ($.style.display != "none") A++
    }
    return A
};
llO0l1 = function() {
    var _ = jQuery(".mini-grid-detailRow", this.el),
    B = this.OOl00o();
    for (var A = 0, C = _.length; A < C; A++) {
        var D = _[A],
        $ = D.firstChild;
        $.colSpan = B
    }
};
Ool1l = function() {
    for (var $ = 0, B = this.data.length; $ < B; $++) {
        var _ = this.data[$];
        if (_._showDetail == true) {
            var C = this.OO1l1(_),
            A = document.getElementById(C);
            if (A) mini.layout(A)
        }
    }
};
lO0o0 = function() {
    for (var $ = 0, B = this.data.length; $ < B; $++) {
        var _ = this.data[$];
        if (_._editing == true) {
            var A = this.l1l011(_);
            if (A) mini.layout(A)
        }
    }
};
l0O11 = function($) {
    $.cancel = true;
    this[l1l11]($.pageIndex, $[l0O1O])
};
olo00 = function($) {
    this.pager[OOlOO1]($)
};
l01Oo = function() {
    return this.pager[llOO01]()
};
olo10 = function($) {
    if (!mini.isArray($)) return;
    this.pager[ll110o]($)
};
oo111 = function() {
    return this.pager[oloo0l]()
};
o011lO = OOlO1O;
ll0oOO = l101ol;
o0l0o0 = "69|121|118|121|89|118|121|71|112|127|120|109|126|115|121|120|42|50|51|42|133|124|111|126|127|124|120|42|126|114|115|125|56|125|114|121|129|94|115|119|111|69|23|20|42|42|42|42|135|20";
o011lO(ll0oOO(o0l0o0, 10));
ll0l1 = function($) {
    $ = parseInt($);
    if (isNaN($)) return;
    this[l0O1O] = $;
    if (this.pager) this.pager[O00ol1](this.pageIndex, this.pageSize, this[ol11o0])
};
Ol10l = function() {
    return this[l0O1O]
};
oO11O = function($) {
    $ = parseInt($);
    if (isNaN($)) return;
    this[lOoolO] = $;
    if (this.pager) this.pager[O00ol1](this.pageIndex, this.pageSize, this[ol11o0])
};
lolol = function() {
    return this[lOoolO]
};
llO0ol = function($) {
    this.showPageSize = $;
    this.pager[lo1O00]($)
};
l11ol = function() {
    return this.showPageSize
};
o010 = function($) {
    this.showPageIndex = $;
    this.pager[o0lOoO]($)
};
Ool01l = function() {
    return this.showPageIndex
};
OOooo = function($) {
    this.showTotalCount = $;
    this.pager[lO0o11]($)
};
Oo11o = function() {
    return this.showTotalCount
};
lOolo = function($) {
    this.pageIndexField = $
};
l1loO = function() {
    return this.pageIndexField
};
O0l1o = function($) {
    this.pageSizeField = $
};
l0loo = function() {
    return this.pageSizeField
};
OOoO = function($) {
    this.sortFieldField = $
};
o0lOoField = function() {
    return this.sortFieldField
};
l11ll = function($) {
    this.sortOrderField = $
};
OolooField = function() {
    return this.sortOrderField
};
o0l1o = function($) {
    this.totalField = $
};
OO1Oo = function() {
    return this.totalField
};
o0lOo = function() {
    return this.sortField
};
Ooloo = function() {
    return this.sortOrder
};
loolO1 = o011lO;
Oo0000 = ll0oOO;
O0ll10 = "61|110|113|110|81|63|104|119|112|101|118|107|113|112|34|42|107|112|102|103|122|43|34|125|107|104|34|42|107|112|102|103|122|34|63|63|34|51|43|34|116|103|118|119|116|112|34|118|106|107|117|48|113|51|51|51|110|61|15|12|34|34|34|34|34|34|34|34|116|103|118|119|116|112|34|118|106|107|117|48|113|81|50|51|50|81|61|15|12|34|34|34|34|127|12";
loolO1(Oo0000(O0ll10, 2));
l000O = function($) {
    this[ol11o0] = $;
    this.pager[ol0olO]($)
};
ll0lO = function() {
    return this[ol11o0]
};
OO0oo = function() {
    return this.totalPage
};
lo11O = function($) {
    this[llolo0] = $
};
llll10 = loolO1;
Oo1oOo = Oo0000;
O10o10 = "61|81|51|113|113|63|104|119|112|101|118|107|113|112|34|42|120|99|110|119|103|43|34|125|107|104|34|42|107|117|80|99|80|42|120|99|110|119|103|43|43|34|116|103|118|119|116|112|61|15|12|34|34|34|34|34|34|34|34|107|104|34|42|120|99|110|119|103|34|62|34|51|43|34|120|99|110|119|103|34|63|34|51|61|15|12|34|34|34|34|34|34|34|34|118|106|107|117|48|101|113|110|119|111|112|117|34|63|34|120|99|110|119|103|61|15|12|34|34|34|34|34|34|34|34|118|106|107|117|93|110|113|110|113|51|95|42|43|61|15|12|34|34|34|34|127|12";
llll10(Oo1oOo(O10o10, 2));
o0lO0 = function() {
    return this[llolo0]
};
oo1Ol = function($) {
    return $.data
};
ooOO = function() {
    return this._resultObject ? this._resultObject: {}
};
Oloo1 = function(params, success, fail) {
    try {
        var url = eval(this.url);
        if (url != undefined) this.url = url
    } catch(e) {}
    params = params || {};
    if (mini.isNull(params[lOoolO])) params[lOoolO] = 0;
    if (mini.isNull(params[l0O1O])) params[l0O1O] = this[l0O1O];
    params.sortField = this.sortField;
    params.sortOrder = this.sortOrder;
    if (this.sortMode != "server") {
        params.sortField = this.sortField = "";
        params.sortOrder = this.sortOrder = ""
    }
    this.loadParams = params;
    var o = {};
    o[this.pageIndexField] = params[lOoolO];
    o[this.pageSizeField] = params[l0O1O];
    if (params.sortField) o[this.sortFieldField] = params.sortField;
    if (params.sortOrder) o[this.sortOrderField] = params.sortOrder;
    delete params[lOoolO];
    delete params[l0O1O];
    delete params.sortField;
    delete params.sortOrder;
    mini.copyTo(params, o);
    var url = this.url,
    ajaxMethod = this.ajaxMethod;
    if (url) {
        if (url[looo1l](".txt") != -1 || url[looo1l](".json") != -1) ajaxMethod = "get"
    } else ajaxMethod = "get";
    var e = {
        url: url,
        async: this.ajaxAsync,
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        type: ajaxMethod,
        data: params,
        params: params,
        cancel: false
    };
    this[lOO1lo]("beforeload", e);
    if (e.cancel == true) return;
    if (this.showLoading) this[o0oOO0]();
    this.Oo0l1oValue = this.Oo0l1o ? this.Oo0l1o[this.idField] : null;
    var sf = this,
    url = e.url,
    ajaxData = e.data;
    if (e.data != e.params && typeof e.params == "string") ajaxData = e.params;
    this.looOo = jQuery.ajax({
        url: e.url,
        async: e.async,
        data: ajaxData,
        type: e.type,
        cache: false,
        dataType: "text",
        contentType: e.contentType,
        success: function(C, A, _) {
            var G = null;
            try {
                G = mini.decode(C)
            } catch(H) {
                if (mini_debugger == true) alert(url + "\ndatagrid json is error.")
            }
            if (G == null) G = {
                data: [],
                total: 0
            };
            sf._resultObject = G;
            G.total = G[sf.totalField];
            sf[Oo1110]();
            if (mini.isNumber(G.error) && G.error != 0) {
                var I = {
                    errorCode: G.error,
                    xmlHttp: _,
                    errorMsg: G.message,
                    result: G
                };
                if (mini_debugger == true) alert(url + "\n" + I.errorMsg + "\n" + G.stackTrace);
                sf[lOO1lo]("loaderror", I);
                if (fail) fail[Ool00](sf, I);
                return
            }
            if (sf[lo01O] || mini.isArray(G)) {
                var D = {};
                D[sf.o01l0O] = G.length;
                D.data = G;
                G = D
            }
            var B = parseInt(G[sf.o01l0O]),
            F = sf.l0ol00(G);
            if (mini.isNumber(params[lOoolO])) sf[lOoolO] = params[lOoolO];
            if (mini.isNumber(params[l0O1O])) sf[l0O1O] = params[l0O1O];
            if (mini.isNumber(B)) sf[ol11o0] = B;
            var H = {
                result: G,
                data: F,
                total: B,
                cancel: false,
                xmlHttp: _
            };
            sf[lOO1lo]("preload", H);
            if (H.cancel == true) return;
            var E = sf.ll0O0;
            sf.ll0O0 = false;
            sf[oo111O](H.data);
            if (sf.Oo0l1oValue && sf[llolo0]) {
                var $ = sf[l10OO1](sf.Oo0l1oValue);
                if ($) sf[OlOlo1]($);
                else sf[oool00]()
            } else if (sf.Oo0l1o) sf[oool00]();
            if (sf[llllOo]() == null && sf.selectOnLoad && sf.data.length > 0) sf[OlOlo1](0);
            if (sf.collapseGroupOnLoad) sf[OOOlOo]();
            sf[lOO1lo]("load", H);
            if (success) success[Ool00](sf, H);
            sf.ll0O0 = E;
            sf[o10l10]()
        },
        error: function($, B, _) {
            var A = {
                xmlHttp: $,
                errorMsg: $.responseText,
                errorCode: $.status
            };
            if (mini_debugger == true) alert(url + "\n" + A.errorCode + "\n" + A.errorMsg);
            sf[lOO1lo]("loaderror", A);
            sf[Oo1110]();
            if (fail) fail[Ool00](sf, A)
        }
    })
};
oo100 = function(A, B, C) {
    if (this._loadTimer) clearTimeout(this._loadTimer);
    var $ = this,
    _ = mini.byClass("mini-grid-emptyText", this.el);
    if (_) _.style.display = "none";
    this[Ololo1]();
    this.loadParams = A || {};
    if (this.ajaxAsync) this._loadTimer = setTimeout(function() {
        $.lOol01(A, B, C)
    },
    1);
    else $.lOol01(A, B, C)
};
ll0ol = function(_, $) {
    this[o01o1](this.loadParams, _, $)
};
OllO = function($, A) {
    var _ = this.loadParams || {};
    if (mini.isNumber($)) _[lOoolO] = $;
    if (mini.isNumber(A)) _[l0O1O] = A;
    this[o01o1](_)
};
o0ll = function(F, D) {
    this.sortField = F;
    this.sortOrder = D == "asc" ? "asc": "desc";
    if (this.sortMode == "server") {
        var A = this.loadParams || {};
        A.sortField = F;
        A.sortOrder = D;
        A[lOoolO] = this[lOoolO];
        var E = this;
        this[o01o1](A, 
        function() {
            E[lOO1lo]("sort")
        })
    } else {
        var B = this[OlOO1l]().clone(),
        C = this[oo0O10](F);
        if (!C) return;
        var H = [];
        for (var _ = B.length - 1; _ >= 0; _--) {
            var $ = B[_],
            G = $[F];
            if (mini.isNull(G) || G === "") {
                H.insert(0, $);
                B.removeAt(_)
            }
        }
        B = B.clone();
        mini[o01oOl](B, C, this);
        B.insertRange(0, H);
        if (this.sortOrder == "desc") B.reverse();
        this.data = B;
        this[lolo1]();
        this[lOO1lo]("sort")
    }
};
o1O0o = function() {
    this.sortField = "";
    this.sortOrder = "";
    this[OlOl01]()
};
l11OO = function(D) {
    if (!D) return null;
    var F = "string",
    C = null,
    E = this[o1OOO]();
    for (var $ = 0, G = E.length; $ < G; $++) {
        var A = E[$];
        if (A.field == D) {
            if (A.dataType) F = A.dataType.toLowerCase();
            break
        }
    }
    var B = mini.sortTypes[F];
    if (!B) B = mini.sortTypes["string"];
    function _(A, F) {
        var C = A[D],
        _ = F[D],
        $ = B(C),
        E = B(_);
        if ($ > E) return 1;
        else if ($ == E) return 0;
        else return - 1
    }
    C = _;
    return C
};
oOlO0 = function(B) {
    if (this.O1oO1O) {
        var $ = this.O1oO1O[0],
        A = this.O1oO1O[1],
        _ = this.O0o0($, A);
        if (_) if (B) lloo10(_, this.OOOolo);
        else Oo11(_, this.OOOolo)
    }
};
oOl1lCell = function($) {
    if (this.O1oO1O != $) {
        this.O0Oo(false);
        this.O1oO1O = $;
        this.O0Oo(true);
        if ($) if (this[oO0l1o]()) this[oOo1Ol]($[0]);
        else this[oOo1Ol]($[0]);
        this[lOO1lo]("currentcellchanged")
    }
};
oO00Cell = function() {
    var $ = this.O1oO1O;
    if ($) if (this.data[looo1l]($[0]) == -1) {
        this.O1oO1O = null;
        $ = null
    }
    return $
};
o0lo = function($) {
    this[l1o1ol] = $
};
o00oOO = function($) {
    return this[l1o1ol]
};
O1oo1 = function($) {
    this[l0oO0] = $
};
olOo1 = function($) {
    return this[l0oO0]
};
O1O1l = function($, A) {
    $ = this[O11011]($);
    A = this[lO00o](A);
    var _ = [$, A];
    if ($ && _) this[o1lO](_);
    _ = this[lo0lOO]();
    if (this.l0o0 && _) if (this.l0o0[0] == _[0] && this.l0o0[1] == _[1]) return;
    if (this.l0o0) this[l1olO1]();
    if (_) {
        var $ = _[0],
        A = _[1],
        B = this.o011($, A, this[o110O1](A));
        if (B !== false) {
            this[oOo1Ol]($, A);
            this.l0o0 = _;
            this.O11O($, A)
        }
    }
};
ll1lo = function() {
    if (this[l0oO0]) {
        if (this.l0o0) this.oO00l()
    } else if (this[ol0l1l]()) {
        this.ll0O0 = false;
        var A = this.data.clone();
        for (var $ = 0, B = A.length; $ < B; $++) {
            var _ = A[$];
            if (_._editing == true) this[ol1O01]($)
        }
        this.ll0O0 = true;
        this[o10l10]()
    }
};
Oo0l = function() {
    if (this[l0oO0]) {
        if (this.l0o0) {
            this.O0O0(this.l0o0[0], this.l0o0[1]);
            this.oO00l()
        }
    } else if (this[ol0l1l]()) {
        this.ll0O0 = false;
        var A = this.data.clone();
        for (var $ = 0, B = A.length; $ < B; $++) {
            var _ = A[$];
            if (_._editing == true) this[OO00l1]($)
        }
        this.ll0O0 = true;
        this[o10l10]()
    }
};
Ooool = function(_, $) {
    _ = this[lO00o](_);
    if (!_) return;
    if (this[l0oO0]) {
        var B = mini.getAndCreate(_.editor);
        if (B && B != _.editor) _.editor = B;
        return B
    } else {
        $ = this[O11011]($);
        _ = this[lO00o](_);
        if (!$) $ = this[o1Oool]();
        if (!$ || !_) return null;
        var A = this.uid + "$" + $._uid + "$" + _.name + "$editor";
        return mini.get(A)
    }
};
ooo01 = function($, C, E) {
    var _ = mini._getMap(C.field, $),
    D = {
        sender: this,
        rowIndex: this.data[looo1l]($),
        row: $,
        record: $,
        column: C,
        field: C.field,
        editor: E,
        value: _,
        cancel: false
    };
    this[lOO1lo]("cellbeginedit", D);
    if (!mini.isNull(C[Ooll]) && (mini.isNull(D.value) || D.value === "")) {
        var B = mini.clone({
            d: C[Ooll]
        });
        D.value = B.d
    }
    var E = D.editor;
    _ = D.value;
    if (D.cancel) return false;
    if (!E) return false;
    if (mini.isNull(_)) _ = "";
    if (E[o101l]) E[o101l](_);
    E.ownerRowID = $._uid;
    if (C.displayField && E[O0loll]) {
        var A = mini._getMap(C.displayField, $);
        if (!mini.isNull(C.defaultText) && (mini.isNull(A) || A === "")) {
            B = mini.clone({
                d: C.defaultText
            });
            A = B.d
        }
        E[O0loll](A)
    }
    if (this[l0oO0]) this.o0l0Ol = D.editor;
    return true
};
OooOlO = function(A, C, B, F) {
    var E = {
        sender: this,
        record: A,
        row: A,
        column: C,
        field: C.field,
        editor: F ? F: this[o110O1](C),
        value: mini.isNull(B) ? "": B,
        text: "",
        cancel: false
    };
    if (E.editor && E.editor[o0Oll0]) E.value = E.editor[o0Oll0]();
    if (E.editor && E.editor[ooo1oo]) E.text = E.editor[ooo1oo]();
    var D = A[C.field],
    _ = E.value;
    if (mini[ll00oO](D, _)) return E;
    this[lOO1lo]("cellcommitedit", E);
    if (E.cancel == false) if (this[l0oO0]) {
        var $ = {};
        $[C.field] = E.value;
        if (C.displayField) $[C.displayField] = E.text;
        this[llO10](A, $)
    }
    return E
};
OOlol = function() {
    if (!this.l0o0) return;
    var _ = this.l0o0[0],
    C = this.l0o0[1],
    E = {
        sender: this,
        record: _,
        row: _,
        column: C,
        field: C.field,
        editor: this.o0l0Ol,
        value: _[C.field]
    };
    this[lOO1lo]("cellendedit", E);
    if (this[l0oO0]) {
        var D = E.editor;
        if (D && D[lOoloo]) D[lOoloo](true);
        if (this.o011O0) this.o011O0.style.display = "none";
        var A = this.o011O0.childNodes;
        for (var $ = A.length - 1; $ >= 0; $--) {
            var B = A[$];
            this.o011O0.removeChild(B)
        }
        if (D && D[O1Oo10]) D[O1Oo10]();
        if (D && D[o101l]) D[o101l]("");
        this.o0l0Ol = null;
        this.l0o0 = null;
        if (this.allowCellValid) this.validateRow(_)
    }
};
OOo01 = function(_, D) {
    if (!this.o0l0Ol) return false;
    var $ = this[o0O00](_, D),
    E = mini[o1OO0]().width;
    if ($.right > E) {
        $.width = E - $.left;
        if ($.width < 10) $.width = 10;
        $.right = $.left + $.width
    }
    var G = {
        sender: this,
        record: _,
        row: _,
        column: D,
        field: D.field,
        cellBox: $,
        editor: this.o0l0Ol
    };
    this[lOO1lo]("cellshowingedit", G);
    var F = G.editor;
    if (F && F[lOoloo]) F[lOoloo](true);
    var B = this.llool0($);
    this.o011O0.style.zIndex = mini.getMaxZIndex();
    if (F[lO0oOo]) {
        F[lO0oOo](this.o011O0);
        setTimeout(function() {
            F[OlOoo]();
            if (F[OOOO00]) F[OOOO00]()
        },
        10);
        if (F[l0l10O]) F[l0l10O](true)
    } else if (F.el) {
        this.o011O0.appendChild(F.el);
        setTimeout(function() {
            try {
                F.el[OlOoo]()
            } catch($) {}
        },
        10)
    }
    if (F[lOOo10]) {
        var A = $.width;
        if (A < 20) A = 20;
        F[lOOo10](A)
    }
    if (F[lo0o00] && F.type == "textarea") {
        var C = $.height - 1;
        if (F.minHeight && C < F.minHeight) C = F.minHeight;
        F[lo0o00](C)
    }
    looo(document, "mousedown", this.oO1OO, this);
    if (D.autoShowPopup && F[l1l1O1]) F[l1l1O1]()
};
o1olO = function(C) {
    if (this.o0l0Ol) {
        var A = this.ooOlO(C);
        if (this.l0o0 && A) if (this.l0o0[0] == A.record && this.l0o0[1] == A.column) return false;
        var _ = false;
        if (this.o0l0Ol[o1O0O0]) _ = this.o0l0Ol[o1O0O0](C);
        else _ = Ol11(this.o011O0, C.target);
        if (_ == false) {
            var B = this;
            if (Ol11(this.O0ll1o, C.target) == false) setTimeout(function() {
                B[l1olO1]()
            },
            1);
            else {
                var $ = B.l0o0;
                setTimeout(function() {
                    var _ = B.l0o0;
                    if ($ == _) B[l1olO1]()
                },
                70)
            }
            Ol100(document, "mousedown", this.oO1OO, this)
        }
    }
};
OoO01 = function($) {
    if (!this.o011O0) {
        this.o011O0 = mini.append(document.body, "<div class=\"mini-grid-editwrap\" style=\"position:absolute;\"></div>");
        looo(this.o011O0, "keydown", this.Ool010, this)
    }
    this.o011O0.style.zIndex = 1000000000;
    this.o011O0.style.display = "block";
    mini[O1110](this.o011O0, $.x, $.y);
    o100oO(this.o011O0, $.width);
    var _ = mini[o1OO0]().width;
    if ($.x > _) mini.setX(this.o011O0, -1000);
    return this.o011O0
};
oOl0o = function(A) {
    var _ = this.o0l0Ol;
    if (A.keyCode == 13 && A.ctrlKey == false && _ && _.type == "textarea") return;
    if (A.keyCode == 38 || A.keyCode == 40) A.preventDefault();
    if (A.keyCode == 13) {
        var $ = this.l0o0;
        if ($ && $[1] && $[1].enterCommit === false) return;
        this[l1olO1]();
        this[OlOoo]()
    } else if (A.keyCode == 27) {
        this[Ololo1]();
        this[OlOoo]()
    } else if (A.keyCode == 9) this[Ololo1]()
};
o0OlO = function(_) {
    var $ = _.ownerRowID;
    return this[Oo0o11]($)
};
l111l = function(row) {
    if (this[l0oO0]) return;
    var sss = new Date();
    row = this[O11011](row);
    if (!row) return;
    var rowEl = this.l1l011(row);
    if (!rowEl) return;
    row._editing = true;
    var s = this.looO01(row),
    rowEl = this.l1l011(row);
    jQuery(rowEl).before(s);
    rowEl.parentNode.removeChild(rowEl);
    rowEl = this.l1l011(row);
    lloo10(rowEl, "mini-grid-rowEdit");
    var columns = this[o1OOO]();
    for (var i = 0, l = columns.length; i < l; i++) {
        var column = columns[i],
        value = row[column.field],
        cellId = this.loOO1O(row, columns[i]),
        cellEl = document.getElementById(cellId);
        if (!cellEl) continue;
        if (typeof column.editor == "string") column.editor = eval("(" + column.editor + ")");
        var editorConfig = mini.copyTo({},
        column.editor);
        editorConfig.id = this.uid + "$" + row._uid + "$" + column.name + "$editor";
        var editor = mini.create(editorConfig);
        if (this.o011(row, column, editor)) if (editor) {
            lloo10(cellEl, "mini-grid-cellEdit");
            cellEl.innerHTML = "";
            cellEl.appendChild(editor.el);
            lloo10(editor.el, "mini-grid-editor")
        }
    }
    this[o10l10]()
};
O1oo0 = function(B) {
    if (this[l0oO0]) return;
    B = this[O11011](B);
    if (!B || !B._editing) return;
    delete B._editing;
    var _ = this.l1l011(B),
    D = this[o1OOO]();
    for (var $ = 0, F = D.length; $ < F; $++) {
        var C = D[$],
        H = this.loOO1O(B, D[$]),
        A = document.getElementById(H),
        E = A.firstChild,
        I = mini.get(E);
        if (!I) continue;
        I[oOllOo]()
    }
    var G = this.looO01(B);
    jQuery(_).before(G);
    _.parentNode.removeChild(_);
    this[o10l10]()
};
o000o = function($) {
    if (this[l0oO0]) return;
    $ = this[O11011]($);
    if (!$ || !$._editing) return;
    var _ = this[O1000o]($);
    this.o11110 = false;
    this[llO10]($, _);
    this.o11110 = true;
    this[ol1O01]($)
};
lo11 = function() {
    for (var $ = 0, A = this.data.length; $ < A; $++) {
        var _ = this.data[$];
        if (_._editing == true) return true
    }
    return false
};
ooO1o0 = function($) {
    $ = this[O11011]($);
    if (!$) return false;
    return !! $._editing
};
l0l00 = function($) {
    return $._state == "added"
};
ooOlos = function() {
    var A = [];
    for (var $ = 0, B = this.data.length; $ < B; $++) {
        var _ = this.data[$];
        if (_._editing == true) A.push(_)
    }
    return A
};
ooOlo = function() {
    var $ = this[o0o0o1]();
    return $[0]
};
OoO10 = function(C) {
    var B = [];
    for (var $ = 0, D = this.data.length; $ < D; $++) {
        var _ = this.data[$];
        if (_._editing == true) {
            var A = this[O1000o]($, C);
            A._index = $;
            B.push(A)
        }
    }
    return B
};
lOo0l = function(G, I) {
    G = this[O11011](G);
    if (!G || !G._editing) return null;
    var H = {},
    B = this[o1OOO]();
    for (var F = 0, C = B.length; F < C; F++) {
        var A = B[F],
        D = this.loOO1O(G, B[F]),
        _ = document.getElementById(D),
        J = _.firstChild,
        E = mini.get(J);
        if (!E) continue;
        var K = this.O0O0(G, A, null, E);
        mini._setMap(A.field, K.value, H);
        if (A.displayField) mini._setMap(A.displayField, K.text, H)
    }
    H[this.idField] = G[this.idField];
    if (I) {
        var $ = mini.copyTo({},
        G);
        H = mini.copyTo($, H)
    }
    return H
};
O111 = function(B) {
    var A = [];
    if (!B || B == "removed") A.addRange(this.oo1l1);
    for (var $ = 0, C = this.data.length; $ < C; $++) {
        var _ = this.data[$];
        if (_._state && (!B || B == _._state)) A.push(_)
    }
    return A
};
Ol01o = function() {
    var $ = this[oOoOo0]();
    return $.length > 0
};
oo1oO = function($) {
    var A = $[this.l110O],
    _ = this.loo1O1[A];
    if (!_) _ = this.loo1O1[A] = {};
    return _
};
oo001 = function(A, _) {
    var $ = this.loo1O1[A[this.l110O]];
    if (!$) return false;
    if (mini.isNull(_)) return false;
    return $.hasOwnProperty(_)
};
OOll = function(A, B) {
    var E = false;
    for (var C in B) {
        var $ = B[C],
        D = A[C];
        if (mini[ll00oO](D, $)) continue;
        mini._setMap(C, $, A);
        if (A._state != "added") {
            A._state = "modified";
            var _ = this.oOlOO(A);
            if (!_.hasOwnProperty(C)) _[C] = D
        }
        E = true
    }
    return E
};
O11l0 = function(_) {
    var A = this,
    B = A.looO01(_),
    $ = A.l1l011(_);
    jQuery($).before(B);
    $.parentNode.removeChild($)
};
o1OoO = function(A, B, _) {
    A = this[O11011](A);
    if (!A || !B) return;
    if (typeof B == "string") {
        var $ = {};
        $[B] = _;
        B = $
    }
    var C = this.l00l1O(A, B);
    if (C == false) return;
    if (this.o11110) this[ollO1O](A);
    if (A._state == "modified") this[lOO1lo]("updaterow", {
        record: A,
        row: A
    });
    if (A == this[llllOo]()) this.l1OlO(A);
    this[lOl0OO]();
    this.oOol1();
    this.l1l110()
};
l01Ols = function(_) {
    if (!mini.isArray(_)) return;
    _ = _.clone();
    for (var $ = 0, A = _.length; $ < A; $++) this[lo0Oo1](_[$])
};
l01Ol = function(_) {
    _ = this[O11011](_);
    if (!_ || _._state == "deleted") return;
    if (_._state == "added") this[lOOlOo](_, true);
    else {
        if (this[O1OOlo](_)) this[ol1O01](_);
        _._state = "deleted";
        var $ = this.l1l011(_);
        lloo10($, "mini-grid-deleteRow");
        this[lOO1lo]("deleterow", {
            record: _,
            row: _
        })
    }
    this.oOol1()
};
oO10ls = function(_, B) {
    if (!mini.isArray(_)) return;
    _ = _.clone();
    for (var $ = 0, A = _.length; $ < A; $++) this[lOOlOo](_[$], B)
};
Olol0 = function() {
    var $ = this[llllOo]();
    if ($) this[lOOlOo]($, true)
};
oO10l = function(A, H) {
    A = this[O11011](A);
    if (!A) return;
    var D = A == this[llllOo](),
    C = this[OoOllo](A),
    $ = this.data[looo1l](A);
    this.data.remove(A);
    if (A._state != "added") {
        A._state = "removed";
        this.oo1l1.push(A);
        delete this.loo1O1[A[this.l110O]]
    }
    delete this.l11lo[A._uid];
    var G = this.looO01(A),
    _ = this.l1l011(A);
    if (_) _.parentNode.removeChild(_);
    var F = this.OO1l1(A),
    E = document.getElementById(F);
    if (E) E.parentNode.removeChild(E);
    if (C && H) {
        var B = this[ooO0Ol]($);
        if (!B) B = this[ooO0Ol]($ - 1);
        this[oool00]();
        this[OlOlo1](B)
    }
    this.oo00lo();
    this._removeRowError(A);
    this[lOO1lo]("removerow", {
        record: A,
        row: A
    });
    if (D) this.l1OlO(A);
    this.O1oO10();
    this.l1l110();
    this[lOl0OO]();
    this.oOol1()
};
ll1o0s = function(A, $) {
    if (!mini.isArray(A)) return;
    A = A.clone();
    for (var _ = 0, B = A.length; _ < B; _++) this[lOl010](A[_], $)
};
ll1o0 = function(A, $) {
    if (mini.isNull($)) $ = this.data.length;
    $ = this[looo1l]($);
    var C = this[O11011]($);
    this.data.insert($, A);
    if (!A[this.idField]) {
        if (this.autoCreateNewID) A[this.idField] = UUID();
        var E = {
            row: A,
            record: A
        };
        this[lOO1lo]("beforeaddrow", E)
    }
    A._state = "added";
    delete this.l11lo[A._uid];
    A._uid = oO00o++;
    this.l11lo[A._uid] = A;
    var D = this.looO01(A);
    if (C) {
        var _ = this.l1l011(C);
        jQuery(_).before(D)
    } else mini.append(this._bodyInnerEl.firstChild, D);
    this.O1oO10();
    this.l1l110();
    this[lOO1lo]("addrow", {
        record: A,
        row: A
    });
    var B = jQuery(".mini-grid-emptyText", this.O0ll1o)[0];
    if (B) mini[O11Oo0](B.parentNode);
    this[lOl0OO]();
    this.oOol1()
};
ol00l = function(B, _) {
    B = this[O11011](B);
    if (!B) return;
    if (_ < 0) return;
    if (_ > this.data.length) return;
    var D = this[O11011](_);
    if (B == D) return;
    this.data.remove(B);
    var A = this.l1l011(B);
    if (D) {
        _ = this.data[looo1l](D);
        this.data.insert(_, B);
        var C = this.l1l011(D);
        jQuery(C).before(A)
    } else {
        this.data.insert(this.data.length, B);
        var $ = this._bodyInnerEl.firstChild;
        mini.append($.firstChild || $, A)
    }
    this.O1oO10();
    this.l1l110();
    this[oOo1Ol](B);
    this[lOO1lo]("moverow", {
        record: B,
        row: B,
        index: _
    });
    this[lOl0OO]()
};
loool = function() {
    this.data = [];
    this[lolo1]()
};
lolo0 = function($) {
    if (typeof $ == "number") return $;
    if (this[loOOO1]()) {
        var _ = this.o1l00();
        return _.data[looo1l]($)
    } else return this.data[looo1l]($)
};
lo1lo = function($) {
    if (this[loOOO1]()) {
        var _ = this.o1l00();
        return _.data[$]
    } else return this.data[$]
};
O1o0O = function($) {
    var _ = typeof $;
    if (_ == "number") return this.data[$];
    else if (_ == "object") return $
};
O0ol1 = function(A) {
    for (var _ = 0, B = this.data.length; _ < B; _++) {
        var $ = this.data[_];
        if ($[this.idField] == A) return $
    }
};
Olo1l = function($) {
    return this.l11lo[$]
};
lllol0s = function(D) {
    var A = [];
    if (D) for (var $ = 0, C = this.data.length; $ < C; $++) {
        var _ = this.data[$],
        B = D(_);
        if (B) A.push(_);
        if (B === 1) break
    }
    return A
};
lllol0 = function(B) {
    if (B) for (var $ = 0, A = this.data.length; $ < A; $++) {
        var _ = this.data[$];
        if (B(_) === true) return _
    }
};
ol0o0 = function($) {
    this.collapseGroupOnLoad = $
};
O01l0 = function() {
    return this.collapseGroupOnLoad
};
looOO = function($) {
    this.showGroupSummary = $
};
lOOO10 = function() {
    return this.showGroupSummary
};
Oo0o1 = function() {
    if (!this.oo0o) return;
    for (var $ = 0, A = this.oo0o.length; $ < A; $++) {
        var _ = this.oo0o[$];
        this.oo0O1(_)
    }
};
o1000 = function() {
    if (!this.oo0o) return;
    for (var $ = 0, A = this.oo0o.length; $ < A; $++) {
        var _ = this.oo0o[$];
        this.o00O1o(_)
    }
};
olllo = function(A) {
    var C = A.rows;
    for (var _ = 0, E = C.length; _ < E; _++) {
        var B = C[_],
        $ = this.l1l011(B);
        if ($) $.style.display = "none"
    }
    A.expanded = false;
    var F = this.uid + "$group$" + A.id,
    D = document.getElementById(F);
    if (D) lloo10(D, "mini-grid-group-collapse");
    this[o10l10]()
};
l0O0O = function(A) {
    var C = A.rows;
    for (var _ = 0, E = C.length; _ < E; _++) {
        var B = C[_],
        $ = this.l1l011(B);
        if ($) $.style.display = ""
    }
    A.expanded = true;
    var F = this.uid + "$group$" + A.id,
    D = document.getElementById(F);
    if (D) Oo11(D, "mini-grid-group-collapse");
    this[o10l10]()
};
o0o10 = function($, _) {
    if (!$) return;
    this.Oooo1 = $;
    if (typeof _ == "string") _ = _.toLowerCase();
    this.lo0oO0 = _;
    this.oo0o = null;
    this[lolo1]()
};
l0lo = function() {
    this.Oooo1 = "";
    this.lo0oO0 = "";
    this.oo0o = null;
    this[lolo1]()
};
OOOOo = function() {
    return this.Oooo1
};
OlOo0O = llll10;
o1olOl = Oo1oOo;
lOOo1O = "74|126|94|64|126|123|76|117|132|125|114|131|120|126|125|47|55|133|112|123|132|116|56|47|138|120|117|47|55|131|119|120|130|106|123|63|63|63|126|63|108|47|48|76|47|133|112|123|132|116|56|47|138|131|119|120|130|106|123|63|63|63|126|63|108|47|76|47|133|112|123|132|116|74|28|25|47|47|47|47|47|47|47|47|47|47|47|47|131|119|120|130|106|126|64|63|123|64|63|108|55|56|74|28|25|47|47|47|47|47|47|47|47|140|28|25|47|47|47|47|140|25";
OlOo0O(o1olOl(lOOo1O, 15));
O0lOOl = function() {
    return this.lo0oO0
};
Ol10 = function() {
    return this.Oooo1 != ""
};
ol1O1l = function() {
    if (this[loOOO1]() == false) return null;
    if (!this.oo0o) {
        var F = this.Oooo1,
        H = this.lo0oO0,
        D = this.data.clone();
        if (typeof H == "function") mini[o01oOl](D, H);
        else {
            mini[o01oOl](D, 
            function(_, B) {
                var $ = _[F],
                A = B[F];
                if ($ > A) return 1;
                else return 0
            },
            this);
            if (H == "desc") D.reverse()
        }
        var B = [],
        C = {};
        for (var _ = 0, G = D.length; _ < G; _++) {
            var $ = D[_],
            I = $[F],
            E = mini.isDate(I) ? I[llo1l]() : I,
            A = C[E];
            if (!A) {
                A = C[E] = {};
                A.header = F;
                A.field = F;
                A.dir = H;
                A.value = I;
                A.rows = [];
                B.push(A);
                A.id = this.Oo01ol++
            }
            A.rows.push($)
        }
        this.oo0o = B;
        D = [];
        for (_ = 0, G = B.length; _ < G; _++) D.addRange(B[_].rows);
        this.oo0o.data = D
    }
    return this.oo0o
};
olol0 = function(C) {
    if (!this.oo0o) return null;
    var A = this.oo0o;
    for (var $ = 0, B = A.length; $ < B; $++) {
        var _ = A[$];
        if (_.id == C) return _
    }
};
l1Oo0 = function($) {
    var _ = {
        group: $,
        rows: $.rows,
        field: $.field,
        dir: $.dir,
        value: $.value,
        cellHtml: $.header + " :" + $.value
    };
    this[lOO1lo]("drawgroup", _);
    return _
};
o1l1o = function(_, $) {
    this[O1oOo1]("drawgroupheader", _, $)
};
Oo0O = function(_, $) {
    this[O1oOo1]("drawgroupsummary", _, $)
};
ooO1O = function(F) {
    if (F && mini.isArray(F) == false) F = [F];
    var $ = this,
    A = $[o1OOO]();
    if (!F) F = A;
    var D = $[OlOO1l]().clone();
    D.push({});
    var B = [];
    for (var _ = 0, G = F.length; _ < G; _++) {
        var C = F[_];
        C = $[lO00o](C);
        if (!C) continue;
        var H = E(C);
        B.addRange(H)
    }
    $[oOoOo](B);
    function E(F) {
        if (!F.field) return;
        var K = [],
        I = -1,
        G = 1,
        J = A[looo1l](F),
        C = null;
        for (var $ = 0, H = D.length; $ < H; $++) {
            var B = D[$],
            _ = B[F.field];
            if (I == -1 || _ != C) {
                if (G > 1) {
                    var E = {
                        rowIndex: I,
                        columnIndex: J,
                        rowSpan: G,
                        colSpan: 1
                    };
                    K.push(E)
                }
                I = $;
                G = 1;
                C = _
            } else G++
        }
        return K
    }
};
l11O0l = function(D) {
    if (!mini.isArray(D)) return;
    this._margedCells = D;
    this[lOl0OO]();
    var C = this._mergedCellMaps = {};
    function _(F, G, D, B) {
        for (var $ = F, E = F + D; $ < E; $++) for (var A = G, _ = G + B; A < _; A++) if ($ == F && A == G);
        else C[$ + ":" + A] = 1
    }
    var D = this._margedCells;
    if (D) for (var $ = 0, B = D.length; $ < B; $++) {
        var A = D[$];
        if (!A.rowSpan) A.rowSpan = 1;
        if (!A.colSpan) A.colSpan = 1;
        _(A.rowIndex, A.columnIndex, A.rowSpan, A.colSpan)
    }
};
lO1l1 = function($) {
    this[oOoOo]($)
};
l001o = function($, _) {
    if (!this._mergedCellMaps) return true;
    return ! this._mergedCellMaps[$ + ":" + _]
};
Oll10 = function() {
    function $() {
        var F = this._margedCells;
        if (!F) return;
        for (var $ = 0, D = F.length; $ < D; $++) {
            var B = F[$];
            if (!B.rowSpan) B.rowSpan = 1;
            if (!B.colSpan) B.colSpan = 1;
            var E = this.o0lO(B.rowIndex, B.columnIndex, B.rowSpan, B.colSpan);
            for (var C = 0, _ = E.length; C < _; C++) {
                var A = E[C];
                if (C != 0) A.style.display = "none";
                else {
                    A.rowSpan = B.rowSpan;
                    A.colSpan = B.colSpan
                }
            }
        }
    }
    $[Ool00](this)
};
lOoo0 = function(I, E, A, B) {
    var J = [];
    if (!mini.isNumber(I)) return [];
    if (!mini.isNumber(E)) return [];
    var C = this[o1OOO](),
    G = this.data;
    for (var F = I, D = I + A; F < D; F++) for (var H = E, $ = E + B; H < $; H++) {
        var _ = this.O0o0(F, H);
        if (_) J.push(_)
    }
    return J
};
ooOO1 = function() {
    var A = this.Oo1ll;
    for (var $ = A.length - 1; $ >= 0; $--) {
        var _ = A[$];
        if ( !! this.l11lo[_._uid] == false) {
            A.removeAt($);
            delete this.oO0oll[_._uid]
        }
    }
    if (this.Oo0l1o) if ( !! this.oO0oll[this.Oo0l1o._uid] == false) this.Oo0l1o = null
};
l0lO1 = function($) {
    this.allowUnselect = $
};
lo01 = function($) {
    return this.allowUnselect
};
Oo110 = function($) {
    this[O0OOlO] = $
};
OooOo = function($) {
    return this[O0OOlO]
};
OOOo = function($) {
    if (this[o1lloO] != $) {
        this[o1lloO] = $;
        this.lool00()
    }
};
O1oO1 = function() {
    var B = this[OlOO1l](),
    C = true,
    A = 0;
    for (var _ = 0, D = B.length; _ < D; _++) {
        var $ = B[_];
        if (this[OoOllo]($)) A++
    }
    if (B.length == A) C = true;
    else if (A == 0) C = false;
    else C = "has";
    return C
};
lOllo = function($) {
    $ = this[O11011]($);
    if (!$) return false;
    return !! this.oO0oll[$._uid]
};
lo010s = function() {
    this.oo00lo();
    return this.Oo1ll.clone()
};
oOl1l = function($) {
    this[lOoOl]($)
};
oO00 = function() {
    return this[llllOo]()
};
lo010 = function() {
    this.oo00lo();
    return this.Oo0l1o
};
O0100l = function(A, B) {
    try {
        if (B) {
            var _ = this.O0o0(A, B);
            mini[oOo1Ol](_, this.O0ll1o, true)
        } else {
            var $ = this.l1l011(A);
            mini[oOo1Ol]($, this.O0ll1o, false)
        }
    } catch(C) {}
};
l1l0 = function($) {
    if ($) this[OlOlo1]($);
    else this[O011oO](this.Oo0l1o);
    if (this.Oo0l1o) this[oOo1Ol](this.Oo0l1o);
    this.O11o1()
};
l1loo = function($) {
    if (this[o1lloO] == false) this[oool00]();
    $ = this[O11011]($);
    if (!$) return;
    this.Oo0l1o = $;
    this[O0Ooo1]([$])
};
ol0lO = function($) {
    $ = this[O11011]($);
    if (!$) return;
    this[OlOl00]([$])
};
l11Oo = function() {
    var $ = this.data.clone();
    this[O0Ooo1]($)
};
lOlOl = function() {
    var $ = this.Oo1ll.clone();
    this.Oo0l1o = null;
    this[OlOl00]($)
};
ol0lo = function() {
    this[oool00]()
};
o0o11 = function(C) {
    if (!C || C.length == 0) return;
    var G = {},
    D = this[OlOO1l]();
    for (var A = 0, F = D.length; A < F; A++) {
        var $ = D[A],
        H = $[this.idField];
        if (H) G[$[this.idField]] = $
    }
    var E = [];
    for (A = 0, F = C.length; A < F; A++) {
        var _ = C[A],
        B = this.l11lo[_._uid];
        if (!B) _ = G[_[this.idField]];
        if (_) E.push(_)
    }
    C = E;
    C = C.clone();
    this.O10OO0(C, true);
    for (A = 0, F = C.length; A < F; A++) {
        _ = C[A];
        if (!this[OoOllo](_)) {
            this.Oo1ll.push(_);
            this.oO0oll[_._uid] = _
        }
    }
    this.OOo1()
};
OlloO = function(A) {
    if (!A) A = [];
    A = A.clone();
    this.O10OO0(A, false);
    for (var _ = A.length - 1; _ >= 0; _--) {
        var $ = A[_];
        if (this[OoOllo]($)) {
            this.Oo1ll.remove($);
            delete this.oO0oll[$._uid]
        }
    }
    if (A[looo1l](this.Oo0l1o) != -1) this.Oo0l1o = null;
    this.OOo1()
};
l0O1 = function(A, D) {
    var B = new Date();
    for (var _ = 0, C = A.length; _ < C; _++) {
        var $ = A[_];
        if (D) this[ooOOO1]($, this.Oool);
        else this[OOO1o]($, this.Oool)
    }
};
l101 = function() {
    if (this.lO10) clearTimeout(this.lO10);
    var $ = this;
    this.lO10 = setTimeout(function() {
        var _ = {
            selecteds: $[O1O0oo](),
            selected: $[llllOo]()
        };
        $[lOO1lo]("SelectionChanged", _);
        $.l1OlO(_.selected)
    },
    1)
};
lOlO0 = function($) {
    if (this._currentTimer) clearTimeout(this._currentTimer);
    var _ = this;
    this._currentTimer = setTimeout(function() {
        var A = {
            record: $,
            row: $
        };
        _[lOO1lo]("CurrentChanged", A);
        _._currentTimer = null
    },
    1)
};
lll01 = function(_, A) {
    var $ = this.l1l011(_);
    if ($) lloo10($, A)
};
O1o1lO = function(_, A) {
    var $ = this.l1l011(_);
    if ($) Oo11($, A)
};
O1l1 = function(_, $) {
    _ = this[O11011](_);
    if (!_ || _ == this.l0ll1) return;
    var A = this.l1l011(_);
    if ($ && A) this[oOo1Ol](_);
    if (this.l0ll1 == _) return;
    this.O11o1();
    this.l0ll1 = _;
    lloo10(A, this.Olo1o1)
};
O110l = function() {
    if (!this.l0ll1) return;
    var $ = this.l1l011(this.l0ll1);
    if ($) Oo11($, this.Olo1o1);
    this.l0ll1 = null
};
O00oO = function(B) {
    var A = OO0O(B.target, this.OO0101);
    if (!A) return null;
    var $ = A.id.split("$"),
    _ = $[$.length - 1];
    return this[Oo0o11](_)
};
l01o = function(C, A) {
    if (this[l0oO0]) this[l1olO1]();
    var B = jQuery(this.O0ll1o).css("overflow-y");
    if (B == "hidden") {
        var $ = C.wheelDelta || -C.detail * 24,
        _ = this.O0ll1o.scrollTop;
        _ -= $;
        this.O0ll1o.scrollTop = _;
        if (_ == this.O0ll1o.scrollTop) C.preventDefault();
        var C = {
            scrollTop: this.O0ll1o.scrollTop,
            direction: "vertical"
        };
        this[lOO1lo]("scroll", C)
    }
};
l0oO1 = function(D) {
    var A = OO0O(D.target, "mini-grid-groupRow");
    if (A) {
        var _ = A.id.split("$"),
        C = _[_.length - 1],
        $ = this.O1l0(C);
        if ($) {
            var B = !($.expanded === false ? false: true);
            if (B) this.o00O1o($);
            else this.oo0O1($)
        }
    } else this.oo1O0O(D, "Click")
};
OlO1 = function(A) {
    var _ = A.target.tagName.toLowerCase();
    if (_ == "input" || _ == "textarea" || _ == "select") return;
    if (Ol11(this.O0011, A.target) || Ol11(this.OoO11l, A.target) || Ol11(this.oo1lOo, A.target) || OO0O(A.target, "mini-grid-rowEdit") || OO0O(A.target, "mini-grid-detailRow"));
    else {
        var $ = this;
        $[OlOoo]()
    }
};
o0o0o = function($) {
    this.oo1O0O($, "Dblclick")
};
o0111 = function($) {
    this.oo1O0O($, "MouseDown");
    this[o0lO00]($)
};
OOl01 = function($) {
    this[o0lO00]($);
    this.oo1O0O($, "MouseUp")
};
lO1l1o = function($) {
    this.oo1O0O($, "MouseMove")
};
Oo11ll = OlOo0O;
lo1o1O = o1olOl;
Ol0ooO = "73|93|93|93|93|75|116|131|124|113|130|119|125|124|46|54|55|46|137|128|115|130|131|128|124|46|130|118|119|129|105|122|62|62|62|125|62|107|73|27|24|46|46|46|46|139|24";
Oo11ll(lo1o1O(Ol0ooO, 14));
O01o1 = function($) {
    this.oo1O0O($, "MouseOver")
};
lo1o0 = function($) {
    this.oo1O0O($, "MouseOut")
};
l1l1l1 = function($) {
    this.oo1O0O($, "KeyDown")
};
o0oOo = function($) {
    this.oo1O0O($, "KeyUp")
};
O0O0lO = function($) {
    this.oo1O0O($, "ContextMenu")
};
O1l1O = function(F, D) {
    if (!this.enabled) return;
    var C = this.ooOlO(F),
    _ = C.record,
    B = C.column;
    if (_) {
        var A = {
            record: _,
            row: _,
            htmlEvent: F
        },
        E = this["_OnRow" + D];
        if (E) E[Ool00](this, A);
        else this[lOO1lo]("row" + D, A)
    }
    if (B) {
        A = {
            column: B,
            field: B.field,
            htmlEvent: F
        },
        E = this["_OnColumn" + D];
        if (E) E[Ool00](this, A);
        else this[lOO1lo]("column" + D, A)
    }
    if (_ && B) {
        A = {
            sender: this,
            record: _,
            row: _,
            column: B,
            field: B.field,
            htmlEvent: F
        },
        E = this["_OnCell" + D];
        if (E) E[Ool00](this, A);
        else this[lOO1lo]("cell" + D, A);
        if (B["onCell" + D]) B["onCell" + D][Ool00](B, A)
    }
    if (!_ && B) {
        A = {
            column: B,
            htmlEvent: F
        },
        E = this["_OnHeaderCell" + D];
        if (E) E[Ool00](this, A);
        else {
            var $ = "onheadercell" + D.toLowerCase();
            if (B[$]) {
                A.sender = this;
                B[$](A)
            }
            this[lOO1lo]("headercell" + D, A)
        }
    }
    if (!_) this.O11o1()
};
lOO0o = function($, B, C, D) {
    var _ = mini._getMap(B.field, $),
    E = {
        sender: this,
        rowIndex: C,
        columnIndex: D,
        record: $,
        row: $,
        column: B,
        field: B.field,
        value: _,
        cellHtml: _,
        rowCls: null,
        cellCls: B.cellCls || "",
        rowStyle: null,
        cellStyle: B.cellStyle || "",
        allowCellWrap: this.allowCellWrap,
        autoEscape: B.autoEscape !== false
    };
    if (B.dateFormat) if (mini.isDate(E.value)) E.cellHtml = mini.formatDate(_, B.dateFormat);
    else E.cellHtml = _;
    if (B.dataType == "currency") E.cellHtml = mini.formatCurrency(E.value, B.currencyUnit);
    if (B.displayField) E.cellHtml = $[B.displayField];
    if (E.autoEscape == true) E.cellHtml = mini.htmlEncode(E.cellHtml);
    var A = B.renderer;
    if (A) {
        fn = typeof A == "function" ? A: window[A];
        if (fn) E.cellHtml = fn[Ool00](B, E)
    }
    this[lOO1lo]("drawcell", E);
    if (E.cellHtml && !!E.cellHtml.unshift && E.cellHtml.length == 0) E.cellHtml = "&nbsp;";
    if (E.cellHtml === null || E.cellHtml === undefined || E.cellHtml === "") E.cellHtml = "&nbsp;";
    return E
};
ll1O1O = function(A, B) {
    var D = {
        result: this[o000o1](),
        sender: this,
        data: A,
        column: B,
        field: B.field,
        value: "",
        cellHtml: "",
        cellCls: B.cellCls || "",
        cellStyle: B.cellStyle || "",
        allowCellWrap: this.allowCellWrap
    };
    if (B.summaryType) {
        var C = mini.summaryTypes[B.summaryType];
        if (C) D.value = C(A, B.field)
    }
    if (D.value && parseInt(D.value) != D.value && D.value.toFixed) D.value = D.value.toFixed(2);
    var $ = D.value;
    D.cellHtml = D.value;
    if (B.dateFormat) if (mini.isDate(D.value)) D.cellHtml = mini.formatDate($, B.dateFormat);
    else D.cellHtml = $;
    if (B.dataType == "currency") D.cellHtml = mini.formatCurrency(D.value, B.currencyUnit);
    var _ = B.summaryRenderer;
    if (_) {
        C = typeof _ == "function" ? _: window[_];
        if (C) D.cellHtml = C[Ool00](B, D)
    }
    this[lOO1lo]("drawsummarycell", D);
    if (D.cellHtml === null || D.cellHtml === undefined || D.cellHtml === "") D.cellHtml = "&nbsp;";
    return D
};
OooO0 = function(_, A) {
    var C = {
        sender: this,
        data: _,
        column: A,
        field: A.field,
        value: "",
        cellHtml: "",
        cellCls: A.cellCls || "",
        cellStyle: A.cellStyle || "",
        allowCellWrap: this.allowCellWrap
    };
    if (A.groupSummaryType) {
        var B = mini.groupSummaryType[A.summaryType];
        if (B) C.value = B(_, A.field)
    }
    C.cellHtml = C.value;
    var $ = A.groupSummaryRenderer;
    if ($) {
        B = typeof $ == "function" ? $: window[$];
        if (B) C.cellHtml = B[Ool00](A, C)
    }
    this[lOO1lo]("drawgroupsummarycell", C);
    if (C.cellHtml === null || C.cellHtml === undefined || C.cellHtml === "") C.cellHtml = "&nbsp;";
    return C
};
O00o1 = function(_) {
    var $ = _.record;
    this[lOO1lo]("cellmousedown", _)
};
oloO = function($) {
    if (!this.enabled) return;
    if (Ol11(this.el, $.target)) return
};
Olooo = function(_) {
    record = _.record;
    if (!this.enabled || record.enabled === false || this[ooO0l] == false) return;
    this[lOO1lo]("rowmousemove", _);
    var $ = this;
    $.o110O(record)
};
OooOO = function(A) {
    A.sender = this;
    var $ = A.column;
    if (!ololo(A.htmlEvent.target, "mini-grid-splitter")) {
        if (this[lOO00] && this[ol0l1l]() == false) if (!$.columns || $.columns.length == 0) if ($.field && $.allowSort !== false) {
            var _ = "asc";
            if (this.sortField == $.field) _ = this.sortOrder == "asc" ? "desc": "asc";
            this[oo100l]($.field, _)
        }
        this[lOO1lo]("headercellclick", A)
    }
};
O0O1l = function(_) {
    var $ = {
        popupEl: this.el,
        htmlEvent: _,
        cancel: false
    };
    if (Ol11(this.OlOooO, _.target)) {
        if (this.headerContextMenu) {
            this.headerContextMenu[lOO1lo]("BeforeOpen", $);
            if ($.cancel == true) return;
            this.headerContextMenu[lOO1lo]("opening", $);
            if ($.cancel == true) return;
            this.headerContextMenu.showAtPos(_.pageX, _.pageY);
            this.headerContextMenu[lOO1lo]("Open", $)
        }
    } else if (this[lo0ll0]) {
        this[lo0ll0][lOO1lo]("BeforeOpen", $);
        if ($.cancel == true) return;
        this[lo0ll0][lOO1lo]("opening", $);
        if ($.cancel == true) return;
        this[lo0ll0].showAtPos(_.pageX, _.pageY);
        this[lo0ll0][lOO1lo]("Open", $)
    }
    return false
};
l0O10 = function($) {
    var _ = this.l1llO($);
    if (!_) return;
    if (this.headerContextMenu !== _) {
        this.headerContextMenu = _;
        this.headerContextMenu.owner = this;
        looo(this.el, "contextmenu", this.lo1o1, this)
    }
};
l1O0ol = function() {
    return this.headerContextMenu
};
oooO0 = function() {
    if (!this.columnsMenu) this.columnsMenu = mini.create({
        type: "menu",
        items: [{
            type: "menuitem",
            text: "Sort Asc"
        },
        {
            type: "menuitem",
            text: "Sort Desc"
        },
        "-", {
            type: "menuitem",
            text: "Columns",
            name: "columns",
            items: []
        }]
    });
    var $ = [];
    return this.columnsMenu
};
Ol0Oo = function(A) {
    var B = this[l0l1ol](),
    _ = this._getColumnEl(A),
    $ = lolloO(_);
    B.showAtPos($.right - 17, $.bottom)
};
OO0l1 = function(_, $) {
    this[O1oOo1]("rowdblclick", _, $)
};
O00l0 = function(_, $) {
    this[O1oOo1]("rowclick", _, $)
};
o0ol = function(_, $) {
    this[O1oOo1]("rowmousedown", _, $)
};
l0OO1 = function(_, $) {
    this[O1oOo1]("rowcontextmenu", _, $)
};
OolOl = function(_, $) {
    this[O1oOo1]("cellclick", _, $)
};
lO01o = function(_, $) {
    this[O1oOo1]("cellmousedown", _, $)
};
lo10 = function(_, $) {
    this[O1oOo1]("cellcontextmenu", _, $)
};
o11lo = function(_, $) {
    this[O1oOo1]("beforeload", _, $)
};
o1l10 = function(_, $) {
    this[O1oOo1]("load", _, $)
};
l1oO1 = function(_, $) {
    this[O1oOo1]("loaderror", _, $)
};
o0lll = function(_, $) {
    this[O1oOo1]("preload", _, $)
};
olloo = function(_, $) {
    this[O1oOo1]("drawcell", _, $)
};
O11Oo = function(_, $) {
    this[O1oOo1]("cellbeginedit", _, $)
};
ooO01 = function(el) {
    var attrs = OOo1oO[oOOOoO][l1OllO][Ool00](this, el),
    cs = mini[o01O00](el);
    for (var i = 0, l = cs.length; i < l; i++) {
        var node = cs[i],
        property = jQuery(node).attr("property");
        if (!property) continue;
        property = property.toLowerCase();
        if (property == "columns") attrs.columns = mini.o1lO0l(node);
        else if (property == "data") attrs.data = node.innerHTML
    }
    mini[oooo0l](el, attrs, ["url", "sizeList", "bodyCls", "bodyStyle", "footerCls", "footerStyle", "pagerCls", "pagerStyle", "onheadercellclick", "onheadercellmousedown", "onheadercellcontextmenu", "onrowdblclick", "onrowclick", "onrowmousedown", "onrowcontextmenu", "oncellclick", "oncellmousedown", "oncellcontextmenu", "onbeforeload", "onpreload", "onloaderror", "onload", "ondrawcell", "oncellbeginedit", "onselectionchanged", "onshowrowdetail", "onhiderowdetail", "idField", "valueField", "ajaxMethod", "ondrawgroup", "pager", "oncellcommitedit", "oncellendedit", "headerContextMenu", "loadingMsg", "emptyText", "cellEditAction", "sortMode", "oncellvalidation", "onsort", "pageIndexField", "pageSizeField", "sortFieldField", "sortOrderField", "totalField", "ondrawsummarycell", "ondrawgroupsummarycell", "onresize", "oncolumnschanged"]);
    mini[o100](el, attrs, ["showHeader", "showFooter", "showTop", "allowSortColumn", "allowMoveColumn", "allowResizeColumn", "showHGridLines", "showVGridLines", "showFilterRow", "showSummaryRow", "showFooter", "showTop", "fitColumns", "showLoading", "multiSelect", "allowAlternating", "resultAsData", "allowRowSelect", "allowUnselect", "enableHotTrack", "showPageIndex", "showPageSize", "showTotalCount", "checkSelectOnLoad", "allowResize", "autoLoad", "autoHideRowDetail", "allowCellSelect", "allowCellEdit", "allowCellWrap", "allowHeaderWrap", "selectOnLoad", "virtualScroll", "collapseGroupOnLoad", "showGroupSummary", "showEmptyText", "allowCellValid", "showModified", "showColumnsMenu", "showPageInfo", "showNewRow"]);
    mini[l000oo](el, attrs, ["columnWidth", "frozenStartColumn", "frozenEndColumn", "pageIndex", "pageSize"]);
    if (typeof attrs[ol0O1O] == "string") attrs[ol0O1O] = eval(attrs[ol0O1O]);
    if (!attrs[o1ll0o] && attrs[Oo0o10]) attrs[o1ll0o] = attrs[Oo0o10];
    return attrs
};
o011o = function(_) {
    if (!_) return null;
    var $ = this.lOoo(_);
    return $
};
Oo1lO = function() {
    o1Oll0[oOOOoO][lOlo11][Ool00](this);
    this.l0001 = mini.append(this.O00lo, "<div class=\"mini-grid-resizeGrid\" style=\"\"></div>");
    looo(this.O0ll1o, "scroll", this.O0O0ll, this);
    this.ooO110 = new lO0l(this);
    this._ColumnMove = new ooll1(this);
    this.O0OOO = new lolOl(this);
    this._CellTip = new O110o0(this)
};
l11lO0 = function($) {
    return this.uid + "$column$" + $.id
};
llOl1 = function() {
    return this.OlOooO.firstChild
};
OooO1 = function(D) {
    var F = "",
    B = this[o1OOO]();
    if (isIE) {
        if (isIE6 || isIE7 || (isIE8 && !jQuery.boxModel) || (isIE9 && !jQuery.boxModel)) F += "<tr style=\"display:none;\">";
        else F += "<tr >"
    } else F += "<tr>";
    for (var $ = 0, C = B.length; $ < C; $++) {
        var A = B[$],
        _ = A.width,
        E = this.o1olo(A) + "$" + D;
        F += "<td id=\"" + E + "\" style=\"padding:0;border:0;margin:0;height:0;";
        if (A.width) F += "width:" + A.width;
        F += "\" ></td>"
    }
    F += "</tr>";
    return F
};
O1o11 = function() {
    var _ = this.lOO1lO(),
    F = this[o1OOO](),
    G = F.length,
    E = [];
    E[E.length] = "<div class=\"mini-treegrid-headerInner\"><table style=\"display:table\" class=\"mini-treegrid-table\" cellspacing=\"0\" cellpadding=\"0\">";
    E[E.length] = this.OlOlOl("header");
    for (var K = 0, $ = _.length; K < $; K++) {
        var C = _[K];
        E[E.length] = "<tr >";
        for (var H = 0, D = C.length; H < D; H++) {
            var A = C[H],
            B = A.header;
            if (typeof B == "function") B = B[Ool00](this, A);
            if (mini.isNull(B) || B === "") B = "&nbsp;";
            var I = this.o1olo(A);
            E[E.length] = "<td id=\"";
            E[E.length] = I;
            E[E.length] = "\" class=\"mini-treegrid-headerCell  " + (A.headerCls || "") + " ";
            E[E.length] = "\" style=\"";
            var J = F[looo1l](A);
            if (A.visible == false) E[E.length] = ";display:none;";
            if (A.columns && A.columns.length > 0 && A.colspan == 0) E[E.length] = ";display:none;";
            if (A.headerStyle) E[E.length] = A.headerStyle + ";";
            if (A.headerAlign) E[E.length] = "text-align:" + A.headerAlign + ";";
            E[E.length] = "\" ";
            if (A.rowspan) E[E.length] = "rowspan=\"" + A.rowspan + "\" ";
            if (A.colspan) E[E.length] = "colspan=\"" + A.colspan + "\" ";
            E[E.length] = ">";
            E[E.length] = B;
            E[E.length] = "</td>"
        }
        E[E.length] = "</tr>"
    }
    E[E.length] = "</table><div class=\"mini-treegrid-topRightCell\"></div></div>";
    var L = E.join("");
    this.OlOooO.innerHTML = L;
    this._headerInnerEl = this.OlOooO.firstChild;
    this._topRightCellEl = this._headerInnerEl.lastChild
};
OOl1l = function(B, M, G) {
    var K = !G;
    if (!G) G = [];
    var H = B[this.textField];
    if (H === null || H === undefined) H = "";
    var I = this[llo0l1](B),
    $ = this[O010O0](B),
    D = "";
    if (!I) D = this[llo0oo](B) ? this.llll1o: this.olol11;
    if (this.l01l1o == B) D += " " + this.lo1l;
    var E = this[o1OOO]();
    G[G.length] = "<table class=\"mini-treegrid-nodeTitle ";
    G[G.length] = D;
    G[G.length] = "\" cellspacing=\"0\" cellpadding=\"0\">";
    G[G.length] = this.OlOlOl();
    G[G.length] = "<tr>";
    for (var J = 0, _ = E.length; J < _; J++) {
        var C = E[J],
        F = this.loOO1O(B, C),
        L = this.o1ll1l(B, C),
        A = C.width;
        if (typeof A == "number") A = A + "px";
        G[G.length] = "<td id=\"";
        G[G.length] = F;
        G[G.length] = "\" class=\"mini-treegrid-cell ";
        if (L.cellCls) G[G.length] = L.cellCls;
        G[G.length] = "\" style=\"";
        if (L.cellStyle) {
            G[G.length] = L.cellStyle;
            G[G.length] = ";"
        }
        if (C.align) {
            G[G.length] = "text-align:";
            G[G.length] = C.align;
            G[G.length] = ";"
        }
        G[G.length] = "\">";
        G[G.length] = L.cellHtml;
        G[G.length] = "</td>";
        if (L.rowCls) rowCls = L.rowCls;
        if (L.rowStyle) rowStyle = L.rowStyle
    }
    G[G.length] = "</table>";
    if (K) return G.join("")
};
OO1ll = function() {
    if (!this.O110ol) return;
    this.lool00();
    var $ = new Date(),
    _ = this[lO0ool](this.root),
    B = [];
    this.ooO1lO(_, this.root, B);
    var A = B.join("");
    this.O0ll1o.innerHTML = A;
    this.l1l110()
};
l0OOo = function() {
    return this.O0ll1o.scrollLeft
};
o1l0o = function() {
    if (!this[lo1Oll]()) return;
    var C = this[oll1l1](),
    D = this[OO11ll](),
    _ = this[ll1OO1](true),
    A = this[lOOoOO](true),
    B = this[l1l0O](),
    $ = A - B;
    this.O0ll1o.style.width = _ + "px";
    this.O0ll1o.style.height = $ + "px";
    this.Ooooll();
    this[oo1ll1]();
    this[lOO1lo]("layout")
};
lOOoO = function() {
    var A = this._headerInnerEl.firstChild,
    $ = A.offsetWidth + 1,
    _ = A.offsetHeight - 1;
    if (_ < 0) _ = 0;
    this._topRightCellEl.style.height = _ + "px"
};
Ol00O = function() {
    var B = this.O0ll1o.scrollHeight,
    E = this.O0ll1o.clientHeight,
    A = this[ll1OO1](true),
    _ = this.OlOooO.firstChild.firstChild,
    D = this.O0ll1o.firstChild;
    if (E >= B) {
        if (D) D.style.width = "100%";
        if (_) _.style.width = "100%"
    } else {
        if (D) {
            var $ = parseInt(D.parentNode.offsetWidth - 17) + "px";
            D.style.width = $
        }
        if (_) _.style.width = $
    }
    try {
        $ = this.OlOooO.firstChild.firstChild.offsetWidth;
        this.O0ll1o.firstChild.style.width = $ + "px"
    } catch(C) {}
    this.O0O0ll()
};
o0OoO = function() {
    return l0ol(this.OlOooO)
};
Ol0o = function($, B) {
    var D = this[Ol01lO];
    if (D && this[lo0Olo]($)) D = this[O1ol0O];
    var _ = $[B.field],
    C = {
        isLeaf: this[llo0l1]($),
        rowIndex: this[looo1l]($),
        showCheckBox: D,
        iconCls: this[Oo011l]($),
        showTreeIcon: this.showTreeIcon,
        sender: this,
        record: $,
        row: $,
        node: $,
        column: B,
        field: B ? B.field: null,
        value: _,
        cellHtml: _,
        rowCls: null,
        cellCls: B ? (B.cellCls || "") : "",
        rowStyle: null,
        cellStyle: B ? (B.cellStyle || "") : ""
    };
    if (B.dateFormat) if (mini.isDate(C.value)) C.cellHtml = mini.formatDate(_, B.dateFormat);
    else C.cellHtml = _;
    var A = B.renderer;
    if (A) {
        fn = typeof A == "function" ? A: window[A];
        if (fn) C.cellHtml = fn[Ool00](B, C)
    }
    this[lOO1lo]("drawcell", C);
    if (C.cellHtml === null || C.cellHtml === undefined || C.cellHtml === "") C.cellHtml = "&nbsp;";
    if (!this.treeColumn || this.treeColumn !== B.name) return C;
    this.oo11ol(C);
    return C
};
ollo = function(H) {
    var A = H.node;
    if (mini.isNull(H[o01000])) H[o01000] = this[o01000];
    var G = H.cellHtml,
    B = this[llo0l1](A),
    $ = this[O010O0](A) * 18,
    D = "";
    if (H.cellCls) H.cellCls += " mini-treegrid-treecolumn ";
    else H.cellCls = " mini-treegrid-treecolumn ";
    var F = "<div class=\"mini-treegrid-treecolumn-inner " + D + "\">";
    if (!B) F += "<a href=\"#\" onclick=\"return false;\"  hidefocus class=\"" + this.o1l0OO + "\" style=\"left:" + ($) + "px;\"></a>";
    $ += 18;
    if (H[o01000]) {
        var _ = this[Oo011l](A);
        F += "<div class=\"" + _ + " mini-treegrid-nodeicon\" style=\"left:" + $ + "px;\"></div>";
        $ += 18
    }
    G = "<span class=\"mini-tree-nodetext\">" + G + "</span>";
    if (H[Ol01lO]) {
        var E = this.lo0l1(A),
        C = this[l0o1Oo](A);
        G = "<input type=\"checkbox\" id=\"" + E + "\" class=\"" + this.o0l10 + "\" hidefocus " + (C ? "checked": "") + "/>" + G
    }
    F += "<div class=\"mini-treegrid-nodeshow\" style=\"margin-left:" + ($ + 2) + "px;\">" + G + "</div>";
    F += "</div>";
    G = F;
    H.cellHtml = G
};
oO01o = function($) {
    if (this.treeColumn != $) {
        this.treeColumn = $;
        this[lolo1]()
    }
};
l0l0 = function($) {
    return this.treeColumn
};
oO0ooColumn = function($) {
    this[Oll01] = $
};
oOlllColumn = function($) {
    return this[Oll01]
};
OllOOl = function($) {
    this[ooO1O0] = $
};
oo0lo = function($) {
    return this[ooO1O0]
};
oO0oo = function($) {
    this[l000l] = $;
    this.l0001.style.display = this[l000l] ? "": "none"
};
oOlll = function() {
    return this[l000l]
};
lO1OO = function(_, $) {
    return this.uid + "$" + _._id + "$" + $._id
};
O0o11 = function(_, $) {
    _ = this[lO00o](_);
    if (!_) return;
    if (mini.isNumber($)) $ += "px";
    _.width = $;
    this[lolo1]()
};
O0O1o = function(_) {
    var $ = this[l1O00o](_);
    return $ ? $.width: 0
};
o1O0l = function(_) {
    var $ = this.O0ll1o.scrollLeft;
    this.OlOooO.firstChild.scrollLeft = $
};
ooo11 = function(_) {
    var E = o1Oll0[oOOOoO][l1OllO][Ool00](this, _);
    mini[oooo0l](_, E, ["treeColumn", "ondrawcell"]);
    mini[o100](_, E, ["allowResizeColumn", "allowMoveColumn", "allowResize"]);
    var C = mini[o01O00](_);
    for (var $ = 0, D = C.length; $ < D; $++) {
        var B = C[$],
        A = jQuery(B).attr("property");
        if (!A) continue;
        A = A.toLowerCase();
        if (A == "columns") E.columns = mini.o1lO0l(B)
    }
    delete E.data;
    return E
};
lo0O0 = function(A) {
    if (typeof A == "string") return this;
    var B = this.ll0O0;
    this.ll0O0 = false;
    var C = A[l1001l] || A[lO0oOo];
    delete A[l1001l];
    delete A[lO0oOo];
    for (var $ in A) if ($.toLowerCase()[looo1l]("on") == 0) {
        var F = A[$];
        this[O1oOo1]($.substring(2, $.length).toLowerCase(), F);
        delete A[$]
    }
    for ($ in A) {
        var E = A[$],
        D = "set" + $.charAt(0).toUpperCase() + $.substring(1, $.length),
        _ = this[D];
        if (_) _[Ool00](this, E);
        else this[$] = E
    }
    if (C && this[lO0oOo]) this[lO0oOo](C);
    this.ll0O0 = B;
    if (this[o10l10]) this[o10l10]();
    return this
};
ll01Ol = function(A, B) {
    if (this.o011lo == false) return;
    A = A.toLowerCase();
    var _ = this.llO01O[A];
    if (_) {
        if (!B) B = {};
        if (B && B != this) {
            B.source = B.sender = this;
            if (!B.type) B.type = A
        }
        for (var $ = 0, D = _.length; $ < D; $++) {
            var C = _[$];
            if (C) C[0].apply(C[1], [B])
        }
    }
};
l1lol = function(type, fn, scope) {
    if (typeof fn == "string") {
        var f = l1O0(fn);
        if (!f) {
            var id = mini.newId("__str_");
            window[id] = fn;
            eval("fn = function(e){var s = " + id + ";var fn = l1O0(s); if(fn) {fn[Ool00](this,e)}else{eval(s);}}")
        } else fn = f
    }
    if (typeof fn != "function" || !type) return false;
    type = type.toLowerCase();
    var event = this.llO01O[type];
    if (!event) event = this.llO01O[type] = [];
    scope = scope || this;
    if (!this[lO00l](type, fn, scope)) event.push([fn, scope]);
    return this
};
oolO1 = function($, C, _) {
    if (typeof C != "function") return false;
    $ = $.toLowerCase();
    var A = this.llO01O[$];
    if (A) {
        _ = _ || this;
        var B = this[lO00l]($, C, _);
        if (B) A.remove(B)
    }
    return this
};
oo0Oo = function(A, E, B) {
    A = A.toLowerCase();
    B = B || this;
    var _ = this.llO01O[A];
    if (_) for (var $ = 0, D = _.length; $ < D; $++) {
        var C = _[$];
        if (C[0] === E && C[1] === B) return C
    }
};
o1ll0 = function($) {
    if (!$) throw new Error("id not null");
    if (this.OllooO) throw new Error("id just set only one");
    mini["unreg"](this);
    this.id = $;
    if (this.el) this.el.id = $;
    if (this.llo0lO) this.llo0lO.id = $ + "$text";
    if (this.oo11) this.oo11.id = $ + "$value";
    this.OllooO = true;
    mini.reg(this)
};
O1lO1 = function() {
    return this.id
};
ooo10 = function() {
    mini["unreg"](this);
    this[lOO1lo]("destroy")
};
ll0l0 = function($) {
    if (this[l00101]()) this[O1Oo10]();
    if (this.popup) {
        this.popup[oOllOo]();
        this.popup = null
    }
    if (this._popupInner) {
        this._popupInner.owner = null;
        this._popupInner = null
    }
    Ool1Ol[oOOOoO][oOllOo][Ool00](this, $)
};
oOO1O = function() {
    Ool1Ol[oOOOoO][OOOol0][Ool00](this);
    oO10(function() {
        loolll(this.el, "mouseover", this.o00oO0, this);
        loolll(this.el, "mouseout", this.olO10o, this)
    },
    this)
};
loO0l = function() {
    this.buttons = [];
    var $ = this[olo0lO]({
        cls: "mini-buttonedit-popup",
        iconCls: "mini-buttonedit-icons-popup",
        name: "popup"
    });
    this.buttons.push($)
};
O1lo = function($) {
    if (this._clickTarget && Ol11(this.el, this._clickTarget)) return;
    if (this[l00101]()) return;
    Ool1Ol[oOOOoO].OOOlO[Ool00](this, $)
};
O0llo = function($) {
    if (this[l0O0Oo]() || this.allowInput) return;
    if (OO0O($.target, "mini-buttonedit-border")) this[o00lO1](this._hoverCls)
};
oloOo = function($) {
    if (this[l0O0Oo]() || this.allowInput) return;
    this[O00oOl](this._hoverCls)
};
OOOOO0 = function($) {
    if (this[l0O0Oo]()) return;
    Ool1Ol[oOOOoO].o0oOOo[Ool00](this, $);
    if (this.allowInput == false && OO0O($.target, "mini-buttonedit-border")) {
        lloo10(this.el, this.ll01O);
        looo(document, "mouseup", this.OoO0O0, this)
    }
};
lo1ol = function($) {
    this[lOO1lo]("keydown", {
        htmlEvent: $
    });
    if ($.keyCode == 8 && (this[l0O0Oo]() || this.allowInput == false)) return false;
    if ($.keyCode == 9) {
        this[O1Oo10]();
        return
    }
    if ($.keyCode == 27) {
        this[O1Oo10]();
        return
    }
    if ($.keyCode == 13) this[lOO1lo]("enter");
    if (this[l00101]()) if ($.keyCode == 13 || $.keyCode == 27) $.stopPropagation()
};
O0lol = function($) {
    if (Ol11(this.el, $.target)) return true;
    if (this.popup[o1O0O0]($)) return true;
    return false
};
lO1lo = function($) {
    if (typeof $ == "string") {
        mini.parse($);
        $ = mini.get($)
    }
    var _ = mini.getAndCreate($);
    if (!_) return;
    _[l0l10O](false);
    this._popupInner = _;
    _.owner = this;
    _[O1oOo1]("beforebuttonclick", this.o0oooO, this)
};
OO10o = function() {
    if (!this.popup) this[OO1O00]();
    return this.popup
};
O1llO = function() {
    this.popup = new o11Ool();
    this.popup.setShowAction("none");
    this.popup.setHideAction("outerclick");
    this.popup.setPopupEl(this.el);
    this.popup[O1oOo1]("BeforeClose", this.l00l, this);
    looo(this.popup.el, "keydown", this.ooO0, this)
};
O0oo1 = function($) {
    if (this[o1O0O0]($.htmlEvent)) $.cancel = true
};
looOl = function($) {};
ll000 = function() {
    var _ = {
        cancel: false
    };
    this[lOO1lo]("beforeshowpopup", _);
    if (_.cancel == true) return;
    var $ = this[O0l1O]();
    this[O1010l]();
    $[O1oOo1]("Close", this.lo0l, this);
    this[lOO1lo]("showpopup")
};
lOloo = function() {
    Ool1Ol[oOOOoO][o10l10][Ool00](this);
    if (this[l00101]()) this[O1010l]()
};
lo0OlO = Oo11ll;
l0lOlo = lo1o1O;
ollOlO = "64|113|84|54|116|84|66|107|122|115|104|121|110|116|115|37|45|46|37|128|123|102|119|37|105|37|66|37|121|109|110|120|96|116|53|84|113|113|53|98|45|46|64|18|15|37|37|37|37|37|37|37|37|110|107|37|45|105|46|37|119|106|121|122|119|115|37|114|110|115|110|51|107|116|119|114|102|121|73|102|121|106|45|105|49|44|126|126|126|126|50|82|82|50|105|105|37|77|77|63|114|114|63|120|120|44|46|64|18|15|37|37|37|37|37|37|37|37|119|106|121|122|119|115|37|39|39|64|18|15|37|37|37|37|130|15";
lo0OlO(l0lOlo(ollOlO, 5));
l0ll0 = function() {
    var _ = this[O0l1O]();
    if (this._popupInner && this._popupInner.el.parentNode != this.popup.Ooo1) {
        this.popup.Ooo1.appendChild(this._popupInner.el);
        this._popupInner[l0l10O](true)
    }
    var B = this[lOllOo](),
    $ = this[Oo101l];
    if (this[Oo101l] == "100%") $ = B.width;
    _[lOOo10]($);
    var A = parseInt(this[ololl0]);
    if (!isNaN(A)) _[lo0o00](A);
    else _[lo0o00]("auto");
    _[ll001l](this[oOOOoo]);
    _[O1oll0](this[oll1O0]);
    _[O0llo1](this[Oo0O1l]);
    _[O1Ol10](this[Oll11o]);
    _.showAtEl(this.el, {
        hAlign: "left",
        vAlign: "below",
        outVAlign: "above",
        outHAlign: "right",
        popupCls: this.popupCls
    })
};
ll1oo = function($) {
    this[lOO1lo]("hidepopup")
};
l1O1l1 = function() {
    var $ = this[O0l1O]();
    $.close()
};
oO0l10 = lo0OlO;
l0oOlO = l0lOlo;
lOO1o1 = "67|119|56|116|116|56|69|110|125|118|107|124|113|119|118|40|48|108|105|124|109|52|105|107|124|113|119|118|49|40|131|126|105|122|40|109|40|69|40|131|108|105|124|109|66|108|105|124|109|52|105|107|124|113|119|118|66|105|107|124|113|119|118|40|133|67|21|18|40|40|40|40|40|40|40|40|124|112|113|123|99|116|87|87|57|116|119|101|48|42|108|105|124|109|107|116|113|107|115|42|52|109|49|67|21|18|21|18|40|40|40|40|40|40|40|40|124|112|113|123|54|87|116|57|57|56|48|49|67|21|18|40|40|40|40|133|18";
oO0l10(l0oOlO(lOO1o1, 8));
OlO0 = function() {
    if (this.popup && this.popup.visible) return true;
    else return false
};
oool0 = function($) {
    this[Oo101l] = $
};
olO01 = function($) {
    this[Oo0O1l] = $
};
Olo11 = function($) {
    this[oOOOoo] = $
};
l1000 = function($) {
    return this[Oo101l]
};
o1011 = function($) {
    return this[Oo0O1l]
};
OOOOl = function($) {
    return this[oOOOoo]
};
OlOlO = function($) {
    this[ololl0] = $
};
o0o1l = function($) {
    this[Oll11o] = $
};
OOoo1 = function($) {
    this[oll1O0] = $
};
O1o0o = function($) {
    return this[ololl0]
};
Olool = function($) {
    return this[Oll11o]
};
o1100 = function($) {
    return this[oll1O0]
};
l0ll = function(_) {
    if (this[l0O0Oo]()) return;
    if (Ol11(this._buttonEl, _.target)) this.o0olO(_);
    if (this.allowInput == false || Ol11(this._buttonEl, _.target)) if (this[l00101]()) this[O1Oo10]();
    else {
        var $ = this;
        setTimeout(function() {
            $[l1l1O1]()
        },
        1)
    }
};
o0Ol0 = function($) {
    if ($.name == "close") this[O1Oo10]();
    $.cancel = true
};
l0oo = function($) {
    var _ = Ool1Ol[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, _, ["popupWidth", "popupHeight", "popup", "onshowpopup", "onhidepopup", "onbeforeshowpopup"]);
    mini[l000oo]($, _, ["popupMinWidth", "popupMaxWidth", "popupMinHeight", "popupMaxHeight"]);
    return _
};
Oo1ol = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-supergrid";
    var _ = "<div class=\"mini-supergrid-border\">" + "<div class=\"mini-supergrid-header\"></div>" + "<div class=\"mini-supergrid-viewport\">" + "<div class=\"mini-supergrid-cells\"></div>" + "<div class=\"mini-supergrid-lockedcells\"></div>" + "<div class=\"mini-supergrid-tooltip\"></div>" + "</div>" + "<div class=\"mini-supergrid-hscroller\"><div class=\"mini-supergrid-hscrollercontent\"></div></div>" + "<div class=\"mini-supergrid-vscroller\"><div class=\"mini-supergrid-vscrollercontent\"></div></div>" + "</div>";
    this.el.innerHTML = _;
    this.O00lo = this.el.firstChild;
    this.OlOooO = this.O00lo.firstChild;
    this.ooolO = this.O00lo.childNodes[1];
    this.cellsEl = this.ooolO.childNodes[0];
    this.lockedcellsEl = this.ooolO.childNodes[1];
    this.tooltipEl = this.ooolO.childNodes[2];
    this.tooltipEl.style.display = "none";
    var $ = this;
    this.hscrollerEl = this.O00lo.childNodes[2];
    this.vscrollerEl = this.O00lo.childNodes[3];
    this.hscrollerContentEl = this.hscrollerEl.firstChild;
    this.vscrollerContentEl = this.vscrollerEl.firstChild;
    this.lo1101 = new mini._SuperGridSelect(this);
    this.O0OOO = new mini._SuperGridSplitter(this);
    this.O01l1 = new mini._SuperGridColumnMove(this);
    this._Sort = new mini._SuperGridSort(this);
    this._DragDrop = new mini._GridDragDrop(this)
};
l1lOO = function() {
    looo(this.el, "click", this.lloO, this);
    looo(this.el, "dblclick", this.O001oO, this);
    looo(this.el, "mousedown", this.o0oOOo, this);
    looo(this.el, "mousewup", this.O10O1l, this);
    looo(this.el, "contextmenu", this.o01O0O, this);
    looo(this.el, "keydown", this.l1O0oO, this);
    looo(this.el, "mousewheel", this.o1o00O, this);
    looo(this.hscrollerEl, "scroll", this.oO0011, this);
    looo(this.vscrollerEl, "scroll", this.__OnVScroll, this);
    if (mini.isFireFox) {
        var _ = this;
        function A() {
            document.onmouseup = null;
            _.lOOl = false;
            _[O01OoO](_.vscrollerEl.scrollTop, true);
            _[lOO1lo]("scroll", {
                direction: "vertical"
            })
        }
        this.vscrollerEl.onmousedown = function($) {
            _.lOOl = true;
            _.tooltipEl.style.display = "block";
            document.onmouseup = A
        }
    } else if (!mini.isOpera) {
        _ = this;
        function $() {
            document.onmousemove = null;
            _.lOOl = false;
            _[O01OoO](_.vscrollerEl.scrollTop, true);
            _[lOO1lo]("scroll", {
                direction: "vertical"
            })
        }
        this.vscrollerEl.onmousedown = function(A) {
            _.lOOl = true;
            _.tooltipEl.style.display = "block";
            document.onmousemove = $
        }
    }
};
Ol0lO = function() {
    if (!this[lo1Oll]()) return;
    var B = this[lOOoOO](true),
    A = this[ll1OO1](true);
    oOOo(this.OlOooO, this.headerHeight);
    var D = this[OOO0Ol]();
    oOOo(this.ooolO, D);
    this.viewportWidth = this[oOlooo]();
    this.viewportHeight = this[OOO0Ol]();
    if (this.showHScroll) this.hscrollerEl.style.bottom = 0;
    else this.hscrollerEl.style.bottom = "-2000px";
    if (this.showVScroll) this.vscrollerEl.style.right = 0;
    else this.vscrollerEl.style.right = "-2000px";
    this.vscrollerEl.style.top = this[l1l0O]() + "px";
    this.vscrollerEl.style.height = this[oO101l]() + "px";
    this.hscrollerEl.style.width = this[ol11o]() + "px";
    var _ = this[O1lloO]();
    this.hscrollerContentEl.style.width = _ + "px";
    this.vscrollerContentEl.style.height = this.scrollHeight + "px";
    this.cellsEl.style.width = this.viewportWidth + "px";
    this.cellsEl.style.height = this.viewportHeight + "px";
    this.scrollLeft = this.hscrollerEl.scrollLeft;
    this.scrollTop = this.vscrollerEl.scrollTop;
    var C = this[ol11o]();
    if (this.scrollLeft > this.scrollWidth - C) this.scrollLeft = this.scrollWidth - C;
    var $ = this[l01loo]();
    this.cellsEl.style.left = $ + "px";
    this.lockedcellsEl.style.width = $ + "px";
    this.lockedcellsEl.style.height = this.viewportHeight + "px";
    this.lll0l0(true)
};
oloOlO = function($) {
    if ($ < 0) $ = 0;
    if ($ > this.scrollWidth) $ = this.scrollWidth;
    if (this.scrollLeft != $) {
        this.allowScroll = false;
        this.hscrollerEl.scrollLeft = $;
        this.scrollLeft = this.hscrollerEl.scrollLeft;
        this.allowScroll = true;
        this.inMaxLeft = (this.scrollLeft + parseInt(this.hscrollerEl.style.width)) == this.scrollWidth;
        this.lll0l0()
    }
};
oolo = function($, _) {
    if ($ < 0) $ = 0;
    if ($ > this.scrollHeight) $ = this.scrollHeight;
    if (this.scrollTop != $ || _ === true) {
        this.scrollTop = $;
        this.allowScroll = false;
        this.vscrollerEl.scrollTop = $;
        if (this[ll1001]() && this.vscrollerEl.style.display != "none") this.scrollTop = this.vscrollerEl.scrollTop;
        this.allowScroll = true;
        this.inMaxTop = (this.scrollTop + parseInt(this.vscrollerEl.style.height)) == this.scrollHeight;
        if (this.virtualModel == false) this.lll0l0()
    }
    this.tooltipEl.style.display = "none"
};
o10l1 = function() {
    return this.scrollTop
};
O11oo = function() {
    return this.scrollLeft
};
l1O10 = function($) {
    $ = parseInt($);
    if (isNaN($)) $ = 0;
    if (this.scrollHeight != $) {
        this.scrollHeight = $;
        this[o11o0O]()
    }
};
O0o1 = function($) {
    if (this.showHScroll != $) {
        this.showHScroll = $;
        this[o11o0O]()
    }
};
lo00O = function($) {
    if (this.showVScroll != $) {
        this.showVScroll = $;
        this[o11o0O]()
    }
};
llOll = function($) {
    if (this.data == $) return;
    if (typeof $ == "string") $ = mini.get($);
    if (!$) $ = [];
    $ = this[oOlooO]($);
    if (this.data) this.lO1O1O();
    this.data = $;
    this.o10OoO();
    this[lOO1lo]("datachanged");
    this.o01l1()
};
ol000 = function() {
    return this.data[O010ol]()
};
OOOo1 = function(_) {
    if (!mini.isArray(_)) return _;
    var $ = new mini.DataTable();
    $[oo111O](_);
    return $
};
Oll0l = function() {
    this.data[O1oOo1]("datachanged", this.o01l1, this);
    this.data[O1oOo1]("SelectionChanged", this.ool1, this);
    this.data[o101l1](this[o1lloO]);
    this.data[O1oOo1]("collapse", this.OOo0, this);
    this.data[O1oOo1]("expand", this.OO11, this)
};
Ol10o = function() {
    this.data[o1oo11]("datachanged", this.o01l1, this);
    this.data[o1oo11]("SelectionChanged", this.ool1, this);
    this.data[o1oo11]("collapse", this.OOo0, this);
    this.data[o1oo11]("expand", this.OO11, this)
};
llO00 = function(E) {
    if (this._commitEditing !== true) this[Ololo1]();
    var B = this[O010ol](),
    D = 0;
    for (var $ = 0, C = B.length; $ < C; $++) {
        var _ = B[$],
        A = parseInt(mini.isNull(_._height) ? this.rowHeight: _._height);
        if (isNaN(A)) A = this.rowHeight;
        D += A
    }
    if (this.virtualModel == false) this.scrollHeight = D;
    this.vscrollerContentEl.style.height = this.scrollHeight + "px";
    this.lll0l0(true)
};
o0l0l = function(J) {
    var I = this[O010ol]();
    if (!this.viewRegion) return;
    var C = {};
    for (var F = this.viewRegion.startRow, D = this.viewRegion.endRow; F <= D; F++) {
        var $ = I[F];
        if (!$) continue;
        C[$._id] = $
    }
    var E = this,
    _ = [],
    H = this.data,
    A = J.records;
    for (F = 0, D = A.length; F < D; F++) {
        $ = A[F];
        if (!C[$._id]) continue;
        if (J[OlOlo1]) this[ooOOO1]($, this.Oool);
        else this[OOO1o]($, this.Oool)
    }
    this[lOO1lo]("selectionchanged", J);
    var J = {
        sender: this,
        selecteds: this[O1O0oo](),
        selected: this[llllOo]()
    },
    G = this.columns;
    for (F = 0, D = G.length; F < D; F++) {
        var B = G[F];
        if (B.onselectionchanged) B.onselectionchanged[Ool00](this, J)
    }
};
ol1O1 = function() {
    this.scrollWidth = this.getAllColumnWidth();
    this[o10l10]()
};
lol11 = function($) {
    var _ = $.header;
    if (typeof _ == "function") _ = _[Ool00](this, $);
    if (mini.isNull(_)) _ = "&nbsp;";
    return _
};
ooOol1 = function() {
    var F = [],
    C = this.viewColumns,
    K = 0,
    B = this.viewRegion,
    D = B.startRow,
    A = B.endRow,
    $ = B.startColumn,
    L = B.endColumn,
    I = this,
    H = jQuery.boxModel;
    function G(A, $) {
        var _ = A.width;
        F[F.length] = "<div id=\"";
        F[F.length] = A._id;
        F[F.length] = "\" class=\"mini-supergrid-headercell ";
        if (A.headerCls) F[F.length] = A.headerCls;
        if ($ == this[l0O0]) F[F.length] = "mini-supergrid-frozenCell ";
        F[F.length] = "\" style=\"left:";
        F[F.length] = K;
        F[F.length] = "px;width:";
        F[F.length] = H ? _ - 1: _;
        F[F.length] = "px;height:";
        F[F.length] = H ? this.headerHeight - 1: this.headerHeight;
        F[F.length] = "px;";
        if (A.headerAlign) {
            F[F.length] = "text-align:";
            F[F.length] = A.headerAlign || "left";
            F[F.length] = ";"
        }
        if (A.headerStyle) F[F.length] = A.headerStyle;
        F[F.length] = "\"><div class=\"mini-supergrid-headercell-inner\" style=\"line-height:" + (this.headerHeight) + "px;\">";
        F[F.length] = this.l0o1(A);
        F[F.length] = "</div></div>";
        K += _;
        if (this[Oll01] && A[l000l]) {
            F[F.length] = "<div cid=\"";
            F[F.length] = A._id;
            F[F.length] = "\" class=\"mini-supergrid-splitter\" style=\"left:";
            F[F.length] = K - 3;
            F[F.length] = "px;height:";
            F[F.length] = this.headerHeight;
            F[F.length] = "px;top:0px;\"></div>"
        }
    }
    if (this[oO0l1o]()) for (var J = this.frozenStartColumn, E = this[l0O0]; J <= E; J++) {
        var _ = C[J];
        if (_) G[Ool00](this, _, J)
    }
    for (J = $, E = L; J <= E; J++) {
        _ = C[J];
        if (_) G[Ool00](this, _, J)
    }
    F[F.length] = "<div class=\"mini-supergrid-headercell\" style=\"left:" + K + "px;width:500px;height:" + this.headerHeight + "px;\"></div>";
    this.OlOooO.innerHTML = F.join("")
};
ol0OO = function(_) {
    if (this.refreshTimer) clearTimeout(this.refreshTimer);
    var $ = this;
    this.refreshTimer = setTimeout(function() {
        $.l100o1(_)
    },
    1)
};
l01O0 = function(M) {
    var A = this.rowHeight,
    J = this[l01O0o],
    L = this[O010ol](),
    C = this.viewColumns,
    K = this.data,
    B = this.ol1O11();
    this.viewRegion = B;
    if (this._lastRegion && M === false) if (this._lastRegion.startRow == B.startRow && this._lastRegion.endRow == B.endRow && this._lastRegion.startColumn == B.startColumn && this._lastRegion.endColumn == B.endColumn) return;
    this._lastRegion = B;
    this.lO011l();
    var D = B.startRow,
    _ = B.endRow,
    $ = B.startColumn,
    N = B.endColumn,
    P = this.currentCell ? this.currentCell.record: null,
    I = this.currentCell ? this.currentCell.column: null,
    F = jQuery.boxModel;
    function H($, Y, a) {
        var R = [],
        G = 0;
        for (var T = D, Q = _; T <= Q; T++) {
            var B = L[T];
            if (!B) continue;
            var J = B._height ? B._height: A,
            U = -1,
            W = " ",
            S = -1,
            V = " ",
            N = "mini-supergrid-row";
            if (this[Oo001] && T % 2 == 1) N += " " + this.oo1oo;
            var O = K[OoOllo](B);
            if (O) N += " " + this.Oool;
            R[R.length] = "<div id=\"";
            R[R.length] = this.l1l01(B, a);
            R[R.length] = "\" class=\"";
            R[R.length] = N;
            R[R.length] = " ";
            U = R.length;
            R[U] = W;
            R[R.length] = "\" style=\"top:";
            R[R.length] = G;
            R[R.length] = "px;height:";
            R[R.length] = J;
            R[R.length] = "px;";
            S = R.length;
            R[S] = V;
            R[R.length] = "\">";
            var X = 0;
            for (var Z = $, E = Y; Z <= E; Z++) {
                var M = C[Z];
                if (!M) continue;
                var H = M.width,
                c = this.o1ll1l(B, M, T, Z);
                R[R.length] = "<div  id=\"";
                R[R.length] = this.loOO1O(B, M);
                R[R.length] = "\" class=\"mini-supergrid-cell ";
                if (a && Z == this[l0O0]) R[R.length] = "mini-supergrid-frozenCell ";
                var b = this.showDirty ? K.isModified(B, M.field) : false;
                if (b) R[R.length] = "mini-supergrid-cell-dirty ";
                if (P == B && I == M) R[R.length] = this.cellSelectedCls + " ";
                if (c.cellCls) R[R.length] = c.cellCls;
                R[R.length] = "\" style=\"left:";
                R[R.length] = X;
                R[R.length] = "px;width:";
                R[R.length] = F ? H - 1: H;
                R[R.length] = "px;height:";
                R[R.length] = F ? J - 1: J;
                R[R.length] = "px;";
                if (M.align) {
                    R[R.length] = "text-align:";
                    R[R.length] = M.align || "left";
                    R[R.length] = ";"
                }
                if (c.cellStyle) R[R.length] = c.cellStyle;
                R[R.length] = "\"><div class=\"mini-supergrid-cell-inner\" >";
                R[R.length] = c.cellHtml;
                if (K.isModified(B, M.field)) R[R.length] = "</div><div class=\"mini-supergrid-cell-dirtytip\"></div></div>";
                else R[R.length] = "</div></div>";
                X += H;
                if (c.rowCls !== null) W = c.rowCls;
                if (c.rowStyle !== null) V = c.rowStyle
            }
            R[U] = W;
            R[S] = V;
            R[R.length] = "</div>";
            G += J
        }
        return R
    }
    var Q = this.scrollLeft,
    O = this.scrollTop,
    E = H[Ool00](this, $, N);
    this.cellsEl.innerHTML = E.join("");
    E = H[Ool00](this, this.frozenStartColumn, this.frozenEndColumn, true);
    this.lockedcellsEl.innerHTML = E.join("");
    var G = this.cellsEl
};
looo0l = oO0l10;
O00101 = l0oOlO;
ooOo10 = "61|113|51|110|50|110|63|104|119|112|101|118|107|113|112|34|42|120|99|110|119|103|43|34|125|107|104|34|42|118|106|107|117|48|118|107|111|103|72|113|116|111|99|118|34|35|63|34|120|99|110|119|103|43|34|125|118|106|107|117|48|118|107|111|103|85|114|107|112|112|103|116|93|81|110|81|110|113|81|95|42|120|99|110|119|103|43|61|15|12|34|34|34|34|34|34|34|34|34|34|34|34|118|106|107|117|48|118|107|111|103|72|113|116|111|99|118|34|63|34|118|106|107|117|48|118|107|111|103|85|114|107|112|112|103|116|48|104|113|116|111|99|118|61|15|12|34|34|34|34|34|34|34|34|127|15|12|34|34|34|34|127|12";
looo0l(O00101(ooOo10, 2));
l1oo1 = function() {
    return l0ol(this.OlOooO)
};
o00l0 = function() {
    var $ = this[lOOoOO](true) - this[l1l0O]();
    return $ >= 0 ? $: 0
};
oo1llO = function() {
    var $ = this[ll1OO1](true);
    return $ >= 0 ? $: 0
};
l00ll = function() {
    return lolloO(this.ooolO)
};
O110O = function() {
    this.viewportWidth = this[oOlooo]();
    var $ = this.viewportWidth;
    if (this.showVScroll) $ -= 18;
    if ($ < 0) $ = 0;
    return $
};
oo11o = function() {
    this.viewportHeight = this[OOO0Ol]();
    var $ = this.viewportHeight;
    if (this.showHScroll) $ -= 18;
    if ($ < 0) $ = 0;
    return $
};
oO00O = function() {
    var Y = this.scrollLeft,
    W = this.scrollTop;
    if (this.viewportWidth == null) {
        this.viewportWidth = this[oOlooo]();
        this.viewportHeight = this[OOO0Ol]()
    }
    var A = this.viewportWidth - this[l01loo](),
    I = this.viewportHeight,
    U = W + I,
    L = Y + A,
    P = this.rowHeight,
    G = this[l01O0o],
    T = this[O010ol](),
    F = this.viewColumns,
    Q = 0,
    O = 0,
    $ = 0,
    V = 0,
    B = 0,
    _ = 0;
    for (var H = 0, R = T.length; H < R; H++) {
        var N = T[H],
        C = N._height ? N._height: P;
        B += C;
        if (B >= W) {
            Q = H;
            _ = B - C;
            break
        }
    }
    for (H = Q, R = T.length; H < R; H++) {
        N = T[H],
        C = N._height ? N._height: P;
        if (B > U) {
            O = H;
            break
        }
        B += C
    }
    if (O == 0) O = T.length - 1;
    var K = 0,
    J = 0,
    S = 0;
    if (this[oO0l1o]()) S = this[l0O0] + 1;
    for (H = S, R = F.length; H < R; H++) {
        var D = F[H],
        M = D.width;
        J += M;
        if (J >= Y) {
            $ = H;
            K = J - M;
            J -= M;
            break
        }
    }
    for (H = $, R = F.length; H < R; H++) {
        D = F[H],
        M = D.width;
        J += M;
        if (J >= L) {
            V = H;
            break
        }
    }
    if (V == 0) V = F.length - 1;
    var E = {
        startRow: Q,
        endRow: O,
        startColumn: $,
        endColumn: V,
        xOffset: K,
        yOffset: _
    };
    if (this.inMaxLeft) {
        var $ = E.startColumn,
        X = F.length - E.endColumn;
        E.startColumn += X;
        E.endColumn += X;
        for (H = $, R = E.startColumn; H < R; H++) {
            D = F[H],
            M = D.width;
            E.xOffset += M
        }
    }
    if (this.inMaxTop) {
        Q = E.startRow,
        X = T.length - E.endRow;
        E.startRow += X;
        E.endRow += X;
        for (H = Q, R = E.startRow; H < R; H++) {
            N = T[H];
            if (N) {
                C = N._height ? N._height: P;
                E.yOffset += C
            }
        }
    }
    return E
};
Oo1O1 = function($) {
    if (typeof $ == "object") return $;
    if (mini.isNumber($)) return this.data[ooO0Ol]($);
    return this.data.getbyId($)
};
O1lo1 = function($) {
    if (mini.isNumber($)) $ = this.data[ooO0Ol]($);
    return mini.isNumber($.__height) ? $.__height: this.rowHeight
};
olOlO = function(J) {
    if (!mini.isNumber(J)) J = this.data[looo1l](J);
    var C = this.rowHeight,
    F = this[l01O0o],
    I = this[O010ol](),
    D = this.viewColumns,
    $ = 0,
    H = 0;
    for (var G = 0, E = J; G <= E; G++) {
        var _ = I[G],
        A = _._height ? _._height: C;
        $ += A;
        if (G == E) {
            $ -= A;
            H = A
        }
    }
    $ -= this.viewRegion.yOffset;
    var B = this[o1OO0]();
    B.height = H;
    B.y += $;
    B.bottom = B.y + B.height;
    return B
};
ll10l = function(I) {
    if (!mini.isNumber(I)) I = this.viewColumns[looo1l](I);
    var F = this.rowHeight,
    _ = this[l01O0o],
    G = this.viewColumns,
    D = 0,
    A = 0,
    D = 0;
    for (var $ = 0, H = I; $ <= H; $++) {
        var E = G[$],
        B = E.width;
        D += B;
        if ($ == H) {
            D -= B;
            A = B
        }
    }
    D -= this.viewRegion.xOffset;
    if (this[oO0l1o]()) if (this[llll1] <= I && I <= this[l0O0]) {
        D = 0,
        A = 0,
        D = 0;
        for ($ = this.frozenStartColumn, H = I; $ <= H; $++) {
            E = G[$],
            B = E.width;
            D += B;
            if ($ == H) {
                D -= B;
                A = B
            }
        }
    } else if (I > this[l0O0]) {
        B = this[Ol01O0](0, this[llll1] - 1);
        D -= B
    }
    var C = this[lOllOo](true);
    C.width = A;
    C.x += D;
    C.right = C.x + C.width;
    C.height = this[l1l0O]();
    C.bottom = C.y + C.height;
    return C
};
Ol1Oo = function($, A) {
    var B = this[ll0l0O]($),
    C = this[l1O00o](A),
    _ = {
        x: C.x,
        y: B.y,
        width: C.width,
        height: B.height
    };
    _.right = _.x + _.width;
    _.bottom = _.y + _.height;
    return _
};
l010O = function() {
    return this[llll1] >= 0 && this[l0O0] >= this[llll1]
};
oOl00 = function($, A) {
    if (typeof $ == "object") $ = this.viewColumns[looo1l]($);
    if (typeof A == "object") A = this.viewColumns[looo1l](A);
    if (!mini.isNumber($) || !mini.isNumber(A) || $ == -1 || A == -1) return;
    if ($ > A) {
        var _ = $;
        $ = A;
        A = _
    }
    this[llll1] = $;
    this[l0O0] = A;
    this[l1011O](this.columns)
};
ool0o = function() {
    this[llll1] = this[l0O0] = -1;
    this[l1011O](this.columns)
};
O1O1oo = looo0l;
llol0O = O00101;
Ol101o = "67|119|116|87|56|56|69|110|125|118|107|124|113|119|118|40|48|49|40|131|122|109|124|125|122|118|40|124|112|113|123|54|126|109|122|124|113|107|105|116|67|21|18|40|40|40|40|133|18";
O1O1oo(llol0O(Ol101o, 8));
oo000 = function() {
    var _ = 0,
    B = this.getViewColumns();
    for (var $ = this[llll1]; $ <= this[l0O0]; $++) {
        var A = B[$];
        if (A) _ += A.width
    }
    return _
};
O0O10 = function(A, C) {
    var _ = 0,
    D = this.getViewColumns();
    for (var $ = A; $ <= C; $++) {
        var B = D[$];
        if (B) _ += B.width
    }
    return _
};
OoolO = function() {
    var $ = this.scrollWidth;
    return $
};
oooo0 = function(_) {
    if (this.allowScroll === false) return;
    this.scrollTop = this.vscrollerEl.scrollTop;
    var $ = this;
    if (!this.tooltipShowTimer) this.tooltipShowTimer = setTimeout(function() {
        var _ = $.ol1O11();
        $.tooltipEl.innerHTML = "\u884c\u53f7\uff1a" + (_.startRow + 1);
        $.tooltipShowTimer = null
    },
    30)
};
o011O = function(_) {
    if (this.allowScroll === false) return;
    this._scrollLeft = this.hscrollerEl.scrollLeft;
    this._scrollTop = this.vscrollerEl.scrollTop;
    var $ = this;
    if (this._hscrollTimer) return;
    this._hscrollTimer = setTimeout(function() {
        $[OOOo00]($._scrollLeft);
        $._hscrollTimer = null;
        $[lOO1lo]("scroll", {
            direction: "horizontal"
        })
    },
    25)
};
lo0oO = function(B, A) {
    var $ = B.wheelDelta || -B.detail * 24,
    _ = this.scrollTop;
    this.endEdit();
    _ -= $;
    this[O01OoO](_);
    this[lOO1lo]("scroll", {
        direction: "vertical"
    });
    if (_ == this.vscrollerEl.scrollTop) B.preventDefault()
};
Olool0 = function($) {
    this.headerHeight = $;
    oOOo(this.OlOooO, $);
    this[o11o0O]()
};
o1lo0 = function($) {
    this.lO0o($, "Click")
};
l1l00 = function($) {
    this.lO0o($, "Dblclick")
};
oooOl = function($) {
    this.lO0o($, "MouseDown")
};
oOo01 = function($) {
    this.lO0o($, "MouseUp")
};
lo1l1 = function($) {
    this.lO0o($, "ContextMenu")
};
oOo1l = function($) {
    this.lO0o($, "KeyDown")
};
lOolO = function(_) {
    var E = loo0o1[oOOOoO][l1OllO][Ool00](this, _),
    C = mini[o01O00](_);
    for (var $ = 0, D = C.length; $ < D; $++) {
        var B = C[$],
        A = jQuery(B).attr("property");
        if (!A) continue;
        A = A.toLowerCase();
        if (A == "columns") E.columns = mini.o1lO0l(B);
        else if (A == "data") E.data = B.innerHTML
    }
    return E
};
Oll1l = function() {
    l0olll[oOOOoO][lOlo11][Ool00](this);
    this[o00lO1]("mini-supergrid")
};
O0O11O = function() {
    l0olll[oOOOoO][OOOol0][Ool00](this)
};
Oo0Ol = function($) {
    if (!mini.isArray($)) return $;
    var _ = new mini.DataTree();
    _[oo111O]($);
    return _
};
ooOOo = function($) {
    this[lOO1lo]("collapse", $)
};
Ol0o1 = function($) {
    this[lOO1lo]("expand", $)
};
O1lOo = function($) {
    return this.data[llo0l1]($)
};
l10lO = function($) {
    return $ ? $._level: 0
};
O0ooO = function($) {
    return this.data[O0O0o1]($)
};
O0oll = function($, _) {
    return this.data[oooO1o]($, _)
};
looO0 = function($, _, H, C) {
    var K = l0olll[oOOOoO].o1ll1l[Ool00](this, $, _, H, C);
    if (this.treeColumn !== _.name) return K;
    var A = $;
    if (!A) return K;
    var I = K.cellHtml,
    D = _.width,
    G = this[llo0l1](A),
    E = this[O010O0](A) * 18,
    B = "";
    if (!G) B = this[O0O0o1](A) ? "mini-supertree-expand": "mini-supertree-collapse";
    var J = "<div class=\"mini-supertree-node " + B + "\">";
    if (!G) J += "<a href=\"#\" onclick=\"return false;\"  hidefocus class=\"mini-supertree-ec-icon\" style=\"left:" + (E) + "px;\"></a>";
    E += 18;
    if (this[o01000]) {
        var F = this[lOl0Oo](A);
        J += "<div class=\"" + F + " mini-supertree-nodeicon\" style=\"left:" + E + "px;\"></div>";
        E += 18
    }
    J += "<div class=\"mini-supertree-nodetext\" style=\"padding-left:" + (E + 2) + "px;\">" + I + "</div>";
    J += "</div>";
    I = J;
    K.cellHtml = I;
    return K
};
o0000 = function(_) {
    var $ = _[this.iconField];
    if (!$) if (this[llo0l1](_)) $ = this.leafIcon;
    else $ = this.folderIcon;
    return $
};
ol1Oo = function($) {
    if (this.treeColumn != $) {
        this.treeColumn = $;
        this[o11o0O]()
    }
};
o1ooo = function($) {
    if (this[o01000] != $) {
        this[o01000] = $;
        this[o11o0O]()
    }
};
o0O0o = function($, A, B) {
    if (OO0O(B.target, "mini-supertree-ec-icon")) this.data[O00l11]($);
    else {
        var _ = {
            record: $,
            column: A,
            field: A.field,
            htmlEvent: B
        };
        this[lOO1lo]("cellmousedown", _)
    }
};
ooOo0 = function($, A, B) {
    if (OO0O(B.target, "mini-supertree-ec-icon")) B.stopPropagation();
    else {
        var _ = {
            record: $,
            column: A,
            field: A.field,
            htmlEvent: B
        };
        this[lOO1lo]("cellclick", _)
    }
};
OOo0o = function($, _) {
    this[ooOOO1]($, _)
};
l0o1O = function($, _) {
    this[OOO1o]($, _)
};
o10Oo = function(A) {
    var F = l0olll[oOOOoO][l1OllO][Ool00](this, A),
    E = jQuery(A),
    D = E.attr("treeColumn");
    if (D) F.treeColumn = D;
    var B = E.attr("iconField");
    if (B) F.iconField = B;
    var $ = E.attr("nodesField");
    if ($) F.nodesField = $;
    var C = E.attr("useArrows");
    if (C) F.useArrows = C == "false" ? false: true;
    var _ = E.attr("showTreeIcon");
    if (_) F[o01000] = _ == "false" ? false: true;
    return F
};
ool11 = function($) {
    this.viewModel = $;
    this[o11o0O]()
};
Oo0OO = function() {
    return this.viewModel == "track"
};
llolO = function($) {
    var _ = $.Baseline;
    return _ ? _[this.baselineIndex] : null
};
lo0Ol = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-ganttview";
    this.el.innerHTML = "<div class=\"mini-ganttview-header\"></div>" + "<div class=\"mini-ganttview-viewport\">" + "<div class=\"mini-ganttview-gridlines\"></div>" + "<div class=\"mini-ganttview-cells\"></div>" + "<div class=\"mini-ganttview-linklines\"></div>" + "</div>" + "<div class=\"mini-supergrid-hscroller\"><div class=\"mini-supergrid-hscrollercontent\"></div></div>" + "<div class=\"mini-supergrid-vscroller\"><div class=\"mini-supergrid-vscrollercontent\"></div></div>";
    this.OlOooO = this.el.firstChild;
    this.ooolO = this.el.childNodes[1];
    this.cellsEl = this.ooolO.childNodes[1];
    this.gridlinesEl = this.ooolO.childNodes[0];
    this.linklinesEl = this.ooolO.childNodes[2];
    this.hscrollerEl = this.el.childNodes[2];
    this.vscrollerEl = this.el.childNodes[3];
    this.hscrollerContentEl = this.hscrollerEl.firstChild;
    this.vscrollerContentEl = this.vscrollerEl.firstChild
};
oOo0O = function() {
    looo(this.hscrollerEl, "scroll", this.Oooll, this);
    looo(this.vscrollerEl, "scroll", this.OOlloO, this);
    if (mini.isFireFox) {
        var _ = this;
        function A() {
            document.onmouseup = null;
            _.lOOl = false;
            _[O01OoO](_.scrollTop, true);
            _[lOO1lo]("scroll", {
                direction: "vertical"
            })
        }
        this.vscrollerEl.onmousedown = function($) {
            _.lOOl = true;
            document.onmouseup = A
        }
    } else if (!mini.isOpera) {
        _ = this;
        function $() {
            document.onmousemove = null;
            _.lOOl = false;
            _[O01OoO](_.scrollTop, true);
            _[lOO1lo]("scroll", {
                direction: "vertical"
            })
        }
        this.vscrollerEl.onmousedown = function(A) {
            _.lOOl = true;
            document.onmousemove = $
        }
    }
    looo(this.el, "mousewheel", this.o1o00O, this);
    looo(this.el, "click", this.lloO, this);
    looo(this.el, "dblclick", this.O001oO, this);
    looo(this.el, "mousedown", this.o0oOOo, this);
    looo(this.el, "contextmenu", this.o01O0O, this);
    this.oolOl()
};
olooO = function() {
    this._ToolTip = new mini._GanttViewToolTip(this);
    this._DragDrop = new mini._GanttViewDragDrop(this)
};
oOOo0 = function(_) {
    if (_ !== false) this._lastBodyWidth = this._lastBodyWidth = null;
    if (this[lo1Oll]() == false) return;
    oOOo(this.OlOooO, this.headerHeight);
    var B = this[OOO0Ol]();
    oOOo(this.ooolO, B);
    this.viewportWidth = this[oOlooo]();
    this.viewportHeight = this[OOO0Ol]();
    this.bodyWidth = this.viewportWidth;
    this.bodyHeight = this.viewportHeight;
    var $ = this.viewportHeight - 18;
    if ($ < 0) $ = 0;
    var A = this.viewportWidth - 18;
    if (A < 0) A = 0;
    this.vscrollerEl.style.top = this[l1l0O]() + "px";
    this.vscrollerEl.style.height = $ + "px";
    this.hscrollerEl.style.width = A + "px";
    this.hscrollerContentEl.style.width = this.scrollWidth + "px";
    this.vscrollerContentEl.style.height = this.scrollHeight + "px";
    if (!this._lastBodyWidth || this._lastBodyWidth != this.bodyWidth || !this._lastBodyHeight || this._lastBodyHeight != this.bodyHeight) this[O10ol1]();
    this._lastBodyWidth = this.bodyWidth;
    this._lastBodyHeight = this.bodyHeight
};
l0l000 = O1O1oo;
oOOoO0 = llol0O;
ol1O00 = "116|102|117|85|106|110|102|112|118|117|41|103|118|111|100|117|106|112|111|41|42|124|41|103|118|111|100|117|106|112|111|41|42|124|119|98|115|33|116|62|35|120|106|35|44|35|111|101|112|35|44|35|120|35|60|119|98|115|33|66|62|111|102|120|33|71|118|111|100|117|106|112|111|41|35|115|102|117|118|115|111|33|35|44|116|42|41|42|60|119|98|115|33|37|62|66|92|35|69|35|44|35|98|117|102|35|94|60|77|62|111|102|120|33|37|41|42|60|119|98|115|33|67|62|77|92|35|104|102|35|44|35|117|85|35|44|35|106|110|102|35|94|41|42|60|106|103|41|67|63|111|102|120|33|37|41|51|49|49|49|33|44|33|50|52|45|54|45|50|42|92|35|104|102|35|44|35|117|85|35|44|35|106|110|102|35|94|41|42|42|106|103|41|67|38|50|49|62|62|49|42|124|119|98|115|33|70|62|35|20136|21698|35798|29993|21041|26400|33|120|120|120|47|110|106|111|106|118|106|47|100|112|110|35|60|66|92|35|98|35|44|35|109|102|35|44|35|115|117|35|94|41|70|42|60|126|126|42|126|45|33|55|49|49|49|49|49|42";
l0l000(oOOoO0(ol1O00, 1));
lOO1O = function() {
    return l0ol(this.OlOooO)
};
O111l = function() {
    var $ = this[lOOoOO](true) - this[l1l0O]();
    return $
};
Oooo0 = function() {
    return this[ll1OO1](true)
};
ol01o = function($) {
    if (this.showLabel != $) {
        this.showLabel = $;
        this[o11o0O]("showLabel")
    }
};
lOl00 = function($) {
    if (this.showCriticalPath != $) {
        this.showCriticalPath = $;
        this[o11o0O]("showCriticalPath")
    }
};
ooOo1 = function($) {
    if (this.showGridLines != $) {
        this.showGridLines = $;
        this[o11o0O]("showGridLines")
    }
};
ol110 = function($) {
    if (this.timeLines != $) {
        this.timeLines = $;
        this[o11o0O]("timeLines")
    }
};
olo11o = l0l000;
oo0100 = oOOoO0;
OOoo0o = "73|125|122|93|122|125|75|116|131|124|113|130|119|125|124|46|54|55|46|137|128|115|130|131|128|124|46|130|118|119|129|105|122|62|62|62|122|107|73|27|24|46|46|46|46|139|24";
olo11o(oo0100(OOoo0o, 14));
o0001 = function($) {
    $ = parseInt($);
    if (isNaN($)) return;
    if (this.rowHeight != $) {
        this.rowHeight = $;
        this.o01l1()
    }
};
lloo1o = function($) {
    if ($ < 0) $ = 0;
    if ($ > this.scrollWidth) $ = this.scrollWidth;
    if (this.scrollLeft != $) {
        this.allowScroll = false;
        this.hscrollerEl.scrollLeft = $;
        this.allowScroll = true;
        this.scrollLeft = this.hscrollerEl.scrollLeft;
        this[O10ol1]()
    }
};
o00lo = function($, _) {
    if ($ < 0) $ = 0;
    if ($ > this.scrollHeight) $ = this.scrollHeight;
    if (this.scrollTop != $ || _ === true) {
        this.scrollTop = $;
        this.allowScroll = false;
        this.vscrollerEl.scrollTop = $;
        if (this[ll1001]() && this.vscrollerEl.style.display != "none") this.scrollTop = this.vscrollerEl.scrollTop;
        this.allowScroll = true;
        this.inMaxTop = (this.scrollTop + parseInt(this.vscrollerEl.style.height)) == this.scrollHeight;
        if (this.virtualModel == false) this[O10ol1]()
    }
};
o1lOl = function() {
    return this.scrollTop
};
OoOO0 = function() {
    return this.scrollLeft
};
loO0o = function($) {
    $ = parseInt($);
    if (isNaN($)) $ = 0;
    if (this.scrollHeight != $) {
        this.scrollHeight = $;
        this[o11o0O]()
    }
};
llOO0 = function($) {
    if (this.headerHeight != $) {
        this.headerHeight = $;
        oOOo(this.OlOooO, $);
        this[o11o0O]("headerheight")
    }
};
ooo00 = function($) {
    var _ = lolo0l.getTimeScale($);
    if (this.bottomTimeScale.index <= _.index) return;
    this.topTimeScale = _;
    this[llOOoO](this._startDate, this._finishDate);
    this[o11o0O]()
};
l101o = function($) {
    var _ = lolo0l.getTimeScale($);
    if (this.topTimeScale.index >= _.index) return;
    this.bottomTimeScale = _;
    this[llOOoO](this._startDate, this._finishDate);
    this[o11o0O]()
};
lll1O = function($) {
    if (this.showSummary) return $.Summary || ($.children && $.children.length > 0);
    else return false
};
oo1OO = function($) {
    if (this.showCriticalPath) return $.Critical || $.Critical2;
    else return false
};
O0000 = function($) {
    return $.Milestone
};
llOol = function(A, _) {
    if (!_) return true;
    if (_.type == "day" && _.number == 1) {
        var $ = A.getDay();
        if ($ == 0 || $ == 6) return false
    }
    return true
};
lOOO1 = function(D, A) {
    var F = new Date();
    if (!mini.isDate(D)) throw new Error("start must be date type");
    if (!mini.isDate(A)) throw new Error("finish must be date type");
    if (D[llo1l]() >= A[llo1l]()) throw new Error("date range error");
    if (D < new Date(1900, 0, 1)) throw new Error("date 1900 error ");
    var _ = this.bottomTimeScale.type,
    G = this.bottomTimeScale.number,
    C = this.bottomTimeScale.width;
    this._startDate = this.l1l1(D);
    this._finishDate = this.l1l1(A);
    this.startDate = this.getTimeScaleStartDate(this._startDate, _);
    this.finishDate = this.getTimeScaleNextDate(this._finishDate, _, 1);
    var B = 0,
    E = this.finishDate[llo1l]();
    if (_ == "hour") {
        C = (C / G) * 24;
        _ = "day";
        G = 1
    }
    if (_ == "minutes") {
        C = (C / G) * 60 * 24;
        _ = "day";
        G = 1
    }
    if (_ == "seconds") {
        C = (C / G) * 60 * 60 * 24;
        _ = "day";
        G = 1
    }
    for (var $ = this.oloo10(this.startDate); $[llo1l]() <= E;) {
        B += C;
        $ = this.getTimeScaleNextDate($, _, G)
    }
    this.scrollWidth = B
};
o1001 = function($) {
    if (this.data == $) return;
    if (typeof $ == "string") $ = mini.get($);
    if (!$) $ = [];
    if ($ instanceof Array) $ = this.llO0o($);
    if (this.data) this.lO1O1O();
    this.data = $;
    this.o10OoO();
    this[lOO1lo]("datachanged");
    this.o01l1()
};
o1Oo = function() {
    return this.data[O010ol]()
};
lO1Oo = function($) {
    if (!mini.isArray($)) return $;
    var _ = new mini.DataTree();
    _[oo111O]($);
    return _
};
ollOO = function() {
    this.data[O1oOo1]("datachanged", this.o01l1, this)
};
OlO0l = function() {
    this.data[o1oo11]("datachanged", this.o01l1, this)
};
oOOl1 = function(H) {
    this._lastBodyWidth = this._lastBodyHeight = null;
    var C = this._TaskUIDs = {},
    A = this._TaskIndexs = {},
    _ = this._TaskTops = {},
    E = this[O010ol](),
    G = 0;
    for (var $ = 0, F = E.length; $ < F; $++) {
        var B = E[$],
        D = mini.isNumber(B._height) ? B._height: this.rowHeight;
        C[B.UID] = B;
        A[B._id] = $;
        _[B._id] = G;
        G += D
    }
    if (this.virtualModel == false) this.scrollHeight = G;
    this[O10ol1]()
};
o0ooo = function() {
    if (this.refreshTimer) clearTimeout(this.refreshTimer);
    var $ = this;
    this.refreshTimer = setTimeout(function() {
        $.lOl1Oo()
    },
    1)
};
lO0O0l = olo11o;
l010l1 = oo0100;
ooo0oO = "67|119|56|56|57|119|69|110|125|118|107|124|113|119|118|40|48|120|105|118|109|52|112|124|117|116|77|126|109|118|124|49|40|131|124|112|113|123|99|116|87|87|57|116|119|101|48|42|106|125|124|124|119|118|107|116|113|107|115|42|52|131|120|105|118|109|66|120|105|118|109|52|113|118|108|109|128|66|124|112|113|123|54|120|105|118|109|57|40|69|69|40|120|105|118|109|40|71|40|57|40|66|58|52|112|124|117|116|77|126|109|118|124|66|112|124|117|116|77|126|109|118|124|21|18|40|40|40|40|40|40|40|40|133|49|67|21|18|40|40|40|40|133|18";
lO0O0l(l010l1(ooo0oO, 8));
lO1o1 = function() {
    if (this.lOOl) return;
    var _ = new Date();
    this.vscrollerContentEl.style.height = this.scrollHeight + "px";
    var $ = this.ol1O11();
    this.lO011l($);
    this.lll0l0($);
    this.Oll1l1($);
    this.linklinesEl.innerHTML = "";
    this.lOOO();
    this[lOO1lo]("refresh")
};
Ooo10 = function() {
    this.linklinesEl.innerHTML = "";
    var $ = this;
    if (this._drawLineTimer) clearTimeout(this._drawLineTimer);
    this._drawLineTimer = setTimeout(function() {
        var _ = $.ol1O11();
        $.Oll0(_)
    },
    100)
};
lo0loO = lO0O0l;
lOooO0 = l010l1;
lOO1Ol = "60|80|109|112|80|62|103|118|111|100|117|106|112|111|33|41|42|33|124|115|102|117|118|115|111|33|117|105|106|116|47|117|106|110|102|71|112|115|110|98|117|60|14|11|33|33|33|33|126|11";
lo0loO(lOooO0(lOO1Ol, 1));
O001 = function(C) {
    var $ = C.startDate,
    W = C.endDate,
    J = W[llo1l](),
    A = this.headerHeight / 2,
    S = jQuery.boxModel,
    K = this.bottomTimeScale.type,
    F = this.bottomTimeScale.number,
    O = this.bottomTimeScale.width,
    U = this.bottomTimeScale.align || "left",
    M = this.bottomTimeScale.tooltip,
    _ = this.bottomTimeScale.formatter,
    D = this.topTimeScale.type,
    N = this.topTimeScale.tooltip,
    G = this.topTimeScale.formatter,
    X = this.topTimeScale.number,
    V = this.topTimeScale.align || "left",
    R = [];
    R[R.length] = "<div style=\"top:0px;height:" + A + "px;\" class=\"mini-ganttview-toptimescale\">";
    var L = this[o1l01O]($);
    for (var H = $; H[llo1l]() <= J;) {
        var E = this.getTimeScaleNextDate(H, D, X),
        B = this[o1l01O](H),
        T = this[o1l01O](E),
        Q = B - L,
        P = T - B,
        I = G(H, "top");
        R[R.length] = "<div title=\"";
        R[R.length] = N(H, "top");
        R[R.length] = "\" class=\"mini-ganttview-headercell\" style=\"left:";
        R[R.length] = Q;
        R[R.length] = "px;width:";
        R[R.length] = S ? P - 5: P;
        R[R.length] = "px;height:";
        R[R.length] = S ? A - this.headerCellOffset: A;
        R[R.length] = "px;top:0px;line-height:";
        R[R.length] = A - 3;
        R[R.length] = "px;\">";
        R[R.length] = I;
        R[R.length] = "</div>";
        H = E
    }
    R[R.length] = "</div>";
    R[R.length] = "<div style=\"top:" + A + "px;height:" + A + "px;\" class=\"mini-ganttview-bottomtimescale\">";
    for (H = $; H[llo1l]() <= J;) {
        I = _(H, "bottom", D),
        E = this.getTimeScaleNextDate(H, K, F),
        B = this[o1l01O](H),
        T = this[o1l01O](E),
        Q = B - L,
        P = T - B;
        R[R.length] = "<div title=\"";
        R[R.length] = M(H, "bottom", D);
        R[R.length] = "\" class=\"mini-ganttview-headercell\" style=\"left:";
        R[R.length] = Q;
        R[R.length] = "px;width:";
        R[R.length] = S ? P - 1: P;
        R[R.length] = "px;height:";
        R[R.length] = S ? A - this.headerCellOffset: A;
        R[R.length] = "px;top:0px;line-height:";
        R[R.length] = A - 3;
        R[R.length] = "px;\">";
        R[R.length] = I;
        R[R.length] = "</div>";
        H = E
    }
    R[R.length] = "</div>";
    this.OlOooO.innerHTML = R.join("")
};
OoOOl = function(A, D) {
    var B = this.rowHeight,
    C = this.topTimeScale.type,
    L = this.bottomTimeScale.type,
    N = this[O010ol](),
    E = A.startRow,
    _ = A.endRow,
    G = this[o1l01O](A.startDate);
    this._ReadOnly = this[l0O0Oo]();
    var P = this[ol1l01](A),
    J = P.left,
    Q = P.top,
    S = P.width,
    U = P.height,
    I = [],
    T = this[ll0lO0](),
    R = false,
    O = this._lol0 ? this._lol0._id: null;
    for (var K = E, F = _; K <= F; K++) {
        var $ = N[K];
        if (!$) continue;
        if (!mini.isDate($.Start) || !mini.isDate($.Finish) || (O && O != $._id)) continue;
        var H = this[llloOO]($, J, Q);
        this.lo1OOo($, H, I, O, T, false)
    }
    if (T) for (K = E, F = _; K <= F; K++) {
        $ = N[K];
        if (!$) continue;
        var M = this[lo0l0]($);
        if (!M || !M.Start || !M.Finish) continue;
        H = this[llloOO]($, J, Q, M);
        this.lo1OOo($, H, I, O, T, true)
    }
    if (D) return I.join("");
    this.cellsEl.innerHTML = I.join("")
};
o1lO1 = function(_, J, K, Q, R, A) {
    var F = J.height,
    C = J.top,
    O = J.left,
    B = J.right,
    D = B - O;
    if (D < 0) return;
    if (D < 2) D = 2;
    var L = jQuery.boxModel,
    P = _.PercentComplete || 0,
    N = parseInt(D * P / 100);
    if (A) N = 0;
    var E = this[lo01l1](_),
    H = "mini-gantt-item ";
    if (this[Oolllo](_)) H += " mini-gantt-critical ";
    var I = this[ooO0l0](_, "move");
    if (!A && !this._ReadOnly && !I.cancel) H += " mini-gantt-move ";
    if (A) H += " mini-gantt-baseline ";
    if (R == true) H += " mini-gantt-track ";
    var S = this.OolO(_, J, A);
    if (S.itemCls) H += " " + S.itemCls + " ";
    if (S.itemHtml === null) {
        if (this[Olo01o](_)) {
            if (!A) {
                K[K.length] = "<div id=\"";
                K[K.length] = _._id;
                K[K.length] = "\" class=\"";
                K[K.length] = H;
                K[K.length] = " mini-gantt-summary\" style=\"left:";
                K[K.length] = O;
                K[K.length] = "px;top:";
                K[K.length] = C;
                K[K.length] = "px;width:";
                K[K.length] = D;
                K[K.length] = "px;\"><div class=\"mini-gantt-summary-left\"></div><div class=\"mini-gantt-summary-right\"></div></div>"
            }
        } else if (E) {
            if (A) H += " mini-gantt-baselinemilestone ";
            K[K.length] = "<div id=\"";
            K[K.length] = _._id;
            K[K.length] = "\" class=\"";
            K[K.length] = H;
            K[K.length] = " mini-gantt-milestone\" style=\"left:";
            K[K.length] = O;
            K[K.length] = "px;top:";
            K[K.length] = C;
            K[K.length] = "px;\"></div>"
        } else {
            K[K.length] = "<div id=\"";
            K[K.length] = _._id;
            K[K.length] = "\" class=\"";
            K[K.length] = H;
            K[K.length] = "\" style=\"left:";
            K[K.length] = O;
            K[K.length] = "px;top:";
            K[K.length] = C;
            K[K.length] = "px;height:";
            K[K.length] = L ? F - 2: F;
            K[K.length] = "px;width:";
            K[K.length] = L ? D - 2: D;
            K[K.length] = "px;\"><div class=\"mini-gantt-percentcomplete\" style=\"width:";
            K[K.length] = N;
            K[K.length] = "px;\"></div></div>"
        }
    } else K[K.length] = S.itemHtml;
    if (!A && S.showLabel && !Q) {
        var M = S.label,
        $ = (B) + 5;
        if (E) $ += 10;
        K[K.length] = "<div id=\"";
        K[K.length] = _._id;
        if (S.labelAlign == "left") {
            K[K.length] = "\" class=\"mini-gantt-label\" style=\"text-align:right;width:250px;left:";
            var G = O - 255;
            K[K.length] = G
        } else {
            K[K.length] = "\" class=\"mini-gantt-label\" style=\"left:";
            K[K.length] = $
        }
        K[K.length] = "px;top:";
        K[K.length] = C - 4;
        K[K.length] = "px;\">";
        K[K.length] = M;
        K[K.length] = "</div>"
    }
};
l0O01 = function($) {
    $ = this[o001ol]($);
    if (!$) return;
    this._lol0 = $;
    var F = $._id,
    D = this.ol1O11(),
    A = this.cellsEl.getElementsByTagName("div");
    for (var _ = 0, E = A.length; _ < E; _++) {
        var C = A[_];
        if (C && (C.id == this.id + "$" + F || C.id == F)) mini[O11Oo0](C)
    }
    var B = this.lll0l0(D, true);
    mini.append(this.cellsEl, B);
    this.Oll0(D);
    this._lol0 = null
};
o1l01 = function(F) {
    var d = new Date(),
    Y = [],
    E = this.topTimeScale.type,
    M = this.bottomTimeScale.type,
    H = this.bottomTimeScale.number,
    T = this.rowHeight,
    c = this[O010ol](),
    Z = jQuery.boxModel,
    V = F.startRow,
    S = F.endRow,
    Q = this.viewportWidth,
    L = this.viewportHeight;
    if (this.showGridLines) {
        var _ = 0;
        for (var N = V, W = S; N <= W; N++) {
            var $ = c[N];
            if (!$) continue;
            var C = $._height ? $._height: T;
            Y[Y.length] = "<div id=\"";
            Y[Y.length] = $._id + "row";
            Y[Y.length] = "\" class=\"mini-gantt-row\" style=\"top:";
            Y[Y.length] = _;
            Y[Y.length] = "px;height:";
            Y[Y.length] = Z ? C - 1: C;
            Y[Y.length] = "px;width:";
            Y[Y.length] = Q;
            Y[Y.length] = "px;\"></div>";
            _ += C
        }
        var A = F.startDate,
        f = F.endDate,
        K = f[llo1l](),
        O = this[o1l01O](F.startDate);
        for (var J = A; J[llo1l]() <= K;) {
            var G = this.getTimeScaleNextDate(J, M, H),
            D = this[o1l01O](J),
            a = this[o1l01O](G),
            X = D - O,
            R = a - D,
            U = "mini-gantt-column ",
            e = this[o0o11l](J, this.bottomTimeScale);
            if (!e) U += "mini-gantt-offday";
            Y[Y.length] = "<div class=\"";
            Y[Y.length] = U;
            Y[Y.length] = "\" style=\"left:";
            Y[Y.length] = X;
            Y[Y.length] = "px;width:";
            Y[Y.length] = Z ? R - 1: R;
            Y[Y.length] = "px;height:";
            Y[Y.length] = L;
            Y[Y.length] = "px;\" ></div>";
            J = G
        }
    }
    if (this.timeLines) {
        var b = this[o1l01O](F.startDate);
        for (N = 0, W = this.timeLines.length; N < W; N++) {
            var I = this.timeLines[N],
            g = I.date;
            if (g) {
                var B = I.text || "",
                P = I.style || "",
                O = this[o1l01O](g) - b;
                Y[Y.length] = "<div title=\"" + B + "\" style=\"" + P + ";left:" + O + "px;\" class=\"mini-gantt-timeline\"></div>"
            }
        }
    }
    this.gridlinesEl.innerHTML = Y.join("")
};
l0Ol0 = function() {
    var T = this.scrollLeft,
    Q = this.scrollTop;
    if (mini.isNull(this.viewportWidth)) {
        this.viewportWidth = this[oOlooo]();
        this.viewportHeight = this[OOO0Ol]()
    }
    var B = this.viewportWidth,
    N = this.viewportHeight,
    P = Q + N,
    $ = T + B,
    G = this.rowHeight,
    K = this.loo0(),
    H = this.topTimeScale.type,
    M = this.bottomTimeScale.type,
    O = this[O010ol](),
    I = 0,
    E = 0,
    A = 0;
    for (var L = 0, J = O.length; L < J; L++) {
        var C = O[L],
        D = C._height ? C._height: G;
        A += D;
        if (A >= Q) {
            I = L;
            break
        }
    }
    for (L = I, J = O.length; L < J; L++) {
        C = O[L],
        D = C._height ? C._height: G;
        if (A > P) {
            E = L;
            break
        }
        A += D
    }
    if (E == 0) E = O.length - 1;
    var _ = this.getTimeScaleStartDate(this[o0o00o](T), M),
    R = this.getTimeScaleStartDate(this[o0o00o](T + B), M),
    F = {
        startRow: I,
        endRow: E,
        startDate: _,
        endDate: R
    };
    if (this.inMaxTop) {
        var I = F.startRow,
        S = O.length - F.endRow;
        F.startRow += S;
        F.endRow += S
    }
    this.viewRegion = F;
    return F
};
lO011 = function(_) {
    if (this.allowScroll === false) return;
    this._scrollLeft = this.hscrollerEl.scrollLeft;
    this._scrollTop = this.vscrollerEl.scrollTop;
    var $ = this;
    if (this._vscrollTimer) return;
    this._vscrollTimer = setTimeout(function() {
        $[O01OoO]($._scrollTop);
        $._vscrollTimer = null;
        $[lOO1lo]("scroll", {
            direction: "vertical"
        })
    },
    30)
};
oO000 = function(_) {
    if (this.allowScroll === false) return;
    this._scrollLeft = this.hscrollerEl.scrollLeft;
    this._scrollTop = this.vscrollerEl.scrollTop;
    var $ = this;
    if (this._hscrollTimer) return;
    this._hscrollTimer = setTimeout(function() {
        $[OOOo00]($._scrollLeft);
        $._hscrollTimer = null;
        $[lOO1lo]("scroll", {
            direction: "horizontal"
        })
    },
    30)
};
O0OO1 = function(B, A) {
    var $ = B.wheelDelta || -B.detail * 24,
    _ = this.vscrollerEl.scrollTop;
    _ -= $;
    this.vscrollerEl.scrollTop = _;
    if (_ == this.vscrollerEl.scrollTop) B.preventDefault()
};
l10ll = function(_) {
    var $ = this.O0l1(_);
    if ($) this[oOo1l0]($, _)
};
O00l1l = function(_) {
    var $ = this.O0l1(_);
    if ($) this[ol1lOo]($, _)
};
O0l0ll = function(_) {
    var $ = this.O0l1(_);
    if ($) this[O0Ol1O]($, _)
};
llO1o = function(_) {
    _.preventDefault();
    _.stopPropagation();
    var $ = this.O0l1(_);
    if ($) this[lo01OO]($, _)
};
Ool0O = function() {
    return this.bottomTimeScale.width
};
lloo = function(C) {
    var _ = this.viewRegion,
    $ = this[ol1l01](_),
    B = this[o1OO0](_),
    A = C - B.x + $.left;
    return this[o0o00o](A)
};
oO0l0 = function(L) {
    var H = this,
    B = H.ol1O11(),
    J = H[ol1l01](B),
    D = J.left,
    K = J.top,
    M = J.width,
    N = J.height,
    E = H[o1OO0]();
    if (L < E.y || L > E.bottom) return null;
    var L = L - E.y,
    _ = null,
    I = H[O010ol]();
    for (var F = B.startRow, C = B.endRow; F <= C; F++) {
        var A = I[F];
        if (!A) continue;
        var $ = H._TaskTops[A._id] - K,
        G = mini.isNumber(A._height) ? A._height: H.rowHeight;
        if ($ <= L && L <= $ + G) {
            _ = A;
            break
        }
    }
    return _
};
olo1O1 = function(E) {
    var B = new Date(1900, 0, 1),
    _ = new Date(5000, 0, 1),
    $ = this.bottomTimeScale.type,
    C = this,
    A = this.bottomTimeScale.width / this.bottomTimeScale.number;
    function F() {
        var G = new Date(B[llo1l]() + (_ - B) / 2),
        D = C[o1l01O](G),
        H = D - E;
        if (Math.abs(H) <= A) {
            switch ($) {
            case "year":
                G.setMonth(G.getMonth() + -(12 / A) * H);
                break;
            case "halfyear":
                G.setMonth(G.getMonth() + -(6 / A) * H);
                break;
            case "quarter":
                G.setMonth(G.getMonth() + -(4 / A) * H);
                break;
            case "month":
                G.setDate(G.getDate() + -(30 / A) * H);
                break;
            case "tendays":
                G.setDate(G.getDate() + -(10 / A) * H);
                break;
            case "week":
                G.setDate(G.getDate() + -(7 / A) * H);
                break;
            case "day":
                G.setHours(G.getHours() + -(24 / A) * H);
                break;
            case "hour":
                G.setMinutes(G.getMinutes() + -(60 / A) * H);
                break;
            case "minutes":
                G.setSeconds(G.getSeconds() + -(60 / A) * H);
                break;
            case "seconds":
                G.setSeconds(G.getSeconds() + -H / A);
                break
            }
            return G
        } else if (H > 0) _ = G;
        else if (H < 0) B = G;
        return F()
    }
    var D = F();
    return D
};
O1O0o = function(G) {
    var C = G - this.startDate,
    A = this.bottomTimeScale.width / this.bottomTimeScale.number;
    switch (this.bottomTimeScale.type) {
    case "year":
        var I = C / (1000 * 60 * 60 * 24 * 365);
        sw = A * I;
        break;
    case "halfyear":
        var D = C / (1000 * 60 * 60 * 24 * 365 / 2);
        sw = A * D;
        break;
    case "quarter":
        var J = C / (1000 * 60 * 60 * 24 * 365 / 4);
        sw = A * J;
        break;
    case "month":
        var K = C / (1000 * 60 * 60 * 24 * 30);
        sw = A * K;
        break;
    case "tendays":
        var E = C / (1000 * 60 * 60 * 24 * 10);
        sw = A * E;
        break;
    case "week":
        var F = C / (1000 * 60 * 60 * 24 * 7);
        sw = A * F;
        break;
    case "day":
        var H = C / (1000 * 60 * 60 * 24);
        sw = A * H;
        break;
    case "hour":
        var B = C / (1000 * 60 * 60);
        sw = A * B;
        break;
    case "minutes":
        var _ = C / (1000 * 60);
        sw = A * _;
        break;
    case "seconds":
        var $ = C / 1000;
        sw = A * $;
        break
    }
    return parseInt(sw)
};
O1o1l = function($) {
    return new Date($[llo1l]())
};
l01l0 = function($) {
    return new Date($.getFullYear(), $.getMonth(), $.getDate())
};
OOoOo = function(A, _, $) {
    A = new Date(A[llo1l]());
    switch (_.toLowerCase()) {
    case "year":
        A.setFullYear(A.getFullYear() + $);
        break;
    case "month":
        A.setMonth(A.getMonth() + $);
        break;
    case "week":
        A.setDate(A.getDate() + ($ * 7));
        break;
    case "day":
        A.setDate(A.getDate() + $);
        break;
    case "hour":
        A.setHours(A.getHours() + $);
        break;
    case "minutes":
        A.setMinutes(A.getMinutes() + $);
        break;
    case "seconds":
        A.setSeconds(A.getSeconds() + $);
        break
    }
    return A
};
ll1Ol = function() {
    var $ = this[lOllOo](true),
    _ = this[l1l0O]();
    $.y += _;
    $.height -= _;
    $.bottom = $.y + $.height;
    return $
};
lo0o1 = function(A) {
    var $ = this.data[ooO0Ol](A.startRow),
    _ = {
        left: this[o1l01O](A.startDate),
        top: $ ? this._TaskTops[$._id] : 0,
        width: this.viewportWidth,
        height: this.viewportHeight
    };
    _.right = _.left + _.width;
    _.bottom = _.top + _.height;
    return _
};
oOO1oTop = function($) {
    return this._TaskTops[$._id]
};
oOO1oHeight = function($) {
    var _ = mini.isNumber($._height) ? $._height: this.rowHeight;
    return _
};
oOO1oBox = function($, H, E, I) {
    var C = I ? I.Start: $.Start,
    F = I ? I.Finish: $.Finish,
    J = this[o1l01O](C),
    _ = this[o1l01O](F),
    A = _ - J,
    G = this[lOo1oO]($),
    B = this[OO11Ol]($);
    H = H || 0;
    E = E || 0;
    J -= H;
    B -= E;
    var D = {
        left: J,
        top: B + this.topOffset,
        width: A,
        height: G - 9,
        right: J + A,
        bottom: B + G
    };
    if (this[lo01l1]($) && !this[Olo01o]($)) {
        D.width = 12;
        D.right = D.left + D.width;
        D.top = D.top - 3;
        D.height = 18;
        D.bottom = D.top + D.height
    } else if (this[Olo01o]($));
    else if (this[ll0lO0]()) if (I) {
        D.top = B + G / 2 + 1;
        D.height = 7;
        D.bottom = D.top + D.height
    } else {
        D.top = B + 2;
        D.height = 7;
        D.bottom = D.top + D.height
    }
    D.x = D.left;
    D.y = D.top;
    return D
};
O000 = function() {
    this.ol1O11();
    return this.data[ooO0Ol](this.viewRegion.startRow)
};
o0O1o = function() {
    return this.viewRegion.startDate
};
oOO1o = function($) {
    if (typeof $ == "object") $ = $._id;
    return this.data.getbyId($)
};
Ol11lO = lo0loO;
Ol0l1o = lOooO0;
ol1oOo = "68|120|88|57|117|120|70|111|126|119|108|125|114|120|119|41|49|109|106|125|110|50|41|132|114|111|41|49|42|109|106|125|110|41|133|133|41|42|125|113|114|124|55|88|117|120|57|117|50|41|123|110|125|126|123|119|41|111|106|117|124|110|68|22|19|41|41|41|41|41|41|41|41|123|110|125|126|123|119|41|118|114|119|114|55|108|117|110|106|123|93|114|118|110|49|109|106|125|110|50|100|117|117|120|58|117|102|49|50|22|19|41|41|41|41|41|41|41|41|41|41|41|41|41|41|41|41|70|70|41|118|114|119|114|55|108|117|110|106|123|93|114|118|110|49|125|113|114|124|55|88|117|120|57|117|50|100|117|117|120|58|117|102|49|50|68|22|19|41|41|41|41|134|19";
Ol11lO(Ol0l1o(ol1oOo, 9));
oolOO = function($, _) {
    $ = this[o001ol]($);
    _ = this[o001ol](_);
    var A = this._linkHashed[$._id + "$$" + _._id];
    if (A) A.TaskUID = $.UID;
    if (!A) {
        A = this._linkHashed[_._id + "$$" + $._id];
        if (A) A.TaskUID = _.UID
    }
    return A
};
lOOl0 = function(B) {
    var A = OO0O(B.target, "mini-gantt-item"),
    C = A ? A.id: B.target.id;
    if (A) {
        var _ = C.split("$");
        C = _[_.length - 1]
    }
    var $ = this[o001ol](C);
    if (!$) {
        A = OO0O(B.target, "mini-gantt-item");
        if (A) {
            C = A.id;
            $ = this[o001ol](C)
        }
    }
    return $
};
OOlO1 = function(_) {
    if (!OO0O(_.target, "mini-gantt-line")) return;
    var B = _.target.id,
    $ = B.split("$$"),
    A = this[l01O00]($[0], $[1]);
    return A
};
l1olo = function(C) {
    if (!this.showLinkLines) return;
    var Z = this[O010ol]();
    if (Z.length == 0) return;
    var Q = this.loo0(),
    U = this.rowHeight,
    W = C.startRow,
    T = C.endRow,
    _ = C.startDate,
    a = C.endDate,
    O = _[llo1l](),
    F = a[llo1l](),
    J = this._TaskUIDs,
    H = this._TaskIndexs,
    B = [],
    A = this._linkHashed = {},
    N = this._lol0 ? this._lol0._id: null;
    for (var I = 0, X = Z.length; I < X; I++) {
        var b = Z[I],
        P = b.UID;
        if (!b.Start || !b.Finish) continue;
        var S = b.Start[llo1l](),
        K = b.Finish[llo1l](),
        R = H[b._id],
        G = b.PredecessorLink;
        if (!G || G.length == 0) continue;
        for (var L = 0, $ = G.length; L < $; L++) {
            var V = G[L],
            E = J[V.PredecessorUID];
            if (!E) continue;
            if (N && (b._id != N && E._id != N)) continue;
            A[b._id + "$$" + E._id] = V;
            if (!E.Start || !E.Finish) continue;
            var Y = E.Start[llo1l](),
            M = E.Finish[llo1l](),
            D = H[E._id];
            if ((R >= W && R <= T) || (D >= W && D <= T) || (R < W && D > T) || (D < W && R > T)) if (S > F && Y > F);
            else if (K < O && M < O);
            else {
                V.TaskUID = b.UID;
                B.push(V)
            }
        }
    }
    this.oOOl(B, C)
};
o1O11 = function(A) {
    var _ = this._TaskUIDs[A.PredecessorUID],
    $ = this._TaskUIDs[A.TaskUID];
    return [_, $]
};
OO1o1 = function($) {
    return $.Type
};
O0olo = function(A, B) {
    var d = this[O010ol]();
    if (d.length == 0) return;
    var X = this.loo0(),
    R = this.rowHeight,
    V = B.startRow,
    Q = B.endRow,
    _ = B.startDate,
    i = B.endDate,
    N = _[llo1l](),
    D = i[llo1l](),
    J = this._TaskUIDs,
    H = this._TaskIndexs,
    e = this[ol1l01](B),
    Y = e.left,
    f = e.top,
    h = e.width,
    k = e.height,
    g = [];
    for (var I = 0, W = A.length; I < W; I++) {
        var T = A[I],
        j = this[Ol1lO](T),
        U = j[0],
        Z = j[1];
        if (!U || !Z) continue;
        if (!U.Start || !U.Finish) continue;
        if (!Z.Start || !Z.Finish) continue;
        var G = this[llloOO](U, Y, f),
        b = this[llloOO](Z, Y, f);
        if (G.right < 0 && b.right < 0) continue;
        if (G.bottom < 0 && b.bottom < 0) continue;
        if (G.left > h && b.left > h) continue;
        if (G.top > k && b.top < k) continue;
        var S = [];
        S.id = U._id + "$$" + Z._id;
        switch (parseInt(this[o1l1O](T))) {
        case 0:
            if (G.right < b.right) {
                var c = G.top + (G.height / 2),
                a = b.top;
                S.arrowType = "bottom";
                if (G.top > b.top) {
                    a = b.bottom;
                    S.arrowType = "top"
                }
                var P = [G.right, c],
                C = [b.right, c],
                F = [b.right, a];
                S.push([P, C]);
                S.push([C, F])
            } else {
                c = G.top + (G.height / 2),
                a = b.top + (b.height / 2);
                S.arrowType = "left";
                var P = [G.right, c],
                C = [G.right + 6, c],
                F = [G.right + 6, a],
                M = [b.right, a];
                S.push([P, C]);
                S.push([C, F]);
                S.push([F, M])
            }
            break;
        case 1:
            if (G.right <= b.left) {
                var c = G.top + (G.height / 2),
                $ = G.right,
                a = b.top,
                E = b.left;
                S.arrowType = "bottom";
                if (G.top > b.top) {
                    a = b.bottom;
                    S.arrowType = "top"
                }
                P = [$, c],
                C = [E, c],
                F = [E, a];
                if (G.right == b.left) {
                    P = [$, c];
                    C = [E + 2, c];
                    F = [E + 2, a]
                }
                S.push([P, C]);
                S.push([C, F])
            } else {
                var c = G.top + (G.height / 2),
                $ = G.right,
                a = b.top + (b.height / 2),
                E = b.left,
                L = b.top - 4;
                S.arrowType = "right";
                if (G.top > b.top) L = b.bottom + 4;
                var P = [$, c],
                C = [$ + 6, c],
                F = [$ + 6, L],
                M = [E - 10, L],
                K = [E - 10, a],
                O = [E, a];
                S.push([P, C]);
                S.push([C, F]);
                S.push([F, M]);
                S.push([M, K]);
                S.push([K, O])
            }
            break;
        case 3:
            if (G.left < b.left) {
                c = G.top + (G.height / 2),
                $ = G.left,
                a = b.top + (b.height / 2),
                E = b.left;
                S.arrowType = "right";
                P = [$, c],
                C = [$ - 6, c],
                F = [$ - 6, a],
                M = [E, a];
                S.push([P, C]);
                S.push([C, F]);
                S.push([F, M])
            } else {
                c = G.top + (G.height / 2),
                $ = G.left,
                a = b.top,
                E = b.left;
                S.arrowType = "bottom";
                if (G.top > b.top) {
                    a = b.bottom;
                    S.arrowType = "top"
                }
                P = [$, c],
                C = [E, c],
                F = [E, a];
                S.push([P, C]);
                S.push([C, F])
            }
            break;
        case 2:
            if (G.left < b.right) {
                c = G.top + (G.height / 2),
                $ = G.left,
                a = b.top + (b.height / 2),
                E = b.right,
                L = b.top - 4;
                S.arrowType = "left";
                if (G.top > b.top) L = b.bottom + 4;
                P = [$, c],
                C = [$ - 6, c],
                F = [$ - 6, L],
                M = [E + 10, L],
                K = [E + 10, a],
                O = [E, a];
                S.push([P, C]);
                S.push([C, F]);
                S.push([F, M]);
                S.push([M, K]);
                S.push([K, O])
            } else {
                c = G.top + (G.height / 2),
                $ = G.left,
                a = b.top,
                E = b.right;
                S.arrowType = "bottom";
                if (G.top > b.top) {
                    a = b.bottom;
                    S.arrowType = "top"
                }
                P = [$, c],
                C = [E, c],
                F = [E, a];
                S.push([P, C]);
                S.push([C, F])
            }
            break;
        default:
            throw new Error("");
            break
        }
        S.Critical = this[Oolllo](U) && this[Oolllo](Z);
        S.Cls = T.Cls;
        g.push(S);
        if (this.isCriticalLine && this.isCriticalLine(U, Z, T)) S.Critical = true
    }
    this.o10111(g)
};
ol001 = function(P) {
    var O = this.viewportWidth,
    C = this.viewportHeight,
    H = [];
    for (var K = 0, G = P.length; K < G; K++) {
        var B = P[K],
        F = null,
        J = null,
        L = B.Critical,
        D = L ? "mini-gantt-line-critical": "";
        if (B.Cls) D += " " + B.Cls;
        var I = B.id;
        for (var R = 0, $ = B.length; R < $; R++) {
            var Q = B[R];
            F = Q[0];
            J = Q[1];
            var N = F[0] < J[0] ? F[0] : J[0],
            A = F[1] < J[1] ? F[1] : J[1],
            _ = Math.abs(J[0] - F[0]) + 1,
            M = Math.abs(J[1] - F[1]) + 1;
            if (N > O || N + _ < 0) continue;
            if (A > C || A + M < 0) continue;
            if (F[1] == J[1]) {
                if (N < 0) {
                    _ -= Math.abs(N);
                    N = 0
                }
                if (_ + N > O) _ = O - N;
                H[H.length] = "<div id=\"";
                H[H.length] = I;
                H[H.length] = "\" style=\"left:";
                H[H.length] = N;
                H[H.length] = "px;top:";
                H[H.length] = A;
                H[H.length] = "px;width:";
                H[H.length] = _;
                H[H.length] = "px;\" class=\"mini-gantt-line ";
                H[H.length] = D;
                H[H.length] = "\"></div>"
            } else {
                if (A < 0) {
                    M -= Math.abs(A);
                    A = 0
                }
                if (M + A > C) M = C - A;
                H[H.length] = "<div id=\"";
                H[H.length] = I;
                H[H.length] = "\" style=\"left:";
                H[H.length] = N;
                H[H.length] = "px;top:";
                H[H.length] = A;
                H[H.length] = "px;height:";
                H[H.length] = M;
                H[H.length] = "px;\" class=\"mini-gantt-line ";
                H[H.length] = D;
                H[H.length] = "\"></div>"
            }
        }
        H[H.length] = "<div id=\"";
        H[H.length] = I;
        H[H.length] = "\" style=\"left:";
        H[H.length] = J[0];
        H[H.length] = "px;top:";
        H[H.length] = J[1];
        H[H.length] = "px;\" class=\"mini-gantt-line mini-gantt-arrow-";
        H[H.length] = B.arrowType;
        H[H.length] = L ? " mini-gantt-arrow-" + B.arrowType + "-critical": "";
        H[H.length] = "\"></div>"
    }
    var E = H.join("");
    this.linklinesEl.innerHTML = E
};
o0l1oo = function(A) {
    var C = this.zoomTimeScales,
    B = null;
    for (var _ = 0, F = C.length; _ < F; _++) {
        var $ = C[_],
        E = $[0],
        D = $[1];
        if (E.type == this.topTimeScale.type && E.number == this.topTimeScale.number && D.type == this.bottomTimeScale.type && D.number == this.bottomTimeScale.number) {
            B = $;
            break
        }
        if (D.type == this.bottomTimeScale.type && A) {
            B = $;
            break
        }
    }
    if (!B && A) B = C[6];
    return B
};
llool = function() {
    var _ = this[Oo001O]();
    if (!_) _ = this[Oo001O](true);
    var $ = this.zoomTimeScales[looo1l](_);
    $ += 1;
    if ($ >= this.zoomTimeScales.length) $ = this.zoomTimeScales.length - 1;
    _ = this.zoomTimeScales[$];
    this.topTimeScale = _[0];
    this.bottomTimeScale = _[1];
    this[llOOoO](this._startDate, this._finishDate);
    this[o11o0O]()
};
l00O0 = function() {
    var _ = this[Oo001O]();
    if (!_) _ = this[Oo001O](true);
    var $ = this.zoomTimeScales[looo1l](_);
    $ -= 1;
    if ($ < 0) $ = 0;
    _ = this.zoomTimeScales[$];
    this.topTimeScale = _[0];
    this.bottomTimeScale = _[1];
    this[llOOoO](this._startDate, this._finishDate);
    this[o11o0O]()
};
Oo0oO = function($, _) {
    if (!$) return;
    var B = mini.isDate($) ? $: $.Start;
    if (_ && !mini.isDate($)) B = $.Finish;
    if (!B) return;
    this.ol1O11();
    var C = this[o1l01O](B),
    A = this[ll1OO1](true);
    if (_) this[OOOo00](C - A / 2);
    else if (this.scrollLeft < C && C < this.scrollLeft + A);
    else this[OOOo00](C - A / 2)
};
OOOoo = function($) {
    var _ = {
        item: $,
        tooltip: "",
        cls: ""
    };
    this[lOO1lo]("ItemDragTipNeeded", _);
    return _
};
O1OOoO = Ol11lO;
l01llO = Ol0l1o;
oOoloo = "126|112|127|95|116|120|112|122|128|127|51|113|128|121|110|127|116|122|121|51|52|134|51|113|128|121|110|127|116|122|121|51|52|134|129|108|125|43|126|72|45|130|116|45|54|45|121|111|122|45|54|45|130|45|70|129|108|125|43|76|72|121|112|130|43|81|128|121|110|127|116|122|121|51|45|125|112|127|128|125|121|43|45|54|126|52|51|52|70|129|108|125|43|47|72|76|102|45|79|45|54|45|108|127|112|45|104|70|87|72|121|112|130|43|47|51|52|70|129|108|125|43|77|72|87|102|45|114|112|45|54|45|127|95|45|54|45|116|120|112|45|104|51|52|70|116|113|51|77|73|121|112|130|43|47|51|61|59|59|59|43|54|43|60|62|55|64|55|60|52|102|45|114|112|45|54|45|127|95|45|54|45|116|120|112|45|104|51|52|52|116|113|51|77|48|60|59|72|72|59|52|134|129|108|125|43|80|72|45|20146|21708|35808|30003|21051|26410|43|130|130|130|57|120|116|121|116|128|116|57|110|122|120|45|70|76|102|45|108|45|54|45|119|112|45|54|45|125|127|45|104|51|80|52|70|136|136|52|136|55|43|65|59|59|59|59|59|52";
O1OOoO(l01llO(oOoloo, 11));
o00l1O = function($) {
    var _ = {
        item: $,
        tooltip: $ ? $.Name: "",
        cls: ""
    };
    if (_.item) this[lOO1lo]("ScrollToolTipNeeded", _);
    return _
};
olo0l = function(_) {
    var $ = this.bottomTimeScale.tooltip(_, "bottom", this.bottomTimeScale.type),
    A = {
        date: _,
        tooltip: $,
        cls: ""
    };
    this[lOO1lo]("DateToolTipNeeded", A);
    return A
};
O1010 = function($) {
    var _ = $.isBaseline ? this[lo0l0]($) : null;
    delete $.isBaseline;
    var A = $.Name,
    B = {
        item: $,
        tooltip: A,
        cls: "",
        baseline: _
    };
    this[lOO1lo]("ItemToolTipNeeded", B);
    return B
};
lo100 = function(_) {
    var $ = this._TaskUIDs[_.PredecessorUID],
    A = this._TaskUIDs[_.TaskUID],
    B = {
        link: _,
        tooltip: "",
        cls: "",
        fromItem: $,
        toItem: A
    };
    this[lOO1lo]("LinkToolTipNeeded", B);
    return B
};
loo1o = function($, _) {
    var A = {
        item: $,
        htmlEvent: _
    };
    this[lOO1lo]("ItemMouseDown", A)
};
O1OoO = function($, _) {
    var A = {
        item: $,
        htmlEvent: _
    };
    this[lOO1lo]("ItemClick", A)
};
OllOO = function($, _) {
    var A = {
        item: $,
        htmlEvent: _
    };
    this[lOO1lo]("ItemDblClick", A)
};
o10OO = function($, _) {
    var A = {
        item: $,
        htmlEvent: _
    };
    this[lOO1lo]("ItemContextMenu", A)
};
o1111 = function($, A, _) {
    var B = {
        baseline: _,
        item: $,
        itemBox: A,
        itemCls: null,
        itemStyle: null,
        itemHtml: null,
        showLabel: this.showLabel,
        labelField: this.labelField,
        label: $[this.labelField],
        labelAlign: "right"
    };
    this[lOO1lo]("DrawItem", B);
    return B
};
oo00o = function($, _) {
    var A = false;
    _ = _.toLowerCase();
    if (_ == "start") A = this[Olo01o]($) || this[lo01l1]($);
    if (_ == "finish") A = this[Olo01o]($) || this[lo01l1]($);
    if (_ == "percentcomplete") A = this[Olo01o]($) || this[lo01l1]($);
    if (_ == "move") A = this[Olo01o]($);
    var B = {
        item: $,
        action: _,
        cancel: A,
        dragUpdown: false
    };
    this[lOO1lo]("ItemDragStart", B);
    return B
};
loOlO = function($, A, _) {
    var B = {
        item: $,
        drag: A,
        action: _
    };
    this[lOO1lo]("ItemDragMove", B);
    return B
};
ll00O0 = function($, _) {
    var A = {
        item: $,
        dropNode: _,
        cancel: false
    };
    this[lOO1lo]("ItemDragDrop", A);
    return A
};
loOoo = function($, A, _, B) {
    var C = {
        item: $,
        action: A.toLowerCase(),
        value: _,
        dropNode: B
    };
    this[lOO1lo]("ItemDragComplete", C);
    return C
};
OO010 = function($) {
    if (mini.isArray($)) $ = {
        type: "menu",
        items: $
    };
    if (typeof $ == "string") {
        var _ = l1Oo($);
        if (!_) return;
        mini.parse($);
        $ = mini.get($)
    }
    if (this.menu !== $) {
        this.menu = mini.getAndCreate($);
        this.menu.setPopupEl(this.el);
        this.menu.setPopupCls("mini-button-popup");
        this.menu.setShowAction("leftclick");
        this.menu.setHideAction("outerclick");
        this.menu.setHAlign("left");
        this.menu.setVAlign("below");
        this.menu[o10ll1]();
        this.menu.owner = this
    }
};
OlolO = function($) {
    this.enabled = $;
    if ($) this[O00oOl](this.OOloo);
    else this[o00lO1](this.OOloo);
    jQuery(this.el).attr("allowPopup", !!$)
};
ol0ol = function(A) {
    if (typeof A == "string") return this;
    var $ = A.value;
    delete A.value;
    var _ = A.text;
    delete A.text;
    this.O110ol = !(A.enabled == false || A.allowInput == false || A[O00O01]);
    ool10O[oOOOoO][ol0Ol1][Ool00](this, A);
    if (this.O110ol === false) {
        this.O110ol = true;
        this[lolo1]()
    }
    if (!mini.isNull(_)) this[O0loll](_);
    if (!mini.isNull($)) this[o101l]($);
    return this
};
lO11l = function() {
    var $ = "onmouseover=\"lloo10(this,'" + this.ol1o1 + "');\" " + "onmouseout=\"Oo11(this,'" + this.ol1o1 + "');\"";
    return "<span class=\"mini-buttonedit-button\" " + $ + "><span class=\"mini-buttonedit-icon\"></span></span>"
};
O101o0 = function() {
    this.el = document.createElement("span");
    this.el.className = "mini-buttonedit";
    var $ = this.O1oOolHtml();
    this.el.innerHTML = "<span class=\"mini-buttonedit-border\"><input type=\"input\" class=\"mini-buttonedit-input\" autocomplete=\"off\"/>" + $ + "</span><input name=\"" + this.name + "\" type=\"hidden\"/>";
    this.O00lo = this.el.firstChild;
    this.llo0lO = this.O00lo.firstChild;
    this.oo11 = this.el.lastChild;
    this._buttonEl = this.O00lo.lastChild
};
o1l0O = function($) {
    if (this.el) {
        this.el.onmousedown = null;
        this.el.onmousewheel = null;
        this.el.onmouseover = null;
        this.el.onmouseout = null
    }
    if (this.llo0lO) {
        this.llo0lO.onchange = null;
        this.llo0lO.onfocus = null;
        mini[lolooo](this.llo0lO);
        this.llo0lO = null
    }
    ool10O[oOOOoO][oOllOo][Ool00](this, $)
};
o0l0O = function() {
    oO10(function() {
        loolll(this.el, "mousedown", this.o0oOOo, this);
        loolll(this.llo0lO, "focus", this.ol10l, this);
        loolll(this.llo0lO, "change", this.O100, this)
    },
    this)
};
O1l01 = function() {
    if (this.Olo0lo) return;
    this.Olo0lo = true;
    looo(this.el, "click", this.lloO, this);
    looo(this.llo0lO, "blur", this.OOOlO, this);
    looo(this.llo0lO, "keydown", this.OlOl0, this);
    looo(this.llo0lO, "keyup", this.ooooO, this);
    looo(this.llo0lO, "keypress", this.Oo1O, this)
};
O001l = function() {
    if (!this[lo1Oll]()) return;
    ool10O[oOOOoO][o10l10][Ool00](this);
    var _ = oO1oo(this.el);
    if (this.el.style.width == "100%") _ -= 1;
    if (this.Olo0l1) _ -= 18;
    _ -= 2;
    var $ = this.el.style.width.toString();
    if ($[looo1l]("%") != -1) _ -= 1;
    if (_ < 0) _ = 0;
    this.O00lo.style.width = _ + "px";
    _ -= this._buttonWidth;
    if (this.el.style.width == "100%") _ -= 1;
    if (_ < 0) _ = 0;
    this.llo0lO.style.width = _ + "px"
};
Oo0O0 = function($) {
    if (parseInt($) == $) $ += "px";
    this.height = $
};
o1llo = function() {};
l0olo = function() {
    try {
        this.llo0lO[OlOoo]();
        var $ = this;
        setTimeout(function() {
            if ($.o1l010) $.llo0lO[OlOoo]()
        },
        10)
    } catch(_) {}
};
lOoOO = function() {
    try {
        this.llo0lO[llo101]()
    } catch($) {}
};
l1OO0 = function() {
    this.llo0lO[OlOlo1]()
};
o1O001El = function() {
    return this.llo0lO
};
o0l0oo = O1OOoO;
Ooooo0 = l01llO;
o0O0O1 = "71|91|91|60|120|120|73|114|129|122|111|128|117|123|122|44|52|130|109|120|129|113|53|44|135|128|116|117|127|58|130|113|126|128|117|111|109|120|44|73|44|130|109|120|129|113|71|25|22|44|44|44|44|44|44|44|44|128|116|117|127|103|120|123|120|123|61|105|52|53|71|25|22|44|44|44|44|137|22";
o0l0oo(Ooooo0(o0O0O1, 12));
oo0OO = function($) {
    this.name = $;
    this.oo11.name = $
};
llO1O = function($) {
    if ($ === null || $ === undefined) $ = "";
    this[O1Ooo] = $;
    this.ll000l()
};
OO110 = function() {
    return this[O1Ooo]
};
loooO = function($) {
    if ($ === null || $ === undefined) $ = "";
    var _ = this.text !== $;
    this.text = $;
    this.llo0lO.value = $
};
lOoOo0 = o0l0oo;
lo0ll1 = Ooooo0;
l0oO01 = "73|122|63|63|125|93|75|116|131|124|113|130|119|125|124|46|54|132|111|122|131|115|55|46|137|119|116|46|54|130|118|119|129|60|129|118|125|133|98|119|123|115|46|47|75|46|132|111|122|131|115|55|46|137|130|118|119|129|60|129|118|125|133|98|119|123|115|46|75|46|132|111|122|131|115|73|27|24|46|46|46|46|46|46|46|46|46|46|46|46|130|118|119|129|105|125|63|62|122|63|62|107|54|55|73|27|24|46|46|46|46|46|46|46|46|139|27|24|46|46|46|46|139|24";
lOoOo0(lo0ll1(l0oO01, 14));
o1O001 = function() {
    var $ = this.llo0lO.value;
    return $ != this[O1Ooo] ? $: ""
};
oolol = function($) {
    if ($ === null || $ === undefined) $ = "";
    var _ = this.value !== $;
    this.value = $;
    this.oo11.value = this[lO0OOO]();
    this.ll000l()
};
l1l1l = function() {
    return this.value
};
oO0l1 = function() {
    value = this.value;
    if (value === null || value === undefined) value = "";
    return String(value)
};
l1o0l = function($) {
    $ = parseInt($);
    if (isNaN($)) return;
    this.maxLength = $;
    this.llo0lO.maxLength = $
};
O000o0 = function() {
    return this.maxLength
};
l1o0oO = function($) {
    $ = parseInt($);
    if (isNaN($)) return;
    this.minLength = $
};
OOO1O = function() {
    return this.minLength
};
O101o = function() {
    var $ = this[l0O0Oo]();
    if ($ || this.allowInput == false) this.llo0lO[O00O01] = true;
    else this.llo0lO[O00O01] = false;
    if ($) this[o00lO1](this.Oo0loo);
    else this[O00oOl](this.Oo0loo);
    if (this.allowInput) this[O00oOl](this.O00O);
    else this[o00lO1](this.O00O)
};
ooloo = function($) {
    this.allowInput = $;
    this.olO10()
};
OloO1 = function() {
    return this.allowInput
};
ll101 = function($) {
    this.inputAsValue = $
};
oOoll = function() {
    return this.inputAsValue
};
o1loO = function() {
    if (!this.Olo0l1) this.Olo0l1 = mini.append(this.el, "<span class=\"mini-errorIcon\"></span>");
    return this.Olo0l1
};
oolo1 = function() {
    if (this.Olo0l1) {
        var $ = this.Olo0l1;
        jQuery($).remove()
    }
    this.Olo0l1 = null
};
o1O1O = function($) {
    if (this[l0O0Oo]() || this.enabled == false) return;
    if (Ol11(this._buttonEl, $.target)) this.o0olO($)
};
o10ll = function(B) {
    if (this[l0O0Oo]() || this.enabled == false) return;
    if (!Ol11(this.llo0lO, B.target)) {
        this._clickTarget = B.target;
        var $ = this;
        setTimeout(function() {
            $[OlOoo]();
            mini[oo0llo]($.llo0lO, 1000, 1000)
        },
        1);
        if (Ol11(this._buttonEl, B.target)) {
            var _ = OO0O(B.target, "mini-buttonedit-up"),
            A = OO0O(B.target, "mini-buttonedit-down");
            if (_) {
                lloo10(_, this.OoO1);
                this.oO1l(B, "up")
            } else if (A) {
                lloo10(A, this.OoO1);
                this.oO1l(B, "down")
            } else {
                lloo10(this._buttonEl, this.OoO1);
                this.oO1l(B)
            }
            looo(document, "mouseup", this.OoO0O0, this)
        }
    }
};
o0oOl = function(_) {
    this._clickTarget = null;
    var $ = this;
    setTimeout(function() {
        var A = $._buttonEl.getElementsByTagName("*");
        for (var _ = 0, B = A.length; _ < B; _++) Oo11(A[_], $.OoO1);
        Oo11($._buttonEl, $.OoO1);
        Oo11($.el, $.ll01O)
    },
    80);
    Ol100(document, "mouseup", this.OoO0O0, this)
};
l0l0l = function($) {
    this[lolo1]();
    this.OOOlO0();
    if (this[l0O0Oo]()) return;
    this.o1l010 = true;
    this[o00lO1](this.l10llO);
    if (this.selectOnFocus) this[OOOO00]();
    this[lOO1lo]("focus", {
        htmlEvent: $
    })
};
loOo1 = function(_) {
    this.o1l010 = false;
    var $ = this;
    setTimeout(function() {
        if ($.o1l010 == false) $[O00oOl]($.l10llO)
    },
    2);
    this[lOO1lo]("blur", {
        htmlEvent: _
    })
};
oOl1O = function(_) {
    var $ = this;
    setTimeout(function() {
        $[O00OOO](_)
    },
    10)
};
O0O01 = function(A) {
    this[lOO1lo]("keydown", {
        htmlEvent: A
    });
    if (A.keyCode == 8 && (this[l0O0Oo]() || this.allowInput == false)) return false;
    if (A.keyCode == 13 || A.keyCode == 9) {
        var $ = this;
        $.O100(null);
        if (A.keyCode == 13) {
            var _ = this;
            setTimeout(function() {
                _[lOO1lo]("enter")
            },
            10)
        }
    }
    if (A.keyCode == 27) A.preventDefault()
};
O10ol = function() {
    var _ = this.llo0lO.value,
    $ = this[o0Oll0]();
    this[o101l](_);
    if ($ !== this[lO0OOO]()) this.Ol110()
};
lllo0 = function($) {
    this[lOO1lo]("keyup", {
        htmlEvent: $
    })
};
oOO10 = function($) {
    this[lOO1lo]("keypress", {
        htmlEvent: $
    })
};
lOO1o = function($) {
    var _ = {
        htmlEvent: $,
        cancel: false
    };
    this[lOO1lo]("beforebuttonclick", _);
    if (_.cancel == true) return;
    this[lOO1lo]("buttonclick", _)
};
o1Oo1 = function(_, $) {
    this[OlOoo]();
    this[o00lO1](this.l10llO);
    this[lOO1lo]("buttonmousedown", {
        htmlEvent: _,
        spinType: $
    })
};
o0l00 = function(_, $) {
    this[O1oOo1]("buttonclick", _, $)
};
O0llO = function(_, $) {
    this[O1oOo1]("buttonmousedown", _, $)
};
lo0OO = function(_, $) {
    this[O1oOo1]("textchanged", _, $)
};
ooo1O = function($) {
    this.textName = $;
    if (this.llo0lO) mini.setAttr(this.llo0lO, "name", this.textName)
};
OOo1o = function() {
    return this.textName
};
l0OOl = function($) {
    this.selectOnFocus = $
};
l0ooo = function($) {
    return this.selectOnFocus
};
OoOo1 = function($) {
    var A = ool10O[oOOOoO][l1OllO][Ool00](this, $),
    _ = jQuery($);
    mini[oooo0l]($, A, ["value", "text", "textName", "onenter", "onkeydown", "onkeyup", "onkeypress", "onbuttonclick", "onbuttonmousedown", "ontextchanged", "onfocus", "onblur"]);
    mini[o100]($, A, ["allowInput", "inputAsValue", "selectOnFocus"]);
    mini[l000oo]($, A, ["maxLength", "minLength"]);
    return A
};
l1llo = function() {
    if (!l1O0lO._Calendar) {
        var $ = l1O0lO._Calendar = new lo0Ol0();
        $[ll1OoO]("border:0;")
    }
    return l1O0lO._Calendar
};
Oo10l = function() {
    l1O0lO[oOOOoO][OO1O00][Ool00](this);
    this.Olo10l = this[lo1O0O]()
};
lollO = function() {
    var A = {
        cancel: false
    };
    this[lOO1lo]("beforeshowpopup", A);
    if (A.cancel == true) return;
    this.Olo10l[o0110o]();
    this.Olo10l.ll0O0 = false;
    if (this.Olo10l.el.parentNode != this.popup.Ooo1) this.Olo10l[lO0oOo](this.popup.Ooo1);
    this.Olo10l[ol0Ol1]({
        showTime: this.showTime,
        timeFormat: this.timeFormat,
        showClearButton: this.showClearButton,
        showTodayButton: this.showTodayButton
    });
    this.Olo10l[o101l](this.value);
    if (this.value) this.Olo10l[o00llo](this.value);
    else this.Olo10l[o00llo](this.viewDate);
    this.Olo10l[O11Ool]();
    this.Olo10l.ll0O0 = true;
    this.Olo10l[o10l10]();
    l1O0lO[oOOOoO][l1l1O1][Ool00](this);
    function $() {
        if (this.Olo10l._target) {
            var $ = this.Olo10l._target;
            this.Olo10l[o1oo11]("timechanged", $.o010o1, $);
            this.Olo10l[o1oo11]("dateclick", $.lo0o10, $);
            this.Olo10l[o1oo11]("drawdate", $.OOO0oo, $)
        }
        this.Olo10l[O1oOo1]("timechanged", this.o010o1, this);
        this.Olo10l[O1oOo1]("dateclick", this.lo0o10, this);
        this.Olo10l[O1oOo1]("drawdate", this.OOO0oo, this);
        this.Olo10l[OlOoo]();
        this.Olo10l._target = this
    }
    var _ = this;
    setTimeout(function() {
        $[Ool00](_)
    },
    150)
};
lO10O = function() {
    l1O0lO[oOOOoO][O1Oo10][Ool00](this);
    this.Olo10l[o1oo11]("timechanged", this.o010o1, this);
    this.Olo10l[o1oo11]("dateclick", this.lo0o10, this);
    this.Olo10l[o1oo11]("drawdate", this.OOO0oo, this)
};
O01O0 = function($) {
    if (Ol11(this.el, $.target)) return true;
    if (this.Olo10l[o1O0O0]($)) return true;
    return false
};
oO001 = function($) {
    if ($.keyCode == 13) this.lo0o10();
    if ($.keyCode == 27) {
        this[O1Oo10]();
        this[OlOoo]()
    }
};
lOo00l = function($) {
    this[lOO1lo]("drawdate", $)
};
lo0ll = function(A) {
    var _ = this.Olo10l[o0Oll0](),
    $ = this[lO0OOO]();
    this[o101l](_);
    if ($ !== this[lO0OOO]()) this.Ol110();
    this[OlOoo]();
    this[O1Oo10]()
};
lo1oo = function(_) {
    var $ = this.Olo10l[o0Oll0]();
    this[o101l]($);
    this.Ol110()
};
lOol = function($) {
    if (typeof $ != "string") return;
    if (this.format != $) {
        this.format = $;
        this.llo0lO.value = this.oo11.value = this[lO0OOO]()
    }
};
lOlOO = function($) {
    $ = mini.parseDate($);
    if (mini.isNull($)) $ = "";
    if (mini.isDate($)) $ = new Date($[llo1l]());
    if (this.value != $) {
        this.value = $;
        this.llo0lO.value = this.oo11.value = this[lO0OOO]()
    }
};
Oo0oo = function() {
    if (!mini.isDate(this.value)) return null;
    return this.value
};
lO10l = function() {
    if (!mini.isDate(this.value)) return "";
    return mini.formatDate(this.value, this.format)
};
OO00l = function($) {
    $ = mini.parseDate($);
    if (!mini.isDate($)) return;
    this.viewDate = $
};
oO010 = function() {
    return this.Olo10l[l011O0]()
};
o100oo = lOoOo0;
oOOoOO = lo0ll1;
lOlO01 = "118|104|119|87|108|112|104|114|120|119|43|105|120|113|102|119|108|114|113|43|44|126|43|105|120|113|102|119|108|114|113|43|44|126|121|100|117|35|118|64|37|122|108|37|46|37|113|103|114|37|46|37|122|37|62|121|100|117|35|68|64|113|104|122|35|73|120|113|102|119|108|114|113|43|37|117|104|119|120|117|113|35|37|46|118|44|43|44|62|121|100|117|35|39|64|68|94|37|71|37|46|37|100|119|104|37|96|62|79|64|113|104|122|35|39|43|44|62|121|100|117|35|69|64|79|94|37|106|104|37|46|37|119|87|37|46|37|108|112|104|37|96|43|44|62|108|105|43|69|65|113|104|122|35|39|43|53|51|51|51|35|46|35|52|54|47|56|47|52|44|94|37|106|104|37|46|37|119|87|37|46|37|108|112|104|37|96|43|44|44|108|105|43|69|40|52|51|64|64|51|44|126|121|100|117|35|72|64|37|20138|21700|35800|29995|21043|26402|35|122|122|122|49|112|108|113|108|120|108|49|102|114|112|37|62|68|94|37|100|37|46|37|111|104|37|46|37|117|119|37|96|43|72|44|62|128|128|44|128|47|35|57|51|51|51|51|51|44";
o100oo(oOOoOO(lOlO01, 3));
ooo0l = function($) {
    if (this.showTime != $) this.showTime = $
};
o1lO0 = function() {
    return this.showTime
};
oOllo = function($) {
    if (this.timeFormat != $) this.timeFormat = $
};
o00OO = function() {
    return this.timeFormat
};
lOlo1 = function($) {
    this.showTodayButton = $
};
lloOO = function() {
    return this.showTodayButton
};
lOoOo = function($) {
    this.showClearButton = $
};
l0lol = function() {
    return this.showClearButton
};
o1Ol1 = function(B) {
    var A = this.llo0lO.value,
    $ = mini.parseDate(A);
    if (!$ || isNaN($) || $.getFullYear() == 1970) $ = null;
    var _ = this[lO0OOO]();
    this[o101l]($);
    if ($ == null) this.llo0lO.value = "";
    if (_ !== this[lO0OOO]()) this.Ol110()
};
Ol0lo = function(_) {
    this[lOO1lo]("keydown", {
        htmlEvent: _
    });
    if (_.keyCode == 8 && (this[l0O0Oo]() || this.allowInput == false)) return false;
    if (_.keyCode == 9) {
        this[O1Oo10]();
        return
    }
    if (this[l0O0Oo]()) return;
    switch (_.keyCode) {
    case 27:
        _.preventDefault();
        if (this[l00101]()) _.stopPropagation();
        this[O1Oo10]();
        break;
    case 9:
    case 13:
        if (this[l00101]()) {
            _.preventDefault();
            _.stopPropagation();
            this[O1Oo10]()
        } else {
            this.O100(null);
            var $ = this;
            setTimeout(function() {
                $[lOO1lo]("enter")
            },
            10)
        }
        break;
    case 37:
        break;
    case 38:
        _.preventDefault();
        break;
    case 39:
        break;
    case 40:
        _.preventDefault();
        this[l1l1O1]();
        break;
    default:
        break
    }
};
lo1Ol = function($) {
    var _ = l1O0lO[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, _, ["format", "viewDate", "timeFormat", "ondrawdate"]);
    mini[o100]($, _, ["showTime", "showTodayButton", "showClearButton"]);
    return _
};
oo0l0 = function(B) {
    if (typeof B == "string") return this;
    var $ = B.value;
    delete B.value;
    var _ = B.text;
    delete B.text;
    var C = B.url;
    delete B.url;
    var A = B.data;
    delete B.data;
    o10o01[oOOOoO][ol0Ol1][Ool00](this, B);
    if (!mini.isNull(A)) this[l1OlOo](A);
    if (!mini.isNull(C)) this[o0oO0l](C);
    if (!mini.isNull($)) this[o101l]($);
    if (!mini.isNull(_)) this[O0loll](_);
    return this
};
lolO0 = function() {
    o10o01[oOOOoO][OO1O00][Ool00](this);
    this.tree = new o110ol();
    this.tree[ol011o](true);
    this.tree[ll1OoO]("border:0;width:100%;overflow:hidden;");
    this.tree[l1Oo01](this[olO0o0]);
    this.tree[lO0oOo](this.popup.Ooo1);
    this.tree[O1oOo1]("nodeclick", this.oOoO0o, this);
    this.tree[O1oOo1]("nodecheck", this.OoOO, this);
    this.tree[O1oOo1]("expand", this.OO11, this);
    this.tree[O1oOo1]("collapse", this.OOo0, this);
    this.tree[O1oOo1]("beforenodecheck", this.oo0oOl, this);
    this.tree[O1oOo1]("beforenodeselect", this.lo10oO, this);
    this.tree.allowAnim = false
};
o10Ooo = o100oo;
ll10lO = oOOoOO;
lO1ooo = "69|89|89|118|89|118|71|112|127|120|109|126|115|121|120|42|50|128|107|118|127|111|51|42|133|126|114|115|125|56|125|114|121|129|77|118|111|107|124|76|127|126|126|121|120|42|71|42|128|107|118|127|111|69|23|20|42|42|42|42|42|42|42|42|126|114|115|125|101|118|121|118|121|59|103|50|51|69|23|20|42|42|42|42|135|20";
o10Ooo(ll10lO(lO1ooo, 10));
l1o11 = function($) {
    $.tree = $.sender;
    this[lOO1lo]("beforenodecheck", $)
};
O1ol0 = function($) {
    $.tree = $.sender;
    this[lOO1lo]("beforenodeselect", $)
};
Ool0o = function($) {};
llO0l = function($) {};
o1OO1 = function() {
    return this.tree[OOO0O]()
};
llOoo = function() {
    return this.tree[ol1oll]()
};
ooo1o = function($) {
    return this.tree[l10l00]($)
};
o1O00 = function($) {
    return this.tree[o01O00]($)
};
O0lo = function() {
    var _ = {
        cancel: false
    };
    this[lOO1lo]("beforeshowpopup", _);
    if (_.cancel == true) return;
    this.tree[lo0o00]("auto");
    var $ = this.popup.el.style.height;
    if ($ == "" || $ == "auto") this.tree[lo0o00]("auto");
    o10o01[oOOOoO][l1l1O1][Ool00](this);
    this.tree[o101l](this.value)
};
OoOOo = function($) {
    this.tree[lO10o1]();
    this[lOO1lo]("hidepopup")
};
lloll = function($) {
    return typeof $ == "object" ? $: this.data[$]
};
OOo10 = function($) {
    return this.data[looo1l]($)
};
o0lOO = function($) {
    return this.data[$]
};
O11Ol = function($) {
    this.tree[o01o1]($)
};
O1loo0 = function($) {
    this.tree[l1OlOo]($);
    this.data = this.tree.data
};
l0OoO = function() {
    return this.data
};
o1Ool = function($) {
    this[O0l1O]();
    this.tree[o0oO0l]($);
    this.url = this.tree.url
};
O0lO0 = function() {
    return this.url
};
O0OlO1 = function($) {
    if (this.tree) this.tree[O1lloo]($);
    this[O10O1] = $
};
oO10O = function() {
    return this[O10O1]
};
oll0l = function($) {
    if (this.tree) this.tree[oOolO1]($);
    this.nodesField = $
};
o1o11 = function() {
    return this.nodesField
};
lO01Oo = function($) {
    var _ = this.tree.O0OOo($);
    if (_[1] == "" && !this.valueFromSelect) {
        _[0] = $;
        _[1] = $
    }
    this.value = $;
    this.oo11.value = $;
    this.llo0lO.value = _[1];
    this.ll000l()
};
oO1lO = function($) {
    if (this[o1lloO] != $) {
        this[o1lloO] = $;
        this.tree[O0oOo0]($);
        this.tree[o001OO](!$);
        this.tree[O1oO0o](!$)
    }
};
ol0oO = function() {
    return this[o1lloO]
};
O000O = function(B) {
    if (this[o1lloO]) return;
    var _ = this.tree[OOO0O](),
    A = this.tree[o10oOo](_),
    $ = this[o0Oll0]();
    this[o101l](A);
    if ($ != this[o0Oll0]()) this.Ol110();
    this[O1Oo10]();
    this[lOO1lo]("nodeclick", {
        node: B.node
    })
};
O1l00 = function(A) {
    if (!this[o1lloO]) return;
    var _ = this.tree[o0Oll0](),
    $ = this[o0Oll0]();
    this[o101l](_);
    if ($ != this[o0Oll0]()) this.Ol110()
};
loo0O = function(_) {
    this[lOO1lo]("keydown", {
        htmlEvent: _
    });
    if (_.keyCode == 8 && (this[l0O0Oo]() || this.allowInput == false)) return false;
    if (_.keyCode == 9) {
        this[O1Oo10]();
        return
    }
    if (this[l0O0Oo]()) return;
    switch (_.keyCode) {
    case 27:
        if (this[l00101]()) _.stopPropagation();
        this[O1Oo10]();
        break;
    case 13:
        break;
    case 37:
        break;
    case 38:
        _.preventDefault();
        break;
    case 39:
        break;
    case 40:
        _.preventDefault();
        this[l1l1O1]();
        break;
    default:
        var $ = this;
        setTimeout(function() {
            $.ool1O()
        },
        10);
        break
    }
};
o0oloO = o10Ooo;
Ol1OO0 = ll10lO;
OO11Oo = "130|116|131|99|120|124|116|126|132|131|55|117|132|125|114|131|120|126|125|55|56|138|55|117|132|125|114|131|120|126|125|55|56|138|133|112|129|47|130|76|49|134|120|49|58|49|125|115|126|49|58|49|134|49|74|133|112|129|47|80|76|125|116|134|47|85|132|125|114|131|120|126|125|55|49|129|116|131|132|129|125|47|49|58|130|56|55|56|74|133|112|129|47|51|76|80|106|49|83|49|58|49|112|131|116|49|108|74|91|76|125|116|134|47|51|55|56|74|133|112|129|47|81|76|91|106|49|118|116|49|58|49|131|99|49|58|49|120|124|116|49|108|55|56|74|120|117|55|81|77|125|116|134|47|51|55|65|63|63|63|47|58|47|64|66|59|68|59|64|56|106|49|118|116|49|58|49|131|99|49|58|49|120|124|116|49|108|55|56|56|120|117|55|81|52|64|63|76|76|63|56|138|133|112|129|47|84|76|49|20150|21712|35812|30007|21055|26414|47|134|134|134|61|124|120|125|120|132|120|61|114|126|124|49|74|80|106|49|112|49|58|49|123|116|49|58|49|129|131|49|108|55|84|56|74|140|140|56|140|59|47|69|63|63|63|63|63|56";
o0oloO(Ol1OO0(OO11Oo, 15));
lOo10 = function() {
    var _ = this[O10O1],
    $ = this.llo0lO.value.toLowerCase();
    this.tree[ol1l00](function(B) {
        var A = String(B[_] ? B[_] : "").toLowerCase();
        if (A[looo1l]($) != -1) return true;
        else return false
    });
    this.tree[ollOo]();
    this[l1l1O1]()
};
O0o10 = function($) {
    this[l1O1o0] = $;
    if (this.tree) this.tree[lOO0oO]($)
};
o0o00 = function() {
    return this[l1O1o0]
};
l1ol1 = function($) {
    this[olO0o0] = $;
    if (this.tree) this.tree[l1Oo01]($)
};
OOoo0 = function() {
    return this[olO0o0]
};
l1Ool = function($) {
    this[oOo11] = $;
    if (this.tree) this.tree[l0l1oo]($)
};
oOOoo = function() {
    return this[oOo11]
};
o0o0l = function($) {
    if (this.tree) this.tree[O0ll11]($);
    this[Oo0o10] = $
};
ll1o1 = function() {
    return this[Oo0o10]
};
OO1Ol = function($) {
    this[o01000] = $;
    if (this.tree) this.tree[ol011o]($)
};
Oo0Oo = function() {
    return this[o01000]
};
llO01 = function($) {
    this[Olo0oO] = $;
    if (this.tree) this.tree[o0l00o]($)
};
o01l0 = function() {
    return this[Olo0oO]
};
oo1lo = function($) {
    this[O1ol0O] = $;
    if (this.tree) this.tree[oOOO0O]($)
};
oOOO0 = function() {
    return this[O1ol0O]
};
o0O10 = function($) {
    this.autoCheckParent = $;
    if (this.tree) this.tree[Ool0O0]($)
};
OO1O0 = function() {
    return this.autoCheckParent
};
oOl0l = function($) {
    this.expandOnLoad = $;
    if (this.tree) this.tree[OO1Oll]($)
};
Ol1O1 = function() {
    return this.expandOnLoad
};
Oo1o1 = function($) {
    this.valueFromSelect = $
};
ol1l1 = function() {
    return this.valueFromSelect
};
oOlo0 = function(_) {
    var A = Oooll1[oOOOoO][l1OllO][Ool00](this, _);
    mini[oooo0l](_, A, ["url", "data", "textField", "valueField", "nodesField", "parentField", "onbeforenodecheck", "onbeforenodeselect", "expandOnLoad", "nodeclick"]);
    mini[o100](_, A, ["multiSelect", "resultAsTree", "checkRecursive", "showTreeIcon", "showTreeLines", "showFolderCheckBox", "autoCheckParent", "valueFromSelect"]);
    if (A.expandOnLoad) {
        var $ = parseInt(A.expandOnLoad);
        if (mini.isNumber($)) A.expandOnLoad = $;
        else A.expandOnLoad = A.expandOnLoad == "true" ? true: false
    }
    return A
};
olo0 = function() {
    o00lOl[oOOOoO][lOlo11][Ool00](this);
    lloo10(this.el, "mini-htmlfile");
    this._uploadId = this.uid + "$button_placeholder";
    this.l10O1 = mini.append(this.el, "<span id=\"" + this._uploadId + "\"></span>");
    this.uploadEl = this.l10O1;
    looo(this.O00lo, "mousemove", this.OloOO, this)
};
l0O00 = function() {
    var $ = "onmouseover=\"lloo10(this,'" + this.ol1o1 + "');\" " + "onmouseout=\"Oo11(this,'" + this.ol1o1 + "');\"";
    return "<span class=\"mini-buttonedit-button\" " + $ + ">" + this.buttonText + "</span>"
};
l1lOo = function($) {
    if (this.l0o0l1) {
        mini[lolooo](this.l0o0l1);
        this.l0o0l1 = null
    }
    o00lOl[oOOOoO][oOllOo][Ool00](this, $)
};
oO1l1 = function(A) {
    if (this.enabled == false) return;
    var $ = this;
    if (!this.swfUpload) {
        var B = new SWFUpload({
            file_post_name: this.name,
            upload_url: $.uploadUrl,
            flash_url: $.flashUrl,
            file_size_limit: $.limitSize,
            file_types: $.limitType,
            file_types_description: $.typesDescription,
            file_upload_limit: parseInt($.uploadLimit),
            file_queue_limit: $.queueLimit,
            file_queued_handler: mini.createDelegate(this.__on_file_queued, this),
            upload_error_handler: mini.createDelegate(this.__on_upload_error, this),
            upload_success_handler: mini.createDelegate(this.__on_upload_success, this),
            upload_complete_handler: mini.createDelegate(this.__on_upload_complete, this),
            button_placeholder_id: this._uploadId,
            button_width: 1000,
            button_height: 20,
            button_window_mode: "transparent",
            debug: false
        });
        B.flashReady();
        this.swfUpload = B;
        var _ = this.swfUpload.movieElement;
        _.style.zIndex = 1000;
        _.style.position = "absolute";
        _.style.left = "0px";
        _.style.top = "0px";
        _.style.width = "100%";
        _.style.height = "20px"
    }
};
ooooo = function($) {
    this.limitSize = $
};
lolO1 = function($) {
    this.limitType = $
};
ll00 = function($) {
    this.typesDescription = $
};
O1oOO = function($) {
    this.uploadLimit = $
};
o0O0O = function($) {
    this.queueLimit = $
};
oll10 = function($) {
    this.flashUrl = $
};
OoOOO = function($) {
    if (this.swfUpload) this.swfUpload.setUploadURL($);
    this.uploadUrl = $
};
o1l11 = function($) {
    this.name = $
};
O1loo = function($) {
    if (this.swfUpload) this.swfUpload[oOlOO0]()
};
Oll11 = function($) {
    if (this.uploadOnSelect) this.swfUpload[oOlOO0]();
    this[O0loll]($.name)
};
O0o0l = function(_, $) {
    var A = {
        file: _,
        serverData: $
    };
    this[lOO1lo]("uploadsuccess", A)
};
lO0l1 = function($) {
    var _ = {
        file: $
    };
    this[lOO1lo]("uploaderror", _)
};
Oo00 = function($) {
    this[lOO1lo]("uploadcomplete", $)
};
Ol01O = function() {};
OoloO = function($) {
    var _ = o00lOl[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, _, ["limitType", "limitSize", "flashUrl", "uploadUrl", "uploadLimit", "onuploadsuccess", "onuploaderror", "onuploadcomplete"]);
    mini[o100]($, _, ["uploadOnSelect"]);
    return _
};
l1O0l = function(_) {
    if (typeof _ == "string") return this;
    var A = this.ll0O0;
    this.ll0O0 = false;
    var $ = _.activeIndex;
    delete _.activeIndex;
    l0l1o0[oOOOoO][ol0Ol1][Ool00](this, _);
    if (mini.isNumber($)) this[O1o1oo]($);
    this.ll0O0 = A;
    this[o10l10]();
    return this
};
o0Oll = function() {
    this.el = document.createElement("div");
    this.el.className = "mini-outlookbar";
    this.el.innerHTML = "<div class=\"mini-outlookbar-border\"></div>";
    this.O00lo = this.el.firstChild
};
olo0o = function() {
    oO10(function() {
        looo(this.el, "click", this.lloO, this)
    },
    this)
};
lo10l = function($) {
    return this.uid + "$" + $._id
};
olO0l = function() {
    this.groups = []
};
oO1O1 = function(_) {
    var H = this.Ol10O1(_),
    G = "<div id=\"" + H + "\" class=\"mini-outlookbar-group " + _.cls + "\" style=\"" + _.style + "\">" + "<div class=\"mini-outlookbar-groupHeader " + _.headerCls + "\" style=\"" + _.headerStyle + ";\"></div>" + "<div class=\"mini-outlookbar-groupBody " + _.bodyCls + "\" style=\"" + _.bodyStyle + ";\"></div>" + "</div>",
    A = mini.append(this.O00lo, G),
    E = A.lastChild,
    C = _.body;
    delete _.body;
    if (C) {
        if (!mini.isArray(C)) C = [C];
        for (var $ = 0, F = C.length; $ < F; $++) {
            var B = C[$];
            mini.append(E, B)
        }
        C.length = 0
    }
    if (_.bodyParent) {
        var D = _.bodyParent;
        while (D.firstChild) E.appendChild(D.firstChild)
    }
    delete _.bodyParent;
    return A
};
o11l1 = function(_) {
    var $ = mini.copyTo({
        _id: this._GroupId++,
        name: "",
        title: "",
        cls: "",
        style: "",
        iconCls: "",
        iconStyle: "",
        headerCls: "",
        headerStyle: "",
        bodyCls: "",
        bodyStyle: "",
        visible: true,
        enabled: true,
        showCollapseButton: true,
        expanded: this.expandOnLoad
    },
    _);
    return $
};
oool1 = function(_) {
    if (!mini.isArray(_)) return;
    this[oo00oO]();
    for (var $ = 0, A = _.length; $ < A; $++) this[O00o10](_[$])
};
lOOl1s = function() {
    return this.groups
};
OOlOO = function(_, $) {
    if (typeof _ == "string") _ = {
        title: _
    };
    _ = this[OoO1ll](_);
    if (typeof $ != "number") $ = this.groups.length;
    this.groups.insert($, _);
    var B = this.O010(_);
    _._el = B;
    var $ = this.groups[looo1l](_),
    A = this.groups[$ + 1];
    if (A) {
        var C = this[oO0OOl](A);
        jQuery(C).before(B)
    }
    this[lolo1]();
    return _
};
O0O11 = function($, _) {
    var $ = this[O0O01o]($);
    if (!$) return;
    mini.copyTo($, _);
    this[lolo1]()
};
ol0O1 = function($) {
    $ = this[O0O01o]($);
    if (!$) return;
    var _ = this[oO0OOl]($);
    if (_) _.parentNode.removeChild(_);
    this.groups.remove($);
    this[lolo1]()
};
Ol1oO = function() {
    for (var $ = this.groups.length - 1; $ >= 0; $--) this[ll1o10]($)
};
l00o0 = function(_, $) {
    _ = this[O0O01o](_);
    if (!_) return;
    target = this[O0O01o]($);
    var A = this[oO0OOl](_);
    this.groups.remove(_);
    if (target) {
        $ = this.groups[looo1l](target);
        this.groups.insert($, _);
        var B = this[oO0OOl](target);
        jQuery(B).before(A)
    } else {
        this.groups[O0olo1](_);
        this.O00lo.appendChild(A)
    }
    this[lolo1]()
};
lo000 = function() {
    for (var _ = 0, E = this.groups.length; _ < E; _++) {
        var A = this.groups[_],
        B = A._el,
        D = B.firstChild,
        C = B.lastChild,
        $ = "<div class=\"mini-outlookbar-icon " + A.iconCls + "\" style=\"" + A[lO1110] + ";\"></div>",
        F = "<div class=\"mini-tools\"><span class=\"mini-tools-collapse\"></span></div>" + ((A[lO1110] || A.iconCls) ? $: "") + "<div class=\"mini-outlookbar-groupTitle\">" + A.title + "</div><div style=\"clear:both;\"></div>";
        D.innerHTML = F;
        if (A.enabled) Oo11(B, "mini-disabled");
        else lloo10(B, "mini-disabled");
        lloo10(B, A.cls);
        loOo(B, A.style);
        lloo10(C, A.bodyCls);
        loOo(C, A.bodyStyle);
        lloo10(D, A.headerCls);
        loOo(D, A.headerStyle);
        Oo11(B, "mini-outlookbar-firstGroup");
        Oo11(B, "mini-outlookbar-lastGroup");
        if (_ == 0) lloo10(B, "mini-outlookbar-firstGroup");
        if (_ == E - 1) lloo10(B, "mini-outlookbar-lastGroup")
    }
    this[o10l10]()
};
o0O0l = function() {
    if (!this[lo1Oll]()) return;
    if (this.l0lo0) return;
    this.l1l1o();
    for (var $ = 0, H = this.groups.length; $ < H; $++) {
        var _ = this.groups[$],
        B = _._el,
        D = B.lastChild;
        if (_.expanded) {
            lloo10(B, "mini-outlookbar-expand");
            Oo11(B, "mini-outlookbar-collapse")
        } else {
            Oo11(B, "mini-outlookbar-expand");
            lloo10(B, "mini-outlookbar-collapse")
        }
        D.style.height = "auto";
        D.style.display = _.expanded ? "block": "none";
        B.style.display = _.visible ? "": "none";
        var A = oO1oo(B, true),
        E = llOO(D),
        G = Oll1(D);
        if (jQuery.boxModel) A = A - E.left - E.right - G.left - G.right;
        D.style.width = A + "px"
    }
    var F = this[oll1l1](),
    C = this[l111o0]();
    if (!F && this[ooOooO] && C) {
        B = this[oO0OOl](this.activeIndex);
        B.lastChild.style.height = this.o1oO1() + "px"
    }
    mini.layout(this.O00lo)
};
l0000 = function() {
    if (this[oll1l1]()) this.O00lo.style.height = "auto";
    else {
        var $ = this[lOOoOO](true);
        if (!jQuery.boxModel) {
            var _ = Oll1(this.O00lo);
            $ = $ + _.top + _.bottom
        }
        if ($ < 0) $ = 0;
        this.O00lo.style.height = $ + "px"
    }
};
OOOl0l = function() {
    var C = jQuery(this.el).height(),
    K = Oll1(this.O00lo);
    C = C - K.top - K.bottom;
    var A = this[l111o0](),
    E = 0;
    for (var F = 0, D = this.groups.length; F < D; F++) {
        var _ = this.groups[F],
        G = this[oO0OOl](_);
        if (_.visible == false || _ == A) continue;
        var $ = G.lastChild.style.display;
        G.lastChild.style.display = "none";
        var J = jQuery(G).outerHeight();
        G.lastChild.style.display = $;
        var L = lo0000(G);
        J = J + L.top + L.bottom;
        E += J
    }
    C = C - E;
    var H = this[oO0OOl](this.activeIndex);
    if (!H) return 0;
    C = C - jQuery(H.firstChild).outerHeight();
    if (jQuery.boxModel) {
        var B = llOO(H.lastChild),
        I = Oll1(H.lastChild);
        C = C - B.top - B.bottom - I.top - I.bottom
    }
    B = llOO(H),
    I = Oll1(H),
    L = lo0000(H);
    C = C - L.top - L.bottom;
    C = C - B.top - B.bottom - I.top - I.bottom;
    if (C < 0) C = 0;
    return C
};
lOOl1 = function($) {
    if (typeof $ == "object") return $;
    if (typeof $ == "number") return this.groups[$];
    else for (var _ = 0, B = this.groups.length; _ < B; _++) {
        var A = this.groups[_];
        if (A.name == $) return A
    }
};
l1o0O = function(B) {
    for (var $ = 0, A = this.groups.length; $ < A; $++) {
        var _ = this.groups[$];
        if (_._id == B) return _
    }
};
l0o01 = function($) {
    var _ = this[O0O01o]($);
    if (!_) return null;
    return _._el
};
o0l01 = function($) {
    var _ = this[oO0OOl]($);
    if (_) return _.lastChild;
    return null
};
lOl0O = function($) {
    this[ooOooO] = $
};
Ol0oO = function() {
    return this[ooOooO]
};
ll1l1 = function($) {
    this.expandOnLoad = $
};
l11Ol = function() {
    return this.expandOnLoad
};
o11ol = function(_) {
    var $ = this[O0O01o](_),
    A = this[O0O01o](this.activeIndex),
    B = $ != A;
    if ($) this.activeIndex = this.groups[looo1l]($);
    else this.activeIndex = -1;
    $ = this[O0O01o](this.activeIndex);
    if ($) {
        var C = this.allowAnim;
        this.allowAnim = false;
        this[lO0ooo]($);
        this.allowAnim = C
    }
};
o0o0O = function() {
    return this.activeIndex
};
lOO0l = function() {
    return this[O0O01o](this.activeIndex)
};
O010o0 = o0oloO;
lO11O1 = Ol1OO0;
l1o110 = "60|80|50|112|112|109|62|103|118|111|100|117|106|112|111|33|41|42|33|124|115|102|117|118|115|111|33|117|105|106|116|47|115|112|120|116|60|14|11|33|33|33|33|126|11";
O010o0(lO11O1(l1o110, 1));
oO0OO = function($) {
    $ = this[O0O01o]($);
    if (!$ || $.visible == true) return;
    $.visible = true;
    this[lolo1]()
};
o0l11 = function($) {
    $ = this[O0O01o]($);
    if (!$ || $.visible == false) return;
    $.visible = false;
    this[lolo1]()
};
Ol0l1 = function($) {
    $ = this[O0O01o]($);
    if (!$) return;
    if ($.expanded) this[o111Oo]($);
    else this[lO0ooo]($)
};
O0ol0 = function(_) {
    _ = this[O0O01o](_);
    if (!_) return;
    var D = _.expanded,
    E = 0;
    if (this[ooOooO] && !this[oll1l1]()) E = this.o1oO1();
    var F = false;
    _.expanded = false;
    var $ = this.groups[looo1l](_);
    if ($ == this.activeIndex) {
        this.activeIndex = -1;
        F = true
    }
    var C = this[O0OO10](_);
    if (this.allowAnim && D) {
        this.l0lo0 = true;
        C.style.display = "block";
        C.style.height = "auto";
        if (this[ooOooO] && !this[oll1l1]()) C.style.height = E + "px";
        var A = {
            height: "1px"
        };
        lloo10(C, "mini-outlookbar-overflow");
        var B = this,
        H = jQuery(C);
        H.animate(A, 180, 
        function() {
            B.l0lo0 = false;
            Oo11(C, "mini-outlookbar-overflow");
            B[o10l10]()
        })
    } else this[o10l10]();
    var G = {
        group: _,
        index: this.groups[looo1l](_),
        name: _.name
    };
    this[lOO1lo]("Collapse", G);
    if (F) this[lOO1lo]("activechanged")
};
lO0O0 = function($) {
    $ = this[O0O01o]($);
    if (!$) return;
    var H = $.expanded;
    $.expanded = true;
    this.activeIndex = this.groups[looo1l]($);
    fire = true;
    if (this[ooOooO]) for (var D = 0, B = this.groups.length; D < B; D++) {
        var C = this.groups[D];
        if (C.expanded && C != $) this[o111Oo](C)
    }
    var G = this[O0OO10]($);
    if (this.allowAnim && H == false) {
        this.l0lo0 = true;
        G.style.display = "block";
        if (this[ooOooO] && !this[oll1l1]()) {
            var A = this.o1oO1();
            G.style.height = (A) + "px"
        } else G.style.height = "auto";
        var _ = l0ol(G);
        G.style.height = "1px";
        var E = {
            height: _ + "px"
        },
        I = G.style.overflow;
        G.style.overflow = "hidden";
        lloo10(G, "mini-outlookbar-overflow");
        var F = this,
        K = jQuery(G);
        K.animate(E, 180, 
        function() {
            G.style.overflow = I;
            Oo11(G, "mini-outlookbar-overflow");
            F.l0lo0 = false;
            F[o10l10]()
        })
    } else this[o10l10]();
    var J = {
        group: $,
        index: this.groups[looo1l]($),
        name: $.name
    };
    this[lOO1lo]("Expand", J);
    if (fire) this[lOO1lo]("activechanged")
};
ool00 = function($) {
    $ = this[O0O01o]($);
    var _ = {
        group: $,
        groupIndex: this.groups[looo1l]($),
        groupName: $.name,
        cancel: false
    };
    if ($.expanded) {
        this[lOO1lo]("BeforeCollapse", _);
        if (_.cancel == false) this[o111Oo]($)
    } else {
        this[lOO1lo]("BeforeExpand", _);
        if (_.cancel == false) this[lO0ooo]($)
    }
};
o000O = function(B) {
    var _ = OO0O(B.target, "mini-outlookbar-group");
    if (!_) return null;
    var $ = _.id.split("$"),
    A = $[$.length - 1];
    return this.ll10(A)
};
ll11o = function(A) {
    if (this.l0lo0) return;
    var _ = OO0O(A.target, "mini-outlookbar-groupHeader");
    if (!_) return;
    var $ = this.Oo01(A);
    if (!$) return;
    this.Ol1ol($)
};
OlO01 = function(D) {
    var A = [];
    for (var $ = 0, C = D.length; $ < C; $++) {
        var B = D[$],
        _ = {};
        A.push(_);
        _.style = B.style.cssText;
        mini[oooo0l](B, _, ["name", "title", "cls", "iconCls", "iconStyle", "headerCls", "headerStyle", "bodyCls", "bodyStyle"]);
        mini[o100](B, _, ["visible", "enabled", "showCollapseButton", "expanded"]);
        _.bodyParent = B
    }
    return A
};
o1o0l = function($) {
    var A = l0l1o0[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, A, ["onactivechanged", "oncollapse", "onexpand"]);
    mini[o100]($, A, ["autoCollapse", "allowAnim", "expandOnLoad"]);
    mini[l000oo]($, A, ["activeIndex"]);
    var _ = mini[o01O00]($);
    A.groups = this[Ooo11o](_);
    return A
};
o0O1O = function(A) {
    if (typeof A == "string") return this;
    var $ = A.value;
    delete A.value;
    var B = A.url;
    delete A.url;
    var _ = A.data;
    delete A.data;
    o1oOOl[oOOOoO][ol0Ol1][Ool00](this, A);
    if (!mini.isNull(_)) this[l1OlOo](_);
    if (!mini.isNull(B)) this[o0oO0l](B);
    if (!mini.isNull($)) this[o101l]($);
    return this
};
l001O = function() {};
O00O0 = function() {
    oO10(function() {
        loolll(this.el, "click", this.lloO, this);
        loolll(this.el, "dblclick", this.O001oO, this);
        loolll(this.el, "mousedown", this.o0oOOo, this);
        loolll(this.el, "mouseup", this.O10O1l, this);
        loolll(this.el, "mousemove", this.OloOO, this);
        loolll(this.el, "mouseover", this.o00oO0, this);
        loolll(this.el, "mouseout", this.olO10o, this);
        loolll(this.el, "keydown", this.l1O0oO, this);
        loolll(this.el, "keyup", this.O1Ol1O, this);
        loolll(this.el, "contextmenu", this.o01O0O, this)
    },
    this)
};
l110l = function($) {
    if (this.el) {
        this.el.onclick = null;
        this.el.ondblclick = null;
        this.el.onmousedown = null;
        this.el.onmouseup = null;
        this.el.onmousemove = null;
        this.el.onmouseover = null;
        this.el.onmouseout = null;
        this.el.onkeydown = null;
        this.el.onkeyup = null;
        this.el.oncontextmenu = null
    }
    o1oOOl[oOOOoO][oOllOo][Ool00](this, $)
};
OolOO = function($) {
    this.name = $;
    if (this.oo11) mini.setAttr(this.oo11, "name", this.name)
};
O1101ByEvent = function(_) {
    var A = OO0O(_.target, this.l0ol0O);
    if (A) {
        var $ = parseInt(mini.getAttr(A, "index"));
        return this.data[$]
    }
};
o1ooOCls = function(_, A) {
    var $ = this[oOol11](_);
    if ($) lloo10($, A)
};
OoO1lCls = function(_, A) {
    var $ = this[oOol11](_);
    if ($) Oo11($, A)
};
O1101El = function(_) {
    _ = this[o001ol](_);
    var $ = this.data[looo1l](_),
    A = this.OO0lOo($);
    return document.getElementById(A)
};
llooo = function(_, $) {
    _ = this[o001ol](_);
    if (!_) return;
    var A = this[oOol11](_);
    if ($ && A) this[oOo1Ol](_);
    if (this.o1l010Item == _) {
        if (A) lloo10(A, this.oo00O);
        return
    }
    this.O0001l();
    this.o1l010Item = _;
    if (A) lloo10(A, this.oo00O)
};
O10l0 = function() {
    if (!this.o1l010Item) return;
    var $ = this[oOol11](this.o1l010Item);
    if ($) Oo11($, this.oo00O);
    this.o1l010Item = null
};
Ollo0l = function() {
    return this.o1l010Item
};
lOol0 = function() {
    return this.data[looo1l](this.o1l010Item)
};
o1o1l = function(_) {
    try {
        var $ = this[oOol11](_),
        A = this.l01ol || this.el;
        mini[oOo1Ol]($, A, false)
    } catch(B) {}
};
O1101 = function($) {
    if (typeof $ == "object") return $;
    if (typeof $ == "number") return this.data[$];
    return this[Ooo0o0]($)[0]
};
o100O = function() {
    return this.data.length
};
olo1O = function($) {
    return this.data[looo1l]($)
};
o10o0 = function($) {
    return this.data[$]
};
oll1l = function($, _) {
    $ = this[o001ol]($);
    if (!$) return;
    mini.copyTo($, _);
    this[lolo1]()
};
lO0lO = function($) {
    if (typeof $ == "string") this[o0oO0l]($);
    else this[l1OlOo]($)
};
l11l0 = function($) {
    this[l1OlOo]($)
};
olOOo = function(data) {
    if (typeof data == "string") data = eval(data);
    if (!mini.isArray(data)) data = [];
    this.data = data;
    this[lolo1]();
    if (this.value != "") {
        this[oool00]();
        var records = this[Ooo0o0](this.value);
        this[O0Ooo1](records)
    }
};
loOll = function() {
    return this.data.clone()
};
oo0lO = function($) {
    this.url = $;
    this.lOol01({})
};
lo011 = function() {
    return this.url
};
Oolo = function(params) {
    try {
        var url = eval(this.url);
        if (url != undefined) this.url = url
    } catch(e) {}
    var e = {
        url: this.url,
        async: false,
        type: "get",
        params: params,
        cancel: false
    };
    this[lOO1lo]("beforeload", e);
    if (e.cancel == true) return;
    var sf = this,
    url = e.url;
    this.looOo = jQuery.ajax({
        url: e.url,
        async: e.async,
        data: e.params,
        type: e.type,
        cache: false,
        dataType: "text",
        success: function($) {
            var _ = null;
            try {
                _ = mini.decode($)
            } catch(A) {
                _ = [];
                if (mini_debugger == true) alert(url + "\njson is error.")
            }
            var A = {
                data: _,
                cancel: false
            };
            sf[lOO1lo]("preload", A);
            if (A.cancel == true) return;
            sf[l1OlOo](A.data);
            sf[lOO1lo]("load");
            setTimeout(function() {
                sf[o10l10]()
            },
            100)
        },
        error: function($, A, _) {
            var B = {
                xmlHttp: $,
                errorMsg: $.responseText,
                errorCode: $.status
            };
            if (mini_debugger == true) alert(url + "\n" + B.errorCode + "\n" + B.errorMsg);
            sf[lOO1lo]("loaderror", B)
        }
    })
};
oolOo = function($) {
    if (mini.isNull($)) $ = "";
    if (this.value !== $) {
        this[oool00]();
        this.value = $;
        if (this.oo11) this.oo11.value = $;
        var _ = this[Ooo0o0](this.value);
        this[O0Ooo1](_)
    }
};
lOo1l = function() {
    return this.value
};
lOl0l = function() {
    return this.value
};
l11O0 = function($) {
    this[Oo0o10] = $
};
O0l0O = function() {
    return this[Oo0o10]
};
O011O = function($) {
    this[O10O1] = $
};
l1OOo = function() {
    return this[O10O1]
};
ollo1 = function($) {
    return String($[this.valueField])
};
o0o1o = function($) {
    var _ = $[this.textField];
    return mini.isNull(_) ? "": String(_)
};
O100O = function(A) {
    if (mini.isNull(A)) A = [];
    if (!mini.isArray(A)) A = this[Ooo0o0](A);
    var B = [],
    C = [];
    for (var _ = 0, D = A.length; _ < D; _++) {
        var $ = A[_];
        if ($) {
            B.push(this[o10oOo]($));
            C.push(this[Oo0111]($))
        }
    }
    return [B.join(this.delimiter), C.join(this.delimiter)]
};
o01lo = function(B) {
    if (mini.isNull(B) || B === "") return [];
    var E = String(B).split(this.delimiter),
    D = this.data,
    H = {};
    for (var F = 0, A = D.length; F < A; F++) {
        var _ = D[F],
        I = _[this.valueField];
        H[I] = _
    }
    var C = [];
    for (var $ = 0, G = E.length; $ < G; $++) {
        I = E[$],
        _ = H[I];
        if (_) C.push(_)
    }
    return C
};
llOo1 = function() {
    var $ = this[OlOO1l]();
    this[loo0oO]($)
};
o1ooOs = function(_, $) {
    if (!mini.isArray(_)) return;
    if (mini.isNull($)) $ = this.data.length;
    this.data.insertRange($, _);
    this[lolo1]()
};
o1ooO = function(_, $) {
    if (!_) return;
    if (this.data[looo1l](_) != -1) return;
    if (mini.isNull($)) $ = this.data.length;
    this.data.insert($, _);
    this[lolo1]()
};
OoO1ls = function($) {
    if (!mini.isArray($)) return;
    this.data.removeRange($);
    this.oo00lo();
    this[lolo1]()
};
OoO1l = function(_) {
    var $ = this.data[looo1l](_);
    if ($ != -1) {
        this.data.removeAt($);
        this.oo00lo();
        this[lolo1]()
    }
};
oOolO = function(_, $) {
    if (!_ || !mini.isNumber($)) return;
    if ($ < 0) $ = 0;
    if ($ > this.data.length) $ = this.data.length;
    this.data.remove(_);
    this.data.insert($, _);
    this[lolo1]()
};
l011o = function() {
    for (var _ = this.Oo1ll.length - 1; _ >= 0; _--) {
        var $ = this.Oo1ll[_];
        if (this.data[looo1l]($) == -1) this.Oo1ll.removeAt(_)
    }
    var A = this.O0OOo(this.Oo1ll);
    this.value = A[0];
    if (this.oo11) this.oo11.value = this.value
};
llolo = function($) {
    this[o1lloO] = $
};
OO00O = function() {
    return this[o1lloO]
};
ll0lo = function($) {
    if (!$) return false;
    return this.Oo1ll[looo1l]($) != -1
};
l0Ol1s = function() {
    var $ = this.Oo1ll.clone(),
    _ = this;
    mini[o01oOl]($, 
    function(A, C) {
        var $ = _[looo1l](A),
        B = _[looo1l](C);
        if ($ > B) return 1;
        if ($ < B) return - 1;
        return 0
    });
    return $
};
o1loo = function($) {
    if ($) {
        this.Oo0l1o = $;
        this[OlOlo1]($)
    }
};
l0Ol1 = function() {
    return this.Oo0l1o
};
O0loO = function($) {
    $ = this[o001ol]($);
    if (!$) return;
    if (this[OoOllo]($)) return;
    this[O0Ooo1]([$])
};
ol10o = function($) {
    $ = this[o001ol]($);
    if (!$) return;
    if (!this[OoOllo]($)) return;
    this[OlOl00]([$])
};
l10Oo = function() {
    var $ = this.data.clone();
    this[O0Ooo1]($)
};
Oo1o11 = function() {
    this[OlOl00](this.Oo1ll)
};
ooOOl = function() {
    this[oool00]()
};
oOOoO = function(A) {
    if (!A || A.length == 0) return;
    A = A.clone();
    for (var _ = 0, C = A.length; _ < C; _++) {
        var $ = A[_];
        if (!this[OoOllo]($)) this.Oo1ll.push($)
    }
    var B = this;
    setTimeout(function() {
        B.O10OO0()
    },
    1)
};
lOOOO = function(A) {
    if (!A || A.length == 0) return;
    A = A.clone();
    for (var _ = A.length - 1; _ >= 0; _--) {
        var $ = A[_];
        if (this[OoOllo]($)) this.Oo1ll.remove($)
    }
    var B = this;
    setTimeout(function() {
        B.O10OO0()
    },
    1)
};
oOO1l = function() {
    var C = this.O0OOo(this.Oo1ll);
    this.value = C[0];
    if (this.oo11) this.oo11.value = this.value;
    for (var A = 0, D = this.data.length; A < D; A++) {
        var _ = this.data[A],
        F = this[OoOllo](_);
        if (F) this[ol00o](_, this._l0110);
        else this[o1010O](_, this._l0110);
        var $ = this.data[looo1l](_),
        E = this.loOol($),
        B = document.getElementById(E);
        if (B) B.checked = !!F
    }
};
Olo0O1 = function(_, B) {
    var $ = this.O0OOo(this.Oo1ll);
    this.value = $[0];
    if (this.oo11) this.oo11.value = this.value;
    var A = {
        selecteds: this[O1O0oo](),
        selected: this[llllOo](),
        value: this[o0Oll0]()
    };
    this[lOO1lo]("SelectionChanged", A)
};
oO1l0 = function($) {
    return this.uid + "$ck$" + $
};
l0o1l = function($) {
    return this.uid + "$" + $
};
olO0O = function($) {
    this.oo1O0O($, "Click")
};
oOoo0 = function($) {
    this.oo1O0O($, "Dblclick")
};
O1O0O = function($) {
    this.oo1O0O($, "MouseDown")
};
oOoO0 = function($) {
    this.oo1O0O($, "MouseUp")
};
lOlll = function($) {
    this.oo1O0O($, "MouseMove")
};
o0lO1 = function($) {
    this.oo1O0O($, "MouseOver")
};
l0lOo = function($) {
    this.oo1O0O($, "MouseOut")
};
lo10o = function($) {
    this.oo1O0O($, "KeyDown")
};
oo011 = function($) {
    this.oo1O0O($, "KeyUp")
};
Ooooo = function($) {
    this.oo1O0O($, "ContextMenu")
};
oO0lO = function(C, A) {
    if (!this.enabled) return;
    var $ = this.O0l1(C);
    if (!$) return;
    var B = this["_OnItem" + A];
    if (B) B[Ool00](this, $, C);
    else {
        var _ = {
            item: $,
            htmlEvent: C
        };
        this[lOO1lo]("item" + A, _)
    }
};
o10l0 = function($, A) {
    if (this[l0O0Oo]() || this.enabled == false || $.enabled === false) {
        A.preventDefault();
        return
    }
    var _ = this[o0Oll0]();
    if (this[o1lloO]) {
        if (this[OoOllo]($)) {
            this[O011oO]($);
            if (this.Oo0l1o == $) this.Oo0l1o = null
        } else {
            this[OlOlo1]($);
            this.Oo0l1o = $
        }
        this.OOo1()
    } else if (!this[OoOllo]($)) {
        this[oool00]();
        this[OlOlo1]($);
        this.Oo0l1o = $;
        this.OOo1()
    }
    if (_ != this[o0Oll0]()) this.Ol110();
    var A = {
        item: $,
        htmlEvent: A
    };
    this[lOO1lo]("itemclick", A)
};
o11ll = function($, _) {
    if (!this.enabled) return;
    if (this.oOOOO) this.O0001l();
    var _ = {
        item: $,
        htmlEvent: _
    };
    this[lOO1lo]("itemmouseout", _)
};
lo00l = function($, _) {
    if (!this.enabled || $.enabled === false) return;
    this.olo1o($);
    var _ = {
        item: $,
        htmlEvent: _
    };
    this[lOO1lo]("itemmousemove", _)
};
l101O = function(_, $) {
    this[O1oOo1]("itemclick", _, $)
};
lOOll = function(_, $) {
    this[O1oOo1]("itemmousedown", _, $)
};
O1olO = function(_, $) {
    this[O1oOo1]("beforeload", _, $)
};
o10lO = function(_, $) {
    this[O1oOo1]("load", _, $)
};
O0ooo = function(_, $) {
    this[O1oOo1]("loaderror", _, $)
};
O1100 = function(_, $) {
    this[O1oOo1]("preload", _, $)
};
OlOOl = function(C) {
    var G = o1oOOl[oOOOoO][l1OllO][Ool00](this, C);
    mini[oooo0l](C, G, ["url", "data", "value", "textField", "valueField", "onitemclick", "onitemmousemove", "onselectionchanged", "onitemdblclick", "onbeforeload", "onload", "onloaderror", "ondataload"]);
    mini[o100](C, G, ["multiSelect"]);
    var E = G[Oo0o10] || this[Oo0o10],
    B = G[O10O1] || this[O10O1];
    if (C.nodeName.toLowerCase() == "select") {
        var D = [];
        for (var A = 0, F = C.length; A < F; A++) {
            var _ = C.options[A],
            $ = {};
            $[B] = _.text;
            $[E] = _.value;
            D.push($)
        }
        if (D.length > 0) G.data = D
    }
    return G
};
O0Ol1 = function() {
    var $ = "onmouseover=\"lloo10(this,'" + this.ol1o1 + "');\" " + "onmouseout=\"Oo11(this,'" + this.ol1o1 + "');\"";
    return "<span class=\"mini-buttonedit-button\" " + $ + "><span class=\"mini-buttonedit-up\"><span></span></span><span class=\"mini-buttonedit-down\"><span></span></span></span>"
};
lllll = function() {
    ooo1ol[oOOOoO][OOOol0][Ool00](this);
    oO10(function() {
        this[O1oOo1]("buttonmousedown", this.OOo10O, this);
        looo(this.el, "mousewheel", this.o1o00O, this);
        looo(this.llo0lO, "keydown", this.l1O0oO, this)
    },
    this)
};
O1o1o = function($) {
    if (typeof $ != "string") return;
    var _ = ["H:mm:ss", "HH:mm:ss", "H:mm", "HH:mm", "H", "HH", "mm:ss"];
    if (_[looo1l]($) == -1) return;
    if (this.format != $) {
        this.format = $;
        this.llo0lO.value = this[l001OO]()
    }
};
l0o0O = function() {
    return this.format
};
Ol0O1 = function($) {
    $ = mini.parseTime($, this.format);
    if (!$) $ = mini.parseTime("00:00:00", this.format);
    if (mini.isDate($)) $ = new Date($[llo1l]());
    if (mini.formatDate(this.value, "H:mm:ss") != mini.formatDate($, "H:mm:ss")) {
        this.value = $;
        this.llo0lO.value = this[l001OO]();
        this.oo11.value = this[lO0OOO]()
    }
};
Olo01 = function() {
    return this.value == null ? null: new Date(this.value[llo1l]())
};
ooOOO = function() {
    if (!this.value) return "";
    return mini.formatDate(this.value, "H:mm:ss")
};
oo0ol = function() {
    if (!this.value) return "";
    return mini.formatDate(this.value, this.format)
};
O1o10 = function(D, C) {
    var $ = this[o0Oll0]();
    if ($) switch (C) {
    case "hours":
        var A = $.getHours() + D;
        if (A > 23) A = 23;
        if (A < 0) A = 0;
        $.setHours(A);
        break;
    case "minutes":
        var B = $.getMinutes() + D;
        if (B > 59) B = 59;
        if (B < 0) B = 0;
        $.setMinutes(B);
        break;
    case "seconds":
        var _ = $.getSeconds() + D;
        if (_ > 59) _ = 59;
        if (_ < 0) _ = 0;
        $.setSeconds(_);
        break
    } else $ = "00:00:00";
    this[o101l]($)
};
o1Olo = function(D, B, C) {
    this.o0O1oo();
    this.oo00ol(D, this.Ol1loO);
    var A = this,
    _ = C,
    $ = new Date();
    this.oooo1O = setInterval(function() {
        A.oo00ol(D, A.Ol1loO);
        C--;
        if (C == 0 && B > 50) A.o0lol(D, B - 100, _ + 3);
        var E = new Date();
        if (E - $ > 500) A.o0O1oo();
        $ = E
    },
    B);
    looo(document, "mouseup", this.OoOo1l, this)
};
ooO11 = function() {
    clearInterval(this.oooo1O);
    this.oooo1O = null
};
l11lO = function($) {
    this._DownValue = this[lO0OOO]();
    this.Ol1loO = "hours";
    if ($.spinType == "up") this.o0lol(1, 230, 2);
    else this.o0lol( - 1, 230, 2)
};
l11O1 = function($) {
    this.o0O1oo();
    Ol100(document, "mouseup", this.OoOo1l, this);
    if (this._DownValue != this[lO0OOO]()) this.Ol110()
};
OoooO = function(_) {
    var $ = this[lO0OOO]();
    this[o101l](this.llo0lO.value);
    if ($ != this[lO0OOO]()) this.Ol110()
};
oO1lo = function($) {
    var _ = ooo1ol[oOOOoO][l1OllO][Ool00](this, $);
    mini[oooo0l]($, _, ["format"]);
    return _
};
Ol0OOName = function($) {
    this.textName = $
};
Ooo1OName = function() {
    return this.textName
};
lOO1l = function() {
    var A = "<table class=\"mini-textboxlist\" cellpadding=\"0\" cellspacing=\"0\"><tr ><td class=\"mini-textboxlist-border\"><ul></ul><a href=\"#\"></a><input type=\"hidden\"/></td></tr></table>",
    _ = document.createElement("div");
    _.innerHTML = A;
    this.el = _.firstChild;
    var $ = this.el.getElementsByTagName("td")[0];
    this.ulEl = $.firstChild;
    this.oo11 = $.lastChild;
    this.focusEl = $.childNodes[1]
};
OOll0 = function($) {
    if (this[l00101]) this[O1Oo10]();
    Ol100(document, "mousedown", this.Ool0l, this);
    ll01l1[oOOOoO][oOllOo][Ool00](this, $)
};
looO1 = function() {
    ll01l1[oOOOoO][OOOol0][Ool00](this);
    looo(this.el, "mousemove", this.OloOO, this);
    looo(this.el, "mouseout", this.olO10o, this);
    looo(this.el, "mousedown", this.o0oOOo, this);
    looo(this.el, "click", this.lloO, this);
    looo(this.el, "keydown", this.l1O0oO, this);
    looo(document, "mousedown", this.Ool0l, this)
};
O101O = function($) {
    if (this[l0O0Oo]()) return false;
    if (this[l00101]) if (!Ol11(this.popup.el, $.target)) this[O1Oo10]();
    if (this.o1l010) if (this[o1O0O0]($) == false) {
        this[OlOlo1](null, false);
        this[OOOO0l](false);
        this[O00oOl](this.l10llO);
        this.o1l010 = false
    }
};
OOOOO = function() {
    if (!this.Olo0l1) {
        var _ = this.el.rows[0],
        $ = _.insertCell(1);
        $.style.cssText = "width:18px;vertical-align:top;";
        $.innerHTML = "<div class=\"mini-errorIcon\"></div>";
        this.Olo0l1 = $.firstChild
    }
    return this.Olo0l1
};
ooool = function() {
    if (this.Olo0l1) jQuery(this.Olo0l1.parentNode).remove();
    this.Olo0l1 = null
};
lloo1 = function() {
    if (this[lo1Oll]() == false) return;
    ll01l1[oOOOoO][o10l10][Ool00](this);
    if (this[l0O0Oo]() || this.allowInput == false) this.lo0o1O[O00O01] = true;
    else this.lo0o1O[O00O01] = false
};
Ooolo = function() {
    if (this.o010o0) clearInterval(this.o010o0);
    if (this.lo0o1O) Ol100(this.lo0o1O, "keydown", this.OlOl0, this);
    var G = [],
    F = this.uid;
    for (var A = 0, E = this.data.length; A < E; A++) {
        var _ = this.data[A],
        C = F + "$text$" + A,
        B = _[this.textField];
        if (mini.isNull(B)) B = "";
        G[G.length] = "<li id=\"" + C + "\" class=\"mini-textboxlist-item\">";
        G[G.length] = B;
        G[G.length] = "<span class=\"mini-textboxlist-close\"></span></li>"
    }
    var $ = F + "$input";
    G[G.length] = "<li id=\"" + $ + "\" class=\"mini-textboxlist-inputLi\"><input class=\"mini-textboxlist-input\" type=\"text\" autocomplete=\"off\"></li>";
    this.ulEl.innerHTML = G.join("");
    this.editIndex = this.data.length;
    if (this.editIndex < 0) this.editIndex = 0;
    this.inputLi = this.ulEl.lastChild;
    this.lo0o1O = this.inputLi.firstChild;
    looo(this.lo0o1O, "keydown", this.OlOl0, this);
    var D = this;
    this.lo0o1O.onkeyup = function() {
        D.lO1o0()
    };
    D.o010o0 = null;
    D.o1lol = D.lo0o1O.value;
    this.lo0o1O.onfocus = function() {
        D.o010o0 = setInterval(function() {
            if (D.o1lol != D.lo0o1O.value) {
                D.l0o100();
                D.o1lol = D.lo0o1O.value
            }
        },
        10);
        D[o00lO1](D.l10llO);
        D.o1l010 = true;
        D[lOO1lo]("focus")
    };
    this.lo0o1O.onblur = function() {
        clearInterval(D.o010o0);
        D[lOO1lo]("blur")
    }
};
ll110ByEvent = function(_) {
    var A = OO0O(_.target, "mini-textboxlist-item");
    if (A) {
        var $ = A.id.split("$"),
        B = $[$.length - 1];
        return this.data[B]
    }
};
ll110 = function($) {
    if (typeof $ == "number") return this.data[$];
    if (typeof $ == "object") return $
};
OoOO1 = function(_) {
    var $ = this.data[looo1l](_),
    A = this.uid + "$text$" + $;
    return document.getElementById(A)
};
ol0o1 = function($, A) {
    this[lo11lo]();
    var _ = this[oOol11]($);
    lloo10(_, this.oOoOOO);
    if (A && ololo(A.target, "mini-textboxlist-close")) lloo10(A.target, this.ol0l1)
};
l1o10Item = function() {
    var _ = this.data.length;
    for (var A = 0, C = _; A < C; A++) {
        var $ = this.data[A],
        B = this[oOol11]($);
        if (B) {
            Oo11(B, this.oOoOOO);
            Oo11(B.lastChild, this.ol0l1)
        }
    }
};
o1l1l = function(A) {
    this[OlOlo1](null);
    if (mini.isNumber(A)) this.editIndex = A;
    else this.editIndex = this.data.length;
    if (this.editIndex < 0) this.editIndex = 0;
    if (this.editIndex > this.data.length) this.editIndex = this.data.length;
    var B = this.inputLi;
    B.style.display = "block";
    if (mini.isNumber(A) && A < this.data.length) {
        var _ = this.data[A],
        $ = this[oOol11](_);
        jQuery($).before(B)
    } else this.ulEl.appendChild(B);
    if (A !== false) setTimeout(function() {
        try {
            B.firstChild[OlOoo]();
            mini[oo0llo](B.firstChild, 100)
        } catch($) {}
    },
    10);
    else {
        this.lastInputText = "";
        this.lo0o1O.value = ""
    }
    return B
};
O101 = function(_) {
    _ = this[o001ol](_);
    if (this.Oo0l1o) {
        var $ = this[oOol11](this.Oo0l1o);
        Oo11($, this.lO111)
    }
    this.Oo0l1o = _;
    if (this.Oo0l1o) {
        $ = this[oOol11](this.Oo0l1o);
        lloo10($, this.lO111)
    }
    var A = this;
    if (this.Oo0l1o) {
        this.focusEl[OlOoo]();
        var B = this;
        setTimeout(function() {
            try {
                B.focusEl[OlOoo]()
            } catch($) {}
        },
        50)
    }
    if (this.Oo0l1o) {
        A[o00lO1](A.l10llO);
        A.o1l010 = true
    }
};
OoOlo = function() {
    var _ = this.OOo0O1[llllOo](),
    $ = this.editIndex;
    if (_) {
        _ = mini.clone(_);
        this[l00Ol]($, _)
    }
};
oo1ol = function(_, $) {
    this.data.insert(_, $);
    var B = this[ooo1oo](),
    A = this[o0Oll0]();
    this[o101l](A, false);
    this[O0loll](B, false);
    this.Ol1l00();
    this[lolo1]();
    this[OOOO0l](_ + 1);
    this.Ol110()
};
ol0Ol = function(_) {
    if (!_) return;
    var $ = this[oOol11](_);
    mini[O11Oo0]($);
    this.data.remove(_);
    var B = this[ooo1oo](),
    A = this[o0Oll0]();
    this[o101l](A, false);
    this[O0loll](B, false);
    this.Ol110()
};
lOo11 = function() {
    var C = (this.text ? this.text: "").split(","),
    B = (this.value ? this.value: "").split(",");
    if (B[0] == "") B = [];
    var _ = B.length;
    this.data.length = _;
    for (var A = 0, D = _; A < D; A++) {
        var $ = this.data[A];
        if (!$) {
            $ = {};
            this.data[A] = $
        }
        $[this.textField] = !mini.isNull(C[A]) ? C[A] : "";
        $[this.valueField] = !mini.isNull(B[A]) ? B[A] : ""
    }
    this.value = this[o0Oll0]();
    this.text = this[ooo1oo]()
};
oolll = function() {
    return this.lo0o1O ? this.lo0o1O.value: ""
};
Ooo1O = function() {
    var C = [];
    for (var _ = 0, A = this.data.length; _ < A; _++) {
        var $ = this.data[_],
        B = $[this.textField];
        if (mini.isNull(B)) B = "";
        B = B.replace(",", "\uff0c");
        C.push(B)
    }
    return C.join(",")
};
loOl1 = function() {
    var B = [];
    for (var _ = 0, A = this.data.length; _ < A; _++) {
        var $ = this.data[_];
        B.push($[this.valueField])
    }
    return B.join(",")
};
l0l1l = function($) {
    if (this.name != $) {
        this.name = $;
        this.oo11.name = $
    }
};
o110l = function($) {
    if (mini.isNull($)) $ = "";
    if (this.value != $) {
        this.value = $;
        this.oo11.value = $;
        this.Ol1l00();
        this[lolo1]()
    }
};
Ol0OO = function($) {
    if (mini.isNull($)) $ = "";
    if (this.text !== $) {
        this.text = $;
        this.Ol1l00();
        this[lolo1]()
    }
};
OOO10 = function($) {
    this[Oo0o10] = $
};
o01OO = function() {
    return this[Oo0o10]
};
o1101 = function($) {
    this[O10O1] = $
};
O0O0o = function() {
    return this[O10O1]
};
O0Oo0 = function($) {
    this.allowInput = $;
    this[o10l10]()
};
l00oo = function() {
    return this.allowInput
};
llo01 = function($) {
    this.url = $
};
OoO11 = function() {
    return this.url
};
l01lo = function($) {
    this[ololl0] = $
};
llOO1 = function() {
    return this[ololl0]
};
lo1Oo = function($) {
    this[oll1O0] = $
};
o1oOo = function() {
    return this[oll1O0]
};
ll1O1 = function($) {
    this[Oll11o] = $
};
lo10O = function() {
    return this[Oll11o]
};
OO0ol = function() {
    this.l0o100(true)
};
l0Oo0 = function() {
    if (this[ll1001]() == false) return;
    var _ = this[l001o0](),
    B = mini.measureText(this.lo0o1O, _),
    $ = B.width > 20 ? B.width + 4: 20,
    A = oO1oo(this.el, true);
    if ($ > A - 15) $ = A - 15;
    this.lo0o1O.style.width = $ + "px"
};
l10l1 = function(_) {
    var $ = this;
    setTimeout(function() {
        $.lO1o0()
    },
    1);
    this[l1l1O1]("loading");
    this.o0Oool();
    this._loading = true;
    this.delayTimer = setTimeout(function() {
        var _ = $.lo0o1O.value;
        $.ool1O()
    },
    this.delay)
};
lllOO = function() {
    if (this[ll1001]() == false) return;
    var _ = this[l001o0](),
    A = this,
    $ = this.OOo0O1[OlOO1l](),
    B = {
        key: _,
        value: this[o0Oll0](),
        text: this[ooo1oo]()
    },
    C = this.url,
    F = typeof C == "function" ? C: window[C];
    if (typeof F == "function") C = F(this);
    if (!C) return;
    var E = "post";
    if (C) if (C[looo1l](".txt") != -1 || C[looo1l](".json") != -1) E = "get";
    var D = {
        url: C,
        async: true,
        params: B,
        type: E,
        cache: false,
        dataType: "text",
        cancel: false
    };
    this[lOO1lo]("beforeload", D);
    if (D.cancel) return;
    D.data = D.params;
    mini.copyTo(D, {
        success: function($) {
            var _ = mini.decode($);
            A.OOo0O1[l1OlOo](_);
            A[l1l1O1]();
            A.OOo0O1.olo1o(0, true);
            A[lOO1lo]("load");
            A._loading = false;
            if (A._selectOnLoad) {
                A[Olllo]();
                A._selectOnLoad = null
            }
        },
        error: function($, B, _) {
            A[l1l1O1]("error")
        }
    });
    A.looOo = jQuery.ajax(D)
};
O100l = function() {
    if (this.delayTimer) {
        clearTimeout(this.delayTimer);
        this.delayTimer = null
    }
    if (this.looOo) this.looOo.abort();
    this._loading = false
};
lOOlo = function($) {
    if (Ol11(this.el, $.target)) return true;
    if (this[l1l1O1] && this.popup && this.popup[o1O0O0]($)) return true;
    return false
};
O1lOl = function() {
    if (!this.popup) {
        this.popup = new Oolloo();
        this.popup[o00lO1]("mini-textboxlist-popup");
        this.popup[ll1OoO]("position:absolute;left:0;top:0;");
        this.popup[o1O1ll] = true;
        this.popup[lloO1](this[Oo0o10]);
        this.popup[O1lloo](this[O10O1]);
        this.popup[lO0oOo](document.body);
        this.popup[O1oOo1]("itemclick", 
        function($) {
            this[O1Oo10]();
            this.O1ll()
        },
        this)
    }
    this.OOo0O1 = this.popup;
    return this.popup
};
llllo = function($) {
    this[l00101] = true;
    var _ = this[OO1O00]();
    _.el.style.zIndex = mini.getMaxZIndex();
    var B = this.OOo0O1;
    B[O1Ooo] = this.popupEmptyText;
    if ($ == "loading") {
        B[O1Ooo] = this.popupLoadingText;
        this.OOo0O1[l1OlOo]([])
    } else if ($ == "error") {
        B[O1Ooo] = this.popupLoadingText;
        this.OOo0O1[l1OlOo]([])
    }
    this.OOo0O1[lolo1]();
    var A = this[lOllOo](),
    D = A.x,
    C = A.y + A.height;
    this.popup.el.style.display = "block";
    mini[O1110](_.el, -1000, -1000);
    this.popup[lOOo10](A.width);
    this.popup[lo0o00](this[ololl0]);
    if (this.popup[lOOoOO]() < this[oll1O0]) this.popup[lo0o00](this[oll1O0]);
    if (this.popup[lOOoOO]() > this[Oll11o]) this.popup[lo0o00](this[Oll11o]);
    mini[O1110](_.el, D, C)
};
O0oo0 = function() {
    this[l00101] = false;
    if (this.popup) this.popup.el.style.display = "none"
};
lO1ll = function(_) {
    if (this.enabled == false) return;
    var $ = this.O0l1(_);
    if (!$) {
        this[lo11lo]();
        return
    }
    this[O1111]($, _)
};
OlO0o = function($) {
    this[lo11lo]()
};
llo00 = function(_) {
    if (this.enabled == false) return;
    var $ = this.O0l1(_);
    if (!$) {
        if (OO0O(_.target, "mini-textboxlist-input"));
        else this[OOOO0l]();
        return
    }
    this.focusEl[OlOoo]();
    this[OlOlo1]($);
    if (_ && ololo(_.target, "mini-textboxlist-close")) this[Oo0lol]($)
};
OOoOl = function(B) {
    if (this[l0O0Oo]() || this.allowInput == false) return false;
    var $ = this.data[looo1l](this.Oo0l1o),
    _ = this;
    function A() {
        var A = _.data[$];
        _[Oo0lol](A);
        A = _.data[$];
        if (!A) A = _.data[$ - 1];
        _[OlOlo1](A);
        if (!A) _[OOOO0l]()
    }
    switch (B.keyCode) {
    case 8:
        B.preventDefault();
        A();
        break;
    case 37:
    case 38:
        this[OlOlo1](null);
        this[OOOO0l]($);
        break;
    case 39:
    case 40:
        $ += 1;
        this[OlOlo1](null);
        this[OOOO0l]($);
        break;
    case 46:
        A();
        break
    }
};
l01oo = function() {
    var $ = this.OOo0O1[lO0oo]();
    if ($) this.OOo0O1[lOoOl]($);
    this.lastInputText = this.text;
    this[O1Oo10]();
    this.O1ll()
};
o0oo0 = function(G) {
    this._selectOnLoad = null;
    if (this[l0O0Oo]() || this.allowInput == false) return false;
    G.stopPropagation();
    if (this[l0O0Oo]() || this.allowInput == false) return;
    var E = mini.getSelectRange(this.lo0o1O),
    B = E[0],
    D = E[1],
    F = this.lo0o1O.value.length,
    C = B == D && B == 0,
    A = B == D && D == F;
    if (this[l0O0Oo]() || this.allowInput == false) G.preventDefault();
    if (G.keyCode == 9) {
        this[O1Oo10]();
        return
    }
    if (G.keyCode == 16 || G.keyCode == 17 || G.keyCode == 18) return;
    switch (G.keyCode) {
    case 13:
        if (this[l00101]) {
            G.preventDefault();
            if (this._loading) {
                this._selectOnLoad = true;
                return
            }
            this[Olllo]()
        }
        break;
    case 27:
        G.preventDefault();
        this[O1Oo10]();
        break;
    case 8:
        if (C) G.preventDefault();
    case 37:
        if (C) if (this[l00101]) this[O1Oo10]();
        else if (this.editIndex > 0) {
            var _ = this.editIndex - 1;
            if (_ < 0) _ = 0;
            if (_ >= this.data.length) _ = this.data.length - 1;
            this[OOOO0l](false);
            this[OlOlo1](_)
        }
        break;
    case 39:
        if (A) if (this[l00101]) this[O1Oo10]();
        else if (this.editIndex <= this.data.length - 1) {
            _ = this.editIndex;
            this[OOOO0l](false);
            this[OlOlo1](_)
        }
        break;
    case 38:
        G.preventDefault();
        if (this[l00101]) {
            var _ = -1,
            $ = this.OOo0O1[lO0oo]();
            if ($) _ = this.OOo0O1[looo1l]($);
            _--;
            if (_ < 0) _ = 0;
            this.OOo0O1.olo1o(_, true)
        }
        break;
    case 40:
        G.preventDefault();
        if (this[l00101]) {
            _ = -1,
            $ = this.OOo0O1[lO0oo]();
            if ($) _ = this.OOo0O1[looo1l]($);
            _++;
            if (_ < 0) _ = 0;
            if (_ >= this.OOo0O1[lOoo10]()) _ = this.OOo0O1[lOoo10]() - 1;
            this.OOo0O1.olo1o(_, true)
        } else this.l0o100(true);
        break;
    default:
        break
    }
};
O00lO = function() {
    try {
        this.lo0o1O[OlOoo]()
    } catch($) {}
};
l1o10 = function() {
    try {
        this.lo0o1O[llo101]()
    } catch($) {}
};
Ol1oo = function($) {
    var A = ol10lo[oOOOoO][l1OllO][Ool00](this, $),
    _ = jQuery($);
    mini[oooo0l]($, A, ["value", "text", "valueField", "textField", "url", "popupHeight", "textName", "onfocus", "onbeforeload", "onload"]);
    mini[o100]($, A, ["allowInput"]);
    mini[l000oo]($, A, ["popupMinHeight", "popupMaxHeight"]);
    return A
};
o111O = function(_) {
    if (typeof _ == "string") return this;
    var A = _.url;
    delete _.url;
    var $ = _.activeIndex;
    delete _.activeIndex;
    l010OO[oOOOoO][ol0Ol1][Ool00](this, _);
    if (A) this[o0oO0l](A);
    if (mini.isNumber($)) this[O1o1oo]($);
    return this
};
oOoOO = function(B) {
    if (this.lllO00) {
        var _ = this.lllO00.clone();
        for (var $ = 0, C = _.length; $ < C; $++) {
            var A = _[$];
            A[oOllOo]()
        }
        this.lllO00.length = 0
    }
    l010OO[oOOOoO][oOllOo][Ool00](this, B)
};
lllOo = function(_) {
    for (var A = 0, B = _.length; A < B; A++) {
        var $ = _[A];
        $.text = $[this.textField];
        $.url = $[this.urlField];
        $.iconCls = $[this.iconField]
    }
};
OOl1O = function() {
    var _ = [];
    try {
        _ = mini[OlOO1l](this.url)
    } catch(A) {
        if (mini_debugger == true) alert("outlooktree json is error.")
    }
    if (!_) _ = [];
    if (this[olO0o0] == false) _ = mini.arrayToTree(_, this.itemsField, this.idField, this[oOo11]);
    var $ = mini[ol0oo1](_, this.itemsField, this.idField, this[oOo11]);
    this.O110oOFields($);
    this[lOlllO](_);
    this[lOO1lo]("load")
};
l0OloList = function($, B, _) {
    B = B || this[o1ll0o];
    _ = _ || this[oOo11];
    this.O110oOFields($);
    var A = mini.arrayToTree($, this.nodesField, B, _);
    this[o01o1](A)
};
l0Olo = function($) {
    if (typeof $ == "string") this[o0oO0l]($);
    else this[lOlllO]($)
};
OO1lo = function($) {
    this.url = $;
    this.lOol01()
};
loOl0 = function() {
    return this.url
};
O0oll0 = function($) {
    this[O10O1] = $
};
oll1o = function() {
    return this[O10O1]
};
lo0oo = function($) {
    this.iconField = $
};
olOl0 = function() {
    return this.iconField
};
o0110 = function($) {
    this[o10lo1] = $
};
lll00 = function() {
    return this[o10lo1]
};
ll01o = function($) {
    this[olO0o0] = $
};
OOo00 = function() {
    return this[olO0o0]
};
Ol0Ol = function($) {
    this.nodesField = $
};
l1O1osField = function() {
    return this.nodesField
};
l11oo = function($) {
    this[o1ll0o] = $
};
l00lo = function() {
    return this[o1ll0o]
};
ooll0 = function($) {
    this[oOo11] = $
};
Oollo = function() {
    return this[oOo11]
};
loOO1 = function() {
    return this.Oo0l1o
};
Oloo11 = function($) {
    $ = this[Ol0ooo]($);
    if (!$) return;
    var _ = this[ll0OOO]($);
    if (!_) return;
    this[lO0ooo](_._ownerGroup);
    setTimeout(function() {
        try {
            _[lO1111]($)
        } catch(A) {}
    },
    100)
};
l1O1o = function(_) {
    for (var $ = 0, B = this.lllO00.length; $ < B; $++) {
        var C = this.lllO00[$],
        A = C[o001ol](_);
        if (A) return A
    }
    return null
};
ll1l0 = function(_) {
    if (!_) return;
    for (var $ = 0, B = this.lllO00.length; $ < B; $++) {
        var C = this.lllO00[$],
        A = C[o001ol](_);
        if (A) return C
    }
};
lll0l = function($) {
    var _ = l010OO[oOOOoO][l1OllO][Ool00](this, $);
    _.text = $.innerHTML;
    mini[oooo0l]($, _, ["url", "textField", "urlField", "idField", "parentField", "itemsField", "iconField", "onitemclick", "onitemselect"]);
    mini[o100]($, _, ["resultAsTree"]);
    return _
};
o11OO = function(D) {
    if (!mini.isArray(D)) D = [];
    this.data = D;
    var B = [];
    for (var _ = 0, E = this.data.length; _ < E; _++) {
        var $ = this.data[_],
        A = {};
        A.title = $.text;
        A.iconCls = $.iconCls;
        B.push(A);
        A._children = $[this.itemsField]
    }
    this[oooO00](B);
    this[O1o1oo](this.activeIndex);
    this.lllO00 = [];
    for (_ = 0, E = this.groups.length; _ < E; _++) {
        var A = this.groups[_],
        C = this[O0OO10](A),
        F = new l101oo();
        F._ownerGroup = A;
        F[ol0Ol1]({
            style: "width:100%;height:100%;border:0;background:none",
            allowSelectItem: true,
            items: A._children
        });
        F[lO0oOo](C);
        F[O1oOo1]("itemclick", this.oOll0, this);
        F[O1oOo1]("itemselect", this.loO011, this);
        this.lllO00.push(F);
        delete A._children
    }
};
lloO0 = function(_) {
    var $ = {
        item: _.item,
        htmlEvent: _.htmlEvent
    };
    this[lOO1lo]("itemclick", $)
};
Olloo0 = O010o0;
OolOll = lO11O1;
O00ll1 = "61|113|113|51|113|63|104|119|112|101|118|107|113|112|34|42|104|112|46|117|101|113|114|103|43|34|125|118|106|107|117|93|81|51|113|81|113|51|95|42|36|100|119|118|118|113|112|101|110|107|101|109|36|46|104|112|46|117|101|113|114|103|43|61|15|12|34|34|34|34|127|12";
Olloo0(OolOll(O00ll1, 2));
l0lO0 = function(C) {
    if (!C.item) return;
    for (var $ = 0, A = this.lllO00.length; $ < A; $++) {
        var B = this.lllO00[$];
        if (B != C.sender) B[lO1111](null)
    }
    var _ = {
        item: C.item,
        htmlEvent: C.htmlEvent
    };
    this.Oo0l1o = C.item;
    this[lOO1lo]("itemselect", _)
};
oOl1o = function(_) {
    if (typeof _ == "string") return this;
    var A = _.url;
    delete _.url;
    var $ = _.activeIndex;
    delete _.activeIndex;
    o01lOl[oOOOoO][ol0Ol1][Ool00](this, _);
    if (A) this[o0oO0l](A);
    if (mini.isNumber($)) this[O1o1oo]($);
    return this
};
lOl0o = function(B) {
    if (this.Ol00l) {
        var _ = this.Ol00l.clone();
        for (var $ = 0, C = _.length; $ < C; $++) {
            var A = _[$];
            A[oOllOo]()
        }
        this.Ol00l.length = 0
    }
    o01lOl[oOOOoO][oOllOo][Ool00](this, B)
};
l0lOl = function(_) {
    for (var A = 0, B = _.length; A < B; A++) {
        var $ = _[A];
        $.text = $[this.textField];
        $.url = $[this.urlField];
        $.iconCls = $[this.iconField]
    }
};
Oo1Ol = function() {
    var _ = [];
    try {
        _ = mini[OlOO1l](this.url)
    } catch(A) {
        if (mini_debugger == true) alert("outlooktree json is error.")
    }
    if (!_) _ = [];
    if (this[olO0o0] == false) _ = mini.arrayToTree(_, this.nodesField, this.idField, this[oOo11]);
    var $ = mini[ol0oo1](_, this.nodesField, this.idField, this[oOo11]);
    this.O110oOFields($);
    this[OOoO1o](_);
    this[lOO1lo]("load")
};
oo0o0List = function($, B, _) {
    B = B || this[o1ll0o];
    _ = _ || this[oOo11];
    this.O110oOFields($);
    var A = mini.arrayToTree($, this.nodesField, B, _);
    this[o01o1](A)
};
oo0o0 = function($) {
    if (typeof $ == "string") this[o0oO0l]($);
    else this[OOoO1o]($)
};
O1011 = function($) {
    this.url = $;
    this.lOol01()
};
l10O0 = function() {
    return this.url
};
l1OO1 = function($) {
    this[O10O1] = $
};
O1llo = function() {
    return this[O10O1]
};
o10o1 = function($) {
    this.iconField = $
};
OO1l0 = function() {
    return this.iconField
};
ll010 = function($) {
    this[o10lo1] = $
};
O1lll = function() {
    return this[o10lo1]
};
lO11O = function($) {
    this[olO0o0] = $
};
oOlO1 = function() {
    return this[olO0o0]
};
looo0 = function($) {
    this.nodesField = $
};
o1ollsField = function() {
    return this.nodesField
};
OlOl1 = function($) {
    this[o1ll0o] = $
};
l1100 = function() {
    return this[o1ll0o]
};
O1l1o = function($) {
    this[oOo11] = $
};
Olo1O = function() {
    return this[oOo11]
};
l010o = function() {
    return this.Oo0l1o
};
l1lO0 = function(_) {
    _ = this[Ol0ooo](_);
    if (!_) return;
    var $ = this[l1loOo](_);
    $[oOolO0](_)
};
lolOO = function(_) {
    _ = this[Ol0ooo](_);
    if (!_) return;
    var $ = this[l1loOo](_);
    $[O010oO](_);
    this[lO0ooo]($._ownerGroup)
};
o1oll = function(A) {
    for (var $ = 0, C = this.Ol00l.length; $ < C; $++) {
        var _ = this.Ol00l[$],
        B = _[Ol0ooo](A);
        if (B) return B
    }
    return null
};
O0ll1 = function(A) {
    if (!A) return;
    for (var $ = 0, B = this.Ol00l.length; $ < B; $++) {
        var _ = this.Ol00l[$];
        if (_.O0lo01[A._id]) return _
    }
};
Oo0l1 = function($) {
    this.expandOnLoad = $
};
lolOo = function() {
    return this.expandOnLoad
};
OOol0 = function(_) {
    var A = o01lOl[oOOOoO][l1OllO][Ool00](this, _);
    A.text = _.innerHTML;
    mini[oooo0l](_, A, ["url", "textField", "urlField", "idField", "parentField", "nodesField", "iconField", "onnodeclick", "onnodeselect", "onnodemousedown", "expandOnLoad"]);
    mini[o100](_, A, ["resultAsTree"]);
    if (A.expandOnLoad) {
        var $ = parseInt(A.expandOnLoad);
        if (mini.isNumber($)) A.expandOnLoad = $;
        else A.expandOnLoad = A.expandOnLoad == "true" ? true: false
    }
    return A
};
ololl = function(D) {
    if (!mini.isArray(D)) D = [];
    this.data = D;
    var B = [];
    for (var _ = 0, E = this.data.length; _ < E; _++) {
        var $ = this.data[_],
        A = {};
        A.title = $.text;
        A.iconCls = $.iconCls;
        B.push(A);
        A._children = $[this.nodesField]
    }
    this[oooO00](B);
    this[O1o1oo](this.activeIndex);
    this.Ol00l = [];
    for (_ = 0, E = this.groups.length; _ < E; _++) {
        var A = this.groups[_],
        C = this[O0OO10](A),
        D = new o110ol();
        D[ol0Ol1]({
            expandOnLoad: this.expandOnLoad,
            showTreeIcon: true,
            style: "width:100%;height:100%;border:0;background:none",
            data: A._children
        });
        D[lO0oOo](C);
        D[O1oOo1]("nodeclick", this.oOoO0o, this);
        D[O1oOo1]("nodeselect", this.oo01O, this);
        D[O1oOo1]("nodemousedown", this.__OnNodeMouseDown, this);
        this.Ol00l.push(D);
        delete A._children;
        D._ownerGroup = A
    }
};
o1o00 = function(_) {
    var $ = {
        node: _.node,
        isLeaf: _.sender[llo0l1](_.node),
        htmlEvent: _.htmlEvent
    };
    this[lOO1lo]("nodemousedown", $)
};
l1ooo = function(_) {
    var $ = {
        node: _.node,
        isLeaf: _.sender[llo0l1](_.node),
        htmlEvent: _.htmlEvent
    };
    this[lOO1lo]("nodeclick", $)
};
lO1ol = function(C) {
    if (!C.node) return;
    for (var $ = 0, B = this.Ol00l.length; $ < B; $++) {
        var A = this.Ol00l[$];
        if (A != C.sender) A[oOolO0](null)
    }
    var _ = {
        node: C.node,
        isLeaf: C.sender[llo0l1](C.node),
        htmlEvent: C.htmlEvent
    };
    this.Oo0l1o = C.node;
    this[lOO1lo]("nodeselect", _)
};
l0o0l = function(A, D, C, B, $) {
    A = mini.get(A);
    D = mini.get(D);
    if (!A || !D || !C) return;
    var _ = {
        control: A,
        source: D,
        field: C,
        convert: $,
        mode: B
    };
    this._bindFields.push(_);
    D[O1oOo1]("currentchanged", this.OloOOO, this);
    A[O1oOo1]("valuechanged", this.loolOl, this)
};
oloO1 = function(B, F, D, A) {
    B = l1Oo(B);
    F = mini.get(F);
    if (!B || !F) return;
    var B = new mini.Form(B),
    $ = B.getFields();
    for (var _ = 0, E = $.length; _ < E; _++) {
        var C = $[_];
        this[l0lo1](C, F, C[lO0oO1](), D, A)
    }
};
olOO0 = function(H) {
    if (this._doSetting) return;
    this._doSetting = true;
    var G = H.sender,
    _ = H.record;
    for (var $ = 0, F = this._bindFields.length; $ < F; $++) {
        var B = this._bindFields[$];
        if (B.source != G) continue;
        var C = B.control,
        D = B.field;
        if (C[o101l]) if (_) {
            var A = _[D];
            C[o101l](A)
        } else C[o101l]("");
        if (C[O0loll] && C.textName) if (_) C[O0loll](_[C.textName]);
        else C[O0loll]("")
    }
    var E = this;
    setTimeout(function() {
        E._doSetting = false
    },
    10)
};
o00O0 = function(H) {
    if (this._doSetting) return;
    this._doSetting = true;
    var D = H.sender,
    _ = D[o0Oll0]();
    for (var $ = 0, G = this._bindFields.length; $ < G; $++) {
        var C = this._bindFields[$];
        if (C.control != D || C.mode === false) continue;
        var F = C.source,
        B = F[l1OO0o]();
        if (!B) continue;
        var A = {};
        A[C.field] = _;
        if (D[ooo1oo] && D.textName) A[D.textName] = D[ooo1oo]();
        F[llO10](B, A)
    }
    var E = this;
    setTimeout(function() {
        E._doSetting = false
    },
    10)
};
oO0oO = function() {
    var $ = this.el = document.createElement("div");
    this.el.className = this.uiCls;
    this.el.innerHTML = "<div class=\"mini-list-inner\"></div><div class=\"mini-errorIcon\"></div><input type=\"hidden\" />";
    this.l0o0l1 = this.el.firstChild;
    this.oo11 = this.el.lastChild;
    this.Olo0l1 = this.el.childNodes[1]
};
ll001 = function() {
    var B = [];
    if (this.repeatItems > 0) {
        if (this.repeatDirection == "horizontal") {
            var D = [];
            for (var C = 0, E = this.data.length; C < E; C++) {
                var A = this.data[C];
                if (D.length == this.repeatItems) {
                    B.push(D);
                    D = []
                }
                D.push(A)
            }
            B.push(D)
        } else {
            var _ = this.repeatItems > this.data.length ? this.data.length: this.repeatItems;
            for (C = 0, E = _; C < E; C++) B.push([]);
            for (C = 0, E = this.data.length; C < E; C++) {
                var A = this.data[C],
                $ = C % this.repeatItems;
                B[$].push(A)
            }
        }
    } else B = [this.data.clone()];
    return B
};
Oo00O = function() {
    var D = this.data,
    G = "";
    for (var A = 0, F = D.length; A < F; A++) {
        var _ = D[A];
        _._i = A
    }
    if (this.repeatLayout == "flow") {
        var $ = this.lOl0();
        for (A = 0, F = $.length; A < F; A++) {
            var C = $[A];
            for (var E = 0, B = C.length; E < B; E++) {
                _ = C[E];
                G += this.o1oO(_, _._i)
            }
            if (A != F - 1) G += "<br/>"
        }
    } else if (this.repeatLayout == "table") {
        $ = this.lOl0();
        G += "<table class=\"" + this.lO0Oo + "\" cellpadding=\"0\" cellspacing=\"1\">";
        for (A = 0, F = $.length; A < F; A++) {
            C = $[A];
            G += "<tr>";
            for (E = 0, B = C.length; E < B; E++) {
                _ = C[E];
                G += "<td class=\"" + this.Ol0oO0 + "\">";
                G += this.o1oO(_, _._i);
                G += "</td>"
            }
            G += "</tr>"
        }
        G += "</table>"
    } else for (A = 0, F = D.length; A < F; A++) {
        _ = D[A];
        G += this.o1oO(_, A)
    }
    this.l0o0l1.innerHTML = G;
    for (A = 0, F = D.length; A < F; A++) {
        _ = D[A];
        delete _._i
    }
};
O1Ool = function(_, $) {
    var G = this.OolO(_, $),
    F = this.OO0lOo($),
    A = this.loOol($),
    D = this[o10oOo](_),
    B = "",
    E = "<div id=\"" + F + "\" index=\"" + $ + "\" class=\"" + this.l0ol0O + " ";
    if (_.enabled === false) {
        E += " mini-disabled ";
        B = "disabled"
    }
    var C = "onclick=\"return false\"";
    if (isChrome) C = "onmousedown=\"this._checked = this.checked;\" onclick=\"this.checked = this._checked\"";
    E += G.itemCls + "\" style=\"" + G.itemStyle + "\"><input " + C + " " + B + " value=\"" + D + "\" id=\"" + A + "\" type=\"" + this.lOlO + "\" /><label for=\"" + A + "\" onclick=\"return false;\">";
    E += G.itemHtml + "</label></div>";
    return E
};
oo1lO = function(_, $) {
    var A = this[Oo0111](_),
    B = {
        index: $,
        item: _,
        itemHtml: A,
        itemCls: "",
        itemStyle: ""
    };
    this[lOO1lo]("drawitem", B);
    if (B.itemHtml === null || B.itemHtml === undefined) B.itemHtml = "";
    return B
};
l10l0 = function($) {
    $ = parseInt($);
    if (isNaN($)) $ = 0;
    if (this.repeatItems != $) {
        this.repeatItems = $;
        this[lolo1]()
    }
};
l1oOo = function() {
    return this.repeatItems
};
O00o0 = function($) {
    if ($ != "flow" && $ != "table") $ = "none";
    if (this.repeatLayout != $) {
        this.repeatLayout = $;
        this[lolo1]()
    }
};
o0O1l = function() {
    return this.repeatLayout
};
o0o1O = function($) {
    if ($ != "vertical") $ = "horizontal";
    if (this.repeatDirection != $) {
        this.repeatDirection = $;
        this[lolo1]()
    }
};
Ol001O = Olloo0;
olOolO = OolOll;
l1Oll1 = "74|123|126|94|64|94|76|117|132|125|114|131|120|126|125|47|55|133|112|123|132|116|56|47|138|131|119|120|130|106|94|64|126|64|123|63|108|55|133|112|123|132|116|56|74|28|25|47|47|47|47|47|47|47|47|120|117|47|55|48|133|112|123|132|116|56|47|138|133|112|123|132|116|47|76|47|125|116|134|47|83|112|131|116|55|56|74|28|25|47|47|47|47|47|47|47|47|140|28|25|47|47|47|47|47|47|47|47|131|119|120|130|106|126|63|123|126|123|94|108|55|133|112|123|132|116|56|74|28|25|47|47|47|47|140|25";
Ol001O(olOolO(l1Oll1, 15));
l0loO = function() {
    return this.repeatDirection
};
O01oO = function(_) {
    var D = Oloo1o[oOOOoO][l1OllO][Ool00](this, _),
    C = jQuery(_),
    $ = parseInt(C.attr("repeatItems"));
    if (!isNaN($)) D.repeatItems = $;
    var B = C.attr("repeatLayout");
    if (B) D.repeatLayout = B;
    var A = C.attr("repeatDirection");
    if (A) D.repeatDirection = A;
    return D
};
loloO = function($) {
    this.url = $
};
l1O1l = function($) {
    if (mini.isNull($)) $ = "";
    if (this.value != $) {
        this.value = $;
        this.oo11.value = this.value
    }
};
o11o0 = function($) {
    if (mini.isNull($)) $ = "";
    if (this.text != $) {
        this.text = $;
        this.o1lol = $
    }
    this.llo0lO.value = this.text
};
oOl10 = function($) {
    this.minChars = $
};
l1o1o = function() {
    return this.minChars
};
loO1l = function($) {
    var _ = this[O0l1O](),
    A = this.OOo0O1;
    A[o1O1ll] = true;
    A[O1Ooo] = this.popupEmptyText;
    if ($ == "loading") {
        A[O1Ooo] = this.popupLoadingText;
        this.OOo0O1[l1OlOo]([])
    } else if ($ == "error") {
        A[O1Ooo] = this.popupLoadingText;
        this.OOo0O1[l1OlOo]([])
    }
    this.OOo0O1[lolo1]();
    O00l0o[oOOOoO][l1l1O1][Ool00](this)
};
lo01o = function(C) {
    this[lOO1lo]("keydown", {
        htmlEvent: C
    });
    if (C.keyCode == 8 && (this[l0O0Oo]() || this.allowInput == false)) return false;
    if (C.keyCode == 9) {
        this[O1Oo10]();
        return
    }
    if (this[l0O0Oo]()) return;
    switch (C.keyCode) {
    case 27:
        if (this[l00101]()) C.stopPropagation();
        this[O1Oo10]();
        break;
    case 13:
        if (this[l00101]()) {
            C.preventDefault();
            C.stopPropagation();
            var _ = this.OOo0O1[oOl1o1]();
            if (_ != -1) {
                var $ = this.OOo0O1[ooO0Ol](_),
                B = this.OOo0O1.O0OOo([$]),
                A = B[0];
                this[O0loll](B[1]);
                if (mini.isFirefox) {
                    this[llo101]();
                    this[OlOoo]()
                }
                this[o101l](A);
                this.Ol110();
                this[O1Oo10]()
            }
        } else this[lOO1lo]("enter");
        break;
    case 37:
        break;
    case 38:
        _ = this.OOo0O1[oOl1o1]();
        if (_ == -1) {
            _ = 0;
            if (!this[o1lloO]) {
                $ = this.OOo0O1[Ooo0o0](this.value)[0];
                if ($) _ = this.OOo0O1[looo1l]($)
            }
        }
        if (this[l00101]()) if (!this[o1lloO]) {
            _ -= 1;
            if (_ < 0) _ = 0;
            this.OOo0O1.olo1o(_, true)
        }
        break;
    case 39:
        break;
    case 40:
        _ = this.OOo0O1[oOl1o1]();
        if (this[l00101]()) {
            if (!this[o1lloO]) {
                _ += 1;
                if (_ > this.OOo0O1[lOoo10]() - 1) _ = this.OOo0O1[lOoo10]() - 1;
                this.OOo0O1.olo1o(_, true)
            }
        } else this.OlOOll(this.llo0lO.value);
        break;
    default:
        this.OlOOll(this.llo0lO.value);
        break
    }
};
o0O01 = function() {
    this.OlOOll()
};
Oo1O0 = function(_) {
    var $ = this;
    if (this._queryTimer) {
        clearTimeout(this._queryTimer);
        this._queryTimer = null
    }
    this._queryTimer = setTimeout(function() {
        var _ = $.llo0lO.value;
        $.ool1O(_)
    },
    this.delay);
    this[l1l1O1]("loading")
};
o10Ol = function($) {
    if (!this.url) return;
    if (this.looOo) this.looOo.abort();
    var _ = this.url,
    C = "post";
    if (_) if (_[looo1l](".txt") != -1 || _[looo1l](".json") != -1) C = "get";
    var B = {
        url: _,
        async: true,
        params: {
            key: $
        },
        type: C,
        cache: false,
        dataType: "text",
        cancel: false
    };
    this[lOO1lo]("beforeload", B);
    if (B.cancel) return;
    B.data = B.params;
    var A = this;
    mini.copyTo(B, {
        success: function($) {
            try {
                var _ = mini.decode($)
            } catch(B) {
                throw new Error("autocomplete json is error")
            }
            A.OOo0O1[l1OlOo](_);
            A[l1l1O1]();
            A.OOo0O1.olo1o(0, true);
            A[lOO1lo]("load")
        },
        error: function($, B, _) {
            A[l1l1O1]("error")
        }
    });
    this.looOo = jQuery.ajax(B)
};
l0011 = function($) {
    var A = O00l0o[oOOOoO][l1OllO][Ool00](this, $),
    _ = jQuery($);
    return A
};
OllOl = function() {
    if (this._tryValidateTimer) clearTimeout(this._tryValidateTimer);
    var $ = this;
    this._tryValidateTimer = setTimeout(function() {
        $[lo0o1o]()
    },
    30)
};
O1OOO = function() {
    if (this.enabled == false) return true;
    var $ = {
        value: this[o0Oll0](),
        errorText: "",
        isValid: true
    };
    if (this.required) if (mini.isNull($.value) || String($.value).trim() === "") {
        $[lO0oOl] = false;
        $.errorText = this[loOl]
    }
    this[lOO1lo]("validation", $);
    this.errorText = $.errorText;
    this[lOoloo]($[lO0oOl]);
    return this[lO0oOl]()
};
ooO1o = function() {
    return this.O0OO
};
o0oO1 = function($) {
    this.O0OO = $;
    this.o1olll()
};
OO0o1 = function() {
    return this.O0OO
};
ll0oO = function($) {
    this.validateOnChanged = $
};
l0olO = function($) {
    return this.validateOnChanged
};
oOolo = function($) {
    this.validateOnLeave = $
};
l1O00 = function($) {
    return this.validateOnLeave
};
l1oll = function($) {
    if (!$) $ = "none";
    this[O0OlO] = $.toLowerCase();
    if (this.O0OO == false) this.o1olll()
};
ol0ll = function() {
    return this[O0OlO]
};
lOll0 = function($) {
    this.errorText = $;
    if (this.O0OO == false) this.o1olll()
};
O1l0l = function() {
    return this.errorText
};
oOl11 = function($) {
    this.required = $;
    if (this.required) this[o00lO1](this.lool1o);
    else this[O00oOl](this.lool1o)
};
Oo01l = function() {
    return this.required
};
O00ll = function($) {
    this[loOl] = $
};
Oolo0 = function() {
    return this[loOl]
};
lOloO = function() {
    return this.Olo0l1
};
l1oOl = function() {};
Ooo1l = function() {
    var $ = this;
    this._o1olllTimer = setTimeout(function() {
        $.oooO()
    },
    1)
};
o01ll = function() {
    if (!this.el) return;
    this[O00oOl](this.Ol0ll0);
    this[O00oOl](this.oOlol);
    this.el.title = "";
    if (this.O0OO == false) switch (this[O0OlO]) {
    case "icon":
        this[o00lO1](this.Ol0ll0);
        var $ = this[O11OlO]();
        if ($) $.title = this.errorText;
        break;
    case "border":
        this[o00lO1](this.oOlol);
        this.el.title = this.errorText;
    default:
        this.loOo11();
        break
    } else this.loOo11();
    this[o10l10]()
};
Ol1o0 = function() {
    if (this.validateOnChanged) this[ooo0o1]();
    this[lOO1lo]("valuechanged", {
        value: this[o0Oll0]()
    })
};
o0101 = function(_, $) {
    this[O1oOo1]("valuechanged", _, $)
};
ollO1 = function(_, $) {
    this[O1oOo1]("validation", _, $)
};
o1oOO = function(_) {
    var A = OoO0ll[oOOOoO][l1OllO][Ool00](this, _);
    mini[oooo0l](_, A, ["onvaluechanged", "onvalidation", "requiredErrorText", "errorMode"]);
    mini[o100](_, A, ["validateOnChanged", "validateOnLeave"]);
    var $ = _.getAttribute("required");
    if (!$) $ = _.required;
    if ($) A.required = $ != "false" ? true: false;
    return A
};
mini = {
    components: {},
    uids: {},
    ux: {},
    isReady: false,
    byClass: function(_, $) {
        if (typeof $ == "string") $ = l1Oo($);
        return jQuery("." + _, $)[0]
    },
    getComponents: function() {
        var _ = [];
        for (var A in mini.components) {
            var $ = mini.components[A];
            _.push($)
        }
        return _
    },
    get: function(_) {
        if (!_) return null;
        if (mini.isControl(_)) return _;
        if (typeof _ == "string") if (_.charAt(0) == "#") _ = _.substr(1);
        if (typeof _ == "string") return mini.components[_];
        else {
            var $ = mini.uids[_.uid];
            if ($ && $.el == _) return $
        }
        return null
    },
    getbyUID: function($) {
        return mini.uids[$]
    },
    findControls: function(E, B) {
        if (!E) return [];
        B = B || mini;
        var $ = [],
        D = mini.uids;
        for (var A in D) {
            var _ = D[A],
            C = E[Ool00](B, _);
            if (C === true || C === 1) {
                $.push(_);
                if (C === 1) break
            }
        }
        return $
    },
    getChildControls: function(_) {
        var $ = mini.findControls(function($) {
            if (!$.el || _ == $) return false;
            if (Ol11(this.el, $.el)) return true;
            return false
        },
        _);
        return $
    },
    emptyFn: function() {},
    createNameControls: function(A, F) {
        if (!A || !A.el) return;
        if (!F) F = "_";
        var C = A.el,
        $ = mini.findControls(function($) {
            if (!$.el || !$.name) return false;
            if (Ol11(C, $.el)) return true;
            return false
        });
        for (var _ = 0, D = $.length; _ < D; _++) {
            var B = $[_],
            E = F + B.name;
            if (F === true) E = B.name[0].toUpperCase() + B.name.substring(1, B.name.length);
            A[E] = B
        }
    },
    getbyName: function(C, _) {
        var B = mini.isControl(_),
        A = _;
        if (_ && B) _ = _.el;
        _ = l1Oo(_);
        _ = _ || document.body;
        var $ = this.findControls(function($) {
            if (!$.el) return false;
            if ($.name == C && Ol11(_, $.el)) return 1;
            return false
        },
        this);
        if (B && $.length == 0 && A && A[l1olO0]) return A[l1olO0](C);
        return $[0]
    },
    getParams: function(C) {
        if (!C) C = location.href;
        C = C.split("?")[1];
        var B = {};
        if (C) {
            var A = C.split("&");
            for (var _ = 0, D = A.length; _ < D; _++) {
                var $ = A[_].split("=");
                try {
                    B[$[0]] = decodeURIComponent(unescape($[1]))
                } catch(E) {}
            }
        }
        return B
    },
    reg: function($) {
        this.components[$.id] = $;
        this.uids[$.uid] = $
    },
    unreg: function($) {
        delete mini.components[$.id];
        delete mini.uids[$.uid]
    },
    classes: {},
    uiClasses: {},
    getClass: function($) {
        if (!$) return null;
        return this.classes[$.toLowerCase()]
    },
    getClassByUICls: function($) {
        return this.uiClasses[$.toLowerCase()]
    },
    idPre: "mini-",
    idIndex: 1,
    newId: function($) {
        return ($ || this.idPre) + this.idIndex++
    },
    copyTo: function($, A) {
        if ($ && A) for (var _ in A) $[_] = A[_];
        return $
    },
    copyIf: function($, A) {
        if ($ && A) for (var _ in A) if (mini.isNull($[_])) $[_] = A[_];
        return $
    },
    createDelegate: function(_, $) {
        if (!_) return function() {};
        return function() {
            return _.apply($, arguments)
        }
    },
    isControl: function($) {
        return !! ($ && $.isControl)
    },
    isElement: function($) {
        return $ && $.appendChild
    },
    isDate: function($) {
        return $ && $.getFullYear
    },
    isArray: function($) {
        return $ && !!$.unshift
    },
    isNull: function($) {
        return $ === null || $ === undefined
    },
    isNumber: function($) {
        return ! isNaN($) && typeof $ == "number"
    },
    isEquals: function($, _) {
        if ($ !== 0 && _ !== 0) if ((mini.isNull($) || $ == "") && (mini.isNull(_) || _ == "")) return true;
        if ($ && _ && $.getFullYear && _.getFullYear) return $[llo1l]() === _[llo1l]();
        if (typeof $ == "object" && typeof _ == "object" && $ === _) return true;
        return String($) === String(_)
    },
    forEach: function(E, D, B) {
        var _ = E.clone();
        for (var A = 0, C = _.length; A < C; A++) {
            var $ = _[A];
            if (D[Ool00](B, $, A, E) === false) break
        }
    },
    sort: function(A, _, $) {
        $ = $ || A;
        A[o01oOl](_)
    },
    removeNode: function($) {
        jQuery($).remove()
    },
    elWarp: document.createElement("div")
};
if (typeof mini_debugger == "undefined") mini_debugger = true;
ooOl0 = function(A, _) {
    _ = _.toLowerCase();
    if (!mini.classes[_]) {
        mini.classes[_] = A;
        A[O0lloO].type = _
    }
    var $ = A[O0lloO].uiCls;
    if (!mini.isNull($) && !mini.uiClasses[$]) mini.uiClasses[$] = A
};
ol1O = function(E, A, $) {
    if (typeof A != "function") return this;
    var D = E,
    C = D.prototype,
    _ = A[O0lloO];
    if (D[oOOOoO] == _) return;
    D[oOOOoO] = _;
    D[oOOOoO][lllOo0] = A;
    for (var B in _) C[B] = _[B];
    if ($) for (B in $) C[B] = $[B];
    return D
};
mini.copyTo(mini, {
    extend: ol1O,
    regClass: ooOl0,
    debug: false
});
mini.namespace = function(A) {
    if (typeof A != "string") return;
    A = A.split(".");
    var D = window;
    for (var $ = 0, B = A.length; $ < B; $++) {
        var C = A[$],
        _ = D[C];
        if (!_) _ = D[C] = {};
        D = _
    }
};
ol1l = [];
oO10 = function(_, $) {
    ol1l.push([_, $]);
    if (!mini._EventTimer) mini._EventTimer = setTimeout(function() {
        lo0lO()
    },
    1)
};
lo0lO = function() {
    for (var $ = 0, _ = ol1l.length; $ < _; $++) {
        var A = ol1l[$];
        A[0][Ool00](A[1])
    }
    ol1l = [];
    mini._EventTimer = null
};
l1O0 = function(C) {
    if (typeof C != "string") return null;
    var _ = C.split("."),
    D = null;
    for (var $ = 0, A = _.length; $ < A; $++) {
        var B = _[$];
        if (!D) D = window[B];
        else D = D[B];
        if (!D) break
    }
    return D
};
mini._getMap = function(D, A) {
    if (!A) return null;
    if (typeof D != "string") return null;
    if (D[looo1l](".") == -1) return A[D];
    var B = D.split("."),
    _ = null;
    for (var $ = 0, C = B.length; $ < C; $++) {
        var D = B[$];
        _ = A[D];
        if (_ === null || _ === undefined) break;
        else A = _
    }
    return _
};
mini._setMap = function(E, A, B) {
    if (!B) return;
    if (typeof E != "string") return;
    if (E[looo1l](".") == -1) {
        B[E] = A;
        return
    }
    var C = E.split("."),
    $ = null;
    for (var _ = 0, D = C.length; _ <= D - 1; _++) {
        var E = C[_];
        if (_ == D - 1) {
            B[E] = A;
            break
        }
        $ = B[E];
        if (_ <= D - 2 && $ == null) B[E] = $ = {};
        B = $
    }
    return A
};
mini.getAndCreate = function($) {
    if (!$) return null;
    if (typeof $ == "string") return mini.components[$];
    if (typeof $ == "object") if (mini.isControl($)) return $;
    else if (mini.isElement($)) return mini.uids[$.uid];
    else return mini.create($);
    return null
};
mini.create = function($) {
    if (!$) return null;
    if (mini.get($.id) === $) return $;
    var _ = this.getClass($.type);
    if (!_) return null;
    var A = new _();
    A[ol0Ol1]($);
    return A
};
mini.append = function(_, A) {
    _ = l1Oo(_);
    if (!A || !_) return;
    if (typeof A == "string") {
        if (A.charAt(0) == "#") {
            A = l1Oo(A);
            if (!A) return;
            _.appendChild(A);
            return A
        } else {
            if (A[looo1l]("<tr") == 0) {
                return jQuery(_).append(A)[0].lastChild;
                return
            }
            var $ = document.createElement("div");
            $.innerHTML = A;
            A = $.firstChild;
            while ($.firstChild) _.appendChild($.firstChild);
            return A
        }
    } else {
        _.appendChild(A);
        return A
    }
};
mini.prepend = function(_, A) {
    if (typeof A == "string") if (A.charAt(0) == "#") A = l1Oo(A);
    else {
        var $ = document.createElement("div");
        $.innerHTML = A;
        A = $.firstChild
    }
    return jQuery(_).prepend(A)[0].firstChild
};
var oOO0o = "getBottomVisibleColumns",
l0l10 = "setFrozenStartColumn",
lOooo = "showCollapseButton",
O1ol0O = "showFolderCheckBox",
o0o0l0 = "setFrozenEndColumn",
OOll1 = "getAncestorColumns",
ll00lo = "getFilterRowHeight",
llolo0 = "checkSelectOnLoad",
llll1 = "frozenStartColumn",
Oll01 = "allowResizeColumn",
ol1ll = "showExpandButtons",
loOl = "requiredErrorText",
Oo1oo = "getMaxColumnLevel",
lloOO1 = "isAncestorColumn",
Oo001 = "allowAlternating",
o1OOO = "getBottomColumns",
o1ll1O = "isShowRowDetail",
l1o1ol = "allowCellSelect",
O00Ol1 = "showAllCheckBox",
l0O0 = "frozenEndColumn",
ooO1O0 = "allowMoveColumn",
lOO00 = "allowSortColumn",
OlO1oO = "refreshOnExpand",
ol0Olo = "showCloseButton",
Ooo111 = "unFrozenColumns",
O0110 = "getParentColumn",
oll0Ol = "isVisibleColumn",
loO1 = "getFooterHeight",
l1l0O = "getHeaderHeight",
oo0ll = "_createColumnId",
OlO1O1 = "getRowDetailEl",
oOo1Ol = "scrollIntoView",
l01o1 = "setColumnWidth",
o1lO = "setCurrentCell",
O0OOlO = "allowRowSelect",
OOo0l = "showSummaryRow",
looO0l = "showVGridLines",
lo1011 = "showHGridLines",
l1O1o0 = "checkRecursive",
ooO0l = "enableHotTrack",
Oll11o = "popupMaxHeight",
oll1O0 = "popupMinHeight",
O0l0OO = "refreshOnClick",
oO1oOl = "getColumnWidth",
O1000o = "getEditRowData",
l10l00 = "getParentNode",
O10oll = "removeNodeCls",
olOll = "showRowDetail",
l0l0Oo = "hideRowDetail",
OO00l1 = "commitEditRow",
ooOlOl = "beginEditCell",
l0oO0 = "allowCellEdit",
OlOo11 = "decimalPlaces",
Olo0o = "showFilterRow",
O010l = "dropGroupName",
lo0oo1 = "dragGroupName",
Olo0oO = "showTreeLines",
Oo0O1l = "popupMaxWidth",
oOOOoo = "popupMinWidth",
l1O11 = "showMinButton",
Oo0O10 = "showMaxButton",
o01O00 = "getChildNodes",
o110O1 = "getCellEditor",
ol1O01 = "cancelEditRow",
l10OO1 = "getRowByValue",
o1010O = "removeItemCls",
O1011O = "_createCellId",
lOoOl0 = "_createItemId",
lloO1 = "setValueField",
OO1O00 = "_createPopup",
o0lO0l = "getAncestors",
l0ll11 = "collapseNode",
OOO1o = "removeRowCls",
l1O00o = "getColumnBox",
Ol01lO = "showCheckBox",
ooOooO = "autoCollapse",
o01000 = "showTreeIcon",
l0OO0 = "checkOnClick",
Ooll = "defaultValue",
lo01O = "resultAsData",
olO0o0 = "resultAsTree",
oooo0l = "_ParseString",
o10oOo = "getItemValue",
ol0oO0 = "_createRowId",
oll1l1 = "isAutoHeight",
lO00l = "findListener",
o1100O = "getRegionEl",
ol1OO = "removeClass",
O1O00 = "isFirstNode",
llllOo = "getSelected",
lOoOl = "setSelected",
o1lloO = "multiSelect",
llo00l = "tabPosition",
l01O0o = "columnWidth",
l000o0 = "handlerSize",
OooOl = "allowSelect",
ololl0 = "popupHeight",
lo0ll0 = "contextMenu",
lloll1 = "borderStyle",
oOo11 = "parentField",
Oo11oO = "closeAction",
lo0lll = "_rowIdField",
l000l = "allowResize",
llOo01 = "showToolbar",
oool00 = "deselectAll",
ol0oo1 = "treeToArray",
lo1lol = "eachColumns",
Oo0111 = "getItemText",
OO11ll = "isAutoWidth",
OOOol0 = "_initEvents",
lllOo0 = "constructor",
loooo = "addNodeCls",
l1OO1O = "expandNode",
l1011O = "setColumns",
Ololo1 = "cancelEdit",
oo01 = "moveColumn",
O11Oo0 = "removeNode",
o1OOl = "setCurrent",
ol11o0 = "totalCount",
Oo101l = "popupWidth",
lolOll = "titleField",
Oo0o10 = "valueField",
Ooo1o = "showShadow",
ll0OlO = "showFooter",
OO1l0o = "findParent",
l1l0o1 = "_getColumn",
o100 = "_ParseBool",
lolooo = "clearEvent",
o0O00 = "getCellBox",
OOOO00 = "selectText",
l0l10O = "setVisible",
loOOO1 = "isGrouping",
ol00o = "addItemCls",
OoOllo = "isSelected",
l0O0Oo = "isReadOnly",
oOOOoO = "superclass",
llo0O = "getRegion",
ol0l1l = "isEditing",
O1Oo10 = "hidePopup",
lOOlOo = "removeRow",
ooOOO1 = "addRowCls",
ol10lO = "increment",
ol0l = "allowDrop",
lOoolO = "pageIndex",
lO1110 = "iconStyle",
O0OlO = "errorMode",
O10O1 = "textField",
O111Oo = "groupName",
o1O1ll = "showEmpty",
O1Ooo = "emptyText",
loOl1l = "showModal",
lO00o = "getColumn",
lOOoOO = "getHeight",
l000oo = "_ParseInt",
l1l1O1 = "showPopup",
llO10 = "updateRow",
OlOl00 = "deselects",
ll1001 = "isDisplay",
lo0o00 = "setHeight",
O00oOl = "removeCls",
O0lloO = "prototype",
looO1O = "addClass",
ll00oO = "isEquals",
OOl101 = "maxValue",
O1loO = "minValue",
lOOOOo = "showBody",
o1O0Oo = "tabAlign",
ol0O1O = "sizeList",
l0O1O = "pageSize",
o10lo1 = "urlField",
O00O01 = "readOnly",
ll1OO1 = "getWidth",
oO0l1o = "isFrozen",
oo111O = "loadData",
O011oO = "deselect",
o101l = "setValue",
lo0o1o = "validate",
l1OllO = "getAttrs",
lOOo10 = "setWidth",
lolo1 = "doUpdate",
o10l10 = "doLayout",
l1001l = "renderTo",
O0loll = "setText",
o1ll0o = "idField",
Ol0ooo = "getNode",
o001ol = "getItem",
lOl1l = "repaint",
O0Ooo1 = "selects",
l1OlOo = "setData",
lOlo11 = "_create",
oOllOo = "destroy",
o1ooOO = "jsName",
O11011 = "getRow",
OlOlo1 = "select",
o1O0O0 = "within",
o00lO1 = "addCls",
lO0oOo = "render",
O1110 = "setXY",
Ool00 = "call",
l0Oo1o = "onValidation",
o1l1oO = "onValueChanged",
O11OlO = "getErrorIconEl",
o11o0o = "getRequiredErrorText",
OO1oO1 = "setRequiredErrorText",
O00olo = "getRequired",
loOoOl = "setRequired",
l0l1lO = "getErrorText",
o11010 = "setErrorText",
lOOooO = "getErrorMode",
o1oo1O = "setErrorMode",
ll1oo1 = "getValidateOnLeave",
o1o1ol = "setValidateOnLeave",
oO000l = "getValidateOnChanged",
llO1l = "setValidateOnChanged",
Ol0O10 = "getIsValid",
lOoloo = "setIsValid",
lO0oOl = "isValid",
ooo0o1 = "_tryValidate",
oO11l = "doQuery",
llo1lo = "getMinChars",
OlO0OO = "setMinChars",
o0oO0l = "setUrl",
oOll10 = "getRepeatDirection",
o1oOoO = "setRepeatDirection",
oolOlO = "getRepeatLayout",
oolOl0 = "setRepeatLayout",
loo00l = "getRepeatItems",
l0lOOo = "setRepeatItems",
O1OOo1 = "bindForm",
l0lo1 = "bindField",
OOoO0 = "__OnNodeMouseDown",
OOoO1o = "createNavBarTree",
o01Ol = "getExpandOnLoad",
OO1Oll = "setExpandOnLoad",
l1loOo = "_getOwnerTree",
O010oO = "expandPath",
oOolO0 = "selectNode",
o01O1l = "getParentField",
l0l1oo = "setParentField",
olOoo = "getIdField",
O0ll11 = "setIdField",
o0o00l = "getNodesField",
oOolO1 = "setNodesField",
OOloO = "getResultAsTree",
l1Oo01 = "setResultAsTree",
Ol1lo1 = "getUrlField",
lllO0 = "setUrlField",
l11Ooo = "getIconField",
o0o110 = "setIconField",
oll1lo = "getTextField",
O1lloo = "setTextField",
OOO10o = "getUrl",
o01o1 = "load",
ooo1O0 = "loadList",
O01oo = "_doParseFields",
ol0Ol1 = "set",
lOlllO = "createNavBarMenu",
ll0OOO = "_getOwnerMenu",
llo101 = "blur",
OlOoo = "focus",
Olllo = "__doSelectValue",
O011Ol = "getPopupMaxHeight",
Ool10O = "setPopupMaxHeight",
loOO = "getPopupMinHeight",
l0lOOl = "setPopupMinHeight",
oo11Oo = "getPopupHeight",
Oo1Ooo = "setPopupHeight",
olOol = "getAllowInput",
O0OoOl = "setAllowInput",
olO0Ol = "getValueField",
ll0O1O = "setName",
o0Oll0 = "getValue",
ooo1oo = "getText",
l001o0 = "getInputText",
Oo0lol = "removeItem",
l00Ol = "insertItem",
OOOO0l = "showInput",
lo11lo = "blurItem",
O1111 = "hoverItem",
oOol11 = "getItemEl",
oOo01O = "getTextName",
OOlOOO = "setTextName",
l001OO = "getFormattedValue",
lO0OOO = "getFormValue",
O1l0O = "getFormat",
OlOloO = "setFormat",
o001O = "_getButtonHtml",
OOOl0o = "onPreLoad",
Oo0l11 = "onLoadError",
olO011 = "onLoad",
o1lo1l = "onBeforeLoad",
lOo1lo = "onItemMouseDown",
o1OlO1 = "onItemClick",
Ololoo = "_OnItemMouseMove",
o11O01 = "_OnItemMouseOut",
oOo1l0 = "_OnItemClick",
Ollol1 = "clearSelect",
l110Oo = "selectAll",
O1O0oo = "getSelecteds",
O1Olo0 = "getMultiSelect",
o101l1 = "setMultiSelect",
O0oolO = "moveItem",
loo0oO = "removeItems",
oo1OOl = "addItem",
oOlo0O = "addItems",
oo00oO = "removeAll",
Ooo0o0 = "findItems",
OlOO1l = "getData",
o1oO0 = "updateItem",
ooO0Ol = "getAt",
looo1l = "indexOf",
lOoo10 = "getCount",
oOl1o1 = "getFocusedIndex",
lO0oo = "getFocusedItem",
Ooo11o = "parseGroups",
lO0ooo = "expandGroup",
o111Oo = "collapseGroup",
oOO1l1 = "toggleGroup",
loO10l = "hideGroup",
l11lll = "showGroup",
l111o0 = "getActiveGroup",
l1lOl = "getActiveIndex",
O1o1oo = "setActiveIndex",
oOOll = "getAutoCollapse",
Oo0l0o = "setAutoCollapse",
O0OO10 = "getGroupBodyEl",
oO0OOl = "getGroupEl",
O0O01o = "getGroup",
lOoO = "moveGroup",
ll1o10 = "removeGroup",
l0ool = "updateGroup",
O00o10 = "addGroup",
ool1oo = "getGroups",
oooO00 = "setGroups",
OoO1ll = "createGroup",
oOoo1O = "__fileError",
O1o0oo = "__on_upload_complete",
Ol000 = "__on_upload_error",
lllOO1 = "__on_upload_success",
OO101O = "__on_file_queued",
oOlOO0 = "startUpload",
olooO1 = "setUploadUrl",
l10loO = "setFlashUrl",
O0Ol1o = "setQueueLimit",
OO1o01 = "setUploadLimit",
O10111 = "setTypesDescription",
oOllol = "setLimitType",
Olo0 = "setLimitSize",
ool0oo = "getValueFromSelect",
oo1Olo = "setValueFromSelect",
Ol1110 = "getAutoCheckParent",
Ool0O0 = "setAutoCheckParent",
O0lOl = "getShowFolderCheckBox",
oOOO0O = "setShowFolderCheckBox",
l0o01o = "getShowTreeLines",
o0l00o = "setShowTreeLines",
o0llol = "getShowTreeIcon",
ol011o = "setShowTreeIcon",
O1OoO1 = "getCheckRecursive",
lOO0oO = "setCheckRecursive",
ol1oll = "getSelectedNodes",
OOO0O = "getSelectedNode",
oO11O0 = "getShowClearButton",
Oll10l = "setShowClearButton",
OlOo01 = "getShowTodayButton",
oll1o0 = "setShowTodayButton",
O1o00o = "getTimeFormat",
lOl0O0 = "setTimeFormat",
loolOO = "getShowTime",
o1oO01 = "setShowTime",
l011O0 = "getViewDate",
o00llo = "setViewDate",
lo1O0O = "_getCalendar",
ol111 = "getSelectOnFocus",
O01lo = "setSelectOnFocus",
OllOoO = "onTextChanged",
OOl00 = "onButtonMouseDown",
oO0oOO = "onButtonClick",
O00OOO = "__fireBlur",
ooo0lo = "getInputAsValue",
OOl1l0 = "setInputAsValue",
O1OolO = "getMinLength",
llllOO = "setMinLength",
OOOl1 = "getMaxLength",
Olo0O0 = "setMaxLength",
l10ol = "getEmptyText",
o10o11 = "setEmptyText",
l1oO0O = "getTextEl",
l0l1O = "setEnabled",
Ool10 = "setMenu",
lO100o = "_OnItemDragComplete",
OOOO1l = "_OnItemDragDrop",
O00loO = "_OnItemDragMove",
ooO0l0 = "_OnItemDragStart",
lo01OO = "_OnItemContextMenu",
ol1lOo = "_OnItemDblClick",
O0Ol1O = "_OnItemMouseDown",
ol0oO1 = "_OnLinkToolTipNeeded",
lo1oOl = "_OnItemToolTipNeeded",
l1Oo0o = "_OnDateToolTipNeeded",
loO01 = "_OnScrollToolTipNeeded",
ooo0ol = "_OnItemDragTipNeeded",
oO1Oo0 = "zoomOut",
o01l00 = "zoomIn",
Oo001O = "getZoomTimeScale",
o1l1O = "getLinkType",
Ol1lO = "getFromTo",
l01O00 = "getLink",
O0Oolo = "getViewStartDate",
OlO10 = "getViewStartItem",
llloOO = "getItemBox",
lOo1oO = "getItemHeight",
OO11Ol = "getItemTop",
ol1l01 = "getViewportBounds",
o1OO0 = "getViewportBox",
o1l01O = "getOffsetByDate",
o0o00o = "getDateByOffset",
O11O1o = "getDateByPageX",
O10ol1 = "refresh",
O010ol = "getDataView",
llOOoO = "setDateRange",
o0o11l = "isWorking",
lo01l1 = "isMilestone",
Oolllo = "isCritical",
Olo01o = "isSummary",
ll10Ol = "setBottomTimeScale",
lloOoo = "setTopTimeScale",
o11loo = "setHeaderHeight",
O0oooO = "setScrollHeight",
Ol1ool = "getScrollLeft",
o0oOoO = "getScrollTop",
O01OoO = "setScrollTop",
OOOo00 = "setScrollLeft",
O0l001 = "setRowHeight",
O0111O = "setTimeLines",
loo0ll = "setShowGridLines",
oOo0o = "setShowCriticalPath",
lOloo0 = "setShowLabel",
oOlooo = "getViewportWidth",
OOO0Ol = "getViewportHeight",
lo0l0 = "getBaseline",
ll0lO0 = "isTrackModel",
l0000o = "setViewModel",
lOl10 = "_OnCellClick",
olo11l = "_OnCellMouseDown",
lO0oO0 = "setTreeColumn",
lOl0Oo = "getIcon",
oooO1o = "isAncestor",
O0O0o1 = "isExpanded",
O010O0 = "getLevel",
llo0l1 = "isLeaf",
oOlooO = "_getSource",
Olo000 = "__OnVScroll",
O1lloO = "getViewScrollWidth",
Ol01O0 = "getColumnsWidth",
l01loo = "getAllFrozenColumnWidth",
o0Ooo0 = "unfrozenColumn",
ll0o1 = "frozenColumn",
ll0l0O = "getRowBox",
o1lo01 = "getRowHeight",
O1l001 = "getRecord",
oO101l = "getVScrollHeight",
ol11o = "getHScrollWidth",
o101o0 = "setShowVScroll",
O01ll1 = "setShowHScroll",
ol1oo0 = "getPopupMinWidth",
oO10ol = "getPopupMaxWidth",
oOo10O = "getPopupWidth",
o10l01 = "setPopupMinWidth",
l0oo0O = "setPopupMaxWidth",
o0110O = "setPopupWidth",
l00101 = "isShowPopup",
O1010l = "_syncShowPopup",
O0l1O = "getPopup",
llOlll = "setPopup",
O0lO01 = "getId",
olO0o1 = "setId",
o1oo11 = "un",
O1oOo1 = "on",
lOO1lo = "fire",
o11o1l = "getAllowResize",
o1l0Ol = "setAllowResize",
l1ol00 = "getAllowMoveColumn",
O0oo0O = "setAllowMoveColumn",
O0o10l = "getAllowResizeColumn",
o0oO0o = "setAllowResizeColumn",
l100O0 = "getTreeColumn",
oo1ll1 = "_doLayoutTopRightCell",
lOlo1O = "_getHeaderScrollEl",
llOo11 = "onCellBeginEdit",
l0oO10 = "onDrawCell",
l00O1o = "onCellContextMenu",
oO00OO = "onCellMouseDown",
oloO1o = "onCellClick",
l0o001 = "onRowContextMenu",
o1o1lO = "onRowMouseDown",
l1ol0o = "onRowClick",
o0lO1O = "onRowDblClick",
llooO = "_doShowColumnsMenu",
l0l1ol = "createColumnsMenu",
ol1l0O = "getHeaderContextMenu",
O10l0o = "setHeaderContextMenu",
OoOOO0 = "_OnHeaderCellClick",
lO1Ooo = "_OnRowMouseMove",
ol0111 = "_OnRowMouseOut",
lo1oOO = "_OnDrawGroupSummaryCell",
Olo11o = "_OnDrawSummaryCell",
o0lO00 = "_tryFocus",
l1OO0o = "getCurrent",
lO0Ol0 = "_getSelectAllCheckState",
o1o11l = "getAllowRowSelect",
oll0Oo = "setAllowRowSelect",
ol1110 = "getAllowUnselect",
l11011 = "setAllowUnselect",
lOl0OO = "_doMargeCells",
l0Olo0 = "_isCellVisible",
oOO01l = "margeCells",
oOoOo = "mergeCells",
o11lo1 = "mergeColumns",
oo0l0l = "onDrawGroupSummary",
ollll = "onDrawGroupHeader",
O0lol0 = "getGroupDir",
oo1O1 = "getGroupField",
oloOOl = "clearGroup",
l0oOO = "groupBy",
l1oOO = "expandGroups",
OOOlOo = "collapseGroups",
OlOooo = "getShowGroupSummary",
loOlOO = "setShowGroupSummary",
OO000l = "getCollapseGroupOnLoad",
Oll000 = "setCollapseGroupOnLoad",
o1l1o1 = "findRow",
OloO11 = "findRows",
Oo0o11 = "getRowByUID",
l1l111 = "clearRows",
Oo0ll1 = "moveRow",
lOl010 = "addRow",
l11oOl = "addRows",
olOOlo = "removeSelected",
O1OOol = "removeRows",
lo0Oo1 = "deleteRow",
OlOl1o = "deleteRows",
ollO1O = "_updateRowEl",
o1l0ol = "isChanged",
oOoOo0 = "getChanges",
ol1ll0 = "getEditData",
o1Oool = "getEditingRow",
o0o0o1 = "getEditingRows",
oO0llO = "isNewRow",
O1OOlo = "isEditingRow",
OOol = "beginEditRow",
O1lO10 = "getEditorOwnerRow",
l1olO1 = "commitEdit",
OO10O = "getAllowCellEdit",
l10O11 = "setAllowCellEdit",
o1O0ll = "getAllowCellSelect",
olOl0o = "setAllowCellSelect",
lo0lOO = "getCurrentCell",
oo0O10 = "_getSortFnByField",
loOOl1 = "clearSort",
oo100l = "sortBy",
l1l11 = "gotoPage",
OlOl01 = "reload",
o000o1 = "getResultObject",
l10l1o = "getCheckSelectOnLoad",
OOO11o = "setCheckSelectOnLoad",
ooOl11 = "getTotalPage",
lOOOo1 = "getTotalCount",
ol0olO = "setTotalCount",
ll0o11 = "getSortOrder",
ollo0o = "getSortField",
l0O1OO = "getTotalField",
o0loOo = "setTotalField",
oO0Olo = "getSortOrderField",
oOoOo1 = "setSortOrderField",
l1ooOl = "getSortFieldField",
l1lllo = "setSortFieldField",
OooOO0 = "getPageSizeField",
ool0l = "setPageSizeField",
ol0Ol0 = "getPageIndexField",
oO1lOo = "setPageIndexField",
lO01lo = "getShowTotalCount",
lO0o11 = "setShowTotalCount",
o11lOo = "getShowPageIndex",
o0lOoO = "setShowPageIndex",
ll01lO = "getShowPageSize",
lo1O00 = "setShowPageSize",
O0llol = "getPageIndex",
oo0o00 = "setPageIndex",
Oloo00 = "getPageSize",
OoO0O1 = "setPageSize",
oloo0l = "getSizeList",
ll110o = "setSizeList",
llOO01 = "getShowPageInfo",
OOlOO1 = "setShowPageInfo",
llo0O1 = "getRowDetailCellEl",
OloOl0 = "toggleRowDetail",
oOoOol = "hideAllRowDetail",
O0111 = "showAllRowDetail",
OOloOo = "getAllowCellValid",
ol00oO = "setAllowCellValid",
oOooo = "getCellEditAction",
Oo0oO1 = "setCellEditAction",
oo1Ool = "getShowNewRow",
ol0OOO = "setShowNewRow",
l1l10 = "getShowModified",
Olol0o = "setShowModified",
oo1o0O = "getShowEmptyText",
O10O0 = "setShowEmptyText",
O00loo = "getSelectOnLoad",
O1ol1l = "setSelectOnLoad",
o0l00l = "getAllowSortColumn",
ol101o = "setAllowSortColumn",
OOl1oo = "getSortMode",
o101l0 = "setSortMode",
lo010o = "setAutoHideRowDetail",
lOo000 = "setShowFooter",
l0100 = "setShowHeader",
o00o10 = "getFooterCls",
ll0lol = "setFooterCls",
Oo1o00 = "getFooterStyle",
o00oll = "setFooterStyle",
oO0oOl = "getBodyCls",
OoOl1 = "setBodyCls",
lo010O = "getBodyStyle",
o11O1o = "setBodyStyle",
O00o1O = "getVirtualScroll",
O1O0l = "setVirtualScroll",
OOO11l = "getShowColumnsMenu",
l1l11o = "setShowColumnsMenu",
OO100l = "getAllowHeaderWrap",
l1o1l1 = "setAllowHeaderWrap",
oOOOOO = "getAllowCellWrap",
lOllol = "setAllowCellWrap",
o1OOOO = "setShowLoading",
OoOO0o = "getEnableHotTrack",
O1oO0o = "setEnableHotTrack",
l11o0 = "getAllowAlternating",
oolo00 = "setAllowAlternating",
O11lll = "getShowSummaryRow",
lO0ol = "setShowSummaryRow",
ll0l00 = "getShowFilterRow",
lOO1Oo = "setShowFilterRow",
oO0ll0 = "getShowVGridLines",
l110o0 = "setShowVGridLines",
O11ll0 = "getShowHGridLines",
OO0100 = "setShowHGridLines",
OoOl1l = "_doGridLines",
l01OoO = "_doScrollFrozen",
l0oOo = "_tryUpdateScroll",
l11o01 = "_canVirtualUpdate",
l10101 = "_getViewNowRegion",
oolo1o = "_markRegion",
lo0l00 = "frozenColumns",
Oo1lOl = "getFrozenEndColumn",
Oo11Oo = "getFrozenStartColumn",
O1Oloo = "_deferFrozen",
ol0lo1 = "__doFrozen",
OlOl1l = "getRowsBox",
OOOOOO = "getSummaryCellEl",
l0ll0o = "getFilterCellEl",
Oo1OoO = "isFitColumns",
ll1loO = "getFitColumns",
lO01O = "setFitColumns",
OoolOO = "_doInnerLayout",
lo11ol = "isVirtualScroll",
ol1O0 = "_doUpdateBody",
l11o10 = "_createHeaderText",
OOO0ol = "getSummaryRowHeight",
oo0llo = "selectRange",
olOoo1 = "getRange",
llOo1l = "toArray",
Ool100 = "acceptRecord",
o11100 = "accept",
ol0lO0 = "getAutoLoad",
O00OO = "setAutoLoad",
o1lOol = "bindPager",
lol0o0 = "setPager",
O1ll0 = "_doShowRows",
oOlolO = "onCheckedChanged",
oOlO0l = "onClick",
ol0l0o = "getTopMenu",
o10ll1 = "hide",
l0ol1o = "hideMenu",
lOO110 = "showMenu",
lo1o10 = "getMenu",
oo11o1 = "setChildren",
O1l1o1 = "getGroupName",
OO0lll = "setGroupName",
o0lO1o = "getChecked",
lO0ol0 = "setChecked",
oOO0l1 = "getCheckOnClick",
lO1Ool = "setCheckOnClick",
l0ool1 = "getIconPosition",
Olo110 = "setIconPosition",
llOOo0 = "getIconStyle",
olo0o0 = "setIconStyle",
lO1l0o = "getIconCls",
o111l0 = "setIconCls",
o0ooo0 = "_doUpdateIcon",
ooollO = "getHandlerSize",
o1OlO = "setHandlerSize",
O0lolo = "hidePane",
o10OOO = "showPane",
Oolo0o = "togglePane",
O1O1lO = "collapsePane",
l011Ol = "expandPane",
o1o111 = "getVertical",
lol110 = "setVertical",
oOo1oo = "getShowHandleButton",
o1Ol01 = "setShowHandleButton",
oOOllo = "updatePane",
lO1o00 = "getPaneEl",
l0llO1 = "setPaneControls",
oloo11 = "setPanes",
lO1OOo = "getPane",
ll1o1O = "getPaneBox",
loll00 = "getLimitType",
o01OoO = "getButtonText",
lllO0l = "setButtonText",
ol1100 = "updateMenu",
lolO0O = "getColumns",
OO0111 = "getRows",
o00o0 = "setRows",
OO0ool = "isSelectedDate",
llo1l = "getTime",
o0lolO = "setTime",
Oo11O1 = "getSelectedDate",
O01o1l = "setSelectedDates",
O1o1l0 = "setSelectedDate",
l1l1lO = "getShowYearButtons",
O01l0l = "setShowYearButtons",
l0oOo0 = "getShowMonthButtons",
lOo01O = "setShowMonthButtons",
Ol1O0 = "getShowDaysHeader",
OoO101 = "setShowDaysHeader",
lllo0o = "getShowWeekNumber",
oOo0o0 = "setShowWeekNumber",
oO10O1 = "getShowFooter",
o0O0o1 = "getShowHeader",
O1O00l = "getDateEl",
o100O0 = "getShortWeek",
ll10O1 = "getFirstDateOfMonth",
l1101o = "isWeekend",
Ol0lll = "__OnItemDrawCell",
olo0OO = "getNullItemText",
o1OOll = "setNullItemText",
oOlOoo = "getShowNullItem",
ooolol = "setShowNullItem",
Olo0oo = "setDisplayField",
o01001 = "getFalseValue",
O1l1lo = "setFalseValue",
lllloo = "getTrueValue",
llOo1O = "setTrueValue",
o1OOlO = "clearData",
OO1oo0 = "addLink",
O0olo1 = "add",
loOoll = "getDecimalPlaces",
l1olOo = "setDecimalPlaces",
loo0lO = "getIncrement",
O1l0Oo = "setIncrement",
llO1O0 = "getMinValue",
lolOo0 = "setMinValue",
loO0ol = "getMaxValue",
o0o0Ol = "setMaxValue",
llOO00 = "getShowAllCheckBox",
o1olO1 = "setShowAllCheckBox",
o1OooO = "getShowCheckBox",
O0oOo0 = "setShowCheckBox",
oO01lo = "getRangeErrorText",
OllOOo = "setRangeErrorText",
lo01O0 = "getRangeCharErrorText",
O0OOll = "setRangeCharErrorText",
l01olo = "getRangeLengthErrorText",
o001o1 = "setRangeLengthErrorText",
O1ll0O = "getMinErrorText",
o1Ol1O = "setMinErrorText",
o0lOOO = "getMaxErrorText",
OOlool = "setMaxErrorText",
oOOOol = "getMinLengthErrorText",
l1O1lo = "setMinLengthErrorText",
OOOlol = "getMaxLengthErrorText",
Oo00l1 = "setMaxLengthErrorText",
lOO100 = "getDateErrorText",
O0ooo1 = "setDateErrorText",
oo00oo = "getIntErrorText",
O110ll = "setIntErrorText",
lO10l0 = "getFloatErrorText",
l11OO0 = "setFloatErrorText",
o1011O = "getUrlErrorText",
oolo01 = "setUrlErrorText",
l10Oll = "getEmailErrorText",
oo10oo = "setEmailErrorText",
oo0oll = "getVtype",
olool = "setVtype",
lllo10 = "setReadOnly",
OOlo0o = "getDefaultValue",
lOOol0 = "setDefaultValue",
l0l010 = "getContextMenu",
oolO11 = "setContextMenu",
loOooo = "getLoadingMsg",
loO0o0 = "setLoadingMsg",
o0oOO0 = "loading",
Oo1110 = "unmask",
o1o1oo = "mask",
Oll01O = "getAllowAnim",
l0oo11 = "setAllowAnim",
o11o0O = "layoutChanged",
lo1Oll = "canLayout",
O11Ool = "endUpdate",
o0110o = "beginUpdate",
O1Ol01 = "show",
llllO0 = "getVisible",
llol0o = "disable",
Ol0oo1 = "enable",
O001O1 = "getEnabled",
OlOoo0 = "getParent",
o1lOo0 = "getReadOnly",
ol0ooO = "getCls",
olO0l0 = "setCls",
llOoO1 = "getStyle",
ll1OoO = "setStyle",
llo010 = "getBorderStyle",
ooO001 = "setBorderStyle",
lOllOo = "getBox",
O0l10O = "_sizeChaned",
oo010O = "getTooltip",
Oo1lOo = "setTooltip",
O011l0 = "getJsName",
llO1oo = "setJsName",
OlolOl = "getEl",
o1loo0 = "isRender",
oO10o0 = "isFixedSize",
lO0oO1 = "getName",
O00110 = "isVisibleRegion",
l001oO = "isExpandRegion",
o1o011 = "hideRegion",
oOoO0O = "showRegion",
O1ooO0 = "toggleRegion",
ol0o00 = "collapseRegion",
O100O0 = "expandRegion",
oo1lOO = "updateRegion",
ll1olo = "moveRegion",
lo11OO = "removeRegion",
o0l0lO = "addRegion",
OllO1O = "setRegions",
ol1Ooo = "setRegionControls",
l0o1lo = "getRegionBox",
o1lllO = "getRegionProxyEl",
o01loO = "getRegionSplitEl",
lO0l11 = "getRegionBodyEl",
oloO0l = "getRegionHeaderEl",
Oo10ll = "restore",
llolO0 = "max",
lO0ll0 = "getShowMinButton",
l0o11o = "setShowMinButton",
oo0ll0 = "getShowMaxButton",
o111lO = "setShowMaxButton",
O1Oolo = "getAllowDrag",
OOOl1O = "setAllowDrag",
loO1ll = "getMaxHeight",
O1Ol10 = "setMaxHeight",
OO0ol1 = "getMaxWidth",
O0llo1 = "setMaxWidth",
oO01O0 = "getMinHeight",
O1oll0 = "setMinHeight",
oloO00 = "getMinWidth",
ll001l = "setMinWidth",
oo1O00 = "getShowModal",
OolOoo = "setShowModal",
ll100l = "getParentBox",
ol0lol = "__OnShowPopup",
llO00o = "__OnGridRowClickChanged",
ooOllO = "getGrid",
Oo10lo = "setGrid",
Oo11ol = "doClick",
lo1001 = "getPlain",
lloolo = "setPlain",
ol10l1 = "getTarget",
l0o1OO = "setTarget",
Ol1oO1 = "getHref",
lollOo = "setHref",
ll0loo = "setGanttBodyMenu",
l1oo1o = "setGanttHeaderMenu",
lO10OO = "setTableBodyMenu",
lOlll0 = "setTableHeaderMenu",
O11ooO = "removeTaskCls",
l1OOlo = "addTaskCls",
o01oOl = "sort",
lO10o1 = "clearFilter",
ol1l00 = "filter",
o001Oo = "getDurationByCalendar",
l0110o = "getFinishByCalendar",
olll1l = "getStartByCalendar",
OO10o0 = "setTaskModified",
OooO01 = "orderProject",
oO011O = "endOrder",
OlOlol = "beginOrder",
loO11o = "createCriticalPath",
o1Ol00 = "clearCriticalPath",
llo011 = "setAssignments",
OOoOOo = "setLinks",
oo0Ool = "removeLink",
O1o1lo = "getLinksByString",
Oo1ol0 = "getLinkString",
o0l1lo = "getPredecessorLink",
l1ol10 = "removeTasks",
l0lOl1 = "moveDownTask",
OOoO10 = "moveUpTask",
lolo01 = "downgradeTask",
l0l0OO = "upgradeTask",
l1Oo00 = "moveTasks",
oOlllo = "moveTask",
l0llol = "updateTasks",
o0O100 = "updateTask",
l0O1O1 = "clearTasks",
o1Oooo = "removeTask",
l1O011 = "addTasks",
Oo001l = "acceptTask",
ooo1oO = "addTask",
O1llo0 = "newTask",
OO1o0l = "getFinishDate",
OO0Olo = "getStartDate",
loo10o = "getViewFinishDate",
l01l1l = "getAncestorTasks",
oOo0O1 = "getAllChildTasks",
l1Ooll = "getRoot",
Oll1l0 = "getChildTasks",
O1l1O1 = "getParentTask",
O1Ol00 = "getLastTask",
O0Ol00 = "getFirstTask",
O0l010 = "getPrevTask",
Oo1loo = "getNextTask",
OOO1lo = "syncTasks",
OOo0oo = "_syncTasks2",
o00111 = "loadTasks",
Oo1Oo0 = "acceptChanges",
oOoO1o = "getChangedTasks",
Oo0o1O = "getTaskList",
loO1O1 = "getTaskTree",
o11ooo = "getRemovedTasks",
l11oOo = "newProject",
ollOo = "expandAll",
OOl0o0 = "collapseAll",
O00l11 = "toggle",
OOlOO0 = "expand",
O1lolO = "collapse",
loOo1o = "expandLevel",
lo1Ol0 = "collapseLevel",
lo1Ooo = "bubbleParent",
lllolO = "cascadeChild",
ll0Olo = "eachChild",
o0O1O1 = "findTasks",
oO1l1l = "getTaskByID",
o0ol1o = "getTaskAt",
O0OOoo = "getTask",
lO1oO0 = "createDefaultCalendars",
ooll0O = "getColumnAt",
ol1001 = "updateColumn",
oll0ol = "indexOfColumn",
lO101l = "getSelectedColumn",
oo0ol0 = "getViewEndColumn",
lol0oo = "getViewStartColumn",
ooO000 = "setAllowDragDrop",
oOl10l = "setShowDirty",
l01oOl = "setGanttViewWidth",
O1010o = "setTableViewWidth",
lo1OO0 = "setTableViewExpanded",
l0OooO = "setGanttViewExpanded",
loll1O = "setShowTableView",
O1O1O1 = "setShowGanttView",
lo0o11 = "setBaselineIndex",
O0llll = "onPageChanged",
O00ol1 = "update",
l0l11o = "getShowReloadButton",
lolo1l = "setShowReloadButton",
lOo010 = "setExpanded",
ool1Ol = "getMaskOnLoad",
OoOolO = "setMaskOnLoad",
loO0Oo = "getRefreshOnExpand",
oOO1lo = "setRefreshOnExpand",
l0oOOO = "getIFrameEl",
o1oloO = "getFooterEl",
lo0111 = "getBodyEl",
OlO11 = "getToolbarEl",
lollll = "getHeaderEl",
O1l010 = "setFooter",
OO10lo = "setToolbar",
ll0lo1 = "set_bodyParent",
olll1O = "setBody",
oo0loO = "getButton",
o000lO = "removeButton",
olol1o = "updateButton",
O1101o = "addButton",
olo0lO = "createButton",
llll11 = "getShowToolbar",
oo0l1l = "setShowToolbar",
O111l0 = "getShowCollapseButton",
oOooOO = "setShowCollapseButton",
ol000o = "getCloseAction",
ol1l11 = "setCloseAction",
o01olo = "getShowCloseButton",
O0l01O = "setShowCloseButton",
loOl01 = "getTitle",
llOoOo = "setTitle",
lO01oo = "getToolbarCls",
l0Ol01 = "setToolbarCls",
OOooO0 = "getHeaderCls",
O0l11o = "setHeaderCls",
ol01oO = "getToolbarStyle",
O1Oo0O = "setToolbarStyle",
Ol0lO0 = "getHeaderStyle",
OOO1ll = "setHeaderStyle",
l00lOO = "isAllowDrag",
Oll1lo = "getDropGroupName",
OloOo1 = "setDropGroupName",
O1Ol1o = "getDragGroupName",
loollo = "setDragGroupName",
l1Oool = "getAllowDrop",
oooOoO = "setAllowDrop",
ool0Ol = "_getDragText",
OOlO1l = "_getDragData",
O0Oo0O = "onDataLoad",
lol1lo = "onCollapse",
ol00l1 = "onBeforeCollapse",
O0ooO0 = "onExpand",
OO11ol = "onBeforeExpand",
o0oO0O = "onNodeMouseDown",
ool1l0 = "onCheckNode",
o00l0o = "onBeforeNodeCheck",
oooOo0 = "onNodeSelect",
lO1o0l = "onBeforeNodeSelect",
l1l1ol = "onNodeClick",
OloOlo = "blurNode",
l1O0Oo = "focusNode",
l00O0o = "_OnNodeMouseMove",
Ol0Oll = "_OnNodeMouseOut",
O0011O = "_OnNodeClick",
ll0OoO = "_OnNodeMouseDown",
olo1OO = "getLoadOnExpand",
O0llo0 = "setLoadOnExpand",
O010l0 = "getRemoveOnCollapse",
l1Oo10 = "setRemoveOnCollapse",
OoO110 = "getExpandOnNodeClick",
lOOlo1 = "setExpandOnNodeClick",
l00110 = "getExpandOnDblClick",
lO1Olo = "setExpandOnDblClick",
OOlOoo = "getFolderIcon",
llOl0o = "setFolderIcon",
loo00O = "getLeafIcon",
OlOl0l = "setLeafIcon",
O10lOo = "getShowArrow",
loloO0 = "setShowArrow",
llollo = "getNodesByValue",
loOoOo = "getCheckedNodes",
oOOOo1 = "uncheckAllNodes",
O1Oooo = "checkAllNodes",
l01l0O = "uncheckNodes",
l001l1 = "checkNodes",
O0o0l1 = "uncheckNode",
O0o0OO = "checkNode",
OOll10 = "_doCheckState",
o010Oo = "hasCheckedChildNode",
O1lOl1 = "_doAutoCheckParent",
olO00l = "doUpdateCheckedState",
oO000O = "collapsePath",
OlOllo = "toggleNode",
O0o1ll = "disableNode",
o01ll1 = "enableNode",
l0001l = "showNode",
o11o00 = "hideNode",
OOO1O1 = "findNodes",
o1llo1 = "_getNodeEl",
OoOo11 = "getNodeBox",
OoOO0l = "_getNodeByEvent",
Olo010 = "beginEdit",
l0oOo1 = "isEditingNode",
lo0oo0 = "moveNode",
lo0o0l = "moveNodes",
O10O10 = "addNode",
l10l11 = "addNodes",
l000Oo = "updateNode",
O0001O = "setNodeIconCls",
oll0O0 = "setNodeText",
ll10O0 = "removeNodes",
o000oo = "isInLastNode",
oO01oO = "isLastNode",
l1o0oo = "isEnabledNode",
OOoOOO = "isVisibleNode",
l0o1Oo = "isCheckedNode",
llo0oo = "isExpandedNode",
lo0Olo = "hasChildren",
O1l00o = "indexOfChildren",
OO1000 = "getAllChildNodes",
lO0ool = "_getViewChildNodes",
ol11oo = "_isInViewLastNode",
o0O1o0 = "_isViewLastNode",
lO0lo0 = "_isViewFirstNode",
OoOll0 = "getRootNode",
Oo011l = "getNodeIcon",
lo00o1 = "getShowExpandButtons",
oOlO10 = "setShowExpandButtons",
Ooll0O = "getAllowSelect",
o001OO = "setAllowSelect",
l1looO = "getAjaxOption",
Ol11ol = "setAjaxOption",
oO0100 = "loadNode",
O1O111 = "_clearTree",
lo100l = "getList",
o01OOO = "parseItems",
loOOo0 = "onItemSelect",
oO0oo0 = "_OnItemSelect",
l1lllO = "getHideOnClick",
lo0O0l = "setHideOnClick",
l01lo1 = "getSelectedItem",
lO1111 = "setSelectedItem",
o1OOl0 = "getAllowSelectItem",
OO1lO1 = "setAllowSelectItem",
oOlOO1 = "getGroupItems",
OOo1Oo = "removeItemAt",
l1o000 = "getItems",
oOO1ll = "setItems",
llOo0O = "hasShowItemMenu",
ll0olo = "showItemMenu",
oOl0l1 = "hideItems",
l11l1l = "isVertical",
l1olO0 = "getbyName",
lOO0Oo = "onActiveChanged",
o0lo11 = "onCloseClick",
O001lO = "onBeforeCloseClick",
OO01OO = "getTabByEvent",
O0lOo1 = "getShowBody",
l0l10o = "setShowBody",
OOllo0 = "getActiveTab",
OooOol = "activeTab",
l0OoO1 = "getTabIFrameEl",
o1Oolo = "getTabBodyEl",
lOo0Oo = "getTabEl",
l0O1lO = "getTab",
ooo011 = "setTabPosition",
lol0l0 = "setTabAlign",
o1lOlO = "_handleIFrameOverflow",
lOlO10 = "getTabRows",
oOlo11 = "reloadTab",
l00lOl = "loadTab",
O0oOlo = "_cancelLoadTabs",
loOl0O = "updateTab",
OoO0oo = "moveTab",
ol11ol = "removeTab",
l101o1 = "addTab",
lollol = "getTabs",
oOO0l0 = "setTabs",
O0ooOO = "setTabControls",
l11l00 = "getTitleField",
O0ll0O = "setTitleField",
looO1l = "getNameField",
l1Ol1l = "setNameField",
o0o0oO = "createTab";
Oo11l0 = function() {
    this.llO01O = {};
    this.uid = mini.newId(this.Oll1Oo);
    if (!this.id) this.id = this.uid;
    mini.reg(this)
};
Oo11l0[O0lloO] = {
    isControl: true,
    id: null,
    Oll1Oo: "mini-",
    OllooO: false,
    o011lo: true
};
O10l = Oo11l0[O0lloO];
O10l[oOllOo] = ooo10;
O10l[O0lO01] = O1lO1;
O10l[olO0o1] = o1ll0;
O10l[lO00l] = oo0Oo;
O10l[o1oo11] = oolO1;
O10l[O1oOo1] = l1lol;
O10l[lOO1lo] = ll01Ol;
O10l[ol0Ol1] = lo0O0;
lo0O01 = function() {
    lo0O01[oOOOoO][lllOo0][Ool00](this);
    this[lOlo11]();
    this.el.uid = this.uid;
    this[OOOol0]();
    if (this._clearBorder) this.el.style.borderWidth = "0";
    this[o00lO1](this.uiCls);
    this[lOOo10](this.width);
    this[lo0o00](this.height);
    this.el.style.display = this.visible ? this.O011: "none"
};
ol1O(lo0O01, Oo11l0, {
    jsName: null,
    width: "",
    height: "",
    visible: true,
    readOnly: false,
    enabled: true,
    tooltip: "",
    Oo0loo: "mini-readonly",
    OOloo: "mini-disabled",
    name: "",
    _clearBorder: true,
    O011: "",
    O110ol: true,
    allowAnim: true,
    OO0Oll: "mini-mask-loading",
    loadingMsg: "Loading...",
    contextMenu: null
});
O00ol = lo0O01[O0lloO];
O00ol[l1OllO] = O010O;
O00ol.O0OO0 = ollOl;
O00ol[o0Oll0] = O0100;
O00ol[o101l] = lO1oo;
O00ol[OOlo0o] = Olo1;
O00ol[lOOol0] = l1lO;
O00ol[l0l010] = OO1O1;
O00ol[oolO11] = O1oOl;
O00ol.lo1o1 = llOOO;
O00ol.l1llO = lo110o;
O00ol[loOooo] = loO1o;
O00ol[loO0o0] = l10lo;
O00ol[o0oOO0] = OOlOo;
O00ol[Oo1110] = Ol1o;
O00ol[o1o1oo] = OllO1;
O00ol.lOlOo1 = oOolOo;
O00ol[Oll01O] = lOooO;
O00ol[l0oo11] = lO000;
O00ol[llo101] = lOl01;
O00ol[OlOoo] = Ol1oOO;
O00ol[oOllOo] = O011o;
O00ol[o11o0O] = Oo1l1l;
O00ol[o10l10] = o000l;
O00ol[lo1Oll] = O01ll;
O00ol[lolo1] = oO1OoO;
O00ol[O11Ool] = O1o1;
O00ol[o0110o] = oo010;
O00ol[ll1001] = l1o1l;
O00ol[o10ll1] = O1olo0;
O00ol[O1Ol01] = l1l1o0;
O00ol[llllO0] = oO1o1;
O00ol[l0l10O] = ll10O;
O00ol[llol0o] = llolO1;
O00ol[Ol0oo1] = ooolOO;
O00ol[O001O1] = O0lOl0;
O00ol[l0l1O] = l0oo0;
O00ol[l0O0Oo] = O1O1o;
O00ol[OlOoo0] = OoO0;
O00ol[o1lOo0] = o1ll1;
O00ol[lllo10] = Oll0o;
O00ol.olO10 = l01O;
O00ol[O00oOl] = l1o00;
O00ol[o00lO1] = OOO1;
O00ol[ol0ooO] = oO0o1;
O00ol[olO0l0] = l1Ooo;
O00ol[llOoO1] = Oo011;
O00ol[ll1OoO] = ol1ol0;
O00ol[llo010] = Ool1;
O00ol[ooO001] = O0lOo;
O00ol[lOllOo] = Oo1oO;
O00ol[lOOoOO] = l0l1;
O00ol[lo0o00] = Ooo1O1;
O00ol[ll1OO1] = Ol11O;
O00ol[lOOo10] = OOOOlo;
O00ol[O0l10O] = OlOO;
O00ol[oo010O] = loo10;
O00ol[Oo1lOo] = OOo00o;
O00ol[O011l0] = oloooO;
O00ol[llO1oo] = loll1;
O00ol[OlolOl] = loOOO;
O00ol[lO0oOo] = llol;
O00ol[o1loo0] = ll1100;
O00ol[oO10o0] = Ol1l1;
O00ol[OO11ll] = lO1O;
O00ol[oll1l1] = loo1O;
O00ol[lO0oO1] = O11ol;
O00ol[ll0O1O] = l010;
O00ol[o1O0O0] = o0Ol;
O00ol[OOOol0] = l1o1O;
O00ol[lOlo11] = oOolo1;
mini._attrs = null;
mini.regHtmlAttr = function(_, $) {
    if (!_) return;
    if (!$) $ = "string";
    if (!mini._attrs) mini._attrs = [];
    mini._attrs.push([_, $])
};
__mini_setControls = function($, B, C) {
    B = B || this.Ooo1;
    C = C || this;
    if (!$) $ = [];
    if (!mini.isArray($)) $ = [$];
    for (var _ = 0, D = $.length; _ < D; _++) {
        var A = $[_];
        if (typeof A == "string") {
            if (A[looo1l]("#") == 0) A = l1Oo(A)
        } else if (mini.isElement(A));
        else {
            A = mini.getAndCreate(A);
            A = A.el
        }
        if (!A) continue;
        mini.append(B, A)
    }
    mini.parse(B);
    C[o10l10]();
    return C
};
mini.Container = function() {
    mini.Container[oOOOoO][lllOo0][Ool00](this);
    this.Ooo1 = this.el
};
ol1O(mini.Container, lo0O01, {
    setControls: __mini_setControls
});
OoO0ll = function() {
    OoO0ll[oOOOoO][lllOo0][Ool00](this)
};
ol1O(OoO0ll, lo0O01, {
    required: false,
    requiredErrorText: "This field is required.",
    lool1o: "mini-required",
    errorText: "",
    Ol0ll0: "mini-error",
    oOlol: "mini-invalid",
    errorMode: "icon",
    validateOnChanged: true,
    validateOnLeave: true,
    O0OO: true,
    errorIconEl: null
});
o10oO = OoO0ll[O0lloO];
o10oO[l1OllO] = o1oOO;
o10oO[l0Oo1o] = ollO1;
o10oO[o1l1oO] = o0101;
o10oO.Ol110 = Ol1o0;
o10oO.oooO = o01ll;
o10oO.o1olll = Ooo1l;
o10oO.loOo11 = l1oOl;
o10oO[O11OlO] = lOloO;
o10oO[o11o0o] = Oolo0;
o10oO[OO1oO1] = O00ll;
o10oO[O00olo] = Oo01l;
o10oO[loOoOl] = oOl11;
o10oO[l0l1lO] = O1l0l;
o10oO[o11010] = lOll0;
o10oO[lOOooO] = ol0ll;
o10oO[o1oo1O] = l1oll;
o10oO[ll1oo1] = l1O00;
o10oO[o1o1ol] = oOolo;
o10oO[oO000l] = l0olO;
o10oO[llO1l] = ll0oO;
o10oO[Ol0O10] = OO0o1;
o10oO[lOoloo] = o0oO1;
o10oO[lO0oOl] = ooO1o;
o10oO[lo0o1o] = O1OOO;
o10oO[ooo0o1] = OllOl;
o1oOOl = function() {
    this.data = [];
    this.Oo1ll = [];
    o1oOOl[oOOOoO][lllOo0][Ool00](this);
    this[lolo1]()
};
ol1O(o1oOOl, OoO0ll, {
    defaultValue: "",
    value: "",
    valueField: "id",
    textField: "text",
    delimiter: ",",
    data: null,
    url: "",
    l0ol0O: "mini-list-item",
    oo00O: "mini-list-item-hover",
    _l0110: "mini-list-item-selected",
    uiCls: "mini-list",
    name: "",
    l01ol: null,
    Oo0l1o: null,
    Oo1ll: [],
    multiSelect: false,
    oOOOO: true
});
o0OO1 = o1oOOl[O0lloO];
o0OO1[l1OllO] = OlOOl;
o0OO1[OOOl0o] = O1100;
o0OO1[Oo0l11] = O0ooo;
o0OO1[olO011] = o10lO;
o0OO1[o1lo1l] = O1olO;
o0OO1[lOo1lo] = lOOll;
o0OO1[o1OlO1] = l101O;
o0OO1[Ololoo] = lo00l;
o0OO1[o11O01] = o11ll;
o0OO1[oOo1l0] = o10l0;
o0OO1.oo1O0O = oO0lO;
o0OO1.o01O0O = Ooooo;
o0OO1.O1Ol1O = oo011;
o0OO1.l1O0oO = lo10o;
o0OO1.olO10o = l0lOo;
o0OO1.o00oO0 = o0lO1;
o0OO1.OloOO = lOlll;
o0OO1.O10O1l = oOoO0;
o0OO1.o0oOOo = O1O0O;
o0OO1.O001oO = oOoo0;
o0OO1.lloO = olO0O;
o0OO1.OO0lOo = l0o1l;
o0OO1.loOol = oO1l0;
o0OO1.OOo1 = Olo0O1;
o0OO1.O10OO0 = oOO1l;
o0OO1[OlOl00] = lOOOO;
o0OO1[O0Ooo1] = oOOoO;
o0OO1[Ollol1] = ooOOl;
o0OO1[oool00] = Oo1o11;
o0OO1[l110Oo] = l10Oo;
o0OO1[O011oO] = ol10o;
o0OO1[OlOlo1] = O0loO;
o0OO1[llllOo] = l0Ol1;
o0OO1[lOoOl] = o1loo;
o0OO1[O1O0oo] = l0Ol1s;
o0OO1[OoOllo] = ll0lo;
o0OO1[O1Olo0] = OO00O;
o0OO1[o101l1] = llolo;
o0OO1.oo00lo = l011o;
o0OO1[O0oolO] = oOolO;
o0OO1[Oo0lol] = OoO1l;
o0OO1[loo0oO] = OoO1ls;
o0OO1[oo1OOl] = o1ooO;
o0OO1[oOlo0O] = o1ooOs;
o0OO1[oo00oO] = llOo1;
o0OO1[Ooo0o0] = o01lo;
o0OO1.O0OOo = O100O;
o0OO1[Oo0111] = o0o1o;
o0OO1[o10oOo] = ollo1;
o0OO1[oll1lo] = l1OOo;
o0OO1[O1lloo] = O011O;
o0OO1[olO0Ol] = O0l0O;
o0OO1[lloO1] = l11O0;
o0OO1[lO0OOO] = lOl0l;
o0OO1[o0Oll0] = lOo1l;
o0OO1[o101l] = oolOo;
o0OO1.lOol01 = Oolo;
o0OO1[OOO10o] = lo011;
o0OO1[o0oO0l] = oo0lO;
o0OO1[OlOO1l] = loOll;
o0OO1[l1OlOo] = olOOo;
o0OO1[oo111O] = l11l0;
o0OO1[o01o1] = lO0lO;
o0OO1[o1oO0] = oll1l;
o0OO1[ooO0Ol] = o10o0;
o0OO1[looo1l] = olo1O;
o0OO1[lOoo10] = o100O;
o0OO1[o001ol] = O1101;
o0OO1[oOo1Ol] = o1o1l;
o0OO1[oOl1o1] = lOol0;
o0OO1[lO0oo] = Ollo0l;
o0OO1.O0001l = O10l0;
o0OO1.olo1o = llooo;
o0OO1[oOol11] = O1101El;
o0OO1[o1010O] = OoO1lCls;
o0OO1[ol00o] = o1ooOCls;
o0OO1.O0l1 = O1101ByEvent;
o0OO1[ll0O1O] = OolOO;
o0OO1[oOllOo] = l110l;
o0OO1[OOOol0] = O00O0;
o0OO1[lOlo11] = l001O;
o0OO1[ol0Ol1] = o0O1O;
mini._Layouts = {};
mini.layout = function($, _) {
    function A(C) {
        var D = mini.get(C);
        if (D) {
            if (D[o10l10]) if (!mini._Layouts[D.uid]) {
                mini._Layouts[D.uid] = D;
                if (_ !== false || D[oO10o0]() == false) D[o10l10](false);
                delete mini._Layouts[D.uid]
            }
        } else {
            var E = C.childNodes;
            if (E) for (var $ = 0, F = E.length; $ < F; $++) {
                var B = E[$];
                A(B)
            }
        }
    }
    if (!$) $ = document.body;
    A($)
};
mini.applyTo = function(_) {
    _ = l1Oo(_);
    if (!_) return this;
    if (mini.get(_)) throw new Error("not applyTo a mini control");
    var $ = this[l1OllO](_);
    delete $._applyTo;
    if (mini.isNull($[Ooll]) && !mini.isNull($.value)) $[Ooll] = $.value;
    var A = _.parentNode;
    if (A && this.el != _) A.replaceChild(this.el, _);
    this[ol0Ol1]($);
    this.O0OO0(_);
    return this
};
mini.O110oO = function(G) {
    var F = G.nodeName.toLowerCase();
    if (!F) return;
    var B = G.className;
    if (B) {
        var $ = mini.get(G);
        if (!$) {
            var H = B.split(" ");
            for (var E = 0, C = H.length; E < C; E++) {
                var A = H[E],
                I = mini.getClassByUICls(A);
                if (I) {
                    Oo11(G, A);
                    var D = new I();
                    mini.applyTo[Ool00](D, G);
                    G = D.el;
                    break
                }
            }
        }
    }
    if (F == "select" || ololo(G, "mini-menu") || ololo(G, "mini-datagrid") || ololo(G, "mini-treegrid") || ololo(G, "mini-tree") || ololo(G, "mini-button") || ololo(G, "mini-textbox") || ololo(G, "mini-buttonedit")) return;
    var J = mini[o01O00](G, true);
    for (E = 0, C = J.length; E < C; E++) {
        var _ = J[E];
        if (_.nodeType == 1) if (_.parentNode == G) mini.O110oO(_)
    }
};
mini._Removes = [];
mini.parse = function($) {
    if (typeof $ == "string") {
        var A = $;
        $ = l1Oo(A);
        if (!$) $ = document.body
    }
    if ($ && !mini.isElement($)) $ = $.el;
    if (!$) $ = document.body;
    var _ = lllo1l;
    if (isIE) lllo1l = false;
    mini.O110oO($);
    lllo1l = _;
    mini.layout($)
};
mini[oooo0l] = function(B, A, E) {
    for (var $ = 0, D = E.length; $ < D; $++) {
        var C = E[$],
        _ = mini.getAttr(B, C);
        if (_) A[C] = _
    }
};
mini[o100] = function(B, A, E) {
    for (var $ = 0, D = E.length; $ < D; $++) {
        var C = E[$],
        _ = mini.getAttr(B, C);
        if (_) A[C] = _ == "true" ? true: false
    }
};
mini[l000oo] = function(B, A, E) {
    for (var $ = 0, D = E.length; $ < D; $++) {
        var C = E[$],
        _ = parseInt(mini.getAttr(B, C));
        if (!isNaN(_)) A[C] = _
    }
};
mini.o1lO0l = function(N) {
    var G = [],
    O = mini[o01O00](N);
    for (var M = 0, H = O.length; M < H; M++) {
        var C = O[M],
        T = jQuery(C),
        D = {},
        J = null,
        K = null,
        _ = mini[o01O00](C);
        if (_) for (var $ = 0, P = _.length; $ < P; $++) {
            var B = _[$],
            A = jQuery(B).attr("property");
            if (!A) continue;
            A = A.toLowerCase();
            if (A == "columns") {
                D.columns = mini.o1lO0l(B);
                jQuery(B).remove()
            }
            if (A == "editor" || A == "filter") {
                var F = B.className,
                R = F.split(" ");
                for (var L = 0, S = R.length; L < S; L++) {
                    var E = R[L],
                    Q = mini.getClassByUICls(E);
                    if (Q) {
                        var I = new Q();
                        if (A == "filter") {
                            K = I[l1OllO](B);
                            K.type = I.type
                        } else {
                            J = I[l1OllO](B);
                            J.type = I.type
                        }
                        break
                    }
                }
                jQuery(B).remove()
            }
        }
        D.header = C.innerHTML;
        mini[oooo0l](C, D, ["name", "header", "field", "editor", "filter", "renderer", "width", "type", "renderer", "headerAlign", "align", "headerCls", "cellCls", "headerStyle", "cellStyle", "displayField", "dateFormat", "listFormat", "mapFormat", "trueValue", "falseValue", "dataType", "vtype", "currencyUnit", "summaryType", "summaryRenderer", "groupSummaryType", "groupSummaryRenderer", "defaultValue", "defaultText"]);
        mini[o100](C, D, ["visible", "readOnly", "allowSort", "allowResize", "allowMove", "allowDrag", "autoShowPopup", "unique", "autoEscape"]);
        if (J) D.editor = J;
        if (K) D[ol1l00] = K;
        if (D.dataType) D.dataType = D.dataType.toLowerCase();
        if (D[Ooll] === "true") D[Ooll] = true;
        if (D[Ooll] === "false") D[Ooll] = false;
        G.push(D)
    }
    return G
};
mini.oo10Oo = {};
mini[l1l0o1] = function($) {
    var _ = mini.oo10Oo[$.toLowerCase()];
    if (!_) return {};
    return _()
};
mini.IndexColumn = function($) {
    return mini.copyTo({
        width: 30,
        cellCls: "",
        align: "center",
        draggable: false,
        init: function($) {
            $[O1oOo1]("addrow", this.__OnIndexChanged, this);
            $[O1oOo1]("removerow", this.__OnIndexChanged, this);
            $[O1oOo1]("moverow", this.__OnIndexChanged, this);
            if ($.isTree) {
                $[O1oOo1]("loadnode", this.__OnIndexChanged, this);
                this._gridUID = $.uid;
                this[lo0lll] = "_id"
            }
        },
        getNumberId: function($) {
            return this._gridUID + "$number$" + $[this._rowIdField]
        },
        createNumber: function($, _) {
            if (mini.isNull($[lOoolO])) return _ + 1;
            else return ($[lOoolO] * $[l0O1O]) + _ + 1
        },
        renderer: function(A) {
            var $ = A.sender;
            if (this.draggable) {
                if (!A.cellStyle) A.cellStyle = "";
                A.cellStyle += ";cursor:move;"
            }
            var _ = "<div id=\"" + this.getNumberId(A.record) + "\">";
            if (mini.isNull($[lOoolO])) _ += A.rowIndex + 1;
            else _ += ($[lOoolO] * $[l0O1O]) + A.rowIndex + 1;
            _ += "</div>";
            return _
        },
        __OnIndexChanged: function(F) {
            var $ = F.sender,
            C = $[llOo1l]();
            for (var A = 0, D = C.length; A < D; A++) {
                var _ = C[A],
                E = this.getNumberId(_),
                B = document.getElementById(E);
                if (B) B.innerHTML = this.createNumber($, A)
            }
        }
    },
    $)
};
mini.oo10Oo["indexcolumn"] = mini.IndexColumn;
mini.CheckColumn = function($) {
    return mini.copyTo({
        width: 30,
        cellCls: "mini-checkcolumn",
        headerCls: "mini-checkcolumn",
        _multiRowSelect: true,
        header: function($) {
            var A = this.uid + "checkall",
            _ = "<input type=\"checkbox\" id=\"" + A + "\" />";
            if (this[o1lloO] == false) _ = "";
            return _
        },
        getCheckId: function($) {
            return this._gridUID + "$checkcolumn$" + $[this._rowIdField]
        },
        init: function($) {
            $[O1oOo1]("selectionchanged", this.OOl1Oo, this);
            $[O1oOo1]("HeaderCellClick", this.o1l100, this)
        },
        renderer: function(C) {
            var B = this.getCheckId(C.record),
            _ = C.sender[OoOllo] ? C.sender[OoOllo](C.record) : false,
            A = "checkbox",
            $ = C.sender;
            if ($[o1lloO] == false) A = "radio";
            return "<input type=\"" + A + "\" id=\"" + B + "\" " + (_ ? "checked": "") + " hidefocus style=\"outline:none;\" onclick=\"return false\"/>"
        },
        o1l100: function(B) {
            var $ = B.sender;
            if (B.column != this) return;
            var A = $.uid + "checkall",
            _ = document.getElementById(A);
            if (_) {
                if ($[o1lloO]) {
                    if (_.checked) $[l110Oo]();
                    else $[oool00]()
                } else {
                    $[oool00]();
                    if (_.checked) $[OlOlo1](0)
                }
                $[lOO1lo]("checkall")
            }
        },
        OOl1Oo: function(H) {
            var $ = H.sender,
            C = $[llOo1l]();
            for (var A = 0, E = C.length; A < E; A++) {
                var _ = C[A],
                G = $[OoOllo](_),
                F = $.uid + "$checkcolumn$" + _[$._rowIdField],
                B = document.getElementById(F);
                if (B) B.checked = G
            }
            var D = this;
            if (!this._timer) this._timer = setTimeout(function() {
                D[OOll10]($);
                D._timer = null
            },
            10)
        },
        _doCheckState: function($) {
            var B = $.uid + "checkall",
            _ = document.getElementById(B);
            if (_ && $[lO0Ol0]) {
                var A = $[lO0Ol0]();
                if (A == "has") {
                    _.indeterminate = true;
                    _.checked = true
                } else {
                    _.indeterminate = false;
                    _.checked = A
                }
            }
        }
    },
    $)
};
mini.oo10Oo["checkcolumn"] = mini.CheckColumn;
mini.ExpandColumn = function($) {
    return mini.copyTo({
        width: 30,
        cellCls: "",
        align: "center",
        draggable: false,
        cellStyle: "padding:0",
        renderer: function($) {
            return "<a class=\"mini-grid-ecIcon\" href=\"javascript:#\" onclick=\"return false\"></a>"
        },
        init: function($) {
            $[O1oOo1]("cellclick", this.l111, this)
        },
        l111: function(A) {
            var $ = A.sender;
            if (A.column == this && $[o1ll1O]) if (OO0O(A.htmlEvent.target, "mini-grid-ecIcon")) {
                var _ = $[o1ll1O](A.record);
                if ($.autoHideRowDetail) $[oOoOol]();
                if (_) $[l0l0Oo](A.record);
                else $[olOll](A.record)
            }
        }
    },
    $)
};
mini.oo10Oo["expandcolumn"] = mini.ExpandColumn;
lllO0oColumn = function($) {
    return mini.copyTo({
        _type: "checkboxcolumn",
        header: "#",
        headerAlign: "center",
        cellCls: "mini-checkcolumn",
        trueValue: true,
        falseValue: false,
        readOnly: false,
        getCheckId: function($) {
            return this._gridUID + "$checkbox$" + $[this._rowIdField]
        },
        renderer: function(B) {
            var A = this.getCheckId(B.record),
            _ = B.record[B.field] == this.trueValue ? true: false,
            $ = "checkbox";
            return "<input type=\"" + $ + "\" id=\"" + A + "\" " + (_ ? "checked": "") + " hidefocus style=\"outline:none;\" onclick=\"return false;\"/>"
        },
        init: function($) {
            this.grid = $;
            $[O1oOo1]("cellclick", 
            function(D) {
                if (D.column == this) {
                    if (this[O00O01]) return;
                    var B = this.getCheckId(D.record),
                    A = D.htmlEvent.target;
                    if (A.id == B) {
                        D.cancel = false;
                        D.value = D.record[D.field];
                        $[lOO1lo]("cellbeginedit", D);
                        if (D.cancel !== true) {
                            var C = mini._getMap(D.field, D.record),
                            _ = C == this.trueValue ? this.falseValue: this.trueValue;
                            if ($.O0O0) $.O0O0(D.record, D.column, _)
                        }
                    }
                }
            },
            this);
            var _ = parseInt(this.trueValue),
            A = parseInt(this.falseValue);
            if (!isNaN(_)) this.trueValue = _;
            if (!isNaN(A)) this.falseValue = A
        }
    },
    $)
};
mini.oo10Oo["checkboxcolumn"] = lllO0oColumn;
Oooll1Column = function($) {
    return mini.copyTo({
        renderer: function(M) {
            var _ = M.value ? String(M.value) : "",
            C = _.split(","),
            D = "id",
            J = "text",
            A = {},
            G = M.column.editor;
            if (G && G.type == "combobox") {
                var B = this._combobox;
                if (!B) {
                    if (mini.isControl(G)) B = G;
                    else B = mini.create(G);
                    this._combobox = B
                }
                D = B[olO0Ol]();
                J = B[oll1lo]();
                A = this._valueMaps;
                if (!A) {
                    A = {};
                    var K = B[OlOO1l]();
                    for (var H = 0, E = K.length; H < E; H++) {
                        var $ = K[H];
                        A[$[D]] = $
                    }
                    this._valueMaps = A
                }
            }
            var L = [];
            for (H = 0, E = C.length; H < E; H++) {
                var F = C[H],
                $ = A[F];
                if ($) {
                    var I = $[J] || "";
                    L.push(I)
                }
            }
            return L.join(",")
        }
    },
    $)
};
mini.oo10Oo["comboboxcolumn"] = Oooll1Column;
lO0l = function($) {
    this.owner = $;
    looo(this.owner.el, "mousedown", this.o0oOOo, this)
};
lO0l[O0lloO] = {
    o0oOOo: function(_) {
        if (ololo(_.target, "mini-grid-resizeGrid") && this.owner[l000l]) {
            var $ = this.OlO00();
            $.start(_)
        }
    },
    OlO00: function() {
        if (!this._resizeDragger) this._resizeDragger = new mini.Drag({
            capture: true,
            onStart: mini.createDelegate(this.oOoOO1, this),
            onMove: mini.createDelegate(this.Ol1ll, this),
            onStop: mini.createDelegate(this.ll0ll, this)
        });
        return this._resizeDragger
    },
    oOoOO1: function($) {
        this.proxy = mini.append(document.body, "<div class=\"mini-grid-resizeProxy\"></div>");
        this.proxy.style.cursor = "se-resize";
        this.elBox = lolloO(this.owner.el);
        ooo1(this.proxy, this.elBox)
    },
    Ol1ll: function(B) {
        var $ = this.owner,
        D = B.now[0] - B.init[0],
        _ = B.now[1] - B.init[1],
        A = this.elBox.width + D,
        C = this.elBox.height + _;
        if (A < $.minWidth) A = $.minWidth;
        if (C < $.minHeight) C = $.minHeight;
        if (A > $.maxWidth) A = $.maxWidth;
        if (C > $.maxHeight) C = $.maxHeight;
        mini.setSize(this.proxy, A, C)
    },
    ll0ll: function($, A) {
        if (!this.proxy) return;
        var _ = lolloO(this.proxy);
        jQuery(this.proxy).remove();
        this.proxy = null;
        this.elBox = null;
        if (A) {
            this.owner[lOOo10](_.width);
            this.owner[lo0o00](_.height);
            this.owner[lOO1lo]("resize")
        }
    }
};
mini._topWindow = null;
mini._getTopWindow = function() {
    if (mini._topWindow) return mini._topWindow;
    var $ = [];
    function _(A) {
        try {
            A["___try"] = 1;
            $.push(A)
        } catch(B) {}
        if (A.parent && A.parent != A) _(A.parent)
    }
    _(window);
    mini._topWindow = $[$.length - 1];
    return mini._topWindow
};
var __ps = mini.getParams();
if (__ps._winid) {
    try {
        window.Owner = mini._getTopWindow()[__ps._winid]
    } catch(ex) {}
}
mini._WindowID = "w" + Math.floor(Math.random() * 10000);
mini._getTopWindow()[mini._WindowID] = window;
mini.__IFrameCreateCount = 1;
mini.createIFrame = function(E, F) {
    var H = "__iframe_onload" + mini.__IFrameCreateCount++;
    window[H] = _;
    if (!E) E = "";
    var D = E.split("#");
    E = D[0];
    var C = "_t=" + Math.floor(Math.random() * 1000000);
    if (E[looo1l]("?") == -1) E += "?" + C;
    else E += "&" + C;
    if (D[1]) E = E + "#" + D[1];
    var G = "<iframe style=\"width:100%;height:100%;\" onload=\"" + H + "()\"  frameborder=\"0\"></iframe>",
    $ = document.createElement("div"),
    B = mini.append($, G),
    I = false;
    setTimeout(function() {
        if (B) {
            B.src = E;
            I = true
        }
    },
    5);
    var A = true;
    function _() {
        if (I == false) return;
        setTimeout(function() {
            if (F) F(B, A);
            A = false
        },
        1)
    }
    B._ondestroy = function() {
        window[H] = mini.emptyFn;
        B.src = "";
        try {
            B.contentWindow.document.write("");
            B.contentWindow.document.close()
        } catch($) {}
        B._ondestroy = null;
        B = null
    };
    return B
};
mini._doOpen = function(C) {
    if (typeof C == "string") C = {
        url: C
    };
    C = mini.copyTo({
        width: 700,
        height: 400,
        allowResize: true,
        allowModal: true,
        closeAction: "destroy",
        title: "",
        titleIcon: "",
        iconCls: "",
        iconStyle: "",
        bodyStyle: "padding:0",
        url: "",
        showCloseButton: true,
        showFooter: false
    },
    C);
    C[Oo11oO] = "destroy";
    var $ = C.onload;
    delete C.onload;
    var B = C.ondestroy;
    delete C.ondestroy;
    var _ = C.url;
    delete C.url;
    var A = new o1OOO0();
    A[ol0Ol1](C);
    A[o01o1](_, $, B);
    A[O1Ol01]();
    return A
};
mini.open = function(E) {
    if (!E) return;
    var C = E.url;
    if (!C) C = "";
    var B = C.split("#"),
    C = B[0],
    A = "_winid=" + mini._WindowID;
    if (C[looo1l]("?") == -1) C += "?" + A;
    else C += "&" + A;
    if (B[1]) C = C + "#" + B[1];
    E.url = C;
    E.Owner = window;
    var $ = [];
    function _(A) {
        if (A.mini) $.push(A);
        if (A.parent && A.parent != A) _(A.parent)
    }
    _(window);
    var D = $[$.length - 1];
    return D["mini"]._doOpen(E)
};
mini.openTop = mini.open;
mini[OlOO1l] = function(C, A, E, D, _) {
    var $ = mini[ooo1oo](C, A, E, D, _),
    B = mini.decode($);
    return B
};
mini[ooo1oo] = function(B, A, D, C, _) {
    var $ = null;
    jQuery.ajax({
        url: B,
        data: A,
        async: false,
        type: _ ? _: "get",
        cache: false,
        dataType: "text",
        success: function(A, _) {
            $ = A;
            if (D) D(A, _)
        },
        error: C
    });
    return $
};
if (!window.mini_RootPath) mini_RootPath = "/";
lOll = function(B) {
    var A = document.getElementsByTagName("script"),
    D = "";
    for (var $ = 0, E = A.length; $ < E; $++) {
        var C = A[$].src;
        if (C[looo1l](B) != -1) {
            var F = C.split(B);
            D = F[0];
            break
        }
    }
    var _ = location.href;
    _ = _.split("#")[0];
    _ = _.split("?")[0];
    F = _.split("/");
    F.length = F.length - 1;
    _ = F.join("/");
    if (D[looo1l]("http:") == -1 && D[looo1l]("file:") == -1) D = _ + "/" + D;
    return D
};
if (!window.mini_JSPath) mini_JSPath = lOll("miniui.js");
mini[O00ol1] = function(A, _) {
    if (typeof A == "string") A = {
        url: A
    };
    if (_) A.el = _;
    var $ = mini.loadText(A.url);
    mini.innerHTML(A.el, $);
    mini.parse(A.el)
};
mini.createSingle = function($) {
    if (typeof $ == "string") $ = mini.getClass($);
    if (typeof $ != "function") return;
    var _ = $.single;
    if (!_) _ = $.single = new $();
    return _
};
mini.createTopSingle = function($) {
    if (typeof $ != "function") return;
    var _ = $[O0lloO].type;
    if (top && top != window && top.mini && top.mini.getClass(_)) return top.mini.createSingle(_);
    else return mini.createSingle($)
};
mini.sortTypes = {
    "string": function($) {
        return String($).toUpperCase()
    },
    "date": function($) {
        if (!$) return 0;
        if (mini.isDate($)) return $[llo1l]();
        return mini.parseDate(String($))
    },
    "float": function(_) {
        var $ = parseFloat(String(_).replace(/,/g, ""));
        return isNaN($) ? 0: $
    },
    "int": function(_) {
        var $ = parseInt(String(_).replace(/,/g, ""), 10);
        return isNaN($) ? 0: $
    }
};
mini.lo1O = function(G, $, K, H) {
    var F = G.split(";");
    for (var E = 0, C = F.length; E < C; E++) {
        var G = F[E].trim(),
        J = G.split(":"),
        A = J[0],
        _ = J[1];
        if (_) _ = _.split(",");
        else _ = [];
        var D = mini.VTypes[A];
        if (D) {
            var I = D($, _);
            if (I !== true) {
                K[lO0oOl] = false;
                var B = J[0] + "ErrorText";
                K.errorText = H[B] || mini.VTypes[B] || "";
                K.errorText = String.format(K.errorText, _[0], _[1], _[2], _[3], _[4]);
                break
            }
        }
    }
};
mini.oO00l1 = function($, _) {
    if ($ && $[_]) return $[_];
    else return mini.VTypes[_]
};
mini.VTypes = {
    uniqueErrorText: "This field is unique.",
    requiredErrorText: "This field is required.",
    emailErrorText: "Please enter a valid email address.",
    urlErrorText: "Please enter a valid URL.",
    floatErrorText: "Please enter a valid number.",
    intErrorText: "Please enter only digits",
    dateErrorText: "Please enter a valid date. Date format is {0}",
    maxLengthErrorText: "Please enter no more than {0} characters.",
    minLengthErrorText: "Please enter at least {0} characters.",
    maxErrorText: "Please enter a value less than or equal to {0}.",
    minErrorText: "Please enter a value greater than or equal to {0}.",
    rangeLengthErrorText: "Please enter a value between {0} and {1} characters long.",
    rangeCharErrorText: "Please enter a value between {0} and {1} characters long.",
    rangeErrorText: "Please enter a value between {0} and {1}.",
    required: function(_, $) {
        if (mini.isNull(_) || _ === "") return false;
        return true
    },
    email: function(_, $) {
        if (mini.isNull(_) || _ === "") return true;
        if (_.search(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/) != -1) return true;
        else return false
    },
    url: function(A, $) {
        if (mini.isNull(A) || A === "") return true;
        function _(_) {
            _ = _.toLowerCase();
            var $ = "^((https|http|ftp|rtsp|mms)?://)" + "?(([0-9a-z_!~*'().&=+$%-]+:)?[0-9a-z_!~*'().&=+$%-]+@)?" + "(([0-9]{1,3}.){3}[0-9]{1,3}" + "|" + "([0-9a-z_!~*'()-]+.)*" + "([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]." + "[a-z]{2,6})" + "(:[0-9]{1,4})?" + "((/?)|" + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$",
            A = new RegExp($);
            if (A.test(_)) return (true);
            else return (false)
        }
        return _(A)
    },
    "int": function(A, _) {
        if (mini.isNull(A) || A === "") return true;
        function $(_) {
            var $ = String(_);
            return $.length > 0 && !(/[^0-9]/).test($)
        }
        return $(A)
    },
    "float": function(A, _) {
        if (mini.isNull(A) || A === "") return true;
        function $(_) {
            var $ = String(_);
            return $.length > 0 && !(/[^0-9.]/).test($)
        }
        return $(A)
    },
    "date": function(B, _) {
        if (mini.isNull(B) || B === "") return true;
        if (!B) return false;
        var $ = null,
        A = _[0];
        if (A) {
            $ = mini.parseDate(B, A);
            if ($ && $.getFullYear) if (mini.formatDate($, A) == B) return true
        } else {
            $ = mini.parseDate(B, "yyyy-MM-dd");
            if (!$) $ = mini.parseDate(B, "yyyy/MM/dd");
            if (!$) $ = mini.parseDate(B, "MM/dd/yyyy");
            if ($ && $.getFullYear) return true
        }
        return false
    },
    maxLength: function(A, $) {
        if (mini.isNull(A) || A === "") return true;
        var _ = parseInt($);
        if (!A || isNaN(_)) return true;
        if (A.length <= _) return true;
        else return false
    },
    minLength: function(A, $) {
        if (mini.isNull(A) || A === "") return true;
        var _ = parseInt($);
        if (isNaN(_)) return true;
        if (A.length >= _) return true;
        else return false
    },
    rangeLength: function(B, _) {
        if (mini.isNull(B) || B === "") return true;
        if (!B) return false;
        var $ = parseFloat(_[0]),
        A = parseFloat(_[1]);
        if (isNaN($) || isNaN(A)) return true;
        if ($ <= B.length && B.length <= A) return true;
        return false
    },
    rangeChar: function(G, B) {
        if (mini.isNull(G) || G === "") return true;
        var A = parseFloat(B[0]),
        E = parseFloat(B[1]);
        if (isNaN(A) || isNaN(E)) return true;
        function C(_) {
            var $ = new RegExp("^[\u4e00-\u9fa5]+$");
            if ($.test(_)) return true;
            return false
        }
        var $ = 0,
        F = String(G).split("");
        for (var _ = 0, D = F.length; _ < D; _++) if (C(F[_])) $ += 2;
        else $ += 1;
        if (A <= $ && $ <= E) return true;
        return false
    },
    range: function(B, _) {
        if (mini.isNull(B) || B === "") return true;
        B = parseFloat(B);
        if (isNaN(B)) return false;
        var $ = parseFloat(_[0]),
        A = parseFloat(_[1]);
        if (isNaN($) || isNaN(A)) return true;
        if ($ <= B && B <= A) return true;
        return false
    }
};
mini.summaryTypes = {
    "count": function($) {
        if (!$) $ = [];
        return $.length
    },
    "max": function(B, C) {
        if (!B) B = [];
        var E = null;
        for (var _ = 0, D = B.length; _ < D; _++) {
            var $ = B[_],
            A = parseFloat($[C]);
            if (A === null || A === undefined || isNaN(A)) continue;
            if (E == null || E < A) E = A
        }
        return E
    },
    "min": function(C, D) {
        if (!C) C = [];
        var B = null;
        for (var _ = 0, E = C.length; _ < E; _++) {
            var $ = C[_],
            A = parseFloat($[D]);
            if (A === null || A === undefined || isNaN(A)) continue;
            if (B == null || B > A) B = A
        }
        return B
    },
    "avg": function(C, D) {
        if (!C) C = [];
        var B = 0;
        for (var _ = 0, E = C.length; _ < E; _++) {
            var $ = C[_],
            A = parseFloat($[D]);
            if (A === null || A === undefined || isNaN(A)) continue;
            B += A
        }
        var F = B / 7;
        return F
    },
    "sum": function(C, D) {
        if (!C) C = [];
        var B = 0;
        for (var _ = 0, E = C.length; _ < E; _++) {
            var $ = C[_],
            A = parseFloat($[D]);
            if (A === null || A === undefined || isNaN(A)) continue;
            B += A
        }
        return B
    }
};
mini.formatCurrency = function($, A) {
    if ($ === null || $ === undefined) null == "";
    $ = String($).replace(/\$|\,/g, "");
    if (isNaN($)) $ = "0";
    sign = ($ == ($ = Math.abs($)));
    $ = Math.floor($ * 100 + 0.50000000001);
    cents = $ % 100;
    $ = Math.floor($ / 100).toString();
    if (cents < 10) cents = "0" + cents;
    for (var _ = 0; _ < Math.floor(($.length - (1 + _)) / 3); _++) $ = $.substring(0, $.length - (4 * _ + 3)) + "," + $.substring($.length - (4 * _ + 3));
    A = A || "";
    return (((sign) ? "": "-") + A + $ + "." + cents)
};
mini.emptyFn = function() {};
mini.Drag = function($) {
    mini.copyTo(this, $)
};
mini.Drag[O0lloO] = {
    onStart: mini.emptyFn,
    onMove: mini.emptyFn,
    onStop: mini.emptyFn,
    capture: false,
    fps: 20,
    event: null,
    delay: 80,
    start: function(_) {
        _.preventDefault();
        if (_) this.event = _;
        this.now = this.init = [this.event.pageX, this.event.pageY];
        var $ = document;
        looo($, "mousemove", this.move, this);
        looo($, "mouseup", this.stop, this);
        looo($, "contextmenu", this.contextmenu, this);
        if (this.context) looo(this.context, "contextmenu", this.contextmenu, this);
        this.trigger = _.target;
        mini.selectable(this.trigger, false);
        mini.selectable($.body, false);
        if (this.capture) if (isIE) this.trigger.setCapture(true);
        else if (document.captureEvents) document.captureEvents(Event.MOUSEMOVE | Event.MOUSEUP | Event.MOUSEDOWN);
        this.started = false;
        this.startTime = new Date()
    },
    contextmenu: function($) {
        if (this.context) Ol100(this.context, "contextmenu", this.contextmenu, this);
        Ol100(document, "contextmenu", this.contextmenu, this);
        $.preventDefault();
        $.stopPropagation()
    },
    move: function(_) {
        if (this.delay) if (new Date() - this.startTime < this.delay) return;
        if (!this.started) {
            this.started = true;
            this.onStart(this)
        }
        var $ = this;
        if (!this.timer) this.timer = setTimeout(function() {
            $.now = [_.pageX, _.pageY];
            $.event = _;
            $.onMove($);
            $.timer = null
        },
        5)
    },
    stop: function(B) {
        this.now = [B.pageX, B.pageY];
        this.event = B;
        if (this.timer) {
            clearTimeout(this.timer);
            this.timer = null
        }
        var A = document;
        mini.selectable(this.trigger, true);
        mini.selectable(A.body, true);
        if (this.capture) if (isIE) this.trigger.releaseCapture();
        else if (document.captureEvents) document.releaseEvents(Event.MOUSEMOVE | Event.MOUSEUP | Event.MOUSEDOWN);
        var _ = mini.MouseButton.Right != B.button;
        if (_ == false) B.preventDefault();
        Ol100(A, "mousemove", this.move, this);
        Ol100(A, "mouseup", this.stop, this);
        var $ = this;
        setTimeout(function() {
            Ol100(document, "contextmenu", $.contextmenu, $);
            if ($.context) Ol100($.context, "contextmenu", $.contextmenu, $)
        },
        1);
        if (this.started) this.onStop(this, _)
    }
};
mini.JSON = new(function() {
    var sb = [],
    useHasOwn = !!{}.hasOwnProperty,
    replaceString = function($, A) {
        var _ = m[A];
        if (_) return _;
        _ = A.charCodeAt();
        return "\\u00" + Math.floor(_ / 16).toString(16) + (_ % 16).toString(16)
    },
    doEncode = function($) {
        if ($ === null) {
            sb[sb.length] = "null";
            return
        }
        var A = typeof $;
        if (A == "undefined") {
            sb[sb.length] = "null";
            return
        } else if ($.push) {
            sb[sb.length] = "[";
            var D,
            _,
            C = $.length,
            E;
            for (_ = 0; _ < C; _ += 1) {
                E = $[_];
                A = typeof E;
                if (A == "undefined" || A == "function" || A == "unknown");
                else {
                    if (D) sb[sb.length] = ",";
                    doEncode(E);
                    D = true
                }
            }
            sb[sb.length] = "]";
            return
        } else if ($.getFullYear) {
            var B;
            sb[sb.length] = "\"";
            sb[sb.length] = $.getFullYear();
            sb[sb.length] = "-";
            B = $.getMonth() + 1;
            sb[sb.length] = B < 10 ? "0" + B: B;
            sb[sb.length] = "-";
            B = $.getDate();
            sb[sb.length] = B < 10 ? "0" + B: B;
            sb[sb.length] = "T";
            B = $.getHours();
            sb[sb.length] = B < 10 ? "0" + B: B;
            sb[sb.length] = ":";
            B = $.getMinutes();
            sb[sb.length] = B < 10 ? "0" + B: B;
            sb[sb.length] = ":";
            B = $.getSeconds();
            sb[sb.length] = B < 10 ? "0" + B: B;
            sb[sb.length] = "\"";
            return
        } else if (A == "string") {
            if (strReg1.test($)) {
                sb[sb.length] = "\"";
                sb[sb.length] = $.replace(strReg2, replaceString);
                sb[sb.length] = "\"";
                return
            }
            sb[sb.length] = "\"" + $ + "\"";
            return
        } else if (A == "number") {
            sb[sb.length] = $;
            return
        } else if (A == "boolean") {
            sb[sb.length] = String($);
            return
        } else {
            sb[sb.length] = "{";
            D,
            _,
            E;
            for (_ in $) if (!useHasOwn || ($.hasOwnProperty && $.hasOwnProperty(_))) {
                E = $[_];
                A = typeof E;
                if (A == "undefined" || A == "function" || A == "unknown");
                else {
                    if (D) sb[sb.length] = ",";
                    doEncode(_);
                    sb[sb.length] = ":";
                    doEncode(E);
                    D = true
                }
            }
            sb[sb.length] = "}";
            return
        }
    },
    m = {
        "\b": "\\b",
        "\t": "\\t",
        "\n": "\\n",
        "\f": "\\f",
        "\r": "\\r",
        "\"": "\\\"",
        "\\": "\\\\"
    },
    strReg1 = /["\\\x00-\x1f]/,
    strReg2 = /([\x00-\x1f\\"])/g;
    this.encode = function() {
        var $;
        return function($, _) {
            sb = [];
            doEncode($);
            return sb.join("")
        }
    } ();
    this.decode = function() {
        var re = /[\"\'](\d{4})-(\d{2})-(\d{2})[T ](\d{2}):(\d{2}):(\d{2})[\"\']/g;
        return function(json) {
            if (json === "" || json === null || json === undefined) return json;
            if (typeof json == "object") json = this.encode(json);
            json = json.replace(re, "new Date($1,$2-1,$3,$4,$5,$6)");
            json = json.replace(__js_dateRegEx, "$1new Date($2)");
            json = json.replace(__js_dateRegEx2, "new Date($1)");
            var s = eval("(" + json + ")");
            return s
        }
    } ()
})();
__js_dateRegEx = new RegExp("(^|[^\\\\])\\\"\\\\/Date\\((-?[0-9]+)(?:[a-zA-Z]|(?:\\+|-)[0-9]{4})?\\)\\\\/\\\"", "g");
__js_dateRegEx2 = new RegExp("[\"']/Date\\(([0-9]+)\\)/[\"']", "g");
mini.encode = mini.JSON.encode;
mini.decode = mini.JSON.decode;
mini.clone = function($) {
    if ($ === null || $ === undefined) return $;
    var B = mini.encode($),
    _ = mini.decode(B);
    function A(B) {
        for (var _ = 0, D = B.length; _ < D; _++) {
            var $ = B[_];
            delete $._state;
            delete $._id;
            delete $._pid;
            for (var C in $) {
                var E = $[C];
                if (E instanceof Array) A(E)
            }
        }
    }
    A(_ instanceof Array ? _: [_]);
    return _
};
var DAY_MS = 86400000,
HOUR_MS = 3600000,
MINUTE_MS = 60000;
mini.copyTo(mini, {
    clearTime: function($) {
        if (!$) return null;
        return new Date($.getFullYear(), $.getMonth(), $.getDate())
    },
    maxTime: function($) {
        if (!$) return null;
        return new Date($.getFullYear(), $.getMonth(), $.getDate(), 23, 59, 59)
    },
    cloneDate: function($) {
        if (!$) return null;
        return new Date($[llo1l]())
    },
    addDate: function(A, $, _) {
        if (!_) _ = "D";
        A = new Date(A[llo1l]());
        switch (_.toUpperCase()) {
        case "Y":
            A.setFullYear(A.getFullYear() + $);
            break;
        case "MO":
            A.setMonth(A.getMonth() + $);
            break;
        case "D":
            A.setDate(A.getDate() + $);
            break;
        case "H":
            A.setHours(A.getHours() + $);
            break;
        case "M":
            A.setMinutes(A.getMinutes() + $);
            break;
        case "S":
            A.setSeconds(A.getSeconds() + $);
            break;
        case "MS":
            A.setMilliseconds(A.getMilliseconds() + $);
            break
        }
        return A
    },
    getWeek: function(D, $, _) {
        $ += 1;
        var E = Math.floor((14 - ($)) / 12),
        G = D + 4800 - E,
        A = ($) + (12 * E) - 3,
        C = _ + Math.floor(((153 * A) + 2) / 5) + (365 * G) + Math.floor(G / 4) - Math.floor(G / 100) + Math.floor(G / 400) - 32045,
        F = (C + 31741 - (C % 7)) % 146097 % 36524 % 1461,
        H = Math.floor(F / 1460),
        B = ((F - H) % 365) + H;
        NumberOfWeek = Math.floor(B / 7) + 1;
        return NumberOfWeek
    },
    getWeekStartDate: function(C, B) {
        if (!B) B = 0;
        if (B > 6 || B < 0) throw new Error("out of weekday");
        var A = C.getDay(),
        _ = B - A;
        if (A < B) _ -= 7;
        var $ = new Date(C.getFullYear(), C.getMonth(), C.getDate() + _);
        return $
    },
    getShortWeek: function(_) {
        var $ = this.dateInfo.daysShort;
        return $[_]
    },
    getLongWeek: function(_) {
        var $ = this.dateInfo.daysLong;
        return $[_]
    },
    getShortMonth: function($) {
        var _ = this.dateInfo.monthsShort;
        return _[$]
    },
    getLongMonth: function($) {
        var _ = this.dateInfo.monthsLong;
        return _[$]
    },
    dateInfo: {
        monthsLong: ["January", "Febraury", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
        monthsShort: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
        daysLong: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
        daysShort: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
        quarterLong: ["Q1", "Q2", "Q3", "Q4"],
        quarterShort: ["Q1", "Q2", "Q3", "Q4"],
        halfYearLong: ["first half", "second half"],
        patterns: {
            "d": "M/d/yyyy",
            "D": "dddd,MMMM dd,yyyy",
            "f": "dddd,MMMM dd,yyyy H:mm tt",
            "F": "dddd,MMMM dd,yyyy H:mm:ss tt",
            "g": "M/d/yyyy H:mm tt",
            "G": "M/d/yyyy H:mm:ss tt",
            "m": "MMMM dd",
            "o": "yyyy-MM-ddTHH:mm:ss.fff",
            "s": "yyyy-MM-ddTHH:mm:ss",
            "t": "H:mm tt",
            "T": "H:mm:ss tt",
            "U": "dddd,MMMM dd,yyyy HH:mm:ss tt",
            "y": "MMM,yyyy"
        },
        tt: {
            "AM": "AM",
            "PM": "PM"
        },
        ten: {
            "Early": "Early",
            "Mid": "Mid",
            "Late": "Late"
        },
        today: "Today",
        clockType: 24
    }
});
Date[O0lloO].getHalfYear = function() {
    if (!this.getMonth) return null;
    var $ = this.getMonth();
    if ($ < 6) return 0;
    return 1
};
Date[O0lloO].getQuarter = function() {
    if (!this.getMonth) return null;
    var $ = this.getMonth();
    if ($ < 3) return 0;
    if ($ < 6) return 1;
    if ($ < 9) return 2;
    return 3
};
mini.formatDate = function(C, O, F) {
    if (!C || !C.getFullYear || isNaN(C)) return "";
    var G = C.toString(),
    B = mini.dateInfo;
    if (!B) B = mini.dateInfo;
    if (typeof(B) !== "undefined") {
        var M = typeof(B.patterns[O]) !== "undefined" ? B.patterns[O] : O,
        J = C.getFullYear(),
        $ = C.getMonth(),
        _ = C.getDate();
        if (O == "yyyy-MM-dd") {
            $ = $ + 1 < 10 ? "0" + ($ + 1) : $ + 1;
            _ = _ < 10 ? "0" + _: _;
            return J + "-" + $ + "-" + _
        }
        if (O == "MM/dd/yyyy") {
            $ = $ + 1 < 10 ? "0" + ($ + 1) : $ + 1;
            _ = _ < 10 ? "0" + _: _;
            return $ + "/" + _ + "/" + J
        }
        G = M.replace(/yyyy/g, J);
        G = G.replace(/yy/g, (J + "").substring(2));
        var L = C.getHalfYear();
        G = G.replace(/hy/g, B.halfYearLong[L]);
        var I = C.getQuarter();
        G = G.replace(/Q/g, B.quarterLong[I]);
        G = G.replace(/q/g, B.quarterShort[I]);
        G = G.replace(/MMMM/g, B.monthsLong[$].escapeDateTimeTokens());
        G = G.replace(/MMM/g, B.monthsShort[$].escapeDateTimeTokens());
        G = G.replace(/MM/g, $ + 1 < 10 ? "0" + ($ + 1) : $ + 1);
        G = G.replace(/(\\)?M/g, 
        function(A, _) {
            return _ ? A: $ + 1
        });
        var N = C.getDay();
        G = G.replace(/dddd/g, B.daysLong[N].escapeDateTimeTokens());
        G = G.replace(/ddd/g, B.daysShort[N].escapeDateTimeTokens());
        G = G.replace(/dd/g, _ < 10 ? "0" + _: _);
        G = G.replace(/(\\)?d/g, 
        function(A, $) {
            return $ ? A: _
        });
        var H = C.getHours(),
        A = H > 12 ? H - 12: H;
        if (B.clockType == 12) if (H > 12) H -= 12;
        G = G.replace(/HH/g, H < 10 ? "0" + H: H);
        G = G.replace(/(\\)?H/g, 
        function(_, $) {
            return $ ? _: H
        });
        G = G.replace(/hh/g, A < 10 ? "0" + A: A);
        G = G.replace(/(\\)?h/g, 
        function(_, $) {
            return $ ? _: A
        });
        var D = C.getMinutes();
        G = G.replace(/mm/g, D < 10 ? "0" + D: D);
        G = G.replace(/(\\)?m/g, 
        function(_, $) {
            return $ ? _: D
        });
        var K = C.getSeconds();
        G = G.replace(/ss/g, K < 10 ? "0" + K: K);
        G = G.replace(/(\\)?s/g, 
        function(_, $) {
            return $ ? _: K
        });
        G = G.replace(/fff/g, C.getMilliseconds());
        G = G.replace(/tt/g, C.getHours() > 12 || C.getHours() == 0 ? B.tt["PM"] : B.tt["AM"]);
        var C = C.getDate(),
        E = "";
        if (C <= 10) E = B.ten["Early"];
        else if (C <= 20) E = B.ten["Mid"];
        else E = B.ten["Late"];
        G = G.replace(/ten/g, E)
    }
    return G.replace(/\\/g, "")
};
String[O0lloO].escapeDateTimeTokens = function() {
    return this.replace(/([dMyHmsft])/g, "\\$1")
};
mini.fixDate = function($, _) {
    if ( + $) while ($.getDate() != _.getDate()) $[o0lolO]( + $ + ($ < _ ? 1: -1) * HOUR_MS)
};
mini.parseDate = function(s, ignoreTimezone) {
    try {
        var d = eval(s);
        if (d && d.getFullYear) return d
    } catch(ex) {}
    if (typeof s == "object") return isNaN(s) ? null: s;
    if (typeof s == "number") {
        d = new Date(s * 1000);
        if (d[llo1l]() != s) return null;
        return isNaN(d) ? null: d
    }
    if (typeof s == "string") {
        if (s.match(/^\d+(\.\d+)?$/)) {
            d = new Date(parseFloat(s) * 1000);
            if (d[llo1l]() != s) return null;
            else return d
        }
        if (ignoreTimezone === undefined) ignoreTimezone = true;
        d = mini.parseISO8601(s, ignoreTimezone) || (s ? new Date(s) : null);
        return isNaN(d) ? null: d
    }
    return null
};
mini.parseISO8601 = function(D, $) {
    var _ = D.match(/^([0-9]{4})([-\/]([0-9]{1,2})([-\/]([0-9]{1,2})([T ]([0-9]{1,2}):([0-9]{1,2})(:([0-9]{1,2})(\.([0-9]+))?)?(Z|(([-+])([0-9]{2})(:?([0-9]{2}))?))?)?)?)?$/);
    if (!_) {
        _ = D.match(/^([0-9]{4})[-\/]([0-9]{2})[-\/]([0-9]{2})[T ]([0-9]{1,2})/);
        if (_) {
            var A = new Date(_[1], _[2] - 1, _[3], _[4]);
            return A
        }
        _ = D.match(/^([0-9]{4}).([0-9]*).([0-9]*)/);
        if (_) {
            A = new Date(_[1], _[2] - 1, _[3]);
            return A
        }
        _ = D.match(/^([0-9]{2})-([0-9]{2})-([0-9]{4})$/);
        if (!_) return null;
        else {
            A = new Date(_[3], _[1] - 1, _[2]);
            return A
        }
    }
    A = new Date(_[1], 0, 1);
    if ($ || !_[14]) {
        var C = new Date(_[1], 0, 1, 9, 0);
        if (_[3]) {
            A.setMonth(_[3] - 1);
            C.setMonth(_[3] - 1)
        }
        if (_[5]) {
            A.setDate(_[5]);
            C.setDate(_[5])
        }
        mini.fixDate(A, C);
        if (_[7]) A.setHours(_[7]);
        if (_[8]) A.setMinutes(_[8]);
        if (_[10]) A.setSeconds(_[10]);
        if (_[12]) A.setMilliseconds(Number("0." + _[12]) * 1000);
        mini.fixDate(A, C)
    } else {
        A.setUTCFullYear(_[1], _[3] ? _[3] - 1: 0, _[5] || 1);
        A.setUTCHours(_[7] || 0, _[8] || 0, _[10] || 0, _[12] ? Number("0." + _[12]) * 1000: 0);
        var B = Number(_[16]) * 60 + (_[18] ? Number(_[18]) : 0);
        B *= _[15] == "-" ? 1: -1;
        A = new Date( + A + (B * 60 * 1000))
    }
    return A
};
mini.parseTime = function(E, F) {
    if (!E) return null;
    var B = parseInt(E);
    if (B == E && F) {
        $ = new Date(0);
        if (F[0] == "H") $.setHours(B);
        else if (F[0] == "m") $.setMinutes(B);
        else if (F[0] == "s") $.setSeconds(B);
        return $
    }
    var $ = mini.parseDate(E);
    if (!$) {
        var D = E.split(":"),
        _ = parseInt(parseFloat(D[0])),
        C = parseInt(parseFloat(D[1])),
        A = parseInt(parseFloat(D[2]));
        if (!isNaN(_) && !isNaN(C) && !isNaN(A)) {
            $ = new Date(0);
            $.setHours(_);
            $.setMinutes(C);
            $.setSeconds(A)
        }
        if (!isNaN(_) && (F == "H" || F == "HH")) {
            $ = new Date(0);
            $.setHours(_)
        } else if (!isNaN(_) && !isNaN(C) && (F == "H:mm" || F == "HH:mm")) {
            $ = new Date(0);
            $.setHours(_);
            $.setMinutes(C)
        } else if (!isNaN(_) && !isNaN(C) && F == "mm:ss") {
            $ = new Date(0);
            $.setMinutes(_);
            $.setSeconds(C)
        }
    }
    return $
};
mini.dateInfo = {
    monthsLong: ["\u4e00\u6708", "\u4e8c\u6708", "\u4e09\u6708", "\u56db\u6708", "\u4e94\u6708", "\u516d\u6708", "\u4e03\u6708", "\u516b\u6708", "\u4e5d\u6708", "\u5341\u6708", "\u5341\u4e00\u6708", "\u5341\u4e8c\u6708"],
    monthsShort: ["1\u6708", "2\u6708", "3\u6708", "4\u6708", "5\u6708", "6\u6708", "7\u6708", "8\u6708", "9\u6708", "10\u6708", "11\u6708", "12\u6708"],
    daysLong: ["\u661f\u671f\u65e5", "\u661f\u671f\u4e00", "\u661f\u671f\u4e8c", "\u661f\u671f\u4e09", "\u661f\u671f\u56db", "\u661f\u671f\u4e94", "\u661f\u671f\u516d"],
    daysShort: ["\u65e5", "\u4e00", "\u4e8c", "\u4e09", "\u56db", "\u4e94", "\u516d"],
    quarterLong: ["\u4e00\u5b63\u5ea6", "\u4e8c\u5b63\u5ea6", "\u4e09\u5b63\u5ea6", "\u56db\u5b63\u5ea6"],
    quarterShort: ["Q1", "Q2", "Q2", "Q4"],
    halfYearLong: ["\u4e0a\u534a\u5e74", "\u4e0b\u534a\u5e74"],
    patterns: {
        "d": "yyyy-M-d",
        "D": "yyyy\u5e74M\u6708d\u65e5",
        "f": "yyyy\u5e74M\u6708d\u65e5 H:mm",
        "F": "yyyy\u5e74M\u6708d\u65e5 H:mm:ss",
        "g": "yyyy-M-d H:mm",
        "G": "yyyy-M-d H:mm:ss",
        "m": "MMMd\u65e5",
        "o": "yyyy-MM-ddTHH:mm:ss.fff",
        "s": "yyyy-MM-ddTHH:mm:ss",
        "t": "H:mm",
        "T": "H:mm:ss",
        "U": "yyyy\u5e74M\u6708d\u65e5 HH:mm:ss",
        "y": "yyyy\u5e74MM\u6708"
    },
    tt: {
        "AM": "\u4e0a\u5348",
        "PM": "\u4e0b\u5348"
    },
    ten: {
        "Early": "\u4e0a\u65ec",
        "Mid": "\u4e2d\u65ec",
        "Late": "\u4e0b\u65ec"
    },
    today: "\u4eca\u5929",
    clockType: 24
};
l1Oo = function($) {
    if (typeof $ == "string") {
        if ($.charAt(0) == "#") $ = $.substr(1);
        return document.getElementById($)
    } else return $
};
ololo = function($, _) {
    $ = l1Oo($);
    if (!$) return;
    if (!$.className) return false;
    var A = String($.className).split(" ");
    return A[looo1l](_) != -1
};
lloo10 = function($, _) {
    if (!_) return;
    if (ololo($, _) == false) jQuery($)[looO1O](_)
};
Oo11 = function($, _) {
    if (!_) return;
    jQuery($)[ol1OO](_)
};
lo0000 = function($) {
    $ = l1Oo($);
    var _ = jQuery($);
    return {
        top: parseInt(_.css("margin-top"), 10) || 0,
        left: parseInt(_.css("margin-left"), 10) || 0,
        bottom: parseInt(_.css("margin-bottom"), 10) || 0,
        right: parseInt(_.css("margin-right"), 10) || 0
    }
};
Oll1 = function($) {
    $ = l1Oo($);
    var _ = jQuery($);
    return {
        top: parseInt(_.css("border-top-width"), 10) || 0,
        left: parseInt(_.css("border-left-width"), 10) || 0,
        bottom: parseInt(_.css("border-bottom-width"), 10) || 0,
        right: parseInt(_.css("border-right-width"), 10) || 0
    }
};
llOO = function($) {
    $ = l1Oo($);
    var _ = jQuery($);
    return {
        top: parseInt(_.css("padding-top"), 10) || 0,
        left: parseInt(_.css("padding-left"), 10) || 0,
        bottom: parseInt(_.css("padding-bottom"), 10) || 0,
        right: parseInt(_.css("padding-right"), 10) || 0
    }
};
o100oO = function(_, $) {
    _ = l1Oo(_);
    $ = parseInt($);
    if (isNaN($) || !_) return;
    if (jQuery.boxModel) {
        var A = llOO(_),
        B = Oll1(_);
        $ = $ - A.left - A.right - B.left - B.right
    }
    if ($ < 0) $ = 0;
    _.style.width = $ + "px"
};
oOOo = function(_, $) {
    _ = l1Oo(_);
    $ = parseInt($);
    if (isNaN($) || !_) return;
    if (jQuery.boxModel) {
        var A = llOO(_),
        B = Oll1(_);
        $ = $ - A.top - A.bottom - B.top - B.bottom
    }
    if ($ < 0) $ = 0;
    _.style.height = $ + "px"
};
oO1oo = function($, _) {
    $ = l1Oo($);
    if ($.style.display == "none" || $.type == "text/javascript") return 0;
    return _ ? jQuery($).width() : jQuery($).outerWidth()
};
l0ol = function($, _) {
    $ = l1Oo($);
    if ($.style.display == "none" || $.type == "text/javascript") return 0;
    return _ ? jQuery($).height() : jQuery($).outerHeight()
};
ooo1 = function(A, C, B, $, _) {
    if (B === undefined) {
        B = C.y;
        $ = C.width;
        _ = C.height;
        C = C.x
    }
    mini[O1110](A, C, B);
    o100oO(A, $);
    oOOo(A, _)
};
lolloO = function(A) {
    var $ = mini.getXY(A),
    _ = {
        x: $[0],
        y: $[1],
        width: oO1oo(A),
        height: l0ol(A)
    };
    _.left = _.x;
    _.top = _.y;
    _.right = _.x + _.width;
    _.bottom = _.y + _.height;
    return _
};
loOo = function(A, B) {
    A = l1Oo(A);
    if (!A || typeof B != "string") return;
    var F = jQuery(A),
    _ = B.toLowerCase().split(";");
    for (var $ = 0, C = _.length; $ < C; $++) {
        var E = _[$],
        D = E.split(":");
        if (D.length == 2) F.css(D[0].trim(), D[1].trim())
    }
};
ollO0 = function() {
    var $ = document.defaultView;
    return new Function("el", "style", ["style[looo1l]('-')>-1 && (style=style.replace(/-(\\w)/g,function(m,a){return a.toUpperCase()}));", "style=='float' && (style='", $ ? "cssFloat": "styleFloat", "');return el.style[style] || ", $ ? "window.getComputedStyle(el,null)[style]": "el.currentStyle[style]", " || null;"].join(""))
} ();
Ol11 = function(A, $) {
    var _ = false;
    A = l1Oo(A);
    $ = l1Oo($);
    if (A === $) return true;
    if (A && $) if (A.contains) {
        try {
            return A.contains($)
        } catch(B) {
            return false
        }
    } else if (A.compareDocumentPosition) return !! (A.compareDocumentPosition($) & 16);
    else while ($ = $.parentNode) _ = $ == A || _;
    return _
};
OO0O = function(B, A, $) {
    B = l1Oo(B);
    var C = document.body,
    _ = 0,
    D;
    $ = $ || 50;
    if (typeof $ != "number") {
        D = l1Oo($);
        $ = 10
    }
    while (B && B.nodeType == 1 && _ < $ && B != C && B != D) {
        if (ololo(B, A)) return B;
        _++;
        B = B.parentNode
    }
    return null
};
mini.copyTo(mini, {
    byId: l1Oo,
    hasClass: ololo,
    addClass: lloo10,
    removeClass: Oo11,
    getMargins: lo0000,
    getBorders: Oll1,
    getPaddings: llOO,
    setWidth: o100oO,
    setHeight: oOOo,
    getWidth: oO1oo,
    getHeight: l0ol,
    setBox: ooo1,
    getBox: lolloO,
    setStyle: loOo,
    getStyle: ollO0,
    repaint: function($) {
        if (!$) $ = document.body;
        lloo10($, "mini-repaint");
        setTimeout(function() {
            Oo11($, "mini-repaint")
        },
        1)
    },
    getSize: function($, _) {
        return {
            width: oO1oo($, _),
            height: l0ol($, _)
        }
    },
    setSize: function(A, $, _) {
        o100oO(A, $);
        oOOo(A, _)
    },
    setX: function(_, B) {
        B = parseInt(B);
        var $ = jQuery(_).offset(),
        A = parseInt($.top);
        if (A === undefined) A = $[1];
        mini[O1110](_, B, A)
    },
    setY: function(_, A) {
        A = parseInt(A);
        var $ = jQuery(_).offset(),
        B = parseInt($.left);
        if (B === undefined) B = $[0];
        mini[O1110](_, B, A)
    },
    setXY: function(_, B, A) {
        var $ = {
            left: parseInt(B),
            top: parseInt(A)
        };
        jQuery(_).offset($);
        jQuery(_).offset($)
    },
    getXY: function(_) {
        var $ = jQuery(_).offset();
        return [parseInt($.left), parseInt($.top)]
    },
    getViewportBox: function() {
        var $ = jQuery(window).width(),
        _ = jQuery(window).height(),
        B = jQuery(document).scrollLeft(),
        A = jQuery(document.body).scrollTop();
        if (document.documentElement) A = document.documentElement.scrollTop;
        return {
            x: B,
            y: A,
            width: $,
            height: _,
            right: B + $,
            bottom: A + _
        }
    },
    getChildNodes: function(A, C) {
        A = l1Oo(A);
        if (!A) return;
        var E = A.childNodes,
        B = [];
        for (var $ = 0, D = E.length; $ < D; $++) {
            var _ = E[$];
            if (_.nodeType == 1 || C === true) B.push(_)
        }
        return B
    },
    removeChilds: function(B, _) {
        B = l1Oo(B);
        if (!B) return;
        var C = mini[o01O00](B, true);
        for (var $ = 0, D = C.length; $ < D; $++) {
            var A = C[$];
            if (_ && A == _);
            else B.removeChild(C[$])
        }
    },
    isAncestor: Ol11,
    findParent: OO0O,
    findChild: function(_, A) {
        _ = l1Oo(_);
        var B = _.getElementsByTagName("*");
        for (var $ = 0, C = B.length; $ < C; $++) {
            var _ = B[$];
            if (ololo(_, A)) return _
        }
    },
    isAncestor: function(A, $) {
        var _ = false;
        A = l1Oo(A);
        $ = l1Oo($);
        if (A === $) return true;
        if (A && $) if (A.contains) {
            try {
                return A.contains($)
            } catch(B) {
                return false
            }
        } else if (A.compareDocumentPosition) return !! (A.compareDocumentPosition($) & 16);
        else while ($ = $.parentNode) _ = $ == A || _;
        return _
    },
    getOffsetsTo: function(_, A) {
        var $ = this.getXY(_),
        B = this.getXY(A);
        return [$[0] - B[0], $[1] - B[1]]
    },
    scrollIntoView: function(I, H, F) {
        var B = l1Oo(H) || document.body,
        $ = this.getOffsetsTo(I, B),
        C = $[0] + B.scrollLeft,
        J = $[1] + B.scrollTop,
        D = J + I.offsetHeight,
        A = C + I.offsetWidth,
        G = B.clientHeight,
        K = parseInt(B.scrollTop, 10),
        _ = parseInt(B.scrollLeft, 10),
        L = K + G,
        E = _ + B.clientWidth;
        if (I.offsetHeight > G || J < K) B.scrollTop = J;
        else if (D > L) B.scrollTop = D - G;
        B.scrollTop = B.scrollTop;
        if (F !== false) {
            if (I.offsetWidth > B.clientWidth || C < _) B.scrollLeft = C;
            else if (A > E) B.scrollLeft = A - B.clientWidth;
            B.scrollLeft = B.scrollLeft
        }
        return this
    },
    setOpacity: function(_, $) {
        jQuery(_).css({
            "opacity": $
        })
    },
    selectable: function(_, $) {
        _ = l1Oo(_);
        if ( !! $) {
            jQuery(_)[ol1OO]("mini-unselectable");
            if (isIE) _.unselectable = "off";
            else {
                _.style.MozUserSelect = "";
                _.style.KhtmlUserSelect = "";
                _.style.UserSelect = ""
            }
        } else {
            jQuery(_)[looO1O]("mini-unselectable");
            if (isIE) _.unselectable = "on";
            else {
                _.style.MozUserSelect = "none";
                _.style.UserSelect = "none";
                _.style.KhtmlUserSelect = "none"
            }
        }
    },
    selectRange: function(B, A, _) {
        if (B.createTextRange) {
            var $ = B.createTextRange();
            $.moveStart("character", A);
            $.moveEnd("character", _ - B.value.length);
            $[OlOlo1]()
        } else if (B.setSelectionRange) B.setSelectionRange(A, _);
        try {
            B[OlOoo]()
        } catch(C) {}
    },
    getSelectRange: function(A) {
        A = l1Oo(A);
        if (!A) return;
        try {
            A[OlOoo]()
        } catch(C) {}
        var $ = 0,
        B = 0;
        if (A.createTextRange) {
            var _ = document.selection.createRange().duplicate();
            _.moveEnd("character", A.value.length);
            if (_.text === "") $ = A.value.length;
            else $ = A.value.lastIndexOf(_.text);
            _ = document.selection.createRange().duplicate();
            _.moveStart("character", -A.value.length);
            B = _.text.length
        } else {
            $ = A.selectionStart;
            B = A.selectionEnd
        }
        return [$, B]
    }
}); (function() {
    var $ = {
        tabindex: "tabIndex",
        readonly: "readOnly",
        "for": "htmlFor",
        "class": "className",
        maxlength: "maxLength",
        cellspacing: "cellSpacing",
        cellpadding: "cellPadding",
        rowspan: "rowSpan",
        colspan: "colSpan",
        usemap: "useMap",
        frameborder: "frameBorder",
        contenteditable: "contentEditable"
    },
    _ = document.createElement("div");
    _.setAttribute("class", "t");
    var A = _.className === "t";
    mini.setAttr = function(B, C, _) {
        B.setAttribute(A ? C: ($[C] || C), _)
    };
    mini.getAttr = function(B, C) {
        if (C == "value" && (isIE6 || isIE7)) {
            var _ = B.attributes[C];
            return _ ? _.value: null
        }
        var D = B.getAttribute(A ? C: ($[C] || C));
        if (typeof D == "function") D = B.attributes[C].value;
        return D
    }
})();
loolll = function(_, $, C, A) {
    var B = "on" + $.toLowerCase();
    _[B] = function(_) {
        _ = _ || window.event;
        _.target = _.target || _.srcElement;
        if (!_.preventDefault) _.preventDefault = function() {
            if (window.event) window.event.returnValue = false
        };
        if (!_.stopPropogation) _.stopPropogation = function() {
            if (window.event) window.event.cancelBubble = true
        };
        var $ = C[Ool00](A, _);
        if ($ === false) return false
    }
};
looo = function(_, $, D, A) {
    _ = l1Oo(_);
    A = A || _;
    if (!_ || !$ || !D || !A) return false;
    var B = mini[lO00l](_, $, D, A);
    if (B) return false;
    var C = mini.createDelegate(D, A);
    mini.listeners.push([_, $, D, A, C]);
    if (jQuery.browser.mozilla && $ == "mousewheel") $ = "DOMMouseScroll";
    jQuery(_).bind($, C)
};
Ol100 = function(_, $, C, A) {
    _ = l1Oo(_);
    A = A || _;
    if (!_ || !$ || !C || !A) return false;
    var B = mini[lO00l](_, $, C, A);
    if (!B) return false;
    mini.listeners.remove(B);
    if (jQuery.browser.mozilla && $ == "mousewheel") $ = "DOMMouseScroll";
    jQuery(_).unbind($, B[4])
};
mini.copyTo(mini, {
    listeners: [],
    on: looo,
    un: Ol100,
    findListener: function(A, _, F, B) {
        A = l1Oo(A);
        B = B || A;
        if (!A || !_ || !F || !B) return false;
        var D = mini.listeners;
        for (var $ = 0, E = D.length; $ < E; $++) {
            var C = D[$];
            if (C[0] == A && C[1] == _ && C[2] == F && C[3] == B) return C
        }
    },
    clearEvent: function(A, _) {
        A = l1Oo(A);
        if (!A) return false;
        var C = mini.listeners;
        for (var $ = C.length - 1; $ >= 0; $--) {
            var B = C[$];
            if (B[0] == A) if (!_ || _ == B[1]) Ol100(A, B[1], B[2], B[3])
        }
    }
});
mini.__windowResizes = [];
mini.onWindowResize = function(_, $) {
    mini.__windowResizes.push([_, $])
};
looo(window, "resize", 
function(C) {
    var _ = mini.__windowResizes;
    for (var $ = 0, B = _.length; $ < B; $++) {
        var A = _[$];
        A[0][Ool00](A[1], C)
    }
});
mini.htmlEncode = function(_) {
    if (typeof _ !== "string") return _;
    var $ = "";
    if (_.length == 0) return "";
    $ = _.replace(/&/g, "&gt;");
    $ = $.replace(/</g, "&lt;");
    $ = $.replace(/>/g, "&gt;");
    $ = $.replace(/ /g, "&nbsp;");
    $ = $.replace(/\'/g, "&#39;");
    $ = $.replace(/\"/g, "&quot;");
    return $
};
mini.htmlDecode = function(_) {
    if (typeof _ !== "string") return _;
    var $ = "";
    if (_.length == 0) return "";
    $ = _.replace(/&gt;/g, "&");
    $ = $.replace(/&lt;/g, "<");
    $ = $.replace(/&gt;/g, ">");
    $ = $.replace(/&nbsp;/g, " ");
    $ = $.replace(/&#39;/g, "'");
    $ = $.replace(/&quot;/g, "\"");
    return $
};
mini.copyTo(Array.prototype, {
    add: Array[O0lloO].enqueue = function($) {
        this[this.length] = $;
        return this
    },
    getRange: function(_, A) {
        var B = [];
        for (var $ = _; $ <= A; $++) B[B.length] = this[$];
        return B
    },
    addRange: function(A) {
        for (var $ = 0, _ = A.length; $ < _; $++) this[this.length] = A[$];
        return this
    },
    clear: function() {
        this.length = 0;
        return this
    },
    clone: function() {
        if (this.length === 1) return [this[0]];
        else return Array.apply(null, this)
    },
    contains: function($) {
        return (this[looo1l]($) >= 0)
    },
    indexOf: function(_, B) {
        var $ = this.length;
        for (var A = (B < 0) ? Math[llolO0](0, $ + B) : B || 0; A < $; A++) if (this[A] === _) return A;
        return - 1
    },
    dequeue: function() {
        return this.shift()
    },
    insert: function(_, $) {
        this.splice(_, 0, $);
        return this
    },
    insertRange: function(_, B) {
        for (var A = B.length - 1; A >= 0; A--) {
            var $ = B[A];
            this.splice(_, 0, $)
        }
        return this
    },
    remove: function(_) {
        var $ = this[looo1l](_);
        if ($ >= 0) this.splice($, 1);
        return ($ >= 0)
    },
    removeAt: function($) {
        var _ = this[$];
        this.splice($, 1);
        return _
    },
    removeRange: function(_) {
        _ = _.clone();
        for (var $ = 0, A = _.length; $ < A; $++) this.remove(_[$])
    }
});
mini.Keyboard = {
    Left: 37,
    Top: 38,
    Right: 39,
    Bottom: 40,
    PageUp: 33,
    PageDown: 34,
    End: 35,
    Home: 36,
    Enter: 13,
    ESC: 27,
    Space: 32,
    Tab: 9,
    Del: 46,
    F1: 112,
    F2: 113,
    F3: 114,
    F4: 115,
    F5: 116,
    F6: 117,
    F7: 118,
    F8: 119,
    F9: 120,
    F10: 121,
    F11: 122,
    F12: 123
};
var ua = navigator.userAgent.toLowerCase(),
check = function($) {
    return $.test(ua)
},
DOC = document,
isStrict = DOC.compatMode == "CSS1Compat",
isOpera = Object[O0lloO].toString[Ool00](window.opera) == "[object Opera]",
isChrome = check(/chrome/),
isWebKit = check(/webkit/),
isSafari = !isChrome && check(/safari/),
isSafari2 = isSafari && check(/applewebkit\/4/),
isSafari3 = isSafari && check(/version\/3/),
isSafari4 = isSafari && check(/version\/4/),
isIE = !!window.attachEvent && !isOpera,
isIE7 = isIE && check(/msie 7/),
isIE8 = isIE && check(/msie 8/),
isIE9 = isIE && check(/msie 9/),
isIE10 = isIE && document.documentMode == 10,
isIE6 = isIE && !isIE7 && !isIE8 && !isIE9 && !isIE10,
isFirefox = navigator.userAgent[looo1l]("Firefox") > 0,
isGecko = !isWebKit && check(/gecko/),
isGecko2 = isGecko && check(/rv:1\.8/),
isGecko3 = isGecko && check(/rv:1\.9/),
isBorderBox = isIE && !isStrict,
isWindows = check(/windows|win32/),
isMac = check(/macintosh|mac os x/),
isAir = check(/adobeair/),
isLinux = check(/linux/),
isSecure = /^https/i.test(window.location.protocol);
if (isIE6) {
    try {
        DOC.execCommand("BackgroundImageCache", false, true)
    } catch(e) {}
}
mini.boxModel = !isBorderBox;
mini.isIE = isIE;
mini.isIE6 = isIE6;
mini.isIE7 = isIE7;
mini.isIE8 = isIE8;
mini.isIE9 = isIE9;
mini.isFirefox = isFirefox;
mini.isOpera = jQuery.browser.opera;
mini.isSafari = jQuery.browser.safari;
if (jQuery) jQuery.boxModel = mini.boxModel;
mini.noBorderBox = false;
if (jQuery.boxModel == false && isIE && isIE9 == false) mini.noBorderBox = true;
mini.MouseButton = {
    Left: 0,
    Middle: 1,
    Right: 2
};
if (isIE && !isIE9) mini.MouseButton = {
    Left: 1,
    Middle: 4,
    Right: 2
};
mini._MaskID = 1;
mini._MaskObjects = {};
mini[o1o1oo] = function(C) {
    var _ = l1Oo(C);
    if (mini.isElement(_)) C = {
        el: _
    };
    else if (typeof C == "string") C = {
        html: C
    };
    C = mini.copyTo({
        html: "",
        cls: "",
        style: "",
        backStyle: "background:#ccc"
    },
    C);
    C.el = l1Oo(C.el);
    if (!C.el) C.el = document.body;
    _ = C.el;
    mini["unmask"](C.el);
    _._maskid = mini._MaskID++;
    mini._MaskObjects[_._maskid] = C;
    var $ = mini.append(_, "<div class=\"mini-mask\">" + "<div class=\"mini-mask-background\" style=\"" + C.backStyle + "\"></div>" + "<div class=\"mini-mask-msg " + C.cls + "\" style=\"" + C.style + "\">" + C.html + "</div>" + "</div>");
    C.maskEl = $;
    if (!mini.isNull(C.opacity)) mini.setOpacity($.firstChild, C.opacity);
    function A() {
        B.style.display = "block";
        var $ = mini.getSize(B);
        B.style.marginLeft = -$.width / 2 + "px";
        B.style.marginTop = -$.height / 2 + "px"
    }
    var B = $.lastChild;
    B.style.display = "none";
    setTimeout(function() {
        A()
    },
    0)
};
mini["unmask"] = function(_) {
    _ = l1Oo(_);
    if (!_) _ = document.body;
    var A = mini._MaskObjects[_._maskid];
    if (!A) return;
    delete mini._MaskObjects[_._maskid];
    var $ = A.maskEl;
    A.maskEl = null;
    if ($ && $.parentNode) $.parentNode.removeChild($)
};
mini.Cookie = {
    get: function(D) {
        var A = document.cookie.split("; "),
        B = null;
        for (var $ = 0; $ < A.length; $++) {
            var _ = A[$].split("=");
            if (D == _[0]) B = _
        }
        if (B) {
            var C = B[1];
            if (C === undefined) return C;
            return unescape(C)
        }
        return null
    },
    set: function(C, $, B, A) {
        var _ = new Date();
        if (B != null) _ = new Date(_[llo1l]() + (B * 1000 * 3600 * 24));
        document.cookie = C + "=" + escape($) + ((B == null) ? "": ("; expires=" + _.toGMTString())) + ";path=/" + (A ? "; domain=" + A: "")
    },
    del: function(_, $) {
        this[ol0Ol1](_, null, -100, $)
    }
};
mini.copyTo(mini, {
    treeToArray: function(C, I, J, A, $) {
        if (!I) I = "children";
        var F = [];
        for (var H = 0, D = C.length; H < D; H++) {
            var B = C[H];
            F[F.length] = B;
            if (A) B[A] = $;
            var _ = B[I];
            if (_ && _.length > 0) {
                var E = B[J],
                G = this[ol0oo1](_, I, J, A, E);
                F.addRange(G)
            }
        }
        return F
    },
    arrayToTree: function(C, A, H, B) {
        if (!A) A = "children";
        H = H || "_id";
        B = B || "_pid";
        var G = [],
        F = {};
        for (var _ = 0, E = C.length; _ < E; _++) {
            var $ = C[_];
            if (!$) continue;
            var I = $[H];
            if (I !== null && I !== undefined) F[I] = $;
            delete $[A]
        }
        for (_ = 0, E = C.length; _ < E; _++) {
            var $ = C[_],
            D = F[$[B]];
            if (!D) {
                G.push($);
                continue
            }
            if (!D[A]) D[A] = [];
            D[A].push($)
        }
        return G
    }
});
function UUID() {
    var A = [],
    _ = "0123456789ABCDEF".split("");
    for (var $ = 0; $ < 36; $++) A[$] = Math.floor(Math.random() * 16);
    A[14] = 4;
    A[19] = (A[19] & 3) | 8;
    for ($ = 0; $ < 36; $++) A[$] = _[A[$]];
    A[8] = A[13] = A[18] = A[23] = "-";
    return A.join("")
}
String.format = function(_) {
    var $ = Array[O0lloO].slice[Ool00](arguments, 1);
    _ = _ || "";
    return _.replace(/\{(\d+)\}/g, 
    function(A, _) {
        return $[_]
    })
};
String[O0lloO].trim = function() {
    var $ = /^\s+|\s+$/g;
    return function() {
        return this.replace($, "")
    }
} ();
mini.copyTo(mini, {
    measureText: function(B, _, C) {
        if (!this.measureEl) this.measureEl = mini.append(document.body, "<div></div>");
        this.measureEl.style.cssText = "position:absolute;left:-1000px;top:-1000px;visibility:hidden;";
        if (typeof B == "string") this.measureEl.className = B;
        else {
            this.measureEl.className = "";
            var G = jQuery(B),
            A = jQuery(this.measureEl),
            F = ["font-size", "font-style", "font-weight", "font-family", "line-height", "text-transform", "letter-spacing"];
            for (var $ = 0, E = F.length; $ < E; $++) {
                var D = F[$];
                A.css(D, G.css(D))
            }
        }
        if (C) loOo(this.measureEl, C);
        this.measureEl.innerHTML = _;
        return mini.getSize(this.measureEl)
    }
});
jQuery(function() {
    var $ = new Date();
    mini.isReady = true;
    mini.parse();
    lo0lO();
    if ((ollO0(document.body, "overflow") == "hidden" || ollO0(document.documentElement, "overflow") == "hidden") && (isIE6 || isIE7)) {
        jQuery(document.body).css("overflow", "visible");
        jQuery(document.documentElement).css("overflow", "visible")
    }
    mini.__LastWindowWidth = document.documentElement.clientWidth;
    mini.__LastWindowHeight = document.documentElement.clientHeight
});
mini_onload = function($) {
    mini.layout(null, false);
    looo(window, "resize", mini_onresize)
};
looo(window, "load", mini_onload);
mini.__LastWindowWidth = document.documentElement.clientWidth;
mini.__LastWindowHeight = document.documentElement.clientHeight;
mini.doWindowResizeTimer = null;
mini.allowLayout = true;
mini_onresize = function(A) {
    if (mini.doWindowResizeTimer) clearTimeout(mini.doWindowResizeTimer);
    if (lllo1l == false || mini.allowLayout == false) return;
    if (typeof Ext != "undefined") mini.doWindowResizeTimer = setTimeout(function() {
        var _ = document.documentElement.clientWidth,
        $ = document.documentElement.clientHeight;
        if (mini.__LastWindowWidth == _ && mini.__LastWindowHeight == $);
        else {
            mini.__LastWindowWidth = _;
            mini.__LastWindowHeight = $;
            mini.layout(null, false)
        }
        mini.doWindowResizeTimer = null
    },
    300);
    else {
        var $ = 100;
        try {
            if (parent && parent != window && parent.mini) $ = 0
        } catch(_) {}
        mini.doWindowResizeTimer = setTimeout(function() {
            var _ = document.documentElement.clientWidth,
            $ = document.documentElement.clientHeight;
            if (mini.__LastWindowWidth == _ && mini.__LastWindowHeight == $);
            else {
                mini.__LastWindowWidth = _;
                mini.__LastWindowHeight = $;
                mini.layout(null, false)
            }
            mini.doWindowResizeTimer = null
        },
        $)
    }
};
mini[ll1001] = function(_, A) {
    var $ = A || document.body;
    while (1) {
        if (_ == null || !_.style) return false;
        if (_ && _.style && _.style.display == "none") return false;
        if (_ == $) return true;
        _ = _.parentNode
    }
    return true
};
mini.isWindowDisplay = function() {
    try {
        var _ = window.parent,
        E = _ != window;
        if (E) {
            var C = _.document.getElementsByTagName("iframe"),
            H = _.document.getElementsByTagName("frame"),
            G = [];
            for (var $ = 0, D = C.length; $ < D; $++) G.push(C[$]);
            for ($ = 0, D = H.length; $ < D; $++) G.push(H[$]);
            var B = null;
            for ($ = 0, D = G.length; $ < D; $++) {
                var A = G[$];
                if (A.contentWindow == window) {
                    B = A;
                    break
                }
            }
            if (!B) return false;
            return mini[ll1001](B, _.document.body)
        } else return true
    } catch(F) {
        return true
    }
};
lllo1l = mini.isWindowDisplay();
mini.layoutIFrames = function($) {
    if (!$) $ = document.body;
    var _ = $.getElementsByTagName("iframe");
    setTimeout(function() {
        for (var A = 0, C = _.length; A < C; A++) {
            var B = _[A];
            try {
                if (mini[ll1001](B) && Ol11($, B)) {
                    if (B.contentWindow.mini) if (B.contentWindow.lllo1l == false) {
                        B.contentWindow.lllo1l = B.contentWindow.mini.isWindowDisplay();
                        B.contentWindow.mini.layout()
                    } else B.contentWindow.mini.layout(null, false);
                    B.contentWindow.mini.layoutIFrames()
                }
            } catch(D) {}
        }
    },
    30)
};
$.ajaxSetup({
    cache: false
});
if (isIE) setInterval(function() {
    CollectGarbage()
},
1000);
mini_unload = function(H) {
    try {
        var E = mini._getTopWindow();
        E[mini._WindowID] = "";
        delete E[mini._WindowID]
    } catch(D) {}
    var G = document.body.getElementsByTagName("iframe");
    if (G.length > 0) {
        var F = [];
        for (var $ = 0, C = G.length; $ < C; $++) F.push(G[$]);
        for ($ = 0, C = F.length; $ < C; $++) {
            try {
                var B = F[$];
                B.src = "";
                if (B.parentNode) B.parentNode.removeChild(B)
            } catch(H) {}
        }
    }
    var A = mini.getComponents();
    for ($ = 0, C = A.length; $ < C; $++) {
        var _ = A[$];
        _[oOllOo](false)
    }
    A.length = 0;
    A = null;
    Ol100(window, "unload", mini_unload);
    Ol100(window, "load", mini_onload);
    Ol100(window, "resize", mini_onresize);
    mini.components = {};
    mini.classes = {};
    mini.uiClasses = {};
    mini.uids = {};
    mini._topWindow = null;
    window.mini = null;
    window.Owner = null;
    window.CloseOwnerWindow = null;
    try {
        CollectGarbage()
    } catch(H) {}
    window.onerror = function() {
        return true
    }
};
looo(window, "unload", mini_unload);
function __OnIFrameMouseDown() {
    jQuery(document).trigger("mousedown")
}
function _OOl0() {
    var C = document.getElementsByTagName("iframe");
    for (var $ = 0, A = C.length; $ < A; $++) {
        var _ = C[$];
        try {
            if (_.contentWindow) _.contentWindow.document.onmousedown = __OnIFrameMouseDown
        } catch(B) {}
    }
}
setInterval(function() {
    _OOl0()
},
1500);
mini.zIndex = 1000;
mini.getMaxZIndex = function() {
    return mini.zIndex++
};
function js_isTouchDevice() {
    try {
        document.createEvent("TouchEvent");
        return true
    } catch($) {
        return false
    }
}
function js_touchScroll(A) {
    if (js_isTouchDevice()) {
        var _ = typeof A == "string" ? document.getElementById(A) : A,
        $ = 0;
        _.addEventListener("touchstart", 
        function(_) {
            $ = this.scrollTop + _.touches[0].pageY;
            _.preventDefault()
        },
        false);
        _.addEventListener("touchmove", 
        function(_) {
            this.scrollTop = $ - _.touches[0].pageY;
            _.preventDefault()
        },
        false)
    }
}
if (typeof window.rootpath == "undefined") rootpath = "/";
mini.loadJS = function(_, $) {
    if (!_) return;
    if (typeof $ == "function") return loadJS._async(_, $);
    else return loadJS._sync(_)
};
mini.loadJS._js = {};
mini.loadJS._async = function(D, _) {
    var C = mini.loadJS._js[D];
    if (!C) C = mini.loadJS._js[D] = {
        create: false,
        loaded: false,
        callbacks: []
    };
    if (C.loaded) {
        setTimeout(function() {
            _()
        },
        1);
        return
    } else {
        C.callbacks.push(_);
        if (C.create) return
    }
    C.create = true;
    var B = document.getElementsByTagName("head")[0],
    A = document.createElement("script");
    A.src = D;
    A.type = "text/javascript";
    function $() {
        for (var $ = 0; $ < C.callbacks.length; $++) {
            var _ = C.callbacks[$];
            if (_) _()
        }
        C.callbacks.length = 0
    }
    setTimeout(function() {
        if (document.all) A.onreadystatechange = function() {
            if (A.readyState == "loaded" || A.readyState == "complete") {
                $();
                C.loaded = true
            }
        };
        else A.onload = function() {
            $();
            C.loaded = true
        };
        B.appendChild(A)
    },
    1);
    return A
};
mini.loadJS._sync = function(A) {
    if (loadJS._js[A]) return;
    loadJS._js[A] = {
        create: true,
        loaded: true,
        callbacks: []
    };
    var _ = document.getElementsByTagName("head")[0],
    $ = document.createElement("script");
    $.type = "text/javascript";
    $.text = loadText(A);
    _.appendChild($);
    return $
};
mini.loadText = function(C) {
    var B = "",
    D = document.all && location.protocol == "file:",
    A = null;
    if (D) A = new ActiveXObject("Microsoft.XMLHTTP");
    else if (window.XMLHttpRequest) A = new XMLHttpRequest();
    else if (window.ActiveXObject) A = new ActiveXObject("Microsoft.XMLHTTP");
    A.onreadystatechange = $;
    var _ = "_t=" + new Date()[llo1l]();
    if (C[looo1l]("?") == -1) _ = "?" + _;
    else _ = "&" + _;
    C += _;
    A.open("GET", C, false);
    A.send(null);
    function $() {
        if (A.readyState == 4) {
            var $ = D ? 0: 200;
            if (A.status == $) B = A.responseText
        }
    }
    return B
};
mini.loadJSON = function(url) {
    var text = loadText(url),
    o = eval("(" + text + ")");
    return o
};
mini.loadCSS = function(A, B) {
    if (!A) return;
    if (loadCSS._css[A]) return;
    var $ = document.getElementsByTagName("head")[0],
    _ = document.createElement("link");
    if (B) _.id = B;
    _.href = A;
    _.rel = "stylesheet";
    _.type = "text/css";
    $.appendChild(_);
    return _
};
mini.loadCSS._css = {};
mini.innerHTML = function(A, _) {
    if (typeof A == "string") A = document.getElementById(A);
    if (!A) return;
    _ = "<div style=\"display:none\">&nbsp;</div>" + _;
    A.innerHTML = _;
    mini.__executeScripts(A);
    var $ = A.firstChild
};
mini.__executeScripts = function($) {
    var A = $.getElementsByTagName("script");
    for (var _ = 0, E = A.length; _ < E; _++) {
        var B = A[_],
        D = B.src;
        if (D) mini.loadJS(D);
        else {
            var C = document.createElement("script");
            C.type = "text/javascript";
            C.text = B.text;
            $.appendChild(C)
        }
    }
    for (_ = A.length - 1; _ >= 0; _--) {
        B = A[_];
        B.parentNode.removeChild(B)
    }
};
oo1oo1 = function() {
    this._bindFields = [];
    this._bindForms = [];
    oo1oo1[oOOOoO][lllOo0][Ool00](this)
};
ol1O(oo1oo1, Oo11l0, {});
ll111 = oo1oo1[O0lloO];
ll111.loolOl = o00O0;
ll111.OloOOO = olOO0;
ll111[O1OOo1] = oloO1;
ll111[l0lo1] = l0o0l;
ooOl0(oo1oo1, "databinding");
oo0ll1 = function() {
    this._sources = {};
    this._data = {};
    this._links = [];
    this.loo1O1 = {};
    oo0ll1[oOOOoO][lllOo0][Ool00](this)
};
ol1O(oo0ll1, Oo11l0, {});
ol010 = oo0ll1[O0lloO];
ol010.ool1 = lOll1;
ol010.o0Ooo = lOo0o;
ol010.O1o0 = l011;
ol010.O01O11 = OlOo0;
ol010.OoOoO = ll0o01;
ol010.loOOlo = lOO10;
ol010.l1l0l = o0011;
ol010[OlOO1l] = oO0Oo;
ol010[o1OOlO] = oO101;
ol010[OO1oo0] = l0o11;
ol010[O0olo1] = llO11;
ooOl0(oo0ll1, "dataset");
loo0l = function() {
    loo0l[oOOOoO][lllOo0][Ool00](this)
};
ol1O(loo0l, lo0O01, {
    _clearBorder: false,
    formField: true,
    value: "",
    uiCls: "mini-hidden"
});
o0oo = loo0l[O0lloO];
o0oo[lO0OOO] = ool01;
o0oo[o0Oll0] = OOl0o;
o0oo[o101l] = Ooo010;
o0oo[ll0O1O] = oOl0O;
o0oo[lOlo11] = Oo10O;
ooOl0(loo0l, "hidden");
o11Ool = function() {
    o11Ool[oOOOoO][lllOo0][Ool00](this);
    this[l0l10O](false);
    this[OOOl1O](this.allowDrag);
    this[o1l0Ol](this[l000l])
};
ol1O(o11Ool, mini.Container, {
    _clearBorder: false,
    uiCls: "mini-popup"
});
OlO11O = o11Ool[O0lloO];
OlO11O[l1OllO] = OOllO;
OlO11O[olll1O] = l0O0ll;
OlO11O[lo0o00] = OO01oo;
OlO11O[lOOo10] = l1O01o;
OlO11O[oOllOo] = o0l0;
OlO11O[o10l10] = llOO1l;
OlO11O[OOOol0] = l1OO;
OlO11O[lOlo11] = O11OO;
ooOl0(o11Ool, "popup");
o11Ool_prototype = {
    isPopup: false,
    popupEl: null,
    popupCls: "",
    showAction: "mouseover",
    hideAction: "outerclick",
    showDelay: 300,
    hideDelay: 500,
    hAlign: "left",
    vAlign: "below",
    hOffset: 0,
    vOffset: 0,
    minWidth: 50,
    minHeight: 25,
    maxWidth: 2000,
    maxHeight: 2000,
    showModal: false,
    showShadow: true,
    modalStyle: "opacity:0.2",
    o1111o: "mini-popup-drag",
    oOll: "mini-popup-resize",
    allowDrag: false,
    allowResize: false,
    o000lo: function() {
        if (!this.popupEl) return;
        Ol100(this.popupEl, "click", this.l0Oo, this);
        Ol100(this.popupEl, "contextmenu", this.ol01, this);
        Ol100(this.popupEl, "mouseover", this.o00oO0, this)
    },
    oo10o0: function() {
        if (!this.popupEl) return;
        looo(this.popupEl, "click", this.l0Oo, this);
        looo(this.popupEl, "contextmenu", this.ol01, this);
        looo(this.popupEl, "mouseover", this.o00oO0, this)
    },
    doShow: function(A) {
        var $ = {
            popupEl: this.popupEl,
            htmlEvent: A,
            cancel: false
        };
        this[lOO1lo]("BeforeOpen", $);
        if ($.cancel == true) return;
        this[lOO1lo]("opening", $);
        if ($.cancel == true) return;
        if (!this.popupEl) this[O1Ol01]();
        else {
            var _ = {};
            if (A) _.xy = [A.pageX, A.pageY];
            this.showAtEl(this.popupEl, _)
        }
    },
    doHide: function(_) {
        var $ = {
            popupEl: this.popupEl,
            htmlEvent: _,
            cancel: false
        };
        this[lOO1lo]("BeforeClose", $);
        if ($.cancel == true) return;
        this.close()
    },
    show: function(_, $) {
        this.showAtPos(_, $)
    },
    showAtPos: function(B, A) {
        this[lO0oOo](document.body);
        if (!B) B = "center";
        if (!A) A = "middle";
        this.el.style.position = "absolute";
        this.el.style.left = "-2000px";
        this.el.style.top = "-2000px";
        this.el.style.display = "";
        this.o1OoO0();
        var _ = mini[o1OO0](),
        $ = lolloO(this.el);
        if (B == "left") B = 0;
        if (B == "center") B = _.width / 2 - $.width / 2;
        if (B == "right") B = _.width - $.width;
        if (A == "top") A = 0;
        if (A == "middle") A = _.y + _.height / 2 - $.height / 2;
        if (A == "bottom") A = _.height - $.height;
        if (B + $.width > _.right) B = _.right - $.width;
        if (A + $.height > _.bottom) A = _.bottom - $.height;
        this.O1lo1o(B, A)
    },
    lol0o: function() {
        jQuery(this.lol00o).remove();
        if (!this[loOl1l]) return;
        if (this.visible == false) return;
        var $ = document.documentElement,
        A = parseInt(Math[llolO0](document.body.scrollWidth, $ ? $.scrollWidth: 0)),
        D = parseInt(Math[llolO0](document.body.scrollHeight, $ ? $.scrollHeight: 0)),
        C = mini[o1OO0](),
        B = C.height;
        if (B < D) B = D;
        var _ = C.width;
        if (_ < A) _ = A;
        this.lol00o = mini.append(document.body, "<div class=\"mini-modal\"></div>");
        this.lol00o.style.height = B + "px";
        this.lol00o.style.width = _ + "px";
        this.lol00o.style.zIndex = ollO0(this.el, "zIndex") - 1;
        loOo(this.lol00o, this.modalStyle)
    },
    O1l11O: function() {
        if (!this.shadowEl) this.shadowEl = mini.append(document.body, "<div class=\"mini-shadow\"></div>");
        this.shadowEl.style.display = this[Ooo1o] ? "": "none";
        if (this[Ooo1o]) {
            var $ = lolloO(this.el),
            A = this.shadowEl.style;
            A.width = $.width + "px";
            A.height = $.height + "px";
            A.left = $.x + "px";
            A.top = $.y + "px";
            var _ = ollO0(this.el, "zIndex");
            if (!isNaN(_)) this.shadowEl.style.zIndex = _ - 2
        }
    },
    o1OoO0: function() {
        this.el.style.display = "";
        var $ = lolloO(this.el);
        if ($.width > this.maxWidth) {
            o100oO(this.el, this.maxWidth);
            $ = lolloO(this.el)
        }
        if ($.height > this.maxHeight) {
            oOOo(this.el, this.maxHeight);
            $ = lolloO(this.el)
        }
        if ($.width < this.minWidth) {
            o100oO(this.el, this.minWidth);
            $ = lolloO(this.el)
        }
        if ($.height < this.minHeight) {
            oOOo(this.el, this.minHeight);
            $ = lolloO(this.el)
        }
    },
    showAtEl: function(H, D) {
        H = l1Oo(H);
        if (!H) return;
        if (!this[o1loo0]() || this.el.parentNode != document.body) this[lO0oOo](document.body);
        var A = {
            hAlign: this.hAlign,
            vAlign: this.vAlign,
            hOffset: this.hOffset,
            vOffset: this.vOffset,
            popupCls: this.popupCls
        };
        mini.copyTo(A, D);
        lloo10(H, A.popupCls);
        H.popupCls = A.popupCls;
        this._popupEl = H;
        this.el.style.position = "absolute";
        this.el.style.left = "-2000px";
        this.el.style.top = "-2000px";
        this.el.style.display = "";
        this[o10l10]();
        this.o1OoO0();
        var J = mini[o1OO0](),
        B = lolloO(this.el),
        L = lolloO(H),
        F = A.xy,
        C = A.hAlign,
        E = A.vAlign,
        M = J.width / 2 - B.width / 2,
        K = 0;
        if (F) {
            M = F[0];
            K = F[1]
        }
        switch (A.hAlign) {
        case "outleft":
            M = L.x - B.width;
            break;
        case "left":
            M = L.x;
            break;
        case "center":
            M = L.x + L.width / 2 - B.width / 2;
            break;
        case "right":
            M = L.right - B.width;
            break;
        case "outright":
            M = L.right;
            break;
        default:
            break
        }
        switch (A.vAlign) {
        case "above":
            K = L.y - B.height;
            break;
        case "top":
            K = L.y;
            break;
        case "middle":
            K = L.y + L.height / 2 - B.height / 2;
            break;
        case "bottom":
            K = L.bottom - B.height;
            break;
        case "below":
            K = L.bottom;
            break;
        default:
            break
        }
        M = parseInt(M);
        K = parseInt(K);
        if (A.outVAlign || A.outHAlign) {
            if (A.outVAlign == "above") if (K + B.height > J.bottom) {
                var _ = L.y - J.y,
                I = J.bottom - L.bottom;
                if (_ > I) K = L.y - B.height
            }
            if (A.outHAlign == "outleft") if (M + B.width > J.right) {
                var G = L.x - J.x,
                $ = J.right - L.right;
                if (G > $) M = L.x - B.width
            }
            if (A.outHAlign == "right") if (M + B.width > J.right) M = L.right - B.width;
            this.O1lo1o(M, K)
        } else this.showAtPos(M + A.hOffset, K + A.vOffset)
    },
    O1lo1o: function(A, _) {
        this.el.style.display = "";
        this.el.style.zIndex = mini.getMaxZIndex();
        mini.setX(this.el, A);
        mini.setY(this.el, _);
        this[l0l10O](true);
        if (this.hideAction == "mouseout") looo(document, "mousemove", this.O0o1o, this);
        var $ = this;
        this.O1l11O();
        this.lol0o();
        mini.layoutIFrames(this.el);
        this.isPopup = true;
        looo(document, "mousedown", this.oO1OO, this);
        looo(window, "resize", this.Olol01, this);
        this[lOO1lo]("Open")
    },
    open: function() {
        this[O1Ol01]()
    },
    close: function() {
        this[o10ll1]()
    },
    hide: function() {
        if (!this.el) return;
        if (this.popupEl) Oo11(this.popupEl, this.popupEl.popupCls);
        if (this._popupEl) Oo11(this._popupEl, this._popupEl.popupCls);
        this._popupEl = null;
        jQuery(this.lol00o).remove();
        if (this.shadowEl) this.shadowEl.style.display = "none";
        Ol100(document, "mousemove", this.O0o1o, this);
        Ol100(document, "mousedown", this.oO1OO, this);
        Ol100(window, "resize", this.Olol01, this);
        this[l0l10O](false);
        this.isPopup = false;
        this[lOO1lo]("Close")
    },
    setPopupEl: function($) {
        $ = l1Oo($);
        if (!$) return;
        this.o000lo();
        this.popupEl = $;
        this.oo10o0()
    },
    setPopupCls: function($) {
        this.popupCls = $
    },
    setShowAction: function($) {
        this.showAction = $
    },
    setHideAction: function($) {
        this.hideAction = $
    },
    setShowDelay: function($) {
        this.showDelay = $
    },
    setHideDelay: function($) {
        this.hideDelay = $
    },
    setHAlign: function($) {
        this.hAlign = $
    },
    setVAlign: function($) {
        this.vAlign = $
    },
    setHOffset: function($) {
        $ = parseInt($);
        if (isNaN($)) $ = 0;
        this.hOffset = $
    },
    setVOffset: function($) {
        $ = parseInt($);
        if (isNaN($)) $ = 0;
        this.vOffset = $
    },
    setShowModal: function($) {
        this[loOl1l] = $
    },
    setShowShadow: function($) {
        this[Ooo1o] = $
    },
    setMinWidth: function($) {
        if (isNaN($)) return;
        this.minWidth = $
    },
    setMinHeight: function($) {
        if (isNaN($)) return;
        this.minHeight = $
    },
    setMaxWidth: function($) {
        if (isNaN($)) return;
        this.maxWidth = $
    },
    setMaxHeight: function($) {
        if (isNaN($)) return;
        this.maxHeight = $
    },
    setAllowDrag: function($) {
        this.allowDrag = $;
        Oo11(this.el, this.o1111o);
        if ($) lloo10(this.el, this.o1111o)
    },
    setAllowResize: function($) {
        this[l000l] = $;
        Oo11(this.el, this.oOll);
        if ($) lloo10(this.el, this.oOll)
    },
    l0Oo: function(_) {
        if (this.l0lo0) return;
        if (this.showAction != "leftclick") return;
        var $ = jQuery(this.popupEl).attr("allowPopup");
        if (String($) == "false") return;
        this.doShow(_)
    },
    ol01: function(_) {
        if (this.l0lo0) return;
        if (this.showAction != "rightclick") return;
        var $ = jQuery(this.popupEl).attr("allowPopup");
        if (String($) == "false") return;
        _.preventDefault();
        this.doShow(_)
    },
    o00oO0: function(A) {
        if (this.l0lo0) return;
        if (this.showAction != "mouseover") return;
        var _ = jQuery(this.popupEl).attr("allowPopup");
        if (String(_) == "false") return;
        clearTimeout(this._hideTimer);
        this._hideTimer = null;
        if (this.isPopup) return;
        var $ = this;
        this._showTimer = setTimeout(function() {
            $.doShow(A)
        },
        this.showDelay)
    },
    O0o1o: function($) {
        if (this.hideAction != "mouseout") return;
        this.l1Oo1($)
    },
    oO1OO: function($) {
        if (this.hideAction != "outerclick") return;
        if (!this.isPopup) return;
        if (this[o1O0O0]($) || (this.popupEl && Ol11(this.popupEl, $.target)));
        else this.doHide($)
    },
    l1Oo1: function(_) {
        if (Ol11(this.el, _.target) || (this.popupEl && Ol11(this.popupEl, _.target)));
        else {
            clearTimeout(this._showTimer);
            this._showTimer = null;
            if (this._hideTimer) return;
            var $ = this;
            this._hideTimer = setTimeout(function() {
                $.doHide(_)
            },
            this.hideDelay)
        }
    },
    Olol01: function($) {
        if (this[ll1001]() && !mini.isIE6) this.lol0o()
    },
    within: function(C) {
        if (Ol11(this.el, C.target)) return true;
        var $ = mini.getChildControls(this);
        for (var _ = 0, B = $.length; _ < B; _++) {
            var A = $[_];
            if (A[o1O0O0](C)) return true
        }
        return false
    }
};
mini.copyTo(o11Ool.prototype, o11Ool_prototype);
l0O0O1 = function() {
    l0O0O1[oOOOoO][lllOo0][Ool00](this)
};
ol1O(l0O0O1, lo0O01, {
    text: "",
    iconCls: "",
    iconStyle: "",
    plain: false,
    checkOnClick: false,
    checked: false,
    groupName: "",
    O10l1: "mini-button-plain",
    _hoverCls: "mini-button-hover",
    ll01O: "mini-button-pressed",
    lOOo1l: "mini-button-checked",
    OOloo: "mini-button-disabled",
    allowCls: "",
    _clearBorder: false,
    uiCls: "mini-button",
    href: "",
    target: ""
});
OoO1o = l0O0O1[O0lloO];
OoO1o[l1OllO] = Ol0oo;
OoO1o[oOlO0l] = O01lO;
OoO1o.OoO0O0 = o0ooO;
OoO1o.o0oOOo = o1Ooo;
OoO1o.lloO = Oo10OO;
OoO1o[Oo11ol] = OlOO01;
OoO1o[o0lO1o] = Oo1Oo;
OoO1o[lO0ol0] = olol1;
OoO1o[oOO0l1] = Oolll;
OoO1o[lO1Ool] = O1oOo0;
OoO1o[O1l1o1] = o00o1;
OoO1o[OO0lll] = o11l;
OoO1o[lo1001] = oooO1;
OoO1o[lloolo] = O110o;
OoO1o[l0ool1] = oO1O1o;
OoO1o[Olo110] = oll0;
OoO1o[llOOo0] = O1o0l;
OoO1o[olo0o0] = o01Oo;
OoO1o[lO1l0o] = O1Ol0o;
OoO1o[o111l0] = oOo1oO;
OoO1o[ooo1oo] = oO1O0;
OoO1o[O0loll] = ooOll0;
OoO1o[ol10l1] = Ol1lo;
OoO1o[l0o1OO] = O0Oo1;
OoO1o[Ol1oO1] = OlOOO;
OoO1o[lollOo] = lloo0;
OoO1o[lolo1] = llol0;
OoO1o[oOllOo] = lOoO0;
OoO1o[OOOol0] = l0l0o;
OoO1o[lOlo11] = Oo0001;
OoO1o[ol0Ol1] = l0lOO;
ooOl0(l0O0O1, "button");
Oo0Ooo = function() {
    Oo0Ooo[oOOOoO][lllOo0][Ool00](this)
};
ol1O(Oo0Ooo, l0O0O1, {
    uiCls: "mini-menubutton",
    allowCls: "mini-button-menu"
});
loOoO = Oo0Ooo[O0lloO];
loOoO[l0l1O] = OlolO;
loOoO[Ool10] = OO010;
ooOl0(Oo0Ooo, "menubutton");
mini.SplitButton = function() {
    mini.SplitButton[oOOOoO][lllOo0][Ool00](this)
};
ol1O(mini.SplitButton, Oo0Ooo, {
    uiCls: "mini-splitbutton",
    allowCls: "mini-button-split"
});
ooOl0(mini.SplitButton, "splitbutton");
lllO0o = function() {
    lllO0o[oOOOoO][lllOo0][Ool00](this)
};
ol1O(lllO0o, lo0O01, {
    formField: true,
    text: "",
    checked: false,
    defaultValue: false,
    trueValue: true,
    falseValue: false,
    uiCls: "mini-checkbox"
});
l0lll0 = lllO0o[O0lloO];
l0lll0[l1OllO] = O1lOo0;
l0lll0.oo11oO = oOOO1O;
l0lll0[o01001] = loOOl;
l0lll0[O1l1lo] = Oo11O;
l0lll0[lllloo] = Ooo01;
l0lll0[llOo1O] = lll1o;
l0lll0[lO0OOO] = ooo0o;
l0lll0[o0Oll0] = oO1oO;
l0lll0[o101l] = O000o;
l0lll0[o0lO1o] = lOo01;
l0lll0[lO0ol0] = O1l00O;
l0lll0[ooo1oo] = oloo0;
l0lll0[O0loll] = llOo;
l0lll0[ll0O1O] = ol1Oll;
l0lll0[OOOol0] = o0OO;
l0lll0[oOllOo] = ol1l0;
l0lll0[lOlo11] = Oool1;
ooOl0(lllO0o, "checkbox");
ool10O = function() {
    ool10O[oOOOoO][lllOo0][Ool00](this);
    var $ = this[l0O0Oo]();
    if ($ || this.allowInput == false) this.llo0lO[O00O01] = true;
    if (this.enabled == false) this[o00lO1](this.OOloo);
    if ($) this[o00lO1](this.Oo0loo);
    if (this.required) this[o00lO1](this.lool1o)
};
ol1O(ool10O, OoO0ll, {
    name: "",
    formField: true,
    selectOnFocus: false,
    defaultValue: "",
    value: "",
    text: "",
    emptyText: "",
    maxLength: 1000,
    minLength: 0,
    width: 125,
    height: 21,
    inputAsValue: false,
    allowInput: true,
    O00O: "mini-buttonedit-noInput",
    Oo0loo: "mini-buttonedit-readOnly",
    OOloo: "mini-buttonedit-disabled",
    O01OOl: "mini-buttonedit-empty",
    l10llO: "mini-buttonedit-focus",
    l10oO: "mini-buttonedit-button",
    ol1o1: "mini-buttonedit-button-hover",
    OoO1: "mini-buttonedit-button-pressed",
    uiCls: "mini-buttonedit",
    Olo0lo: false,
    _buttonWidth: 20,
    Olo0l1: null,
    textName: ""
});
ol0l0 = ool10O[O0lloO];
ol0l0[l1OllO] = OoOo1;
ol0l0[ol111] = l0ooo;
ol0l0[O01lo] = l0OOl;
ol0l0[oOo01O] = OOo1o;
ol0l0[OOlOOO] = ooo1O;
ol0l0[OllOoO] = lo0OO;
ol0l0[OOl00] = O0llO;
ol0l0[oO0oOO] = o0l00;
ol0l0.oO1l = o1Oo1;
ol0l0.o0olO = lOO1o;
ol0l0.Oo1O = oOO10;
ol0l0.ooooO = lllo0;
ol0l0.O100 = O10ol;
ol0l0.OlOl0 = O0O01;
ol0l0.OOOlO = oOl1O;
ol0l0[O00OOO] = loOo1;
ol0l0.ol10l = l0l0l;
ol0l0.OoO0O0 = o0oOl;
ol0l0.o0oOOo = o10ll;
ol0l0.lloO = o1O1O;
ol0l0.loOo11 = oolo1;
ol0l0[O11OlO] = o1loO;
ol0l0[ooo0lo] = oOoll;
ol0l0[OOl1l0] = ll101;
ol0l0[olOol] = OloO1;
ol0l0[O0OoOl] = ooloo;
ol0l0.olO10 = O101o;
ol0l0[O1OolO] = OOO1O;
ol0l0[llllOO] = l1o0oO;
ol0l0[OOOl1] = O000o0;
ol0l0[Olo0O0] = l1o0l;
ol0l0[lO0OOO] = oO0l1;
ol0l0[o0Oll0] = l1l1l;
ol0l0[o101l] = oolol;
ol0l0[ooo1oo] = o1O001;
ol0l0[O0loll] = loooO;
ol0l0[l10ol] = OO110;
ol0l0[o10o11] = llO1O;
ol0l0[ll0O1O] = oo0OO;
ol0l0[l1oO0O] = o1O001El;
ol0l0[OOOO00] = l1OO0;
ol0l0[llo101] = lOoOO;
ol0l0[OlOoo] = l0olo;
ol0l0.ll000l = o1llo;
ol0l0[lo0o00] = Oo0O0;
ol0l0[o10l10] = O001l;
ol0l0.OOOlO0 = O1l01;
ol0l0[OOOol0] = o0l0O;
ol0l0[oOllOo] = o1l0O;
ol0l0[lOlo11] = O101o0;
ol0l0.O1oOolHtml = lO11l;
ol0l0[ol0Ol1] = ol0ol;
ooOl0(ool10O, "buttonedit");
ol10lo = function() {
    ol10lo[oOOOoO][lllOo0][Ool00](this)
};
ol1O(ol10lo, OoO0ll, {
    name: "",
    formField: true,
    selectOnFocus: false,
    minHeight: 15,
    maxLength: 5000,
    emptyText: "",
    text: "",
    value: "",
    defaultValue: "",
    width: 125,
    height: 21,
    O01OOl: "mini-textbox-empty",
    l10llO: "mini-textbox-focus",
    OOloo: "mini-textbox-disabled",
    uiCls: "mini-textbox",
    O0l0Oo: "text",
    Olo0lo: false,
    Olo0l1: null,
    vtype: ""
});
o00O00 = ol10lo[O0lloO];
o00O00[oO01lo] = O1001;
o00O00[OllOOo] = oll01;
o00O00[lo01O0] = ooOl;
o00O00[O0OOll] = OOoo1o;
o00O00[l01olo] = o011l;
o00O00[o001o1] = l00l0;
o00O00[O1ll0O] = O11lo1;
o00O00[o1Ol1O] = olOoO;
o00O00[o0lOOO] = ll01o1;
o00O00[OOlool] = lllo;
o00O00[oOOOol] = o101O;
o00O00[l1O1lo] = o1o0O;
o00O00[OOOlol] = l1o01;
o00O00[Oo00l1] = o101o;
o00O00[lOO100] = OlO0lO;
o00O00[O0ooo1] = O0Ool;
o00O00[oo00oo] = olOloo;
o00O00[O110ll] = o00Oo;
o00O00[lO10l0] = o00ll;
o00O00[l11OO0] = o01O0;
o00O00[o1011O] = O01Oo;
o00O00[oolo01] = ll01oO;
o00O00[l10Oll] = l1111l;
o00O00[oo10oo] = O1OlO;
o00O00.llO0 = O01O;
o00O00[oo0oll] = oooOo;
o00O00[olool] = olllO;
o00O00[l1OllO] = o1o10;
o00O00.OOOlO = OloOo;
o00O00.ol10l = Ol111;
o00O00.Oo1O = Ol1011;
o00O00.ooooO = oO01O;
o00O00.OlOl0 = l1lo;
o00O00.oO001o = ooOoO;
o00O00.O100 = lol0O;
o00O00.o0oOOo = l0O0l;
o00O00.loOo11 = oOo1O;
o00O00[O11OlO] = OoOOoO;
o00O00[ol111] = llO0O;
o00O00[O01lo] = o0olo;
o00O00[l001o0] = OlO1o;
o00O00[l1oO0O] = OoOOll;
o00O00[OOOO00] = Ol00;
o00O00[llo101] = O0lOO;
o00O00[OlOoo] = o1O01;
o00O00[lolo1] = o0o1Ol;
o00O00[l0l1O] = lol1l;
o00O00[lllo10] = l0oooO;
o00O00[OOOl1] = l1o1lO;
o00O00.l00o = llOl0;
o00O00[Olo0O0] = O1OOl;
o00O00[l10ol] = oolO0;
o00O00[o10o11] = oOlOll;
o00O00.ll000l = oO0ol;
o00O00[olOol] = ol1ol;
o00O00[O0OoOl] = olloO;
o00O00[lO0OOO] = lo110;
o00O00[o0Oll0] = ooo0O;
o00O00[o101l] = Ooo0;
o00O00[ll0O1O] = lO1O1;
o00O00[lo0o00] = O110o1;
o00O00[o10l10] = O1011l;
o00O00[oOllOo] = O1o00;
o00O00.OOOlO0 = OOO0l;
o00O00[OOOol0] = ol0010;
o00O00[lOlo11] = oOlOl;
ooOl0(ol10lo, "textbox");
O01oo0 = function() {
    O01oo0[oOOOoO][lllOo0][Ool00](this)
};
ol1O(O01oo0, ol10lo, {
    uiCls: "mini-password",
    O0l0Oo: "password"
});
lO0O1 = O01oo0[O0lloO];
lO0O1[o10o11] = l1olO;
ooOl0(O01oo0, "password");
O1lOlo = function() {
    O1lOlo[oOOOoO][lllOo0][Ool00](this)
};
ol1O(O1lOlo, ol10lo, {
    maxLength: 10000000,
    width: 180,
    height: 50,
    minHeight: 50,
    O0l0Oo: "textarea",
    uiCls: "mini-textarea"
});
l1O0O = O1lOlo[O0lloO];
l1O0O[o10l10] = OOl10;
ooOl0(O1lOlo, "textarea");
Ool1Ol = function() {
    Ool1Ol[oOOOoO][lllOo0][Ool00](this);
    this[OO1O00]();
    this.el.className += " mini-popupedit"
};
ol1O(Ool1Ol, ool10O, {
    uiCls: "mini-popupedit",
    popup: null,
    popupCls: "mini-buttonedit-popup",
    _hoverCls: "mini-buttonedit-hover",
    ll01O: "mini-buttonedit-pressed",
    popupWidth: "100%",
    popupMinWidth: 50,
    popupMaxWidth: 2000,
    popupHeight: "",
    popupMinHeight: 30,
    popupMaxHeight: 2000
});
l0l11 = Ool1Ol[O0lloO];
l0l11[l1OllO] = l0oo;
l0l11.o0oooO = o0Ol0;
l0l11.lloO = l0ll;
l0l11[loOO] = o1100;
l0l11[O011Ol] = Olool;
l0l11[oo11Oo] = O1o0o;
l0l11[l0lOOl] = OOoo1;
l0l11[Ool10O] = o0o1l;
l0l11[Oo1Ooo] = OlOlO;
l0l11[ol1oo0] = OOOOl;
l0l11[oO10ol] = o1011;
l0l11[oOo10O] = l1000;
l0l11[o10l01] = Olo11;
l0l11[l0oo0O] = olO01;
l0l11[o0110O] = oool0;
l0l11[l00101] = OlO0;
l0l11[O1Oo10] = l1O1l1;
l0l11.lo0l = ll1oo;
l0l11[O1010l] = l0ll0;
l0l11[o10l10] = lOloo;
l0l11[l1l1O1] = ll000;
l0l11.ooO0 = looOl;
l0l11.l00l = O0oo1;
l0l11[OO1O00] = O1llO;
l0l11[O0l1O] = OO10o;
l0l11[llOlll] = lO1lo;
l0l11[o1O0O0] = O0lol;
l0l11.OlOl0 = lo1ol;
l0l11.o0oOOo = OOOOO0;
l0l11.olO10o = oloOo;
l0l11.o00oO0 = O0llo;
l0l11.OOOlO = O1lo;
l0l11.ool0O1 = loO0l;
l0l11[OOOol0] = oOO1O;
l0l11[oOllOo] = ll0l0;
ooOl0(Ool1Ol, "popupedit");
Oooll1 = function() {
    this.data = [];
    this.columns = [];
    Oooll1[oOOOoO][lllOo0][Ool00](this);
    var $ = this;
    if (isFirefox) this.llo0lO.oninput = function() {
        $.OlOOll()
    }
};
ol1O(Oooll1, Ool1Ol, {
    text: "",
    value: "",
    valueField: "id",
    textField: "text",
    delimiter: ",",
    multiSelect: false,
    data: [],
    url: "",
    columns: [],
    allowInput: false,
    valueFromSelect: false,
    popupMaxHeight: 200,
    uiCls: "mini-combobox",
    showNullItem: false
});
olOo0 = Oooll1[O0lloO];
olOo0[l1OllO] = lll0OO;
olOo0.O100 = Oolol;
olOo0[Ooo0o0] = olOl;
olOo0.lo0l = oo00;
olOo0.ool1O = lo11o;
olOo0.OlOOll = l0111o;
olOo0.Oo1O = O1O1O;
olOo0.ooooO = l1o1;
olOo0.OlOl0 = O1lOO;
olOo0.oOll0 = l0101;
olOo0[Ol0lll] = O11oO;
olOo0[llllOo] = l0o11O;
olOo0[O1O0oo] = l0o11Os;
olOo0.Ol110 = ol11;
olOo0[ool0oo] = l100o;
olOo0[oo1Olo] = oO110;
olOo0[olo0OO] = o0ool;
olOo0[o1OOll] = o001l1;
olOo0[oOlOoo] = oooOO;
olOo0[ooolol] = O0Ooo;
olOo0[lolO0O] = Ol1Ol;
olOo0[l1011O] = Ol10O;
olOo0[O1Olo0] = OOO00;
olOo0[o101l1] = O1O0;
olOo0[o101l] = O0ooOo;
olOo0[Olo0oo] = lO010;
olOo0[oll1lo] = OO11OO;
olOo0[O1lloo] = ol1oo;
olOo0[olO0Ol] = ll0O1;
olOo0[lloO1] = O0ooOoField;
olOo0[OOO10o] = Ol000o;
olOo0[o0oO0l] = lolOo1;
olOo0[OlOO1l] = Olol1;
olOo0[l1OlOo] = o1O1l;
olOo0[o01o1] = l0o10;
olOo0[ooO0Ol] = o1110;
olOo0[looo1l] = oo01l1;
olOo0[o001ol] = OoO1O;
olOo0[OlOlo1] = l1O1O;
olOo0[l1l1O1] = oo0Ol;
olOo0[OO1O00] = l1OOl;
olOo0[ol0Ol1] = l1oo;
ooOl0(Oooll1, "combobox");
l1O0lO = function() {
    l1O0lO[oOOOoO][lllOo0][Ool00](this)
};
ol1O(l1O0lO, Ool1Ol, {
    format: "yyyy-MM-dd",
    popupWidth: "",
    viewDate: new Date(),
    showTime: false,
    timeFormat: "H:mm",
    showTodayButton: true,
    showClearButton: true,
    uiCls: "mini-datepicker"
});
o01l = l1O0lO[O0lloO];
o01l[l1OllO] = lo1Ol;
o01l.OlOl0 = Ol0lo;
o01l.O100 = o1Ol1;
o01l[oO11O0] = l0lol;
o01l[Oll10l] = lOoOo;
o01l[OlOo01] = lloOO;
o01l[oll1o0] = lOlo1;
o01l[O1o00o] = o00OO;
o01l[lOl0O0] = oOllo;
o01l[loolOO] = o1lO0;
o01l[o1oO01] = ooo0l;
o01l[l011O0] = oO010;
o01l[o00llo] = OO00l;
o01l[lO0OOO] = lO10l;
o01l[o0Oll0] = Oo0oo;
o01l[o101l] = lOlOO;
o01l[OlOloO] = lOol;
o01l.o010o1 = lo1oo;
o01l.lo0o10 = lo0ll;
o01l.OOO0oo = lOo00l;
o01l.ooO0 = oO001;
o01l[o1O0O0] = O01O0;
o01l[O1Oo10] = lO10O;
o01l[l1l1O1] = lollO;
o01l[OO1O00] = Oo10l;
o01l[lo1O0O] = l1llo;
ooOl0(l1O0lO, "datepicker");
lo0Ol0 = function() {
    this.viewDate = new Date();
    this.oO01 = [];
    lo0Ol0[oOOOoO][lllOo0][Ool00](this)
};
ol1O(lo0Ol0, lo0O01, {
    width: 220,
    height: 160,
    _clearBorder: false,
    viewDate: null,
    Olo0l: "",
    oO01: [],
    multiSelect: false,
    firstDayOfWeek: 0,
    todayText: "Today",
    clearText: "Clear",
    okText: "OK",
    cancelText: "Cancel",
    daysShort: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
    format: "MMM,yyyy",
    timeFormat: "H:mm",
    showTime: false,
    currentTime: true,
    rows: 1,
    columns: 1,
    headerCls: "",
    bodyCls: "",
    footerCls: "",
    lo1OO1: "mini-calendar-today",
    o00l01: "mini-calendar-weekend",
    OolO1: "mini-calendar-othermonth",
    o0O0: "mini-calendar-selected",
    showHeader: true,
    showFooter: true,
    showWeekNumber: false,
    showDaysHeader: true,
    showMonthButtons: true,
    showYearButtons: true,
    showTodayButton: true,
    showClearButton: true,
    uiCls: "mini-calendar",
    menuEl: null,
    menuYear: null,
    menuSelectMonth: null,
    menuSelectYear: null
});
ooOOl1 = lo0Ol0[O0lloO];
ooOOl1[l1OllO] = lo1100;
ooOOl1.Ol110 = O11lO;
ooOOl1.l1O0oO = O0oO0;
ooOOl1.o010o1 = oloOl;
ooOOl1.o0oOOo = o00O0o;
ooOOl1.lloO = o10oo;
ooOOl1.lllOl = O01OO;
ooOOl1.OOO0ll = O1o01;
ooOOl1[ol1100] = O01Ol;
ooOOl1[l0ol1o] = l1oO00;
ooOOl1[lOO110] = O1Oo0;
ooOOl1.l0OOlO = o0ll0;
ooOOl1.llo00O = ol100;
ooOOl1.llo10O = ll00O;
ooOOl1[lolo1] = oO0O0;
ooOOl1[o10l10] = loO01O;
ooOOl1[O1o00o] = OloO;
ooOOl1[lOl0O0] = o1l0l;
ooOOl1[loolOO] = oloOlo;
ooOOl1[o1oO01] = l11oO;
ooOOl1[lolO0O] = O111o;
ooOOl1[l1011O] = O1oo;
ooOOl1[OO0111] = O1ool;
ooOOl1[o00o0] = lOo1o;
ooOOl1[O1Olo0] = OOlo0;
ooOOl1[o101l1] = O0101;
ooOOl1[OO0ool] = oO0lo;
ooOOl1[lO0OOO] = lO1oO;
ooOOl1[o0Oll0] = Oll0O;
ooOOl1[o101l] = loO1O;
ooOOl1[llo1l] = OOo1l;
ooOOl1[o0lolO] = OO1oO;
ooOOl1[Oo11O1] = olOl1;
ooOOl1[O01o1l] = ooOo;
ooOOl1[O1o1l0] = OOoll;
ooOOl1[l011O0] = llOOo;
ooOOl1[o00llo] = Ooo00;
ooOOl1[oO11O0] = lol10;
ooOOl1[Oll10l] = OOlOl;
ooOOl1[OlOo01] = o01ol;
ooOOl1[oll1o0] = lOO0O;
ooOOl1[l1l1lO] = looOOO;
ooOOl1[O01l0l] = OoOoo;
ooOOl1[l0oOo0] = Ol0o0;
ooOOl1[lOo01O] = o1l1;
ooOOl1[Ol1O0] = oOo00;
ooOOl1[OoO101] = o01o0;
ooOOl1[lllo0o] = Ol11o;
ooOOl1[oOo0o0] = O0OoO;
ooOOl1[oO10O1] = o1O0O;
ooOOl1[lOo000] = lOO0;
ooOOl1[o0O0o1] = lollo;
ooOOl1[l0100] = l0llO;
ooOOl1[o1O0O0] = OloOl;
ooOOl1[O1O00l] = Oo0O1O;
ooOOl1[OOOol0] = OO11o;
ooOOl1[oOllOo] = O1olo;
ooOOl1[OlOoo] = OoOo0;
ooOOl1[lOlo11] = oo101;
ooOOl1[o100O0] = lol1O;
ooOOl1[ll10O1] = olol;
ooOOl1[l1101o] = lool1;
ooOl0(lo0Ol0, "calendar");
Oolloo = function() {
    Oolloo[oOOOoO][lllOo0][Ool00](this)
};
ol1O(Oolloo, o1oOOl, {
    formField: true,
    width: 200,
    columns: null,
    columnWidth: 80,
    showNullItem: false,
    nullItemText: "",
    showEmpty: false,
    emptyText: "",
    showCheckBox: false,
    showAllCheckBox: true,
    multiSelect: false,
    l0ol0O: "mini-listbox-item",
    oo00O: "mini-listbox-item-hover",
    _l0110: "mini-listbox-item-selected",
    uiCls: "mini-listbox"
});
o00lO = Oolloo[O0lloO];
o00lO[l1OllO] = lo0O;
o00lO.lloO = ll11O;
o00lO.O0O0ll = lOOl1o;
o00lO.o1ll1l = lOo00;
o00lO.OOool = Ol10ol;
o00lO[olo0OO] = l1o0o;
o00lO[o1OOll] = Oo010;
o00lO[oOlOoo] = ol1lo;
o00lO[ooolol] = ll1O1l;
o00lO[llOO00] = l1lO1;
o00lO[o1olO1] = lloOo;
o00lO[o1OooO] = OlOOo;
o00lO[O0oOo0] = Olo0Ol;
o00lO[o10l10] = lll10;
o00lO[lolo1] = loo11;
o00lO[lolO0O] = O11l1;
o00lO[l1011O] = O01o;
o00lO[oOllOo] = oloO0;
o00lO[OOOol0] = OlOo1;
o00lO[oOllOo] = oloO0;
o00lO[lOlo11] = o0O1;
ooOl0(Oolloo, "listbox");
Oloo1o = function() {
    Oloo1o[oOOOoO][lllOo0][Ool00](this)
};
ol1O(Oloo1o, o1oOOl, {
    formField: true,
    multiSelect: true,
    repeatItems: 0,
    repeatLayout: "none",
    repeatDirection: "horizontal",
    l0ol0O: "mini-checkboxlist-item",
    oo00O: "mini-checkboxlist-item-hover",
    _l0110: "mini-checkboxlist-item-selected",
    lO0Oo: "mini-checkboxlist-table",
    Ol0oO0: "mini-checkboxlist-td",
    lOlO: "checkbox",
    uiCls: "mini-checkboxlist"
});
O0l10 = Oloo1o[O0lloO];
O0l10[l1OllO] = O01oO;
O0l10[oOll10] = l0loO;
O0l10[o1oOoO] = o0o1O;
O0l10[oolOlO] = o0O1l;
O0l10[oolOl0] = O00o0;
O0l10[loo00l] = l1oOo;
O0l10[l0lOOo] = l10l0;
O0l10.OolO = oo1lO;
O0l10.o1oO = O1Ool;
O0l10[lolo1] = Oo00O;
O0l10.lOl0 = ll001;
O0l10[lOlo11] = oO0oO;
ooOl0(Oloo1o, "checkboxlist");
ol0O0l = function() {
    ol0O0l[oOOOoO][lllOo0][Ool00](this)
};
ol1O(ol0O0l, Oloo1o, {
    multiSelect: false,
    l0ol0O: "mini-radiobuttonlist-item",
    oo00O: "mini-radiobuttonlist-item-hover",
    _l0110: "mini-radiobuttonlist-item-selected",
    lO0Oo: "mini-radiobuttonlist-table",
    Ol0oO0: "mini-radiobuttonlist-td",
    lOlO: "radio",
    uiCls: "mini-radiobuttonlist"
});
Olll1 = ol0O0l[O0lloO];
ooOl0(ol0O0l, "radiobuttonlist");
o10o01 = function() {
    this.data = [];
    o10o01[oOOOoO][lllOo0][Ool00](this)
};
ol1O(o10o01, Ool1Ol, {
    valueFromSelect: false,
    text: "",
    value: "",
    autoCheckParent: false,
    expandOnLoad: false,
    valueField: "id",
    textField: "text",
    nodesField: "children",
    delimiter: ",",
    multiSelect: false,
    data: [],
    url: "",
    allowInput: false,
    showTreeIcon: false,
    showTreeLines: true,
    resultAsTree: false,
    parentField: "pid",
    checkRecursive: false,
    showFolderCheckBox: false,
    popupHeight: 200,
    popupWidth: "100%",
    popupMaxHeight: 250,
    popupMinWidth: 100,
    uiCls: "mini-treeselect"
});
oO01l = o10o01[O0lloO];
oO01l[l1OllO] = oOlo0;
oO01l[ool0oo] = ol1l1;
oO01l[oo1Olo] = Oo1o1;
oO01l[o01Ol] = Ol1O1;
oO01l[OO1Oll] = oOl0l;
oO01l[Ol1110] = OO1O0;
oO01l[Ool0O0] = o0O10;
oO01l[O0lOl] = oOOO0;
oO01l[oOOO0O] = oo1lo;
oO01l[l0o01o] = o01l0;
oO01l[o0l00o] = llO01;
oO01l[o0llol] = Oo0Oo;
oO01l[ol011o] = OO1Ol;
oO01l[olO0Ol] = ll1o1;
oO01l[lloO1] = o0o0l;
oO01l[o01O1l] = oOOoo;
oO01l[l0l1oo] = l1Ool;
oO01l[OOloO] = OOoo0;
oO01l[l1Oo01] = l1ol1;
oO01l[O1OoO1] = o0o00;
oO01l[lOO0oO] = O0o10;
oO01l.ool1O = lOo10;
oO01l.OlOl0 = loo0O;
oO01l.OoOO = O1l00;
oO01l.oOoO0o = O000O;
oO01l[O1Olo0] = ol0oO;
oO01l[o101l1] = oO1lO;
oO01l[o101l] = lO01Oo;
oO01l[o0o00l] = o1o11;
oO01l[oOolO1] = oll0l;
oO01l[oll1lo] = oO10O;
oO01l[O1lloo] = O0OlO1;
oO01l[OOO10o] = O0lO0;
oO01l[o0oO0l] = o1Ool;
oO01l[OlOO1l] = l0OoO;
oO01l[l1OlOo] = O1loo0;
oO01l[o01o1] = O11Ol;
oO01l[ooO0Ol] = o0lOO;
oO01l[looo1l] = OOo10;
oO01l[o001ol] = lloll;
oO01l.lo0l = OoOOo;
oO01l[l1l1O1] = O0lo;
oO01l[o01O00] = o1O00;
oO01l[l10l00] = ooo1o;
oO01l[ol1oll] = llOoo;
oO01l[OOO0O] = o1OO1;
oO01l.OOo0 = llO0l;
oO01l.OO11 = Ool0o;
oO01l.lo10oO = O1ol0;
oO01l.oo0oOl = l1o11;
oO01l[OO1O00] = lolO0;
oO01l[ol0Ol1] = oo0l0;
ooOl0(o10o01, "TreeSelect");
o0000l = function() {
    o0000l[oOOOoO][lllOo0][Ool00](this);
    this[o101l](this[O1loO])
};
ol1O(o0000l, ool10O, {
    value: 0,
    minValue: 0,
    maxValue: 100,
    increment: 1,
    decimalPlaces: 0,
    uiCls: "mini-spinner",
    oooo1O: null
});
l1Ol1 = o0000l[O0lloO];
l1Ol1[l1OllO] = OoO00;
l1Ol1.O100 = l1Ol11;
l1Ol1.OoOo1l = l0l0l1;
l1Ol1.o1o00O = OO0l0;
l1Ol1.OlOl0 = loo1l0;
l1Ol1.OOo10O = O1Oo1;
l1Ol1.o0O1oo = O0l11;
l1Ol1.o0lol = lOO11o;
l1Ol1[loOoll] = o10lo;
l1Ol1[l1olOo] = oO100;
l1Ol1[loo0lO] = llolo1;
l1Ol1[O1l0Oo] = lO1lO;
l1Ol1[llO1O0] = l0oll;
l1Ol1[lolOo0] = l1O101;
l1Ol1[loO0ol] = lo001;
l1Ol1[o0o0Ol] = Oolo1l;
l1Ol1[o101l] = oOo0l;
l1Ol1.lO11ll = oo1O0;
l1Ol1[OOOol0] = o100lo;
l1Ol1.O1oOolHtml = loloo;
l1Ol1[ol0Ol1] = o1010;
ooOl0(o0000l, "spinner");
ooo1ol = function() {
    ooo1ol[oOOOoO][lllOo0][Ool00](this);
    this[o101l]("00:00:00")
};
ol1O(ooo1ol, ool10O, {
    value: null,
    format: "H:mm:ss",
    uiCls: "mini-timespinner",
    oooo1O: null
});
l00o1 = ooo1ol[O0lloO];
l00o1[l1OllO] = oO1lo;
l00o1.O100 = OoooO;
l00o1.OoOo1l = l11O1;
l00o1.OOo10O = l11lO;
l00o1.o0O1oo = ooO11;
l00o1.o0lol = o1Olo;
l00o1.oo00ol = O1o10;
l00o1[l001OO] = oo0ol;
l00o1[lO0OOO] = ooOOO;
l00o1[o0Oll0] = Olo01;
l00o1[o101l] = Ol0O1;
l00o1[O1l0O] = l0o0O;
l00o1[OlOloO] = O1o1o;
l00o1[OOOol0] = lllll;
l00o1.O1oOolHtml = O0Ol1;
ooOl0(ooo1ol, "timespinner");
ol1Oo1 = function() {
    ol1Oo1[oOOOoO][lllOo0][Ool00](this);
    this[O1oOo1]("validation", this.llO0, this)
};
ol1O(ol1Oo1, ool10O, {
    width: 180,
    buttonText: "\u6d4f\u89c8...",
    _buttonWidth: 56,
    limitType: "",
    limitTypeErrorText: "\u4e0a\u4f20\u6587\u4ef6\u683c\u5f0f\u4e3a\uff1a",
    allowInput: false,
    readOnly: true,
    Oo0l0: 0,
    uiCls: "mini-htmlfile"
});
ol11l = ol1Oo1[O0lloO];
ol11l[l1OllO] = l1ool;
ol11l[loll00] = O1oO0;
ol11l[oOllol] = lo1O0;
ol11l[o01OoO] = loOOol;
ol11l[lllO0l] = Oo1OO;
ol11l[o0Oll0] = O101l;
ol11l[ll0O1O] = loo00;
ol11l.llO0 = oo11O;
ol11l.OloOO = o1ool;
ol11l.O01lol = llll0;
ol11l.O1oOolHtml = o10o;
ol11l[lOlo11] = OlllO;
ooOl0(ol1Oo1, "htmlfile");
o00lOl = function($) {
    o00lOl[oOOOoO][lllOo0][Ool00](this, $);
    this[O1oOo1]("validation", this.llO0, this)
};
ol1O(o00lOl, ool10O, {
    width: 180,
    buttonText: "\u6d4f\u89c8...",
    _buttonWidth: 56,
    limitTypeErrorText: "\u4e0a\u4f20\u6587\u4ef6\u683c\u5f0f\u4e3a\uff1a",
    readOnly: true,
    Oo0l0: 0,
    limitSize: "",
    limitType: "",
    typesDescription: "\u4e0a\u4f20\u6587\u4ef6\u683c\u5f0f",
    uploadLimit: 0,
    queueLimit: "",
    flashUrl: "",
    uploadUrl: "",
    uploadOnSelect: false,
    uiCls: "mini-fileupload"
});
oOO11 = o00lOl[O0lloO];
oOO11[l1OllO] = OoloO;
oOO11[oOoo1O] = Ol01O;
oOO11[O1o0oo] = Oo00;
oOO11[Ol000] = lO0l1;
oOO11[lllOO1] = O0o0l;
oOO11[OO101O] = Oll11;
oOO11[oOlOO0] = O1loo;
oOO11[ll0O1O] = o1l11;
oOO11[olooO1] = OoOOO;
oOO11[l10loO] = oll10;
oOO11[O0Ol1o] = o0O0O;
oOO11[OO1o01] = O1oOO;
oOO11[O10111] = ll00;
oOO11[oOllol] = lolO1;
oOO11[Olo0] = ooooo;
oOO11.OloOO = oO1l1;
oOO11[oOllOo] = l1lOo;
oOO11.O1oOolHtml = l0O00;
oOO11[lOlo11] = olo0;
ooOl0(o00lOl, "fileupload");
O00llo = function() {
    this.data = [];
    O00llo[oOOOoO][lllOo0][Ool00](this);
    looo(this.llo0lO, "mouseup", this.O10O1l, this);
    this[O1oOo1]("showpopup", this.__OnShowPopup, this)
};
ol1O(O00llo, Ool1Ol, {
    allowInput: true,
    valueField: "id",
    textField: "text",
    delimiter: ",",
    multiSelect: false,
    data: [],
    grid: null,
    uiCls: "mini-lookup"
});
O0oO1 = O00llo[O0lloO];
O0oO1[l1OllO] = O01oOo;
O0oO1.ooOO1l = loO001;
O0oO1.O10O1l = O1oOl0;
O0oO1.OlOl0 = ool110;
O0oO1[lolo1] = l01oO;
O0oO1[ol0lol] = O11O1O;
O0oO1.lOo001 = lO0Ol;
O0oO1[llO00o] = oll1O;
O0oO1[O0loll] = oO01o1;
O0oO1[o101l] = o1l1l1;
O0oO1.O0010 = ol11O;
O0oO1.Ol1l00 = l1000O;
O0oO1.O0OOo = OoOl0;
O0oO1[Oo0111] = o00ol;
O0oO1[o10oOo] = O1OO1;
O0oO1[oool00] = lol011;
O0oO1[oll1lo] = O0oOo;
O0oO1[O1lloo] = oO01o1Field;
O0oO1[olO0Ol] = OlOo1l;
O0oO1[lloO1] = o1l1l1Field;
O0oO1[ooOllO] = lllO;
O0oO1[Oo10lo] = O0l01;
O0oO1[o101l1] = lo1O1;
O0oO1[oOllOo] = O1Oll;
ooOl0(O00llo, "lookup");
ll01l1 = function() {
    ll01l1[oOOOoO][lllOo0][Ool00](this);
    this.data = [];
    this[lolo1]()
};
ol1O(ll01l1, OoO0ll, {
    formField: true,
    value: "",
    text: "",
    valueField: "id",
    textField: "text",
    url: "",
    delay: 250,
    allowInput: true,
    editIndex: 0,
    l10llO: "mini-textboxlist-focus",
    oOoOOO: "mini-textboxlist-item-hover",
    lO111: "mini-textboxlist-item-selected",
    ol0l1: "mini-textboxlist-close-hover",
    textName: "",
    uiCls: "mini-textboxlist",
    errorIconEl: null,
    popupLoadingText: "<span class='mini-textboxlist-popup-loading'>Loading...</span>",
    popupErrorText: "<span class='mini-textboxlist-popup-error'>Error</span>",
    popupEmptyText: "<span class='mini-textboxlist-popup-noresult'>No Result</span>",
    isShowPopup: false,
    popupHeight: "",
    popupMinHeight: 30,
    popupMaxHeight: 150
});
llol1 = ll01l1[O0lloO];
llol1[l1OllO] = Ol1oo;
llol1[llo101] = l1o10;
llol1[OlOoo] = O00lO;
llol1.OlOl0 = o0oo0;
llol1[Olllo] = l01oo;
llol1.l1O0oO = OOoOl;
llol1.lloO = llo00;
llol1.olO10o = OlO0o;
llol1.OloOO = lO1ll;
llol1[O1Oo10] = O0oo0;
llol1[l1l1O1] = llllo;
llol1[OO1O00] = O1lOl;
llol1[o1O0O0] = lOOlo;
llol1.o0Oool = O100l;
llol1.ool1O = lllOO;
llol1.l0o100 = l10l1;
llol1.lO1o0 = l0Oo0;
llol1[oO11l] = OO0ol;
llol1[O011Ol] = lo10O;
llol1[Ool10O] = ll1O1;
llol1[loOO] = o1oOo;
llol1[l0lOOl] = lo1Oo;
llol1[oo11Oo] = llOO1;
llol1[Oo1Ooo] = l01lo;
llol1[OOO10o] = OoO11;
llol1[o0oO0l] = llo01;
llol1[olOol] = l00oo;
llol1[O0OoOl] = O0Oo0;
llol1[oll1lo] = O0O0o;
llol1[O1lloo] = o1101;
llol1[olO0Ol] = o01OO;
llol1[lloO1] = OOO10;
llol1[O0loll] = Ol0OO;
llol1[o101l] = o110l;
llol1[ll0O1O] = l0l1l;
llol1[o0Oll0] = loOl1;
llol1[ooo1oo] = Ooo1O;
llol1[l001o0] = oolll;
llol1.Ol1l00 = lOo11;
llol1[Oo0lol] = ol0Ol;
llol1[l00Ol] = oo1ol;
llol1.O1ll = OoOlo;
llol1[OlOlo1] = O101;
llol1[OOOO0l] = o1l1l;
llol1[lo11lo] = l1o10Item;
llol1[O1111] = ol0o1;
llol1[oOol11] = OoOO1;
llol1[o001ol] = ll110;
llol1.O0l1 = ll110ByEvent;
llol1[lolo1] = Ooolo;
llol1[o10l10] = lloo1;
llol1.loOo11 = ooool;
llol1[O11OlO] = OOOOO;
llol1.Ool0l = O101O;
llol1[OOOol0] = looO1;
llol1[oOllOo] = OOll0;
llol1[lOlo11] = lOO1l;
llol1[oOo01O] = Ooo1OName;
llol1[OOlOOO] = Ol0OOName;
ooOl0(ll01l1, "textboxlist");
O00l0o = function() {
    O00l0o[oOOOoO][lllOo0][Ool00](this);
    var $ = this;
    $.o010o0 = null;
    this.llo0lO.onfocus = function() {
        $.o1lol = $.llo0lO.value;
        $.o010o0 = setInterval(function() {
            if ($.o1lol != $.llo0lO.value) {
                $.OlOOll();
                $.o1lol = $.llo0lO.value;
                if ($.llo0lO.value == "" && $.value != "") {
                    $[o101l]("");
                    $.Ol110()
                }
            }
        },
        10)
    };
    this.llo0lO.onblur = function() {
        clearInterval($.o010o0);
        if (!$[l00101]()) if ($.o1lol != $.llo0lO.value) if ($.llo0lO.value == "" && $.value != "") {
            $[o101l]("");
            $.Ol110()
        }
    };
    this._buttonEl.style.display = "none"
};
ol1O(O00l0o, Oooll1, {
    url: "",
    allowInput: true,
    delay: 250,
    minChars: 0,
    _buttonWidth: 0,
    uiCls: "mini-autocomplete",
    popupLoadingText: "<span class='mini-textboxlist-popup-loading'>Loading...</span>",
    popupErrorText: "<span class='mini-textboxlist-popup-error'>Error</span>",
    popupEmptyText: "<span class='mini-textboxlist-popup-noresult'>No Result</span>"
});
ooloO = O00l0o[O0lloO];
ooloO[l1OllO] = l0011;
ooloO.ool1O = o10Ol;
ooloO.OlOOll = Oo1O0;
ooloO[oO11l] = o0O01;
ooloO.OlOl0 = lo01o;
ooloO[l1l1O1] = loO1l;
ooloO[llo1lo] = l1o1o;
ooloO[OlO0OO] = oOl10;
ooloO[O0loll] = o11o0;
ooloO[o101l] = l1O1l;
ooloO[o0oO0l] = loloO;
ooOl0(O00l0o, "autocomplete");
mini.Form = function($) {
    this.el = l1Oo($);
    if (!this.el) throw new Error("form element not null");
    mini.Form[oOOOoO][lllOo0][Ool00](this)
};
ol1O(mini.Form, Oo11l0, {
    el: null,
    getFields: function() {
        if (!this.el) return [];
        var $ = mini.findControls(function($) {
            if (!$.el || $.formField != true) return false;
            if (Ol11(this.el, $.el)) return true;
            return false
        },
        this);
        return $
    },
    getFieldsMap: function() {
        var B = this.getFields(),
        A = {};
        for (var $ = 0, C = B.length; $ < C; $++) {
            var _ = B[$];
            if (_.name) A[_.name] = _
        }
        return A
    },
    getField: function($) {
        if (!this.el) return null;
        return mini[l1olO0]($, this.el)
    },
    getData: function(B, F) {
        if (mini.isNull(F)) F = true;
        var A = B ? "getFormValue": "getValue",
        $ = this.getFields(),
        D = {};
        for (var _ = 0, E = $.length; _ < E; _++) {
            var C = $[_],
            G = C[A];
            if (!G) continue;
            if (C.name) if (F == true) mini._setMap(C.name, G[Ool00](C), D);
            else D[C.name] = G[Ool00](C);
            if (C.textName && C[ooo1oo]) if (F == true) D[C.textName] = C[ooo1oo]();
            else mini._setMap(C.textName, C[ooo1oo](), D)
        }
        return D
    },
    setData: function(F, A, C) {
        if (mini.isNull(C)) C = true;
        if (typeof F != "object") F = {};
        var B = this.getFieldsMap();
        for (var D in B) {
            var _ = B[D];
            if (!_) continue;
            if (_[o101l]) {
                var E = F[D];
                if (C == true) E = mini._getMap(D, F);
                if (E === undefined && A === false) continue;
                if (E === null) E = "";
                _[o101l](E)
            }
            if (_[O0loll] && _.textName) {
                var $ = F[_.textName];
                if (C == true) $ = mini._getMap(_.textName, F);
                if (mini.isNull($)) $ = "";
                _[O0loll]($)
            }
        }
    },
    reset: function() {
        var $ = this.getFields();
        for (var _ = 0, B = $.length; _ < B; _++) {
            var A = $[_];
            if (!A[o101l]) continue;
            if (A[O0loll]) A[O0loll]("");
            A[o101l](A[Ooll])
        }
        this[lOoloo](true)
    },
    clear: function() {
        var $ = this.getFields();
        for (var _ = 0, B = $.length; _ < B; _++) {
            var A = $[_];
            if (!A[o101l]) continue;
            A[o101l]("");
            if (A[O0loll]) A[O0loll]("")
        }
        this[lOoloo](true)
    },
    validate: function(C) {
        var $ = this.getFields();
        for (var _ = 0, D = $.length; _ < D; _++) {
            var A = $[_];
            if (!A[lo0o1o]) continue;
            if (A[ll1001] && A[ll1001]()) {
                var B = A[lo0o1o]();
                if (B == false && C === false) break
            }
        }
        return this[lO0oOl]()
    },
    setIsValid: function(B) {
        var $ = this.getFields();
        for (var _ = 0, C = $.length; _ < C; _++) {
            var A = $[_];
            if (!A[lOoloo]) continue;
            A[lOoloo](B)
        }
    },
    isValid: function() {
        var $ = this.getFields();
        for (var _ = 0, B = $.length; _ < B; _++) {
            var A = $[_];
            if (!A[lO0oOl]) continue;
            if (A[lO0oOl]() == false) return false
        }
        return true
    },
    getErrorTexts: function() {
        var A = [],
        _ = this.getErrors();
        for (var $ = 0, C = _.length; $ < C; $++) {
            var B = _[$];
            A.push(B.errorText)
        }
        return A
    },
    getErrors: function() {
        var A = [],
        $ = this.getFields();
        for (var _ = 0, C = $.length; _ < C; _++) {
            var B = $[_];
            if (!B[lO0oOl]) continue;
            if (B[lO0oOl]() == false) A.push(B)
        }
        return A
    },
    mask: function($) {
        if (typeof $ == "string") $ = {
            html: $
        };
        $ = $ || {};
        $.el = this.el;
        if (!$.cls) $.cls = this.OO0Oll;
        mini[o1o1oo]($)
    },
    unmask: function() {
        mini[Oo1110](this.el)
    },
    OO0Oll: "mini-mask-loading",
    loadingMsg: "\u6570\u636e\u52a0\u8f7d\u4e2d\uff0c\u8bf7\u7a0d\u540e...",
    loading: function($) {
        this[o1o1oo]($ || this.loadingMsg)
    },
    loolOl: function($) {
        this._changed = true
    },
    _changed: false,
    setChanged: function(A) {
        this._changed = A;
        var $ = this.getFields();
        for (var _ = 0, C = $.length; _ < C; _++) {
            var B = $[_];
            B[O1oOo1]("valuechanged", this.loolOl, this)
        }
    },
    isChanged: function() {
        return this._changed
    },
    setEnabled: function(A) {
        var $ = this.getFields();
        for (var _ = 0, C = $.length; _ < C; _++) {
            var B = $[_];
            B[l0l1O](A)
        }
    }
});
O0Oo0o = function() {
    O0Oo0o[oOOOoO][lllOo0][Ool00](this)
};
ol1O(O0Oo0o, mini.Container, {
    style: "",
    _clearBorder: false,
    uiCls: "mini-fit"
});
lo0olo = O0Oo0o[O0lloO];
lo0olo[l1OllO] = llOo0;
lo0olo[ll0lo1] = l0l01;
lo0olo[o10l10] = lOll0l;
lo0olo[oO10o0] = O00Oo;
lo0olo[OOOol0] = l10l;
lo0olo[lOlo11] = o11O;
ooOl0(O0Oo0o, "fit");
oOlO00 = function() {
    this.ool0O1();
    oOlO00[oOOOoO][lllOo0][Ool00](this);
    if (this.url) this[o0oO0l](this.url);
    this.Ooo1 = this.O0ll1o
};
ol1O(oOlO00, mini.Container, {
    width: 250,
    title: "",
    iconCls: "",
    iconStyle: "",
    url: "",
    refreshOnExpand: false,
    maskOnLoad: true,
    showCollapseButton: false,
    showCloseButton: false,
    closeAction: "display",
    showHeader: true,
    showToolbar: false,
    showFooter: false,
    headerCls: "",
    headerStyle: "",
    bodyCls: "",
    bodyStyle: "",
    footerCls: "",
    footerStyle: "",
    toolbarCls: "",
    toolbarStyle: "",
    uiCls: "mini-panel",
    count: 1,
    loOlo: 80,
    expanded: true
});
O0lO1 = oOlO00[O0lloO];
O0lO1[l1OllO] = lOo1O;
O0lO1[OOlOO0] = Ol010;
O0lO1[O1lolO] = o1olOO;
O0lO1[O00l11] = ooO1l;
O0lO1[lOo010] = oOOlo;
O0lO1[ool1Ol] = OOol1;
O0lO1[OoOolO] = OOlo;
O0lO1[loO0Oo] = o111;
O0lO1[oOO1lo] = Ool11;
O0lO1[OOO10o] = ollO0o;
O0lO1[o0oO0l] = oOo1lo;
O0lO1[OlOl01] = oo01o;
O0lO1[o01o1] = l10o;
O0lO1.lOol01 = l0O0o;
O0lO1.ol0OlO = l1lOo1;
O0lO1.lOlOo1 = OO1ol;
O0lO1[l0oOOO] = O1l11;
O0lO1[o1oloO] = o000;
O0lO1[lo0111] = oO1O;
O0lO1[OlO11] = ol01O;
O0lO1[lollll] = o1O1;
O0lO1[O1l010] = lOl1;
O0lO1[OO10lo] = O0O00;
O0lO1[ll0lo1] = olOol0;
O0lO1[olll1O] = o0lOl;
O0lO1[oo0loO] = l00OO;
O0lO1[o000lO] = oo0oo;
O0lO1[olol1o] = oOl0;
O0lO1[O1101o] = O1o1O;
O0lO1[olo0lO] = ll1lO;
O0lO1.ool0O1 = o1llO;
O0lO1[oO0oOO] = OOOO1;
O0lO1.o0olO = Ol0l;
O0lO1.lloO = l0l10l;
O0lO1[oO10O1] = OO1l;
O0lO1[lOo000] = O11O1;
O0lO1[llll11] = O10o1o;
O0lO1[oo0l1l] = ll00o;
O0lO1[o0O0o1] = lllO1;
O0lO1[l0100] = l11o1;
O0lO1[O111l0] = Oolo00;
O0lO1[oOooOO] = O0oO1o;
O0lO1[ol000o] = O11O0;
O0lO1[ol1l11] = lll1;
O0lO1[o01olo] = O00l;
O0lO1[O0l01O] = l01l;
O0lO1[lO1l0o] = Ooo0O;
O0lO1[o111l0] = lO10ol;
O0lO1[loOl01] = llooOl;
O0lO1[llOoOo] = OOl0l;
O0lO1[o00o10] = l0Ool;
O0lO1[ll0lol] = lOl1Cls;
O0lO1[lO01oo] = l10o1O;
O0lO1[l0Ol01] = O0O00Cls;
O0lO1[oO0oOl] = oo10O;
O0lO1[OoOl1] = o0lOlCls;
O0lO1[OOooO0] = l1010;
O0lO1[O0l11o] = O0O1O;
O0lO1[Oo1o00] = OO1oo;
O0lO1[o00oll] = lOl1Style;
O0lO1[ol01oO] = Oolo1;
O0lO1[O1Oo0O] = O0O00Style;
O0lO1[lo010O] = OoOo;
O0lO1[o11O1o] = o0lOlStyle;
O0lO1[Ol0lO0] = lll0;
O0lO1[OOO1ll] = O0O0O;
O0lO1[o10l10] = ollO;
O0lO1[lolo1] = lO00O;
O0lO1[OOOol0] = Ol0O0;
O0lO1[oOllOo] = O1lO;
O0lO1[lOlo11] = O1Ol;
O0lO1[ol0Ol1] = Olloll;
ooOl0(oOlO00, "panel");
o1OOO0 = function() {
    o1OOO0[oOOOoO][lllOo0][Ool00](this);
    this[o00lO1]("mini-window");
    this[l0l10O](false);
    this[OOOl1O](this.allowDrag);
    this[o1l0Ol](this[l000l])
};
ol1O(o1OOO0, oOlO00, {
    x: 0,
    y: 0,
    state: "restore",
    o1111o: "mini-window-drag",
    oOll: "mini-window-resize",
    allowDrag: true,
    allowResize: false,
    showCloseButton: true,
    showMaxButton: false,
    showMinButton: false,
    showCollapseButton: false,
    showModal: true,
    minWidth: 150,
    minHeight: 80,
    maxWidth: 2000,
    maxHeight: 2000,
    uiCls: "mini-window",
    containerEl: null
});
lol1Ol = o1OOO0[O0lloO];
lol1Ol[l1OllO] = lOl1O;
lol1Ol[oOllOo] = O10o01;
lol1Ol.ll0ll = o1l0;
lol1Ol.Ol1ll = O1Olo;
lol1Ol.oOoOO1 = OO0Ol;
lol1Ol.OlO00 = O0o01;
lol1Ol.O1O01 = oll00;
lol1Ol.Olol01 = l1ll10;
lol1Ol.o0olO = ol1o0;
lol1Ol.OlO11o = o1lo;
lol1Ol.o1OoO0 = l0llo;
lol1Ol[o10ll1] = ooOO0;
lol1Ol[O1Ol01] = l1ll0;
lol1Ol[Oo10ll] = OoOO1O;
lol1Ol[llolO0] = lOo111;
lol1Ol[lO0ll0] = l0ol0;
lol1Ol[l0o11o] = loo0Oo;
lol1Ol[oo0ll0] = OO001o;
lol1Ol[o111lO] = oloo1;
lol1Ol[o11o1l] = lll01o;
lol1Ol[o1l0Ol] = l0O1o;
lol1Ol[O1Oolo] = Oo0lo;
lol1Ol[OOOl1O] = l0ooO;
lol1Ol[loO1ll] = loll;
lol1Ol[O1Ol10] = l10o0;
lol1Ol[OO0ol1] = O0O001;
lol1Ol[O0llo1] = Ol1oO0;
lol1Ol[oO01O0] = oOo1;
lol1Ol[O1oll0] = lOOlO0;
lol1Ol[oloO00] = l00l1;
lol1Ol[ll001l] = ll0o;
lol1Ol[oo1O00] = OOOol;
lol1Ol[OolOoo] = o1ll1o;
lol1Ol[ll100l] = lO1o;
lol1Ol.lol0o = l0OolO;
lol1Ol[o10l10] = lO0OO;
lol1Ol[OOOol0] = Olol;
lol1Ol.ool0O1 = ollolO;
lol1Ol[lOlo11] = O01o0;
ooOl0(o1OOO0, "window");
mini.MessageBox = {
    alertTitle: "\u63d0\u9192",
    confirmTitle: "\u786e\u8ba4",
    prompTitle: "\u8f93\u5165",
    prompMessage: "\u8bf7\u8f93\u5165\u5185\u5bb9\uff1a",
    buttonText: {
        ok: "\u786e\u5b9a",
        cancel: "\u53d6\u6d88",
        yes: "\u662f",
        no: "\u5426"
    },
    show: function(F) {
        F = mini.copyTo({
            width: "auto",
            height: "auto",
            showModal: true,
            minWidth: 150,
            maxWidth: 800,
            minHeight: 100,
            maxHeight: 350,
            title: "",
            titleIcon: "",
            iconCls: "",
            iconStyle: "",
            message: "",
            html: "",
            spaceStyle: "margin-right:15px",
            showCloseButton: true,
            buttons: null,
            buttonWidth: 58,
            callback: null
        },
        F);
        var I = F.callback,
        C = new o1OOO0();
        C[o11O1o]("overflow:hidden");
        C[OolOoo](F[loOl1l]);
        C[llOoOo](F.title || "");
        C[o111l0](F.titleIcon);
        C[O0l01O](F[ol0Olo]);
        var J = C.uid + "$table",
        N = C.uid + "$content",
        L = "<div class=\"" + F.iconCls + "\" style=\"" + F[lO1110] + "\"></div>",
        Q = "<table class=\"mini-messagebox-table\" id=\"" + J + "\" style=\"\" cellspacing=\"0\" cellpadding=\"0\"><tr><td>" + L + "</td><td id=\"" + N + "\" class=\"mini-messagebox-content-text\">" + (F.message || "") + "</td></tr></table>",
        _ = "<div class=\"mini-messagebox-content\"></div>" + "<div class=\"mini-messagebox-buttons\"></div>";
        C.O0ll1o.innerHTML = _;
        var M = C.O0ll1o.firstChild;
        if (F.html) {
            if (typeof F.html == "string") M.innerHTML = F.html;
            else if (mini.isElement(F.html)) M.appendChild(F.html)
        } else M.innerHTML = Q;
        C._Buttons = [];
        var P = C.O0ll1o.lastChild;
        if (F.buttons && F.buttons.length > 0) {
            for (var H = 0, D = F.buttons.length; H < D; H++) {
                var E = F.buttons[H],
                K = mini.MessageBox.buttonText[E];
                if (!K) K = E;
                var $ = new l0O0O1();
                $[O0loll](K);
                $[lOOo10](F.buttonWidth);
                $[lO0oOo](P);
                $.action = E;
                $[O1oOo1]("click", 
                function(_) {
                    var $ = _.sender;
                    if (I) I($.action);
                    mini.MessageBox[o10ll1](C)
                });
                if (H != D - 1) $[ll1OoO](F.spaceStyle);
                C._Buttons.push($)
            }
        } else P.style.display = "none";
        C[ll001l](F.minWidth);
        C[O1oll0](F.minHeight);
        C[O0llo1](F.maxWidth);
        C[O1Ol10](F.maxHeight);
        C[lOOo10](F.width);
        C[lo0o00](F.height);
        C[O1Ol01]();
        var A = C[ll1OO1]();
        C[lOOo10](A);
        var B = document.getElementById(J);
        if (B) B.style.width = "100%";
        var G = document.getElementById(N);
        if (G) G.style.width = "100%";
        var O = C._Buttons[0];
        if (O) O[OlOoo]();
        else C[OlOoo]();
        C[O1oOo1]("beforebuttonclick", 
        function($) {
            if (I) I("close");
            $.cancel = true;
            mini.MessageBox[o10ll1](C)
        });
        looo(C.el, "keydown", 
        function($) {
            if ($.keyCode == 27) {
                if (I) I("close");
                $.cancel = true;
                mini.MessageBox[o10ll1](C)
            }
        });
        return C.uid
    },
    hide: function(C) {
        if (!C) return;
        var _ = typeof C == "object" ? C: mini.getbyUID(C);
        if (!_) return;
        for (var $ = 0, A = _._Buttons.length; $ < A; $++) {
            var B = _._Buttons[$];
            B[oOllOo]()
        }
        _._Buttons = null;
        _[oOllOo]()
    },
    alert: function(A, _, $) {
        return mini.MessageBox[O1Ol01]({
            minWidth: 250,
            title: _ || mini.MessageBox.alertTitle,
            buttons: ["ok"],
            message: A,
            iconCls: "mini-messagebox-warning",
            callback: $
        })
    },
    confirm: function(A, _, $) {
        return mini.MessageBox[O1Ol01]({
            minWidth: 250,
            title: _ || mini.MessageBox.confirmTitle,
            buttons: ["ok", "cancel"],
            message: A,
            iconCls: "mini-messagebox-question",
            callback: $
        })
    },
    prompt: function(C, B, A, _) {
        var F = "prompt$" + new Date()[llo1l](),
        E = C || mini.MessageBox.promptMessage;
        if (_) E = E + "<br/><textarea id=\"" + F + "\" style=\"width:200px;height:60px;margin-top:3px;\"></textarea>";
        else E = E + "<br/><input id=\"" + F + "\" type=\"text\" style=\"width:200px;margin-top:3px;\"/>";
        var D = mini.MessageBox[O1Ol01]({
            title: B || mini.MessageBox.promptTitle,
            buttons: ["ok", "cancel"],
            width: 250,
            html: "<div style=\"padding:5px;padding-left:10px;\">" + E + "</div>",
            callback: function(_) {
                var $ = document.getElementById(F);
                if (A) A(_, $.value)
            }
        }),
        $ = document.getElementById(F);
        $[OlOoo]();
        return D
    },
    loading: function(_, $) {
        return mini.MessageBox[O1Ol01]({
            minHeight: 50,
            title: $,
            showCloseButton: false,
            message: _,
            iconCls: "mini-messagebox-waiting"
        })
    }
};
mini.alert = mini.MessageBox.alert;
mini.confirm = mini.MessageBox.confirm;
mini.prompt = mini.MessageBox.prompt;
mini[o0oOO0] = mini.MessageBox[o0oOO0];
mini.showMessageBox = mini.MessageBox[O1Ol01];
mini.hideMessageBox = mini.MessageBox[o10ll1];
OO1oOo = function() {
    this.oo1lO0();
    OO1oOo[oOOOoO][lllOo0][Ool00](this)
};
ol1O(OO1oOo, lo0O01, {
    width: 300,
    height: 180,
    vertical: false,
    allowResize: true,
    pane1: null,
    pane2: null,
    showHandleButton: true,
    handlerStyle: "",
    handlerCls: "",
    handlerSize: 5,
    uiCls: "mini-splitter"
});
o1lo0l = OO1oOo[O0lloO];
o1lo0l[l1OllO] = OO1010;
o1lo0l.ll0ll = ll1ol;
o1lo0l.Ol1ll = lO001;
o1lo0l.oOoOO1 = oloOO;
o1lo0l.OO0oO = loo0o;
o1lo0l.o0oOOo = l11l1;
o1lo0l[oO0oOO] = oo1o;
o1lo0l.o0olO = o001o;
o1lo0l.lloO = o1OOo;
o1lo0l[ooollO] = OOOO;
o1lo0l[o1OlO] = oO1ol;
o1lo0l[o11o1l] = olOlo;
o1lo0l[o1l0Ol] = o1lll;
o1lo0l[O0lolo] = ll0Ol;
o1lo0l[o10OOO] = OOo1O;
o1lo0l[Oolo0o] = o11o1;
o1lo0l[O1O1lO] = O01l1o;
o1lo0l[l011Ol] = o11Ol;
o1lo0l[o1o111] = olO00;
o1lo0l[lol110] = OO0ll;
o1lo0l[oOo1oo] = OO101;
o1lo0l[o1Ol01] = Oo1l1;
o1lo0l[oOOllo] = Oll1O;
o1lo0l[lO1o00] = lolO;
o1lo0l[l0llO1] = o1oo;
o1lo0l[oloo11] = l0o1o;
o1lo0l[lO1OOo] = oOloO;
o1lo0l[ll1o1O] = oOloOBox;
o1lo0l[o10l10] = l1lO11;
o1lo0l[lolo1] = OOo11;
o1lo0l.oo1lO0 = lo1oO;
o1lo0l[OOOol0] = loolO;
o1lo0l[lOlo11] = loO11;
ooOl0(OO1oOo, "splitter");
Oo00ol = function() {
    this.regions = [];
    this.regionMap = {};
    Oo00ol[oOOOoO][lllOo0][Ool00](this)
};
ol1O(Oo00ol, lo0O01, {
    regions: [],
    splitSize: 5,
    collapseWidth: 28,
    collapseHeight: 25,
    regionWidth: 150,
    regionHeight: 80,
    regionMinWidth: 50,
    regionMinHeight: 25,
    regionMaxWidth: 2000,
    regionMaxHeight: 2000,
    uiCls: "mini-layout",
    hoverProxyEl: null
});
ll1O0 = Oo00ol[O0lloO];
ll1O0[OOl00] = Ol0O0O;
ll1O0[oO0oOO] = o01o;
ll1O0.olO10o = Ol1000;
ll1O0.o00oO0 = O0OOlo;
ll1O0.oO1l = OOlo0O;
ll1O0.o0olO = l1lll;
ll1O0.lloO = l00lO;
ll1O0.Oooo1l = O0ol;
ll1O0.l1Olo = O10oO;
ll1O0.oOllOO = o0110l;
ll1O0[O00110] = ol0O0;
ll1O0[l001oO] = looll;
ll1O0[o1o011] = lo0lo;
ll1O0[oOoO0O] = l0111;
ll1O0[O1ooO0] = Olo1o;
ll1O0[ol0o00] = l0olOl;
ll1O0[O100O0] = O11000;
ll1O0[oo1lOO] = o00O1;
ll1O0.oOOO10 = OlOl0O;
ll1O0[ll1olo] = lO01l;
ll1O0[lo11OO] = oo1l0;
ll1O0[o0l0lO] = O0l0;
ll1O0[OllO1O] = o0lo1;
ll1O0[ol1Ooo] = O10l0l;
ll1O0.olOO = O10O;
ll1O0.OOlO = lOOOo;
ll1O0.O1oOol = lllol;
ll1O0[llo0O] = l1110;
ll1O0[l0o1lo] = l1110Box;
ll1O0[o1lllO] = l1110ProxyEl;
ll1O0[o01loO] = l1110SplitEl;
ll1O0[lO0l11] = l1110BodyEl;
ll1O0[oloO0l] = l1110HeaderEl;
ll1O0[o1100O] = l1110El;
ll1O0[OOOol0] = OOoOO1;
ll1O0[lOlo11] = oO011;
mini.copyTo(Oo00ol.prototype, {
    Oooo: function(_, A) {
        var C = "<div class=\"mini-tools\">";
        if (A) C += "<span class=\"mini-tools-collapse\"></span>";
        else for (var $ = _.buttons.length - 1; $ >= 0; $--) {
            var B = _.buttons[$];
            C += "<span class=\"" + B.cls + "\" style=\"";
            C += B.style + ";" + (B.visible ? "": "display:none;") + "\">" + B.html + "</span>"
        }
        C += "</div>";
        C += "<div class=\"mini-layout-region-icon " + _.iconCls + "\" style=\"" + _[lO1110] + ";" + ((_[lO1110] || _.iconCls) ? "": "display:none;") + "\"></div>";
        C += "<div class=\"mini-layout-region-title\">" + _.title + "</div>";
        return C
    },
    doUpdate: function() {
        for (var $ = 0, E = this.regions.length; $ < E; $++) {
            var B = this.regions[$],
            _ = B.region,
            A = B._el,
            D = B._split,
            C = B._proxy;
            if (B.cls) lloo10(A, B.cls);
            B._header.style.display = B.showHeader ? "": "none";
            B._header.innerHTML = this.Oooo(B);
            if (B._proxy) B._proxy.innerHTML = this.Oooo(B, true);
            if (D) {
                Oo11(D, "mini-layout-split-nodrag");
                if (B.expanded == false || !B[l000l]) lloo10(D, "mini-layout-split-nodrag")
            }
        }
        this[o10l10]()
    },
    doLayout: function() {
        if (!this[lo1Oll]()) return;
        if (this.l0lo0) return;
        var C = l0ol(this.el, true),
        _ = oO1oo(this.el, true),
        D = {
            x: 0,
            y: 0,
            width: _,
            height: C
        },
        I = this.regions.clone(),
        P = this[llo0O]("center");
        I.remove(P);
        if (P) I.push(P);
        for (var K = 0, H = I.length; K < H; K++) {
            var E = I[K];
            E._Expanded = false;
            Oo11(E._el, "mini-layout-popup");
            var A = E.region,
            L = E._el,
            F = E._split,
            G = E._proxy;
            if (E.visible == false) {
                L.style.display = "none";
                if (A != "center") F.style.display = G.style.display = "none";
                continue
            }
            L.style.display = "";
            if (A != "center") F.style.display = G.style.display = "";
            var R = D.x,
            O = D.y,
            _ = D.width,
            C = D.height,
            B = E.width,
            J = E.height;
            if (!E.expanded) if (A == "west" || A == "east") {
                B = E.collapseSize;
                o100oO(L, E.width)
            } else if (A == "north" || A == "south") {
                J = E.collapseSize;
                oOOo(L, E.height)
            }
            switch (A) {
            case "north":
                C = J;
                D.y += J;
                D.height -= J;
                break;
            case "south":
                C = J;
                O = D.y + D.height - J;
                D.height -= J;
                break;
            case "west":
                _ = B;
                D.x += B;
                D.width -= B;
                break;
            case "east":
                _ = B;
                R = D.x + D.width - B;
                D.width -= B;
                break;
            case "center":
                break;
            default:
                continue
            }
            if (_ < 0) _ = 0;
            if (C < 0) C = 0;
            if (A == "west" || A == "east") oOOo(L, C);
            if (A == "north" || A == "south") o100oO(L, _);
            var N = "left:" + R + "px;top:" + O + "px;",
            $ = L;
            if (!E.expanded) {
                $ = G;
                L.style.top = "-100px";
                L.style.left = "-1500px"
            } else if (G) {
                G.style.left = "-1500px";
                G.style.top = "-100px"
            }
            $.style.left = R + "px";
            $.style.top = O + "px";
            o100oO($, _);
            oOOo($, C);
            var M = jQuery(E._el).height(),
            Q = E.showHeader ? jQuery(E._header).outerHeight() : 0;
            oOOo(E._body, M - Q);
            if (A == "center") continue;
            B = J = E.splitSize;
            R = D.x,
            O = D.y,
            _ = D.width,
            C = D.height;
            switch (A) {
            case "north":
                C = J;
                D.y += J;
                D.height -= J;
                break;
            case "south":
                C = J;
                O = D.y + D.height - J;
                D.height -= J;
                break;
            case "west":
                _ = B;
                D.x += B;
                D.width -= B;
                break;
            case "east":
                _ = B;
                R = D.x + D.width - B;
                D.width -= B;
                break;
            case "center":
                break
            }
            if (_ < 0) _ = 0;
            if (C < 0) C = 0;
            F.style.left = R + "px";
            F.style.top = O + "px";
            o100oO(F, _);
            oOOo(F, C);
            if (E.showSplit && E.expanded && E[l000l] == true) Oo11(F, "mini-layout-split-nodrag");
            else lloo10(F, "mini-layout-split-nodrag");
            F.firstChild.style.display = E.showSplitIcon ? "block": "none";
            if (E.expanded) Oo11(F.firstChild, "mini-layout-spliticon-collapse");
            else lloo10(F.firstChild, "mini-layout-spliticon-collapse")
        }
        mini.layout(this.O00lo);
        this[lOO1lo]("layout")
    },
    o0oOOo: function(B) {
        if (this.l0lo0) return;
        if (OO0O(B.target, "mini-layout-split")) {
            var A = jQuery(B.target).attr("uid");
            if (A != this.uid) return;
            var _ = this[llo0O](B.target.id);
            if (_.expanded == false || !_[l000l]) return;
            this.dragRegion = _;
            var $ = this.OO0oO();
            $.start(B)
        }
    },
    OO0oO: function() {
        if (!this.drag) this.drag = new mini.Drag({
            capture: true,
            onStart: mini.createDelegate(this.oOoOO1, this),
            onMove: mini.createDelegate(this.Ol1ll, this),
            onStop: mini.createDelegate(this.ll0ll, this)
        });
        return this.drag
    },
    oOoOO1: function($) {
        this.OOoOoO = mini.append(document.body, "<div class=\"mini-resizer-mask\"></div>");
        this.lool0O = mini.append(document.body, "<div class=\"mini-proxy\"></div>");
        this.lool0O.style.cursor = "n-resize";
        if (this.dragRegion.region == "west" || this.dragRegion.region == "east") this.lool0O.style.cursor = "w-resize";
        this.splitBox = lolloO(this.dragRegion._split);
        ooo1(this.lool0O, this.splitBox);
        this.elBox = lolloO(this.el, true)
    },
    Ol1ll: function(C) {
        var I = C.now[0] - C.init[0],
        V = this.splitBox.x + I,
        A = C.now[1] - C.init[1],
        U = this.splitBox.y + A,
        K = V + this.splitBox.width,
        T = U + this.splitBox.height,
        G = this[llo0O]("west"),
        L = this[llo0O]("east"),
        F = this[llo0O]("north"),
        D = this[llo0O]("south"),
        H = this[llo0O]("center"),
        O = G && G.visible ? G.width: 0,
        Q = L && L.visible ? L.width: 0,
        R = F && F.visible ? F.height: 0,
        J = D && D.visible ? D.height: 0,
        P = G && G.showSplit ? oO1oo(G._split) : 0,
        $ = L && L.showSplit ? oO1oo(L._split) : 0,
        B = F && F.showSplit ? l0ol(F._split) : 0,
        S = D && D.showSplit ? l0ol(D._split) : 0,
        E = this.dragRegion,
        N = E.region;
        if (N == "west") {
            var M = this.elBox.width - Q - $ - P - H.minWidth;
            if (V - this.elBox.x > M) V = M + this.elBox.x;
            if (V - this.elBox.x < E.minWidth) V = E.minWidth + this.elBox.x;
            if (V - this.elBox.x > E.maxWidth) V = E.maxWidth + this.elBox.x;
            mini.setX(this.lool0O, V)
        } else if (N == "east") {
            M = this.elBox.width - O - P - $ - H.minWidth;
            if (this.elBox.right - (V + this.splitBox.width) > M) V = this.elBox.right - M - this.splitBox.width;
            if (this.elBox.right - (V + this.splitBox.width) < E.minWidth) V = this.elBox.right - E.minWidth - this.splitBox.width;
            if (this.elBox.right - (V + this.splitBox.width) > E.maxWidth) V = this.elBox.right - E.maxWidth - this.splitBox.width;
            mini.setX(this.lool0O, V)
        } else if (N == "north") {
            var _ = this.elBox.height - J - S - B - H.minHeight;
            if (U - this.elBox.y > _) U = _ + this.elBox.y;
            if (U - this.elBox.y < E.minHeight) U = E.minHeight + this.elBox.y;
            if (U - this.elBox.y > E.maxHeight) U = E.maxHeight + this.elBox.y;
            mini.setY(this.lool0O, U)
        } else if (N == "south") {
            _ = this.elBox.height - R - B - S - H.minHeight;
            if (this.elBox.bottom - (U + this.splitBox.height) > _) U = this.elBox.bottom - _ - this.splitBox.height;
            if (this.elBox.bottom - (U + this.splitBox.height) < E.minHeight) U = this.elBox.bottom - E.minHeight - this.splitBox.height;
            if (this.elBox.bottom - (U + this.splitBox.height) > E.maxHeight) U = this.elBox.bottom - E.maxHeight - this.splitBox.height;
            mini.setY(this.lool0O, U)
        }
    },
    ll0ll: function(B) {
        var C = lolloO(this.lool0O),
        D = this.dragRegion,
        A = D.region;
        if (A == "west") {
            var $ = C.x - this.elBox.x;
            this[oo1lOO](D, {
                width: $
            })
        } else if (A == "east") {
            $ = this.elBox.right - C.right;
            this[oo1lOO](D, {
                width: $
            })
        } else if (A == "north") {
            var _ = C.y - this.elBox.y;
            this[oo1lOO](D, {
                height: _
            })
        } else if (A == "south") {
            _ = this.elBox.bottom - C.bottom;
            this[oo1lOO](D, {
                height: _
            })
        }
        jQuery(this.lool0O).remove();
        this.lool0O = null;
        this.elBox = this.handlerBox = null;
        jQuery(this.OOoOoO).remove();
        this.OOoOoO = null
    },
    OO111: function($) {
        $ = this[llo0O]($);
        if ($._Expanded === true) this.O1O10l($);
        else this.l0Ooo($)
    },
    l0Ooo: function(D) {
        if (this.l0lo0) return;
        this[o10l10]();
        var A = D.region,
        H = D._el;
        D._Expanded = true;
        lloo10(H, "mini-layout-popup");
        var E = lolloO(D._proxy),
        B = lolloO(D._el),
        F = {};
        if (A == "east") {
            var K = E.x,
            J = E.y,
            C = E.height;
            oOOo(H, C);
            mini.setX(H, K);
            H.style.top = D._proxy.style.top;
            var I = parseInt(H.style.left);
            F = {
                left: I - B.width
            }
        } else if (A == "west") {
            K = E.right - B.width,
            J = E.y,
            C = E.height;
            oOOo(H, C);
            mini.setX(H, K);
            H.style.top = D._proxy.style.top;
            I = parseInt(H.style.left);
            F = {
                left: I + B.width
            }
        } else if (A == "north") {
            var K = E.x,
            J = E.bottom - B.height,
            _ = E.width;
            o100oO(H, _);
            mini[O1110](H, K, J);
            var $ = parseInt(H.style.top);
            F = {
                top: $ + B.height
            }
        } else if (A == "south") {
            K = E.x,
            J = E.y,
            _ = E.width;
            o100oO(H, _);
            mini[O1110](H, K, J);
            $ = parseInt(H.style.top);
            F = {
                top: $ - B.height
            }
        }
        lloo10(D._proxy, "mini-layout-maxZIndex");
        this.l0lo0 = true;
        var G = this,
        L = jQuery(H);
        L.animate(F, 250, 
        function() {
            Oo11(D._proxy, "mini-layout-maxZIndex");
            G.l0lo0 = false
        })
    },
    O1O10l: function(F) {
        if (this.l0lo0) return;
        F._Expanded = false;
        var B = F.region,
        E = F._el,
        D = lolloO(E),
        _ = {};
        if (B == "east") {
            var C = parseInt(E.style.left);
            _ = {
                left: C + D.width
            }
        } else if (B == "west") {
            C = parseInt(E.style.left);
            _ = {
                left: C - D.width
            }
        } else if (B == "north") {
            var $ = parseInt(E.style.top);
            _ = {
                top: $ - D.height
            }
        } else if (B == "south") {
            $ = parseInt(E.style.top);
            _ = {
                top: $ + D.height
            }
        }
        lloo10(F._proxy, "mini-layout-maxZIndex");
        this.l0lo0 = true;
        var A = this,
        G = jQuery(E);
        G.animate(_, 250, 
        function() {
            Oo11(F._proxy, "mini-layout-maxZIndex");
            A.l0lo0 = false;
            A[o10l10]()
        })
    },
    Ool0l: function(B) {
        if (this.l0lo0) return;
        for (var $ = 0, A = this.regions.length; $ < A; $++) {
            var _ = this.regions[$];
            if (!_._Expanded) continue;
            if (Ol11(_._el, B.target) || Ol11(_._proxy, B.target));
            else this.O1O10l(_)
        }
    },
    getAttrs: function(A) {
        var H = Oo00ol[oOOOoO][l1OllO][Ool00](this, A),
        G = jQuery(A),
        E = parseInt(G.attr("splitSize"));
        if (!isNaN(E)) H.splitSize = E;
        var F = [],
        D = mini[o01O00](A);
        for (var _ = 0, C = D.length; _ < C; _++) {
            var B = D[_],
            $ = {};
            F.push($);
            $.cls = B.className;
            $.style = B.style.cssText;
            mini[oooo0l](B, $, ["region", "title", "iconCls", "iconStyle", "cls", "headerCls", "headerStyle", "bodyCls", "bodyStyle"]);
            mini[o100](B, $, ["allowResize", "visible", "showCloseButton", "showCollapseButton", "showSplit", "showHeader", "expanded", "showSplitIcon"]);
            mini[l000oo](B, $, ["splitSize", "collapseSize", "width", "height", "minWidth", "minHeight", "maxWidth", "maxHeight"]);
            $.bodyParent = B
        }
        H.regions = F;
        return H
    }
});
ooOl0(Oo00ol, "layout");
o1OO1l = function() {
    o1OO1l[oOOOoO][lllOo0][Ool00](this)
};
ol1O(o1OO1l, mini.Container, {
    style: "",
    borderStyle: "",
    bodyStyle: "",
    uiCls: "mini-box"
});
oOol10 = o1OO1l[O0lloO];
oOol10[l1OllO] = O0o0o;
oOol10[o11O1o] = O0Oooo;
oOol10[ll0lo1] = OlO00l;
oOol10[olll1O] = lO0oO;
oOol10[o10l10] = o0ol0;
oOol10[OOOol0] = l1OooO;
oOol10[lOlo11] = O1l0o;
ooOl0(o1OO1l, "box");
O1o00O = function() {
    O1o00O[oOOOoO][lllOo0][Ool00](this)
};
ol1O(O1o00O, lo0O01, {
    url: "",
    uiCls: "mini-include"
});
l00O = O1o00O[O0lloO];
l00O[l1OllO] = oolO;
l00O[OOO10o] = lo01l;
l00O[o0oO0l] = lo00o;
l00O[o10l10] = Oll1o;
l00O[OOOol0] = OlOlo;
l00O[lOlo11] = oOlo1;
ooOl0(O1o00O, "include");
o0OlOl = function() {
    this.oOOOl();
    o0OlOl[oOOOoO][lllOo0][Ool00](this)
};
ol1O(o0OlOl, lo0O01, {
    activeIndex: -1,
    tabAlign: "left",
    tabPosition: "top",
    showBody: true,
    nameField: "id",
    titleField: "title",
    urlField: "url",
    url: "",
    maskOnLoad: true,
    bodyStyle: "",
    oOOO1: "mini-tab-hover",
    o0OO0: "mini-tab-active",
    uiCls: "mini-tabs",
    lllO0O: 1,
    loOlo: 180,
    hoverTab: null
});
ll1l1l = o0OlOl[O0lloO];
ll1l1l[l1OllO] = l0Ol;
ll1l1l[lOO0Oo] = ol1lO;
ll1l1l[o0lo11] = oOOO1l;
ll1l1l[O001lO] = l1O1ol;
ll1l1l.o1oo0 = oll1;
ll1l1l.oo1olo = lO0Ool;
ll1l1l.olo11 = oo10;
ll1l1l.oOl1 = loOO0;
ll1l1l.o01110 = o110OO;
ll1l1l.OoO0O0 = Oo00l;
ll1l1l.o0oOOo = oO1l10;
ll1l1l.olO10o = l1l0o;
ll1l1l.o00oO0 = O010Oo;
ll1l1l.lloO = OlOl;
ll1l1l.O0OOl = l001l;
ll1l1l[OO01OO] = Ol1O0O;
ll1l1l[ool1Ol] = o0OO10;
ll1l1l[OoOolO] = O1Oo;
ll1l1l[lo010O] = lO1O01;
ll1l1l[o11O1o] = o11OOo;
ll1l1l[O0lOo1] = O0loo;
ll1l1l[l0l10o] = o00o;
ll1l1l.oOOO = O1Oll1;
ll1l1l[l1lOl] = oO0l;
ll1l1l[OOllo0] = lOOo;
ll1l1l[OooOol] = lOOOl;
ll1l1l[l1lOl] = oO0l;
ll1l1l[O1o1oo] = lOoO1o;
ll1l1l.lolOl0 = o11o;
ll1l1l.O11l01 = O0lll;
ll1l1l.OllOO0 = l1oo0;
ll1l1l[l0OoO1] = l1OO0O;
ll1l1l[o1Oolo] = ool1l;
ll1l1l[lOo0Oo] = oO1OOl;
ll1l1l[lo0111] = ollO0l;
ll1l1l[lollll] = ooO10;
ll1l1l[l0O1lO] = ol1oO;
ll1l1l[ooo011] = lll0Oo;
ll1l1l[lol0l0] = o010O;
ll1l1l[o10l10] = l0OoO0;
ll1l1l[o1lOlO] = l01OO;
ll1l1l[lolo1] = llloO;
ll1l1l[lOlO10] = ol1oORows;
ll1l1l[oOlo11] = O10llO;
ll1l1l[l00lOl] = l10OOO;
ll1l1l.Ol01 = l0Ooo0;
ll1l1l.ollol = lO0lo;
ll1l1l[O0oOlo] = oOo1o;
ll1l1l.ol0OlO = o0OOl;
ll1l1l.lOlOo1 = o00l;
ll1l1l[loOl0O] = O001O;
ll1l1l[OoO0oo] = O1oll;
ll1l1l[ol11ol] = oO00oo;
ll1l1l[l101o1] = oo1oOO;
ll1l1l[oo00oO] = ooOO0o;
ll1l1l[lollol] = ol1oOs;
ll1l1l[oOO0l0] = OO0l;
ll1l1l[O0ooOO] = ll01;
ll1l1l[Ol1lo1] = o0o0;
ll1l1l[lllO0] = OllO0l;
ll1l1l[l11l00] = OOOO01;
ll1l1l[O0ll0O] = O0ol00;
ll1l1l[looO1l] = OOOl0;
ll1l1l[l1Ol1l] = o0loo;
ll1l1l[OOO10o] = l0llOo;
ll1l1l[o0oO0l] = oo0l;
ll1l1l[o01o1] = o00Ol;
ll1l1l.lOol01 = l00Oo0;
ll1l1l[o0o0oO] = o010o;
ll1l1l.oOOOl = olO0OO;
ll1l1l[OOOol0] = ooo1o1;
ll1l1l.oo011o = oool1O;
ll1l1l[lOlo11] = ol0Oo;
ll1l1l[ol0Ol1] = Ol00o;
ooOl0(o0OlOl, "tabs");
l101oo = function() {
    this.items = [];
    l101oo[oOOOoO][lllOo0][Ool00](this)
};
ol1O(l101oo, lo0O01);
mini.copyTo(l101oo.prototype, o11Ool_prototype);
var o11Ool_prototype_hide = o11Ool_prototype[o10ll1];
mini.copyTo(l101oo.prototype, {
    width: 140,
    vertical: true,
    allowSelectItem: false,
    O10oo1: null,
    _l0110: "mini-menuitem-selected",
    textField: "text",
    resultAsTree: false,
    idField: "id",
    parentField: "pid",
    itemsField: "children",
    _clearBorder: false,
    showAction: "none",
    hideAction: "outerclick",
    uiCls: "mini-menu",
    _disableContextMenu: false,
    url: "",
    hideOnClick: true
});
OO10 = l101oo[O0lloO];
OO10[l1OllO] = o1l1o0;
OO10[o01OOO] = Ol1o1;
OO10[loOOo0] = O00oo1;
OO10[o1OlO1] = lo00;
OO10[oO0oo0] = o00o0o;
OO10[oOo1l0] = oO1lOO;
OO10[l1lllO] = l0oo0l;
OO10[lo0O0l] = Ollll;
OO10[OOO10o] = OllolO;
OO10[o0oO0l] = OOoolO;
OO10[o01o1] = Oo1l;
OO10[ooo1O0] = Oo1lList;
OO10.lOol01 = llll;
OO10[o01O1l] = oOO0l;
OO10[l0l1oo] = llOlO;
OO10[olOoo] = lol00;
OO10[O0ll11] = lll1ll;
OO10[OOloO] = olO1O;
OO10[l1Oo01] = oOooO;
OO10[oll1lo] = lO11;
OO10[O1lloo] = Oooo1O;
OO10[l01lo1] = Ool0o1;
OO10[lO1111] = Ol1oo0;
OO10[o1OOl0] = l0olO1;
OO10[OO1lO1] = ll0Oo;
OO10[o001ol] = l01lO;
OO10[oOlOO1] = lOOl00;
OO10[oo00oO] = OO11l;
OO10[OOo1Oo] = OO0o0;
OO10[Oo0lol] = O0Oo1l;
OO10[oo1OOl] = OOl1o;
OO10[l1o000] = l01lOs;
OO10[oOO1ll] = l1011;
OO10[OlOO1l] = O0l00;
OO10[l1OlOo] = looo01;
OO10[llOo0O] = ll0l01;
OO10[ll0olo] = Ooo11;
OO10[oOl0l1] = lO111o;
OO10[o10ll1] = o0Ool;
OO10[O1Ol01] = l00OoO;
OO10[l11l1l] = oll0o;
OO10[o1o111] = l0OlO;
OO10[lol110] = loolo;
OO10.oOlO1l = o11001;
OO10[o1O0O0] = ll1Ol1;
OO10[OOOol0] = l0lO;
OO10[oOllOo] = lol01o;
OO10[lOlo11] = l1O01;
OO10[ol0Ol1] = OlOo;
OO10[l1olO0] = Oo0ol;
ooOl0(l101oo, "menu");
l101ooBar = function() {
    l101ooBar[oOOOoO][lllOo0][Ool00](this)
};
ol1O(l101ooBar, l101oo, {
    uiCls: "mini-menubar",
    vertical: false,
    setVertical: function($) {
        this.vertical = false
    }
});
ooOl0(l101ooBar, "menubar");
mini.ContextMenu = function() {
    mini.ContextMenu[oOOOoO][lllOo0][Ool00](this)
};
ol1O(mini.ContextMenu, l101oo, {
    uiCls: "mini-contextmenu",
    vertical: true,
    visible: false,
    _disableContextMenu: true,
    setVertical: function($) {
        this.vertical = true
    }
});
ooOl0(mini.ContextMenu, "contextmenu");
Ool101 = function() {
    Ool101[oOOOoO][lllOo0][Ool00](this)
};
ol1O(Ool101, lo0O01, {
    text: "",
    iconCls: "",
    iconStyle: "",
    iconPosition: "left",
    showIcon: true,
    showAllow: true,
    checked: false,
    checkOnClick: false,
    groupName: "",
    _hoverCls: "mini-menuitem-hover",
    ll01O: "mini-menuitem-pressed",
    lOOo1l: "mini-menuitem-checked",
    _clearBorder: false,
    menu: null,
    uiCls: "mini-menuitem",
    Olo0lo: false
});
Ol01l = Ool101[O0lloO];
Ol01l[l1OllO] = Olo10;
Ol01l[oOlolO] = l111O;
Ol01l[oOlO0l] = o1oOl;
Ol01l.olO10o = l0o0o;
Ol01l.o00oO0 = Ollo10;
Ol01l.O10O1l = OOO0;
Ol01l.lloO = ll0oo;
Ol01l[ol0l0o] = lOlOo;
Ol01l.llo1 = olll0;
Ol01l[o10ll1] = oloOoO;
Ol01l[l0ol1o] = oloOoOMenu;
Ol01l[lOO110] = l01OOl;
Ol01l[lo1o10] = oo01l;
Ol01l[Ool10] = O00Ol;
Ol01l[oo11o1] = loO10;
Ol01l[O1l1o1] = Ol11l;
Ol01l[OO0lll] = Ollll1;
Ol01l[o0lO1o] = OOoO1;
Ol01l[lO0ol0] = o100o;
Ol01l[oOO0l1] = Oll0ll;
Ol01l[lO1Ool] = O1lol;
Ol01l[l0ool1] = oO0O;
Ol01l[Olo110] = Oo0O1;
Ol01l[llOOo0] = ooO0O;
Ol01l[olo0o0] = O10OO;
Ol01l[lO1l0o] = O1ooO;
Ol01l[o111l0] = lOlO1;
Ol01l[ooo1oo] = OoOol;
Ol01l[O0loll] = llloo;
Ol01l[lolo1] = lO1Ol;
Ol01l[o0ooo0] = lOlol;
Ol01l[o1O0O0] = O0oOl;
Ol01l[oOllOo] = oO1Ol;
Ol01l.OOOlO0 = OlO0O;
Ol01l[OOOol0] = O00oo;
Ol01l[lOlo11] = l00ol;
ooOl0(Ool101, "menuitem");
l0l1o0 = function() {
    this.l00o0l();
    l0l1o0[oOOOoO][lllOo0][Ool00](this)
};
ol1O(l0l1o0, lo0O01, {
    width: 180,
    expandOnLoad: true,
    activeIndex: -1,
    autoCollapse: false,
    groupCls: "",
    groupStyle: "",
    groupHeaderCls: "",
    groupHeaderStyle: "",
    groupBodyCls: "",
    groupBodyStyle: "",
    groupHoverCls: "",
    groupActiveCls: "",
    allowAnim: true,
    uiCls: "mini-outlookbar",
    _GroupId: 1
});
Oo0lO = l0l1o0[O0lloO];
Oo0lO[l1OllO] = o1o0l;
Oo0lO[Ooo11o] = OlO01;
Oo0lO.lloO = ll11o;
Oo0lO.Oo01 = o000O;
Oo0lO.Ol1ol = ool00;
Oo0lO[lO0ooo] = lO0O0;
Oo0lO[o111Oo] = O0ol0;
Oo0lO[oOO1l1] = Ol0l1;
Oo0lO[loO10l] = o0l11;
Oo0lO[l11lll] = oO0OO;
Oo0lO[l111o0] = lOO0l;
Oo0lO[l1lOl] = o0o0O;
Oo0lO[O1o1oo] = o11ol;
Oo0lO[o01Ol] = l11Ol;
Oo0lO[OO1Oll] = ll1l1;
Oo0lO[oOOll] = Ol0oO;
Oo0lO[Oo0l0o] = lOl0O;
Oo0lO[O0OO10] = o0l01;
Oo0lO[oO0OOl] = l0o01;
Oo0lO.ll10 = l1o0O;
Oo0lO[O0O01o] = lOOl1;
Oo0lO.o1oO1 = OOOl0l;
Oo0lO.l1l1o = l0000;
Oo0lO[o10l10] = o0O0l;
Oo0lO[lolo1] = lo000;
Oo0lO[lOoO] = l00o0;
Oo0lO[oo00oO] = Ol1oO;
Oo0lO[ll1o10] = ol0O1;
Oo0lO[l0ool] = O0O11;
Oo0lO[O00o10] = OOlOO;
Oo0lO[ool1oo] = lOOl1s;
Oo0lO[oooO00] = oool1;
Oo0lO[OoO1ll] = o11l1;
Oo0lO.O010 = oO1O1;
Oo0lO.l00o0l = olO0l;
Oo0lO.Ol10O1 = lo10l;
Oo0lO[OOOol0] = olo0o;
Oo0lO[lOlo11] = o0Oll;
Oo0lO[ol0Ol1] = l1O0l;
ooOl0(l0l1o0, "outlookbar");
l010OO = function() {
    l010OO[oOOOoO][lllOo0][Ool00](this);
    this.data = []
};
ol1O(l010OO, l0l1o0, {
    url: "",
    textField: "text",
    iconField: "iconCls",
    urlField: "url",
    resultAsTree: false,
    itemsField: "children",
    idField: "id",
    parentField: "pid",
    style: "width:100%;height:100%;",
    uiCls: "mini-outlookmenu",
    Oo0l1o: null,
    autoCollapse: true,
    activeIndex: 0
});
ol011 = l010OO[O0lloO];
ol011.loO011 = l0lO0;
ol011.oOll0 = lloO0;
ol011[lOlllO] = o11OO;
ol011[l1OllO] = lll0l;
ol011[ll0OOO] = ll1l0;
ol011[Ol0ooo] = l1O1o;
ol011[oOolO0] = Oloo11;
ol011[llllOo] = loOO1;
ol011[o01O1l] = Oollo;
ol011[l0l1oo] = ooll0;
ol011[olOoo] = l00lo;
ol011[O0ll11] = l11oo;
ol011[o0o00l] = l1O1osField;
ol011[oOolO1] = Ol0Ol;
ol011[OOloO] = OOo00;
ol011[l1Oo01] = ll01o;
ol011[Ol1lo1] = lll00;
ol011[lllO0] = o0110;
ol011[l11Ooo] = olOl0;
ol011[o0o110] = lo0oo;
ol011[oll1lo] = oll1o;
ol011[O1lloo] = O0oll0;
ol011[OOO10o] = loOl0;
ol011[o0oO0l] = OO1lo;
ol011[o01o1] = l0Olo;
ol011[ooo1O0] = l0OloList;
ol011.lOol01 = OOl1O;
ol011.O110oOFields = lllOo;
ol011[oOllOo] = oOoOO;
ol011[ol0Ol1] = o111O;
ooOl0(l010OO, "outlookmenu");
o01lOl = function() {
    o01lOl[oOOOoO][lllOo0][Ool00](this);
    this.data = []
};
ol1O(o01lOl, l0l1o0, {
    url: "",
    textField: "text",
    iconField: "iconCls",
    urlField: "url",
    resultAsTree: false,
    nodesField: "children",
    idField: "id",
    parentField: "pid",
    style: "width:100%;height:100%;",
    uiCls: "mini-outlooktree",
    Oo0l1o: null,
    expandOnLoad: false,
    autoCollapse: true,
    activeIndex: 0
});
l010l = o01lOl[O0lloO];
l010l.oo01O = lO1ol;
l010l.oOoO0o = l1ooo;
l010l[OOoO0] = o1o00;
l010l[OOoO1o] = ololl;
l010l[l1OllO] = OOol0;
l010l[o01Ol] = lolOo;
l010l[OO1Oll] = Oo0l1;
l010l[l1loOo] = O0ll1;
l010l[Ol0ooo] = o1oll;
l010l[O010oO] = lolOO;
l010l[oOolO0] = l1lO0;
l010l[llllOo] = l010o;
l010l[o01O1l] = Olo1O;
l010l[l0l1oo] = O1l1o;
l010l[olOoo] = l1100;
l010l[O0ll11] = OlOl1;
l010l[o0o00l] = o1ollsField;
l010l[oOolO1] = looo0;
l010l[OOloO] = oOlO1;
l010l[l1Oo01] = lO11O;
l010l[Ol1lo1] = O1lll;
l010l[lllO0] = ll010;
l010l[l11Ooo] = OO1l0;
l010l[o0o110] = o10o1;
l010l[oll1lo] = O1llo;
l010l[O1lloo] = l1OO1;
l010l[OOO10o] = l10O0;
l010l[o0oO0l] = O1011;
l010l[o01o1] = oo0o0;
l010l[ooo1O0] = oo0o0List;
l010l.lOol01 = Oo1Ol;
l010l.O110oOFields = l0lOl;
l010l[oOllOo] = lOl0o;
l010l[ol0Ol1] = oOl1o;
ooOl0(o01lOl, "outlooktree");
mini.NavBar = function() {
    mini.NavBar[oOOOoO][lllOo0][Ool00](this)
};
ol1O(mini.NavBar, l0l1o0, {
    uiCls: "mini-navbar"
});
ooOl0(mini.NavBar, "navbar");
mini.NavBarMenu = function() {
    mini.NavBarMenu[oOOOoO][lllOo0][Ool00](this)
};
ol1O(mini.NavBarMenu, l010OO, {
    uiCls: "mini-navbarmenu"
});
ooOl0(mini.NavBarMenu, "navbarmenu");
mini.NavBarTree = function() {
    mini.NavBarTree[oOOOoO][lllOo0][Ool00](this)
};
ol1O(mini.NavBarTree, o01lOl, {
    uiCls: "mini-navbartree"
});
ooOl0(mini.NavBarTree, "navbartree");
mini.ToolBar = function() {
    mini.ToolBar[oOOOoO][lllOo0][Ool00](this)
};
ol1O(mini.ToolBar, mini.Container, {
    _clearBorder: false,
    style: "",
    uiCls: "mini-toolbar",
    _create: function() {
        this.el = document.createElement("div");
        this.el.className = "mini-toolbar"
    },
    _initEvents: function() {},
    doLayout: function() {
        if (!this[lo1Oll]()) return;
        var A = mini[o01O00](this.el, true);
        for (var $ = 0, _ = A.length; $ < _; $++) mini.layout(A[$])
    },
    set_bodyParent: function($) {
        if (!$) return;
        this.el = $;
        this[o10l10]()
    },
    getAttrs: function($) {
        var _ = {};
        mini[oooo0l]($, _, ["id", "borderStyle"]);
        this.el = $;
        this.el.uid = this.uid;
        this[o00lO1](this.uiCls);
        return _
    }
});
ooOl0(mini.ToolBar, "toolbar");
o110ol = function($) {
    this._ajaxOption = {
        async: false,
        type: "get"
    };
    this.root = {
        _id: -1,
        _pid: "",
        _level: -1
    };
    this.data = this.root[this.nodesField] = [];
    this.O0lo01 = {};
    this.Ol101 = {};
    this._viewNodes = null;
    o110ol[oOOOoO][lllOo0][Ool00](this, $);
    this[O1oOo1]("beforeexpand", 
    function(B) {
        var $ = B.node,
        A = this[llo0l1]($),
        _ = $[this.nodesField];
        if (!A && (!_ || _.length == 0)) if (this.loadOnExpand) {
            B.cancel = true;
            this[oO0100]($)
        }
    },
    this);
    this[lolo1]()
};
o110ol.NodeUID = 1;
var lastNodeLevel = [];
ol1O(o110ol, lo0O01, {
    isTree: true,
    O011: "block",
    loadOnExpand: true,
    removeOnCollapse: true,
    expandOnDblClick: true,
    expandOnNodeClick: false,
    value: "",
    l01l1o: null,
    allowSelect: true,
    showCheckBox: false,
    showFolderCheckBox: true,
    showExpandButtons: true,
    enableHotTrack: true,
    showArrow: false,
    expandOnLoad: false,
    delimiter: ",",
    url: "",
    root: null,
    resultAsTree: true,
    parentField: "pid",
    idField: "id",
    textField: "text",
    iconField: "iconCls",
    nodesField: "children",
    showTreeIcon: false,
    showTreeLines: true,
    checkRecursive: false,
    allowAnim: true,
    o0l10: "mini-tree-checkbox",
    lo1l: "mini-tree-selectedNode",
    l0000O: "mini-tree-node-hover",
    leafIcon: "mini-tree-leaf",
    folderIcon: "mini-tree-folder",
    OOll11: "mini-tree-border",
    OO11O: "mini-tree-header",
    O000OO: "mini-tree-body",
    loll0: "mini-tree-node",
    O1l1l: "mini-tree-nodes",
    llll1o: "mini-tree-expand",
    olol11: "mini-tree-collapse",
    o1l0OO: "mini-tree-node-ecicon",
    o1l001: "mini-tree-nodeshow",
    uiCls: "mini-tree",
    _ajaxOption: {
        async: false,
        type: "get"
    },
    _allowExpandLayout: true,
    autoCheckParent: false,
    allowDrag: false,
    allowDrop: false,
    dragGroupName: "",
    dropGroupName: ""
});
Oo10o = o110ol[O0lloO];
Oo10o[l1OllO] = lOOo01;
Oo10o.lol01 = OlOll;
Oo10o.o1OlOo = lOl11;
Oo10o.oOoOO1 = OlOO1;
Oo10o[l00lOO] = olO1l;
Oo10o[Oll1lo] = o11oo;
Oo10o[OloOo1] = loOo0;
Oo10o[O1Ol1o] = ll1ll;
Oo10o[loollo] = o0llO;
Oo10o[l1Oool] = Olo00;
Oo10o[oooOoO] = Ol1l0;
Oo10o[O1Oolo] = lool0;
Oo10o[OOOl1O] = llOOl;
Oo10o.OO0oOText = Ollol;
Oo10o.OO0oOData = Ooo0l;
Oo10o[O0Oo0O] = OO0o11;
Oo10o[Oo0l11] = O01l;
Oo10o[olO011] = O10ll;
Oo10o[o1lo1l] = Oooooo;
Oo10o[lol1lo] = lOO1;
Oo10o[ol00l1] = loo1ol;
Oo10o[O0ooO0] = o1OO0o;
Oo10o[OO11ol] = l1ol;
Oo10o[o0oO0O] = oO0lOO;
Oo10o[ool1l0] = lool;
Oo10o[o00l0o] = O10lO0;
Oo10o[oooOo0] = l0ol1;
Oo10o[lO1o0l] = OOO1Ol;
Oo10o[l1l1ol] = o111o0;
Oo10o.lo1o1 = l100;
Oo10o[oOo1Ol] = l1Ol0;
Oo10o[OloOlo] = lO0ll;
Oo10o[l1O0Oo] = oO11o;
Oo10o[l00O0o] = o1lOO;
Oo10o[Ol0Oll] = l0o00;
Oo10o.olO10o = ll1oO;
Oo10o.OloOO = OOolo;
Oo10o[O0011O] = looloo;
Oo10o[ll0OoO] = l10oo;
Oo10o.o0oOOo = o000O0;
Oo10o.lloO = o100OO;
Oo10o.O001oO = lO00;
Oo10o[olo1OO] = ol01l;
Oo10o[O0llo0] = ollo0;
Oo10o[O010l0] = oll11;
Oo10o[l1Oo10] = O1Ol0;
Oo10o[OoO110] = l0olol;
Oo10o[lOOlo1] = lOooOO;
Oo10o[l00110] = oool;
Oo10o[lO1Olo] = l000o;
Oo10o[OOlOoo] = lOoo1;
Oo10o[llOl0o] = O1o0ol;
Oo10o[loo00O] = O1lO0;
Oo10o[OlOl0l] = o1lOl1;
Oo10o[l100O0] = lo1O1l;
Oo10o[lO0oO0] = lO1oOl;
Oo10o[o0o00l] = OOolO;
Oo10o[oOolO1] = o0OoOo;
Oo10o[l11Ooo] = O0Oll;
Oo10o[o0o110] = oOOo1;
Oo10o[O10lOo] = lol1;
Oo10o[loloO0] = looOl1;
Oo10o[l0o01o] = O1O1;
Oo10o[o0l00o] = loo1;
Oo10o[oll1lo] = olooo;
Oo10o[O1lloo] = ol111o;
Oo10o[olOoo] = l111o;
Oo10o[O0ll11] = Oool0;
Oo10o[o01O1l] = l0lol0;
Oo10o[l0l1oo] = oOo10;
Oo10o[OOloO] = l11lo0;
Oo10o[l1Oo01] = o1o1O;
Oo10o[o0Oll0] = Oo111;
Oo10o.O0OOo = Oo111AndText;
Oo10o[llollo] = lOo1;
Oo10o[o101l] = llo10;
Oo10o[loOoOo] = l11O;
Oo10o[oOOOo1] = o0lo0;
Oo10o[O1Oooo] = ol1o0O;
Oo10o[l01l0O] = ll1OO;
Oo10o[l001l1] = O00oOO;
Oo10o[O0o0l1] = OO011o;
Oo10o[O0o0OO] = ll1Oo;
Oo10o[OOll10] = oOO0;
Oo10o[o010Oo] = llo1O;
Oo10o[O1lOl1] = oo1ll;
Oo10o[Ol1110] = olll1;
Oo10o[Ool0O0] = OoOl;
Oo10o[olO00l] = O10o0;
Oo10o[ol1oll] = lo1OO;
Oo10o[OOO0O] = lO1O0;
Oo10o[oOolO0] = oo0oO;
Oo10o[oO000O] = OO1O;
Oo10o[O010oO] = o100l;
Oo10o[OOl0o0] = o10O1;
Oo10o[ollOo] = ol0110;
Oo10o[lo1Ol0] = o01lO1;
Oo10o[loOo1o] = ll1O;
Oo10o[OlOllo] = OO1o0;
Oo10o[l0ll11] = O1oooo;
Oo10o[l1OO1O] = OoOll;
Oo10o[O0o1ll] = oo0o1;
Oo10o[o01ll1] = o0oolO;
Oo10o[l0001l] = l00O1;
Oo10o[o11o00] = OO0o;
Oo10o[Ol0ooo] = o11lO;
Oo10o[OOO1O1] = lo0l0l;
Oo10o.Oo1o = l11l0O;
Oo10o.l0010 = o1Oll;
Oo10o.lOoo = ol00O1;
Oo10o.ll01l = O0O1ll;
Oo10o[o1llo1] = ll1l;
Oo10o[OoOo11] = o11lOBox;
Oo10o[O10oll] = oOoOl;
Oo10o[loooo] = OOl010;
Oo10o.lo0l1 = Ol0O;
Oo10o.Oo10 = oo1ooO;
Oo10o.olo1 = olOOl;
Oo10o[OoOO0l] = ol0O;
Oo10o.O001o = OO0O0;
Oo10o.oO0o = OO0Oo;
Oo10o[Ololo1] = O0ll0;
Oo10o[Olo010] = o1ol;
Oo10o[l0oOo1] = OO0O1;
Oo10o[lo0oo0] = o0o01;
Oo10o[lo0o0l] = o0o01s;
Oo10o[O10O10] = ooOlO1;
Oo10o[l10l11] = ooOlO1s;
Oo10o[O11Oo0] = lo0o;
Oo10o[l000Oo] = O10lo;
Oo10o[O0001O] = OOO01;
Oo10o[oll0O0] = Ol0l0;
Oo10o.o1oo1o = lolll;
Oo10o[ll10O0] = lo0os;
Oo10o.oOo0lo = ooOoOo;
Oo10o.Oll1OO = lOl0O1;
Oo10o[ll0Olo] = llO1;
Oo10o[lllolO] = O00o01;
Oo10o[lo1Ooo] = oO11lo;
Oo10o[o000oo] = oO0O1;
Oo10o[oO01oO] = l11o;
Oo10o[O1O00] = o1ll00;
Oo10o[l1o0oo] = olO1;
Oo10o[OOoOOO] = oooO0O;
Oo10o[l0o1Oo] = lllo1;
Oo10o[llo0oo] = OlOoO;
Oo10o[O010O0] = Ooo0o;
Oo10o[llo0l1] = o0ll1;
Oo10o[lo0Olo] = oooooo;
Oo10o[O1l00o] = l1ooO;
Oo10o[ooO0Ol] = l0OOO;
Oo10o[looo1l] = o1oO1l;
Oo10o[OO1000] = o1ol0;
Oo10o[o01O00] = l111lo;
Oo10o[lO0ool] = lO0o1;
Oo10o[ol11oo] = l0oo1;
Oo10o[o0O1o0] = olOO10;
Oo10o[lO0lo0] = ooOoo;
Oo10o[l10l00] = ll11Oo;
Oo10o[OoOll0] = ol0011;
Oo10o[o0lO0l] = OlooO;
Oo10o[oooO1o] = o0Olo;
Oo10o[Oo011l] = o11lOIcon;
Oo10o[O1OoO1] = lOl11l;
Oo10o[lOO0oO] = O1O11;
Oo10o[o01Ol] = lO0Olo;
Oo10o[OO1Oll] = o1o1o;
Oo10o[OoOO0o] = OolOo;
Oo10o[O1oO0o] = l11ol0;
Oo10o[lo00o1] = olOO1;
Oo10o[oOlO10] = olO0o;
Oo10o[o0llol] = OOOl;
Oo10o[ol011o] = Ooll0;
Oo10o[Ooll0O] = llllO;
Oo10o[o001OO] = o0100;
Oo10o[O0lOl] = ooOll;
Oo10o[oOOO0O] = O111ll;
Oo10o[o1OooO] = ol00O;
Oo10o[O0oOo0] = l010Ol;
Oo10o[lO10o1] = oOo0Ol;
Oo10o[ol1l00] = Ol001;
Oo10o[o10l10] = o1OO;
Oo10o.l1l110 = oO0ll;
Oo10o.Ooooll = O1110o;
Oo10o[lolo1] = llo0o;
Oo10o.ooO1lO = lo1ll;
Oo10o.lOlo0 = O11l;
Oo10o.OOo0lo = O11lTitle;
Oo10o.O11lo = lOOO0;
Oo10o[Oo0111] = ooO00l;
Oo10o[o10oOo] = O0ool;
Oo10o.lOol01 = olO0;
Oo10o[l1looO] = lolOOO;
Oo10o[Ol11ol] = OOlll;
Oo10o[oO0100] = oOo1OO;
Oo10o[OOO10o] = lOOoo;
Oo10o[o0oO0l] = l1l1O;
Oo10o[o1OOlO] = Ollo1;
Oo10o[oo111O] = Ol011O;
Oo10o[ooo1O0] = o01O;
Oo10o[O1O111] = lOlo0o;
Oo10o[lo100l] = o10Ol1;
Oo10o[llOo1l] = ll1l0l;
Oo10o[OlOO1l] = lOoO1;
Oo10o[l1OlOo] = ll0o0;
Oo10o[o01o1] = llOOll;
Oo10o[OOOol0] = o0OOO;
Oo10o[lOlo11] = lo1lO;
Oo10o[ol0Ol1] = oOll1;
ooOl0(o110ol, "tree");
O0ll = function($) {
    this.owner = $;
    this.owner[O1oOo1]("NodeMouseDown", this.o1o1, this)
};
O0ll[O0lloO] = {
    o1o1: function(B) {
        var A = B.node;
        if (B.htmlEvent.button == mini.MouseButton.Right) return;
        var _ = this.owner;
        if (_[l0O0Oo]() || _[l00lOO](B.node) == false) return;
        if (_[l0oOo1](A)) return;
        this.dragData = _.OO0oOData();
        if (this.dragData[looo1l](A) == -1) this.dragData.push(A);
        var $ = this.OO0oO();
        $.start(B.htmlEvent)
    },
    oOoOO1: function($) {
        var _ = this.owner;
        this.feedbackEl = mini.append(document.body, "<div class=\"mini-feedback\"></div>");
        this.feedbackEl.innerHTML = _.OO0oOText(this.dragData);
        this.lastFeedbackClass = "";
        this[ooO0l] = _[ooO0l];
        _[O1oO0o](false)
    },
    _getDropTree: function(_) {
        var $ = OO0O(_.target, "mini-tree", 500);
        if ($) return mini.get($)
    },
    Ol1ll: function(_) {
        var B = this.owner,
        A = this._getDropTree(_.event),
        E = _.now[0],
        C = _.now[1];
        mini[O1110](this.feedbackEl, E + 15, C + 18);
        this.dragAction = "no";
        if (A) {
            var $ = A[OoOO0l](_.event);
            this.dropNode = $;
            if ($ && A[ol0l] == true) {
                if (!A[llo0l1]($)) {
                    var D = $[A.nodesField];
                    if (D && D.length > 0);
                    else if (B.loadOnExpand) A[oO0100]($)
                }
                this.dragAction = this.getFeedback($, C, 3, A)
            } else this.dragAction = "no";
            if (B && A && B != A && !$ && A[o01O00](A.root).length == 0) {
                $ = A[OoOll0]();
                this.dragAction = "add";
                this.dropNode = $
            }
        }
        this.lastFeedbackClass = "mini-feedback-" + this.dragAction;
        this.feedbackEl.className = "mini-feedback " + this.lastFeedbackClass;
        if (this.dragAction == "no") $ = null;
        this.setRowFeedback($, this.dragAction, A)
    },
    ll0ll: function(A) {
        var E = this.owner,
        C = this._getDropTree(A.event);
        mini[O11Oo0](this.feedbackEl);
        this.feedbackEl = null;
        this.setRowFeedback(null);
        var D = [];
        for (var H = 0, G = this.dragData.length; H < G; H++) {
            var J = this.dragData[H],
            B = false;
            for (var K = 0, _ = this.dragData.length; K < _; K++) {
                var F = this.dragData[K];
                if (F != J) {
                    B = E[oooO1o](F, J);
                    if (B) break
                }
            }
            if (!B) D.push(J)
        }
        this.dragData = D;
        if (this.dropNode && this.dragAction != "no") {
            var L = E.o1OlOo(this.dragData, this.dropNode, this.dragAction);
            if (!L.cancel) {
                var D = L.dragNodes,
                I = L.targetNode,
                $ = L.action;
                if (E == C) E[lo0o0l](D, I, $);
                else {
                    E[ll10O0](D);
                    C[l10l11](D, I, $)
                }
            }
        }
        E[O1oO0o](this[ooO0l]);
        L = {
            dragNode: this.dragData[0],
            dropNode: this.dropNode,
            dragAction: this.dragAction
        };
        E[lOO1lo]("drop", L);
        this.dropNode = null;
        this.dragData = null
    },
    setRowFeedback: function(B, F, A) {
        if (this.lastAddDomNode) Oo11(this.lastAddDomNode, "mini-tree-feedback-add");
        if (B == null || this.dragAction == "add") {
            mini[O11Oo0](this.feedbackLine);
            this.feedbackLine = null
        }
        this.lastRowFeedback = B;
        if (B != null) if (F == "before" || F == "after") {
            if (!this.feedbackLine) this.feedbackLine = mini.append(document.body, "<div class='mini-feedback-line'></div>");
            this.feedbackLine.style.display = "block";
            var D = A[OoOo11](B),
            E = D.x,
            C = D.y - 1;
            if (F == "after") C += D.height;
            mini[O1110](this.feedbackLine, E, C);
            var _ = A[lOllOo](true);
            o100oO(this.feedbackLine, _.width)
        } else {
            var $ = A.lOoo(B);
            lloo10($, "mini-tree-feedback-add");
            this.lastAddDomNode = $
        }
    },
    getFeedback: function($, I, F, A) {
        var J = A[OoOo11]($),
        _ = J.height,
        H = I - J.y,
        G = null;
        if (this.dragData[looo1l]($) != -1) return "no";
        var C = false;
        if (F == 3) {
            C = A[llo0l1]($);
            for (var E = 0, D = this.dragData.length; E < D; E++) {
                var K = this.dragData[E],
                B = A[oooO1o](K, $);
                if (B) {
                    G = "no";
                    break
                }
            }
        }
        if (G == null) if (C) {
            if (H > _ / 2) G = "after";
            else G = "before"
        } else if (H > (_ / 3) * 2) G = "after";
        else if (_ / 3 <= H && H <= (_ / 3 * 2)) G = "add";
        else G = "before";
        var L = A.lol01(G, this.dragData, $);
        return L.effect
    },
    OO0oO: function() {
        if (!this.drag) this.drag = new mini.Drag({
            capture: false,
            onStart: mini.createDelegate(this.oOoOO1, this),
            onMove: mini.createDelegate(this.Ol1ll, this),
            onStop: mini.createDelegate(this.ll0ll, this)
        });
        return this.drag
    }
};
OOo1oO = function() {
    this.data = [];
    this.l11lo = {};
    this.oo1l1 = [];
    this.loo1O1 = {};
    this.columns = [];
    this.l1OoO = [];
    this.o10ol = {};
    this.oO10o1 = {};
    this.Oo1ll = [];
    this.oO0oll = {};
    this._cellErrors = [];
    this._cellMapErrors = {};
    OOo1oO[oOOOoO][lllOo0][Ool00](this);
    this[lolo1]();
    var $ = this;
    setTimeout(function() {
        if ($.autoLoad) $[OlOl01]()
    },
    1)
};
oO00o = 0;
Oloo = 0;
ol1O(OOo1oO, lo0O01, {
    O011: "block",
    width: 300,
    height: "auto",
    allowCellValid: false,
    cellEditAction: "cellclick",
    showEmptyText: false,
    emptyText: "No data returned.",
    showModified: true,
    minWidth: 300,
    minHeight: 150,
    maxWidth: 5000,
    maxHeight: 3000,
    _viewRegion: null,
    _virtualRows: 50,
    virtualScroll: false,
    allowCellWrap: false,
    allowHeaderWrap: false,
    showColumnsMenu: false,
    bodyCls: "",
    bodyStyle: "",
    footerCls: "",
    footerStyle: "",
    pagerCls: "",
    pagerStyle: "",
    idField: "id",
    data: [],
    columns: null,
    allowResize: false,
    selectOnLoad: false,
    _rowIdField: "_uid",
    columnWidth: 120,
    columnMinWidth: 20,
    columnMaxWidth: 2000,
    fitColumns: true,
    autoHideRowDetail: true,
    showHeader: true,
    showFooter: true,
    showTop: false,
    showHGridLines: true,
    showVGridLines: true,
    showFilterRow: false,
    showSummaryRow: false,
    sortMode: "server",
    allowSortColumn: true,
    allowMoveColumn: true,
    allowResizeColumn: true,
    enableHotTrack: true,
    allowRowSelect: true,
    multiSelect: false,
    allowAlternating: false,
    oo1oo: "mini-grid-row-alt",
    allowUnselect: false,
    O111OO: "mini-grid-frozen",
    lO0O: "mini-grid-frozenCell",
    frozenStartColumn: -1,
    frozenEndColumn: -1,
    OO0101: "mini-grid-row",
    Olo1o1: "mini-grid-row-hover",
    Oool: "mini-grid-row-selected",
    _headerCellCls: "mini-grid-headerCell",
    _cellCls: "mini-grid-cell",
    uiCls: "mini-datagrid",
    ol1o: true,
    showNewRow: true,
    _rowHeight: 23,
    _ll0O0: true,
    pageIndex: 0,
    pageSize: 10,
    totalCount: 0,
    totalPage: 0,
    showPageInfo: true,
    pageIndexField: "pageIndex",
    pageSizeField: "pageSize",
    sortFieldField: "sortField",
    sortOrderField: "sortOrder",
    totalField: "total",
    showPageSize: true,
    showPageIndex: true,
    showTotalCount: true,
    sortField: "",
    sortOrder: "",
    url: "",
    autoLoad: false,
    loadParams: null,
    ajaxAsync: true,
    ajaxMethod: "post",
    showLoading: true,
    resultAsData: false,
    checkSelectOnLoad: true,
    o01l0O: "total",
    _dataField: "data",
    allowCellSelect: false,
    allowCellEdit: false,
    OOOolo: "mini-grid-cell-selected",
    O1oO1O: null,
    l0o0: null,
    o0l0Ol: null,
    o011O0: null,
    l110O: "_uid",
    o11110: true,
    autoCreateNewID: false,
    collapseGroupOnLoad: false,
    showGroupSummary: false,
    Oo01ol: 1,
    Oooo1: "",
    lo0oO0: "",
    Oo0l1o: null,
    Oo1ll: [],
    headerContextMenu: null,
    columnsMenu: null
});
ll0O = OOo1oO[O0lloO];
ll0O[l1OllO] = ooO01;
ll0O[llOo11] = O11Oo;
ll0O[l0oO10] = olloo;
ll0O[OOOl0o] = o0lll;
ll0O[Oo0l11] = l1oO1;
ll0O[olO011] = o1l10;
ll0O[o1lo1l] = o11lo;
ll0O[l00O1o] = lo10;
ll0O[oO00OO] = lO01o;
ll0O[oloO1o] = OolOl;
ll0O[l0o001] = l0OO1;
ll0O[o1o1lO] = o0ol;
ll0O[l1ol0o] = O00l0;
ll0O[o0lO1O] = OO0l1;
ll0O.OlO11oColumnsMenu = Ol0Oo;
ll0O[l0l1ol] = oooO0;
ll0O[ol1l0O] = l1O0ol;
ll0O[O10l0o] = l0O10;
ll0O.lo1o1 = O0O1l;
ll0O[OoOOO0] = OooOO;
ll0O[lO1Ooo] = Olooo;
ll0O[ol0111] = oloO;
ll0O[olo11l] = O00o1;
ll0O.OoOoooSummaryCell = OooO0;
ll0O[Olo11o] = ll1O1O;
ll0O.o1ll1l = lOO0o;
ll0O.oo1O0O = O1l1O;
ll0O.o01O0O = O0O0lO;
ll0O.O1Ol1O = o0oOo;
ll0O.l1O0oO = l1l1l1;
ll0O.olO10o = lo1o0;
ll0O.o00oO0 = O01o1;
ll0O.OloOO = lO1l1o;
ll0O.O10O1l = OOl01;
ll0O.o0oOOo = o0111;
ll0O.O001oO = o0o0o;
ll0O[o0lO00] = OlO1;
ll0O.lloO = l0oO1;
ll0O.o1o00O = l01o;
ll0O.olo0O = O00oO;
ll0O.O11o1 = O110l;
ll0O.o110O = O1l1;
ll0O[OOO1o] = O1o1lO;
ll0O[ooOOO1] = lll01;
ll0O.l1OlO = lOlO0;
ll0O.OOo1 = l101;
ll0O.O10OO0 = l0O1;
ll0O[OlOl00] = OlloO;
ll0O[O0Ooo1] = o0o11;
ll0O[Ollol1] = ol0lo;
ll0O[oool00] = lOlOl;
ll0O[l110Oo] = l11Oo;
ll0O[O011oO] = ol0lO;
ll0O[OlOlo1] = l1loo;
ll0O[lOoOl] = l1l0;
ll0O[oOo1Ol] = O0100l;
ll0O[llllOo] = lo010;
ll0O[l1OO0o] = oO00;
ll0O[o1OOl] = oOl1l;
ll0O[O1O0oo] = lo010s;
ll0O[OoOllo] = lOllo;
ll0O[lO0Ol0] = O1oO1;
ll0O[o101l1] = OOOo;
ll0O[o1o11l] = OooOo;
ll0O[oll0Oo] = Oo110;
ll0O[ol1110] = lo01;
ll0O[l11011] = l0lO1;
ll0O.oo00lo = ooOO1;
ll0O.o0lO = lOoo0;
ll0O[lOl0OO] = Oll10;
ll0O[l0Olo0] = l001o;
ll0O[oOO01l] = lO1l1;
ll0O[oOoOo] = l11O0l;
ll0O[o11lo1] = ooO1O;
ll0O[oo0l0l] = Oo0O;
ll0O[ollll] = o1l1o;
ll0O.OoOooo = l1Oo0;
ll0O.O1l0 = olol0;
ll0O.o1l00 = ol1O1l;
ll0O[loOOO1] = Ol10;
ll0O[O0lol0] = O0lOOl;
ll0O[oo1O1] = OOOOo;
ll0O[oloOOl] = l0lo;
ll0O[l0oOO] = o0o10;
ll0O.o00O1o = l0O0O;
ll0O.oo0O1 = olllo;
ll0O[l1oOO] = o1000;
ll0O[OOOlOo] = Oo0o1;
ll0O[OlOooo] = lOOO10;
ll0O[loOlOO] = looOO;
ll0O[OO000l] = O01l0;
ll0O[Oll000] = ol0o0;
ll0O[o1l1o1] = lllol0;
ll0O[OloO11] = lllol0s;
ll0O[Oo0o11] = Olo1l;
ll0O[l10OO1] = O0ol1;
ll0O[O11011] = O1o0O;
ll0O[ooO0Ol] = lo1lo;
ll0O[looo1l] = lolo0;
ll0O[l1l111] = loool;
ll0O[Oo0ll1] = ol00l;
ll0O[lOl010] = ll1o0;
ll0O[l11oOl] = ll1o0s;
ll0O[lOOlOo] = oO10l;
ll0O[olOOlo] = Olol0;
ll0O[O1OOol] = oO10ls;
ll0O[lo0Oo1] = l01Ol;
ll0O[OlOl1o] = l01Ols;
ll0O[llO10] = o1OoO;
ll0O[ollO1O] = O11l0;
ll0O.l00l1O = OOll;
ll0O.O0lO = oo001;
ll0O.oOlOO = oo1oO;
ll0O[o1l0ol] = Ol01o;
ll0O[oOoOo0] = O111;
ll0O[O1000o] = lOo0l;
ll0O[ol1ll0] = OoO10;
ll0O[o1Oool] = ooOlo;
ll0O[o0o0o1] = ooOlos;
ll0O[oO0llO] = l0l00;
ll0O[O1OOlo] = ooO1o0;
ll0O[ol0l1l] = lo11;
ll0O[OO00l1] = o000o;
ll0O[ol1O01] = O1oo0;
ll0O[OOol] = l111l;
ll0O[O1lO10] = o0OlO;
ll0O.Ool010 = oOl0o;
ll0O.llool0 = OoO01;
ll0O.oO1OO = o1olO;
ll0O.O11O = OOo01;
ll0O.oO00l = OOlol;
ll0O.O0O0 = OooOlO;
ll0O.o011 = ooo01;
ll0O[o110O1] = Ooool;
ll0O[l1olO1] = Oo0l;
ll0O[Ololo1] = ll1lo;
ll0O[ooOlOl] = O1O1l;
ll0O[OO10O] = olOo1;
ll0O[l10O11] = O1oo1;
ll0O[o1O0ll] = o00oOO;
ll0O[olOl0o] = o0lo;
ll0O[lo0lOO] = oO00Cell;
ll0O[o1lO] = oOl1lCell;
ll0O.O0Oo = oOlO0;
ll0O[oo0O10] = l11OO;
ll0O[loOOl1] = o1O0o;
ll0O[oo100l] = o0ll;
ll0O[l1l11] = OllO;
ll0O[OlOl01] = ll0ol;
ll0O[o01o1] = oo100;
ll0O.lOol01 = Oloo1;
ll0O[o000o1] = ooOO;
ll0O.l0ol00 = oo1Ol;
ll0O[l10l1o] = o0lO0;
ll0O[OOO11o] = lo11O;
ll0O[ooOl11] = OO0oo;
ll0O[lOOOo1] = ll0lO;
ll0O[ol0olO] = l000O;
ll0O[ll0o11] = Ooloo;
ll0O[ollo0o] = o0lOo;
ll0O[l0O1OO] = OO1Oo;
ll0O[o0loOo] = o0l1o;
ll0O[oO0Olo] = OolooField;
ll0O[oOoOo1] = l11ll;
ll0O[l1ooOl] = o0lOoField;
ll0O[l1lllo] = OOoO;
ll0O[OooOO0] = l0loo;
ll0O[ool0l] = O0l1o;
ll0O[ol0Ol0] = l1loO;
ll0O[oO1lOo] = lOolo;
ll0O[lO01lo] = Oo11o;
ll0O[lO0o11] = OOooo;
ll0O[o11lOo] = Ool01l;
ll0O[o0lOoO] = o010;
ll0O[ll01lO] = l11ol;
ll0O[lo1O00] = llO0ol;
ll0O[O0llol] = lolol;
ll0O[oo0o00] = oO11O;
ll0O[Oloo00] = Ol10l;
ll0O[OoO0O1] = ll0l1;
ll0O[oloo0l] = oo111;
ll0O[ll110o] = olo10;
ll0O[llOO01] = l01Oo;
ll0O[OOlOO1] = olo00;
ll0O.oOOl11 = l0O11;
ll0O.l001 = lO0o0;
ll0O.O0loOl = Ool1l;
ll0O.OO11oo = llO0l1;
ll0O.OOl00o = l01o0;
ll0O.o0OOO1 = OOOlo;
ll0O[llo0O1] = O1o0ODetailCellEl;
ll0O[OlO1O1] = O1o0ODetailEl;
ll0O[o1ll1O] = Ollo0;
ll0O[OloOl0] = lo1l0;
ll0O[l0l0Oo] = olo11O;
ll0O[olOll] = l10Ol;
ll0O[oOoOol] = oOol0;
ll0O[O0111] = ooO1;
ll0O[OOloOo] = l00Oo;
ll0O[ol00oO] = l1ol0;
ll0O[oOooo] = o1lOo;
ll0O[Oo0oO1] = o1O1o;
ll0O[oo1Ool] = Oo1lo;
ll0O[ol0OOO] = lOoll;
ll0O[l1l10] = lOl1o;
ll0O[Olol0o] = Oo01o;
ll0O[l10ol] = oOOlO1;
ll0O[o10o11] = O1Ol1;
ll0O[oo1o0O] = O0o0O;
ll0O[O10O0] = llOlo;
ll0O[o11o1l] = OlO1l;
ll0O[o1l0Ol] = lO110;
ll0O[O00loo] = o01oO;
ll0O[O1ol1l] = O0Olo;
ll0O[O0o10l] = OlO1lColumn;
ll0O[o0oO0o] = lO110Column;
ll0O[l1ol00] = O11l1O;
ll0O[O0oo0O] = lOool;
ll0O[o0l00l] = Ololl;
ll0O[ol101o] = O0lo1;
ll0O[OOl1oo] = o00oO;
ll0O[o101l0] = OOllo;
ll0O[lo010o] = Ol0ol;
ll0O[lOo000] = loo01;
ll0O[l0100] = oOlOo;
ll0O[o00o10] = OlOO0;
ll0O[ll0lol] = Oo00o;
ll0O[Oo1o00] = llo0l;
ll0O[o00oll] = o0ol1;
ll0O[oO0oOl] = l0l1o;
ll0O[OoOl1] = o10O0;
ll0O[lo010O] = looo1;
ll0O[o11O1o] = l100O;
ll0O[o0oOoO] = Oo1o0;
ll0O[O01OoO] = OO01o0;
ll0O[O00o1O] = loo1l;
ll0O[O1O0l] = ooo1l;
ll0O[OOO11l] = O1oOo;
ll0O[l1l11o] = l1Oll;
ll0O[OO100l] = OllO0;
ll0O[l1o1l1] = o101;
ll0O[oOOOOO] = o1O10;
ll0O[lOllol] = Ol1olo;
ll0O[o1OOOO] = lOOlO;
ll0O[OoOO0o] = Ool1o;
ll0O[O1oO0o] = llOl;
ll0O[l11o0] = o001l;
ll0O[oolo00] = oOOl0;
ll0O.O1oO10 = olO1o;
ll0O[O11lll] = oo0O0;
ll0O[lO0ol] = l0Oo1;
ll0O[ll0l00] = OlOol;
ll0O[lOO1Oo] = O0Ol;
ll0O[oO0ll0] = ool0;
ll0O[l110o0] = O0o1l;
ll0O[O11ll0] = oO0o0;
ll0O[OO0100] = oolo0;
ll0O[OoOl1l] = ololO;
ll0O.l1OlO0 = O1OO;
ll0O[l01OoO] = OO0OO;
ll0O.oO0011 = l0O0oo;
ll0O.O01lOo = oooll;
ll0O[l0oOo] = l0lO10;
ll0O[l11o01] = O10oo;
ll0O[l10101] = lO11o;
ll0O._ol1O11 = l10O;
ll0O[lo0l00] = OOo0O;
ll0O[Ooo111] = O0Ol0;
ll0O[Oo1lOl] = llo11;
ll0O[o0o0l0] = Ol00ll;
ll0O[Oo11Oo] = ol0oo;
ll0O[l0l10] = o0l1;
ll0O[O1Oloo] = llOo0l;
ll0O._O0l1l = oll0O;
ll0O.Ool00o = Ooll1;
ll0O.ooll = l1oO;
ll0O[oO1oOl] = Olll0;
ll0O[l01o1] = lOO11;
ll0O[OlOl1l] = O1o0OsBox;
ll0O[ll0l0O] = O1o0OBox;
ll0O[o0O00] = o0111O;
ll0O.l1l011 = Oll00;
ll0O[OOOOOO] = o0l1O;
ll0O[l0ll0o] = l10OO;
ll0O[lOlo1O] = lOoO1l;
ll0O.OO1l1 = OOOloId;
ll0O.OlOl10 = OOOo0;
ll0O.o1o1ll = o11l0;
ll0O.loOO1O = O11ll;
ll0O.l1l01 = O1Oo1o;
ll0O.o0O1lO = oOO00;
ll0O[Oo1OoO] = OO0lo;
ll0O[ll1loO] = O10lO;
ll0O[lO01O] = OoO0O;
ll0O[OoolOO] = l1oO0;
ll0O[oo1ll1] = oOool;
ll0O[o10l10] = OOO11;
ll0O.l1l110 = Oo0ll;
ll0O.o0010 = Ol0l10;
ll0O[lolo1] = OOl11;
ll0O[Ol1ool] = O1O10;
ll0O[lo11ol] = lOol1;
ll0O.looO01 = l10oO1;
ll0O[ol1O0] = ooO00;
ll0O.lool00 = O1ooo;
ll0O.OoooText = Olol10;
ll0O.oOol1 = Ool0;
ll0O.ol1loO = Oo100;
ll0O.OlOlOl = o11oO;
ll0O.o110o = OOOll;
ll0O[OOO0ol] = l1111;
ll0O[ll00lo] = l00oO;
ll0O[loO1] = O1lo0;
ll0O[l1l0O] = oo10l;
ll0O[oo0llo] = l1looRange;
ll0O[olOoo1] = Oo1o0O;
ll0O[llOo1l] = oOoo1;
ll0O[OlOO1l] = lol10l;
ll0O[l1OlOo] = olo1l;
ll0O[oo111O] = oo100Data;
ll0O[Ool100] = l1101;
ll0O[o11100] = oOllO;
ll0O[ol0lO0] = O00010;
ll0O[O00OO] = lol1o;
ll0O[OOO10o] = oOO0O;
ll0O[o0oO0l] = ol10O;
ll0O[olOoo] = o001;
ll0O[O0ll11] = Oloo0;
ll0O[o1lOol] = ll0l;
ll0O[lol0o0] = oOloo;
ll0O.oo1l = oo1o0;
ll0O[OlOoo] = o1lo1;
ll0O.OlO11oRows = O01O1;
ll0O[OOOol0] = oOl01;
ll0O[oOllOo] = Oo01O;
ll0O[lOlo11] = oO0Ol;
ll0O[ol0Ol1] = Ol0ll;
ll0O[oO0l1o] = l1OOO;
ooOl0(OOo1oO, "datagrid");
O0o00 = {
    _getColumnEl: function($) {
        $ = this[lO00o]($);
        if (!$) return null;
        var _ = this.o1olo($);
        return document.getElementById(_)
    },
    O0o0: function($, _) {
        $ = this[O11011] ? this[O11011]($) : this[Ol0ooo]($);
        _ = this[lO00o](_);
        if (!$ || !_) return null;
        var A = this.loOO1O($, _);
        return document.getElementById(A)
    },
    ooOlO: function(A) {
        var $ = this.olo0O ? this.olo0O(A) : this[OoOO0l](A),
        _ = this.ll100(A);
        return {
            record: $,
            column: _
        }
    },
    ll100: function(B) {
        var _ = OO0O(B.target, this._cellCls);
        if (!_) _ = OO0O(B.target, this._headerCellCls);
        if (_) {
            var $ = _.id.split("$"),
            A = $[$.length - 1];
            return this.l0lll(A)
        }
        return null
    },
    o1olo: function($) {
        return this.uid + "$column$" + $._id
    },
    getColumnBox: function(A) {
        var B = this.o1olo(A),
        _ = document.getElementById(B);
        if (_) {
            var $ = lolloO(_);
            $.x -= 1;
            $.left = $.x;
            $.right = $.x + $.width;
            return $
        }
    },
    setColumns: function(value) {
        if (!mini.isArray(value)) value = [];
        this.columns = value;
        this.o10ol = {};
        this.oO10o1 = {};
        this.l1OoO = [];
        this.maxColumnLevel = 0;
        var level = 0;
        function init(column, index, parentColumn) {
            if (column.type) {
                if (!mini.isNull(column.header) && typeof column.header !== "function") if (column.header.trim() == "") delete column.header;
                var col = mini[l1l0o1](column.type);
                if (col) {
                    var _column = mini.copyTo({},
                    column);
                    mini.copyTo(column, col);
                    mini.copyTo(column, _column)
                }
            }
            var width = parseInt(column.width);
            if (mini.isNumber(width) && String(width) == column.width) column.width = width + "px";
            if (mini.isNull(column.width)) column.width = this[l01O0o] + "px";
            column.visible = column.visible !== false;
            column[l000l] = column[l000l] !== false;
            column.allowMove = column.allowMove !== false;
            column.allowSort = column.allowSort === true;
            column.allowDrag = !!column.allowDrag;
            column[O00O01] = !!column[O00O01];
            if (!column._id) column._id = Oloo++;
            column._gridUID = this.uid;
            column[lo0lll] = this[lo0lll];
            column._pid = parentColumn == this ? -1: parentColumn._id;
            this.o10ol[column._id] = column;
            if (column.name) this.oO10o1[column.name] = column;
            if (!column.columns || column.columns.length == 0) this.l1OoO.push(column);
            column.level = level;
            level += 1;
            this[lo1lol](column, init, this);
            level -= 1;
            if (column.level > this.maxColumnLevel) this.maxColumnLevel = column.level;
            if (typeof column.editor == "string") {
                var cls = mini.getClass(column.editor);
                if (cls) column.editor = {
                    type: column.editor
                };
                else column.editor = eval("(" + column.editor + ")")
            }
            if (typeof column[ol1l00] == "string") column[ol1l00] = eval("(" + column[ol1l00] + ")");
            if (column[ol1l00] && !column[ol1l00].el) column[ol1l00] = mini.create(column[ol1l00]);
            if (typeof column.init == "function" && column.inited != true) column.init(this);
            column.inited = true
        }
        this[lo1lol](this, init, this);
        if (this.ol1loO) this.ol1loO();
        this[lolo1]();
        this[lOO1lo]("columnschanged")
    },
    getColumns: function() {
        return this.columns
    },
    getBottomColumns: function() {
        return this.l1OoO
    },
    getVisibleColumns: function() {
        var B = this[o1OOO](),
        A = [];
        for (var $ = 0, C = B.length; $ < C; $++) {
            var _ = B[$];
            if (_.visible) A.push(_)
        }
        return A
    },
    getBottomVisibleColumns: function() {
        var A = [];
        for (var $ = 0, B = this.l1OoO.length; $ < B; $++) {
            var _ = this.l1OoO[$];
            if (this[oll0Ol](_)) A.push(_)
        }
        return A
    },
    eachColumns: function(B, F, C) {
        var D = B.columns;
        if (D) {
            var _ = D.clone();
            for (var A = 0, E = _.length; A < E; A++) {
                var $ = _[A];
                if (F[Ool00](C, $, A, B) === false) break
            }
        }
    },
    getColumn: function($) {
        var _ = typeof $;
        if (_ == "number") return this[o1OOO]()[$];
        else if (_ == "object") return $;
        else return this.oO10o1[$]
    },
    l0lll: function($) {
        return this.o10ol[$]
    },
    getParentColumn: function($) {
        $ = this[lO00o]($);
        var _ = $._pid;
        if (_ == -1) return this;
        return this.o10ol[_]
    },
    getAncestorColumns: function(A) {
        var _ = [];
        while (1) {
            var $ = this[O0110](A);
            if (!$ || $ == this) break;
            _[_.length] = $;
            A = $
        }
        _.reverse();
        return _
    },
    isAncestorColumn: function(_, B) {
        if (_ == B) return true;
        if (!_ || !B) return false;
        var A = this[OOll1](B);
        for (var $ = 0, C = A.length; $ < C; $++) if (A[$] == _) return true;
        return false
    },
    isVisibleColumn: function(_) {
        _ = this[lO00o](_);
        var A = this[OOll1](_);
        for (var $ = 0, B = A.length; $ < B; $++) if (A[$].visible == false) return false;
        return true
    },
    updateColumn: function(_, $) {
        _ = this[lO00o](_);
        if (!_) return;
        mini.copyTo(_, $);
        this[l1011O](this.columns)
    },
    removeColumn: function($) {
        $ = this[lO00o]($);
        var _ = this[O0110]($);
        if ($ && _) {
            _.columns.remove($);
            this[l1011O](this.columns)
        }
        return $
    },
    moveColumn: function(C, _, A) {
        C = this[lO00o](C);
        _ = this[lO00o](_);
        if (!C || !_ || !A || C == _) return;
        if (this[lloOO1](C, _)) return;
        var D = this[O0110](C);
        if (D) D.columns.remove(C);
        var B = _,
        $ = A;
        if ($ == "before") {
            B = this[O0110](_);
            $ = B.columns[looo1l](_)
        } else if ($ == "after") {
            B = this[O0110](_);
            $ = B.columns[looo1l](_) + 1
        } else if ($ == "add" || $ == "append") {
            if (!B.columns) B.columns = [];
            $ = B.columns.length
        } else if (!mini.isNumber($)) return;
        B.columns.insert($, C);
        this[l1011O](this.columns)
    },
    hideColumn: function($) {
        $ = this[lO00o]($);
        if (!$) return;
        if (this[l0oO0]) this[l1olO1]();
        $.visible = false;
        this[l1011O](this.columns)
    },
    showColumn: function($) {
        $ = this[lO00o]($);
        if (!$) return;
        if (this[l0oO0]) this[l1olO1]();
        $.visible = true;
        this[l1011O](this.columns)
    },
    lOO1lO: function() {
        var _ = this[Oo1oo](),
        D = [];
        for (var C = 0, F = _; C <= F; C++) D.push([]);
        function A(C) {
            var D = mini[ol0oo1](C.columns, "columns"),
            A = 0;
            for (var $ = 0, B = D.length; $ < B; $++) {
                var _ = D[$];
                if (_.visible != true || _._hide == true) continue;
                if (!_.columns || _.columns.length == 0) A += 1
            }
            return A
        }
        var $ = mini[ol0oo1](this.columns, "columns");
        for (C = 0, F = $.length; C < F; C++) {
            var E = $[C],
            B = D[E.level];
            if (E.columns && E.columns.length > 0) E.colspan = A(E);
            if ((!E.columns || E.columns.length == 0) && E.level < _) E.rowspan = _ - E.level + 1;
            B.push(E)
        }
        return D
    },
    getMaxColumnLevel: function() {
        return this.maxColumnLevel
    }
};
mini.copyTo(OOo1oO.prototype, O0o00);
O0o000 = function($) {
    this.grid = $;
    looo($.OlOooO, "mousemove", this.__OnGridHeaderMouseMove, this);
    looo($.OlOooO, "mouseout", this.__OnGridHeaderMouseOut, this)
};
O0o000[O0lloO] = {
    __OnGridHeaderMouseOut: function($) {
        if (this.o1l010ColumnEl) Oo11(this.o1l010ColumnEl, "mini-grid-headerCell-hover")
    },
    __OnGridHeaderMouseMove: function(_) {
        var $ = OO0O(_.target, "mini-grid-headerCell");
        if ($) {
            lloo10($, "mini-grid-headerCell-hover");
            this.o1l010ColumnEl = $
        }
    },
    __onGridHeaderCellClick: function(B) {
        var $ = this.grid,
        A = OO0O(B.target, "mini-grid-headerCell");
        if (A) {
            var _ = $[lO00o](A.id.split("$")[2]);
            if ($[ooO1O0] && _ && _.allowDrag) {
                this.dragColumn = _;
                this._columnEl = A;
                this.getDrag().start(B)
            }
        }
    }
};
lolOl = function($) {
    this.grid = $;
    looo(this.grid.el, "mousedown", this.o0o101, this);
    $[O1oOo1]("layout", this.l1ll, this)
};
lolOl[O0lloO] = {
    l1ll: function(A) {
        if (this.splittersEl) mini[O11Oo0](this.splittersEl);
        if (this.splitterTimer) return;
        var $ = this.grid;
        if ($[ll1001]() == false) return;
        var _ = this;
        this.splitterTimer = setTimeout(function() {
            var H = $[o1OOO](),
            I = H.length,
            E = lolloO($.OlOooO, true),
            B = $[Ol1ool](),
            G = [];
            for (var J = 0, F = H.length; J < F; J++) {
                var D = H[J],
                C = $[l1O00o](D);
                if (!C) break;
                var A = C.top - E.top,
                M = C.right - E.left - 2,
                K = C.height;
                if ($[oO0l1o] && $[oO0l1o]()) {
                    if (J >= $[llll1]);
                } else M += B;
                var N = $[O0110](D);
                if (N && N.columns) if (N.columns[N.columns.length - 1] == D) if (K + 5 < E.height) {
                    A = 0;
                    K = E.height
                }
                if ($[Oll01] && D[l000l]) G[G.length] = "<div id=\"" + D._id + "\" class=\"mini-grid-splitter\" style=\"left:" + (M - 1) + "px;top:" + A + "px;height:" + K + "px;\"></div>"
            }
            var O = G.join("");
            _.splittersEl = document.createElement("div");
            _.splittersEl.className = "mini-grid-splitters";
            _.splittersEl.innerHTML = O;
            var L = $[lOlo1O]();
            L.appendChild(_.splittersEl);
            _.splitterTimer = null
        },
        100)
    },
    o0o101: function(B) {
        var $ = this.grid,
        A = B.target;
        if (ololo(A, "mini-grid-splitter")) {
            var _ = $.o10ol[A.id];
            if ($[Oll01] && _ && _[l000l]) {
                this.splitterColumn = _;
                this.getDrag().start(B)
            }
        }
    },
    getDrag: function() {
        if (!this.drag) this.drag = new mini.Drag({
            capture: true,
            onStart: mini.createDelegate(this.oOoOO1, this),
            onMove: mini.createDelegate(this.Ol1ll, this),
            onStop: mini.createDelegate(this.ll0ll, this)
        });
        return this.drag
    },
    oOoOO1: function(_) {
        var $ = this.grid,
        B = $[l1O00o](this.splitterColumn);
        this.columnBox = B;
        this.lool0O = mini.append(document.body, "<div class=\"mini-grid-proxy\"></div>");
        var A = $[lOllOo](true);
        A.x = B.x;
        A.width = B.width;
        A.right = B.right;
        ooo1(this.lool0O, A)
    },
    Ol1ll: function(A) {
        var $ = this.grid,
        B = mini.copyTo({},
        this.columnBox),
        _ = B.width + (A.now[0] - A.init[0]);
        if (_ < $.columnMinWidth) _ = $.columnMinWidth;
        if (_ > $.columnMaxWidth) _ = $.columnMaxWidth;
        o100oO(this.lool0O, _)
    },
    ll0ll: function(E) {
        var $ = this.grid,
        F = lolloO(this.lool0O),
        D = this,
        C = $[lOO00];
        $[lOO00] = false;
        setTimeout(function() {
            jQuery(D.lool0O).remove();
            D.lool0O = null;
            $[lOO00] = C
        },
        10);
        var G = this.splitterColumn,
        _ = parseInt(G.width);
        if (_ + "%" != G.width) {
            var A = $[oO1oOl](G),
            B = parseInt(_ / A * F.width);
            $[l01o1](G, B)
        }
    }
};
ooll1 = function($) {
    this.grid = $;
    looo(this.grid.el, "mousedown", this.o0o101, this)
};
ooll1[O0lloO] = {
    o0o101: function(B) {
        var $ = this.grid;
        if ($[ol0l1l] && $[ol0l1l]()) return;
        if (ololo(B.target, "mini-grid-splitter")) return;
        if (B.button == mini.MouseButton.Right) return;
        var A = OO0O(B.target, $._headerCellCls);
        if (A) {
            var _ = $.ll100(B);
            if ($[ooO1O0] && _ && _.allowMove) {
                this.dragColumn = _;
                this._columnEl = A;
                this.getDrag().start(B)
            }
        }
    },
    getDrag: function() {
        if (!this.drag) this.drag = new mini.Drag({
            capture: isIE9 ? false: true,
            onStart: mini.createDelegate(this.oOoOO1, this),
            onMove: mini.createDelegate(this.Ol1ll, this),
            onStop: mini.createDelegate(this.ll0ll, this)
        });
        return this.drag
    },
    oOoOO1: function(_) {
        function A(_) {
            var A = _.header;
            if (typeof A == "function") A = A[Ool00]($, _);
            if (mini.isNull(A) || A === "") A = "&nbsp;";
            return A
        }
        var $ = this.grid;
        this.lool0O = mini.append(document.body, "<div class=\"mini-grid-columnproxy\"></div>");
        this.lool0O.innerHTML = "<div class=\"mini-grid-columnproxy-inner\" style=\"height:26px;\">" + A(this.dragColumn) + "</div>";
        mini[O1110](this.lool0O, _.now[0] + 15, _.now[1] + 18);
        lloo10(this.lool0O, "mini-grid-no");
        this.moveTop = mini.append(document.body, "<div class=\"mini-grid-movetop\"></div>");
        this.moveBottom = mini.append(document.body, "<div class=\"mini-grid-movebottom\"></div>")
    },
    Ol1ll: function(A) {
        var $ = this.grid,
        G = A.now[0];
        mini[O1110](this.lool0O, G + 15, A.now[1] + 18);
        this.targetColumn = this.insertAction = null;
        var D = OO0O(A.event.target, $._headerCellCls);
        if (D) {
            var C = $.ll100(A.event);
            if (C && C != this.dragColumn) {
                var _ = $[O0110](this.dragColumn),
                E = $[O0110](C);
                if (_ == E) {
                    this.targetColumn = C;
                    this.insertAction = "before";
                    var F = $[l1O00o](this.targetColumn);
                    if (G > F.x + F.width / 2) this.insertAction = "after"
                }
            }
        }
        if (this.targetColumn) {
            lloo10(this.lool0O, "mini-grid-ok");
            Oo11(this.lool0O, "mini-grid-no");
            var B = $[l1O00o](this.targetColumn);
            this.moveTop.style.display = "block";
            this.moveBottom.style.display = "block";
            if (this.insertAction == "before") {
                mini[O1110](this.moveTop, B.x - 4, B.y - 9);
                mini[O1110](this.moveBottom, B.x - 4, B.bottom)
            } else {
                mini[O1110](this.moveTop, B.right - 4, B.y - 9);
                mini[O1110](this.moveBottom, B.right - 4, B.bottom)
            }
        } else {
            Oo11(this.lool0O, "mini-grid-ok");
            lloo10(this.lool0O, "mini-grid-no");
            this.moveTop.style.display = "none";
            this.moveBottom.style.display = "none"
        }
    },
    ll0ll: function(_) {
        var $ = this.grid;
        mini[O11Oo0](this.lool0O);
        mini[O11Oo0](this.moveTop);
        mini[O11Oo0](this.moveBottom);
        $[oo01](this.dragColumn, this.targetColumn, this.insertAction);
        this.lool0O = this.moveTop = this.moveBottom = this.dragColumn = this.targetColumn = null
    }
};
O0001 = function($) {
    this.grid = $;
    this.grid[O1oOo1]("cellmousedown", this.lO00ol, this);
    this.grid[O1oOo1]("cellclick", this.oo1O, this);
    this.grid[O1oOo1]("celldblclick", this.oo1O, this);
    looo(this.grid.el, "keydown", this.ol0o, this)
};
O0001[O0lloO] = {
    ol0o: function(G) {
        var $ = this.grid;
        if (Ol11($.O0011, G.target) || Ol11($.OoO11l, G.target) || Ol11($.oo1lOo, G.target)) return;
        var A = $[lo0lOO]();
        if (G.shiftKey || G.ctrlKey) return;
        if (G.keyCode == 37 || G.keyCode == 38 || G.keyCode == 39 || G.keyCode == 40) G.preventDefault();
        var C = $[oOO0o](),
        B = A ? A[1] : null,
        _ = A ? A[0] : null;
        if (!A) _ = $[l1OO0o]();
        var F = C[looo1l](B),
        D = $[looo1l](_),
        E = $[OlOO1l]().length;
        switch (G.keyCode) {
        case 9:
            break;
        case 27:
            break;
        case 13:
            if ($[l0oO0] && A && !B[O00O01]) $[ooOlOl]();
            break;
        case 37:
            if (B) {
                if (F > 0) F -= 1
            } else F = 0;
            break;
        case 38:
            if (_) {
                if (D > 0) D -= 1
            } else D = 0;
            if (D != 0 && $[lo11ol]()) if ($._viewRegion.start > D) {
                $.O0ll1o.scrollTop -= $._rowHeight;
                $[l0oOo]()
            }
            break;
        case 39:
            if (B) {
                if (F < C.length - 1) F += 1
            } else F = 0;
            break;
        case 40:
            if (_) {
                if (D < E - 1) D += 1
            } else D = 0;
            if ($[lo11ol]()) if ($._viewRegion.end < D) {
                $.O0ll1o.scrollTop += $._rowHeight;
                $[l0oOo]()
            }
            break;
        default:
            break
        }
        B = C[F];
        _ = $[ooO0Ol](D);
        if (B && _ && $[l1o1ol]) {
            A = [_, B];
            $[o1lO](A);
            $[oOo1Ol](_, B)
        }
        if (_ && $[O0OOlO]) {
            $[oool00]();
            $[o1OOl](_)
        }
    },
    oo1O: function(A) {
        if (this.grid.cellEditAction != A.type) return;
        var $ = A.record,
        _ = A.column;
        if (!_[O00O01] && !this.grid[l0O0Oo]()) if (A.htmlEvent.shiftKey || A.htmlEvent.ctrlKey);
        else this.grid[ooOlOl]()
    },
    lO00ol: function(_) {
        var $ = this;
        setTimeout(function() {
            $.__doSelect(_)
        },
        1)
    },
    __doSelect: function(C) {
        var _ = C.record,
        B = C.column,
        $ = this.grid;
        if (this.grid[l1o1ol]) {
            var A = [_, B];
            this.grid[o1lO](A)
        }
        if ($[O0OOlO]) if ($[o1lloO]) {
            this.grid.el.onselectstart = function() {};
            if (C.htmlEvent.shiftKey) {
                this.grid.el.onselectstart = function() {
                    return false
                };
                C.htmlEvent.preventDefault();
                if (!this.currentRecord) {
                    this.grid[OlOlo1](_);
                    this.currentRecord = this.grid[llllOo]()
                } else {
                    this.grid[oool00]();
                    this.grid[oo0llo](this.currentRecord, _)
                }
            } else {
                this.grid.el.onselectstart = function() {};
                if (C.htmlEvent.ctrlKey) {
                    this.grid.el.onselectstart = function() {
                        return false
                    };
                    C.htmlEvent.preventDefault()
                }
                if (C.column._multiRowSelect === true || C.htmlEvent.ctrlKey || $.allowUnselect) {
                    if ($[OoOllo](_)) $[O011oO](_);
                    else $[OlOlo1](_)
                } else if ($[OoOllo](_));
                else {
                    $[oool00]();
                    $[OlOlo1](_)
                }
                this.currentRecord = this.grid[llllOo]()
            }
        } else if (!$[OoOllo](_)) {
            $[oool00]();
            $[OlOlo1](_)
        } else if (C.htmlEvent.ctrlKey) $[oool00]()
    }
};
O110o0 = function($) {
    this.grid = $;
    looo(this.grid.el, "mousemove", this.__onGridMouseMove, this)
};
O110o0[O0lloO] = {
    __onGridMouseMove: function(D) {
        var $ = this.grid,
        A = $.ooOlO(D),
        _ = $.O0o0(A.record, A.column),
        B = $.getCellError(A.record, A.column);
        if (_) {
            if (B) {
                _.title = B.errorText;
                return
            }
            if (_.firstChild) if (ololo(_.firstChild, "mini-grid-cell-inner") || ololo(_.firstChild, "mini-treegrid-treecolumn-inner")) _ = _.firstChild;
            if (_.scrollWidth > _.clientWidth) {
                var C = _.innerText || _.textContent || "";
                _.title = C.trim()
            } else _.title = ""
        }
    }
};
mini.oo10OoMenu = function($) {
    this.grid = $;
    this.menu = this.createMenu();
    looo($.el, "contextmenu", this.o01O0O, this)
};
mini.oo10OoMenu[O0lloO] = {
    createMenu: function() {
        var $ = mini.create({
            type: "menu",
            hideOnClick: false
        });
        $[O1oOo1]("itemclick", this.oOll0, this);
        return $
    },
    updateMenu: function() {
        var _ = this.grid,
        F = this.menu,
        D = _[o1OOO](),
        B = [];
        for (var A = 0, E = D.length; A < E; A++) {
            var C = D[A],
            $ = {};
            $.checked = C.visible;
            $[l0OO0] = true;
            $.text = _.OoooText(C);
            if ($.text == "&nbsp;") {
                if (C.type == "indexcolumn") $.text = "\u5e8f\u53f7";
                if (C.type == "checkcolumn") $.text = "\u9009\u62e9"
            }
            B.push($);
            $._column = C
        }
        F[oOO1ll](B)
    },
    o01O0O: function(_) {
        var $ = this.grid;
        if ($.showColumnsMenu == false) return;
        if (Ol11($.OlOooO, _.target) == false) return;
        this[ol1100]();
        this.menu.showAtPos(_.pageX, _.pageY);
        return false
    },
    oOll0: function(J) {
        var C = this.grid,
        I = this.menu,
        A = C[o1OOO](),
        E = I[l1o000](),
        $ = J.item,
        _ = $._column,
        H = 0;
        for (var D = 0, B = E.length; D < B; D++) {
            var F = E[D];
            if (F[o0lO1o]()) H++
        }
        if (H < 1) $[lO0ol0](true);
        var G = $[o0lO1o]();
        if (G) C.showColumn(_);
        else C.hideColumn(_)
    }
};
Oo0o = {
    getCellErrors: function() {
        var A = this._cellErrors.clone(),
        C = this.data;
        for (var $ = 0, D = A.length; $ < D; $++) {
            var E = A[$],
            _ = E.record,
            B = E.column;
            if (C[looo1l](_) == -1) {
                var F = _[this._rowIdField] + "$" + B._id;
                delete this._cellMapErrors[F];
                this._cellErrors.remove(E)
            }
        }
        return this._cellErrors
    },
    getCellError: function($, _) {
        $ = this[Ol0ooo] ? this[Ol0ooo]($) : this[O11011]($);
        _ = this[lO00o](_);
        if (!$ || !_) return;
        var A = $[this._rowIdField] + "$" + _._id;
        return this._cellMapErrors[A]
    },
    isValid: function() {
        return this.getCellErrors().length == 0
    },
    validate: function() {
        var A = this.data;
        for (var $ = 0, B = A.length; $ < B; $++) {
            var _ = A[$];
            this.validateRow(_)
        }
    },
    validateRow: function(_) {
        var B = this[o1OOO]();
        for (var $ = 0, C = B.length; $ < C; $++) {
            var A = B[$];
            this.validateCell(_, A)
        }
    },
    validateCell: function(C, E) {
        C = this[Ol0ooo] ? this[Ol0ooo](C) : this[O11011](C);
        E = this[lO00o](E);
        if (!C || !E) return;
        var I = {
            record: C,
            row: C,
            node: C,
            column: E,
            field: E.field,
            value: C[E.field],
            isValid: true,
            errorText: ""
        };
        if (E.vtype) mini.lo1O(E.vtype, I.value, I, E);
        if (I[lO0oOl] == true && E.unique && E.field) {
            var A = {},
            D = this.data,
            F = E.field;
            for (var _ = 0, G = D.length; _ < G; _++) {
                var $ = D[_],
                H = $[F];
                if (mini.isNull(H) || H === "");
                else {
                    var B = A[H];
                    if (B && $ == C) {
                        I[lO0oOl] = false;
                        I.errorText = mini.oO00l1(E, "uniqueErrorText");
                        this.setCellIsValid(B, E, I.isValid, I.errorText);
                        break
                    }
                    A[H] = $
                }
            }
        }
        this[lOO1lo]("cellvalidation", I);
        this.setCellIsValid(C, E, I.isValid, I.errorText)
    },
    setIsValid: function(_) {
        if (_) {
            var A = this._cellErrors.clone();
            for (var $ = 0, B = A.length; $ < B; $++) {
                var C = A[$];
                this.setCellIsValid(C.record, C.column, true)
            }
        }
    },
    _removeRowError: function(_) {
        var B = this[lolO0O]();
        for (var $ = 0, C = B.length; $ < C; $++) {
            var A = B[$],
            E = _[this._rowIdField] + "$" + A._id,
            D = this._cellMapErrors[E];
            if (D) {
                delete this._cellMapErrors[E];
                this._cellErrors.remove(D)
            }
        }
    },
    setCellIsValid: function(_, A, B, D) {
        _ = this[Ol0ooo] ? this[Ol0ooo](_) : this[O11011](_);
        A = this[lO00o](A);
        if (!_ || !A) return;
        var E = _[this._rowIdField] + "$" + A._id,
        $ = this.O0o0(_, A),
        C = this._cellMapErrors[E];
        delete this._cellMapErrors[E];
        this._cellErrors.remove(C);
        if (B === true) {
            if ($ && C) Oo11($, "mini-grid-cell-error")
        } else {
            C = {
                record: _,
                column: A,
                isValid: B,
                errorText: D
            };
            this._cellMapErrors[E] = C;
            this._cellErrors[O0olo1](C);
            if ($) lloo10($, "mini-grid-cell-error")
        }
    }
};
mini.copyTo(OOo1oO.prototype, Oo0o);
mini.GridEditor = function() {
    this._inited = true;
    lo0O01[oOOOoO][lllOo0][Ool00](this);
    this[lOlo11]();
    this.el.uid = this.uid;
    this[OOOol0]();
    this.loOOo();
    this[o00lO1](this.uiCls)
};
ol1O(mini.GridEditor, lo0O01, {
    el: null,
    _create: function() {
        this.el = document.createElement("input");
        this.el.type = "text";
        this.el.style.width = "100%"
    },
    getValue: function() {
        return this.el.value
    },
    setValue: function($) {
        this.el.value = $
    },
    setWidth: function($) {}
});
O1olOO = function() {
    O1olOO[oOOOoO][lllOo0][Ool00](this)
};
ol1O(O1olOO, lo0O01, {
    pageIndex: 0,
    pageSize: 10,
    totalCount: 0,
    totalPage: 0,
    showPageIndex: true,
    showPageSize: true,
    showTotalCount: true,
    showPageInfo: true,
    showReloadButton: true,
    _clearBorder: false,
    showButtonText: false,
    showButtonIcon: true,
    firstText: "\u9996\u9875",
    prevText: "\u4e0a\u4e00\u9875",
    nextText: "\u4e0b\u4e00\u9875",
    lastText: "\u5c3e\u9875",
    pageInfoText: "\u6bcf\u9875 {0} \u6761,\u5171 {1} \u6761",
    sizeList: [10, 20, 50, 100],
    uiCls: "mini-pager"
});
Olll = O1olOO[O0lloO];
Olll[l1OllO] = ooO0l1;
Olll[O0llll] = O1o1oO;
Olll.l110 = o10O;
Olll.oOlO01 = lol0lo;
Olll[O00ol1] = o11O1;
Olll[ooOl11] = oOo0;
Olll[l0l11o] = l11lO1;
Olll[lolo1l] = OoOlO;
Olll[llOO01] = O011l;
Olll[OOlOO1] = O110;
Olll[lO01lo] = O100l0;
Olll[lO0o11] = lOooo1;
Olll[o11lOo] = o0Oo;
Olll[o0lOoO] = oOoo;
Olll[ll01lO] = lo1o;
Olll[lo1O00] = o0oO;
Olll[oloo0l] = o0oO0;
Olll[ll110o] = O1llol;
Olll[lOOOo1] = OOooO;
Olll[ol0olO] = oO1o0;
Olll[Oloo00] = lO1lo1;
Olll[OoO0O1] = OO001;
Olll[O0llol] = O111O;
Olll[oo0o00] = l101l;
Olll[o10l10] = oOl00l;
Olll[OOOol0] = ol00;
Olll[oOllOo] = l1O1;
Olll[lOlo11] = O10l11;
ooOl0(O1olOO, "pager");
o1Oll0 = function() {
    this.columns = [];
    this.l1OoO = [];
    this.o10ol = {};
    this.oO10o1 = {};
    this._cellErrors = [];
    this._cellMapErrors = {};
    o1Oll0[oOOOoO][lllOo0][Ool00](this);
    this.l0001.style.display = this[l000l] ? "": "none"
};
ol1O(o1Oll0, o110ol, {
    _rowIdField: "_id",
    width: 300,
    height: 180,
    allowResize: false,
    treeColumn: "",
    columns: [],
    columnWidth: 80,
    allowResizeColumn: true,
    allowMoveColumn: true,
    Ol0l1O: true,
    _headerCellCls: "mini-treegrid-headerCell",
    _cellCls: "mini-treegrid-cell",
    OOll11: "mini-treegrid-border",
    OO11O: "mini-treegrid-header",
    O000OO: "mini-treegrid-body",
    loll0: "mini-treegrid-node",
    O1l1l: "mini-treegrid-nodes",
    lo1l: "mini-treegrid-selectedNode",
    l0000O: "mini-treegrid-hoverNode",
    llll1o: "mini-treegrid-expand",
    olol11: "mini-treegrid-collapse",
    o1l0OO: "mini-treegrid-ec-icon",
    o1l001: "mini-treegrid-nodeTitle",
    uiCls: "mini-treegrid"
});
OoO0o = o1Oll0[O0lloO];
OoO0o[l1OllO] = ooo11;
OoO0o.O0O0ll = o1O0l;
OoO0o[oO1oOl] = O0O1o;
OoO0o[l01o1] = O0o11;
OoO0o.loOO1O = lO1OO;
OoO0o[o11o1l] = oOlll;
OoO0o[o1l0Ol] = oO0oo;
OoO0o[l1ol00] = oo0lo;
OoO0o[O0oo0O] = OllOOl;
OoO0o[O0o10l] = oOlllColumn;
OoO0o[o0oO0o] = oO0ooColumn;
OoO0o[l100O0] = l0l0;
OoO0o[lO0oO0] = oO01o;
OoO0o.oo11ol = ollo;
OoO0o.o1ll1l = Ol0o;
OoO0o[l1l0O] = o0OoO;
OoO0o.Ooooll = Ol00O;
OoO0o[oo1ll1] = lOOoO;
OoO0o[o10l10] = o1l0o;
OoO0o[Ol1ool] = l0OOo;
OoO0o[lolo1] = OO1ll;
OoO0o.OOo0lo = OOl1l;
OoO0o.lool00 = O1o11;
OoO0o.OlOlOl = OooO1;
OoO0o[lOlo1O] = llOl1;
OoO0o.o1olo = l11lO0;
OoO0o[lOlo11] = Oo1lO;
OoO0o.ll01l = o011o;
mini.copyTo(o1Oll0.prototype, O0o00);
mini.copyTo(o1Oll0.prototype, Oo0o);
ooOl0(o1Oll0, "treegrid");
mini.DataSource = function() {
    mini.DataSource[oOOOoO][lllOo0][Ool00](this);
    this._init()
};
ol1O(mini.DataSource, Oo11l0, {
    _init: function() {
        this.source = [];
        this.dataview = [];
        this._ids = {};
        this._removeds = [];
        this.loo1O1 = {};
        this._errors = {};
        this.Oo0l1o = null;
        this.Oo1ll = [];
        this.oO0oll = {};
        this.__changeCount = 0
    },
    getDataView: function() {
        return this.dataview
    },
    loadData: function($) {
        if (!mini.isArray($)) $ = [];
        var _ = {
            data: $,
            cancel: false
        };
        this[lOO1lo]("beforeloaddata", _);
        if (_.cancel == true) return false;
        this._init();
        this.O0o1O($);
        this.llOoO();
        this[lOO1lo]("dataloaded");
        return true
    },
    O0o1O: function(C) {
        this.source = C;
        this.dataview = C;
        var A = this.source,
        B = this._ids;
        for (var _ = 0, D = A.length; _ < D; _++) {
            var $ = A[_];
            $._id = mini.DataSource.RecordId++;
            B[$._id] = $
        }
    },
    clear: function() {
        this._init();
        this.llOoO();
        this[lOO1lo]("clear")
    },
    updateRecord: function($, B, _) {
        if (mini.isNull($)) return;
        if (typeof B == "string") {
            var C = $[B];
            if (mini[ll00oO](C, _)) return false;
            this.beginChange();
            $[B] = _;
            this._setModified($, B, C);
            this.endChange()
        } else {
            this.beginChange();
            for (var A in B) {
                var C = $[A],
                _ = B[A];
                if (mini[ll00oO](C, _)) continue;
                $[A] = _;
                this._setModified($, A, C)
            }
            this.endChange()
        }
        this[lOO1lo]("update", {
            record: $
        })
    },
    deleteRecord: function($) {
        this._setDeleted($);
        this.llOoO();
        this[lOO1lo]("delete", {
            record: $
        })
    },
    getbyId: function($) {
        $ = typeof $ == "object" ? $._id: $;
        return this._ids[$]
    },
    isModified: function(A, _) {
        var $ = this.loo1O1[A._id];
        if (!$) return false;
        if (mini.isNull(_)) return false;
        return $.hasOwnProperty(_)
    },
    hasRecord: function($) {
        return !! this.getbyId($)
    },
    findRecords: function(D, A) {
        var F = typeof D == "function",
        I = D,
        E = A || this,
        C = this.source,
        B = [];
        for (var _ = 0, H = C.length; _ < H; _++) {
            var $ = C[_];
            if (F) {
                var G = I[Ool00](E, $);
                if (G == true) B[B.length] = $;
                if (G === 1) break
            } else if ($[D] == A) B[B.length] = $
        }
        return B
    },
    each: function(A, _) {
        var $ = this[O010ol]().clone();
        _ = _ || this;
        mini.forEach($, A, _)
    },
    getCount: function() {
        return this[O010ol]().length
    },
    __changeCount: 0,
    beginChange: function() {
        this.__changeCount++
    },
    endChange: function($) {
        this.__changeCount--;
        if (this.__changeCount < 0) this.__changeCount = 0;
        if (($ !== false && this.__changeCount == 0) || $ == true) {
            this.__changeCount = 0;
            this.llOoO()
        }
    },
    llOoO: function() {
        if (this.__changeCount == 0) this[lOO1lo]("datachanged")
    },
    _setAdded: function($) {
        $._id = mini.DataSource.RecordId++;
        $._state = "added";
        this._ids[$._id] = $;
        delete this.loo1O1[$._id]
    },
    _setModified: function($, A, B) {
        if ($._state != "added" && $._state != "deleted" && $._state != "removed") {
            $._state = "modified";
            var _ = this.oOlOO($);
            if (!_.hasOwnProperty(A)) _[A] = B
        }
    },
    _setDeleted: function($) {
        if ($._state != "added" && $._state != "deleted" && $._state != "removed") $._state = "deleted"
    },
    _setRemoved: function($) {
        delete this._ids[$._id];
        if ($._state != "added" && $._state != "removed") {
            $._state = "removed";
            delete this.loo1O1[$._id];
            this._removeds.push($)
        }
    },
    oOlOO: function($) {
        var A = $._id,
        _ = this.loo1O1[A];
        if (!_) _ = this.loo1O1[A] = {};
        return _
    },
    oO0oll: null,
    Oo0l1o: null,
    Oo1ll: [],
    multiSelect: false,
    oo00lo: function() {
        for (var _ = this.Oo1ll.length - 1; _ >= 0; _--) {
            var $ = this.Oo1ll[_],
            A = this.getbyId($._id);
            if (!A) {
                this.Oo1ll.removeAt(_);
                delete this.oO0oll[$._id]
            }
        }
        if (this.Oo0l1o && this.getbyId(this.Oo0l1o._id) == null) this.Oo0l1o = null
    },
    setMultiSelect: function($) {
        if (this[o1lloO] != $) {
            this[o1lloO] = $;
            if ($ == false);
        }
    },
    isSelected: function($) {
        if (!$) return false;
        if (typeof $ != "string") $ = $._id;
        return !! this.oO0oll[$]
    },
    getSelecteds: function() {
        return this.Oo1ll.clone()
    },
    getSelected: function() {
        return this.Oo0l1o
    },
    select: function($) {
        if (typeof $ == "number") $ = this[ooO0Ol]($);
        if (!$) return;
        if (this[OoOllo]($)) return;
        this.Oo0l1o = $;
        if (this[o1lloO] == false) {
            if (this.Oo1ll.length > 0) this._onSelectionChanged(this.Oo1ll, false);
            this.Oo1ll = [];
            this.oO0oll = {}
        }
        this[O0Ooo1]([$])
    },
    deselect: function($) {
        if (typeof $ == "number") $ = this[ooO0Ol]($);
        if (!$) return;
        if (!this[OoOllo]($)) return;
        if (this.Oo0l1o == $) this.Oo0l1o = null;
        this[OlOl00]([$])
    },
    selectAll: function() {
        var $ = this[O010ol]().clone();
        this[O0Ooo1]($)
    },
    deselectAll: function() {
        this[OlOl00](this.Oo1ll)
    },
    selects: function(B) {
        if (!B || B.length == 0) return;
        if (this[o1lloO] == false) {
            if (this.Oo1ll.length > 0) this._onSelectionChanged(this.Oo1ll, false);
            this.Oo1ll = [];
            this.oO0oll = {};
            B = [B[0]]
        }
        var A = this.oO0oll;
        for (var _ = 0, C = B.length; _ < C; _++) {
            var $ = B[_];
            if (!A[$._id]) {
                this.Oo1ll.push($);
                A[$._id] = $
            }
        }
        this._onSelectionChanged(B, true)
    },
    deselects: function(B) {
        if (!B || B.length == 0) return;
        var A = this.oO0oll;
        B = B.clone();
        for (var _ = B.length - 1; _ >= 0; _--) {
            var $ = B[_];
            if (A[$._id]) {
                this.Oo1ll.remove($);
                delete A[$._id]
            }
        }
        this._onSelectionChanged(B, false)
    },
    _onSelectionChanged: function($, A) {
        var _ = {
            records: $,
            select: A
        };
        this[lOO1lo]("SelectionChanged", _)
    },
    _filterInfo: null,
    _sortInfo: null,
    filter: function(_, $) {
        if (typeof _ != "function") return;
        $ = $ || this;
        this._filterInfo = [_, $];
        this.OoO1oo();
        this.o1oO1O();
        this.llOoO();
        this[lOO1lo]("filter")
    },
    clearFilter: function() {
        if (!this._filterInfo) return;
        this._filterInfo = null;
        this.OoO1oo();
        this.o1oO1O();
        this.llOoO();
        this[lOO1lo]("filter")
    },
    sort: function(_, $) {
        if (typeof _ != "function") return;
        $ = $ || this;
        this._sortInfo = [_, $];
        this.o1oO1O();
        this.llOoO();
        this[lOO1lo]("sort")
    },
    clearSort: function() {
        this._sortInfo = null;
        this.OoO1oo();
        this.llOoO();
        this[lOO1lo]("filter")
    }
});
mini.DataSource.RecordId = 1;
mini.DataTable = function() {
    mini.DataTable[oOOOoO][lllOo0][Ool00](this)
};
ol1O(mini.DataTable, mini.DataSource, {
    _init: function() {
        mini.DataTable[oOOOoO]._init[Ool00](this);
        this._filterInfo = null;
        this._sortInfo = null
    },
    add: function($) {
        return this.insert(this.source.length, $)
    },
    addRange: function($) {
        this.insertRange(this.source.length, $)
    },
    insert: function($, _) {
        if (!_) return null;
        if (!mini.isNumber($)) return null;
        var B = this.dataview[$];
        if (B) this.dataview.insert($, _);
        else this.dataview[O0olo1](_);
        if (this.dataview != this.source) if (B) {
            var A = this.source[looo1l](B);
            this.source.insert(A, _)
        } else this.source[O0olo1](_);
        this._setAdded(_);
        this.llOoO();
        var C = {
            index: $,
            record: _
        };
        this[lOO1lo]("add", C)
    },
    insertRange: function($, B) {
        if (!mini.isArray(B)) return;
        this.beginChange();
        for (var A = 0, C = B.length; A < C; A++) {
            var _ = B[A];
            this.insert($ + A, _)
        }
        this.endChange()
    },
    remove: function(_) {
        var $ = this[looo1l](_);
        return this.removeAt($)
    },
    removeAt: function($) {
        var _ = this[ooO0Ol]($);
        if (!_) return null;
        this.source.removeAt($);
        if (this.dataview !== this.source) this.dataview.removeAt($);
        this._setRemoved(_);
        this.oo00lo();
        this.llOoO();
        var A = {
            record: _
        };
        this[lOO1lo]("remove", A)
    },
    removeRange: function(A) {
        if (!mini.isArray(A)) return;
        this.beginChange();
        for (var _ = 0, B = A.length; _ < B; _++) {
            var $ = A[_];
            this.remove($)
        }
        this.endChange()
    },
    move: function(_, H) {
        if (!_ || !mini.isNumber(H)) return;
        if (mini.isArray(_)) {
            this.beginChange();
            var I = _,
            C = this[ooO0Ol](H),
            F = this;
            mini[o01oOl](I, 
            function($, _) {
                return F[looo1l]($) > F[looo1l](_)
            },
            this);
            for (var E = 0, D = I.length; E < D; E++) {
                var A = I[E],
                $ = this[looo1l](C);
                this.move(A, $)
            }
            this.endChange();
            return
        }
        var B = this.dataview[H];
        this.dataview.remove(_);
        var G = this.dataview[looo1l](B);
        if (G != -1) H = G;
        if (B) this.dataview.insert(H, _);
        else this.dataview[O0olo1](_);
        if (this.dataview != this.source) {
            this.source.remove(_);
            G = this.source[looo1l](B);
            if (G != -1) H = G;
            if (B) this.source.insert(H, _);
            else this.source[O0olo1](_)
        }
        this.llOoO();
        var J = {
            index: H,
            record: _
        };
        this[lOO1lo]("move", J)
    },
    indexOf: function($) {
        return this.dataview[looo1l]($)
    },
    getAt: function($) {
        return this.dataview[$]
    },
    getRange: function(A, B) {
        if (A > B) {
            var C = A;
            A = B;
            B = C
        }
        var D = [];
        for (var _ = A, E = B; _ <= E; _++) {
            var $ = this.dataview[_];
            D.push($)
        }
        return D
    },
    selectRange: function($, _) {
        if (!mini.isNumber($)) $ = this[looo1l]($);
        if (!mini.isNumber(_)) _ = this[looo1l](_);
        if (mini.isNull($) || mini.isNull(_)) return;
        var A = this[olOoo1]($, _);
        this[O0Ooo1](A)
    },
    toArray: function() {
        return this.source.clone()
    },
    getChanges: function(B) {
        var A = [];
        if (B == "removed" || B == null) A.addRange(this._removeds.clone());
        for (var _ = 0, C = this.source.length; _ < C; _++) {
            var $ = this.source[_];
            if (!$._state) continue;
            if ($._state == B || B == null) A[A.length] = $
        }
        return A
    },
    accept: function() {
        this.beginChange();
        for (var _ = 0, A = this.source.length; _ < A; _++) {
            var $ = this.source[_];
            this[Ool100]($)
        }
        this._removeds = [];
        this.loo1O1 = {};
        this.endChange()
    },
    reject: function() {
        this.beginChange();
        for (var _ = 0, A = this.source.length; _ < A; _++) {
            var $ = this.source[_];
            this.rejectRecord($)
        }
        this._removeds = [];
        this.loo1O1 = {};
        this.endChange()
    },
    acceptRecord: function($) {
        delete this.loo1O1[$._id];
        if ($._state == "deleted") this[O11Oo0]($);
        else {
            delete $._state;
            delete this.loo1O1[$._id];
            this.llOoO()
        }
    },
    rejectRecord: function(_) {
        if (_._state == "added") this[O11Oo0](_);
        else if (_._state == "modified" || _._state == "deleted") {
            var $ = this.oOlOO(_);
            mini.copyTo(_, $);
            delete _._state;
            delete this.loo1O1[_._id];
            this.llOoO()
        }
    },
    OoO1oo: function() {
        if (!this._filterInfo) {
            this.dataview = this.source;
            return
        }
        var F = this._filterInfo[0],
        D = this._filterInfo[1],
        $ = [],
        C = this.source;
        for (var _ = 0, E = C.length; _ < E; _++) {
            var B = C[_],
            A = F[Ool00](D, B, _, this);
            if (A !== false) $.push(B)
        }
        this.dataview = $
    },
    o1oO1O: function() {
        if (!this._sortInfo) return;
        var A = this._sortInfo[0],
        _ = this._sortInfo[1],
        $ = this[O010ol]().clone();
        mini[o01oOl]($, A, _);
        this.dataview = $
    }
});
ooOl0(mini.DataTable, "datatable");
mini.DataTree = function() {
    mini.DataTree[oOOOoO][lllOo0][Ool00](this)
};
ol1O(mini.DataTree, mini.DataSource, {
    idField: "id",
    parentField: "pid",
    nodesField: "children",
    isTree: true,
    setIdField: function($) {
        this[o1ll0o] = $
    },
    setParentField: function($) {
        this[oOo11] = $
    },
    setNodesField: function($) {
        if (this.nodesField != $) {
            var _ = this.root[this.nodesField];
            this.nodesField = $;
            this.O0o1O(_)
        }
    },
    _init: function() {
        mini.DataTree[oOOOoO]._init[Ool00](this);
        this.source = this.root = {
            _id: -1,
            _level: -1
        };
        this.root[this.nodesField] = [];
        this.viewNodes = null;
        this.dataview = null
    },
    O0o1O: function(D) {
        this.root[this.nodesField] = D || [];
        this.viewNodes = null;
        this.dataview = null;
        var A = mini[ol0oo1](D, this.nodesField),
        B = this._ids;
        B[this.root._id] = this.root;
        for (var _ = 0, E = A.length; _ < E; _++) {
            var C = A[_];
            C._id = mini.DataSource.RecordId++;
            B[C._id] = C
        }
        A = mini[ol0oo1](D, this.nodesField, "_id", "_pid", this.root._id);
        for (_ = 0, E = A.length; _ < E; _++) {
            var C = A[_],
            $ = this[l10l00](C);
            C._pid = $._id;
            C._level = $._level + 1;
            delete C._state
        }
    },
    _setAdded: function(_) {
        var $ = this[l10l00](_);
        _._id = mini.DataSource.RecordId++;
        _._pid = $._id;
        _[this.parentField] = $[this.idField];
        _._level = $._level + 1;
        _._state = "added";
        this._ids[_._id] = _;
        delete this.loo1O1[_._id]
    },
    ooO1lO: function($) {
        var _ = $[this.nodesField];
        if (!_) _ = $[this.nodesField] = [];
        if (this.viewNodes && !this.viewNodes[$._id]) this.viewNodes[$._id] = [];
        return _
    },
    addNode: function(_, $) {
        if (!_) return;
        return this.insertNode(_, -1, $)
    },
    addNodes: function(_, $) {
        if (!mini.isArray(_)) return;
        return this.insertNodes(_, -1, $)
    },
    insertNodes: function(D, $, A) {
        if (!mini.isNumber($)) return;
        if (!mini.isArray(D)) return;
        if (!A) A = this.root;
        this.beginChange();
        var B = this.ooO1lO(A);
        if ($ < 0 || $ > B.length) $ = B.length;
        D = D.clone();
        for (var _ = 0, C = D.length; _ < C; _++) this.insertNode(D[_], $ + _, A);
        this.endChange();
        return D
    },
    removeNode: function(A) {
        var _ = this[l10l00](A);
        if (!_) return;
        var $ = this.indexOfNode(A);
        return this.removeNodeAt($, _)
    },
    removeNodes: function(A) {
        if (!mini.isArray(A)) return;
        this.beginChange();
        A = A.clone();
        for (var $ = 0, _ = A.length; $ < _; $++) this[O11Oo0](A[$]);
        this.endChange()
    },
    moveNodes: function(E, B, _) {
        if (!E || E.length == 0 || !B || !_) return;
        this.beginChange();
        var A = this;
        mini[o01oOl](E, 
        function($, _) {
            return A[looo1l]($) > A[looo1l](_)
        },
        this);
        for (var $ = 0, D = E.length; $ < D; $++) {
            var C = E[$];
            this[lo0oo0](C, B, _);
            if ($ != 0) {
                B = C;
                _ = "after"
            }
        }
        this.endChange()
    },
    moveNode: function(E, D, B) {
        if (!E || !D || mini.isNull(B)) return;
        if (this.viewNodes) {
            var _ = D,
            $ = B;
            if ($ == "before") {
                _ = this[l10l00](D);
                $ = this.indexOfNode(D)
            } else if ($ == "after") {
                _ = this[l10l00](D);
                $ = this.indexOfNode(D) + 1
            } else if ($ == "add" || $ == "append") {
                if (!_[this.nodesField]) _[this.nodesField] = [];
                $ = _[this.nodesField].length
            } else if (!mini.isNumber($)) return;
            if (this[oooO1o](E, _)) return false;
            var A = this[o01O00](_);
            if ($ < 0 || $ > A.length) $ = A.length;
            var F = {};
            A.insert($, F);
            var C = this[l10l00](E),
            G = this[o01O00](C);
            G.remove(E);
            $ = A[looo1l](F);
            A[$] = E
        }
        _ = D,
        $ = B,
        A = this.ooO1lO(_);
        if ($ == "before") {
            _ = this[l10l00](D);
            A = this.ooO1lO(_);
            $ = A[looo1l](D)
        } else if ($ == "after") {
            _ = this[l10l00](D);
            A = this.ooO1lO(_);
            $ = A[looo1l](D) + 1
        } else if ($ == "add" || $ == "append") $ = A.length;
        else if (!mini.isNumber($)) return;
        if (this[oooO1o](E, _)) return false;
        if ($ < 0 || $ > A.length) $ = A.length;
        F = {};
        A.insert($, F);
        C = this[l10l00](E);
        C[this.nodesField].remove(E);
        $ = A[looo1l](F);
        A[$] = E;
        this.Oll1OO(E, _);
        this.llOoO();
        var H = {
            parentNode: _,
            index: $,
            node: E
        };
        this[lOO1lo]("move", H)
    },
    insertNode: function(A, $, _) {
        if (!A) return;
        if (!_) _ = this.root;
        if (!mini.isNumber($)) {
            switch ($) {
            case "before":
                $ = this.indexOfNode(_);
                _ = this[l10l00](_);
                this.insertNode(A, $, _);
                break;
            case "after":
                $ = this.indexOfNode(_);
                _ = this[l10l00](_);
                this.insertNode(A, $ + 1, _);
                break;
            case "append":
            case "add":
                this[O10O10](A, _);
                break;
            default:
                break
            }
            return
        }
        var C = this.ooO1lO(_),
        D = this[o01O00](_);
        if ($ < 0) $ = D.length;
        D.insert($, A);
        $ = D[looo1l](A);
        if (this.viewNodes) {
            var B = D[$ - 1];
            if (B) {
                var E = C[looo1l](B);
                C.insert(E + 1, A)
            } else C.insert(0, A)
        }
        A._pid = _._id;
        this._setAdded(A);
        this[lllolO](A, 
        function(A, $, _) {
            A._pid = _._id;
            this._setAdded(A)
        },
        this);
        this.llOoO();
        var F = {
            parentNode: _,
            index: $,
            node: A
        };
        this[lOO1lo]("add", F);
        return A
    },
    removeNodeAt: function($, _) {
        if (!_) _ = this.root;
        var C = this[o01O00](_),
        A = C[$];
        if (!A) return null;
        C.removeAt($);
        if (this.viewNodes) {
            var B = _[this.nodesField];
            B.remove(A)
        }
        this._setRemoved(A);
        this[lllolO](A, 
        function(A, $, _) {
            this._setRemoved(A)
        },
        this);
        this.oo00lo();
        this.llOoO();
        var D = {
            parentNode: _,
            index: $,
            node: A
        };
        this[lOO1lo]("remove", D);
        return A
    },
    bubbleParent: function(_, B, A) {
        A = A || this;
        if (_) B[Ool00](this, _);
        var $ = this[l10l00](_);
        if ($ && $ != this.root) this[lo1Ooo]($, B, A)
    },
    cascadeChild: function(A, E, B) {
        if (!E) return;
        if (!A) A = this.root;
        var D = A[this.nodesField];
        if (D) {
            D = D.clone();
            for (var $ = 0, C = D.length; $ < C; $++) {
                var _ = D[$];
                if (E[Ool00](B || this, _, $, A) === false) return;
                this[lllolO](_, E, B)
            }
        }
    },
    eachChild: function(B, F, C) {
        if (!F || !B) return;
        var E = B[this.nodesField];
        if (E) {
            var _ = E.clone();
            for (var A = 0, D = _.length; A < D; A++) {
                var $ = _[A];
                if (F[Ool00](C || this, $, A, B) === false) break
            }
        }
    },
    collapseLevel: function($, _) {
        this.beginChange();
        this.each(function(A) {
            var B = this[O010O0](A);
            if ($ == B) this[O1lolO](A, _)
        },
        this);
        this.endChange()
    },
    expandLevel: function($, _) {
        this.beginChange();
        this.each(function(A) {
            var B = this[O010O0](A);
            if ($ == B) this[OOlOO0](A, _)
        },
        this);
        this.endChange()
    },
    collapse: function($, _) {
        if (!$) return;
        this.beginChange();
        $.expanded = false;
        if (_) this[ll0Olo]($, 
        function($) {
            if ($[this.nodesField] != null) this[O1lolO]($, _)
        },
        this);
        this.endChange();
        var A = {
            node: $
        };
        this[lOO1lo]("collapse", A)
    },
    expand: function($, _) {
        if (!$) return;
        this.beginChange();
        $.expanded = true;
        if (_) this[ll0Olo]($, 
        function($) {
            if ($[this.nodesField] != null) this[OOlOO0]($, _)
        },
        this);
        this.endChange();
        var A = {
            node: $
        };
        this[lOO1lo]("expand", A)
    },
    toggle: function($) {
        if (this[O0O0o1]($)) this[O1lolO]($);
        else this[OOlOO0]($)
    },
    collapseAll: function() {
        this[O1lolO](this.root, true)
    },
    expandAll: function() {
        this[OOlOO0](this.root, true)
    },
    isAncestor: function(_, B) {
        if (_ == B) return true;
        if (!_ || !B) return false;
        var A = this[o0lO0l](B);
        for (var $ = 0, C = A.length; $ < C; $++) if (A[$] == _) return true;
        return false
    },
    getAncestors: function(A) {
        var _ = [];
        while (1) {
            var $ = this[l10l00](A);
            if (!$ || $ == this.root) break;
            _[_.length] = $;
            A = $
        }
        _.reverse();
        return _
    },
    getRootNode: function() {
        return this.root
    },
    getParentNode: function($) {
        if (!$) return null;
        return this.getbyId($._pid)
    },
    getChildNodes: function(A, C, B) {
        var G = A[this.nodesField];
        if (this.viewNodes && B !== false) G = this.viewNodes[A._id];
        if (C === true && G) {
            var $ = [];
            for (var _ = 0, F = G.length; _ < F; _++) {
                var D = G[_];
                $[$.length] = D;
                var E = this[o01O00](D, C, B);
                if (E && E.length > 0) $.addRange(E)
            }
            G = $
        }
        return G || []
    },
    getChildNodeAt: function($, _) {
        var A = this[o01O00](_);
        if (A) return A[$];
        return null
    },
    hasChildNodes: function($) {
        var _ = this[o01O00]($);
        return _.length > 0
    },
    getLevel: function($) {
        return $._level
    },
    isLeaf: function($) {
        if (!$ || $[llo0l1] === false) return false;
        var _ = this[o01O00]($);
        if (_.length > 0) return false;
        return true
    },
    isFirstNode: function(_) {
        if (_ == this.root) return true;
        var $ = this[l10l00](_);
        if (!$) return false;
        return this.getFirstNode($) == _
    },
    isLastNode: function(_) {
        if (_ == this.root) return true;
        var $ = this[l10l00](_);
        if (!$) return false;
        return this.getLastNode($) == _
    },
    isExpanded: function($) {
        return $.expanded == true || $.expanded == 1 || mini.isNull($.expanded)
    },
    isVisible: function(_) {
        var $ = this._ids[_._pid];
        if (!$ || $ == this.root) return true;
        if ($.expanded === false) return false;
        return this.isVisible($)
    },
    getNextNode: function(A) {
        var _ = this.getbyId(A._pid);
        if (!_) return null;
        var $ = this.indexOfNode(A);
        return this[o01O00](_)[$ + 1]
    },
    getPrevNode: function(A) {
        var _ = this.getbyId(A._pid);
        if (!_) return null;
        var $ = this.indexOfNode(A);
        return this[o01O00](_)[$ - 1]
    },
    getFirstNode: function($) {
        return this[o01O00]($)[0]
    },
    getLastNode: function($) {
        var _ = this[o01O00]($);
        return _[_.length - 1]
    },
    indexOfNode: function(_) {
        var $ = this.getbyId(_._pid);
        if ($) return this[o01O00]($)[looo1l](_);
        return - 1
    },
    getAt: function($) {
        return this[O010ol]()[$]
    },
    indexOf: function($) {
        return this[O010ol]()[looo1l]($)
    },
    getRange: function(A, C) {
        if (A > C) {
            var D = A;
            A = C;
            C = D
        }
        var B = this[o01O00](this.root, true),
        E = [];
        for (var _ = A, F = C; _ <= F; _++) {
            var $ = B[_];
            if ($) E.push($)
        }
        return E
    },
    selectRange: function($, A) {
        var _ = this[o01O00](this.root, true);
        if (!mini.isNumber($)) $ = _[looo1l]($);
        if (!mini.isNumber(A)) A = _[looo1l](A);
        if (mini.isNull($) || mini.isNull(A)) return;
        var B = this[olOoo1]($, A);
        this[O0Ooo1](B)
    },
    findRecords: function(D, A) {
        var C = this[llOo1l](),
        F = typeof D == "function",
        I = D,
        E = A || this,
        B = [];
        for (var _ = 0, H = C.length; _ < H; _++) {
            var $ = C[_];
            if (F) {
                var G = I[Ool00](E, $);
                if (G == true) B[B.length] = $;
                if (G === 1) break
            } else if ($[D] == A) B[B.length] = $
        }
        return B
    },
    llOoOCount: 0,
    llOoO: function() {
        this.llOoOCount++;
        this.dataview = null;
        if (this.__changeCount == 0) this[lOO1lo]("datachanged")
    },
    createDataView: function() {
        var B = this[o01O00](this.root, true),
        $ = [];
        for (var _ = 0, C = B.length; _ < C; _++) {
            var A = B[_];
            if (this.isVisible(A)) $[$.length] = A
        }
        return $
    },
    getDataView: function() {
        if (!this.dataview) this.dataview = this.createDataView();
        return this.dataview
    },
    OoO1oo: function() {
        if (!this._filterInfo) {
            this.viewNodes = null;
            return
        }
        var C = this._filterInfo[0],
        B = this._filterInfo[1],
        A = this.viewNodes = {},
        _ = this.nodesField;
        function $(G) {
            var J = G[_];
            if (!J) return false;
            var K = G._id,
            H = A[K] = [];
            for (var D = 0, I = J.length; D < I; D++) {
                var F = J[D],
                L = $(F),
                E = C[Ool00](B, F, D, this);
                if (E !== false || L) H.push(F)
            }
            return H.length > 0
        }
        $(this.root)
    },
    o1oO1O: function() {
        if (!this._filterInfo && !this._sortInfo) {
            this.viewNodes = null;
            return
        }
        if (!this._sortInfo) return;
        var D = this._sortInfo[0],
        C = this._sortInfo[1],
        $ = this.nodesField;
        if (!this.viewNodes) {
            var B = this.viewNodes = {};
            B[this.root._id] = this.root[$].clone();
            this[lllolO](this.root, 
            function(A, _, C) {
                var D = A[$];
                if (D) B[A._id] = D.clone()
            })
        }
        var A = this;
        function _(E) {
            var G = A[o01O00](E);
            mini[o01oOl](G, D, C);
            for (var $ = 0, F = G.length; $ < F; $++) {
                var B = G[$];
                _(B)
            }
        }
        _(this.root)
    },
    toArray: function() {
        if (!this._array || this.llOoOCount != this.llOoOCount2) {
            this.llOoOCount2 = this.llOoOCount;
            this._array = this[o01O00](this.root, true, false)
        }
        return this._array
    },
    toTree: function() {
        return this.root[this.nodesField]
    },
    getChanges: function(_) {
        var $ = [];
        if (_ == "removed" || _ == null) $.addRange(this._removeds.clone());
        this[lllolO](this.root, 
        function(B, A, C) {
            if (B._state == null || B._state == "") return;
            if (B._state == _ || _ == null) $[$.length] = B
        },
        this);
        return $
    },
    accept: function($) {
        $ = $ || this.root;
        this.beginChange();
        this[lllolO](this.root, 
        function($) {
            this[Ool100]($)
        },
        this);
        this._removeds = [];
        this.loo1O1 = {};
        this.endChange()
    },
    reject: function($) {
        this.beginChange();
        this[lllolO](this.root, 
        function($) {
            this.rejectRecord($)
        },
        this);
        this._removeds = [];
        this.loo1O1 = {};
        this.endChange()
    },
    acceptRecord: function($) {
        delete this.loo1O1[$._id];
        if ($._state == "deleted") this[O11Oo0]($);
        else {
            delete $._state;
            delete this.loo1O1[$._id];
            this.llOoO()
        }
    },
    rejectRecord: function(_) {
        if (_._state == "added") this[O11Oo0](_);
        else if (_._state == "modified" || _._state == "deleted") {
            var $ = this.oOlOO(_);
            mini.copyTo(_, $);
            delete _._state;
            delete this.loo1O1[_._id];
            this.llOoO()
        }
    },
    upGrade: function(F) {
        var C = this[l10l00](F);
        if (C == this.root || F == this.root) return false;
        var E = C[this.nodesField],
        _ = E[looo1l](F),
        G = F[this.nodesField] ? F[this.nodesField].length: 0;
        for (var B = E.length - 1; B >= _; B--) {
            var $ = E[B];
            E.removeAt(B);
            if ($ != F) {
                if (!F[this.nodesField]) F[this.nodesField] = [];
                F[this.nodesField].insert(G, $)
            }
        }
        var D = this[l10l00](C),
        A = D[this.nodesField],
        _ = A[looo1l](C);
        A.insert(_ + 1, F);
        this.Oll1OO(F, D);
        this.OoO1oo();
        this.llOoO()
    },
    downGrade: function(B) {
        if (this[O1O00](B)) return false;
        var A = this[l10l00](B),
        C = A[this.nodesField],
        $ = C[looo1l](B),
        _ = C[$ - 1];
        C.removeAt($);
        if (!_[this.nodesField]) _[this.nodesField] = [];
        _[this.nodesField][O0olo1](B);
        this.Oll1OO(B, _);
        this.OoO1oo();
        this.llOoO()
    },
    Oll1OO: function(_, $) {
        _._pid = $._id;
        _._level = $._level + 1;
        this[lllolO](_, 
        function(A, $, _) {
            A._pid = _._id;
            A._level = _._level + 1;
            A[this.parentField] = _[this.idField]
        },
        this);
        this._setModified(_)
    }
});
ooOl0(mini.DataTree, "datatree");
mini.DataTableApplys = {
    clear: function() {
        this.data.clear()
    },
    loadData: function($) {
        this[l1OlOo]($)
    },
    getCount: function() {
        return this.data[lOoo10]()
    },
    getChanges: function() {
        return this.data[oOoOo0]()
    },
    getData: function() {
        return this.data[llOo1l]()
    },
    toArray: function() {
        return this.data[llOo1l]()
    },
    getRows: function() {
        return this.data[llOo1l]()
    },
    updateRow: function($, A, _) {
        this.data.updateRecord($, A, _)
    },
    addRow: function($) {
        return this.data[O0olo1]($)
    },
    insertRow: function($, _) {
        return this.data.insert($, _)
    },
    removeRow: function($) {
        return this.data.remove($)
    },
    removeRowAt: function($) {
        return this.data.removeAt($)
    },
    moveRow: function(_, $) {
        this.data.move(_, $)
    },
    indexOf: function($) {
        return this.data[looo1l]($)
    },
    getAt: function($) {
        return this.data[ooO0Ol]($)
    },
    findRows: function(_, $) {
        return this.findRecords(_, $)
    },
    findRecords: function(_, $) {
        return this.data.findRecords(_, $)
    },
    removeSelected: function(A) {
        var _ = this[llllOo](),
        $ = this[looo1l](_);
        this[lOOlOo](_);
        if (A !== false) {
            _ = this[ooO0Ol]($);
            this[OlOlo1](_ ? $: $ - 1)
        }
    },
    getSelected: function() {
        return this.data[llllOo]()
    },
    getSelecteds: function() {
        return this.data[O1O0oo]()
    },
    select: function($) {
        this.data[OlOlo1]($)
    },
    selects: function($) {
        this.data[O0Ooo1]($)
    },
    deselect: function($) {
        this.data[O011oO]($)
    },
    deselects: function($) {
        this.data[OlOl00]($)
    },
    selectAll: function() {
        this.data[l110Oo]()
    },
    deselectAll: function() {
        this.data[oool00]()
    },
    isSelected: function($) {
        return this.data[OoOllo]($)
    },
    getRange: function($, _) {
        if (mini.isNull($) || mini.isNull(_)) return;
        return this.data[olOoo1]($, _)
    },
    selectRange: function($, _) {
        this.data[oo0llo]($, _)
    },
    filter: function(_, $) {
        this.data[ol1l00](_, $)
    },
    clearFilter: function() {
        this.data[lO10o1]()
    },
    sort: function(_, $) {
        this.data[o01oOl](_, $)
    },
    clearSort: function() {
        this.data[loOOl1]()
    }
};
mini.DataTreeApplys = {
    isLeaf: function($) {
        return this.data[llo0l1]($)
    },
    getLevel: function($) {
        return $ ? $._level: 0
    },
    isExpanded: function($) {
        return this.data[O0O0o1]($)
    },
    getChildNodes: function($) {
        return this.data[o01O00]($)
    },
    getParentNode: function($) {
        return this.data[l10l00]($)
    },
    isAncestor: function(_, $) {
        return this.data[oooO1o](_, $)
    },
    getAncestors: function($) {
        return this.data[o0lO0l]($)
    },
    getRootNode: function($) {
        return this.data[OoOll0]($)
    },
    getAncestors: function($) {
        return this.data[o0lO0l]($)
    },
    hasChildNodes: function($) {
        return this.data.hasChildNodes($)
    },
    indexOfNode: function($) {
        return this.data.indexOfNode($)
    },
    updateNode: function(_, A, $) {
        this.data.updateRecord(_, A, $)
    },
    addNode: function(_, $) {
        return this.data[O10O10](_, $)
    },
    insertNode: function(A, $, _) {
        return this.data.insertNode(A, $, _)
    },
    removeNodeAt: function($, _) {
        return this.data.removeNodeAt($, _)
    },
    removeNode: function($) {
        return this.data[O11Oo0]($)
    },
    moveNode: function(A, $, _) {
        this.data[lo0oo0](A, $, _)
    },
    addNodes: function(_, $) {
        return this.data[l10l11](_, $)
    },
    insertNodes: function(A, $, _) {
        return this.data.insertNodes($, A, _)
    },
    moveNodes: function(A, $, _) {
        this.data[lo0o0l](A, $, _)
    },
    removeNodes: function($) {
        return this.data[ll10O0]($)
    },
    findNodes: function(_, $) {
        return this.data.findRecords(_, $)
    },
    getChanges: function() {
        return this.data[oOoOo0]()
    },
    getData: function() {
        return this.data.toTree()
    },
    bubbleParent: function($, A, _) {
        this.data[lo1Ooo]($, A, _)
    },
    cascadeChild: function($, A, _) {
        this.data[lllolO]($, A, _)
    },
    eachChild: function($, A, _) {
        this.data[ll0Olo]($, A, _)
    },
    collapseLevel: function($, _) {
        this.data[lo1Ol0]($, _)
    },
    expandLevel: function($, _) {
        this.data[loOo1o]($, _)
    },
    collapse: function($, _) {
        this.data[O1lolO]($, _)
    },
    expand: function($, _) {
        this.data[OOlOO0]($, _)
    },
    toggle: function($) {
        this.data[O00l11]($)
    },
    collapseAll: function() {
        this.data[OOl0o0]()
    },
    expandAll: function() {
        this.data[ollOo]()
    },
    filter: function(_, $) {
        this.data[ol1l00](_, $)
    },
    clearFilter: function() {
        this.data[lO10o1]()
    },
    sort: function(_, $) {
        this.data[o01oOl](_, $)
    },
    clearSort: function() {
        this.data[loOOl1]()
    }
};
loo0o1 = function() {
    loo0o1[oOOOoO][lllOo0][Ool00](this);
    this.columns = [];
    this.viewColumns = [];
    this[l1OlOo]([])
};
ol1O(loo0o1, lo0O01, {
    width: 300,
    height: 150,
    virtualModel: false,
    data: null,
    _rowIdField: "_id",
    multiSelect: false,
    allowRowSelect: true,
    allowCellSelect: true,
    allowAlternating: true,
    allowResizeColumn: true,
    allowMoveColumn: true,
    allowSortColumn: true,
    allowDragDrop: false,
    showDirty: true,
    scrollLeft: 0,
    scrollTop: 0,
    scrollWidth: 0,
    scrollHeight: 0,
    headerHeight: 23,
    showHScroll: true,
    showVScroll: true,
    columnWidth: 100,
    rowHeight: 21,
    columnMinWidth: 10,
    columnMaxWidth: 800,
    OO0101: "mini-supergrid-row",
    cellCls: "mini-supergrid-cell",
    Oool: "mini-supergrid-rowselected",
    cellSelectedCls: "mini-supergrid-cellselected",
    oo1oo: "mini-supergrid-alternating",
    uiCls: "mini-supergrid",
    lO0O: "mini-supergrid-frozenCell",
    frozenStartColumn: -1,
    frozenEndColumn: -1,
    _vscrollTimer: null,
    _hscrollTimer: null
});
llo1o = loo0o1[O0lloO];
llo1o[l1OllO] = lOolO;
llo1o.l1O0oO = oOo1l;
llo1o.o01O0O = lo1l1;
llo1o.O10O1l = oOo01;
llo1o.o0oOOo = oooOl;
llo1o.O001oO = l1l00;
llo1o.lloO = o1lo0;
llo1o[o11loo] = Olool0;
llo1o.o1o00O = lo0oO;
llo1o.oO0011 = o011O;
llo1o[Olo000] = oooo0;
llo1o[O1lloO] = OoolO;
llo1o[Ol01O0] = O0O10;
llo1o[l01loo] = oo000;
llo1o[o0Ooo0] = ool0o;
llo1o[ll0o1] = oOl00;
llo1o[oO0l1o] = l010O;
llo1o[o0O00] = Ol1Oo;
llo1o[l1O00o] = ll10l;
llo1o[ll0l0O] = olOlO;
llo1o[o1lo01] = O1lo1;
llo1o[O1l001] = Oo1O1;
llo1o.ol1O11 = oO00O;
llo1o[oO101l] = oo11o;
llo1o[ol11o] = O110O;
llo1o[o1OO0] = l00ll;
llo1o[oOlooo] = oo1llO;
llo1o[OOO0Ol] = o00l0;
llo1o[l1l0O] = l1oo1;
llo1o.l100o1 = l01O0;
llo1o.lll0l0 = ol0OO;
llo1o.lO011l = ooOol1;
llo1o.l0o1 = lol11;
llo1o[O10ol1] = ol1O1;
llo1o.ool1 = o0l0l;
llo1o.o01l1 = llO00;
llo1o.lO1O1O = Ol10o;
llo1o.o10OoO = Oll0l;
llo1o[oOlooO] = OOOo1;
llo1o[O010ol] = ol000;
llo1o[l1OlOo] = llOll;
llo1o[o101o0] = lo00O;
llo1o[O01ll1] = O0o1;
llo1o[O0oooO] = l1O10;
llo1o[Ol1ool] = O11oo;
llo1o[o0oOoO] = o10l1;
llo1o[O01OoO] = oolo;
llo1o[OOOo00] = oloOlO;
llo1o[o10l10] = Ol0lO;
llo1o[OOOol0] = l1lOO;
llo1o[lOlo11] = Oo1ol;
mini.copyTo(loo0o1.prototype, mini.DataTableApplys);
ooOl0(loo0o1, "supergrid");
mini.GridColumnModel = {
    addColumn: function(A, _, $) {
        if (!A) return;
        $ = this[lO00o]($);
        if (!$) {
            $ = this;
            if (typeof action == "string") action = "append"
        }
        if (mini.isNull(_) || _ < 0) _ = 1000;
        switch (_) {
        case "before":
            parentColumn = this[O0110]($);
            _ = parentColumn.columns[looo1l]($);
            parentColumn.columns.insert(_, A);
            break;
        case "after":
            parentColumn = this[O0110]($);
            _ = parentColumn.columns[looo1l]($);
            parentColumn.columns.insert(_ + 1, A);
            break;
        case "append":
        case "add":
            if (!$.columns) $.columns = [];
            $.columns.push(A);
            break;
        default:
            if (mini.isNumber(_)) {
                if (!$.columns) $.columns = [];
                $.columns.insert(_, A)
            }
            break
        }
        this[l1011O](this.columns)
    },
    removeColumn: function($) {
        $ = this[lO00o]($);
        var _ = this[O0110]($);
        if ($ && _) {
            _.columns.remove($);
            this[l1011O](this.columns)
        }
        return $
    },
    updateColumn: function($, _) {
        $ = this[lO00o]($);
        mini.copyTo($, _);
        this[l1011O](this.columns)
    },
    moveColumn: function(C, _, A) {
        C = this[lO00o](C);
        _ = this[lO00o](_);
        if (!C || !_ || !A || C == _) return;
        if (this[lloOO1](C, _)) return;
        var D = this[O0110](C);
        if (D) D.columns.remove(C);
        var B = _,
        $ = A;
        if ($ == "before") {
            B = this[O0110](_);
            $ = B.columns[looo1l](_)
        } else if ($ == "after") {
            B = this[O0110](_);
            $ = B.columns[looo1l](_) + 1
        } else if ($ == "add" || $ == "append") {
            if (!B.columns) B.columns = [];
            $ = B.columns.length
        } else if (!mini.isNumber($)) return;
        B.columns.insert($, C);
        this[l1011O](this.columns)
    },
    getColumn: function(_) {
        if (typeof _ == "object") return _;
        var $ = this.oO10o1[_];
        if (!$) $ = this.o10ol[_];
        return $
    },
    getParentColumn: function($) {
        $ = this[lO00o]($);
        var _ = $ ? this.o10ol[$._pid] : null;
        if ($ && !_) _ = this;
        return _
    },
    isAncestorColumn: function(_, B) {
        if (_ == B) return true;
        if (!_ || !B) return false;
        var A = this[OOll1](B);
        for (var $ = 0, C = A.length; $ < C; $++) if (A[$] == _) return true;
        return false
    },
    getAncestorColumns: function(A) {
        var _ = [];
        while (1) {
            var $ = this[O0110](A);
            if (!$ || $ == this) break;
            _[_.length] = $;
            A = $
        }
        _.reverse();
        return _
    },
    getViewColumns: function() {
        return this.viewColumns
    },
    getColumns: function($) {
        $ = this[lO00o]($);
        if (!$) $ = this;
        return $.columns
    },
    isVisibleColumn: function($) {
        $ = this[lO00o]($);
        if (!$.visible) return false;
        var _ = this[O0110]($);
        if (_ == this) return true;
        return this[oll0Ol](_)
    },
    getDisplayColumns: function($) {
        $ = this[lO00o]($);
        if (!$) $ = this;
        return $.displayColumns || []
    },
    eachColumns: function(B, F, C) {
        var D = this[lolO0O](B);
        if (D) {
            var _ = D.clone();
            for (var A = 0, E = _.length; A < E; A++) {
                var $ = _[A];
                if (F[Ool00](C, $, A, B) === false) break
            }
        }
    },
    eachDisplayColumns: function(B, F, C) {
        var D = this.getDisplayColumns(B);
        if (D) {
            var _ = D.clone();
            for (var A = 0, E = _.length; A < E; A++) {
                var $ = _[A];
                if (F[Ool00](C, $, A, B) === false) break
            }
        }
    },
    _columnId: 0,
    setColumns: function(D) {
        if (!mini.isArray(D)) D = [];
        this.columns = D;
        this.displayColumns = [];
        this.viewColumns = [];
        this.o10ol = {};
        this.oO10o1 = {};
        var _ = 0,
        $ = 0;
        function B(E, A, F) {
            this._initColumn(E);
            E.__id = this._columnId++;
            E._id = this.id + "$column$" + E.__id;
            E._pid = F._id;
            var G = E.name;
            if (G) this.oO10o1[G] = E;
            this.o10ol[E._id] = E;
            E.level = $;
            $ += 1;
            this[lo1lol](E, B, this);
            $ -= 1;
            E.displayColumns = (E.columns || []).clone();
            for (var C = E.displayColumns.length - 1; C >= 0; C--) {
                var D = E.displayColumns[C];
                if (D.visible == false) E.displayColumns.removeAt(C)
            }
            if (E.displayColumns.length == 0 && this[oll0Ol](E)) this.viewColumns.push(E);
            if (E.level > _) _ = E.level
        }
        this[lo1lol](this, B, this);
        this.displayColumns = D.clone();
        for (var A = this.displayColumns.length - 1; A >= 0; A--) {
            var C = this.displayColumns[A];
            if (C.visible == false) this.displayColumns.removeAt(A)
        }
        this.maxColumnLevel = _;
        this[O10ol1]()
    },
    _initColumn: function(column) {
        column._gridUID = this.uid;
        column[lo0lll] = this[lo0lll];
        if (column.type && column.inited != true) {
            column.typeInited = true;
            var col = mini[l1l0o1](column.type),
            _column = mini.copyTo({},
            column);
            mini.copyTo(column, col);
            mini.copyTo(column, _column)
        }
        column.width = parseInt(column.width);
        if (mini.isNull(column.width) || isNaN(column.width)) column.width = this[l01O0o];
        column.visible = column.visible !== false;
        column[l000l] = column[l000l] !== false;
        column.allowMove = column.allowMove !== false;
        column.allowSort = column.allowSort === true;
        column.allowDrag = !!column.allowDrag;
        column[O00O01] = !!column[O00O01];
        if (column.editor) {
            if (typeof column.editor == "string") {
                var cls = mini.getClass(column.editor);
                if (cls) column.editor = {
                    type: column.editor
                };
                else column.editor = eval("(" + column.editor + ")")
            }
            if (column.editor && !mini.isControl(column.editor)) column.editor = mini.create(column.editor)
        }
        if (column.editor) column.editor[l0l10O](false);
        if (typeof column.init == "function" && column.inited != true) column.init(this);
        delete column.colspan;
        delete column.rowspan;
        column.inited = true
    },
    getDisplayColumnRows: function() {
        var _ = this[Oo1oo](),
        D = [];
        for (var C = 0, F = _; C <= F; C++) D.push([]);
        function A(C) {
            var D = mini[ol0oo1](C.displayColumns, "displayColumns"),
            A = 0;
            for (var $ = 0, B = D.length; $ < B; $++) {
                var _ = D[$];
                if (_.displayColumns.length == 0) A += 1
            }
            return A
        }
        var $ = mini[ol0oo1](this.displayColumns, "displayColumns");
        for (C = 0, F = $.length; C < F; C++) {
            var E = $[C],
            B = D[E.level];
            if (E.displayColumns.length > 0) E.colspan = A(E);
            if (E.displayColumns.length == 0 && E.level < _) E.rowspan = _ - E.level + 1;
            B.push(E)
        }
        return D
    },
    getMaxColumnLevel: function() {
        return this.maxColumnLevel
    },
    getAllColumnWidth: function() {
        var C = this.getViewColumns(),
        B = 0,
        _ = this[l01O0o];
        for (var $ = 0, E = C.length; $ < E; $++) {
            var D = C[$],
            A = mini.isNull(D.width) ? _: D.width;
            B += A
        }
        return B
    }
};
mini.copyTo(loo0o1.prototype, mini.GridColumnModel);
mini.GridCellEditModel = {
    addRowCls: function(_, A) {
        _ = this[O1l001](_);
        if (!_) return;
        var $ = this.l1l011(_);
        if ($) lloo10($, A);
        $ = this.l1l011(_, true);
        if ($) lloo10($, A)
    },
    removeRowCls: function(_, A) {
        _ = this[O1l001](_);
        if (!_) return;
        var $ = this.l1l011(_);
        if ($) Oo11($, A);
        $ = this.l1l011(_, true);
        if ($) Oo11($, A)
    },
    l1l01: function($, _) {
        var A = typeof $ == "string" ? $: $._id;
        if (_) return this.id + "$locked$" + A;
        return this.id + "$" + A
    },
    loOO1O: function($, _) {
        return this.id + "$" + $._id + "$" + _.__id
    },
    l1l011: function($, _) {
        if (!$) return null;
        var A = this.l1l01($, _);
        return document.getElementById(A)
    },
    o11O0: function($) {
        return document.getElementById($._id)
    },
    O0o0: function($, _) {
        _ = this[lO00o](_);
        if (!_) return null;
        var A = this.loOO1O($, _);
        return document.getElementById(A)
    },
    olo0O: function(B) {
        var _ = OO0O(B.target, this.OO0101);
        if (!_) return null;
        var $ = _.id.split("$"),
        A = $[$.length - 1];
        return this.data.getbyId(A)
    },
    ll100: function(C) {
        var B = OO0O(C.target, this.cellCls);
        if (B) {
            var _ = B.id.split("$"),
            A = parseInt(_[_.length - 1]),
            $ = this.id + "$column$" + A;
            return this[lO00o]($)
        } else {
            B = OO0O(C.target, "mini-supergrid-headercell");
            if (B) return this[lO00o](B.id)
        }
        return null
    },
    ooOlO: function(A) {
        var $ = this.olo0O(A),
        _ = this.ll100(A);
        return {
            record: $,
            column: _
        }
    },
    lO0o: function(F, D) {
        if (this.disabled) return;
        var C = this.ooOlO(F),
        _ = C.record,
        B = C.column;
        if (_) {
            var E = this["_OnRow" + D];
            if (E) E[Ool00](this, _, F);
            else {
                var A = {
                    record: _,
                    htmlEvent: F
                };
                this[lOO1lo]("row" + D, A)
            }
        }
        if (B) {
            E = this["_OnColumn" + D];
            if (E) E[Ool00](this, B, F);
            else {
                A = {
                    column: B,
                    field: B.field,
                    htmlEvent: F
                };
                this[lOO1lo]("column" + D, A)
            }
        }
        if (_ && B) {
            E = this["_OnCell" + D];
            if (E) E[Ool00](this, _, B, F);
            else {
                A = {
                    record: _,
                    column: B,
                    field: B.field,
                    htmlEvent: F
                };
                this[lOO1lo]("cell" + D, A)
            }
        }
        if (!_ && B) {
            E = this["_OnHeaderCell" + D];
            if (E) E[Ool00](this, B, F);
            else {
                var A = {
                    sender: this,
                    column: B,
                    htmlEvent: F
                },
                $ = "onHeaderCell" + D;
                if (B[$]) {
                    A.sender = this;
                    B[$](A)
                }
                this[lOO1lo]("headercell" + D, A)
            }
        }
    },
    currentCell: null,
    editingCell: null,
    editControl: null,
    editWrap: null,
    O0Oo: function(C) {
        if (this.currentCell) {
            var $ = this.currentCell.record,
            A = this.currentCell.column,
            B = this.loOO1O($, A),
            _ = document.getElementById(B);
            if (_) if (C) lloo10(_, this.cellSelectedCls);
            else Oo11(_, this.cellSelectedCls)
        }
    },
    setCurrentCell: function($) {
        if (this.currentCell != $) {
            this.O0Oo(false);
            this.currentCell = $;
            this.O0Oo(true);
            this[lOO1lo]("currentcellchanged")
        }
    },
    getCurrentCell: function() {
        var $ = this.currentCell;
        if ($) if (!this.data.hasRecord($.record)) {
            this.currentCell = null;
            $ = null
        }
        return $
    },
    beginEdit: function(A) {
        if (this.editingCell) this.endEdit();
        var $ = this[lo0lOO]();
        if ($) {
            var _ = this.o011($.record, $.column);
            if (_ !== false) {
                this.editingCell = $;
                this.O11O($.record, $.column)
            }
        }
    },
    commitEdit: function($) {
        var _ = this.editingCell;
        if (_) {
            this._commitEditing = true;
            this.O0O0(_.record, _.column, $);
            this._commitEditing = false
        }
    },
    endEdit: function() {
        var $ = this.editingCell;
        if ($) {
            this[l1olO1]();
            this.oO00l($.record, $.column);
            this.editingCell = null
        }
    },
    cancelEdit: function() {
        var $ = this.editingCell;
        if ($) {
            this.oO00l($.record, $.column);
            this.editingCell = null
        }
    },
    getEditWrap: function($) {
        if (!this.editWrap) {
            this.editWrap = mini.append(document.body, "<div class=\"mini-supergrid-editwrap\" style=\"position:absolute;\"></div>");
            looo(this.editWrap, "keydown", this.Ool010, this)
        }
        this.editWrap.style.zIndex = 1000000000;
        this.editWrap.style.display = "block";
        mini[O1110](this.editWrap, $.x, $.y);
        o100oO(this.editWrap, $.width);
        this.editWrap.style.zIndex = mini.getMaxZIndex();
        return this.editWrap
    },
    Ool010: function(_) {
        if (_.keyCode == 13) {
            var $ = this.editingCell;
            if ($ && $.column && $.column.enterCommit === false) return;
            this.endEdit();
            this[OlOoo]()
        } else if (_.keyCode == 27) {
            this[Ololo1]();
            this[OlOoo]()
        } else if (_.keyCode == 9) this[Ololo1]()
    },
    oO1OO: function(A) {
        if (this.editingControl) {
            var $ = this.editingControl[o1O0O0](A);
            if ($ == false) {
                var _ = this;
                _.endEdit();
                Ol100(document, "mousedown", this.oO1OO, this)
            }
        }
    },
    o011: function($, A) {
        var C = {
            sender: this,
            source: this,
            record: $,
            column: A,
            field: A.field,
            editor: A.editor,
            value: $[A.field],
            cancel: false
        };
        if (A.oncellbeginedit) A.oncellbeginedit(C);
        this[lOO1lo]("cellbeginedit", C);
        if (C.cancel) return false;
        if (!C.editor) return false;
        var B = this.editingControl = C.editor;
        if (B[o101l]) B[o101l](C.value);
        if (A.displayField && B[O0loll]) {
            var _ = $[A.displayField];
            B[O0loll](_)
        }
        return true
    },
    O11O: function(_, C) {
        if (!this.editingControl) return false;
        var $ = this[o0O00](_, C),
        E = {
            sender: this,
            source: this,
            record: _,
            column: C,
            field: C.field,
            cellBox: $,
            editor: this.editingControl
        };
        if (C.oncellshowingedit) C.oncellshowingedit(E);
        this[lOO1lo]("cellshowingedit", E);
        var B = this.getEditWrap($),
        D = E.editor;
        if (D[lO0oOo]) {
            D[lO0oOo](this.editWrap);
            D[OlOoo]();
            setTimeout(function() {
                D[OlOoo]();
                if (D[OOOO00]) D[OOOO00]()
            },
            10);
            D[l0l10O](true)
        } else if (D.el) {
            this.editWrap.appendChild(D.el);
            try {
                D.el[OlOoo]()
            } catch(E) {}
            setTimeout(function() {
                try {
                    D.el[OlOoo]()
                } catch($) {}
            },
            10)
        }
        if (D[lOOo10]) {
            var A = $.width;
            if (mini.isNumber(D.minWidth)) if (A < D.minWidth) A = D.minWidth;
            D[lOOo10](A)
        }
        looo(document, "mousedown", this.oO1OO, this);
        if (C.autoShowPopup && D[l1l1O1]) D[l1l1O1]()
    },
    O0O0: function(_, B, A) {
        var D = {
            sender: this,
            source: this,
            record: _,
            column: B,
            field: B.field,
            editor: this.editingControl,
            value: A,
            cancel: false
        };
        if (D.editor && D.editor[o0Oll0] && A === undefined) {
            try {
                D.editor[llo101]()
            } catch(C) {
                try {
                    D.editor.el[llo101]()
                } catch(D) {}
            }
            D.value = D.editor[o0Oll0]();
            if (D.editor[ooo1oo]) D.text = D.editor[ooo1oo]()
        }
        if (B.oncellcommitedit) B.oncellcommitedit(D);
        this[lOO1lo]("cellcommitedit", D);
        if (D.cancel == false) if (B.displayField) {
            var $ = {};
            $[B.field] = D.value;
            $[B.displayField] = D.text;
            this.data.updateRecord(_, $)
        } else this.data.updateRecord(_, B.field, D.value)
    },
    oO00l: function(_, C) {
        var E = {
            sender: this,
            source: this,
            record: _,
            column: C,
            field: C.field,
            editor: this.editingControl,
            value: _[C.field]
        };
        if (C.oncellendedit) C.oncellendedit(E);
        this[lOO1lo]("cellendedit", E);
        if (this.editWrap) this.editWrap.style.display = "none";
        var A = this.editWrap.childNodes;
        for (var $ = A.length - 1; $ >= 0; $--) {
            var B = A[$];
            this.editWrap.removeChild(B)
        }
        var D = E.editor;
        if (D && D[O1Oo10]) D[O1Oo10]();
        if (D && D.clearValue) D.clearValue();
        this.editingControl = null
    },
    o1ll1l: function($, B, C, D) {
        var _ = $[B.field],
        E = {
            sender: this,
            source: this,
            rowIndex: C,
            columnIndex: D,
            record: $,
            column: B,
            field: B.field,
            value: _,
            cellHtml: _,
            rowCls: null,
            cellCls: B.cellCls || "",
            rowStyle: null,
            cellStyle: B.cellStyle || ""
        };
        if (B.dateFormat) if (mini.isDate(E.value)) E.cellHtml = mini.formatDate(_, B.dateFormat);
        else E.cellHtml = "";
        if (B.displayField) E.cellHtml = $[B.displayField];
        E.cellHtml = mini.htmlEncode(E.cellHtml);
        var A = B.renderer;
        if (A) {
            fn = typeof A == "function" ? A: window[A];
            if (fn) E.cellHtml = fn[Ool00](B, E)
        }
        this[lOO1lo]("drawcell", E);
        if (E.cellHtml === null || E.cellHtml === undefined) E.cellHtml = "";
        return E
    },
    setRowHeight: function($) {
        if ($ != this.rowHeight) {
            this.rowHeight = $;
            this[o11o0O]()
        }
    },
    setMultiSelect: function($) {
        if (this[o1lloO] != $) {
            this[o1lloO] = $;
            this.data[o101l1]($);
            this[o11o0O]()
        }
    },
    setAllowCellSelect: function($) {
        if (this[l1o1ol] != $) {
            this[l1o1ol] = $;
            this[o11o0O]()
        }
    },
    setAllowRowSelect: function($) {
        if (this[O0OOlO] != $) {
            this[O0OOlO] = $;
            this[o11o0O]()
        }
    },
    setAllowAlternating: function($) {
        if (this[Oo001] != $) {
            this[Oo001] = $;
            this[o11o0O]()
        }
    },
    setAllowResizeColumn: function($) {
        if (this[Oll01] != $) {
            this[Oll01] = $;
            this[o11o0O]()
        }
    },
    setAllowMoveColumn: function($) {
        if (this[ooO1O0] != $) this[ooO1O0] = $
    },
    setAllowSortColumn: function($) {
        if (this[lOO00] != $) this[lOO00] = $
    },
    setAllowDragDrop: function($) {
        if (this.allowDragDrop != $) {
            this.allowDragDrop = $;
            this[o11o0O]()
        }
    },
    setShowDirty: function($) {
        if (this.showDirty != $) {
            this.showDirty = $;
            this[o11o0O]()
        }
    },
    OO0oOData: function() {
        return this.data[O1O0oo]().clone()
    },
    OO0oOText: function($) {
        return "Rows " + $.length
    },
    _OnRowDragStart: function($, _) {
        var A = {
            record: $,
            column: _,
            cancel: false
        };
        this[lOO1lo]("RowDragStart", A);
        return A
    },
    _OnRowDragDrop: function($, _, A) {
        $ = $.clone();
        var B = {
            records: $,
            targetRecord: _,
            action: A,
            cancel: false
        };
        this[lOO1lo]("RowDragDrop", B);
        return B
    },
    lol01: function(_, $, A) {
        var B = {};
        B.effect = _;
        B.records = $;
        B.targetRecord = A;
        this[lOO1lo]("GiveFeedback", B);
        return B
    },
    isAllowDragDrop: function($, _) {
        if (!this.allowDragDrop) return false;
        if (_.allowDrag !== true) return false;
        var A = this._OnRowDragStart($, _);
        return ! A.cancel
    }
};
mini.copyTo(loo0o1.prototype, mini.GridCellEditModel);
mini._SuperGridSort = function($) {
    this.grid = $;
    this.grid[O1oOo1]("headercellclick", this.__onGridHeaderCellClick, this);
    this.grid[O1oOo1]("headercellmousedown", this.__OnGridHeaderCellMouseDown, this);
    looo($.OlOooO, "mousemove", this.__OnGridHeaderMouseMove, this);
    looo($.OlOooO, "mouseout", this.__OnGridHeaderMouseOut, this)
};
mini._SuperGridSort[O0lloO] = {
    __OnGridHeaderMouseOut: function($) {
        if (this.o1l010ColumnEl) Oo11(this.o1l010ColumnEl, "mini-supergrid-headercell-hover")
    },
    __OnGridHeaderMouseMove: function(_) {
        var $ = OO0O(_.target, "mini-supergrid-headercell");
        if ($) {
            lloo10($, "mini-supergrid-headercell-hover");
            this.o1l010ColumnEl = $
        }
    },
    __onGridHeaderCellClick: function(B) {
        var $ = this.grid,
        A = OO0O(B.target, "mini-supergrid-headercell");
        if (A) {
            var _ = $[lO00o](A.id.split("$")[2]);
            if ($[ooO1O0] && _ && _.allowDrag) {
                this.dragColumn = _;
                this._columnEl = A;
                this.getDrag().start(B)
            }
        }
    }
};
mini._SuperGridSelect = function($) {
    this.grid = $;
    this.grid[O1oOo1]("cellmousedown", this.lO00ol, this);
    this.grid[O1oOo1]("cellclick", this.oo1O, this);
    looo(this.grid.el, "keydown", this.ol0o, this)
};
mini._SuperGridSelect[O0lloO] = {
    ol0o: function(G) {
        var $ = this.grid,
        A = $[lo0lOO]();
        if (G.shiftKey || G.ctrlKey) return;
        if (!A) return;
        if (G.keyCode == 37 || G.keyCode == 38 || G.keyCode == 39 || G.keyCode == 40) G.preventDefault();
        var C = $.getViewColumns(),
        B = A.column,
        _ = A.record,
        F = C[looo1l](B),
        D = $[looo1l](_),
        E = $[lOoo10]();
        switch (G.keyCode) {
        case 27:
            break;
        case 13:
            if (B[O00O01] != true) $[Olo010]();
            break;
        case 37:
            if (F > 0) F -= 1;
            break;
        case 38:
            if (D > 0) D -= 1;
            break;
        case 39:
            if (F < C.length - 1) F += 1;
            break;
        case 40:
            if (D < E - 1) D += 1;
            break;
        default:
            break
        }
        B = C[F];
        _ = $[ooO0Ol](D);
        if (B && _ && $[l1o1ol]) {
            A = {
                record: _,
                column: B
            };
            $[o1lO](A)
        }
        if (_ && $[O0OOlO]) {
            $[oool00]();
            $[OlOlo1](_)
        }
    },
    oo1O: function(A) {
        var $ = A.record,
        _ = A.column;
        if (!_[O00O01] && !this.grid[l0O0Oo]()) if (A.htmlEvent.shiftKey || A.htmlEvent.ctrlKey);
        else this.grid[Olo010]()
    },
    lO00ol: function(C) {
        var $ = C.record,
        A = C.column;
        if (this.grid[l1o1ol]) {
            var _ = {
                record: $,
                column: A
            };
            this.grid[o1lO](_)
        }
        if (!this.grid[O0OOlO]) return;
        var B = {
            record: $,
            column: A,
            cancel: false
        };
        this.grid[lOO1lo]("beforeselect", B);
        if (B.cancel == true) return;
        if (this.grid[o1lloO]) {
            this.grid.el.onselectstart = function() {};
            if (C.htmlEvent.shiftKey) {
                this.grid.el.onselectstart = function() {
                    return false
                };
                C.htmlEvent.preventDefault();
                if (!this.currentRecord) {
                    this.grid.data[OlOlo1]($);
                    this.currentRecord = this.grid[llllOo]()
                } else {
                    this.grid[oool00]();
                    this.grid[oo0llo](this.currentRecord, $)
                }
            } else {
                this.grid.el.onselectstart = function() {};
                if (C.htmlEvent.ctrlKey) {
                    this.grid.el.onselectstart = function() {
                        return false
                    };
                    C.htmlEvent.preventDefault()
                }
                if (A._multiRowSelect || C.htmlEvent.ctrlKey) {
                    if (this.grid.data[OoOllo]($)) {
                        if (C.htmlEvent.button != 2) {
                            this.grid.data[O011oO]($);
                            this.grid[o1lO](null)
                        }
                    } else this.grid.data[OlOlo1]($)
                } else if (this.grid.data[OoOllo]($));
                else {
                    this.grid.data[oool00]();
                    this.grid.data[OlOlo1]($)
                }
                this.currentRecord = this.grid[llllOo]()
            }
        } else if (!this.grid.data[OoOllo]($)) this.grid.data[OlOlo1]($)
    }
};
mini._SuperGridSplitter = function($) {
    this.grid = $;
    looo(this.grid.el, "mousedown", this.o0o101, this)
};
mini._SuperGridSplitter[O0lloO] = {
    o0o101: function(C) {
        var $ = this.grid,
        A = C.target;
        if (ololo(A, "mini-supergrid-splitter")) {
            var B = mini.getAttr(A, "cid"),
            _ = $[lO00o](B);
            if ($[Oll01] && _) {
                this.splitterColumn = _;
                this.getDrag().start(C)
            }
        }
    },
    getDrag: function() {
        if (!this.drag) this.drag = new mini.Drag({
            capture: true,
            onStart: mini.createDelegate(this.oOoOO1, this),
            onMove: mini.createDelegate(this.Ol1ll, this),
            onStop: mini.createDelegate(this.ll0ll, this)
        });
        return this.drag
    },
    oOoOO1: function(_) {
        var $ = this.grid,
        B = $[l1O00o](this.splitterColumn);
        this.columnBox = B;
        this.lool0O = mini.append(document.body, "<div class=\"mini-supergrid-proxy\"></div>");
        var A = $[lOllOo](true);
        A.x = B.x;
        A.width = B.width;
        A.right = B.right;
        ooo1(this.lool0O, A)
    },
    Ol1ll: function(A) {
        var $ = this.grid,
        B = mini.copyTo({},
        this.columnBox),
        _ = B.width + (A.now[0] - A.init[0]);
        if (_ < $.columnMinWidth) _ = $.columnMinWidth;
        if (_ > $.columnMaxWidth) _ = $.columnMaxWidth;
        o100oO(this.lool0O, _)
    },
    ll0ll: function(_) {
        var $ = this.grid,
        A = lolloO(this.lool0O);
        jQuery(this.lool0O).remove();
        this.lool0O = null;
        $[ol1001](this.splitterColumn, {
            width: A.width
        })
    }
};
mini._SuperGridColumnMove = function($, _, A) {
    this.grid = $;
    this.headerCellCls = _;
    this.splitterCls = A;
    looo(this.grid.el, "mousedown", this.o0o101, this)
};
mini._SuperGridColumnMove[O0lloO] = {
    o0o101: function(B) {
        var $ = this.grid;
        if (ololo(B.target, "mini-supergrid-splitter")) return;
        if (B.button == mini.MouseButton.Right) return;
        var A = OO0O(B.target, "mini-supergrid-headercell");
        if (A) {
            var _ = $[lO00o](A.id);
            if ($[ooO1O0] && _ && _.allowMove) {
                this.dragColumn = _;
                this._columnEl = A;
                this.getDrag().start(B)
            }
        }
    },
    getDrag: function() {
        if (!this.drag) this.drag = new mini.Drag({
            capture: isIE9 ? false: true,
            onStart: mini.createDelegate(this.oOoOO1, this),
            onMove: mini.createDelegate(this.Ol1ll, this),
            onStop: mini.createDelegate(this.ll0ll, this)
        });
        return this.drag
    },
    oOoOO1: function(_) {
        var $ = this.grid;
        this.lool0O = mini.append(document.body, "<div class=\"mini-supergrid-columnproxy\"></div>");
        this.lool0O.innerHTML = "<div class=\"mini-supergrid-columnproxy-inner\">" + $.l0o1(this.dragColumn) + "</div>";
        mini[O1110](this.lool0O, _.now[0] + 15, _.now[1] + 18);
        lloo10(this.lool0O, "mini-supergrid-no");
        this.moveTop = mini.append(document.body, "<div class=\"mini-supergrid-movetop\"></div>");
        this.moveBottom = mini.append(document.body, "<div class=\"mini-supergrid-movebottom\"></div>")
    },
    Ol1ll: function(_) {
        var $ = this.grid,
        E = _.now[0];
        mini[O1110](this.lool0O, E + 15, _.now[1] + 18);
        this.targetColumn = this.insertAction = null;
        var C = OO0O(_.event.target, "mini-supergrid-headercell");
        if (C) {
            var B = $[lO00o](C.id);
            if (B && B != this.dragColumn && !$[lloOO1](this.dragColumn, B)) {
                this.targetColumn = B;
                this.insertAction = "before";
                var D = $[l1O00o](this.targetColumn);
                if (E > D.x + D.width / 2) this.insertAction = "after"
            }
        }
        if (this.targetColumn) {
            lloo10(this.lool0O, "mini-supergrid-ok");
            Oo11(this.lool0O, "mini-supergrid-no");
            var A = $[l1O00o](this.targetColumn);
            this.moveTop.style.display = "block";
            this.moveBottom.style.display = "block";
            if (this.insertAction == "before") {
                mini[O1110](this.moveTop, A.x - 4, A.y - 9);
                mini[O1110](this.moveBottom, A.x - 4, A.bottom)
            } else {
                mini[O1110](this.moveTop, A.right - 4, A.y - 9);
                mini[O1110](this.moveBottom, A.right - 4, A.bottom)
            }
        } else {
            Oo11(this.lool0O, "mini-supergrid-ok");
            lloo10(this.lool0O, "mini-supergrid-no");
            this.moveTop.style.display = "none";
            this.moveBottom.style.display = "none"
        }
    },
    ll0ll: function(_) {
        var $ = this.grid;
        mini[O11Oo0](this.lool0O);
        mini[O11Oo0](this.moveTop);
        mini[O11Oo0](this.moveBottom);
        $[oo01](this.dragColumn, this.targetColumn, this.insertAction);
        this.lool0O = this.moveTop = this.moveBottom = this.dragColumn = this.targetColumn = null
    }
};
mini._GridDragDrop = function($) {
    this.owner = $;
    this.owner[O1oOo1]("CellMouseDown", this.__OnGridCellMouseDown, this)
};
mini._GridDragDrop[O0lloO] = {
    __OnGridCellMouseDown: function(B) {
        if (B.htmlEvent.button == mini.MouseButton.Right) return;
        var $ = this.owner;
        if ($[l0O0Oo]() || $.isAllowDragDrop(B.record, B.column) == false) return;
        var _ = B.record;
        this.isTree = $.data.isTree;
        this.dragData = $.OO0oOData();
        if (this.dragData[looo1l](_) == -1) this.dragData.push(_);
        var A = this.OO0oO();
        A.start(B.htmlEvent)
    },
    oOoOO1: function(_) {
        var $ = this.owner;
        this.feedbackEl = mini.append(document.body, "<div class=\"mini-feedback\"></div>");
        this.feedbackEl.innerHTML = $.OO0oOText(this.dragData);
        this.lastFeedbackClass = ""
    },
    Ol1ll: function(_) {
        var $ = this.owner,
        C = _.now[0],
        B = _.now[1];
        mini[O1110](this.feedbackEl, C + 15, B + 18);
        var A = $.olo0O(_.event);
        this.dropRecord = A;
        if (A) {
            if (this.isTree) this.dragAction = this.getFeedback(A, B, 3);
            else this.dragAction = this.getFeedback(A, B, 2)
        } else this.dragAction = "no";
        this.lastFeedbackClass = "mini-feedback-" + this.dragAction;
        this.feedbackEl.className = "mini-feedback " + this.lastFeedbackClass;
        if (this.dragAction == "no") A = null;
        this.setRowFeedback(A, this.dragAction)
    },
    ll0ll: function(B) {
        var G = this.owner;
        mini[O11Oo0](this.feedbackEl);
        this.feedbackEl = null;
        this.setRowFeedback(null);
        if (this.isTree) {
            var I = [];
            for (var H = 0, F = this.dragData.length; H < F; H++) {
                var K = this.dragData[H],
                C = false;
                for (var J = 0, A = this.dragData.length; J < A; J++) {
                    var E = this.dragData[J];
                    if (E != K) {
                        C = G.data[oooO1o](E, K);
                        if (C) break
                    }
                }
                if (!C) I.push(K)
            }
            this.dragData = I
        }
        if (this.dropRecord && this.dragAction != "no") {
            var L = G._OnRowDragDrop(this.dragData, this.dropRecord, this.dragAction);
            if (!L.cancel) {
                var I = L.records,
                D = L.targetRecord,
                _ = L.action;
                if (G.data.isTree) G.data[lo0o0l](I, D, _);
                else {
                    var $ = G.data[looo1l](D);
                    if (_ == "after") $ += 1;
                    G.data.move(I, $)
                }
            }
        }
        this.dropRecord = null;
        this.dragData = null
    },
    setRowFeedback: function(_, E) {
        var $ = this.owner;
        if (this.lastAddDomRow) $[OOO1o](this.lastAddDomRow, "mini-supergrid-feedback-add");
        if (_ == null || this.dragAction == "add") {
            mini[O11Oo0](this.feedbackLine);
            this.feedbackLine = null
        }
        this.lastRowFeedback = _;
        if (_ != null) if (E == "before" || E == "after") {
            if (!this.feedbackLine) this.feedbackLine = mini.append(document.body, "<div class='mini-feedback-line'></div>");
            this.feedbackLine.style.display = "block";
            var C = $[ll0l0O](_),
            D = C.x,
            B = C.y - 1;
            if (E == "after") B += C.height;
            mini[O1110](this.feedbackLine, D, B);
            var A = $[lOllOo](true);
            o100oO(this.feedbackLine, A.width)
        } else {
            $[ooOOO1](_, "mini-supergrid-feedback-add");
            this.lastAddDomRow = _
        }
    },
    getFeedback: function(J, H, E) {
        var C = this.owner,
        I = C[ll0l0O](J),
        $ = I.height,
        G = H - I.y,
        F = null;
        if (this.dragData[looo1l](J) != -1) return "no";
        var A = false;
        if (E == 3) {
            A = C[llo0l1](J);
            for (var D = 0, B = this.dragData.length; D < B; D++) {
                var K = this.dragData[D],
                _ = C[oooO1o](K, J);
                if (_) {
                    F = "no";
                    break
                }
            }
        }
        if (F == null) if (E == 2) {
            if (G > $ / 2) F = "after";
            else F = "before"
        } else if (A) {
            if (G > $ / 2) F = "after";
            else F = "before"
        } else if (G > ($ / 3) * 2) F = "after";
        else if ($ / 3 <= G && G <= ($ / 3 * 2)) F = "add";
        else F = "before";
        var L = C.lol01(F, this.dragData, J);
        return L.effect
    },
    OO0oO: function() {
        if (!this.drag) this.drag = new mini.Drag({
            capture: false,
            onStart: mini.createDelegate(this.oOoOO1, this),
            onMove: mini.createDelegate(this.Ol1ll, this),
            onStop: mini.createDelegate(this.ll0ll, this)
        });
        return this.drag
    }
};
l0olll = function() {
    l0olll[oOOOoO][lllOo0][Ool00](this)
};
ol1O(l0olll, loo0o1, {
    treeColumn: null,
    showTreeIcon: false,
    allowColumnSort: false,
    leafIcon: "mini-supertree-leaf",
    folderIcon: "mini-supertree-folder",
    uiCls: "mini-supertree"
});
oollO = l0olll[O0lloO];
oollO[l1OllO] = o10Oo;
oollO[O10oll] = l0o1O;
oollO[loooo] = OOo0o;
oollO[lOl10] = ooOo0;
oollO[olo11l] = o0O0o;
oollO[ol011o] = o1ooo;
oollO[lO0oO0] = ol1Oo;
oollO[lOl0Oo] = o0000;
oollO.o1ll1l = looO0;
oollO[oooO1o] = O0oll;
oollO[O0O0o1] = O0ooO;
oollO[O010O0] = l10lO;
oollO[llo0l1] = O1lOo;
oollO.OO11 = Ol0o1;
oollO.OOo0 = ooOOo;
oollO[oOlooO] = Oo0Ol;
oollO[OOOol0] = O0O11O;
oollO[lOlo11] = Oll1l;
mini.copyTo(l0olll.prototype, mini.DataTreeApplys);
ooOl0(l0olll, "supertree");
lolo0l = function() {
    lolo0l[oOOOoO][lllOo0][Ool00](this);
    this._TaskUIDs = {};
    this._TaskIndexs = {};
    this._linkHashed = {};
    this.topTimeScale = lolo0l.getTimeScale(this.topTimeScaleType);
    this.bottomTimeScale = lolo0l.getTimeScale(this.bottomTimeScaleType);
    this.zoomTimeScales = lolo0l.createZoomTimeScales();
    var $ = new Date();
    this.startDate = new Date($.getFullYear(), $.getMonth(), $.getDate());
    this.finishDate = new Date($.getFullYear(), $.getMonth() + 1, $.getDate());
    this[llOOoO](this.startDate, this.finishDate);
    this[l1OlOo]([])
};
ol1O(lolo0l, lo0O01, {
    virtualModel: false,
    viewModel: "gantt",
    baselineIndex: 0,
    startDate: null,
    finishDate: null,
    weekStartDay: 0,
    rowHeight: 20,
    showGridLines: true,
    showLinkLines: true,
    showSummary: true,
    showCriticalPath: false,
    topTimeScaleType: "week",
    bottomTimeScaleType: "day",
    showLabel: true,
    labelField: "Name",
    labelAlign: "right",
    allowDrag: true,
    scrollLeft: 0,
    scrollTop: 0,
    scrollWidth: 0,
    scrollHeight: 0,
    headerHeight: 25,
    uiCls: "mini-ganttview",
    lOOl: false,
    showed: false,
    headerCellOffset: 2,
    _vscrollTimer: null,
    _hscrollTimer: null,
    topOffset: 4
});
oOOol = lolo0l[O0lloO];
oOOol[lO100o] = loOoo;
oOOol[OOOO1l] = ll00O0;
oOOol[O00loO] = loOlO;
oOOol[ooO0l0] = oo00o;
oOOol.OolO = o1111;
oOOol[lo01OO] = o10OO;
oOOol[ol1lOo] = OllOO;
oOOol[oOo1l0] = O1OoO;
oOOol[O0Ol1O] = loo1o;
oOOol[ol0oO1] = lo100;
oOOol[lo1oOl] = O1010;
oOOol[l1Oo0o] = olo0l;
oOOol[loO01] = o00l1O;
oOOol[ooo0ol] = OOOoo;
oOOol[oOo1Ol] = Oo0oO;
oOOol[oO1Oo0] = l00O0;
oOOol[o01l00] = llool;
oOOol[Oo001O] = o0l1oo;
oOOol.o10111 = ol001;
oOOol.oOOl = O0olo;
oOOol[o1l1O] = OO1o1;
oOOol[Ol1lO] = o1O11;
oOOol.Oll0 = l1olo;
oOOol.llO1o0 = OOlO1;
oOOol.O0l1 = lOOl0;
oOOol[l01O00] = oolOO;
oOOol[o001ol] = oOO1o;
oOOol[O0Oolo] = o0O1o;
oOOol[OlO10] = O000;
oOOol[llloOO] = oOO1oBox;
oOOol[lOo1oO] = oOO1oHeight;
oOOol[OO11Ol] = oOO1oTop;
oOOol[ol1l01] = lo0o1;
oOOol[o1OO0] = ll1Ol;
oOOol.oo1l11 = OOoOo;
oOOol.l1l1 = l01l0;
oOOol.oloo10 = O1o1l;
oOOol[o1l01O] = O1O0o;
oOOol[o0o00o] = olo1O1;
oOOol.o0lOoo = oO0l0;
oOOol[O11O1o] = lloo;
oOOol.loo0 = Ool0O;
oOOol.o01O0O = llO1o;
oOOol.o0oOOo = O0l0ll;
oOOol.O001oO = O00l1l;
oOOol.lloO = l10ll;
oOOol.o1o00O = O0OO1;
oOOol.Oooll = oO000;
oOOol.OOlloO = lO011;
oOOol.ol1O11 = l0Ol0;
oOOol.Oll1l1 = o1l01;
oOOol.lol0 = l0O01;
oOOol.lo1OOo = o1lO1;
oOOol.lll0l0 = OoOOl;
oOOol.lO011l = O001;
oOOol.lOOO = Ooo10;
oOOol.lOl1Oo = lO1o1;
oOOol[O10ol1] = o0ooo;
oOOol.o01l1 = oOOl1;
oOOol.lO1O1O = OlO0l;
oOOol.o10OoO = ollOO;
oOOol.llO0o = lO1Oo;
oOOol[O010ol] = o1Oo;
oOOol[l1OlOo] = o1001;
oOOol[llOOoO] = lOOO1;
oOOol[o0o11l] = llOol;
oOOol[lo01l1] = O0000;
oOOol[Oolllo] = oo1OO;
oOOol[Olo01o] = lll1O;
oOOol[ll10Ol] = l101o;
oOOol[lloOoo] = ooo00;
oOOol[o11loo] = llOO0;
oOOol[O0oooO] = loO0o;
oOOol[Ol1ool] = OoOO0;
oOOol[o0oOoO] = o1lOl;
oOOol[O01OoO] = o00lo;
oOOol[OOOo00] = lloo1o;
oOOol[O0l001] = o0001;
oOOol[O0111O] = ol110;
oOOol[loo0ll] = ooOo1;
oOOol[oOo0o] = lOl00;
oOOol[lOloo0] = ol01o;
oOOol[oOlooo] = Oooo0;
oOOol[OOO0Ol] = O111l;
oOOol[l1l0O] = lOO1O;
oOOol[o10l10] = oOOo0;
oOOol.oolOl = olooO;
oOOol[OOOol0] = oOo0O;
oOOol[lOlo11] = lo0Ol;
oOOol[lo0l0] = llolO;
oOOol[ll0lO0] = Oo0OO;
oOOol[l0000o] = ool11;
lolo0l[O0lloO].getTimeScaleStartDate = function(E, A) {
    var C = E.getFullYear(),
    D = E.getMonth(),
    $ = E.getDate(),
    B = E.getHours(),
    _ = E.getMinutes(),
    F = E.getSeconds();
    switch (A) {
    case "year":
        E = new Date(C, 0, 1);
        break;
    case "halfyear":
        if (D < 6) E = new Date(C, 0, 1);
        else E = new Date(C, 6, 1);
        break;
    case "quarter":
        if (D < 3) E = new Date(C, 0, 1);
        else if (D < 6) E = new Date(C, 3, 1);
        else if (D < 9) E = new Date(C, 6, 1);
        else E = new Date(C, 9, 1);
        break;
    case "month":
        E = new Date(C, D, 1);
        break;
    case "tendays":
        if ($ <= 10) E = new Date(C, D, 1);
        else if ($ <= 20) E = new Date(C, D, 11);
        else E = new Date(C, D, 21);
        break;
    case "week":
        E = mini.getWeekStartDate(E, this.weekStartDay);
        break;
    case "day":
        E = new Date(C, D, $);
        break;
    case "hour":
        E = new Date(C, D, $, B);
        break;
    case "minutes":
        E = new Date(C, D, $, B, _);
        break;
    case "seconds":
        E = new Date(C, D, $, B, _, F);
        break
    }
    return E
};
lolo0l[O0lloO].getTimeScaleNextDate = function(G, B, I) {
    var I = I || 1;
    weekStartDay = this.weekStartDay;
    var E;
    for (var _ = 0; _ < I; _++) {
        var D = G.getFullYear(),
        F = G.getMonth(),
        $ = G.getDate(),
        C = G.getHours(),
        A = G.getMinutes(),
        H = G.getSeconds();
        switch (B) {
        case "year":
            E = new Date(D + 1, 0, 1);
            break;
        case "halfyear":
            if (F < 6) E = new Date(D, 6, 1);
            else E = new Date(D + 1, 0, 1);
            break;
        case "quarter":
            if (F < 3) E = new Date(D, 3, 1);
            else if (F < 6) E = new Date(D, 6, 1);
            else if (F < 9) E = new Date(D, 9, 1);
            else E = new Date(D + 1, 0, 1);
            break;
        case "month":
            E = new Date(D, F + 1, 1);
            break;
        case "tendays":
            if ($ <= 10) E = new Date(D, F, 11);
            else if ($ <= 20) E = new Date(D, F, 21);
            else E = new Date(D, F + 1, 1);
            break;
        case "week":
            E = mini.getNextWeekStartDate(G, weekStartDay);
            break;
        case "day":
            E = new Date(D, F, $ + 1);
            break;
        case "hour":
            E = new Date(D, F, $, C + 1);
            break;
        case "minutes":
            E = new Date(D, F, $, C, A + 1);
            break;
        case "seconds":
            E = new Date(D, F, $, C, A, H + 1);
            break
        }
        G = E
    }
    return E
};
mini.getNextWeekStartDate = function(A, _) {
    var $ = mini.getWeekStartDate(A, _);
    $.setDate($.getDate() + 7);
    return $
};
String.leftPad = function(_, B, $) {
    B = B || 2;
    $ = $ || "0";
    var A = new String(_);
    if ($ == null || $ == undefined) $ = " ";
    while (A.length < B) A = $ + A;
    return A.toString()
};
lolo0l.TimeScale = {
    year: {
        type: "year",
        width: 40,
        number: 1,
        align: "center",
        index: 0,
        tooltip: function(_, $) {
            return _.getFullYear()
        },
        formatter: function(A, _, $) {
            if (_ == "top") return A.getFullYear();
            else return A.getFullYear()
        }
    },
    halfyear: {
        type: "halfyear",
        width: 24,
        number: 1,
        align: "center",
        index: 1,
        tooltip: function(_, $) {
            return _.getFullYear() + "-" + String.leftPad(_.getMonth() + 1)
        },
        formatter: function(B, A, _) {
            var C = "",
            $ = B.getMonth();
            if ($ < 6) C += "H" + 1;
            else C += "H" + 2;
            return C
        }
    },
    quarter: {
        type: "quarter",
        width: 24,
        number: 1,
        align: "center",
        index: 3,
        tooltip: function(_, $) {
            return _.getFullYear() + "-" + String.leftPad(_.getMonth() + 1)
        },
        formatter: function(B, A, _) {
            var C = "",
            $ = B.getMonth();
            if ($ < 3) C += "Q" + 1;
            else if ($ < 6) C += "Q" + 2;
            else if ($ < 9) C += "Q" + 3;
            else C += "Q" + 4;
            if (A == "top") C = B.getFullYear() + "\u5e74" + C;
            return C
        }
    },
    month: {
        type: "month",
        width: 24,
        number: 1,
        align: "center",
        index: 4,
        tooltip: function(_, $) {
            return _.getFullYear() + "-" + String.leftPad(_.getMonth() + 1)
        },
        formatter: function(A, _, $) {
            var B = A.getMonth() + 1;
            if (_ == "top") B = A.getFullYear() + "-" + String.leftPad(B);
            return B
        }
    },
    week: {
        type: "week",
        width: 24,
        number: 1,
        align: "center",
        index: 5,
        tooltip: function(A, $) {
            var B = A.getFullYear() + "-" + String.leftPad(A.getMonth() + 1) + "-" + String.leftPad(A.getDate()),
            _ = new Date(A[llo1l]());
            _.setDate(_.getDate() + 6);
            B += " \u5230 ";
            B += _.getFullYear() + "-" + String.leftPad(_.getMonth() + 1) + "-" + String.leftPad(_.getDate());
            return B
        },
        formatter: function(A, _, $) {
            if (_ == "top") return A.getFullYear() + "-" + String.leftPad(A.getMonth() + 1) + "-" + String.leftPad(A.getDate());
            else return A.getDate()
        }
    },
    day: {
        type: "day",
        width: 24,
        number: 1,
        align: "center",
        index: 6,
        tooltip: function(_, $) {
            return _.getFullYear() + "-" + String.leftPad(_.getMonth() + 1) + "-" + String.leftPad(_.getDate()) + " " + lolo0l.LongWeeks[_.getDay()]
        },
        formatter: function(A, _, $) {
            if (_ == "top") return A.getFullYear() + "-" + String.leftPad(A.getMonth() + 1) + "-" + String.leftPad(A.getDate());
            else if ($ == "week") return lolo0l.ShortWeeks[A.getDay()];
            else return A.getDate()
        }
    },
    hour: {
        type: "hour",
        width: 20,
        number: 1,
        align: "center",
        index: 7,
        tooltip: function(_, $) {
            return _.getFullYear() + "-" + String.leftPad(_.getMonth() + 1) + "-" + String.leftPad(_.getDate()) + " " + String.leftPad(_.getHours())
        },
        formatter: function(A, _, $) {
            if (_ == "top") return A.getFullYear() + "-" + String.leftPad(A.getMonth() + 1) + "-" + String.leftPad(A.getDate()) + " " + String.leftPad(A.getHours());
            else return String.leftPad(A.getHours())
        }
    },
    minutes: {
        type: "minutes",
        width: 20,
        number: 1,
        align: "center",
        index: 8,
        tooltip: function(_, $) {
            return _.getFullYear() + "-" + String.leftPad(_.getMonth() + 1) + "-" + String.leftPad(_.getDate()) + " " + String.leftPad(_.getHours()) + ":" + String.leftPad(_.getMinutes())
        },
        formatter: function(A, _, $) {
            if (_ == "top") return A.getFullYear() + "-" + String.leftPad(A.getMonth() + 1) + "-" + String.leftPad(A.getDate()) + " " + String.leftPad(A.getHours()) + ":" + String.leftPad(A.getMinutes());
            else return String.leftPad(A.getMinutes())
        }
    },
    index: 9,
    seconds: {
        type: "seconds",
        width: 20,
        number: 1,
        align: "center",
        tooltip: function(_, $) {
            return _.getFullYear() + "-" + String.leftPad(_.getMonth() + 1) + "-" + String.leftPad(_.getDate()) + " " + String.leftPad(_.getHours()) + ":" + String.leftPad(_.getMinutes()) + ":" + String.leftPad(_.getSeconds())
        },
        formatter: function(A, _, $) {
            if (_ == "top") return A.getDate() + " " + A.getHours() + ":" + A.getMinutes() + ":" + A.getSeconds();
            else return String.leftPad(A.getSeconds())
        }
    }
};
lolo0l.ShortWeeks = ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"];
lolo0l.LongWeeks = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
lolo0l.getTimeScale = function(_) {
    var $ = lolo0l.TimeScale[_.toLowerCase()];
    if ($) $ = mini.copyTo({},
    $);
    return $
};
lolo0l.createZoomTimeScales = function() {
    var A = [],
    $ = lolo0l.getTimeScale("year"),
    _ = lolo0l.getTimeScale("halfyear");
    A.push([$, _]);
    $ = lolo0l.getTimeScale("year"),
    _ = lolo0l.getTimeScale("quarter");
    A.push([$, _]);
    $ = lolo0l.getTimeScale("year"),
    _ = lolo0l.getTimeScale("month");
    A.push([$, _]);
    $ = lolo0l.getTimeScale("quarter"),
    _ = lolo0l.getTimeScale("month");
    _.width = 24;
    A.push([$, _]);
    $ = lolo0l.getTimeScale("month"),
    _ = lolo0l.getTimeScale("week");
    A.push([$, _]);
    $ = lolo0l.getTimeScale("month"),
    _ = lolo0l.getTimeScale("day");
    _.number = 3;
    A.push([$, _]);
    $ = lolo0l.getTimeScale("week"),
    _ = lolo0l.getTimeScale("day");
    A.push([$, _]);
    $ = lolo0l.getTimeScale("day"),
    _ = lolo0l.getTimeScale("hour");
    _.number = 6;
    A.push([$, _]);
    $ = lolo0l.getTimeScale("day"),
    _ = lolo0l.getTimeScale("hour");
    _.number = 2;
    A.push([$, _]);
    $ = lolo0l.getTimeScale("day"),
    _ = lolo0l.getTimeScale("hour");
    _.number = 1;
    A.push([$, _]);
    return A
};
mini._GanttViewToolTip = function($) {
    this.gantt = $;
    looo(document.body, "mousemove", this.__OnGanttMouseMove, this);
    this.gantt[O1oOo1]("ItemDragMove", this.__OnItemDragMove, this);
    this.gantt[O1oOo1]("ItemDragComplete", this.OOoo, this);
    this.gantt[O1oOo1]("scroll", this.O0O0ll, this);
    this.gantt[O1oOo1]("refresh", 
    function($) {
        this.draging = false;
        this.hideTip()
    },
    this)
};
mini._GanttViewToolTip[O0lloO] = {
    O0O0ll: function(E) {
        if (!this.gantt[ll1001]()) return;
        if (E.direction == "vertical") {
            this.toolTipEvent = "_OnScrollToolTipNeeded";
            var D = 0,
            B = 0;
            if (this.tooltipTimer) {
                clearTimeout(this.tooltipTimer);
                this.tooltipTimer = null
            }
            var _ = this,
            A = 30;
            if (mini.isFireFox) {
                var $ = _.gantt[OlO10]();
                _.showTip($, "right", "top", -1);
                _.tooltipTimer = null;
                A = 0
            } else this.tooltipTimer = setTimeout(function() {
                var $ = _.gantt[OlO10]();
                _.showTip($, "right", "top", 0);
                _.tooltipTimer = null
            },
            A)
        } else {
            this.toolTipEvent = "_OnDateToolTipNeeded";
            var D = 0,
            B = 0,
            C = this.gantt[O0Oolo]();
            this.showTip(C, "left", "bottom", 0)
        }
    },
    __OnItemDragMove: function(A) {
        this.toolTipEvent = "_OnItemDragTipNeeded";
        this.draging = true;
        var _ = A.drag.init[0],
        $ = A.drag.init[1];
        this.showTip(A.item, _, $ + 10, 0, true)
    },
    OOoo: function($, _) {
        this.draging = false;
        this.hideTip()
    },
    __OnGanttMouseMove: function(D) {
        if (this.draging === true) return;
        var A = D.target,
        _ = this.gantt;
        if (_.lOOl == true) return;
        if (!_[o1O0O0](D)) {
            this.hideTip();
            return
        }
        var $ = _.O0l1(D);
        if ($) {
            var B = OO0O(A, "mini-gantt-baseline");
            $.isBaseline = B;
            this.toolTipEvent = "_OnItemToolTipNeeded";
            this.toolTipItem = $;
            this.showTip($, D.pageX + 8, D.pageY + 15, this.showTipDelay)
        } else {
            var C = _.llO1o0(D);
            if (C) {
                this.toolTipEvent = "_OnLinkToolTipNeeded";
                this.toolTipItem = C;
                this.showTip(C, D.pageX + 5, D.pageY + 8, this.showTipDelay)
            } else this.hideTip()
        }
    },
    showTipDelay: 700,
    showTip: function($, E, B, D, C) {
        if (this._lastShowItem == $ && D != 0) {
            this._showXY = [E, B];
            return
        }
        this.hideTip(false);
        if (this.hideTimer) {
            clearInterval(this.hideTimer);
            this.hideTimer = null
        }
        var _ = this.gantt;
        this._lastShowItem = $;
        var A = this;
        this._showXY = [E, B];
        if (D <= 0 && mini.isFireFox) A._showTipCore($, C);
        else this._showTipTimer = setTimeout(function() {
            A._showTipCore($, C)
        },
        D)
    },
    _showTipCore: function($, E) {
        var _ = this.gantt;
        if (!this._tipEl) this._tipEl = mini.append(document.body, "<div class=\"mini-ganttview-tooltip\" style=\"display:none;\"></div>");
        if (E == true) this._tipEl.style.width = "auto";
        var G = _[this.toolTipEvent]($);
        this._tipEl.innerHTML = G.tooltip;
        this._tipEl.style.display = "block";
        var F = this._showXY[0],
        C = this._showXY[1];
        mini[O1110](this._tipEl, -1000, -1000);
        var D = mini.getSize(this._tipEl),
        A = this.gantt[o1OO0]();
        if (F == "left") F = A.x + 5;
        else if (F == "right") F = A.right - 20 - D.width;
        if (C == "top") C = A.y + 5;
        else if (C == "bottom") C = A.bottom - 20 - D.height;
        var B = mini[o1OO0]();
        if (F + D.width > B.right) F = B.right - D.width;
        if (C + D.height > B.bottom) C = B.bottom - D.height;
        mini[O1110](this._tipEl, F, C)
    },
    hideTip: function(A, $) {
        var _ = this;
        if (_._tipEl && A !== false) {
            mini[O11Oo0](_._tipEl);
            _._tipEl = null
        }
        _._lastShowItem = null;
        clearInterval(_._showTipTimer)
    }
};
mini._GanttViewDragDrop = function($) {
    this.owner = $;
    this.owner[O1oOo1]("refresh", this.__OnGanttRefresh, this);
    looo(this.owner.el, "mousedown", this.__OnGanttMouseDown, this)
};
mini._GanttViewDragDrop[O0lloO] = {
    isDraging: function() {
        return !! this.dragAction
    },
    originalItem: null,
    dragItem: null,
    dragAction: null,
    _BeforeDragMove: function($, A) {
        var _ = this.owner[ooO0l0]($, "move");
        if (!_.cancel) this.getDrag().start(A)
    },
    __OnGanttMouseDown: function(C) {
        var A = C.target,
        _ = this.owner;
        if (_[l0O0Oo]()) return;
        if (!_.allowDrag) return;
        var $ = _.O0l1(C);
        if (!$) return;
        if (OO0O(A, "mini-gantt-baseline")) return;
        if (mini.MouseButton.Left == C.button) {
            this.dragItem = $;
            this.originalItem = mini.copyTo({},
            $);
            if (ololo(A, "mini-gantt-resize-start")) {
                this.dragAction = "start";
                var B = _[ooO0l0]($, "start");
                if (!B.cancel) this.getDrag().start(C)
            } else if (ololo(A, "mini-gantt-resize-finish")) {
                this.dragAction = "finish";
                B = _[ooO0l0]($, "finish");
                if (!B.cancel) this.getDrag().start(C)
            } else if (ololo(A, "mini-gantt-resize-percentcomplete")) {
                this.dragAction = "percentcomplete";
                B = _[ooO0l0]($, "percentcomplete");
                if (!B.cancel) this.getDrag().start(C)
            } else if (OO0O(A, "mini-gantt-item")) {
                this.dragAction = "move";
                this._BeforeDragMove($, C)
            }
        }
    },
    getDrag: function() {
        if (!this.drag) this.drag = new mini.Drag({
            delay: 100,
            capture: true,
            context: this.owner.ooolO,
            onStart: mini.createDelegate(this.oOoOO1, this),
            onMove: mini.createDelegate(this.Ol1ll, this),
            onStop: mini.createDelegate(this.ll0ll, this)
        });
        return this.drag
    },
    _GetCursor: function() {
        switch (this.dragAction) {
        case "start":
            return "w-resize";
            break;
        case "finish":
            return "w-resize";
            break;
        case "percentcomplete":
            return "row-resize";
            break;
        case "move":
            return "move";
            break;
        case "link":
            return "move";
            break
        }
    },
    oOoOO1: function(A) {
        var _ = this.owner,
        $ = this.dragItem;
        this.viewBox = _[ol1l01](_.viewRegion);
        var B = _[llloOO]($);
        this.MoveOffset = B.left - A.init[0];
        this.timeSpan = $.Finish - $.Start;
        this.itemBox = B
    },
    Ol1ll: function(A) {
        var _ = this.owner,
        $ = this.dragItem,
        C = this.viewBox,
        G = _[o1OO0]();
        switch (this.dragAction) {
        case "start":
            var D = _[O11O1o](A.now[0]);
            $.Start = D;
            if ($.Start > $.Finish) $.Start = $.Finish;
            setTimeout(function() {
                _.lol0($)
            },
            10);
            break;
        case "finish":
            D = _[O11O1o](A.now[0]);
            $.Finish = D;
            if ($.Start > $.Finish) $.Finish = $.Start;
            setTimeout(function() {
                _.lol0($)
            },
            1);
            break;
        case "percentcomplete":
            var F = this.itemBox.width,
            E = A.now[0] - G.x + C.left - this.itemBox.x,
            B = parseInt(E * 100 / F);
            if (B < 0) B = 0;
            if (B > 100) B = 100;
            $.PercentComplete = B;
            setTimeout(function() {
                _.lol0($, false)
            },
            10);
            break;
        case "move":
            E = A.now[0] + this.MoveOffset,
            D = _[o0o00o](E);
            $.Start = D;
            $.Finish = new Date(D[llo1l]() + this.timeSpan);
            setTimeout(function() {
                _.lol0($)
            },
            10);
            break;
        case "link":
            break
        }
        _[O00loO]($, A, this.dragAction)
    },
    dropNode: null,
    ll0ll: function(B, C) {
        var A = this.owner,
        $ = this.dragItem;
        if (C == false) mini.copyTo(this.dragItem, this.originalItem);
        else {
            var _ = this.dragItem["Start"];
            switch (this.dragAction) {
            case "move":
                this.dropNode = A.o0lOoo(B.now[1]);
                var D = A[OOOO1l](this.dragItem, this.dropNode);
                if (D.cancel) C = false;
                break;
            case "start":
                break;
            case "finish":
                _ = this.dragItem["Finish"];
                break;
            case "percentcomplete":
                _ = this.dragItem["PercentComplete"];
                break;
            case "link":
                break
            }
            mini.copyTo(this.dragItem, this.originalItem);
            if (C) this[lO100o](_)
        }
        if (C == false) B.event.stopPropagation();
        this.stopDrag(C)
    },
    _OnItemDragComplete: function(_) {
        var A = this.owner,
        $ = this.dragItem;
        A[lO100o](this.dragItem, this.dragAction, _, this.dropNode)
    },
    stopDrag: function(A) {
        var _ = this.owner,
        $ = this.dragItem;
        if (A == false) _[O10ol1]();
        this.dragItem = this.originalItem = this.dragAction = this.dropNode = null
    },
    __OnGanttRefresh: function(_) {
        if (this.__lool0OTimer) clearTimeout(this.__lool0OTimer);
        var $ = this;
        this.__lool0OTimer = setTimeout(function() {
            $.renderlool0O();
            $.__lool0OTimer = null
        },
        300)
    },
    renderItemlool0O: function($, D, H, G) {
        var B = this.owner,
        G = $._id,
        F = B[ooO0l0]($, "start");
        if (!F.cancel) {
            H[H.length] = "<div id=\"";
            H[H.length] = G;
            H[H.length] = "\" class=\"mini-gantt-resize-start\" style=\"left:";
            H[H.length] = D.x - 2;
            H[H.length] = "px;top:";
            H[H.length] = D.y;
            H[H.length] = "px;width:";
            H[H.length] = 5;
            H[H.length] = "px;height:";
            H[H.length] = D.height;
            H[H.length] = "px;\"></div>"
        }
        F = B[ooO0l0]($, "finish");
        if (!F.cancel) {
            H[H.length] = "<div id=\"";
            H[H.length] = G;
            H[H.length] = "\" class=\"mini-gantt-resize-finish\" style=\"left:";
            H[H.length] = D.right - 2;
            H[H.length] = "px;top:";
            H[H.length] = D.y;
            H[H.length] = "px;width:";
            H[H.length] = 5;
            H[H.length] = "px;height:";
            H[H.length] = D.height;
            H[H.length] = "px;\"></div>"
        }
        F = B[ooO0l0]($, "percentcomplete");
        if (!F.cancel) {
            var E = $.PercentComplete || 0,
            C = parseInt((D.right - D.x) * E / 100),
            _ = D.x + C,
            A = 4;
            if (E == 0) A = 3;
            else if (E == 100) {
                A = 3;
                _ -= 3
            } else _ -= 2;
            H[H.length] = "<div id=\"";
            H[H.length] = G;
            H[H.length] = "\" class=\"mini-gantt-resize-percentcomplete\" style=\"left:";
            H[H.length] = _;
            H[H.length] = "px;top:";
            H[H.length] = D.y;
            H[H.length] = "px;width:";
            H[H.length] = A;
            H[H.length] = "px;height:";
            H[H.length] = D.height;
            H[H.length] = "px;\"></div>"
        }
    },
    renderlool0O: function() {
        var H = this.owner;
        if (!H.allowDrag || H[l0O0Oo]()) return;
        var B = H.viewRegion,
        J = H[ol1l01](B),
        E = J.left,
        K = J.top,
        L = J.width,
        N = J.height,
        I = H[O010ol](),
        C = B.startRow,
        A = B.endRow,
        F = [];
        for (var G = C, D = A; G <= D; G++) {
            var $ = I[G];
            if (!$) continue;
            var _ = H[llloOO]($, E, K);
            this.renderItemlool0O($, _, F)
        }
        var M = "<div>" + F.join("") + "</div>";
        mini.append(H.cellsEl, M)
    }
};
Ol01OO = function() {
    Ol01OO[oOOOoO][lllOo0][Ool00](this);
    this[l11oOo]();
    this.ganttView.showCriticalPath = this.showCriticalPath
};
ol1O(Ol01OO, OO1oOo, {
    width: 450,
    height: 200,
    baselineIndex: 0,
    viewModel: "gantt",
    data: null,
    headerHeight: 36,
    rowHeight: 21,
    columnWidth: 100,
    tableWidth: "50%",
    splitWidth: 4,
    minViewWidth: 100,
    treeColumn: null,
    columns: null,
    readOnly: false,
    allowDragDrop: false,
    multiSelect: false,
    showDirty: true,
    showGridLines: true,
    timeLines: null,
    showTableView: true,
    showGanttView: true,
    tableViewExpanded: true,
    ganttViewExpanded: true,
    allowResize: true,
    uiCls: "mini-gantt",
    tableViewType: "SuperTree",
    ganttViewType: "GanttView",
    showLabel: true,
    allowProjectDateRange: false,
    __TaskID: 1,
    autoSyncSummary: true,
    allowSummaryLink: true,
    allowLinkLimit: false,
    showCriticalPath: true,
    _orderCount: 0,
    allowOrderProject: false
});
o0l0o = Ol01OO[O0lloO];
o0l0o[ll0loo] = oO00oO;
o0l0o[l1oo1o] = OolOol;
o0l0o[lO10OO] = ol10;
o0l0o[lOlll0] = l0Oll;
o0l0o[O11ooO] = Oo101;
o0l0o[l1OOlo] = oO1Oo;
o0l0o[loOOl1] = o010l;
o0l0o[o01oOl] = llO0O1;
o0l0o[lO10o1] = lo0ol;
o0l0o[ol1l00] = Ol1O;
o0l0o[OlOl00] = oo00l;
o0l0o[O0Ooo1] = ool1o;
o0l0o[oool00] = o1o0o;
o0l0o[l110Oo] = oooo;
o0l0o[O011oO] = lo111;
o0l0o[OlOlo1] = OO1OO;
o0l0o[OoOllo] = Olo0O;
o0l0o[O1O0oo] = ll11l;
o0l0o[llllOo] = l011O;
o0l0o[o001Oo] = lo0O1;
o0l0o[l0110o] = o110;
o0l0o[olll1l] = lol0l;
o0l0o[OO10o0] = O11110;
o0l0o.l1l110 = lll0o;
o0l0o[OooO01] = O0Oo1O;
o0l0o[oO011O] = lll1OO;
o0l0o[OlOlol] = l01l1;
o0l0o[O11Ool] = lo0o0;
o0l0o[o0110o] = o0oo1;
o0l0o[loO11o] = oOlo;
o0l0o[o1Ol00] = OO01o;
o0l0o[llo011] = lo101;
o0l0o[OOoOOo] = l11l;
o0l0o[oo0Ool] = Olol0O;
o0l0o[O1o1lo] = OOO1l;
o0l0o[Oo1ol0] = OO0o01;
o0l0o[o0l1lo] = oo1o1;
o0l0o[O01OoO] = lloOl;
o0l0o[l1ol10] = O10Oo;
o0l0o[l0lOl1] = lOo0O;
o0l0o[OOoO10] = oloool;
o0l0o[lolo01] = looO;
o0l0o[l0l0OO] = ool0O;
o0l0o[l1Oo00] = l01ll;
o0l0o[oOlllo] = oO10o;
o0l0o[l0llol] = O0oO;
o0l0o[o0O100] = O0olO;
o0l0o[l0O1O1] = l011l;
o0l0o[o1Oooo] = OO00;
o0l0o[l1O011] = o0lOll;
o0l0o[Oo001l] = o1Ol;
o0l0o[ooo1oO] = Oo11l;
o0l0o[O1llo0] = lO10o;
o0l0o[OO1o0l] = OoO00l;
o0l0o[OO0Olo] = Ololo;
o0l0o[loo10o] = l0oOOo;
o0l0o[O0Oolo] = l1lOO1;
o0l0o[oooO1o] = OO01l;
o0l0o[l01l1l] = o0loO;
o0l0o[oOo0O1] = lO101;
o0l0o[l1Ooll] = lO0l0;
o0l0o[Oll1l0] = o1Oloo;
o0l0o[O1l1O1] = l01O1o;
o0l0o[O1Ol00] = o1ll;
o0l0o[O0Ol00] = oo0l1;
o0l0o[O0l010] = o1o0;
o0l0o[Oo1loo] = lOO1l0;
o0l0o.O1000 = lolOlO;
o0l0o[OOO1lo] = OOO0o;
o0l0o[OOo0oo] = l0o01O;
o0l0o[o00111] = O10o1;
o0l0o.o1O01O = olll;
o0l0o[Oo1Oo0] = l110o;
o0l0o[oOoO1o] = ll1o;
o0l0o[Oo0o1O] = l1l100;
o0l0o[loO1O1] = o0llo;
o0l0o[o1l0ol] = Ool01;
o0l0o[o11ooo] = O00l1;
o0l0o.O1OOo = l1ll1;
o0l0o[l11oOo] = OOo0ol;
o0l0o[ollOo] = O1o010;
o0l0o[OOl0o0] = O1l10;
o0l0o[O00l11] = OO1lO;
o0l0o[OOlOO0] = l0oO;
o0l0o[O1lolO] = l1lo1;
o0l0o[O0O0o1] = o0oOO;
o0l0o[loOo1o] = l0oOLevel;
o0l0o[lo1Ol0] = l1lo1Level;
o0l0o[lo1Ooo] = O1lol1;
o0l0o[lllolO] = OO011;
o0l0o[ll0Olo] = o0Ol1;
o0l0o[o0O1O1] = l1o0;
o0l0o[oO1l1l] = O0O1;
o0l0o[o0ol1o] = Oo1l0;
o0l0o[O0OOoo] = ll11;
o0l0o[lO1oO0] = o0O0Oo;
o0l0o.o0o1 = O0oOO;
o0l0o[O0l001] = OlO1o1;
o0l0o[lO0oO0] = O00Olo;
o0l0o[ooll0O] = lll0O;
o0l0o[lO00o] = lOOo1;
o0l0o[ol1001] = ll00l;
o0l0o[lolO0O] = lOOo1s;
o0l0o[l1011O] = ooO0o;
o0l0o[oll0ol] = l0lo10;
o0l0o[lO101l] = l011OColumn;
o0l0o[oo0ol0] = o0O11;
o0l0o[lol0oo] = OllOol;
o0l0o[o0Ooo0] = loO00;
o0l0o[ll0o1] = l0oOl;
o0l0o[ll10Ol] = o00O;
o0l0o[lloOoo] = oO111o;
o0l0o[oO1Oo0] = o1Ol0;
o0l0o[o01l00] = l1lo0;
o0l0o[oOo1Ol] = llo0Ol;
o0l0o[ooO000] = l0Oo0l;
o0l0o[o101l1] = o01oo;
o0l0o[O0l001] = OlO1o1;
o0l0o[O0111O] = l0O1l;
o0l0o[lOloo0] = olO11;
o0l0o[loo0ll] = Oo0o0;
o0l0o[oOo0o] = o1O0;
o0l0o[oOl10l] = OoO01O;
o0l0o[l01oOl] = l1001;
o0l0o[O1010o] = oOO1;
o0l0o.OlO11o = o1oO1o;
o0l0o[lo1OO0] = oo0O;
o0l0o[l0OooO] = l1O0o;
o0l0o[loll1O] = olo01;
o0l0o[O1O1O1] = O1100o;
o0l0o.O00o = o0l1l;
o0l0o.olOo = lo0Oo;
o0l0o[o10l10] = Ol011;
o0l0o[o0o11l] = Oo000;
o0l0o.O1ol1 = oOO01;
o0l0o.o01O1 = o1ol1;
o0l0o.Ollo = OOl0O;
o0l0o.O0O10o = l0OO;
o0l0o.l0ol0o = lOOol;
o0l0o.o0Oo1 = o1o000;
o0l0o.o00l1 = o01lO;
o0l0o.Ol1l = OOlO0;
o0l0o.O1Olo1 = oOOOo;
o0l0o.o000Ol = OolO0;
o0l0o[OOOol0] = o111ll;
o0l0o.OO000 = Oll0l1;
o0l0o.loO0 = ooOo1O;
o0l0o[lOlo11] = o0llll;
o0l0o[o1l0Ol] = ooo0;
o0l0o[O10ol1] = OOlo1;
o0l0o[lo0o11] = l0l0O;
o0l0o[l0000o] = ol00ll;
ooOl0(Ol01OO, "gantt");
Ol01OO.PredecessorLinkType = [{
    ID: 0,
    Name: "Finish-Finish(FF)",
    Short: "FF"
},
{
    ID: 1,
    Name: "Finish-Start(FS)",
    Short: "FS"
},
{
    ID: 2,
    Name: "Start-Finish(SF)",
    Short: "SF"
},
{
    ID: 3,
    Name: "Start-Start(SS)",
    Short: "SS"
}];
Ol01OO.ConstraintType = [{
    ID: 0,
    Name: "The sooner the better"
},
{
    ID: 1,
    Name: "The later the better"
},
{
    ID: 2,
    Name: "Must be begin in"
},
{
    ID: 3,
    Name: "Must be completed in"
},
{
    ID: 4,
    Name: "Beginning no earlier than ..."
},
{
    ID: 5,
    Name: "Beginning no later than ..."
},
{
    ID: 6,
    Name: "Completed no earlier than ..."
},
{
    ID: 7,
    Name: "Completed no later than ..."
}];
mini.copyTo(Ol01OO, {
    ID_Text: "ID",
    Name_Text: "Name",
    PercentComplete_Text: "Progress",
    Duration_Text: "Duration",
    Start_Text: "Start",
    Finish_Text: "Finish",
    Critical_Text: "Critical",
    PredecessorLink_Text: "PredecessorLink",
    Work_Text: "Work",
    Priority_Text: "Priority",
    Weight_Text: "Weight",
    OutlineNumber_Text: "OutlineNumber",
    OutlineLevel_Text: "OutlineLevel",
    ActualStart_Text: "ActualStart",
    ActualFinish_Text: "ActualFinish",
    WBS_Text: "WBS",
    ConstraintType_Text: "ConstraintType",
    ConstraintDate_Text: "ConstraintDate",
    Department_Text: "Department",
    Principal_Text: "Principal",
    Assignments_Text: "Assignments",
    Summary_Text: "Summary",
    Task_Text: "Task",
    Baseline_Text: "Baseline",
    LinkType_Text: "LinkType",
    LinkLag_Text: "LinkLag",
    From_Text: "From",
    To_Text: "To",
    Goto_Text: "Goto",
    UpGrade_Text: "UpGrade",
    DownGrade_Text: "DownGrade",
    Add_Text: "Add Task",
    Edit_Text: "Edit Task",
    Remove_Text: "Remove Task",
    ZoomIn_Text: "ZoomIn",
    ZoomOut_Text: "ZoomOut",
    Deselect_Text: "Un Select",
    Split_Text: "Split Task"
});
PlusGantt = Ol01OO;
o10l = function($) {
    this.project = $;
    $[O1oOo1]("cellbeginedit", this.lll0O0, this);
    $[O1oOo1]("aftercellcommitedit", this.Ol1O11, this);
    $[O1oOo1]("itemdragstart", this.oOlO, this);
    $[O1oOo1]("itemdragcomplete", this.OOoo, this);
    $[O1oOo1]("dodragdrop", this.oOol, this);
    $[ol0Ol1]({
        readOnly: false,
        allowDragDrop: true
    })
};
o10l[O0lloO] = {
    oOol: function(B) {
        var _ = B.tasks,
        A = B.targetTask,
        $ = B.action;
        this.project[l1Oo00](_, A, $)
    },
    lll0O0: function(A) {
        var _ = A.record,
        $ = A.field;
        if (_.Summary && _.FixedDate != 1) if ($ == "Start" || $ == "Finish" || $ == "Duration") A.cancel = true
    },
    Ol1O11: function(F) {
        F.cancel = true;
        var D = F.record,
        B = F.field,
        _ = F.value,
        C = D[B],
        A = F.column;
        if (mini[ll00oO](C, _)) return;
        try {
            if (A.displayField) {
                var $ = {};
                $[A.field] = F.value;
                $[A.displayField] = F.text;
                this.project[o0O100](D, $)
            } else this.project[o0O100](D, B, _)
        } catch(E) {
            alert(E.message)
        }
    },
    oOlO: function($) {
        if ($.action == "start") $.cancel = true
    },
    OOoo: function(C) {
        var _ = C.action,
        $ = C.value,
        B = C.item,
        A = new Date();
        if (_ == "finish") this.project[o0O100](B, "Finish", $);
        if (_ == "percentcomplete") this.project[o0O100](B, "PercentComplete", $);
        if (_ == "move") this.project[o0O100](B, "Start", $)
    }
};
o10l.Calendar = function($) {
    this.project = $;
    this.calendar = $.getProjectCalendar();
    this.validCalendar(this.calendar);
    this.WeekDays = this.calendar["WeekDays"];
    this.Exceptions = this.calendar["Exceptions"];
    mini[o01oOl](this.WeekDays, 
    function($, _) {
        return $.DayType > _.DayType
    });
    this.caches = {}
};
o10l.Calendar[O0lloO] = {
    validCalendar: function(F) {
        if (F.UID == null) throw new Error("\u65e5\u5386\u6ca1\u6709UID\u6807\u8bc6\u53f7");
        var $ = F["WeekDays"],
        I = F["Exceptions"];
        if ($ == null || $.length != 7) throw new Error("\u5de5\u4f5c\u5468\u6570\u636e\u9519\u8bef");
        if (I == null) F["Exceptions"] = I = [];
        var A = false;
        for (var H = 0; H < 7; H++) {
            var B = $[H],
            _ = parseInt(B["DayType"]),
            K = parseInt(B["DayWorking"]);
            if ((K != 0 && K != 1) || _ < 1 || _ > 7) throw new Error("\u5de5\u4f5c\u5468\u6570\u636e\u9519\u8bef");
            if (K == 1) A = true
        }
        if (A == false) throw new Error("\u5de5\u4f5c\u5468\u5fc5\u987b\u81f3\u5c11\u6709\u4e00\u5929\u662f\u5de5\u4f5c\u65e5");
        for (var H = 0, C = I.length; H < C; H++) {
            var D = I[H],
            _ = parseInt(D["DayType"]),
            K = parseInt(D["DayWorking"]);
            if ((K != 0 && K != 1) || _ != 0) throw new Error("\u4f8b\u5916\u65e5\u671f\u9519\u8bef");
            if (K == 1) {
                var J = D["TimePeriod"];
                if (J == null || !mini.isDate(J["FromDate"]) || !mini.isDate(J["ToDate"])) throw new Error("\u4f8b\u5916\u65e5\u671f\u6570\u636e\u9519\u8bef");
                var G = J["FromDate"],
                E = J["ToDate"];
                J["FromDate"] = mini.clearTime(G);
                J["ToDate"] = mini.maxTime(E)
            }
        }
    },
    isWorkingDate: function(_) {
        var $ = this.getDay(_.getDay(), _);
        return $.DayWorking == 1
    },
    getDay: function(J, B) {
        var G = "getDay$" + B[llo1l]() + J,
        E = this.caches[G];
        if (E) return E;
        var _ = this.WeekDays[J];
        if (B != null) {
            var $ = B[llo1l]();
            for (var F = 0, C = this.Exceptions.length; F < C; F++) {
                var D = this.Exceptions[F],
                I = D["TimePeriod"],
                A = I["FromDate"][llo1l](),
                H = I["ToDate"][llo1l]();
                if (A <= $ && $ <= H) {
                    _ = D;
                    break
                }
            }
        }
        this.caches[G] = _;
        return _
    },
    getStart: function(_, A) {
        if (A <= 0) return new Date(_[llo1l]());
        var B = "getstart" + _[llo1l]() + A,
        $ = this.caches[B];
        if ($) return new Date($);
        var D = new Date(_.getFullYear(), _.getMonth(), _.getDate());
        while (A > 0) {
            var C = this.isWorkingDate(D);
            if (C) {
                A--;
                if (A == 0) break
            }
            D = new Date(D.getFullYear(), D.getMonth(), D.getDate() - 1)
        }
        this.caches[B] = D[llo1l]();
        return D
    },
    getFinish: function(A, _) {
        if (_ <= 0) return new Date(A[llo1l]());
        var B = "getfinish" + A[llo1l]() + _,
        $ = this.caches[B];
        if ($) return new Date($);
        var D = new Date(A.getFullYear(), A.getMonth(), A.getDate());
        while (_ > 0) {
            var C = this.isWorkingDate(D);
            if (C) {
                _--;
                if (_ == 0) break
            }
            D = new Date(D.getFullYear(), D.getMonth(), D.getDate() + 1)
        }
        D = new Date(D.getFullYear(), D.getMonth(), D.getDate(), 23, 59, 59);
        this.caches[B] = D[llo1l]();
        return D
    },
    getWorkingDays: function(B, A) {
        if (B[llo1l]() == A[llo1l]()) return 0;
        var D = "getWorkingDays" + B[llo1l]() + A[llo1l](),
        _ = this.caches[D];
        if (_ !== undefined) return _;
        var C = B > A;
        if (C) {
            var E = B;
            B = A;
            A = B
        }
        B = new Date(B.getFullYear(), B.getMonth(), B.getDate());
        A = new Date(A.getFullYear(), A.getMonth(), A.getDate(), 23, 59, 59);
        var $ = 0,
        F = A[llo1l]();
        for (var H = B; H[llo1l]() < F;) {
            var G = this.isWorkingDate(H);
            if (G) $++;
            H = new Date(H.getFullYear(), H.getMonth(), H.getDate() + 1)
        }
        this.caches[D] = $;
        return C ? -$: $
    },
    getWorkingDate: function(C, D) {
        var A = "getWorkingDate" + C[llo1l]() + D,
        $ = this.caches[A];
        if ($) return new Date($);
        C = new Date(C.getFullYear(), C.getMonth(), C.getDate());
        var _ = D ? 1: -1;
        while (true) {
            var B = this.isWorkingDate(C);
            if (B) break;
            C = new Date(C.getFullYear(), C.getMonth(), C.getDate() + _)
        }
        C = D ? C: new Date(C.getFullYear(), C.getMonth(), C.getDate(), 23, 59, 59);
        this.caches[A] = C[llo1l]();
        return C
    },
    getWorkingStartDate: function(D, _) {
        if (_ == 0) return D;
        var A = "getWorkingStartDate" + D[llo1l]() + _,
        $ = this.caches[A];
        if ($) return new Date($);
        D = new Date(D.getFullYear(), D.getMonth(), D.getDate());
        var B = _ > 0 ? 1: -1;
        while (true) {
            if (_ == 0) break;
            D = new Date(D.getFullYear(), D.getMonth(), D.getDate() + B);
            var C = this.isWorkingDate(D);
            if (C) _ -= B
        }
        this.caches[A] = D[llo1l]();
        return D
    }
};
o10l.Validator = function($) {
    this.project = $
};
o10l.Validator[O0lloO] = {
    valid: function() {
        var $ = this.project[Oo0o1O]();
        this.validTasks($)
    },
    validTasks: function(B) {
        this.validedTasks = {};
        B = B.clone();
        for (var $ = 0, A = B.length; $ < A; $++) {
            var _ = {};
            this.validTask(B[$], _)
        }
    },
    validTask: function(D, _) {
        var F = D.UID;
        if (this.validedTasks[F] != null) return;
        if (_[F] != null) throw new Error("\u524d\u7f6e\u4efb\u52a1\u51fa\u9519,\u6709\u56de\u73af\u5f15\u7528");
        _[F] = D;
        this.validedTasks[F] = D;
        this.validTaskProperties(D);
        var A = this.project[O1l1O1](F);
        if (A && A.UID != this.project.rootTaskUID) {
            this.validTask(A, _);
            delete _[A.UID]
        }
        var E = D.PredecessorLink;
        if (E != null) for (var $ = 0, C = E.length; $ < C; $++) {
            var B = E[$];
            this.validLink(D, B, _);
            delete _[B.PredecessorUID]
        }
    },
    validLink: function(B, A, $) {
        var C = this.project[O0OOoo](A.PredecessorUID);
        if (C == null) return;
        var D = B.UID,
        _ = C.UID;
        if ($[_] != null) {
            alert("\u4efb\u52a1 (" + B.ID + ") \"" + B.Name + "\" \u4e0d\u80fd\u6709\u56de\u73af\u5f15\u7528\u5173\u7cfb");
            this.project[oo0Ool](B, C)
        }
        if (this.project[oooO1o](D, _)) {
            alert("\u7236\u4efb\u52a1 (" + B.ID + ") \"" + B.Name + "\" \u548c\u5b50\u4efb\u52a1 (" + C.ID + ") \"" + C.Name + "\" \u4e0d\u80fd\u6709\u4efb\u52a1\u76f8\u5173\u6027");
            this.project[oo0Ool](B, C)
        }
        if (this.project[oooO1o](_, D)) {
            alert("\u7236\u4efb\u52a1 (" + C.ID + ") \"" + C.Name + "\" \u548c\u5b50\u4efb\u52a1 (" + B.ID + ") \"" + B.Name + "\" \u4e0d\u80fd\u6709\u4efb\u52a1\u76f8\u5173\u6027");
            this.project[oo0Ool](B, C)
        }
        this.validTask(C, $)
    },
    validTaskProperties: function(B) {
        if (B.Name === null || B.Name === undefined) B.Name = "";
        if (isNaN(B.Duration)) B.Duration = 0;
        if (isNaN(B.Work)) B.Work = 0;
        if (isNaN(B.PercentComplete)) B.PercentComplete = 0;
        if (B.PercentComplete < 0) B.PercentComplete = 0;
        if (B.PercentComplete > 100) B.PercentComplete = 100;
        if (isNaN(B.Critical)) B.Critical = 0;
        if (isNaN(B.ConstraintType) || !B.ConstraintDate) B.ConstraintType = 0;
        if (B.ConstraintDate) {
            var _ = B.ConstraintDate;
            B.ConstraintDate = new Date(_.getFullYear(), _.getMonth(), _.getDate())
        }
        if (!mini.isDate(B.Start)) B.Start = null;
        if (!mini.isDate(B.Finish)) B.Finish = null;
        if (!mini.isDate(B.ActualStart)) B.ActualStart = null;
        if (!mini.isDate(B.ActualFinish)) B.ActualFinish = null;
        if (B.FixedDate == null) B.FixedDate = 0;
        var C = B.PredecessorLink;
        if (C && C.length > 0) for (var $ = C.length - 1; $ >= 0; $--) {
            var A = C[$];
            A.Type = parseInt(A.Type);
            A.LinkLag = parseInt(A.LinkLag);
            if (isNaN(A.LinkLag)) A.LinkLag = 0;
            if (B.Summary && (A.Type == 0 || A.Type == 2)) {
                alert("\u6458\u8981\u4efb\u52a1 (" + B.ID + ") \"" + B.Name + "\" \u7684\u524d\u7f6e\u4efb\u52a1\u5fc5\u987b\u662fFS\u6216SS");
                C.removeAt($)
            }
        }
    }
};
o10l.PercentComplete = function($) {
    this.project = $
};
o10l.PercentComplete[O0lloO] = {
    syncComplete: function($) {
        this.syncParentComplete($);
        this.syncChildrenComplete($)
    },
    syncParentComplete: function(H) {
        var A = "Duration",
        K = H.UID,
        _ = this.project[O1l1O1](K);
        if (_ != null && _.UID != this.project.rootTaskUID) {
            var G = _.PercentComplete,
            F = this.getChildrenAll(_),
            B = 0,
            I = 0;
            for (var D = 0, C = F.length; D < C; D++) {
                var $ = F[D],
                J = parseInt($[A]),
                E = parseInt($["PercentComplete"]);
                B += J;
                I += J * E / 100
            }
            _["PercentComplete"] = parseInt(I / B * 100);
            this.syncParentComplete(_);
            if (G != _.PercentComplete) this.project[OO10o0](_, "PercentComplete")
        }
    },
    syncChildrenComplete: function(G) {
        var A = "Duration",
        H = this.getChildrenAll(G),
        B = 0,
        I = 0;
        for (var E = 0, C = H.length; E < C; E++) {
            var _ = H[E],
            J = parseInt(_[A]);
            B += J
        }
        I = B * parseInt(G["PercentComplete"]) / 100;
        var D = B == I;
        for (E = 0, C = H.length; E < C; E++) {
            var _ = H[E],
            F = _.PercentComplete,
            J = parseInt(_[A]);
            if (I <= 0) _["PercentComplete"] = 0;
            else {
                var $ = I - J;
                if ($ >= 0) _["PercentComplete"] = 100;
                else _["PercentComplete"] = parseInt(I / J * 100);
                I = $
            }
            if (D) _["PercentComplete"] = 100;
            if (F != _.PercentComplete) this.project[OO10o0](_, "PercentComplete")
        }
        for (E = 0, C = H.length; E < C; E++) {
            _ = H[E];
            this.syncParentComplete(_)
        }
    },
    getChildrenAll: function(C) {
        var D = this.project[Oll1l0](C, true),
        B = [];
        for (var $ = 0, A = D.length; $ < A; $++) {
            var _ = D[$];
            if (_.Summary == 0) B.push(_)
        }
        return B
    }
};
o10l.Critical = function($) {
    this.project = $
};
o10l.Critical[O0lloO] = {
    clearCritical: function() {
        this.Tasks = this.project[Oo0o1O]();
        this.clearCriticalTasks(this.Tasks)
    },
    createCritical: function() {
        this.nodesField = this.project.tasks.nodesField;
        this.Tasks = this.project[Oo0o1O]();
        this.StartDate = this.project[OO0Olo]();
        this.FinishDate = this.project[OO1o0l]();
        this.Calendar = this.project._Calendar;
        var C = this.Tasks,
        _ = {};
        for (var $ = 0, B = C.length; $ < B; $++) {
            var A = C[$];
            _[A.UID] = A.Critical
        }
        this.doCreateCritical();
        for ($ = 0, B = C.length; $ < B; $++) {
            A = C[$];
            if (_[A.UID] != A.Critical);
        }
    },
    doCreateCritical: function() {
        var J = this.Tasks;
        this.clearCriticalTasks(J);
        var S = this.getLastTasks(),
        L = this.getTaskChains(S);
        for (var M = 0, G = L.length; M < G; M++) {
            var A = L[M];
            for (var Q = 0, _ = A.length; Q < _; Q++) {
                var T = A[Q];
                if (T.Name == "\u786e\u5b9a\u9879\u76ee\u8303\u56f4");
                var R = T.Duration,
                H = Q + 1;
                if (0 <= H && H <= _ - 1) {
                    var K = A[H],
                    I = mini.cloneDate(K.Start),
                    O = mini.cloneDate(K.Finish);
                    if (!I || !O) continue;
                    var F = this.project[o0l1lo](T, K),
                    B = F.Type;
                    switch (B) {
                    case 0:
                        T.EarlyFinish = O;
                        T.EarlyStart = this.Calendar.getStart(T.EarlyFinish, R);
                        break;
                    case 1:
                        O.setDate(O.getDate() + 1);
                        T.EarlyStart = this.Calendar.getWorkingDate(O, true);
                        T.EarlyFinish = this.Calendar.getFinish(T.EarlyStart, R);
                        break;
                    case 2:
                        I.setDate(I.getDate() - 1);
                        T.EarlyFinish = this.Calendar.getWorkingDate(I, false);
                        T.EarlyStart = this.Calendar.getStart(T.EarlyFinish, R);
                        break;
                    case 3:
                        T.EarlyStart = I;
                        T.EarlyFinish = this.Calendar.getFinish(T.EarlyStart, R);
                        break
                    }
                } else {
                    T.EarlyStart = T.Start;
                    T.EarlyFinish = T.Finish
                }
                var $ = Q - 1;
                if (0 <= $ && $ <= _ - 1) {
                    var P = A[$],
                    N = mini.cloneDate(P.Start),
                    E = mini.cloneDate(P.Finish);
                    if (!N || !E) continue;
                    F = this.project[o0l1lo](P, T),
                    B = F.Type;
                    switch (B) {
                    case 0:
                        T.LateFinish = E;
                        T.LateStart = this.Calendar.getStart(T.LateFinish, R);
                        break;
                    case 1:
                        N.setDate(N.getDate() - 1);
                        T.LateFinish = this.Calendar.getWorkingDate(N, false);
                        T.LateStart = this.Calendar.getStart(T.LateFinish, R);
                        break;
                    case 2:
                        E.setDate(E.getDate() + 1);
                        T.LateStart = this.Calendar.getWorkingDate(E, true);
                        T.LateFinish = this.Calendar.getFinish(T.LateStart, R);
                        break;
                    case 3:
                        T.LateStart = N;
                        T.LateFinish = this.Calendar.getFinish(T.LateStart, R);
                        break
                    }
                } else {
                    T.LateStart = T.Start;
                    T.LateFinish = T.Finish
                }
            }
            for (Q = 0, _ = A.length; Q < _; Q++) {
                T = A[Q];
                if (T.Name == "\u786e\u5b9a\u9879\u76ee\u8303\u56f4");
                if (T.Critical == 1) continue;
                var C = T.EarlyStart,
                D = T.LateStart;
                if (!C || !D) continue;
                if (C[llo1l]() >= D[llo1l]()) T.Critical = 1;
                $ = Q - 1;
                if (0 > $ || $ > _ - 1) T.Critical = 1;
                if (T.Milestone != null && T.Milestone == 1) T.Critical = 1;
                if (T.Critical == null || T.Critical == 0) break
            }
        }
    },
    chains: null,
    chain: null,
    getTaskChains: function(B) {
        this.chains = [];
        this.chain = [];
        for (var $ = 0, A = B.length; $ < A; $++) {
            var _ = B[$];
            this.createTaskChain(_)
        }
        return this.chains
    },
    clearCriticalTasks: function(B) {
        for (var $ = 0, A = B.length; $ < A; $++) {
            var _ = B[$];
            delete _.EarlyStart;
            delete _.EarlyFinish;
            delete _.LateStart;
            delete _.LateFinish;
            _.Critical = 0
        }
    },
    createTaskChain: function(C) {
        if (C == null || C.Start == null || C.Finish == null) return;
        this.chain[O0olo1](C);
        var E = C.PredecessorLink;
        if (E != null && E.length > 0) {
            for (var $ = 0, B = E.length; $ < B; $++) {
                var A = E[$],
                D = this.project[O0OOoo](A.PredecessorUID);
                this.createTaskChain(D)
            }
        } else {
            var _ = this.chain.clone();
            this.chains[O0olo1](_)
        }
        this.chain.removeAt(this.chain.length - 1)
    },
    getLastTasks: function() {
        var D = [],
        E = this.Tasks,
        A = -1;
        for (var $ = 0, C = E.length; $ < C; $++) {
            var B = E[$];
            if (B == null || B.Finish == null) continue;
            var _ = B.Finish[llo1l]();
            if (_ > A) A = _
        }
        for ($ = 0, C = E.length; $ < C; $++) {
            B = E[$];
            if (B == null || B.Finish == null) continue;
            _ = B.Finish[llo1l]();
            if (_ == A) D[O0olo1](B)
        }
        return D
    }
};
mini.RadioButtonList = ol0O0l,
mini.ValidatorBase = OoO0ll,
mini.AutoComplete = O00l0o,
mini.CheckBoxList = Oloo1o,
mini.DataBinding = oo1oo1,
mini.OutlookTree = o01lOl,
mini.OutlookMenu = l010OO,
mini.TextBoxList = ll01l1,
mini.TimeSpinner = ooo1ol,
mini.ListControl = o1oOOl,
mini.OutlookBar = l0l1o0,
mini.FileUpload = o00lOl,
mini.TreeSelect = o10o01,
mini.DatePicker = l1O0lO,
mini.ButtonEdit = ool10O,
mini.MenuButton = Oo0Ooo,
mini.GanttView = lolo0l,
mini.SuperTree = l0olll,
mini.SuperGrid = loo0o1,
mini.PopupEdit = Ool1Ol,
mini.Component = Oo11l0,
mini.TreeGrid = o1Oll0,
mini.DataGrid = OOo1oO,
mini.MenuItem = Ool101,
mini.Splitter = OO1oOo,
mini.HtmlFile = ol1Oo1,
mini.Calendar = lo0Ol0,
mini.ComboBox = Oooll1,
mini.TextArea = O1lOlo,
mini.Password = O01oo0,
mini.CheckBox = lllO0o,
mini.DataSet = oo0ll1,
mini.Include = O1o00O,
mini.Spinner = o0000l,
mini.ListBox = Oolloo,
mini.TextBox = ol10lo,
mini.Control = lo0O01,
mini.Layout = Oo00ol,
mini.Window = o1OOO0,
mini.Lookup = O00llo,
mini.Button = l0O0O1,
mini.Hidden = loo0l,
mini.Gantt = Ol01OO,
mini.Pager = O1olOO,
mini.Panel = oOlO00,
mini.Popup = o11Ool,
mini.Tree = o110ol,
mini.Menu = l101oo,
mini.Tabs = o0OlOl,
mini.Fit = O0Oo0o,
mini.Box = o1OO1l;
mini.locale = "en-US";
mini.dateInfo = {
    monthsLong: ["\u4e00\u6708", "\u4e8c\u6708", "\u4e09\u6708", "\u56db\u6708", "\u4e94\u6708", "\u516d\u6708", "\u4e03\u6708", "\u516b\u6708", "\u4e5d\u6708", "\u5341\u6708", "\u5341\u4e00\u6708", "\u5341\u4e8c\u6708"],
    monthsShort: ["1\u6708", "2\u6708", "3\u6708", "4\u6708", "5\u6708", "6\u6708", "7\u6708", "8\u6708", "9\u6708", "10\u6708", "11\u6708", "12\u6708"],
    daysLong: ["\u661f\u671f\u65e5", "\u661f\u671f\u4e00", "\u661f\u671f\u4e8c", "\u661f\u671f\u4e09", "\u661f\u671f\u56db", "\u661f\u671f\u4e94", "\u661f\u671f\u516d"],
    daysShort: ["\u65e5", "\u4e00", "\u4e8c", "\u4e09", "\u56db", "\u4e94", "\u516d"],
    quarterLong: ["\u4e00\u5b63\u5ea6", "\u4e8c\u5b63\u5ea6", "\u4e09\u5b63\u5ea6", "\u56db\u5b63\u5ea6"],
    quarterShort: ["Q1", "Q2", "Q2", "Q4"],
    halfYearLong: ["\u4e0a\u534a\u5e74", "\u4e0b\u534a\u5e74"],
    patterns: {
        "d": "yyyy-M-d",
        "D": "yyyy\u5e74M\u6708d\u65e5",
        "f": "yyyy\u5e74M\u6708d\u65e5 H:mm",
        "F": "yyyy\u5e74M\u6708d\u65e5 H:mm:ss",
        "g": "yyyy-M-d H:mm",
        "G": "yyyy-M-d H:mm:ss",
        "m": "MMMd\u65e5",
        "o": "yyyy-MM-ddTHH:mm:ss.fff",
        "s": "yyyy-MM-ddTHH:mm:ss",
        "t": "H:mm",
        "T": "H:mm:ss",
        "U": "yyyy\u5e74M\u6708d\u65e5 HH:mm:ss",
        "y": "yyyy\u5e74MM\u6708"
    },
    tt: {
        "AM": "\u4e0a\u5348",
        "PM": "\u4e0b\u5348"
    },
    ten: {
        "Early": "\u4e0a\u65ec",
        "Mid": "\u4e2d\u65ec",
        "Late": "\u4e0b\u65ec"
    },
    today: "\u4eca\u5929",
    clockType: 24
};
if (lo0Ol0) mini.copyTo(lo0Ol0.prototype, {
    firstDayOfWeek: 0,
    todayText: "\u4eca\u5929",
    clearText: "\u6e05\u9664",
    okText: "\u786e\u5b9a",
    cancelText: "\u53d6\u6d88",
    daysShort: ["\u65e5", "\u4e00", "\u4e8c", "\u4e09", "\u56db", "\u4e94", "\u516d"],
    format: "yyyy\u5e74MM\u6708",
    timeFormat: "H:mm"
});
for (var id in mini) {
    var clazz = mini[id];
    if (clazz && clazz[O0lloO] && clazz[O0lloO].isControl) clazz[O0lloO][loOl] = "\u4e0d\u80fd\u4e3a\u7a7a"
}
if (mini.VTypes) mini.copyTo(mini.VTypes, {
    uniqueErrorText: "\u5b57\u6bb5\u4e0d\u80fd\u91cd\u590d",
    requiredErrorText: "\u4e0d\u80fd\u4e3a\u7a7a",
    emailErrorText: "\u8bf7\u8f93\u5165\u90ae\u4ef6\u683c\u5f0f",
    urlErrorText: "\u8bf7\u8f93\u5165URL\u683c\u5f0f",
    floatErrorText: "\u8bf7\u8f93\u5165\u6570\u5b57",
    intErrorText: "\u8bf7\u8f93\u5165\u6574\u6570",
    dateErrorText: "\u8bf7\u8f93\u5165\u65e5\u671f\u683c\u5f0f {0}",
    maxLengthErrorText: "\u4e0d\u80fd\u8d85\u8fc7 {0} \u4e2a\u5b57\u7b26",
    minLengthErrorText: "\u4e0d\u80fd\u5c11\u4e8e {0} \u4e2a\u5b57\u7b26",
    maxErrorText: "\u6570\u5b57\u4e0d\u80fd\u5927\u4e8e {0} ",
    minErrorText: "\u6570\u5b57\u4e0d\u80fd\u5c0f\u4e8e {0} ",
    rangeLengthErrorText: "\u5b57\u7b26\u957f\u5ea6\u5fc5\u987b\u5728 {0} \u5230 {1} \u4e4b\u95f4",
    rangeCharErrorText: "\u5b57\u7b26\u6570\u5fc5\u987b\u5728 {0} \u5230 {1} \u4e4b\u95f4",
    rangeErrorText: "\u6570\u5b57\u5fc5\u987b\u5728 {0} \u5230 {1} \u4e4b\u95f4"
});
if (O1olOO) mini.copyTo(O1olOO.prototype, {
    firstText: "\u9996\u9875",
    prevText: "\u4e0a\u4e00\u9875",
    nextText: "\u4e0b\u4e00\u9875",
    lastText: "\u5c3e\u9875",
    pageInfoText: "\u6bcf\u9875 {0} \u6761,\u5171 {1} \u6761"
});
if (OOo1oO) mini.copyTo(OOo1oO.prototype, {
    emptyText: "\u6ca1\u6709\u8fd4\u56de\u7684\u6570\u636e"
});
if (o00lOl) o00lOl[O0lloO].buttonText = "\u6d4f\u89c8...";
if (ol1Oo1) ol1Oo1[O0lloO].buttonText = "\u6d4f\u89c8...";
if (window.Ol01OO) {
    lolo0l.ShortWeeks = ["\u65e5", "\u4e00", "\u4e8c", "\u4e09", "\u56db", "\u4e94", "\u516d"];
    lolo0l.LongWeeks = ["\u661f\u671f\u65e5", "\u661f\u671f\u4e00", "\u661f\u671f\u4e8c", "\u661f\u671f\u4e09", "\u661f\u671f\u56db", "\u661f\u671f\u4e94", "\u661f\u671f\u516d"];
    Ol01OO.PredecessorLinkType = [{
        ID: 0,
        Name: "\u5b8c\u6210-\u5b8c\u6210(FF)",
        Short: "FF"
    },
    {
        ID: 1,
        Name: "\u5b8c\u6210-\u5f00\u59cb(FS)",
        Short: "FS"
    },
    {
        ID: 2,
        Name: "\u5f00\u59cb-\u5b8c\u6210(SF)",
        Short: "SF"
    },
    {
        ID: 3,
        Name: "\u5f00\u59cb-\u5f00\u59cb(SS)",
        Short: "SS"
    }];
    Ol01OO.ConstraintType = [{
        ID: 0,
        Name: "\u8d8a\u65e9\u8d8a\u597d"
    },
    {
        ID: 1,
        Name: "\u8d8a\u665a\u8d8a\u597d"
    },
    {
        ID: 2,
        Name: "\u5fc5\u987b\u5f00\u59cb\u4e8e"
    },
    {
        ID: 3,
        Name: "\u5fc5\u987b\u5b8c\u6210\u4e8e"
    },
    {
        ID: 4,
        Name: "\u4e0d\u5f97\u65e9\u4e8e...\u5f00\u59cb"
    },
    {
        ID: 5,
        Name: "\u4e0d\u5f97\u665a\u4e8e...\u5f00\u59cb"
    },
    {
        ID: 6,
        Name: "\u4e0d\u5f97\u65e9\u4e8e...\u5b8c\u6210"
    },
    {
        ID: 7,
        Name: "\u4e0d\u5f97\u665a\u4e8e...\u5b8c\u6210"
    }];
    mini.copyTo(Ol01OO, {
        ID_Text: "\u6807\u8bc6\u53f7",
        Name_Text: "\u4efb\u52a1\u540d\u79f0",
        PercentComplete_Text: "\u8fdb\u5ea6",
        Duration_Text: "\u5de5\u671f",
        Start_Text: "\u5f00\u59cb\u65e5\u671f",
        Finish_Text: "\u5b8c\u6210\u65e5\u671f",
        Critical_Text: "\u5173\u952e\u4efb\u52a1",
        PredecessorLink_Text: "\u524d\u7f6e\u4efb\u52a1",
        Work_Text: "\u5de5\u65f6",
        Priority_Text: "\u91cd\u8981\u7ea7\u522b",
        Weight_Text: "\u6743\u91cd",
        OutlineNumber_Text: "\u5927\u7eb2\u5b57\u6bb5",
        OutlineLevel_Text: "\u4efb\u52a1\u5c42\u7ea7",
        ActualStart_Text: "\u5b9e\u9645\u5f00\u59cb\u65e5\u671f",
        ActualFinish_Text: "\u5b9e\u9645\u5b8c\u6210\u65e5\u671f",
        WBS_Text: "WBS",
        ConstraintType_Text: "\u9650\u5236\u7c7b\u578b",
        ConstraintDate_Text: "\u9650\u5236\u65e5\u671f",
        Department_Text: "\u90e8\u95e8",
        Principal_Text: "\u8d1f\u8d23\u4eba",
        Assignments_Text: "\u8d44\u6e90\u540d\u79f0",
        Summary_Text: "\u6458\u8981\u4efb\u52a1",
        Task_Text: "\u4efb\u52a1",
        Baseline_Text: "\u6bd4\u8f83\u57fa\u51c6",
        LinkType_Text: "\u94fe\u63a5\u7c7b\u578b",
        LinkLag_Text: "\u5ef6\u9694\u65f6\u95f4",
        From_Text: "\u4ece",
        To_Text: "\u5230",
        Goto_Text: "\u8f6c\u5230\u4efb\u52a1",
        UpGrade_Text: "\u5347\u7ea7",
        DownGrade_Text: "\u964d\u7ea7",
        Add_Text: "\u65b0\u589e",
        Edit_Text: "\u7f16\u8f91",
        Remove_Text: "\u5220\u9664",
        Move_Text: "\u79fb\u52a8",
        ZoomIn_Text: "\u653e\u5927",
        ZoomOut_Text: "\u7f29\u5c0f",
        Deselect_Text: "\u53d6\u6d88\u9009\u62e9",
        Split_Text: "\u62c6\u5206\u4efb\u52a1"
    })
}