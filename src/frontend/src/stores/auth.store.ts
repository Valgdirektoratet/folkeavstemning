import { fetchWrapper } from "@/helpers";
import { defineStore } from "pinia";
import { ref } from "vue";

type Claim = {
	type: string;
	value: string;
};
export interface IAuthStoreState {
	isLoggedIn: boolean;
	user: Claim[] | null;
	logoutUrl: string | undefined;
	expiresIn: Date;
}

export const useAuthStore = defineStore("auth", () => {
	const state = ref<IAuthStoreState>({
		isLoggedIn: false,
		user: null,
		logoutUrl: undefined,
		expiresIn: new Date(),
	});

	const getAuthenticatedUserAsync = async (slide?: boolean) => {
		await fetchWrapper
			.get(`/bff/user?slide=${slide ?? true}`)
			.then((user: Claim[]) => {
				state.value.user = user;
				state.value.logoutUrl = user.find(
					(claim) => claim.type === "bff:logout_url",
				)?.value;
				state.value.isLoggedIn = true;
				state.value.expiresIn = new Date(
					new Date().getTime() +
						parseInt(
							user.find((x) => x.type === "bff:session_expires_in")?.value ??
								"0",
						) *
							1000,
				);
			})
			.catch(() => {
				state.value.isLoggedIn = false;
				state.value.user = null;
			});
	};

	return {
		state,
		getAuthenticatedUserAsync,
	};
});
