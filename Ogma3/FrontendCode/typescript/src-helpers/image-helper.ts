type ResizeOptions = {
	resize: boolean;
	width?: number;
	height?: number;
};

type FormatOptions =
	| {
			format: `image/png`;
			quality: undefined;
	  }
	| {
			format: `image/${string}`;
			quality?: number;
	  };

type ImageOptions = ResizeOptions & FormatOptions;

export const modifyImage = async (image: File, options: ImageOptions) => {
	const bitmap = await createImageBitmap(image);
	const { width, height } = bitmap;

	let newWidth = width;
	let newHeight = height;

	if (options.resize) {
		const w = options.width ?? options.height ?? 0;
		const h = options.height ?? options.width ?? 0;

		const ratio = Math.min(w / width, h / height, 1);

		newWidth = Math.round(width * ratio);
		newHeight = Math.round(height * ratio);
	}

	const canvas = new OffscreenCanvas(newWidth, newHeight);
	const ctx = canvas.getContext("2d");
	ctx?.drawImage(bitmap, 0, 0, newWidth, newHeight);
	bitmap.close();

	const mime = options.format ?? image.type;

	const blob = await canvas.convertToBlob({ type: mime, quality: options.quality });

	const resizedImage = new File([blob], image.name, { type: mime });
	const dataTransfer = new DataTransfer();
	dataTransfer.items.add(resizedImage);
	return dataTransfer.files;
};
