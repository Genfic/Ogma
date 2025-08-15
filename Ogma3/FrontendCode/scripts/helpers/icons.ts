interface Svg {
	svg: string;
	name: string;
}

export const fetchIcon = async (icon: string): Promise<Svg | null> => {
	const [set, name] = icon.split(":");
	const res = await fetch(`https://api.iconify.design/${set}/${name}.svg`);

	if (res.ok) {
		const svg = await res.text();
		return { svg: svg.trim(), name: icon };
	}
	return null;
};