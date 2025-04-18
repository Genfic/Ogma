import { tinytime } from "@angius/tinytime";

const opts = { padDays: true, padMonth: true, padHours: true };

export const iso8601 = tinytime("{YYYY}-{MM}-{DD} {H}:{mm}:{ss}", opts);
export const EU = tinytime("{DD}.{Mo}.{YYYY} {H}:{mm}", opts);
export const long = tinytime("{Do} {MMMM} {YYYY}, {H}:{mm}", opts);
