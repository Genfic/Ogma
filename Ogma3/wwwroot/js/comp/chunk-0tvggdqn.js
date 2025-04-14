function Ne(e) {
	return Object.keys(e).reduce((n, r) => {
		let s = e[r];
		if (n[r] = Object.assign({}, s), ae(s.value) && !Re(s.value) && !Array.isArray(s.value)) n[r].value = Object.assign({}, s.value);
		if (Array.isArray(s.value)) n[r].value = s.value.slice(0);
		return n;
	}, {});
}

function ve(e) {
	if (!e) return {};
	return Object.keys(e).reduce((n, r) => {
		let s = e[r];
		return n[r] = !(ae(s) && ("value" in s)) ? { value: s } : s, n[r].attribute || (n[r].attribute = Ie(r)), n[r].parse = "parse" in n[r] ? n[r].parse : typeof n[r].value !== "string", n;
	}, {});
}

function Le(e) {
	return Object.keys(e).reduce((n, r) => {
		return n[r] = e[r].value, n;
	}, {});
}

function pe(e, t) {
	let n = Ne(t);
	return Object.keys(t).forEach((s) => {
		let i = n[s], l = e.getAttribute(i.attribute), o = e[s];
		if (l != null) i.value = i.parse ? fe(l) : l;
		if (o != null) i.value = Array.isArray(o) ? o.slice(0) : o;
		i.reflect && ce(e, i.attribute, i.value, !!i.parse), Object.defineProperty(e, s, {
			get() {
				return i.value;
			}, set(f) {
				let d = i.value;
				i.value = f, i.reflect && ce(this, i.attribute, i.value, !!i.parse);
				for (let a = 0, c = this.__propertyChangedCallbacks.length; a < c; a++) this.__propertyChangedCallbacks[a](s, f, d);
			}, enumerable: !0, configurable: !0
		});
	}), n;
}

function fe(e) {
	if (!e) return;
	try {
		return JSON.parse(e);
	} catch (t) {
		return e;
	}
}

function ce(e, t, n, r) {
	if (n == null || n === !1) return e.removeAttribute(t);
	let s = r ? JSON.stringify(n) : n;
	if (e.__updating[t] = !0, s === "true") s = "";
	e.setAttribute(t, s), Promise.resolve().then(() => delete e.__updating[t]);
}

function Ie(e) {
	return e.replace(/\.?([A-Z]+)/g, (t, n) => "-" + n.toLowerCase()).replace("_", "-").replace(/^-/, "");
}

function ae(e) {
	return e != null && (typeof e === "object" || typeof e === "function");
}

function Re(e) {
	return Object.prototype.toString.call(e) === "[object Function]";
}

function He(e) {
	return typeof e === "function" && e.toString().indexOf("class") === 0;
}

var D;

function je() {
	Object.defineProperty(D, "renderRoot", { value: D });
}

function _e(e, t) {
	let n = Object.keys(t);
	return class r extends e {
		static get observedAttributes() {
			return n.map((s) => t[s].attribute);
		}

		constructor() {
			super();
			this.__initialized = !1, this.__released = !1, this.__releaseCallbacks = [], this.__propertyChangedCallbacks = [], this.__updating = {}, this.props = {};
		}

		connectedCallback() {
			if (this.__initialized) return;
			this.__releaseCallbacks = [], this.__propertyChangedCallbacks = [], this.__updating = {}, this.props = pe(this, t);
			let s = Le(this.props), i = this.Component, l = D;
			try {
				if (D = this, this.__initialized = !0, He(i)) new i(s, { element: this }); else i(s, { element: this });
			} finally {
				D = l;
			}
		}

		async disconnectedCallback() {
			if (await Promise.resolve(), this.isConnected) return;
			this.__propertyChangedCallbacks.length = 0;
			let s = null;
			while (s = this.__releaseCallbacks.pop()) s(this);
			delete this.__initialized, this.__released = !0;
		}

		attributeChangedCallback(s, i, l) {
			if (!this.__initialized) return;
			if (this.__updating[s]) return;
			if (s = this.lookupProp(s), s in t) {
				if (l == null && !this[s]) return;
				this[s] = t[s].parse ? fe(l) : l;
			}
		}

		lookupProp(s) {
			if (!t) return;
			return n.find((i) => s === i || s === t[i].attribute);
		}

		get renderRoot() {
			return this.shadowRoot || this.attachShadow({ mode: "open" });
		}

		addReleaseCallback(s) {
			this.__releaseCallbacks.push(s);
		}

		addPropertyChangedCallback(s) {
			this.__propertyChangedCallbacks.push(s);
		}
	};
}

