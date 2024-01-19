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
        <template v-if="Object.keys(folkeavstemningStore.state.folkeavstemninger).length !== 0">
            <li v-for="folkeavstemning in sortedFolkeavstemninger" class="marker:text-orange-500 py-2">
                <p class="font-semibold text-primary-600">{{ folkeavstemning.navn }}</p>
                <p>{{ folkeavstemning.informasjonHeader }}</p>
                
                <StemmerettStatus 
                    :folkeavstemning="folkeavstemning" 
                    :stemmerett="stemmegivningStore.state.stemmeretter[folkeavstemning.folkeavstemningId as string]" />
 
            </li>
        </template>
    </ul>
    <Spinner v-if="folkeavstemningStore.state.loading" />
</template>