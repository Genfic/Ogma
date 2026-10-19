import { dirname, join } from "node:path";

/** Absolute path to the `FrontendCode` directory, i.e. the one containing `package.json`. */
export const frontendRoot = join(dirname(Bun.main), "..");

/** Absolute path to the repository root. */
export const solutionRoot = join(frontendRoot, "..");

/** Absolute path to the web project, which owns the static assets. */
export const webProjectRoot = join(solutionRoot, "Ogma3");

/** Absolute path to the web project's `wwwroot`, the output target for all built assets. */
export const wwwroot = join(webProjectRoot, "wwwroot");