var lt = Symbol("element-context");

function de(e, t = {}, n = {}) {
	let { BaseElement: r = HTMLElement, extension: s, customElements: i = window.customElements } = n;
	return (l) => {
		if (!e) throw new Error("tag is required to register a Component");
		let o = i.get(e);
		if (o) return o.prototype.Component = l, o;
		return o = _e(r, ve(t)), o.prototype.Component = l, o.prototype.registeredTag = e, i.define(e, o, s), o;
	};
}

var b = {
	context: void 0, registry: void 0, effects: void 0, done: !1, getContextId() {
		return he(this.context.count);
	}, getNextContextId() {
		return he(this.context.count++);
	}
};

function he(e) {
	let t = String(e), n = t.length - 1;
	return b.context.id + (n ? String.fromCharCode(96 + n) : "") + t;
}

function qe(e) {
	b.context = e;
}

var Fe = !0, Ve = (e, t) => e === t, ut = Symbol("solid-proxy");
var Ue = Symbol("solid-track"), Ge = Symbol("solid-dev-component"), X = { equals: Ve }, ge = null, xe = Pe, O = 1, Y = 2,
	Be = { owned: null, cleanups: null, context: null, owner: null }, re = {}, h = null, u = null, U = null, q = null, g = null, S = null,
	x = null, J = 0, v = { afterUpdate: null, afterCreateOwner: null, afterCreateSignal: null, afterRegisterGraph: null };

function F(e, t) {
	let n = g, r = h, s = e.length === 0, i = t === void 0 ? r : t, l = s ? { owned: null, cleanups: null, context: null, owner: null } : {
		owned: null,
		cleanups: null,
		context: i ? i.context : null,
		owner: i
	}, o = s ? () => e(() => {
		throw new Error("Dispose method must be an explicit argument to createRoot function");
	}) : () => e(() => p(() => H(l)));
	v.afterCreateOwner && v.afterCreateOwner(l), h = l, g = null;
	try {
		return T(o, !0);
	} finally {
		g = n, h = r;
	}
}

function N(e, t) {
	t = t ? Object.assign({}, X, t) : X;
	let n = { value: e, observers: null, observerSlots: null, comparator: t.equals || void 0 };
	{
		if (t.name) n.name = t.name;
		if (t.internal) n.internal = !0; else if (We(n), v.afterCreateSignal) v.afterCreateSignal(n);
	}
	let r = (s) => {
		if (typeof s === "function") if (u && u.running && u.sources.has(n)) s = s(n.tValue); else s = s(n.value);
		return ke(n, s);
	};
	return [Ce.bind(n), r];
}

function we(e, t, n) {
	let r = z(e, t, !0, O, n);
	if (U && u && u.running) S.push(r); else G(r);
}

function Z(e, t, n) {
	let r = z(e, t, !1, O, n);
	if (U && u && u.running) S.push(r); else G(r);
}

function Ae(e, t, n) {
	xe = Qe;
	let r = z(e, t, !1, O, n), s = W && Oe(W);
	if (s) r.suspense = s;
	if (!n || !n.render) r.user = !0;
	x ? x.push(r) : G(r);
}

function V(e, t, n) {
	n = n ? Object.assign({}, X, n) : X;
	let r = z(e, t, !0, 0, n);
	if (r.observers = null, r.observerSlots = null, r.comparator = n.equals || void 0, U && u && u.running) r.tState = O, S.push(r); else G(r);
	return Ce.bind(r);
}

function De(e) {
	return e && typeof e === "object" && "then" in e;
}

