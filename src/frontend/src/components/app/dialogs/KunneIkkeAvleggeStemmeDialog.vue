<script setup lang="ts">
import { Errors } from '@/helpers/avgiStemme';
import { useVModel } from '@vueuse/core';
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
const { t } = useI18n();

const props = defineProps<{
  modelValue: boolean;
  folkeavstemningId: string;
  reason: Errors;
}>();

const text = computed(() => {
  switch (props.reason) {
      case Errors.FikkIkkeAvlagtStemme:
        // Kryss i manntall - Send vote again
        return t("stemmegivning_view.kunne_ikke_avlegge_stemme_dialog.lagret_lokalt_beskrivelse")
      case Errors.AvstemningLukket:
        return t("stemmegivning_view.kunne_ikke_avlegge_stemme_dialog.avstemning_lukket")
      case Errors.AvstemningIkkeStartet:
        return t("stemmegivning_view.kunne_ikke_avlegge_stemme_dialog.avstemning_ikke_startet")
      case Errors.KanIkkeLageStemmepakke:
        return t("stemmegivning_view.kunne_ikke_avlegge_stemme_dialog.kan_ikke_lage_stemmepakke")
      case Errors.UkjentFeil:
        return t("stemmegivning_view.kunne_ikke_avlegge_stemme_dialog.ukjent_feil_beskrivelse")
      case Errors.UgyldigStemme:
        return t("stemmegivning_view.kunne_ikke_avlegge_stemme_dialog.ugyldig_stemme")
      case Errors.HarIkkeStemmerett:
        return t("stemmegivning_view.kunne_ikke_avlegge_stemme_dialog.har_ikke_stemmerett")
      case Errors.HarAlleredeStemt:
        return t("stemmegivning_view.kunne_ikke_avlegge_stemme_dialog.har_allerede_stemt")
      default:
        return t("stemmegivning_view.kunne_ikke_avlegge_stemme_dialog.ukjent_feil_beskrivelse")
    }
});

const emit = defineEmits(["update:modelValue"]);
const isOpen = useVModel(props, "modelValue", emit);
</script>
<template>
  <Modal v-model:modal-is-open="isOpen" :closeable="false">
    <template #title>
      <div class="flex flex-col items-center gap-2">
        <span aria-hidden="true" class=" material-symbols-outlined text-orange-500 scale-150">error</span>
        <h2 class="text-2xl font-semibold text-primary-600">{{
          $t('stemmegivning_view.kunne_ikke_avlegge_stemme_dialog.dialog_tittel') }}</h2>
      </div>
    </template>
    <template #description>
      <p>
        {{ text }}
      </p>
     
    </template>
    <template #buttons>
      <StyledButton primary rounded @click="isOpen = false">{{ $t('button.lukk') }}</StyledButton>
    </template>
  </Modal>
</template>

