<script setup lang="ts">
import { FolkeavstemningService, Stemmerett } from '@/api/folkeavstemning';
import { useStemmegivningStore } from '@/stores';
import { DialogDescription, DialogTitle } from '@headlessui/vue';
import { useVModel } from '@vueuse/core';
import { computed } from 'vue';

const stemmegivningStore = useStemmegivningStore();
const props = defineProps<{
    modelValue: boolean;
    folkeavstemningId: string;
    valgtSvaralternativ: string;
    valgSvaralternativTekst: string;
}>();

const emit = defineEmits(["update:modelValue", "result"]);
const isOpen = useVModel(props, "modelValue", emit);
const stemmerett = computed(() => stemmegivningStore.state.stemmeretter[props.folkeavstemningId]);

async function submit() {
  const result = await stemmegivningStore.avgiStemme(props.folkeavstemningId, props.valgtSvaralternativ);
  emit("result", result)
}


</script>
<template>
    <Modal v-model:modal-is-open="isOpen" :closeable="true">
        <template #title>
            <h2 class="text-2xl font-semibold text-primary-500" id="tittel" aria-hidden="true">
                {{ $t('stemmegivning_view.bekreft_stemme_dialog.dialog_tittel') }}
            </h2>
        </template>
        <template #description>
            <p>
                {{ $t('stemmegivning_view.bekreft_stemme_dialog.dialog_beskrivelse_del_1') }}
            </p>
            <p class="font-semibold">{{ valgSvaralternativTekst }}</p>

            <p>{{ $t('stemmegivning_view.bekreft_stemme_dialog.dialog_beskrivelse_del_2') }}</p>

        </template>
        <template #buttons>
            <StyledButton :loading="stemmerett === Stemmerett.AVLEGGER_STEMME"
                :disabled="stemmerett === Stemmerett.AVLEGGER_STEMME" primary @click="submit">
                {{ $t('button.avgi_stemme') }}
            </StyledButton>
            <StyledButton outlined @click="isOpen = false">
                {{ $t('button.avbryt') }}
            </StyledButton>
        </template>
    </Modal>
</template>