function ct(e, t, n) {
	let r, s, i;
	if (typeof t === "function") r = e, s = t, i = n || {}; else r = !0, s = e, i = t || {};
	let l = null, o = re, f = null, d = !1, a = !1, c = "initialValue" in i, M = typeof r === "function" && V(r),
		A = new Set, [C, E] = (i.storage || N)(i.initialValue), [I, j] = N(void 0), [k, P] = N(void 0, { equals: !1 }), [$, L] = N(c ? "ready" : "unresolved");
	if (b.context) {
		if (f = b.getNextContextId(), i.ssrLoadFrom === "initial") o = i.initialValue; else if (b.load && b.has(f)) o = b.load(f);
	}

	function R(w, y, m, _) {
		if (l === w) {
			if (l = null, _ !== void 0 && (c = !0), (w === o || y === o) && i.onHydrated) queueMicrotask(() => i.onHydrated(_, { value: y }));
			if (o = re, u && w && d) u.promises.delete(w), d = !1, T(() => {
				u.running = !0, ue(y, m);
			}, !1); else ue(y, m);
		}
		return y;
	}

	function ue(w, y) {
		T(() => {
			if (y === void 0) E(() => w);
			L(y !== void 0 ? "errored" : c ? "ready" : "unresolved"), j(y);
			for (let m of A.keys()) m.decrement();
			A.clear();
		}, !1);
	}

	function te() {
		let w = W && Oe(W), y = C(), m = I();
		if (m !== void 0 && !l) throw m;
		if (g && !g.user && w) we(() => {
			if (k(), l) {
				if (w.resolved && u && d) u.promises.add(l); else if (!A.has(w)) w.increment(), A.add(w);
			}
		});
		return y;
	}

	function ne(w = !0) {
		if (w !== !1 && a) return;
		a = !1;
		let y = M ? M() : r;
		if (d = u && u.running, y == null || y === !1) {
			R(l, p(C));
			return;
		}
		if (u && l) u.promises.delete(l);
		let m = o !== re ? o : p(() => s(y, { value: C(), refetching: w }));
		if (!De(m)) return R(l, m, void 0, y), m;
		if (l = m, "value" in m) {
			if (m.status === "success") R(l, m.value, void 0, y); else R(l, void 0, se(m.value), y);
			return m;
		}
		return a = !0, queueMicrotask(() => a = !1), T(() => {
			L(c ? "refreshing" : "pending"), P();
		}, !1), m.then((_) => R(m, _, void 0, y), (_) => R(m, void 0, se(_), y));
	}

	if (Object.defineProperties(te, {
		state: { get: () => $() }, error: { get: () => I() }, loading: {
			get() {
				let w = $();
				return w === "pending" || w === "refreshing";
			}
		}, latest: {
			get() {
				if (!c) return te();
				let w = I();
				if (w && !l) throw w;
				return C();
			}
		}
	}), M) we(() => ne(!1)); else ne(!1);
	return [te, { refetch: ne, mutate: E }];
}

function p(e) {
	if (!q && g === null) return e();
	let t = g;
	g = null;
	try {
		if (q) return q.untrack(e);
		return e();
	} finally {
		g = t;
	}
}

function ft(e) {
	Ae(() => p(e));
}

function ie(e) {
	if (h === null) console.warn("cleanups created outside a `createRoot` or `render` will never be run"); else if (h.cleanups === null) h.cleanups = [e]; else h.cleanups.push(e);
	return e;
}

function Ye(e) {
	if (u && u.running) return e(), u.done;
	let t = g, n = h;
	return Promise.resolve().then(() => {
		g = t, h = n;
		let r;
		if (U || W) r = u || (u = {
			sources: new Set,
			effects: [],
			promises: new Set,
			disposed: new Set,
			queue: new Set,
			running: !0
		}), r.done || (r.done = new Promise((s) => r.resolve = s)), r.running = !0;
		return T(e, !1), g = h = null, r ? r.done : void 0;
	});
}

var [at, be] = N(!1);

function We(e) {
	if (h) {
		if (h.sourceMap) h.sourceMap.push(e); else h.sourceMap = [e];
		e.graph = h;
	}
	if (v.afterRegisterGraph) v.afterRegisterGraph(e);
}

function Oe(e) {
	let t;
	return h && h.context && (t = h.context[e.id]) !== void 0 ? t : e.defaultValue;
}

