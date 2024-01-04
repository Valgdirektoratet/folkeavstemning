import { createRouter, createWebHistory } from "vue-router";

import { Stemmerett } from "@/api/folkeavstemning";
import { useAuthStore, useStemmegivningStore } from "@/stores";
// Import views
import LandingView from "@/views/LandingView.vue";
import StemmegivningView from "@/views/StemmegivningView.vue";
import CookieErklæringViewVue from "@/views/CookieErklæringView.vue";

const router = createRouter({
	history: createWebHistory(import.meta.env.BASE_URL),
	routes: [
		{
			path: "/",
			name: "LandingsSide",
			component: LandingView,
		},
		{
			path: "/stemmegivning/:folkeavstemningId",
			name: "Stemmegivning",
			component: StemmegivningView,
		},
		{
			path: "/erklaring-om-informasjonskapsler",
			name: "Informasjonskapsler",
			component: CookieErklæringViewVue,
		},
		{
			path: '/:pathMatch(.*)*',
			redirect: to => {
				return { path: '/' }
			},
		}
	],
});

router.beforeEach(async (to, from) => {
	const authStore = useAuthStore();
	const stemmegivningStore = useStemmegivningStore();

	if (authStore.state.isLoggedIn && to.query.sendToFolkeavstemning === "true") {
		for (const folkeavstemning of Object.keys(
			stemmegivningStore.state.stemmeretter,
		)) {
			if (
				stemmegivningStore.state.stemmeretter[folkeavstemning] !==
				Stemmerett.HAR_IKKE_STEMMERETT
			) {
				return {
					name: "Stemmegivning",
					params: {
						folkeavstemningId: folkeavstemning,
					},
				};
			}
		}
	}
});

export default router;
