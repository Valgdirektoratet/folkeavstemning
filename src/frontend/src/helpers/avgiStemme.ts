import {
	ApiError,
	KeyEndpointService,
	StemmegivningService,
	Stemmerett,
	type VoteKeys,
} from "@/api/folkeavstemning";
import {
	OpenAPI as StemmemottakOpenAPI,
	ValgUrneService,
} from "@/api/stemmemottak";
import { getNonce } from "@/helpers/randomGenerator";
import { failure, success } from "@/helpers/result";
import { useAuthStore } from "@/stores";
import { fetchWrapper } from ".";
import { blind, unblind } from "./blindSignature";
import { encrypt } from "./encryption";
import { computeHash } from "./hashing";

export enum Errors {
	KanIkkeLageStemmepakke = 0,
	HarIkkeStemmerett = 1,
	HarAlleredeStemt = 2,
	FikkIkkeAvlagtStemme = 3,
	AvstemningLukket = 4,
	AvstemningIkkeStartet = 5,
	UkjentFeil = 6,
	UgyldigStemme = 7,
}

async function isStemmemottakHealthy() {
	return await fetchWrapper.get(`${StemmemottakOpenAPI.BASE}/status`)
		.then(() => success("OK"))
		.catch(() => failure(Errors.UkjentFeil));
}

async function saveStemmeLocally(
	folkeavstemningsId: string,
	signature: string,
	encryptedStemmepakke: string,
) {

	const authStore = useAuthStore();
	try {
		localStorage.setItem(
			`stemmepakke_${folkeavstemningsId}_${await computeHash(authStore.state.user?.find(x => x.type === "pid")?.value ?? "")}`,
			JSON.stringify({
				signature: signature,
				data: encryptedStemmepakke,
			}),
		);
	} catch (e) {
		console.log(e);
	}
}

async function createStemmepakke(folkeavstemningsId: string, stemme: string) {
	try {
		const nonce = getNonce();
		const stemmepakke = JSON.stringify({ valg: stemme, nonce: nonce });
		const keys =
			await KeyEndpointService.keyEndpointGetKeys(folkeavstemningsId);
		const encryptedStemmepakke = await encrypt(
			stemmepakke,
			// biome-ignore lint/style/noNonNullAssertion: <explanation>
			keys.encryptionPublicKey!,
		);
		return success({ encryptedStemmepakke, keys });
	} catch {
		return failure(Errors.KanIkkeLageStemmepakke);
	}
}

async function createSignature(
	folkeavstemningsId: string,
	encryptedStemmepakke: string,
	keys: VoteKeys,
) {
	const { blindedMessage, r } = await blind(
		encryptedStemmepakke,
		keys.n ?? "",
		keys.e ?? "",
	);
	try {
		const blindSignature = await StemmegivningService.stemmegivningAvleggStemme(
			folkeavstemningsId,
			blindedMessage,
		);
		const signature = unblind(blindSignature, r, keys.n ?? "");
		return success(signature);
	} catch (e) {
		if (e instanceof ApiError) {
			switch (e.status) {
				case 400: {
					const stemmerett: Stemmerett = e.body;
					switch (stemmerett) {
						case Stemmerett.HAR_IKKE_STEMMERETT:
							return failure(Errors.HarIkkeStemmerett);
						case Stemmerett.HAR_KRYSS_IMANNTALL:
							return failure(Errors.HarAlleredeStemt);
						case Stemmerett.STEMMEGIVNING_IKKE_STARTET:
							return failure(Errors.AvstemningIkkeStartet);
						case Stemmerett.STEMMEGIVNING_LUKKET:
							return failure(Errors.AvstemningLukket);
					}
				}
			}
		}
		return failure(Errors.UkjentFeil);
	}
}

async function leggStemmeIUrne(
	folkeavstemningsId: string,
	stemmepakke: string,
	signature: string,
) {
	try {
		await ValgUrneService.valgUrneLeggInnStemme(folkeavstemningsId, {
			signatur: signature,
			data: stemmepakke,
		});
		return success("Avlagt stemme");
	} catch (e) {
		if (e instanceof ApiError) {
			if (e.status === 400) {
				switch (e.body) {
					case "AvstemningIkkeStartet":
						return failure(Errors.AvstemningIkkeStartet);
					case "AvstemningLukket":
						return failure(Errors.AvstemningLukket);
					case "AlleredeLevertStemme":
						return failure(Errors.HarAlleredeStemt);
					case "UgyldigSignatur":
					case "UgyldigData":
						return failure(Errors.UgyldigStemme);
				}
			}
		}
		return failure(Errors.FikkIkkeAvlagtStemme);
	}
}

export {
	isStemmemottakHealthy,
	saveStemmeLocally,
	createStemmepakke,
	createSignature,
	leggStemmeIUrne,
};