var W;

function Ce() {
	let e = u && u.running;
	if (this.sources && (e ? this.tState : this.state)) if ((e ? this.tState : this.state) === O) G(this); else {
		let t = S;
		S = null, T(() => Q(this), !1), S = t;
	}
	if (g) {
		let t = this.observers ? this.observers.length : 0;
		if (!g.sources) g.sources = [this], g.sourceSlots = [t]; else g.sources.push(this), g.sourceSlots.push(t);
		if (!this.observers) this.observers = [g], this.observerSlots = [g.sources.length - 1]; else this.observers.push(g), this.observerSlots.push(g.sources.length - 1);
	}
	if (e && u.sources.has(this)) return this.tValue;
	return this.value;
}

function ke(e, t, n) {
	let r = u && u.running && u.sources.has(e) ? e.tValue : e.value;
	if (!e.comparator || !e.comparator(r, t)) {
		if (u) {
			let s = u.running;
			if (s || !n && u.sources.has(e)) u.sources.add(e), e.tValue = t;
			if (!s) e.value = t;
		} else e.value = t;
		if (e.observers && e.observers.length) T(() => {
			for (let s = 0; s < e.observers.length; s += 1) {
				let i = e.observers[s], l = u && u.running;
				if (l && u.disposed.has(i)) continue;
				if (l ? !i.tState : !i.state) {
					if (i.pure) S.push(i); else x.push(i);
					if (i.observers) $e(i);
				}
				if (!l) i.state = O; else i.tState = O;
			}
			if (S.length > 1e6) {
				if (S = [], Fe) throw new Error("Potential Infinite Loop Detected.");
				throw new Error;
			}
		}, !1);
	}
	return t;
}

function G(e) {
	if (!e.fn) return;
	H(e);
	let t = J;
	if (ye(e, u && u.running && u.sources.has(e) ? e.tValue : e.value, t), u && !u.running && u.sources.has(e)) queueMicrotask(() => {
		T(() => {
			u && (u.running = !0), g = h = e, ye(e, e.tValue, t), g = h = null;
		}, !1);
	});
}

function ye(e, t, n) {
	let r, s = h, i = g;
	g = h = e;
	try {
		r = e.fn(t);
	} catch (l) {
		if (e.pure) if (u && u.running) e.tState = O, e.tOwned && e.tOwned.forEach(H), e.tOwned = void 0; else e.state = O, e.owned && e.owned.forEach(H), e.owned = null;
		return e.updatedAt = n + 1, le(l);
	} finally {
		g = i, h = s;
	}
	if (!e.updatedAt || e.updatedAt <= n) {
		if (e.updatedAt != null && "observers" in e) ke(e, r, !0); else if (u && u.running && e.pure) u.sources.add(e), e.tValue = r; else e.value = r;
		e.updatedAt = n;
	}
}

function z(e, t, n, r = O, s) {
	let i = {
		fn: e,
		state: r,
		updatedAt: null,
		owned: null,
		sources: null,
		sourceSlots: null,
		cleanups: null,
		value: t,
		owner: h,
		context: h ? h.context : null,
		pure: n
	};
	if (u && u.running) i.state = 0, i.tState = r;
	if (h === null) console.warn("computations created outside a `createRoot` or `render` will never be disposed"); else if (h !== Be) if (u && u.running && h.pure) if (!h.tOwned) h.tOwned = [i]; else h.tOwned.push(i); else if (!h.owned) h.owned = [i]; else h.owned.push(i);
	if (s && s.name) i.name = s.name;
	if (q && i.fn) {
		let [l, o] = N(void 0, { equals: !1 }), f = q.factory(i.fn, o);
		ie(() => f.dispose());
		let d = () => Ye(o).then(() => a.dispose()), a = q.factory(i.fn, d);
		i.fn = (c) => {
			return l(), u && u.running ? a.track(c) : f.track(c);
		};
	}
	return v.afterCreateOwner && v.afterCreateOwner(i), i;
}

