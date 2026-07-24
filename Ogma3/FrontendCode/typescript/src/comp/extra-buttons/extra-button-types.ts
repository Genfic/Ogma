export interface ExtraButtonContext {
	input: HTMLTextAreaElement | HTMLInputElement;
	finishEdit: (newEnd: number, newStart?: number) => void;
}
