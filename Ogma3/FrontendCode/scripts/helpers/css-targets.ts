import browserslist from "browserslist";
import { browserslistToTargets } from "lightningcss";

export const cssTargets = browserslistToTargets(browserslist("last 2 years and > 0.1% and not dead"));