function K(e) {
	let t = u && u.running;
	if ((t ? e.tState : e.state) === 0) return;
	if ((t ? e.tState : e.state) === Y) return Q(e);
	if (e.suspense && p(e.suspense.inFallback)) return e.suspense.effects.push(e);
	let n = [e];
	while ((e = e.owner) && (!e.updatedAt || e.updatedAt < J)) {
		if (t && u.disposed.has(e)) return;
		if (t ? e.tState : e.state) n.push(e);
	}
	for (let r = n.length - 1; r >= 0; r--) {
		if (e = n[r], t) {
			let s = e, i = n[r + 1];
			while ((s = s.owner) && s !== i) if (u.disposed.has(s)) return;
		}
		if ((t ? e.tState : e.state) === O) G(e); else if ((t ? e.tState : e.state) === Y) {
			let s = S;
			S = null, T(() => Q(e, n[0]), !1), S = s;
		}
	}
}

function T(e, t) {
	if (S) return e();
	let n = !1;
	if (!t) S = [];
	if (x) n = !0; else x = [];
	J++;
	try {
		let r = e();
		return Ke(n), r;
	} catch (r) {
		if (!n) x = null;
		S = null, le(r);
	}
}

function Ke(e) {
	if (S) {
		if (U && u && u.running) Xe(S); else Pe(S);
		S = null;
	}
	if (e) return;
	let t;
	if (u) {
		if (!u.promises.size && !u.queue.size) {
			let { sources: r, disposed: s } = u;
			x.push.apply(x, u.effects), t = u.resolve;
			for (let i of x) "tState" in i && (i.state = i.tState), delete i.tState;
			u = null, T(() => {
				for (let i of s) H(i);
				for (let i of r) {
					if (i.value = i.tValue, i.owned) for (let l = 0, o = i.owned.length; l < o; l++) H(i.owned[l]);
					if (i.tOwned) i.owned = i.tOwned;
					delete i.tValue, delete i.tOwned, i.tState = 0;
				}
				be(!1);
			}, !1);
		} else if (u.running) {
			u.running = !1, u.effects.push.apply(u.effects, x), x = null, be(!0);
			return;
		}
	}
	let n = x;
	if (x = null, n.length) T(() => xe(n), !1); else v.afterUpdate && v.afterUpdate();
	if (t) t();
}

function Pe(e) {
	for (let t = 0; t < e.length; t++) K(e[t]);
}

function Xe(e) {
	for (let t = 0; t < e.length; t++) {
		let n = e[t], r = u.queue;
		if (!r.has(n)) r.add(n), U(() => {
			r.delete(n), T(() => {
				u.running = !0, K(n);
			}, !1), u && (u.running = !1);
		});
	}
}

function Qe(e) {
	let t, n = 0;
	for (t = 0; t < e.length; t++) {
		let r = e[t];
		if (!r.user) K(r); else e[n++] = r;
	}
	if (b.context) {
		if (b.count) {
			b.effects || (b.effects = []), b.effects.push(...e.slice(0, n));
			return;
		}
		qe();
	}
	if (b.effects && (b.done || !b.count)) e = [...b.effects, ...e], n += b.effects.length, delete b.effects;
	for (t = 0; t < n; t++) K(e[t]);
}

function Q(e, t) {
	let n = u && u.running;
	if (n) e.tState = 0; else e.state = 0;
	for (let r = 0; r < e.sources.length; r += 1) {
		let s = e.sources[r];
		if (s.sources) {
			let i = n ? s.tState : s.state;
			if (i === O) {
				if (s !== t && (!s.updatedAt || s.updatedAt < J)) K(s);
			} else if (i === Y) Q(s, t);
		}
	}
}

function $e(e) {
	let t = u && u.running;
	for (let n = 0; n < e.observers.length; n += 1) {
		let r = e.observers[n];
		if (t ? !r.tState : !r.state) {
			if (t) r.tState = Y; else r.state = Y;
			if (r.pure) S.push(r); else x.push(r);
			r.observers && $e(r);
		}
	}
}

