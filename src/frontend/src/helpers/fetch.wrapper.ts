export const fetchWrapper = {
	get: request("GET"),
	post: request("POST"),
	put: request("PUT"),
	delete: request("DELETE"),
	filePost: request("POST"),
};

function request(method: string) {
	return async (url: string, body?: object, timeout = 5000) => {
		const requestHeaders: HeadersInit = new Headers();
		requestHeaders.set("x-csrf", "1");

		if (body) {
			requestHeaders.set("Content-Type", "application/json");
		}

		const controller = new AbortController();
		const id = setTimeout(() => controller.abort(), timeout);

		const requestOptions: RequestInit = {
			method: method,
			body: JSON.stringify(body) ?? null,
			headers: requestHeaders,
			credentials: "same-origin",
			signal: controller.signal
		};

		const response = await fetch(url, requestOptions);

		clearTimeout(id);
		return handleResponse(response);
	};
}

// helper functions

async function handleResponse(response: Response) {
	const isJson = response.headers
		?.get("content-type")
		?.includes("application/json");
	const data = isJson ? await response.json() : null;

	// check for error response
	if (!response.ok) {
		// get error message from body or default to response status
		const error = data || data?.errors || response?.status;
		return Promise.reject(error);
	}

	return data;
}
