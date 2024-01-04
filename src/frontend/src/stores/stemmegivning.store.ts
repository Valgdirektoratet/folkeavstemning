import { Stemmerett, StemmerettService } from "@/api/folkeavstemning";
import {
	Errors,
	createSignature,
	createStemmepakke,
	isStemmemottakHealthy,
	leggStemmeIUrne,
	saveStemmeLocally,
} from "@/helpers/avgiStemme";
import { computeHash } from "@/helpers/hashing";
import { failure, success } from "@/helpers/result";
import { defineStore } from "pinia";
import { ref } from "vue";
import { useAuthStore } from ".";

export interface IStemmegivningStoreState {
	stemmeretter: Record<string, Stemmerett>;
}

export const useStemmegivningStore = defineStore("stemmegivning", () => {
	const state = ref<IStemmegivningStoreState>({
		stemmeretter: {},
	});

	const authStore = useAuthStore();

	const getAllStemmeretter = async () => {
		const stemmeretter = await StemmerettService.stemmerettGetStemmerett();

		for (const folkeavstemning of Object.keys(stemmeretter)) {
			if (
				stemmeretter[folkeavstemning] === Stemmerett.HAR_KRYSS_IMANNTALL &&
				localStorage.getItem(`stemmepakke_${folkeavstemning}_${await computeHash(authStore.state.user?.find(x => x.type === "pid")?.value ?? "")}`)
			) {
				state.value.stemmeretter[folkeavstemning] =
					Stemmerett.MANGLER_INNSENDT_STEMME;
				continue;
			}
			state.value.stemmeretter[folkeavstemning] = stemmeretter[folkeavstemning];
		}

		return stemmeretter;
	};

	const retryAvgiStemme = async (folkeavstemningsId: string) => {
		if (
			state.value.stemmeretter[folkeavstemningsId] !==
			Stemmerett.MANGLER_INNSENDT_STEMME
		) {
			return failure(Errors.UkjentFeil);
		}

		const stemmepakkeKey = `stemmepakke_${folkeavstemningsId}_${await computeHash(authStore.state.user?.find(x => x.type === "pid")?.value ?? "")}`; 

		state.value.stemmeretter[folkeavstemningsId] = Stemmerett.AVLEGGER_STEMME;

		const stemmepakke = localStorage.getItem(stemmepakkeKey);

		if (stemmepakke === null) return failure(Errors.UkjentFeil);

		const parsedStemmepakke = JSON.parse(stemmepakke);

		const result = await leggStemmeIUrne(
			folkeavstemningsId,
			parsedStemmepakke.data,
			parsedStemmepakke.signature,
		);

		if (result.kind === "success") {
			localStorage.removeItem(stemmepakkeKey);
			state.value.stemmeretter[folkeavstemningsId] =
				Stemmerett.HAR_KRYSS_IMANNTALL;
		} else {
			switch (result.error) {
				case Errors.HarIkkeStemmerett:
					state.value.stemmeretter[folkeavstemningsId] =
						Stemmerett.HAR_IKKE_STEMMERETT;
					break;
				case Errors.HarAlleredeStemt:
					state.value.stemmeretter[folkeavstemningsId] =
						Stemmerett.HAR_KRYSS_IMANNTALL;
					break;
				default:
					state.value.stemmeretter[folkeavstemningsId] =
						Stemmerett.MANGLER_INNSENDT_STEMME;
					break;
			}
		}
		return result;
	};

	const avgiStemme = async (folkeavstemningsId: string, stemme: string) => {
		const stemmerett = state.value.stemmeretter[folkeavstemningsId];
		if (stemmerett !== Stemmerett.HAR_STEMMERETT)
			return failure(Errors.HarIkkeStemmerett);

		state.value.stemmeretter[folkeavstemningsId] = Stemmerett.AVLEGGER_STEMME;

		const result = await avgiStemmeIntern(folkeavstemningsId, stemme);

		if (result.kind === "failure") {
			switch (result.error) {
				case Errors.KanIkkeLageStemmepakke:
					state.value.stemmeretter[folkeavstemningsId] = stemmerett;
					break;
				case Errors.HarIkkeStemmerett:
					state.value.stemmeretter[folkeavstemningsId] =
						Stemmerett.HAR_IKKE_STEMMERETT;
					break;
				case Errors.HarAlleredeStemt:
					state.value.stemmeretter[folkeavstemningsId] =
						Stemmerett.HAR_KRYSS_IMANNTALL;
					break;
				case Errors.FikkIkkeAvlagtStemme:
					state.value.stemmeretter[folkeavstemningsId] =
						Stemmerett.MANGLER_INNSENDT_STEMME;
					break;
				case Errors.UkjentFeil:
					state.value.stemmeretter[folkeavstemningsId] = stemmerett;
					break;
			}
		} else {
			state.value.stemmeretter[folkeavstemningsId] =
				Stemmerett.HAR_KRYSS_IMANNTALL;
		}
		return result;
	};

	const avgiStemmeIntern = async (
		folkeavstemningsId: string,
		stemme: string,
	) => {
		
		const stemmepakke = await createStemmepakke(folkeavstemningsId, stemme);
		if (stemmepakke.kind === "failure") return stemmepakke;
		
		const stemmemottakCheck = await isStemmemottakHealthy();
		if (stemmemottakCheck.kind === "failure") {
			return stemmemottakCheck;
		}
		
		const signature = await createSignature(
			folkeavstemningsId,
			stemmepakke.value.encryptedStemmepakke,
			stemmepakke.value.keys,
		);
		if (signature.kind === "failure") {
			return signature;
		}

		const stemmegivningStatus = await leggStemmeIUrne(
			folkeavstemningsId,
			stemmepakke.value.encryptedStemmepakke,
			signature.value,
		);
		if (stemmegivningStatus.kind === "failure") {
			if (stemmegivningStatus.error === Errors.FikkIkkeAvlagtStemme)
				await saveStemmeLocally(
					folkeavstemningsId,
					signature.value,
					stemmepakke.value.encryptedStemmepakke,
				);
			return stemmegivningStatus;
		}

		return success("Ok");
	};

	return {
		state,
		getAllStemmeretter,
		avgiStemme,
		retryAvgiStemme,
	};
});