function H(e) {
	let t;
	if (e.sources) while (e.sources.length) {
		let n = e.sources.pop(), r = e.sourceSlots.pop(), s = n.observers;
		if (s && s.length) {
			let i = s.pop(), l = n.observerSlots.pop();
			if (r < s.length) i.sourceSlots[l] = r, s[r] = i, n.observerSlots[r] = l;
		}
	}
	if (e.tOwned) {
		for (t = e.tOwned.length - 1; t >= 0; t--) H(e.tOwned[t]);
		delete e.tOwned;
	}
	if (u && u.running && e.pure) Te(e, !0); else if (e.owned) {
		for (t = e.owned.length - 1; t >= 0; t--) H(e.owned[t]);
		e.owned = null;
	}
	if (e.cleanups) {
		for (t = e.cleanups.length - 1; t >= 0; t--) e.cleanups[t]();
		e.cleanups = null;
	}
	if (u && u.running) e.tState = 0; else e.state = 0;
	delete e.sourceMap;
}

function Te(e, t) {
	if (!t) e.tState = 0, u.disposed.add(e);
	if (e.owned) for (let n = 0; n < e.owned.length; n++) Te(e.owned[n]);
}

function se(e) {
	if (e instanceof Error) return e;
	return new Error(typeof e === "string" ? e : "Unknown error", { cause: e });
}

function me(e, t, n) {
	try {
		for (let r of t) r(e);
	} catch (r) {
		le(r, n && n.owner || null);
	}
}

function le(e, t = h) {
	let n = ge && t && t.context && t.context[ge], r = se(e);
	if (!n) throw r;
	if (x) x.push({
		fn() {
			me(r, n, t);
		}, state: O
	}); else me(r, n, t);
}

var Je = Symbol("fallback");

function Se(e) {
	for (let t = 0; t < e.length; t++) e[t]();
}

function Ze(e, t, n = {}) {
	let r = [], s = [], i = [], l = 0, o = t.length > 1 ? [] : null;
	return ie(() => Se(i)), () => {
		let f = e() || [], d = f.length, a, c;
		return f[Ue], p(() => {
			let A, C, E, I, j, k, P, $, L;
			if (d === 0) {
				if (l !== 0) Se(i), i = [], r = [], s = [], l = 0, o && (o = []);
				if (n.fallback) r = [Je], s[0] = F((R) => {
					return i[0] = R, n.fallback();
				}), l = 1;
			} else if (l === 0) {
				s = new Array(d);
				for (c = 0; c < d; c++) r[c] = f[c], s[c] = F(M);
				l = d;
			} else {
				E = new Array(d), I = new Array(d), o && (j = new Array(d));
				for (k = 0, P = Math.min(l, d); k < P && r[k] === f[k]; k++) ;
				for (P = l - 1, $ = d - 1; P >= k && $ >= k && r[P] === f[$]; P--, $--) E[$] = s[P], I[$] = i[P], o && (j[$] = o[P]);
				A = new Map, C = new Array($ + 1);
				for (c = $; c >= k; c--) L = f[c], a = A.get(L), C[c] = a === void 0 ? -1 : a, A.set(L, c);
				for (a = k; a <= P; a++) if (L = r[a], c = A.get(L), c !== void 0 && c !== -1) E[c] = s[a], I[c] = i[a], o && (j[c] = o[a]), c = C[c], A.set(L, c); else i[a]();
				for (c = k; c < d; c++) if (c in E) {
					if (s[c] = E[c], i[c] = I[c], o) o[c] = j[c], o[c](c);
				} else s[c] = F(M);
				s = s.slice(0, l = d), r = f.slice(0);
			}
			return s;
		});

		function M(A) {
			if (i[c] = A, o) {
				let [C, E] = N(c, { name: "index" });
				return o[c] = E, t(f[c], C);
			}
			return t(f[c]);
		}
	};
}

var ze = (e) => `Attempting to access a stale value from <${e}> that could possibly be undefined. This may occur because you are reading the accessor returned from the component at a time where it has already been unmounted. We recommend cleaning up any stale timers or async, or reading from the initial condition.`;

function dt(e) {
	let t = "fallback" in e && { fallback: () => e.fallback };
	return V(Ze(() => e.each, e.children, t || void 0), void 0, { name: "value" });
}

