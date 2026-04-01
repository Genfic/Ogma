// ./types/import-meta.d.ts

interface ImportMetaEnv {
	/** * True when running in development mode (e.g., bun build --watch)
	 * False during production builds.
	 */
	readonly DEV: boolean;
	readonly PROD: boolean;
	readonly [key: string]: any;
}

interface ImportMeta {
	readonly env: ImportMetaEnv;
}
