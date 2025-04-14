import { b as i } from "./chunk-118zdqkz.js";
import { g as _, o as y, v as w } from "./chunk-r2jgxqeq.js";
import { A as u, C as $, G as v, H as p, J as g, M as V, N as b, O as k, w as n, x as f } from "./chunk-1dj32eyg.js";

var I = v("<button type=\"button\" title=\"Give it a star!\"><o-icon class=\"material-icons-outlined\"></o-icon><span class=\"count\">", !0, !1, !1),
	S = (s) => {
		b();
		let [a, c] = n(!1), [E, m] = n(0);
		u(async () => {
			let t = await y(s.storyId);
			if (t.ok) {
				let { count: e, didVote: r } = t.data;
				m(e), c(r);
			} else i.error(`Error fetching data: ${t.statusText}`);
		});
		let h = async () => {
			let e = await (a() ? _ : w)({ storyId: s.storyId }, { RequestVerificationToken: s.csrf });
			if (e.ok) {
				let { count: r, didVote: o } = e.data;
				m(r), c(o);
			} else i.error(`Error fetching data: ${e.statusText}`);
		};
		return () => (() => {
			var t = I(), e = t.firstChild, r = e.nextSibling;
			return t.$$click = h, e._$owner = $(), V(r, () => E() ?? 0), f((o) => {
				var l = `votes action-btn large ${a() ? "active" : ""}`, d = a() ? "ic:round-star" : "ic:round-star-border";
				return l !== o.e && g(t, o.e = l), d !== o.t && (e.icon = o.t = d), o;
			}, { e: void 0, t: void 0 }), t;
		})();
	};
k("o-vote", { storyId: 0, csrf: "" }, S);
p(["click"]);

//# debugId=68B3CD6C5AED2E8964756E2164756E21
