<script setup lang="ts">
import type { FolkeavstemningDto } from '@/api/folkeavstemning';
import { useVModel } from '@vueuse/core';
const props = defineProps<{
  modelValue: boolean;
  folkeavstemning?: FolkeavstemningDto;
  valgtSvaralternativ?: string;
}>();

const emit = defineEmits(["update:modelValue"]);
const isOpen = useVModel(props, "modelValue", emit);
</script>
<template>
  <Modal v-model:modal-is-open="isOpen" :closeable="false">

    <template #title>
      <div class="flex flex-col items-center gap-2">

        <span aria-hidden="true" class=" material-symbols-outlined text-orange-500 scale-150">check_circle</span>
        <h2 class="text-2xl font-semibold text-primary-600">{{
          $t('stemmegivning_view.avgitt_stemme_dialog.dialog_tittel') }}</h2>
      </div>
    </template>
    <template #description>
      <p class="font-bold">{{ folkeavstemning?.navn }}</p>
    </template>
    <template #buttons>
      <StyledLink primary @click="isOpen = false" to="/">{{ $t('button.lukk') }}</StyledLink>
    </template>
  </Modal>
</template>

