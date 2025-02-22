interface Seed {
	Icons: string[];
	AdditionalIcons: string[];
}

declare module "seed.json5" {
	const seed: Seed;
	export default seed;
}
