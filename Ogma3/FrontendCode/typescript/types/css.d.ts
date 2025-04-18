declare module "*.module.css" {
	const classes: { [key: string]: string };
	export default classes;

	const code: string;
	export { code };
}

declare module "*.css" {
	const code: string;
	export default code;
}

declare module "*.module.scss" {
	const classes: { [key: string]: string };
	export default classes;
}

declare module "*.module.sass" {
	const classes: { [key: string]: string };
	export default classes;
}