function ht(e) {
	let t = e.keyed, n = V(() => e.when, void 0, { name: "condition value" }),
		r = t ? n : V(n, void 0, { equals: (s, i) => !s === !i, name: "condition" });
	return V(() => {
		let s = r();
		if (s) {
			let i = e.children;
			return typeof i === "function" && i.length > 0 ? p(() => i(t ? s : () => {
				if (!p(r)) throw ze("Show");
				return n();
			})) : i;
		}
		return e.fallback;
	}, void 0, { name: "value" });
}

if (globalThis) if (!globalThis.Solid$$) globalThis.Solid$$ = !0; else console.warn("You appear to have multiple instances of Solid. This can lead to unexpected behavior.");
var et = ["allowfullscreen", "async", "autofocus", "autoplay", "checked", "controls", "default", "disabled", "formnovalidate", "hidden", "indeterminate", "inert", "ismap", "loop", "multiple", "muted", "nomodule", "novalidate", "open", "playsinline", "readonly", "required", "reversed", "seamless", "selected"],
	xt = new Set(["className", "value", "readOnly", "formNoValidate", "isMap", "noModule", "playsInline", ...et]);
var At = Object.assign(Object.create(null), { className: "class", htmlFor: "for" }), Ot = Object.assign(Object.create(null), {
	class: "className",
	formnovalidate: { $: "formNoValidate", BUTTON: 1, INPUT: 1 },
	ismap: { $: "isMap", IMG: 1 },
	nomodule: { $: "noModule", SCRIPT: 1 },
	playsinline: { $: "playsInline", VIDEO: 1 },
	readonly: { $: "readOnly", INPUT: 1, TEXTAREA: 1 }
});

function tt(e, t, n) {
	let r = n.length, s = t.length, i = r, l = 0, o = 0, f = t[s - 1].nextSibling, d = null;
	while (l < s || o < i) {
		if (t[l] === n[o]) {
			l++, o++;
			continue;
		}
		while (t[s - 1] === n[i - 1]) s--, i--;
		if (s === l) {
			let a = i < r ? o ? n[o - 1].nextSibling : n[i - o] : f;
			while (o < i) e.insertBefore(n[o++], a);
		} else if (i === o) while (l < s) {
			if (!d || !d.has(t[l])) t[l].remove();
			l++;
		} else if (t[l] === n[i - 1] && n[o] === t[s - 1]) {
			let a = t[--s].nextSibling;
			e.insertBefore(n[o++], t[l++].nextSibling), e.insertBefore(n[--i], a), t[s] = n[i];
		} else {
			if (!d) {
				d = new Map;
				let c = o;
				while (c < i) d.set(n[c], c++);
			}
			let a = d.get(t[l]);
			if (a != null) if (o < a && a < i) {
				let c = l, M = 1, A;
				while (++c < s && c < i) {
					if ((A = d.get(t[c])) == null || A !== a + M) break;
					M++;
				}
				if (M > a - o) {
					let C = t[l];
					while (o < a) e.insertBefore(n[o++], C);
				} else e.replaceChild(n[o++], t[l++]);
			} else l++; else t[l++].remove();
		}
	}
}

function Ee(e, t, n, r) {
	if (n !== void 0 && !r) r = [];
	if (typeof t !== "function") return ee(e, t, r, n);
	Z((s) => ee(e, t(), s, n), r);
}

function nt(e) {
	return !!b.context && !b.done && (!e || e.isConnected);
}

