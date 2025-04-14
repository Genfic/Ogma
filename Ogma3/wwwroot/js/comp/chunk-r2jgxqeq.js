async function a(e, t, n, i, r) {
	let s = await fetch(e, { method: t, headers: { "Content-Type": "application/json", ...i }, body: n ? JSON.stringify(n) : null, ...r }),
		d = s.headers.get("content-type"), o;
	if (d?.includes("application/json")) o = await s.json(); else o = await s.text();
	return { ok: s.ok, status: s.status, statusText: s.statusText, headers: s.headers, data: o };
}

var m = async (e, t, n) => await a("/api/clubjoin", "DELETE", e, t, n);
var c = async (e, t, n) => await a("/api/ShelfStories", "DELETE", e, t, n);
var l = async (e, t, n) => await a("/api/users/block", "DELETE", e, t, n),
	I = async (e, t, n) => await a("/api/users/follow", "DELETE", e, t, n), R = async (e, t, n) => await a("/api/votes", "DELETE", e, t, n);
var C = async (e, t, n) => await a(`/api/clubs/story/${e}`, "GET", void 0, t, n),
	T = async (e, t) => await a("/api/clubs/user", "GET", void 0, e, t);
var y = async (e, t, n) => await a(`/api/folders?clubId=${e}`, "GET", void 0, t, n);
var q = async (e, t) => await a("/api/notifications/count", "GET", void 0, e, t),
	b = async (e, t) => await a("/api/quotes/random", "GET", void 0, e, t);
var f = async (e, t, n, i) => await a(`/api/ShelfStories/${e}?page=${t}`, "GET", void 0, n, i),
	A = async (e, t, n) => await a(`/api/ShelfStories/${e}/quick`, "GET", void 0, t, n);
var w = async (e, t, n) => await a(`/api/votes/${e}`, "GET", void 0, t, n);
var E = async (e, t, n) => await a("/api/clubjoin", "POST", e, t, n);
var G = async (e, t, n) => await a("/api/folders/AddStory", "POST", e, t, n);
var H = async (e, t, n) => await a("/api/reports", "POST", e, t, n);
var x = async (e, t, n) => await a("/api/ShelfStories", "POST", e, t, n);
var S = async (e, t, n) => await a("/api/users/block", "POST", e, t, n),
	h = async (e, t, n) => await a("/api/users/follow", "POST", e, t, n);
var g = async (e, t, n) => await a("/api/votes", "POST", e, t, n);
export {
	m as c,
	c as d,
	l as e,
	I as f,
	R as g,
	C as h,
	T as i,
	y as j,
	q as k,
	b as l,
	f as m,
	A as n,
	w as o,
	E as p,
	G as q,
	H as r,
	x as s,
	S as t,
	h as u,
	g as v
};

//# debugId=87A76D46381D049864756E2164756E21
