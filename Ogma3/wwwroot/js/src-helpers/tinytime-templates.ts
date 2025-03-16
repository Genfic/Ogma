import { tinytime } from "@angius/tinytime";

const opts = { padDays: true, padMonth: true, padHours: true };
export const iso8601 = tinytime("{YYYY}-{MM}-{DD} {H}:{mm}:{ss}", opts);

export const EU = tinytime("{DD}.{MM}.{YYYY} {H}:{mm}", opts);

export const long = tinytime("{DDo} {MMMM} {YYYY}, {H}:{mm}", opts);