function ee(e, t, n, r, s) {
	let i = nt(e);
	if (i) {
		!n && (n = [...e.childNodes]);
		let f = [];
		for (let d = 0; d < n.length; d++) {
			let a = n[d];
			if (a.nodeType === 8 && a.data.slice(0, 2) === "!$") a.remove(); else f.push(a);
		}
		n = f;
	}
	while (typeof n === "function") n = n();
	if (t === n) return n;
	let l = typeof t, o = r !== void 0;
	if (e = o && n[0] && n[0].parentNode || e, l === "string" || l === "number") {
		if (i) return n;
		if (l === "number") {
			if (t = t.toString(), t === n) return n;
		}
		if (o) {
			let f = n[0];
			if (f && f.nodeType === 3) f.data !== t && (f.data = t); else f = document.createTextNode(t);
			n = B(e, n, r, f);
		} else if (n !== "" && typeof n === "string") n = e.firstChild.data = t; else n = e.textContent = t;
	} else if (t == null || l === "boolean") {
		if (i) return n;
		n = B(e, n, r);
	} else if (l === "function") return Z(() => {
		let f = t();
		while (typeof f === "function") f = f();
		n = ee(e, f, n, r);
	}), () => n; else if (Array.isArray(t)) {
		let f = [], d = n && Array.isArray(n);
		if (oe(f, t, n, s)) return Z(() => n = ee(e, f, n, r, !0)), () => n;
		if (i) {
			if (!f.length) return n;
			if (r === void 0) return n = [...e.childNodes];
			let a = f[0];
			if (a.parentNode !== e) return n;
			let c = [a];
			while ((a = a.nextSibling) !== r) c.push(a);
			return n = c;
		}
		if (f.length === 0) {
			if (n = B(e, n, r), o) return n;
		} else if (d) if (n.length === 0) Me(e, f, r); else tt(e, n, f); else n && B(e), Me(e, f);
		n = f;
	} else if (t.nodeType) {
		if (i && t.parentNode) return n = o ? [t] : t;
		if (Array.isArray(n)) {
			if (o) return n = B(e, n, r, t);
			B(e, n, null, t);
		} else if (n == null || n === "" || !e.firstChild) e.appendChild(t); else e.replaceChild(t, e.firstChild);
		n = t;
	} else console.warn("Unrecognized value. Skipped inserting", t);
	return n;
}

function oe(e, t, n, r) {
	let s = !1;
	for (let i = 0, l = t.length; i < l; i++) {
		let o = t[i], f = n && n[e.length], d;
		if (o == null || o === !0 || o === !1) ; else if ((d = typeof o) === "object" && o.nodeType) e.push(o); else if (Array.isArray(o)) s = oe(e, o, f) || s; else if (d === "function") if (r) {
			while (typeof o === "function") o = o();
			s = oe(e, Array.isArray(o) ? o : [o], Array.isArray(f) ? f : [f]) || s;
		} else e.push(o), s = !0; else {
			let a = String(o);
			if (f && f.nodeType === 3 && f.data === a) e.push(f); else e.push(document.createTextNode(a));
		}
	}
	return s;
}

function Me(e, t, n = null) {
	for (let r = 0, s = t.length; r < s; r++) e.insertBefore(t[r], n);
}

function B(e, t, n, r) {
	if (n === void 0) return e.textContent = "";
	let s = r || document.createTextNode("");
	if (t.length) {
		let i = !1;
		for (let l = t.length - 1; l >= 0; l--) {
			let o = t[l];
			if (s !== o) {
				let f = o.parentNode === e;
				if (!i && !l) f ? e.replaceChild(s, o) : e.insertBefore(s, n); else f && o.remove();
			} else i = !0;
		}
	} else e.insertBefore(s, n);
	return [s];
}

var Ct = Symbol();

function rt(e) {
	let t = Object.keys(e), n = {};
	for (let r = 0; r < t.length; r++) {
		let [s, i] = N(e[t[r]]);
		Object.defineProperty(n, t[r], {
			get: s, set(l) {
				i(() => l);
			}
		});
	}
	return n;
}

function st(e) {
	if (e.assignedSlot && e.assignedSlot._$owner) return e.assignedSlot._$owner;
	let t = e.parentNode;
	while (t && !t._$owner && !(t.assignedSlot && t.assignedSlot._$owner)) t = t.parentNode;
	return t && t.assignedSlot ? t.assignedSlot._$owner : e._$owner;
}

function it(e) {
	return (t, n) => {
		let { element: r } = n;
		return F((s) => {
			let i = rt(t);
			r.addPropertyChangedCallback((o, f) => i[o] = f), r.addReleaseCallback(() => {
				r.renderRoot.textContent = "", s();
			});
			let l = e(i, n);
			return Ee(r.renderRoot, l);
		}, st(r));
	};
}

function Nt(e, t, n) {
	if (arguments.length === 2) n = t, t = {};
	return de(e, t)(it(n));
}

export { je as w, N as x, ct as y, ft as z, ie as A, dt as B, ht as C, Nt as D };

//# debugId=09F5FAF732F9668C64756E2164756E21
