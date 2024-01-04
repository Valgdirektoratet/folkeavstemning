<script setup lang="ts">
import { useAuthStore, useFolkeavstemningStore, useStemmegivningStore } from '@/stores';
import { computed } from 'vue'

import Spinner from './Spinner.vue';
import { Stemmerett } from '@/api/folkeavstemning';

const folkeavstemningStore = useFolkeavstemningStore();
const stemmegivningStore = useStemmegivningStore();
const authStore = useAuthStore();

const sortedFolkeavstemninger = computed(() =>
    folkeavstemningStore.state.folkeavstemninger
        .sort(x => stemmegivningStore.state.stemmeretter[x.folkeavstemningId as string] !== Stemmerett.HAR_IKKE_STEMMERETT ? -1 : 1))
</script>

<template>
    <h2 v-if="authStore.state.isLoggedIn" class="text-2xl font-bold text-primary-500 pb-8">
        {{ $t('aktive_folkeavstemninger.overskrift', Object.values(stemmegivningStore.state.stemmeretter).filter(x => x !== Stemmerett.HAR_IKKE_STEMMERETT).length) }}
    </h2>

    <ul class="list-square md:list-outside px-4 md:px-0 text-left">
        <div v-if="Object.keys(folkeavstemningStore.state.folkeavstemninger).length !== 0">
            <li v-for="folkeavstemning in sortedFolkeavstemninger" class="marker:text-orange-500 py-2">
                <p class="font-semibold text-primary-600">{{ folkeavstemning.navn }}</p>
                <p>{{ folkeavstemning.informasjonHeader }}</p>
                <StyledLink primary class="my-4"
                    v-if="stemmegivningStore.state.stemmeretter[folkeavstemning.folkeavstemningId as string] == Stemmerett.HAR_STEMMERETT"
                    @click="folkeavstemningStore.setCurrentFolkeavstemningsValue(folkeavstemning.folkeavstemningId)"
                    :to="{ name: 'Stemmegivning', params: { folkeavstemningId: folkeavstemning.folkeavstemningId ?? '' }, hash: '#main' }">
                    <span aria-hidden="true" class="material-symbols-outlined">chevron_right</span>
                    <span class="pr-2">
                        {{ $t('button.gå_til_avstemning') }}
                    </span>
                </StyledLink>
                <div v-if="stemmegivningStore.state.stemmeretter[folkeavstemning.folkeavstemningId as string] == Stemmerett.HAR_IKKE_STEMMERETT"
                    class="flex items-center gap-2">
                    <p class="text-left text-primary-600 text-sm py-2">{{ $t('stemmerett.har_ikke_stemmerett')}}</p>
                </div>
                <div v-if="stemmegivningStore.state.stemmeretter[folkeavstemning.folkeavstemningId as string] == Stemmerett.STEMMEGIVNING_IKKE_STARTET"
                     class="flex items-center gap-2">
                    <p class="text-left text-primary-600 text-sm py-2">{{ $t('stemmerett.har_stemmerett')}}</p>
                </div>
                <div v-if="stemmegivningStore.state.stemmeretter[folkeavstemning.folkeavstemningId as string] == Stemmerett.STEMMEGIVNING_LUKKET"
                     class="flex items-center gap-2">
                    <p class="text-left text-primary-600 text-sm py-2">{{ $t('stemmerett.avstemning_lukket')}}</p>
                </div>
                <div v-if="stemmegivningStore.state.stemmeretter[folkeavstemning.folkeavstemningId as string] == Stemmerett.HAR_KRYSS_IMANNTALL"
                    class="flex items-center gap-2">
                    <span aria-hidden="true" class="material-symbols-outlined text-orange-500">check_circle</span>
                    <p class="text-left text-primary-600 text-sm">{{ $t('stemmerett.har_stemt') }}</p>
                </div>
                <StyledLink primary
                    v-if="stemmegivningStore.state.stemmeretter[folkeavstemning.folkeavstemningId as string] == Stemmerett.MANGLER_INNSENDT_STEMME"
                    class="my-4"
                    :to="{ name: 'Stemmegivning', params: { folkeavstemningId: folkeavstemning.folkeavstemningId ?? '' } }">
                    <span aria-hidden="true" class="material-symbols-outlined">chevron_right</span>
                    <span class="pr-2">
                        {{ $t('stemmerett.send_inn_stemme_på_nytt') }}
                    </span>
                </StyledLink>
            </li>
        </div>
        <div v-if="folkeavstemningStore.state.loading" class="grid place-items-center w-full h-full">
            <Spinner />
        </div>
    </ul>
</template>