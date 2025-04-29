import { browserslistToTargets } from "lightningcss";
import browserslist from "browserslist";

export const cssTargets = browserslistToTargets(browserslist("last 2 years and > 0.1% and not dead"));
