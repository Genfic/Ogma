export const stripNullish = <TIn, TOut>(
	value: TIn | null | undefined,
	ifNotNull: (val: TIn) => TOut,
	ifNull: TOut,
): TOut => {
	return value ? ifNotNull(value) : ifNull;
};
