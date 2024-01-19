<script setup lang="ts">
import { Stemmerett, type FolkeavstemningDto } from '@/api/folkeavstemning';
import { useFolkeavstemningStore } from '@/stores';

const folkeavstemningStore = useFolkeavstemningStore();
const props = defineProps<{
    stemmerett: Stemmerett,
    folkeavstemning: FolkeavstemningDto
}>();

</script>

<template>
    <StyledLink primary class="my-4" v-if="stemmerett === Stemmerett.HAR_STEMMERETT"
        @click="folkeavstemningStore.setCurrentFolkeavstemningsValue(folkeavstemning.folkeavstemningId)"
        :to="{ name: 'Stemmegivning', params: { folkeavstemningId: folkeavstemning.folkeavstemningId ?? '' }, hash: '#main' }">
        <span aria-hidden="true" class="material-symbols-outlined">chevron_right</span>
        <span class="pr-2">
            {{ $t('button.gå_til_avstemning') }}
        </span>
    </StyledLink>
    <div v-if="stemmerett === Stemmerett.HAR_IKKE_STEMMERETT" class="flex items-center gap-2">
        <p class="text-left text-primary-600 text-sm py-2">{{ $t('stemmerett.har_ikke_stemmerett') }}</p>
    </div>
    <div v-if="stemmerett === Stemmerett.STEMMEGIVNING_IKKE_STARTET" class="flex items-center gap-2">
        <p class="text-left text-primary-600 text-sm py-2">{{ $t('stemmerett.har_stemmerett') }}</p>
    </div>
    <div v-if="stemmerett === Stemmerett.STEMMEGIVNING_LUKKET" class="flex items-center gap-2">
        <p class="text-left text-primary-600 text-sm py-2">{{ $t('stemmerett.avstemning_lukket') }}</p>
    </div>
    <div v-if="stemmerett === Stemmerett.HAR_KRYSS_IMANNTALL" class="flex items-center gap-2">
        <span aria-hidden="true" class="material-symbols-outlined text-orange-500">check_circle</span>
        <p class="text-left text-primary-600 text-sm">{{ $t('stemmerett.har_stemt') }}</p>
    </div>
    <StyledLink primary v-if="stemmerett === Stemmerett.MANGLER_INNSENDT_STEMME" class="my-4"
        :to="{ name: 'Stemmegivning', params: { folkeavstemningId: folkeavstemning.folkeavstemningId ?? '' } }">
        <span aria-hidden="true" class="material-symbols-outlined">chevron_right</span>
        <span class="pr-2">
            {{ $t('stemmerett.send_inn_stemme_på_nytt') }}
        </span>
    </StyledLink>
</template>