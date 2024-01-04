<script lang="ts" setup>
import { useAuthStore, useFolkeavstemningStore, useStemmegivningStore } from '@/stores';
import { computed, onBeforeMount, ref } from 'vue';
import { Stemmerett } from '@/api/folkeavstemning';
import { useRoute } from 'vue-router';
import KunneIkkeAvleggeStemmeDialog from '@/components/app/dialogs/KunneIkkeAvleggeStemmeDialog.vue';
import { Errors } from "@/helpers/avgiStemme";
import type { Result } from "@/helpers/result";
const folkeavstemningStore = useFolkeavstemningStore();
const stemmegivningStore = useStemmegivningStore();
const authStore = useAuthStore();
const route = useRoute();

const folkeavstemningId = route.params.folkeavstemningId.toString() ?? '';
const stemmerett = computed(() => stemmegivningStore.state.stemmeretter[folkeavstemningId ?? '']);
const currentFolkeavstemning = folkeavstemningStore.state.folkeavstemninger.find(x => x.folkeavstemningId == folkeavstemningId);
const valgtSvaralternativ = ref("");
const valgSvaralternativTekst = computed(() => currentFolkeavstemning?.sak?.svaralternativer ? currentFolkeavstemning.sak.svaralternativer[valgtSvaralternativ.value] : valgtSvaralternativ.value ?? valgtSvaralternativ.value)

const showAvgittStemmeDialog = ref(false);
const showBekreftStemmeDialog = ref(false);
const showKunneIkkeAvleggeStemmeDialog = ref(false);
const reasonForFailure = ref(Errors.UkjentFeil);

const isFolkeavstemningActive = computed(() => {
  // kan dette gjøres på en bedre måte? sammenligne stemmerett eller lignende?
  const currentDate = new Date();
  if (currentDate.getTime() <= new Date(currentFolkeavstemning?.lukker ?? '').getTime() && currentDate.getTime() >= new Date(currentFolkeavstemning?.åpner ?? '').getTime()) {
    return true;
  }
  return false;
});

const timeoutAvgittStemmeDialog = (result: Result<string, Errors>) => {
  if (result.kind === "success") {
    showBekreftStemmeDialog.value = false;
    showAvgittStemmeDialog.value = true;
  }
  else {
    showKunneIkkeAvleggeStemmeDialog.value = true;
    reasonForFailure.value = result.error;
  }
};

const avgiStemme = async () => {
  if (stemmerett.value === Stemmerett.MANGLER_INNSENDT_STEMME) {
    const result = await stemmegivningStore.retryAvgiStemme(folkeavstemningId)
    timeoutAvgittStemmeDialog(result);
    return;
  }
  return showBekreftStemmeDialog.value = true;
}

onBeforeMount(async () => {
  await stemmegivningStore.getAllStemmeretter();
});
</script>

