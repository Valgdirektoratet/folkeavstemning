import { defineStore } from "pinia";
import { ref } from "vue";

import type { FolkeavstemningDto } from "@/api/folkeavstemning";
import { FolkeavstemningService } from "@/api/folkeavstemning/";

export interface IFolkeavstemningStoreState {
	folkeavstemninger: FolkeavstemningDto[];
	currentFolkeavstemning: FolkeavstemningDto | undefined;
	currentFolkeavstemningId: string;
	loading: boolean;
}

export const useFolkeavstemningStore = defineStore("folkeavstemning", () => {
	const state = ref<IFolkeavstemningStoreState>({
		folkeavstemninger: [],
		currentFolkeavstemning: undefined,
		currentFolkeavstemningId: "",
		loading: false,
	});

	const loadFolkeavstemningerAsync = async () => {
		state.value.loading = true;
		try {
			state.value.folkeavstemninger =
				await FolkeavstemningService.folkeavstemningGetFolkeavstemning();
		} catch {
			state.value.folkeavstemninger = [];
		}
		state.value.loading = false;
	};

	const setCurrentFolkeavstemningsValue = (
		folkeavstemningsId: string | undefined,
	) => {
		if (folkeavstemningsId) {
			state.value.currentFolkeavstemningId = folkeavstemningsId;
			state.value.currentFolkeavstemning = state.value.folkeavstemninger.find(
				(x) => x.folkeavstemningId === folkeavstemningsId,
			);
		} else {
			state.value.currentFolkeavstemning = undefined;
		}
	};

	return {
		state,
		loadFolkeavstemningerAsync,
		setCurrentFolkeavstemningsValue,
	};
});
