export interface Success<TSuccess> {
	kind: "success";
	value: TSuccess;
}

export interface Failure<TFailure> {
	kind: "failure";
	error: TFailure;
}

export type Result<TSuccess, TFailure> = Success<TSuccess> | Failure<TFailure>;

export function success<TSuccess>(value: TSuccess): Success<TSuccess> {
	return {
		kind: "success",
		value,
	};
}

export function failure<TFailure>(error: TFailure): Failure<TFailure> {
	return {
		kind: "failure",
		error,
	};
}
