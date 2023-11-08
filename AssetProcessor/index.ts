import * as yaml from "yaml";
import glob from "tiny-glob";
import { promises as fs } from "fs";
import { Config } from "./types/config";

console.log(process.cwd());


const cfgFile = await fs.readFile('config.yml', 'utf8');
const cfg = yaml.parse(cfgFile) as Config;


const filePromises = cfg.js.in?.map((path): Promise<string[]> => glob(path));
const files = (await Promise.all(filePromises)).flat();

console.log(files);