# On-demand icons

In Ogma's codebase, there's no need to manually add icons; all are automatically fetched from
[Iconify](https://iconify.design/) via their API. This holds true for both the SSR parts and the
CSR parts of Ogma.

## Client-side rendering

[Icon plugin](../../Ogma3/FrontendCode/scripts/plugins/icon-plugin.ts) works by collecting icons
during the build process requesting the ones not found in the [cache directory](../../Ogma3/FrontendCode/typescript/generated/icons)
from the Iconify API, and placing them in the cache directory. Bun's bundler picks them up and
bundles them all nice like.

### Usage

```tsx
import IconName from "icons:[collection]:[icon]";

const Comp = () => <IconName />;
```

Where `[collection]` is the icon collection (lucide, mdi, boxicons, etc.) and `[icon]` is the icon
name. Names copied from [Icones](https://icones.js.org/) work perfectly well, when prefixed with
`icon:`

The generated `IconName` component has all the same props as a regular SVG element.

### Example

```tsx
import LucideButterfly from "icons:lucide-lab:butterfly";

export const PrettyButterfly = () => {
	let color = $signal('red');

	const randomizeColor = () => {
		color = '#' + Math.floor(Math.random() * 16777215).toString(16);
	}

	return (
		<button onclick={randomizeColor}>
			<LucideButterfly
				width={24}
				height={24}
				class="butterfly"
				style={{ color }}
			/>
		</button>
	)
}
```

## Server-side rendering

Server-side support consists of four parts:

1. [IconCollector](../../Ogma3/Services/IconService/IconCollector.cs)
   * Collects all the icons requested by the IconTagHelper
2. [IconCache](../../Ogma3/Services/IconService/IconCache.cs)
	* Fetches icons from the Iconify API if they're not found in the cache
	* Caches all fetched icons in a concurrent dictionary, keyed with the icon name and
      storing the icon body, width, and height as the values
3. [IconSpritesheet](../../Ogma3/Infrastructure/TagHelpers/IconSpritesheetTagHelper.cs)
	* Fetches all icons collected in the IconCollector from the cache
    * Creates a spritesheet with `<defs>` and `<symbol>` per icon
4. [IconTagHelper](../../Ogma3/Infrastructure/TagHelpers/IconTagHelper.cs)
   * Adds the requested icon to the IconCollector
   * Produces the svg element with `<use>` element inside, referencing the icon from IconSpritesheet

### Usage

Usage is even easier than the client-side version.

```html
<icon name="[collection]:[name]"></icon>
```

Where `[collection]` is the icon collection (lucide, mdi, boxicons, etc.) and `[icon]` is the icon
name. Names copied from [Icones](https://icones.js.org/) work as-is.

The icon also takes a `size` parameter, which decides the width, height, and viewBox of the icon.

### Example

```html
<a href="/" title="Home">
	<icon name="si:home-line" size="62"></icon>
</a>
```

will produce

```html
<a href="/" title="Home">
	<svg class="icon" part="icon" width="62" height="62" viewBox="0 0 62 62">
		<use href="#icon:si:home-line"></use>
	</svg>
</a>
```

and a spritesheet

```html
<svg xmlns="http://www.w3.org/2000/svg" style="display: none;">
	<defs>
		<symbol id="icon:si:home-line" viewBox="0 0 62 62" width="62px" height="62px">
			...
		</symbol>
	</defs>
</svg>
```