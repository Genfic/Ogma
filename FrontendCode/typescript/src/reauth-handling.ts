/**
 * Shared handling of the machine-readable `reauthentication_required` problem details
 * response (HTTP 428) returned by account-security API endpoints.
 *
 * The response body is an RFC 9457 problem details document whose `reauthenticationUrl`
 * extension points at the reauthentication page with the pending transaction bound to it.
 * The caller must not silently replay the destructive request after the redirect — the user
 * returns to a confirmation stage and triggers the action again, at which point the endpoint
 * re-checks and consumes the grant.
 */
export interface ReauthProblemDetails {
	title?: string;
	status?: number;
	reauthenticationUrl?: string;
	transactionId?: string;
}

export const REAUTHENTICATION_REQUIRED_STATUS = 428;

export const isReauthenticationRequired = (status: number): boolean => status === REAUTHENTICATION_REQUIRED_STATUS;

export const redirectToReauthentication = <T>(problem: ReauthProblemDetails | T): void => {
	const url =
		problem && typeof problem === "object" ? (problem as ReauthProblemDetails).reauthenticationUrl : undefined;

	if (typeof url === "string" && url.startsWith("/")) {
		window.location.href = url;
		return;
	}

	// Fall back to the plain reauthentication page
	window.location.href = "/reauthenticate";
};

/**
 * Checks a fetch response for `reauthentication_required` (HTTP 428) and redirects if present.
 * Returns true when the response was a reauthentication challenge (the caller should stop).
 * Accepted as a loose-typed union member because generated clients don't know about 428 yet.
 */
export const handleReauthResponse = <TData>(res: { status: number; data?: TData }): boolean => {
	if (!isReauthenticationRequired(res.status)) {
		return false;
	}
	redirectToReauthentication(res.data);
	return true;
};
