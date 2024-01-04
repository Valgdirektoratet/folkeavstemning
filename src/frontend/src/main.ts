import "vue-material-design-icons/styles.css";
import "./main.css";

import matomo from "@/matomo/matomo";
import messages from "@intlify/unplugin-vue-i18n/messages";

import { createPinia } from "pinia";
import { createApp } from "vue";
import { createI18n } from "vue-i18n";

import App from "./App.vue";
import router from "./router";
import {
	useAuthStore,
	useFolkeavstemningStore,
	useStemmegivningStore,
} from "./stores";

import { ApiError, OpenAPI, StemmerettService } from "@/api/folkeavstemning";
import { OpenAPI as StemmemottakOpenAPI } from "@/api/stemmemottak";

const i18n = createI18n({
	legacy: false,
	globalInjection: true,
	locale: "nb",
	fallbackLocale: "nb",
	availableLocales: ["nb", "nn"],
	messages: messages,
	datetimeFormats: {
		nb: {
			short: {
				year: "2-digit",
				month: "2-digit",
				day: "2-digit",
			},
			long: {
				year: "numeric",
				month: "long",
				day: "numeric",
			},
			dayInYear: {
				month: "long",
				day: "numeric",
			},
		},
	},
});

const app = createApp(App);

app.use(i18n);
OpenAPI.HEADERS = {
	"x-csrf": "1",
	"Accept-Language": localStorage.getItem("language") ?? "nb",
};
app.use(createPinia());

const authStore = useAuthStore();
const folkeavstemningStore = useFolkeavstemningStore();
await authStore.getAuthenticatedUserAsync();
await folkeavstemningStore.loadFolkeavstemningerAsync();

if (authStore.state.isLoggedIn) {
	await useStemmegivningStore().getAllStemmeretter().catch((e: Error) => {
		
		// vi kan få en feil med sessions som gjør at bruker får status som logget inn men den faktisk ikke er det.
		if(e instanceof ApiError){
			switch (e.status) {
				case 401:
					window.location.href = authStore.state.logoutUrl ?? "/bff/logout";
					break;
				default:
					break;
			}
		}
	});
	StemmerettService.stemmerettGetUrlToStemmemottak().then((url) => {
		StemmemottakOpenAPI.BASE = url.endsWith("/") ? url.slice(0, -1) : url;
	});
}

app.use(router);

// app.use(matomo, {
// 	host: "https://analytics.valg.no",
// 	containerId: "",
// 	router,
// });

app.mount("#app");