<template>
  <main>
    <div class="bg-primary-100 py-2">
      <Container>
        <div class="text-left flex items-center text-sm">
          <StyledLink class="text-sm text-primary-600 flex items-center" to="/">
            <span class="underline">{{ $t('stemmegivning_view.forside') }}</span>
          </StyledLink>

          <span aria-hidden="true" class="material-symbols-outlined">chevron_right</span>
          <span aria-hidden="true" class="truncate">{{ currentFolkeavstemning?.navn ?? $t('stemmegivning_view.stemmegivning') }}</span>

        </div>
        <div class="flex flex-col gap-4 py-6">
          <h1 class="text-4xl font-bold text-primary-500" id="main">
            {{ currentFolkeavstemning?.navn }}
          </h1>
          <div v-if="stemmerett !== Stemmerett.HAR_IKKE_STEMMERETT">
            <p class="text-lg max-w-[80ch]">{{ $t('stemmegivning_view.du_har_stemmerett_i', [currentFolkeavstemning?.navn]) }}
            </p>
            <p class="max-w-[80ch] mt-2">{{ $t('stemmegivning_view.viktig_informasjon') }}</p>
            <div class="border-b-2 mt-4" />
          </div>
          <p class="text-sm max-w-[80ch]">
            {{ currentFolkeavstemning?.informasjonHeader }}
          </p>
          <p class="text-sm max-w-[80ch]" v-html="currentFolkeavstemning?.informasjonBody">

          </p>
          <p class="text-sm max-w-[80ch]" v-if="stemmerett === Stemmerett.HAR_IKKE_STEMMERETT">
            {{ $t('stemmegivning_view.ikke_stemmerett_informasjon', [currentFolkeavstemning?.navn]) }}
            {{ $t('stemmegivning_view.ikke_stemmerett_les_mer') }} <a
              class="underline hover:text-primary-500 focus:ring-2 py-1 ring-primary-500 outline-none" target="_blank"
              href="https://valg.no/folkeavstemning">{{ $t('stemmegivning_view.ikke_stemmerett_link') }}</a>
          </p>
        </div>

        <StyledLink v-if="!authStore.state.isLoggedIn" primary href="/bff/login" class="mb-4">{{
          $t("button.logg_inn") }}
        </StyledLink>
      </Container>
    </div>

    <Container class="py-10">
      <h2 class="font-bold text-lg text-primary-700 mb-6 max-w-[80ch]">
        {{ currentFolkeavstemning?.sak?.spørsmål }}
      </h2>
      <div class="flex flex-col gap-4 pb-6">
        <div v-for="(svaralternativTekst, svaralternativ) in currentFolkeavstemning?.sak?.svaralternativer"
          class="flex items-center gap-4">
          <input :id="svaralternativ" type="radio" v-if="stemmerett === Stemmerett.HAR_STEMMERETT"
            class="w-6 h-6 accent-primary-600 hover:cursor-pointer disabled:cursor-default text-primary-500 before:bg-primary-500 rounded hover:shadow-[0px_0px_0px_6px_rgba(0,0,0,031)] hover:shadow-primary-300 hover:bg-primary-300"
            v-model="valgtSvaralternativ" :value="svaralternativ" :disabled="!isFolkeavstemningActive" />
          <div class="w-6 h-6 text-center rounded-full border-2 border-gray-100 ring-1 ring-gray-300 bg-gray-300"
            v-else />

          <label :for="svaralternativ" class="hover:cursor-pointer"
            :disabled="stemmerett !== Stemmerett.HAR_STEMMERETT || !isFolkeavstemningActive">
            {{ svaralternativTekst }}
          </label>
        </div>
      </div>
      <StyledButton :loading="stemmerett === Stemmerett.AVLEGGER_STEMME" primary
        v-if="(stemmerett === Stemmerett.HAR_STEMMERETT || stemmerett === Stemmerett.AVLEGGER_STEMME || stemmerett === Stemmerett.MANGLER_INNSENDT_STEMME) && isFolkeavstemningActive"
        :disabled="valgtSvaralternativ === '' && stemmerett !== Stemmerett.MANGLER_INNSENDT_STEMME" @click="avgiStemme">
        <span v-if="stemmerett === Stemmerett.HAR_STEMMERETT">{{ $t('button.avgi_stemme') }}</span>
        <span v-if="stemmerett === Stemmerett.MANGLER_INNSENDT_STEMME">{{
          $t('button.send_inn_stemme')
        }}</span>
        <span v-if="stemmerett === Stemmerett.AVLEGGER_STEMME">{{ $t('button.avlegger_stemme') }}</span>
      </StyledButton>
      <div v-if="stemmerett === Stemmerett.HAR_KRYSS_IMANNTALL && isFolkeavstemningActive"
        class="flex gap-3 items-center text-left">
        <span aria-hidden="true" class="material-symbols-outlined text-orange-500">check_circle</span>
        <p class="text-lg">
          {{ $t('stemmerett.har_stemt') }}
        </p>
      </div>

      <div
        v-if="!isFolkeavstemningActive && (stemmerett === Stemmerett.STEMMEGIVNING_IKKE_STARTET || stemmerett === Stemmerett.STEMMEGIVNING_LUKKET)">
        <div class="mb-4 border bg-primary-100 rounded-lg flex gap-2 p-2">
          <span aria-hidden="true" class="material-symbols-outlined">info</span> 
          {{ $t('stemmegivning_view.avgi_stemme_i_mellom', [$d(new Date(currentFolkeavstemning?.åpner ?? new Date()), "dayInYear"), $d(new Date(currentFolkeavstemning?.lukker ?? new Date()), "long")]) }}
        </div>
        <StyledButton primary disabled>{{ $t('button.avgi_stemme') }}</StyledButton>
      </div>
    </Container>

    <BekreftStemmeDialog v-model="showBekreftStemmeDialog"
      :folkeavstemning-id="currentFolkeavstemning?.folkeavstemningId ?? ''" :valgt-svaralternativ="valgtSvaralternativ"
      :valg-svaralternativ-tekst="valgSvaralternativTekst" @result="timeoutAvgittStemmeDialog" />

    <KunneIkkeAvleggeStemmeDialog :folkeavstemning-id="currentFolkeavstemning?.folkeavstemningId ?? ''"
      v-model="showKunneIkkeAvleggeStemmeDialog" :reason="reasonForFailure" />

    <AvgittStemmeDialog v-model="showAvgittStemmeDialog" :valgt-svaralternativ="valgtSvaralternativ"
      :folkeavstemning="currentFolkeavstemning" />
  </main>
</template>

<style scoped>
input[type='radio'] {
  appearance: none;
  margin: 0;

  border: 0.15em solid;
  border-radius: 50%;
  display: grid;
  place-content: center;
}

input[type='radio']::before {
  content: "";
  width: 1rem;
  height: 1rem;
  border-radius: 50%;

  transform: scale(0);
}

input[type='radio']:checked::before {
  transform: scale(1);
}
</style>