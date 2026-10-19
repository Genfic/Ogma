import * as v from "valibot";

export const strippedHexColor = v.pipe(
	v.string(),
	v.hexColor(),
	v.transform((h) => h.replace("#", "")),
);

export const checkboxBool = v.pipe(
	v.picklist(["on", "off"]),
	v.transform((x) => x === "on"),
	v.boolean(),
);
