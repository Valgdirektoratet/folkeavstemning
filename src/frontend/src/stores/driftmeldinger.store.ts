import { fetchWrapper } from "@/helpers";
import { defineStore } from "pinia";
import { ref } from "vue";

export enum DriftmeldingsType {
    SUCCESS = 'Success',
    INFORMATION = 'Information',
    ERROR = 'Error',
    WARNING = 'Warning',
}

type Driftsmelding = {
    type?: DriftmeldingsType;
    tittel?: string;
    informasjon?: string;
};

type Driftsmeldinger = Record<string, Driftsmelding>;

export interface IDriftsmeldingerStoreState {
	driftsmeldinger: Driftsmelding[];
}

export const useDriftsmeldingerStore = defineStore("driftsmeldinger", () => {
	const state = ref<IDriftsmeldingerStoreState>({
		driftsmeldinger: [],
	});

	const getAllDriftsmeldingerAsync = async () => {
		await fetchWrapper.get("/api/driftsmeldinger").then(
			(driftsmeldinger: Driftsmeldinger) => {
				for (const driftsmelding of Object.keys(driftsmeldinger)) {
					state.value.driftsmeldinger.push(driftsmeldinger[driftsmelding]);
				}
			},
		);
	};

	return {
		state,
		getAllDriftsmeldingerAsync,
	};
});
